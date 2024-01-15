using OpenQA.Selenium;

namespace Desafio.Alura.AeC.Infrastructure.Extensions
{
    /// <summary>
    /// WebDriverExtensions métodos de extensão úteis para um IWebDriver e IWebElement
    /// </summary>
    public static class WebDriverExtensions
    {
        /// <summary>
        /// Busca um elemento de forma segura, sem retornar exceção caso não tenha encontrado.        
        /// </summary>
        /// <param name="element">O próprio elemento html web</param>     
        /// <param name="by">Mecanismo de Busca</param>
        public static IWebElement? FindElementSafe(this IWebElement element, By by)
        {
            try
            {
                return element.FindElement(by);
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        }

        /// <summary>
        /// Busca um elemento de forma segura, sem retornar exceção caso não tenha encontrado.
        /// </summary>
        /// <param name="driver">O próprio driver</param>     
        /// <param name="by">Mecanismo de Busca</param>
        public static IWebElement? FindElementSafe(this IWebDriver driver, By by)
        {
            try
            {
                return driver.FindElement(by);
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        }
    }
}
