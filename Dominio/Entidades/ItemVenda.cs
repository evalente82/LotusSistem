using System;
using System.ComponentModel.DataAnnotations.Schema; // Garanta que este using está aqui

namespace Dominio.Entidades
{
    public class ItemVenda
    {
        public Guid Id { get; set; }

        public Guid IdVenda { get; set; }
        [ForeignKey("IdVenda")]
        public Venda Venda { get; set; }

        public Guid IdProduto { get; set; }
        [ForeignKey("IdProduto")]
        public Produto Produto { get; set; }


        public decimal Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}