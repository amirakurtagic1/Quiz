namespace QuizService.Model
{
    public class TakeQuizModel
    {
        public TakeQuizModel(int quizId, int questionId, string answer, int userId)
        {
            this.QuizId = quizId;
            this.QuestionId = questionId;
            this.Answer = answer;
            this.UserId = userId;
        }
        public int QuizId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public string Answer { get; set; }
        public int UserId { get; set; }
    }
}
