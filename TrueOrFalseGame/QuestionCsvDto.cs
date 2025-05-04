using System.Globalization;
using CsvHelper.Configuration;

namespace TrueOrFalseGame;

public class QuestionCsvDto
{
    /// <summary>
    /// Question text (first column in CSV)
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Correct answer (second column)
    /// Can be in format: true/yes/да/1 or false/no/нет/0
    /// </summary>
    public string RawAnswer { get; set; }

    /// <summary>
    /// Explanation shown when answer is wrong (third column)
    /// </summary>
    public string Explanation { get; set; }

    /// <summary>
    /// Custom CSV mapping configuration
    /// </summary>
    public sealed class Map : ClassMap<QuestionCsvDto>
    {
        public Map()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.Text).Name("Question", "Вопрос");
            Map(m => m.RawAnswer).Name("Answer", "Ответ");
            Map(m => m.Explanation).Name("Explanation", "Пояснение");
        }
    }
}