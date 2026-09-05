using SQLite;
using maui_amarv.Models;

namespace maui_amarv.Helpers
{
    public class BackupHelper
    {
        private readonly SQLiteAsyncConnection _database;

        public BackupHelper(SQLiteAsyncConnection database)
        {
            _database = database;
        }

        // SALVAR
        public Task<int> Salvar(Backup backup)
        {
            return _database.InsertAsync(backup);
        }

        // LISTAR
        public Task<List<Backup>> Listar()
        {
            return _database
                .Table<Backup>()
                .OrderByDescending(b => b.DataBackup)
                .ToListAsync();
        }

        // BUSCAR O ÚLTIMO BACKUP
        public Task<Backup> Ultimo()
        {
            return _database
                .Table<Backup>()
                .OrderByDescending(b => b.DataBackup)
                .FirstOrDefaultAsync();
        }

        // ATUALIZAR
        public Task<int> Atualizar(Backup backup)
        {
            return _database.UpdateAsync(backup);
        }

        // EXCLUIR
        public Task<int> Excluir(Backup backup)
        {
            return _database.DeleteAsync(backup);
        }

        // EXCLUIR TODOS
        public Task<int> ExcluirTodos()
        {
            return _database
                .Table<Backup>()
                .DeleteAsync();
        }
    }
}