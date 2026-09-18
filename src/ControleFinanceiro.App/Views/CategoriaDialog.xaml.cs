using System.Windows;
using ControleFinanceiro.App.ViewModels;
using ControleFinanceiro.Data.Models;

namespace ControleFinanceiro.App.Views;

public partial class CategoriaDialog : Window
{
    private readonly CategoriaEditViewModel _viewModel;

    public CategoriaDialog(CategoriaEditViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;

        TextNome.Text = _viewModel.Nome;

        if (_viewModel.Tipo == TipoTransacao.Receita)
        {
            RadioReceita.IsChecked = true;
        }
        else
        {
            RadioDespesa.IsChecked = true;
        }
    }

    private void Salvar_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.Nome = TextNome.Text;
        _viewModel.Tipo = RadioReceita.IsChecked == true ? TipoTransacao.Receita : TipoTransacao.Despesa;

        if (!_viewModel.Validar(out var erro))
        {
            MessageBox.Show(erro, "Dados inválidos", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        DialogResult = true;
    }

    private void Cancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
