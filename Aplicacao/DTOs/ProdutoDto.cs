using System;

namespace Aplicacao.DTOs
{
    public class ProdutoDto
    {
        public Guid Id { get; set; }
        public string CodigoBarras { get; set; }
        public string Descricao { get; set; }
        public decimal PrecoVenda { get; set; }
        public decimal EstoqueAtual { get; set; }
    }
}