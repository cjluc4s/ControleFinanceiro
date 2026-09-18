using ControleFinanceiro.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Data;

public class ControleFinanceiroContext : DbContext
{
    public ControleFinanceiroContext(DbContextOptions<ControleFinanceiroContext> options)
        : base(options)
    {
    }

    public DbSet<Categoria> Categorias => Set<Categoria>();

    public DbSet<Transacao> Transacoes => Set<Transacao>();

    public DbSet<Conta> Contas => Set<Conta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transacao>()
            .Property(t => t.Valor)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Conta>()
            .Property(c => c.SaldoInicial)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Conta>().HasData(
            new Conta { Id = 1, Nome = "Conta Principal", Tipo = TipoConta.Corrente, SaldoInicial = 0 }
        );

        modelBuilder.Entity<Categoria>().HasData(
            new Categoria { Id = 1, Nome = "Salário", Tipo = TipoTransacao.Receita },
            new Categoria { Id = 2, Nome = "Outras receitas", Tipo = TipoTransacao.Receita },
            new Categoria { Id = 3, Nome = "Moradia", Tipo = TipoTransacao.Despesa },
            new Categoria { Id = 4, Nome = "Alimentação", Tipo = TipoTransacao.Despesa },
            new Categoria { Id = 5, Nome = "Transporte", Tipo = TipoTransacao.Despesa },
            new Categoria { Id = 6, Nome = "Lazer", Tipo = TipoTransacao.Despesa },
            new Categoria { Id = 7, Nome = "Saúde", Tipo = TipoTransacao.Despesa },
            new Categoria { Id = 8, Nome = "Outras despesas", Tipo = TipoTransacao.Despesa }
        );
    }
}
