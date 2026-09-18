using System.Windows;
using System.Windows.Input;
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

    private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Delete && ViewModel.ExcluirCommand.CanExecute(null))
        {
            e.Handled = true;
            ViewModel.ExcluirCommand.Execute(null);
        }
    }

    private void Fechar_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
