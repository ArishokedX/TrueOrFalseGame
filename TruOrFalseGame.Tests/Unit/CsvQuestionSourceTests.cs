using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace TrueOrFalseGame.Tests
{
    public class CsvQuestionSourceTests
    {
        private const string TestFilePath = "test_questions.csv";
        private const string InvalidTestFilePath = "invalid_questions.csv";
        private readonly CsvQuestionSource _source;
        private CsvQuestionSource _invalid_source;

        public CsvQuestionSourceTests()
        {
            File.WriteAllText(TestFilePath,
                "Question;Answer;Explanation\n" +
                "Is Earth round?;yes;Science fact\n" +
                "Can pigs fly?;0;Gravity affects all;faf");
            File.WriteAllText(InvalidTestFilePath,
                "Question;Answer;Explanation\n" +
                "Is Earth round?;yess;Science fact\n" +
                "Can pigs fly?;123;Gravity affects all");
            _source = new CsvQuestionSource(TestFilePath,null,null);
        }


        [Fact]
        public void LoadQuestions_ValidFile_ReturnsCorrectCount()
        {
            var questions = _source.LoadQuestions().ToList();
            Assert.Equal(2, questions.Count);
            questions[0].CorrectAnswer.Should().BeTrue();
            questions[1].CorrectAnswer.Should().BeFalse();
            questions[0].Explanation.Should().Contain("fact");
           // questions[1].Text.Should().Contain("Gravity");
        }

        [Fact]
        public void ParseQuestionLine_CorrectData_ReturnsValidQuestion()
        {
            var question = _source.ParseQuestionLine("Test;1;Explanation");

            Assert.Equal("Test", question.Text);
            Assert.True(question.CorrectAnswer);
            Assert.Equal("Explanation", question.Explanation);
        }

        [Theory]
        [InlineData("")]
        [InlineData("OnlyText")]
        [InlineData("Text;True")]
        [InlineData("Text;InvalidBoolean;ex")]
        public void ParseQuestionLine_InvalidData_ThrowsException(string line)
        {
            Assert.Throws<FormatException>(() => _source.ParseQuestionLine(line));
        }

        [Fact]
        public void ParseInvalidCsv_InvalidData_ThrowsException()
        {
            Assert.Throws<FormatException>(() => new CsvQuestionSource(InvalidTestFilePath, null, null).LoadQuestions());
        }

        [Fact]
        public void IsAvailable_WhenFileExists_ReturnsTrue()
        {
            Assert.True(_source.IsAvailable());
        }
    }
}