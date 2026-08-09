Public Class Class_configuracion_usuario
    Function Retorna_parametros_limite_actividades_fecha_tareas(ByRef Numero_Actividades As Integer, _
                                                                ByRef Fecha_Ini As String, _
                                                                ByRef Fecha_Fin As String, _
                                                                ByVal Id_Usuario As Integer) As String
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "Select Numero_Tarea_Lista,Fecha_Ini_Lista,Fecha_Fin_Lista" & _
            " from configuracion_usuario where " & _
            "USUARIO_WORKFLOW_IDU_SUARIO=" & Id_Usuario
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("configuracion_usuario")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_parametros_limite_actividades_fecha_tareas = "Error funcion Leer_Datos_Configuracion_Usuario_workflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Numero_Actividades = 0
                Fecha_Ini = ""
                Fecha_Fin = ""
                Retorna_parametros_limite_actividades_fecha_tareas = "YES"
                Exit Function
            Else
                Dim Tempvalor As Object = Datset.Tables(0).Rows(0).Item(0)
                If IsDBNull(Tempvalor) Then
                    Numero_Actividades = 0
                Else
                    Numero_Actividades = Datset.Tables(0).Rows(0).Item(0)
                End If
                Dim Tempovalor As Object = Datset.Tables(0).Rows(0).Item(1)
                If IsDBNull(Tempovalor) Then
                    Fecha_Ini = ""
                Else
                    Fecha_Ini = Datset.Tables(0).Rows(0).Item(1)
                End If
                Tempovalor = Datset.Tables(0).Rows(0).Item(2)
                If IsDBNull(Tempovalor) Then
                    Fecha_Fin = ""
                Else
                    Fecha_Fin = Datset.Tables(0).Rows(0).Item(2)
                End If
                Retorna_parametros_limite_actividades_fecha_tareas = "YES"
            End If
        Catch ex As Exception
            Retorna_parametros_limite_actividades_fecha_tareas = "Inconsistencia general funcion Leer_Datos_Configuracion_Usuario_workflow " & ex.Message
        End Try
    End Function
End Class
