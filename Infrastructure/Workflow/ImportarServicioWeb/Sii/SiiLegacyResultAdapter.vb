Imports System

Public NotInheritable Class SiiLegacyResult
    Public Property Code As String
    Public Property dato_lista As String
End Class

' Única traducción nueva autorizada de contratos modernos a códigos históricos SII.
Public NotInheritable Class SiiLegacyResultAdapter
    Public Function Translate(ByVal result As ImportItemResultDto) As SiiLegacyResult
        If result Is Nothing Then Throw New ArgumentNullException("result")
        Select Case If(result.Status, String.Empty).Trim().ToUpperInvariant()
            Case "COMPLETED", "COMPLETADA"
                Return New SiiLegacyResult With {.Code = "YES", .dato_lista = BuildLegacyList(result)}
            Case "REQUIRESDECISION", "REQUIEREDECISION"
                Return New SiiLegacyResult With {.Code = "CTRL", .dato_lista = String.Empty}
            Case "STOPPED", "DETENIDA"
                Return New SiiLegacyResult With {.Code = "CTRLRETURN", .dato_lista = String.Empty}
            Case Else
                Return New SiiLegacyResult With {.Code = If(result.ErrorCode, "IMPORT_ERROR"), .dato_lista = String.Empty}
        End Select
    End Function

    Private Shared Function BuildLegacyList(ByVal result As ImportItemResultDto) As String
        Return String.Join("|", New String() {String.Empty, If(result.DocumentId.HasValue, result.DocumentId.Value.ToString(), String.Empty),
            String.Empty, Clean(result.DocumentName), String.Empty, result.TaskId.ToString(), String.Empty, "fa-file"})
    End Function

    Private Shared Function Clean(ByVal value As String) As String
        Return If(value, String.Empty).Replace("|", String.Empty).Trim()
    End Function
End Class
