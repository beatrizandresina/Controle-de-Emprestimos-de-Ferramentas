using ControleFerramentas.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace ControleFerramentas.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<PessoaModel> Pessoas { get; set; }
        public DbSet<FerramentaModel> Ferramentas { get; set; }
        public DbSet<EmprestimoModel> Emprestimos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PessoaModel>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<FerramentaModel>()
                .HasKey(f => f.Id);

            modelBuilder.Entity<EmprestimoModel>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<EmprestimoModel>()
                .HasOne(e => e.Pessoa)
                .WithMany()
                .HasForeignKey(e => e.PessoaId);

            modelBuilder.Entity<EmprestimoModel>()
                .HasOne(e => e.Ferramenta)
                .WithMany()
                .HasForeignKey(e => e.FerramentaId);
        }
    }
}
