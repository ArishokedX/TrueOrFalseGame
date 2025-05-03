using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueOrFalseGame
{
    public class Record
    {
        private const char Separator = ';';
        private const int QuestionIndex = 0;
        private const int AnswerIndex = 1;
        private const int ClueIndex = 2;
        public string Question { get; private set; }
        public Answer Answer { get; private set; }
        public string Clue { get; private set; }
        /// <summary>
        /// Parses the provided string and returns a corresponding record.Params in string must be separated by ';'.
        /// First param in string is question.
        /// Second param in string is answer(must be Yes/No).
        /// Third param in string is clue.
        /// </summary>
        /// <param name="line">Contains string to parse.</param>
        /// <returns>Returns a record based on the provided string.</returns>
        public static Record ParseCSVLine(string line)
        {
            
            try
            {
                string[] parts = line.Split(Separator);
                return new Record()
                {
                    Question = parts[QuestionIndex].Trim(),
                    Answer = parts[AnswerIndex]?.Trim().ToUpper() == "YES" ? Answer.Yes : Answer.No,
                    Clue = parts[ClueIndex].Trim()
                };
            }
            catch(Exception ex)
            {
                throw new FormatException("Invalid string structure provided");
            }
        }
    }
}
