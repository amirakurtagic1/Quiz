using System.Collections.Generic;

/// <summary>
/// This should be a Dto object.
/// Every class should be in a separate file and we should use a single Dto object for GET, POST, PUT purposes.
/// It will make easier to map Dto to entity and it will be easier to read the code.
/// This should be done for all models.
/// </summary>
namespace QuizService.Model;

public class QuizResponseModel
{
    public class AnswerItem
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class QuestionItem
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public IEnumerable<AnswerItem> Answers { get; set; }
        public int CorrectAnswerId { get; set; }
    }

    public long Id { get; set; }
    public string Title { get; set; }
    public IEnumerable<QuestionItem> Questions { get; set; }
    public IDictionary<string, string> Links { get; set; }
}