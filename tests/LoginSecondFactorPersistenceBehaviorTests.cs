using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Login2Fa = GestionDocumental_Docuarchi.net;

internal static class LoginSecondFactorPersistenceBehaviorTests
{
    private static readonly DateTime Now = new DateTime(2026, 10, 9, 18, 0, 0, DateTimeKind.Utc);

    private static int Main()
    {
        try
        {
            CreatesVersionedChallengeWithoutAuthPayload();
            ReadsCanonicalProjectionWithoutInventingLogin();
            FifthAttemptBlocksAtomically();
            FinalizationUsesRowLockAndExpectedState();
            ReplacementFailureRollsBackRevocation();
            InvalidProtectedCodeFailsBeforeOpeningConnection();
            Console.WriteLine("login-second-factor persistence behavior tests: passed");
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error);
            return 1;
        }
    }

    private static void CreatesVersionedChallengeWithoutAuthPayload()
    {
        var executor = new FakeExecutor();
        executor.NonQueryResults.Enqueue(1);
        var fixture = Fixture(executor);
        Assert(fixture.Repository.Create(Challenge(0), ProtectedCode(), "session-hash"), "creación");
        Equal(1, fixture.Transaction.Commits, "commit de creación");
        Equal(0, fixture.Transaction.Rollbacks, "rollback de creación");
        var command = executor.Commands.Single();
        Contains(command.Sql, "AuthPayloadJson");
        Contains(command.Sql, "@schemaVersion,NULL");
        Equal("7:11:RADICADOR:42", command.Value("@authUserId"), "identidad canónica");
        Equal(1, Convert.ToInt32(command.Value("@schemaVersion")), "versión");
        Equal("active", command.Value("@keyId"), "key id");
    }

    private static void ReadsCanonicalProjectionWithoutInventingLogin()
    {
        var executor = new FakeExecutor();
        executor.ReaderTables.Enqueue(Row(Login2Fa.SegundoFactorChallengeState.SENT, 0, 0, Now));
        var fixture = Fixture(executor);
        var data = fixture.Repository.GetVerificationData(KnownId(), "session-hash");
        Equal("7:11:RADICADOR:42", data.Challenge.CanonicalIdentity, "proyección canónica");
        Equal(ProtectedCode(), data.ProtectedCode, "HMAC protegido");
        Throws<NotSupportedException>(() => fixture.Repository.GetForVerification(KnownId(), "session-hash"), "adaptador legacy explícito");
        Assert(executor.Commands.All(c => c.Sql.IndexOf("usuario_", StringComparison.OrdinalIgnoreCase) < 0), "No consulta tablas de usuarios.");
    }

    private static void FifthAttemptBlocksAtomically()
    {
        var executor = new FakeExecutor();
        executor.ReaderTables.Enqueue(Row(Login2Fa.SegundoFactorChallengeState.SENT, 4, 0, Now));
        executor.NonQueryResults.Enqueue(1);
        var fixture = Fixture(executor);
        var result = fixture.Repository.RegisterFailedAttemptData(KnownId(), 4);
        Equal(5, result.Attempts, "quinto intento");
        Equal(Login2Fa.SegundoFactorChallengeState.BLOCKED, result.State, "bloqueo");
        Assert(executor.Commands[0].Sql.EndsWith("FOR UPDATE", StringComparison.Ordinal), "lectura bloqueante");
        Equal("BLOCKED", executor.Commands[1].Value("@nextState"), "estado persistido");
        Equal(1, fixture.Transaction.Commits, "commit del bloqueo");
    }

    private static void FinalizationUsesRowLockAndExpectedState()
    {
        var executor = new FakeExecutor();
        executor.ReaderTables.Enqueue(Row(Login2Fa.SegundoFactorChallengeState.SENT, 1, 0, Now));
        executor.NonQueryResults.Enqueue(1);
        var fixture = Fixture(executor);
        Assert(fixture.Repository.TryBeginFinalization(KnownId(), 1), "adquisición");
        Contains(executor.Commands[0].Sql, "FOR UPDATE");
        Contains(executor.Commands[1].Sql, "State=@expectedState");
        Equal("FINALIZING", executor.Commands[1].Value("@nextState"), "estado adquirido");
        Equal(0, Convert.ToInt32(executor.Commands[1].Value("@expectedAttempts")) - 1, "intento esperado");
    }

    private static void ReplacementFailureRollsBackRevocation()
    {
        var executor = new FakeExecutor();
        executor.ReaderTables.Enqueue(Row(Login2Fa.SegundoFactorChallengeState.SENT, 0, 0, Now.AddMinutes(-2)));
        executor.NonQueryResults.Enqueue(1);
        executor.NonQueryResults.Enqueue(0);
        var fixture = Fixture(executor);
        Throws<InvalidOperationException>(() => fixture.Repository.ReplaceForResend(KnownId(), Challenge(1, Guid.NewGuid()), ProtectedCode(), "session-hash", Now), "insert fallido");
        Equal(0, fixture.Transaction.Commits, "sin commit parcial");
        Equal(1, fixture.Transaction.Rollbacks, "rollback de revocación");
    }

    private static void InvalidProtectedCodeFailsBeforeOpeningConnection()
    {
        var fixture = Fixture(new FakeExecutor());
        Throws<ArgumentException>(() => fixture.Repository.Create(Challenge(0), "not-a-hmac", "session-hash"), "HMAC inválido");
        Equal(0, fixture.Factory.Calls, "conexión no abierta");
    }

    private static FixtureData Fixture(FakeExecutor executor)
    {
        var connection = new FakeConnection();
        var transaction = new FakeTransaction(connection);
        var factory = new FakeConnectionFactory(connection);
        var repository = new Login2Fa.MySqlSecondFactorChallengeRepository(factory, executor, new FakeTransactionFactory(transaction),
            new Login2Fa.ContextoModulo { CodigoModulo = "LOGIN_2FA", IdUsuario = 1, IdGrupo = 0, LoginUsuario = "SYSTEM" }, new FakeClock(Now));
        return new FixtureData(repository, factory, transaction);
    }

    private static Login2Fa.SegundoFactorChallenge Challenge(int resendCount, Guid? id = null)
    {
        var identity = new Login2Fa.SegundoFactorIdentity(7, 11, "RADICADOR", "42", "LUZ.AGUILERA");
        return new Login2Fa.SegundoFactorChallenge(id ?? KnownId(), identity, Login2Fa.SegundoFactorPurpose.LOGIN,
            Login2Fa.SegundoFactorChallengeState.CREATED, 0, resendCount, Now, Now.AddMinutes(5));
    }

    private static Guid KnownId() { return new Guid("11111111-2222-3333-4444-555555555555"); }
    private static string ProtectedCode() { return "v1:active:" + Convert.ToBase64String(new byte[32]); }

    private static DataTable Row(Login2Fa.SegundoFactorChallengeState state, int attempts, int resendCount, DateTime lastSent)
    {
        var table = new DataTable();
        table.Columns.Add("ChallengeId", typeof(string)); table.Columns.Add("AuthUserId", typeof(string));
        table.Columns.Add("Purpose", typeof(string)); table.Columns.Add("State", typeof(string));
        table.Columns.Add("Attempts", typeof(int)); table.Columns.Add("ResendCount", typeof(int));
        table.Columns.Add("CreatedAtUtc", typeof(DateTime)); table.Columns.Add("ExpiresAtUtc", typeof(DateTime));
        table.Columns.Add("LastSentAtUtc", typeof(DateTime)); table.Columns.Add("TerminalAtUtc", typeof(DateTime));
        table.Columns.Add("UpdatedAtUtc", typeof(DateTime)); table.Columns.Add("SchemaVersion", typeof(int));
        table.Columns.Add("SessionBindingHash", typeof(string)); table.Columns.Add("CodeHash", typeof(string));
        table.Columns.Add("Consumed", typeof(bool)); table.Columns.Add("KeyId", typeof(string));
        table.Rows.Add(KnownId().ToString("D"), "7:11:RADICADOR:42", "LOGIN", state.ToString(), attempts, resendCount,
            Now.AddMinutes(-3), Now.AddMinutes(5), lastSent, DBNull.Value, Now, 1, "session-hash", ProtectedCode(), false, "active");
        return table;
    }

    private static void Assert(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
    private static void Contains(string value, string expected) { Assert(value.IndexOf(expected, StringComparison.Ordinal) >= 0, "No contiene: " + expected); }
    private static void Equal<T>(T expected, T actual, string label) { if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new InvalidOperationException(label + ": esperado=" + expected + ", actual=" + actual); }
    private static void Throws<T>(Action action, string label) where T : Exception { try { action(); } catch (T) { return; } throw new InvalidOperationException(label + ": no lanzó " + typeof(T).Name); }

    private sealed class FixtureData
    {
        public FixtureData(Login2Fa.MySqlSecondFactorChallengeRepository repository, FakeConnectionFactory factory, FakeTransaction transaction) { Repository = repository; Factory = factory; Transaction = transaction; }
        public Login2Fa.MySqlSecondFactorChallengeRepository Repository { get; private set; }
        public FakeConnectionFactory Factory { get; private set; }
        public FakeTransaction Transaction { get; private set; }
    }

    private sealed class RecordedCommand
    {
        public string Sql; public Dictionary<string, object> Values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        public object Value(string name) { return Values[name]; }
    }

    private sealed class FakeExecutor : Login2Fa.IDataExecutor
    {
        public readonly Queue<int> NonQueryResults = new Queue<int>();
        public readonly Queue<DataTable> ReaderTables = new Queue<DataTable>();
        public readonly List<RecordedCommand> Commands = new List<RecordedCommand>();
        public int ExecuteNonQuery(IDbConnection connection, IDbTransaction transaction, string sql, IEnumerable<IDataParameter> parameters) { Commands.Add(Record(sql, parameters)); return NonQueryResults.Dequeue(); }
        public object ExecuteScalar(IDbConnection connection, IDbTransaction transaction, string sql, IEnumerable<IDataParameter> parameters) { throw new NotSupportedException(); }
        public T ExecuteReader<T>(IDbConnection connection, IDbTransaction transaction, string sql, IEnumerable<IDataParameter> parameters, Func<IDataReader, T> projector)
        {
            Commands.Add(Record(sql, parameters));
            using (var reader = ReaderTables.Dequeue().CreateDataReader()) return projector(reader);
        }
        private static RecordedCommand Record(string sql, IEnumerable<IDataParameter> parameters)
        {
            var result = new RecordedCommand { Sql = sql };
            foreach (var parameter in parameters ?? Enumerable.Empty<IDataParameter>()) result.Values[parameter.ParameterName] = parameter.Value;
            return result;
        }
    }

    private sealed class FakeConnectionFactory : Login2Fa.IModuleConnectionFactory
    {
        private readonly IDbConnection _connection; public int Calls;
        public FakeConnectionFactory(IDbConnection connection) { _connection = connection; }
        public IDbConnection CreateOpenConnection(Login2Fa.ContextoModulo context) { Calls++; return _connection; }
    }
    private sealed class FakeClock : Login2Fa.ISecondFactorClock
    {
        private readonly DateTime _value; public FakeClock(DateTime value) { _value = value; }
        public DateTime UtcNow { get { return _value; } }
    }
    private sealed class FakeTransactionFactory : Login2Fa.ITransactionFactory
    {
        private readonly IDbTransaction _transaction; public FakeTransactionFactory(IDbTransaction transaction) { _transaction = transaction; }
        public IDbTransaction BeginTransaction(IDbConnection connection) { return _transaction; }
    }
    private sealed class FakeTransaction : IDbTransaction
    {
        public FakeTransaction(IDbConnection connection) { Connection = connection; }
        public int Commits; public int Rollbacks; public IDbConnection Connection { get; private set; }
        public IsolationLevel IsolationLevel { get { return IsolationLevel.ReadCommitted; } }
        public void Commit() { Commits++; } public void Rollback() { Rollbacks++; } public void Dispose() { }
    }
    private sealed class FakeConnection : IDbConnection
    {
        public string ConnectionString { get; set; } public int ConnectionTimeout { get { return 0; } } public string Database { get { return "fake"; } }
        public ConnectionState State { get { return ConnectionState.Open; } }
        public IDbTransaction BeginTransaction() { throw new NotSupportedException(); } public IDbTransaction BeginTransaction(IsolationLevel il) { throw new NotSupportedException(); }
        public void ChangeDatabase(string databaseName) { } public void Close() { } public IDbCommand CreateCommand() { throw new NotSupportedException(); }
        public void Open() { } public void Dispose() { }
    }
}
