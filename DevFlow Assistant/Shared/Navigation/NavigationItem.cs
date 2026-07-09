using System.Windows.Input;
using DevFlow_Assistant.Shared;

namespace DevFlow_Assistant.Shared.Navigation;

public sealed class NavigationItem : ViewModelBase
{
    private bool _isActive;

    public NavigationItem(string label, string icon, string group, ICommand command)
    {
        Label = label;
        Icon = icon;
        Group = group;
        Command = command;
    }

    public string Label { get; }
    public string Icon { get; }
    public string Group { get; }
    public ICommand Command { get; }

    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }
}
