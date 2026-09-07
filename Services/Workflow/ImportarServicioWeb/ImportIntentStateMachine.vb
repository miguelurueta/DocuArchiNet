Imports System
Imports System.Collections.Generic

Public NotInheritable Class ImportIntentStateMachine
    Private Shared ReadOnly Permitidas As IDictionary(Of FaseImportacionServicio, ISet(Of FaseImportacionServicio)) = CrearTabla()
    Private ReadOnly _clock As IImportacionServicioClock
    Private ReadOnly _audit As IImportIntentTransitionAudit

    Public Sub New(ByVal clock As IImportacionServicioClock, ByVal audit As IImportIntentTransitionAudit)
        If clock Is Nothing OrElse audit Is Nothing Then Throw New ArgumentNullException("dependency")
        _clock = clock : _audit = audit
    End Sub

    Public Function Intentar(ByVal intentId As String, ByVal item As ResultadoElementoImportacion,
                             ByVal destino As FaseImportacionServicio, ByVal versionAnterior As String,
                             ByVal versionNueva As String, ByVal correlationId As String) As ResultadoTransicionImportacion
        If item Is Nothing OrElse Not EsPermitida(item.Fase, destino) Then
            _audit.Registrar(Nothing, False, "INVALID_STATE_TRANSITION")
            Return New ResultadoTransicionImportacion With {.Codigo = "INVALID_STATE_TRANSITION", .MensajeVisible = "La transición solicitada no es válida."}
        End If
        Dim cambio = New TransicionImportacion With {.IntentId = intentId, .ClientItemId = item.ClientItemId,
            .FaseAnterior = item.Fase, .FaseNueva = destino, .VersionAnterior = versionAnterior,
            .VersionNueva = versionNueva, .FechaUtc = _clock.UtcNow(), .CorrelationId = correlationId}
        _audit.Registrar(cambio, True, String.Empty)
        Return New ResultadoTransicionImportacion With {.Aceptada = True, .Transicion = cambio}
    End Function

    Public Shared Function EsPermitida(ByVal origen As FaseImportacionServicio, ByVal destino As FaseImportacionServicio) As Boolean
        Return Permitidas.ContainsKey(origen) AndAlso Permitidas(origen).Contains(destino)
    End Function

    Private Shared Function CrearTabla() As IDictionary(Of FaseImportacionServicio, ISet(Of FaseImportacionServicio))
        Dim t = New Dictionary(Of FaseImportacionServicio, ISet(Of FaseImportacionServicio))()
        Agregar(t, FaseImportacionServicio.Creada, FaseImportacionServicio.Validada, FaseImportacionServicio.FallidaAntesDePersistir, FaseImportacionServicio.Detenida)
        Agregar(t, FaseImportacionServicio.Validada, FaseImportacionServicio.RecursoObtenido, FaseImportacionServicio.FallidaAntesDePersistir, FaseImportacionServicio.Detenida)
        Agregar(t, FaseImportacionServicio.RecursoObtenido, FaseImportacionServicio.ExpedientePreparado, FaseImportacionServicio.ResultadoIncierto, FaseImportacionServicio.Detenida)
        Agregar(t, FaseImportacionServicio.ExpedientePreparado, FaseImportacionServicio.IndicesActualizados, FaseImportacionServicio.ResultadoIncierto)
        Agregar(t, FaseImportacionServicio.IndicesActualizados, FaseImportacionServicio.DocumentoAlmacenado, FaseImportacionServicio.ResultadoIncierto)
        Agregar(t, FaseImportacionServicio.DocumentoAlmacenado, FaseImportacionServicio.CacheActualizado, FaseImportacionServicio.ResultadoIncierto)
        Agregar(t, FaseImportacionServicio.CacheActualizado, FaseImportacionServicio.Completada, FaseImportacionServicio.Parcial)
        Agregar(t, FaseImportacionServicio.ResultadoIncierto, FaseImportacionServicio.RequiereDecision, FaseImportacionServicio.Reconciliada)
        Agregar(t, FaseImportacionServicio.RequiereDecision, FaseImportacionServicio.Reconciliada)
        Agregar(t, FaseImportacionServicio.Reconciliada, FaseImportacionServicio.Completada, FaseImportacionServicio.Parcial)
        Return t
    End Function

    Private Shared Sub Agregar(ByVal tabla As IDictionary(Of FaseImportacionServicio, ISet(Of FaseImportacionServicio)),
                               ByVal origen As FaseImportacionServicio, ParamArray ByVal destinos() As FaseImportacionServicio)
        tabla(origen) = New HashSet(Of FaseImportacionServicio)(destinos)
    End Sub
End Class
