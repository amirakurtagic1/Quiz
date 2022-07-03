using System.Collections.Generic;
using System.Threading.Tasks;
using QuizService.Model;

namespace QuizService.Services.Interfaces
{
    public interface IQuizService
    {
        Task<IEnumerable<QuizResponseModel>> GetAsync();
        Task<QuizResponseModel> GetById(int id);
        Task<int> GetQuizResult(int quizId, int userId);
        Task<QuestionResponseModel> GetQuestionByIdAsync(int id);
        Task<long> CreateAsync(QuizCreateModel quizCreateModel);
        Task<long> CreateAnswerAsync(int id, int qid, AnswerCreateModel answer);
        Task<int> UpdateAsync(int id, QuizUpdateModel quizUpdateModel);
        Task<int> UpdateQuestionAsync(int id, QuestionUpdateModel questionUpdateModel);
        Task<int> UpdateAnswerAsync(int id, AnswerUpdateModel answerUpdateModel);
        Task<int> DeleteAsync(int id);
        Task DeleteQuestionAsync(int id);
        Task DeleteAnswerAsync(int id);
        Task<long> CreateQuizResponseAsync(List<TakeQuizModel> response);
        Task<long?> CreateQuestionAsync(int quizId, QuestionCreateModel questionCreateModel);
        Task<long> CreateQuestionsAsync(int quizId, List<QuestionCreateModel> questions);
        Task<long> CreateAnswersAsync(int quizId, int questionId, List<AnswerCreateModel> answers);

    }
}