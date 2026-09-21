Imports System
Imports System.Threading

' Ruta primaria moderna para consultar el sujeto SII. La función legacy original
' permanece intacta y puede usarse como fallback de rollout explícitamente configurado.
Public NotInheritable Class ModernSiiExpedientSubjectResolver
    Implements ISiiExpedientSubjectResolver

    Private ReadOnly _provider As SiiImportProvider
    Private ReadOnly _normalizer As IImportExpedientIdentityNormalizer
    Private ReadOnly _legacyFallback As ISiiExpedientSubjectResolver
    Private ReadOnly _fallbackEnabled As Boolean

    Public Sub New(ByVal provider As SiiImportProvider,
                   ByVal normalizer As IImportExpedientIdentityNormalizer,
                   ByVal legacyFallback As ISiiExpedientSubjectResolver,
                   ByVal fallbackEnabled As Boolean)
        If provider Is Nothing Then Throw New ArgumentNullException("provider")
        If normalizer Is Nothing Then Throw New ArgumentNullException("normalizer")
        _provider = provider : _normalizer = normalizer
        _legacyFallback = legacyFallback : _fallbackEnabled = fallbackEnabled
    End Sub

    Public Function Resolver(ByVal contexto As ContextoImportacionServicio,
                             ByVal inscripcion As InscripcionImportacion,
                             ByVal configuracion As ConfiguracionExpedienteImportacion) As ResultadoEfectoExpedienteImportacion Implements ISiiExpedientSubjectResolver.Resolver
        If contexto Is Nothing OrElse inscripcion Is Nothing OrElse configuracion Is Nothing OrElse
           String.IsNullOrWhiteSpace(configuracion.NombreGabinete) Then Return Failure("SII_SUBJECT_INPUT_INVALID", False)
        Try
            Dim correlationId = "DOC67-SUBJECT-" & contexto.IdTarea.ToString(Globalization.CultureInfo.InvariantCulture)
            Dim subject = _provider.ResolveExpedientSubjectAsync(configuracion.NombreGabinete, inscripcion.Matricula,
                inscripcion.Proponente, correlationId, CancellationToken.None, contexto.IdTarea).ConfigureAwait(False).GetAwaiter().GetResult()
            If subject Is Nothing Then Return FallbackOrFailure(contexto, inscripcion, configuracion, "SII_SUBJECT_INVALID_RESPONSE", False)

            inscripcion.IdentificacionSujeto = MergeValue(inscripcion.IdentificacionSujeto, subject.Identificacion)
            inscripcion.RazonSocial = MergeValue(inscripcion.RazonSocial, subject.RazonSocial)
            inscripcion.Matricula = MergeValue(inscripcion.Matricula, subject.MatriculaCanonica)
            inscripcion.MatriculaPropietario = MergeValue(inscripcion.MatriculaPropietario, subject.MatriculaPropietario)
            inscripcion.IdentificacionPropietario = MergeValue(inscripcion.IdentificacionPropietario, subject.IdentificacionPropietario)
            inscripcion.NombrePropietario = MergeValue(inscripcion.NombrePropietario, subject.NombrePropietario)

            Dim normalized = _normalizer.Normalizar(contexto, configuracion.NombreGabinete, inscripcion.Matricula, inscripcion.Proponente)
            If normalized Is Nothing OrElse Not normalized.Valida Then Return Failure(If(normalized Is Nothing, "EXPEDIENT_IDENTITY_INVALID", normalized.Codigo), False)
            inscripcion.MatriculaNormalizada = normalized.ValorPersistencia
            If configuracion.Modo = ModoExpedienteImportacion.SinExpediente Then Return Confirmed()
            If String.IsNullOrWhiteSpace(inscripcion.IdentificacionSujeto) OrElse String.IsNullOrWhiteSpace(inscripcion.RazonSocial) Then
                Return FallbackOrFailure(contexto, inscripcion, configuracion, "SII_SUBJECT_INCOMPLETE", False)
            End If
            Dim identityResult = LegacySiiExpedientSubjectResolver.MaterializeIdentityFields(inscripcion, configuracion)
            If identityResult IsNot Nothing Then Return identityResult
            Return Confirmed()
        Catch ex As ExternalImportHttpException
            Dim code = If(ex.ErrorDetail Is Nothing, "SII_SUBJECT_TRANSPORT_FAILED", SafeCode(ex.ErrorDetail.Code))
            Return FallbackOrFailure(contexto, inscripcion, configuracion, code, code = "EXTERNAL_TIMEOUT" OrElse code = "EXTERNAL_UNAVAILABLE")
        Catch ex As InvalidOperationException
            Return FallbackOrFailure(contexto, inscripcion, configuracion, SafeCode(ex.Message), Retryable(ex.Message))
        Catch
            Return FallbackOrFailure(contexto, inscripcion, configuracion, "SII_SUBJECT_INVALID_RESPONSE", False)
        End Try
    End Function

    Private Shared Function MergeValue(ByVal destination As String, ByVal source As String) As String
        Dim value = If(source, String.Empty).Trim()
        Return If(String.IsNullOrWhiteSpace(value), destination, value)
    End Function

    Private Function FallbackOrFailure(ByVal contexto As ContextoImportacionServicio,
                                       ByVal inscripcion As InscripcionImportacion,
                                       ByVal configuracion As ConfiguracionExpedienteImportacion,
                                       ByVal code As String, ByVal retryable As Boolean) As ResultadoEfectoExpedienteImportacion
        If _fallbackEnabled AndAlso _legacyFallback IsNot Nothing Then
            Dim fallback = _legacyFallback.Resolver(contexto, inscripcion, configuracion)
            If fallback IsNot Nothing AndAlso fallback.Estado = EstadoEfectoExpedienteImportacion.Confirmado Then Return fallback
        End If
        Return Failure(code, retryable)
    End Function

    Private Shared Function SafeCode(ByVal value As String) As String
        Dim candidate = If(value, String.Empty).Trim().ToUpperInvariant()
        If Text.RegularExpressions.Regex.IsMatch(candidate, "^[A-Z][A-Z0-9_]{2,99}$") Then Return candidate
        Return "SII_SUBJECT_INVALID_RESPONSE"
    End Function

    Private Shared Function Retryable(ByVal code As String) As Boolean
        Dim safe = SafeCode(code)
        Return safe = "EXTERNAL_TIMEOUT" OrElse safe = "EXTERNAL_UNAVAILABLE" OrElse safe = "SII_SUBJECT_TOKEN_INVALID"
    End Function

    Private Shared Function Confirmed() As ResultadoEfectoExpedienteImportacion
        Return New ResultadoEfectoExpedienteImportacion With {.Estado=EstadoEfectoExpedienteImportacion.Confirmado,.Codigo="SII_SUBJECT_CONFIRMED"}
    End Function

    Private Shared Function Failure(ByVal code As String, ByVal retryable As Boolean) As ResultadoEfectoExpedienteImportacion
        Return New ResultadoEfectoExpedienteImportacion With {.Estado=EstadoEfectoExpedienteImportacion.Fallido,.Codigo=SafeCode(code),.Reintentable=retryable}
    End Function
End Class
