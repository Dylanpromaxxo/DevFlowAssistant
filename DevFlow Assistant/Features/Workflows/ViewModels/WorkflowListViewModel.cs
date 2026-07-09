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
    private readonly INavigationService _navigationService;
    private Workflow? _selectedWorkflow;
    private string _searchText = string.Empty;

    public WorkflowListViewModel(IWorkflowService workflowService, INavigationService navigationService)
    {
        _workflowService = workflowService;
        _navigationService = navigationService;

        RefreshCommand = new AsyncRelayCommand(_ => LoadAsync());
        CreateCommand = new RelayCommand(_ => _navigationService.NavigateTo<WorkflowCreateViewModel>());
        OpenDetailsCommand = new RelayCommand(_ => OpenSelected(), _ => SelectedWorkflow is not null);
        EditCommand = new RelayCommand(_ => EditSelected(), _ => SelectedWorkflow is not null);
        DeleteCommand = new AsyncRelayCommand(_ => DeleteSelectedAsync(), _ => SelectedWorkflow is not null);

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

    public ICommand RefreshCommand { get; }
    public ICommand CreateCommand { get; }
    public ICommand OpenDetailsCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }

    public async Task LoadAsync()
    {
        Workflows.Clear();
        var workflows = await _workflowService.GetAllAsync();

        foreach (var workflow in workflows.Where(MatchesSearch))
        {
            Workflows.Add(workflow);
        }
    }

    private bool MatchesSearch(Workflow workflow)
    {
        return string.IsNullOrWhiteSpace(SearchText)
            || workflow.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
            || (workflow.Description?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    private void OpenSelected()
    {
        if (SelectedWorkflow is null)
        {
            return;
        }

        _navigationService.NavigateTo<WorkflowDetailsViewModel>(viewModel => viewModel.WorkflowId = SelectedWorkflow.Id);
    }

    private void EditSelected()
    {
        if (SelectedWorkflow is null)
        {
            return;
        }

        _navigationService.NavigateTo<WorkflowEditViewModel>(viewModel => viewModel.WorkflowId = SelectedWorkflow.Id);
    }

    private async Task DeleteSelectedAsync()
    {
        if (SelectedWorkflow is null)
        {
            return;
        }

        await _workflowService.DeleteAsync(SelectedWorkflow.Id);
        SelectedWorkflow = null;
        await LoadAsync();
    }
}
