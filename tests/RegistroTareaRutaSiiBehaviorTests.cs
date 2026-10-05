using System;
using Workflow = GestionDocumental_Docuarchi.net;

internal static class RegistroTareaRutaSiiBehaviorTests
{
    private static int Main()
    {
        try
        {
            RejectsInvalidBeforeDependencies();
            RejectsPermissionBeforeSii();
            RejectsMismatchedSiiReceipt();
            AcceptsOptionalEnrollmentAndSubtype();
            ReportsHistoricalReceiptWithoutDispatch();
            ReconcilesExistingEventWithoutReportingNewRegistration();
            KeepsExistingReceiptPendingWithoutReportingNewRegistration();
            ConfirmsSuccessfulDispatch();
            KeepsDurablePendingWhenDocuarchiFails();
            Console.WriteLine("doc87 registro ruta behavior tests: passed");
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error.Message); return 1; }
    }

    private static void RejectsInvalidBeforeDependencies()
    {
        var ports = new Ports();
        var result = ports.Service().Ejecutar(Context(), new Workflow.SolicitudRegistroTareaRutaSii { recibo = "S1", id_tramite = 1, id_actividad = 1 });
        Equal("INVALID_REQUEST", result.Codigo, "solicitud inválida");
        Equal(0, ports.Repository.PermissionCalls, "permiso no consultado");
    }
    private static void RejectsPermissionBeforeSii()
    {
        var ports = new Ports(); ports.Repository.Permission = false;
        var result = ports.Service().Ejecutar(Context(), Request());
        Equal("FORBIDDEN", result.Codigo, "sin permiso");
        Equal(0, ports.Sii.Calls, "SII no llamado");
    }
    private static void RejectsMismatchedSiiReceipt()
    {
        var ports = new Ports(); ports.Sii.Receipt = "S999999999";
        var result = ports.Service().Ejecutar(Context(), Request());
        Equal("SII_CONTEXT_MISMATCH", result.Codigo, "recibo SII cruzado");
        Equal(0, ports.Repository.ResolveCalls, "catálogo no consultado");
        Equal(0, ports.Repository.PersistCalls, "sin escritura");
    }
    private static void ConfirmsSuccessfulDispatch()
    {
        var ports = new Ports();
        var result = ports.Service().Ejecutar(Context(), Request());
        Equal("YES", result.Codigo, "registro confirmado");
        Equal(1, ports.Repository.PersistCalls, "una persistencia");
        Equal(1, ports.Repository.ConfirmCalls, "evento confirmado");
        Equal(0, ports.Repository.PendingCalls, "sin pendiente");
    }
    private static void ReportsHistoricalReceiptWithoutDispatch()
    {
        var ports = new Ports(); ports.Repository.PersistCode = "ALREADY_REGISTERED"; ports.Repository.IncludeEvent = false;
        var result = ports.Service().Ejecutar(Context(), Request());
        Equal("ALREADY_REGISTERED", result.Codigo, "recibo histórico detectado");
        Equal(0, ports.Relation.Calls, "duplicado histórico no despacha relación");
        Equal(0, ports.Repository.ConfirmCalls, "duplicado histórico no confirma evento inexistente");
    }
    private static void ReconcilesExistingEventWithoutReportingNewRegistration()
    {
        var ports = new Ports(); ports.Repository.PersistCode = "ALREADY_REGISTERED";
        var result = ports.Service().Ejecutar(Context(), Request());
        Equal("ALREADY_REGISTERED", result.Codigo, "reintento conserva semántica de duplicado");
        Equal(1, ports.Relation.Calls, "evento existente se reconcilia");
        Equal(1, ports.Repository.ConfirmCalls, "evento existente queda confirmado");
    }
    private static void KeepsExistingReceiptPendingWithoutReportingNewRegistration()
    {
        var ports = new Ports(); ports.Repository.PersistCode = "ALREADY_REGISTERED"; ports.Relation.Throw = true;
        var result = ports.Service().Ejecutar(Context(), Request());
        Equal("ALREADY_REGISTERED_RELATION_PENDING", result.Codigo, "duplicado con relación pendiente");
        Equal(1, ports.Repository.PendingCalls, "evento existente continúa reintentable");
    }
    private static void AcceptsOptionalEnrollmentAndSubtype()
    {
        var ports = new Ports(); ports.Sii.Enrollment = ""; ports.Sii.Subtype = ""; ports.Sii.ProcedureType = "INS";
        var result = ports.Service().Ejecutar(Context(), Request());
        Equal("YES", result.Codigo, "matrícula y subtipo opcionales");
        Equal(1, ports.Repository.ResolveCalls, "catálogo valida trámite seleccionado");
        Equal("INS", ports.Repository.LastSii.Recibo.tipotramite, "tipo de recibo autoritativo conservado");
        Equal(1, ports.Repository.PersistCalls, "registro permitido con opcionales vacíos");
    }
    private static void KeepsDurablePendingWhenDocuarchiFails()
    {
        var ports = new Ports(); ports.Relation.Throw = true;
        var result = ports.Service().Ejecutar(Context(), Request());
        Equal("REGISTERED_RELATION_PENDING", result.Codigo, "relación pendiente");
        Equal(1, ports.Repository.PersistCalls, "tarea persistida");
        Equal(1, ports.Repository.PendingCalls, "evento reintentable");
    }
    private static Workflow.ContextoModulo Context() => new Workflow.ContextoModulo { CodigoModulo = "TEST", IdUsuario = 7, LoginUsuario = "test" };
    private static Workflow.SolicitudRegistroTareaRutaSii Request() => new Workflow.SolicitudRegistroTareaRutaSii { recibo = "S002470047", id_tramite = 12, id_actividad = 40 };
    private static void Equal<T>(T expected, T actual, string label) { if (!object.Equals(expected, actual)) throw new InvalidOperationException($"{label}: esperado {expected}, recibido {actual}"); }

    private sealed class Ports
    {
        public FakeRepository Repository = new FakeRepository();
        public FakeRelation Relation = new FakeRelation();
        public FakeSii Sii = new FakeSii();
        public Workflow.ServicioRegistroTareaRutaSii Service() => new Workflow.ServicioRegistroTareaRutaSii(Repository, Relation, Sii);
    }
    private sealed class FakeRepository : Workflow.IRegistroTareaRutaSiiRepository
    {
        public bool Permission = true; public bool IncludeEvent = true; public string PersistCode = "REGISTERED"; public int PermissionCalls, ResolveCalls, PersistCalls, ConfirmCalls, PendingCalls;
        public Workflow.ConsultaAutoritativaRegistroRutaSii LastSii;
        public bool TienePermiso(Workflow.ContextoModulo c) { PermissionCalls++; return Permission; }
        public Workflow.DatosAutoritativosRegistroRutaSii ResolverContexto(Workflow.ContextoModulo c, Workflow.SolicitudRegistroTareaRutaSii r, Workflow.ConsultaAutoritativaRegistroRutaSii s) { ResolveCalls++; LastSii = s; return new Workflow.DatosAutoritativosRegistroRutaSii(); }
        public Workflow.ResultadoRegistroTareaRutaSii RegistrarTareaConOutbox(Workflow.ContextoModulo c, Workflow.DatosAutoritativosRegistroRutaSii d) { PersistCalls++; return new Workflow.ResultadoRegistroTareaRutaSii { Codigo = PersistCode, Evento = IncludeEvent ? new Workflow.EventoRelacionRutaSii { OperationId = Guid.NewGuid().ToString(), Recibo = "S002470047" } : null }; }
        public void ConfirmarEvento(Workflow.ContextoModulo c, string id, string status) { ConfirmCalls++; }
        public void MarcarEventoPendiente(Workflow.ContextoModulo c, string id, string code) { PendingCalls++; }
    }
    private sealed class FakeRelation : Workflow.IRelacionRutaSiiGateway
    {
        public bool Throw; public int Calls;
        public string Materializar(Workflow.ContextoModulo c, Workflow.EventoRelacionRutaSii e) { Calls++; if (Throw) throw new InvalidOperationException("simulada"); return "CONFIRMED"; }
    }
    private sealed class FakeSii : Workflow.IConsultaAutoritativaRutaSii
    {
        public int Calls; public string Receipt = "S002470047"; public string Enrollment = "407564"; public string Subtype = "INS"; public string ProcedureType = "INS";
        public bool Consultar(string receipt, ref Workflow.ConsultaAutoritativaRegistroRutaSii data) { Calls++; data = new Workflow.ConsultaAutoritativaRegistroRutaSii { Recibo = new Workflow.Class_parram_consultarRecibo { tipotramite = ProcedureType }, Radicado = new Workflow.Class_parram_consultarRadicado { recibo = Receipt, radicado = "18333837", matricula = Enrollment, nombre = "PRUEBA", subtipotramite = Subtype } }; return true; }
    }
}
