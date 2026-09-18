using System.Windows;
using ControleFinanceiro.App.ViewModels;

namespace ControleFinanceiro.App.Views;

public partial class CategoriasWindow : Wpf.Ui.Controls.FluentWindow
{
    public CategoriasViewModel ViewModel { get; }

    public CategoriasWindow(CategoriasViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = ViewModel;

        Loaded += async (_, _) => await ViewModel.CarregarAsync();
    }

    private void Fechar_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
