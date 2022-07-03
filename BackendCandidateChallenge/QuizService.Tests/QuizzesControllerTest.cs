using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using QuizService.Model;
using Xunit;
using System.IO;
namespace QuizService.Tests;

public class QuizzesControllerTest
{
    const string QuizApiEndPoint = "/api/quizzes/";

    [Fact]
    public async Task PostNewQuizAddsQuiz()
    {
        var quiz = new QuizCreateModel("Test title");
        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(quiz));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}"),
                content);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);
        }
    }

    [Fact]
    public async Task AQuizExistGetReturnsQuiz()
    {
        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            const long quizId = 1;
            var response = await client.GetAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Content);
            var quiz = JsonConvert.DeserializeObject<QuizResponseModel>(await response.Content.ReadAsStringAsync());
            Assert.Equal(quizId, quiz.Id);
            Assert.Equal("My first quiz", quiz.Title);
        }
    }

    [Fact]
    public async Task AQuizDoesNotExistGetFails()
    {
        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            const long quizId = 999;
            var response = await client.GetAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}"));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    [Fact]

    public async Task AQuizDoesNotExists_WhenPostingAQuestion_ReturnsNotFound()
    {
        const string QuizApiEndPoint = "/api/quizzes/999/questions";

        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            const long quizId = 999;
            var question = new QuestionCreateModel("The answer to everything is what?");
            var content = new StringContent(JsonConvert.SerializeObject(question));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}"), content);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    /// <summary>
    /// There are separate calls for the quiz and the questions (I implemented a method that would insert all the questions at once, but I used separate calls because of the ids, 
    /// this could be implemented differently). Then we have a multi-answer call post that will post the answers to the database and update the answerId in the questions table. 
    /// The UserId is hard-coded for now, but if we want, we can create a table with user information.
    /// </summary>
    [Fact]
    public async void AQuizResponseReturnsPoints()
    {
        var quiz = new QuizCreateModel("Quiz number 3");
        var listOfQuestions = new List<QuestionCreateModel> { new QuestionCreateModel("Which number is between 3 and 5?"), new QuestionCreateModel("What number comes after 8?") };
        var listOfAnswersForQuestion1 = new List<AnswerCreateModel> { new AnswerCreateModel("4"), new AnswerCreateModel("1") };
        var listOfAnswersForQuestion2 = new List<AnswerCreateModel> { new AnswerCreateModel("9"), new AnswerCreateModel("7") };

        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(quiz));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}"), content);

            response.StatusCode.Equals(HttpStatusCode.Created);

            var quizId = int.Parse(response.Headers.Location.ToString().Split("/").Last());

            content = new StringContent(JsonConvert.SerializeObject(new QuestionCreateModel("Which number is between 3 and 5?")));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var question1Response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}/questions"), content);
            var question1Id = int.Parse(question1Response.Headers.Location.ToString().Split("/").Last());

            Assert.Equal(HttpStatusCode.Created, question1Response.StatusCode);

            content = new StringContent(JsonConvert.SerializeObject(new QuestionCreateModel("What number comes after 8?")));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var question2Response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}/questions"), content);
            var question2Id = int.Parse(question2Response.Headers.Location.ToString().Split("/").Last());

            Assert.Equal(HttpStatusCode.Created, question2Response.StatusCode);

            content = new StringContent(JsonConvert.SerializeObject(listOfAnswersForQuestion1));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var answersForQ1Response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}/questions/{question1Id}/multiple-answers"), content);

            Assert.Equal(HttpStatusCode.OK, answersForQ1Response.StatusCode);

            content = new StringContent(JsonConvert.SerializeObject(listOfAnswersForQuestion2));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var answersForQ2Response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}/questions/{question2Id}/multiple-answers"), content);
            Assert.Equal(HttpStatusCode.OK, answersForQ2Response.StatusCode);



            var quizResponses = new List<TakeQuizModel> { new TakeQuizModel(quizId, question1Id, "4", 1), new TakeQuizModel(quizId, question2Id, "7", 1) };
            content = new StringContent(JsonConvert.SerializeObject(quizResponses));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var quizResponse = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}response"), content);

            var pointsResult = await client.GetAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}/user/1/result"));
            var receiveStream = pointsResult.Content.ReadAsStreamAsync().Result;
            var points = new StreamReader(receiveStream, Encoding.UTF8);
            
            Assert.Equal(1, int.Parse(points.ReadToEnd()));
        }
    }
}