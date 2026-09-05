using maui_amarv.Models;

namespace maui_amarv.Views;

public partial class BackupPage : ContentPage
{
    private UsuarioSistema usuarioLogado;

    public BackupPage(UsuarioSistema usuario)
    {
        InitializeComponent();

        usuarioLogado = usuario;

        lbl_usuario.Text = "Usuário: " + usuarioLogado.Nome + " | Perfil: " + usuarioLogado.Perfil;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (usuarioLogado.Perfil != "Administrador")
        {
            await DisplayAlert(
                "Acesso negado",
                "Somente o administrador pode realizar backup.",
                "OK");

            await Navigation.PopAsync();
        }
    }

    private async void Button_Backup_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (usuarioLogado.Perfil != "Administrador")
            {
                await DisplayAlert("Acesso negado", "Somente administrador pode realizar backup.", "OK");
                return;
            }

            string pasta = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "AmarV");

            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            string banco = Path.Combine(pasta, "amarv.db3");

            if (!File.Exists(banco))
            {
                await DisplayAlert("Erro", "Banco de dados não encontrado.", "OK");
                return;
            }

            string nomeBackup = "backup_amarv_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".db3";
            string caminhoBackup = Path.Combine(pasta, nomeBackup);

            File.Copy(banco, caminhoBackup, true);

            Backup backup = new Backup
            {
                DataBackup = DateTime.Now,
                Status = "Backup realizado",
                LocalArquivo = caminhoBackup
            };

            backup.RealizarBackup();

            backup.LocalArquivo = caminhoBackup;

            await App.Database.Backup.Salvar(backup);

            lbl_status.Text =
                "Último backup: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") +
                "\nArquivo: " + nomeBackup;

            await DisplayAlert(
                "Backup realizado",
                "Backup salvo em Documentos > AmarV:\n" + nomeBackup,
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