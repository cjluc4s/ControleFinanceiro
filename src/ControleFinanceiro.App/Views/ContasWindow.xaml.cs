using System.Windows;
using ControleFinanceiro.App.ViewModels;

namespace ControleFinanceiro.App.Views;

public partial class ContasWindow : Wpf.Ui.Controls.FluentWindow
{
    public ContasViewModel ViewModel { get; }

    public ContasWindow(ContasViewModel viewModel)
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
