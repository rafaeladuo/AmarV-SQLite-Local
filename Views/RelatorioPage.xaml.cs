using System.Net;
using System.Text;
using maui_amarv.Models;
using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace maui_amarv.Views;

public partial class RelatoriosPage : ContentPage
{
    private UsuarioSistema usuarioLogado;
    private string textoRelatorio = "";
    private List<Paciente> pacientesDoRelatorio = new List<Paciente>();

    public RelatoriosPage(UsuarioSistema usuario)
    {
        InitializeComponent();

        usuarioLogado = usuario;

        ConfigurarTelaPorPerfil();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        pck_paciente.ItemsSource = await App.Database.Pacientes.Listar();
    }

    private void ConfigurarTelaPorPerfil()
    {
        lbl_perfil.Text = "Perfil: " + usuarioLogado.Perfil;

        if (usuarioLogado.Perfil == "Usuario")
        {
            area_usuario.IsVisible = true;
            area_admin.IsVisible = false;
        }
        else if (usuarioLogado.Perfil == "Administrador")
        {
            area_usuario.IsVisible = false;
            area_admin.IsVisible = true;
        }
        else
        {
            area_usuario.IsVisible = false;
            area_admin.IsVisible = false;
        }
    }

    private async Task<List<Paciente>> ObterPacientesFiltrados()
    {
        List<Paciente> pacientes = await App.Database.Pacientes.Listar();

        string filtroRua = entry_filtro_rua.Text;
        string filtroMae = entry_filtro_mae.Text;
        string filtroIdadeTexto = entry_filtro_idade.Text;

        if (!string.IsNullOrWhiteSpace(filtroRua))
        {
            pacientes = pacientes
                .Where(p => !string.IsNullOrWhiteSpace(p.Rua) &&
                            p.Rua.ToLower().Contains(filtroRua.ToLower()))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(filtroMae))
        {
            pacientes = pacientes
                .Where(p => !string.IsNullOrWhiteSpace(p.NomeMae) &&
                            p.NomeMae.ToLower().Contains(filtroMae.ToLower()))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(filtroIdadeTexto))
        {
            int idade = Convert.ToInt32(filtroIdadeTexto);

            pacientes = pacientes
                .Where(p => p.IdadeAnos == idade)
                .ToList();
        }

        return pacientes;
    }

    private async void Button_RelatorioPaciente_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (pck_paciente.SelectedItem == null)
            {
                await DisplayAlert("Atenção", "Selecione um paciente.", "OK");
                return;
            }

            Paciente paciente = (Paciente)pck_paciente.SelectedItem;

            var regras = await App.Database.ListarRegrasPorIdade(paciente.IdadeTotalMeses);
            var historico = await App.Database.HistoricoVacinal.ListarPorPaciente(paciente.Id);

            AnaliseEsquemaVacinal analise = new AnaliseEsquemaVacinal();

            string resultadoAnalise = analise.AnalisarEsquemaVacinal(paciente, regras, historico);

            textoRelatorio =
                "RELATÓRIO DE SITUAÇÃO VACINAL DO PACIENTE\n\n" +
                "Nome: " + paciente.Nome + "\n" +
                "Nome da mãe: " + paciente.NomeMae + "\n" +
                "Idade: " + paciente.IdadeFormatada + "\n" +
                "Rua: " + paciente.Rua + "\n" +
                "Endereço: " + paciente.Endereco + "\n" +
                "Telefone: " + paciente.Telefone + "\n" +
                "Cartão SUS: " + paciente.CartaoSUS + "\n\n" +
                resultadoAnalise;

            lbl_relatorio.Text = textoRelatorio;

            pacientesDoRelatorio = new List<Paciente> { paciente };

            RelatorioVacinalPaciente relatorio = new RelatorioVacinalPaciente();
            relatorio.GerarRelatorioVacinalPaciente(paciente);
            relatorio.Conteudo = textoRelatorio;

            await App.Database.SalvarRelatorioVacinalPaciente(relatorio);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void Button_Pendentes_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (usuarioLogado.Perfil != "Administrador")
            {
                await DisplayAlert("Acesso negado", "Relatório administrativo permitido apenas para administrador.", "OK");
                return;
            }

            pacientesDoRelatorio = new List<Paciente>();
            List<string> lista = new List<string>();

            var pacientes = await ObterPacientesFiltrados();

            foreach (Paciente paciente in pacientes)
            {
                var regras = await App.Database.ListarRegrasPorIdade(paciente.IdadeTotalMeses);
                var historico = await App.Database.HistoricoVacinal.ListarPorPaciente(paciente.Id);

                int pendencias = CalcularPendencias(regras, historico);

                if (pendencias > 0)
                {
                    pacientesDoRelatorio.Add(paciente);

                    lista.Add(
                        "Paciente: " + paciente.Nome + "\n" +
                        "Nome da mãe: " + paciente.NomeMae + "\n" +
                        "Idade: " + paciente.IdadeFormatada + "\n" +
                        "Rua: " + paciente.Rua + "\n" +
                        "Telefone: " + paciente.Telefone + "\n" +
                        "Pendências: " + pendencias + "\n");
                }
            }

            textoRelatorio =
                "RELATÓRIO DE PACIENTES COM VACINAS ATRASADAS/PENDENTES\n\n" +
                "Total encontrado: " + pacientesDoRelatorio.Count + "\n\n" +
                string.Join("\n----------------------\n", lista);

            if (pacientesDoRelatorio.Count == 0)
                textoRelatorio += "Nenhum paciente encontrado.";

            lbl_relatorio.Text = textoRelatorio;

            RelatorioPacienteComVacinasAtrasadas relatorio = new RelatorioPacienteComVacinasAtrasadas();
            relatorio.GerarRelatorioPacientesAtrasados(pacientesDoRelatorio.Count);
            relatorio.Conteudo = textoRelatorio;

            await App.Database.SalvarRelatorioPacientesAtrasados(relatorio);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void Button_Regulares_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (usuarioLogado.Perfil != "Administrador")
            {
                await DisplayAlert("Acesso negado", "Relatório administrativo permitido apenas para administrador.", "OK");
                return;
            }

            pacientesDoRelatorio = new List<Paciente>();
            List<string> lista = new List<string>();

            var pacientes = await ObterPacientesFiltrados();

            foreach (Paciente paciente in pacientes)
            {
                var regras = await App.Database.ListarRegrasPorIdade(paciente.IdadeTotalMeses);
                var historico = await App.Database.HistoricoVacinal.ListarPorPaciente(paciente.Id);

                int pendencias = CalcularPendencias(regras, historico);

                if (pendencias == 0)
                {
                    pacientesDoRelatorio.Add(paciente);

                    lista.Add(
                        "Paciente: " + paciente.Nome + "\n" +
                        "Nome da mãe: " + paciente.NomeMae + "\n" +
                        "Idade: " + paciente.IdadeFormatada + "\n" +
                        "Rua: " + paciente.Rua + "\n" +
                        "Telefone: " + paciente.Telefone + "\n" +
                        "Situação: Regular\n");
                }
            }

            textoRelatorio =
                "RELATÓRIO DE PACIENTES EM SITUAÇÃO REGULAR\n\n" +
                "Total encontrado: " + pacientesDoRelatorio.Count + "\n\n" +
                string.Join("\n----------------------\n", lista);

            if (pacientesDoRelatorio.Count == 0)
                textoRelatorio += "Nenhum paciente encontrado.";

            lbl_relatorio.Text = textoRelatorio;

            RelatorioPacientesEmSituacaoRegular relatorio = new RelatorioPacientesEmSituacaoRegular();
            relatorio.GerarRelatorioSituacaoRegular(pacientesDoRelatorio.Count);
            relatorio.Conteudo = textoRelatorio;

            await App.Database.SalvarRelatorioPacientesRegulares(relatorio);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void Button_Geral_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (usuarioLogado.Perfil != "Administrador")
            {
                await DisplayAlert("Acesso negado", "Relatório administrativo permitido apenas para administrador.", "OK");
                return;
            }

            pacientesDoRelatorio = await ObterPacientesFiltrados();

            int totalPacientes = pacientesDoRelatorio.Count;
            int totalPendentes = 0;
            int totalRegulares = 0;

            foreach (Paciente paciente in pacientesDoRelatorio)
            {
                var regras = await App.Database.ListarRegrasPorIdade(paciente.IdadeTotalMeses);
                var historico = await App.Database.HistoricoVacinal.ListarPorPaciente(paciente.Id);

                int pendencias = CalcularPendencias(regras, historico);

                if (pendencias > 0)
                    totalPendentes++;
                else
                    totalRegulares++;
            }

            textoRelatorio =
                "RELATÓRIO GERAL ADMINISTRATIVO\n\n" +
                "Data: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "\n\n" +
                "Filtro rua: " + entry_filtro_rua.Text + "\n" +
                "Filtro idade: " + entry_filtro_idade.Text + "\n" +
                "Filtro nome da mãe: " + entry_filtro_mae.Text + "\n\n" +
                "Total de pacientes encontrados: " + totalPacientes + "\n" +
                "Pacientes com pendências/atrasos: " + totalPendentes + "\n" +
                "Pacientes em situação regular: " + totalRegulares + "\n";

            lbl_relatorio.Text = textoRelatorio;

            Relatorio relatorio = new Relatorio
            {
                DataGeracao = DateTime.Now,
                Periodo = "Geral",
                Conteudo = textoRelatorio
            };

            await App.Database.SalvarRelatorio(relatorio);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private int CalcularPendencias(List<RegraVacinal> regras, List<HistoricoVacinal> historico)
    {
        int pendencias = 0;

        foreach (RegraVacinal regra in regras)
        {
            HistoricoVacinal registro = historico.FirstOrDefault(h => h.RegraVacinalId == regra.Id);

            int dosesTomadas = 0;

            if (registro != null)
                dosesTomadas = registro.DosesTomadas;

            if (dosesTomadas < regra.DosesNecessarias)
                pendencias++;
        }

        return pendencias;
    }

    private async void Button_Excel_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (pacientesDoRelatorio.Count == 0)
            {
                await DisplayAlert("Atenção", "Gere um relatório primeiro.", "OK");
                return;
            }

            string pasta = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "AmarV");

            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            string caminhoArquivo = Path.Combine(
                pasta,
                "relatorio_amarv_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");

            StringBuilder csv = new StringBuilder();

            csv.AppendLine("Nome;Nome da Mae;Idade;Rua;Endereco;Telefone;CPF;Cartao SUS;Condicao Clinica");

            foreach (Paciente paciente in pacientesDoRelatorio)
            {
                csv.AppendLine(
                    PrepararCsv(paciente.Nome) + ";" +
                    PrepararCsv(paciente.NomeMae) + ";" +
                    PrepararCsv(paciente.IdadeFormatada) + ";" +
                    PrepararCsv(paciente.Rua) + ";" +
                    PrepararCsv(paciente.Endereco) + ";" +
                    PrepararCsv(paciente.Telefone) + ";" +
                    PrepararCsv(paciente.Cpf) + ";" +
                    PrepararCsv(paciente.CartaoSUS) + ";" +
                    PrepararCsv(paciente.CondicaoClinica));
            }

            File.WriteAllText(caminhoArquivo, csv.ToString(), Encoding.UTF8);

            await Launcher.Default.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(caminhoArquivo)
            });

            await DisplayAlert(
                "Exportado",
                "Arquivo CSV exportado em Documentos > AmarV.",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private string PrepararCsv(string valor)
    {
        if (valor == null)
            return "";

        valor = valor.Replace(";", ",");
        valor = valor.Replace("\n", " ");
        valor = valor.Replace("\r", " ");

        return valor;
    }

    private async void Button_Compartilhar_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(textoRelatorio))
        {
            await DisplayAlert("Atenção", "Gere um relatório primeiro.", "OK");
            return;
        }

        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Title = "Relatório Amar V",
            Text = textoRelatorio
        });
    }

    private async void Button_Imprimir_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(textoRelatorio))
            {
                await DisplayAlert("Atenção", "Gere um relatório primeiro.", "OK");
                return;
            }

            string pasta = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "AmarV");

            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            string caminhoArquivo = Path.Combine(pasta, "relatorio_amarv.html");

            string html = @"
<html>
<head>
<meta charset='utf-8'>
<title>Relatório Amar V</title>
<style>
body {
    font-family: Arial, sans-serif;
    margin: 40px;
    color: #222;
}
h1 {
    color: #0A4A9E;
    text-align: center;
}
pre {
    font-family: Arial, sans-serif;
    white-space: pre-wrap;
    font-size: 16px;
}
</style>
</head>
<body>
<h1>Amar V - Relatório</h1>
<pre>" + WebUtility.HtmlEncode(textoRelatorio) + @"</pre>
</body>
</html>";

            File.WriteAllText(caminhoArquivo, html);

            await Launcher.Default.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(caminhoArquivo)
            });

            await DisplayAlert(
                "Relatório gerado",
                "O relatório foi salvo em Documentos > AmarV. Abra e use Ctrl + P para imprimir.",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void Button_Menu_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}