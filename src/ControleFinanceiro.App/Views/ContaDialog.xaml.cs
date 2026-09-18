using System.Globalization;
using System.Windows;
using ControleFinanceiro.App.ViewModels;
using ControleFinanceiro.Data.Models;

namespace ControleFinanceiro.App.Views;

public partial class ContaDialog : Window
{
    private readonly ContaEditViewModel _viewModel;

    public ContaDialog(ContaEditViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;

        TextNome.Text = _viewModel.Nome;
        TextSaldoInicial.Text = _viewModel.SaldoInicial.ToString(CultureInfo.CurrentCulture);

        ComboTipo.ItemsSource = Enum.GetValues<TipoConta>();
        ComboTipo.SelectedItem = _viewModel.Tipo;
    }

    private void Salvar_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.Nome = TextNome.Text;
        _viewModel.Tipo = (TipoConta)(ComboTipo.SelectedItem ?? TipoConta.Corrente);

        if (!decimal.TryParse(TextSaldoInicial.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var saldoInicial))
        {
            MessageBox.Show("Informe um saldo inicial numérico válido.", "Valor inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _viewModel.SaldoInicial = saldoInicial;

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
