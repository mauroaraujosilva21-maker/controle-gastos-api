using System;
using System.Collections.Generic;

namespace ControleGastos.DTOs
{
    // Estrutura para os totais individuais de cada pessoa
    public class ResumoPessoaDto
    {
        public Guid PessoaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal TotalReceitas { get; set; }
        public decimal TotalDespesas { get; set; }
        
        // Propriedade calculada automaticamente: Receita menos Despesa
        public decimal Saldo => TotalReceitas - TotalDespesas; 
    }

    // Estrutura para o relatório completo (lista de pessoas + soma geral)
    public class RelatorioTotaisDto
    {
        // Uma lista contendo o resumo de cada pessoa cadastrada
        public List<ResumoPessoaDto> DetalhesPorPessoa { get; set; } = new();

        // Somas gerais de toda a residência
        public decimal TotalGeralReceitas { get; set; }
        public decimal TotalGeralDespesas { get; set; }
        
        // Saldo final somando tudo
        public decimal SaldoLiquidoGeral => TotalGeralReceitas - TotalGeralDespesas;
    }
}