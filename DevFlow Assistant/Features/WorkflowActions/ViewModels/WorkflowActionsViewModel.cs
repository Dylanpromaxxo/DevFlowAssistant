using System.Collections.ObjectModel;
using System.Windows.Input;
using DevFlow_Assistant.Features.Workflows.ViewModels;
using DevFlow_Assistant.Shared;
using DevFlow_Assistant.Shared.Navigation;
using DevFlowAssistant.Application.Models;
using DevFlowAssistant.Application.Services.Interface;
using DevFlowAssistant.Domain.Entities;
using DevFlowAssistant.Domain.Enums;

namespace DevFlow_Assistant.Features.WorkflowActions.ViewModels;

public class WorkflowActionsViewModel : ViewModelBase
{
    private readonly IWorkflowActionService _actionService;
    private readonly INavigationService _navigationService;
    private int _workflowId;
    private WorkflowAction? _selectedAction;
    private string _name = string.Empty;
    private string _selectedActionType = WorkflowActionTypes.OpenUrl;
    private string _value = string.Empty;
    private string _arguments = string.Empty;
    private string _workingDirectory = string.Empty;
    private int _timeoutSeconds = 120;
    private bool _continueOnError;
    private string _validationMessage = string.Empty;

    public WorkflowActionsViewModel(IWorkflowActionService actionService, INavigationService navigationService)
    {
        _actionService = actionService;
        _navigationService = navigationService;

        AddCommand = new AsyncRelayCommand(_ => AddAsync());
        SaveSelectedCommand = new AsyncRelayCommand(_ => SaveSelectedAsync(), _ => SelectedAction is not null);
        DeleteSelectedCommand = new AsyncRelayCommand(_ => DeleteSelectedAsync(), _ => SelectedAction is not null);
        MoveUpCommand = new AsyncRelayCommand(_ => MoveAsync(-1), _ => SelectedAction is not null);
        MoveDownCommand = new AsyncRelayCommand(_ => MoveAsync(1), _ => SelectedAction is not null);
        BackCommand = new RelayCommand(_ => _navigationService.NavigateTo<WorkflowDetailsViewModel>(vm => vm.WorkflowId = WorkflowId));
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

    public ObservableCollection<WorkflowAction> Actions { get; } = [];
    public ObservableCollection<string> ActionTypes { get; } = new(WorkflowActionTypes.All);

    public WorkflowAction? SelectedAction
    {
        get => _selectedAction;
        set
        {
            if (SetProperty(ref _selectedAction, value) && value is not null)
            {
                Name = value.Name;
                SelectedActionType = value.ActionType;
                Value = value.Value;
                Arguments = value.Arguments ?? string.Empty;
                WorkingDirectory = value.WorkingDirectory ?? string.Empty;
                TimeoutSeconds = value.TimeoutSeconds;
                ContinueOnError = value.ContinueOnError;
            }
        }
    }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string SelectedActionType
    {
        get => _selectedActionType;
        set => SetProperty(ref _selectedActionType, value);
    }

    public string Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }

    public string Arguments
    {
        get => _arguments;
        set => SetProperty(ref _arguments, value);
    }

    public string WorkingDirectory
    {
        get => _workingDirectory;
        set => SetProperty(ref _workingDirectory, value);
    }

    public int TimeoutSeconds
    {
        get => _timeoutSeconds;
        set => SetProperty(ref _timeoutSeconds, value);
    }

    public bool ContinueOnError
    {
        get => _continueOnError;
        set => SetProperty(ref _continueOnError, value);
    }

    public string ValidationMessage
    {
        get => _validationMessage;
        set => SetProperty(ref _validationMessage, value);
    }

    public ICommand AddCommand { get; }
    public ICommand SaveSelectedCommand { get; }
    public ICommand DeleteSelectedCommand { get; }
    public ICommand MoveUpCommand { get; }
    public ICommand MoveDownCommand { get; }
    public ICommand BackCommand { get; }

    public async Task LoadAsync()
    {
        Actions.Clear();
        foreach (var action in await _actionService.GetByWorkflowIdAsync(WorkflowId))
        {
            Actions.Add(action);
        }
    }

    private async Task AddAsync()
    {
        if (!ValidateForm())
        {
            return;
        }

        await _actionService.AddAsync(new CreateWorkflowActionRequest(
            WorkflowId,
            Name,
            SelectedActionType,
            Value,
            Arguments,
            WorkingDirectory,
            TimeoutSeconds,
            ContinueOnError));

        ClearForm();
        await LoadAsync();
    }

    private async Task SaveSelectedAsync()
    {
        if (SelectedAction is null || !ValidateForm())
        {
            return;
        }

        await _actionService.UpdateAsync(new UpdateWorkflowActionRequest(
            SelectedAction.Id,
            SelectedAction.WorkflowId,
            Name,
            SelectedActionType,
            Value,
            Arguments,
            WorkingDirectory,
            SelectedAction.SortOrder,
            SelectedAction.IsEnabled,
            TimeoutSeconds,
            ContinueOnError,
            SelectedAction.CreatedAt));

        await LoadAsync();
    }

    private async Task DeleteSelectedAsync()
    {
        if (SelectedAction is null)
        {
            return;
        }

        await _actionService.DeleteAsync(SelectedAction.Id);
        SelectedAction = null;
        ClearForm();
        await LoadAsync();
    }

    private async Task MoveAsync(int direction)
    {
        if (SelectedAction is null)
        {
            return;
        }

        await _actionService.MoveAsync(WorkflowId, SelectedAction.Id, direction);
        await LoadAsync();
    }

    private bool ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            ValidationMessage = "Escribe un nombre para la acción.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Value))
        {
            ValidationMessage = "Escribe el valor principal de la acción.";
            return false;
        }

        ValidationMessage = string.Empty;
        return true;
    }

    private void ClearForm()
    {
        Name = string.Empty;
        SelectedActionType = WorkflowActionTypes.OpenUrl;
        Value = string.Empty;
        Arguments = string.Empty;
        WorkingDirectory = string.Empty;
        TimeoutSeconds = 120;
        ContinueOnError = false;
    }
}
