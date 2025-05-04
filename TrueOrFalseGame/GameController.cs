using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueOrFalseGame
{
    public class GameController
    {
        private GameEngine _engine;
        private readonly IQuestionSource _source;
        public IEnumerable<string> NegativeAnswersArray { get; private set; }
        public IEnumerable<string> PositiveAnswersArray { get; private set; }
        public event Action<string> OnQuestionAsked;
        public event Action<GameResult> OnAnswerProcessed;
        public event Action<GameResult> OnGameEnded;

        public GameController(IQuestionSource questionSource, IEnumerable<string> positiveAnswersArray, IEnumerable<string> negativeAnswersArray)
        {
            // _engine = engine ?? throw new ArgumentNullException(nameof(engine));
            _source = questionSource ?? throw new GameControllerExceptions(nameof(questionSource),null);
            NegativeAnswersArray = negativeAnswersArray;
            PositiveAnswersArray = positiveAnswersArray;
        }

        public void StartGame()
        {
            try
            {
                var questions = _source.LoadQuestions();
                if (!questions.Any())
                {
                    throw new GameControllerExceptions("Error. Questions number must exceed zero.",null);
                }

                _engine = new GameEngine(questions, _source.MaxMistakesAllowed, PositiveAnswersArray,
                    NegativeAnswersArray);

                while (!_engine.IsGameEnded)
                {
                    AskCurrentQuestion();
                }

                NotifyGameEnd();
            }
            catch (GameEngineExceptions ex)
            {
                OnGameEnded?.Invoke(new GameResult(
                    IsCorrect: false,
                    Explanation: $"Error: {ex.Message}",
                    IsGameOver: true,
                    IsWinner: false,
                    Score: _engine?.Score ?? 0,
                    QuestionsLeft: _engine?.QuestionsLeft ?? 0,
                    AttemptsLeft: _engine?.AttemptsLeft ?? 0
                ));
            }
        }

        private void AskCurrentQuestion()
        {
            OnQuestionAsked?.Invoke(_engine.CurrentQuestion.Text);
        }

        public void SubmitAnswer(string userAnswer)
        {
            var result = _engine.ProcessAnswer(userAnswer);
            OnAnswerProcessed?.Invoke(result);
        }

        private void NotifyGameEnd()
        {
            var finalResult = new GameResult(
                IsCorrect: false,
                Explanation: string.Empty,
                IsGameOver: true,
                IsWinner: _engine.IsWinner,
                Score: _engine.Score,
                QuestionsLeft: _engine.QuestionsLeft,
                AttemptsLeft: _engine.AttemptsLeft
            );

            OnGameEnded?.Invoke(finalResult);
        }

        public bool ValidateAnswer(string answer)
        {
            
            return _engine.IsValidAnswerFormat(answer.Trim().ToLowerInvariant());
        }
    }
}
