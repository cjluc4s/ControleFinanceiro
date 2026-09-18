using System.ComponentModel.DataAnnotations;

namespace ControleFinanceiro.Data.Models;

public class Categoria
{
    public int Id { get; set; }

    [Required, MaxLength(60)]
    public string Nome { get; set; } = string.Empty;

    public TipoTransacao Tipo { get; set; }

    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
}
