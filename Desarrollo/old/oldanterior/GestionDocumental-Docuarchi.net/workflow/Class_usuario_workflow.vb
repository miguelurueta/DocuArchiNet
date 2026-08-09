Imports GestionDocumental_Docuarchi.net.Class_config_general_service
Imports MySql.Data.MySqlClient
Public Structure stru_list_usuarios
    Public id_actividad As Integer
    Public nombre_actividad As String
    Public cargo_usuario As String
    Public nombre_usuario As String
    Public result As String
    Public reault_cambio_estado As String
End Structure
Public Class table_boot_lista_usuario_workflow_balance
    Property idU_suario As Integer
    Property estado_balanceo_grupo As Integer
    Property estado_asigna_tarea As Integer
    Property login_Usuario As String
    Property Nombre_usuario As String
    Property Cargo_Usuario As String
    Property Nombre_Grupo As String
    Property result As String
    Property row_usuario_workflow_blance As IList(Of row_usuario_workflow_blance)
End Class
Public Class row_usuario_workflow_blance
    Public idU_suario As Integer
    Public estado_balanceo_grupo As Integer
    Public estado_asigna_tarea As Integer
    Public login_Usuario As String
    Public Nombre_usuario As String
    Public Cargo_Usuario As String
    Public Nombre_Grupo As String
End Class
Public Class Class_list_realcion_usuario_actvida_flujo
    Property Error_gestion As String
    Property Class_service_ilist_drowlist As IList(Of Class_service_ilist_drowlist)
End Class
Public Class Class_usuario_workflow
    Function Cambia_estado_asignacion_usuario_workflow(ByVal id_usuario_operacion As Integer,
                                                       ByVal id_usuario_afectado As Integer,
                                                       ByVal estado_asignacion As Integer) As String
        '---------------------------------------------------------------------------
        'Funcion : Cambia el estado de asignación del usuario workflow     
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_usuario_operacion  : Usuario que realiza la transación
        'id_usuario_afectado   : Usuario afectado en la transación
        'estado_asignacion     : Representa el estado asignacion
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-11-06
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim DateCreate As Date = Now
        Dim fecha_trans As String = ""
        Dim Result As String = ""
        Dim Refclas_gestion_fecha As New ClassGestionFechas
        Result = Refclas_gestion_fecha.Formatea_fecha_time_framework(DateCreate,
                                                                      fecha_trans)
        If Result <> "YES" Then
            Cambia_estado_asignacion_usuario_workflow = Result
            Exit Function
        End If
        Dim operacion As String = ""
        If estado_asignacion = 0 Then
            operacion = "DESACTIVA ASIGNACION"
        Else
            operacion = "ACTIVA ASIGNACION"
        End If
        Dim sql_update_estado As String = "update usuario_workflow set UTIL_ASIGNA_TAREA=" & estado_asignacion & " where idU_suario=" & id_usuario_afectado
        Dim sql_insert_log As String = "Insert into wf_log_gestion_usuario (usuario_workflow_idU_suario,operacion,Id_usuario_wf_afectado,fecha_hora) values (" &
            id_usuario_operacion & ",'" & operacion & "'," & id_usuario_afectado & ",'" & fecha_trans & "')"
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Dim myTrans As MySqlTransaction
        myTrans = myConnection.BeginTransaction()
        myCommand.Connection = myConnection
        myCommand.Transaction = myTrans
        Dim paramter_resp As Integer = 0
        Try
            myCommand.CommandText = sql_update_estado
            paramter_resp = myCommand.ExecuteNonQuery()
            If paramter_resp = 0 Then
                myConnection.Close()
                Cambia_estado_asignacion_usuario_workflow = "Imposible actualizar estado balanceo " & sql_update_estado
                Exit Function
            End If

            myCommand.CommandText = sql_insert_log
            paramter_resp = myCommand.ExecuteNonQuery()
            If paramter_resp = 0 Then
                myTrans.Rollback()
                myConnection.Close()
                Cambia_estado_asignacion_usuario_workflow = "Imposible insertar registro log  " & sql_insert_log
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Cambia_estado_asignacion_usuario_workflow = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
                myConnection.Close()
                Cambia_estado_asignacion_usuario_workflow = "Error Actualizando  " & sql_update_estado
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Cambia_estado_asignacion_usuario_workflow = "An exception of type " & ex.GetType().ToString() &
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                End If
            End Try
            Cambia_estado_asignacion_usuario_workflow = "Error general funcion  Cambia_estado_asignacion_usuario_workflow  " & e.Message
            Exit Function
        End Try
    End Function
    Function Inactiva_usuario_workflow_balanceo_grupo(ByVal id_usuario_operacion As Integer,
                                                      ByVal id_usuario_afectado As Integer) As String
        '---------------------------------------------------------------------------
        'Funcion : Inactiva usuario workflow para balaceo de grupo
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_usuario_operacion  : Usuario que realiza la transación
        'id_usuario_afectado   : Usuario afectado en la transación
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-08-18
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim DateCreate As Date = Now
        Dim fecha_trans As String = ""
        Dim Result As String = ""
        Dim Refclas_gestion_fecha As New ClassGestionFechas
        Result = Refclas_gestion_fecha.Formatea_fecha_time_framework(DateCreate,
                                                                      fecha_trans)
        If Result <> "YES" Then
            Inactiva_usuario_workflow_balanceo_grupo = Result
            Exit Function
        End If
        Dim Class_wf_registro_asignacion_ruta As New Class_wf_registro_asignacion_ruta
        Dim estado_asignacion As String = ""
        Result = Class_wf_registro_asignacion_ruta.Solicita_exitencia_registro_asignacion_usuario_ruta(id_usuario_afectado,
                                                                                                       estado_asignacion)
        If Result <> "YES" Then
            Inactiva_usuario_workflow_balanceo_grupo = Result
            Exit Function
        End If
        Dim sql_update_estado As String = "update usuario_workflow set estado_balanceo_grupo=0 where idU_suario=" & id_usuario_afectado
        Dim Sql_delete_operation As String = "delete from wf_registro_asignacion_ruta where usuario_workflow_idu_suario=" & id_usuario_afectado
        Dim sql_insert_log As String = "Insert into wf_log_gestion_usuario (usuario_workflow_idU_suario,operacion,Id_usuario_wf_afectado,fecha_hora) values (" &
            id_usuario_operacion & ",'" & "DESACTIVA BALANCEO'," & id_usuario_afectado & ",'" & fecha_trans & "')"
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Dim myTrans As MySqlTransaction
        myTrans = myConnection.BeginTransaction()
        myCommand.Connection = myConnection
        myCommand.Transaction = myTrans
        Dim paramter_resp As Integer = 0
        Try
            myCommand.CommandText = sql_update_estado
            paramter_resp = myCommand.ExecuteNonQuery()
            If paramter_resp = 0 Then
                myConnection.Close()
                Inactiva_usuario_workflow_balanceo_grupo = "Imposible actualizar estado balanceo " & sql_update_estado
                Exit Function
            End If
            If estado_asignacion = "YES" Then
                myCommand.CommandText = Sql_delete_operation
                paramter_resp = myCommand.ExecuteNonQuery()
                If paramter_resp = 0 Then
                    myTrans.Rollback()
                    myConnection.Close()
                    Inactiva_usuario_workflow_balanceo_grupo = "Imposible eliminar registro balanceo grupo  " & Sql_delete_operation
                    Exit Function
                End If
            End If
            myCommand.CommandText = sql_insert_log
            paramter_resp = myCommand.ExecuteNonQuery()
            If paramter_resp = 0 Then
                myTrans.Rollback()
                myConnection.Close()
                Inactiva_usuario_workflow_balanceo_grupo = "Imposible insertar registro log  " & sql_insert_log
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Inactiva_usuario_workflow_balanceo_grupo = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
                myConnection.Close()
                Inactiva_usuario_workflow_balanceo_grupo = "Error Actualizando  " & sql_update_estado
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Inactiva_usuario_workflow_balanceo_grupo = "An exception of type " & ex.GetType().ToString() &
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                End If
            End Try
            Inactiva_usuario_workflow_balanceo_grupo = "Error general funcio   Inactiva_usuario_workflow_balanceo_grupo  " & e.Message
            Exit Function
        End Try
    End Function
    Function Activa_usuario_workflow_balanceo_grupo(ByVal id_usuario_operacion As Integer,
                                                    ByVal id_usuario_afectado As Integer) As String

        '---------------------------------------------------------------------------
        'Funcion : Activa usuario workflow para balaceo de grupo
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_usuario_operacion  : Usuario que realiza la transación
        'id_usuario_afectado   : Usuario afectado en la transación
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-08-18
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------

        Dim DateCreate As Date = Now
        Dim fecha_trans As String = ""
        Dim Result As String = ""
        Dim Refclas_gestion_fecha As New ClassGestionFechas
        Result = Refclas_gestion_fecha.Formatea_fecha_time_base_mysql(DateCreate,
                                                                      fecha_trans)
        If Result <> "YES" Then
            Activa_usuario_workflow_balanceo_grupo = Result
            Exit Function
        End If
        Dim Class_wf_registro_asignacion_ruta As New Class_wf_registro_asignacion_ruta
        Dim estado_asignacion As String = ""
        Result = Class_wf_registro_asignacion_ruta.Solicita_exitencia_registro_asignacion_usuario_ruta(id_usuario_afectado,
                                                                                                       estado_asignacion)
        If Result <> "YES" Then
            Activa_usuario_workflow_balanceo_grupo = Result
            Exit Function
        End If
        Dim sql_update_estado As String = "update usuario_workflow set estado_balanceo_grupo=1 where idU_suario=" & id_usuario_afectado
        Dim Sql_delete_operation As String = "delete from wf_registro_asignacion_ruta where usuario_workflow_idu_suario=" & id_usuario_afectado
        Dim sql_insert_log As String = "Insert into wf_log_gestion_usuario (usuario_workflow_idU_suario,operacion,Id_usuario_wf_afectado,fecha_hora) values (" &
            id_usuario_operacion & ",'" & "ACTIVA BALANCEO'," & id_usuario_afectado & ",'" & fecha_trans & "')"
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Dim myTrans As MySqlTransaction
        myTrans = myConnection.BeginTransaction()
        myCommand.Connection = myConnection
        myCommand.Transaction = myTrans
        Dim paramter_resp As Integer = 0
        Try
            myCommand.CommandText = sql_update_estado
            paramter_resp = myCommand.ExecuteNonQuery()
            If paramter_resp = 0 Then
                myConnection.Close()
                Activa_usuario_workflow_balanceo_grupo = "Imposible actualizar estado balanceo " & sql_update_estado
                Exit Function
            End If
            myCommand.CommandText = sql_insert_log
            paramter_resp = myCommand.ExecuteNonQuery()
            If paramter_resp = 0 Then
                myTrans.Rollback()
                myConnection.Close()
                Activa_usuario_workflow_balanceo_grupo = "Imposible insertar registro log  " & sql_insert_log
                Exit Function
            End If
            If estado_asignacion = "YES" Then
                myCommand.CommandText = Sql_delete_operation
                paramter_resp = myCommand.ExecuteNonQuery()
                If paramter_resp = 0 Then
                    myTrans.Rollback()
                    myConnection.Close()
                    Activa_usuario_workflow_balanceo_grupo = "Imposible eliminar registro balanceo grupo  " & Sql_delete_operation
                    Exit Function
                End If
            End If
            myTrans.Commit()
            myConnection.Close()
            Activa_usuario_workflow_balanceo_grupo = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
                myConnection.Close()
                Activa_usuario_workflow_balanceo_grupo = "Error Actualizando  " & sql_update_estado
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Activa_usuario_workflow_balanceo_grupo = "An exception of type " & ex.GetType().ToString() &
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                End If
            End Try
            Activa_usuario_workflow_balanceo_grupo = "Error general funcio   Activa_usuario_workflow_balanceo_grupo  " & e.Message
            Exit Function
        End Try
    End Function
    Function Solicita_lista_usuarios_workflow_balanceo(ByVal paraMeter As String,
                                                       ByRef resultList As Object) As String
        '---------------------------------------------------------------------------
        'Funcion : Lista estructura de usuarios para balanceo
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'resultList            : Retorna lista de estructura de usuarios workflow
        '                      : disponibles para activar o desactivar para balanceo
        '                      : de cargas
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-08-17
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT uw.idU_suario,uw.estado_balanceo_grupo,uw.UTIL_ASIGNA_TAREA,uw.login_Usuario,uw.Nombre_usuario,uw.Cargo_Usuario,gw.Nombre_Grupo" &
            " FROM usuario_workflow as uw " &
            " inner join  grupos_workflow as gw on (gw.Id_Grupo= uw.Grupos_Workflow_Id_Grupo)" &
            " where (uw.login_Usuario like '%" & paraMeter & "%' or uw.Nombre_usuario like '%" & paraMeter & "%') and ESTADO_USUARIO=1 order by uw.Nombre_usuario"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("usuario_workflow")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_lista_usuarios_workflow_balanceo = "Funcion  Solicita_lista_usuarios_workflow_balanceo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                'Dim item As New table_boot_lista_usuario_workflow_balance
                'item.idU_suario = 0
                'item.estado_balanceo_grupo = 0
                'item.estado_asigna_tarea = 0
                'item.login_Usuario = ""
                'item.Nombre_usuario = ""
                'item.Cargo_Usuario = ""
                'item.Nombre_Grupo = ""
                'item.result = "YES"
                'resultList.Add(item)
                Solicita_lista_usuarios_workflow_balanceo = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim item As New row_usuario_workflow_blance
                    item.idU_suario = Datset.Tables(0).Rows(i).Item(0)
                    item.estado_balanceo_grupo = Datset.Tables(0).Rows(i).Item(1)
                    item.estado_asigna_tarea = Datset.Tables(0).Rows(i).Item(2)
                    item.login_Usuario = Datset.Tables(0).Rows(i).Item(3)
                    item.Nombre_usuario = Datset.Tables(0).Rows(i).Item(4)
                    item.Cargo_Usuario = Datset.Tables(0).Rows(i).Item(5)
                    item.Nombre_Grupo = Datset.Tables(0).Rows(i).Item(6)
                    resultList.Add(item)
                Next
                Solicita_lista_usuarios_workflow_balanceo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_usuarios_workflow_balanceo = "Inconsistencia general funcion Solicita_lista_usuarios_workflow_balanceo " & ex.Message
        End Try
    End Function
    Function Solicita_id_usuario_gestion_usuario_workflow(ByVal id_usuario_wf As Integer,
                                                          ByRef id_usuario_gestion As Integer) As String
        '------------------------------------------------------------------------------
        'Función : Solicita el  usuario de gestión relacionado al usuario workflow
        'con el parametro de identificación del usuario
        'id_usuario_wf : Indentificación del usuario workflow
        'Fecha : 2021-10-30
        'Ing : Miguel Angel Urueta Miranda
        '-------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT Relacion_Gestion FROM usuario_workflow" &
            " WHERE idU_suario=" & id_usuario_wf
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("usuario_workflow")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_usuario_gestion_usuario_workflow = "Funcion  Solicita_id_usuario_gestion_usuario_workflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_usuario_gestion_usuario_workflow = "Imposible encontrar los datos del usuario worklflow (" & id_usuario_wf & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    id_usuario_gestion = 0
                Else
                    id_usuario_gestion = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_id_usuario_gestion_usuario_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_usuario_gestion_usuario_workflow = "Inconsistencia general función Solicita_id_usuario_gestion_usuario_workflow " & ex.Message
        End Try
    End Function
    Function Solicita_nombre_cargo_usuario_workflow(ByVal id_usuario_wf As Integer,
                                                    ByRef nombre_usuario As String,
                                                    ByRef cargo_usuario As String) As String
        '-------------------------------------------------------------
        'Función : Solicita el nombre y el cargo del usuario workflow
        'con el parametro de identificación del usuario
        'id_usuario_wf : Indentificación del usuario workflow
        'Fecha : 2019-10-24
        'Ing : Miguel Angel Urueta Miranda
        '-------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT Nombre_Usuario,Cargo_Usuario FROM usuario_workflow" &
            " WHERE idU_suario=" & id_usuario_wf
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("usuario_workflow")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombre_cargo_usuario_workflow = "Funcion  Solicita_nombre_cargo_usuario_workflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_nombre_cargo_usuario_workflow = "Imposible encontrar los datos del usuario worklflow (" & id_usuario_wf & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    nombre_usuario = ""
                Else
                    nombre_usuario = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) Then
                    cargo_usuario = ""
                Else
                    cargo_usuario = Datset.Tables(0).Rows(0).Item(1)
                End If
                Solicita_nombre_cargo_usuario_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombre_cargo_usuario_workflow = "Inconsistencia general función Solicita_nombre_cargo_usuario_workflow " & ex.Message
        End Try
    End Function
    Function Solicita_caracterizacion_usuario_workflow(ByVal id_usuario_wf As Integer,
                                                       ByRef nombre_usuario As String,
                                                       ByRef cargo_usuario As String,
                                                       ByRef loguin_usuario As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita datos de caracterización usuario workflow
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_usuario_wf         : Representa la identificacion del usuario workflow
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'id_usuario_radicador  : Retorna la idnetificación del usuario radicador
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT Nombre_Usuario,Cargo_Usuario,login_Usuario FROM usuario_workflow" &
            " WHERE idU_suario=" & id_usuario_wf
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("usuario_workflow")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_caracterizacion_usuario_workflow = "Funcion  Solicita_caracterizacion_usuario_workflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_caracterizacion_usuario_workflow = "Imposible encontrar los datos del usuario worklflow (" & id_usuario_wf & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    nombre_usuario = ""
                Else
                    nombre_usuario = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) Then
                    cargo_usuario = ""
                Else
                    cargo_usuario = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) Then
                    loguin_usuario = ""
                Else
                    loguin_usuario = Datset.Tables(0).Rows(0).Item(2)
                End If
                Solicita_caracterizacion_usuario_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_caracterizacion_usuario_workflow = "Inconsistencia general función Solicita_caracterizacion_usuario_workflow " & ex.Message
        End Try
    End Function
    Function Valida_lista_usuarios_workflow_para_envio_tarea(ByVal page As Page) As String
        Try
            Dim Hidden_vi_reasigna As HtmlInputHidden = page.FindControl("Hidden_vi_reasigna")
            If Hidden_vi_reasigna Is Nothing Then
                Valida_lista_usuarios_workflow_para_envio_tarea = "Imposible encontrar el control (Hidden_vi_reasigna) "
                Exit Function
            End If
            Dim GridView_envia_usuario As GridView = page.FindControl("GridView_envia_usuario")
            If GridView_envia_usuario Is Nothing Then
                Valida_lista_usuarios_workflow_para_envio_tarea = "Imposible encontrar el control (GridView_envia_usuario) "
                Exit Function
            End If
            Dim titulo_label_lista_usuario_ruta As Label = page.FindControl("titulo_label_lista_usuario_ruta")
            If titulo_label_lista_usuario_ruta Is Nothing Then
                Valida_lista_usuarios_workflow_para_envio_tarea = "Imposible encontrar el control (titulo_label_lista_usuario_ruta) "
                Exit Function
            End If
            Dim Hidden_sel_actividad As HtmlInputHidden = page.FindControl("Hidden_sel_actividad")
            If Hidden_sel_actividad Is Nothing Then
                Valida_lista_usuarios_workflow_para_envio_tarea = "Imposible encontrar el control (Hidden_sel_actividad) "
                Exit Function
            End If
            Dim UpdateGeneral_lista_usuarios_ruta As UpdatePanel = page.FindControl("UpdateGeneral_lista_usuarios_ruta")
            If UpdateGeneral_lista_usuarios_ruta Is Nothing Then
                Valida_lista_usuarios_workflow_para_envio_tarea = "Imposible encontrar el control (UpdateGeneral_lista_usuarios_ruta) "
                Exit Function
            End If
            Dim ModalPopupExtender_edition_lista_usuarios_ruta As AjaxControlToolkit.ModalPopupExtender = page.FindControl("ModalPopupExtender_edition_lista_usuarios_ruta")
            If ModalPopupExtender_edition_lista_usuarios_ruta Is Nothing Then
                Valida_lista_usuarios_workflow_para_envio_tarea = "Imposible encontrar el control (ModalPopupExtender_edition_lista_usuarios_ruta) "
                Exit Function
            End If
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Or HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "-1" Then
                Valida_lista_usuarios_workflow_para_envio_tarea = "Debe seleccionar una tarea para listar los usuarios "
                Exit Function
            End If
            If HttpContext.Current.Session.Item("CAMBIO_USUARIO") = "0" Then
                Valida_lista_usuarios_workflow_para_envio_tarea = "El usuario no tiene permiso para enviar la tarea a otro usuario"
                Exit Function
            End If
            Dim Refclas_f As New Class_flujo_trabajo_workflow
            Dim Result As String = ""
            Result = Refclas_f.Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                           HttpContext.Current.Session.Item("Id_Usuario_Workflow"))
            If Result <> "YES" Then
                Valida_lista_usuarios_workflow_para_envio_tarea = Result
                Exit Function
            End If
            '-----------------------------------------------
            'Verifica la existencia de ruta abierta cerrada
            '-----------------------------------------------
            Dim estado_tramite_ruta As Integer = 0
            Dim tramite As String = ""
            Dim Refclas_workflow_rutas As New Class_worflow_rutas
            Result = Refclas_workflow_rutas.Solicita_etado_abierto_cerrado_ruta_tarea(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                      HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                      estado_tramite_ruta,
                                                                                      tramite)
            If Result <> "YES" Then
                Valida_lista_usuarios_workflow_para_envio_tarea = Result
                Exit Function
            End If
            If estado_tramite_ruta = 1 Then
                Valida_lista_usuarios_workflow_para_envio_tarea = "La tarea pertenece al tipo trámite (" & tramite & ") de ruta cerrada. Imposible enviar tarea a usuario"
                Exit Function
            End If
            Dim refclasgestion As New Classgestionrespuesta
            Result = refclasgestion.Verifica_respuesta_radicado_sin_respuesta(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                              HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"))
            If Result <> "YES" And Result <> "El trámite requiere de una confirmación de respuesta" _
                And Result <> "El trámite requiere de un radicado de respuesta" Then
                Valida_lista_usuarios_workflow_para_envio_tarea = Result
                Exit Function
            Else
                If Result = "El trámite requiere de una confirmación de respuesta" Or Result = "El trámite requiere de un radicado de respuesta" Then
                    HttpContext.Current.Session.Item("WF_ESTADO_RESPUESTA_TRAMITE_USUARIO") = 1
                    Hidden_vi_reasigna.Value = "1"
                Else
                    Hidden_vi_reasigna.Value = "2"
                    HttpContext.Current.Session.Item("WF_ESTADO_RESPUESTA_TRAMITE_USUARIO") = 2
                End If
            End If
            Dim Ref_class_usuario_workflow As New Class_usuario_workflow
            Result = Ref_class_usuario_workflow.Solicita_listado_usuarios_workflow_ruta(Val(HttpContext.Current.Session.Item("Id_Ruta_Workflow")),
                                                                                        1,
                                                                                        "",
                                                                                        GridView_envia_usuario,
                                                                                        titulo_label_lista_usuario_ruta,
                                                                                        Hidden_sel_actividad,
                                                                                        UpdateGeneral_lista_usuarios_ruta)
            If Result <> "YES" Then
                Valida_lista_usuarios_workflow_para_envio_tarea = Result
                Exit Function
            Else
                ModalPopupExtender_edition_lista_usuarios_ruta.Show()
                Valida_lista_usuarios_workflow_para_envio_tarea = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Valida_lista_usuarios_workflow_para_envio_tarea = "Inconsistencia general funcion Valida_lista_usuarios_workflow_para_envio_tarea " & ex.Message
        End Try
    End Function
    Function Solicita_listado_usuarios_workflow_ruta(ByVal id_ruta As Integer,
                                                     ByVal tipo_consulta As Integer,
                                                     ByVal valor_consulta As String,
                                                     ByRef grediview As GridView,
                                                     ByRef reflabel As Label,
                                                     ByRef hideselecion As HtmlInputHidden,
                                                     ByRef update As UpdatePanel) As String
        Try
            Dim Sql_consulta As String = ""
            If tipo_consulta = 1 Then
                Sql_consulta = ""
                Sql_consulta = "Select UW.IDU_SUARIO,GW.ID_ACTIVIDAD,UW.NOMBRE_USUARIO," &
                "UW.CARGO_USUARIO,GW.NOMBRE_GRUPO,UW.LOGIN_USUARIO from USUARIO_WORKFLOW as UW " &
                "Inner join GRUPOS_WORKFLOW as GW on " &
                "(GW.ID_GRUPO=UW.GRUPOS_WORKFLOW_ID_GRUPO) " &
                "WHERE UW.GRUPOS_WORKFLOW_RUTAS_WORKFLOW_ID_RUTA=" & id_ruta &
                " and ESTADO_USUARIO=1 and UTIL_ASIGNA_TAREA=1 ORDER BY UW.NOMBRE_USUARIO ASC"
            Else
                Sql_consulta = "Select UW.IDU_SUARIO,GW.ID_ACTIVIDAD,UW.NOMBRE_USUARIO," &
                "UW.CARGO_USUARIO,GW.NOMBRE_GRUPO,UW.LOGIN_USUARIO from USUARIO_WORKFLOW as UW " &
                "Inner join GRUPOS_WORKFLOW as GW on " &
                "(GW.ID_GRUPO=UW.GRUPOS_WORKFLOW_ID_GRUPO) " &
                " WHERE (UW.NOMBRE_USUARIO like '%" & valor_consulta & "%'" &
                " or UW.CARGO_USUARIO like '%" & valor_consulta & "%'" &
                " or UW.LOGIN_USUARIO like '%" & valor_consulta & "%'" &
                ") and  UW.GRUPOS_WORKFLOW_RUTAS_WORKFLOW_ID_RUTA=" & id_ruta & " and ESTADO_USUARIO=1 and UTIL_ASIGNA_TAREA=1 ORDER BY UW.NOMBRE_USUARIO ASC"
            End If
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("listado_actividades_workflow")
            Dim Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_listado_usuarios_workflow_ruta = "Error listando descripción tabla listado_actividades_workflow  " & Result
                Exit Function
            End If
            Datset.Tables(0).Columns.Add("DESTINO", GetType(String))
            If Datset.Tables(0).Rows.Count = 0 Then
                reflabel.Text = "0 usuario(s) "
                grediview.DataSource = Nothing
                hideselecion.Value = ""
                grediview.DataBind()
                update.Update()
                Solicita_listado_usuarios_workflow_ruta = "YES"
                Exit Function
            Else
                reflabel.Text = Datset.Tables(0).Rows.Count & " usuario(s) "
                grediview.DataSource = Datset
                hideselecion.Value = ""
                grediview.DataBind()
                update.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim divhtml_ As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fad fa-share-all")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent_envio_usuario_actividad(event,this);")
                    ahtml.Attributes.Add("title", "Enviar a (" & grediview.Rows(i).Cells(3).Text.ToString() & ")")
                    ahtml.Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(2).Text.ToString())
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn bg-info btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent_detalle_usuario(event,this);")
                    ahtml.Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    ihtml.Attributes.Add("class", "fad fa-user")
                    ahtml.Attributes.Add("title", "Actividad de usuario")
                    ahtml.Controls.Add(ihtml)
                    divhtml_.Controls.Add(ahtml)
                    divhtml_.Style.Add("display", "inline-flex")
                    divhtml.Style.Add("display", "inline-flex")
                    grediview.Rows(i).Cells(Datset.Tables(0).Columns.Count).Controls.Add(divhtml)
                    grediview.Rows(i).Cells(0).Controls.Add(divhtml_)
                    For z As Integer = 0 To grediview.Rows(i).Cells.Count - 2
                        If z > 0 Then
                            grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If
                    Next
                Next
                Solicita_listado_usuarios_workflow_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_listado_usuarios_workflow_ruta = "Inconsistencia general función Solicita_listado_actividades_ruta " & ex.Message
        End Try
    End Function
    Function Solicita_lista_usuario_workflow_flujo_trabajo_id_usuario_wf(ByVal id_usuario_wf_flujo As Integer,
                                                                         ByRef drop_list As DropDownList) As String
        Try
            drop_list.Items.Clear()
            Dim Parametro_Consulta As String = "SELECT idU_suario,Nombre_Usuario,Cargo_Usuario FROM usuario_workflow " &
           " where idU_suario=" & id_usuario_wf_flujo & " and ESTADO_USUARIO=1"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_lista_usuario_workflow_flujo_trabajo_id_usuario_wf = "Función Solicita_lista_usuario_workflow_flujo_trabajo_id_usuario_wf dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                drop_list.Items.Clear()
                Solicita_lista_usuario_workflow_flujo_trabajo_id_usuario_wf = "YES"
                Exit Function
            Else
                drop_list.Items.Clear()
                Dim ilist As New ListItem
                ilist.Value = 0
                ilist.Text = ""
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilist = New ListItem
                    ilist.Value = Datset.Tables(0).Rows(i).Item(0)
                    ilist.Text = Datset.Tables(0).Rows(i).Item(1) & "(" & Datset.Tables(0).Rows(i).Item(2) & ")"
                    drop_list.Items.Add(ilist)
                Next
                Solicita_lista_usuario_workflow_flujo_trabajo_id_usuario_wf = "YES"
                Exit Function
            End If
            Solicita_lista_usuario_workflow_flujo_trabajo_id_usuario_wf = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_lista_usuario_workflow_flujo_trabajo_id_usuario_wf = "Inconsistencias general funcion Solicita_lista_usuario_workflow_flujo_trabajo_id_usuario_wf " & ex.Message
        End Try
    End Function
    Function Solicita_lista_usuario_workflow_flujo_trabajo_id_grupo_worikflow(ByVal id_grupo_wf As Integer,
                                                                              ByVal agrega_item_selecion_usuario As Integer,
                                                                              ByRef drop_list As DropDownList) As String
        Try
            drop_list.Items.Clear()
            Dim Parametro_Consulta As String = "SELECT idU_suario,Nombre_Usuario,Cargo_Usuario FROM usuario_workflow " &
           " where Grupos_Workflow_Id_Grupo=" & id_grupo_wf & " and ESTADO_USUARIO=1"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("usuario_workflow")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_lista_usuario_workflow_flujo_trabajo_id_grupo_worikflow = "Función Solicita_lista_usuario_workflow_flujo_trabajo_id_usuario_wf dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                drop_list.Items.Clear()
                Solicita_lista_usuario_workflow_flujo_trabajo_id_grupo_worikflow = "YES"
                Exit Function
            Else
                drop_list.Items.Clear()
                Dim ilist As New ListItem
                ilist.Value = 0
                ilist.Text = ""
                If agrega_item_selecion_usuario = 1 Then
                    drop_list.Items.Add(ilist)
                End If
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilist = New ListItem
                    ilist.Value = Datset.Tables(0).Rows(i).Item(0)
                    ilist.Text = Datset.Tables(0).Rows(i).Item(1) & "(" & Datset.Tables(0).Rows(i).Item(2) & ")"
                    drop_list.Items.Add(ilist)
                Next
                Solicita_lista_usuario_workflow_flujo_trabajo_id_grupo_worikflow = "YES"
                Exit Function
            End If
            Solicita_lista_usuario_workflow_flujo_trabajo_id_grupo_worikflow = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_lista_usuario_workflow_flujo_trabajo_id_grupo_worikflow = "Inconsistencias general funcion Solicita_lista_usuario_workflow_flujo_trabajo_id_grupo_worikflow " & ex.Message
        End Try
    End Function
    Function Solicita_lista_usuario_workflow(ByVal agrega_item_selecion_usuario As Integer,
                                             ByRef drop_list As DropDownList) As String
        Try
            drop_list.Items.Clear()
            Dim Parametro_Consulta As String = "SELECT idU_suario,Nombre_Usuario,Cargo_Usuario FROM usuario_workflow " &
           " where  ESTADO_USUARIO=1 order by Nombre_Usuario"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("usuario_workflow")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_lista_usuario_workflow = "Función Solicita_lista_usuario_workflow dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                drop_list.Items.Clear()
                Solicita_lista_usuario_workflow = "YES"
                Exit Function
            Else
                drop_list.Items.Clear()
                Dim ilist As New ListItem
                ilist.Value = 0
                ilist.Text = ""
                If agrega_item_selecion_usuario = 1 Then
                    drop_list.Items.Add(ilist)
                End If
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilist = New ListItem
                    ilist.Value = Datset.Tables(0).Rows(i).Item(0)
                    ilist.Text = Datset.Tables(0).Rows(i).Item(1) & "(" & Datset.Tables(0).Rows(i).Item(2) & ")"
                    drop_list.Items.Add(ilist)
                Next
                Solicita_lista_usuario_workflow = "YES"
                Exit Function
            End If
            Solicita_lista_usuario_workflow = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_lista_usuario_workflow = "Inconsistencias general funcion Solicita_lista_usuario_workflow " & ex.Message
        End Try
    End Function
    Function Solicita_id_grupo_usuario_workflow(ByVal id_usuario_workflow As Integer,
                                                ByRef id_grupo_wf As Integer) As String
        Try
            Dim Parametro_Consulta As String = "SELECT Grupos_Workflow_Id_Grupo FROM usuario_workflow " &
            " where idU_suario=" & id_usuario_workflow
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("usuario_workflow")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_grupo_usuario_workflow = "Función Solicita_id_grupo_usuario_workflow dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_grupo_usuario_workflow = "Imposible encontrar el grupo del codigo usuario workflow  (" & id_usuario_workflow & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    Solicita_id_grupo_usuario_workflow = "El usuario workflow  (" & id_usuario_workflow & "), no tiene relacionado un grupo workflow"
                    Exit Function
                Else
                    id_grupo_wf = Datset.Tables(0).Rows(0).Item(0)
                    Solicita_id_grupo_usuario_workflow = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Solicita_id_grupo_usuario_workflow = "Inconsistencia general funcion Solicita_id_grupo_usuario_workflow " & ex.Message
        End Try
    End Function
    Function Solicita_usuarios_relacionados_actividad_flujo(ByVal inserta_null As Integer,
                                                            ByVal id_actividad_flujo As Integer,
                                                            ByRef Class_service_ilist_drowlist As IList(Of Class_service_ilist_drowlist)) As String
        '---------------------------------------------------------------------------
        'Funcion : Lista usuarios relacionados a actividad flujo
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'inserta_null           : Determina si inserta la primera fila vacia
        'id_actividad_flujo     : id actividad flujo 
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'drowp_list             : Retorna lista de usuarios en formato drowplis
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                  : 2024-12-05
        'Elabora                : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_grupos_workflow As New Class_grupos_workflow
            Dim id_grupo_workflow As Integer = 0
            Dim id_actividad_workflow As Integer = 0
            Dim struregistro_actividaes_flujos_trabajo As struregistro_actividaes_flujos_trabajo = Nothing
            Dim Class_wf_registro_actividaes_flujos_trabajo As New Class_wf_registro_actividaes_flujos_trabajo
            If id_actividad_flujo = 0 Or id_actividad_flujo = -1 Then
                Solicita_usuarios_relacionados_actividad_flujo = "YES"
                Exit Function
            End If
            Result = Class_wf_registro_actividaes_flujos_trabajo.Solicita_estructura_actividad_flujo_trabajo(id_actividad_flujo,
                                                                                                             struregistro_actividaes_flujos_trabajo)
            If Result <> "YES" Then
                Solicita_usuarios_relacionados_actividad_flujo = Result
                Exit Function
            End If
            Result = Class_grupos_workflow.Solicita_id_grupo_actividad_workflow(struregistro_actividaes_flujos_trabajo.listado_actividades_workflow_Id_Actividad,
                                                                                id_grupo_workflow)
            If Result <> "YES" Then
                Solicita_usuarios_relacionados_actividad_flujo = Result
                Exit Function
            End If
            Result = Solicita_class_usuarios_activos_relacionado_actividad_grupo(inserta_null,
                                                                                 id_grupo_workflow,
                                                                                 Class_service_ilist_drowlist)
            If Result <> "YES" Then
                Solicita_usuarios_relacionados_actividad_flujo = Result
                Exit Function
            Else
                Solicita_usuarios_relacionados_actividad_flujo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_usuarios_relacionados_actividad_flujo = "Inconsistencia general función Solicita_usuarios_relacionados_actividad_flujo " & ex.Message
        End Try
    End Function
    Function Solicita_usuarios_relacionados_actividad_flujo(ByVal inserta_null As Integer,
                                                            ByVal id_actividad_flujo As Integer,
                                                            ByRef drowp_list As DropDownList,
                                                            ByRef update As UpdatePanel) As String
        '---------------------------------------------------------------------------
        'Funcion : Lista usuarios relacionados a actividad flujo
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'inserta_null           : Determina si inserta la primera fila vacia
        'id_actividad_flujo     : id actividad flujo 
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'drowp_list             : Retorna lista de usuarios en formato drowplis
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                  : 2023-05-25
        'Elabora                : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_grupos_workflow As New Class_grupos_workflow
            Dim id_grupo_workflow As Integer = 0
            Dim id_actividad_workflow As Integer = 0
            Dim struregistro_actividaes_flujos_trabajo As struregistro_actividaes_flujos_trabajo = Nothing
            Dim Class_wf_registro_actividaes_flujos_trabajo As New Class_wf_registro_actividaes_flujos_trabajo
            drowp_list.Items.Clear()
            update.Update()
            If id_actividad_flujo = 0 Or id_actividad_flujo = -1 Then
                Solicita_usuarios_relacionados_actividad_flujo = "YES"
                Exit Function
            End If
            Result = Class_wf_registro_actividaes_flujos_trabajo.Solicita_estructura_actividad_flujo_trabajo(id_actividad_flujo,
                                                                                                             struregistro_actividaes_flujos_trabajo)
            If Result <> "YES" Then
                Solicita_usuarios_relacionados_actividad_flujo = Result
                Exit Function
            End If
            Result = Class_grupos_workflow.Solicita_id_grupo_actividad_workflow(struregistro_actividaes_flujos_trabajo.listado_actividades_workflow_Id_Actividad,
                                                                                id_grupo_workflow)
            If Result <> "YES" Then
                Solicita_usuarios_relacionados_actividad_flujo = Result
                Exit Function
            End If
            Result = Solicita_usuarios_activos_relacionado_actividad_grupo_drowlis(inserta_null,
                                                                                   id_grupo_workflow,
                                                                                   drowp_list,
                                                                                   update)
            If Result <> "YES" Then
                Solicita_usuarios_relacionados_actividad_flujo = Result
                Exit Function
            Else
                Solicita_usuarios_relacionados_actividad_flujo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_usuarios_relacionados_actividad_flujo = "Inconsistencia general función Solicita_usuarios_relacionados_actividad_flujo " & ex.Message
        End Try
    End Function
    Function Solicita_class_usuarios_activos_relacionado_actividad_grupo(ByVal inserta_null As Integer,
                                                                         ByVal id_grupo_workflow As Integer,
                                                                         ByRef Class_service_ilist_drowlist As IList(Of Class_service_ilist_drowlist)) As String
        '---------------------------------------------------------------------------
        'Funcion : Lista usuarios relacionados a actividad grupo workflow
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'inserta_null           : Determina si inserta la primera fila vacia
        'id_grupo_workflow      : grupo workflow que agrupa el usuario
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'drowp_list             : Retorna lista estructura en formato drowplis
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-12-05
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT idU_suario,Nombre_Usuario,Cargo_Usuario FROM  usuario_workflow as uw " &
                          " where uw.Grupos_Workflow_Id_Grupo=" & id_grupo_workflow & " and ESTADO_USUARIO=1 order by Nombre_Usuario,Cargo_Usuario "
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_class_usuarios_activos_relacionado_actividad_grupo = "Error funcion Solicita_class_usuarios_activos_relacionado_actividad_grupo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_class_usuarios_activos_relacionado_actividad_grupo = "YES"
                Exit Function
            Else
                Dim Item As New Class_service_ilist_drowlist
                Item.value_campo = ""
                Item.id_value = "0"
                If inserta_null = 1 Then
                    Class_service_ilist_drowlist.Add(Item)
                End If
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Item = New Class_service_ilist_drowlist
                    Item.value_campo = Datset.Tables(0).Rows(i).Item(1) & "(" & Datset.Tables(0).Rows(i).Item(2) & ")"
                    Item.id_value = Datset.Tables(0).Rows(i).Item(0)
                    Class_service_ilist_drowlist.Add(Item)
                Next
                Solicita_class_usuarios_activos_relacionado_actividad_grupo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_class_usuarios_activos_relacionado_actividad_grupo = "Inconsistencia general funcion Solicita_class_usuarios_activos_relacionado_actividad_grupo " & ex.Message
        End Try
    End Function
    Function Solicita_usuarios_activos_relacionado_actividad_grupo_drowlis(ByVal inserta_null As Integer,
                                                                           ByVal id_grupo_workflow As Integer,
                                                                           ByRef drowp_list As DropDownList,
                                                                           ByRef update As UpdatePanel) As String


        '---------------------------------------------------------------------------
        'Funcion : Lista usuarios relacionados a actividad grupo workflow
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'inserta_null           : Determina si inserta la primera fila vacia
        'id_grupo_workflow      : grupo workflow que agrupa el usuario
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'drowp_list             : Retorna lista estructura en formato drowplis
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-05-24
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT idU_suario,Nombre_Usuario,Cargo_Usuario FROM  usuario_workflow as uw " &
                    " where uw.Grupos_Workflow_Id_Grupo=" & id_grupo_workflow & " and ESTADO_USUARIO=1 order by Nombre_Usuario,Cargo_Usuario "
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_usuarios_activos_relacionado_actividad_grupo_drowlis = "Error funcion Solicita_usuarios_activos_relacionado_actividad_grupo_drowlis " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                drowp_list.Items.Clear()
                update.Update()
                Solicita_usuarios_activos_relacionado_actividad_grupo_drowlis = "YES"
                Exit Function
            Else
                update.Update()
                drowp_list.Items.Clear()
                Dim ilist As New ListItem
                If inserta_null = 1 Then
                    ilist.Value = -1
                    ilist.Text = ""
                    drowp_list.Items.Add(ilist)
                End If
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilist = New ListItem
                    ilist.Value = Datset.Tables(0).Rows(i).Item(0)
                    ilist.Text = Datset.Tables(0).Rows(i).Item(1) & "(" & Datset.Tables(0).Rows(i).Item(2) & ")"
                    drowp_list.Items.Add(ilist)
                Next
                Solicita_usuarios_activos_relacionado_actividad_grupo_drowlis = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_usuarios_activos_relacionado_actividad_grupo_drowlis = "Inconsistencia general funcion Solicita_usuarios_activos_relacionado_actividad_grupo_drowlis " & ex.Message
        End Try
    End Function
    Function Solicita_usuarios_activos_relacionado_actividad_grupo(ByVal inserta_null As Integer,
                                                                   ByVal id_grupo_workflow As Integer,
                                                                   ByRef stru_list_usuarios As Object) As String


        '---------------------------------------------------------------------------
        'Funcion : Lista usuarios relacionados a actividad grupo workflow
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'inserta_null           : Determina si inserta la primera fila vacia
        'id_grupo_workflow      : grupo workflow que agrupa el usuario
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'stru_list_actividades  : Retorna la estructura de usuarios
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-04-29
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT idU_suario,Nombre_Usuario,Cargo_Usuario FROM  usuario_workflow as uw " &
                    " where uw.Grupos_Workflow_Id_Grupo=" & id_grupo_workflow & " and ESTADO_USUARIO=1 and UTIL_ASIGNA_TAREA=1 order by Nombre_Usuario,Cargo_Usuario "
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_usuarios_activos_relacionado_actividad_grupo = "Error Consultando en tabla " & "LISTADO_ACTIVIDADES_WORKFLOW" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Dim item As New stru_list_usuarios
                item.id_actividad = -1
                item.nombre_actividad = ""
                item.result = "YES"
                stru_list_usuarios.Add(item)
                Solicita_usuarios_activos_relacionado_actividad_grupo = "YES"
                Exit Function
            Else
                If inserta_null = 1 Then
                    Dim item As New stru_list_usuarios
                    item.id_actividad = -1
                    item.nombre_actividad = ""
                    item.result = "YES"
                    stru_list_usuarios.Add(item)
                End If
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim item As New stru_list_usuarios
                    item.id_actividad = Datset.Tables(0).Rows(i).Item(0)
                    item.nombre_actividad = Datset.Tables(0).Rows(i).Item(1) & "(" & Datset.Tables(0).Rows(i).Item(2) & ")"
                    item.result = "YES"
                    stru_list_usuarios.Add(item)
                Next
                Solicita_usuarios_activos_relacionado_actividad_grupo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_usuarios_activos_relacionado_actividad_grupo = "Inconsistencia general funcion Solicita_usuarios_activos_relacionado_actividad_grupo " & ex.Message
        End Try
    End Function
    Function Retorna_relacion_id_usuario_gestion_con_usuario_wrokflow(ByVal id_usuario_workflow As Integer,
                                                                     ByRef id_usuario_gestion As Integer) As String
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT Relacion_Gestion FROM usuario_workflow " &
            " WHERE idU_suario='" & id_usuario_workflow & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("usuario_workflow")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                Retorna_relacion_id_usuario_gestion_con_usuario_wrokflow = "Función Retorna_relacion_id_usuario_gestion_con_usuario_wrokflow dice : error de conexión " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_relacion_id_usuario_gestion_con_usuario_wrokflow = "Imposible encontrar el el usuario workflow (" & id_usuario_workflow & ") contacte al administrador"
                Exit Function
            Else
                id_usuario_gestion = Datset.Tables(0).Rows(0).Item(0)
                If id_usuario_gestion = 0 Then
                    Retorna_relacion_id_usuario_gestion_con_usuario_wrokflow = "Imposible encontrar la relación del usuario workflow (" & id_usuario_workflow & ") con el usuario de gestión,  contacte al administrador para crear la relación"
                    Exit Function
                End If
                Retorna_relacion_id_usuario_gestion_con_usuario_wrokflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_relacion_id_usuario_gestion_con_usuario_wrokflow = "Inconsistencia general función Retorna_relacion_id_usuario_gestion_con_usuario_wrokflow " & ex.Message
        End Try
    End Function
    Function Retorna_id_usuario_workflow_usuario_gestion(ByVal id_usuario_gestion As Integer,
    ByRef id_usuario_workflow As Integer, ByRef id_grupo_workflow As Integer, ByRef nombre_usuario As String) As String
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT idU_suario,ESTADO_USUARIO,Nombre_Usuario,Grupos_Workflow_Id_Grupo FROM usuario_workflow " &
            " WHERE Relacion_Gestion='" & id_usuario_gestion & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("usuario_workflow")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                Retorna_id_usuario_workflow_usuario_gestion = "Función Retorna_id_usuario_workflow_usuario_gestion dice error de conexion o consultando " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_usuario_workflow_usuario_gestion = "Imposible encontrar el usuario workflow para el usuario de gestión " & id_usuario_gestion & " contacte al administrador para crear la relación"
                Exit Function
            Else
                Dim estado As Integer = 0
                id_usuario_workflow = Datset.Tables(0).Rows(0).Item(0)
                estado = Datset.Tables(0).Rows(0).Item(1)
                nombre_usuario = Datset.Tables(0).Rows(0).Item(2)
                If Datset.Tables(0).Rows(0).IsNull(3) = True Then
                    id_grupo_workflow = 0
                Else
                    id_grupo_workflow = Datset.Tables(0).Rows(0).Item(3)
                End If
                If estado <> 1 Then
                    Retorna_id_usuario_workflow_usuario_gestion = "El usuario workflow " & nombre_usuario & " esta inactivo imposible asignar "
                    Exit Function
                Else
                    Retorna_id_usuario_workflow_usuario_gestion = "YES"
                    Exit Function
                End If
                If id_grupo_workflow = 0 Then
                    Retorna_id_usuario_workflow_usuario_gestion = "El usuario workflow " & nombre_usuario & " no tiene grupo asignado imposible asignar "
                    Exit Function
                Else
                    Retorna_id_usuario_workflow_usuario_gestion = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Retorna_id_usuario_workflow_usuario_gestion = "Inconsistencia general función Retorna_id_usuario_workflow_usuario_gestion " & ex.Message()
        End Try
    End Function
End Class
