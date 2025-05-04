using Moq;
using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace TrueOrFalseGame.Tests
{
    public class GameControllerTests
    {
        private readonly Mock<IQuestionSource> _mockSource;
        private readonly GameController _controller;

        public GameControllerTests()
        {
            _mockSource = new Mock<IQuestionSource>();
            _mockSource.Setup(x => x.LoadQuestions()).Returns(new List<Question>
            {
                new Question { Text = "Q1", CorrectAnswer = true }
            });

            _controller = new GameController(
                _mockSource.Object,1);
        }

        [Fact]
        public void StartGame_WithValidQuestions_TriggersQuestionEvent()
        {
            string receivedQuestion = null;
            _controller.OnQuestionAsked += q => receivedQuestion = q;

            _controller.StartGame();
            Assert.Equal("Q1", receivedQuestion);
        }
        [Fact]
        public void StartGame_FullSimulation_GameEndsSuccessfully()
        {
            // Arrange
            var mockSource = new Mock<IQuestionSource>();
            mockSource.Setup(s => s.LoadQuestions()).Returns(new List<Question>
            {
                new Question { Text = "Is the sky red?", CorrectAnswer = false, Explanation = "explanation 1" },
                new Question { Text = "Is the sky blue?", CorrectAnswer = true, Explanation = "explanation 2" },
                new Question { Text = "Grass is green?", CorrectAnswer = true, Explanation = "explanation 3" },
                new Question { Text = "Grass is blue?", CorrectAnswer = false, Explanation = "explanation 4" },
                new Question { Text = "Sun is yellow??", CorrectAnswer = true, Explanation = "explanation 5" }
            });
            mockSource.Setup(s => s.PositiveAnswersArray).Returns(new[] { "yes", "true" });
            mockSource.Setup(s => s.NegativeAnswersArray).Returns(new[] { "no", "false" });

            var controller = new GameController(mockSource.Object, 2);

            string currentQuestion = null;
            GameResult finalResult = null;
            GameResult questionResult = null;

            controller.OnQuestionAsked += question => currentQuestion = question;
            controller.OnAnswerProcessed += q => questionResult = q;
            controller.OnGameEnded += result => finalResult = result;
            // Act all answers is correct

            controller.StartGame();

            controller.SubmitAnswer("no"); // Answer first question correctly
            questionResult.IsCorrect.Should().BeTrue();
            questionResult.Explanation.Should().BeNullOrWhiteSpace();
            questionResult.QuestionsLeft.Should().Be(4);

            controller.SubmitAnswer("yes");  // Answer second question correctly
            questionResult.IsCorrect.Should().BeTrue();
            questionResult.Explanation.Should().BeNullOrWhiteSpace();
            questionResult.QuestionsLeft.Should().Be(3);

            controller.SubmitAnswer("yes");
            questionResult.IsCorrect.Should().BeTrue();
            questionResult.QuestionsLeft.Should().Be(2);

            controller.SubmitAnswer("no");
            questionResult.IsCorrect.Should().BeTrue();
            questionResult.QuestionsLeft.Should().Be(1);
            questionResult.Explanation.Should().BeNullOrWhiteSpace();

            controller.SubmitAnswer("yes");
            questionResult.IsCorrect.Should().BeTrue();
            questionResult.QuestionsLeft.Should().Be(0);
            questionResult.Explanation.Should().BeNullOrWhiteSpace();
            // Assert
            Assert.NotNull(finalResult);
            Assert.True(finalResult.IsWinner);
            Assert.Equal(5, finalResult.Score);
            Assert.Equal(0, finalResult.QuestionsLeft);
            Assert.Equal(2, finalResult.AttemptsLeft);

            // Act 4 answers is correct. fifth anwser is wrong

            controller.StartGame();

            controller.SubmitAnswer("no"); // Answer first question correctly
            questionResult.IsCorrect.Should().BeTrue();
            questionResult.Explanation.Should().BeNullOrWhiteSpace();
            questionResult.QuestionsLeft.Should().Be(4);

            controller.SubmitAnswer("yes");  // Answer second question correctly
            questionResult.IsCorrect.Should().BeTrue();
            questionResult.Explanation.Should().BeNullOrWhiteSpace();
            questionResult.QuestionsLeft.Should().Be(3);

            controller.SubmitAnswer("yes");
            questionResult.IsCorrect.Should().BeTrue();
            questionResult.QuestionsLeft.Should().Be(2);

            controller.SubmitAnswer("no");
            questionResult.IsCorrect.Should().BeTrue();
            questionResult.QuestionsLeft.Should().Be(1);
            questionResult.Explanation.Should().BeNullOrWhiteSpace();

            controller.SubmitAnswer("no");
            questionResult.IsCorrect.Should().BeFalse();
            questionResult.QuestionsLeft.Should().Be(0);
            questionResult.Explanation.Should().NotBeNullOrWhiteSpace();
            // Assert
            Assert.NotNull(finalResult);
            Assert.True(finalResult.IsWinner);
            Assert.Equal(4, finalResult.Score);
            Assert.Equal(0, finalResult.QuestionsLeft);
            Assert.Equal(1, finalResult.AttemptsLeft);

            // Act  1 right answer, 2 wrong answers 

            controller.StartGame();

            controller.SubmitAnswer("yes"); // Answer first question correctly
            questionResult.IsCorrect.Should().BeFalse();
            questionResult.Explanation.Should().NotBeNullOrWhiteSpace();
            questionResult.QuestionsLeft.Should().Be(4);

            controller.SubmitAnswer("yes");  // Answer second question correctly
            questionResult.IsCorrect.Should().BeTrue();
            questionResult.Explanation.Should().BeNullOrWhiteSpace();
            questionResult.QuestionsLeft.Should().Be(3);

            controller.SubmitAnswer("no");  // Answer second question correctly
            questionResult.IsCorrect.Should().BeFalse();
            questionResult.Explanation.Should().NotBeNullOrWhiteSpace();
            questionResult.QuestionsLeft.Should().Be(2);

            // Assert
            Assert.NotNull(finalResult);
            Assert.True(!finalResult.IsWinner);
            Assert.Equal(1, finalResult.Score);
            Assert.Equal(2, finalResult.QuestionsLeft);
            Assert.Equal(0, finalResult.AttemptsLeft);
        }

        [Fact]
        public void SubmitAnswer_CorrectAnswer_TriggersCorrectResult()
        {
           // GameResult result = null;
           // _controller.OnAnswerProcessed += r => result = r;

          //  _controller.StartGame();
          //  _controller.SubmitAnswer("да");

           // Assert.True(result.IsCorrect);
        }
    }
}