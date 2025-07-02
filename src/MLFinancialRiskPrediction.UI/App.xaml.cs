namespace MLFinancialRiskPrediction.UI
{
    using System.Windows;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using MLFinancialRiskPrediction.UI.Extensions;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Host = CreateHostBuilder().Build();
        }

        public static IHost? Host { get; private set; }

        public static T GetRequiredService<T>() where T : notnull
        {
            return Host.Services.GetRequiredService<T>() ?? throw new InvalidOperationException("Host not initialized");
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            if (Host != null)
            {
                await Host.StartAsync();
            }

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (Host != null)
            {
                await Host.StopAsync();
                Host.Dispose();
            }

            base.OnExit(e);
        }

        private static IHostBuilder CreateHostBuilder()
        {
            return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddUIServices();
                });
        }
    }
}