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
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<Conversa> Conversas { get; set; }
        public DbSet<ChatHistorico> ChatHistorico { get; set; } // Adicionado o DbSet que faltava

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapeia as entidades para os nomes de tabela exatos no PostgreSQL
            modelBuilder.Entity<Produto>().ToTable("Produtos");
            modelBuilder.Entity<Venda>().ToTable("Vendas");
            modelBuilder.Entity<ItemVenda>().ToTable("ItensVenda");
            modelBuilder.Entity<Conversa>().ToTable("Conversas");
            modelBuilder.Entity<ChatHistorico>().ToTable("ChatHistorico");

            modelBuilder.Entity<Conversa>()
                .HasMany(c => c.Mensagens)
                .WithOne(m => m.Conversa)
                .HasForeignKey(m => m.ConversaId);

            // --- INÍCIO DO DATA SEEDING CORRIGIDO ---

            // 1. Definir os GUIDs que usaremos para as chaves primárias
            //    Isso garante que podemos criar os relacionamentos corretamente.
            var produtoCocaId = Guid.Parse("c3d3b7a0-9a6a-4b0a-9d0a-0a0a0a0a0a0a");
            var produtoFandangosId = Guid.Parse("c3d3b7a0-9a6a-4b0a-9d0a-0b0b0b0b0b0b");
            var produtoAguaId = Guid.Parse("c3d3b7a0-9a6a-4b0a-9d0a-0c0c0c0c0c0c");

            var venda1Id = Guid.Parse("d1e1f1a1-1b1c-1d1e-1f1a-1b1c1d1e1f1a");
            var venda2Id = Guid.Parse("d2e2f2a2-2b2c-2d2e-2f2a-2b2c2d2e2f2a");

            var itemVenda1Id = Guid.Parse("e1f1a1b1-c1d1-e1f1-a1b1-c1d1e1f1a1b1");
            var itemVenda2Id = Guid.Parse("e2f2a2b2-c2d2-e2f2-a2b2-c2d2e2f2a2b2");
            var itemVenda3Id = Guid.Parse("e3f3a3b3-c3d3-e3f3-a3b3-c3d3e3f3a3b3");

            var dataAtual = DateTime.UtcNow;

            // 2. Popular a tabela de Produtos
            modelBuilder.Entity<Produto>().HasData(
                new Produto
                {
                    Id = produtoCocaId,
                    Descricao = "Coca-Cola 2L",
                    CodigoBarras = "789456123",
                    PrecoVenda = 10.00m,
                    EstoqueAtual = 100,
                    CriadoEmUTC = dataAtual,
                    AtualizadoEmUTC = dataAtual,
                    IsAtivo = true
                },
                new Produto
                {
                    Id = produtoFandangosId,
                    Descricao = "Fandangos 140g",
                    CodigoBarras = "789123456",
                    PrecoVenda = 7.50m,
                    EstoqueAtual = 50,
                    CriadoEmUTC = dataAtual,
                    AtualizadoEmUTC = dataAtual,
                    IsAtivo = true
                },
                new Produto
                {
                    Id = produtoAguaId,
                    Descricao = "Água Mineral 500ml",
                    CodigoBarras = "789789789",
                    PrecoVenda = 3.00m,
                    EstoqueAtual = 200,
                    CriadoEmUTC = dataAtual,
                    AtualizadoEmUTC = dataAtual,
                    IsAtivo = true
                }
            );

            // 3. Popular a tabela de Vendas
            modelBuilder.Entity<Venda>().HasData(
                new Venda
                {
                    Id = venda1Id,
                    IdPDV = "PDV-01",
                    MomentoDaVendaUTC = dataAtual.AddMinutes(-10), // Usando a data atual como referência
                    ValorTotal = 25.00m,
                    SincronizadoEmUTC = dataAtual
                },
                new Venda
                {
                    Id = venda2Id,
                    IdPDV = "PDV-01",
                    MomentoDaVendaUTC = dataAtual.AddMinutes(-5),
                    ValorTotal = 3.00m,
                    SincronizadoEmUTC = dataAtual
                }
            );

            // 4. Popular a tabela de ItensVenda, ligando os produtos às vendas
            modelBuilder.Entity<ItemVenda>().HasData(
                // Itens da Venda 1
                new ItemVenda { Id = itemVenda1Id, IdVenda = venda1Id, IdProduto = produtoCocaId, Quantidade = 1, PrecoUnitario = 10.00m },
                new ItemVenda { Id = itemVenda2Id, IdVenda = venda1Id, IdProduto = produtoFandangosId, Quantidade = 2, PrecoUnitario = 7.50m },
                // Item da Venda 2
                new ItemVenda { Id = itemVenda3Id, IdVenda = venda2Id, IdProduto = produtoAguaId, Quantidade = 1, PrecoUnitario = 3.00m }
            );
            // --- FIM DO DATA SEEDING CORRIGIDO ---
        }

    }
}