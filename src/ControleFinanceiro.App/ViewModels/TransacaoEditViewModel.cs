using CommunityToolkit.Mvvm.ComponentModel;
using ControleFinanceiro.Data.Models;

namespace ControleFinanceiro.App.ViewModels;

public partial class TransacaoEditViewModel : ObservableObject
{
    [ObservableProperty]
    private string _descricao = string.Empty;

    [ObservableProperty]
    private decimal _valor;

    [ObservableProperty]
    private DateTime _data = DateTime.Today;

    [ObservableProperty]
    private TipoTransacao _tipo = TipoTransacao.Despesa;

    [ObservableProperty]
    private Categoria? _categoriaSelecionada;

    [ObservableProperty]
    private Conta? _contaSelecionada;

    public List<Categoria> CategoriasDisponiveis { get; }

    public List<Conta> ContasDisponiveis { get; }

    public int? TransacaoId { get; private set; }

    public TransacaoEditViewModel(List<Categoria> todasCategorias, List<Conta> todasContas, Transacao? transacaoExistente = null)
    {
        CategoriasDisponiveis = todasCategorias;
        ContasDisponiveis = todasContas;

        if (transacaoExistente is not null)
        {
            TransacaoId = transacaoExistente.Id;
            Descricao = transacaoExistente.Descricao;
            Valor = transacaoExistente.Valor;
            Data = transacaoExistente.Data;
            Tipo = transacaoExistente.Tipo;
            CategoriaSelecionada = todasCategorias.FirstOrDefault(c => c.Id == transacaoExistente.CategoriaId);
            ContaSelecionada = todasContas.FirstOrDefault(c => c.Id == transacaoExistente.ContaId);
        }
        else
        {
            ContaSelecionada = todasContas.FirstOrDefault();
        }
    }

    public bool Validar(out string erro)
    {
        if (string.IsNullOrWhiteSpace(Descricao))
        {
            erro = "Informe uma descrição.";
            return false;
        }

        if (Valor <= 0)
        {
            erro = "O valor deve ser maior que zero.";
            return false;
        }

        if (CategoriaSelecionada is null)
        {
            erro = "Selecione uma categoria.";
            return false;
        }

        if (ContaSelecionada is null)
        {
            erro = "Selecione uma conta.";
            return false;
        }

        erro = string.Empty;
        return true;
    }

    public void AplicarEm(Transacao transacao)
    {
        transacao.Descricao = Descricao.Trim();
        transacao.Valor = Valor;
        transacao.Data = Data;
        transacao.Tipo = Tipo;
        transacao.CategoriaId = CategoriaSelecionada!.Id;
        transacao.ContaId = ContaSelecionada!.Id;
    }
}
