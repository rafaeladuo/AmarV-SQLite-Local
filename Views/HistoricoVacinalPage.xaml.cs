using maui_amarv.Models;

namespace maui_amarv.Views;

public partial class HistoricoVacinalPage : ContentPage
{
    private Dictionary<RegraVacinal, Picker> pickers = new Dictionary<RegraVacinal, Picker>();
    private Dictionary<RegraVacinal, DatePicker> datas = new Dictionary<RegraVacinal, DatePicker>();

    public HistoricoVacinalPage()
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
        await CarregarVacinasDoPaciente();
    }

    private async Task CarregarVacinasDoPaciente()
    {
        layout_vacinas.Children.Clear();
        pickers.Clear();
        datas.Clear();

        if (pck_paciente.SelectedItem == null)
            return;

        Paciente paciente = (Paciente)pck_paciente.SelectedItem;

        lbl_idade.Text = "Idade: " + paciente.IdadeFormatada;

        var regras = await App.Database.ListarRegrasPorIdade(paciente.IdadeTotalMeses);
        var historico = await App.Database.HistoricoVacinal.ListarPorPaciente(paciente.Id);

        foreach (var regra in regras)
        {
            int dosesRegistradas = 0;
            DateTime dataUltimaAplicacao = DateTime.Now;

            var registro = historico.FirstOrDefault(h => h.RegraVacinalId == regra.Id);

            if (registro != null)
            {
                dosesRegistradas = registro.DosesTomadas;
                dataUltimaAplicacao = registro.DataUltimaAplicacao;
            }

            AdicionarCampoVacina(regra, dosesRegistradas, dataUltimaAplicacao);
        }

        if (regras.Count == 0)
        {
            layout_vacinas.Children.Add(new Label
            {
                Text = "Nenhuma vacina indicada para a idade informada.",
                TextColor = Colors.Black
            });
        }
    }

    private void AdicionarCampoVacina(RegraVacinal regra, int dosesRegistradas, DateTime dataUltimaAplicacao)
    {
        Label titulo = new Label
        {
            Text = regra.NomeVacina + " — necessário: " + regra.DosesNecessarias + " dose(s)",
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.Black
        };

        Label observacao = new Label
        {
            Text = regra.Observacao,
            FontSize = 12,
            TextColor = Colors.Gray
        };

        Picker picker = new Picker
        {
            Title = "Quantas doses tomou?",
            BackgroundColor = Colors.White,
            TextColor = Colors.Black,
            TitleColor = Colors.Gray
        };

        for (int i = 0; i <= regra.DosesNecessarias; i++)
        {
            picker.Items.Add(i + " dose(s)");
        }

        if (dosesRegistradas >= 0 && dosesRegistradas <= regra.DosesNecessarias)
            picker.SelectedIndex = dosesRegistradas;
        else
            picker.SelectedIndex = 0;

        DatePicker dataPicker = new DatePicker
        {
            Format = "dd/MM/yyyy",
            Date = dataUltimaAplicacao,
            TextColor = Colors.Black,
            BackgroundColor = Colors.White
        };

        Frame frame = new Frame
        {
            Padding = 12,
            BackgroundColor = Colors.White,
            BorderColor = Color.FromArgb("#CCCCCC"),
            Content = new VerticalStackLayout
            {
                Spacing = 5,
                Children =
                {
                    titulo,
                    observacao,
                    new Label { Text = "Data da última aplicação:", TextColor = Colors.Black },
                    dataPicker,
                    picker
                }
            }
        };

        layout_vacinas.Children.Add(frame);
        pickers[regra] = picker;
        datas[regra] = dataPicker;
    }

    private async void Button_Salvar_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (pck_paciente.SelectedItem == null)
            {
                await DisplayAlert("Atenção", "Selecione um paciente.", "OK");
                return;
            }

            Paciente paciente = (Paciente)pck_paciente.SelectedItem;

            await App.Database.HistoricoVacinal.ExcluirPorPaciente(paciente.Id);

            foreach (var item in pickers)
            {
                RegraVacinal regra = item.Key;
                Picker picker = item.Value;

                int dosesTomadas = picker.SelectedIndex;

                HistoricoVacinal historico = new HistoricoVacinal
                {
                    PacienteId = paciente.Id,
                    NomePaciente = paciente.Nome,
                    RegraVacinalId = regra.Id,
                    NomeVacina = regra.NomeVacina,
                    DosesTomadas = dosesTomadas,
                    DataUltimaAplicacao = datas[regra].Date.Value,
                    DataRegistro = DateTime.Now,
                    Observacao = regra.Observacao
                };

                await App.Database.HistoricoVacinal.Salvar(historico);
            }

            await DisplayAlert("Sucesso", "Histórico vacinal salvo com sucesso.", "OK");

            await Navigation.PushAsync(new AnaliseVacinalPage());
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