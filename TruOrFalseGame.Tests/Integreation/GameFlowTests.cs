using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using TrueOrFalseGame;

namespace TruOrFalseGame.Tests.Integreation
{
    public class GameFlowTests
    {
        private readonly string _tempQuestionsPath;
        private readonly char _separator;
        private readonly int _maxMistakesAllowed;

        public GameFlowTests()
        {
            _tempQuestionsPath = Path.GetTempFileName();
            _separator = ';';
            _maxMistakesAllowed = 2;
            File.WriteAllLines(_tempQuestionsPath, new[]
            {
                "Text;CorrectAnswer;Explanation",
                "Тест 1;True;Пояснение 1",
                "Тест 2;False;Пояснение 2"
            });
        }

        [Fact]
        public void Game_WithTwoQuestions_CompletesWhenAllAnswered()
        {

            IQuestionSource qs = new CsvQuestionSource(_tempQuestionsPath, new[]  { "yes", "True" },
            new[] { "no", "False" }, _separator);
            var gameController = new GameController(qs, _maxMistakesAllowed);

            GameResult questionResult = null;
            GameResult gameResult = null;
            string receivedQuestion = string.Empty;
            // Arrange
            gameController.OnAnswerProcessed += q => questionResult = q;
            gameController.OnQuestionAsked += q => receivedQuestion = q;
            gameController.OnGameEnded += q => gameResult = q;

            
           // gameController.StartGame();
            // receivedQuestion.Should().NotBeNull();
            // receivedQuestion.Should().Contain("Тест");
            //
            // gameController.SubmitAnswer("yes");
            //
            // questionResult.Should().NotBeNull();
            // questionResult.IsCorrect.Should().BeTrue();
            //
            // receivedQuestion.Should().NotBeNull();
            // receivedQuestion.Should().Contain("Тест 2");
            //
            // gameController.SubmitAnswer("no");
            //
            // questionResult.Should().NotBeNull();
            // questionResult.IsCorrect.Should().BeTrue();
            //
            // gameController.SubmitAnswer("no");
            // gameController.SubmitAnswer("no");
            //
            // gameResult.Should().NotBeNull();
            // gameResult.Should().NotBeNull();
            // gameResult.AttemptsLeft.Should().Be(_maxMistakesAllowed);
            // gameResult.IsWinner.Should().BeTrue();
            // gameResult.Score.Should().Be(2);
        }

        public void Dispose()
        {
            File.Delete(_tempQuestionsPath);
        }
    }
}
