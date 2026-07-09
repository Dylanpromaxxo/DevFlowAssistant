using System.Windows.Input;

namespace DevFlow_Assistant.Shared.Navigation;

public sealed record NavigationItem(string Label, ICommand Command);
