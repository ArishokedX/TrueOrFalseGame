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
        public event Action<string> OnQuestionAsked;
        public event Action<GameResult> OnAnswerProcessed;
        public event Action<GameResult> OnGameEnded;
        private int _maxMistakesAllowed;
        public GameController(IQuestionSource questionSource,int maxMistakesAllowed = 2)
        {
            // _engine = engine ?? throw new ArgumentNullException(nameof(engine));
            _source = questionSource ?? throw new GameControllerExceptions(nameof(questionSource),null);
            _maxMistakesAllowed = maxMistakesAllowed;
        }

        public void StartGame()
        {
            try
            {
                var questions = _source.LoadQuestions();
                if (!questions.Any())
                {
                    throw new GameControllerExceptions("Error. Questions number must exceed zero.", null);
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

                _engine = new GameEngine(questions, _source.PositiveAnswersArray,
                    _source.NegativeAnswersArray, _maxMistakesAllowed);


                AskCurrentQuestion();
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
            if (_engine.IsGameEnded) return;

            var result = _engine.ProcessAnswer(userAnswer);
            OnAnswerProcessed?.Invoke(result);

            if (!_engine.IsGameEnded)
            {
                AskCurrentQuestion(); // Ask next question
            }
            if(_engine.IsGameEnded)
            {
                NotifyGameEnd();
            }
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
