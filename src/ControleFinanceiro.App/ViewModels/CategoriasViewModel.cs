using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ControleFinanceiro.App.Services;
using ControleFinanceiro.App.Views;
using ControleFinanceiro.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.App.ViewModels;

public partial class CategoriasViewModel : ObservableObject
{
    private readonly AppDbContextFactory _dbContextFactory;

    [ObservableProperty]
    private ObservableCollection<Categoria> _categorias = new();

    [ObservableProperty]
    private Categoria? _categoriaSelecionada;

    public bool HouveAlteracoes { get; private set; }

    public CategoriasViewModel(AppDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task CarregarAsync()
    {
        using var context = _dbContextFactory.CreateContext();
        var lista = await context.Categorias.AsNoTracking().OrderBy(c => c.Nome).ToListAsync();
        Categorias = new ObservableCollection<Categoria>(lista);
    }

    [RelayCommand]
    private async Task Adicionar()
    {
        var editViewModel = new CategoriaEditViewModel();
        var dialog = new CategoriaDialog(editViewModel) { Owner = Application.Current.MainWindow };

        if (dialog.ShowDialog() == true)
        {
            using var context = _dbContextFactory.CreateContext();
            var categoria = new Categoria();
            editViewModel.AplicarEm(categoria);
            context.Categorias.Add(categoria);
            await context.SaveChangesAsync();
            HouveAlteracoes = true;
            await CarregarAsync();
        }
    }

    [RelayCommand]
    private async Task Editar()
    {
        if (CategoriaSelecionada is null)
        {
            return;
        }

        var editViewModel = new CategoriaEditViewModel(CategoriaSelecionada);
        var dialog = new CategoriaDialog(editViewModel) { Owner = Application.Current.MainWindow };

        if (dialog.ShowDialog() == true)
        {
            using var context = _dbContextFactory.CreateContext();
            var categoria = await context.Categorias.FindAsync(editViewModel.CategoriaId);
            if (categoria is not null)
            {
                editViewModel.AplicarEm(categoria);
                await context.SaveChangesAsync();
                HouveAlteracoes = true;
                await CarregarAsync();
            }
        }
    }

    [RelayCommand]
    private async Task Excluir()
    {
        if (CategoriaSelecionada is null)
        {
            return;
        }

        using var context = _dbContextFactory.CreateContext();

        var emUso = await context.Transacoes.AnyAsync(t => t.CategoriaId == CategoriaSelecionada.Id);
        if (emUso)
        {
            MessageBox.Show(
                $"A categoria \"{CategoriaSelecionada.Nome}\" está em uso por lançamentos e não pode ser excluída.",
                "Categoria em uso",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        var resultado = MessageBox.Show(
            $"Excluir a categoria \"{CategoriaSelecionada.Nome}\"?",
            "Confirmar exclusão",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (resultado != MessageBoxResult.Yes)
        {
            return;
        }

        var categoria = await context.Categorias.FindAsync(CategoriaSelecionada.Id);
        if (categoria is not null)
        {
            context.Categorias.Remove(categoria);
            await context.SaveChangesAsync();
            HouveAlteracoes = true;
            await CarregarAsync();
        }
    }
}
