using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace TrueOrFalseGame.Tests
{
    public class ValidationTests
    {
        [Theory]
        [InlineData("", false)]
        [InlineData("да", true)]
        [InlineData("YES", true)] // Проверка регистра
        [InlineData("no", true)]
        [InlineData("нет", true)] // Проверка регистра
        [InlineData("true", false)]
        public void ValidateAnswer_DifferentInputs_ValidatesCorrectly(string input, bool expected)
        {
            IEnumerable<Question> questions = new List<Question>
            {
                new Question { Text = "Is the sky blue?", CorrectAnswer = true, Explanation = "explanation 1" },
                new Question { Text = "Is the sky red?", CorrectAnswer = false, Explanation = "explanation 2" },
                new Question { Text = "Grass is green?", CorrectAnswer = true, Explanation = "explanation 3" }
            };
            var engine = new GameEngine(questions,  ["yes", "да"], ["нет", "no"]);
            Assert.Equal(expected, engine.IsValidAnswerFormat(input));
        }
    }
}
