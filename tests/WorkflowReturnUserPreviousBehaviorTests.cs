using System;
using System.Collections.Generic;
using System.Data;
using Workflow = GestionDocumental_Docuarchi.net;

// Arnés ejecutable contra el ensamblado VB compilado. Los puertos se duplican
// para verificar el comportamiento del caso de uso sin requerir ambiente ni BD.
internal static class WorkflowReturnUserPreviousBehaviorTests
{
    private const long TaskId = 843;

    private static int Main()
    {
        try
        {
            PreviewValidHistoryIsReadOnly();
            PreviewBlocksMissingPermissionBeforeReads();
            PreviewBlocksUnavailableTask();
            PreviewBlocksAbsentHistory();
            PreviewBlocksGroupHistory();
            PreviewBlocksRetiredOrInconsistentHistory();
            PreviewBlocksSelfReturnUsingAuthenticatedWorkflowUser();
            RepositoryPreservesDistinctHistoricalFlowUser();
            ExecutionBlocksChangedTokenBeforeMotor();
            ExecutionSerializesDistinctTokensBeforeSecondMotorCall();
            AdapterUsesApprovedNoResponseParameters();
            ExecutionKeepsSuccessWhenAuditFails();
            Console.WriteLine("workflow-return-user-previous behavior tests: passed");
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error.Message);
            return 1;
        }
    }

    private static void PreviewValidHistoryIsReadOnly()
    {
        var ports = new Ports();
        var response = CreateService(ports).Previsualizar(Context(), new Workflow.SolicitudPreviewDevolverUsuarioAnterior { IdTarea = TaskId });

        Assert(response.Error == null, "El preview válido no debe bloquearse.");
        Equal("opaque-preview-token", response.TokenVersion, "token del preview");
        Equal("Auxiliar contable", response.Contexto.ActividadAnterior, "actividad histórica");
        Equal("Usuario histórico", response.Contexto.UsuarioAnterior, "usuario histórico");
        Equal(1, ports.Tareas.Calls, "lectura de tarea");
        Equal(1, ports.Autorizacion.Calls, "lectura de autorización");
        Equal(1, ports.Historial.Calls, "lectura de historial");
        Equal(1, ports.Token.EmitCalls, "emisión de token");
        Equal(0, ports.Guard.Calls, "lock en preview");
        Equal(0, ports.Ejecutor.Calls, "mutación en preview");
        Equal(0, ports.Auditoria.Calls, "auditoría en preview");
    }

    private static void PreviewBlocksAbsentHistory()
    {
        var ports = new Ports
        {
            Historial = new FakeHistorial
            {
                Result = BlockedHistory(Workflow.CodigosBloqueoDevolverUsuarioAnterior.HistorialNoDisponible)
            }
        };
        var response = CreateService(ports).Previsualizar(Context(), new Workflow.SolicitudPreviewDevolverUsuarioAnterior { IdTarea = TaskId });

        Equal(Workflow.CodigosBloqueoDevolverUsuarioAnterior.HistorialNoDisponible, response.Error.Codigo, "bloqueo por historial ausente");
        AssertNoPreviewWrite(ports, "historial ausente");
    }

    private static void PreviewBlocksMissingPermissionBeforeReads()
    {
        var ports = new Ports();
        var context = Context();
        context.PuedeDevolverUsuarioAnterior = false;
        var response = CreateService(ports).Previsualizar(context, new Workflow.SolicitudPreviewDevolverUsuarioAnterior { IdTarea = TaskId });

        Equal(Workflow.CodigosBloqueoDevolverUsuarioAnterior.PermisoDenegado, response.Error.Codigo, "bloqueo por permiso ausente");
        Equal(0, ports.Tareas.Calls, "lectura de tarea sin permiso");
        Equal(0, ports.Autorizacion.Calls, "lectura de autorización sin permiso");
        Equal(0, ports.Historial.Calls, "lectura de historial sin permiso");
        AssertNoPreviewWrite(ports, "permiso ausente");
    }

    private static void PreviewBlocksUnavailableTask()
    {
        var ports = new Ports
        {
            Tareas = new FakeTareas { Result = null }
        };
        var response = CreateService(ports).Previsualizar(Context(), new Workflow.SolicitudPreviewDevolverUsuarioAnterior { IdTarea = TaskId });

        Equal(Workflow.CodigosBloqueoDevolverUsuarioAnterior.TareaNoDisponible, response.Error.Codigo, "bloqueo por tarea no disponible");
        Equal(0, ports.Historial.Calls, "lectura de historial sin tarea");
        AssertNoPreviewWrite(ports, "tarea no disponible");
    }

    private static void PreviewBlocksGroupHistory()
    {
        var ports = new Ports
        {
            Historial = new FakeHistorial
            {
                Result = BlockedHistory(Workflow.CodigosBloqueoDevolverUsuarioAnterior.HistorialGrupo)
            }
        };
        var response = CreateService(ports).Previsualizar(Context(), new Workflow.SolicitudPreviewDevolverUsuarioAnterior { IdTarea = TaskId });

        Equal(Workflow.CodigosBloqueoDevolverUsuarioAnterior.HistorialGrupo, response.Error.Codigo, "bloqueo por historial de grupo");
        AssertNoPreviewWrite(ports, "historial de grupo");
    }

    private static void PreviewBlocksRetiredOrInconsistentHistory()
    {
        foreach (var description in new[] { "usuario retirado", "Ruta o Flujo inconsistente" })
        {
            var ports = new Ports
            {
                Historial = new FakeHistorial
                {
                    Result = BlockedHistory(Workflow.CodigosBloqueoDevolverUsuarioAnterior.DestinoNoDisponible)
                }
            };
            var response = CreateService(ports).Previsualizar(Context(), new Workflow.SolicitudPreviewDevolverUsuarioAnterior { IdTarea = TaskId });

            Equal(Workflow.CodigosBloqueoDevolverUsuarioAnterior.DestinoNoDisponible, response.Error.Codigo, "bloqueo por " + description);
            AssertNoPreviewWrite(ports, description);
        }
    }

    private static void PreviewBlocksSelfReturnUsingAuthenticatedWorkflowUser()
    {
        var ports = new Ports
        {
            Historial = new FakeHistorial
            {
                Result = ValidHistory(userId: 101)
            }
        };
        var context = Context();
        context.IdUsuarioWorkflow = 101;
        var response = CreateService(ports).Previsualizar(context, new Workflow.SolicitudPreviewDevolverUsuarioAnterior { IdTarea = TaskId });

        Equal(Workflow.CodigosBloqueoDevolverUsuarioAnterior.AutoDevolucion, response.Error.Codigo, "auto-devolución contra usuario Workflow autenticado");
        AssertNoPreviewWrite(ports, "auto-devolución");
    }

    private static void ExecutionBlocksChangedTokenBeforeMotor()
    {
        var ports = new Ports();
        ports.Token.IsValid = false;
        var response = CreateService(ports).Ejecutar(Context(), new Workflow.SolicitudEjecutarDevolverUsuarioAnterior
        {
            IdTarea = TaskId,
            TokenVersion = "token-historial-cambiado"
        });

        Equal(Workflow.CodigosBloqueoDevolverUsuarioAnterior.VersionConflicto, response.CodigoBloqueo, "bloqueo por token o historial cambiado");
        Equal(1, ports.Token.ValidateCalls, "revalidación de token");
        Equal(0, ports.Ejecutor.Calls, "motor con token o historial cambiado");
        Equal(1, ports.Auditoria.Calls, "auditoría de bloqueo de token");
    }

    private static void RepositoryPreservesDistinctHistoricalFlowUser()
    {
        var repository = new Workflow.MySqlDevolverUsuarioAnteriorRepository(new NullConnectionFactory(), new HistoricalFlowDataExecutor());
        var task = ActiveTask();
        task.IdFlujoTrabajo = 7;
        task.IdActividadFlujoActual = 8;

        var history = repository.ObtenerAntecedente(Context(), task);

        Assert(history.EsValido, "Un usuario de flujo histórico distinto no debe invalidar el antecedente.");
        Equal(101, history.UsuarioHistorico.IdUsuarioWorkflow, "usuario destino histórico");
        Equal(555, history.UsuarioHistorico.IdUsuarioWorkflowFlujoTrabajo, "usuario de flujo histórico preservado");
    }

    private static void ExecutionSerializesDistinctTokensBeforeSecondMotorCall()
    {
        var ports = new Ports();
        ports.Guard.Results.Enqueue(AcquiredGuard());
        ports.Guard.Results.Enqueue(BlockedGuard());
        var service = CreateService(ports);

        var first = service.Ejecutar(Context(), new Workflow.SolicitudEjecutarDevolverUsuarioAnterior { IdTarea = TaskId, TokenVersion = "token-A" });
        var second = service.Ejecutar(Context(), new Workflow.SolicitudEjecutarDevolverUsuarioAnterior { IdTarea = TaskId, TokenVersion = "token-B" });

        Assert(first.Exito, "La primera ejecución protegida debe alcanzar el motor.");
        Equal(Workflow.CodigosBloqueoDevolverUsuarioAnterior.EnProgreso, second.CodigoBloqueo, "bloqueo de segundo token");
        Equal(2, ports.Guard.Calls, "adquisiciones por tarea");
        Equal(1, ports.Ejecutor.Calls, "una sola mutación con tokens distintos");
        Equal(1, ports.Token.ValidateCalls, "el segundo token bloquea antes de revalidarse en motor");
        Equal(2, ports.Auditoria.Calls, "auditorías de ambas solicitudes");
    }

    private static void AdapterUsesApprovedNoResponseParameters()
    {
        var executor = new CapturingExecutor();
        var success = executor.Ejecutar(Context(), ActiveTask(), ValidHistory().UsuarioHistorico);

        Assert(success.Exito, "El adaptador debe normalizar el éxito del motor.");
        Equal(1, executor.Calls, "llamada única al motor");
        Assert(executor.PageWasNull, "El adaptador debe pasar Page = Nothing.");
        Equal(0, executor.Notifica, "notifica");
        Equal(0, executor.NotificaEnvioCorreo, "notifica_envio_correo");
        Equal(0, executor.IdConector, "id_conector");
        Equal(0, executor.ActualizaInterfazLegacy, "actualización legacy");
        Equal(0, executor.ActivaEventosDinamicos, "eventos dinámicos");
        Equal(0, executor.ActivaReasignaSii, "reasignación SII");
        Equal(0, executor.ActivaReasignaTareaWorkflow, "reasignación Workflow");

        var rejectedExecutor = new CapturingExecutor { LegacyResult = "NO" };
        var rejected = rejectedExecutor.Ejecutar(Context(), ActiveTask(), ValidHistory().UsuarioHistorico);
        Equal(Workflow.CodigosBloqueoDevolverUsuarioAnterior.Rechazada, rejected.CodigoBloqueo, "rechazo del motor");
        Equal(1, rejectedExecutor.Calls, "una llamada al motor rechazado");
    }

    private static void ExecutionKeepsSuccessWhenAuditFails()
    {
        var ports = new Ports();
        ports.Auditoria.RegistrationResult = false;
        var response = CreateService(ports).Ejecutar(Context(), new Workflow.SolicitudEjecutarDevolverUsuarioAnterior
        {
            IdTarea = TaskId,
            TokenVersion = "token-auditoria"
        });

        Assert(response.Exito, "La falla posterior de auditoría no debe revertir el éxito.");
        Equal(1, ports.Ejecutor.Calls, "mutación previa a auditoría");
        Equal(1, ports.Auditoria.Calls, "intento de auditoría");
        Equal(1, response.Advertencias.Count, "advertencia saneada de auditoría");
    }

    private static Workflow.ServicioDevolverUsuarioAnterior CreateService(Ports ports)
    {
        return new Workflow.ServicioDevolverUsuarioAnterior(
            ports.Tareas,
            ports.Autorizacion,
            ports.Historial,
            ports.Token,
            ports.Guard,
            ports.Ejecutor,
            ports.Auditoria);
    }

    private static Workflow.ContextoModuloWorkflow Context()
    {
        return new Workflow.ContextoModuloWorkflow
        {
            IdUsuarioWorkflow = 100,
            IdGrupoWorkflow = 10,
            IdRutaWorkflow = 20,
            LoginUsuario = "usuario-prueba",
            PuedeDevolverUsuarioAnterior = true
        };
    }

    private static Workflow.TareaDevolverUsuarioAnterior ActiveTask()
    {
        return new Workflow.TareaDevolverUsuarioAnterior
        {
            IdTarea = TaskId,
            IdEstadoActual = 90,
            IdRuta = 20,
            IdActividadActual = 30,
            IdFlujoTrabajo = 0,
            IdActividadFlujoActual = 0,
            EstaActiva = true
        };
    }

    private static Workflow.ResultadoHistorialDevolverUsuarioAnterior ValidHistory(int userId = 101)
    {
        return new Workflow.ResultadoHistorialDevolverUsuarioAnterior
        {
            UsuarioHistorico = new Workflow.UsuarioHistoricoDevolverUsuarioAnterior
            {
                IdEstadoHistorico = 89,
                IdUsuarioWorkflow = userId,
                IdActividad = 29,
                IdRuta = 20,
                IdFlujoTrabajo = 0,
                IdActividadFlujoTrabajo = 0,
                IdUsuarioWorkflowFlujoTrabajo = 0,
                NombreActividad = "Auxiliar contable",
                NombreUsuario = "Usuario histórico"
            }
        };
    }

    private static Workflow.ResultadoHistorialDevolverUsuarioAnterior BlockedHistory(string code)
    {
        return new Workflow.ResultadoHistorialDevolverUsuarioAnterior
        {
            CodigoBloqueo = code,
            MensajeFuncional = "Bloqueo controlado de prueba"
        };
    }

    private static Workflow.ResultadoGuardDevolverUsuarioAnterior AcquiredGuard()
    {
        return new Workflow.ResultadoGuardDevolverUsuarioAnterior { Adquirido = true, Lease = new FakeLease() };
    }

    private static Workflow.ResultadoGuardDevolverUsuarioAnterior BlockedGuard()
    {
        return new Workflow.ResultadoGuardDevolverUsuarioAnterior
        {
            Adquirido = false,
            CodigoBloqueo = Workflow.CodigosBloqueoDevolverUsuarioAnterior.EnProgreso,
            MensajeFuncional = "La tarea ya está en proceso."
        };
    }

    private static void AssertNoPreviewWrite(Ports ports, string scenario)
    {
        Equal(0, ports.Token.EmitCalls, "emisión de token en " + scenario);
        Equal(0, ports.Guard.Calls, "lock en " + scenario);
        Equal(0, ports.Ejecutor.Calls, "mutación en " + scenario);
        Equal(0, ports.Auditoria.Calls, "auditoría en " + scenario);
    }

    private static void Equal<T>(T expected, T actual, string label)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new InvalidOperationException("Fallo en " + label + ": se esperaba '" + expected + "' y se recibió '" + actual + "'.");
        }
    }

    private static void Assert(bool condition, string label)
    {
        if (!condition) throw new InvalidOperationException(label);
    }

    private sealed class Ports
    {
        internal FakeTareas Tareas = new FakeTareas();
        internal FakeAutorizacion Autorizacion = new FakeAutorizacion();
        internal FakeHistorial Historial = new FakeHistorial();
        internal FakeToken Token = new FakeToken();
        internal FakeGuard Guard = new FakeGuard();
        internal FakeEjecutor Ejecutor = new FakeEjecutor();
        internal FakeAuditoria Auditoria = new FakeAuditoria();
    }

    private sealed class FakeTareas : Workflow.IDevolverUsuarioAnteriorTareaRepository
    {
        internal int Calls;
        internal Workflow.TareaDevolverUsuarioAnterior Result = ActiveTask();

        public Workflow.TareaDevolverUsuarioAnterior ObtenerTarea(Workflow.ContextoModuloWorkflow contexto, long idTarea)
        {
            Calls++;
            return Result;
        }
    }

    private sealed class FakeAutorizacion : Workflow.IDevolverUsuarioAnteriorAutorizacionRepository
    {
        internal int Calls;

        public Workflow.ResultadoAutorizacionDevolverUsuarioAnterior Evaluar(Workflow.ContextoModuloWorkflow contexto, Workflow.TareaDevolverUsuarioAnterior tarea)
        {
            Calls++;
            return new Workflow.ResultadoAutorizacionDevolverUsuarioAnterior { Autorizado = true };
        }
    }

    private sealed class FakeHistorial : Workflow.IDevolverUsuarioAnteriorHistorialRepository
    {
        internal int Calls;
        internal Workflow.ResultadoHistorialDevolverUsuarioAnterior Result = ValidHistory();

        public Workflow.ResultadoHistorialDevolverUsuarioAnterior ObtenerAntecedente(Workflow.ContextoModuloWorkflow contexto, Workflow.TareaDevolverUsuarioAnterior tarea)
        {
            Calls++;
            return Result;
        }
    }

    private sealed class FakeToken : Workflow.IDevolverUsuarioAnteriorTokenCodec
    {
        internal int EmitCalls;
        internal int ValidateCalls;
        internal bool IsValid = true;

        public string Emitir(Workflow.ContextoModuloWorkflow contexto, Workflow.TareaDevolverUsuarioAnterior tarea, Workflow.UsuarioHistoricoDevolverUsuarioAnterior usuarioHistorico)
        {
            EmitCalls++;
            return "opaque-preview-token";
        }

        public bool Validar(Workflow.ContextoModuloWorkflow contexto, Workflow.TareaDevolverUsuarioAnterior tarea, Workflow.UsuarioHistoricoDevolverUsuarioAnterior usuarioHistorico, string tokenVersion)
        {
            ValidateCalls++;
            return IsValid;
        }
    }

    private sealed class FakeGuard : Workflow.IDevolverUsuarioAnteriorConcurrencyGuard
    {
        internal int Calls;
        internal readonly Queue<Workflow.ResultadoGuardDevolverUsuarioAnterior> Results = new Queue<Workflow.ResultadoGuardDevolverUsuarioAnterior>();

        public Workflow.ResultadoGuardDevolverUsuarioAnterior Adquirir(Workflow.ContextoModuloWorkflow contexto, long idTarea)
        {
            Calls++;
            return Results.Count > 0 ? Results.Dequeue() : AcquiredGuard();
        }
    }

    private sealed class FakeLease : Workflow.IDevolverUsuarioAnteriorConcurrencyLease
    {
        public void Dispose()
        {
        }
    }

    private sealed class FakeEjecutor : Workflow.IDevolverUsuarioAnteriorLegacyExecutor
    {
        internal int Calls;

        public Workflow.ResultadoEjecucionDevolverUsuarioAnterior Ejecutar(Workflow.ContextoModuloWorkflow contexto, Workflow.TareaDevolverUsuarioAnterior tarea, Workflow.UsuarioHistoricoDevolverUsuarioAnterior usuarioHistorico)
        {
            Calls++;
            return new Workflow.ResultadoEjecucionDevolverUsuarioAnterior { Exito = true, EstadoFinal = "completada" };
        }
    }

    private sealed class CapturingExecutor : Workflow.WorkflowLegacyDevolverUsuarioAnteriorExecutorAdapter
    {
        internal int Calls;
        internal bool PageWasNull;
        internal int Notifica;
        internal int NotificaEnvioCorreo;
        internal int IdConector;
        internal int ActualizaInterfazLegacy;
        internal int ActivaEventosDinamicos;
        internal int ActivaReasignaSii;
        internal int ActivaReasignaTareaWorkflow;
        internal string LegacyResult = "YES";

        protected override string TerminarTareaWorkflow(
            string idUsuarioDestino,
            string idActividadDestino,
            long idTarea,
            string nombreActividad,
            ref System.Web.UI.Page pagina,
            ref string resultadoEvento,
            int notifica,
            ref string resultadoCorreo,
            int idFlujoTrabajo,
            int idActividadFlujoTrabajo,
            int idUsuarioWorkflowFlujoTrabajo,
            int notificaEnvioCorreo,
            int idConector,
            int idUsuarioWorkflowEnvia,
            int idActividadWorkflowEnvia,
            int actualizaInterfazLegacy,
            int activaEventosDinamicos,
            int activaReasignaSii,
            int activaReasignaTareaWorkflow)
        {
            Calls++;
            PageWasNull = pagina == null;
            Notifica = notifica;
            NotificaEnvioCorreo = notificaEnvioCorreo;
            IdConector = idConector;
            ActualizaInterfazLegacy = actualizaInterfazLegacy;
            ActivaEventosDinamicos = activaEventosDinamicos;
            ActivaReasignaSii = activaReasignaSii;
            ActivaReasignaTareaWorkflow = activaReasignaTareaWorkflow;
            return LegacyResult;
        }
    }

    private sealed class FakeAuditoria : Workflow.IDevolverUsuarioAnteriorAuditoriaRepository
    {
        internal int Calls;
        internal bool RegistrationResult = true;

        public bool Registrar(Workflow.AuditoriaDevolverUsuarioAnterior auditoria)
        {
            Calls++;
            return RegistrationResult;
        }
    }

    private sealed class NullConnectionFactory : Workflow.IModuleConnectionFactory
    {
        public IDbConnection CreateOpenConnection(Workflow.ContextoModulo contexto)
        {
            return null;
        }
    }

    private sealed class HistoricalFlowDataExecutor : Workflow.IDataExecutor
    {
        public int ExecuteNonQuery(IDbConnection connection, IDbTransaction transaction, string commandText, IEnumerable<IDataParameter> parameters)
        {
            throw new NotSupportedException();
        }

        public object ExecuteScalar(IDbConnection connection, IDbTransaction transaction, string commandText, IEnumerable<IDataParameter> parameters)
        {
            throw new NotSupportedException();
        }

        public T ExecuteReader<T>(IDbConnection connection, IDbTransaction transaction, string commandText, IEnumerable<IDataParameter> parameters, Func<IDataReader, T> projector)
        {
            var table = commandText.IndexOf("FROM estados_tarea_workflow", StringComparison.OrdinalIgnoreCase) >= 0
                ? HistoricalStates()
                : HistoricalUser();
            using (var reader = table.CreateDataReader())
            {
                return projector(reader);
            }
        }

        private static DataTable HistoricalStates()
        {
            var table = new DataTable();
            table.Columns.Add("id_Estado", typeof(long));
            table.Columns.Add("Inicio_Tareas_Workflow_id_Tarea", typeof(long));
            table.Columns.Add("Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta", typeof(int));
            table.Columns.Add("Id_Actividad", typeof(int));
            table.Columns.Add("Id_Usuario", typeof(int));
            table.Columns.Add("ID_FLUJO_TRABAJO", typeof(int));
            table.Columns.Add("ID_ACTIVIDAD_FLUJO_TRABAJO", typeof(int));
            table.Columns.Add("ID_USUARIO_WORKFLOW_FLUJO_TRABAJO", typeof(int));
            table.Rows.Add(89L, TaskId, 20, 29, 101, 7, 8, 555);
            return table;
        }

        private static DataTable HistoricalUser()
        {
            var table = new DataTable();
            table.Columns.Add("ID_USUARIO", typeof(int));
            table.Columns.Add("ID_ACTIVIDAD", typeof(int));
            table.Columns.Add("NOMBRE_USUARIO", typeof(string));
            table.Columns.Add("NOMBRE_ACTIVIDAD", typeof(string));
            table.Rows.Add(101, 29, "Usuario histórico", "Auxiliar contable");
            return table;
        }
    }
}
