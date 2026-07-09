namespace DevFlow_Assistant.Shared.Navigation;

public interface INavigationService
{
    event Action<object>? CurrentViewModelChanged;
    object CurrentViewModel { get; }
    void NavigateTo<TViewModel>() where TViewModel : class;
    void NavigateTo<TViewModel>(Action<TViewModel> configure) where TViewModel : class;
}
