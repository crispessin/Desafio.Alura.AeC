using Desafio.Alura.AeC.Domain.Conteudo;
using Desafio.Alura.AeC.Infrastructure.Persistence.DbContexts;

namespace Desafio.Alura.AeC.Infrastructure.Persistence.Repositories
{
    public class ConteudoRepository : IConteudoRepository
    {
        private readonly AppDbContext _dbContext;

        public ConteudoRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(List<Conteudo> conteudos)
        {
            conteudos.ForEach(conteudo =>
            {
                _dbContext.Add(conteudo);
                _dbContext.SaveChanges();
            });
        }
    }

}
