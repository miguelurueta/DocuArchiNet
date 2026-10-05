Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public NotInheritable Class RegistroTareaRutaSiiCommandAdapter
    Private Sub New()
    End Sub

    Public Shared Function TryParse(ByVal value As Object, ByRef command As SolicitudRegistroTareaRutaSii) As Boolean
        command = Nothing
        Try
            Dim token As JToken
            If TypeOf value Is String Then
                token = JToken.Parse(Convert.ToString(value))
            Else
                token = JToken.FromObject(value)
            End If
            If token.Type = JTokenType.Object Then
                command = token.ToObject(Of SolicitudRegistroTareaRutaSii)()
                Return command IsNot Nothing
            End If
            If token.Type <> JTokenType.Array Then Return False
            command = New SolicitudRegistroTareaRutaSii()
            For Each item In token.Children(Of JObject)()
                Dim name = Convert.ToString(item("name_campo"))
                Dim fieldValue = Convert.ToString(item("value_campo"))
                Select Case name
                    Case "recibo" : command.recibo = fieldValue
                    Case "id_tramite" : Integer.TryParse(fieldValue, command.id_tramite)
                    Case "id_actividad" : Integer.TryParse(fieldValue, command.id_actividad)
                End Select
            Next
            Return True
        Catch
            command = Nothing
            Return False
        End Try
    End Function
End Class
