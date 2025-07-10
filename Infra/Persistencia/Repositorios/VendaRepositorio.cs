// Substitua o VendaRepositorio.cs anterior por este:
using Aplicacao.Interfaces;
using Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Infra.Persistencia.Repositorios
{
    public class VendaRepositorio : IVendaRepositorio
    {
        private readonly DbContext _contexto;

        public VendaRepositorio(DbContext contexto)
        {
            _contexto = contexto;
        }

        public async Task AdicionarVendaAsync(Venda venda)
        {
            // Ao adicionar no PDV, a propriedade já vem como true.
            await _contexto.Set<Venda>().AddAsync(venda);
            await _contexto.SaveChangesAsync();
        }

        public async Task<IEnumerable<Venda>> ObterVendasNaoSincronizadasAsync()
        {
            // Esta consulta só funcionará corretamente no PdvDbContext,
            // pois o `PrecisaSincronizar` será mapeado para a coluna do SQLite.
            // Para isso, precisamos configurar o PdvDbContext.

            // Simulação da consulta - a configuração final será no DbContext.
            return await _contexto.Set<Venda>()
                .Where(v => v.PrecisaSincronizar)
                .Include(v => v.Itens) // Inclui os itens para enviar ao servidor
                .ToListAsync();
        }

        public async Task MarcarVendasComoSincronizadasAsync(IEnumerable<Guid> idsDasVendas)
        {
            var vendasParaAtualizar = await _contexto.Set<Venda>()
                .Where(v => idsDasVendas.Contains(v.Id))
                .ToListAsync();

            foreach (var venda in vendasParaAtualizar)
            {
                venda.PrecisaSincronizar = false;
            }

            _contexto.Set<Venda>().UpdateRange(vendasParaAtualizar);
            await _contexto.SaveChangesAsync();
        }
    }
}