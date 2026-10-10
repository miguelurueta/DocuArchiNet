using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Login2Fa = GestionDocumental_Docuarchi.net;

internal static class LoginSecondFactorPreAuthenticationBehaviorTests
{
    private static int Main()
    {
        try
        {
            ContextIsRestrictedToPreAuthentication();
            CentralConfigurationIsAuthoritativeAndExact();
            PrincipalAdaptersUseTheFourVerifiedContracts();
            DisabledConfigurationFinalizesOnce();
            ActiveConfigurationHasZeroAuthenticatedEffects();
            InvalidAndMissingDataFailClosed();
            Console.WriteLine("login-second-factor preauthentication behavior tests: passed");
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error);
            return 1;
        }
    }

    private static void ContextIsRestrictedToPreAuthentication()
    {
        var ordinary = new Login2Fa.ContextoModulo { CodigoModulo = "GESTOR DOCUMENTAL", IdUsuario = 0, LoginUsuario = "LUZ.AGUILERA" };
        var preauth = new Login2Fa.ContextoPreautenticacionModulo { CodigoModulo = "GESTOR DOCUMENTAL", IdUsuario = 0, LoginUsuario = "LUZ.AGUILERA" };
        Assert(!ordinary.EsValido(), "El contexto común debe seguir exigiendo usuario resuelto.");
        Assert(preauth.EsValido(), "El contexto especializado admite ID cero.");
        preauth.IdUsuario = 1;
        Assert(!preauth.EsValido(), "El contexto especializado no debe reutilizarse después de resolver identidad.");
    }

    private static void CentralConfigurationIsAuthoritativeAndExact()
    {
        var executor = new FakeExecutor();
        executor.ReaderTables.Enqueue(ModuleTable(0));
        var repository = new Login2Fa.OdbcSecondFactorLoginModuleRepository(new FakeConnectionFactory(), executor, Context("CATALOGO"));
        var module = repository.Resolve("EMPRESA A", "RAD GESTION");
        Equal(7, module.EmpresaId, "empresa central");
        Equal(11, module.ModuloId, "módulo central");
        Assert(!module.Configuration.IsRequired, "cero desactiva");
        Contains(executor.Commands.Single().Sql, "RAZON_SOCIAL_EMPRESA=?");
        Equal("EMPRESA A", executor.Commands.Single().Values[0], "parámetro empresa");
        Equal("RAD GESTION", executor.Commands.Single().Values[1], "parámetro módulo");

        executor = new FakeExecutor();
        executor.ReaderTables.Enqueue(ModuleTable(1));
        repository = new Login2Fa.OdbcSecondFactorLoginModuleRepository(new FakeConnectionFactory(), executor, Context("CATALOGO"));
        Assert(repository.Resolve("EMPRESA A", "RAD GESTION").Configuration.IsRequired, "uno activa");

        executor = new FakeExecutor();
        var duplicated = ModuleTable(0); duplicated.ImportRow(duplicated.Rows[0]); executor.ReaderTables.Enqueue(duplicated);
        repository = new Login2Fa.OdbcSecondFactorLoginModuleRepository(new FakeConnectionFactory(), executor, Context("CATALOGO"));
        Throws<InvalidOperationException>(() => repository.Resolve("EMPRESA A", "RAD GESTION"), "catálogo ambiguo");
    }

    private static void PrincipalAdaptersUseTheFourVerifiedContracts()
    {
        AssertPrincipal(new Login2Fa.MySqlDocuarchiSecondFactorPrincipalRepository(new FakeConnectionFactory(), Executor(PrincipalTable("Clave_Usuario", "idusuario", "correo"))), "usuarios_da", "Clave_Usuario");
        AssertPrincipal(new Login2Fa.MySqlGestorSecondFactorPrincipalRepository(new FakeConnectionFactory(), Executor(PrincipalTable("Id_Remit_Dest_Int", "Login_Usuario", "Correo_Electronico"))), "remit_dest_interno", "Id_Remit_Dest_Int");
        AssertPrincipal(new Login2Fa.MySqlRadicacionSecondFactorPrincipalRepository(new FakeConnectionFactory(), Executor(PrincipalTable("id_usuario", "Login_usuario", "Correo_Usuario"))), "usuario_radicador", "id_usuario");
        AssertPrincipal(new Login2Fa.MySqlWorkflowSecondFactorPrincipalRepository(new FakeConnectionFactory(), Executor(PrincipalTable("idU_suario", "login_Usuario", "Correo_Usuario"))), "usuario_workflow", "idU_suario");

        var duplicated = PrincipalTable("Id_Remit_Dest_Int", "Login_Usuario", "Correo_Electronico");
        duplicated.ImportRow(duplicated.Rows[0]);
        var duplicateRepository = new Login2Fa.MySqlGestorSecondFactorPrincipalRepository(new FakeConnectionFactory(), Executor(duplicated));
        Throws<InvalidOperationException>(() => duplicateRepository.Resolve(Context("GESTOR DOCUMENTAL")), "principal ambiguo");

        var missingEmail = PrincipalTable("Id_Remit_Dest_Int", "Login_Usuario", "Correo_Electronico");
        missingEmail.Rows[0]["Correo_Electronico"] = DBNull.Value;
        var missingEmailRepository = new Login2Fa.MySqlGestorSecondFactorPrincipalRepository(new FakeConnectionFactory(), Executor(missingEmail));
        Equal(String.Empty, missingEmailRepository.Resolve(Context("GESTOR DOCUMENTAL")).MaskedDestination, "correo opcional antes de conocer la bandera");
    }

    private static void DisabledConfigurationFinalizesOnce()
    {
        foreach (var type in ModuleTypes())
        {
            foreach (int? flag in new int?[] { 0, null })
            {
                var finalizer = new FakeFinalizer();
                var service = Service(type, flag, finalizer, new FakePrincipalRepository());
                var result = service.Execute(Request(type));
                Equal(Login2Fa.SecondFactorPreAuthenticationStatus.FINALIZED, result.Status, "estado apagado " + type);
                Equal(1, finalizer.Calls, "finalización única " + type);
                Equal(42L, finalizer.LastContext.InternalUserId, "ID autoritativo " + type);
                Equal(type, finalizer.LastContext.ModuleType, "tipo autoritativo " + type);

                var noEmailFinalizer = new FakeFinalizer();
                var noEmailPrincipal = new FakePrincipalRepository { Result = new Login2Fa.SecondFactorPrincipal(42, "LUZ.AGUILERA", null) };
                var noEmailResult = Service(type, flag, noEmailFinalizer, noEmailPrincipal).Execute(Request(type));
                Equal(Login2Fa.SecondFactorPreAuthenticationStatus.FINALIZED, noEmailResult.Status, "apagado sin correo " + type);
                Equal(1, noEmailFinalizer.Calls, "finalización sin correo " + type);
            }
        }
    }

    private static void ActiveConfigurationHasZeroAuthenticatedEffects()
    {
        foreach (var type in ModuleTypes())
        {
            var finalizer = new FakeFinalizer();
            var result = Service(type, 1, finalizer, new FakePrincipalRepository()).Execute(Request(type));
            Equal(Login2Fa.SecondFactorPreAuthenticationStatus.SECOND_FACTOR_REQUIRED, result.Status, "estado activo " + type);
            Equal("SECOND_FACTOR_REQUIRED", result.PublicCode, "código activo");
            Equal(0, finalizer.Calls, "cero finalización " + type);
            Equal("l***@example.test", result.Principal.MaskedDestination, "destino mínimo");
        }
    }

    private static void InvalidAndMissingDataFailClosed()
    {
        var finalizer = new FakeFinalizer();
        var missingModule = new Login2Fa.SecondFactorPreAuthenticationService(new FakeModuleRepository(null), Resolver(new FakePrincipalRepository()), finalizer);
        Equal(Login2Fa.SecondFactorPreAuthenticationStatus.REJECTED, missingModule.Execute(Request("GESTOR DOCUMENTAL")).Status, "módulo ausente");
        Equal(0, finalizer.Calls, "sin efectos por módulo ausente");

        var missingPrincipal = Service("GESTOR DOCUMENTAL", 1, finalizer, new FakePrincipalRepository { Result = null });
        Equal(Login2Fa.SecondFactorPreAuthenticationStatus.REJECTED, missingPrincipal.Execute(Request("GESTOR DOCUMENTAL")).Status, "principal ausente");
        Equal(0, finalizer.Calls, "sin efectos por principal ausente");

        var missingEmail = Service("GESTOR DOCUMENTAL", 1, finalizer, new FakePrincipalRepository { Result = new Login2Fa.SecondFactorPrincipal(42, "LUZ.AGUILERA", null) });
        var missingEmailResult = missingEmail.Execute(Request("GESTOR DOCUMENTAL"));
        Equal(Login2Fa.SecondFactorPreAuthenticationStatus.REJECTED, missingEmailResult.Status, "correo obligatorio con 2FA activo");
        Equal("SECOND_FACTOR_EMAIL_UNAVAILABLE", missingEmailResult.PublicCode, "código de correo ausente");
        Equal(0, finalizer.Calls, "sin efectos por correo ausente con 2FA activo");

        var invalidConfiguration = new FakeModuleRepositoryException();
        var service = new Login2Fa.SecondFactorPreAuthenticationService(invalidConfiguration, Resolver(new FakePrincipalRepository()), finalizer);
        Equal("PREAUTHENTICATION_FAILED", service.Execute(Request("GESTOR DOCUMENTAL")).PublicCode, "configuración inválida sanitizada");

        var firstCompanyExecutor = new FakeExecutor(); firstCompanyExecutor.ReaderTables.Enqueue(ModuleTable(0));
        var secondCompanyExecutor = new FakeExecutor(); secondCompanyExecutor.ReaderTables.Enqueue(ModuleTable(0));
        var firstCompany = new Login2Fa.OdbcSecondFactorLoginModuleRepository(new FakeConnectionFactory(), firstCompanyExecutor, Context("CATALOGO"));
        var secondCompany = new Login2Fa.OdbcSecondFactorLoginModuleRepository(new FakeConnectionFactory(), secondCompanyExecutor, Context("CATALOGO"));
        firstCompany.Resolve("EMPRESA A", "RAD GESTION"); secondCompany.Resolve("EMPRESA B", "RAD GESTION");
        Equal("EMPRESA A", firstCompanyExecutor.Commands.Single().Values[0], "aislamiento empresa A");
        Equal("EMPRESA B", secondCompanyExecutor.Commands.Single().Values[0], "aislamiento empresa B");
    }

    private static Login2Fa.SecondFactorPreAuthenticationService Service(string type, int? flag, FakeFinalizer finalizer, Login2Fa.ISecondFactorPrincipalRepository principal)
    {
        var config = new Login2Fa.SegundoFactorConfiguration(flag, flag == 1 ? (int?)1 : null, flag == 1 ? (int?)5 : null);
        var module = new Login2Fa.SecondFactorLoginModule(7, 11, "RAD GESTION", type, config);
        return new Login2Fa.SecondFactorPreAuthenticationService(new FakeModuleRepository(module), Resolver(principal), finalizer);
    }

    private static Login2Fa.SecondFactorPrincipalRepositoryResolver Resolver(Login2Fa.ISecondFactorPrincipalRepository repository)
    {
        return new Login2Fa.SecondFactorPrincipalRepositoryResolver(repository, repository, repository, repository);
    }

    private static Login2Fa.SecondFactorPreAuthenticationRequest Request(string type)
    {
        return new Login2Fa.SecondFactorPreAuthenticationRequest("EMPRESA A", "RAD GESTION", "LUZ.AGUILERA", Context(type));
    }

    private static Login2Fa.ContextoPreautenticacionModulo Context(string type)
    {
        return new Login2Fa.ContextoPreautenticacionModulo { CodigoModulo = type, IdUsuario = 0, LoginUsuario = "LUZ.AGUILERA" };
    }

    private static IEnumerable<string> ModuleTypes()
    {
        return new[] { "DOCUARCHI CONTENEDOR", "GESTOR DOCUMENTAL", "RADICACION DOCUMENTAL", "WORKFLOW DOCUMENTAL" };
    }

    private static void AssertPrincipal(Login2Fa.ISecondFactorPrincipalRepository repository, string table, string idColumn)
    {
        var principal = repository.Resolve(Context("MODULE"));
        Equal(42L, principal.InternalUserId, idColumn);
        Equal("LUZ.AGUILERA", principal.NormalizedLogin, "login canónico");
        var executor = ((FakeConnectionFactory)null); // La inspección SQL se realiza en la prueba Node estructural.
        Assert(principal.MaskedDestination.Contains("***@"), table);
    }

    private static FakeExecutor Executor(DataTable table) { var value = new FakeExecutor(); value.ReaderTables.Enqueue(table); return value; }

    private static DataTable ModuleTable(int required)
    {
        var table = new DataTable();
        table.Columns.Add("ID_EMPRESA", typeof(int)); table.Columns.Add("ID_MODULO", typeof(int));
        table.Columns.Add("NOMBRE_MODULO", typeof(string)); table.Columns.Add("TIPO_MODULO", typeof(string));
        table.Columns.Add("RequiereSegundoFactor", typeof(int)); table.Columns.Add("SecondFactorProviderType", typeof(int));
        table.Columns.Add("SegundoFactorTiempoExpira", typeof(int));
        table.Rows.Add(7, 11, "RAD GESTION", "GESTOR DOCUMENTAL", required, required == 1 ? (object)1 : DBNull.Value, required == 1 ? (object)5 : DBNull.Value);
        return table;
    }

    private static DataTable PrincipalTable(string id, string login, string email)
    {
        var table = new DataTable(); table.Columns.Add(id, typeof(long)); table.Columns.Add(login, typeof(string)); table.Columns.Add(email, typeof(string));
        table.Rows.Add(42L, "LUZ.AGUILERA", "luz@example.test"); return table;
    }

    private static void Assert(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
    private static void Equal<T>(T expected, T actual, string label) { if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new InvalidOperationException(label + ": esperado=" + expected + ", actual=" + actual); }
    private static void Contains(string value, string expected) { Assert(value.IndexOf(expected, StringComparison.OrdinalIgnoreCase) >= 0, "No contiene " + expected); }
    private static void Throws<T>(Action action, string label) where T : Exception { try { action(); } catch (T) { return; } throw new InvalidOperationException(label + ": no lanzó " + typeof(T).Name); }

    private sealed class FakeModuleRepository : Login2Fa.ISecondFactorLoginModuleRepository
    {
        private readonly Login2Fa.SecondFactorLoginModule _module; public FakeModuleRepository(Login2Fa.SecondFactorLoginModule module) { _module = module; }
        public Login2Fa.SecondFactorLoginModule Resolve(string companyName, string moduleName) { return _module; }
    }
    private sealed class FakeModuleRepositoryException : Login2Fa.ISecondFactorLoginModuleRepository
    {
        public Login2Fa.SecondFactorLoginModule Resolve(string companyName, string moduleName) { throw new InvalidOperationException("dato interno"); }
    }
    private sealed class FakePrincipalRepository : Login2Fa.ISecondFactorPrincipalRepository
    {
        public Login2Fa.SecondFactorPrincipal Result = new Login2Fa.SecondFactorPrincipal(42, "LUZ.AGUILERA", "luz@example.test");
        public Login2Fa.SecondFactorPrincipal Resolve(Login2Fa.ContextoPreautenticacionModulo context) { return Result; }
    }
    private sealed class FakeFinalizer : Login2Fa.ILegacyLoginFinalizer
    {
        public int Calls; public Login2Fa.LegacyLoginFinalizationContext LastContext;
        public Login2Fa.LegacyLoginFinalizationResult FinalizeLogin(Login2Fa.LegacyLoginFinalizationContext context)
        {
            Calls++; LastContext = context; return new Login2Fa.LegacyLoginFinalizationResult { Success = true, LocalRoute = "/legacy", PublicCode = "YES" };
        }
    }
    private sealed class RecordedCommand { public string Sql; public readonly List<object> Values = new List<object>(); }
    private sealed class FakeExecutor : Login2Fa.IDataExecutor
    {
        public readonly Queue<DataTable> ReaderTables = new Queue<DataTable>(); public readonly List<RecordedCommand> Commands = new List<RecordedCommand>();
        public int ExecuteNonQuery(IDbConnection c, IDbTransaction t, string s, IEnumerable<IDataParameter> p) { throw new NotSupportedException(); }
        public object ExecuteScalar(IDbConnection c, IDbTransaction t, string s, IEnumerable<IDataParameter> p) { throw new NotSupportedException(); }
        public T ExecuteReader<T>(IDbConnection c, IDbTransaction t, string s, IEnumerable<IDataParameter> p, Func<IDataReader, T> projector)
        {
            var command = new RecordedCommand { Sql = s }; foreach (var item in p ?? Enumerable.Empty<IDataParameter>()) command.Values.Add(item.Value); Commands.Add(command);
            using (var reader = ReaderTables.Dequeue().CreateDataReader()) return projector(reader);
        }
    }
    private sealed class FakeConnectionFactory : Login2Fa.IModuleConnectionFactory
    {
        public IDbConnection CreateOpenConnection(Login2Fa.ContextoModulo context) { return new FakeConnection(); }
    }
    private sealed class FakeConnection : IDbConnection
    {
        public string ConnectionString { get; set; } public int ConnectionTimeout { get { return 0; } } public string Database { get { return "fake"; } } public ConnectionState State { get { return ConnectionState.Open; } }
        public IDbTransaction BeginTransaction() { throw new NotSupportedException(); } public IDbTransaction BeginTransaction(IsolationLevel il) { throw new NotSupportedException(); }
        public void ChangeDatabase(string value) { } public void Close() { } public IDbCommand CreateCommand() { throw new NotSupportedException(); } public void Open() { } public void Dispose() { }
    }
}
