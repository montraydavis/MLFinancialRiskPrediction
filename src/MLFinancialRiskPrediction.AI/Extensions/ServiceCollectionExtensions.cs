namespace MLFinancialRiskPrediction.AI.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.ML;
    using Microsoft.SemanticKernel;

    using MLFinancialRiskPrediction.AI.Plugins;
    using MLFinancialRiskPrediction.Configuration;
    using MLFinancialRiskPrediction.Services;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMLServices(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerService, ConsoleLogger>();
            services.AddSingleton<DataLoader>();
            services.AddSingleton<FeatureEngineer>();
            services.AddSingleton<RiskScoreTrainer>();
            services.AddSingleton<LoanApprovalTrainer>();
            services.AddSingleton<ModelEvaluator>();
            services.AddKeyedSingleton("FinancialRiskV2", new MLContext(ModelConfiguration.RandomSeed));

            return services;
        }

        public static IServiceCollection AddSemanticKernel(this IServiceCollection services)
        {
            string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
                           "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";

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("OPENAI_API_KEY is not set.");
            }

            services.AddOpenAIChatCompletion("gpt-4o-mini", apiKey);

            services.AddSingleton<Kernel>(provider =>
            {
                Kernel kernel = new Kernel(provider);
                kernel.Plugins.AddFromType<EnsemblePredictorPlugin>(serviceProvider: provider);
                return kernel;
            });

            return services;
        }
    }
}