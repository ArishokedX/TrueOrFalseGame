using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace TrueOrFalseGame
{
    public class CsvQuestionSource : IQuestionSource
    {
        private readonly string _filePath;
        private char _separator;
        private readonly IReadOnlyCollection<string> _negativeAnswersArray;
        private readonly IReadOnlyCollection<string> _positiveAnswersArray;
        public IReadOnlyCollection<string> DefaultPositiveAnswers = ["да", "yes", "1", "true", "+"];
        public IReadOnlyCollection<string> DefaultNegativeAnswers = ["нет", "no", "0", "false", "-"];
        public IEnumerable<string> PositiveAnswersArray
        {
            get
            {
                return _positiveAnswersArray;
            }
        }
        public IEnumerable<string> NegativeAnswersArray
        {
            get
            {
                return _negativeAnswersArray;
            }
        }
        public CsvQuestionSource(string filePath, IEnumerable<string> positiveAnswersArray , IEnumerable<string> negativeAnswersArray, char separator = ';')
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));
            

            _filePath = filePath;
            _separator = separator;
            _positiveAnswersArray = positiveAnswersArray?.Select(x=> x.Trim().ToLowerInvariant()).ToList() ?? DefaultPositiveAnswers;
            _negativeAnswersArray = negativeAnswersArray?.Select(x => x.Trim().ToLowerInvariant()).ToList() ?? DefaultNegativeAnswers;
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

        
        public Question ParseQuestionLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                throw new FormatException($"Input string is should not be empty,");

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

        private bool ParseBool(string value)
        {
            if (_positiveAnswersArray.Contains(value.ToLowerInvariant()) ||
                _negativeAnswersArray.Contains(value.ToLowerInvariant()))
                return _positiveAnswersArray.Contains(value.ToLowerInvariant());
            else
                throw new FormatException($"Invalid boolean value: {value}");


        }
    }
}
