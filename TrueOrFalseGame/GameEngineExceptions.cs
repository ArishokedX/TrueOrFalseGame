namespace TrueOrFalseGame;

public class GameEngineExceptions : Exception
{
    public GameEngineExceptions(string message, Exception innerException = null)
        : base(message, innerException)
    {
    }
}