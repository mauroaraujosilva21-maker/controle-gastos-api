using System;
using System.Collections.Generic; // Adicionado para suportar a ICollection
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControleGastos.Models
{
    [Table("Pessoas")]
    public class Pessoa
    {
        [Key]
        [Column(TypeName = "uuid")]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Nome { get; set; } = string.Empty;

        public int Idade { get; set; }

        // 🌟 ADICIONE ESTA LINHA AQUI EMBAIXO PARA CRIAR A CONEXÃO COM AS TRANSAÇÕES:
        public virtual ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
    }
}