Imports System
Imports System.Text.RegularExpressions

Public NotInheritable Class NullExternalServiceAttemptRecorder
    Implements IExternalServiceAttemptRecorder
    Public Sub Registrar(ByVal intento As IntentoServicioExterno) Implements IExternalServiceAttemptRecorder.Registrar
    End Sub
End Class

Public NotInheritable Class ExternalServiceAttemptNormalizer
    Private Shared ReadOnly SafeCode As New Regex("^[A-Z][A-Z0-9_]{2,99}$", RegexOptions.CultureInvariant)

    Public Shared Function Create(ByVal providerId As String, ByVal operation As String, ByVal correlationId As String,
                                  ByVal startedUtc As DateTime, ByVal finishedUtc As DateTime, ByVal durationMs As Long,
                                  ByVal failure As Exception, Optional ByVal intentId As String = Nothing,
                                  Optional ByVal clientItemId As String = Nothing, Optional ByVal operationId As String = Nothing,
                                  Optional ByVal taskId As Nullable(Of Long) = Nothing, Optional ByVal radicado As String = Nothing,
                                  Optional ByVal codigoBarras As String = Nothing, Optional ByVal referenciaProveedor As String = Nothing) As IntentoServicioExterno
        Dim result As New IntentoServicioExterno With {.ProviderId=providerId,.Operacion=operation,.CorrelationId=Limit(correlationId,128),
            .IntentId=Limit(intentId,32),.ClientItemId=Limit(clientItemId,128),.OperationId=Limit(operationId,128),
            .TaskId=taskId,.Radicado=Limit(radicado,40),.CodigoBarras=Limit(codigoBarras,20),.ReferenciaProveedor=Limit(referenciaProveedor,120),
            .FechaInicioUtc=startedUtc,.FechaFinUtc=finishedUtc,.DuracionMs=Math.Max(0,durationMs),.Exitoso=(failure Is Nothing)}
        If failure Is Nothing Then Return result
        Dim external = TryCast(failure, ExternalImportHttpException)
        Dim code = If(external IsNot Nothing AndAlso external.ErrorDetail IsNot Nothing, external.ErrorDetail.Code, SafeFailureCode(failure))
        result.CodigoError = code : result.CodigoDependencia = code
        If external IsNot Nothing AndAlso external.ErrorDetail IsNot Nothing Then result.EstadoHttp = external.ErrorDetail.HttpStatus
        Select Case code
            Case "EXTERNAL_TIMEOUT" : result.CategoriaError="TIMEOUT" : result.Reintentable=True
            Case "EXTERNAL_UNAVAILABLE" : result.CategoriaError="NETWORK" : result.Reintentable=True
            Case "EXTERNAL_ACCESS_DENIED", "SII_TOKEN_INVALID_CREDENTIALS" : result.CategoriaError="AUTHENTICATION"
            Case "EXTERNAL_CANCELLED" : result.CategoriaError="CANCELLED"
            Case "SII_TOKEN_INVALID", "SII_QUERY_PROVIDER_ERROR", "SII_TOKEN_REJECTED" : result.CategoriaError="BUSINESS_REJECTION" : result.Reintentable=(code="SII_TOKEN_INVALID")
            Case "SII_ITEM_NOT_FOUND" : result.CategoriaError="BUSINESS_REJECTION"
            Case Else : result.CategoriaError="INVALID_RESPONSE"
        End Select
        result.MensajeDiagnostico = "Fallo normalizado del proveedor externo."
        Return result
    End Function

    Private Shared Function SafeFailureCode(ByVal failure As Exception) As String
        If failure Is Nothing Then Return Nothing
        Dim candidate = If(failure.Message, String.Empty).Trim().ToUpperInvariant()
        If SafeCode.IsMatch(candidate) Then Return candidate
        If TypeOf failure Is OperationCanceledException Then Return "EXTERNAL_TIMEOUT"
        Return "EXTERNAL_INVALID_RESPONSE"
    End Function
    Private Shared Function Limit(ByVal value As String, ByVal maximum As Integer) As String
        Dim clean = If(value, String.Empty).Trim()
        Return If(clean.Length > maximum, clean.Substring(0, maximum), clean)
    End Function
End Class
