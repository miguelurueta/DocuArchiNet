Imports System.Math
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient
Imports Microsoft.VisualBasic
Imports System
Imports System.Text
Imports System.CodeDom.Compiler
Imports System.Reflection
Imports System.IO
Imports System.Collections.Specialized
Imports Neodynamic.WebControls.ImageDraw

Public Class ClassWorkflowUsuario
    Function Solicita_usuario_activos_relacionado_actividad_grupo_sin_asiginacion_ruta(ByVal id_actividad As Integer,
                                                                                       ByRef id_usuario_wf As Integer) As String

        '---------------------------------------------------------------------------------------------------------------
        '---------
        'Funcion :
        '---------
        'Solicita usuarios activos relacionados a un gupo workflow y una actividad workflow sin registro de asignacion
        '
        '-----------
        'Parametros:
        '-----------
        'id_actividad - identificación de la actividad workflow -  id_actividad
        '-----------
        'Retorno   ;
        '----------
        'id_usuaario_wf- retorna usuario sin registro
        '
        '----------
        'Fecha     : 2022-09-07
        '----------
        '----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        '------------------------------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "select uw.idu_suario from  usuario_workflow as uw  " &
                      "inner join grupos_workflow as gw on (uw.Grupos_Workflow_Id_Grupo=gw.Id_Grupo and  gw.id_actividad=" & id_actividad & ")" &
                       " where  uw.estado_balanceo_grupo=1 and  uw.idu_suario  not in (select wr.usuario_workflow_idU_suario from wf_registro_asignacion_ruta " &
                       "as wr where wr.listado_actividades_workflow_Id_Actividad=" & id_actividad & ")"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("usuario_workflow")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_usuario_activos_relacionado_actividad_grupo_sin_asiginacion_ruta = "Función Solicita_usuario_activos_relacionado_actividad_grupo_sin_asiginacion_ruta dice :   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_usuario_wf = 0
                Solicita_usuario_activos_relacionado_actividad_grupo_sin_asiginacion_ruta = "YES"
                Exit Function
            Else
                id_usuario_wf = Datset.Tables(0).Rows(0).Item(0)
                Solicita_usuario_activos_relacionado_actividad_grupo_sin_asiginacion_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_usuario_activos_relacionado_actividad_grupo_sin_asiginacion_ruta = "inconsistencia general funcion Solicita_usuario_activos_relacionado_actividad_grupo_sin_asiginacion_ruta " & ex.Message
        End Try
    End Function
    Function Solicita_estado_balanceo_ruta_usuario_workflow(ByVal id_usuario_workflow As Integer,
                                                            ByRef estado_balanceo_grupo As Integer) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita estado balanceo ruta usuario workflow
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        '
        'id_usuario_workflow   : Id usuario workflow de balanceo ruta
        'estado_balanceo_grupo : Representa el estado de balanceo del usuario
        '                        
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-09-29
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT  estado_balanceo_grupo " &
              " from usuario_workflow  " &
              " where idU_suario= " & id_usuario_workflow
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("usuario_workflow")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_balanceo_ruta_usuario_workflow = "Función Solicita_estado_balanceo_ruta_usuario_workflow dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_balanceo_grupo = 0
                Solicita_estado_balanceo_ruta_usuario_workflow = "YES"
                Exit Function
            Else
                estado_balanceo_grupo = Datset.Tables(0).Rows(0).Item(0)
                Solicita_estado_balanceo_ruta_usuario_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_balanceo_ruta_usuario_workflow = "Inconsistencia general funcion Solicita_estado_balanceo_ruta_usuario_workflow (" & ex.Message & ")"
        End Try
    End Function
    Function Solicita_usuario_activos_relacionado_actividad_grupo_sin_asiginacion_flujo(ByVal id_registro_actividad_flujo As Integer,
                                                                                        ByVal id_actividad As Integer,
                                                                                        ByRef id_usuario_wf As Integer) As String

        '---------------------------------------------------------------------------------------------------------------
        '---------
        'Funcion :
        '---------
        'Solicita usuarios activos relacionados a un gupo workflow y una actividad workflow sin registro de asignacion
        'en el flujo de trabajo
        '-----------
        'Parametros:
        '-----------
        'id_actividad - identificación de la actividad workflow ,  id_registro_actividad_flujo - registra la identificacion
        'de la actividad en el flujo de trabajo
        '-----------
        'Retorno   ;
        '----------
        'id_usuaario_wf- retorna usuario sin registro
        '
        '----------
        'Fecha     : 2022-09-08
        '----------
        '----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        '------------------------------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "select uw.idu_suario from  usuario_workflow as uw  " &
                      "inner join grupos_workflow as gw on (uw.Grupos_Workflow_Id_Grupo=gw.Id_Grupo and  gw.id_actividad=" & id_actividad & ")" &
                       " where  uw.estado_balanceo_grupo=1 and  uw.idu_suario  not in (select wr.usuario_workflow_idU_suario from wf_registro_asignacion_flujo " &
                       "as wr where wr.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=" & id_registro_actividad_flujo & ")"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("usuario_workflow")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_usuario_activos_relacionado_actividad_grupo_sin_asiginacion_flujo = "Función Solicita_usuario_activos_relacionado_actividad_grupo_sin_asiginacion_flujo dice :   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_usuario_wf = 0
                Solicita_usuario_activos_relacionado_actividad_grupo_sin_asiginacion_flujo = "YES"
                Exit Function
            Else
                id_usuario_wf = Datset.Tables(0).Rows(0).Item(0)
                Solicita_usuario_activos_relacionado_actividad_grupo_sin_asiginacion_flujo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_usuario_activos_relacionado_actividad_grupo_sin_asiginacion_flujo = "inconsistencia general funcion Solicita_usuario_activos_relacionado_actividad_grupo_sin_asiginacion_flujo " & ex.Message
        End Try
    End Function
    Function Solicita_usuarios_activos_relacionado_actividad_grupo(ByVal id_actividad As Integer,
                                                                   ByRef id_usuaario_wf As Integer) As String

        '---------------------------------------------------------------------------------------------------------------
        '---------
        'Funcion :
        '---------
        'Solicita usuarios activos relacionados a un gupo workflow y una actividad workflow
        '
        '-----------
        'Parametros:
        '-----------
        'id_actividad - identificación de la actividad workflow -  id_actividad
        '-----------
        'Retorno   ;
        '----------
        'id_usuaario_wf- retorna usuario activo relacionado a la tarea
        '
        '----------
        'Fecha     : 2022-09-07
        '----------
        '----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        '------------------------------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "Select uw.idu_suario from  usuario_workflow as uw  " &
                  "inner join grupos_workflow as gw on (uw.Grupos_Workflow_Id_Grupo=gw.Id_Grupo And  gw.id_actividad=" & id_actividad & ")" &
                   " WHERE uw.estado_balanceo_grupo=1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("usuario_workflow")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_usuarios_activos_relacionado_actividad_grupo = "Función Solicita_usuarios_activos_relacionado_actividad_grupo dice :   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_usuaario_wf = 0
                Solicita_usuarios_activos_relacionado_actividad_grupo = "YES"
                Exit Function
            Else
                id_usuaario_wf = Datset.Tables(0).Rows(0).Item(0)
                Solicita_usuarios_activos_relacionado_actividad_grupo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_usuarios_activos_relacionado_actividad_grupo = "Inconsistencia general funcion Solicita_usuarios_activos_relacionado_actividad_grupo " & ex.Message
        End Try
    End Function
    Function Cambiar_Contraseña_Wf(ByVal Pawsuno As String,
                                   ByVal paswdos As String) As String
        Try
            '*****************************************************
            'Verifica que los campos contraseña no esten vacios
            '*****************************************************
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Dim Result As String = ""
            If Pawsuno = "" Then
                Cambiar_Contraseña_Wf = "La primera contraseña debe informarse"
                Exit Function
            End If
            If paswdos = "" Then
                Cambiar_Contraseña_Wf = "La segunda contraseña debe informarse"
                Exit Function
            End If
            '******************************************************
            'La contraseña debe tener mas de ocho caracteres 
            '*****************************************************
            If Pawsuno.Length < 8 Then
                Cambiar_Contraseña_Wf = "La contraseña debe tener mínimo 8 caracteres"
                Exit Function
            End If
            '******************************************************
            'Compara las contraseña entre minusculas y mayusculas
            '******************************************************
            Dim compara As Integer = -2
            Dim Srcomuno As String = Pawsuno
            Dim Srcomdos As String = paswdos
            compara = StrComp(Srcomuno, Srcomdos,
             CompareMethod.Binary)
            If compara = 0 Then
            Else
                Cambiar_Contraseña_Wf = "Las contraseñas no coinciden, tenga en cuenta que el sistema diferencia entre minúsculas y mayusculas"
                Exit Function
            End If
            '******************************************************
            'Encriptacion de contraseñas
            '******************************************************
            Dim Contraseña_Encript As String = Pawsuno
            Result = Encrip_Value(Contraseña_Encript)
            If Result <> "YES" Then
                Cambiar_Contraseña_Wf = "Imposible Encriptar la contraseña " & Result
                Exit Function
            End If
            '*******************************************************
            'Actualizacion de la base de datos
            '*******************************************************
            Dim Sqlupdate As String = "Update usuario_workflow   " &
                "set Pasword_Usuario='" & Pawsuno & "'" &
                ", pasw_encript='" & Contraseña_Encript & "'" &
                " where idU_suario=" & HttpContext.Current.Session.Item("Id_Usuario_Workflow")
            Dim Resultado_Insertar As String = ref.SELECTION_INSERT_COMMAND(Sqlupdate)
            If Resultado_Insertar <> "YES" Then
                Cambiar_Contraseña_Wf = "Funcion update Error : " & Resultado_Insertar
                Exit Function
            End If
            Cambiar_Contraseña_Wf = "YES"
        Catch ex As Exception
            Cambiar_Contraseña_Wf = "Inconsistencia General Funcion Cambiar_Contraseña_Wf " & ex.Message
        End Try
    End Function
    Public Function S_Actualizacion_Intervalo_Alarma(
                                                     ByVal Id_user As String,
                                                     ByRef Intervalo As String) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Resultado_Insertar As String = ""
            Dim SqlInsert As String = "UPDATE Intervalo_Alarmas_usuario  SET INTERVALO=" & Intervalo _
            & " WHERE Usuario_Workflow_idu_suario=" & Id_user
            Resultado_Insertar = ref.SELECTION_INSERT_COMMAND(SqlInsert)
            If Resultado_Insertar = "YES" Then
                HttpContext.Current.Session.Item("Parametro_Intervalo_Alarma") = Intervalo
                'Ref_Conect.CONEXION_MYSQL_C.Close()
                S_Actualizacion_Intervalo_Alarma = Resultado_Insertar
                Return S_Actualizacion_Intervalo_Alarma
            Else
                'Ref_Conect.CONEXION_MYSQL_C.Close()
                S_Actualizacion_Intervalo_Alarma = Resultado_Insertar
                Return S_Actualizacion_Intervalo_Alarma
            End If
        Catch ex As Exception
            S_Actualizacion_Intervalo_Alarma = ex.Message
        End Try
    End Function
    Function S_Insercion_Intervalo_Alarma(ByVal id_usuario As Integer,
                                          ByVal login_usuario As String,
                                          ByVal Intervalo As String) As String
        '-------------------------------------------------------
        'Funcion: Agrega configuración de intervalo de alarma
        'al usuario si este no tiene.
        'Fecha : 2013-12-19
        'Ing Miguel Angel Urueta Miranda
        '-------------------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Resultado_Insertar As String = ""
            Dim SqlInsert As String = "Insert Into Intervalo_Alarmas_usuario(Usuario_Workflow_idu_suario,LOGIN_USUARIO,INTERVALO) VALUES (" _
            & id_usuario & ",'" & login_usuario & "'," & Intervalo & ")"
            Resultado_Insertar = ref.SELECTION_INSERT_COMMAND(SqlInsert)
            If Resultado_Insertar = "YES" Then
                HttpContext.Current.Session.Item("Parametro_Intervalo_Alarma") = Intervalo
                S_Insercion_Intervalo_Alarma = Resultado_Insertar
                Return S_Insercion_Intervalo_Alarma
            Else
                S_Insercion_Intervalo_Alarma = Resultado_Insertar
                Return S_Insercion_Intervalo_Alarma
            End If
        Catch ex As Exception
            S_Insercion_Intervalo_Alarma = ex.Message
        End Try
    End Function
    Function Leer_Datos_Configuracion_Usuario(ByRef Numero_Actividades As String,
                                              ByRef Fecha_Ini As String,
                                              ByRef Fecha_Fin As String,
                                              ByVal Id_Usuario As String) As String
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "Select Numero_Tarea_Lista,Fecha_Ini_Lista,Fecha_Fin_Lista" &
            " from configuracion_usuario where " &
            "USUARIO_WORKFLOW_IDU_SUARIO=" & Id_Usuario
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Leer_Datos_Configuracion_Usuario = "Error Consultando en tabla " & "USUARIO_WORKFLOW_IDU_SUARIO" & Result
                Exit Function
            End If
            If Datset Is Nothing Or Datset.Tables.Count = 0 Then
                Leer_Datos_Configuracion_Usuario = "La función Leer_Datos_Configuracion_Usuario no encontro la tabla 0 "
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Numero_Actividades = ""
                Fecha_Ini = ""
                Fecha_Fin = ""
                Leer_Datos_Configuracion_Usuario = "YES"
                Exit Function
            Else
                Dim Tempovalor As Object = Datset.Tables(0).Rows(0).Item(0)
                If IsDBNull(Tempovalor) Then
                    Numero_Actividades = "0"
                Else
                    Numero_Actividades = Tempovalor
                End If
                Tempovalor = Datset.Tables(0).Rows(0).Item(1)
                If IsDBNull(Tempovalor) Then
                    Fecha_Ini = ""
                Else
                    Fecha_Ini = Tempovalor

                End If
                Tempovalor = Datset.Tables(0).Rows(0).Item(2)
                If IsDBNull(Tempovalor) Then
                    Fecha_Fin = ""
                Else
                    Fecha_Fin = Tempovalor

                End If

                If Numero_Actividades = "0" Then
                    Numero_Actividades = ""
                End If

                Leer_Datos_Configuracion_Usuario = "YES"
            End If
        Catch ex As Exception
            Leer_Datos_Configuracion_Usuario = "Error Consultando datos configuracion usuario" & ex.Message
        End Try
    End Function
    Function Intervalo_Alarma_Usuario() As String
        '*********************************************************
        'Function :  Solicita Intervalo Alarma Usuario
        'Fecha    : 2010-06-08
        'Ing      : Miguel Angel Urueta Miranda
        'Proced   : Solicta el itervalo de jecucion de alarmas
        'Parameter: Id uaurio, Parametro de confirmacion
        'Actualización 2013-12-18 se adapto la conexión para sitio
        'web gestor documental 
        '**********************************************************

        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Dim Parametro_Consulta As String = "select INTERVALO from INTERVALO_ALARMAS_USUARIO " &
            "WHERE USUARIO_WORKFLOW_IDU_SUARIO=" & HttpContext.Current.Session.Item("Id_Usuario_Workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Intervalo_Alarma_Usuario = Result
                Return Intervalo_Alarma_Usuario
                Exit Function
            End If
            If Datset Is Nothing Or Datset.Tables.Count = 0 Then
                Intervalo_Alarma_Usuario = " Error Verificando solicitando intervalo  de alarmas " & Parametro_Consulta
                Return Intervalo_Alarma_Usuario
                Exit Function
            Else
                If Datset.Tables(0).Rows.Count = 0 Then
                    HttpContext.Current.Session.Item("Parametro_Intervalo_Alarma") = -1
                    Intervalo_Alarma_Usuario = "YES"
                    Return Intervalo_Alarma_Usuario
                    Exit Function
                Else
                    HttpContext.Current.Session.Item("Parametro_Intervalo_Alarma") = ((Datset.Tables(0).Rows(0).Item(0) * 60) + 25) * 1000
                    Intervalo_Alarma_Usuario = "YES"
                    Return Intervalo_Alarma_Usuario
                    Exit Function
                End If
            End If
            Intervalo_Alarma_Usuario = "YES"
            Return Intervalo_Alarma_Usuario
        Catch ex As Exception
            Intervalo_Alarma_Usuario = ex.Message
        End Try
    End Function

    Function Intervalo_Alarma_Usuario(ByRef intervalo As Integer) As String
        '*********************************************************
        'Function :  Solicita Intervalo Alarma Usuario
        'Fecha    : 2010-06-08
        'Ing      : Miguel Angel Urueta Miranda
        'Proced   : Solicta el itervalo de jecucion de alarmas
        'Parameter: Id uaurio, Parametro de confirmacion
        'Actualización 2013-12-18 se adapto la conexión para sitio
        'web gestor documental 
        '**********************************************************

        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Dim Parametro_Consulta As String = "select INTERVALO from INTERVALO_ALARMAS_USUARIO " &
            "WHERE USUARIO_WORKFLOW_IDU_SUARIO=" & HttpContext.Current.Session.Item("Id_Usuario_Workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Intervalo_Alarma_Usuario = Result
                Return Intervalo_Alarma_Usuario
                Exit Function
            End If
            If Datset Is Nothing Or Datset.Tables.Count = 0 Then
                Intervalo_Alarma_Usuario = " Error Verificando solicitando intervalo  de alarmas " & Parametro_Consulta
                Return Intervalo_Alarma_Usuario
                Exit Function
            Else
                If Datset.Tables(0).Rows.Count = 0 Then
                    intervalo = -1
                    HttpContext.Current.Session.Item("Parametro_Intervalo_Alarma") = -1
                    Intervalo_Alarma_Usuario = "YES"
                    Return Intervalo_Alarma_Usuario
                    Exit Function
                Else
                    intervalo = Datset.Tables(0).Rows(0).Item(0)
                    HttpContext.Current.Session.Item("Parametro_Intervalo_Alarma") = ((Datset.Tables(0).Rows(0).Item(0) * 60) + 25) * 1000
                    Intervalo_Alarma_Usuario = "YES"
                    Return Intervalo_Alarma_Usuario
                    Exit Function
                End If
            End If
            Intervalo_Alarma_Usuario = "YES"
            Return Intervalo_Alarma_Usuario
        Catch ex As Exception
            Intervalo_Alarma_Usuario = ex.Message
        End Try
    End Function
    Function Solicita_nombre_cargo_usuario_workflow(ByVal id_usuario As Integer,
                                                    ByRef nombre_usuario As String,
                                                    ByRef nombre_cargo As String) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("usuario_workflow")
            Dim Parametro_Consulta As String = "select Nombre_Usuario, Cargo_Usuario from   usuario_workflow uw " &
            "where uw.idU_suario=" & id_usuario
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombre_cargo_usuario_workflow = "Función Solicita_nombre_cargo_usuario_workflow dice " & Result
                Return Intervalo_Alarma_Usuario()
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                nombre_usuario = ""
                nombre_cargo = ""
                Solicita_nombre_cargo_usuario_workflow = "YES"
                Exit Function
            Else
                nombre_usuario = Datset.Tables(0).Rows(0).Item(0)
                nombre_cargo = Datset.Tables(0).Rows(0).Item(1)
                Solicita_nombre_cargo_usuario_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombre_cargo_usuario_workflow = "Inconsistencia general función Solicita_nombre_cargo_usuario_workflow " & ex.Message
        End Try
    End Function
    Function Solicita_nombre_grupo_actividad_usuario_workflow(ByVal id_usuario As Integer,
                                                              ByRef nombre_grupo As String,
                                                              ByRef nombre_actividad_workflow As String) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("usuario_workflow")
            Dim Parametro_Consulta As String = "select gw.Nombre_Grupo, law.Nombre_Actividad from   usuario_workflow uw " &
                " left outer join  grupos_workflow as gw on (gw.Id_Grupo=uw.Grupos_Workflow_Id_Grupo) " &
                " left outer join listado_actividades_workflow as law on (law.Id_Actividad=gw.id_Actividad) " &
            "where uw.idU_suario=" & id_usuario
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombre_grupo_actividad_usuario_workflow = "Función Solicita_nombre_grupo_actividad_usuario_workflow dice " & Result
                Return Intervalo_Alarma_Usuario()
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                nombre_grupo = ""
                nombre_actividad_workflow = ""
                Solicita_nombre_grupo_actividad_usuario_workflow = "YES"
                Exit Function
            Else
                nombre_grupo = Datset.Tables(0).Rows(0).Item(0)
                nombre_actividad_workflow = Datset.Tables(0).Rows(0).Item(1)
                Solicita_nombre_grupo_actividad_usuario_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombre_grupo_actividad_usuario_workflow = "Inconsistencia general función Solicita_nombre_grupo_actividad_usuario_workflow " & ex.Message
        End Try
    End Function
    Function Solicita_id_grupo_usuario_workflow(ByVal id_usuario_worflow As Integer,
                                                ByRef id_grupo_workflow As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita el grupo relacionado al usuario workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'id_usuario_worflow  : Representa la identificación del usuario workflow
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'id_grupo_workflow  : Retorna la idnetificación del grupo workflow
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha  modoficado     : 2024-12-19
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("usuario_workflow")
            Dim Parametro_Consulta As String = "select Grupos_Workflow_Id_Grupo from   usuario_workflow uw " &
            "where uw.idU_suario=" & id_usuario_worflow
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_grupo_usuario_workflow = "Función Solicita_id_grupo_usuario_workflow dice (" & Result & ")"
                Return Intervalo_Alarma_Usuario()
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_grupo_usuario_workflow = "Imposible encontrar el grupo del usuario workflow (" & id_usuario_worflow & ")"
                Exit Function
            Else
                id_grupo_workflow = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_grupo_usuario_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_grupo_usuario_workflow = "Inconsistencia general función Solicita_id_grupo_usuario_workflow " & ex.Message
        End Try
    End Function
    Function Atualizar_Intervalo_Usuario(ByVal Id_Usuario As Integer,
                                         ByVal Intervalo As Integer) As String
        Try
            Dim Resultado_Insertar As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim SqlInsert As String = "UPDATE  USUARIO_WORKFLOW SET INTERVALO_USUARIO=" & Intervalo &
            " WHERE  idu_suario=" & Id_Usuario
            Resultado_Insertar = ref.SELECTION_INSERT_COMMAND(SqlInsert)
            If Resultado_Insertar = "YES" Then
                If Intervalo = -1 Then
                    HttpContext.Current.Session.Item("Parametro_Intervalo_workflow") = -1
                Else
                    HttpContext.Current.Session.Item("Parametro_Intervalo_workflow") = Intervalo * 100000
                End If
                Atualizar_Intervalo_Usuario = "YES"
                Exit Function
            Else

                Atualizar_Intervalo_Usuario = Resultado_Insertar
                Return Atualizar_Intervalo_Usuario
                Exit Function
            End If
        Catch ex As Exception
            Atualizar_Intervalo_Usuario = ex.Message
        End Try
    End Function
    Function Solicita_firma_usuario_workflow(ByVal id_usuario_workflow As Integer,
                                             ByRef ruta_firma As String) As String

        '-------------------------------------------------------------------------
        'Funcion : Bajar archivo firma de usuario wokflow
        'Descripcion : Funcion que baja la firma de la base del usuario workflow
        'y regresa la ruta del archivo
        'Ingeniero : Miguel Angel Urueta Miranda
        'Fecha : 2018-11-06
        '--------------------------------------------------------------------------
        Try
            Dim bDatos() As Byte
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("USUARIO_WORKFLOW")
            Dim Parametro_Consulta As String = "select FIRMA_USUARIO from USUARIO_WORKFLOW where IDU_SUARIO =" & id_usuario_workflow
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_firma_usuario_workflow = "Error función Solicita_firma_usuario_workflow  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_firma_usuario_workflow = "Imposible encontrar el usuario workflow "
                Exit Function
            Else
                Dim Tempvalor As Object = Datset.Tables(0).Rows(0).Item(0)
                If IsDBNull(Tempvalor) Then
                    Solicita_firma_usuario_workflow = "Usuario workflow sin firma Registrada en la base de datos "
                    Exit Function
                Else
                    bDatos = CType(Datset.Tables(0).Rows(0).Item(0), Byte())
                End If
            End If

            '********************************************
            'Bajando el archivo firma  de mysql
            '********************************************
            ImageDraw.LicenseOwner = "Miguel Angel Urueta Miranda-Developer License"
            ImageDraw.LicenseKey = "28Q48MH26VEUUW84A4FH9YV8Q33LJ7PC6WF84EZF3AMC93SVP2FQ"
            'Dim oFileStream As FileStream
            'ruta_firma = HttpContext.Current.Server.MapPath(HttpContext.Current.Session("GA_RUTA_FIRMA_GESTION") & "\" & id_usuario_workflow & ".bmp")
            ruta_firma = HttpContext.Current.Session("GA_RUTA_FIRMA_GESTION") & "\" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & ".bmp"
            If File.Exists(ruta_firma) Then File.Delete(ruta_firma)
            Dim imgDraw As New Neodynamic.WebControls.ImageDraw.ImageDraw()
            'Create an instance of ImageElement class
            Dim imgElem As New Neodynamic.WebControls.ImageDraw.ImageElement
            'Set the source property
            imgElem.Source = Neodynamic.WebControls.ImageDraw.ImageSource.Binary
            'Set the binary content of the image
            imgElem.SourceBinary = bDatos
            imgDraw.Elements.Add(imgElem)
            imgDraw.Save(ruta_firma)
            Solicita_firma_usuario_workflow = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_firma_usuario_workflow = "Incinsistencia general función Solicita_firma_usuario_workflow " & ex.Message
        End Try

    End Function
    Function Solicita_correo_usuario_workflow(ByVal id_usuario As Integer,
                                             ByRef correos As String) As String
        Try
            Dim Result As String = ""
            correos = ""
            Dim Parametro_Consulta As String = " SELECT  Correo_Usuario " &
              " from usuario_workflow  " &
              " where idU_suario= " & id_usuario
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("usuario_workflow")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_correo_usuario_workflow = "Función Solicita_correo_usuario_workflow dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_correo_usuario_workflow = "Imposible encontrar correo electrónico de usuario workflow  (" & id_usuario & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    correos = ""
                Else
                    correos = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_correo_usuario_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_correo_usuario_workflow = "Inconsistencia general función Solicita_correo_usuario_workflow " & ex.Message
        End Try
    End Function


    Function Solicita_estado_envio_correo_usuario_workflow(ByVal id_usuario_workflow As Integer,
                                                           ByRef estado_envio_correo As Integer) As String
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT  estado_envio_correo " &
              " from usuario_workflow  " &
              " where idU_suario= " & id_usuario_workflow
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("usuario_workflow")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_envio_correo_usuario_workflow = "Función Solicita_estado_envio_correo_usuario_workflow dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estado_envio_correo_usuario_workflow = "Imposible encontrar el estado de notificación de correo del usuario workflow  (" & id_usuario_workflow & ")"
                Exit Function
            Else
                estado_envio_correo = Datset.Tables(0).Rows(0).Item(0)
                Solicita_estado_envio_correo_usuario_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_envio_correo_usuario_workflow = "Inconsistencia general funcion Solicita_estado_envio_correo_usuario_workflow (" & ex.Message & ")"
        End Try
    End Function
    Function Cambia_estado_pagiancion_usuario(ByVal estado_paginacion As Integer,
                                              ByVal id_usuario_wf As Integer) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Resultado_Insertar As String = ""
            Dim update As String = "update permisos_usuario_workflow set  UTIL_PAGINACION=" & estado_paginacion &
                " where Usuario_Workflow_idU_suario=" & id_usuario_wf
            Resultado_Insertar = ref.SELECTION_INSERT_COMMAND(update)
            If Resultado_Insertar = "YES" Then
                Cambia_estado_pagiancion_usuario = "YES"
                Exit Function
            Else
                Cambia_estado_pagiancion_usuario = "Error intentando cambiar el estado de paginación"
                Exit Function
            End If
        Catch ex As Exception
            Cambia_estado_pagiancion_usuario = "Inconsistencia general función Cambia_estado_pagiancion_usuario " & ex.Message
        End Try
    End Function
End Class
