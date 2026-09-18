using System.ComponentModel.DataAnnotations;

namespace ControleFinanceiro.Data.Models;

public class Conta
{
    public int Id { get; set; }

    [Required, MaxLength(60)]
    public string Nome { get; set; } = string.Empty;

    public TipoConta Tipo { get; set; }

    public decimal SaldoInicial { get; set; }

    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
}
