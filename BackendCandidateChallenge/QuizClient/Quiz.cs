namespace QuizClient;
/// <summary>
/// Should be class.
/// </summary>
public struct Quiz
{
    public int Id;
    public string Title;
    public static Quiz NotFound => default(Quiz);
}