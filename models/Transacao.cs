using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControleGastos.Models
{
    [Table("Transacoes")]
    public class Transacao
    {
        [Key]
        [Column(TypeName = "uuid")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        public decimal Valor { get; set; }

        [Required]
        public TipoTransacao Tipo { get; set; }

        // 🌟 AQUI ESTÁ A CHAVE: Força o EF a usar a coluna certa sem criar "PessoaId1"
        [Required]
        [ForeignKey("Pessoa")]
        [Column("PessoaId", TypeName = "uuid")]
        public Guid PessoaId { get; set; }

        // Propriedade de navegação inversa
        public virtual Pessoa? Pessoa { get; set; }
    }

    public enum TipoTransacao
    {
        Receita = 0,
        Despesa = 1
    }
}