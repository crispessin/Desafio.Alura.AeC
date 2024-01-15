namespace Desafio.Alura.AeC.Domain.Conteudo
{
    /// <summary>
    /// IConteudoColetor responsável por intermediar o busca de dados do scraper e armazenar os dados em um repositório
    /// </summary>
    public interface IConteudoColetor
    {
        /// <summary>
        /// Coleta e Armazena um termo.
        /// </summary>
        /// <param name="url">URL do Site Destino</param>
        /// <param name="termo">Termo de Busca. Ex: RPA, C#, Java</param>
        public void ColetarEArmazenar(string url, string termo);
    }
}
