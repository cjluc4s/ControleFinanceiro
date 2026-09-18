using System.Windows.Input;
using ControleFinanceiro.App.ViewModels;
using Wpf.Ui.Controls;

namespace ControleFinanceiro.App;

public partial class MainWindow : FluentWindow
{
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();

        _viewModel = new MainViewModel(App.DbContextFactory);
        DataContext = _viewModel;

        Loaded += async (_, _) => await _viewModel.CarregarAsync();
    }

    private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Delete && _viewModel.ExcluirCommand.CanExecute(null))
        {
            e.Handled = true;
            _viewModel.ExcluirCommand.Execute(null);
        }
    }
}
