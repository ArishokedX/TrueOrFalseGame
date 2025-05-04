using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using TrueOrFalseGame;


namespace TruOrFalseGame.Tests.Unit
{
    public class QuestionParserTests
    {
        [Fact]
        public void ParseQuestion_ValidLine_ReturnsQuestionObject()
        {
            // Arrange
            var line = "Верите ли вы, что Земля плоская?;No;Научно доказано, что Земля имеет форму геоида";
            // Act
            var qe = new CsvQuestionSource("", Encoding.UTF8, ';');
            var question = qe.Parse(line);

            // Assert
            question.Should().NotBeNull();
            question.Text.Should().Be("Верите ли вы, что Земля плоская?");
            question.CorrectAnswer.Should().BeFalse();
            question.Explanation.Should().Contain("геоида");
        }

        [Theory]
        [InlineData("")]
        [InlineData("1;;False;No explanation")]
        public void ParseQuestion_InvalidLine_ThrowsException(string invalidLine)
        {
            var qe = new CsvQuestionSource("", Encoding.UTF8, ';');
            // Act & Assert
            Assert.Throws<FormatException>(() => qe.Parse(invalidLine));
        }
    }
}
