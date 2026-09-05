using SQLite;
using maui_amarv.Models;

namespace maui_amarv.Helpers
{
    public class HistoricoVacinalHelper
    {
        private readonly SQLiteAsyncConnection _database;

        public HistoricoVacinalHelper(SQLiteAsyncConnection database)
        {
            _database = database;
        }

        public Task<int> Salvar(HistoricoVacinal historico)
        {
            return _database.InsertAsync(historico);
        }

        public Task<int> Atualizar(HistoricoVacinal historico)
        {
            return _database.UpdateAsync(historico);
        }

        public Task<List<HistoricoVacinal>> ListarPorPaciente(int pacienteId)
        {
            return _database.Table<HistoricoVacinal>()
                            .Where(h => h.PacienteId == pacienteId)
                            .ToListAsync();
        }

        public Task<List<HistoricoVacinal>> ListarTodos()
        {
            return _database.Table<HistoricoVacinal>()
                            .ToListAsync();
        }

        public Task<int> ExcluirPorPaciente(int pacienteId)
        {
            return _database.Table<HistoricoVacinal>()
                            .Where(h => h.PacienteId == pacienteId)
                            .DeleteAsync();
        }
    }
}