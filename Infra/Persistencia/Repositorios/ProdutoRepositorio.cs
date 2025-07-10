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
            return await _contexto.Set<Produto>()
                .Where(p => p.IsAtivo)
                .ToListAsync();
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

        public async Task DeletarLogicamenteAsync(Guid id)
        {
            var produto = await _contexto.Set<Produto>().FindAsync(id);
            if (produto != null)
            {
                produto.IsAtivo = false;
                produto.AtualizadoEmUTC = DateTime.UtcNow;
                _contexto.Set<Produto>().Update(produto);
                await _contexto.SaveChangesAsync();
            }
        }

        public async Task<Produto> ObterPorCodigoBarrasAsync(string codigo)
        {
            return await _contexto.Set<Produto>()
                .FirstOrDefaultAsync(p => p.CodigoBarras == codigo && p.IsAtivo);
        }

        public async Task SincronizarCatalogoAsync(IEnumerable<Produto> produtosDoServidor)
        {
            var produtosLocais = await _contexto.Set<Produto>().ToListAsync();

            // Adiciona ou Atualiza produtos
            foreach (var produtoServidor in produtosDoServidor)
            {
                var produtoLocal = produtosLocais.FirstOrDefault(p => p.Id == produtoServidor.Id);
                if (produtoLocal == null)
                {
                    // Produto novo, adiciona
                    await _contexto.Set<Produto>().AddAsync(produtoServidor);
                }
                else
                {
                    // Produto existente, atualiza
                    produtoLocal.Descricao = produtoServidor.Descricao;
                    produtoLocal.PrecoVenda = produtoServidor.PrecoVenda;
                    produtoLocal.CodigoBarras = produtoServidor.CodigoBarras;
                    produtoLocal.EstoqueAtual = produtoServidor.EstoqueAtual; // Ou outra regra de estoque
                    produtoLocal.IsAtivo = produtoServidor.IsAtivo;
                    produtoLocal.AtualizadoEmUTC = produtoServidor.AtualizadoEmUTC;
                    _contexto.Set<Produto>().Update(produtoLocal);
                }
            }

            // Desativa ou Deleta produtos que não existem mais no servidor
            var idsProdutosServidor = produtosDoServidor.Select(p => p.Id).ToList();
            var produtosParaRemover = produtosLocais.Where(p => !idsProdutosServidor.Contains(p.Id)).ToList();
            if (produtosParaRemover.Any())
            {
                _contexto.Set<Produto>().RemoveRange(produtosParaRemover);
            }

            await _contexto.SaveChangesAsync();
        }
    }
}