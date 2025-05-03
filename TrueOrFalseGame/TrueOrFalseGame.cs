using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueOrFalseGame
{
    public class TrueOrFalseGame
    {
        /// <summary>
        /// Event for user to input answer
        /// Take string as answer from user.
        /// </summary>
        public event Func<string, Answer> InputAnswer;
        /// <summary>
        /// Event for user to show right answer
        /// </summary>
        public event Action<bool,string> ShowAnswer;
        /// <summary>
        /// Event triggered when the game ends
        /// </summary>
        public event Action<TrueOrFalseGame> GameEnded;
        /// <summary>
        /// Contains default path to file
        /// </summary>
        private readonly string _filePath;
        /// <summary>
        /// Contains questions
        /// </summary>
        private List<Record> _records;
        /// <summary>
        /// Contains current record index
        /// </summary>
        private int _curIndex;
        /// <summary>
        /// Contains max errors to lose game.
        /// </summary>
        private readonly int _maxErrors;
        /// <summary>
        /// Contains errors done by user.
        /// </summary>
        public int ErrorsDone { get; private set; }
        /// <summary>
        /// Returns attempts left to answers questions
        /// </summary>
        public int AttemptsLeft
        {
            get
            {
                return _maxErrors - ErrorsDone;
            }
        }

        /// <summary>
        /// Contains count of right answers done by user
        /// </summary>
        public int RightAnswersCount { get; private set; }
        /// <summary>
        /// Contains count of question to ask.
        /// </summary>
        public int QuestionsLeft
        {
            get
            {
                return _records.Count - _curIndex ;
            }
        }

        /// <summary>
        /// Contains state of the game
        /// </summary>
        public GameState State { get; private set; }

        /// <summary>
        /// Initializes a new instance of the TrueOrFalseGame class, which acts as a true or false game handler.
        /// </summary>
        /// <param name="filePath">Initializes default path to file.</param>
        public TrueOrFalseGame(string filePath,int maxErrors = 2)
        {
            _filePath=filePath;
            State = GameState.NotStarted;
            _maxErrors = maxErrors;
        }
        /// <summary>
        /// Reading csv file. File must contain at least one line. Records must be separated by ';'.
        /// First param in line is question,
        /// Second param in line is answer(must be Yes/No)
        /// Third param is clue.
        /// </summary>
        /// <param name="filePath">Full path to file</param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FormatException"></exception>
        public void ReadCsv(string filePath = "")
        {
            filePath = string.IsNullOrWhiteSpace(filePath) ? _filePath : filePath; 
            try
            {
                _records = File.ReadAllLines(filePath).Select(Record.ParseCSVLine).ToList();
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("Wrong file path");
            }
            catch (IOException)
            {
                throw new IOException("Wrong file structure");
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("File is empty.");
            }
            catch (FormatException)
            {
                throw;
            }
        }
        /// <summary>
        /// Initialize start params and starts game( we can 
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public void Start()
        {
            if (_records == null)
            {
                throw new ArgumentNullException("For game start load answers from csv.");
            }

            if (InputAnswer == null)
            {
                throw new ArgumentNullException("You must set event for user input.");
            }

            if (ShowAnswer == null)
            {
                throw new ArgumentNullException("You must set event for show answer.");
            }

            if (GameEnded == null)
            {
                throw new ArgumentNullException("You must set event for game end.");
            }
            State = GameState.InProgress;
            ErrorsDone = 0;
            RightAnswersCount = 0;
            _curIndex = 0;
            while (State == GameState.InProgress && _curIndex < _records.Count && ErrorsDone < _maxErrors)
            {
                var userAnswer = InputAnswer(_records[_curIndex].Question);
                bool isRight = userAnswer == _records[_curIndex].Answer;
                ShowAnswer(isRight, isRight ? "" : _records[_curIndex].Clue);
                if (!isRight)
                    ErrorsDone++;
                else
                {
                    RightAnswersCount++;
                }
                _curIndex++;
            }

            if (State != GameState.Aborted)
            {
                State = ErrorsDone == _maxErrors ? GameState.Lost : GameState.Won;
                GameEnded(this);
            }
        }
    

        public void GameEnd()
        {
            if (GameEnded == null)
            {
                throw new ArgumentNullException("You must set event for end of the game.");
            }
            State = GameState.Aborted;
            GameEnded(this);
        }
    }
}
