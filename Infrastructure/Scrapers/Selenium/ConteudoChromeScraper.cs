using Desafio.Alura.AeC.Domain.Conteudo;
using Desafio.Alura.AeC.Infrastructure.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace Desafio.Alura.AeC.Infrastructure.Scrapers.Selenium
{
    public class ConteudoChromeScraper : IConteudoScraper
    {
        private readonly ILogger<ConteudoChromeScraper> _logger;
        private readonly IWebDriver _driver;

        public ConteudoChromeScraper(ILogger<ConteudoChromeScraper> logger)
        {
            _logger = logger;
            _driver = InicializarDriver();
        }

        private static IWebDriver InicializarDriver()
        {
            ChromeOptions options = new()
            {
                PageLoadStrategy = PageLoadStrategy.Normal
            };

            options.AddArgument("no-sandbox");
            options.AddArgument("--profile-directory=Default");
            options.AddArgument("--disable-web-security");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--start-maximized");
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--ignore-ssl-error");

            options.AddExcludedArgument("enable-logging");

            return new ChromeDriver(options);
        }

        public List<Conteudo> GetConteudosByTermo(string url, string termo)
        {
            _logger.LogInformation("Iniciando busca por termo: {termo}", termo);

            // GetLinkDasPaginasByTermo retorna uma lista de links que direcionam pra uma página que tem o conteúdo
            // Para cada conteudo retornado, abre a página e chama ExtrairConteudoDoLink            
            // Como o ExtrairConteudoDoLink retorna List<string> então teremos uma List<List<Conteudo>> de dados fazemos o SelectMany para juntar tudo em uma lista simples (List<Conteudo>)
            var conteudos = GetLinkDasPaginasByTermo(url, termo)
                .SelectMany(ExtrairConteudoDoLink)
                .ToList();

            // Se não retornou dados, apenas exibe um log informando.
            if (!conteudos.Any())
            {
                _logger.LogInformation("Nenhum resultado encontrado.");
            }

            _driver.Close();

            _logger.LogInformation("Busca concluída");

            return conteudos;
        }

        /// <summary>
        /// Busca todas as paginas que precisam ter o conteudo extraido.
        /// </summary>
        /// <param name="url">Link do site</param>
        /// <param name="termo">Termo buscado</param>
        /// <returns>Lista de Páginas. Exemplo: [https://www.alura.com.br/busca?pagina=1&query=TERMO, https://www.alura.com.br/busca?pagina=2&query=TERMO"]</returns>
        private List<string> GetLinkDasPaginasByTermo(string url, string termo)
        {
            // Navega até a página principal do conteudo do site            
            if (!GoToUrl(url))
            {
                // Se não conseguir, retorna uma lista vazia.
                return new List<string>();
            }

            _driver.FindElementSafe(By.XPath("//*[@id=\"header-barraBusca-form-campoBusca\"]"))?.SendKeys(termo);

            // Enter Para Pesquisar
            var actions = new Actions(_driver);
            actions.SendKeys(Keys.Enter);
            actions.Build().Perform();

            //Se o número de páginas for muito grande, retorna com "..."
            var paginasComContinuacao = _driver.FindElementSafe(By.CssSelector("a.paginationLink.paginationLink--withSuspensionPoints"));

            if (paginasComContinuacao != null)
            {
                var ultimaPagina = paginasComContinuacao.GetAttribute("href");
                var numeroDePaginas = paginasComContinuacao.Text;

                // Cria um link da primeira página até a última.
                // Exemplo: /busca?pagina=1, /busca?pagina=2....
                return Enumerable.Range(1, Int32.Parse(numeroDePaginas))
                    .Select(pagina => ultimaPagina.Replace($"={numeroDePaginas}&", $"={pagina}&"))
                    .ToList();
            }

            // Se não tiver "..." então busca os links de forma simples
            return _driver.FindElements(By.XPath("//*[@id=\"busca\"]/nav/nav/a"))
                .Select(link => link.Text) // Pega o número da pagina
                .Select(pagina => _driver.FindElementSafe(By.XPath($"//*[@id=\"busca\"]/nav/nav/a[{pagina}]"))?.GetAttribute("href") ?? "")
                .ToList();
        }

        /// <summary>
        /// Carrega um endereço no navegador, se tiver erro apenas exibe um log e retorna se teve sucesso ou não.
        /// </summary>
        /// <param name="url">Url do Site destino</param>
        private bool GoToUrl(string url)
        {
            try
            {
                _driver.Navigate().GoToUrl(url);
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Erro o acessar endereço: {link}", url);
                return false;
            }
        }

        /// <summary>
        /// 
        /// Extrai o conteúdo de um página.        
        /// Cada página tem uma lista de Resultados que podem ser Cursos, Formações, PodCasts.        
        /// O extrai dados de cada resultado retornado na busca paginada, como nome e descrição.
        /// Depois, acessa cada CARD da lista para pegar dados de carga horária e instrutores.        
        ///
        /// </summary>
        /// <param name="paginaLink">Link da página que deverá ter o conteúdo extraido. Exemplo: https://www.alura.com.br/busca?pagina=1&query=TERMO</param>
        /// <returns>Lista de Conteúdo encontrado</returns>
        private List<Conteudo> ExtrairConteudoDoLink(string paginaLink)
        {            
            // Acessa a página com resultado
            if (!GoToUrl(paginaLink))
            {
                // Se não conseguir retorna uma lista vazia
                return new List<Conteudo>();
            }
            
            var resultados = _driver.FindElements(By.XPath("//*[@id=\"busca-resultados\"]/ul/li"))
                .Select(resultado =>
                {
                    var link = resultado.FindElementSafe(By.TagName("a"))?.GetAttribute("href") ?? "";
                    var nome = resultado.FindElementSafe(By.ClassName("busca-resultado-nome"))?.Text ?? "";
                    var descricao = resultado.FindElementSafe(By.ClassName("busca-resultado-descricao"))?.Text ?? "";
                    return (link, nome, descricao);
                })
                .ToList();

            var conteudos = new List<Conteudo>();
            
            resultados.ForEach(resultado =>
            {
                _logger.LogInformation("Buscando dados no link: {link}", resultado.link);

                try
                {
                    // acessa a página do conteudo para pegar a carga horária e instrutores
                    if (GoToUrl(resultado.link))
                    {
                        var cargaHoraria = GetCargaHoraria();
                        var instrutores = GetInstrutores();

                        conteudos.Add(new Conteudo(resultado.nome, resultado.descricao, string.Join(";", instrutores), cargaHoraria));
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Erro buscar dados no link: {link}", resultado.link);
                }
            });

            return conteudos;
        }

        /// <summary>
        /// Busca a carga horária da Formação, do Curso ou Episódio.
        /// </summary>
        /// <returns>Carga horária encontrada no formato encontrado. Exemplo: 50h, 40 min</returns>
        private string? GetCargaHoraria()
        {
            return _driver.FindElementSafe(By.ClassName("formacao__info-destaque"))?.Text
                ?? _driver.FindElementSafe(By.ClassName("course-card-wrapper-infos"))?.Text
                ?? _driver.FindElementSafe(By.ClassName("episode-programming-time"))?.Text;
        }

        /// <summary>
        /// Verifica se existe apenas um instrutor, se sim retorna, senão tenta buscar todos os instrutores da página.
        /// </summary>
        /// <returns>Lista de Instrutores únicos encontrados</returns>
        private List<string> GetInstrutores()
        {
            return _driver.FindElementSafe(By.ClassName("instructor-title--name"))?.Text?.AsList()
                ?? _driver.FindElements(By.XPath("//*[@id=\"instrutores\"]/div/ul/li/div/h3"))
                .Select(x => x.Text)
                .Where(x => x.Trim().Length > 0)
                .Distinct()
                .ToList();
        }
    }
}
