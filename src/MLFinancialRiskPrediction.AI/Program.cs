namespace MLFinancialRiskPrediction.AI
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.Connectors.OpenAI;

    using MLFinancialRiskPrediction.AI.Extensions;

    internal class Program
    {
        static async Task Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddMLServices()
                           .AddSemanticKernel();
                })
                .Build();

            ILoggerService logger = host.Services.GetRequiredService<ILoggerService>();
            Kernel kernel = host.Services.GetRequiredService<Kernel>();

            Console.WriteLine("""
                === Financial Risk Prediction AI Chat ===

                Available commands:
                    - Ask about risk scores or loan approvals
                    - Request to see current loan sample
                    - Ask to update loan parameters
                    - Type 'exit' to quit

                =========================================
                """);

            OpenAIPromptExecutionSettings settings = new()
            {
                ModelId = "gpt-4o-mini",
                Temperature = 0.3,
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            KernelArguments kernelArgs = new KernelArguments(settings);

            while (true)
            {
                Console.Write("\nYou: ");
                string? userInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userInput) || userInput.ToLower() == "exit")
                {
                    break;
                }

                try
                {
                    FunctionResult response = await kernel.InvokePromptAsync(userInput, kernelArgs);

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"AI: {response.GetValue<string>()}");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.ResetColor();
                }
            }

            logger.Log("Chat session ended.");
        }
    }
}