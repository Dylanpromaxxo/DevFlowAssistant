using System.Windows.Input;
using DevFlow_Assistant.Shared;
using DevFlow_Assistant.Shared.Navigation;
using DevFlowAssistant.Application.Models;
using DevFlowAssistant.Application.Services.Interface;

namespace DevFlow_Assistant.Features.Workflows.ViewModels;

public class WorkflowEditViewModel : ViewModelBase
{
    private readonly IWorkflowService _workflowService;
    private readonly INavigationService _navigationService;
    private int _workflowId;
    private string _name = string.Empty;
    private string _description = string.Empty;
    private bool _isActive = true;
    private string _validationMessage = string.Empty;

    public WorkflowEditViewModel(IWorkflowService workflowService, INavigationService navigationService)
    {
        _workflowService = workflowService;
        _navigationService = navigationService;
        SaveCommand = new AsyncRelayCommand(_ => SaveAsync());
        CancelCommand = new RelayCommand(_ => _navigationService.NavigateTo<WorkflowDetailsViewModel>(vm => vm.WorkflowId = WorkflowId));
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

    public string ValidationMessage
    {
        get => _validationMessage;
        set => SetProperty(ref _validationMessage, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    private async Task LoadAsync()
    {
        var workflow = await _workflowService.GetByIdAsync(WorkflowId);
        if (workflow is null)
        {
            ValidationMessage = "No se encontró el workflow.";
            return;
        }

        Name = workflow.Name;
        Description = workflow.Description ?? string.Empty;
        IsActive = workflow.IsActive;
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            ValidationMessage = "El nombre es obligatorio.";
            return;
        }

        await _workflowService.UpdateAsync(new UpdateWorkflowRequest(WorkflowId, Name, Description, IsActive));
        _navigationService.NavigateTo<WorkflowDetailsViewModel>(viewModel => viewModel.WorkflowId = WorkflowId);
    }
}
