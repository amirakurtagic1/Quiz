using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using QuizService.Model.Domain;
using QuizService.Repositories.Interfaces;

namespace QuizService.Repositories
{
    public class QuizRepository : IQuizRepository
    {
        private readonly IDbConnection _connection;
        public QuizRepository(IDbConnection connection)
        {
            this._connection = connection;
        }

        public async Task<int> CreateAnswerAsync(Answer answer)
        {
            const string sql = "INSERT INTO Answer (Text, QuestionId) VALUES(@Text, @QuestionId); SELECT LAST_INSERT_ROWID();";
            return (int)await _connection.ExecuteScalarAsync(sql, new { Text = answer.Text, QuestionId = answer.QuestionId });
        }

        public async Task<long> CreateAsync(Quiz quiz)
        {
            var sql = $"INSERT INTO Quiz (Title) VALUES('{quiz.Title}'); SELECT LAST_INSERT_ROWID();";
            return (long)await _connection.ExecuteScalarAsync(sql);
        }

        public async Task<int?> CreateQuestionAsync(Question question)
        {
            const string sql = "INSERT INTO Question (Text, QuizId) VALUES(@Text, @QuizId); SELECT LAST_INSERT_ROWID();";
            return (int)await _connection.ExecuteScalarAsync(sql, new { Text = question.Text, QuizId = question.QuizId });
        }

        public async Task DeleteAnswerAsync(int id)
        {
            const string sql = "DELETE FROM Answer WHERE Id = @AnswerId";
            await _connection.ExecuteScalarAsync(sql, new { AnswerId = id });
        }

        public async Task<int> DeleteAsync(int id)
        {
            const string sql = "DELETE FROM Quiz WHERE Id = @Id";
            return await _connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task DeleteQuestionAsync(int id)
        {
            const string sql = "DELETE FROM Question WHERE Id = @QuestionId";
            await _connection.ExecuteScalarAsync(sql, new { QuestionId = id });
        }

        public async Task<Dictionary<int, IList<Answer>>> GetAnswersByQuizId(int id)
        {
            const string answersSql = "SELECT a.Id, a.Text, a.QuestionId FROM Answer a INNER JOIN Question q ON a.QuestionId = q.Id WHERE q.QuizId = @QuizId;";
            return (await _connection.QueryAsync<Answer>(answersSql, new { QuizId = id }))
                .Aggregate(new Dictionary<int, IList<Answer>>(), (dict, answer) =>
                {
                    if (!dict.ContainsKey(answer.QuestionId))
                        dict.Add(answer.QuestionId, new List<Answer>());
                    dict[answer.QuestionId].Add(answer);
                    return dict;
                });
        }

        public async Task<IEnumerable<Quiz>> GetAsync()
        {
            const string sql = "SELECT * FROM Quiz;";
            return await _connection.QueryAsync<Quiz>(sql);
        }

        public async Task<Quiz> GetById(int id)
        {
            const string quizSql = "SELECT * FROM Quiz WHERE Id = @Id;";
            return await _connection.QuerySingleOrDefaultAsync<Quiz>(quizSql, new { Id = id });
        }

        public async Task<IEnumerable<Question>> GetQuestionsByQuizId(int id)
        {
            const string questionsSql = "SELECT * FROM Question WHERE QuizId = @QuizId;";
            return await _connection.QueryAsync<Question>(questionsSql, new { QuizId = id });
        }

        public async Task<int> UpdateAnswerAsync(Answer answer)
        {
            const string sql = "UPDATE Answer SET Text = @Text WHERE Id = @AnswerId";
            return await _connection.ExecuteAsync(sql, new { AnswerId = answer.Id, Text = answer.Text });
        }

        public async Task<int> UpdateAsync(Quiz quiz)
        {
            const string sql = "UPDATE Quiz SET Title = @Title WHERE Id = @Id";
            return await _connection.ExecuteAsync(sql, new { Id = quiz.Id, Title = quiz.Title });
        }

        public async Task<int> UpdateQuestionAsync(Question question)
        {
            const string sql = "UPDATE Question SET Text = @Text, CorrectAnswerId = @CorrectAnswerId WHERE Id = @QuestionId";
            return await _connection.ExecuteAsync(sql, new { QuestionId = question.Id, Text = question.Text, CorrectAnswerId = question.CorrectAnswerId });
        }
    }
}