Imports System

' Resuelve expedientes antes del almacenamiento y produce el plan lógico completo de la intención.
Public NotInheritable Class ImportExpedientCoordinator
    Private ReadOnly _configurations As IImportExpedientConfigurationRepository
    Private ReadOnly _subjects As ISiiExpedientSubjectResolver
    Private ReadOnly _expedients As IImportExpedientRepository
    Private ReadOnly _cache As IImportExpedientCacheRepository
    Private ReadOnly _planBuilder As ImportExpedientPlan

    Public Sub New(ByVal configurations As IImportExpedientConfigurationRepository,
                   ByVal subjects As ISiiExpedientSubjectResolver,
                   ByVal expedients As IImportExpedientRepository,
                   ByVal cache As IImportExpedientCacheRepository,
                   ByVal planBuilder As ImportExpedientPlan)
        If configurations Is Nothing OrElse subjects Is Nothing OrElse expedients Is Nothing OrElse
           cache Is Nothing OrElse planBuilder Is Nothing Then Throw New ArgumentNullException("dependency")
        _configurations = configurations : _subjects = subjects : _expedients = expedients
        _cache = cache : _planBuilder = planBuilder
    End Sub

    Public Function Resolver(ByVal contexto As ContextoImportacionServicio,
                             ByVal intencion As IntencionImportacionServicio) As PlanExpedienteImportacion
        If contexto Is Nothing OrElse intencion Is Nothing OrElse intencion.Inscripciones Is Nothing OrElse
           intencion.Inscripciones.Count = 0 Then Return EmptyPlan(intencion, "EXPEDIENT_PLAN_INPUT_INVALID")
        Dim configuration = _configurations.Obtener(contexto)
        If configuration Is Nothing Then Return EmptyPlan(intencion, "EXPEDIENT_CONFIGURATION_UNAVAILABLE")
        If Not configuration.ExpedienteObligatorio Then Return EmptyPlan(intencion, "EXPEDIENT_CREATION_DISABLED")

        Dim ordinal As Integer = 0
        For Each inscription In intencion.Inscripciones
            ordinal += 1
            inscription.Orden = ordinal
            inscription.NombreGabinete = configuration.NombreGabinete
            inscription.RadicadoSii = If(intencion.ContextoOriginal Is Nothing, String.Empty, intencion.ContextoOriginal.Radicado)
            inscription.RolExpediente = Role(configuration, ordinal)
            Dim subject = _subjects.Resolver(contexto, inscription, configuration)
            If subject Is Nothing OrElse subject.Estado <> EstadoEfectoExpedienteImportacion.Confirmado Then Return EmptyPlan(intencion, If(subject Is Nothing, "SII_SUBJECT_UNAVAILABLE", subject.Codigo))

            Dim resolved = ResolveOne(contexto, inscription, configuration)
            If resolved Is Nothing OrElse resolved.Estado <> EstadoEfectoExpedienteImportacion.Confirmado OrElse Not resolved.IdExpediente.HasValue Then Return EmptyPlan(intencion, If(resolved Is Nothing, "EXPEDIENT_DESTINATION_UNRESOLVED", resolved.Codigo))
            inscription.IdExpediente = resolved.IdExpediente
            inscription.EstadoExpediente = EstadoEfectoExpedienteImportacion.Confirmado

            Dim cached = _cache.RegistrarVerificado(contexto, inscription)
            If cached Is Nothing OrElse cached.Estado <> EstadoEfectoExpedienteImportacion.Confirmado Then Return EmptyPlan(intencion, If(cached Is Nothing, "EXPEDIENT_CACHE_UNAVAILABLE", cached.Codigo))
            inscription.EstadoCache = EstadoEfectoExpedienteImportacion.Confirmado
        Next
        Return _planBuilder.Construir(intencion, intencion.Inscripciones)
    End Function

    Private Function ResolveOne(ByVal contexto As ContextoImportacionServicio,
                                ByVal inscription As InscripcionImportacion,
                                ByVal configuration As ConfiguracionExpedienteImportacion) As ResultadoEfectoExpedienteImportacion
        Dim cached = _cache.Obtener(contexto, inscription)
        If cached IsNot Nothing AndAlso cached.EstadoCache = EstadoEfectoExpedienteImportacion.Conflicto Then
            Return New ResultadoEfectoExpedienteImportacion With {.Estado = EstadoEfectoExpedienteImportacion.Conflicto, .Codigo = "EXPEDIENT_CACHE_CONFLICT"}
        End If
        If cached IsNot Nothing AndAlso cached.EstadoCache = EstadoEfectoExpedienteImportacion.ResultadoIncierto Then
            Return New ResultadoEfectoExpedienteImportacion With {.Estado = EstadoEfectoExpedienteImportacion.ResultadoIncierto, .Codigo = "EXPEDIENT_CACHE_READ_FAILED", .Reintentable = True}
        End If
        If cached IsNot Nothing AndAlso cached.IdExpediente.HasValue Then
            Dim verified = _expedients.Verificar(contexto, cached.IdExpediente.Value, configuration)
            If verified.Estado = EstadoEfectoExpedienteImportacion.Confirmado Then Return verified
            If verified.Estado = EstadoEfectoExpedienteImportacion.Conflicto Then Return verified
        End If
        Dim found = _expedients.Buscar(contexto, inscription, configuration)
        If found.Estado = EstadoEfectoExpedienteImportacion.Confirmado OrElse
           found.Estado = EstadoEfectoExpedienteImportacion.Conflicto OrElse
           found.Estado = EstadoEfectoExpedienteImportacion.ResultadoIncierto Then Return found
        If found.Estado <> EstadoEfectoExpedienteImportacion.Ausente Then Return found
        Return _expedients.Crear(contexto, inscription, configuration)
    End Function

    Private Shared Function Role(ByVal configuration As ConfiguracionExpedienteImportacion,
                                 ByVal ordinal As Integer) As RolExpedienteImportacion
        If Not configuration.MultiplesExpedientes Then Return RolExpedienteImportacion.Unico
        Return If(ordinal = 1, RolExpedienteImportacion.Primario, RolExpedienteImportacion.Secundario)
    End Function

    Private Shared Function EmptyPlan(ByVal intent As IntencionImportacionServicio, ByVal code As String) As PlanExpedienteImportacion
        Return New PlanExpedienteImportacion With {
            .IntentId = If(intent Is Nothing, Nothing, intent.Id),
            .Estado = EstadoEfectoExpedienteImportacion.Fallido,
            .Codigo = If(code, "EXPEDIENT_DESTINATION_UNRESOLVED")
        }
    End Function
End Class
