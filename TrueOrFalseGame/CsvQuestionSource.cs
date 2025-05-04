using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace TrueOrFalseGame
{
    public class CsvQuestionSource : IQuestionSource
    {
        private readonly string _filePath;
        private readonly int _maxMistakesAllowed;
        private readonly char _separator;

        public int MaxMistakesAllowed => _maxMistakesAllowed;

        public CsvQuestionSource(string filePath, int maxMistakesAllowed = 2, char separator = ';')
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));
            if (maxMistakesAllowed <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    paramName: nameof(maxMistakesAllowed),
                    actualValue: maxMistakesAllowed,
                    message: "Game difficulty must be positive. " +
                             $"Value '{maxMistakesAllowed}' is not valid. " +
                             "Please specify how many mistakes are allowed before game over.");

            }

            _filePath = filePath;
            _maxMistakesAllowed = maxMistakesAllowed;
            _separator = separator;
        }

        public bool IsAvailable()
        {
            return File.Exists(_filePath);
        }

        public IEnumerable<Question> LoadQuestions()
        {
            if (!IsAvailable())
                throw new FileNotFoundException($"Question file not found: {_filePath}");

            try
            {
                return File.ReadAllLines(_filePath)
                    .Skip(1) // Пропускаем заголовок
                    .Select(ParseQuestionLine)
                    .Where(q => q != null)
                    .ToList();
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                throw new QuestionSourceException("Failed to read questions file", ex);
            }
        }

        private Question ParseQuestionLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return null;

            var parts = line.Split(_separator);

            if (parts.Length < 3)
                throw new FormatException($"Invalid question format in line: {line}");

            try
            {
                Question q = new Question();
                return new Question()
                {
                    Text = parts[0].Trim(),
                    CorrectAnswer = ParseBool(parts[1].Trim()),
                    Explanation = parts[2].Trim()
                };
            }
            catch (FormatException ex)
            {
                throw new FormatException($"Failed to parse question: {line}", ex);
            }
        }

        private static bool ParseBool(string value)
        {
            return value.ToLowerInvariant() switch
            {
                "true" or "yes" or "да" or "1" => true,
                "false" or "no" or "нет" or "0" => false,
                _ => throw new FormatException($"Invalid boolean value: {value}")
            };
        }
    }

    public class QuestionSourceException : Exception
    {
        public QuestionSourceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
