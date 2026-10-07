namespace ControleFerramentas.Models
{
    public class FerramentaModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string? Observacao { get; set; }
        public bool Disponivel { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataUltimoEmprestimo { get; set; }
        public DateTime? DataUltimaDevolucao { get; set; }
    }
}
