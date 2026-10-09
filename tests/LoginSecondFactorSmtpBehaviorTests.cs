using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using Login2Fa = GestionDocumental_Docuarchi.net;

internal static class LoginSecondFactorSmtpBehaviorTests
{
    private static readonly DateTime Expires = new DateTime(2026, 10, 9, 19, 30, 0, DateTimeKind.Utc);

    private static int Main()
    {
        try
        {
            ModelsAreImmutable();
            RepositoryEnforcesCardinalityAndContextSnapshot();
            RepositoryMapsAndCapsConfiguration();
            RepositoryRejectsInvalidRowsAndSanitizesFailures();
            FrameworkAdapterAppliesOptionsWithoutUsingDomain();
            TransportBuildsMinimalMessageAndDisposes();
            TransportFailsClosedBeforeClientAndOnSendError();
            SenderMapsOnlyApprovedPublicResults();
            Console.WriteLine("login-second-factor SMTP behavior tests: passed");
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error);
            return 1;
        }
    }

    private static void ModelsAreImmutable()
    {
        foreach (var type in new[] { typeof(Login2Fa.SecondFactorSmtpConfiguration), typeof(Login2Fa.SecondFactorSmtpConfigurationResolution), typeof(Login2Fa.SecondFactorSmtpDelivery) })
            Assert(type.GetProperties().All(property => !property.CanWrite), type.Name + " debe ser inmutable.");
        Equal(5, Enum.GetValues(typeof(Login2Fa.SecondFactorSmtpDeliveryStatus)).Length, "estados SMTP");
    }

    private static void RepositoryEnforcesCardinalityAndContextSnapshot()
    {
        var disabled = Resolve(ConfigurationTable());
        Equal(Login2Fa.SecondFactorSmtpDeliveryStatus.Disabled, disabled.Resolution.Status, "cero filas");
        Assert(disabled.Resolution.Configuration == null, "Disabled no expone configuración.");
        Assert(disabled.Connection.Disposed, "conexión liberada con cero filas");

        var table = ConfigurationTable();
        AddValidRow(table); AddValidRow(table);
        var ambiguous = Resolve(table);
        Equal(Login2Fa.SecondFactorSmtpDeliveryStatus.AmbiguousConfiguration, ambiguous.Resolution.Status, "múltiples filas");

        var context = Context();
        var executor = new FakeExecutor(ConfigurationTableWithValidRow());
        var factory = new FakeConnectionFactory();
        var repository = new Login2Fa.MySqlSecondFactorSmtpConfigurationRepository(factory, executor, context);
        context.CodigoModulo = "ALTERADO";
        repository.Resolve();
        Equal("RADICACION", factory.Context.CodigoModulo, "snapshot defensivo");
        Contains(executor.Sql, "WHERE ESTADO_ENVIO=@enabled");
        Equal(1, Convert.ToInt32(executor.Parameters.Single(p => p.ParameterName == "@enabled").Value), "parámetro enabled");
    }

    private static void RepositoryMapsAndCapsConfiguration()
    {
        var normal = Resolve(ConfigurationTableWithValidRow(timeout: 1));
        Equal(Login2Fa.SecondFactorSmtpDeliveryStatus.Submitted, normal.Resolution.Status, "configuración válida");
        var config = normal.Resolution.Configuration;
        Equal("smtp.example.test", config.Host, "host"); Equal(587, config.Port, "puerto");
        Equal(100000, config.TimeoutMilliseconds, "timeout compatible");
        Assert(config.EnableSsl && !config.UseDefaultCredentials && config.IsBodyHtml, "banderas");
        Equal("legacy-domain", config.Domain, "dominio preservado");

        var capped = Resolve(ConfigurationTableWithValidRow(timeout: 2));
        Equal(120000, capped.Resolution.Configuration.TimeoutMilliseconds, "timeout acotado");

        var defaults = ConfigurationTableWithValidRow(credential: 0, password: null);
        var defaultResult = Resolve(defaults).Resolution;
        Equal(Login2Fa.SecondFactorSmtpDeliveryStatus.Submitted, defaultResult.Status, "credencial predeterminada");
        Assert(defaultResult.Configuration.UseDefaultCredentials, "usa credenciales predeterminadas");
    }

    private static void RepositoryRejectsInvalidRowsAndSanitizesFailures()
    {
        var invalidTables = new[] {
            ConfigurationTableWithValidRow(host: null), ConfigurationTableWithValidRow(port: 0),
            ConfigurationTableWithValidRow(port: 65536), ConfigurationTableWithValidRow(username: "invalid"),
            ConfigurationTableWithValidRow(timeout: 0), ConfigurationTableWithValidRow(timeout: 21475),
            ConfigurationTableWithValidRow(ssl: 2), ConfigurationTableWithValidRow(enabled: 0),
            ConfigurationTableWithValidRow(body: 2), ConfigurationTableWithValidRow(credential: 2),
            ConfigurationTableWithValidRow(credential: 1, password: null)
        };
        foreach (var table in invalidTables)
            Equal(Login2Fa.SecondFactorSmtpDeliveryStatus.InvalidConfiguration, Resolve(table).Resolution.Status, "fila inválida");

        var factory = new FakeConnectionFactory { ThrowOnCreate = new InvalidOperationException("smtp-secret-marker") };
        var result = new Login2Fa.MySqlSecondFactorSmtpConfigurationRepository(factory, new FakeExecutor(ConfigurationTable()), Context()).Resolve();
        Equal(Login2Fa.SecondFactorSmtpDeliveryStatus.Failed, result.Status, "error de repositorio");
    }

    private static void FrameworkAdapterAppliesOptionsWithoutUsingDomain()
    {
        var config = Configuration(useDefaultCredentials: false);
        using (var adapter = new Login2Fa.FrameworkSmtpClientFactory().Create(config))
        {
            var field = adapter.GetType().GetField("_client", BindingFlags.Instance | BindingFlags.NonPublic);
            var client = (SmtpClient)field.GetValue(adapter);
            Equal(config.Host, client.Host, "host del framework"); Equal(config.Port, client.Port, "puerto del framework");
            Equal(config.TimeoutMilliseconds, client.Timeout, "timeout del framework"); Assert(client.EnableSsl, "SSL del framework");
            Assert(!client.UseDefaultCredentials, "credenciales explícitas");
            var credential = (NetworkCredential)client.Credentials.GetCredential(config.Host, config.Port, "smtp");
            Equal(config.Username, credential.UserName, "usuario"); Equal(config.Password, credential.Password, "contraseña");
            Assert(String.IsNullOrEmpty(credential.Domain), "DOMINIO_SMTP no se aplica.");
        }

        using (var adapter = new Login2Fa.FrameworkSmtpClientFactory().Create(Configuration(useDefaultCredentials: true)))
        {
            var field = adapter.GetType().GetField("_client", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert(((SmtpClient)field.GetValue(adapter)).UseDefaultCredentials, "credenciales predeterminadas aplicadas");
        }
    }

    private static void TransportBuildsMinimalMessageAndDisposes()
    {
        var factory = new FakeSmtpClientFactory();
        var result = new Login2Fa.SecondFactorSmtpTransport(factory).Send(Configuration(), Message());
        Equal(Login2Fa.SecondFactorSmtpDeliveryStatus.Submitted, result.Status, "envío aceptado");
        Equal("OTP_SUBMITTED", result.PublicCode, "código de envío");
        Assert(factory.Client.Disposed, "cliente liberado"); Equal(1, factory.Client.SendCalls, "un envío");
        Equal("Código de verificación", factory.Client.Subject, "asunto fijo");
        Equal("sender@example.test", factory.Client.From, "remitente"); Equal("user@example.test", factory.Client.To, "destinatario");
        Contains(factory.Client.Body, "123456"); Contains(factory.Client.Body, "2026-10-09 19:30 UTC");
        Contains(factory.Client.Body, "No comparta"); Contains(factory.Client.Body, "ignore este mensaje");
        Assert(factory.Client.Body.IndexOf("user@example.test", StringComparison.OrdinalIgnoreCase) < 0, "el cuerpo no contiene destinatario");
    }

    private static void TransportFailsClosedBeforeClientAndOnSendError()
    {
        var invalidRecipient = new Login2Fa.SecondFactorEmailMessage(new Login2Fa.SecondFactorRecipient("invalid", "u***@example.test"), "123456", Expires);
        var untouched = new FakeSmtpClientFactory();
        var invalid = new Login2Fa.SecondFactorSmtpTransport(untouched).Send(Configuration(), invalidRecipient);
        Equal(Login2Fa.SecondFactorSmtpDeliveryStatus.Failed, invalid.Status, "destinatario inválido"); Equal(0, untouched.CreateCalls, "sin cliente ante MailAddress inválido");

        var throwing = new FakeSmtpClientFactory { Client = new FakeSmtpClient { Error = new InvalidOperationException("secret-marker") } };
        var failed = new Login2Fa.SecondFactorSmtpTransport(throwing).Send(Configuration(), Message());
        Equal(Login2Fa.SecondFactorSmtpDeliveryStatus.Failed, failed.Status, "error SMTP");
        Equal("OTP_DELIVERY_FAILED", failed.PublicCode, "error sanitizado"); Assert(throwing.Client.Disposed, "cliente liberado tras error");
        Assert(failed.PublicCode.IndexOf("secret-marker", StringComparison.Ordinal) < 0, "sin secreto");
    }

    private static void SenderMapsOnlyApprovedPublicResults()
    {
        var cases = new Dictionary<Login2Fa.SecondFactorSmtpDeliveryStatus, string> {
            { Login2Fa.SecondFactorSmtpDeliveryStatus.Disabled, "OTP_DISABLED" },
            { Login2Fa.SecondFactorSmtpDeliveryStatus.InvalidConfiguration, "OTP_CONFIGURATION_INVALID" },
            { Login2Fa.SecondFactorSmtpDeliveryStatus.AmbiguousConfiguration, "OTP_CONFIGURATION_AMBIGUOUS" },
            { Login2Fa.SecondFactorSmtpDeliveryStatus.Failed, "OTP_DELIVERY_FAILED" }
        };
        foreach (var item in cases)
        {
            var result = new Login2Fa.SecondFactorSmtpEmailSender(new FakeConfigurationRepository(item.Key), new FakeTransport()).Send(Message());
            Assert(!result.Success, "estado no exitoso"); Equal(item.Value, result.PublicCode, "mapeo público");
        }

        var success = new Login2Fa.SecondFactorSmtpEmailSender(new FakeConfigurationRepository(Login2Fa.SecondFactorSmtpDeliveryStatus.Submitted), new FakeTransport()).Send(Message());
        Assert(success.Success, "Submitted exitoso"); Equal("OTP_SUBMITTED", success.PublicCode, "Submitted público");

        var malicious = new FakeTransport { Delivery = new Login2Fa.SecondFactorSmtpDelivery(Login2Fa.SecondFactorSmtpDeliveryStatus.Failed, "secret-marker") };
        var sanitized = new Login2Fa.SecondFactorSmtpEmailSender(new FakeConfigurationRepository(Login2Fa.SecondFactorSmtpDeliveryStatus.Submitted), malicious).Send(Message());
        Equal("OTP_DELIVERY_FAILED", sanitized.PublicCode, "fachada sanitiza transporte");

        var exception = new Login2Fa.SecondFactorSmtpEmailSender(new ThrowingConfigurationRepository(), new FakeTransport()).Send(Message());
        Equal("OTP_DELIVERY_FAILED", exception.PublicCode, "fachada sanitiza excepción");
    }

    private static RepositoryResult Resolve(DataTable table)
    {
        var executor = new FakeExecutor(table); var factory = new FakeConnectionFactory();
        var resolution = new Login2Fa.MySqlSecondFactorSmtpConfigurationRepository(factory, executor, Context()).Resolve();
        return new RepositoryResult(resolution, factory.Connection, executor);
    }

    private static Login2Fa.ContextoModulo Context() { return new Login2Fa.ContextoModulo { CodigoModulo = "RADICACION", IdUsuario = 7, IdGrupo = 0, LoginUsuario = "SYSTEM" }; }
    private static Login2Fa.SecondFactorSmtpConfiguration Configuration(bool useDefaultCredentials = false)
    {
        return new Login2Fa.SecondFactorSmtpConfiguration("smtp.example.test", 587, "sender@example.test", "sender@example.test", "secret", "legacy-domain", 120000, true, useDefaultCredentials, false);
    }
    private static Login2Fa.SecondFactorEmailMessage Message() { return new Login2Fa.SecondFactorEmailMessage(new Login2Fa.SecondFactorRecipient("user@example.test", "u***@example.test"), "123456", Expires); }

    private static DataTable ConfigurationTable()
    {
        var table = new DataTable();
        foreach (var name in new[] { "SERV_SMTP", "PUERTO_SERV_SMTP", "USUARIO_SMTP", "PASW_SMTP", "DOMINIO_SMTP", "SMTP_TIEMPO", "ESTADO_SSL", "ESTADO_ENVIO", "ESTADO_BODY", "ESTADO_CREDENCIAL" }) table.Columns.Add(name, typeof(object));
        return table;
    }
    private static DataTable ConfigurationTableWithValidRow(string host = "smtp.example.test", int port = 587, string username = "sender@example.test", string password = "secret", long timeout = 1, int ssl = 1, int enabled = 1, int body = 1, int credential = 1)
    {
        var table = ConfigurationTable(); AddValidRow(table, host, port, username, password, timeout, ssl, enabled, body, credential); return table;
    }
    private static void AddValidRow(DataTable table, string host = "smtp.example.test", int port = 587, string username = "sender@example.test", string password = "secret", long timeout = 1, int ssl = 1, int enabled = 1, int body = 1, int credential = 1)
    {
        table.Rows.Add(Value(host), port, Value(username), Value(password), "legacy-domain", timeout, ssl, enabled, body, credential);
    }
    private static object Value(string value) { return value == null ? (object)DBNull.Value : value; }

    private static void Assert(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
    private static void Contains(string value, string expected) { Assert(value != null && value.IndexOf(expected, StringComparison.Ordinal) >= 0, "No contiene: " + expected); }
    private static void Equal<T>(T expected, T actual, string label) { if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new InvalidOperationException(label + ": esperado=" + expected + ", actual=" + actual); }

    private sealed class RepositoryResult
    {
        public RepositoryResult(Login2Fa.SecondFactorSmtpConfigurationResolution resolution, FakeConnection connection, FakeExecutor executor) { Resolution = resolution; Connection = connection; Executor = executor; }
        public Login2Fa.SecondFactorSmtpConfigurationResolution Resolution; public FakeConnection Connection; public FakeExecutor Executor;
    }
    private sealed class FakeExecutor : Login2Fa.IDataExecutor
    {
        private readonly DataTable _table; public string Sql; public List<IDataParameter> Parameters;
        public FakeExecutor(DataTable table) { _table = table; }
        public int ExecuteNonQuery(IDbConnection c, IDbTransaction t, string s, IEnumerable<IDataParameter> p) { throw new NotSupportedException(); }
        public object ExecuteScalar(IDbConnection c, IDbTransaction t, string s, IEnumerable<IDataParameter> p) { throw new NotSupportedException(); }
        public T ExecuteReader<T>(IDbConnection c, IDbTransaction t, string s, IEnumerable<IDataParameter> p, Func<IDataReader, T> projector) { Sql = s; Parameters = p.ToList(); using (var reader = _table.CreateDataReader()) return projector(reader); }
    }
    private sealed class FakeConnectionFactory : Login2Fa.IModuleConnectionFactory
    {
        public FakeConnection Connection; public Login2Fa.ContextoModulo Context; public Exception ThrowOnCreate;
        public IDbConnection CreateOpenConnection(Login2Fa.ContextoModulo context) { if (ThrowOnCreate != null) throw ThrowOnCreate; Context = context; Connection = new FakeConnection(); return Connection; }
    }
    private sealed class FakeConnection : IDbConnection
    {
        public bool Disposed; public string ConnectionString { get; set; } public int ConnectionTimeout { get { return 0; } } public string Database { get { return "fake"; } } public ConnectionState State { get { return Disposed ? ConnectionState.Closed : ConnectionState.Open; } }
        public IDbTransaction BeginTransaction() { throw new NotSupportedException(); } public IDbTransaction BeginTransaction(IsolationLevel level) { throw new NotSupportedException(); } public void ChangeDatabase(string name) { } public void Close() { Disposed = true; } public IDbCommand CreateCommand() { throw new NotSupportedException(); } public void Open() { } public void Dispose() { Disposed = true; }
    }
    private sealed class FakeSmtpClientFactory : Login2Fa.ISecondFactorSmtpClientFactory
    {
        public int CreateCalls; public FakeSmtpClient Client = new FakeSmtpClient();
        public Login2Fa.ISecondFactorSmtpClient Create(Login2Fa.SecondFactorSmtpConfiguration configuration) { CreateCalls++; return Client; }
    }
    private sealed class FakeSmtpClient : Login2Fa.ISecondFactorSmtpClient
    {
        public int SendCalls; public bool Disposed; public Exception Error; public string Subject; public string Body; public string From; public string To;
        public void Send(MailMessage message) { SendCalls++; Subject = message.Subject; Body = message.Body; From = message.From.Address; To = message.To.Single().Address; if (Error != null) throw Error; }
        public void Dispose() { Disposed = true; }
    }
    private sealed class FakeConfigurationRepository : Login2Fa.ISecondFactorSmtpConfigurationRepository
    {
        private readonly Login2Fa.SecondFactorSmtpDeliveryStatus _status; public FakeConfigurationRepository(Login2Fa.SecondFactorSmtpDeliveryStatus status) { _status = status; }
        public Login2Fa.SecondFactorSmtpConfigurationResolution Resolve() { return new Login2Fa.SecondFactorSmtpConfigurationResolution(_status, _status == Login2Fa.SecondFactorSmtpDeliveryStatus.Submitted ? Configuration() : null); }
    }
    private sealed class ThrowingConfigurationRepository : Login2Fa.ISecondFactorSmtpConfigurationRepository { public Login2Fa.SecondFactorSmtpConfigurationResolution Resolve() { throw new InvalidOperationException("secret-marker"); } }
    private sealed class FakeTransport : Login2Fa.ISecondFactorSmtpTransport
    {
        public Login2Fa.SecondFactorSmtpDelivery Delivery = new Login2Fa.SecondFactorSmtpDelivery(Login2Fa.SecondFactorSmtpDeliveryStatus.Submitted, "OTP_SUBMITTED");
        public Login2Fa.SecondFactorSmtpDelivery Send(Login2Fa.SecondFactorSmtpConfiguration configuration, Login2Fa.SecondFactorEmailMessage message) { return Delivery; }
    }
}
