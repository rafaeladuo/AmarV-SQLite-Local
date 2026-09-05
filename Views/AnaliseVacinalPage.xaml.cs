using System.Net;
using maui_amarv.Models;
using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace maui_amarv.Views;

public partial class AnaliseVacinalPage : ContentPage
{
    private string textoAnalise = "";

    public AnaliseVacinalPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        pck_paciente.ItemsSource = await App.Database.Pacientes.Listar();
    }

    private async void pck_paciente_SelectedIndexChanged(object sender, EventArgs e)
    {
        await GerarAnalise();
    }

    private async Task GerarAnalise()
    {
        if (pck_paciente.SelectedItem == null)
            return;

        Paciente paciente = (Paciente)pck_paciente.SelectedItem;

        var regras = await App.Database.ListarRegrasPorIdade(paciente.IdadeTotalMeses);
        var historico = await App.Database.HistoricoVacinal.ListarPorPaciente(paciente.Id);

        AnaliseEsquemaVacinal analise = new AnaliseEsquemaVacinal();

        textoAnalise = analise.AnalisarEsquemaVacinal(paciente, regras, historico);

        lbl_analise.Text = textoAnalise;

        await App.Database.SalvarAnaliseVacinal(analise);
    }

    private async void Button_Compartilhar_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(textoAnalise))
        {
            await DisplayAlert("Atenção", "Gere uma análise primeiro.", "OK");
            return;
        }

        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Title = "Análise Vacinal Amar V",
            Text = textoAnalise
        });
    }

    private async void Button_Imprimir_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(textoAnalise))
            {
                await DisplayAlert("Atenção", "Gere uma análise primeiro.", "OK");
                return;
            }

            string pasta = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "AmarV");

            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            string caminhoArquivo = Path.Combine(pasta, "analise_vacinal.html");

            string html = @"
<html>
<head>
<meta charset='utf-8'>
<title>Análise Vacinal Amar V</title>
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
<h1>Amar V - Análise Vacinal</h1>
<pre>" + WebUtility.HtmlEncode(textoAnalise) + @"</pre>
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