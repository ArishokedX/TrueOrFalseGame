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
        private int _maxMistakesAllowed;
        public GameController(IQuestionSource questionSource, IEnumerable<string> positiveAnswersArray, IEnumerable<string> negativeAnswersArray,int maxMistakesAllowed = 2)
        {
            // _engine = engine ?? throw new ArgumentNullException(nameof(engine));
            _source = questionSource ?? throw new GameControllerExceptions(nameof(questionSource),null);
            NegativeAnswersArray = negativeAnswersArray;
            PositiveAnswersArray = positiveAnswersArray;
            _maxMistakesAllowed = maxMistakesAllowed;
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
                if (_maxMistakesAllowed <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        paramName: nameof(_maxMistakesAllowed),
                        actualValue: _maxMistakesAllowed,
                        message: "Game difficulty must be positive. " +
                                 $"Value '{_maxMistakesAllowed}' is not valid. " +
                                 "Please specify how many mistakes are allowed before game over.");

                }
                _engine = new GameEngine(questions,_maxMistakesAllowed, PositiveAnswersArray,
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
