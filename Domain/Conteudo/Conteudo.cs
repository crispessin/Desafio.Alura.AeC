namespace Desafio.Alura.AeC.Domain.Conteudo
{
    public class Conteudo
    {
        public Conteudo(string? titulo, string? descricao, string? professor, string? cargaHoraria)
        {
            var now = DateTimeOffset.UtcNow;
            DataCriacao = now;
            DataAtualizacao = now;
            Titulo = titulo;
            Descricao = descricao;
            Professor = professor;            
            CargaHoraria = cargaHoraria;            
        }

        public int Id { get; set; }
        public DateTimeOffset DataCriacao { get; set; }
        public DateTimeOffset DataAtualizacao { get; set; }
        public string? Titulo { get; set; }
        public string? Professor { get; set; }
        public string? CargaHoraria { get; set; }
        public string? Descricao { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Conteudo conteudo &&
                   Id == conteudo.Id;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
