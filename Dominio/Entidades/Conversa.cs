using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dominio.Entidades
{
    public class Conversa
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Titulo { get; set; } = "Nova Conversa";

        [Required]
        public DateTime DataCriacao { get; set; }

        // Propriedade de navegação para o Entity Framework
        public virtual ICollection<ChatHistorico> Mensagens { get; set; } = new List<ChatHistorico>();
    }
}