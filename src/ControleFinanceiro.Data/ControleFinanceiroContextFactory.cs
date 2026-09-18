using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ControleFinanceiro.Data;

public class ControleFinanceiroContextFactory : IDesignTimeDbContextFactory<ControleFinanceiroContext>
{
    public ControleFinanceiroContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ControleFinanceiroContext>();
        optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=ControleFinanceiro;Trusted_Connection=True;TrustServerCertificate=True;");

        return new ControleFinanceiroContext(optionsBuilder.Options);
    }
}
