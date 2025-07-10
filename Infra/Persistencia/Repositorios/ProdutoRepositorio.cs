using Aplicacao.Interfaces;
using Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Infra.Persistencia.Repositorios
{
    public class ProdutoRepositorio : IProdutoRepositorio
    {
        private readonly DbContext _contexto;

        // O DbContext específico (ServidorDbContext ou PdvDbContext)
        // será injetado aqui pelo contêiner de Injeção de Dependência
        // de cada aplicação (Servidor ou PDV).
        public ProdutoRepositorio(DbContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<Produto> ObterPorIdAsync(Guid id)
        {
            return await _contexto.Set<Produto>().FindAsync(id);
        }

        public async Task<IEnumerable<Produto>> ObterTodosAsync()
        {
            return await _contexto.Set<Produto>().ToListAsync();
        }

        public async Task AdicionarAsync(Produto produto)
        {
            await _contexto.Set<Produto>().AddAsync(produto);
            await _contexto.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Produto produto)
        {
            _contexto.Set<Produto>().Update(produto);
            await _contexto.SaveChangesAsync();
        }
    }
}