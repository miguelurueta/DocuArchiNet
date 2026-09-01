using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Workflow = GestionDocumental_Docuarchi.net;

// Arnés de escritura con dobles locales; nunca abre MySQL ni una conexión de ambiente.
internal static class WorkflowNotesWriteRepositoryTests
{
    private const long TaskId = 843;
    private const int UserId = 101;

    private static int Main()
    {
        try
        {
            PreflightRejectsAnUnsafeSchemaWithoutWrites();
            PreflightExceptionsFailClosedWithoutWrites();
            RetryReturnsTheStoredResultWithoutWritingAgain();
            CreationReturnsCanonicalVersionAndStoresItForRetries();
            SuccessfulUpdateReturnsAReplacementVersionAndAuditsIt();
            ConditionalMutationsDoNotAuditCrossedResourcesOrConflicts();
            AuditFailureRollsBackTheWholeCreation();
            Console.WriteLine("workflow-notes-write repository tests: passed");
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error.Message);
            return 1;
        }
    }

    private static void PreflightRejectsAnUnsafeSchemaWithoutWrites()
    {
        var executor = new FakeExecutor { Preflight = Preflight(false) };
        var connection = new FakeConnection();
        var result = Repository(executor, connection).Crear(Context(), Task(), CreateRequest());

        Equal(Workflow.CodigosResultadoNotasWorkflow.Unavailable, result.Codigo, "preflight incompatible");
        Equal(0, executor.NonQueryCalls, "escrituras tras preflight incompatible");
        Equal(0, connection.TransactionsStarted, "transacción tras preflight incompatible");
    }

    private static void PreflightExceptionsFailClosedWithoutWrites()
    {
        var executor = new FakeExecutor { ThrowOnPreflight = true };
        var connection = new FakeConnection();
        var result = Repository(executor, connection).Actualizar(Context(), Task(), UpdateRequest());

        Equal(Workflow.CodigosResultadoNotasWorkflow.Unavailable, result.Codigo, "error de preflight");
        Equal(0, executor.NonQueryCalls, "escrituras tras error de preflight");
        Equal(0, connection.TransactionsStarted, "transacción tras error de preflight");
    }

    private static void RetryReturnsTheStoredResultWithoutWritingAgain()
    {
        var executor = new FakeExecutor { Preflight = Preflight(true), ExistingNoteId = 77L };
        var connection = new FakeConnection();
        var result = Repository(executor, connection).Crear(Context(), Task(), CreateRequest());

        Equal(Workflow.CodigosResultadoNotasWorkflow.Exito, result.Codigo, "reintento idempotente");
        Equal(77L, result.Nota.IdNota, "nota original de reintento");
        Equal(FakeExecutor.StoredVersion, result.Nota.Version, "versión original de reintento");
        Equal(0, executor.NonQueryCalls, "segunda nota o auditoría por reintento");
        Equal(1, connection.Transaction.CommitCalls, "confirmación de lectura idempotente");
    }

    private static void CreationReturnsCanonicalVersionAndStoresItForRetries()
    {
        var executor = new FakeExecutor { Preflight = Preflight(true), InsertedNoteId = 55L };
        var result = Repository(executor, new FakeConnection()).Crear(Context(), Task(), CreateRequest());

        Equal(Workflow.CodigosResultadoNotasWorkflow.Exito, result.Codigo, "creación condicionada");
        Equal(55L, result.Nota.IdNota, "identificador de creación");
        Equal(64, result.Nota.Version.Length, "ETag SHA-256 de creación");
        Assert(executor.Commands.Any(command => command.Contains("Version_Resultado=@version")), "respuesta idempotente almacenada");
        Assert(executor.Commands.Any(command => command.Contains("workflow_notas_version")), "versión vigente almacenada");
        Assert(executor.Commands.Any(command => command.Contains("estados_tarea_workflow")), "tarea vigente en la inserción");
    }

    private static void ConditionalMutationsDoNotAuditCrossedResourcesOrConflicts()
    {
        AssertCrossTaskIsRejectedBeforeMutation();
        AssertRejectedConditionalMutation("propietario distinto", UpdateRequest());
        AssertRejectedConditionalMutation("conflicto de versión", UpdateRequest());
        AssertRejectedConditionalMutation("tarea que cambió de estado", UpdateRequest());
        AssertRejectedConditionalDelete();
    }

    private static void SuccessfulUpdateReturnsAReplacementVersionAndAuditsIt()
    {
        var executor = new FakeExecutor { Preflight = Preflight(true), ConditionalRows = 1 };
        var result = Repository(executor, new FakeConnection()).Actualizar(Context(), Task(), UpdateRequest());

        Equal(Workflow.CodigosResultadoNotasWorkflow.Exito, result.Codigo, "actualización condicionada");
        Assert(result.Nota != null && result.Nota.Version != null && result.Nota.Version.Length == 64,
            "versión resultante de actualización");
        Assert(executor.Commands.Any(command => command.StartsWith("UPDATE ANOTACION_TAREA", StringComparison.Ordinal)),
            "actualización parametrizada");
        Assert(executor.Commands.Any(command => command.Contains("workflow_notas_version")),
            "versión vigente actualizada con la nota");
        Assert(executor.Commands.Any(command => command.Contains("wf_log_workflow")),
            "auditoría de actualización");
    }

    private static void AssertCrossTaskIsRejectedBeforeMutation()
    {
        var executor = new FakeExecutor { Preflight = Preflight(true) };
        var connection = new FakeConnection();
        var result = Repository(executor, connection).Actualizar(Context(), Task(), UpdateRequest(TaskId + 1));

        Equal(Workflow.CodigosResultadoNotasWorkflow.Unavailable, result.Codigo, "tarea cruzada");
        Equal(0, executor.NonQueryCalls, "mutación de tarea cruzada");
        Assert(!executor.Commands.Any(command => command.Contains("wf_log_workflow")), "auditoría de tarea cruzada");
    }

    private static void AssertRejectedConditionalMutation(string label, Workflow.SolicitudActualizarNotaWorkflow request)
    {
        var executor = new FakeExecutor { Preflight = Preflight(true), ConditionalRows = 0 };
        var connection = new FakeConnection();
        var result = Repository(executor, connection).Actualizar(Context(), Task(), request);

        Equal(Workflow.CodigosResultadoNotasWorkflow.VersionConflict, result.Codigo, label);
        Equal(1, executor.NonQueryCalls, $"mutación condicionada para {label}");
        var mutation = executor.Commands.First(command => command.StartsWith("UPDATE ANOTACION_TAREA", StringComparison.Ordinal));
        Assert(mutation.Contains("ID_ANOTACION=@idNota"), $"nota condicionada para {label}");
        Assert(mutation.Contains("INICIO_TAREAS_WORKFLOW_ID_TAREA=@idTarea"), $"tarea condicionada para {label}");
        Assert(mutation.Contains("ID_USUARIO=@idUsuario"), $"propietario condicionado para {label}");
        Assert(mutation.Contains("ID_ACTIVIDAD=@idActividad"), $"actividad condicionada para {label}");
        Assert(mutation.Contains("ESTADO_TAREA=1"), $"estado condicionado para {label}");
        Assert(mutation.Contains("estados_tarea_workflow"), $"tarea vigente condicionada para {label}");
        Assert(mutation.Contains("workflow_notas_version"), $"libro de versión para {label}");
        Assert(mutation.Contains("vn.Version_Nota=@version"), $"versión condicionada para {label}");
        Assert(mutation.Contains("@versionResultante"), $"versión resultante para {label}");
        Assert(mutation.Contains("@version"), $"versión condicionada para {label}");
        Assert(!executor.Commands.Any(command => command.Contains("wf_log_workflow")), $"auditoría para {label}");
    }

    private static void AssertRejectedConditionalDelete()
    {
        var executor = new FakeExecutor { Preflight = Preflight(true), ConditionalRows = 0 };
        var result = Repository(executor, new FakeConnection()).Eliminar(Context(), Task(), new Workflow.SolicitudEliminarNotaWorkflow
        {
            IdTarea = TaskId,
            IdNota = 77,
            Version = "versión-esperada"
        });

        Equal(Workflow.CodigosResultadoNotasWorkflow.VersionConflict, result.Codigo, "borrado con versión desactualizada");
        var mutation = executor.Commands.First(command => command.StartsWith("DELETE at, vn FROM ANOTACION_TAREA", StringComparison.Ordinal));
        Assert(mutation.Contains("workflow_notas_version"), "libro de versión al borrar");
        Assert(mutation.Contains("vn.Version_Nota=@version"), "versión condicionada al borrar");
        Assert(!executor.Commands.Any(command => command.Contains("wf_log_workflow")), "auditoría de borrado conflictivo");
    }

    private static void AuditFailureRollsBackTheWholeCreation()
    {
        var executor = new FakeExecutor { Preflight = Preflight(true), ThrowOnAudit = true, ExistingNoteId = 0L, InsertedNoteId = 55L };
        var connection = new FakeConnection();
        var result = Repository(executor, connection).Crear(Context(), Task(), CreateRequest());

        Equal(Workflow.CodigosResultadoNotasWorkflow.Unavailable, result.Codigo, "fallo de auditoría");
        Equal(0, connection.Transaction.CommitCalls, "commit tras fallo de auditoría");
        Equal(1, connection.Transaction.RollbackCalls, "rollback tras fallo de auditoría");
        Assert(executor.Commands.Any(command => command.Contains("workflow_notas_idempotencia")), "reserva idempotente");
        Assert(executor.Commands.Any(command => command.Contains("INSERT INTO ANOTACION_TAREA")), "inserción de nota");
        Assert(executor.Commands.Any(command => command.Contains("wf_log_workflow")), "intento de auditoría");
    }

    private static Workflow.MySqlNotasWorkflowRepository Repository(FakeExecutor executor, FakeConnection connection) =>
        new Workflow.MySqlNotasWorkflowRepository(new FakeFactory(connection), executor);

    private static Workflow.ContextoModuloWorkflow Context() => new Workflow.ContextoModuloWorkflow
    {
        IdUsuarioWorkflow = UserId,
        IdGrupoWorkflow = 202,
        IdRutaWorkflow = 303,
        LoginUsuario = "usuario-prueba",
        PuedeInteractuarAnotaciones = true
    };

    private static Workflow.TareaWorkflow Task() => new Workflow.TareaWorkflow
    {
        IdTarea = TaskId,
        IdRuta = 303,
        IdActividadOrigen = 6,
        EstaActiva = true,
        TokenVersion = "estado-1"
    };

    private static Workflow.SolicitudCrearNotaWorkflow CreateRequest() => new Workflow.SolicitudCrearNotaWorkflow
    {
        IdTarea = TaskId,
        Contenido = "nota de prueba",
        IdSolicitudCliente = "e2ca5ad4-24de-4c01-8d2d-ef7e243c269f"
    };

    private static Workflow.SolicitudActualizarNotaWorkflow UpdateRequest(long taskId = TaskId) => new Workflow.SolicitudActualizarNotaWorkflow
    {
        IdTarea = taskId,
        IdNota = 77,
        Contenido = "nota actualizada",
        Version = "versión-esperada"
    };

    private static DataTable Preflight(bool available)
    {
        var table = new DataTable();
        foreach (var name in new[]
        {
            "NOTAS_INNODB", "AUDIT_INNODB", "IDEMPOTENCIA_INNODB", "NOTAS_COLUMNAS", "NOTAS_TEXTO_UTF8", "AUDIT_COLUMNAS",
            "IDEMPOTENCIA_COLUMNAS", "NOTAS_INDICES", "AUDIT_INDICES", "IDEMPOTENCIA_UNICIDAD", "IDEMPOTENCIA_INDICES",
            "VERSIONES_INNODB", "VERSIONES_COLUMNAS", "VERSIONES_TIPO", "VERSIONES_INDICES"
        }) table.Columns.Add(name, typeof(int));

        table.Rows.Add(
            available ? 1 : 0,
            1,
            1,
            7,
            1,
            9,
            7,
            2,
            1,
            3,
            1,
            1,
            5,
            1,
            1);
        return table;
    }

    private static void Equal<T>(T expected, T actual, string label)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            throw new InvalidOperationException($"{label}: esperado {expected}, recibido {actual}.");
    }

    private static void Assert(bool condition, string label)
    {
        if (!condition) throw new InvalidOperationException(label);
    }

    private sealed class FakeFactory : Workflow.IModuleConnectionFactory
    {
        private readonly FakeConnection _connection;

        public FakeFactory(FakeConnection connection) => _connection = connection;

        public IDbConnection CreateOpenConnection(Workflow.ContextoModulo context) => _connection;
    }

    private sealed class FakeExecutor : Workflow.IDataExecutor
    {
        public const string StoredVersion = "d8f79250f5db0efcb443e0d28b3c770647be10378f3708d8fec359874b2ddc23";
        public DataTable Preflight = Preflight(true);
        public bool ThrowOnPreflight;
        public bool ThrowOnAudit;
        public int ConditionalRows = 1;
        public long ExistingNoteId;
        public long InsertedNoteId = 55L;
        public int NonQueryCalls;
        public readonly List<string> Commands = new List<string>();

        public int ExecuteNonQuery(IDbConnection connection, IDbTransaction transaction, string commandText, IEnumerable<IDataParameter> parameters)
        {
            NonQueryCalls++;
            Commands.Add(commandText);
            if (ThrowOnAudit && commandText.Contains("wf_log_workflow")) throw new InvalidOperationException("AUDIT_WRITE_FAILED");
            if (commandText.StartsWith("UPDATE ANOTACION_TAREA", StringComparison.Ordinal) || commandText.StartsWith("DELETE at, vn FROM ANOTACION_TAREA", StringComparison.Ordinal))
                return ConditionalRows;
            return 1;
        }

        public object ExecuteScalar(IDbConnection connection, IDbTransaction transaction, string commandText, IEnumerable<IDataParameter> parameters)
        {
            Commands.Add(commandText);
            if (commandText.Contains("SELECT LAST_INSERT_ID()")) return InsertedNoteId;
            if (commandText.Contains("workflow_notas_idempotencia")) return ExistingNoteId > 0 ? $"{ExistingNoteId}|{StoredVersion}" : "0|";
            return 0L;
        }

        public T ExecuteReader<T>(IDbConnection connection, IDbTransaction transaction, string commandText, IEnumerable<IDataParameter> parameters, Func<IDataReader, T> projector)
        {
            Commands.Add(commandText);
            if (ThrowOnPreflight) throw new InvalidOperationException("PREFLIGHT_UNAVAILABLE");
            using (var reader = Preflight.CreateDataReader()) return projector(reader);
        }
    }

    private sealed class FakeConnection : IDbConnection
    {
        public readonly FakeTransaction Transaction;
        public int TransactionsStarted;

        public FakeConnection()
        {
            Transaction = new FakeTransaction(this);
        }

        public string ConnectionString { get; set; }
        public int ConnectionTimeout => 0;
        public string Database => "fake";
        public ConnectionState State => ConnectionState.Open;
        public IDbTransaction BeginTransaction()
        {
            TransactionsStarted++;
            return Transaction;
        }
        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel) => BeginTransaction();
        public void ChangeDatabase(string databaseName) { }
        public void Close() { }
        public IDbCommand CreateCommand() => null;
        public void Open() { }
        public void Dispose() { }
    }

    private sealed class FakeTransaction : IDbTransaction
    {
        private readonly IDbConnection _connection;
        public int CommitCalls;
        public int RollbackCalls;

        public FakeTransaction(IDbConnection connection) => _connection = connection;

        public IDbConnection Connection => _connection;
        public IsolationLevel IsolationLevel => IsolationLevel.ReadCommitted;
        public void Commit() => CommitCalls++;
        public void Rollback() => RollbackCalls++;
        public void Dispose() { }
    }
}
