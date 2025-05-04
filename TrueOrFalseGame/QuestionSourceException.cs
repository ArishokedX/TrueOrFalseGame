namespace TrueOrFalseGame;

public class QuestionSourceException : Exception
{
    public QuestionSourceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}