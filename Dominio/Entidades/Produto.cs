// Exemplo: Dominio/Entidades/Produto.cs
namespace Dominio.Entidades;

public class Produto
{
    public Guid Id { get; set; }
    public string CodigoBarras { get; set; }
    public string Descricao { get; set; }
    public decimal PrecoVenda { get; set; }
    public decimal EstoqueAtual { get; set; }
    public DateTime CriadoEmUTC { get; set; }
    public DateTime AtualizadoEmUTC { get; set; }
}