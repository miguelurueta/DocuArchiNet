using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Workflow = GestionDocumental_Docuarchi.net;

// Arnés de repositorio con conexión y ejecutor simulados; jamás abre MySQL.
internal static class WorkflowNotesReadRepositoryTests
{
    private const long TaskId = 843;

    private static int Main()
    {
        try
        {
            ListUsesBoundParametersAndStableOrder();
            CountUsesCountStarAndOperationalVisibility();
            ContentMissDoesNotExposeForeignNote();
            Console.WriteLine("workflow-notes-read repository tests: passed");
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error.Message);
            return 1;
        }
    }

    private static void ListUsesBoundParametersAndStableOrder()
    {
        var data = new DataTable();
        data.Columns.Add("ID_NOTA", typeof(long));
        data.Columns.Add("ID_TAREA", typeof(long));
        data.Columns.Add("ID_AUTOR", typeof(int));
        data.Columns.Add("ID_ACTIVIDAD_ORIGEN", typeof(int));
        data.Columns.Add("CONTENIDO_VERSION", typeof(string));
        data.Columns.Add("FECHA_CREACION", typeof(DateTime));
        data.Rows.Add(7L, TaskId, 101, 6, "nota más reciente", new DateTime(2026, 1, 2, 3, 4, 5));
        data.Rows.Add(6L, TaskId, 101, 6, "nota anterior", new DateTime(2026, 1, 2, 3, 4, 4));
        var executor = new FakeExecutor(data);
        var response = Repository(executor).Listar(Context(), Task(), new Workflow.SolicitudListarNotasWorkflow { IdTarea = TaskId, TamanoPagina = 1 });

        Assert(executor.Sql.Contains("ORDER BY at.FECHA_ANOTACION DESC, at.ID_ANOTACION DESC"), "orden estable");
        Assert(executor.Sql.Contains("@idTarea") && executor.Sql.Contains("@limite"), "parámetros de listado");
        Assert(executor.Sql.IndexOf("SELECT *", StringComparison.OrdinalIgnoreCase) < 0, "listado sin SELECT estrella");
        Assert(executor.Parameters.Contains("@idTarea") && executor.Parameters.Contains("@limite"), "valores ligados");
        Equal(1, response.Notas.Count, "tamaño de página");
        Assert(response.TieneMas, "fila adicional");
        Assert(response.Notas[0].Version.Length == 64, "ETag calculado en .NET desde contenido persistido");
    }

    private static void CountUsesCountStarAndOperationalVisibility()
    {
        var data = new DataTable();
        data.Columns.Add("TOTAL", typeof(int));
        data.Rows.Add(3);
        var executor = new FakeExecutor(data);
        var response = Repository(executor).Contar(Context(), Task(), new Workflow.SolicitudContarNotasWorkflow { IdTarea = TaskId });

        Assert(executor.Sql.Contains("COUNT(*)"), "COUNT estrella");
        Assert(executor.Sql.Contains("at.ESTADO_TAREA = 1"), "visibilidad operativa");
        Assert(executor.Sql.IndexOf("SELECT *", StringComparison.OrdinalIgnoreCase) < 0, "contador sin SELECT estrella");
        Equal(3, response.Contador, "contador parametrizado");
    }

    private static void ContentMissDoesNotExposeForeignNote()
    {
        var data = new DataTable();
        var executor = new FakeExecutor(data);
        var response = Repository(executor).Consultar(Context(), Task(), new Workflow.SolicitudConsultarNotaWorkflow { IdTarea = TaskId, IdNota = 99 });

        Equal(Workflow.CodigosResultadoNotasWorkflow.NoteNotFound, response.Codigo, "nota ausente o ajena");
        Assert(response.Nota == null, "contenido de nota ajena");
        Assert(executor.Sql.Contains("at.ID_ANOTACION = @idNota"), "filtro por nota");
        Assert(executor.Sql.Contains("at.INICIO_TAREAS_WORKFLOW_ID_TAREA = @idTarea"), "filtro por tarea");
        Assert(executor.Parameters.Contains("@idNota") && executor.Parameters.Contains("@idTarea"), "parámetros de contenido");
    }

    private static Workflow.MySqlNotasWorkflowRepository Repository(FakeExecutor executor) =>
        new Workflow.MySqlNotasWorkflowRepository(new FakeFactory(), executor);

    private static Workflow.ContextoModuloWorkflow Context() => new Workflow.ContextoModuloWorkflow
    {
        IdUsuarioWorkflow = 101,
        IdGrupoWorkflow = 202,
        IdRutaWorkflow = 303,
        LoginUsuario = "usuario-prueba",
        PuedeInteractuarAnotaciones = true
    };

    private static Workflow.TareaWorkflow Task() => new Workflow.TareaWorkflow
    {
        IdTarea = TaskId,
        IdRuta = 303,
        EstaActiva = true,
        TokenVersion = "estado-1"
    };

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
        public IDbConnection CreateOpenConnection(Workflow.ContextoModulo context) => new FakeConnection();
    }

    private sealed class FakeExecutor : Workflow.IDataExecutor
    {
        private readonly DataTable _data;
        public string Sql = string.Empty;
        public HashSet<string> Parameters = new HashSet<string>(StringComparer.Ordinal);

        public FakeExecutor(DataTable data) => _data = data;

        public int ExecuteNonQuery(IDbConnection connection, IDbTransaction transaction, string commandText, IEnumerable<IDataParameter> parameters) => 0;

        public object ExecuteScalar(IDbConnection connection, IDbTransaction transaction, string commandText, IEnumerable<IDataParameter> parameters) => null;

        public T ExecuteReader<T>(IDbConnection connection, IDbTransaction transaction, string commandText, IEnumerable<IDataParameter> parameters, Func<IDataReader, T> projector)
        {
            Sql = commandText;
            Parameters = new HashSet<string>((parameters ?? Enumerable.Empty<IDataParameter>()).Select(parameter => parameter.ParameterName), StringComparer.Ordinal);
            using (var reader = _data.CreateDataReader()) return projector(reader);
        }
    }

    private sealed class FakeConnection : IDbConnection
    {
        public string ConnectionString { get; set; }
        public int ConnectionTimeout => 0;
        public string Database => "fake";
        public ConnectionState State => ConnectionState.Open;
        public IDbTransaction BeginTransaction() => null;
        public IDbTransaction BeginTransaction(IsolationLevel il) => null;
        public void ChangeDatabase(string databaseName) { }
        public void Close() { }
        public IDbCommand CreateCommand() => null;
        public void Open() { }
        public void Dispose() { }
    }
}
