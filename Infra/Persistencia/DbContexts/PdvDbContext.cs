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
        public DbSet<ItemVenda> ItensVenda { get; set; }

        // CORRETO PARA O PDV:
        public DbSet<VendaPdv> Vendas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapeia as entidades para os nomes de tabela exatos que criamos no SQLite
            modelBuilder.Entity<Produto>().ToTable("Produtos");
            modelBuilder.Entity<VendaPdv>().ToTable("Vendas");
            modelBuilder.Entity<ItemVenda>().ToTable("ItensVenda");
            modelBuilder.Entity<Venda>().Ignore(v => v.SincronizadoEmUTC);
        }
    }
}