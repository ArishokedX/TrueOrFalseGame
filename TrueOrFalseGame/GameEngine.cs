using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueOrFalseGame
{
    public class GameEngine
    {
        private readonly List<Question> _questions;
        private readonly int _maxMistakesAllowed;
        private int _currentQuestionIndex;
        private int _mistakesCount;
        private static IEnumerable<string> _positiveAnwsersArray;
        private static IEnumerable<string> _negativeAnwsersArray;
        public int Score { get; private set; }
        public bool IsGameEnded => _currentQuestionIndex >= _questions.Count || IsGameLost;
        public bool IsGameLost => _mistakesCount >= _maxMistakesAllowed;
        public bool IsWinner => !IsGameLost && _currentQuestionIndex == _questions.Count;
        public Question CurrentQuestion => _questions[_currentQuestionIndex];
        public int QuestionsLeft => _questions.Count - _currentQuestionIndex;
        public int AttemptsLeft => _maxMistakesAllowed - _mistakesCount;

        public GameEngine(IEnumerable<Question> questions, int maxMistakesAllowed = 2,
            IEnumerable<string> positiveAnwsersArray = null, IEnumerable<string> negativeAnwsersArray = null)
        {
            _questions = new List<Question>(questions ?? throw new ArgumentNullException(nameof(questions)));
            _maxMistakesAllowed = maxMistakesAllowed > 0
                ? maxMistakesAllowed
                : throw new ArgumentException("Max mistakes must be positive");
            _positiveAnwsersArray = positiveAnwsersArray;
            _negativeAnwsersArray = negativeAnwsersArray;
        }

        public GameResult ProcessAnswer(string userAnswer)
        {
            if (IsGameEnded)
                throw new InvalidOperationException("Game is already over");

            var isCorrect = CheckAnswer(CurrentQuestion, userAnswer);

            if (isCorrect) Score++;
            else _mistakesCount++;

            _currentQuestionIndex++;

            return new GameResult(
                IsCorrect: isCorrect,
                Explanation: isCorrect ? string.Empty : CurrentQuestion.Explanation,
                IsGameOver: IsGameEnded,
                IsWinner: IsWinner,
                Score: Score,
                QuestionsLeft: QuestionsLeft,
                AttemptsLeft: AttemptsLeft
            );
        }

        private static bool CheckAnswer(Question question, string userAnswer)
        {
            if (string.IsNullOrWhiteSpace(userAnswer))
                return false;

            var normalizedInput = userAnswer.Trim().ToLowerInvariant();
            return question.CorrectAnswer
                ? IsPositiveAnswer(normalizedInput)
                : IsNegativeAnswer(normalizedInput);
        }

        private static bool IsPositiveAnswer(string input) =>
            _positiveAnwsersArray.Contains(input);

        private static bool IsNegativeAnswer(string input) =>
            _negativeAnwsersArray.Contains(input);

        public bool ProcessAnswerValidation(string answer)
        {
            return _positiveAnwsersArray.Contains(answer) || _negativeAnwsersArray.Contains(answer);
        }
    }
}
