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
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }
    }
}