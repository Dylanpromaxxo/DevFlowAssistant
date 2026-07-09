using System.IO;
using System.Windows;
using DevFlow_Assistant.Features.Dashboard.ViewModels;
using DevFlow_Assistant.Features.ExecutionLogs.ViewModels;
using DevFlow_Assistant.Features.Shell.ViewModels;
using DevFlow_Assistant.Features.WorkflowActions.ViewModels;
using DevFlow_Assistant.Features.Workflows.ViewModels;
using DevFlow_Assistant.Shared.Navigation;
using DevFlowAssistant.Application.Interfaces;
using DevFlowAssistant.Application.Services.implementation;
using DevFlowAssistant.Application.Services.Interface;
using DevFlowAssistant.Infrastructure;
using DevFlowAssistant.Infrastructure.Database;
using DevFlowAssistant.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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

            using (var scope = Services.CreateScope())
            {
                var databaseInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
                databaseInitializer.InitializeAsync().GetAwaiter().GetResult();
            }

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            string databasePath = GetDatabasePath();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite($"Data Source={databasePath}");
            });

            services.AddScoped<IWorkflowRepository, WorkflowRepository>();
            services.AddScoped<IWorkflowActionRepository, WorkflowActionRepository>();
            services.AddScoped<IExecutionLogRepository, ExecutionLogRepository>();
            services.AddScoped<IWorkflowService, WorkflowService>();
            services.AddScoped<IWorkflowActionService, WorkflowActionService>();
            services.AddScoped<IExecutionLogService, ExecutionLogService>();
            services.AddScoped<IWorkflowExecutionService, WorkflowExecutionService>();
            services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();

            services.AddSingleton<INavigationService, NavigationService>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<WorkflowListViewModel>();
            services.AddTransient<WorkflowCreateViewModel>();
            services.AddTransient<WorkflowEditViewModel>();
            services.AddTransient<WorkflowDetailsViewModel>();
            services.AddTransient<WorkflowActionsViewModel>();
            services.AddTransient<ExecutionLogsViewModel>();
            services.AddSingleton<MainWindowViewModel>();
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
