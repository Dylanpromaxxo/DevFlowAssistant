using Microsoft.Extensions.DependencyInjection;

namespace DevFlow_Assistant.Shared.Navigation;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private object? _currentViewModel;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public event Action<object>? CurrentViewModelChanged;

    public object CurrentViewModel => _currentViewModel ?? throw new InvalidOperationException("No hay una vista activa.");

    public void NavigateTo<TViewModel>() where TViewModel : class
    {
        var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
        SetCurrent(viewModel);
    }

    public void NavigateTo<TViewModel>(Action<TViewModel> configure) where TViewModel : class
    {
        var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
        configure(viewModel);
        SetCurrent(viewModel);
    }

    private void SetCurrent(object viewModel)
    {
        _currentViewModel = viewModel;
        CurrentViewModelChanged?.Invoke(viewModel);
    }
}
