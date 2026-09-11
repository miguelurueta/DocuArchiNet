Imports System
Imports System.Net
Imports System.Net.Http

Public NotInheritable Class ExternalImportHttpError
    Public Sub New(ByVal code As String, ByVal message As String, ByVal correlationId As String, Optional ByVal httpStatus As Nullable(Of Integer) = Nothing)
        Me.Code = code
        Me.Message = message
        Me.CorrelationId = correlationId
        Me.HttpStatus = httpStatus
    End Sub

    Public ReadOnly Property Code As String
    Public ReadOnly Property Message As String
    Public ReadOnly Property CorrelationId As String
    Public ReadOnly Property HttpStatus As Nullable(Of Integer)
End Class

Public NotInheritable Class ExternalImportHttpException
    Inherits Exception

    Public Sub New(ByVal [error] As ExternalImportHttpError, Optional ByVal innerException As Exception = Nothing)
        MyBase.New(If([error] Is Nothing, "Error de proveedor externo.", [error].Message), innerException)
        Me.ErrorDetail = [error]
    End Sub

    Public ReadOnly Property ErrorDetail As ExternalImportHttpError
End Class

Public NotInheritable Class ExternalImportHttpErrorMapper
    Public Function FromStatus(ByVal statusCode As HttpStatusCode, ByVal correlationId As String) As ExternalImportHttpError
        If statusCode = HttpStatusCode.Unauthorized OrElse statusCode = HttpStatusCode.Forbidden Then
            Return Create("EXTERNAL_ACCESS_DENIED", "El proveedor rechazó el acceso.", correlationId, CInt(statusCode))
        End If
        If CInt(statusCode) >= 500 Then
            Return Create("EXTERNAL_UNAVAILABLE", "El proveedor no está disponible.", correlationId, CInt(statusCode))
        End If
        Return Create("EXTERNAL_INVALID_RESPONSE", "El proveedor devolvió una respuesta no válida.", correlationId, CInt(statusCode))
    End Function

    Public Function FromTransport(ByVal exception As Exception, ByVal callerCancelled As Boolean,
                                  ByVal correlationId As String) As ExternalImportHttpError
        If callerCancelled Then Return Create("EXTERNAL_CANCELLED", "La operación fue cancelada.", correlationId)
        If TypeOf exception Is OperationCanceledException Then Return Create("EXTERNAL_TIMEOUT", "El proveedor excedió el tiempo permitido.", correlationId)
        If TypeOf exception Is HttpRequestException Then Return Create("EXTERNAL_UNAVAILABLE", "El proveedor no está disponible.", correlationId)
        Return Create("EXTERNAL_INVALID_RESPONSE", "El proveedor devolvió una respuesta no válida.", correlationId)
    End Function

    Public Function InvalidResponse(ByVal correlationId As String) As ExternalImportHttpError
        Return Create("EXTERNAL_INVALID_RESPONSE", "El proveedor devolvió una respuesta no válida.", correlationId)
    End Function

    Private Shared Function Create(ByVal code As String, ByVal message As String,
                                   ByVal correlationId As String, Optional ByVal httpStatus As Nullable(Of Integer) = Nothing) As ExternalImportHttpError
        Return New ExternalImportHttpError(code, message, If(correlationId, String.Empty).Trim(), httpStatus)
    End Function
End Class
