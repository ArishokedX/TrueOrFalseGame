using System.Text;

namespace TrueOrFalseGame
{
    using System.Reflection;
    using System.Runtime;
    using Microsoft.Extensions.Configuration;
    internal class Program
    {
        // const string DefaultPath = "Questions.csv";
        // private const int DefaultMaxMistakes = 2;
        private static GameController gameController;
        private const string jsonPath = "settings.json";
        static void Main(string[] args)
        {

            try
            {

                InitializeFromJson(out var settings);
                IQuestionSource qs = new CsvQuestionSource(settings.QuestionsFilePath, settings.MaxMistakesAllowed,
                    settings.Separator);
                gameController = new GameController(qs,settings.PositiveAsnwersArray.Select(x=>x.Trim()), settings.NegativeAsnwersArray.Select(x => x.Trim()));
                gameController.OnQuestionAsked += OnQuestionAskedHandler;
                gameController.OnAnswerProcessed += OnAnswerProcessedHandler;
                gameController.OnGameEnded += OnGameEndedHandler;
              
                do
                {

                    Console.Clear();
                    Console.WriteLine(
                        $"RulesЖ.Э+" +
                        $"Your answer question. If ypu reach {settings.MaxMistakesAllowed} mistakes you lose."+
                        $"\n{settings.PositiveAsnwers} => if it's true"+
                        $"\n{settings.NegativeAsnwers} => if it's false");
                    gameController.StartGame();
                    Console.Write("Do you wish to try again? Press Yes to continue: ");
                } while (Console.ReadLine()?.ToLowerInvariant() == "yes");


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                ;
            }

        }


        private static void InitializeFromJson(out GameSettings settings)
        {
            if (!File.Exists(jsonPath))
            {
                throw new FileNotFoundException($"Configuration file {jsonPath} not found");
            }
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(jsonPath, optional: false, reloadOnChange: true)
                .Build();

            
            settings = configuration.GetSection("GameSettings").Get<GameSettings>()
                           ?? throw new InvalidOperationException("GameSettings section is missing");

            if (string.IsNullOrWhiteSpace(settings.QuestionsFilePath))
            {
                throw new InvalidOperationException("QuestionsFilePath is not configured");
            }

            if (settings.MaxMistakesAllowed <= 0)
            {
                throw new InvalidOperationException("MaxMistakesAllowed must be positive");
            }

            
            if (!File.Exists(settings.QuestionsFilePath))
            {
                throw new FileNotFoundException($"Questions file not found at: {settings.QuestionsFilePath}");
            }

            Console.WriteLine($"Config loaded successfully. Questions file: {settings.QuestionsFilePath}");
        }

        private static void OnQuestionAskedHandler(string question)
        {
            Console.WriteLine(question);
            bool validAnswer = false;
            string userAnswer=string.Empty;
            Console.WriteLine(@"Do you believe in previous statement?");
            while (!validAnswer)
            {
                userAnswer = Console.ReadLine();
                validAnswer = gameController.ValidateAnswer(userAnswer);
                if(!validAnswer)
                    Console.WriteLine(@"Please, enter valid answer.");
            }
            
            gameController.SubmitAnswer(userAnswer);
            // return userAnswer == "YES" ? true : false;
        }

        private static void OnAnswerProcessedHandler(GameResult ansResult)
        {
            Console.WriteLine(ansResult.IsCorrect
                ? $"Amazing! You was right.\nQuestions left " +
                  $"{ansResult.QuestionsLeft}.\nAttempts left {ansResult.AttemptsLeft}"
                : $"Oops, you were wrong.\n{ansResult.Explanation}.\nQuestions left " +
                  $"{ansResult.QuestionsLeft}.\nAttempts left {ansResult.AttemptsLeft}");
            Console.WriteLine();
        }

        private static void OnGameEndedHandler(GameResult gameResult)
        {
            
            Console.WriteLine(gameResult.IsWinner ? $"Congratulations! You won the game. You answered right for {gameResult.Score}"
                : $"Nice try. You answered right for {gameResult.Score} questions. Questions left {gameResult.QuestionsLeft}");
          
               
        
        }
    }
}
