using Dominio.Entidades;

namespace Aplicacao.Interfaces
{
    public interface IVendaRepositorio
    {
        Task AdicionarVendaAsync(Venda venda);
        // Exemplo para o PDV: buscar vendas que precisam ser sincronizadas
        Task<IEnumerable<Venda>> ObterVendasNaoSincronizadasAsync();
        Task MarcarVendasComoSincronizadasAsync(IEnumerable<Guid> idsDasVendas);
    }
}