using CommunityToolkit.Mvvm.ComponentModel;
using ControleFinanceiro.Data.Models;

namespace ControleFinanceiro.App.ViewModels;

public partial class ContaEditViewModel : ObservableObject
{
    [ObservableProperty]
    private string _nome = string.Empty;

    [ObservableProperty]
    private TipoConta _tipo = TipoConta.Corrente;

    [ObservableProperty]
    private decimal _saldoInicial;

    public int? ContaId { get; private set; }

    public ContaEditViewModel(Conta? contaExistente = null)
    {
        if (contaExistente is not null)
        {
            ContaId = contaExistente.Id;
            Nome = contaExistente.Nome;
            Tipo = contaExistente.Tipo;
            SaldoInicial = contaExistente.SaldoInicial;
        }
    }

    public bool Validar(out string erro)
    {
        if (string.IsNullOrWhiteSpace(Nome))
        {
            erro = "Informe um nome para a conta.";
            return false;
        }

        erro = string.Empty;
        return true;
    }

    public void AplicarEm(Conta conta)
    {
        conta.Nome = Nome.Trim();
        conta.Tipo = Tipo;
        conta.SaldoInicial = SaldoInicial;
    }
}
