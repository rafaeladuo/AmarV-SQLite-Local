namespace maui_amarv.Models
{
    public class UsuarioDoSistema : Pessoa
    {
        public string Login { get; set; }

        public string Senha { get; set; }

        public string Perfil { get; set; }

        public string Status { get; set; }

        public bool Ativo { get; set; }

        public virtual bool RealizarLogin(string login, string senha)
        {
            return Login == login && Senha == senha && Ativo;
        }

        public virtual void AlterarSenha(string novaSenha)
        {
            Senha = novaSenha;
        }

        public virtual string ConsultarPerfil()
        {
            return Perfil;
        }
    }
}