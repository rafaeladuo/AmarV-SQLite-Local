using SQLite;

namespace maui_amarv.Models
{
    public class AnaliseEsquemaVacinal
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int PacienteId { get; set; }

        public DateTime DataAnalise { get; set; }

        public string StatusVacinal { get; set; }

        public string ResultadoAnalise { get; set; }

        public string VacinasEmAtraso { get; set; }

        public string VacinasPendentes { get; set; }

        public string VacinasDisponiveisRedePrivada { get; set; }

        public string VacinasDisponiveisSUS { get; set; }

        public string SugestaoRecuperacao { get; set; }

        public string VacinasContraindicadas { get; set; }

        public string AnalisarEsquemaVacinal(
            Paciente paciente,
            List<RegraVacinal> regras,
            List<HistoricoVacinal> historico)
        {
            PacienteId = paciente.Id;
            DataAnalise = DateTime.Now;

            List<string> emDia = IdentificarVacinasEmDia(regras, historico);
            List<string> pendentes = IdentificarVacinasPendentes(regras, historico);
            List<string> sus = IdentificarVacinasDisponiveisSUS(regras);
            List<string> privado = IdentificarVacinasDisponiveisRedePrivada(regras);
            List<string> contraindicadas = AlertarVacinasContraindicadas(paciente, regras, historico);
            List<string> primeiraVisita = GerarPrimeiraVisita(paciente, regras, historico);
            List<string> segundaVisita = GerarSegundaVisita(paciente, regras, historico);

            VacinasPendentes = string.Join("\n", pendentes);
            VacinasDisponiveisSUS = string.Join("\n", sus);
            VacinasDisponiveisRedePrivada = string.Join("\n", privado);
            VacinasContraindicadas = string.Join("\n", contraindicadas);

            if (pendentes.Count > 0)
                StatusVacinal = "Paciente com vacinas pendentes";
            else
                StatusVacinal = "Paciente em situação regular";

            SugestaoRecuperacao =
                MontarLista("1ª VISITA", primeiraVisita) +
                "\n" +
                MontarLista("2ª VISITA / RETORNO", segundaVisita);

            ResultadoAnalise =
                "AMAR V - Relatório de Análise Vacinal\n\n" +
                "Paciente: " + paciente.Nome + "\n" +
                "Idade: " + paciente.IdadeFormatada + "\n" +
                "Condição clínica: " + paciente.CondicaoClinica + "\n" +
                "Status: " + StatusVacinal + "\n\n" +

                MontarLista("VACINAS EM DIA", emDia) + "\n" +
                MontarLista("VACINAS PENDENTES", pendentes) + "\n" +
                MontarLista("VACINAS DISPONÍVEIS NO SUS", sus) + "\n" +
                MontarLista("VACINAS DISPONÍVEIS NA REDE PRIVADA", privado) + "\n" +
                MontarLista("VACINAS QUE PRECISAM DE AVALIAÇÃO PROFISSIONAL", contraindicadas) + "\n" +
                SugestaoRecuperacao + "\n" +
                "\nObservação: este relatório é um apoio à decisão e não substitui avaliação profissional.";

            return ResultadoAnalise;
        }

        public List<string> IdentificarVacinasPendentes(
            List<RegraVacinal> regras,
            List<HistoricoVacinal> historico)
        {
            List<string> lista = new List<string>();

            foreach (RegraVacinal regra in regras)
            {
                HistoricoVacinal registro = historico.FirstOrDefault(h => h.RegraVacinalId == regra.Id);

                int dosesTomadas = 0;

                if (registro != null)
                    dosesTomadas = registro.DosesTomadas;

                int faltam = regra.DosesNecessarias - dosesTomadas;

                if (faltam > 0)
                    lista.Add(regra.NomeVacina + " — faltam " + faltam + " dose(s)");
            }

            return lista;
        }

        public List<string> IdentificarVacinasEmDia(
            List<RegraVacinal> regras,
            List<HistoricoVacinal> historico)
        {
            List<string> lista = new List<string>();

            foreach (RegraVacinal regra in regras)
            {
                HistoricoVacinal registro = historico.FirstOrDefault(h => h.RegraVacinalId == regra.Id);

                int dosesTomadas = 0;

                if (registro != null)
                    dosesTomadas = registro.DosesTomadas;

                if (dosesTomadas >= regra.DosesNecessarias)
                    lista.Add(regra.NomeVacina + " — esquema completo");
            }

            return lista;
        }

        public List<string> IdentificarVacinasDisponiveisSUS(List<RegraVacinal> regras)
        {
            List<string> lista = new List<string>();

            foreach (RegraVacinal regra in regras)
            {
                if (regra.TipoCalendario == "SUS")
                    lista.Add(regra.NomeVacina);
            }

            return lista;
        }

        public List<string> IdentificarVacinasDisponiveisRedePrivada(List<RegraVacinal> regras)
        {
            List<string> lista = new List<string>();

            foreach (RegraVacinal regra in regras)
            {
                if (regra.TipoCalendario == "Privado")
                    lista.Add(regra.NomeVacina);
            }

            return lista;
        }

        public List<string> GerarPrimeiraVisita(
            Paciente paciente,
            List<RegraVacinal> regras,
            List<HistoricoVacinal> historico)
        {
            List<string> lista = new List<string>();

            foreach (RegraVacinal regra in regras)
            {
                HistoricoVacinal registro = historico.FirstOrDefault(h => h.RegraVacinalId == regra.Id);

                int dosesTomadas = 0;

                if (registro != null)
                    dosesTomadas = registro.DosesTomadas;

                int faltam = regra.DosesNecessarias - dosesTomadas;

                if (faltam > 0 && !PrecisaAvaliacaoAntesAplicar(paciente, regra))
                {
                    lista.Add(regra.NomeVacina + " — aplicar 1 dose na primeira visita");
                }

                if (regra.NomeVacina == "Duplo Adulto" &&
                    dosesTomadas >= regra.DosesNecessarias &&
                    registro != null)
                {
                    DateTime proximoReforco = registro.DataUltimaAplicacao.AddYears(10);

                    if (DateTime.Now >= proximoReforco)
                        lista.Add("Duplo Adulto — aplicar reforço de 10 anos na primeira visita");
                }
            }

            return lista;
        }

        public List<string> GerarSegundaVisita(
            Paciente paciente,
            List<RegraVacinal> regras,
            List<HistoricoVacinal> historico)
        {
            List<string> lista = new List<string>();

            foreach (RegraVacinal regra in regras)
            {
                HistoricoVacinal registro = historico.FirstOrDefault(h => h.RegraVacinalId == regra.Id);

                int dosesTomadas = 0;

                if (registro != null)
                    dosesTomadas = registro.DosesTomadas;

                int faltam = regra.DosesNecessarias - dosesTomadas;

                if (faltam > 1 && !PrecisaAvaliacaoAntesAplicar(paciente, regra))
                {
                    lista.Add(regra.NomeVacina + " — agendar retorno para próxima dose. " + ObterIntervaloSugerido(regra.NomeVacina));
                }
            }

            return lista;
        }

        public List<string> AlertarVacinasContraindicadas(
            Paciente paciente,
            List<RegraVacinal> regras,
            List<HistoricoVacinal> historico)
        {
            List<string> lista = new List<string>();

            foreach (RegraVacinal regra in regras)
            {
                HistoricoVacinal registro = historico.FirstOrDefault(h => h.RegraVacinalId == regra.Id);

                int dosesTomadas = 0;

                if (registro != null)
                    dosesTomadas = registro.DosesTomadas;

                int faltam = regra.DosesNecessarias - dosesTomadas;

                if (faltam > 0 && PrecisaAvaliacaoAntesAplicar(paciente, regra))
                    lista.Add(regra.NomeVacina + " — " + MotivoContraindicacao(paciente, regra));
            }

            return lista;
        }

        private bool PrecisaAvaliacaoAntesAplicar(Paciente paciente, RegraVacinal regra)
        {
            bool vacinaVirusVivo =
                regra.NomeVacina == "Febre Amarela" ||
                regra.NomeVacina == "Tríplice Viral / SCR" ||
                regra.NomeVacina == "Tetraviral" ||
                regra.NomeVacina == "Varicela";

            if (paciente.Gestante && vacinaVirusVivo)
                return true;

            if (paciente.Lactante &&
                paciente.CriancaAmamentadaMenor6Meses &&
                regra.NomeVacina == "Febre Amarela")
                return true;

            if ((paciente.Imunossuprimido ||
                 paciente.HIV ||
                 paciente.Quimioterapia ||
                 paciente.Transplantado ||
                 paciente.UsoCorticoideOuImunobiologico) &&
                vacinaVirusVivo)
                return true;

            if (paciente.AlergiaGrave)
                return true;

            if (paciente.FebreOuDoencaAguda)
                return true;

            return false;
        }

        private string MotivoContraindicacao(Paciente paciente, RegraVacinal regra)
        {
            bool vacinaVirusVivo =
                regra.NomeVacina == "Febre Amarela" ||
                regra.NomeVacina == "Tríplice Viral / SCR" ||
                regra.NomeVacina == "Tetraviral" ||
                regra.NomeVacina == "Varicela";

            if (paciente.Gestante && vacinaVirusVivo)
                return "gestante e vacina de vírus vivo. Avaliação profissional necessária.";

            if (paciente.Lactante &&
                paciente.CriancaAmamentadaMenor6Meses &&
                regra.NomeVacina == "Febre Amarela")
                return "lactante com bebê menor de 6 meses. Avaliar Febre Amarela.";

            if ((paciente.Imunossuprimido ||
                 paciente.HIV ||
                 paciente.Quimioterapia ||
                 paciente.Transplantado ||
                 paciente.UsoCorticoideOuImunobiologico) &&
                vacinaVirusVivo)
                return "imunossupressão/imunodeficiência e vacina de vírus vivo.";

            if (paciente.AlergiaGrave)
                return "alergia grave/anafilaxia informada.";

            if (paciente.FebreOuDoencaAguda)
                return "febre, infecção ou doença aguda. Considerar adiar vacinação.";

            return "condição clínica exige avaliação profissional.";
        }

        private string ObterIntervaloSugerido(string nomeVacina)
        {
            if (nomeVacina == "Hepatite B")
                return "Sugestão: 2ª dose após 30 dias e 3ª dose conforme esquema.";

            if (nomeVacina == "Duplo Adulto")
                return "Sugestão: 2ª dose após 60 dias e 3ª dose cerca de 6 meses após a primeira.";

            if (nomeVacina == "Tríplice Viral / SCR")
                return "Sugestão: respeitar intervalo mínimo entre as doses.";

            if (nomeVacina == "VIP")
                return "Sugestão: seguir intervalo mínimo do calendário vacinal.";

            return "Seguir intervalo recomendado no calendário vacinal.";
        }

        private string MontarLista(string titulo, List<string> itens)
        {
            string texto = titulo + ":\n";

            if (itens.Count == 0)
            {
                texto += "Nenhum item identificado.\n";
                return texto;
            }

            foreach (string item in itens)
                texto += "• " + item + "\n";

            return texto;
        }
    }
}