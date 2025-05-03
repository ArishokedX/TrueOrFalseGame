namespace TrueOrFalseGame
{
    using Microsoft.Extensions.Configuration;
    internal class Program
    {
        // const string DefaultPath = "Questions.csv";
        // private const int DefaultMaxMistakes = 2;
        private const string jsonPath = "settings.json";
        static void Main(string[] args)
        {
            
            try
            {
               // GameSettings settings;
                if (!TryInitializeFromJson(out var settings))
                {
                    Console.ReadLine();
                    return;
                }
                var game = new GameManger(settings);
                game.InputAnswer += InputAnswerHandler;
                game.ShowAnswer += ShowAnswerHandler;
                game.GameEnded += GameEndedHandler;
                game.ReadCsv();
                do
                {
                    Console.Clear();
                    game.Start();
                    Console.Write("Do you wish to try again? Press Yes to continue: ");
                } while (Console.ReadLine()?.ToUpper() == "YES");


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }


        private static bool TryInitializeFromJson(out GameSettings settings)
        {
            var result = true;
            settings = null;
            IConfigurationRoot configuration;
            try
            {
                configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(jsonPath)
                    .Build();
                settings = configuration.GetSection("GameSettings").Get<GameSettings>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                result= false;
            }

            if (string.IsNullOrWhiteSpace(settings.QuestionsFilePath))
            {
                Console.WriteLine($"Error in reading path to questions file from {jsonPath}.");
                result = false;
            }

            if (settings.MaxMistakesAllowed == default)
            {
                Console.WriteLine($"Error in reading max allowed mistakes count from {jsonPath}.");
                result = false;
            }

            return result;
        }
        private static Answer InputAnswerHandler(string question)
        {
            string userAnswer=string.Empty;
            bool correctInput = false;
            while (!correctInput)
            {
                Console.WriteLine(question);
                Console.WriteLine(@"Do you believe in previous statement. Yes\No?");
                userAnswer = Console.ReadLine().ToUpper();
                correctInput = userAnswer == "YES" || userAnswer == "NO";
            } 
            
            return userAnswer == "YES" ? Answer.Yes : Answer.No;
        }

        private static void ShowAnswerHandler(bool isRirght,string clue)
        { 
            Console.WriteLine(isRirght ? "Amazing! You was right." : "Oops, you were wrong."+clue);
        }

        private static void GameEndedHandler(GameManger sender)
        {
            switch (sender.State)
            {
                case GameState.Won:
                    Console.WriteLine($"Congratulations! You won the game. Attempts left {sender.AttemptsLeft}");
                break;
                case GameState.Lost:
                    Console.WriteLine($"Nice try. Your answered right for {sender.RightAnswersCount} questions. Questions left {sender.QuestionsLeft}");
                    break;
                case GameState.Aborted:
                    Console.WriteLine($"Game was aborted by user.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unxpected game state.");
            } 
               

        }
    }
}
