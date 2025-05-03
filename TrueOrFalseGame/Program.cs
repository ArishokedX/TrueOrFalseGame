namespace TrueOrFalseGame
{
    internal class Program
    {
        const string continueAnswer = "YES";
        static void Main(string[] args)
        {
            try
            {
                const string filePath= @"D:\Questions(in).csv";
                var game = new TrueOrFalseGame(filePath);
                game.InputAnswer += InputAnswerHandler;
                game.ShowAnswer += ShowAnswerHandler;
                game.GameEnded += GameEndedHandler;
                game.ReadCsv();
                do
                {
                    Console.Clear();
                    game.Start();
                    Console.Write("Do you wish to try again? Press Yes to continue: ");
                } while (Console.ReadLine()?.ToUpper() == continueAnswer);
                

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

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

        private static void GameEndedHandler(TrueOrFalseGame sender)
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
