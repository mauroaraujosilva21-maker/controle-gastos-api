using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ControleGastos.Data;
using System.Linq;
using System.Threading.Tasks;
using ControleGastos.DTOs;
using ControleGastos.Models;

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

        // 1. LISTAR TODAS AS PESSOAS
        [HttpGet]
        public async Task<IActionResult> ObterTodas()
        {
            var pessoas = await _context.Pessoas.AsNoTracking().ToListAsync();
            return Ok(pessoas);
        }

        // 2. BUSCAR PESSOA POR ID
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var p = await _context.Pessoas.FindAsync(id);
            if (p == null) return NotFound("Pessoa não encontrada.");
            return Ok(p);
        }

        // 3. CADASTRAR NOVA PESSOA
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] Pessoa novaPessoa)
        {
            _context.Pessoas.Add(novaPessoa);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(ObterPorId), new { id = novaPessoa.Id }, novaPessoa);
        }

        // 4. REQUISITO: CONSULTA DE TOTAIS
        [HttpGet("totais")]
        public async Task<IActionResult> ObterTotais()
        {
            // Busca apenas os dados necessários de forma plana, desativando o rastreamento
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
    } // Fim da classe PessoasController
} // Fim do namespace