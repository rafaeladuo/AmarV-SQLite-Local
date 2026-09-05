using maui_amarv.Models;

namespace maui_amarv.Views;

public partial class EditarPacientePage : ContentPage
{
    private Paciente pacienteAtual;

    public EditarPacientePage(Paciente paciente)
    {
        InitializeComponent();

        pacienteAtual = paciente;

        entry_nome.Text = pacienteAtual.Nome;
        entry_idade_anos.Text = pacienteAtual.IdadeAnos.ToString();
        entry_idade_meses.Text = pacienteAtual.IdadeMesesAdicionais.ToString();
        entry_condicao.Text = pacienteAtual.CondicaoClinica;

        chk_gestante.IsChecked = pacienteAtual.Gestante;
        entry_semanas_gestacao.Text = pacienteAtual.SemanasGestacao.ToString();
        layout_gestante.IsVisible = pacienteAtual.Gestante;

        chk_lactante.IsChecked = pacienteAtual.Lactante;
        chk_crianca_menor_6_meses.IsChecked = pacienteAtual.CriancaAmamentadaMenor6Meses;
        layout_lactante.IsVisible = pacienteAtual.Lactante;
    }

    private void chk_gestante_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        layout_gestante.IsVisible = chk_gestante.IsChecked;

        if (!chk_gestante.IsChecked)
        {
            entry_semanas_gestacao.Text = "0";
        }
    }

    private void chk_lactante_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        layout_lactante.IsVisible = chk_lactante.IsChecked;

        if (!chk_lactante.IsChecked)
        {
            chk_crianca_menor_6_meses.IsChecked = false;
        }
    }

    private async void Button_Salvar_Clicked(object sender, EventArgs e)
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

            int semanasGestacao = 0;

            if (chk_gestante.IsChecked && !string.IsNullOrWhiteSpace(entry_semanas_gestacao.Text))
            {
                semanasGestacao = Convert.ToInt32(entry_semanas_gestacao.Text);
            }

            if (semanasGestacao < 0 || semanasGestacao > 42)
            {
                await DisplayAlert("Atenção", "As semanas de gestação devem estar entre 0 e 42.", "OK");
                return;
            }

            pacienteAtual.Nome = entry_nome.Text;
            pacienteAtual.IdadeAnos = idadeAnos;
            pacienteAtual.IdadeMesesAdicionais = idadeMeses;
            pacienteAtual.CondicaoClinica = entry_condicao.Text;
            pacienteAtual.Gestante = chk_gestante.IsChecked;
            pacienteAtual.SemanasGestacao = semanasGestacao;
            pacienteAtual.Lactante = chk_lactante.IsChecked;
            pacienteAtual.CriancaAmamentadaMenor6Meses = chk_crianca_menor_6_meses.IsChecked;

            await App.Database.Pacientes.Atualizar(pacienteAtual);

            await DisplayAlert("Sucesso", "Cadastro atualizado.", "OK");

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void Button_Voltar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}