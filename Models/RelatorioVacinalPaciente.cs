namespace maui_amarv.Models
{
    public class RelatorioVacinalPaciente : Relatorio
    {
        public int PacienteId { get; set; }

        public string StatusVacinal { get; set; }

        public Relatorio GerarRelatorioVacinalPaciente(Paciente paciente)
        {
            DataGeracao = DateTime.Now;
            PacienteId = paciente.Id;
            StatusVacinal = "Relatório individual gerado";
            Conteudo = "Relatório vacinal do paciente: " + paciente.Nome;

            return this;
        }
    }
}