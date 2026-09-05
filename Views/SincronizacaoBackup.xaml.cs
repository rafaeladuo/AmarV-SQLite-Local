using System.Text;
using System.Text.Json;
using maui_amarv.Models;

namespace maui_amarv.Views;

public partial class SincronizacaoBackup : ContentPage
{
    private UsuarioSistema usuarioLogado;

    private IDispatcherTimer? _relogioTimer;


    // =====================================================
    // CONFIGURAÇÃO DO SERVIDOR
    // =====================================================

    private const string IpPadrao = "192.168.88.254";

    private string IpServidor
    {
        get
        {
            return Preferences.Get(
                "IpServidor",
                IpPadrao);
        }
    }

    private string UrlBackup =>
        $"http://{IpServidor}/amarv/api/backup.php";

    private string UrlRestaurar =>
        $"http://{IpServidor}/amarv/api/restaurar.php";

    private string UrlSincronizacao =>
        $"http://{IpServidor}/amarv/api/sincronizar.php";

    // =====================================================
    // SALVAR IP DO SERVIDOR
    // =====================================================

    private async void Button_SalvarIp_Clicked(
        object sender,
        EventArgs e)
    {
        try
        {
            string ip = txt_IpServidor.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(ip))
            {
                await DisplayAlert(
                    "IP inválido",
                    "Digite o IP do servidor.",
                    "OK");

                return;
            }

            if (!System.Net.IPAddress.TryParse(ip, out _))
            {
                await DisplayAlert(
                    "IP inválido",
                    "Digite um endereço IP válido.\n\n" +
                    "Exemplo:\n" +
                    "192.168.88.254",
                    "OK");

                return;
            }

            Preferences.Set(
                "IpServidor",
                ip);

            lbl_status.Text =
                "IP do servidor salvo:\n" +
                ip;

            await DisplayAlert(
                "Servidor",
                "IP do servidor salvo com sucesso.\n\n" +
                "IP:\n" +
                ip,
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Erro",
                "Não foi possível salvar o IP.\n\n" +
                ex.Message,
                "OK");
        }
    }

    // =====================================================
    // CONSTRUTOR
    // =====================================================

    public SincronizacaoBackup(UsuarioSistema usuario)
    {
        InitializeComponent();

        usuarioLogado = usuario;

        IniciarRelogio();
    }


    // =====================================================
    // RELÓGIO
    // =====================================================

    private void IniciarRelogio()
    {
        AtualizarRelogio();

        _relogioTimer =
            Dispatcher.CreateTimer();

        _relogioTimer.Interval =
            TimeSpan.FromSeconds(1);

        _relogioTimer.Tick += (s, e) =>
        {
            AtualizarRelogio();
        };

        _relogioTimer.Start();
    }


    private void AtualizarRelogio()
    {
        lbl_relogio.Text =
            DateTime.Now.ToString(
                "dd/MM/yyyy HH:mm:ss");
    }


    // =====================================================
    // CLIENTE HTTP
    // =====================================================

    private HttpClient CriarCliente()
    {
        var cliente = new HttpClient();

        cliente.Timeout =
            TimeSpan.FromSeconds(30);

        return cliente;
    }


    // =====================================================
    // BACKUP
    // =====================================================

    private async void Button_Backup_Clicked(
        object sender,
        EventArgs e)
    {
        try
        {
            lbl_status.Text =
                "Preparando backup...";


            var pacientes =
                await App.Database.Pacientes.Listar();


            var historico =
                await App.Database.HistoricoVacinal
                    .ListarTodos();


            var backup = new
            {
                aplicativo = "Amar V",

                usuario = new
                {
                    nome = usuarioLogado.Nome,
                    perfil = usuarioLogado.Perfil
                },

                dataBackup =
                    DateTime.Now,

                pacientes =
                    pacientes,

                historico =
                    historico
            };


            string json =
                JsonSerializer.Serialize(
                    backup,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });


            lbl_status.Text =
                "Enviando backup para o servidor...";


            using HttpClient cliente =
                CriarCliente();


            using StringContent conteudo =
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");


            HttpResponseMessage resposta =
                await cliente.PostAsync(
                    UrlBackup,
                    conteudo);


            string respostaTexto =
                await resposta.Content
                    .ReadAsStringAsync();


            if (!resposta.IsSuccessStatusCode)
            {
                throw new Exception(
                    "Erro HTTP: " +
                    (int)resposta.StatusCode +
                    "\n\n" +
                    respostaTexto);
            }


            var resultado =
                JsonSerializer.Deserialize<RespostaBackup>(
                    respostaTexto,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });


            if (resultado == null ||
                !resultado.sucesso)
            {
                throw new Exception(
                    resultado?.mensagem ??
                    "Servidor não confirmou o backup.");
            }


            Backup registroBackup =
                new Backup
                {
                    DataBackup =
                        DateTime.Now,

                    Status =
                        "Backup enviado ao servidor",

                    LocalArquivo =
                        resultado.arquivo
                };


            registroBackup.RealizarBackup();


            await App.Database.Backup
                .Salvar(registroBackup);


            lbl_status.Text =
                "Backup realizado com sucesso!\n" +
                "Arquivo: " +
                resultado.arquivo;


            await DisplayAlert(
                "Backup",
                "Backup enviado para o servidor com sucesso.\n\n" +
                "Arquivo: " +
                resultado.arquivo,
                "OK");
        }
        catch (HttpRequestException ex)
        {
            lbl_status.Text =
                "Falha na conexão.";


            await DisplayAlert(
                "Falha na conexão",
                "O aplicativo não conseguiu se conectar ao servidor.\n\n" +
                "Endereço:\n" +
                UrlBackup +
                "\n\nDetalhes:\n" +
                ex.Message,
                "OK");
        }
        catch (TaskCanceledException)
        {
            lbl_status.Text =
                "Tempo de conexão esgotado.";


            await DisplayAlert(
                "Tempo esgotado",
                "O servidor demorou muito para responder.",
                "OK");
        }
        catch (Exception ex)
        {
            lbl_status.Text =
                "Erro ao realizar backup.";


            await DisplayAlert(
                "Erro no Backup",
                ex.Message,
                "OK");
        }
    }


    // =====================================================
    // ABRIR BACKUPS DO SERVIDOR
    // =====================================================

    private async void Button_AbrirBackup_Clicked(
        object sender,
        EventArgs e)
    {
        try
        {
            lbl_status.Text =
                "Procurando backups no servidor...";


            using HttpClient cliente =
                CriarCliente();


            string url =
                UrlRestaurar;


            HttpResponseMessage resposta =
                await cliente.GetAsync(url);


            string respostaTexto =
                await resposta.Content
                    .ReadAsStringAsync();


            if (!resposta.IsSuccessStatusCode)
            {
                throw new Exception(
                    "Erro HTTP: " +
                    (int)resposta.StatusCode +
                    "\n\n" +
                    respostaTexto);
            }


            var resultado =
                JsonSerializer.Deserialize<RespostaListaBackups>(
                    respostaTexto,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });


            if (resultado == null ||
                !resultado.sucesso)
            {
                throw new Exception(
                    resultado?.mensagem ??
                    "Não foi possível listar os backups.");
            }


            if (resultado.backups == null ||
                resultado.backups.Count == 0)
            {
                await DisplayAlert(
                    "Backups",
                    "Nenhum backup foi encontrado no servidor.",
                    "OK");

                return;
            }


            // =============================================
            // MONTAR LISTA PARA ESCOLHA
            // =============================================

            var opcoes =
                resultado.backups
                    .Select(b =>
                        $"{b.arquivo}\n" +
                        $"Data: {b.data}")
                    .ToArray();


            string? escolha =
                await DisplayActionSheet(
                    "Escolha o backup para restaurar",
                    "Cancelar",
                    null,
                    opcoes);


            if (string.IsNullOrWhiteSpace(escolha) ||
                escolha == "Cancelar")
            {
                lbl_status.Text =
                    "Restauração cancelada.";

                return;
            }


            // =============================================
            // DESCOBRIR QUAL ARQUIVO FOI ESCOLHIDO
            // =============================================

            var backupEscolhido =
                resultado.backups
                    .FirstOrDefault(b =>
                        escolha.StartsWith(
                            b.arquivo,
                            StringComparison.OrdinalIgnoreCase));


            if (backupEscolhido == null)
            {
                throw new Exception(
                    "Não foi possível identificar o backup escolhido.");
            }


            bool confirmar =
                await DisplayAlert(
                    "Confirmar restauração",
                    "Você escolheu:\n\n" +
                    backupEscolhido.arquivo +
                    "\n\n" +
                    "Os dados desse backup serão restaurados.\n\n" +
                    "Deseja continuar?",
                    "Sim",
                    "Não");


            if (!confirmar)
            {
                lbl_status.Text =
                    "Restauração cancelada.";

                return;
            }


            await RestaurarBackupEscolhido(
                backupEscolhido.arquivo);
        }
        catch (HttpRequestException ex)
        {
            lbl_status.Text =
                "Falha na conexão.";


            await DisplayAlert(
                "Falha na conexão",
                "Não foi possível acessar o servidor.\n\n" +
                ex.Message,
                "OK");
        }
        catch (TaskCanceledException)
        {
            await DisplayAlert(
                "Tempo esgotado",
                "O servidor demorou muito para responder.",
                "OK");
        }
        catch (Exception ex)
        {
            lbl_status.Text =
                "Erro ao procurar backups.";


            await DisplayAlert(
                "Erro",
                ex.Message,
                "OK");
        }
    }


    // =====================================================
    // RESTAURAR BACKUP ESCOLHIDO
    // =====================================================

    private async Task RestaurarBackupEscolhido(
        string nomeArquivo)
    {
        try
        {
            lbl_status.Text =
                "Baixando backup escolhido...";


            using HttpClient cliente =
                CriarCliente();


            string url =
                UrlRestaurar +
                "?arquivo=" +
                Uri.EscapeDataString(
                    nomeArquivo);


            HttpResponseMessage resposta =
                await cliente.GetAsync(url);


            string respostaTexto =
                await resposta.Content
                    .ReadAsStringAsync();


            if (!resposta.IsSuccessStatusCode)
            {
                throw new Exception(
                    "Erro HTTP: " +
                    (int)resposta.StatusCode +
                    "\n\n" +
                    respostaTexto);
            }


            var resultado =
                JsonSerializer.Deserialize<RespostaRestauracao>(
                    respostaTexto,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });


            if (resultado == null ||
                !resultado.sucesso)
            {
                throw new Exception(
                    resultado?.mensagem ??
                    "Servidor não encontrou o backup.");
            }


            if (resultado.dados == null)
            {
                throw new Exception(
                    "O backup retornado está vazio.");
            }


            lbl_status.Text =
                "Backup encontrado. Restaurando dados...";


            // =============================================
            // RESTAURAR PACIENTES
            // =============================================

            if (resultado.dados.pacientes != null)
            {
                foreach (
                    var paciente
                    in resultado.dados.pacientes)
                {
                    paciente.Id = 0;


                    await App.Database.Pacientes
                        .Salvar(paciente);
                }
            }


            // =============================================
            // RESTAURAR HISTÓRICO
            // =============================================

            if (resultado.dados.historico != null)
            {
                foreach (
                    var historico
                    in resultado.dados.historico)
                {
                    historico.Id = 0;


                    await App.Database.HistoricoVacinal
                        .Salvar(historico);
                }
            }


            lbl_status.Text =
                "Backup restaurado com sucesso!\n" +
                "Arquivo: " +
                resultado.arquivo;


            await DisplayAlert(
                "Restauração concluída",
                "O backup selecionado foi restaurado com sucesso.\n\n" +
                "Arquivo:\n" +
                resultado.arquivo,
                "OK");
        }
        catch (HttpRequestException ex)
        {
            lbl_status.Text =
                "Falha na conexão.";


            await DisplayAlert(
                "Falha na conexão",
                ex.Message,
                "OK");
        }
        catch (TaskCanceledException)
        {
            await DisplayAlert(
                "Tempo esgotado",
                "O servidor demorou muito para responder.",
                "OK");
        }
        catch (Exception ex)
        {
            lbl_status.Text =
                "Erro ao restaurar backup.";


            await DisplayAlert(
                "Erro na Restauração",
                ex.Message,
                "OK");
        }
    }


    // =====================================================
    // SINCRONIZAÇÃO
    // =====================================================

    private async void Button_Sincronizar_Clicked(
        object sender,
        EventArgs e)
    {
        try
        {
            lbl_status.Text =
                "Lendo dados locais...";


            var pacientes =
                await App.Database.Pacientes
                    .Listar();


            var historico =
                await App.Database.HistoricoVacinal
                    .ListarTodos();


            int totalRegistros =
                pacientes.Count +
                historico.Count;


            var dados = new
            {
                usuario = new
                {
                    nome =
                        usuarioLogado.Nome,

                    perfil =
                        usuarioLogado.Perfil
                },

                dataSincronizacao =
                    DateTime.Now,

                pacientes =
                    pacientes,

                historico =
                    historico
            };


            string json =
                JsonSerializer.Serialize(
                    dados,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });


            lbl_status.Text =
                "Enviando dados para o servidor...";


            using HttpClient cliente =
                CriarCliente();


            using StringContent conteudo =
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");


            HttpResponseMessage resposta =
                await cliente.PostAsync(
                    UrlSincronizacao,
                    conteudo);


            string respostaTexto =
                await resposta.Content
                    .ReadAsStringAsync();


            if (!resposta.IsSuccessStatusCode)
            {
                throw new Exception(
                    "Erro HTTP: " +
                    (int)resposta.StatusCode +
                    "\n\n" +
                    respostaTexto);
            }


            var resultado =
                JsonSerializer.Deserialize<RespostaSincronizacao>(
                    respostaTexto,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });


            if (resultado == null ||
                !resultado.sucesso)
            {
                throw new Exception(
                    resultado?.mensagem ??
                    "Servidor não confirmou a sincronização.");
            }


            Sincronizacao sincronizacao =
                new Sincronizacao
                {
                    DataSincronizacao =
                        DateTime.Now,

                    Status =
                        "Sincronizado com o servidor",

                    QuantidadeRegistros =
                        totalRegistros
                };


            sincronizacao.SincronizarDados();


            await App.Database.Sincronizacao
                .Salvar(sincronizacao);


            lbl_status.Text =
                "Sincronização concluída!\n" +
                "Registros enviados: " +
                totalRegistros;


            await DisplayAlert(
                "Sincronização",
                "Dados enviados para o servidor com sucesso.",
                "OK");
        }
        catch (HttpRequestException ex)
        {
            lbl_status.Text =
                "Falha na conexão.";


            await DisplayAlert(
                "Falha na conexão",
                "Não foi possível conectar ao servidor.\n\n" +
                "Detalhes:\n" +
                ex.Message,
                "OK");
        }
        catch (TaskCanceledException)
        {
            await DisplayAlert(
                "Tempo esgotado",
                "O servidor demorou muito para responder.",
                "OK");
        }
        catch (Exception ex)
        {
            lbl_status.Text =
                "Erro na sincronização.";


            await DisplayAlert(
                "Erro",
                ex.Message,
                "OK");
        }
    }


    // =====================================================
    // VOLTAR
    // =====================================================

    private async void Button_Menu_Clicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}


// =========================================================
// RESPOSTA DO BACKUP
// =========================================================

public class RespostaBackup
{
    public bool sucesso { get; set; }

    public string mensagem { get; set; }

    public string arquivo { get; set; }
}


// =========================================================
// RESPOSTA DA LISTA DE BACKUPS
// =========================================================

public class RespostaListaBackups
{
    public bool sucesso { get; set; }

    public string mensagem { get; set; }

    public List<BackupServidor> backups { get; set; }
}


// =========================================================
// BACKUP ENCONTRADO NO SERVIDOR
// =========================================================

public class BackupServidor
{
    public string arquivo { get; set; }

    public string data { get; set; }

    public long tamanho { get; set; }
}


// =========================================================
// RESPOSTA DA RESTAURAÇÃO
// =========================================================

public class RespostaRestauracao
{
    public bool sucesso { get; set; }

    public string mensagem { get; set; }

    public string arquivo { get; set; }

    public DadosBackup dados { get; set; }
}


// =========================================================
// DADOS DO BACKUP
// =========================================================

public class DadosBackup
{
    public List<Paciente> pacientes { get; set; }

    public List<HistoricoVacinal> historico { get; set; }
}


// =========================================================
// RESPOSTA DA SINCRONIZAÇÃO
// =========================================================

public class RespostaSincronizacao
{
    public bool sucesso { get; set; }

    public string mensagem { get; set; }

    public string arquivo { get; set; }
}
