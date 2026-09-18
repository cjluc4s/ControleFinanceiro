using System.ComponentModel.DataAnnotations;

namespace ControleFinanceiro.Data.Models;

public class Transacao
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Descricao { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public DateTime Data { get; set; } = DateTime.Today;

    public TipoTransacao Tipo { get; set; }

    public int CategoriaId { get; set; }

    public Categoria? Categoria { get; set; }

    public int ContaId { get; set; }

    public Conta? Conta { get; set; }
}
