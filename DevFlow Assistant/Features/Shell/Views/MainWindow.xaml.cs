using System.Windows;
using DevFlow_Assistant.Features.Shell.ViewModels;

namespace DevFlow_Assistant
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
