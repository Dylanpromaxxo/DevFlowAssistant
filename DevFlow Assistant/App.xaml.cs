using DevFlowAssistant.Application.Interfaces;
using DevFlowAssistant.Application.Services.implementation;
using DevFlowAssistant.Application.Services.Interface;
using DevFlowAssistant.Infrastructure;
using DevFlowAssistant.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;

namespace DevFlow_Assistant
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            Services = services.BuildServiceProvider();

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            string databasePath = GetDatabasePath();

            // Solo para probar. Luego puedes quitarlo.
            MessageBox.Show(databasePath);

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite($"Data Source={databasePath}");
            });

            services.AddScoped<IWorkflowRepository, WorkflowRepository>();
            services.AddScoped<IWorkflowService, WorkflowService>();

            services.AddTransient<MainWindow>();
        }

        private static string GetDatabasePath()
        {
            string appDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "DevFlowAssistant"
            );

            Directory.CreateDirectory(appDataFolder);

            string userDatabasePath = Path.Combine(appDataFolder, "DevFlow.db");

            if (!File.Exists(userDatabasePath))
            {
                string sourceDatabasePath = Path.Combine(
                    AppContext.BaseDirectory,
                    "Data",
                    "DevFlow.db"
                );

                File.Copy(sourceDatabasePath, userDatabasePath);
            }

            return userDatabasePath;
        }
    }
}