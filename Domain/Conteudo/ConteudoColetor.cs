using Desafio.Alura.AeC.Domain.Conteudo;

namespace Desafio.Alura.AeC.Domain.Conteudo
{
    public class ConteudoColetor : IConteudoColetor
    {
        private readonly IConteudoRepository _conteudoRepository;
        private readonly IConteudoScraper _conteudoScraper;

        public ConteudoColetor(IConteudoRepository cursoRepository, 
                               IConteudoScraper conteudoScraper)
        {
            _conteudoRepository = cursoRepository;
            _conteudoScraper = conteudoScraper;
        }

        public void ColetarEArmazenar(string url, string termo)
        {
            var conteudos = _conteudoScraper.GetConteudosByTermo(url, termo);

            _conteudoRepository.Add(conteudos);
        }
    }
}
