using maui_amarv.Models;

namespace maui_amarv.Views;

public partial class CadastroPaciente : ContentPage
{
    private bool alterandoCondicoes = false;

    public CadastroPaciente()
    {
        InitializeComponent();

        chk_nenhuma.IsChecked = true;

        ConfigurarToqueNasLinhas();
    }

    private void ConfigurarToqueNasLinhas()
    {
        AdicionarToque(linha_nenhuma, chk_nenhuma);
        AdicionarToque(linha_gestante, chk_gestante);
        AdicionarToque(linha_lactante, chk_lactante);
        AdicionarToque(linha_lactante_bebe, chk_lactante_bebe);
        AdicionarToque(linha_imunossuprimido, chk_imunossuprimido);
        AdicionarToque(linha_hiv, chk_hiv);
        AdicionarToque(linha_quimioterapia, chk_quimioterapia);
        AdicionarToque(linha_transplante, chk_transplante);
        AdicionarToque(linha_corticoide, chk_corticoide);
        AdicionarToque(linha_alergia, chk_alergia);
        AdicionarToque(linha_febre, chk_febre);
    }

    private void AdicionarToque(HorizontalStackLayout linha, CheckBox checkBox)
    {
        var tap = new TapGestureRecognizer();

        tap.Tapped += (s, e) =>
        {
            checkBox.IsChecked = !checkBox.IsChecked;
        };

        linha.GestureRecognizers.Add(tap);
    }

    private void Condicao_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (alterandoCondicoes)
            return;

        alterandoCondicoes = true;

        CheckBox check = sender as CheckBox;

        if (check == chk_nenhuma && chk_nenhuma.IsChecked)
        {
            DesmarcarTodasMenosNenhuma();
        }
        else
        {
            if (check != chk_nenhuma && check.IsChecked)
            {
                chk_nenhuma.IsChecked = false;
            }

            if (!ExisteAlgumaCondicaoMarcada())
            {
                chk_nenhuma.IsChecked = true;
            }
        }

        alterandoCondicoes = false;
    }

    private bool ExisteAlgumaCondicaoMarcada()
    {
        return chk_gestante.IsChecked ||
               chk_lactante.IsChecked ||
               chk_lactante_bebe.IsChecked ||
               chk_imunossuprimido.IsChecked ||
               chk_hiv.IsChecked ||
               chk_quimioterapia.IsChecked ||
               chk_transplante.IsChecked ||
               chk_corticoide.IsChecked ||
               chk_alergia.IsChecked ||
               chk_febre.IsChecked;
    }

    private void DesmarcarTodasMenosNenhuma()
    {
        chk_gestante.IsChecked = false;
        chk_lactante.IsChecked = false;
        chk_lactante_bebe.IsChecked = false;
        chk_imunossuprimido.IsChecked = false;
        chk_hiv.IsChecked = false;
        chk_quimioterapia.IsChecked = false;
        chk_transplante.IsChecked = false;
        chk_corticoide.IsChecked = false;
        chk_alergia.IsChecked = false;
        chk_febre.IsChecked = false;
    }

    private string ObterCondicoesClinicas()
    {
        List<string> condicoes = new List<string>();

        if (chk_nenhuma.IsChecked)
            condicoes.Add("Nenhuma");

        if (chk_gestante.IsChecked)
            condicoes.Add("Gestante");

        if (chk_lactante.IsChecked)
            condicoes.Add("Lactante");

        if (chk_lactante_bebe.IsChecked)
            condicoes.Add("Lactante com bebê menor de 6 meses");

        if (chk_imunossuprimido.IsChecked)
            condicoes.Add("Imunossuprimido");

        if (chk_hiv.IsChecked)
            condicoes.Add("HIV");

        if (chk_quimioterapia.IsChecked)
            condicoes.Add("Quimioterapia");

        if (chk_transplante.IsChecked)
            condicoes.Add("Transplantado");

        if (chk_corticoide.IsChecked)
            condicoes.Add("Uso de corticoide ou imunobiológico");

        if (chk_alergia.IsChecked)
            condicoes.Add("Alergia grave / Anafilaxia");

        if (chk_febre.IsChecked)
            condicoes.Add("Febre, infecção ou doença aguda");

        if (condicoes.Count == 0)
            return "Nenhuma";

        return string.Join("; ", condicoes);
    }

    private int ObterSemanasGestacao()
    {
        if (string.IsNullOrWhiteSpace(entry_semanas_gestacao.Text))
            return 0;

        return Convert.ToInt32(entry_semanas_gestacao.Text);
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(entry_nome.Text))
            {
                await DisplayAlert("Atenção", "Digite o nome do paciente.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(entry_idade_anos.Text))
            {
                await DisplayAlert("Atenção", "Digite a idade em anos.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(entry_idade_meses.Text))
            {
                entry_idade_meses.Text = "0";
            }

            int idadeAnos = Convert.ToInt32(entry_idade_anos.Text);
            int idadeMeses = Convert.ToInt32(entry_idade_meses.Text);

            if (idadeAnos < 0)
            {
                await DisplayAlert("Atenção", "A idade não pode ser negativa.", "OK");
                return;
            }

            if (idadeMeses < 0 || idadeMeses > 11)
            {
                await DisplayAlert("Atenção", "Os meses adicionais devem estar entre 0 e 11.", "OK");
                return;
            }

            Paciente paciente = new Paciente
            {
                Nome = entry_nome.Text,
                NomeMae = entry_nome_mae.Text,
                Cpf = entry_cpf.Text,
                Telefone = entry_telefone.Text,
                Rua = entry_rua.Text,
                Endereco = entry_endereco.Text,
                CartaoSUS = entry_cartao_sus.Text,
                Sexo = pck_sexo.SelectedItem == null ? "" : pck_sexo.SelectedItem.ToString(),

                IdadeAnos = idadeAnos,
                IdadeMesesAdicionais = idadeMeses,
                CondicaoClinica = ObterCondicoesClinicas(),

                Gestante = chk_gestante.IsChecked,
                SemanasGestacao = ObterSemanasGestacao(),
                Lactante = chk_lactante.IsChecked,
                CriancaAmamentadaMenor6Meses = chk_lactante_bebe.IsChecked,
                Imunossuprimido = chk_imunossuprimido.IsChecked,
                HIV = chk_hiv.IsChecked,
                Quimioterapia = chk_quimioterapia.IsChecked,
                Transplantado = chk_transplante.IsChecked,
                UsoCorticoideOuImunobiologico = chk_corticoide.IsChecked,
                AlergiaGrave = chk_alergia.IsChecked,
                FebreOuDoencaAguda = chk_febre.IsChecked
            };

            await App.Database.Pacientes.Salvar(paciente);

            await DisplayAlert("Sucesso", "Paciente cadastrado com sucesso.", "OK");

            await Navigation.PushAsync(new HistoricoVacinalPage());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void Button_Consultar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ConsultaPacientes());
    }

    private async void Button_Menu_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}