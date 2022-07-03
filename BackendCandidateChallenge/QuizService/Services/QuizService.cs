using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using QuizService.Model;
using QuizService.Model.Domain;
using QuizService.Repositories.Interfaces;
using QuizService.Services.Interfaces;

namespace QuizService.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IMapper _mapper;
        public QuizService(IQuizRepository quizRepository, IMapper mapper)
        {
            this._quizRepository = quizRepository;
            this._mapper = mapper;
        }

        public async Task<long> CreateAnswerAsync(int id, int qId, AnswerCreateModel answerCreateModel)
        {
            var answer = _mapper.Map<AnswerCreateModel, Answer>(answerCreateModel);
            answer.QuestionId = qId;
            return await _quizRepository.CreateAnswerAsync(answer);
        }

        public async Task<long> CreateAnswersAsync(int quizId, int questionId, List<AnswerCreateModel> answers)
        {
            var listOfAnswers = _mapper.Map<IEnumerable<Answer>>(answers).ToList();
            listOfAnswers.ForEach(x => x.QuestionId = questionId);
            var correctAnswer = listOfAnswers.First();
            correctAnswer.QuestionId = questionId;
            var correctAnswerId = await _quizRepository.CreateAnswerAsync(correctAnswer);
            listOfAnswers.RemoveAt(0);
            var numberOfCreatedAnswers = await _quizRepository.CreateAnswersAsync(quizId, questionId, listOfAnswers);
            numberOfCreatedAnswers += 1;

            var question = await _quizRepository.GetQuestionByIdAsync(questionId);
            question.CorrectAnswerId = Convert.ToInt32(correctAnswerId);
            var response = await _quizRepository.UpdateQuestionAsync(question);

            return numberOfCreatedAnswers;
        }

        public async Task<long> CreateAsync(QuizCreateModel quizCreateModel)
        {
            var quiz = _mapper.Map<QuizCreateModel, Quiz>(quizCreateModel);
            return await _quizRepository.CreateAsync(quiz);
        }

        public async Task<long?> CreateQuestionAsync(int quizId, QuestionCreateModel questionCreateModel)
        {
            var quiz = await _quizRepository.GetById(quizId);
            if (quiz == null) return null;
            var question = _mapper.Map<QuestionCreateModel, Question>(questionCreateModel);
            question.QuizId = quizId;
            return await _quizRepository.CreateQuestionAsync(question);
        }

        public async Task<long> CreateQuestionsAsync(int quizId, List<QuestionCreateModel> questions)
        {
            var questionEntities = this._mapper.Map<IEnumerable<Question>>(questions).ToList();
            return await this._quizRepository.CreateQuestionsAsync(quizId, questionEntities);
        }

        public async Task<long> CreateQuizResponseAsync(List<TakeQuizModel> response)
        {
            foreach (var item in response)
            {
                item.AnswerId = (int)await _quizRepository.GetAnswerByText(item.Answer);
            }
            var quizResponses = _mapper.Map<IEnumerable<QuizResponse>>(response).ToList();
            return await _quizRepository.CreateQuizResponseAsync(quizResponses);
        }

        public async Task DeleteAnswerAsync(int id)
        {
            await _quizRepository.DeleteAnswerAsync(id);
        }

        public async Task<int> DeleteAsync(int id)
        {
            return await _quizRepository.DeleteAsync(id);
        }

        public async Task DeleteQuestionAsync(int id)
        {
            await _quizRepository.DeleteQuestionAsync(id);
        }

        public async Task<IEnumerable<QuizResponseModel>> GetAsync()
        {
            var quizzes = await _quizRepository.GetAsync();
            return _mapper.Map<IEnumerable<QuizResponseModel>>(quizzes);
        }

        public async Task<QuizResponseModel> GetById(int id)
        {
            var quiz = await _quizRepository.GetById(id);
            if (quiz == null) return null;

            var questions = await _quizRepository.GetQuestionsByQuizId(id);
            var answers = await _quizRepository.GetAnswersByQuizId(id);

            return new QuizResponseModel
            {
                Id = quiz.Id,
                Title = quiz.Title,
                Questions = questions.Select(question => new QuizResponseModel.QuestionItem
                {
                    Id = question.Id,
                    Text = question.Text,
                    Answers = answers.ContainsKey(question.Id)
                        ? answers[question.Id].Select(answer => new QuizResponseModel.AnswerItem
                        {
                            Id = answer.Id,
                            Text = answer.Text
                        })
                        : new QuizResponseModel.AnswerItem[0],
                    CorrectAnswerId = question.CorrectAnswerId
                }),
                Links = new Dictionary<string, string>
            {
                {"self", $"/api/quizzes/{id}"},
                {"questions", $"/api/quizzes/{id}/questions"}
            }
            };
        }

        public async Task<QuestionResponseModel> GetQuestionByIdAsync(int id)
        {
            var question = await _quizRepository.GetQuestionByIdAsync(id);
            return _mapper.Map<Question, QuestionResponseModel>(question);

        }

        public async Task<int> GetQuizResult(int quizId, int userId)
        {
            var quiz = await GetById(quizId);
            var quizResponses = (await _quizRepository.GetQuizResponsesAsync(quizId, userId)).ToList();
            var questions = quiz.Questions.ToList();
            var points = quiz.Questions.Count(x => x.CorrectAnswerId.Equals(quizResponses.Single(y => y.QuestionId.Equals(x.Id)).AnswerId));
            return points;
        }

        public async Task<int> UpdateAnswerAsync(int id, AnswerUpdateModel answerUpdateModel)
        {
            var answer = _mapper.Map<AnswerUpdateModel, Answer>(answerUpdateModel);
            answer.Id = id;
            return await _quizRepository.UpdateAnswerAsync(answer);
        }

        public async Task<int> UpdateAsync(int id, QuizUpdateModel quizUpdateModel)
        {
            var quiz = _mapper.Map<QuizUpdateModel, Quiz>(quizUpdateModel);
            quiz.Id = id;

            return await _quizRepository.UpdateAsync(quiz);
        }

        public async Task<int> UpdateQuestionAsync(int id, QuestionUpdateModel questionUpdateModel)
        {
            var question = _mapper.Map<QuestionUpdateModel, Question>(questionUpdateModel);
            question.Id = id;
            return await _quizRepository.UpdateQuestionAsync(question);

        }
    }
}