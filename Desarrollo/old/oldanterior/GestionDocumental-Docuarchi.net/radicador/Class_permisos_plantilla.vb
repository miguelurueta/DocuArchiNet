Public Structure plantillas
    Dim id_plantilla As Integer
    Dim nombre_plantilla As String
    Dim tipo_plantilla As String
    Dim permiso_radicado As Integer
    Dim permiso_consulta As Integer
End Structure
Public Class Class_permisos_plantilla
    Function Solicita_plantillas_radicado_permitidas_usuario_radicador(ByVal Id_Usuario As String, _
                                                                       ByRef Matri_plantillas_radicacion() As plantillas) As String
        '-----------------------------------------------------------------------
        'Funcion : genera la matriz de plantillas permitidas al usuario
        'Fecha : 2014-04-04
        'Ing : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------
        Try
            Dim Campos As String = " PP.System_Plantilla_Radicado_id_Plantilla,"
            Campos = Campos & "SPR.NOMBRE_PLANTILLA_RADICADO,"
            Campos = Campos & "SPR.TIPO_PLANTILLA, pp.Permiso_Radicado, pp.Permiso_Consulta "
            Dim Sqlstext As String = "Select " & Campos & " from permisos_plantilla pp" &
            " inner join system_plantilla_radicado as spr on (spr.id_plantilla=pp.system_plantilla_Radicado_id_plantilla) " &
            " where Usuario_Radicador_id_usuario = " &
            Val(Id_Usuario)
            Dim refra As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As New DataSet
            Result = refra.SELECTION_SELECT_FIELD(Sqlstext, Datset)
            If Result <> "YES" Then
                Solicita_plantillas_radicado_permitidas_usuario_radicador = "Funcion Solicita_plantillas_radicado_permitidas_usuario_radicador dice " & Result
                Exit Function
            End If
            For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                ReDim Preserve Matri_plantillas_radicacion(i)
                Matri_plantillas_radicacion(i).id_plantilla = Datset.Tables(0).Rows(i).Item(0)
                Matri_plantillas_radicacion(i).nombre_plantilla = Datset.Tables(0).Rows(i).Item(1)
                Matri_plantillas_radicacion(i).tipo_plantilla = Datset.Tables(0).Rows(i).Item(2)
                Matri_plantillas_radicacion(i).permiso_radicado = Datset.Tables(0).Rows(i).Item(3)
                Matri_plantillas_radicacion(i).permiso_consulta = Datset.Tables(0).Rows(i).Item(4)
            Next
            Solicita_plantillas_radicado_permitidas_usuario_radicador = "YES"
        Catch ex As Exception
            Solicita_plantillas_radicado_permitidas_usuario_radicador = "Inconsistencia general funcion Solicita_plantillas_radicado_permitidas_usuario_radicador dice " & ex.Message
        End Try
    End Function
End Class
