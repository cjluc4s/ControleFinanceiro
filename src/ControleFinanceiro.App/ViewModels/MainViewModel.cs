using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ControleFinanceiro.App.Services;
using ControleFinanceiro.App.Views;
using ControleFinanceiro.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.App.ViewModels;

public record FiltroItem(int? Valor, string Texto)
{
    public override string ToString() => Texto;
}

public partial class MainViewModel : ObservableObject
{
    private static readonly string[] NomesMeses =
    {
        "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
        "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"
    };

    private readonly AppDbContextFactory _dbContextFactory;
    private List<Categoria> _categorias = new();
    private List<Conta> _contas = new();

    [ObservableProperty]
    private ObservableCollection<Transacao> _transacoes = new();

    [ObservableProperty]
    private Transacao? _transacaoSelecionada;

    [ObservableProperty]
    private decimal _totalReceitas;

    [ObservableProperty]
    private decimal _totalDespesas;

    [ObservableProperty]
    private decimal _saldo;

    [ObservableProperty]
    private decimal _patrimonioTotal;

    [ObservableProperty]
    private ObservableCollection<FiltroItem> _mesesDisponiveis;

    [ObservableProperty]
    private ObservableCollection<FiltroItem> _anosDisponiveis = new();

    [ObservableProperty]
    private FiltroItem _mesSelecionado;

    [ObservableProperty]
    private FiltroItem _anoSelecionado;

    public MainViewModel(AppDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;

        _mesesDisponiveis = new ObservableCollection<FiltroItem>(
            new[] { new FiltroItem(null, "Todos os meses") }
                .Concat(Enumerable.Range(1, 12).Select(m => new FiltroItem(m, NomesMeses[m - 1]))));

        var hoje = DateTime.Today;
        _mesSelecionado = _mesesDisponiveis.First(m => m.Valor == hoje.Month);
        _anoSelecionado = new FiltroItem(hoje.Year, hoje.Year.ToString());
    }

    partial void OnMesSelecionadoChanged(FiltroItem value)
    {
        if (value is not null)
        {
            _ = CarregarAsync();
        }
    }

    partial void OnAnoSelecionadoChanged(FiltroItem value)
    {
        if (value is not null)
        {
            _ = CarregarAsync();
        }
    }

    public async Task CarregarAsync()
    {
        using var context = _dbContextFactory.CreateContext();

        _categorias = await context.Categorias.AsNoTracking().OrderBy(c => c.Nome).ToListAsync();
        _contas = await context.Contas.AsNoTracking().OrderBy(c => c.Nome).ToListAsync();

        var anosComLancamentos = await context.Transacoes.AsNoTracking()
            .Select(t => t.Data.Year)
            .Distinct()
            .ToListAsync();

        var anos = anosComLancamentos.Append(DateTime.Today.Year).Distinct().OrderDescending();
        AnosDisponiveis = new ObservableCollection<FiltroItem>(
            new[] { new FiltroItem(null, "Todos os anos") }
                .Concat(anos.Select(a => new FiltroItem(a, a.ToString()))));

        var query = context.Transacoes
            .Include(t => t.Categoria)
            .Include(t => t.Conta)
            .AsNoTracking()
            .AsQueryable();

        if (MesSelecionado.Valor is { } mes)
        {
            query = query.Where(t => t.Data.Month == mes);
        }

        if (AnoSelecionado.Valor is { } ano)
        {
            query = query.Where(t => t.Data.Year == ano);
        }

        var lista = await query.OrderByDescending(t => t.Data).ToListAsync();

        Transacoes = new ObservableCollection<Transacao>(lista);
        AtualizarTotais();

        var somaSaldoInicial = await context.Contas.SumAsync(c => c.SaldoInicial);
        var somaReceitasTotal = await context.Transacoes.Where(t => t.Tipo == TipoTransacao.Receita).SumAsync(t => t.Valor);
        var somaDespesasTotal = await context.Transacoes.Where(t => t.Tipo == TipoTransacao.Despesa).SumAsync(t => t.Valor);
        PatrimonioTotal = somaSaldoInicial + somaReceitasTotal - somaDespesasTotal;
    }

    private void AtualizarTotais()
    {
        TotalReceitas = Transacoes.Where(t => t.Tipo == TipoTransacao.Receita).Sum(t => t.Valor);
        TotalDespesas = Transacoes.Where(t => t.Tipo == TipoTransacao.Despesa).Sum(t => t.Valor);
        Saldo = TotalReceitas - TotalDespesas;
    }

    [RelayCommand]
    private async Task Adicionar()
    {
        var editViewModel = new TransacaoEditViewModel(_categorias, _contas);
        var dialog = new TransacaoDialog(editViewModel) { Owner = Application.Current.MainWindow };

        if (dialog.ShowDialog() == true)
        {
            using var context = _dbContextFactory.CreateContext();
            var transacao = new Transacao();
            editViewModel.AplicarEm(transacao);
            context.Transacoes.Add(transacao);
            await context.SaveChangesAsync();
            await CarregarAsync();
        }
    }

    [RelayCommand]
    private async Task Editar()
    {
        if (TransacaoSelecionada is null)
        {
            return;
        }

        var editViewModel = new TransacaoEditViewModel(_categorias, _contas, TransacaoSelecionada);
        var dialog = new TransacaoDialog(editViewModel) { Owner = Application.Current.MainWindow };

        if (dialog.ShowDialog() == true)
        {
            using var context = _dbContextFactory.CreateContext();
            var transacao = await context.Transacoes.FindAsync(editViewModel.TransacaoId);
            if (transacao is not null)
            {
                editViewModel.AplicarEm(transacao);
                await context.SaveChangesAsync();
                await CarregarAsync();
            }
        }
    }

    [RelayCommand]
    private async Task Excluir()
    {
        if (TransacaoSelecionada is null)
        {
            return;
        }

        var resultado = MessageBox.Show(
            $"Excluir o lançamento \"{TransacaoSelecionada.Descricao}\"?",
            "Confirmar exclusão",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (resultado != MessageBoxResult.Yes)
        {
            return;
        }

        using var context = _dbContextFactory.CreateContext();
        var transacao = await context.Transacoes.FindAsync(TransacaoSelecionada.Id);
        if (transacao is not null)
        {
            context.Transacoes.Remove(transacao);
            await context.SaveChangesAsync();
            await CarregarAsync();
        }
    }

    [RelayCommand]
    private async Task GerenciarCategorias()
    {
        var viewModel = new CategoriasViewModel(_dbContextFactory);
        var window = new CategoriasWindow(viewModel) { Owner = Application.Current.MainWindow };
        window.ShowDialog();

        if (viewModel.HouveAlteracoes)
        {
            await CarregarAsync();
        }
    }

    [RelayCommand]
    private async Task GerenciarContas()
    {
        var viewModel = new ContasViewModel(_dbContextFactory);
        var window = new ContasWindow(viewModel) { Owner = Application.Current.MainWindow };
        window.ShowDialog();

        if (viewModel.HouveAlteracoes)
        {
            await CarregarAsync();
        }
    }
}
