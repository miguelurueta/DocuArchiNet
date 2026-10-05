using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using Workflow = GestionDocumental_Docuarchi.net;

internal static class RegistroTareaRutaSiiMySqlIntegrationTests
{
    private const string WorkflowSchema = "doc87_workflow_it";
    private const string DocuarchiSchema = "doc87_docuarchi_it";
    private static string _baseConnectionString;

    private static int Main()
    {
        try
        {
            _baseConnectionString = ReadLocalTestConnection();
            RecreateSchemas();
            RollbackAtEveryWorkflowStage();
            HistoricalReceiptWithoutOutboxIsRejected();
            RetryAndConflictAreIdempotent();
            ConcurrentRegistrationCreatesOneTask();
            Console.WriteLine("doc87 mysql integration tests: passed");
            return 0;
        }
        catch (Exception error)
        {
            var mysql = error as MySqlException;
            Console.Error.WriteLine("DOC87_MYSQL_INTEGRATION_FAILED type=" + error.GetType().Name + " code=" + (mysql == null ? "NA" : mysql.Number.ToString()));
            return 1;
        }
        finally
        {
            try { DropSchemas(); }
            catch (Exception cleanupError) { Console.Error.WriteLine("DOC87_MYSQL_CLEANUP_FAILED type=" + cleanupError.GetType().Name); }
        }
    }

    private static void RollbackAtEveryWorkflowStage()
    {
        var stages = new[]
        {
            new { Table = "F_W_E_REGISTROPUBLICO", Column = "DATOS_RECIBO" },
            new { Table = "INICIO_TAREAS_WORKFLOW", Column = "Rutas_Workflow_id_Ruta" },
            new { Table = "DAT_ADIC_TARREGISTROPUBLICO", Column = "DATOS_RECIBO" },
            new { Table = "ESTADOS_TAREA_WORKFLOW", Column = "Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta" },
            new { Table = "workflow_registro_ruta_sii_outbox", Column = "operation_id" }
        };

        foreach (var stage in stages)
        {
            ResetWorkflowTables();
            Execute(WorkflowConnection(), "CREATE TRIGGER doc87_force_failure BEFORE INSERT ON " + stage.Table +
                " FOR EACH ROW SET NEW." + stage.Column + "=NULL");
            var failed = false;
            try { Repository().RegistrarTareaConOutbox(Context(), Data("S870000001")); }
            catch { failed = true; }
            True(failed, "la etapa " + stage.Table + " debía fallar");
            Equal(0L, Count(WorkflowConnection(), "F_W_E_REGISTROPUBLICO"), "rollback registro público");
            Equal(0L, Count(WorkflowConnection(), "INICIO_TAREAS_WORKFLOW"), "rollback tarea");
            Equal(0L, Count(WorkflowConnection(), "DAT_ADIC_TARREGISTROPUBLICO"), "rollback datos adicionales");
            Equal(0L, Count(WorkflowConnection(), "ESTADOS_TAREA_WORKFLOW"), "rollback estado");
            Equal(0L, Count(WorkflowConnection(), "workflow_registro_ruta_sii_outbox"), "rollback outbox");
        }
    }

    private static void RetryAndConflictAreIdempotent()
    {
        ResetWorkflowTables();
        ResetDocuarchiTables();
        Execute(DocuarchiConnection(), "INSERT INTO ra_sii_cache_exepediente (IdExpediente,Matricula,NombreGabinete) VALUES (7001,'407564','GABINETE-IT')");

        var repository = Repository();
        var registered = repository.RegistrarTareaConOutbox(Context(), Data("S870000002"));
        Equal("REGISTERED", registered.Codigo, "alta Workflow inicial");

        var unavailable = new Workflow.MySqlRelacionRutaSiiGateway(new FixedConnectionFactory(InvalidDocuarchiConnection()));
        var remoteFailed = false;
        try { unavailable.Materializar(Context(), registered.Evento); }
        catch { remoteFailed = true; repository.MarcarEventoPendiente(Context(), registered.Evento.OperationId, "DOCUARCHI_UNAVAILABLE"); }
        True(remoteFailed, "caída remota simulada");
        Equal("RETRYABLE", ScalarString(WorkflowConnection(), "SELECT status FROM workflow_registro_ruta_sii_outbox LIMIT 1"), "evento reintentable");

        var retry = repository.RegistrarTareaConOutbox(Context(), Data("S870000002"));
        Equal("ALREADY_REGISTERED", retry.Codigo, "reintento reutiliza evento");
        var gateway = Gateway();
        Equal("CONFIRMED", gateway.Materializar(Context(), retry.Evento), "relación creada");
        repository.ConfirmarEvento(Context(), retry.Evento.OperationId, "CONFIRMED");
        Equal("CONFIRMED", gateway.Materializar(Context(), retry.Evento), "relación repetida idempotente");
        Equal(1L, Count(WorkflowConnection(), "INICIO_TAREAS_WORKFLOW"), "reintento sin segunda tarea");
        Equal(1L, Count(DocuarchiConnection(), "ra_relacion_radicado_externo_expediente"), "reintento sin segunda relación");

        Execute(DocuarchiConnection(), "INSERT INTO ra_relacion_radicado_externo_expediente (expediente_archivo_ID_EXPEDIENTE,RadicadoExterno,FechaRegistro) VALUES (9009,'S870000003',NOW())");
        var conflict = new Workflow.EventoRelacionRutaSii { Recibo = "S870000003", Matricula = "S0407564", NombreGabinete = "GABINETE-IT" };
        Equal("RELATION_CONFLICT", gateway.Materializar(Context(), conflict), "conflicto no sobrescrito");
        Equal("9009", ScalarString(DocuarchiConnection(), "SELECT expediente_archivo_ID_EXPEDIENTE FROM ra_relacion_radicado_externo_expediente WHERE RadicadoExterno='S870000003'"), "expediente original conservado");
    }

    private static void HistoricalReceiptWithoutOutboxIsRejected()
    {
        ResetWorkflowTables();
        Execute(WorkflowConnection(), "INSERT INTO F_W_E_REGISTROPUBLICO (DATOS_RECIBO) VALUES ('S870000005')");
        var result = Repository().RegistrarTareaConOutbox(Context(), Data("S870000005"));
        Equal("ALREADY_REGISTERED", result.Codigo, "recibo histórico detectado");
        Equal(0L, Count(WorkflowConnection(), "INICIO_TAREAS_WORKFLOW"), "duplicado histórico sin tarea");
        Equal(0L, Count(WorkflowConnection(), "workflow_registro_ruta_sii_outbox"), "duplicado histórico sin outbox");
    }

    private static void ConcurrentRegistrationCreatesOneTask()
    {
        ResetWorkflowTables();
        var start = new ManualResetEventSlim(false);
        var results = new List<Workflow.ResultadoRegistroTareaRutaSii>();
        var sync = new object();
        var tasks = Enumerable.Range(0, 2).Select(_ => Task.Run(() =>
        {
            start.Wait();
            var value = Repository().RegistrarTareaConOutbox(Context(), Data("R870000004"));
            lock (sync) results.Add(value);
        })).ToArray();
        start.Set();
        Task.WaitAll(tasks);

        Equal(2, results.Count, "dos respuestas concurrentes");
        Equal(1, results.Count(r => r.Codigo == "REGISTERED"), "una solicitud registra");
        Equal(1, results.Count(r => r.Codigo == "ALREADY_REGISTERED"), "una solicitud reutiliza");
        Equal(1L, Count(WorkflowConnection(), "INICIO_TAREAS_WORKFLOW"), "una tarea concurrente");
        Equal(1L, Count(WorkflowConnection(), "workflow_registro_ruta_sii_outbox"), "un evento concurrente");
    }

    private static Workflow.MySqlRegistroTareaRutaSiiRepository Repository()
    {
        return new Workflow.MySqlRegistroTareaRutaSiiRepository(new FixedConnectionFactory(WorkflowConnection()), new Workflow.AdoNetDataExecutor());
    }

    private static Workflow.MySqlRelacionRutaSiiGateway Gateway()
    {
        return new Workflow.MySqlRelacionRutaSiiGateway(new FixedConnectionFactory(DocuarchiConnection()));
    }

    private static Workflow.ContextoModulo Context()
    {
        return new Workflow.ContextoModulo { CodigoModulo = "DOC87_IT", IdUsuario = 8700, LoginUsuario = "doc87-it" };
    }

    private static Workflow.DatosAutoritativosRegistroRutaSii Data(string receipt)
    {
        return new Workflow.DatosAutoritativosRegistroRutaSii
        {
            Recibo = receipt, CodigoBarras = "DOC87-BARCODE", Matricula = "S0407564", RazonSocial = "PRUEBA D'YCK/S.A.S",
            IdTramite = 87, NombreTramite = "INSCRIPCION", DescripcionTramite = "PRUEBA INTEGRACION", IdActividad = 870,
            CodigoSede = "IT", IdGabinete = 87, NombreGabinete = "GABINETE-IT", IdRuta = 87, NombreRuta = "REGISTROPUBLICO"
        };
    }

    private static void RecreateSchemas()
    {
        var adminConnection = AdminConnection();
        Execute(adminConnection, "DROP DATABASE IF EXISTS " + WorkflowSchema);
        Execute(adminConnection, "DROP DATABASE IF EXISTS " + DocuarchiSchema);
        Execute(adminConnection, "CREATE DATABASE " + WorkflowSchema);
        Execute(adminConnection, "CREATE DATABASE " + DocuarchiSchema);
    }

    private static void DropSchemas()
    {
        if (string.IsNullOrWhiteSpace(_baseConnectionString)) return;
        Execute(AdminConnection(), "DROP DATABASE IF EXISTS " + WorkflowSchema);
        Execute(AdminConnection(), "DROP DATABASE IF EXISTS " + DocuarchiSchema);
    }

    private static void ResetWorkflowTables()
    {
        Execute(WorkflowConnection(), "SET FOREIGN_KEY_CHECKS=0");
        foreach (var table in new[] { "workflow_registro_ruta_sii_outbox", "ESTADOS_TAREA_WORKFLOW", "DAT_ADIC_TARREGISTROPUBLICO", "INICIO_TAREAS_WORKFLOW", "F_W_E_REGISTROPUBLICO" })
            Execute(WorkflowConnection(), "DROP TABLE IF EXISTS " + table);
        Execute(WorkflowConnection(), "SET FOREIGN_KEY_CHECKS=1");
        Execute(WorkflowConnection(), "CREATE TABLE F_W_E_REGISTROPUBLICO (DATOS_RECIBO VARCHAR(10) NOT NULL,CODIGO_BARRAS VARCHAR(64),SECUENCIA_DOCUMENTO INT,RAZON_SOCIAL VARCHAR(120),MATRICULA VARCHAR(32),COD_SEDE VARCHAR(16),SECUENCIA_SERVICIO VARCHAR(8),TIPO_DOCUMENTO VARCHAR(120),FECHA_DOCUMENTO DATETIME,INSCRIPCION_DOCUMENTO VARCHAR(8),ID_GABINETE INT,NOMBRE_GABINETE VARCHAR(120),ID_RUTA INT,FLAG VARCHAR(4)) ENGINE=InnoDB");
        Execute(WorkflowConnection(), "CREATE TABLE INICIO_TAREAS_WORKFLOW (id_Tarea BIGINT NOT NULL AUTO_INCREMENT,Rutas_Workflow_id_Ruta INT NOT NULL,Fecha_Ini_Workflow DATETIME,Flag_sistema INT,id_dat_ext BIGINT,PRIMARY KEY(id_Tarea)) ENGINE=InnoDB");
        Execute(WorkflowConnection(), "CREATE TABLE DAT_ADIC_TARREGISTROPUBLICO (DATOS_RECIBO VARCHAR(10) NOT NULL,CODIGO_BARRAS VARCHAR(64),RAZON_SOCIAL VARCHAR(120),MATRICULA VARCHAR(32),SEDE VARCHAR(16),NOMBRE_GABINETE VARCHAR(120),FLUJO_INTERNO_WF INT,TRAMITE VARCHAR(120),DESCRIPCIONTRAMITE VARCHAR(120),INICIO_TAREAS_WORKFLOW_ID_TAREA BIGINT,ID_GABINETE INT,ID_IMAGEN BIGINT NULL,FLUJO_TRABAJO_WF INT) ENGINE=InnoDB");
        Execute(WorkflowConnection(), "CREATE TABLE ESTADOS_TAREA_WORKFLOW (Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta INT NOT NULL,Inicio_Tareas_Workflow_id_Tarea BIGINT,Id_Actividad INT,FECHA_INICIO DATETIME,ESTADO_PRIORIDAD INT,ESTADO_TAREA INT,Id_Usuario INT NULL,ID_FLUJO_TRABAJO INT,ID_ACTIVIDAD_FLUJO_TRABAJO INT,ID_USUARIO_WORKFLOW_FLUJO_TRABAJO INT) ENGINE=InnoDB");
        Execute(WorkflowConnection(), "CREATE TABLE workflow_registro_ruta_sii_outbox (operation_id CHAR(36) NOT NULL,route_id INT NOT NULL,route_name VARCHAR(80) NOT NULL,task_id BIGINT NOT NULL,receipt VARCHAR(10) NOT NULL,enrollment VARCHAR(32),cabinet_name VARCHAR(120),status VARCHAR(24) NOT NULL,attempts INT NOT NULL DEFAULT 0,last_error_code VARCHAR(64),created_at DATETIME NOT NULL,updated_at DATETIME NOT NULL,next_attempt_at DATETIME NULL,PRIMARY KEY(operation_id),UNIQUE KEY uq_doc87_route_receipt(route_id,receipt)) ENGINE=InnoDB");
    }

    private static void ResetDocuarchiTables()
    {
        Execute(DocuarchiConnection(), "DROP TABLE IF EXISTS ra_relacion_radicado_externo_expediente");
        Execute(DocuarchiConnection(), "DROP TABLE IF EXISTS ra_sii_cache_exepediente");
        Execute(DocuarchiConnection(), "CREATE TABLE ra_sii_cache_exepediente (IdExpediente BIGINT NOT NULL,Matricula VARCHAR(32) NOT NULL,NombreGabinete VARCHAR(120) NOT NULL) ENGINE=InnoDB");
        Execute(DocuarchiConnection(), "CREATE TABLE ra_relacion_radicado_externo_expediente (expediente_archivo_ID_EXPEDIENTE BIGINT NOT NULL,RadicadoExterno VARCHAR(10) NOT NULL,FechaRegistro DATETIME NOT NULL,UNIQUE KEY uq_doc87_receipt(RadicadoExterno)) ENGINE=InnoDB");
    }

    private static string ReadLocalTestConnection()
    {
        foreach (var view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
        {
            using (var hive = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
            using (var key = hive.OpenSubKey(@"SOFTWARE\ODBC\ODBC.INI\MembershipUsers"))
            {
                if (key == null) continue;
                var builder = new MySqlConnectionStringBuilder
                {
                    Server = Convert.ToString(key.GetValue("SERVER")), UserID = Convert.ToString(key.GetValue("UID")),
                    Password = Convert.ToString(key.GetValue("PWD")), Database = Convert.ToString(key.GetValue("DATABASE"))
                };
                uint port;
                if (uint.TryParse(Convert.ToString(key.GetValue("PORT")), out port)) builder.Port = port;
                return builder.ConnectionString;
            }
        }
        throw new InvalidOperationException("DOC87_TEST_DSN_NOT_FOUND");
    }

    private static string AdminConnection() { var b = new MySqlConnectionStringBuilder(_baseConnectionString) { Database = "" }; return b.ConnectionString; }
    private static string WorkflowConnection() { var b = new MySqlConnectionStringBuilder(_baseConnectionString) { Database = WorkflowSchema }; return b.ConnectionString; }
    private static string DocuarchiConnection() { var b = new MySqlConnectionStringBuilder(_baseConnectionString) { Database = DocuarchiSchema }; return b.ConnectionString; }
    private static string InvalidDocuarchiConnection() { var b = new MySqlConnectionStringBuilder(_baseConnectionString) { Database = "doc87_missing_schema" }; return b.ConnectionString; }

    private static MySqlConnection Open(string connectionString) { var connection = new MySqlConnection(connectionString); connection.Open(); return connection; }
    private static void Execute(string connectionString, string sql) { using (var c = Open(connectionString)) using (var cmd = new MySqlCommand(sql, c)) cmd.ExecuteNonQuery(); }
    private static long Count(string connectionString, string table) { return Convert.ToInt64(Scalar(connectionString, "SELECT COUNT(*) FROM " + table)); }
    private static string ScalarString(string connectionString, string sql) { return Convert.ToString(Scalar(connectionString, sql)); }
    private static object Scalar(string connectionString, string sql) { using (var c = Open(connectionString)) using (var cmd = new MySqlCommand(sql, c)) return cmd.ExecuteScalar(); }
    private static void True(bool value, string label) { if (!value) throw new InvalidOperationException(label); }
    private static void Equal<T>(T expected, T actual, string label) { if (!object.Equals(expected, actual)) throw new InvalidOperationException(label + ": esperado " + expected + ", recibido " + actual); }

    private sealed class FixedConnectionFactory : Workflow.IModuleConnectionFactory
    {
        private readonly string _connectionString;
        public FixedConnectionFactory(string connectionString) { _connectionString = connectionString; }
        public IDbConnection CreateOpenConnection(Workflow.ContextoModulo context) { return Open(_connectionString); }
    }
}
