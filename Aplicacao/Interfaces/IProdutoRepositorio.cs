using Dominio.Entidades;

namespace Aplicacao.Interfaces
{
    public interface IProdutoRepositorio
    {
        Task<Produto> ObterPorIdAsync(Guid id);
        Task<IEnumerable<Produto>> ObterTodosAsync();
        Task AdicionarAsync(Produto produto);
        Task AtualizarAsync(Produto produto);
        Task DeletarLogicamenteAsync(Guid id);
        Task<Produto> ObterPorCodigoBarrasAsync(string codigo);
        Task SincronizarCatalogoAsync(IEnumerable<Produto> produtosDoServidor);
    }
}