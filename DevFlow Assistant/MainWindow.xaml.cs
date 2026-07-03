using DevFlowAssistant.Application.Services.implementation;
using DevFlowAssistant.Application.Services.Interface;
using DevFlowAssistant.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DevFlow_Assistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AppDbContext _context;
        private readonly IWorkflowService _service;

        public MainWindow(IWorkflowService service)
        {
            InitializeComponent();
            _service = service;
            Loaded += MainWindow_loaded;
        }


        private async void MainWindow_loaded(object sender, RoutedEventArgs e)
        {
            await LoadWorkflowsAsync();

        }

        private async Task LoadWorkflowsAsync()
        {
            var workflows = await _service.GetAllAsync();
            WorkflowsList.ItemsSource = workflows;
        }

        private async void CreateWorkflow_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var name = WorkflowNameTextBox.Text;
                var descripcion = WorkflowDescriptionTextBox.Text;

                await _service.CreateAsync(name, descripcion);


                WorkflowDescriptionTextBox.Clear();
                WorkflowNameTextBox.Clear();


                await LoadWorkflowsAsync();

                MessageBox.Show("Workflow creado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}