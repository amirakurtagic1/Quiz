namespace QuizService.Model.Domain
{
    public class QuizResponse
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public int UserId { get; set; }
    }
}
