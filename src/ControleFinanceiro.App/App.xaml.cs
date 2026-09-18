using System.Windows;
using ControleFinanceiro.App.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ControleFinanceiro.App;

public partial class App : Application
{
    public static AppDbContextFactory DbContextFactory { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' não encontrada em appsettings.json.");

        DbContextFactory = new AppDbContextFactory(connectionString);

        try
        {
            using var context = DbContextFactory.CreateContext();
            context.Database.Migrate();
        }
        catch (Exception ex) when (ex is SqlException or InvalidOperationException)
        {
            MessageBox.Show(
                $"Não foi possível conectar ao SQL Server.\n\nVerifique se a instância está instalada e se a connection string em appsettings.json está correta.\n\nDetalhes: {ex.Message}",
                "Erro de conexão",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown();
            return;
        }

        var mainWindow = new MainWindow();
        mainWindow.Show();
    }
}
