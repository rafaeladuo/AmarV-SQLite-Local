using maui_amarv.Models;

namespace maui_amarv.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void Button_Entrar_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(entry_login.Text))
            {
                await DisplayAlert("Atenção", "Digite o login.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(entry_senha.Text))
            {
                await DisplayAlert("Atenção", "Digite a senha.", "OK");
                return;
            }

            UsuarioSistema usuario = await App.Database.BuscarUsuario(entry_login.Text, entry_senha.Text);

            if (usuario == null)
            {
                await DisplayAlert("Erro", "Login ou senha inválidos.", "OK");
                return;
            }

            await Navigation.PushAsync(new MenuPrincipal(usuario));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }
}