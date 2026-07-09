using System.Collections.ObjectModel;
using System.Windows.Input;
using DevFlow_Assistant.Features.Workflows.ViewModels;
using DevFlow_Assistant.Shared;
using DevFlow_Assistant.Shared.Navigation;
using DevFlowAssistant.Application.Services.Interface;
using DevFlowAssistant.Domain;

namespace DevFlow_Assistant.Features.Workflows.ViewModels;

public class WorkflowListViewModel : ViewModelBase
{
    private readonly IWorkflowService _workflowService;
    private readonly IWorkflowExecutionService _executionService;
    private readonly INavigationService _navigationService;
    private Workflow? _selectedWorkflow;
    private string _searchText = string.Empty;
    private string _statusMessage = string.Empty;

    public WorkflowListViewModel(
        IWorkflowService workflowService,
        IWorkflowExecutionService executionService,
        INavigationService navigationService)
    {
        _workflowService = workflowService;
        _executionService = executionService;
        _navigationService = navigationService;

        RefreshCommand = new AsyncRelayCommand(_ => LoadAsync());
        CreateCommand = new RelayCommand(_ => _navigationService.NavigateTo<WorkflowCreateViewModel>());
        OpenDetailsCommand = new RelayCommand(OpenDetails, CanUseWorkflow);
        EditCommand = new RelayCommand(Edit, CanUseWorkflow);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync, CanUseWorkflow);
        ExecuteCommand = new AsyncRelayCommand(ExecuteAsync, CanUseWorkflow);

        _ = LoadAsync();
    }

    public ObservableCollection<Workflow> Workflows { get; } = [];

    public Workflow? SelectedWorkflow
    {
        get => _selectedWorkflow;
        set => SetProperty(ref _selectedWorkflow, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                _ = LoadAsync();
            }
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ICommand RefreshCommand { get; }
    public ICommand CreateCommand { get; }
    public ICommand OpenDetailsCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ExecuteCommand { get; }

    public async Task LoadAsync()
    {
        Workflows.Clear();
        var workflows = await _workflowService.GetAllAsync();

        foreach (var workflow in workflows.Where(MatchesSearch))
        {
            Workflows.Add(workflow);
        }

        StatusMessage = Workflows.Count == 0
            ? "No hay workflows para mostrar."
            : $"{Workflows.Count} workflow(s) disponibles.";
    }

    private bool MatchesSearch(Workflow workflow)
    {
        return string.IsNullOrWhiteSpace(SearchText)
            || workflow.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
            || (workflow.Description?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    private bool CanUseWorkflow(object? parameter)
    {
        return parameter is Workflow || SelectedWorkflow is not null;
    }

    private Workflow? ResolveWorkflow(object? parameter)
    {
        return parameter as Workflow ?? SelectedWorkflow;
    }

    private void OpenDetails(object? parameter)
    {
        var workflow = ResolveWorkflow(parameter);
        if (workflow is null)
        {
            return;
        }

        _navigationService.NavigateTo<WorkflowDetailsViewModel>(viewModel => viewModel.WorkflowId = workflow.Id);
    }

    private void Edit(object? parameter)
    {
        var workflow = ResolveWorkflow(parameter);
        if (workflow is null)
        {
            return;
        }

        _navigationService.NavigateTo<WorkflowEditViewModel>(viewModel => viewModel.WorkflowId = workflow.Id);
    }

    private async Task DeleteAsync(object? parameter)
    {
        var workflow = ResolveWorkflow(parameter);
        if (workflow is null)
        {
            return;
        }

        await _workflowService.DeleteAsync(workflow.Id);
        SelectedWorkflow = null;
        await LoadAsync();
    }

    private async Task ExecuteAsync(object? parameter)
    {
        var workflow = ResolveWorkflow(parameter);
        if (workflow is null)
        {
            return;
        }

        StatusMessage = $"Ejecutando {workflow.Name}...";
        var logs = await _executionService.ExecuteAsync(workflow.Id);
        StatusMessage = $"Ejecucion finalizada para {workflow.Name}. Registros generados: {logs.Count}.";
    }
}
