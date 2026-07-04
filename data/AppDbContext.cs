using Microsoft.EntityFrameworkCore;
using ControleGastos.Models;

namespace ControleGastos.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Tabelas do nosso banco de dados
        public DbSet<Pessoa> Pessoas { get; set; }
        public DbSet<Transacao> Transacoes { get; set; } // Nova tabela adicionada!

        // Esta função serve para configurar regras mais avançadas do banco de dados
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // CONFIGURAÇÃO DA DELEÇÃO EM CASCATA:
            // Estamos dizendo que: Uma Transação tem UMA Pessoa, e uma Pessoa pode ter MUITAS Transações.
            // A ligação é feita pelo 'PessoaId'. Se a Pessoa for deletada, DeleteBehavior.Cascade
            // se encarrega de apagar automaticamente todas as transações ligadas a ela.
            modelBuilder.Entity<Transacao>()
                .HasOne<Pessoa>()
                .WithMany()
                .HasForeignKey(t => t.PessoaId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}