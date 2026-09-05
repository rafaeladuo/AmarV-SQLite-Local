namespace maui_amarv.Models
{
    public class Paciente : Pessoa
    {
        public DateTime DataNascimento { get; set; }

        public string Sexo { get; set; }

        public string CartaoSUS { get; set; }

        public string NomeMae { get; set; }

        public string Rua { get; set; }

        public int IdadeAnos { get; set; }

        public int IdadeMesesAdicionais { get; set; }

        public string CondicaoClinica { get; set; }

        public bool Gestante { get; set; }

        public int SemanasGestacao { get; set; }

        public bool Lactante { get; set; }

        public bool CriancaAmamentadaMenor6Meses { get; set; }

        public bool Imunossuprimido { get; set; }

        public bool HIV { get; set; }

        public bool Quimioterapia { get; set; }

        public bool Transplantado { get; set; }

        public bool UsoCorticoideOuImunobiologico { get; set; }

        public bool AlergiaGrave { get; set; }

        public bool FebreOuDoencaAguda { get; set; }

        public int IdadeTotalMeses
        {
            get
            {
                return (IdadeAnos * 12) + IdadeMesesAdicionais;
            }
        }

        public string IdadeFormatada
        {
            get
            {
                return IdadeAnos + " ano(s) e " + IdadeMesesAdicionais + " mês(es)";
            }
        }

        public void CadastrarPaciente()
        {
        }

        public void AtualizarPaciente()
        {
        }

        public Paciente ConsultarPaciente()
        {
            return this;
        }
    }
}