using System;

namespace Dominio.Entidades
{
    /// <summary>
    /// Representa um item específico dentro de uma Venda.
    /// </summary>
    public class ItemVenda
    {
        public Guid Id { get; set; }

        // Chave estrangeira e propriedade de navegação para a Venda
        public Guid IdVenda { get; set; }
        public Venda Venda { get; set; }

        // Chave estrangeira e propriedade de navegação para o Produto
        public Guid IdProduto { get; set; }
        public Produto Produto { get; set; }

        /// <summary>
        /// Quantidade vendida do produto (pode ser unitário ou em Kg, por isso decimal).
        /// </summary>
        public decimal Quantidade { get; set; }

        /// <summary>
        /// Preço unitário do produto NO MOMENTO DA VENDA.
        /// É crucial armazenar isso aqui para garantir que o registro da venda
        /// não seja afetado por futuras alterações no preço do produto.
        /// </summary>
        public decimal PrecoUnitario { get; set; }
    }
}