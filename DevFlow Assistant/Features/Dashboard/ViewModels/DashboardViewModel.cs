using System.Collections.ObjectModel;
using System.Windows.Input;
using DevFlow_Assistant.Features.Workflows.ViewModels;
using DevFlow_Assistant.Shared;
using DevFlow_Assistant.Shared.Navigation;
using DevFlowAssistant.Application.Services.Interface;
using DevFlowAssistant.Domain;

namespace DevFlow_Assistant.Features.Dashboard.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private readonly IWorkflowService _workflowService;
    private readonly IExecutionLogService _logService;
    private readonly IWorkflowExecutionService _executionService;
    private readonly INavigationService _navigationService;
    private int _workflowCount;
    private string _lastStatus = "Sin ejecuciones.";
    private string _statusMessage = "Listo para ejecutar workflows.";

    public DashboardViewModel(
        IWorkflowService workflowService,
        IExecutionLogService logService,
        IWorkflowExecutionService executionService,
        INavigationService navigationService)
    {
        _workflowService = workflowService;
        _logService = logService;
        _executionService = executionService;
        _navigationService = navigationService;

        OpenWorkflowsCommand = new RelayCommand(_ => _navigationService.NavigateTo<WorkflowListViewModel>());
        CreateWorkflowCommand = new RelayCommand(_ => _navigationService.NavigateTo<WorkflowCreateViewModel>());
        ExecuteWorkflowCommand = new AsyncRelayCommand(ExecuteWorkflowAsync, parameter => parameter is Workflow);

        _ = LoadAsync();
    }

    public int WorkflowCount
    {
        get => _workflowCount;
        set => SetProperty(ref _workflowCount, value);
    }

    public string LastStatus
    {
        get => _lastStatus;
        set => SetProperty(ref _lastStatus, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ICommand OpenWorkflowsCommand { get; }
    public ICommand CreateWorkflowCommand { get; }
    public ICommand ExecuteWorkflowCommand { get; }

    public ObservableCollection<Workflow> RecentWorkflows { get; } = [];
    public ObservableCollection<ExecutionLog> RecentLogs { get; } = [];

    public async Task LoadAsync()
    {
        var workflows = await _workflowService.GetAllAsync();
        WorkflowCount = workflows.Count;

        RecentWorkflows.Clear();
        foreach (var workflow in workflows.OrderByDescending(workflow => workflow.UpdatedAt ?? workflow.CreatedAt).Take(5))
        {
            RecentWorkflows.Add(workflow);
        }

        RecentLogs.Clear();
        var logs = await _logService.GetAllRecentAsync(10);
        foreach (var log in logs)
        {
            RecentLogs.Add(log);
        }

        LastStatus = logs.FirstOrDefault()?.Status ?? "Sin ejecuciones.";
    }

    private async Task ExecuteWorkflowAsync(object? parameter)
    {
        if (parameter is not Workflow workflow)
        {
            return;
        }

        StatusMessage = $"Ejecutando {workflow.Name}...";
        var logs = await _executionService.ExecuteAsync(workflow.Id);
        StatusMessage = $"Ejecucion finalizada. Registros generados: {logs.Count}.";
        await LoadAsync();
    }
}
