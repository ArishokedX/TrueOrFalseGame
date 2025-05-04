using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TrueOrFalseGame
{
    public class GameEngine
    {
        private readonly List<Question> _questions;
        private readonly int _maxMistakesAllowed;
        private int _currentQuestionIndex;
        private int _mistakesCount;
        private readonly IReadOnlyCollection<string> _positiveAnswersArray;
        private readonly IReadOnlyCollection<string> _negativeAnswersArray;
        public int Score { get; private set; }
        public bool IsGameEnded => _currentQuestionIndex >= _questions.Count || IsGameLost;
        public bool IsGameLost => _mistakesCount >= _maxMistakesAllowed;
        public bool IsWinner => !IsGameLost && _currentQuestionIndex == _questions.Count;
        public Question CurrentQuestion => _questions[_currentQuestionIndex];
        public int QuestionsLeft => _questions.Count - _currentQuestionIndex;
        public int AttemptsLeft => _maxMistakesAllowed - _mistakesCount;

        public GameEngine(IEnumerable<Question> questions,
            IEnumerable<string> positiveAnwsersArray , IEnumerable<string> negativeAnwsersArray, int maxMistakesAllowed = 2)
        {
            _questions = new List<Question>(questions ?? throw new GameEngineExceptions(nameof(questions)));
            _maxMistakesAllowed = maxMistakesAllowed > 0
                ? maxMistakesAllowed
                : throw new GameEngineExceptions("Max mistakes must be positive",null);
            _positiveAnswersArray = positiveAnwsersArray.ToList();
            _negativeAnswersArray = negativeAnwsersArray.ToList();
            _currentQuestionIndex = 0;
        }

        public GameResult ProcessAnswer(string userAnswer)
        {
            if (IsGameEnded)
                throw new GameEngineExceptions("Game is already over");
            var isCorrect = CheckAnswer(CurrentQuestion, userAnswer);

            if (isCorrect) Score++;
            else _mistakesCount++;
            var explanation = CurrentQuestion.Explanation;
            _currentQuestionIndex++;
            return new GameResult(
                IsCorrect: isCorrect,
                Explanation: isCorrect ? string.Empty : explanation,
                IsGameOver: IsGameEnded,
                IsWinner: IsWinner,
                Score: Score,
                QuestionsLeft: QuestionsLeft,
                AttemptsLeft: AttemptsLeft
            );
            

        }

        private bool CheckAnswer(Question question, string userAnswer)
        {
            if (string.IsNullOrWhiteSpace(userAnswer))
                return false;

            var normalizedInput = userAnswer.Trim().ToLowerInvariant();
            return question.CorrectAnswer
                ? IsPositiveAnswer(normalizedInput)
                : IsNegativeAnswer(normalizedInput);
        }

        private  bool IsPositiveAnswer(string input) =>
            _positiveAnswersArray.Contains(input);

        private  bool IsNegativeAnswer(string input) =>
            _negativeAnswersArray.Contains(input);

        public bool IsValidAnswerFormat(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            string normalized = input.Trim().ToLowerInvariant();
            return _positiveAnswersArray.Contains(normalized) ||
                   _negativeAnswersArray.Contains(normalized);
        }
    }
}
