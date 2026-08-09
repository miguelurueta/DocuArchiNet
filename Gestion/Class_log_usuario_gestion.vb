Imports System.Math
Public Structure structure_log_usuario_gestion
    Dim id_log As Long
    Dim id_usuario_gestion As Integer
    Dim Fecha_Inicio_Seccion As String
    Dim Fecha_Fin_Seccion As String
    Dim Direccion_ip_Nombre As String
    Dim Valor_Log As String
    Dim TIEMPO_SESION_USUARIO As Long
End Structure
Public Class Class_log_usuario_gestion
    Function Solicita_datos_estructura_log_usuario_gestion(ByVal id_log As Long, _
                                                           ByRef structure_log_usuario_gestion As structure_log_usuario_gestion) As String
        Try
            Dim Result As String = ""
            Dim Sql_consulta As String = "Select id_log,id_usuario_gestion,Fecha_Inicio_Seccion,Fecha_Fin_Seccion,Direccion_ip_Nombre,Valor_Log, TIEMPO_SESION_USUARIO " & _
                " from log_usuario_gestion where id_log=" & id_log
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_estructura_log_usuario_gestion = "Funcion Solicita_datos_estructura_log_usuario_gestion dice : (" & Result & ")"
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_estructura_log_usuario_gestion = "Imposible encontrar la estructura del log de usuario de gestion   (" & id_log & ") "
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    structure_log_usuario_gestion.id_log = 0
                Else
                    structure_log_usuario_gestion.id_log = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    structure_log_usuario_gestion.id_usuario_gestion = 0
                Else
                    structure_log_usuario_gestion.id_usuario_gestion = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    structure_log_usuario_gestion.Fecha_Inicio_Seccion = ""
                Else
                    Dim j As Date = Datset.Tables(0).Rows(0).Item(2)
                    structure_log_usuario_gestion.Fecha_Inicio_Seccion = Trim(CStr(j.ToString("yyyy'-'MM'-'dd HH':'mm':'ss")))
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) = True Then
                    structure_log_usuario_gestion.Fecha_Fin_Seccion = ""
                Else
                    Dim j As Date = Datset.Tables(0).Rows(0).Item(3)
                    structure_log_usuario_gestion.Fecha_Fin_Seccion = Trim(CStr(j.ToString("yyyy'-'MM'-'dd HH':'mm':'ss")))
                End If
                If Datset.Tables(0).Rows(0).IsNull(4) = True Then
                    structure_log_usuario_gestion.Direccion_ip_Nombre = ""
                Else
                    structure_log_usuario_gestion.Direccion_ip_Nombre = Datset.Tables(0).Rows(0).Item(4)
                End If
                If Datset.Tables(0).Rows(0).IsNull(5) = True Then
                    structure_log_usuario_gestion.Valor_Log = ""
                Else
                    structure_log_usuario_gestion.Valor_Log = Datset.Tables(0).Rows(0).Item(5)
                End If
                If Datset.Tables(0).Rows(0).IsNull(6) = True Then
                    structure_log_usuario_gestion.TIEMPO_SESION_USUARIO = 0
                Else
                    structure_log_usuario_gestion.TIEMPO_SESION_USUARIO = Datset.Tables(0).Rows(0).Item(6)
                End If
                Solicita_datos_estructura_log_usuario_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_estructura_log_usuario_gestion = "Inconsistencia general funcion Solicita_datos_estructura_log_usuario_gestion " & ex.Message
        End Try
    End Function
    Function Actualiza_log_sesion_usuario_gestion_documental(ByVal id_log As Long) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim refclas_gestion_fechas As New ClassGestionFechas
            Dim date1al As String = ""
            Dim Result As String = ""
            Result = refclas_gestion_fechas.Formatea_fecha_time_db(Date.Now, _
                                                                   date1al)
            If Result <> "YES" Then
                Actualiza_log_sesion_usuario_gestion_documental = "Imposible formatear fecha " & Result
                Exit Function
            End If
            Dim structure_log_usuario_gestion As structure_log_usuario_gestion = Nothing
            Result = Me.Solicita_datos_estructura_log_usuario_gestion(id_log, _
                                                                      structure_log_usuario_gestion)
            If Result <> "YES" Then
                Actualiza_log_sesion_usuario_gestion_documental = Result
                Exit Function
            End If
            Dim Minuto_Dur As Long = 0
            Result = refclas_gestion_fechas.Resta_fechas_db(structure_log_usuario_gestion.Fecha_Inicio_Seccion, _
                                                            date1al, _
                                                            Minuto_Dur)
            If Result <> "YES" Then
                Actualiza_log_sesion_usuario_gestion_documental = Result
                Exit Function
            End If
            If Minuto_Dur <= -1 Then
                Minuto_Dur = CInt(Abs(Minuto_Dur))
            End If
            Dim sql_insert As String = "update log_usuario_gestion set Fecha_Fin_Seccion=" & _
                "'" & date1al & "',TIEMPO_SESION_USUARIO=" & Minuto_Dur & " where id_log=" & id_log
            Result = ref.SELECTION_INSERT_COMMAND(sql_insert)
            If Result <> "YES" Then
                Actualiza_log_sesion_usuario_gestion_documental = Result
                Exit Function
            Else
                Actualiza_log_sesion_usuario_gestion_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_log_sesion_usuario_gestion_documental = "Inconsistencia general función Actualiza_log_sesion_usuario_gestion_documental " & ex.Message
        End Try
    End Function
    Function RegistroSesionLogusuarioGestionDocumental(ByVal id_usuario As Integer, _
                                                          ByVal direcion_ip As String, _
                                                          ByRef codigo_transaccion As Integer) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim refclas_gestion_fechas As New ClassGestionFechas
            Dim date1al As String = ""
            Dim Result As String = ""
            Result = refclas_gestion_fechas.Formatea_fecha_time_db(Date.Now, _
                                                                   date1al)
            If Result <> "YES" Then
                RegistroSesionLogusuarioGestionDocumental = "Imposible formatear fecha " & Result
                Exit Function
            End If
            Dim sql_insert As String = "Insert into log_usuario_gestion (id_usuario_gestion,Fecha_Inicio_Seccion,Direccion_ip_Nombre) values (" & _
                id_usuario & ",'" & date1al & "','" & direcion_ip & "')"
            Result = ref.SELECTION_LAST_INSERT_COMMAND(sql_insert, codigo_transaccion)
            If Result <> "YES" Then
                RegistroSesionLogusuarioGestionDocumental = Result
                Exit Function
            Else
                RegistroSesionLogusuarioGestionDocumental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            RegistroSesionLogusuarioGestionDocumental = "Inconsistencia general función RegistroSesionLogusuarioGestionDocumental " & ex.Message
        End Try
    End Function
End Class
