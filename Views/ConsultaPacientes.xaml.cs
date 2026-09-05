using maui_amarv.Models;

namespace maui_amarv.Views;

public partial class ConsultaPacientes : ContentPage
{
    public ConsultaPacientes()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        collection_pacientes.ItemsSource = await App.Database.Pacientes.Listar();
    }

    private async void Button_Editar_Clicked(object sender, EventArgs e)
    {
        try
        {
            Button botao = (Button)sender;
            Paciente paciente = (Paciente)botao.BindingContext;

            await Navigation.PushAsync(new EditarPacientePage(paciente));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void Button_Deletar_Clicked(object sender, EventArgs e)
    {
        try
        {
            Button botao = (Button)sender;
            Paciente paciente = (Paciente)botao.BindingContext;

            bool confirmar = await DisplayAlert(
                "Confirmar exclusão",
                "Deseja deletar o cadastro de " + paciente.Nome + "? O histórico vacinal também será apagado.",
                "Sim",
                "Não");

            if (!confirmar)
                return;

            await App.Database.Pacientes.ExcluirComHistorico(paciente);

            await DisplayAlert("Sucesso", "Cadastro deletado.", "OK");

            collection_pacientes.ItemsSource = await App.Database.Pacientes.Listar();
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

    private async void Button_Menu_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}