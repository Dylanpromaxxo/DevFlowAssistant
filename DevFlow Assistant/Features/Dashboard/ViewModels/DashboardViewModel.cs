using System.Collections.ObjectModel;
using DevFlow_Assistant.Shared;
using DevFlowAssistant.Application.Services.Interface;
using DevFlowAssistant.Domain;

namespace DevFlow_Assistant.Features.Dashboard.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private readonly IWorkflowService _workflowService;
    private readonly IExecutionLogService _logService;
    private int _workflowCount;
    private string _lastStatus = "Sin ejecuciones.";

    public DashboardViewModel(IWorkflowService workflowService, IExecutionLogService logService)
    {
        _workflowService = workflowService;
        _logService = logService;
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

    public ObservableCollection<ExecutionLog> RecentLogs { get; } = [];

    public async Task LoadAsync()
    {
        var workflows = await _workflowService.GetAllAsync();
        WorkflowCount = workflows.Count;

        RecentLogs.Clear();
        var logs = await _logService.GetAllRecentAsync(10);
        foreach (var log in logs)
        {
            RecentLogs.Add(log);
        }

        LastStatus = logs.FirstOrDefault()?.Status ?? "Sin ejecuciones.";
    }
}
