using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ControleFinanceiro.App.Services;
using ControleFinanceiro.App.Views;
using ControleFinanceiro.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.App.ViewModels;

public partial class ContasViewModel : ObservableObject
{
    private readonly AppDbContextFactory _dbContextFactory;

    [ObservableProperty]
    private ObservableCollection<ContaResumo> _contas = new();

    [ObservableProperty]
    private ContaResumo? _contaSelecionada;

    public bool HouveAlteracoes { get; private set; }

    public ContasViewModel(AppDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task CarregarAsync()
    {
        using var context = _dbContextFactory.CreateContext();

        var contas = await context.Contas.AsNoTracking().OrderBy(c => c.Nome).ToListAsync();
        var transacoes = await context.Transacoes.AsNoTracking().ToListAsync();

        var resumos = contas.Select(conta =>
        {
            var receitas = transacoes.Where(t => t.ContaId == conta.Id && t.Tipo == TipoTransacao.Receita).Sum(t => t.Valor);
            var despesas = transacoes.Where(t => t.ContaId == conta.Id && t.Tipo == TipoTransacao.Despesa).Sum(t => t.Valor);
            return new ContaResumo { Conta = conta, SaldoAtual = conta.SaldoInicial + receitas - despesas };
        });

        Contas = new ObservableCollection<ContaResumo>(resumos);
    }

    [RelayCommand]
    private async Task Adicionar()
    {
        var editViewModel = new ContaEditViewModel();
        var dialog = new ContaDialog(editViewModel) { Owner = Application.Current.MainWindow };

        if (dialog.ShowDialog() == true)
        {
            using var context = _dbContextFactory.CreateContext();
            var conta = new Conta();
            editViewModel.AplicarEm(conta);
            context.Contas.Add(conta);
            await context.SaveChangesAsync();
            HouveAlteracoes = true;
            await CarregarAsync();
        }
    }

    [RelayCommand]
    private async Task Editar()
    {
        if (ContaSelecionada is null)
        {
            return;
        }

        var editViewModel = new ContaEditViewModel(ContaSelecionada.Conta);
        var dialog = new ContaDialog(editViewModel) { Owner = Application.Current.MainWindow };

        if (dialog.ShowDialog() == true)
        {
            using var context = _dbContextFactory.CreateContext();
            var conta = await context.Contas.FindAsync(editViewModel.ContaId);
            if (conta is not null)
            {
                editViewModel.AplicarEm(conta);
                await context.SaveChangesAsync();
                HouveAlteracoes = true;
                await CarregarAsync();
            }
        }
    }

    [RelayCommand]
    private async Task Excluir()
    {
        if (ContaSelecionada is null)
        {
            return;
        }

        using var context = _dbContextFactory.CreateContext();

        if (await context.Contas.CountAsync() <= 1)
        {
            MessageBox.Show(
                "É necessário manter ao menos uma conta cadastrada.",
                "Não é possível excluir",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        var emUso = await context.Transacoes.AnyAsync(t => t.ContaId == ContaSelecionada.Id);
        if (emUso)
        {
            MessageBox.Show(
                $"A conta \"{ContaSelecionada.Nome}\" está em uso por lançamentos e não pode ser excluída.",
                "Conta em uso",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        var resultado = MessageBox.Show(
            $"Excluir a conta \"{ContaSelecionada.Nome}\"?",
            "Confirmar exclusão",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (resultado != MessageBoxResult.Yes)
        {
            return;
        }

        var conta = await context.Contas.FindAsync(ContaSelecionada.Id);
        if (conta is not null)
        {
            context.Contas.Remove(conta);
            await context.SaveChangesAsync();
            HouveAlteracoes = true;
            await CarregarAsync();
        }
    }
}
