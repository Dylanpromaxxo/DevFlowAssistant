using System.Collections.ObjectModel;
using System.Windows.Input;
using DevFlow_Assistant.Features.Dashboard.ViewModels;
using DevFlow_Assistant.Features.ExecutionLogs.ViewModels;
using DevFlow_Assistant.Features.Workflows.ViewModels;
using DevFlow_Assistant.Shared;
using DevFlow_Assistant.Shared.Navigation;

namespace DevFlow_Assistant.Features.Shell.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private object? _currentViewModel;
    private string _currentTitle = "Dashboard";
    private string _statusMessage = "Listo.";

    public MainWindowViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        _navigationService.CurrentViewModelChanged += viewModel => CurrentViewModel = viewModel;

        NavigateDashboardCommand = new RelayCommand(_ => NavigateTo<DashboardViewModel>());
        NavigateWorkflowsCommand = new RelayCommand(_ => NavigateTo<WorkflowListViewModel>());
        NavigateLogsCommand = new RelayCommand(_ => NavigateTo<ExecutionLogsViewModel>());

        NavigationItems =
        [
            new NavigationItem("Dashboard", "D", "Principal", NavigateDashboardCommand),
            new NavigationItem("Workflows", "W", "Automatizacion", NavigateWorkflowsCommand),
            new NavigationItem("Historial", "H", "Automatizacion", NavigateLogsCommand)
        ];

        NavigateTo<DashboardViewModel>();
    }

    public ObservableCollection<NavigationItem> NavigationItems { get; }

    public ICommand NavigateDashboardCommand { get; }
    public ICommand NavigateWorkflowsCommand { get; }
    public ICommand NavigateLogsCommand { get; }

    public string CurrentTitle
    {
        get => _currentTitle;
        private set => SetProperty(ref _currentTitle, value);
    }

    public object? CurrentViewModel
    {
        get => _currentViewModel;
        private set => SetProperty(ref _currentViewModel, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    private void NavigateTo<TViewModel>() where TViewModel : class
    {
        _navigationService.NavigateTo<TViewModel>();
        var title = typeof(TViewModel).Name.Replace("ViewModel", string.Empty);
        CurrentTitle = title;
        StatusMessage = $"Vista actual: {title}.";

        foreach (var item in NavigationItems)
        {
            item.IsActive = IsActiveItem<TViewModel>(item.Label);
        }
    }

    private static bool IsActiveItem<TViewModel>(string label)
    {
        var viewModelName = typeof(TViewModel).Name;
        return label switch
        {
            "Dashboard" => viewModelName.Contains("Dashboard", StringComparison.OrdinalIgnoreCase),
            "Workflows" => viewModelName.Contains("Workflow", StringComparison.OrdinalIgnoreCase),
            "Historial" => viewModelName.Contains("ExecutionLog", StringComparison.OrdinalIgnoreCase),
            _ => false
        };
    }
}
