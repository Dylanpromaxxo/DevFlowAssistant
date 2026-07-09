using System.Collections.ObjectModel;
using System.Windows.Input;
using DevFlow_Assistant.Features.ExecutionLogs.ViewModels;
using DevFlow_Assistant.Features.WorkflowActions.ViewModels;
using DevFlow_Assistant.Shared;
using DevFlow_Assistant.Shared.Navigation;
using DevFlowAssistant.Application.Services.Interface;
using DevFlowAssistant.Domain.Entities;

namespace DevFlow_Assistant.Features.Workflows.ViewModels;

public class WorkflowDetailsViewModel : ViewModelBase
{
    private readonly IWorkflowService _workflowService;
    private readonly IWorkflowActionService _actionService;
    private readonly IWorkflowExecutionService _executionService;
    private readonly INavigationService _navigationService;
    private int _workflowId;
    private string _name = string.Empty;
    private string _description = string.Empty;
    private bool _isActive;
    private string _statusMessage = string.Empty;

    public WorkflowDetailsViewModel(
        IWorkflowService workflowService,
        IWorkflowActionService actionService,
        IWorkflowExecutionService executionService,
        INavigationService navigationService)
    {
        _workflowService = workflowService;
        _actionService = actionService;
        _executionService = executionService;
        _navigationService = navigationService;

        ExecuteCommand = new AsyncRelayCommand(_ => ExecuteAsync());
        EditCommand = new RelayCommand(_ => _navigationService.NavigateTo<WorkflowEditViewModel>(vm => vm.WorkflowId = WorkflowId));
        ManageActionsCommand = new RelayCommand(_ => _navigationService.NavigateTo<WorkflowActionsViewModel>(vm => vm.WorkflowId = WorkflowId));
        ViewLogsCommand = new RelayCommand(_ => _navigationService.NavigateTo<ExecutionLogsViewModel>(vm => vm.WorkflowId = WorkflowId));
        BackCommand = new RelayCommand(_ => _navigationService.NavigateTo<WorkflowListViewModel>());
    }

    public int WorkflowId
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

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ObservableCollection<WorkflowAction> Actions { get; } = [];

    public ICommand ExecuteCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand ManageActionsCommand { get; }
    public ICommand ViewLogsCommand { get; }
    public ICommand BackCommand { get; }

    public async Task LoadAsync()
    {
        var workflow = await _workflowService.GetByIdAsync(WorkflowId);
        if (workflow is null)
        {
            StatusMessage = "No se encontró el workflow.";
            return;
        }

        Name = workflow.Name;
        Description = workflow.Description ?? "Sin descripción.";
        IsActive = workflow.IsActive;

        Actions.Clear();
        foreach (var action in await _actionService.GetByWorkflowIdAsync(WorkflowId))
        {
            Actions.Add(action);
        }
    }

    private async Task ExecuteAsync()
    {
        StatusMessage = "Ejecutando workflow...";
        var logs = await _executionService.ExecuteAsync(WorkflowId);
        StatusMessage = $"Ejecución finalizada. Registros generados: {logs.Count}.";
    }
}
