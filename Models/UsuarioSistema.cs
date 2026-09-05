using SQLite;

namespace maui_amarv.Models
{
    public class UsuarioSistema
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Login { get; set; }

        public string Senha { get; set; }

        public string Perfil { get; set; }

        public bool Ativo { get; set; }
    }
}