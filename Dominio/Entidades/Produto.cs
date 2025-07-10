using System;
using System.ComponentModel.DataAnnotations; // Adicione este using!

namespace Dominio.Entidades
{
    public class Produto
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "O código de barras é obrigatório.")]
        [StringLength(50, ErrorMessage = "O código de barras não pode ter mais de 50 caracteres.")]
        public string CodigoBarras { get; set; }
        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [StringLength(100, ErrorMessage = "A descrição não pode ter mais de 100 caracteres.")]
        public string Descricao { get; set; }
        [Required(ErrorMessage = "O preço de venda é obrigatório.")]
        [Range(0.01, 99999.99, ErrorMessage = "O preço deve ser maior que zero.")]
        public decimal PrecoVenda { get; set; }
        [Required(ErrorMessage = "O estoque é obrigatório.")]
        [Range(0, 99999.999, ErrorMessage = "O estoque não pode ser negativo.")]
        public decimal EstoqueAtual { get; set; }
        public DateTime CriadoEmUTC { get; set; }
        public DateTime AtualizadoEmUTC { get; set; }
        public bool IsAtivo { get; set; } = true;
    }
}