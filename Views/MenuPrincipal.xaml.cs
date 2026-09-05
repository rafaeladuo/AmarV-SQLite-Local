using maui_amarv.Models;

namespace maui_amarv.Views;

public partial class MenuPrincipal : ContentPage
{
    private UsuarioSistema usuarioLogado;

    public MenuPrincipal(UsuarioSistema usuario)
    {
        InitializeComponent();

        usuarioLogado = usuario;

        lbl_usuario.Text = "Usuário: " + usuario.Nome + " | Perfil: " + usuario.Perfil;

        ConfigurarMenuPorPerfil();
    }

    private void ConfigurarMenuPorPerfil()
    {
        if (usuarioLogado.Perfil == "Usuario")
        {
            // Usuário do Sistema
            btn_cadastro.IsVisible = true;
            btn_historico.IsVisible = true;
            btn_analise.IsVisible = true;
            btn_consulta.IsVisible = true;
            btn_calendario.IsVisible = true;
            btn_relatorios.IsVisible = true;
            btn_sincronizacao.IsVisible = true;

            // Administrador
            btn_vacinas.IsVisible = false;
            btn_atualizar_calendario.IsVisible = false;
            btn_usuarios.IsVisible = false;
            btn_backup.IsVisible = false;
        }
        else if (usuarioLogado.Perfil == "Administrador")
        {
            // Administrador
            btn_cadastro.IsVisible = false;
            btn_historico.IsVisible = false;
            btn_analise.IsVisible = false;
            btn_consulta.IsVisible = false;
            btn_calendario.IsVisible = false;

            btn_relatorios.IsVisible = true;
            btn_sincronizacao.IsVisible = true;
            btn_vacinas.IsVisible = true;
            btn_atualizar_calendario.IsVisible = true;
            btn_usuarios.IsVisible = true;
            btn_backup.IsVisible = true;
        }
        else
        {
            btn_cadastro.IsVisible = false;
            btn_historico.IsVisible = false;
            btn_analise.IsVisible = false;
            btn_consulta.IsVisible = false;
            btn_calendario.IsVisible = false;
            btn_relatorios.IsVisible = false;
            btn_sincronizacao.IsVisible = false;
            btn_vacinas.IsVisible = false;
            btn_atualizar_calendario.IsVisible = false;
            btn_usuarios.IsVisible = false;
            btn_backup.IsVisible = false;
        }
    }

    private async void Button_Cadastro_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CadastroPaciente());
    }

    private async void Button_Historico_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HistoricoVacinalPage());
    }

    private async void Button_Analise_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AnaliseVacinalPage());
    }

    private async void Button_Consulta_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ConsultaPacientes());
    }

    private async void Button_Calendario_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CalendarioVacinal());
    }

    private async void Button_Relatorios_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RelatoriosPage(usuarioLogado));
    }

    private async void Button_Sincronizacao_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SincronizacaoPage(usuarioLogado));
    }

    private async void Button_Vacinas_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ConsultaVacinas());
    }

    private async void Button_AtualizarCalendario_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AtualizarCalendarioPage());
    }

    private async void Button_Usuarios_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GerenciarUsuariosPage());
    }

    private async void Button_Backup_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new BackupPage(usuarioLogado));
    }

    private void Button_Sair_Clicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new LoginPage());
    }
}