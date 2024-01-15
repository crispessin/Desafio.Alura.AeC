using Desafio.Alura.AeC.Domain.Conteudo;
using Microsoft.EntityFrameworkCore;

namespace Desafio.Alura.AeC.Infrastructure.Persistence.DbContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Conteudo> Conteudos { get; set; }
    }
}
