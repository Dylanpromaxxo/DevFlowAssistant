using System.Windows.Input;
using DevFlow_Assistant.Features.Workflows.ViewModels;
using DevFlow_Assistant.Shared;
using DevFlow_Assistant.Shared.Navigation;
using DevFlowAssistant.Application.Models;
using DevFlowAssistant.Application.Services.Interface;

namespace DevFlow_Assistant.Features.Workflows.ViewModels;

public class WorkflowCreateViewModel : ViewModelBase
{
    private readonly IWorkflowService _workflowService;
    private readonly INavigationService _navigationService;
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _validationMessage = string.Empty;

    public WorkflowCreateViewModel(IWorkflowService workflowService, INavigationService navigationService)
    {
        _workflowService = workflowService;
        _navigationService = navigationService;
        SaveCommand = new AsyncRelayCommand(_ => SaveAsync());
        CancelCommand = new RelayCommand(_ => _navigationService.NavigateTo<WorkflowListViewModel>());
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

    public string ValidationMessage
    {
        get => _validationMessage;
        set => SetProperty(ref _validationMessage, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            ValidationMessage = "Escribe un nombre para el workflow.";
            return;
        }

        var workflow = await _workflowService.CreateAsync(new CreateWorkflowRequest(Name, Description));
        _navigationService.NavigateTo<WorkflowDetailsViewModel>(viewModel => viewModel.WorkflowId = workflow.Id);
    }
}
