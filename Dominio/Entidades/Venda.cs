
namespace Dominio.Entidades
{
    /// <summary>
    /// Representa uma transação de venda completa, contendo um ou mais itens.
    /// </summary>
    public class Venda
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador do Ponto de Venda (PDV) que originou a transação.
        /// Pode ser o nome da máquina ou um GUID configurado no PDV.
        /// </summary>
        public string IdPDV { get; set; }

        /// <summary>
        /// Data e hora em que a venda foi finalizada, em formato universal (UTC).
        /// </summary>
        public DateTime MomentoDaVendaUTC { get; set; }

        /// <summary>
        /// Valor total da venda, calculado a partir da soma dos itens.
        /// </summary>
        public decimal ValorTotal { get; set; }

        /// <summary>
        /// A lista de itens que compõem esta venda.
        /// Esta é a propriedade de navegação para a relação um-para-muitos.
        /// </summary>
        public ICollection<ItemVenda> Itens { get; set; }

        public Venda()
        {
            // Boa prática: inicializar coleções para evitar erros de referência nula.
            Itens = new List<ItemVenda>();
        }

        public DateTime SincronizadoEmUTC { get; set; }
    }
}