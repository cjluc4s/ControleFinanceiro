using ControleFinanceiro.Data.Models;

namespace ControleFinanceiro.App.ViewModels;

public class ContaResumo
{
    public required Conta Conta { get; init; }

    public decimal SaldoAtual { get; init; }

    public int Id => Conta.Id;

    public string Nome => Conta.Nome;

    public TipoConta Tipo => Conta.Tipo;

    public decimal SaldoInicial => Conta.SaldoInicial;
}
