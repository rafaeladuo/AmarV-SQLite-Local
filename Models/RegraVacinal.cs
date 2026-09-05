using SQLite;

namespace maui_amarv.Models
{
    public class RegraVacinal
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string NomeVacina { get; set; }

        public string Fabricante { get; set; }

        public string TipoVacina { get; set; }

        public int IdadeMinimaMeses { get; set; }

        public int IdadeMaximaMeses { get; set; }

        public int DosesNecessarias { get; set; }

        public string IntervaloEntreDoses { get; set; }

        public string ViaAdministracao { get; set; }

        public string LocalAplicacao { get; set; }

        public string TipoCalendario { get; set; }

        public string Indicacao { get; set; }

        public string Contraindicacao { get; set; }

        public string EventosAdversos { get; set; }

        public string Observacao { get; set; }

        public string Resumo
        {
            get
            {
                return NomeVacina + " - " + DosesNecessarias + " dose(s)";
            }
        }
    }
}