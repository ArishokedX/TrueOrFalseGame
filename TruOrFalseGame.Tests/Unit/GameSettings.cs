using System.Text;
using Xunit;

namespace TrueOrFalseGame.Tests
{
    public class GameSettingsTests
    {
        [Fact]
        public void Encoding_WhenUtf8Set_ReturnsUtf8Encoding()
        {
            var settings = new GameSettings { EncodingString = "utf-8" };
            Assert.Equal(Encoding.UTF8, settings.Encoding);
        }

        [Fact]
        public void PositiveAnswersArray_WithCommaSeparated_ReturnsSplitValues()
        {
            var settings = new GameSettings { PositiveAsnwers = "yes,да,1" };
            Assert.Equal(3, settings.PositiveAsnwersArray.Count());
        }


    }
}