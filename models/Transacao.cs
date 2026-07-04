using System;

namespace ControleGastos.Models
{
    // Define os dois tipos possíveis de transação que o sistema aceita
    public enum TipoTransacao
    {
        Despesa,
        Receita
    }

    public class Transacao
    {
        // Identificador único da transação gerado automaticamente
        public Guid Id { get; set; } = Guid.NewGuid();

        // O que é a transação (ex: "Compras do mês", "Salário")
        public string Descricao { get; set; } = string.Empty;

        // Valor em dinheiro da transação
        public decimal Valor { get; set; }

        // Guarda se é Despesa ou Receita
        public TipoTransacao Tipo { get; set; }

        // CHAVE ESTRANGEIRA: Guarda o ID da pessoa que fez essa transação.
        // É através daqui que o banco sabe a quem pertence esse gasto/ganho.
        public Guid PessoaId { get; set; }
    }
}