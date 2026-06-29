using Microsoft.Extensions.DependencyInjection;
using WinFormMain.Services;

namespace WinFormMain
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var services = new ServiceCollection();
            ConfigureServices(services);

            using var serviceProvider = services.BuildServiceProvider();
            var mainForm = serviceProvider.GetRequiredService<MainForm>();
            Application.Run(mainForm);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDynamoDBService, DynamoDBService>();
            services.AddTransient<MainForm>();
        }
    }
}