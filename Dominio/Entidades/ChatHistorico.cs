using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Entidades
{
    public class ChatHistorico
    {
        [Key]
        public int Id { get; set; }

        // --- ADIÇÃO DA CHAVE ESTRANGEIRA ---
        [Required]
        public int ConversaId { get; set; }

        [ForeignKey("ConversaId")]
        public virtual Conversa Conversa { get; set; } = null!;
        // --- FIM DA ADIÇÃO ---

        [Required]
        [MaxLength(20)]
        public string Autor { get; set; } = string.Empty;

        [Required]
        public string Conteudo { get; set; } = string.Empty;

        [Required]
        public DateTime Timestamp { get; set; }
    }
}