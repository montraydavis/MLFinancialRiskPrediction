namespace MLFinancialRiskPrediction.AI
{
    public class ConsoleLogger : ILoggerService
    {
        public void Log(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[LOG]:\n{message}");
            Console.ResetColor();
        }
    }
}
