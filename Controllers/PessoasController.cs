using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ControleGastos.Data;
using ControleGastos.Models;
using ControleGastos.DTOs; // Importa o formato de relatório que criamos
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ControleGastos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PessoasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PessoasController(AppDbContext context)
        {
            _context = context;
        }

        // 1. REQUISITO: LISTAGEM DE PESSOAS
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var pessoas = await _context.Pessoas.ToListAsync();
            return Ok(pessoas);
        }

        // 2. REQUISITO: CRIAÇÃO DE PESSOA
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] Pessoa pessoa)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Pessoas.Add(pessoa);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Listar), new { id = pessoa.Id }, pessoa);
        }

        // 3. REQUISITO: DELEÇÃO DE PESSOA
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(Guid id)
        {
            var pessoa = await _context.Pessoas.FindAsync(id);
            if (pessoa == null) return NotFound("Pessoa não encontrada.");

            _context.Pessoas.Remove(pessoa);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 4. REQUISITO: CONSULTA DE TOTAIS
        // Acessível via HTTP GET: api/pessoas/totais
        [HttpGet("totais")]
        public async Task<IActionResult> ObterTotais()
        {
            // Busca todas as pessoas e todas as transações do banco
            var pessoas = await _context.Pessoas.ToListAsync();
            var transacoes = await _context.Transacoes.ToListAsync();

            // Cria o objeto principal do relatório que vamos preencher
            var relatorio = new RelatorioTotaisDto();

            // Passa de pessoa em pessoa calculando os totais individuais
            foreach (var pessoa in pessoas)
            {
                // Filtra as transações que pertencem a ESTA pessoa específica
                var transacoesDaPessoa = transacoes.Where(t => t.PessoaId == pessoa.Id).ToList();

                // Soma as receitas e despesas dela
                var totalReceitas = transacoesDaPessoa.Where(t => t.Tipo == TipoTransacao.Receita).Sum(t => t.Valor);
                var totalDespesas = transacoesDaPessoa.Where(t => t.Tipo == TipoTransacao.Despesa).Sum(t => t.Valor);

                // Monta o resumo individual da pessoa
                var resumoPessoa = new ResumoPessoaDto
                {
                    PessoaId = pessoa.Id,
                    Nome = pessoa.Nome,
                    TotalReceitas = totalReceitas,
                    TotalDespesas = totalDespesas
                };

                // Adiciona o resumo desta pessoa na lista do relatório
                relatorio.DetalhesPorPessoa.Add(resumoPessoa);

                // Acumula os valores para o Total Geral da casa
                relatorio.TotalGeralReceitas += totalReceitas;
                relatorio.TotalGeralDespesas += totalDespesas;
            }

            // Retorna o relatório completo montado
            return Ok(relatorio);
        }
    }
}