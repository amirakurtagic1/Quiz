using System.Collections.Generic;
using System.Threading.Tasks;
using QuizService.Model.Domain;

namespace QuizService.Repositories.Interfaces
{
    public interface IQuizRepository
    {
        Task<IEnumerable<Quiz>> GetAsync();
        Task<Quiz> GetById(int id);
        Task<IEnumerable<QuizResponse>> GetQuizResponsesAsync(int quizId, int userId);
        Task<long> GetAnswerByText(string text);
        Task<Question> GetQuestionByIdAsync(int id);
        Task<IEnumerable<Question>> GetQuestionsByQuizId(int id);
        Task<Dictionary<int, IList<Answer>>> GetAnswersByQuizId(int id);
        Task<long> CreateAsync(Quiz quiz);
        Task<long> CreateQuizResponseAsync(List<QuizResponse> responses);
        Task<long?> CreateQuestionAsync(Question question);
        Task<long> CreateQuestionsAsync(int quizId, List<Question> questions);
        Task<long> CreateAnswersAsync(int quizId, int questionId, List<Answer> answers);
        Task<long> CreateAnswerAsync(Answer answer);
        Task<int> UpdateAsync(Quiz quiz);
        Task<int> UpdateQuestionAsync(Question question);
        Task<int> UpdateAnswerAsync(Answer answer);
        Task<int> DeleteAsync(int id);
        Task DeleteQuestionAsync(int id);
        Task DeleteAnswerAsync(int id);
    }
}