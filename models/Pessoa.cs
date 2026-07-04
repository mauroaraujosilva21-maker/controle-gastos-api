using System;

namespace ControleGastos.Models
{
    // Esta classe define como uma "Pessoa" será salva no nosso sistema
    public class Pessoa
    {
        // O ID é o identificador único. O 'Guid' gera um código único automático 
        // parecido com isso: 123e4567-e89b-12d3-a456-426614174000
        public Guid Id { get; set; } = Guid.NewGuid();

        // Armazena o nome da pessoa (começa com um texto vazio de padrão)
        public string Nome { get; set; } = string.Empty;

        // Armazena a idade da pessoa
        public int Idade { get; set; }
    }
}