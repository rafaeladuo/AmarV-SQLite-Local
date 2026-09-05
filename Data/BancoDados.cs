using SQLite;
using maui_amarv.Models;
using maui_amarv.Helpers;

namespace maui_amarv.Data
{
    public class BancoDados
    {
        private readonly SQLiteAsyncConnection _database;
        public PacienteHelper Pacientes { get; }
        public HistoricoVacinalHelper HistoricoVacinal { get; }
        public SincronizacaoHelper Sincronizacao { get; }
        public BackupHelper Backup { get; }
        public BancoDados(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);

            _database.CreateTableAsync<Pessoa>().Wait();
            _database.CreateTableAsync<UsuarioDoSistema>().Wait();
            Pacientes = new PacienteHelper(_database);
            HistoricoVacinal = new HistoricoVacinalHelper(_database);
            Sincronizacao = new SincronizacaoHelper(_database);
            Backup = new BackupHelper(_database);
            _database.CreateTableAsync<AdministradorDoSistema>().Wait();
            _database.CreateTableAsync<Paciente>().Wait();
            _database.CreateTableAsync<Sessao>().Wait();
            _database.CreateTableAsync<CondicaoClinica>().Wait();
            _database.CreateTableAsync<Vacina>().Wait();
            _database.CreateTableAsync<Dose>().Wait();
            _database.CreateTableAsync<CalendarioVacinal>().Wait();
            _database.CreateTableAsync<CalendarioVacinalSUS>().Wait();
            _database.CreateTableAsync<CalendarioVacinalRedePrivada>().Wait();
            _database.CreateTableAsync<AnaliseEsquemaVacinal>().Wait();
            _database.CreateTableAsync<Relatorio>().Wait();
            _database.CreateTableAsync<RelatorioVacinalPaciente>().Wait();
            _database.CreateTableAsync<RelatorioPacienteComVacinasAtrasadas>().Wait();
            _database.CreateTableAsync<RelatorioPacientesEmSituacaoRegular>().Wait();
            _database.CreateTableAsync<RegraVacinal>().Wait();
            _database.CreateTableAsync<UsuarioSistema>().Wait();
            _database.CreateTableAsync<Paciente>().Wait();
            _database.CreateTableAsync<HistoricoVacinal>().Wait();
            _database.CreateTableAsync<Sincronizacao>().Wait();
            _database.CreateTableAsync<Backup>().Wait();
        }


        // =========================
        // PACIENTE
        // =========================


        // =========================
        // HISTÓRICO VACINAL
        // =========================

    

        // =========================
        // CONDIÇÃO CLÍNICA
        // =========================

        public Task<int> SalvarCondicaoClinica(CondicaoClinica condicao)
        {
            return _database.InsertAsync(condicao);
        }

        public Task<int> AtualizarCondicaoClinica(CondicaoClinica condicao)
        {
            return _database.UpdateAsync(condicao);
        }

        public Task<List<CondicaoClinica>> ListarCondicoesPorPaciente(int pacienteId)
        {
            return _database.Table<CondicaoClinica>()
                            .Where(c => c.PacienteId == pacienteId)
                            .ToListAsync();
        }

        // =========================
        // REGRA VACINAL / VACINAS E DOSES
        // =========================

        public Task<int> SalvarRegraVacinal(RegraVacinal regra)
        {
            return _database.InsertAsync(regra);
        }

        public Task<int> AtualizarRegraVacinal(RegraVacinal regra)
        {
            return _database.UpdateAsync(regra);
        }

        public Task<int> ExcluirRegraVacinal(RegraVacinal regra)
        {
            return _database.DeleteAsync(regra);
        }

        public Task<List<RegraVacinal>> ListarRegrasVacinais()
        {
            return _database.Table<RegraVacinal>().ToListAsync();
        }

        public Task<List<RegraVacinal>> ListarRegrasPorTipo(string tipo)
        {
            return _database.Table<RegraVacinal>()
                            .Where(r => r.TipoCalendario == tipo)
                            .ToListAsync();
        }

        public Task<List<RegraVacinal>> ListarRegrasPorIdade(int idadeTotalMeses)
        {
            return _database.Table<RegraVacinal>()
                            .Where(r => idadeTotalMeses >= r.IdadeMinimaMeses &&
                                        idadeTotalMeses <= r.IdadeMaximaMeses)
                            .ToListAsync();
        }

        // =========================
        // VACINA
        // =========================

        public Task<int> SalvarVacina(Vacina vacina)
        {
            return _database.InsertAsync(vacina);
        }

        public Task<int> AtualizarVacina(Vacina vacina)
        {
            return _database.UpdateAsync(vacina);
        }

        public Task<int> ExcluirVacina(Vacina vacina)
        {
            return _database.DeleteAsync(vacina);
        }

        public Task<List<Vacina>> ListarVacinas()
        {
            return _database.Table<Vacina>().ToListAsync();
        }

        // =========================
        // DOSE
        // =========================

        public Task<int> SalvarDose(Dose dose)
        {
            return _database.InsertAsync(dose);
        }

        public Task<int> AtualizarDose(Dose dose)
        {
            return _database.UpdateAsync(dose);
        }

        public Task<List<Dose>> ListarDosesPorVacina(int vacinaId)
        {
            return _database.Table<Dose>()
                            .Where(d => d.VacinaId == vacinaId)
                            .ToListAsync();
        }

        // =========================
        // USUÁRIO DO SISTEMA DO DIAGRAMA
        // =========================

        public Task<int> SalvarUsuarioDoSistema(UsuarioDoSistema usuario)
        {
            return _database.InsertAsync(usuario);
        }

        public Task<int> AtualizarUsuarioDoSistema(UsuarioDoSistema usuario)
        {
            return _database.UpdateAsync(usuario);
        }

        public Task<List<UsuarioDoSistema>> ListarUsuariosDoSistema()
        {
            return _database.Table<UsuarioDoSistema>().ToListAsync();
        }

        public Task<UsuarioDoSistema> BuscarUsuarioDoSistema(string login, string senha)
        {
            return _database.Table<UsuarioDoSistema>()
                            .Where(u => u.Login == login &&
                                        u.Senha == senha &&
                                        u.Ativo == true)
                            .FirstOrDefaultAsync();
        }

        // =========================
        // USUÁRIO ATUAL DO APP
        // =========================

        public Task<UsuarioSistema> BuscarUsuario(string login, string senha)
        {
            return _database.Table<UsuarioSistema>()
                            .Where(u => u.Login == login &&
                                        u.Senha == senha &&
                                        u.Ativo == true)
                            .FirstOrDefaultAsync();
        }

        public Task<List<UsuarioSistema>> ListarUsuarios()
        {
            return _database.Table<UsuarioSistema>().ToListAsync();
        }

        public Task<int> SalvarUsuario(UsuarioSistema usuario)
        {
            return _database.InsertAsync(usuario);
        }

        public Task<int> AtualizarUsuario(UsuarioSistema usuario)
        {
            return _database.UpdateAsync(usuario);
        }

        public Task<int> ExcluirUsuario(UsuarioSistema usuario)
        {
            return _database.DeleteAsync(usuario);
        }

        // =========================
        // SESSÃO
        // =========================

        public Task<int> SalvarSessao(Sessao sessao)
        {
            return _database.InsertAsync(sessao);
        }

        public Task<int> AtualizarSessao(Sessao sessao)
        {
            return _database.UpdateAsync(sessao);
        }

        public Task<List<Sessao>> ListarSessoes()
        {
            return _database.Table<Sessao>().ToListAsync();
        }

        // =========================
        // ANÁLISE VACINAL
        // =========================

        public Task<int> SalvarAnaliseVacinal(AnaliseEsquemaVacinal analise)
        {
            return _database.InsertAsync(analise);
        }

        public Task<List<AnaliseEsquemaVacinal>> ListarAnalisesPorPaciente(int pacienteId)
        {
            return _database.Table<AnaliseEsquemaVacinal>()
                            .Where(a => a.PacienteId == pacienteId)
                            .ToListAsync();
        }

        // =========================
        // RELATÓRIOS
        // =========================

        public Task<int> SalvarRelatorio(Relatorio relatorio)
        {
            return _database.InsertAsync(relatorio);
        }

        public Task<List<Relatorio>> ListarRelatorios()
        {
            return _database.Table<Relatorio>().ToListAsync();
        }

        public Task<int> SalvarRelatorioVacinalPaciente(RelatorioVacinalPaciente relatorio)
        {
            return _database.InsertAsync(relatorio);
        }

        public Task<int> SalvarRelatorioPacientesAtrasados(RelatorioPacienteComVacinasAtrasadas relatorio)
        {
            return _database.InsertAsync(relatorio);
        }

        public Task<int> SalvarRelatorioPacientesRegulares(RelatorioPacientesEmSituacaoRegular relatorio)
        {
            return _database.InsertAsync(relatorio);
        }

        // =========================
        // SINCRONIZAÇÃO
        // =========================

      
        // =========================
        // BACKUP
        // =========================


        // =========================
        // USUÁRIOS INICIAIS
        // =========================

        public async Task CarregarUsuariosIniciais()
        {
            var usuarios = await _database.Table<UsuarioSistema>().ToListAsync();

            if (usuarios.Count > 0)
                return;

            var lista = new List<UsuarioSistema>
            {
                new UsuarioSistema
                {
                    Nome = "Administrador",
                    Login = "admin",
                    Senha = "123",
                    Perfil = "Administrador",
                    Ativo = true
                },

                new UsuarioSistema
                {
                    Nome = "Usuário do Sistema",
                    Login = "usuario",
                    Senha = "123",
                    Perfil = "Usuario",
                    Ativo = true
                }
            };

            await _database.InsertAllAsync(lista);
        }

        // =========================
        // REGRAS VACINAIS INICIAIS
        // =========================

        public async Task CarregarRegrasIniciais()
        {
            var regrasExistentes = await _database.Table<RegraVacinal>().ToListAsync();

            if (regrasExistentes.Count > 0)
                return;

            var regras = new List<RegraVacinal>
            {
                new RegraVacinal
                {
                    NomeVacina = "BCG",
                    Fabricante = "Conforme disponibilidade da rede pública",
                    TipoVacina = "Atenuada",
                    IdadeMinimaMeses = 0,
                    IdadeMaximaMeses = 59,
                    DosesNecessarias = 1,
                    IntervaloEntreDoses = "Dose única",
                    ViaAdministracao = "Intradérmica",
                    LocalAplicacao = "Braço direito",
                    TipoCalendario = "SUS",
                    Indicacao = "Prevenção das formas graves de tuberculose.",
                    Contraindicacao = "Imunodeficiência grave e reação grave anterior.",
                    EventosAdversos = "Lesão local, cicatriz vacinal e raramente reação regional.",
                    Observacao = "Dose única ao nascer. Indicada até 4 anos e 11 meses."
                },

                new RegraVacinal
                {
                    NomeVacina = "Hepatite B",
                    Fabricante = "Conforme disponibilidade da rede pública",
                    TipoVacina = "Recombinante",
                    IdadeMinimaMeses = 0,
                    IdadeMaximaMeses = 9999,
                    DosesNecessarias = 3,
                    IntervaloEntreDoses = "0, 1 e 6 meses",
                    ViaAdministracao = "Intramuscular",
                    LocalAplicacao = "Vasto lateral da coxa ou deltoide",
                    TipoCalendario = "SUS",
                    Indicacao = "Prevenção contra hepatite B.",
                    Contraindicacao = "Anafilaxia a dose anterior ou componente da vacina.",
                    EventosAdversos = "Dor local, febre baixa e mal-estar.",
                    Observacao = "Esquema de 3 doses."
                },

                new RegraVacinal
                {
                    NomeVacina = "VIP",
                    Fabricante = "Conforme disponibilidade da rede pública",
                    TipoVacina = "Inativada",
                    IdadeMinimaMeses = 2,
                    IdadeMaximaMeses = 228,
                    DosesNecessarias = 3,
                    IntervaloEntreDoses = "Seguir intervalo mínimo do calendário vacinal",
                    ViaAdministracao = "Intramuscular",
                    LocalAplicacao = "Vasto lateral da coxa ou deltoide",
                    TipoCalendario = "SUS",
                    Indicacao = "Prevenção contra poliomielite.",
                    Contraindicacao = "Anafilaxia a dose anterior ou componente da vacina.",
                    EventosAdversos = "Dor local, febre baixa e irritabilidade.",
                    Observacao = "Vacina inativada contra poliomielite. Para não vacinados, considerar esquema de recuperação conforme avaliação profissional."
                },

                new RegraVacinal
                {
                    NomeVacina = "Pentavalente",
                    Fabricante = "Conforme disponibilidade da rede pública",
                    TipoVacina = "Combinada",
                    IdadeMinimaMeses = 2,
                    IdadeMaximaMeses = 59,
                    DosesNecessarias = 3,
                    IntervaloEntreDoses = "Seguir calendário infantil",
                    ViaAdministracao = "Intramuscular",
                    LocalAplicacao = "Vasto lateral da coxa",
                    TipoCalendario = "SUS",
                    Indicacao = "Proteção contra difteria, tétano, coqueluche, hepatite B e Hib.",
                    Contraindicacao = "Anafilaxia a dose anterior ou componente da vacina.",
                    EventosAdversos = "Febre, dor local, irritabilidade e sonolência.",
                    Observacao = "Indicada na infância."
                },

                new RegraVacinal
                {
                    NomeVacina = "Rotavírus",
                    Fabricante = "Conforme disponibilidade da rede pública",
                    TipoVacina = "Atenuada oral",
                    IdadeMinimaMeses = 2,
                    IdadeMaximaMeses = 23,
                    DosesNecessarias = 2,
                    IntervaloEntreDoses = "Seguir idade máxima e intervalo permitido",
                    ViaAdministracao = "Oral",
                    LocalAplicacao = "Via oral",
                    TipoCalendario = "SUS",
                    Indicacao = "Prevenção contra gastroenterite por rotavírus.",
                    Contraindicacao = "Imunodeficiência grave, histórico de invaginação intestinal ou alergia grave.",
                    EventosAdversos = "Irritabilidade, vômitos e diarreia leve.",
                    Observacao = "Respeitar idade máxima."
                },

                new RegraVacinal
                {
                    NomeVacina = "Pneumo 10",
                    Fabricante = "Conforme disponibilidade da rede pública",
                    TipoVacina = "Conjugada",
                    IdadeMinimaMeses = 2,
                    IdadeMaximaMeses = 59,
                    DosesNecessarias = 3,
                    IntervaloEntreDoses = "Seguir calendário infantil",
                    ViaAdministracao = "Intramuscular",
                    LocalAplicacao = "Vasto lateral da coxa ou deltoide",
                    TipoCalendario = "SUS",
                    Indicacao = "Prevenção contra doenças pneumocócicas.",
                    Contraindicacao = "Anafilaxia a dose anterior ou componente da vacina.",
                    EventosAdversos = "Dor local, febre e irritabilidade.",
                    Observacao = "Esquema infantil."
                },

                new RegraVacinal
                {
                    NomeVacina = "Meningo C",
                    Fabricante = "Conforme disponibilidade da rede pública",
                    TipoVacina = "Conjugada",
                    IdadeMinimaMeses = 3,
                    IdadeMaximaMeses = 59,
                    DosesNecessarias = 2,
                    IntervaloEntreDoses = "Seguir calendário infantil",
                    ViaAdministracao = "Intramuscular",
                    LocalAplicacao = "Vasto lateral da coxa ou deltoide",
                    TipoCalendario = "SUS",
                    Indicacao = "Prevenção contra meningite meningocócica C.",
                    Contraindicacao = "Anafilaxia a dose anterior ou componente da vacina.",
                    EventosAdversos = "Dor local, febre e irritabilidade.",
                    Observacao = "Esquema infantil."
                },

                new RegraVacinal
                {
                    NomeVacina = "Covid baby",
                    Fabricante = "Conforme disponibilidade",
                    TipoVacina = "Conforme fabricante disponível",
                    IdadeMinimaMeses = 6,
                    IdadeMaximaMeses = 7,
                    DosesNecessarias = 2,
                    IntervaloEntreDoses = "Conforme orientação vigente",
                    ViaAdministracao = "Intramuscular",
                    LocalAplicacao = "Vasto lateral da coxa",
                    TipoCalendario = "SUS",
                    Indicacao = "Prevenção contra Covid-19.",
                    Contraindicacao = "Anafilaxia a dose anterior ou componente da vacina.",
                    EventosAdversos = "Dor local, febre e mal-estar.",
                    Observacao = "Conforme calendário atualizado."
                },

                new RegraVacinal
                {
                    NomeVacina = "Febre Amarela",
                    Fabricante = "Conforme disponibilidade da rede pública",
                    TipoVacina = "Atenuada",
                    IdadeMinimaMeses = 9,
                    IdadeMaximaMeses = 9999,
                    DosesNecessarias = 1,
                    IntervaloEntreDoses = "Dose única, conforme indicação",
                    ViaAdministracao = "Subcutânea",
                    LocalAplicacao = "Região deltoide",
                    TipoCalendario = "SUS",
                    Indicacao = "Prevenção contra febre amarela.",
                    Contraindicacao = "Gestantes, imunossuprimidos e lactantes com bebê menor de 6 meses devem passar por avaliação profissional.",
                    EventosAdversos = "Dor local, febre, dor de cabeça e mal-estar.",
                    Observacao = "A partir de 9 meses, conforme indicação."
                },

                new RegraVacinal
                {
                    NomeVacina = "Meningo ACWY",
                    Fabricante = "Conforme disponibilidade da rede pública",
                    TipoVacina = "Conjugada",
                    IdadeMinimaMeses = 12,
                    IdadeMaximaMeses = 168,
                    DosesNecessarias = 1,
                    IntervaloEntreDoses = "Dose única conforme calendário",
                    ViaAdministracao = "Intramuscular",
                    LocalAplicacao = "Deltoide",
                    TipoCalendario = "SUS",
                    Indicacao = "Prevenção contra meningite meningocócica dos sorogrupos A, C, W e Y.",
                    Contraindicacao = "Anafilaxia a dose anterior ou componente da vacina.",
                    EventosAdversos = "Dor local, febre baixa e mal-estar.",
                    Observacao = "Até 14 anos."
                },

                new RegraVacinal
                {
                    NomeVacina = "Tríplice Viral / SCR",
                    Fabricante = "Conforme disponibilidade da rede pública",
                    TipoVacina = "Atenuada",
                    IdadeMinimaMeses = 12,
                    IdadeMaximaMeses = 348,
                    DosesNecessarias = 2,
                    IntervaloEntreDoses = "Respeitar intervalo mínimo entre as doses",
                    ViaAdministracao = "Subcutânea",
                    LocalAplicacao = "Região deltoide",
                    TipoCalendario = "SUS",
                    Indicacao = "Prevenção contra sarampo, caxumba e rubéola.",
                    Contraindicacao = "Gestantes, imunossuprimidos e alergia grave a componente da vacina.",
                    EventosAdversos = "Febre, exantema leve, dor local e mal-estar.",
                    Observacao = "Até 29 anos: 2 doses."
                },

                new RegraVacinal
                {
                    NomeVacina = "Tríplice Viral / SCR",
                    Fabricante = "Conforme disponibilidade da rede pública",
                    TipoVacina = "Atenuada",
                    IdadeMinimaMeses = 360,
                    IdadeMaximaMeses = 708,
                    DosesNecessarias = 1,
                    IntervaloEntreDoses = "Dose única para esta faixa, conforme histórico",
                    ViaAdministracao = "Subcutânea",
                    LocalAplicacao = "Região deltoide",
                    TipoCalendario = "SUS",
                    Indicacao = "Prevenção contra sarampo, caxumba e rubéola.",
                    Contraindicacao = "Gestantes, imunossuprimidos e alergia grave a componente da vacina.",
                    EventosAdversos = "Febre, exantema leve, dor local e mal-estar.",
                    Observacao = "De 30 a 59 anos: 1 dose."
                },

                new RegraVacinal
                {
                    NomeVacina = "DTP",
                    Fabricante = "Conforme disponibilidade da rede pública",
                    TipoVacina = "Combinada",
                    IdadeMinimaMeses = 15,
                    IdadeMaximaMeses = 83,
                    DosesNecessarias = 2,
                    IntervaloEntreDoses = "Reforços conforme calendário infantil",
                    ViaAdministracao = "Intramuscular",
                    LocalAplicacao = "Vasto lateral da coxa ou deltoide",
                    TipoCalendario = "SUS",
                    Indicacao = "Proteção contra difteria, tétano e coqueluche.",
                    Contraindicacao = "Reação grave a dose anterior.",
                    EventosAdversos = "Febre, dor local, irritabilidade e sonolência.",
                    Observacao = "Reforços na infância."
                },

                new RegraVacinal
                {
                    NomeVacina = "Tetraviral",
                    Fabricante = "Conforme disponibilidade da rede pública",
                    TipoVacina = "Atenuada",
                    IdadeMinimaMeses = 15,
                    IdadeMaximaMeses = 83,
                    DosesNecessarias = 1,
                    IntervaloEntreDoses = "Dose única conforme calendário",
                    ViaAdministracao = "Subcutânea",
                    LocalAplicacao = "Região deltoide",
                    TipoCalendario = "SUS",
                    Indicacao = "Proteção contra sarampo, caxumba, rubéola e varicela.",
                    Contraindicacao = "Gestantes, imunossuprimidos e alergia grave.",
                    EventosAdversos = "Febre, exantema leve, dor local e mal-estar.",
                    Observacao = "Indicada na infância."
                },

                new RegraVacinal
                {
                    NomeVacina = "Hepatite A",
                    Fabricante = "Conforme disponibilidade da rede pública",
                    TipoVacina = "Inativada",
                    IdadeMinimaMeses = 15,
                    IdadeMaximaMeses = 59,
                    DosesNecessarias = 1,
                    IntervaloEntreDoses = "Dose única no calendário SUS",
                    ViaAdministracao = "Intramuscular",
                    LocalAplicacao = "Vasto lateral da coxa ou deltoide",
                    TipoCalendario = "SUS",
                    Indicacao = "Prevenção contra hepatite A.",
                    Contraindicacao = "Anafilaxia a dose anterior ou componente da vacina.",
                    EventosAdversos = "Dor local, febre baixa e mal-estar.",
                    Observacao = "Indicada até 4 anos e 11 meses."
                },

                new RegraVacinal
                {
                    NomeVacina = "Varicela",
                    Fabricante = "Conforme disponibilidade da rede pública",
                    TipoVacina = "Atenuada",
                    IdadeMinimaMeses = 48,
                    IdadeMaximaMeses = 83,
                    DosesNecessarias = 2,
                    IntervaloEntreDoses = "Respeitar intervalo indicado entre doses",
                    ViaAdministracao = "Subcutânea",
                    LocalAplicacao = "Região deltoide",
                    TipoCalendario = "SUS",
                    Indicacao = "Prevenção contra varicela.",
                    Contraindicacao = "Gestantes, imunossuprimidos e alergia grave.",
                    EventosAdversos = "Dor local, febre e exantema leve.",
                    Observacao = "De 4 anos até 6 anos e 11 meses."
                },

                new RegraVacinal
                {
                    NomeVacina = "HPV",
                    Fabricante = "Conforme disponibilidade da rede pública",
                    TipoVacina = "Recombinante",
                    IdadeMinimaMeses = 108,
                    IdadeMaximaMeses = 168,
                    DosesNecessarias = 1,
                    IntervaloEntreDoses = "Dose única no SUS, conforme regra vigente",
                    ViaAdministracao = "Intramuscular",
                    LocalAplicacao = "Deltoide",
                    TipoCalendario = "SUS",
                    Indicacao = "Prevenção contra infecções pelo HPV e doenças relacionadas.",
                    Contraindicacao = "Anafilaxia a dose anterior ou componente da vacina.",
                    EventosAdversos = "Dor local, febre baixa, cefaleia e mal-estar.",
                    Observacao = "Indicado de 9 a 14 anos. Dose única no SUS."
                },

                new RegraVacinal
                {
                    NomeVacina = "Duplo Adulto",
                    Fabricante = "Conforme disponibilidade da rede pública",
                    TipoVacina = "Toxoide",
                    IdadeMinimaMeses = 84,
                    IdadeMaximaMeses = 9999,
                    DosesNecessarias = 3,
                    IntervaloEntreDoses = "2ª dose após 60 dias e 3ª dose cerca de 6 meses após a primeira; reforço a cada 10 anos",
                    ViaAdministracao = "Intramuscular",
                    LocalAplicacao = "Deltoide",
                    TipoCalendario = "SUS",
                    Indicacao = "Proteção contra difteria e tétano.",
                    Contraindicacao = "Reação grave a dose anterior.",
                    EventosAdversos = "Dor local, febre baixa e mal-estar.",
                    Observacao = "A partir de 7 anos. Após esquema completo, reforço a cada 10 anos."
                },

                new RegraVacinal
                {
                    NomeVacina = "VOP",
                    Fabricante = "Conforme disponibilidade da rede pública",
                    TipoVacina = "Atenuada oral",
                    IdadeMinimaMeses = 15,
                    IdadeMaximaMeses = 59,
                    DosesNecessarias = 2,
                    IntervaloEntreDoses = "Reforços conforme calendário infantil",
                    ViaAdministracao = "Oral",
                    LocalAplicacao = "Via oral",
                    TipoCalendario = "SUS",
                    Indicacao = "Prevenção contra poliomielite.",
                    Contraindicacao = "Imunodeficiência grave e contato domiciliar com imunodeprimido.",
                    EventosAdversos = "Raramente eventos neurológicos associados.",
                    Observacao = "Somente infância. Não aparece para adulto."
                },

                new RegraVacinal
                {
                    NomeVacina = "Pneumo 23",
                    Fabricante = "Conforme disponibilidade da rede pública",
                    TipoVacina = "Polissacarídica",
                    IdadeMinimaMeses = 720,
                    IdadeMaximaMeses = 9999,
                    DosesNecessarias = 1,
                    IntervaloEntreDoses = "Conforme indicação clínica",
                    ViaAdministracao = "Intramuscular",
                    LocalAplicacao = "Deltoide",
                    TipoCalendario = "SUS",
                    Indicacao = "Prevenção contra doenças pneumocócicas em idosos ou grupos especiais.",
                    Contraindicacao = "Anafilaxia a dose anterior ou componente da vacina.",
                    EventosAdversos = "Dor local, febre baixa e mal-estar.",
                    Observacao = "Acima de 60 anos ou grupos indicados."
                },

                new RegraVacinal
                {
                    NomeVacina = "Meningocócica B",
                    Fabricante = "Conforme disponibilidade da rede privada",
                    TipoVacina = "Recombinante",
                    IdadeMinimaMeses = 2,
                    IdadeMaximaMeses = 9999,
                    DosesNecessarias = 2,
                    IntervaloEntreDoses = "Conforme idade e orientação do serviço privado",
                    ViaAdministracao = "Intramuscular",
                    LocalAplicacao = "Vasto lateral da coxa ou deltoide",
                    TipoCalendario = "Privado",
                    Indicacao = "Prevenção contra doença meningocócica B.",
                    Contraindicacao = "Anafilaxia a dose anterior ou componente da vacina.",
                    EventosAdversos = "Dor local, febre e mal-estar.",
                    Observacao = "Vacina disponível na rede privada."
                },

                new RegraVacinal
                {
                    NomeVacina = "Hexavalente",
                    Fabricante = "Conforme disponibilidade da rede privada",
                    TipoVacina = "Combinada",
                    IdadeMinimaMeses = 2,
                    IdadeMaximaMeses = 59,
                    DosesNecessarias = 3,
                    IntervaloEntreDoses = "Conforme calendário infantil privado",
                    ViaAdministracao = "Intramuscular",
                    LocalAplicacao = "Vasto lateral da coxa",
                    TipoCalendario = "Privado",
                    Indicacao = "Proteção combinada contra múltiplas doenças da infância.",
                    Contraindicacao = "Anafilaxia a dose anterior ou componente da vacina.",
                    EventosAdversos = "Febre, dor local e irritabilidade.",
                    Observacao = "Vacina combinada utilizada na rede privada."
                },

                new RegraVacinal
                {
                    NomeVacina = "Pneumocócica 13",
                    Fabricante = "Conforme disponibilidade da rede privada",
                    TipoVacina = "Conjugada",
                    IdadeMinimaMeses = 2,
                    IdadeMaximaMeses = 9999,
                    DosesNecessarias = 1,
                    IntervaloEntreDoses = "Conforme idade e orientação profissional",
                    ViaAdministracao = "Intramuscular",
                    LocalAplicacao = "Vasto lateral da coxa ou deltoide",
                    TipoCalendario = "Privado",
                    Indicacao = "Prevenção contra doenças pneumocócicas.",
                    Contraindicacao = "Anafilaxia a dose anterior ou componente da vacina.",
                    EventosAdversos = "Dor local, febre e mal-estar.",
                    Observacao = "Disponível na rede privada."
                },

                new RegraVacinal
                {
                    NomeVacina = "Rotavírus Pentavalente",
                    Fabricante = "Conforme disponibilidade da rede privada",
                    TipoVacina = "Atenuada oral",
                    IdadeMinimaMeses = 2,
                    IdadeMaximaMeses = 23,
                    DosesNecessarias = 3,
                    IntervaloEntreDoses = "Respeitar idade máxima e intervalo indicado",
                    ViaAdministracao = "Oral",
                    LocalAplicacao = "Via oral",
                    TipoCalendario = "Privado",
                    Indicacao = "Prevenção contra gastroenterite por rotavírus.",
                    Contraindicacao = "Imunodeficiência grave, histórico de invaginação intestinal ou alergia grave.",
                    EventosAdversos = "Irritabilidade, vômitos e diarreia leve.",
                    Observacao = "Versão privada do esquema de rotavírus."
                }
            };

            await _database.InsertAllAsync(regras);
        }
    }
}