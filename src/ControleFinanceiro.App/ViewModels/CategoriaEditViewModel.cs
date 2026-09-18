using CommunityToolkit.Mvvm.ComponentModel;
using ControleFinanceiro.Data.Models;

namespace ControleFinanceiro.App.ViewModels;

public partial class CategoriaEditViewModel : ObservableObject
{
    [ObservableProperty]
    private string _nome = string.Empty;

    [ObservableProperty]
    private TipoTransacao _tipo = TipoTransacao.Despesa;

    public int? CategoriaId { get; private set; }

    public CategoriaEditViewModel(Categoria? categoriaExistente = null)
    {
        if (categoriaExistente is not null)
        {
            CategoriaId = categoriaExistente.Id;
            Nome = categoriaExistente.Nome;
            Tipo = categoriaExistente.Tipo;
        }
    }

    public bool Validar(out string erro)
    {
        if (string.IsNullOrWhiteSpace(Nome))
        {
            erro = "Informe um nome para a categoria.";
            return false;
        }

        erro = string.Empty;
        return true;
    }

    public void AplicarEm(Categoria categoria)
    {
        categoria.Nome = Nome.Trim();
        categoria.Tipo = Tipo;
    }
}
