using Microsoft.EntityFrameworkCore;
using Dominio.Entidades;

namespace Infra.Persistencia.DbContexts
{
    // Contexto específico para o PDV, usando SQLite.
    public class PdvDbContext : DbContext
    {
        public PdvDbContext(DbContextOptions<PdvDbContext> options) : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }

        // Nota: A configuração da string de conexão e do provedor (SQLite)
        // será feita no projeto de UI do LotusPDV, não aqui.
    }
}