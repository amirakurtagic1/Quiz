using System.Collections.Generic;
using System.Threading.Tasks;
using QuizService.Model;
using QuizService.Model.Domain;

namespace QuizService.Services.Interfaces
{
    public interface IQuizService
    {
        Task<IEnumerable<QuizResponseModel>> GetAsync();
        Task<QuizResponseModel> GetById(int id);
        Task<long> CreateAsync(QuizCreateModel quizCreateModel);
        Task<int> CreateAnswerAsync(int id, int qid, AnswerCreateModel answer);
        Task<int> UpdateAsync(int id, QuizUpdateModel quizUpdateModel);
        Task<int> UpdateQuestionAsync(int id, QuestionUpdateModel questionUpdateModel);
        Task<int> UpdateAnswerAsync(int id, AnswerUpdateModel answerUpdateModel);
        Task<int> DeleteAsync(int id);
        Task DeleteQuestionAsync(int id);
        Task DeleteAnswerAsync(int id);
        Task<int?> CreateQuestionAsync(int quizId, QuestionCreateModel questionCreateModel);

    }
}