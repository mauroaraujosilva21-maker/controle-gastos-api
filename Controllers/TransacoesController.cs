using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ControleGastos.Data;
using ControleGastos.Models;
using System.Threading.Tasks;
using System.Linq;

namespace ControleGastos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransacoesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransacoesController(AppDbContext context)
        {
            _context = context;
        }

        // 1. REQUISITO: LISTAGEM DE TRANSAÇÕES
        // Acessível via HTTP GET: api/transacoes
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var transacoes = await _context.Transacoes.ToListAsync();
            return Ok(transacoes);
        }

        // 2. REQUISITO: CRIAÇÃO DE TRANSAÇÃO COM VALIDAÇÃO
        // Acessível via HTTP POST: api/transacoes
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] Transacao transacao)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // VALIDAÇÃO 1: Verificar se a pessoa informada realmente existe no banco
            var pessoa = await _context.Pessoas.FindAsync(transacao.PessoaId);
            if (pessoa == null)
            {
                return BadRequest("A pessoa informada para esta transação não existe.");
            }

            // VALIDAÇÃO 2 (REGRA DE NEGÓCIO): Menores de 18 anos só podem ter DESPESAS
            if (pessoa.Idade < 18 && transacao.Tipo == TipoTransacao.Receita)
            {
                // Se for menor de idade e o tipo for Receita, barramos o cadastro aqui
                return BadRequest("Menores de 18 anos não podem possuir transações do tipo Receita.");
            }

            // Se passou pelas validações, adiciona e salva no banco de dados
            _context.Transacoes.Add(transacao);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Listar), new { id = transacao.Id }, transacao);
        }

        // 3. NOVO REQUISITO: ENGENHARIA DOS TOTAIS DA CASA E POR PESSOA
        // Acessível via HTTP GET: api/transacoes/totais
        [HttpGet("totais")]
        public async Task<IActionResult> ObterTotaisPainel()
        {
            // 1. Puxa as transações do banco para calcular o Total Geral acumulado da Casa
            var todasTransacoes = await _context.Transacoes.ToListAsync();
            
            var totalReceitasCasa = todasTransacoes.Where(t => t.Tipo == TipoTransacao.Receita).Sum(t => t.Valor);
            var totalDespesasCasa = todasTransacoes.Where(t => t.Tipo == TipoTransacao.Despesa).Sum(t => t.Valor);
            var saldoGeralCasa = totalReceitasCasa - totalDespesasCasa;

            // 2. Busca as pessoas trazendo suas respectivas transações de forma limpa
            var pessoasDoBanco = await _context.Pessoas.Include(p => p.Transacoes).ToListAsync();
            
            var resumoPessoas = pessoasDoBanco.Select(p => new
            {
                PessoaId = p.Id,
                Nome = p.Nome,
                Receitas = p.Transacoes.Where(t => t.Tipo == TipoTransacao.Receita).Sum(t => t.Valor),
                Despesas = p.Transacoes.Where(t => t.Tipo == TipoTransacao.Despesa).Sum(t => t.Valor),
                Saldo = p.Transacoes.Where(t => t.Tipo == TipoTransacao.Receita).Sum(t => t.Valor) - 
                        p.Transacoes.Where(t => t.Tipo == TipoTransacao.Despesa).Sum(t => t.Valor)
            }).ToList();

            // 3. Retorna o payload estruturado para alimentar os cards superiores e a tabela do React
            return Ok(new
            {
                TotalReceitasCasa = totalReceitasCasa,
                TotalDespesasCasa = totalDespesasCasa,
                SaldoGeralCasa = saldoGeralCasa,
                Pessoas = resumoPessoas
            });
        }
    }
}