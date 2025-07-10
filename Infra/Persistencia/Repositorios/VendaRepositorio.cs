using Aplicacao.Interfaces;
using Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infra.Persistencia.Repositorios
{
    public class VendaRepositorio : IVendaRepositorio
    {
        private readonly DbContext _contexto;

        public VendaRepositorio(DbContext contexto)
        {
            _contexto = contexto;
        }

        // Funciona para ambos os contextos, pois VendaPdv herda de Venda.
        public async Task AdicionarVendaAsync(Venda venda)
        {
            // O EF Core sabe se o objeto é do tipo Venda ou VendaPdv e salva na tabela correta.
            await _contexto.Set<Venda>().AddAsync(venda);
            await _contexto.SaveChangesAsync();
        }

        // Este método é chamado pelo ServidorDbContext.
        // O Set<Venda>() aqui vai consultar a tabela "Vendas" do PostgreSQL.
        public async Task<IEnumerable<Venda>> ObterTodasAsVendasAsync()
        {
            return await _contexto.Set<Venda>()
                .OrderByDescending(v => v.MomentoDaVendaUTC)
                .AsNoTracking()
                .ToListAsync();
        }

        // Este método é chamado pelo PdvDbContext.
        // O Set<VendaPdv>() aqui vai consultar a tabela "Vendas" do SQLite.
        public async Task<IEnumerable<Venda>> ObterVendasNaoSincronizadasAsync()
        {
            return await _contexto.Set<VendaPdv>()
            .Where(v => v.PrecisaSincronizar)
            .Include(v => v.Itens)
            .AsNoTracking()
            .ToListAsync();
        }

        public async Task MarcarVendasComoSincronizadasAsync(IEnumerable<Guid> idsDasVendas)
        {
            var vendasParaAtualizar = await _contexto.Set<VendaPdv>()
                .Where(v => idsDasVendas.Contains(v.Id))
                .ToListAsync();

            foreach (var venda in vendasParaAtualizar)
            {
                venda.PrecisaSincronizar = false;
            }

            _contexto.UpdateRange(vendasParaAtualizar);
            await _contexto.SaveChangesAsync();
        }
    }
}