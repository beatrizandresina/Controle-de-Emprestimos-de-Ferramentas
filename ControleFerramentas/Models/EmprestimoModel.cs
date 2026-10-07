namespace ControleFerramentas.Models
{
    public class EmprestimoModel
    {
        public int Id { get; set; }
        public int FerramentaId { get; set; }
        public int PessoaId { get; set; }
        public DateTime DataEmprestimo { get; set; }
        public DateTime? DataDevolucao { get; set; }
        public FerramentaModel Ferramenta { get; set; }
        public PessoaModel Pessoa { get; set; }
    }
}
