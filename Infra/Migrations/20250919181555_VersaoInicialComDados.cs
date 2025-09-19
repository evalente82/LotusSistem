using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class VersaoInicialComDados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Conversas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CodigoBarras = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PrecoVenda = table.Column<decimal>(type: "numeric", nullable: false),
                    EstoqueAtual = table.Column<decimal>(type: "numeric", nullable: false),
                    CriadoEmUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEmUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsAtivo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vendas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdPDV = table.Column<string>(type: "text", nullable: false),
                    MomentoDaVendaUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    SincronizadoEmUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatHistorico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConversaId = table.Column<int>(type: "integer", nullable: false),
                    Autor = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Conteudo = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatHistorico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatHistorico_Conversas_ConversaId",
                        column: x => x.ConversaId,
                        principalTable: "Conversas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItensVenda",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdVenda = table.Column<Guid>(type: "uuid", nullable: false),
                    IdProduto = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantidade = table.Column<decimal>(type: "numeric", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensVenda", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensVenda_Produtos_IdProduto",
                        column: x => x.IdProduto,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItensVenda_Vendas_IdVenda",
                        column: x => x.IdVenda,
                        principalTable: "Vendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "AtualizadoEmUTC", "CodigoBarras", "CriadoEmUTC", "Descricao", "EstoqueAtual", "IsAtivo", "PrecoVenda" },
                values: new object[,]
                {
                    { new Guid("c3d3b7a0-9a6a-4b0a-9d0a-0a0a0a0a0a0a"), new DateTime(2025, 9, 19, 18, 15, 54, 705, DateTimeKind.Utc).AddTicks(5941), "789456123", new DateTime(2025, 9, 19, 18, 15, 54, 705, DateTimeKind.Utc).AddTicks(5941), "Coca-Cola 2L", 100m, true, 10.00m },
                    { new Guid("c3d3b7a0-9a6a-4b0a-9d0a-0b0b0b0b0b0b"), new DateTime(2025, 9, 19, 18, 15, 54, 705, DateTimeKind.Utc).AddTicks(5941), "789123456", new DateTime(2025, 9, 19, 18, 15, 54, 705, DateTimeKind.Utc).AddTicks(5941), "Fandangos 140g", 50m, true, 7.50m },
                    { new Guid("c3d3b7a0-9a6a-4b0a-9d0a-0c0c0c0c0c0c"), new DateTime(2025, 9, 19, 18, 15, 54, 705, DateTimeKind.Utc).AddTicks(5941), "789789789", new DateTime(2025, 9, 19, 18, 15, 54, 705, DateTimeKind.Utc).AddTicks(5941), "Água Mineral 500ml", 200m, true, 3.00m }
                });

            migrationBuilder.InsertData(
                table: "Vendas",
                columns: new[] { "Id", "IdPDV", "MomentoDaVendaUTC", "SincronizadoEmUTC", "ValorTotal" },
                values: new object[,]
                {
                    { new Guid("d1e1f1a1-1b1c-1d1e-1f1a-1b1c1d1e1f1a"), "PDV-01", new DateTime(2025, 9, 19, 18, 5, 54, 705, DateTimeKind.Utc).AddTicks(5941), new DateTime(2025, 9, 19, 18, 15, 54, 705, DateTimeKind.Utc).AddTicks(5941), 25.00m },
                    { new Guid("d2e2f2a2-2b2c-2d2e-2f2a-2b2c2d2e2f2a"), "PDV-01", new DateTime(2025, 9, 19, 18, 10, 54, 705, DateTimeKind.Utc).AddTicks(5941), new DateTime(2025, 9, 19, 18, 15, 54, 705, DateTimeKind.Utc).AddTicks(5941), 3.00m }
                });

            migrationBuilder.InsertData(
                table: "ItensVenda",
                columns: new[] { "Id", "IdProduto", "IdVenda", "PrecoUnitario", "Quantidade" },
                values: new object[,]
                {
                    { new Guid("e1f1a1b1-c1d1-e1f1-a1b1-c1d1e1f1a1b1"), new Guid("c3d3b7a0-9a6a-4b0a-9d0a-0a0a0a0a0a0a"), new Guid("d1e1f1a1-1b1c-1d1e-1f1a-1b1c1d1e1f1a"), 10.00m, 1m },
                    { new Guid("e2f2a2b2-c2d2-e2f2-a2b2-c2d2e2f2a2b2"), new Guid("c3d3b7a0-9a6a-4b0a-9d0a-0b0b0b0b0b0b"), new Guid("d1e1f1a1-1b1c-1d1e-1f1a-1b1c1d1e1f1a"), 7.50m, 2m },
                    { new Guid("e3f3a3b3-c3d3-e3f3-a3b3-c3d3e3f3a3b3"), new Guid("c3d3b7a0-9a6a-4b0a-9d0a-0c0c0c0c0c0c"), new Guid("d2e2f2a2-2b2c-2d2e-2f2a-2b2c2d2e2f2a"), 3.00m, 1m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatHistorico_ConversaId",
                table: "ChatHistorico",
                column: "ConversaId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensVenda_IdProduto",
                table: "ItensVenda",
                column: "IdProduto");

            migrationBuilder.CreateIndex(
                name: "IX_ItensVenda_IdVenda",
                table: "ItensVenda",
                column: "IdVenda");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatHistorico");

            migrationBuilder.DropTable(
                name: "ItensVenda");

            migrationBuilder.DropTable(
                name: "Conversas");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropTable(
                name: "Vendas");
        }
    }
}
