using Microsoft.EntityFrameworkCore;
using Dominio.Entidades;

namespace Infra.Persistencia.DbContexts
{
    // Contexto específico para o Servidor, usando PostgreSQL.
    public class ServidorDbContext : DbContext
    {
        public ServidorDbContext(DbContextOptions<ServidorDbContext> options) : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }

        // CORRETO PARA O SERVIDOR:
        public DbSet<Venda> Vendas { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapeia as entidades para os nomes de tabela exatos que criamos no PostgreSQL
            modelBuilder.Entity<Produto>().ToTable("Produtos");
            modelBuilder.Entity<Venda>().ToTable("Vendas");
            modelBuilder.Entity<ItemVenda>().ToTable("ItensVenda");
        }

    }
}