using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueOrFalseGame;

namespace TruOrFalseGame.Tests.Unit
{
    public class GameEngineTests
    {
        [Fact]
        public void Constructor_NullQuestions_ThrowsException()
        {
            Assert.Throws<GameEngineExceptions>(() => new GameEngine(null, new List<string> { "yes" }, new List<string> { "no" }));
        }

        [Fact]
        public void Constructor_NonPositiveMaxMistakes_ThrowsException()
        {
            var questions = new List<Question> { new Question { Text = "Is the sky blue?",
                CorrectAnswer = true, Explanation = "explanation 1" } };
            Assert.Throws<GameEngineExceptions>(() => new GameEngine(questions, new List<string> { "yes" }, new List<string> { "no" }, 0));
        }

        [Fact]
        public void ProcessAnswer_CorrectAnswer_IncreasesScore()
        {
            var questions = new List<Question> { new Question { Text = "Is the sky blue?", 
                CorrectAnswer = true, Explanation = "explanation 1" } };
            var engine = new GameEngine(questions, new List<string> { "yes" }, new List<string> { "no" });

            var result = engine.ProcessAnswer("yes");

            Assert.True(result.IsCorrect);
            Assert.Equal(1, engine.Score);
        }

        [Fact]
        public void ProcessAnswer_WrongAnswer_IncreasesMistakeCount()
        {
            var questions = new List<Question> { new Question { Text = "Is the sky blue?", CorrectAnswer = true, Explanation = "explanation 1" } };
            var engine = new GameEngine(questions, new List<string> { "yes" }, 
                new List<string> { "no" },2);

            var result = engine.ProcessAnswer("no");

            Assert.False(result.IsCorrect);
            Assert.Equal(1, engine.AttemptsLeft);
        }

        [Fact]
        public void ProcessAnswer_GameEndsOnMaxMistakes()
        {
            var questions = new List<Question>
        {
            new Question { Text = "Is the sky blue?", CorrectAnswer = true, Explanation = "explanation 1" },
            new Question { Text = "Is fire cold?", CorrectAnswer = false, Explanation = "explanation 1" }
        };
            var engine = new GameEngine(questions, new List<string> { "yes" },
                new List<string> { "no" }, 2);

            engine.ProcessAnswer("no"); // First mistake
            var result = engine.ProcessAnswer("yes"); // Second mistake, game over

            Assert.True(result.IsGameOver);
            Assert.True(engine.IsGameEnded);
            Assert.True(engine.IsGameLost);
        }

        [Fact]
        public void IsValidAnswerFormat_CorrectInput_ReturnsTrue()
        {
            var engine = new GameEngine(new List<Question>(), new List<string> { "yes" }, new List<string> { "no" });

            Assert.True(engine.IsValidAnswerFormat("yes"));
            Assert.True(engine.IsValidAnswerFormat("no"));
        }

        [Fact]
        public void IsValidAnswerFormat_InvalidInput_ReturnsFalse()
        {
            var engine = new GameEngine(new List<Question>(), new List<string> { "yes" }, new List<string> { "no" });

            Assert.False(engine.IsValidAnswerFormat("maybe"));
        }
    }

}
