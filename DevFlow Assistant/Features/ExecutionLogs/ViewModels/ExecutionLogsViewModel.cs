using System.Collections.ObjectModel;
using System.Windows.Input;
using DevFlow_Assistant.Shared;
using DevFlowAssistant.Application.Services.Interface;
using DevFlowAssistant.Domain;

namespace DevFlow_Assistant.Features.ExecutionLogs.ViewModels;

public class ExecutionLogsViewModel : ViewModelBase
{
    private readonly IExecutionLogService _logService;
    private int? _workflowId;

    public ExecutionLogsViewModel(IExecutionLogService logService)
    {
        _logService = logService;
        RefreshCommand = new AsyncRelayCommand(_ => LoadAsync());
        _ = LoadAsync();
    }

    public int? WorkflowId
    {
        get => _workflowId;
        set
        {
            if (SetProperty(ref _workflowId, value))
            {
                _ = LoadAsync();
            }
        }
    }

    public ObservableCollection<ExecutionLog> Logs { get; } = [];

    public ICommand RefreshCommand { get; }

    public async Task LoadAsync()
    {
        Logs.Clear();
        var logs = WorkflowId.HasValue
            ? await _logService.GetRecentAsync(WorkflowId.Value, 50)
            : await _logService.GetAllRecentAsync(50);

        foreach (var log in logs)
        {
            Logs.Add(log);
        }
    }
}
