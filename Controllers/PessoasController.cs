using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ControleGastos.Data;
using ControleGastos.Models;
using ControleGastos.DTOs;
using System;
using System.Collections.Generic;
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

            // 4. REQUISITO: CONSULTA DE TOTAIS
        [HttpGet("totais")]
        public async Task<IActionResult> ObterTotais()
        {
            // Busca apenas os dados necessários de forma plana, desativando o rastreamento (AsNoTracking)
            var pessoasComTransacoes = await _context.Pessoas
                .AsNoTracking()
                .Select(p => new
                {
                    p.Id,
                    Nome = p.Nome ?? "",
                    Transacoes = p.Transacoes.Select(t => new { t.Tipo, t.Valor }).ToList()
                })
                .ToListAsync();

            var relatorio = new RelatorioTotaisDto();

            foreach (var pessoa in pessoasComTransacoes)
            {
                // Calcula os totais com base na projeção limpa
                var totalReceitas = pessoa.Transacoes.Where(t => t.Tipo == TipoTransacao.Receita).Sum(t => t.Valor);
                var totalDespesas = pessoa.Transacoes.Where(t => t.Tipo == TipoTransacao.Despesa).Sum(t => t.Valor);

                var resumoPessoa = new ResumoPessoaDto
                {
                    PessoaId = pessoa.Id,
                    Nome = pessoa.Nome,
                    TotalReceitas = totalReceitas,
                    TotalDespesas = totalDespesas
                };

                relatorio.DetalhesPorPessoa.Add(resumoPessoa);

                relatorio.TotalGeralReceitas += totalReceitas;
                relatorio.TotalGeralDespesas += totalDespesas;
            }

            return Ok(relatorio);
        }

            return Ok(relatorio);
        }
    }
}