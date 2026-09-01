using System;
using System.Collections.Generic;
using Workflow = GestionDocumental_Docuarchi.net;

// Arnés ejecutable contra el ensamblado VB compilado; todos los puertos son fakes y no abre una BD.
internal static class WorkflowNotesReadBehaviorTests
{
    private const long TaskId = 843;

    private static int Main()
    {
        try
        {
            ListBlocksMissingPermissionBeforeReads();
            ListRejectsCrossContextCursorBeforeRepository();
            ListNormalizesDefaultPageAndProtectsContinuation();
            ContentDoesNotExposeForeignNote();
            CreateRejectsInvalidClientRequestIdBeforeRepository();
            CreateRejectsInvalidContentBeforeRepository();
            CreateRejectsInactiveTaskBeforeRepository();
            Console.WriteLine("workflow-notes-read behavior tests: passed");
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error.Message);
            return 1;
        }
    }

    private static void ListBlocksMissingPermissionBeforeReads()
    {
        var ports = new Ports();
        var context = Context();
        context.PuedeInteractuarAnotaciones = false;
        var response = Service(ports).Listar(context, new Workflow.SolicitudListarNotasWorkflow { IdTarea = TaskId, TamanoPagina = 1 });

        Equal(Workflow.CodigosResultadoNotasWorkflow.Forbidden, response.Codigo, "bloqueo de permiso");
        Equal(0, ports.Tasks.Calls, "lectura de tarea sin permiso");
        Equal(0, ports.Notes.ListarCalls, "lectura de notas sin permiso");
        AssertNoWrites(ports, "permiso ausente");
    }

    private static void ListRejectsCrossContextCursorBeforeRepository()
    {
        var ports = new Ports { Cursor = new FakeCursor { Valid = false } };
        var response = Service(ports).Listar(Context(), new Workflow.SolicitudListarNotasWorkflow
        {
            IdTarea = TaskId,
            Cursor = "cursor-de-otro-contexto",
            TamanoPagina = 1
        });

        Equal(Workflow.CodigosResultadoNotasWorkflow.NoteNotFound, response.Codigo, "cursor cruzado");
        Equal(1, ports.Tasks.Calls, "validación de tarea antes del cursor");
        Equal(0, ports.Notes.ListarCalls, "repositorio con cursor cruzado");
        AssertNoWrites(ports, "cursor cruzado");
    }

    private static void ListNormalizesDefaultPageAndProtectsContinuation()
    {
        var ports = new Ports();
        ports.Notes.ListarResult = new Workflow.ResultadoNotasWorkflow
        {
            TieneMas = true,
            Notas = new List<Workflow.NotaWorkflow>
            {
                new Workflow.NotaWorkflow { IdNota = 17, IdTarea = TaskId, FechaCreacionUtc = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc) }
            }
        };
        var response = Service(ports).Listar(Context(), new Workflow.SolicitudListarNotasWorkflow { IdTarea = TaskId });

        Equal(25, ports.Notes.LastListar.TamanoPagina, "tamaño por defecto");
        Equal("cursor-protegido", response.CursorSiguiente, "cursor de continuación");
        Equal(1, ports.Cursor.ProtectCalls, "protección de cursor");
        AssertNoWrites(ports, "listado autorizado");
    }

    private static void ContentDoesNotExposeForeignNote()
    {
        var ports = new Ports();
        ports.Notes.ConsultarResult = new Workflow.ResultadoNotasWorkflow
        {
            Codigo = Workflow.CodigosResultadoNotasWorkflow.NoteNotFound,
            MensajeFuncional = "La nota solicitada no está disponible."
        };
        var response = Service(ports).Consultar(Context(), new Workflow.SolicitudConsultarNotaWorkflow { IdTarea = TaskId, IdNota = 99 });

        Equal(Workflow.CodigosResultadoNotasWorkflow.NoteNotFound, response.Codigo, "nota ajena");
        Assert(response.Nota == null, "la nota ajena no debe exponerse");
        AssertNoWrites(ports, "contenido cruzado");
    }

    private static void CreateRejectsInvalidClientRequestIdBeforeRepository()
    {
        var ports = new Ports();
        var response = Service(ports).Crear(Context(), new Workflow.SolicitudCrearNotaWorkflow
        {
            IdTarea = TaskId,
            Contenido = "nota de prueba",
            IdSolicitudCliente = "no-es-un-uuid"
        });

        Equal(Workflow.CodigosResultadoNotasWorkflow.InvalidContent, response.Codigo, "UUID inválido");
        Equal(0, ports.Tasks.Calls, "lectura de tarea con UUID inválido");
        AssertNoWrites(ports, "UUID inválido");
    }

    private static void CreateRejectsInvalidContentBeforeRepository()
    {
        var ports = new Ports();
        var response = Service(ports).Crear(Context(), new Workflow.SolicitudCrearNotaWorkflow
        {
            IdTarea = TaskId,
            Contenido = "\0contenido",
            IdSolicitudCliente = Guid.NewGuid().ToString()
        });

        Equal(Workflow.CodigosResultadoNotasWorkflow.InvalidContent, response.Codigo, "contenido inválido");
        Equal(0, ports.Tasks.Calls, "lectura de tarea con contenido inválido");
        AssertNoWrites(ports, "contenido inválido");
    }

    private static void CreateRejectsInactiveTaskBeforeRepository()
    {
        var ports = new Ports();
        ports.Tasks.Active = false;
        var response = Service(ports).Crear(Context(), new Workflow.SolicitudCrearNotaWorkflow
        {
            IdTarea = TaskId,
            Contenido = "nota",
            IdSolicitudCliente = Guid.NewGuid().ToString()
        });

        Equal(Workflow.CodigosResultadoNotasWorkflow.TaskNotActive, response.Codigo, "tarea inactiva");
        Equal(0, ports.Notes.CreateCalls, "creación con tarea inactiva");
    }

    private static Workflow.ServicioNotasWorkflow Service(Ports ports) =>
        new Workflow.ServicioNotasWorkflow(ports.Tasks, ports.Notes, ports.Cursor);

    private static Workflow.ContextoModuloWorkflow Context()
    {
        return new Workflow.ContextoModuloWorkflow
        {
            IdUsuarioWorkflow = 101,
            IdGrupoWorkflow = 202,
            IdRutaWorkflow = 303,
            LoginUsuario = "usuario-prueba",
            PuedeInteractuarAnotaciones = true
        };
    }

    private static void AssertNoWrites(Ports ports, string label)
    {
        Equal(0, ports.Notes.CreateCalls, $"creación en {label}");
        Equal(0, ports.Notes.UpdateCalls, $"actualización en {label}");
        Equal(0, ports.Notes.DeleteCalls, $"eliminación en {label}");
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

    private sealed class Ports
    {
        public FakeTasks Tasks = new FakeTasks();
        public FakeNotes Notes = new FakeNotes();
        public FakeCursor Cursor = new FakeCursor();
    }

    private sealed class FakeTasks : Workflow.ITareaWorkflowRepository
    {
        public int Calls;
        public bool Active = true;

        public Workflow.TareaWorkflow ObtenerTarea(Workflow.ContextoModuloWorkflow context, long idTarea)
        {
            Calls++;
            return new Workflow.TareaWorkflow
            {
                IdTarea = idTarea,
                IdRuta = context.IdRutaWorkflow,
                EstaActiva = Active,
                TokenVersion = "estado-1"
            };
        }
    }

    private sealed class FakeCursor : Workflow.INotasWorkflowCursorCodec
    {
        public bool Valid = true;
        public int ProtectCalls;

        public string Proteger(Workflow.ContextoModuloWorkflow context, Workflow.TareaWorkflow task, DateTime fecha, long idNota)
        {
            ProtectCalls++;
            return "cursor-protegido";
        }

        public bool Validar(Workflow.ContextoModuloWorkflow context, Workflow.TareaWorkflow task, string cursor, ref DateTime fecha, ref long idNota)
        {
            fecha = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            idNota = 17;
            return Valid;
        }
    }

    private sealed class FakeNotes : Workflow.INotasWorkflowRepository
    {
        public int ListarCalls;
        public int CreateCalls;
        public int UpdateCalls;
        public int DeleteCalls;
        public Workflow.SolicitudListarNotasWorkflow LastListar;
        public Workflow.ResultadoNotasWorkflow ListarResult = new Workflow.ResultadoNotasWorkflow();
        public Workflow.ResultadoNotasWorkflow ConsultarResult = new Workflow.ResultadoNotasWorkflow();

        public Workflow.ResultadoNotasWorkflow Listar(Workflow.ContextoModuloWorkflow context, Workflow.TareaWorkflow task, Workflow.SolicitudListarNotasWorkflow request)
        {
            ListarCalls++;
            LastListar = request;
            return ListarResult;
        }

        public Workflow.ResultadoNotasWorkflow Contar(Workflow.ContextoModuloWorkflow context, Workflow.TareaWorkflow task, Workflow.SolicitudContarNotasWorkflow request) => new Workflow.ResultadoNotasWorkflow();

        public Workflow.ResultadoNotasWorkflow Crear(Workflow.ContextoModuloWorkflow context, Workflow.TareaWorkflow task, Workflow.SolicitudCrearNotaWorkflow request)
        {
            CreateCalls++;
            return new Workflow.ResultadoNotasWorkflow();
        }

        public Workflow.ResultadoNotasWorkflow Consultar(Workflow.ContextoModuloWorkflow context, Workflow.TareaWorkflow task, Workflow.SolicitudConsultarNotaWorkflow request) => ConsultarResult;

        public Workflow.ResultadoNotasWorkflow Actualizar(Workflow.ContextoModuloWorkflow context, Workflow.TareaWorkflow task, Workflow.SolicitudActualizarNotaWorkflow request)
        {
            UpdateCalls++;
            return new Workflow.ResultadoNotasWorkflow();
        }

        public Workflow.ResultadoNotasWorkflow Eliminar(Workflow.ContextoModuloWorkflow context, Workflow.TareaWorkflow task, Workflow.SolicitudEliminarNotaWorkflow request)
        {
            DeleteCalls++;
            return new Workflow.ResultadoNotasWorkflow();
        }
    }
}
