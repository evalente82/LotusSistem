using System;
using System.Collections.Generic;

namespace Aplicacao.DTOs
{
    // DTO para enviar uma nova venda do PDV para o Servidor
    public class RegistrarVendaDto
    {
        public Guid Id { get; set; } // Id gerado no PDV
        public string IdPDV { get; set; }
        public DateTime MomentoDaVendaUTC { get; set; }
        public List<ItemVendaDto> Itens { get; set; } = new List<ItemVendaDto>();
    }

    public class ItemVendaDto
    {
        public Guid IdProduto { get; set; }
        public decimal Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}