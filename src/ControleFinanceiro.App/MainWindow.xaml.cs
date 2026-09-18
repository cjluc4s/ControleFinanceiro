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
}
