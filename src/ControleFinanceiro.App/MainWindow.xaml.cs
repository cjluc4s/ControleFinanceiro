using System.Windows;
using ControleFinanceiro.App.ViewModels;

namespace ControleFinanceiro.App;

public partial class MainWindow : Window
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
