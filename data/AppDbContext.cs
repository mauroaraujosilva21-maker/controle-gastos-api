using Microsoft.EntityFrameworkCore;
using ControleGastos.Models;

namespace ControleGastos.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Pessoa> Pessoas { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configura o relacionamento: Uma Pessoa tem Muitas Transações
            modelBuilder.Entity<Pessoa>()
                .HasMany(p => p.Transacoes)
                .WithOne(t => t.Pessoa)
                .HasForeignKey(t => t.PessoaId)
                .OnDelete(DeleteBehavior.Cascade); // Deleta em cascata automaticamente
        } 
    } // 👈 Chave que faltava para fechar a classe
} // 👈 Chave que faltava para fechar o namespace