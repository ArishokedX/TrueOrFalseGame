namespace TrueOrFalseGame;

public record GameResult(
    bool IsCorrect,
    string Explanation,
    bool IsGameOver,
    bool IsWinner,
    int Score,
    int QuestionsLeft,
    int AttemptsLeft
);