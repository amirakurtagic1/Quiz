using System.Collections.Generic;
using System.Threading.Tasks;
using QuizService.Model.Domain;

namespace QuizService.Repositories.Interfaces
{
    public interface IQuizRepository
    {
        Task<IEnumerable<Quiz>> GetAsync();
        Task<Quiz> GetById(int id);
        Task<IEnumerable<Question>> GetQuestionsByQuizId(int id);
        Task<Dictionary<int, IList<Answer>>> GetAnswersByQuizId(int id);
        Task <long> CreateAsync(Quiz quiz);
        Task<int?> CreateQuestionAsync(Question question);
        Task<int> CreateAnswerAsync(Answer answer);
        Task<int> UpdateAsync(Quiz quiz);
        Task<int> UpdateQuestionAsync(Question question);
        Task<int> UpdateAnswerAsync(Answer answer);
        Task<int> DeleteAsync(int id);
        Task DeleteQuestionAsync(int id);
        Task DeleteAnswerAsync(int id);
    }
}