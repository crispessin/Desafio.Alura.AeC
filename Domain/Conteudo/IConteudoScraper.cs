namespace Desafio.Alura.AeC.Domain.Conteudo
{
    /// <summary>
    /// IConteudoScraper responsável por recuperar o conteúdo do site da Alura.
    /// </summary>
    public interface IConteudoScraper
    {
        /// <summary>
        /// Obtém o conteúdo do site retornando: 
        /// <c>Titulo: Curso, Formação, Artigo, etc
        /// Professor: um ou vários
        /// Carga Horária: Carga do Curso, Tempo do Podcast
        /// Descrição: Descrição do Conteúdo.</c>
        /// </summary>
        /// <param name="url">URL do Site Destino</param>
        /// <param name="termo">Termo de Busca. Ex: RPA, C#, Java</param>
        List<Conteudo> GetConteudosByTermo(string url, string termo);
    }
}
