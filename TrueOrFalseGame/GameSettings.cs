using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueOrFalseGame
{
    public class GameSettings
    {

        public string QuestionsFilePath { get; set; }
        public int MaxMistakesAllowed { get; set; }
        public string EncodingString { get; set; }
        public char Separator { get; set; }

        public string PositiveAsnwers { get; set; }
        public string NegativeAsnwers { get; set; }
        public Encoding Encoding
        {
            get
            {
                var result = Encoding.Default;
                switch (EncodingString?.ToLower())
                {
                    case "utf-8":
                        result = Encoding.UTF8;
                        break;
                    case "utf-32":
                        result = Encoding.UTF32;
                        break;
                    case "unicode":
                        result = Encoding.Unicode;
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        public IEnumerable<string> PositiveAsnwersArray
        {
            get
            {
                
                return PositiveAsnwers.Trim().ToLowerInvariant().Split(',');
            }
        }
        public IEnumerable<string> NegativeAsnwersArray
        {
            get
            {
                return NegativeAsnwers.Trim().ToLowerInvariant().Split(',');
            }
        }
    }
}
