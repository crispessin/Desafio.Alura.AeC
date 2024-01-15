namespace Desafio.Alura.AeC.Domain.Conteudo
{
    public interface IConteudoRepository
    {
        /// <summary>
        /// IConteudoRepository representa um repositório de dados para conteúdos
        /// </summary>
        /// <param name="conteudo">Lista de Conteúdo que devem ser persistidos</param>        
        public void Add(List<Conteudo> conteudo);
    }
}
