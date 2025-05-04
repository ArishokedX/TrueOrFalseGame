namespace TrueOrFalseGame;

public class GameControllerExceptions : Exception
{
    public GameControllerExceptions(string message, Exception innerException = null)
        : base(message, innerException)
    {
    }
}