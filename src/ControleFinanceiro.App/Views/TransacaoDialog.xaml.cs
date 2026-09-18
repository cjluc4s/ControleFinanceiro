using System.Globalization;
using System.Windows;
using ControleFinanceiro.App.ViewModels;
using ControleFinanceiro.Data.Models;

namespace ControleFinanceiro.App.Views;

public partial class TransacaoDialog : Window
{
    private readonly TransacaoEditViewModel _viewModel;

    public TransacaoDialog(TransacaoEditViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;

        TextDescricao.Text = _viewModel.Descricao;
        TextValor.Text = _viewModel.Valor == 0 ? string.Empty : _viewModel.Valor.ToString(CultureInfo.CurrentCulture);
        DatePickerData.SelectedDate = _viewModel.Data;

        if (_viewModel.Tipo == TipoTransacao.Receita)
        {
            RadioReceita.IsChecked = true;
        }
        else
        {
            RadioDespesa.IsChecked = true;
        }

        AtualizarCategorias();
        ComboCategoria.SelectedItem = _viewModel.CategoriaSelecionada;

        ComboConta.ItemsSource = _viewModel.ContasDisponiveis;
        ComboConta.SelectedItem = _viewModel.ContaSelecionada;
    }

    private void TipoAlterado(object sender, RoutedEventArgs e)
    {
        AtualizarCategorias();
    }

    private void AtualizarCategorias()
    {
        if (ComboCategoria is null)
        {
            return;
        }

        var tipoSelecionado = RadioReceita.IsChecked == true ? TipoTransacao.Receita : TipoTransacao.Despesa;
        ComboCategoria.ItemsSource = _viewModel.CategoriasDisponiveis
            .Where(c => c.Tipo == tipoSelecionado)
            .ToList();
    }

    private void Salvar_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.Descricao = TextDescricao.Text;
        _viewModel.Data = DatePickerData.SelectedDate ?? DateTime.Today;
        _viewModel.Tipo = RadioReceita.IsChecked == true ? TipoTransacao.Receita : TipoTransacao.Despesa;
        _viewModel.CategoriaSelecionada = ComboCategoria.SelectedItem as Categoria;
        _viewModel.ContaSelecionada = ComboConta.SelectedItem as Conta;

        if (!decimal.TryParse(TextValor.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var valor))
        {
            MessageBox.Show("Informe um valor numérico válido.", "Valor inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _viewModel.Valor = valor;

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
