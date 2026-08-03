Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports MySql.Data.MySqlClient
Imports System.Math

Public Structure stru_estados_flujo_tarea
    Dim id_Estado As Long
    Dim ID_ACTIVIDAD_FLUJO_TRABAJO As Integer
    Dim ID_USUARIO_WORKFLOW_FLUJO_TRABAJO As Integer
End Structure
Public Structure stru_estado
    Dim id_Estado As Long
    Dim id_Ruta As Integer
    Dim id_Tarea As Long
    Dim Id_Actividad As Integer
    Dim Id_Usuario As Integer
    Dim Fecha_Inicio As String
    Dim Fecha_Seleccion As String
    Dim Fecha_Fin As String
    Dim Duracion_Inicio_Seleccion As Integer
    Dim Duracion_Seleccion_Fin As Integer
    Dim Estado_Prioridad As Integer
    Dim Estado_Tarea As Integer
    Dim ID_FLUJO_TRABAJO As Integer
    Dim ID_ACTIVIDAD_FLUJO_TRABAJO As Integer
    Dim ID_USUARIO_WORKFLOW_FLUJO_TRABAJO As Integer
    Dim ESTADO_RECUPERACION_FLUJO_TRABAJO As Integer
End Structure
Public Structure stru_campo_tarea
    Dim nombre_campo As String
    Dim valor_campo As Object
End Structure
Public Class table_boot_row_estado_tarea
    Public ID_TAREA As Integer
    Public ACTIVIDAD As String
    Public USUARIO As String
    Public CARGO As String
    Public result As String
End Class
Public Class Result_row_estado_tarea
    Public result As String
    Public row_estado_tarea As List(Of table_boot_row_estado_tarea)
End Class
Public Structure StruUsuarioTareaAsignada
    Dim LoginUsuario As String
    Dim NombreUsuario As String
    Dim CargoUsuario As String
End Structure
Public Class Class_estados_tarea_workflow
    Function ActualizaEstadoTareaWorkflow(ByVal IdUsuarioWorkflow As Integer,
                                          ByVal IdTareaWorkflow As Long,
                                          ByVal IdActividadWorkflow As Integer,
                                          ByVal idPendiente As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion :  Actualiza estado tarea workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdUsuarioWorkflow   : Representa la identificación del usuario workflow
        'IdTareaWorkflow     : Representa la identificación de la tarea workflow
        'IdActividadWorkflow : Representa la identificación de la actividad workflow
        'IdPendiente         : Representa la identificacion del estado pendiente de la tarea workflow
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2013-01-18
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Minuto_Dur As Long = 0
            Dim Fecha_Fromat As String = ""
            Dim DateCreate As Date = Now
            Dim fecha_inicio_bd As String = ""
            Dim fecha_inicio_fm As String = ""
            Dim Result As String = ""
            Result = Me.SolicitaFechaInicioTareaworkflow(IdTareaWorkflow,
                                                         fecha_inicio_bd)
            If Result <> "YES" Then
                ActualizaEstadoTareaWorkflow = Result
                Exit Function
            End If
            Dim Refclas_gestion_fecha As New ClassGestionFechas
            fecha_inicio_fm = fecha_inicio_bd
            '-----------------------------
            'Formatea framework actual
            '-----------------------------
            Result = Refclas_gestion_fecha.Formatea_fecha_time_framework(DateCreate,
                                                                         Fecha_Fromat)
            If Result <> "YES" Then
                ActualizaEstadoTareaWorkflow = Result
                Exit Function
            End If
            '-----------------------------------
            'Solicita los minutos de diferencia
            '-----------------------------------
            Result = Refclas_gestion_fecha.Resta_fechas_db(fecha_inicio_fm,
                                                           Fecha_Fromat,
                                                           Minuto_Dur)
            If Result <> "YES" Then
                ActualizaEstadoTareaWorkflow = Result
                Exit Function
            End If
            If Minuto_Dur <= -1 Then
                Minuto_Dur = CInt(Abs(Minuto_Dur))
            End If
            Dim Parametro_Insert As String = "UPDATE ESTADOS_TAREA_WORKFLOW " &
            " SET ID_USUARIO=" & IdUsuarioWorkflow &
            ", FECHA_SELECCION='" & Fecha_Fromat & "'" &
            ",DURACION_INICIO_SELECCION=" & Minuto_Dur &
            ",ESTADO_TAREA=0" &
            " WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & IdTareaWorkflow &
            " AND ID_ACTIVIDAD=" & IdActividadWorkflow & " and FECHA_FIN IS NULL"
            Dim Parametro_Insert2 As String = ""
            Parametro_Insert2 = "UPDATE TAREA_PENDIENTE " &
                "SET ESTADOS_PENDIENTE=0 WHERE ID_PENDIENTE=" & idPendiente
            Dim ref As New conect.Dbase_Conction_Mysql
            ref = New conect.Dbase_Conction_Mysql
            If idPendiente = -1 Then
                Result = ref.SELECTION_INSERT_COMMAND(Parametro_Insert)
                If Result <> "YES" Then
                    ActualizaEstadoTareaWorkflow = " Error actualizando estado tarea workflow  función Actualizando_Estado_Tarea dice " & Parametro_Insert
                    Exit Function
                Else
                    ActualizaEstadoTareaWorkflow = "YES"
                    Exit Function
                End If
            Else
                Result = ActualizaEstadoTareaworkflowAtomica(Parametro_Insert, Parametro_Insert2)
                If Result <> "YES" Then
                    ActualizaEstadoTareaWorkflow = " Error # 12566 Actualizando Estado Tarea Workflow   "
                    Exit Function
                Else
                    ActualizaEstadoTareaWorkflow = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            ActualizaEstadoTareaWorkflow = "Inconsistencia general función ActualizaEstadoTareaWorkflow " & ex.Message
        End Try
    End Function
    Function ActualizandoEstadoTareaWorkflowPendienteAtomica(ByVal IdUsuarioWorkflow As Integer,
                                                             ByVal IdTareaWorkflow As Long,
                                                             ByVal IdActividadWorkflow As Integer,
                                                             ByVal IdRutaWorkflow As Integer,
                                                             ByVal IdPendiente As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion :  Actualiza estado tarea con asignación a pendiente
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdUsuarioWorkflow   : Representa la identificación del usuario workflow
        'IdTareaWorkflow     : Representa la identificación de la tarea workflow
        'IdActividadWorkflow : Representa la identificación de la actividad workflow
        'IdRutaWorkflow      : Representa la identiicación de la ruta
        'IdPendiente         : Representa la identificacion del estado pendiente de la tarea workflow
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2013-01-18
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Parametro_Update As String = "UPDATE ESTADOS_TAREA_WORKFLOW " &
                        "SET ESTADO_TAREA=0" &
                        " WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & IdTareaWorkflow &
                        " AND Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta =" & IdRutaWorkflow &
                        " AND Id_Usuario=" & IdUsuarioWorkflow &
                        " AND ID_ACTIVIDAD=" & IdActividadWorkflow & " and FECHA_FIN IS NULL AND ESTADO_TAREA=1"
            Dim Parametro_Insert2 As String = ""
            Parametro_Insert2 = "UPDATE TAREA_PENDIENTE " &
            "SET ESTADOS_PENDIENTE=0 WHERE ID_PENDIENTE=" & IdPendiente
            Result = ActualizaEstadoTareaworkflowAtomica(Parametro_Update, Parametro_Insert2)
            If Result <> "YES" Then
                ActualizandoEstadoTareaWorkflowPendienteAtomica = " Error # 4 AETPA Actualizando Estado Tarea Workflow   "
                Exit Function
            Else
                ActualizandoEstadoTareaWorkflowPendienteAtomica = "YES"
                Exit Function
            End If
        Catch ex As Exception
            ActualizandoEstadoTareaWorkflowPendienteAtomica = ex.Message
        End Try
    End Function
    Function ActualizaEstadoTareaRecuperada(ByVal IdUsuarioWorkflow As Integer,
                                            ByVal IdTareaWorkflow As Long,
                                            ByVal IdActividadWorkflow As Integer,
                                            ByVal IdActividadUsuarioWorkflow As Integer,
                                            ByVal IdFlujoTrabajo As Integer,
                                            ByVal IdActividadFlujoTrabajo As Integer,
                                            ByVal IdUsuarioWorkflowFlujoTrabajo As Integer,
                                            ByVal IdPendiente As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Actualiza el esyado de una tarea recuperada
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdUsuarioWorkflow   : Representa la identificación del usuario workflow
        'IdTareaWorkflow     : Representa la identificiación de una tarea workflow
        'IdActividadWorkflow : Representa la identificiación de una actividad workflow a actualizar
        'IdActividadUsUsuarioWorkflow : Representa la identificiación de la actividad del usuario workflow
        'IdFlujoTrabajo      : Representa la identiiccación del del flujo de trabajo workflow
        'IdActividadFlujoTrabajo : Representa la identificación de la activdad de flujo de trabajo
        'IdUsuarioWorkflowFlujoTrabajo : Representa la identificación del usuario de flujo de trabajo
        'IdPendiente         : Rpresenta el estado pendiente de la tarea
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        'CARACTERIZACIÓN 'Actualizado 2018-10-17 Ing Miguel Angel Urueta Miranda
        'S agrego la funcinalidad de una nueva formula del calculo
        'de los minutos trancurrridos de asignación y un nuevo formato
        'de fecha seleccion tipo militar o  24 horas
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2018-10-17
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try

            Dim Minuto_Dur As Long = 0
            Dim Fecha_Fromat As String = ""
            Dim DateCreate As Date = Now
            Dim fecha_inicio_bd As String = ""
            Dim fecha_inicio_fm As String = ""
            Dim Result As String = ""
            Result = Me.SolicitaFechaInicioTareaworkflow(IdTareaWorkflow,
                                                    fecha_inicio_bd)
            If Result <> "YES" Then
                ActualizaEstadoTareaRecuperada = Result
                Exit Function
            End If
            Dim Refclas_gestion_fecha As New ClassGestionFechas
            fecha_inicio_fm = fecha_inicio_bd
            '--------------------------------------
            'Formatea framework fecha hora actual
            '--------------------------------------
            Result = Refclas_gestion_fecha.Formatea_fecha_time_framework(DateCreate,
                                                                         Fecha_Fromat)
            If Result <> "YES" Then
                ActualizaEstadoTareaRecuperada = Result
                Exit Function
            End If
            '-----------------------------------
            'Solicita los minutos de diferencia
            '-----------------------------------
            Result = Refclas_gestion_fecha.Resta_fechas_db(fecha_inicio_fm,
                                                           Fecha_Fromat,
                                                           Minuto_Dur)
            If Result <> "YES" Then
                ActualizaEstadoTareaRecuperada = Result
                Exit Function
            End If
            If Minuto_Dur <= -1 Then
                Minuto_Dur = CInt(Abs(Minuto_Dur))
            End If
            Dim Parametro_Insert As String = "UPDATE ESTADOS_TAREA_WORKFLOW " &
            " SET ID_USUARIO=" & IdUsuarioWorkflow &
            ", FECHA_SELECCION='" & Fecha_Fromat & "'" &
            ",DURACION_INICIO_SELECCION=" & Minuto_Dur &
            ",ID_ACTIVIDAD=" & IdActividadUsuarioWorkflow &
            ",ID_FLUJO_TRABAJO=" & IdFlujoTrabajo &
            ",ID_ACTIVIDAD_FLUJO_TRABAJO=" & IdActividadFlujoTrabajo &
            ",ID_USUARIO_WORKFLOW_FLUJO_TRABAJO=" & IdUsuarioWorkflowFlujoTrabajo &
            ",ESTADO_RECUPERACION_FLUJO_TRABAJO=1" &
            " WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & IdTareaWorkflow &
            " AND ID_ACTIVIDAD=" & IdActividadWorkflow & " and FECHA_FIN IS NULL"
            Dim Parametro_Insert2 As String = ""
            Parametro_Insert2 = "UPDATE TAREA_PENDIENTE " &
                "SET ESTADOS_PENDIENTE=0 WHERE ID_PENDIENTE=" & IdPendiente
            Dim ref As New conect.Dbase_Conction_Mysql
            ref = New conect.Dbase_Conction_Mysql
            If IdPendiente = -1 Then
                Result = ref.SELECTION_INSERT_COMMAND(Parametro_Insert)
                If Result <> "YES" Then
                    ActualizaEstadoTareaRecuperada = " Error Actualizando Estado Tarea Workflow function  ActualizaEstadoTareaRecuperada  " & Parametro_Insert
                    Exit Function
                Else
                    HttpContext.Current.Session("WF_NUMERO_TAREAS_SELECCIONADAS_W") = HttpContext.Current.Session("WF_NUMERO_TAREAS_SELECCIONADAS_W") + 1
                    ActualizaEstadoTareaRecuperada = "YES"
                    Exit Function
                End If
            Else
                Result = ActualizaEstadoTareaworkflowAtomica(Parametro_Insert, Parametro_Insert2)
                If Result <> "YES" Then
                    ActualizaEstadoTareaRecuperada = " Error # 12567 Actualizando Estado Tarea Workflow   "
                    Exit Function
                Else
                    HttpContext.Current.Session("WF_NUMERO_TAREAS_SELECCIONADAS_W") = HttpContext.Current.Session("WF_NUMERO_TAREAS_SELECCIONADAS_W") + 1
                    ActualizaEstadoTareaRecuperada = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            ActualizaEstadoTareaRecuperada = "Inconsistencia general funcion ActualizaEstadoTareaRecuperada " & ex.Message
        End Try
    End Function
    Function ActualizaEstadoTareaworkflowAtomica(ByVal ParametroUpdate As String,
                                                 ByVal ParametroInsert As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Actualiza estado tarea workflow y registra nuevo estado tarea workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'ParametroUpdate     : Representa comando sql que actuliza la tarea workflow
        'ParametroInsert     : Representa el comando sql que registra el nuevo estdo de la tarea workflow
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------


        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Dim myTrans As MySqlTransaction
        myTrans = myConnection.BeginTransaction()
        myCommand.Connection = myConnection
        myCommand.Transaction = myTrans
        Try
            myCommand.CommandText = ParametroUpdate
            myCommand.ExecuteNonQuery()
            myCommand.CommandText = ParametroInsert
            myCommand.ExecuteNonQuery()
            myTrans.Commit()
            ActualizaEstadoTareaworkflowAtomica = "YES"
            myConnection.Close()
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
                myConnection.Close()
                ActualizaEstadoTareaworkflowAtomica = "Error Actualizando  " & ParametroUpdate &
                " Insertando "

            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    ActualizaEstadoTareaworkflowAtomica = "An exception of type " & ex.GetType().ToString() &
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                End If
            End Try
        End Try
    End Function
    Function SolicitaFechaInicioTareaworkflow(ByVal IdTareaWorkflow As Long,
                                              ByRef FechaInicioTareaWorkflow As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la fecha inicio de una tarea workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTareaWorkflow     : Representa la identificación de la tarea workflow
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'FechaInicioTareaWorkflow  : Retorna la fecha de inicio de la tarea workflow 
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta = "SELECT FECHA_INICIO FROM ESTADOS_TAREA_WORKFLOW " &
            " WHERE " &
            " INICIO_TAREAS_WORKFLOW_ID_TAREA = " & IdTareaWorkflow &
            "  AND FECHA_FIN IS NULL "
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ESTADOS_TAREA_WORKFLOW")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaFechaInicioTareaworkflow = "Función SolicitaFechaInicioTareaworkflow : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaFechaInicioTareaworkflow = "Imposible encontrar la fecha de inicio de la tarea (" & IdTareaWorkflow & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    SolicitaFechaInicioTareaworkflow = "La fecha de la tarea  (" & IdTareaWorkflow & ") se encuentra vacia"
                    Exit Function
                Else
                    Dim j As Date = Datset.Tables(0).Rows(0).Item(0)
                    FechaInicioTareaWorkflow = Trim(CStr(j.ToString("yyyy'-'MM'-'dd HH':'mm':'ss")))
                End If
                SolicitaFechaInicioTareaworkflow = "YES"
            End If
        Catch ex As Exception
            SolicitaFechaInicioTareaworkflow = "Inconsistencia general función SolicitaFechaInicioTareaworkflow " & ex.Message
        End Try
    End Function
    Function Solicita_fecha_inicio_tarea_estado_pendiente(ByVal id_tarea As Long,
                                                          ByVal Id_Actividad As Integer,
                                                          ByRef fecha_inicio As String) As String
        Try
            Dim Sql_consulta = "SELECT FECHA_INICIO FROM ESTADOS_TAREA_WORKFLOW " &
             " WHERE ID_ACTIVIDAD=" & Id_Actividad &
             " AND INICIO_TAREAS_WORKFLOW_ID_TAREA = " & id_tarea &
             " AND ESTADO_TAREA = 1 AND FECHA_FIN IS NULL "
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ESTADOS_TAREA_WORKFLOW")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_fecha_inicio_tarea_estado_pendiente = "Función Solicita_fecha_inicio_tarea_estado_pendiente dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_fecha_inicio_tarea_estado_pendiente = "Imposible encontrar la fecha de inicio de la tarea (" & id_tarea & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    Solicita_fecha_inicio_tarea_estado_pendiente = "La fecha de la tarea  (" & id_tarea & ") se encuentra vacia"
                    Exit Function
                Else
                    Dim j As Date = Datset.Tables(0).Rows(0).Item(0)
                    fecha_inicio = Trim(CStr(j.ToString("yyyy'-'MM'-'dd HH':'mm':'ss")))
                End If
                Solicita_fecha_inicio_tarea_estado_pendiente = "YES"
            End If
        Catch ex As Exception
            Solicita_fecha_inicio_tarea_estado_pendiente = "Inconsistencia general función Solicita_fecha_inicio_tarea_estado_pendiente " & ex.Message
        End Try
    End Function
    Function Solicita_id_actividad_flujo_trabajo_id_usuario_flujo_workflow(ByVal id_flujo_trabajo As Integer,
                                                                           ByVal id_usuario_workflow As Integer,
                                                                           ByVal id_tarea_selecionada As Integer,
                                                                           ByRef id_actividad_flujo_trabajo As Integer,
                                                                           ByRef id_usuario_workflow_flujo_trabajo As Integer) As String
        '-----------------------------------------------------------------
        'Funcion : Solicita el id de la actividad relacionada al flujo
        'de trabajo y el id del flujo usuario de workflow relacionado
        'al flujo de trabajo
        'Fecha : 2017-09-29
        '------------------------------------------------------------------
        Try
            id_actividad_flujo_trabajo = 0
            id_usuario_workflow_flujo_trabajo = 0
            Dim sqlconsulta As String = "Select ID_ACTIVIDAD_FLUJO_TRABAJO,ID_USUARIO_WORKFLOW_FLUJO_TRABAJO from estados_tarea_workflow " &
                " where ID_FLUJO_TRABAJO=" & id_flujo_trabajo & "  AND fecha_fin is null and Inicio_Tareas_Workflow_Id_Tarea=" & id_tarea_selecionada
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Solicita_id_actividad_flujo_trabajo_id_usuario_flujo_workflow = "Error función Solicita_id_actividad_flujo_trabajo_id_usuario_flujo_workflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_actividad_flujo_trabajo_id_usuario_flujo_workflow = "YES"
                Exit Function
            Else
                id_actividad_flujo_trabajo = Datset.Tables(0).Rows(0).Item(0)
                id_usuario_workflow_flujo_trabajo = Datset.Tables(0).Rows(0).Item(1)
                Solicita_id_actividad_flujo_trabajo_id_usuario_flujo_workflow = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Solicita_id_actividad_flujo_trabajo_id_usuario_flujo_workflow = "Inconsistencia general funcion Solicita_id_actividad_flujo_trabajo_id_usuario_flujo_workflow " & ex.Message
        End Try

    End Function
    Function SolicitaEstadosFlujoDocumentalIdTareaUsuarioFlujo(ByVal IdTareaWorkflow As Long,
                                                               ByVal IdActividadFlujoDocumental As Integer,
                                                               ByVal IdFlujoTrabajo As Integer,
                                                               ByRef StruEstadosFlujoTarea() As stru_estados_flujo_tarea,
                                                               ByRef IStru As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita los estados donde participo el usuario en el flujo de trabajo
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTareaWorkflow            : Representa la identificación de la tarea workflow
        'IdActividadFlujoDocumental : Representa la identiicación de la actividad de flujo documental
        'IdFlujoTrabajo             : Representa la idntificación de flujo de trabajo
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'StruEstadosFlujoTarea  : Retorna la estructura de los estados donde participa el uuario en en
        '                         el flujo de trabajo
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-07
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "SELECT id_Estado,ID_ACTIVIDAD_FLUJO_TRABAJO,ID_USUARIO_WORKFLOW_FLUJO_TRABAJO FROM estados_tarea_workflow " &
            "  where ID_FLUJO_TRABAJO=" & IdFlujoTrabajo &
            " AND ID_ACTIVIDAD_FLUJO_TRABAJO=" & IdActividadFlujoDocumental & " AND Inicio_Tareas_Workflow_id_Tarea=" & IdTareaWorkflow
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                SolicitaEstadosFlujoDocumentalIdTareaUsuarioFlujo = "Función Solicita_estados_flujo_documental_id_tarea_usuario_flujo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaEstadosFlujoDocumentalIdTareaUsuarioFlujo = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve StruEstadosFlujoTarea(IStru)
                    StruEstadosFlujoTarea(IStru).id_Estado = Datset.Tables(0).Rows(i).Item(0)
                    StruEstadosFlujoTarea(IStru).ID_ACTIVIDAD_FLUJO_TRABAJO = Datset.Tables(0).Rows(i).Item(1)
                    StruEstadosFlujoTarea(IStru).ID_USUARIO_WORKFLOW_FLUJO_TRABAJO = Datset.Tables(0).Rows(i).Item(2)
                    IStru = IStru + 1
                Next
                SolicitaEstadosFlujoDocumentalIdTareaUsuarioFlujo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaEstadosFlujoDocumentalIdTareaUsuarioFlujo = "Inconsistencia general función Solicita_estados_flujo_documental_id_tarea_usuario_flujo " & ex.Message
        End Try
    End Function
    Function SolicitaIdTareaAsignadaUsuarioWorkflow(ByVal IdAcitividadWorkflow As Integer,
                                                    ByVal IdUsuarioWorkflow As Integer,
                                                    ByRef IdTareaWorkflow As Long) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita identificacion tarea asignada usuario workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdAcitividadWorkflow : Representa la identificación de la actividad workflow
        'IdUsuarioWorkflow    : Representa la identificación del usuario workflow
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdTareaWorkflow  : Retorna la identificaicón asignada al usuario workflow
        '                          
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-08
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim SqlConsulta = ""
            SqlConsulta = "SELECT Inicio_Tareas_Workflow_id_Tarea FROM ESTADOS_TAREA_WORKFLOW" &
            " WHERE ID_ACTIVIDAD=" & IdAcitividadWorkflow &
            " AND ID_USUARIO=" & IdUsuarioWorkflow & " AND " &
            " ESTADO_TAREA = 0 AND" &
            " FECHA_SELECCION IS NOT NULL and FECHA_FIN IS NULL LIMIT 1"
            Dim ConexionDatabase As New conect.Dbase_Conction_Mysql
            Dim DataSet As New DataSet("ESTADOS_TAREA_WORKFLOW")
            Result = ConexionDatabase.SELECTION_SELECT_FIELD(SqlConsulta, DataSet)
            If Result <> "YES" Then
                SolicitaIdTareaAsignadaUsuarioWorkflow = "Funcion  SolicitaIdTareaAsignadaUsuarioWorkflow " & Result
                Exit Function
            End If
            If DataSet.Tables(0).Rows.Count = 0 Then
                IdTareaWorkflow = 0
                SolicitaIdTareaAsignadaUsuarioWorkflow = "YES"
                Exit Function
            Else
                IdTareaWorkflow = DataSet.Tables(0).Rows(0).Item(0)
                SolicitaIdTareaAsignadaUsuarioWorkflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaIdTareaAsignadaUsuarioWorkflow = "Inconsistencia general funcion SolicitaIdTareaAsignadaUsuarioWorkflow " & ex.Message
        End Try
    End Function
    Function SolicitaEstadoTareaAsignadaUsuarioWorkflow(ByVal IdAcitividadWorkflow As Integer,
                                                        ByVal IdUsuarioWorkflow As Integer,
                                                        ByRef EstadoTareaSeleccionada As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita el si el usuario tiene una tarea asignada
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdAcitividadWorkflow : Representa la identificación de la actividad workflow
        'IdUsuarioWorkflow    : Representa la identificación del usuario workflow
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'EstadoTareaSeleccionada  : Retorna el estado si el usuario tiene una tarea seecionada
        '                           0- Usuario sin tarea seleccionada 1-Usuario con tarea seleccionada
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-04
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Result As String = ""
            Dim SqlConsulta = ""
            SqlConsulta = "SELECT * FROM ESTADOS_TAREA_WORKFLOW" &
            " WHERE ID_ACTIVIDAD=" & IdAcitividadWorkflow &
            " AND ID_USUARIO=" & IdUsuarioWorkflow & " AND " &
            " ESTADO_TAREA = 0 AND" &
            " FECHA_SELECCION IS NOT NULL and FECHA_FIN IS NULL LIMIT 1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ESTADOS_TAREA_WORKFLOW")
            Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                SolicitaEstadoTareaAsignadaUsuarioWorkflow = "Funcion  SolicitaEstadoTareaAsignadaUsuarioWorkflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                EstadoTareaSeleccionada = 0
                SolicitaEstadoTareaAsignadaUsuarioWorkflow = "YES"
                Exit Function
            Else
                EstadoTareaSeleccionada = 1
                SolicitaEstadoTareaAsignadaUsuarioWorkflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaEstadoTareaAsignadaUsuarioWorkflow = "Inconsistencia general funcion SolicitaEstadoTareaAsignadaUsuarioWorkflow "
        End Try
    End Function
    Function SolicitaNumeroActividadesSelecionadasUsuario(ByVal IdAcitividadWorkflow As Integer,
                                                          ByVal IdUsuarioWorkflow As Integer,
                                                          ByRef NUmeroActividadesSeleccionadas As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita el numero de actividades seleccionadas 
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdAcitividadWorkflow : Representa la identificación de la actividad workflow
        'IdUsuarioWorkflow    : Representa la identificación del usuario workflow
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'NUmeroActividadesSeleccionadas  : Retorna el numero de actividades seleccionadas
        '                          
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2012-12-08
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT * FROM ESTADOS_TAREA_WORKFLOW" &
            " WHERE ID_ACTIVIDAD=" & IdAcitividadWorkflow &
            " AND ID_USUARIO=" & IdUsuarioWorkflow & " AND " &
            " ESTADO_TAREA = 0 AND" &
            " FECHA_SELECCION IS NOT NULL and FECHA_FIN IS NULL "
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ESTADOS_TAREA_WORKFLOW")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaNumeroActividadesSelecionadasUsuario = "Funcion  SolicitaNumeroActividadesSelecionadasUsuario " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                NUmeroActividadesSeleccionadas = 0
                SolicitaNumeroActividadesSelecionadasUsuario = "YES"
                Exit Function
            Else
                NUmeroActividadesSeleccionadas = Datset.Tables(0).Rows.Count.ToString
                SolicitaNumeroActividadesSelecionadasUsuario = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaNumeroActividadesSelecionadasUsuario = "Error Consultando tareas selecionadas usuario" & ex.Message
        End Try
    End Function
    Function SolicitaEstructuraUsuarioTareaAsignada(ByVal IdTareaworkflow As Integer,
                                                    ByVal IdusuarioWorkflow As Integer,
                                                    ByVal IdActividadWorkflow As Integer,
                                                    ByRef StruUsuarioTareaAsignada As StruUsuarioTareaAsignada) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura del usuario que tiene una tarea asignada
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'id_script           : Representa la identificación del script de validación
        'campo_radicacion    : Representa el nombre del campo de radicación destino
        'id_plantilla_radicacion : 
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'id_usuario_radicador  : Retorna la idnetificación del usuario radicador
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Sql_consulta As String = "SELECT  uw.login_Usuario, uw.nombre_usuario, uw.Cargo_Usuario " &
            " FROM estados_tarea_workflow etw inner join usuario_workflow uw " &
            " on (uw.idU_suario=etw.id_usuario)  where etw.inicio_tareas_workflow_id_tarea=" & IdTareaworkflow &
            " and etw.fecha_seleccion is not null  and  etw.fecha_fin is null and ID_ACTIVIDAD <> " & IdActividadWorkflow &
            " and ID_USUARIO <> " & IdusuarioWorkflow
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaEstructuraUsuarioTareaAsignada = "Error Consultando en tabla " & "estados_tarea_workflow" & Sql_consulta
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                StruUsuarioTareaAsignada = Nothing
                SolicitaEstructuraUsuarioTareaAsignada = "YES"
                Exit Function
            Else
                StruUsuarioTareaAsignada.LoginUsuario = Datset.Tables(0).Rows(0).Item(0).ToString()
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    StruUsuarioTareaAsignada.NombreUsuario = ""
                Else
                    StruUsuarioTareaAsignada.NombreUsuario = Datset.Tables(0).Rows(0).Item(1).ToString()
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    StruUsuarioTareaAsignada.CargoUsuario = ""
                Else
                    StruUsuarioTareaAsignada.CargoUsuario = Datset.Tables(0).Rows(0).Item(2).ToString()
                End If
                SolicitaEstructuraUsuarioTareaAsignada = "YES"
                Exit Function
            End If

        Catch ex As Exception
            SolicitaEstructuraUsuarioTareaAsignada = "Error general funcion SolicitaEstructuraUsuarioTareaAsignada " & ex.Message
        End Try
    End Function
    Function Solicita_datos_estructura_tareas_seleccionada(ByVal id_actividad As Integer,
                                                           ByVal id_usuario As Integer,
                                                           ByVal id_tarea As Long,
                                                           ByRef stru_estado As stru_estado) As String
        '-----------------------------------------------------------
        'Función : Solicita la estructura con los datos de 
        'la tarea seleccionada tienedo como parametros
        'id_actividad : identidicacion de la actividad worlkflow
        'del usuario logueado
        'id_usuario : Indentiicación del usuario worlkflow logueado
        'id_tarea : Identificación de la tarea seleccionada
        'stru_estado : Estructura con los datos contenidos
        'Fecha : 2019-10-23
        'Ing : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT id_Estado,Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta,Inicio_Tareas_Workflow_id_Tarea," &
            "Id_Actividad,Id_Usuario,Fecha_Inicio " &
            ",Fecha_Seleccion,Fecha_Fin,Duracion_Inicio_Seleccion,Duracion_Seleccion_Fin,Estado_Prioridad," &
            "Estado_Tarea,ID_FLUJO_TRABAJO,ID_ACTIVIDAD_FLUJO_TRABAJO,ID_USUARIO_WORKFLOW_FLUJO_TRABAJO," &
            "ESTADO_RECUPERACION_FLUJO_TRABAJO " &
            "FROM ESTADOS_TAREA_WORKFLOW " &
            " WHERE ID_ACTIVIDAD=" & id_actividad &
            " AND ID_USUARIO=" & id_usuario & " AND " &
            " FECHA_SELECCION IS NOT NULL and FECHA_FIN IS NULL and Inicio_Tareas_Workflow_id_Tarea=" & id_tarea
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ESTADOS_TAREA_WORKFLOW")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_estructura_tareas_seleccionada = "Funcion  Solicita_datos_estructura_tareas_seleccionada dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_estructura_tareas_seleccionada = "Imposible encontrar los datos de la tarea (" & id_tarea & ")"
                Exit Function
            Else
                stru_estado.id_Estado = Datset.Tables(0).Rows(0).Item(0)
                stru_estado.id_Ruta = Datset.Tables(0).Rows(0).Item(1)
                stru_estado.id_Tarea = Datset.Tables(0).Rows(0).Item(2)
                stru_estado.Id_Actividad = Datset.Tables(0).Rows(0).Item(3)
                If Datset.Tables(0).Rows(0).IsNull(4) Then
                    stru_estado.Id_Usuario = 0
                Else
                    stru_estado.Id_Usuario = Datset.Tables(0).Rows(0).Item(4)
                End If
                If Datset.Tables(0).Rows(0).IsNull(5) Then
                    stru_estado.Fecha_Inicio = ""
                Else
                    stru_estado.Fecha_Inicio = Datset.Tables(0).Rows(0).Item(5)
                End If
                If Datset.Tables(0).Rows(0).IsNull(6) Then
                    stru_estado.Fecha_Seleccion = ""
                Else
                    stru_estado.Fecha_Seleccion = Datset.Tables(0).Rows(0).Item(6)
                End If
                If Datset.Tables(0).Rows(0).IsNull(7) Then
                    stru_estado.Fecha_Fin = ""
                Else
                    stru_estado.Fecha_Fin = Datset.Tables(0).Rows(0).Item(7)
                End If
                If Datset.Tables(0).Rows(0).IsNull(8) Then
                    stru_estado.Duracion_Inicio_Seleccion = 0
                Else
                    stru_estado.Duracion_Inicio_Seleccion = Datset.Tables(0).Rows(0).Item(8)
                End If
                If Datset.Tables(0).Rows(0).IsNull(9) Then
                    stru_estado.Duracion_Seleccion_Fin = 0
                Else
                    stru_estado.Duracion_Seleccion_Fin = Datset.Tables(0).Rows(0).Item(9)
                End If
                stru_estado.Estado_Prioridad = Datset.Tables(0).Rows(0).Item(10)
                stru_estado.Estado_Tarea = Datset.Tables(0).Rows(0).Item(11)
                stru_estado.ID_FLUJO_TRABAJO = Datset.Tables(0).Rows(0).Item(12)
                stru_estado.ID_ACTIVIDAD_FLUJO_TRABAJO = Datset.Tables(0).Rows(0).Item(13)
                stru_estado.ID_USUARIO_WORKFLOW_FLUJO_TRABAJO = Datset.Tables(0).Rows(0).Item(14)
                stru_estado.ESTADO_RECUPERACION_FLUJO_TRABAJO = Datset.Tables(0).Rows(0).Item(15)
                Solicita_datos_estructura_tareas_seleccionada = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_estructura_tareas_seleccionada = "Inconsistencia general función Solicita_datos_estructura_tareas_seleccionada " & ex.Message
        End Try
    End Function
    Function Solicita_datos_estructura_tareas_seleccionada(ByVal id_tarea As Long,
                                                           ByRef stru_estado As stru_estado) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura de una tarea asignada
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_tarea              : Representa la identificación de la tarea
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'stru_estado         : Retorna la estructura de la tarea 
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-11-23
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT id_Estado,Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta,Inicio_Tareas_Workflow_id_Tarea," &
            "Id_Actividad,Id_Usuario,Fecha_Inicio " &
            ",Fecha_Seleccion,Fecha_Fin,Duracion_Inicio_Seleccion,Duracion_Seleccion_Fin,Estado_Prioridad," &
            "Estado_Tarea,ID_FLUJO_TRABAJO,ID_ACTIVIDAD_FLUJO_TRABAJO,ID_USUARIO_WORKFLOW_FLUJO_TRABAJO," &
            "ESTADO_RECUPERACION_FLUJO_TRABAJO " &
            "FROM ESTADOS_TAREA_WORKFLOW " &
            " WHERE Inicio_Tareas_Workflow_id_Tarea=" & id_tarea &
            " and FECHA_SELECCION IS NOT NULL and FECHA_FIN IS NULL "
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ESTADOS_TAREA_WORKFLOW")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_estructura_tareas_seleccionada = "Funcion  Solicita_datos_estructura_tareas_seleccionada dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_estructura_tareas_seleccionada = "Imposible encontrar los datos de la tarea (" & id_tarea & ")"
                Exit Function
            Else
                stru_estado.id_Estado = Datset.Tables(0).Rows(0).Item(0)
                stru_estado.id_Ruta = Datset.Tables(0).Rows(0).Item(1)
                stru_estado.id_Tarea = Datset.Tables(0).Rows(0).Item(2)
                stru_estado.Id_Actividad = Datset.Tables(0).Rows(0).Item(3)
                If Datset.Tables(0).Rows(0).IsNull(4) Then
                    stru_estado.Id_Usuario = 0
                Else
                    stru_estado.Id_Usuario = Datset.Tables(0).Rows(0).Item(4)
                End If
                If Datset.Tables(0).Rows(0).IsNull(5) Then
                    stru_estado.Fecha_Inicio = ""
                Else
                    stru_estado.Fecha_Inicio = Datset.Tables(0).Rows(0).Item(5)
                End If
                If Datset.Tables(0).Rows(0).IsNull(6) Then
                    stru_estado.Fecha_Seleccion = ""
                Else
                    stru_estado.Fecha_Seleccion = Datset.Tables(0).Rows(0).Item(6)
                End If
                If Datset.Tables(0).Rows(0).IsNull(7) Then
                    stru_estado.Fecha_Fin = ""
                Else
                    stru_estado.Fecha_Fin = Datset.Tables(0).Rows(0).Item(7)
                End If
                If Datset.Tables(0).Rows(0).IsNull(8) Then
                    stru_estado.Duracion_Inicio_Seleccion = 0
                Else
                    stru_estado.Duracion_Inicio_Seleccion = Datset.Tables(0).Rows(0).Item(8)
                End If
                If Datset.Tables(0).Rows(0).IsNull(9) Then
                    stru_estado.Duracion_Seleccion_Fin = 0
                Else
                    stru_estado.Duracion_Seleccion_Fin = Datset.Tables(0).Rows(0).Item(9)
                End If
                stru_estado.Estado_Prioridad = Datset.Tables(0).Rows(0).Item(10)
                stru_estado.Estado_Tarea = Datset.Tables(0).Rows(0).Item(11)
                stru_estado.ID_FLUJO_TRABAJO = Datset.Tables(0).Rows(0).Item(12)
                stru_estado.ID_ACTIVIDAD_FLUJO_TRABAJO = Datset.Tables(0).Rows(0).Item(13)
                stru_estado.ID_USUARIO_WORKFLOW_FLUJO_TRABAJO = Datset.Tables(0).Rows(0).Item(14)
                stru_estado.ESTADO_RECUPERACION_FLUJO_TRABAJO = Datset.Tables(0).Rows(0).Item(15)
                Solicita_datos_estructura_tareas_seleccionada = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_estructura_tareas_seleccionada = "Inconsistencia general función Solicita_datos_estructura_tareas_seleccionada " & ex.Message
        End Try
    End Function
    Function SolicitaDatosEstructuraTareasAsinada(ByVal IdTareaWorkflow As Long,
                                                  ByRef stru_estado As stru_estado) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura de una tarea asignada 
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'IdTareaWorkflow              : Representa la identificación de la tarea
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'stru_estado         : Retorna la estructura de la tarea 
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2025-05-19
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT id_Estado,Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta,Inicio_Tareas_Workflow_id_Tarea," &
            "Id_Actividad,Id_Usuario,Fecha_Inicio " &
            ",Fecha_Seleccion,Fecha_Fin,Duracion_Inicio_Seleccion,Duracion_Seleccion_Fin,Estado_Prioridad," &
            "Estado_Tarea,ID_FLUJO_TRABAJO,ID_ACTIVIDAD_FLUJO_TRABAJO,ID_USUARIO_WORKFLOW_FLUJO_TRABAJO," &
            "ESTADO_RECUPERACION_FLUJO_TRABAJO " &
            "FROM ESTADOS_TAREA_WORKFLOW " &
            " WHERE Inicio_Tareas_Workflow_id_Tarea=" & IdTareaWorkflow &
            " and  FECHA_FIN IS NULL "
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ESTADOS_TAREA_WORKFLOW")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaDatosEstructuraTareasAsinada = "Funcion  SolicitaDatosEstructuraTareasAsinada dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaDatosEstructuraTareasAsinada = "Imposible encontrar los datos de la tarea.  (" & IdTareaWorkflow & ")"
                Exit Function
            Else
                stru_estado.id_Estado = Datset.Tables(0).Rows(0).Item(0)
                stru_estado.id_Ruta = Datset.Tables(0).Rows(0).Item(1)
                stru_estado.id_Tarea = Datset.Tables(0).Rows(0).Item(2)
                stru_estado.Id_Actividad = Datset.Tables(0).Rows(0).Item(3)
                If Datset.Tables(0).Rows(0).IsNull(4) Then
                    stru_estado.Id_Usuario = 0
                Else
                    stru_estado.Id_Usuario = Datset.Tables(0).Rows(0).Item(4)
                End If
                If Datset.Tables(0).Rows(0).IsNull(5) Then
                    stru_estado.Fecha_Inicio = ""
                Else
                    stru_estado.Fecha_Inicio = Datset.Tables(0).Rows(0).Item(5)
                End If
                If Datset.Tables(0).Rows(0).IsNull(6) Then
                    stru_estado.Fecha_Seleccion = ""
                Else
                    stru_estado.Fecha_Seleccion = Datset.Tables(0).Rows(0).Item(6)
                End If
                If Datset.Tables(0).Rows(0).IsNull(7) Then
                    stru_estado.Fecha_Fin = ""
                Else
                    stru_estado.Fecha_Fin = Datset.Tables(0).Rows(0).Item(7)
                End If
                If Datset.Tables(0).Rows(0).IsNull(8) Then
                    stru_estado.Duracion_Inicio_Seleccion = 0
                Else
                    stru_estado.Duracion_Inicio_Seleccion = Datset.Tables(0).Rows(0).Item(8)
                End If
                If Datset.Tables(0).Rows(0).IsNull(9) Then
                    stru_estado.Duracion_Seleccion_Fin = 0
                Else
                    stru_estado.Duracion_Seleccion_Fin = Datset.Tables(0).Rows(0).Item(9)
                End If
                stru_estado.Estado_Prioridad = Datset.Tables(0).Rows(0).Item(10)
                stru_estado.Estado_Tarea = Datset.Tables(0).Rows(0).Item(11)
                stru_estado.ID_FLUJO_TRABAJO = Datset.Tables(0).Rows(0).Item(12)
                stru_estado.ID_ACTIVIDAD_FLUJO_TRABAJO = Datset.Tables(0).Rows(0).Item(13)
                stru_estado.ID_USUARIO_WORKFLOW_FLUJO_TRABAJO = Datset.Tables(0).Rows(0).Item(14)
                stru_estado.ESTADO_RECUPERACION_FLUJO_TRABAJO = Datset.Tables(0).Rows(0).Item(15)
                SolicitaDatosEstructuraTareasAsinada = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaDatosEstructuraTareasAsinada = "Inconsistencia general función SolicitaDatosEstructuraTareasAsinada " & ex.Message
        End Try
    End Function
    Function Solicita_id_ruta_tarea(ByVal id_tarea As Long,
                                    ByRef id_ruta As Integer) As String
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta FROM estados_tarea_workflow" &
            " WHERE Inicio_Tareas_Workflow_id_Tarea=" & id_tarea
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ESTADOS_TAREA_WORKFLOW")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_ruta_tarea = "Funcion  Solicita_id_ruta_tarea " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_ruta_tarea = "Imposible ecnontrar el ruta de la tarea (" & id_tarea & ")"
                Exit Function
            Else
                id_ruta = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_ruta_tarea = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_ruta_tarea = "Inconsistencia general función Solicita_id_ruta_tarea " & ex.Message
        End Try
    End Function
    Function Solicita_id_actividad_tarea(ByVal id_tarea_workflow As Long,
                                         ByRef id_actividad_workflow As Integer) As String
        '---------------------------------------------------------------------------------------------------------------
        '---------
        'Funcion :
        '---------
        'Solicita la actividad a la que pertence la tarea asignada
        '
        '-----------
        'Parametros:
        '-----------
        'id_tarea_workflow - identificación de la tarea workflow
        '-----------
        'Retorno   ;
        '----------
        '
        '
        'id_actividad_workflow : retorna la actividad a la que pertenece la tarea
        '
        '----------
        'Fecha     : 2023-04-28
        '----------
        '----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        '------------------------------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT Id_Actividad FROM estados_tarea_workflow" &
            " WHERE Inicio_Tareas_Workflow_id_Tarea=" & id_tarea_workflow & " and fecha_fin is null"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ESTADOS_TAREA_WORKFLOW")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_actividad_tarea = "Funcion  Solicita_id_ruta_tarea " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_actividad_tarea = "Imposible ecnontrar la actividad de la tarea (" & id_tarea_workflow & ")"
                Exit Function
            Else
                id_actividad_workflow = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_actividad_tarea = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_actividad_tarea = "Inconsistencia general funcion Solicita_id_actividad_tarea " & ex.Message
        End Try
    End Function
    Function Solicita_estado_prioridad(ByVal ID_ACTIVIDAD As String,
                                      ByVal Id_Usuario As String,
                                      ByVal Tarea_Seleccion As String,
                                      ByRef Prioridad As String,
                                      ByVal estado_tarea As Integer) As String

        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT ESTADO_PRIORIDAD,Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta,Fecha_Seleccion FROM ESTADOS_TAREA_WORKFLOW" &
            " WHERE ID_ACTIVIDAD=" & ID_ACTIVIDAD &
            " AND ID_USUARIO=" & Id_Usuario & " AND " &
            " Inicio_Tareas_Workflow_id_Tarea =" & Tarea_Seleccion &
            " AND FECHA_FIN IS NULL ORDER BY  FECHA_INICIO DESC"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("LISTADO_ACTIVIDADES_WORKFLOW")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                Prioridad = ""
                Solicita_estado_prioridad = "Error Consultando en tabla " & " ESTADOS_TAREA_WORKFLOW " & Sql_consulta
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Prioridad = ""
                Solicita_estado_prioridad = "YES"
                Exit Function
            Else
                Prioridad = Datset.Tables(0).Rows(0).Item(0).ToString & "|" & Datset.Tables(0).Rows(0).Item(1).ToString
                Dim Tempvalor As Object = Datset.Tables(0).Rows(0).Item(2)
                If IsDBNull(Tempvalor) Then
                    Prioridad = Prioridad & "|"
                Else
                    Prioridad = Prioridad & "|" & Datset.Tables(0).Rows(0).Item(2).ToString
                End If
                Solicita_estado_prioridad = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Solicita_estado_prioridad = "Error general funcion Solicita_estado_prioridad " & ex.Message
        End Try
    End Function
    Function Solicita_datos_lista_tarea_workflow_cache(ByVal id_tarea_workflow As Long,
                                                       ByRef stru_campo_tarea_() As stru_campo_tarea) As String
        Try
            If HttpContext.Current.Session.Item("CAHCE_CONSULTA_INERT_WF") Is Nothing Then
                Solicita_datos_lista_tarea_workflow_cache = "Su sesión a caducado por favor salga e ingrese nuevamente"
                Exit Function
            End If
            If HttpContext.Current.Session.Item("CAHCE_CONSULTA_INERT_WF") = "" Then
                Solicita_datos_lista_tarea_workflow_cache = "El cache de consulta workflow para asignación esta vacio"
                Exit Function
            End If
            Dim Sql_consulta = ""
            If HttpContext.Current.Session.Item("WF_ESTADO_EVALUA_SCRIPT_INICIO") = 1 Then
                Sql_consulta = HttpContext.Current.Session.Item("CAHCE_CONSULTA_INERT_WF")
            Else
                Sql_consulta = HttpContext.Current.Session.Item("CAHCE_CONSULTA_INERT_WF") &
                "  etw.ID_ACTIVIDAD=" & HttpContext.Current.Session.Item("Id_actividad_Workflow") &
                " AND etw.ID_USUARIO=" & HttpContext.Current.Session("Id_Usuario_Workflow") & " AND " &
                " etw.Inicio_Tareas_Workflow_id_Tarea =" & id_tarea_workflow &
                " AND etw.FECHA_FIN IS NULL ORDER BY  etw.FECHA_INICIO DESC"
            End If

            Dim ClassGestionFechas As New ClassGestionFechas
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ESTADOS_TAREAS_WORKFLOW")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                Solicita_datos_lista_tarea_workflow_cache = "Error funcion Solicita_datos_lista_tarea_workflow_cache  " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_lista_tarea_workflow_cache = "Inposible enconrar datos de la tarea (" & id_tarea_workflow & "), para insertar el registro en el data table"
                Exit Function
            Else
                For i = 0 To Datset.Tables(0).Columns.Count - 1
                    ReDim Preserve stru_campo_tarea_(i)
                    stru_campo_tarea_(i).nombre_campo = Datset.Tables(0).Columns(i).ColumnName
                    Dim fecha_temp As Object
                    Select Case Datset.Tables(0).Columns(i).DataType.ToString
                        Case "System.DateTime"
                            If Datset.Tables(0).Rows(0).IsNull(i) = False Then
                                result = ClassGestionFechas.Formatea_fecha_time_db(Datset.Tables(0).Rows(0).Item(i),
                                                                                   fecha_temp)
                                stru_campo_tarea_(i).valor_campo = fecha_temp
                            Else
                                stru_campo_tarea_(i).valor_campo = Datset.Tables(0).Rows(0).Item(i)
                            End If
                        Case "System.Int16"
                            If Datset.Tables(0).Rows(0).IsNull(i) = False Then
                                stru_campo_tarea_(i).valor_campo = Datset.Tables(0).Rows(0).Item(i)
                            Else
                                stru_campo_tarea_(i).valor_campo = Datset.Tables(0).Rows(0).Item(i)
                            End If
                        Case Else
                            If Datset.Tables(0).Rows(0).IsNull(i) = False Then
                                stru_campo_tarea_(i).valor_campo = Datset.Tables(0).Rows(0).Item(i)
                            Else
                                stru_campo_tarea_(i).valor_campo = Datset.Tables(0).Rows(0).Item(i)
                            End If
                    End Select
                Next
                Solicita_datos_lista_tarea_workflow_cache = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_lista_tarea_workflow_cache = "Inconsistencia general funcion Solicita_datos_lista_tarea_workflow_cache " & ex.Message
        End Try
    End Function
    Function Solicita_estructura_tarea_asignada(ByVal id_tarea_workflow As Integer,
                                                ByRef stru_estado As stru_estado) As String
        '---------------------------------------------------------------------------------------------------------------
        '---------
        'Funcion :
        '---------
        'Solicita los datos de estructura de la tarea seleccionada
        '
        '-----------
        'Parametros:
        '-----------
        'id_tarea_workflow - identificación de la tarea workflow
        '-----------
        'Retorno   ;
        '----------
        'stru_estado - Retorna los datos de la estructura de registro anterior de la tarea asignada
        '----------
        'Fecha     : 2022-04-26
        '----------
        '----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        '------------------------------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT id_Estado,Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta,Inicio_Tareas_Workflow_id_Tarea," &
                "Id_Actividad,Id_Usuario,Fecha_Inicio " &
                ",Fecha_Seleccion,Fecha_Fin,Duracion_Inicio_Seleccion,Duracion_Seleccion_Fin,Estado_Prioridad," &
                "Estado_Tarea,ID_FLUJO_TRABAJO,ID_ACTIVIDAD_FLUJO_TRABAJO,ID_USUARIO_WORKFLOW_FLUJO_TRABAJO," &
                "ESTADO_RECUPERACION_FLUJO_TRABAJO " &
                "FROM ESTADOS_TAREA_WORKFLOW " &
                " WHERE  Inicio_Tareas_Workflow_id_Tarea=" & id_tarea_workflow & " and Fecha_Fin is null"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ESTADOS_TAREA_WORKFLOW")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_estructura_tarea_asignada = "Funcion  Solicita_estructura_tarea_asignada dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estructura_tarea_asignada = "Imposible encontrar los datos de la tarea (" & id_tarea_workflow & ")"
                Exit Function
            Else
                Dim index As Integer = 0
                stru_estado.id_Estado = Datset.Tables(0).Rows(index).Item(0)
                stru_estado.id_Ruta = Datset.Tables(0).Rows(index).Item(1)
                stru_estado.id_Tarea = Datset.Tables(0).Rows(index).Item(2)
                stru_estado.Id_Actividad = Datset.Tables(0).Rows(index).Item(3)
                If Datset.Tables(0).Rows(index).IsNull(4) Then
                    stru_estado.Id_Usuario = 0
                Else
                    stru_estado.Id_Usuario = Datset.Tables(0).Rows(index).Item(4)
                End If
                If Datset.Tables(0).Rows(index).IsNull(5) Then
                    stru_estado.Fecha_Inicio = ""
                Else
                    stru_estado.Fecha_Inicio = Datset.Tables(0).Rows(index).Item(5)
                End If
                If Datset.Tables(0).Rows(index).IsNull(6) Then
                    stru_estado.Fecha_Seleccion = ""
                Else
                    stru_estado.Fecha_Seleccion = Datset.Tables(0).Rows(index).Item(6)
                End If
                If Datset.Tables(0).Rows(0).IsNull(7) Then
                    stru_estado.Fecha_Fin = ""
                Else
                    stru_estado.Fecha_Fin = Datset.Tables(0).Rows(index).Item(7)
                End If
                If Datset.Tables(0).Rows(index).IsNull(8) Then
                    stru_estado.Duracion_Inicio_Seleccion = 0
                Else
                    stru_estado.Duracion_Inicio_Seleccion = Datset.Tables(0).Rows(index).Item(8)
                End If
                If Datset.Tables(0).Rows(index).IsNull(9) Then
                    stru_estado.Duracion_Seleccion_Fin = 0
                Else
                    stru_estado.Duracion_Seleccion_Fin = Datset.Tables(0).Rows(index).Item(9)
                End If
                stru_estado.Estado_Prioridad = Datset.Tables(0).Rows(index).Item(10)
                stru_estado.Estado_Tarea = Datset.Tables(0).Rows(index).Item(11)
                stru_estado.ID_FLUJO_TRABAJO = Datset.Tables(0).Rows(index).Item(12)
                stru_estado.ID_ACTIVIDAD_FLUJO_TRABAJO = Datset.Tables(0).Rows(index).Item(13)
                stru_estado.ID_USUARIO_WORKFLOW_FLUJO_TRABAJO = Datset.Tables(0).Rows(index).Item(14)
                stru_estado.ESTADO_RECUPERACION_FLUJO_TRABAJO = Datset.Tables(0).Rows(index).Item(15)
                Solicita_estructura_tarea_asignada = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_tarea_asignada = "Inconsistencia general funcion Solicita_estructura_tarea_asignada " & ex.Message
        End Try
    End Function
    Function ReasignaTareaWorflowUsuarioSII(ByVal IdTareaWorkflow As Long,
                                            ByVal IdActividad As Integer,
                                            ByVal IdUsuarioWorkflow As Integer,
                                            ByVal stru_estado As stru_estado,
                                            ByVal struregistro_actividaes_flujos_trabajo As struregistro_actividaes_flujos_trabajo,
                                            ByRef NombreUsuario As String,
                                            ByRef CargoUsuario As String,
                                            ByRef NombreActividad As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Reasigna tarea usuario SII
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_actividad          : Respresenta la indentificacion de la actividad a 
        '                        reasignar
        'id_usuario_workflow   : Id usuario workflow que se le reasigna la tarea
        'id_tarea              : Identifica la tarea a reasignar
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-05-09
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            If HttpContext.Current.Session.Item("REASIGNA_TAREA_WORKFLOW_SII") = 0 Then
                ReasignaTareaWorflowUsuarioSII = "No tiene permisos para reasignar tareas en el módulo de integración con el sistema SII.
                                           Por favor, contacte al administrador para solicitar acceso o verificar su perfil de usuario."
                Exit Function
            End If
            If IdUsuarioWorkflow = 0 Or IdUsuarioWorkflow = -1 Then
                ReasignaTareaWorflowUsuarioSII = "Debe seleccionar un usuario del flujo de trabajo para poder asignar la tarea. Por favor,
                                           elija un usuario antes de continuar."
                Exit Function
            End If
            If IdActividad = 0 Or IdActividad = -1 Then
                ReasignaTareaWorflowUsuarioSII = "Debe seleccionar una actividad del flujo de trabajo para poder asignar la tarea. Por favor,
                                           elija una actividad válida antes de continuar."
                Exit Function
            End If
            'Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            'Result = Class_estados_tarea_workflow.Solicita_estructura_tarea_asignada(IdTareaWorkflow,
            '                                                                         stru_estado)
            'If Result <> "YES" Then
            '    ReasignaTareaUsuarioSII = Result
            '    Exit Function
            'End If
            Dim Class_wf_registro_actividaes_flujos_trabajo As New Class_wf_registro_actividaes_flujos_trabajo
            'Dim struregistro_actividaes_flujos_trabajo As struregistro_actividaes_flujos_trabajo = Nothing
            Dim id_actividad_workflow As Integer = 0
            Dim id_actividad_flujo_trabajo As Integer = 0
            Dim id_usuario_worlflow_flujo_trabajo As Integer = 0
            If stru_estado.ID_FLUJO_TRABAJO <> 0 Then
                'Result = Class_wf_registro_actividaes_flujos_trabajo.Solicita_estructura_actividad_flujo_trabajo(IdActividad,
                '                                                                                                 struregistro_actividaes_flujos_trabajo)
                'If Result <> "YES" Then
                '    ReasignaTareaUsuarioSII = Result
                '    Exit Function
                'End If
                id_actividad_workflow = struregistro_actividaes_flujos_trabajo.listado_actividades_workflow_Id_Actividad
                id_actividad_flujo_trabajo = struregistro_actividaes_flujos_trabajo.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO
                id_usuario_worlflow_flujo_trabajo = struregistro_actividaes_flujos_trabajo.ID_USUARIO_WORKFLOW
            Else
                id_actividad_workflow = IdActividad
            End If
            Dim Class_usuario_workflow As New Class_usuario_workflow
            If IdUsuarioWorkflow <> 0 And IdUsuarioWorkflow <> -1 Then
                Result = Class_usuario_workflow.Solicita_nombre_cargo_usuario_workflow(IdUsuarioWorkflow,
                                                                                       NombreUsuario,
                                                                                       CargoUsuario)
                If Result <> "YES" Then
                    ReasignaTareaWorflowUsuarioSII = Result
                    Exit Function
                End If
            End If
            If IdUsuarioWorkflow = -1 Then
                IdUsuarioWorkflow = 0
            End If
            Dim Class_Listado_Actividades_workflow As New Class_Listado_Actividades_workflow
            Result = Class_Listado_Actividades_workflow.Retorna_Nombre_Actividad_id_actividad(id_actividad_workflow,
                                                                                              NombreActividad)
            If Result <> "YES" Then
                ReasignaTareaWorflowUsuarioSII = Result
                Exit Function
            End If
            'Dim Class_wf_log_asignacion_balanceo As New Class_wf_log_asignacion_balanceo
            'Dim stru_registro_balanceo As stru_registro_balanceo = Nothing
            'Dim estado_registro_balanceo As Integer = 0
            'Result = Class_wf_log_asignacion_balanceo.Solicita_estructura_registro_balanceo(stru_estado.id_Estado,
            '                                                                                stru_registro_balanceo)
            'If Result <> "YES" Then
            '    Reasigna_tarea_usuario_sii = Result
            '    Exit Function
            'End If
            Dim classwWorkflow As New ClassWorkflow
            Dim objet As Object = Nothing
            Result = classwWorkflow.Terminar_Tarea_Workflow(IdUsuarioWorkflow,
                                                            id_actividad_workflow,
                                                            IdTareaWorkflow,
                                                            "",
                                                            objet,
                                                            "",
                                                            1,
                                                            "",
                                                            stru_estado.ID_FLUJO_TRABAJO, id_actividad_flujo_trabajo,
                                                            id_usuario_worlflow_flujo_trabajo, 0, 0, 0, 0, 0, 0, 1, 0)
            ReasignaTareaWorflowUsuarioSII = Result
            Exit Function
        Catch ex As Exception
            ReasignaTareaWorflowUsuarioSII = "Inconsistencia general funcion ReasignaTareaWorflowUsuarioSII " & ex.Message
        End Try
    End Function
    Function Reasigna_tarea_workflow(ByVal id_tarea As Long,
                                     ByVal id_actividad As Integer,
                                     ByVal id_usuario_workflow As Integer,
                                     ByRef nombre_usuario As String,
                                     ByRef cargo_usuario As String,
                                     ByRef nombre_actividad As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Reasigna tarea workflow
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_actividad          : Respresenta la indentificacion de la actividad a 
        '                        reasignar
        'id_usuario_workflow   : Id usuario workflow que se le reasigna la tarea
        'id_tarea              : Identifica la tarea a reasignar
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-05-09
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim stru_estado As stru_estado = Nothing
            If HttpContext.Current.Session.Item("REASIGNA_TAREA_WORKFLOW") = 0 Then
                Reasigna_tarea_workflow = "El usuario no tiene permiso para reasignar la tarea"
                Exit Function
            End If
            If id_actividad = 0 Or id_actividad = -1 Then
                Reasigna_tarea_workflow = "Debe seleccionar la actividad o grupo"
                Exit Function
            End If
            Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            Result = Class_estados_tarea_workflow.Solicita_estructura_tarea_asignada(id_tarea,
                                                                                     stru_estado)
            If Result <> "YES" Then
                Reasigna_tarea_workflow = Result
                Exit Function
            End If
            Dim Class_wf_registro_actividaes_flujos_trabajo As New Class_wf_registro_actividaes_flujos_trabajo
            Dim struregistro_actividaes_flujos_trabajo As struregistro_actividaes_flujos_trabajo = Nothing
            Dim id_actividad_workflow As Integer = 0
            Dim id_actividad_flujo_trabajo As Integer = 0
            Dim id_usuario_worlflow_flujo_trabajo As Integer = 0
            If stru_estado.ID_FLUJO_TRABAJO <> 0 Then
                Result = Class_wf_registro_actividaes_flujos_trabajo.Solicita_estructura_actividad_flujo_trabajo(id_actividad,
                                                                                                                 struregistro_actividaes_flujos_trabajo)
                If Result <> "YES" Then
                    Reasigna_tarea_workflow = Result
                    Exit Function
                End If
                id_actividad_workflow = struregistro_actividaes_flujos_trabajo.listado_actividades_workflow_Id_Actividad
                id_actividad_flujo_trabajo = struregistro_actividaes_flujos_trabajo.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO
                id_usuario_worlflow_flujo_trabajo = struregistro_actividaes_flujos_trabajo.ID_USUARIO_WORKFLOW
            Else
                id_actividad_workflow = id_actividad
            End If
            Dim Class_usuario_workflow As New Class_usuario_workflow
            If id_usuario_workflow <> 0 And id_usuario_workflow <> -1 Then
                Result = Class_usuario_workflow.Solicita_nombre_cargo_usuario_workflow(id_usuario_workflow,
                                                                                       nombre_usuario,
                                                                                       cargo_usuario)
                If Result <> "YES" Then
                    Reasigna_tarea_workflow = Result
                    Exit Function
                End If
            End If
            If id_usuario_workflow = -1 Then
                id_usuario_workflow = 0
            End If
            Dim Class_Listado_Actividades_workflow As New Class_Listado_Actividades_workflow
            Result = Class_Listado_Actividades_workflow.Retorna_Nombre_Actividad_id_actividad(id_actividad_workflow,
                                                                                              nombre_actividad)
            If Result <> "YES" Then
                Reasigna_tarea_workflow = Result
                Exit Function
            End If
            'Dim Class_wf_log_asignacion_balanceo As New Class_wf_log_asignacion_balanceo
            'Dim stru_registro_balanceo As stru_registro_balanceo = Nothing
            'Dim estado_registro_balanceo As Integer = 0
            'Result = Class_wf_log_asignacion_balanceo.Solicita_estructura_registro_balanceo(stru_estado.id_Estado,
            '                                                                                stru_registro_balanceo)
            'If Result <> "YES" Then
            '    Reasigna_tarea_workflow = Result
            '    Exit Function
            'End If
            Dim classwWorkflow As New ClassWorkflow
            Dim objet As Object = Nothing
            Result = classwWorkflow.Terminar_Tarea_Workflow(id_usuario_workflow, id_actividad_workflow, id_tarea, "", objet, "", 1, "",
                                                            stru_estado.ID_FLUJO_TRABAJO, id_actividad_flujo_trabajo,
                                                            id_usuario_worlflow_flujo_trabajo, 0, 0, 0, 0, 0, 0, 0, 1)
            Reasigna_tarea_workflow = Result
            Exit Function
        Catch ex As Exception
            Reasigna_tarea_workflow = "Inconsistencia general funcion Reasigna_tarea_usuario_sii " & ex.Message
        End Try
    End Function
    Function Solicita_datos_tarea_usuario_anterior_a_devolver(ByVal id_tarea_workflow As Integer,
                                                              ByVal notifica_estado As Integer,
                                                              ByRef stru_estado As stru_estado) As String

        '---------------------------------------------------------------------------------------------------------------
        '---------
        'Funcion :
        '---------
        'Solicita los datos del registro anterior de la tarea asignada
        '
        '-----------
        'Parametros:
        '-----------
        'id_tarea_workflow - identificación de la tarea workflow
        '-----------
        'Retorno   ;
        '----------
        'estado_registro_asignacion, valores (NO) no contiene registros
        '(YES) contiene registros
        'stru_estado - Retorna los datos de la estructura de registro anterior de la tarea asignada
        '----------
        'Fecha     : 2022-09-09
        '----------
        '----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        '------------------------------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT id_Estado,Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta,Inicio_Tareas_Workflow_id_Tarea," &
                "Id_Actividad,Id_Usuario,Fecha_Inicio " &
                ",Fecha_Seleccion,Fecha_Fin,Duracion_Inicio_Seleccion,Duracion_Seleccion_Fin,Estado_Prioridad," &
                "Estado_Tarea,ID_FLUJO_TRABAJO,ID_ACTIVIDAD_FLUJO_TRABAJO,ID_USUARIO_WORKFLOW_FLUJO_TRABAJO," &
                "ESTADO_RECUPERACION_FLUJO_TRABAJO " &
                "FROM ESTADOS_TAREA_WORKFLOW " &
                " WHERE  Inicio_Tareas_Workflow_id_Tarea=" & id_tarea_workflow & " order by id_estado desc limit 2"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ESTADOS_TAREA_WORKFLOW")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_tarea_usuario_anterior_a_devolver = "Funcion  Solicita_datos_estructura_tareas_seleccionada dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_tarea_usuario_anterior_a_devolver = "Imposible encontrar los datos de la tarea (" & id_tarea_workflow & ")"
                Exit Function
            Else
                Dim index As Integer = 1
                Dim estado_registro_devolver As String = ""
                If Datset.Tables(0).Rows.Count = 1 Then
                    If notifica_estado = 0 Then
                        Solicita_datos_tarea_usuario_anterior_a_devolver = "El sistema no registra un usuario anterior para devolver la tarea "
                        Exit Function
                    Else
                        index = 0
                        estado_registro_devolver = "NA"
                    End If
                End If
                stru_estado.id_Estado = Datset.Tables(0).Rows(index).Item(0)
                stru_estado.id_Ruta = Datset.Tables(0).Rows(index).Item(1)
                stru_estado.id_Tarea = Datset.Tables(0).Rows(index).Item(2)
                stru_estado.Id_Actividad = Datset.Tables(0).Rows(index).Item(3)
                If Datset.Tables(0).Rows(index).IsNull(4) Then
                    stru_estado.Id_Usuario = 0
                Else
                    stru_estado.Id_Usuario = Datset.Tables(0).Rows(index).Item(4)
                End If
                If Datset.Tables(0).Rows(index).IsNull(5) Then
                    stru_estado.Fecha_Inicio = ""
                Else
                    stru_estado.Fecha_Inicio = Datset.Tables(0).Rows(index).Item(5)
                End If
                If Datset.Tables(0).Rows(index).IsNull(6) Then
                    stru_estado.Fecha_Seleccion = ""
                Else
                    stru_estado.Fecha_Seleccion = Datset.Tables(0).Rows(index).Item(6)
                End If
                If Datset.Tables(0).Rows(0).IsNull(7) Then
                    stru_estado.Fecha_Fin = ""
                Else
                    stru_estado.Fecha_Fin = Datset.Tables(0).Rows(index).Item(7)
                End If
                If Datset.Tables(0).Rows(index).IsNull(8) Then
                    stru_estado.Duracion_Inicio_Seleccion = 0
                Else
                    stru_estado.Duracion_Inicio_Seleccion = Datset.Tables(0).Rows(index).Item(8)
                End If
                If Datset.Tables(0).Rows(index).IsNull(9) Then
                    stru_estado.Duracion_Seleccion_Fin = 0
                Else
                    stru_estado.Duracion_Seleccion_Fin = Datset.Tables(0).Rows(index).Item(9)
                End If
                stru_estado.Estado_Prioridad = Datset.Tables(0).Rows(index).Item(10)
                stru_estado.Estado_Tarea = Datset.Tables(0).Rows(index).Item(11)
                stru_estado.ID_FLUJO_TRABAJO = Datset.Tables(0).Rows(index).Item(12)
                stru_estado.ID_ACTIVIDAD_FLUJO_TRABAJO = Datset.Tables(0).Rows(index).Item(13)
                stru_estado.ID_USUARIO_WORKFLOW_FLUJO_TRABAJO = Datset.Tables(0).Rows(index).Item(14)
                stru_estado.ESTADO_RECUPERACION_FLUJO_TRABAJO = Datset.Tables(0).Rows(index).Item(15)
                If estado_registro_devolver = "NA" Then
                    Solicita_datos_tarea_usuario_anterior_a_devolver = "NA"
                    Exit Function
                Else
                    Solicita_datos_tarea_usuario_anterior_a_devolver = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Solicita_datos_tarea_usuario_anterior_a_devolver = "Inconsistencia general funcion Solicita_datos_tarea_usuario_anterior_a_devolver " & ex.Message
        End Try
    End Function
    Function Solicita_estado_gestion_radicado_tarea_workflow(ByVal id_tarea_workflow As Long,
                                                             ByRef estado_activida_modulo_rad As Integer) As String

        '---------------------------------------------------------------------------------------------------------------
        '---------
        'Funcion :
        '---------
        'Solicita el estado de gestión en el modulo radicación
        '
        '-----------
        'Parametros:
        '-----------
        'id_tarea_workflow - identificación de la tarea workflow
        '-----------
        'Retorno   ;
        '----------
        'estado_activida_modulo_rad, valores (1) la actividad esta en gestión de radicación (0) la tarea workflow no esta
        'en estado de gestión en le mudulo de radicación
        '
        '----------
        'Fecha     : 2022-12-26
        '----------
        '----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        '------------------------------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT ESTADO_ACTIVIDA_MODULO_RAD " &
                    "FROM ESTADOS_TAREA_WORKFLOW " &
                    " WHERE  Inicio_Tareas_Workflow_id_Tarea=" & id_tarea_workflow & " and Fecha_Fin is null"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ESTADOS_TAREA_WORKFLOW")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then

                Solicita_estado_gestion_radicado_tarea_workflow = "Funcion  Solicita_estado_gestion_radicado_tarea_workflow dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_activida_modulo_rad = 0
                Solicita_estado_gestion_radicado_tarea_workflow = "Imposible encontrar los datos de la tarea  (" & id_tarea_workflow & ") para el estado de gestión modulo radicación"
                Exit Function
            Else
                estado_activida_modulo_rad = Datset.Tables(0).Rows(0).Item(0)
                Solicita_estado_gestion_radicado_tarea_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_gestion_radicado_tarea_workflow = "Inconsistencia general función Solicita_estado_gestion_radicado_tarea_workflow " & ex.Message
        End Try
    End Function
    Function Solicita_estado_asignacion_tarea_workflow(ByVal id_tarea_workflow As Long,
                                                       ByRef estado_asignacion As String,
                                                       ByRef nombre_usuario As String,
                                                       ByRef cargo_usuario As String,
                                                       ByRef login_usuario As String) As String
        '---------------------------------------------------------------------------------------------------------------
        '---------
        'Funcion :
        '---------
        'Solicita el estado de asginación de la tarea workflow, esta fución es importante para la recuperación de tareas
        '
        '-----------
        'Parametros:
        '-----------
        'id_tarea_workflow - identificación de la tarea workflow
        '-----------
        'Retorno   ;
        '----------
        'estado_activida_modulo_rad, valores (YES) si la actividad esta asignada (NO) si la tarea workflow no esta
        'en estado asignada, usuario_asignado : Devuelve el usuario asignado
        'cargo_usuario : retorna el cargo del usuario asignado
        'login_usuario : Retorna el cargo del usuario asignado
        '----------
        'Fecha     : 2023-01-16
        '----------
        '----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        '------------------------------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT  uw.login_Usuario, uw.nombre_usuario, uw.cargo_usuario " &
                        "FROM ESTADOS_TAREA_WORKFLOW as etw " &
                        " left outer join  usuario_workflow as uw on (uw.idU_suario=etw.id_usuario) " &
                        " WHERE  Inicio_Tareas_Workflow_id_Tarea=" & id_tarea_workflow & " and id_usuario is not null and fecha_seleccion is not null and fecha_fin is null"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ESTADOS_TAREA_WORKFLOW")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then

                Solicita_estado_asignacion_tarea_workflow = "Funcion  Solicita_estado_asignacion_tarea_workflow dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_asignacion = "NO"
                Solicita_estado_asignacion_tarea_workflow = "YES"
                Exit Function
            Else
                estado_asignacion = "YES"
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    login_usuario = ""
                Else
                    login_usuario = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) Then
                    nombre_usuario = ""
                Else
                    nombre_usuario = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) Then
                    cargo_usuario = ""
                Else
                    cargo_usuario = Datset.Tables(0).Rows(0).Item(2)
                End If
                Solicita_estado_asignacion_tarea_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_asignacion_tarea_workflow = "Inconsistencia general funcion Solicita_estado_asignacion_tarea_workflow " & ex.Message
        End Try
    End Function
    Function Lista_estado_tarea_asignada(ByVal id_tarea_workflow As Long,
                                         ByRef resultList As List(Of table_boot_row_estado_tarea)) As String
        '---------------------------------------------------------------------------
        'Funcion : Lista estructur de tarea asignada
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_tarea_workflow     : Identifica la tarea a reasignar
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'resultList            : Retorna la estructura del eatado de la tarea
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-05-09
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT etw.Inicio_Tareas_Workflow_id_Tarea as ID_TAREA,law.Nombre_Actividad as ACTIVIDAD," &
            "uw.Nombre_Usuario as USUARIO,uw.cargo_usuario as CARGO " &
            " FROM estados_tarea_workflow as etw" &
            " INNER JOIN listado_actividades_workflow as law on (law.Id_Actividad=etw.Id_Actividad)" &
            " left outer join usuario_workflow as uw on (uw.idU_suario=etw.Id_Usuario)" &
            " WHERE Inicio_Tareas_Workflow_id_Tarea=" & id_tarea_workflow & " and fecha_fin is null"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ESTADOS_TAREA_WORKFLOW")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_estado_tarea_asignada = "Funcion  Solicita_id_ruta_tarea " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_estado_tarea_asignada = "YES"
                Exit Function
            Else
                Dim item As New table_boot_row_estado_tarea
                item.ID_TAREA = Datset.Tables(0).Rows(0).Item(0)
                item.ACTIVIDAD = Datset.Tables(0).Rows(0).Item(1)
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    item.USUARIO = ""
                Else
                    item.USUARIO = Datset.Tables(0).Rows(0).Item(2)
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) = True Then
                    item.CARGO = ""
                Else
                    item.CARGO = Datset.Tables(0).Rows(0).Item(3)
                End If
                resultList.Add(item)
                Lista_estado_tarea_asignada = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_estado_tarea_asignada = "Inconsistencia general funcion Lista_estado_tarea_asignada " & ex.Message
        End Try
    End Function
    Function Obtener_Estado_Prioridad(ByVal id_tarea As Long,
                                      ByRef Prioridad As String) As String

        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT ESTADO_PRIORIDAD,Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta,Fecha_Seleccion FROM ESTADOS_TAREA_WORKFLOW" &
            " WHERE Fecha_Fin is null and " &
            " Inicio_Tareas_Workflow_id_Tarea =" & id_tarea
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ESTADOS_TAREA_WORKFLOW")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                Prioridad = ""
                Obtener_Estado_Prioridad = "Error Consultando en tabla " & "ESTADOS_TAREA_WORKFLOW" & Sql_consulta
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Prioridad = ""
                Obtener_Estado_Prioridad = "YES"
                Exit Function
            Else
                Prioridad = Datset.Tables(0).Rows(0).Item(0).ToString & "|" & Datset.Tables(0).Rows(0).Item(1).ToString
                Dim Tempvalor As Object = Datset.Tables(0).Rows(0).Item(2)
                If IsDBNull(Tempvalor) Then
                    Prioridad = Prioridad & "|"
                Else
                    Prioridad = Prioridad & "|" & Datset.Tables(0).Rows(0).Item(2).ToString
                End If
                Obtener_Estado_Prioridad = "YES"
            End If

        Catch ex As Exception
            Obtener_Estado_Prioridad = "Error Consultando prioridad id tarea seleccionada" & ex.ToString()
        End Try
    End Function
    Function Actualiza_etado_modulo_radicado(ByVal sql_actualiza As String) As String
        Try
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Result = ref.SELECTION_INSERT_COMMAND(sql_actualiza)
            If Result <> "YES" Then
                Actualiza_etado_modulo_radicado = "Error Actualiza_etado_modulo_radicado " & sql_actualiza _
               & " Descripcion Error  " & Result
                Exit Function
            Else
                Actualiza_etado_modulo_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_etado_modulo_radicado = "Inconsistencia función Actualiza_etado_modulo_radicado " & ex.Message
        End Try
    End Function
End Class
