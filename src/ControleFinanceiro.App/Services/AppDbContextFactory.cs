using ControleFinanceiro.Data;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.App.Services;

public class AppDbContextFactory
{
    private readonly DbContextOptions<ControleFinanceiroContext> _options;

    public AppDbContextFactory(string connectionString)
    {
        _options = new DbContextOptionsBuilder<ControleFinanceiroContext>()
            .UseSqlServer(connectionString)
            .Options;
    }

    public ControleFinanceiroContext CreateContext() => new(_options);
}
