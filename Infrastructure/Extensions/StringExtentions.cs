using System.Diagnostics.CodeAnalysis;

namespace Desafio.Alura.AeC.Infrastructure.Extensions
{
    public static class StringExtentions
    {
        /// <summary>
        /// Transforma uma string em um Lista de strings (string -> List<string>)
        /// </summary>
        /// <param name="self">A própria string</param>        
        public static List<string> AsList(this string self)
        {
            return new List<string> { self };
        }
    }
}
