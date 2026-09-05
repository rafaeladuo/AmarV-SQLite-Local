using maui_amarv.Models;

namespace maui_amarv.Views;

public partial class SincronizacaoPage : ContentPage
{
    private UsuarioSistema usuarioLogado;

    public SincronizacaoPage(UsuarioSistema usuario)
    {
        InitializeComponent();

        usuarioLogado = usuario;
    }

    private async void Button_Sincronizar_Clicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PushAsync(
            new SincronizacaoBackup(usuarioLogado));
    }

    private async void Button_Menu_Clicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}