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
Imports System.Math
Public Class CdClassSeleccionTarea
    Property AppError As String
    Property Obj_ilist_row_generic As Object     'Seralizado DATA-SET
    Property Obj_ilist_fileds_generic As Object  'class_campos_table_bostra_table
    Property CdTareasWorkflow As CdTareasWorkflow
    Property ExistenciaAprobacionTarea As Integer
End Class
Public Class Classselecciotarea
    Public Structure Slecciondocumento
        Dim Contenedor As String
        Dim tag As String
    End Structure
    Function Solicita_radicado_id_tarea_seleccionada(ByVal id_ruta As Integer, _
                                                     ByVal id_tarea As Long, _
                                                     ByRef radicado As String) As String
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassWorkflow
            Dim Ref_clas_rutas As New Class_worflow_rutas
            Dim nombre_ruta As String = ""
            Result = Ref_clas_rutas.Retorna_nombre_ruta_por_id_ruta(id_ruta.ToString,
                                                                    nombre_ruta)
            If Result <> "YES" Then
                Result = Ref_clas_rutas.Retorna_nombre_ruta_workflow(nombre_ruta)
                If Result <> "YES" Then
                    Solicita_radicado_id_tarea_seleccionada = Result
                    Exit Function
                End If
            End If
            Dim nombre_campo_radicado As String = ""
            Dim Ref_class_config_listado As New Class_configuracion_listado_ruta
            Result = Ref_class_config_listado.SolicitaNombreCampoRadicadoRuta(id_ruta,
                                                                              nombre_campo_radicado)
            If Result <> "YES" Then
                Solicita_radicado_id_tarea_seleccionada = Result
                Exit Function
            End If
            Dim sqlconsulta As String = "Select " & nombre_campo_radicado & " from dat_adic_tar" & nombre_ruta & " where INICIO_TAREAS_WORKFLOW_ID_TAREA='" & id_tarea & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("dat_adic_tar" & nombre_ruta)
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Solicita_radicado_id_tarea_seleccionada = "Error Consultando en tabla " & "dat_adic_tar" & nombre_ruta
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_radicado_id_tarea_seleccionada = "Imposible encontrar el consecutivo de radicado del tarea (" & id_tarea & ")  en la ruta dat_adic_tar" & nombre_ruta
                Exit Function
            Else
                radicado = Datset.Tables(0).Rows(0).Item(0)
                Solicita_radicado_id_tarea_seleccionada = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_radicado_id_tarea_seleccionada = "Inconsistencia general función Solicita_radicado_id_tarea_seleccionada " & ex.Message
        End Try
    End Function
    Function Retorna_id_tarea_seleccionada_radicado(ByVal radicado As String,
                                                    ByVal id_ruta As Integer,
                                                    ByRef id_tarea As Long) As String
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassWorkflow
            Dim Ref_clas_rutas As New Class_worflow_rutas
            Dim nombre_ruta As String = ""
            Result = Ref_clas_rutas.Retorna_nombre_ruta_por_id_ruta(id_ruta.ToString,
                                                                    nombre_ruta)
            If Result <> "YES" Then
                Result = Ref_clas_rutas.Retorna_nombre_ruta_workflow(nombre_ruta)
                If Result <> "YES" Then
                    Retorna_id_tarea_seleccionada_radicado = Result
                    Exit Function
                End If
            End If
            Dim nombre_campo_radicado As String = ""
            Dim Ref_class_config_listado As New Class_configuracion_listado_ruta
            Result = Ref_class_config_listado.SolicitaNombreCampoRadicadoRuta(id_ruta,
                                                                              nombre_campo_radicado)
            If Result <> "YES" Then
                Retorna_id_tarea_seleccionada_radicado = Result
                Exit Function
            End If
            Dim sqlconsulta As String = "Select INICIO_TAREAS_WORKFLOW_ID_TAREA from dat_adic_tar" & nombre_ruta & " where " & nombre_campo_radicado & "='" & radicado & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("dat_adic_tar" & nombre_ruta)
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Retorna_id_tarea_seleccionada_radicado = "Error Consultando en tabla " & "dat_adic_tar" & nombre_ruta
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_tarea_seleccionada_radicado = "YES"
                Exit Function
            Else
                id_tarea = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_tarea_seleccionada_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_tarea_seleccionada_radicado = "Inconsistencia general función Retorna_id_tarea_seleccionada_radicado " & ex.Message
        End Try
    End Function



    Function Obtener_Id_Tarea_Selecionada(ByVal ID_ACTIVIDAD As String, ByVal Id_Usuario As String, ByRef Actividad_Seleccion As String) As String
        '--------------------------------------------------
        'Funcion   : Obtener_Id_Tarea_Selecionada
        'Ingeniero : Miguel Angel Urueta Miranda
        'Fecha     : 2009-07-28
        'Procedimiento : la funcion permite identificar
        'si el id de la tarea asignada
        '---------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT Inicio_Tareas_Workflow_id_Tarea FROM ESTADOS_TAREA_WORKFLOW" &
            " WHERE ID_ACTIVIDAD=" & ID_ACTIVIDAD &
            " AND ID_USUARIO=" & Id_Usuario & " AND " &
            " ESTADO_TAREA = 0 AND" &
            " FECHA_SELECCION IS NOT NULL and fecha_fin is null ORDER BY  FECHA_INICIO DESC"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ESTADOS_TAREA_WORKFLOW")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                Obtener_Id_Tarea_Selecionada = "Error Consultando en tabla " & "ESTADOS_TAREA_WORKFLOW" & Sql_consulta
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Actividad_Seleccion = "0"
                Obtener_Id_Tarea_Selecionada = "YES"
                Exit Function
            Else
                Actividad_Seleccion = Datset.Tables(0).Rows(0).Item(0).ToString
                Obtener_Id_Tarea_Selecionada = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Obtener_Id_Tarea_Selecionada = "Error Consultando id tarea seleccionada" & ex.ToString()
        End Try
    End Function
    Function Verifica_Tarea_Seleccionada_Uusario(ByRef pag As Page) As String
        '----------------------------------------------------------------
        'Funcion   : Verifica_Tarea_Seleccionada_Usuario
        'Ingeniero : Miguel Angel Urueta Miranda
        'Feha      : 2009-07-27
        'Procedimiento : Veriifca si el usuario tiene tarea selecciondas
        'y actualiza para que le usuario pueda mirar los documentos en el
        'listview seleccion
        '----------------------------------------------------------------
        Try
            Dim Ref2 As New ClassListandoTareas
            Dim Result As String = ""
            Dim Id_Activida As String = ""
            Dim Switc As String = "1"
            If HttpContext.Current.Session("Id_Usuario_Workflow") = 0 Then
                Verifica_Tarea_Seleccionada_Uusario = "Usuario sin identidad "
                Exit Function
            End If
            If HttpContext.Current.Session("Id_Grupo_Workflow") = 0 Then
                Verifica_Tarea_Seleccionada_Uusario = "Usuario sin grupo "
                Exit Function
            End If
            '------consulta id actividad usuario
            Result = ""
            Dim Ref_Class_grupos_workflow As New Class_grupos_workflow
            Result = Ref_Class_grupos_workflow.Solicita_id_actividad_usuario_workflow(Id_Activida,
                                                                                      HttpContext.Current.Session("Id_Grupo_Workflow"))
            If Result <> "YES" Or Id_Activida = "" Then
                Verifica_Tarea_Seleccionada_Uusario = "Error #08 Imposible Obtener Id actividad " & Result
                Exit Function
            End If
            '------------------------------
            'Obtener id tarea seleccionda
            '------------------------------
            Dim Numero_Actividades As String = "0"
            Result = ""
            Result = Obtener_Id_Tarea_Selecionada(Id_Activida,
                                                  HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                  Numero_Actividades)
            If Result <> "YES" Then
                Verifica_Tarea_Seleccionada_Uusario = "Error #09 " & Result
                Exit Function
            End If
            If Numero_Actividades = "0" Then
                HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0"
                Verifica_Tarea_Seleccionada_Uusario = "YES"
                Exit Function
            End If
            HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = Numero_Actividades
            Result = ""
            Result = Asigna_tarea(Numero_Actividades,
                                  1,
                                  Id_Activida,
                                  0,
                                  pag)
            If Result <> "YES" Then
                Verifica_Tarea_Seleccionada_Uusario = Result
                Exit Function
            Else
                Verifica_Tarea_Seleccionada_Uusario = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Verifica_Tarea_Seleccionada_Uusario = ex.ToString
        End Try
    End Function
    Function Verifica_Tarea_Seleccionada_Uusario_Inicio(ByRef pag As Page) As String
        '----------------------------------------------------------------
        'Funcion   : Verifica_Tarea_Seleccionada_Usuario
        'Ingeniero : Miguel Angel Urueta Miranda
        'Feha      : 2009-07-27
        'Procedimiento : Veriifca si el usuario tiene tarea selecciondas
        'y actualiza para que le usuario pueda mirar los documentos en el
        'listview seleccion
        '----------------------------------------------------------------
        Try
            Dim Ref2 As New ClassListandoTareas
            Dim Result As String = ""
            Dim Id_Activida As String = ""
            Dim Switc As String = "1"
            If HttpContext.Current.Session("Id_Usuario_Workflow") = 0 Then
                Verifica_Tarea_Seleccionada_Uusario_Inicio = "Usuario sin identidad "
                Exit Function
            End If
            If HttpContext.Current.Session("Id_Grupo_Workflow") = 0 Then
                Verifica_Tarea_Seleccionada_Uusario_Inicio = "Usuario sin grupo "
                Exit Function
            End If
            '------consulta id actividad usuario
            Result = ""
            Dim Ref_Class_grupos_workflow As New Class_grupos_workflow
            Result = Ref_Class_grupos_workflow.Solicita_id_actividad_usuario_workflow(Id_Activida,
                                                                                      HttpContext.Current.Session("Id_Grupo_Workflow"))
            If Result <> "YES" Or Id_Activida = "" Then
                Verifica_Tarea_Seleccionada_Uusario_Inicio = "Error #08 Imposible Obtener Id actividad " & Result
                Exit Function
            End If
            '------------------------------
            'Obtener id tarea seleccionda
            '------------------------------
            Dim id_tarea_seleccion As String = "0"
            Result = ""
            Result = Obtener_Id_Tarea_Selecionada(Id_Activida,
                                                  HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                  id_tarea_seleccion)
            If Result <> "YES" Then
                Verifica_Tarea_Seleccionada_Uusario_Inicio = "Error #09 " & Result
                Exit Function
            End If
            Dim Refclas_seleccion As New Classselecciotarea
            If id_tarea_seleccion = "0" Then
                HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0"
                Result = Refclas_seleccion.Actualiza_interface_estado_flujo_ruta(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                                                HttpContext.Current.Session.Item("WF_ID_ACTIVIDAD"),
                                                                                pag)
                If Result <> "YES" Then
                    Verifica_Tarea_Seleccionada_Uusario_Inicio = Result
                    Exit Function
                End If
                Verifica_Tarea_Seleccionada_Uusario_Inicio = "YES"
                Exit Function
            End If
            HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = id_tarea_seleccion
            Result = Asigna_tarea(id_tarea_seleccion,
                                  1,
                                  Id_Activida,
                                  0,
                                  pag)
            If Result <> "YES" Then
                Verifica_Tarea_Seleccionada_Uusario_Inicio = Result
                Exit Function
            Else
                Result = Refclas_seleccion.Actualiza_interface_estado_flujo_ruta(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                 HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                 HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                                                 HttpContext.Current.Session.Item("WF_ID_ACTIVIDAD"),
                                                                                 pag)
                If Result <> "YES" Then
                    Verifica_Tarea_Seleccionada_Uusario_Inicio = Result
                    Exit Function
                End If
                Verifica_Tarea_Seleccionada_Uusario_Inicio = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_Tarea_Seleccionada_Uusario_Inicio = "Inconsistencia general función Verifica_Tarea_Seleccionada_Uusario_Inicio " & ex.Message
        End Try
    End Function
    Function Determina_Id_Actividad_Grid(ByVal Id_row As Integer,
                                         ByVal RefDatdrid As GridView,
                                         ByRef id_actividad As Integer) As String
        '**********************************************
        'Funcion que retorna el dato de una columna
        'en un row especifico
        'Fecha : 2012-11-17
        'Ingeniero : Miguel Angel Urueta Miranda
        '**********************************************
        Dim datcolm As DataGridColumn = Nothing
        Dim Idex_colum As Integer = -1
        Dim incre As Integer = 0
        Dim str As String = ""
        Try
            For i As Integer = 0 To RefDatdrid.HeaderRow.Cells.Count
                str = str & RefDatdrid.HeaderRow.Cells(i).Text
                If RefDatdrid.HeaderRow.Cells(i).Text = "id_tarea" Then
                    Idex_colum = incre
                    Exit For
                End If
                incre = incre + 1

            Next
            If Idex_colum = -1 Then
                Determina_Id_Actividad_Grid = "Imposible encontrar id tarea en grid"
                Exit Function
            End If
            id_actividad = RefDatdrid.Rows(Id_row).Cells(Idex_colum).Text
            Determina_Id_Actividad_Grid = "YES"
        Catch ex As Exception
            Determina_Id_Actividad_Grid = "Function Determina_Id_Actividad_Grid " & ex.Message
        End Try
    End Function
    Function Determina_Tipo_Actividad_Usuario(ByRef Id_Activida As String,
                                              ByRef TipoActividad As String) As String
        '******************************************
        'Funcion : Determina_Tipo_Actividad_Usuario
        'Funcion que determina el tipo de actividad
        'para generar la interface necesaria
        'Fecha : 2012-11-16
        '******************************************
        Dim Ref As New Classselecciotarea
        Dim Ref2 As New ClassListandoTareas
        Try
            '-----Verifica que el usuario tenga id asignada
            If HttpContext.Current.Session("Id_Usuario_Workflow") = 0 Then
                Determina_Tipo_Actividad_Usuario = "Usuario sin identidad"
                Exit Function
            End If
            '-----Verifica que el usuario pertenezca a grupo
            If HttpContext.Current.Session("Id_Grupo_Workflow") = 0 Then
                Determina_Tipo_Actividad_Usuario = "Usuario sin grupo"
                Exit Function
            End If
            Dim Result As String
            '------consulta id actividad usuario
            Result = ""
            Dim Ref_Class_grupos_workflow As New Class_grupos_workflow
            Result = Ref_Class_grupos_workflow.Solicita_id_actividad_usuario_workflow(Id_Activida,
                                                                                      HttpContext.Current.Session("Id_Grupo_Workflow"))
            If Result <> "YES" Or Id_Activida = "" Then
                Determina_Tipo_Actividad_Usuario = "Error #10  Imposible Obtener Id actividad " & Result
                Exit Function
            End If
            Dim IdActividad As Integer = Val(Id_Activida)
            Dim Class_Listado_Actividades_workflow As New Class_Listado_Actividades_workflow
            Result = Class_Listado_Actividades_workflow.SolicitaNombreTipoActividadGeneralWorkflow(IdActividad,
                                                                                             TipoActividad)
            If Result <> "YES" Then
                Determina_Tipo_Actividad_Usuario = "Error #11 Consultando el tipo de actividad general" & Result
                Exit Function
            End If
            Determina_Tipo_Actividad_Usuario = "YES"
        Catch ex As Exception
            Determina_Tipo_Actividad_Usuario = "Inconsitencia general funcion Determina_Tipo_Actividad_Usuario " & ex.Message
        End Try
    End Function

    Function Evalua_evento_crear_imagen(ByVal id_Tarea As String,
                                        ByVal id_actividad As Integer,
                                        ByVal Tipo_Actividad As String,
                                        ByRef mEval As Object,
                                        ByRef Resultado As String) As String
        '*******************************************************************
        'Fucion : Ejecuta el script eb CREARIMAGENES para genera dianmicamente
        'Los campos de enlace del sistema para que los capture el usuario
        'para la digitacion de datos de la imagen digitalizada el sistema
        'genera dos matricez separadas por IPOSIVOL Y
        'Fecha 2012-12-15
        'Ing Miguel Angel Urueta Miranda
        '********************************************************************
        Try
            Resultado = ""
            Dim Conection_conectro_C = "Persist Security Info=" _
                 & True & ";database=" & HttpContext.Current.Session("DB_NAME_MODULO").ToString _
                 & ";server=" & HttpContext.Current.Session("IP_SERVER_MODULO").ToString _
                & ";user id=" & HttpContext.Current.Session("USER_DBMS_MODULO").ToString _
                & ";pwd=" & HttpContext.Current.Session("PASW_DBMS_MODULO").ToString

            Dim ResultadoComp As String = ""
            If HttpContext.Current.Session("CREARIMAGENES") = "" Then
                Evalua_evento_crear_imagen = "YES"
                Exit Function
            End If
            Dim MatriId() As String
            Dim mParam() As Object = {Conection_conectro_C, HttpContext.Current.Session("Id_Usuario_Workflow").ToString, id_Tarea.ToString,
                HttpContext.Current.Session("Id_Ruta_Workflow").ToString}
            Dim Resultado1 As String = ""
            Dim refcla As New ClassEdtiScript
            Resultado1 = refcla.Compila_Evalua(ResultadoComp, HttpContext.Current.Session("CREARIMAGENES"), "CREARIMAGENES", mParam)
            If Resultado1 <> "YES" Then
                Evalua_evento_crear_imagen = "Error #13 Error Compilando Function CREARIMAGENES" & Resultado1
                Exit Function
            End If
            If ResultadoComp <> "" Then
                '------------------------------------
                'Consulta si trae id tarea el enlase
                '------------------------------------
                Resultado = ResultadoComp
                If InStr(ResultadoComp, "POSITIVOQL_") Then
                    MatriId = ResultadoComp.Split("POSITIVOQL_")
                    If Not MatriId Is Nothing Then
                        Resultado = ResultadoComp
                    End If
                Else
                    Evalua_evento_crear_imagen = "El script CREARIMAGENES presenta inconsistencias " + Left(ResultadoComp, 60)
                    Exit Function
                End If
            End If
            Evalua_evento_crear_imagen = "YES"
        Catch ex As Exception
            Evalua_evento_crear_imagen = "Funcion Evalua_evento_crear_imagen " & ex.Message
        End Try
    End Function
    Function Actualizando_Estado_Tarea_Acrualiza_usuario_tarea_workflow(ByVal Id_Usuario As String,
                                                                        ByVal Id_Tarea As String,
                                                                        ByVal Id_Actividad As String,
                                                                        ByVal id_activdad_user As String,
                                                                        ByVal id_flujo_trabajo As Integer,
                                                                        ByVal id_actividad_flujo_trabajo As Integer,
                                                                        ByVal id_usuario_workflow_flujo_trabajo As Integer,
                                                                        ByVal estado_recuperacion_flujo_trabajo As Integer) As String
        '****************************************************************
        'Function : Actualizando_Estado_Tarea
        'Fecha    : 2017-12-13
        'Ing      : Miguel Angel Urueta Miranda
        'Proced   : Actualiza el usuario workflow que tiene asignada
        'la tarea y la actividad esta función se llama cuando se actualiza
        'el destinatario de un documento desde la interface edita
        'coorrepondencia
        '****************************************************************
        Try
            Dim Parametro_Insert As String = "UPDATE ESTADOS_TAREA_WORKFLOW " &
            " SET ID_USUARIO=" & Id_Usuario &
            ",ID_ACTIVIDAD=" & Id_Actividad &
            ",ID_FLUJO_TRABAJO=" & id_flujo_trabajo &
            ",ID_ACTIVIDAD_FLUJO_TRABAJO=" & id_actividad_flujo_trabajo &
            ",ID_USUARIO_WORKFLOW_FLUJO_TRABAJO=" & id_usuario_workflow_flujo_trabajo &
            ",ESTADO_RECUPERACION_FLUJO_TRABAJO=" & estado_recuperacion_flujo_trabajo &
            " WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & Id_Tarea &
            " AND  FECHA_FIN IS NULL"
            Dim ref As New conect.Dbase_Conction_Mysql
            ref = New conect.Dbase_Conction_Mysql
            Dim result As String = ""
            result = ref.SELECTION_INSERT_COMMAND(Parametro_Insert)
            If result <> "YES" Then
                Actualizando_Estado_Tarea_Acrualiza_usuario_tarea_workflow = " Error Actualizando Estado Tarea Workflow función  Actualizando_Estado_Tarea_Acrualiza_usuario_tarea_workflow  " & Parametro_Insert
                Exit Function
            Else
                Actualizando_Estado_Tarea_Acrualiza_usuario_tarea_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Actualizando_Estado_Tarea_Acrualiza_usuario_tarea_workflow = ex.Message
        End Try
    End Function
    Function Agrega_icono_image_tre_view(ByVal file_imagen As String,
                                         ByRef tred As TreeNode) As String
        Try
            Dim fil_inf As New FileInfo(file_imagen)
            Select Case UCase(fil_inf.Extension)
                Case ".TIF"
                    tred.ImageUrl = "../workflow/imageneswf/file-image-light_16.png"
                Case ".TIFF"
                    tred.ImageUrl = "../workflow/imageneswf/file-image-light_16.png"
                Case ".JPG"
                    tred.ImageUrl = "../workflow/imageneswf/file-image-light_16.png"
                Case ".BMP"
                    tred.ImageUrl = "../workflow/imageneswf/file-image-light_16.png"
                Case ".PDF"
                    tred.ImageUrl = "../workflow/imageneswf/file-pdf-light.png"
                Case ".DOC"
                    tred.ImageUrl = "../workflow/imageneswf/file-word-light.png"
                Case ".DOCX"
                    tred.ImageUrl = "../workflow/imageneswf/file-word-light.png"
                Case ".XLS"
                    tred.ImageUrl = "../workflow/imageneswf/file-excel-light.png"
                Case ".XLSX"
                    tred.ImageUrl = "../workflow/imageneswf/file-excel-light.png"
                Case ".PPT"
                    tred.ImageUrl = "../workflow/imageneswf/file-powerpoint-light.png"
                Case ".PPTX"
                    tred.ImageUrl = "../workflow/imageneswf/file-powerpoint-light.png"
                Case Else
                    tred.ImageUrl = "../workflow/imageneswf/page_white.png"
            End Select
            Agrega_icono_image_tre_view = "YES"
        Catch ex As Exception
            Agrega_icono_image_tre_view = "Inconsistencia general función Agrega_icono_image_tre_view " & ex.Message
        End Try
    End Function
    Function Agrega_icono_image_tre_view_extension(ByVal file_extension As String,
                                                   ByRef tred As TreeNode) As String
        Try
            Select Case file_extension
                Case "-1"
                    tred.ImageUrl = "../workflow/imageneswf/file-image-light_16.png"
                Case "-10"
                    tred.ImageUrl = "../workflow/imageneswf/file-image-light_16.png"
                Case "-11"
                    tred.ImageUrl = "../workflow/imageneswf/file-image-light_16.png"
                Case "-3"
                    tred.ImageUrl = "../workflow/imageneswf/file-image-light_16.png"
                Case "-30"
                    tred.ImageUrl = "../workflow/imageneswf/file-image-light_16.png"
                Case "-33"
                    tred.ImageUrl = "../workflow/imageneswf/file-image-light_16.png"
                Case "-4"
                    tred.ImageUrl = "../workflow/imageneswf/file-image-light_16.png"
                Case "-40"
                    tred.ImageUrl = "../workflow/imageneswf/file-image-light_16.png"
                Case "-44"
                    tred.ImageUrl = "../workflow/imageneswf/file-image-light_16.png"
                Case "-2"
                    tred.ImageUrl = "../workflow/imageneswf/file-pdf-light.png"
                Case "-20"
                    tred.ImageUrl = "../workflow/imageneswf/file-pdf-light.png"
                Case "-22"
                    tred.ImageUrl = "../workflow/imageneswf/file-pdf-light.png"
                Case "-5"
                    tred.ImageUrl = "../workflow/imageneswf/file-word-light.png"
                Case "-50"
                    tred.ImageUrl = "../workflow/imageneswf/file-word-light.png"
                Case "-55"
                    tred.ImageUrl = "../workflow/imageneswf/file-word-light.png"
                Case "-51"
                    tred.ImageUrl = "../workflow/imageneswf/file-word-light.png"
                Case "-510"
                    tred.ImageUrl = "../workflow/imageneswf/file-word-light.png"
                Case "-561"
                    tred.ImageUrl = "../workflow/imageneswf/file-word-light.png"
                Case "-52"
                    tred.ImageUrl = "../workflow/imageneswf/file-excel-light.png"
                Case "-520"
                    tred.ImageUrl = "../workflow/imageneswf/file-excel-light.png"
                Case "-572"
                    tred.ImageUrl = "../workflow/imageneswf/file-excel-light.png"
                Case "-53"
                    tred.ImageUrl = "../workflow/imageneswf/file-excel-light.png"
                Case "-530"
                    tred.ImageUrl = "../workflow/imageneswf/file-excel-light.png"
                Case "-583"
                    tred.ImageUrl = "../workflow/imageneswf/file-excel-light.png"
                Case "-54"
                    tred.ImageUrl = "../workflow/imageneswf/file-powerpoint-light.png"
                Case "-540"
                    tred.ImageUrl = "../workflow/imageneswf/file-powerpoint-light.png"
                Case "-594"
                    tred.ImageUrl = "../workflow/imageneswf/file-powerpoint-light.png"
                Case "-15"
                    tred.ImageUrl = "../workflow/imageneswf/file-powerpoint-light.png"
                Case "-550"
                    tred.ImageUrl = "../workflow/imageneswf/file-powerpoint-light.png"
                Case "-605"
                    tred.ImageUrl = "../workflow/imageneswf/file-powerpoint-light.png"
                Case Else
                    tred.ImageUrl = "../workflow/imageneswf/page_white.png"
            End Select
            Agrega_icono_image_tre_view_extension = "YES"
        Catch ex As Exception
            Agrega_icono_image_tre_view_extension = "Inconsistencia general función Agrega_icono_image_tre_view_extension " & ex.Message
        End Try
    End Function
    Function Actualiza_seleccion_workflow_indice(ByVal descripcion_tipo_documento As String,
                                                 ByRef tre_node As TreeNode,
                                                 ByRef hidden_selecion_actualiza_treview As Object,
                                                 ByRef ref_update As UpdatePanel) As String
        Try
            If descripcion_tipo_documento = "" Then
                tre_node.Text = "Documento"
                hidden_selecion_actualiza_treview.value = descripcion_tipo_documento
            Else
                tre_node.Text = descripcion_tipo_documento
                hidden_selecion_actualiza_treview.value = descripcion_tipo_documento
            End If
            ref_update.Update()
            Actualiza_seleccion_workflow_indice = "YES"
        Catch ex As Exception
            Actualiza_seleccion_workflow_indice = "Inconsistencia general función Actualiza_seleccion_workflow_indice " & ex.Message
        End Try
    End Function

    Function Inicializa_Tarea_Workflow(ByVal id_Tarea As String,
                                       ByVal id_actividad As Integer,
                                       ByVal Tipo_Actividad As String,
                                       ByRef gridv As GridView,
                                       ByVal id_gridvi As Integer,
                                       ByRef mEval As Object,
                                       ByRef Oblectre As Object) As String
        Dim Ref As New Classselecciotarea
        Dim Ref2 As New ClassListandoTareas
        Dim i2 As Integer = 0
        Dim Result As String = ""
        Dim Evalua_Uusario As Integer = 0
        Dim Switc As String = "0"
        Dim MatriId As String()
        Dim ResultadoComp As String = ""
        Erase MatriId
        Try
            '-----Verifica que el usuario tenga id asignada
            If HttpContext.Current.Session("Id_Usuario_Workflow") = 0 Then
                Inicializa_Tarea_Workflow = "Usuario sin identidad"
                Exit Function
            End If
            '-----Verifica que el usuario pertenezca a grupo
            If HttpContext.Current.Session("Id_Grupo_Workflow") = 0 Then
                Inicializa_Tarea_Workflow = "Usuario sin grupo"
                Exit Function
            End If
            '------------------------------------------------------------------------------------
            'Funcion Enlace para las actividades de enlace
            '-------------------------------------------------------------------------------------
            Dim Conection_conectro_C As String = ""
            Conection_conectro_C = "Persist Security Info=" _
                  & True & ";database=" & HttpContext.Current.Session("DB_NAME_MODULO").ToString _
                  & ";server=" & HttpContext.Current.Session("IP_SERVER_MODULO").ToString _
                 & ";user id=" & HttpContext.Current.Session("USER_DBMS_MODULO").ToString _
                 & ";pwd=" & HttpContext.Current.Session("PASW_DBMS_MODULO").ToString

            '*************************************************
            '------determina si algun usuario tomo la tarea
            '*************************************************
            If id_Tarea = "" Then
                Inicializa_Tarea_Workflow = "El sistema no registra codigo para el evento de seleccion " & vbCrLf _
              & " Informar al administrador que adicione el codigo al evento PREINICIO"
                Exit Function
            End If
            Result = ""
            Dim StruUsuarioTareaAsignada As StruUsuarioTareaAsignada = Nothing
            Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            Dim IdTareaWorkflow As Integer = Val(id_Tarea)
            Result = Class_estados_tarea_workflow.SolicitaEstructuraUsuarioTareaAsignada(IdTareaWorkflow,
                                                                                         HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                                                         HttpContext.Current.Session("Id_Grupo_Workflow"),
                                                                                         StruUsuarioTareaAsignada)
            If Result <> "YES" Then
                Inicializa_Tarea_Workflow = Result
                Exit Function
            End If

            If Not StruUsuarioTareaAsignada.CargoUsuario Is Nothing Then
                Inicializa_Tarea_Workflow = "No es posible continuar: el usuario " & StruUsuarioTareaAsignada.LoginUsuario & " (" & StruUsuarioTareaAsignada.CargoUsuario & ") ya tiene la tarea seleccionada, por favor actualizar la lista de tareas."
                Exit Function
            End If
            '-------------------------------------------------------------
            'Consulta si se ejecuta script preinicio para evaluar usuario
            '-------------------------------------------------------------
            Dim Resultado2 As String = ""
            Dim ref_Class_grupos_workflow As New Class_grupos_workflow
            Resultado2 = ref_Class_grupos_workflow.SolicitaEstadoEjecucionEventoInicio(Evalua_Uusario,
                                                                                             HttpContext.Current.Session("Id_Grupo_Workflow"))
            If Resultado2 <> "YES" Then
                Inicializa_Tarea_Workflow = Resultado2
                Exit Function
            End If
            '----------------------------------------
            'Ejecuta script PREINICIO
            'evalua usuario esta en 1
            '----------------------------------------
            If Evalua_Uusario = 0 Then
                If mEval Is Nothing Then
                    Inicializa_Tarea_Workflow = "Error #16 Funcion de Compilacion Eval Sin compilar "
                    Exit Function
                End If
                Dim mParamP() As Object = {Conection_conectro_C, HttpContext.Current.Session("Id_Usuario_Workflow").ToString}
                Dim Resultado4 As String = ""
                If HttpContext.Current.Session("PREINICIO") <> "" Then
                    Dim refcla As New ClassEdtiScript
                    Resultado4 = refcla.Compila_Evalua(ResultadoComp, HttpContext.Current.Session("PREINICIO"), "PREINICIO", mParamP)
                    If Resultado4 <> "YES" Then
                        Inicializa_Tarea_Workflow = Resultado4 & " PREINICIO no compila"
                        Exit Function
                    End If
                    If Not ResultadoComp Is Nothing Then
                        If ResultadoComp <> "YES" Then
                            Inicializa_Tarea_Workflow = " El evento programdo PREINICIO dice " & ResultadoComp
                            Exit Function
                        End If
                    End If
                End If
            End If
            '----------------------------------------
            'Ejecuta Script tomartarea
            '----------------------------------------
            Dim Ref_class_listado As New Class_Listado_Actividades_workflow
            Dim id_actividad_evento As Integer = 0
            Dim nombre_actividad As String = ""
            Result = Ref_class_listado.Retorna_actividad_grupo_workflow(HttpContext.Current.Session("Id_Grupo_Workflow"),
                                                                        id_actividad_evento,
                                                                        nombre_actividad)
            If Result <> "YES" Then
                Inicializa_Tarea_Workflow = "Inicializa tarea dice " & Result
                Exit Function
            End If
            Dim mParamT() As Object = {Conection_conectro_C,
                                       HttpContext.Current.Session("Id_Usuario_Workflow").ToString,
                                       id_Tarea,
                                       id_actividad_evento}
            Dim Resultado0 As String = ""
            If HttpContext.Current.Session("TOMARTAREA") <> "" Then
                Dim refcla As New ClassEdtiScript
                ResultadoComp = ""
                Dim Resultado4 As String = refcla.Compila_Evalua(ResultadoComp,
                                                                 HttpContext.Current.Session("TOMARTAREA"),
                                                                 "TOMARTAREA",
                                                                 mParamT)
                If Resultado4 <> "YES" Then
                    Inicializa_Tarea_Workflow = Resultado4 & " TOMARTAREA no compila"
                    Exit Function
                End If
                If Not ResultadoComp Is Nothing Then
                    If ResultadoComp <> "YES" Then
                        Inicializa_Tarea_Workflow = " El evento programdo TOMARTAREA dice " & ResultadoComp
                        Exit Function
                    End If
                End If
            End If
            Inicializa_Tarea_Workflow = "YES"
        Catch ex As Exception
            Inicializa_Tarea_Workflow = "Eror general " & ex.Message
        End Try
    End Function



    Function Obtener_Id_Tarea_Sleccion(ByRef ID_ACTIVIDAD As String,
                                       ByRef Id_Tarea As String) As String

        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT INICIO_TAREAS_WORKFLOW_ID_TAREA " &
            "FROM ESTADOS_TAREA_WORKFLOW " &
            " WHERE ID_ACTIVIDAD= " & ID_ACTIVIDAD &
            " AND ID_USUARIO IS NULL  AND FECHA_FIN IS NULL " &
            " AND ESTADO_TAREA =0 " &
            " ORDER BY ESTADO_PRIORIDAD desc,  FECHA_INICIO LIMIT 1"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ESTADOS_TAREA_WORKFLOW")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Id_Tarea = "0"
                Obtener_Id_Tarea_Sleccion = "Error Consultando en tabla " & "ESTADOS_TAREA_WORKFLOW" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Id_Tarea = "0"
                Obtener_Id_Tarea_Sleccion = "YES"
                Exit Function
            Else
                Id_Tarea = Datset.Tables(0).Rows(0).Item(0).ToString()
                Obtener_Id_Tarea_Sleccion = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Obtener_Id_Tarea_Sleccion = "Error Consultando Tareas Disponibles" & ex.Message
        End Try
    End Function

    Function Actualiza_interface_estado_flujo_ruta(ByVal id_tarea As Long,
                                                   ByVal id_ruta As Integer,
                                                   ByVal id_usuario_workflow As Integer,
                                                   ByVal id_actividad As Integer,
                                                   ByRef pag As Page) As String
        Try
            Dim Result As String = ""
            Dim Panel_EnviaActividad As Panel = pag.FindControl("Panel_EnviaActividad")
            If Panel_EnviaActividad Is Nothing Then
                Actualiza_interface_estado_flujo_ruta = "Impsosible encontrar el control (Panel_EnviaActividad)"
                Exit Function
            End If
            Dim Panel_EnviarUsuario As Panel = pag.FindControl("Panel_EnviarUsuario")
            If Panel_EnviarUsuario Is Nothing Then
                Actualiza_interface_estado_flujo_ruta = "Impsosible encontrar el control (Panel_EnviarUsuario)"
                Exit Function
            End If
            Dim Panel_devolver_tarea As Panel = pag.FindControl("Panel_devolver_tarea")
            If Panel_devolver_tarea Is Nothing Then
                Actualiza_interface_estado_flujo_ruta = "Impsosible encontrar el control (Panel_devolver_tarea)"
                Exit Function
            End If
            Dim Panel_autoterminar As Panel = pag.FindControl("Panel_autoterminar")
            If Panel_autoterminar Is Nothing Then
                Actualiza_interface_estado_flujo_ruta = "Impsosible encontrar el control (Panel_autoterminar)"
                Exit Function
            End If
            Dim Panel_autoriza As Panel = pag.FindControl("Panel_autoriza")
            If Panel_autoriza Is Nothing Then
                Actualiza_interface_estado_flujo_ruta = "Impsosible encontrar el control (Panel_autoriza)"
                Exit Function
            End If
            Dim CheckBox_auturiza As CheckBox = pag.FindControl("CheckBox_auturiza")
            If CheckBox_auturiza Is Nothing Then
                Actualiza_interface_estado_flujo_ruta = "Impsosible encontrar el control (CheckBox_auturiza)"
                Exit Function
            End If
            Dim Label_estado_selecion As Label = pag.FindControl("Label_estado_selecion")
            If Label_estado_selecion Is Nothing Then
                Actualiza_interface_estado_flujo_ruta = "Impsosible encontrar el control (Label_estado_selecion)"
                Exit Function
            End If
            Dim Panel_enviar_flujo As Panel = pag.FindControl("Panel_enviar_flujo")
            If Panel_enviar_flujo Is Nothing Then
                Actualiza_interface_estado_flujo_ruta = "Impsosible encontrar el control (Panel_enviar_flujo)"
                Exit Function
            End If

            Dim Panel_Buttonanotacion As Panel = pag.FindControl("Panel_Buttonanotacion")
            If Panel_Buttonanotacion Is Nothing Then
                Actualiza_interface_estado_flujo_ruta = "Impsosible encontrar el control (Panel_Buttonanotacion)"
                Exit Function
            End If
            Dim updatemenu As UpdatePanel = pag.FindControl("updatemenu")
            If updatemenu Is Nothing Then
                Actualiza_interface_estado_flujo_ruta = "Impsosible encontrar el control (updatemenu)"
                Exit Function
            End If
            Dim UpdatePanel_estado_tarea As UpdatePanel = pag.FindControl("UpdatePanel_estado_tarea")
            If UpdatePanel_estado_tarea Is Nothing Then
                Actualiza_interface_estado_flujo_ruta = "Imposible encontrar el control (UpdatePanel_estado_tarea)"
                Exit Function
            End If
            Dim UpdatePanel_menu_cab As UpdatePanel = pag.FindControl("UpdatePanel_menu_cab")
            If UpdatePanel_menu_cab Is Nothing Then
                Actualiza_interface_estado_flujo_ruta = "Imposible encontrar el control (UpdatePanel_menu_cab)"
                Exit Function
            End If
            Dim Label_estado_tarea_selecion As Label = pag.FindControl("Label_estado_tarea_selecion")
            If Label_estado_tarea_selecion Is Nothing Then
                Actualiza_interface_estado_flujo_ruta = "Imposible encontrar el control (Label_estado_tarea_selecion)"
                Exit Function
            End If
            Dim workflowPage As Webworkflow = TryCast(pag, Webworkflow)
            Dim modernTaskContextEnabled As Boolean = workflowPage IsNot Nothing AndAlso workflowPage.WorkflowCentroTrabajoModernPresentationEnabled
            Dim modernNotesEnabled As Boolean = workflowPage IsNot Nothing AndAlso workflowPage.WorkflowCentroTrabajoModernActive
            Dim Label_contexto_tramite As Label = Nothing
            Dim Label_contexto_estado As Label = Nothing
            If modernTaskContextEnabled Then
                Label_contexto_tramite = pag.FindControl("Label_contexto_tramite")
                Label_contexto_estado = pag.FindControl("Label_contexto_estado")
                If Label_contexto_tramite Is Nothing OrElse Label_contexto_estado Is Nothing Then
                    Actualiza_interface_estado_flujo_ruta = "Imposible encontrar los controles de contexto DOC-2"
                    Exit Function
                End If
            End If
            Dim Panel_detalle_tarea As Panel = pag.FindControl("Panel_detalle_tarea")
            If Panel_detalle_tarea Is Nothing Then
                Actualiza_interface_estado_flujo_ruta = "Imposible encontrar el control (Panel_detalle_tarea)"
                Exit Function
            End If
            Dim Panel_tramitar_tarea As Panel = pag.FindControl("Panel_tramitar_tarea")
            If Panel_tramitar_tarea Is Nothing Then
                Actualiza_interface_estado_flujo_ruta = "Imposible encontrar el control (Panel_tramitar_tarea)"
                Exit Function
            End If
            Dim Panel_documentos_tarea As Panel = pag.FindControl("Panel_documentos_tarea")
            If Panel_documentos_tarea Is Nothing Then
                Actualiza_interface_estado_flujo_ruta = "Imposible encontrar el control (Panel_documentos_tarea)"
                Exit Function
            End If
            Dim Panel_info_tarea As Panel = pag.FindControl("Panel_info_tarea")
            If Panel_info_tarea Is Nothing Then
                Actualiza_interface_estado_flujo_ruta = "Imposible encontrar el control (Panel_info_tarea)"
                Exit Function
            End If
            If id_tarea = 0 Or id_tarea = -1 Then
                Panel_EnviaActividad.Visible = False
                Panel_EnviarUsuario.Visible = False
                Panel_devolver_tarea.Visible = False
                Panel_autoterminar.Visible = False
                Panel_autoriza.Visible = False
                Panel_enviar_flujo.Visible = False
                Panel_detalle_tarea.Visible = False
                Panel_tramitar_tarea.Visible = False
                Panel_documentos_tarea.Visible = False
                Panel_info_tarea.Visible = False
                Panel_Buttonanotacion.Visible = False
                Label_estado_selecion.Visible = False
                Label_estado_tarea_selecion.Text = "Estado"
                If modernTaskContextEnabled Then
                    Label_contexto_tramite.Text = String.Empty
                    Label_contexto_estado.Text = String.Empty
                End If
                updatemenu.Update()
                UpdatePanel_estado_tarea.Update()
                UpdatePanel_menu_cab.Update()
                Actualiza_interface_estado_flujo_ruta = "YES"
                Exit Function
            Else
                Panel_EnviaActividad.Visible = True
                Panel_EnviarUsuario.Visible = True
                Panel_devolver_tarea.Visible = True
                Panel_autoterminar.Visible = True
                Panel_detalle_tarea.Visible = True
                Panel_tramitar_tarea.Visible = True
                Panel_documentos_tarea.Visible = True
                Panel_autoriza.Visible = True
                Panel_enviar_flujo.Visible = True
                Panel_Buttonanotacion.Visible = Not modernNotesEnabled
                Label_estado_selecion.Visible = True
                Panel_info_tarea.Visible = True
                UpdatePanel_menu_cab.Update()
            End If
            If HttpContext.Current.Session("TIPOACTIVIDADWF") = "ENLASE" Then
                Panel_autoterminar.Visible = True
            Else
                Panel_autoterminar.Visible = False
            End If
            Dim Ref_class_workflow_flujo As New Class_flujo_trabajo_workflow
            Dim Ref_class_worlflow_ruta As New Class_worflow_rutas
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim id_flujo_trabajo As Integer = 0
            Dim estado_cerrado_flujo As Integer = 0
            Dim estado_ruta As Integer = 0
            Dim nombre_flujo_trabajo As String = ""
            Dim tipo_proceso_contexto As String = ""
            Dim nombre_proceso_contexto As String = ""
            Dim estado_contexto As String = ""
            Result = Class_DAT_ADIC_TAR.SolicitaIdFlujoTrabajoIdTareaRutaWorkflow(HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                                  id_tarea,
                                                                                  id_flujo_trabajo)
            If Result <> "YES" Then
                Actualiza_interface_estado_flujo_ruta = Result
                Exit Function
            End If
            If id_flujo_trabajo <> 0 Then
                Result = Ref_class_workflow_flujo.SolicitaEstadoAbiertoCerradoFlujoDocumental(id_flujo_trabajo,
                                                                                                   estado_cerrado_flujo)
                If Result <> "YES" Then
                    Actualiza_interface_estado_flujo_ruta = Result
                    Exit Function
                End If
                Result = Ref_class_workflow_flujo.SolicitaNombreFlujoTrabajoPorIdFlujo(id_flujo_trabajo,
                                                                                            nombre_flujo_trabajo)
                If Result <> "YES" Then
                    Actualiza_interface_estado_flujo_ruta = Result
                    Exit Function
                End If
                If estado_cerrado_flujo = 1 Then
                    Panel_EnviarUsuario.Visible = False
                    Panel_EnviaActividad.Visible = False
                    Label_estado_selecion.Text = "Flujo : " & nombre_flujo_trabajo & " Tipo : Cerrado"
                    Panel_autoterminar.Visible = False
                    estado_contexto = "Cerrado"
                Else
                    Panel_EnviarUsuario.Visible = True
                    Panel_EnviaActividad.Visible = True
                    Label_estado_selecion.Text = "Flujo : " & nombre_flujo_trabajo & " Tipo : Abierto"
                    estado_contexto = "Abierto"
                End If
                tipo_proceso_contexto = "Flujo"
                nombre_proceso_contexto = nombre_flujo_trabajo

            Else
                Result = Ref_class_worlflow_ruta.Solicita_etado_abierto_cerrado_ruta_tarea(id_tarea,
                                                                                           id_ruta,
                                                                                           estado_ruta,
                                                                                           "")
                If Result <> "YES" Then
                    Actualiza_interface_estado_flujo_ruta = Result
                    Exit Function
                End If
                If estado_ruta = 1 Then
                    Panel_EnviarUsuario.Visible = False
                    Panel_EnviaActividad.Visible = False
                    Label_estado_selecion.Text = "Ruta : " & HttpContext.Current.Session("WF_RUTAWORKFLOW") & " Tipo : Cerrado"
                    Panel_autoterminar.Visible = False
                    estado_contexto = "Cerrado"
                Else
                    Panel_EnviarUsuario.Visible = True
                    Panel_EnviaActividad.Visible = True
                    Label_estado_selecion.Text = "Ruta : " & HttpContext.Current.Session("WF_RUTAWORKFLOW") & " Tipo : Abierto"
                    estado_contexto = "Abierto"
                End If
                tipo_proceso_contexto = "Ruta"
                nombre_proceso_contexto = Convert.ToString(HttpContext.Current.Session("WF_RUTAWORKFLOW"))
            End If
            If HttpContext.Current.Session("CAMBIO_USUARIO") = 0 Then
                Panel_EnviarUsuario.Visible = False
            End If
            If HttpContext.Current.Session("Cambio_Ruta") = 0 Then
                Panel_EnviaActividad.Visible = False
            End If
            Dim Ref_autoriza_tarea_workflow As New Class_autoriza_tarea_worklfow
            Dim existencia_autorizacion As String = ""
            Result = Ref_autoriza_tarea_workflow.SolicitaExistenciaAutorizacion(id_tarea,
                                                                                  id_actividad,
                                                                                  id_usuario_workflow,
                                                                                  existencia_autorizacion)
            If Result <> "YES" Then
                Actualiza_interface_estado_flujo_ruta = Result
                Exit Function
            End If
            If existencia_autorizacion = "YES" Then
                CheckBox_auturiza.Checked = True
            Else
                CheckBox_auturiza.Checked = False
            End If
            '---------------------------------------------
            '----Asigna los datos del radicado asignado
            '---------------------------------------------
            Dim class_config_list_ruta As New Class_configuracion_listado_ruta
            Dim nombre_campo_beneficiario As String = ""
            Result = class_config_list_ruta.SolicitaNombreCampoBenificiarioRuta(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                                nombre_campo_beneficiario)
            If Result <> "YES" Then
                Actualiza_interface_estado_flujo_ruta = Result
                Exit Function
            End If
            Dim class_dat As New Class_DAT_ADIC_TAR
            Dim beneficiario As String = ""
            If nombre_campo_beneficiario <> "" Then
                Result = class_dat.SolicitaBeneficiarioTareaWorkflow(HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                     nombre_campo_beneficiario,
                                                                     id_tarea,
                                                                     beneficiario)

                If Result <> "YES" Then
                    Actualiza_interface_estado_flujo_ruta = Result
                    Exit Function
                End If
            End If
            Dim Radicado As String = ""
            Result = class_dat.Solicita_radicado_id_tarea_seleccionada(id_tarea,
                                                                       Radicado)

            If Result <> "YES" Then
                Actualiza_interface_estado_flujo_ruta = Result
                Exit Function
            End If
            Dim nombre_campo_tramite As String = ""
            Dim tramite As String = ""
            Result = class_config_list_ruta.SolicitaNombreCampoTramiteRuta(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                           nombre_campo_tramite)
            If Result <> "YES" Then
                Actualiza_interface_estado_flujo_ruta = Result
                Exit Function
            End If
            Result = class_dat.SolicitaTramiteFlujoWorkflow(id_tarea,
                                                            HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                            nombre_campo_tramite,
                                                            HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                            tramite,
                                                            0)

            If Result <> "YES" Then
                Actualiza_interface_estado_flujo_ruta = Result
                Exit Function
            End If
            If modernTaskContextEnabled Then
                Label_contexto_tramite.Text = System.Web.HttpUtility.HtmlEncode(tramite)
                Label_contexto_estado.Text = System.Web.HttpUtility.HtmlEncode(estado_contexto)
                Label_estado_tarea_selecion.Text = "Radicado " & System.Web.HttpUtility.HtmlEncode(Radicado)
                If Not String.IsNullOrWhiteSpace(beneficiario) Then
                    Label_estado_tarea_selecion.Text &= " · " & System.Web.HttpUtility.HtmlEncode(beneficiario)
                End If
                Label_estado_selecion.Text = tipo_proceso_contexto & " · " & System.Web.HttpUtility.HtmlEncode(nombre_proceso_contexto)
            Else
                Label_estado_tarea_selecion.Text = "-Radicado : " & Radicado & "      -Solicitante : " & beneficiario & "    -Tramite : " & tramite
            End If
            UpdatePanel_estado_tarea.Update()
            Actualiza_interface_estado_flujo_ruta = "YES"
            updatemenu.Update()
            Exit Function
        Catch ex As Exception
            Actualiza_interface_estado_flujo_ruta = "Inconsistencia general función Actualiza_interface_estado_flujo_ruta " & ex.Message
        End Try
    End Function

    Function SeleccionTareaInicioWorkflow(ByVal IdRutaWorkflow As Integer,
                                          ByVal IdActividadWorkflow As Integer,
                                          ByVal IdUsuarioWorkflow As Integer,
                                          ByVal NombreRutaWorkflow As String,
                                          ByRef CdClassSeleccionTarea As CdClassSeleccionTarea) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Selecciona tarea workflow y retorna los datos estructuruados de asignación como
        'la lista de tareas agrupadas o listas de tareas relacionadas al radicado, los doatos de asig
        'nación de la tarea al usuario sin hacer cambios en el estado de tarea en la base de datos.
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdRuutaWorkflow           : Representa la identificaicón de la ruta workflow
        'IdActividadWorkflow       : Representa la identificación de la actividad workflow
        'NombreRutaWorkflow        :Representa el nombre de la ruta workflow
        'IdUsuarioWorkflow         :Representa la identificación del usuario workflow
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CdClassSeleccionTarea    : Retorna la estructura de la tarea seleccionada
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-09
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            Dim IdTareaWorkflow As Long = 0
            Result = Class_estados_tarea_workflow.SolicitaIdTareaAsignadaUsuarioWorkflow(IdActividadWorkflow,
                                                                                         IdActividadWorkflow,
                                                                                         IdTareaWorkflow)
            If Result <> "YES" Then
                If Result <> "YES" Then
                    SeleccionTareaInicioWorkflow = Result
                    Exit Function
                End If
            End If
            If IdTareaWorkflow = 0 Then
                SeleccionTareaInicioWorkflow = "YES"
                Exit Function
            End If
            Dim Class_configuracion_listado_ruta As New Class_configuracion_listado_ruta
            Dim NombreCampoRadicado As String = ""
            Result = Class_configuracion_listado_ruta.SolicitaNombreCampoRadicadoRuta(IdRutaWorkflow,
                                                                                      NombreCampoRadicado)
            If Result <> "YES" Then
                SeleccionTareaInicioWorkflow = Result
                Exit Function
            End If
            Dim RadicadoTarea As String = ""
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = Class_DAT_ADIC_TAR.SolicitaRadicadoTareaWorkflow(NombreCampoRadicado,
                                                                      NombreRutaWorkflow,
                                                                      IdTareaWorkflow,
                                                                      RadicadoTarea)
            If Result <> "YES" Then
                SeleccionTareaInicioWorkflow = Result
                Exit Function
            End If
            If RadicadoTarea = "" Then
                SeleccionTareaInicioWorkflow = "No fue posible localizar el consecutivo de radicado de la tarea (" & IdTareaWorkflow & "). Verifique que la tarea esté correctamente configurada y relacionada a un radicado"
                Exit Function
            End If
            Dim IdTipoFlujoTarea As Integer = 0
            Result = Class_DAT_ADIC_TAR.SolicitaIdTipoFlujoTareaWorkflow(IdTareaWorkflow,
                                                                         NombreRutaWorkflow,
                                                                         IdTipoFlujoTarea)
            If Result <> "YES" Then
                SeleccionTareaInicioWorkflow = Result
                Exit Function
            End If
            Dim TipoTareaPadre As Integer = 0
            Result = Class_DAT_ADIC_TAR.SolicitaTipoTareaPadreWorkflow(IdTareaWorkflow,
                                                                       NombreRutaWorkflow,
                                                                       TipoTareaPadre)
            If Result <> "YES" Then
                SeleccionTareaInicioWorkflow = Result
                Exit Function
            End If
            Dim NombreGabinete As String = ""
            Dim NombreCampoRadicadoGabnete As String = ""
            Dim AplicaTrd As Integer = 0
            Dim Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Dim Class_system1 As New Class_system1
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim Class_wf_rel_tarea_agrupada As New Class_wf_rel_tarea_agrupada
            Dim CdTareaListaAgrupaPadre As New CdTareaListaAgrupaPadre
            Dim class_stru_Row_Gabinete_Generic As New class_stru_Row_Gabinete_Generic
            If TipoTareaPadre = 1 Then
                Result = Class_wf_rel_tarea_agrupada.SolicitaListaEstructuraTareaAgrupadaPadre(IdTareaWorkflow,
                                                                                               NombreRutaWorkflow,
                                                                                               CdTareaListaAgrupaPadre)
                If Result <> "YES" Then
                    SeleccionTareaInicioWorkflow = Result
                    Exit Function
                End If
                CdClassSeleccionTarea.Obj_ilist_fileds_generic = CdTareaListaAgrupaPadre.Obj_ilist_fileds_generic
                CdClassSeleccionTarea.Obj_ilist_row_generic = CdTareaListaAgrupaPadre.Obj_ilist_row_generic
            Else
                Result = Class_DAT_ADIC_TAR.SolicitaNombreGabneteTareaWokflow(NombreRutaWorkflow,
                                                                              IdTareaWorkflow,
                                                                              NombreGabinete)
                If Result <> "YES" Then
                    SeleccionTareaInicioWorkflow = Result
                    Exit Function
                End If
                Result = Class_DETALLE_GABIENETE.SolicitaNombreCampoRadicadoGabinete(NombreGabinete,
                                                                                     NombreCampoRadicadoGabnete)
                If Result <> "YES" Then
                    SeleccionTareaInicioWorkflow = Result
                    Exit Function
                End If
                Result = Class_system1.VerificaOpcionAplicarTablaRetencion(AplicaTrd,
                                                                           NombreGabinete)
                If Result <> "YES" Then
                    SeleccionTareaInicioWorkflow = Result
                    Exit Function
                End If
                Result = ClassDaGabinete.SolicitaDocumentosRelacionadosRadicadoEnlace(NombreCampoRadicado,
                                                                                      NombreGabinete,
                                                                                      RadicadoTarea,
                                                                                      AplicaTrd,
                                                                                      class_stru_Row_Gabinete_Generic)
                If Result <> "YES" Then
                    SeleccionTareaInicioWorkflow = Result
                    Exit Function
                End If
                CdClassSeleccionTarea.Obj_ilist_fileds_generic = class_stru_Row_Gabinete_Generic.Obj_ilist_fileds_generic
                CdClassSeleccionTarea.Obj_ilist_row_generic = class_stru_Row_Gabinete_Generic.Obj_ilist_row_generic
            End If
            Dim Class_TareasWorkflow As New Class_TareasWorkflow
            CdClassSeleccionTarea.CdTareasWorkflow = New CdTareasWorkflow
            Dim IdFlujoTarea As Integer = 0
            Result = Class_TareasWorkflow.SolicitaDaosTareaAsignadaWorkflow(IdTareaWorkflow,
                                                                           IdRutaWorkflow,
                                                                           IdUsuarioWorkflow,
                                                                           NombreRutaWorkflow,
                                                                           IdActividadWorkflow,
                                                                           RadicadoTarea,
                                                                           IdFlujoTarea,
                                                                           CdClassSeleccionTarea.CdTareasWorkflow)
            If Result <> "YES" Then
                SeleccionTareaInicioWorkflow = Result
                Exit Function
            End If
            SeleccionTareaInicioWorkflow = "YES"
            Exit Function
        Catch ex As Exception
            SeleccionTareaInicioWorkflow = "Inconsistencia general funcion SeleccionTareaInicioWorkflow " & ex.Message
        End Try
    End Function
    Function SeleccionTareaWorkflow(ByVal IdTareaWorkflow As Long,
                                    ByVal IdRuutaWorkflow As Integer,
                                    ByVal IdActividadWorkflow As Integer,
                                    ByVal IdActividadUsuario As Integer,
                                    ByVal idTipoActividad As Integer,
                                    ByVal TipoAsignacion As Integer,
                                    ByVal NombreRutaWorkflow As String,
                                    ByVal IdUsuarioWorkflow As Integer,
                                    ByVal IdGrupoWorkflow As Integer,
                                    ByVal EvaluaEventoEnlace As Integer,
                                    ByVal CodeEventoEnlace As String,
                                    ByRef CdClassSeleccionTarea As CdClassSeleccionTarea) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Selecciona tarea workflow y retorna los datos estructuruados de asignación como
        'la lista de tareas agrupadas o listas de tareas relacionadas al radicado, los doatos de asig
        'nación de la tarea al usuario haciendo cambios en el estado en la base de datos.
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTareaWorkflow           : Representa la identificación de la tarea workflow
        'IdRuutaWorkflow           : Representa la identificaicón de la ruta workflow
        'IdActividadWorkflow       : Representa la identificación de la actividad workflow
        'IdActividadUsuario        : Representa la identificación de la actividad del usuario workflow
        'idTipoActividad           : Representa la identificación del tipo de actividad worflow
        'TipoAsignacion            : Representa el tipo de asignación de la tarea 1-Tarea asignada curso
        '                            normal  2-Tarea seleccionada desde recuperación 3-Tarea seleccionar 
        '                            desde el estado pendiente
        'NombreRutaWorkflow        :Representa el nombre de la ruta workflow
        'IdUsuarioWorkflow         :Representa la identificación del usuario workflow
        'IdGrupoWorkflow           :Representa la identificacion del grupo workflow
        'EvaluaEventoEnlace        :Representa la evaluación del evento enlace para tareas tipo enlace
        'CodeEventoEnlace          :Representa la evaluación del codigo de enlace
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CdClassSeleccionTarea    : Retorna la estructura de la tarea seleccionada
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-09
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            Dim EstadoTareaSeleccionda As Integer = 0
            Result = Class_estados_tarea_workflow.SolicitaEstadoTareaAsignadaUsuarioWorkflow(IdActividadUsuario,
                                                                                             IdUsuarioWorkflow,
                                                                                             EstadoTareaSeleccionda)
            If Result <> "YES" Then
                SeleccionTareaWorkflow = Result
                Exit Function
            End If
            If EstadoTareaSeleccionda = 1 Then
                SeleccionTareaWorkflow = "El usuario ya tiene una tarea seleccionada. No es posible asignarle una nueva tarea."
                Exit Function
            End If
            Dim StruUsuarioTareaAsignada As StruUsuarioTareaAsignada = Nothing
            Result = Class_estados_tarea_workflow.SolicitaEstructuraUsuarioTareaAsignada(IdTareaWorkflow,
                                                                                        IdUsuarioWorkflow,
                                                                                        IdGrupoWorkflow,
                                                                                        StruUsuarioTareaAsignada)
            If Result <> "YES" Then
                SeleccionTareaWorkflow = Result
                Exit Function
            End If
            If Not StruUsuarioTareaAsignada.CargoUsuario Is Nothing Then
                SeleccionTareaWorkflow = "No es posible continuar: el usuario " & StruUsuarioTareaAsignada.LoginUsuario & " (" & StruUsuarioTareaAsignada.CargoUsuario & ") ya tiene la tarea seleccionada, por favor actualizar la lista de tareas."
                Exit Function
            End If
            Dim Class_flujo_trabajo_workflow As New Class_flujo_trabajo_workflow
            Dim IdFlujoTrabajo As Integer = 0
            Dim IdActividadFlujoTrabajo As Integer = 0
            Dim IdUsuarioWorkflowFlujoTrabajo As Integer = 0
            '//----Valida exitencia de la tarea del usuario en el flujo, Si no pertence se sale de la asignación-----/////
            If TipoAsignacion = 2 Then
                Result = Class_flujo_trabajo_workflow.SolicitaExistenciaUsuarioFlujoTarea(NombreRutaWorkflow,
                                                                                          IdTareaWorkflow,
                                                                                          IdUsuarioWorkflow,
                                                                                          IdActividadUsuario,
                                                                                          IdActividadFlujoTrabajo,
                                                                                          IdFlujoTrabajo,
                                                                                          IdUsuarioWorkflowFlujoTrabajo)
                If Result <> "YES" Then
                    SeleccionTareaWorkflow = Result
                    Exit Function
                End If
            End If
            Dim Class_configuracion_listado_ruta As New Class_configuracion_listado_ruta
            Dim NombreCampoRadicado As String = ""
            Result = Class_configuracion_listado_ruta.SolicitaNombreCampoRadicadoRuta(IdRuutaWorkflow,
                                                                                      NombreCampoRadicado)
            If Result <> "YES" Then
                SeleccionTareaWorkflow = Result
                Exit Function
            End If
            Dim RadicadoTarea As String = ""
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = Class_DAT_ADIC_TAR.SolicitaRadicadoTareaWorkflow(NombreCampoRadicado,
                                                                      NombreRutaWorkflow,
                                                                      IdTareaWorkflow,
                                                                      RadicadoTarea)
            If Result <> "YES" Then
                SeleccionTareaWorkflow = Result
                Exit Function
            End If
            If RadicadoTarea = "" Then
                SeleccionTareaWorkflow = "No fue posible localizar el consecutivo de radicado de la tarea (" & IdTareaWorkflow & "). Verifique que la tarea esté correctamente configurada y relacionada a un radicado"
                Exit Function
            End If
            Dim IdTipoFlujoTarea As Integer = 0
            Result = Class_DAT_ADIC_TAR.SolicitaIdTipoFlujoTareaWorkflow(IdTareaWorkflow,
                                                                         NombreRutaWorkflow,
                                                                         IdTipoFlujoTarea)
            If Result <> "YES" Then
                SeleccionTareaWorkflow = Result
                Exit Function
            End If
            Dim ClassEdtiScript As New ClassEdtiScript

            Dim Class_configuracion_gabinete As New Class_configuracion_gabinete
            Dim ClassDaGabinete As New ClassDaGabinete
            If EvaluaEventoEnlace = 1 Then
                '---------Ejecuta el escript enlace
                If CodeEventoEnlace <> "" Then
                    '--------------------------------------------------------
                    'Actualiza los indices de las imagenes por código
                    '---------------------------------------------------------
                    Result = ClassEdtiScript.EjecutaEventoEnlaceDocumentosWorkflow(IdTareaWorkflow,
                                                                                   IdActividadUsuario,
                                                                                   CodeEventoEnlace)
                    If Result <> "YES" Then
                        SeleccionTareaWorkflow = Result
                        Exit Function
                    End If
                Else
                    Result = ClassDaGabinete.ActualizaIndiceDocumentosGabineteRleacionadoTareaWorkflow(IdTareaWorkflow,
                                                                                                       NombreRutaWorkflow,
                                                                                                       RadicadoTarea)
                    If Result <> "YES" Then
                        SeleccionTareaWorkflow = Result
                        Exit Function
                    End If
                End If
            End If
            '//--------Ejecuta eventos compilados en tiempo de ejecución----////
            Result = ClassEdtiScript.EjecutaEventosTareaWorkflow(IdActividadUsuario,
                                                                 IdGrupoWorkflow,
                                                                 IdUsuarioWorkflow,
                                                                 IdTareaWorkflow)
            If Result <> "YES" Then
                SeleccionTareaWorkflow = Result
                Exit Function
            End If
            Dim TipoTareaPadre As Integer = 0
            Result = Class_DAT_ADIC_TAR.SolicitaTipoTareaPadreWorkflow(IdTareaWorkflow,
                                                                       NombreRutaWorkflow,
                                                                       TipoTareaPadre)
            If Result <> "YES" Then
                SeleccionTareaWorkflow = Result
                Exit Function
            End If
            Dim NombreGabinete As String = ""
            Dim NombreCampoRadicadoGabnete As String = ""
            Dim AplicaTrd As Integer = 0
            Dim Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Dim Class_system1 As New Class_system1
            Dim Class_wf_rel_tarea_agrupada As New Class_wf_rel_tarea_agrupada
            Dim CdTareaListaAgrupaPadre As New CdTareaListaAgrupaPadre
            Dim class_stru_Row_Gabinete_Generic As New class_stru_Row_Gabinete_Generic
            If TipoTareaPadre = 1 Then
                Result = Class_wf_rel_tarea_agrupada.SolicitaListaEstructuraTareaAgrupadaPadre(IdTareaWorkflow,
                                                                                               NombreRutaWorkflow,
                                                                                               CdTareaListaAgrupaPadre)
                If Result <> "YES" Then
                    SeleccionTareaWorkflow = Result
                    Exit Function
                End If
                CdClassSeleccionTarea.Obj_ilist_fileds_generic = CdTareaListaAgrupaPadre.Obj_ilist_fileds_generic
                CdClassSeleccionTarea.Obj_ilist_row_generic = CdTareaListaAgrupaPadre.Obj_ilist_row_generic
            Else
                Result = Class_DAT_ADIC_TAR.SolicitaNombreGabneteTareaWokflow(NombreRutaWorkflow,
                                                                              IdTareaWorkflow,
                                                                              NombreGabinete)
                If Result <> "YES" Then
                    SeleccionTareaWorkflow = Result
                    Exit Function
                End If
                Result = Class_DETALLE_GABIENETE.SolicitaNombreCampoRadicadoGabinete(NombreGabinete,
                                                                                     NombreCampoRadicadoGabnete)
                If Result <> "YES" Then
                    SeleccionTareaWorkflow = Result
                    Exit Function
                End If
                Result = Class_system1.VerificaOpcionAplicarTablaRetencion(AplicaTrd,
                                                                           NombreGabinete)
                If Result <> "YES" Then
                    SeleccionTareaWorkflow = Result
                    Exit Function
                End If
                Result = ClassDaGabinete.SolicitaDocumentosRelacionadosRadicadoEnlace(NombreCampoRadicado,
                                                                                      NombreGabinete,
                                                                                      RadicadoTarea,
                                                                                      AplicaTrd,
                                                                                      class_stru_Row_Gabinete_Generic)
                If Result <> "YES" Then
                    SeleccionTareaWorkflow = Result
                    Exit Function
                End If
                CdClassSeleccionTarea.Obj_ilist_fileds_generic = class_stru_Row_Gabinete_Generic.Obj_ilist_fileds_generic
                CdClassSeleccionTarea.Obj_ilist_row_generic = class_stru_Row_Gabinete_Generic.Obj_ilist_row_generic
            End If
            Dim Class_TareasWorkflow As New Class_TareasWorkflow
            CdClassSeleccionTarea.CdTareasWorkflow = New CdTareasWorkflow
            Dim IdFlujoTarea As Integer = 0
            Result = Class_TareasWorkflow.SolicitaDaosTareaAsignadaWorkflow(IdTareaWorkflow,
                                                                    IdRuutaWorkflow,
                                                                    IdUsuarioWorkflow,
                                                                    NombreRutaWorkflow,
                                                                    IdActividadWorkflow,
                                                                    RadicadoTarea,
                                                                    IdFlujoTarea,
                                                                    CdClassSeleccionTarea.CdTareasWorkflow)
            If Result <> "YES" Then
                SeleccionTareaWorkflow = Result
                Exit Function
            End If
            Dim IdPendiente As Integer = 0
            Dim stru_campo_tarea_() As stru_campo_tarea = Nothing
            Result = Me.AsignaTareaWorkflow(IdTareaWorkflow,
                                            IdActividadWorkflow,
                                            IdActividadUsuario,
                                            IdRuutaWorkflow,
                                            NombreRutaWorkflow,
                                            RadicadoTarea,
                                            TipoAsignacion,
                                            IdUsuarioWorkflow,
                                            IdFlujoTrabajo,
                                            IdActividadFlujoTrabajo,
                                            IdUsuarioWorkflowFlujoTrabajo,
                                            IdPendiente,
                                            stru_campo_tarea_)
            SeleccionTareaWorkflow = Result
            Exit Function
        Catch ex As Exception
            SeleccionTareaWorkflow = "Inconsistencia general funcion SeleccionTareaWorkflow " & ex.Message
        End Try
    End Function
    Function Seleccion_tarea_workflow(ByRef Gred As GridView,
                                      ByVal Labttex As String,
                                      ByRef Resultado As String,
                                      ByVal nat As String,
                                      ByRef mEval As Object,
                                      ByRef Page As Page,
                                      ByVal evalua_reasigna_respuesta_recuperacion_tarea As Integer,
                                      ByVal resultado_correo As String,
                                      ByVal evalua_no_disponible_ignorar As Integer,
                                      ByVal evalua_seleccionar_tarea_enlace As Integer,
                                      ByVal evalua_pertenencia_flujo_trabajo As Integer,
                                      ByVal id_actividad_tarea As Integer,
                                      ByVal id_tarea As Long,
                                      ByVal determina_tipo_selecion As Integer,
                                      ByVal evalua_permiso_asignacion_respuesta As Integer,
                                      ByVal evalua_actualiza_indices_documentos_enlace As Integer,
                                      ByVal option_solo_actualiza_indices_documentos_enlace As Integer,
                                      ByVal evalua_estado_tarea_recuperada As Integer,
                                      ByRef stru_campo_tarea_() As stru_campo_tarea) As String
        '***************************************************
        'Funcion : selecciona las tareas pendientes lista
        'das en los gredview 1 y 2 activa el popup enlace
        'para las actividades de enlaces y asiga documentos
        'a el usuario
        '***************************************************
        Dim Mens As New Classscrripjava
        Dim Refclas As New Classselecciotarea
        Dim Result As String = ""
        Dim RES As String = ""
        Dim Index As Integer = -1
        Dim TipoActividad As String = ""
        Dim id_tipo_flujo_trabajo As Integer = 1
        Try
            Dim Hidden_id_tarea_sel As Object = Page.FindControl("Hidden_id_tarea_sel")
            If Hidden_id_tarea_sel Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (Hidden_id_tarea_sel)"
                Exit Function
            End If
            Dim IframeDitaliza As Object = Page.FindControl("IframeDitaliza_")
            If IframeDitaliza Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (IframeDitaliza_)"
                Exit Function
            End If
            Dim UpdatePanel_iframe_digitaliza As UpdatePanel = Page.FindControl("UpdatePanel_iframe_digitaliza")
            If UpdatePanel_iframe_digitaliza Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (UpdatePanel_iframe_digitaliza)"
                Exit Function
            End If
            Dim Label_estado_enlace As Label = Page.FindControl("Label_estado_enlace")
            If Label_estado_enlace Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (Label_estado_enlace)"
                Exit Function
            End If
            Dim UpdatePanel_estado_enlace As UpdatePanel = Page.FindControl("UpdatePanel_estado_enlace")
            If UpdatePanel_estado_enlace Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (UpdatePanel_estado_enlace)"
                Exit Function
            End If
            Dim UpdateDatos As UpdatePanel = Page.FindControl("UpdateDatos")
            If UpdateDatos Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (UpdateDatos)"
                Exit Function
            End If
            Dim ModalPopupExtender_edition_admon_documentos As AjaxControlToolkit.ModalPopupExtender = Page.FindControl("ModalPopupExtender_edition_admon_documentos")
            If ModalPopupExtender_edition_admon_documentos Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (ModalPopupExtender_edition_admon_documentos)"
                Exit Function
            End If
            Dim TreeViewseleccion As TreeView = Page.FindControl("TreeViewseleccion")
            If TreeViewseleccion Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (TreeViewseleccion)"
                Exit Function
            End If
            Dim Hidden_id_tarea_selecionada As Object = Page.FindControl("Hidden_id_tarea_selecionada")
            If Hidden_id_tarea_selecionada Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (Hidden_id_tarea_selecionada)"
                Exit Function
            End If
            Dim UpdatePanel_general_variable As UpdatePanel = Page.FindControl("UpdatePanel_general_variable")
            If UpdatePanel_general_variable Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (UpdatePanel_general_variable)"
                Exit Function
            End If

            Dim LabelEspera As Label = Page.FindControl("LabelEspera")
            If LabelEspera Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (LabelEspera)"
                Exit Function
            End If
            Dim Hidden_resultado_selecion As Object = Page.FindControl("Hidden_resultado_selecion")
            If Hidden_resultado_selecion Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (Hidden_resultado_selecion)"
                Exit Function
            End If
            Dim UpdatePanelnumeroespera As UpdatePanel = Page.FindControl("UpdatePanelnumeroespera")
            If UpdatePanelnumeroespera Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (UpdatePanelnumeroespera)"
                Exit Function
            End If
            Dim UpdatePanelseleccion As UpdatePanel = Page.FindControl("UpdatePanelseleccion")
            If UpdatePanelseleccion Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (UpdatePanelseleccion)"
                Exit Function
            End If
            Dim HiddenIdFlujo As Object = Page.FindControl("HiddenIdFlujo")
            If HiddenIdFlujo Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (HiddenIdFlujo)"
                Exit Function
            End If
            Dim Panel_EnviaActividad As Panel = Page.FindControl("Panel_EnviaActividad")
            If Panel_EnviaActividad Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (Panel_EnviaActividad)"
                Exit Function
            End If
            Dim Panel_EnviarUsuario As Panel = Page.FindControl("Panel_EnviarUsuario")
            If Panel_EnviarUsuario Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (Panel_EnviarUsuario)"
                Exit Function
            End If
            Dim Panel_devolver_tarea As Panel = Page.FindControl("Panel_devolver_tarea")
            If Panel_devolver_tarea Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (Panel_devolver_tarea)"
                Exit Function
            End If
            Dim Panel_autoterminar As Panel = Page.FindControl("Panel_autoterminar")
            If Panel_autoterminar Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (Panel_autoterminar)"
                Exit Function
            End If
            Dim Panel_autoriza As Panel = Page.FindControl("Panel_autoriza")
            If Panel_autoriza Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (Panel_autoriza)"
                Exit Function
            End If
            Dim updatemenu As UpdatePanel = Page.FindControl("updatemenu")
            If updatemenu Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (updatemenu)"
                Exit Function
            End If
            Dim UpdatePanel_estado_tarea As UpdatePanel = Page.FindControl("UpdatePanel_estado_tarea")
            If UpdatePanel_estado_tarea Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (UpdatePanel_estado_tarea)"
                Exit Function
            End If

            Dim Label_estado_tarea_selecion As Label = Page.FindControl("Label_estado_tarea_selecion")
            If Label_estado_tarea_selecion Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (Label_estado_tarea_selecion)"
                Exit Function
            End If
            Dim GridView_list_documento_relacion As GridView = Page.FindControl("GridView_list_documento_relacion")
            If GridView_list_documento_relacion Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (GridView_list_documento_relacion)"
                Exit Function
            End If
            Dim hiden_seleccion_documento As HtmlInputHidden = Page.FindControl("hiden_seleccion_documento")
            If hiden_seleccion_documento Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (hiden_seleccion_documento)"
                Exit Function
            End If
            Dim Hidden_numero_doc_rel As HtmlInputHidden = Page.FindControl("Hidden_numero_doc_rel")
            If Hidden_numero_doc_rel Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (Hidden_numero_doc_rel)"
                Exit Function
            End If
            Dim UpdatePanelseleccion_digitalizado As UpdatePanel = Page.FindControl("UpdatePanelseleccion_digitalizado")
            If UpdatePanelseleccion_digitalizado Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (UpdatePanelseleccion_digitalizado)"
                Exit Function
            End If
            Dim UpdatePanelseleccion_label_documentos As UpdatePanel = Page.FindControl("UpdatePanelseleccion_label_documentos")
            If UpdatePanelseleccion_label_documentos Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (UpdatePanelseleccion_label_documentos)"
                Exit Function
            End If
            Dim Label_documentos As Label = Page.FindControl("Label_documentos")
            If Label_documentos Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (Label_documentos)"
                Exit Function
            End If
            Dim ref_TextBox_login_autoriza_reasignacion_tarea_recuperada As TextBox = Page.FindControl("TextBox_login_autoriza_reasignacion_tarea_recuperada")
            If ref_TextBox_login_autoriza_reasignacion_tarea_recuperada Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (TextBox_login_autoriza_reasignacion_tarea_recuperada)"
                Exit Function
            End If
            Dim ref_TextBox_pasw_autoriza_reasignacion_tarea_recuperada As TextBox = Page.FindControl("TextBox_pasw_autoriza_reasignacion_tarea_recuperada")
            If ref_TextBox_pasw_autoriza_reasignacion_tarea_recuperada Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (TextBox_pasw_autoriza_reasignacion_tarea_recuperada)"
                Exit Function
            End If
            Dim ref_ModalPopupExtender_edition_autoriza_reasignacion_tarea_recuperada As AjaxControlToolkit.ModalPopupExtender = Page.FindControl("ModalPopupExtender_edition_autoriza_reasignacion_tarea_recuperada")
            If ref_ModalPopupExtender_edition_autoriza_reasignacion_tarea_recuperada Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (ModalPopupExtender_edition_autoriza_reasignacion_tarea_recuperada)"
                Exit Function
            End If
            Dim ref_UpdatePanel_autoriza_reasignacion_tarea_recuperada As UpdatePanel = Page.FindControl("UpdatePanel_autoriza_reasignacion_tarea_recuperada")
            If ref_UpdatePanel_autoriza_reasignacion_tarea_recuperada Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (UpdatePanel_autoriza_reasignacion_tarea_recuperada)"
                Exit Function
            End If
            Dim ref_ModalPopupExtender_edition_autoriza_reasignacion_tarea_recuperada_enlazada As AjaxControlToolkit.ModalPopupExtender = Page.FindControl("ModalPopupExtender_edition_autoriza_reasignacion_tarea_recuperada_enlazada")
            If ref_ModalPopupExtender_edition_autoriza_reasignacion_tarea_recuperada_enlazada Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (ModalPopupExtender_edition_autoriza_reasignacion_tarea_recuperada_enlazada)"
                Exit Function
            End If
            Dim ref_TextBox_login_autoriza_reasignacion_tarea_recuperada_enlazada As TextBox = Page.FindControl("TextBox_login_autoriza_reasignacion_tarea_recuperada_enlazada")
            If ref_TextBox_login_autoriza_reasignacion_tarea_recuperada_enlazada Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (TextBox_login_autoriza_reasignacion_tarea_recuperada_enlazada)"
                Exit Function
            End If
            Dim ref_TextBox_pasw_autoriza_reasignacion_tarea_recuperada_enlazada As TextBox = Page.FindControl("TextBox_pasw_autoriza_reasignacion_tarea_recuperada_enlazada")
            If ref_TextBox_pasw_autoriza_reasignacion_tarea_recuperada_enlazada Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (TextBox_pasw_autoriza_reasignacion_tarea_recuperada_enlazada)"
                Exit Function
            End If
            Dim ref_UpdatePanel_autoriza_reasignacion_tarea_recuperada_enlazada As UpdatePanel = Page.FindControl("UpdatePanel_autoriza_reasignacion_tarea_recuperada_enlazada")
            If ref_UpdatePanel_autoriza_reasignacion_tarea_recuperada_enlazada Is Nothing Then
                Seleccion_tarea_workflow = "Imposible encontrar el control (UpdatePanel_autoriza_reasignacion_tarea_recuperada_enlazada)"
                Exit Function
            End If
            Index = Val(nat)
            Dim Actividad_Seleccion As Integer = 0
            Result = ""
            If HttpContext.Current.Session("Id_Grupo_Workflow") = 0 Then
                Seleccion_tarea_workflow = "Su sesión caduco o fue imposible identificar el grupo al que usted pertenece "
                Exit Function
            End If
            Dim Ref_Class_grupos_workflow As New Class_grupos_workflow
            Dim id_actividad_usuario_logueado As Integer = 0
            Result = Ref_Class_grupos_workflow.Solicita_id_actividad_usuario_workflow(id_actividad_usuario_logueado,
                                                                                      HttpContext.Current.Session("Id_Grupo_Workflow"))
            If Result <> "YES" Then
                Seleccion_tarea_workflow = "Error #10 SELECCIONA-WF Imposible Obtener Id actividad " & Result
                Exit Function
            End If
            '---------------------------------------------------------------------
            'Determinar tarea asignda por usuario 

            '---------------------------------------------------------------------
            'Determina el tipo de actividad a la que pertenece el usuario
            '---------------------------------------------------------------------
            Dim Class_Listado_Actividades_workflow As New Class_Listado_Actividades_workflow
            Result = Class_Listado_Actividades_workflow.SolicitaNombreTipoActividadGeneralWorkflow(id_actividad_usuario_logueado,
                                                                                                   TipoActividad)
            If Result <> "YES" Then
                Seleccion_tarea_workflow = Result
                Exit Function
            End If
            Dim Radicado As String = ""
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(id_tarea,
                                                                                    Radicado)
            If Result <> "YES" Then
                Seleccion_tarea_workflow = Result
                Exit Function
            End If
            Dim id_respuesta As Integer = 0
            '---------------------------------------------------------------------------
            'Evalua la reasingación de una respuesta en caso de recuperación de tarea
            '---------------------------------------------------------------------------
            Dim Class_ra_respuesta_radicado As New Class_ra_respuesta_radicado
            If evalua_reasigna_respuesta_recuperacion_tarea = 1 Then
                'Dim Refclas_resp As New Classgestionrespuesta
                Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado_usuario_no_propietario(Radicado,
                                                                                                          id_respuesta)
                If Result <> "YES" Then
                    Seleccion_tarea_workflow = Result
                    Exit Function
                End If
                If id_respuesta <> 0 Then
                    '-----------------------------------------------
                    'Retorna datos estructura respuesta radicado
                    '-----------------------------------------------
                    Dim stru_envio As stru_envio = Nothing
                    Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
                    Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta,
                                                                                                stru_envio)
                    If Result <> "YES" Then
                        Seleccion_tarea_workflow = Result
                        Exit Function
                    End If
                    '
                    If stru_envio.FECHA_RESPUETA <> "" Then
                        id_respuesta = 0
                    End If
                    '-----------------------------------------------
                    'Verifica estado solicitudes de aprobación sin
                    'desición
                    '-----------------------------------------------
                    Dim Estado_solicitud_aprobacion As String = ""
                    Dim ref_class_solicitud As New ClassRaSolicitudesAprobacion
                    If id_respuesta <> 0 Then
                        Result = ref_class_solicitud.Verifica_solicitudes_de_aprobacion_sin_desicion(id_tarea,
                                                                                                     Estado_solicitud_aprobacion,
                                                                                                     stru_envio.ID_REMIT_DEST_INT)
                        If Result <> "YES" Then
                            Seleccion_tarea_workflow = Result
                            Exit Function
                        End If
                        If Estado_solicitud_aprobacion = "YES" Then
                            Seleccion_tarea_workflow = "Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar"
                            Exit Function
                        End If
                    End If
                    '--------------------------------------------------------------------------
                    'Activa ventana autorización de asignación si el usuario no tiene permiso
                    'si la tarea no recupera desde una actividad que no pertenezca a enlace
                    '--------------------------------------------------------------------------
                    If id_respuesta <> 0 And TipoActividad <> "ENLASE" And evalua_permiso_asignacion_respuesta = 1 Then
                        If HttpContext.Current.Session.Item("REASIGNA_TAREA_WORKFLOW") = 0 Then
                            ref_TextBox_login_autoriza_reasignacion_tarea_recuperada.Text = ""
                            ref_TextBox_pasw_autoriza_reasignacion_tarea_recuperada.Text = ""
                            ref_UpdatePanel_autoriza_reasignacion_tarea_recuperada.Update()
                            ref_ModalPopupExtender_edition_autoriza_reasignacion_tarea_recuperada.Show()
                            Seleccion_tarea_workflow = "AUTORIZA RECUPERA"
                            Exit Function
                        End If
                    End If
                    '--------------------------------------------------------------------------
                    'Activa ventana autorización de asignación si el usuario no tiene permiso
                    'si la tarea no recupera desde una actividad de tipo enlace recuperada
                    '--------------------------------------------------------------------------
                    If id_respuesta <> 0 And TipoActividad = "ENLASE" And evalua_permiso_asignacion_respuesta = 1 And evalua_actualiza_indices_documentos_enlace = 1 Then
                        If HttpContext.Current.Session.Item("REASIGNA_TAREA_WORKFLOW") = 0 Then
                            ref_TextBox_login_autoriza_reasignacion_tarea_recuperada_enlazada.Text = ""
                            ref_TextBox_pasw_autoriza_reasignacion_tarea_recuperada_enlazada.Text = ""
                            ref_UpdatePanel_autoriza_reasignacion_tarea_recuperada_enlazada.Update()
                            ref_ModalPopupExtender_edition_autoriza_reasignacion_tarea_recuperada_enlazada.Show()
                            Seleccion_tarea_workflow = "AUTORIZA RECUPERA"
                            Exit Function
                        End If
                    End If
                End If
            End If
            '-----Retorna el tipo de flujo interno o externo
            Dim refclas_workflow_digitalizacion As New ClassWorkflowDigitalizacion
            Dim id_tipo_flujo As Integer = 0
            Dim refclas_dat_adit As New Class_DAT_ADIC_TAR
            Result = refclas_dat_adit.SolicitaIdTipoFlujoTareaWorkflow(id_tarea,
                                                                       HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                       id_tipo_flujo)
            If Result <> "YES" Then
                Seleccion_tarea_workflow = Result
                Exit Function
            End If
            '-------------------------------------------------------------
            'Solicita estado pendiente tarea
            '-------------------------------------------------------------
            Dim id_pendiente As Integer = -1
            Dim Class_tarea_pendiente As New Class_tarea_pendiente
            Result = Class_tarea_pendiente.Solicita_estado_pendiente_tarea_workflow(id_tarea,
                                                                                    id_pendiente)
            If Result <> "YES" Then
                Seleccion_tarea_workflow = Result
                Exit Function
            End If
            '---- CASO POPUP ENLACE WORKFLOW 
            If TipoActividad = "ENLASE" And evalua_seleccionar_tarea_enlace = 1 Then
                HttpContext.Current.Session("ID_TAREA_SELECCIONDA_ENLACE") = id_tarea
                HttpContext.Current.Session("SELECCIONTEMPORAL") = id_tarea & "|" & id_actividad_tarea & "|" & Index & "|" & TipoActividad
                HttpContext.Current.Session("WF_TAGSELECCION_EMERGENTE") = ""
                HiddenIdFlujo.Value = HttpContext.Current.Session("SELECCIONTEMPORAL")
                HttpContext.Current.Session("DG_TIPODIGITALIZACION") = ""
                HttpContext.Current.Session("DG_ID_TRAMITE") = 0
                HttpContext.Current.Session("DG_TIPO_TRAMITE") = ""
                HttpContext.Current.Session("DG_ID_GABINETE") = 0
                HttpContext.Current.Session("DG_NOMBRE_GABINETE") = ""
                HttpContext.Current.Session("DG_RADICADO") = ""
                HttpContext.Current.Session("DG_LISTA_CHEQUEO") = -1
                HttpContext.Current.Session("DG_ID_CONFIG_DIGITALIZACION") = -1
                HttpContext.Current.Session("DG_SELECION_TREE") = ""
                HttpContext.Current.Session("DG_NOMBRE_TRAMITE") = ""
                Dim Ruta_Web_Escaner As String = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER").ToString.Replace("\", "/")
                Dim Refclas_digitalizacion As New ClassWorkflowDigitalizacion
                '---------------------------------------------------------------
                'Asigna confgiguración de los parametros para listar documentos
                '---------------------------------------------------------------
                If id_tipo_flujo = 1 Then
                    Result = Refclas_digitalizacion.SolicitaParametrosParaListartiposDocumentalesTareaWorkflowInterna(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                                                                      id_tarea,
                                                                                                                      HttpContext.Current.Session("DG_TIPO_TRAMITE"),
                                                                                                                      HttpContext.Current.Session("DG_ID_TRAMITE"),
                                                                                                                      HttpContext.Current.Session("DG_ID_GABINETE"),
                                                                                                                      HttpContext.Current.Session("DG_NOMBRE_GABINETE"),
                                                                                                                      HttpContext.Current.Session("DG_ID_CONFIG_DIGITALIZACION"),
                                                                                                                      HttpContext.Current.Session("DG_RADICADO"),
                                                                                                                      HttpContext.Current.Session("DG_NOMBRE_TRAMITE"))
                    HttpContext.Current.Session("DG_TIPODIGITALIZACION") = "TRAMITE"
                    If HttpContext.Current.Session("DG_ID_TRAMITE") = 0 Then
                        HttpContext.Current.Session("DG_TRAMITE_DIGITAIZACION") = -1
                    Else
                        HttpContext.Current.Session("DG_TRAMITE_DIGITAIZACION") = HttpContext.Current.Session("DG_ID_TRAMITE")
                    End If
                Else
                    Result = refclas_workflow_digitalizacion.SolicitaParametrosParaListartiposDocumentalesTareaWorkflowExterna(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                                                                              id_tarea,
                                                                                                                              HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                                                                              HttpContext.Current.Session("DG_TIPO_TRAMITE"),
                                                                                                                              HttpContext.Current.Session("DG_ID_GABINETE"),
                                                                                                                              HttpContext.Current.Session("DG_NOMBRE_GABINETE"),
                                                                                                                              HttpContext.Current.Session("DG_RADICADO"),
                                                                                                                              HttpContext.Current.Session("DG_NOMBRE_TRAMITE"))
                    If Result <> "YES" Then
                        Seleccion_tarea_workflow = Result
                        Exit Function
                    End If
                    Dim ref_class_tipo_doc_entrante As New Class_tipo_doc_entrante
                    Result = ref_class_tipo_doc_entrante.RetornaIdTipoTramitePorNombreTipo(HttpContext.Current.Session("DG_NOMBRE_TRAMITE"),
                                                                                           HttpContext.Current.Session("DG_ID_TRAMITE"))
                    If Result <> "YES" Then
                        Seleccion_tarea_workflow = Result
                        Exit Function
                    End If
                    Dim Refclas_config As New Class_ra_dig_config_digitalizacion
                    Result = Refclas_config.Solicita_id_configuracion_digitalizacion(HttpContext.Current.Session("DG_ID_TRAMITE"),
                                                                                     "RADICACION ENTRANTE",
                                                                                     HttpContext.Current.Session("DG_ID_CONFIG_DIGITALIZACION"),
                                                                                     0)
                    If Result <> "YES" Then
                        Seleccion_tarea_workflow = Result
                        Exit Function
                    End If
                    HttpContext.Current.Session("DG_TIPODIGITALIZACION") = "TRAMITE"
                    If HttpContext.Current.Session("DG_ID_TRAMITE") = 0 Then
                        HttpContext.Current.Session("DG_TRAMITE_DIGITAIZACION") = -1
                    Else
                        HttpContext.Current.Session("DG_TRAMITE_DIGITAIZACION") = HttpContext.Current.Session("DG_ID_TRAMITE")
                    End If
                End If
                Dim Ref_class_detalle_gabinete As New Class_DETALLE_GABIENETE
                Dim nombre_campo_radicado_gabinete As String = ""
                Result = Ref_class_detalle_gabinete.SolicitaNombreCampoRadicadoGabinete(HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                        nombre_campo_radicado_gabinete)
                If Result <> "YES" Then
                    Seleccion_tarea_workflow = Result
                    Exit Function
                End If
                Dim Ref_class_system1 As New Class_system1
                Dim inventario_documental As Integer
                Dim aplica_trd As Integer
                Dim asigna_unidad As Integer
                Result = Ref_class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                                             inventario_documental,
                                                                                                             aplica_trd,
                                                                                                             asigna_unidad)
                If Result <> "YES" Then
                    Seleccion_tarea_workflow = Result
                    Exit Function
                End If
                Dim ref_class_da_gabinete As New ClassDaGabinete
                Result = ref_class_da_gabinete.Lista_documentos_relacionados_a_radicado_enlace(nombre_campo_radicado_gabinete,
                                                                                               HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                               HttpContext.Current.Session("DG_RADICADO"),
                                                                                               aplica_trd,
                                                                                               id_tarea,
                                                                                               1,
                                                                                               GridView_list_documento_relacion,
                                                                                               Label_documentos,
                                                                                               hiden_seleccion_documento,
                                                                                               UpdatePanelseleccion_digitalizado,
                                                                                               UpdatePanelseleccion_label_documentos,
                                                                                               Val(Hidden_numero_doc_rel.Value))

                If Result <> "YES" Then
                    Seleccion_tarea_workflow = Result
                    Exit Function
                End If
                'Elimina documentos digitalizados
                Dim Refclasdigitaliza As New ClassWorkflowDigitalizacion
                Result = Refclasdigitaliza.EliminaDocumentosDigigitalizados(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER"))
                If Result <> "YES" Then
                    Seleccion_tarea_workflow = "Inconsistencia eliminando documentos temporales " & Result
                    Exit Function
                End If
                Dim class_config_list_ruta As New Class_configuracion_listado_ruta
                Dim nombre_campo_beneficiario As String = ""
                Result = class_config_list_ruta.SolicitaNombreCampoBenificiarioRuta(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                                       nombre_campo_beneficiario)
                If Result <> "YES" Then
                    Seleccion_tarea_workflow = Result
                    Exit Function
                End If
                Dim class_dat As New Class_DAT_ADIC_TAR
                Dim beneficiario As String = ""
                If nombre_campo_beneficiario <> "" Then
                    Result = class_dat.SolicitaBeneficiarioTareaWorkflow(HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                   nombre_campo_beneficiario,
                                                                   id_tarea,
                                                                   beneficiario)

                    If Result <> "YES" Then
                        Seleccion_tarea_workflow = Result
                        Exit Function
                    End If
                End If
                IframeDitaliza.Attributes.Add("src", "../workflow/WebFormEscan.aspx")
                UpdatePanel_iframe_digitaliza.Update()
                Label_estado_enlace.Text = "-Radicado : " & HttpContext.Current.Session("DG_RADICADO") & "      -Solicitante : " & beneficiario & "       -Gabinete : " & HttpContext.Current.Session("DG_NOMBRE_GABINETE") & "       -Tramite : " & HttpContext.Current.Session("DG_NOMBRE_TRAMITE")
                UpdatePanel_estado_enlace.Update()
                UpdateDatos.Update()
                ModalPopupExtender_edition_admon_documentos.Show()
                Seleccion_tarea_workflow = "ENLACE"
                Exit Function
            End If
            '---CASO ACTUALIZA INDICES ENLACE
            Dim ClassEdtiScript As New ClassEdtiScript
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim Class_configuracion_gabinete As New Class_configuracion_gabinete
            Dim ClassDaGabinete As New ClassDaGabinete
            If evalua_actualiza_indices_documentos_enlace = 1 Then
                '---------Ejecuta el escript enlace
                If HttpContext.Current.Session("ENLASE") <> "" Then
                    '--------------------------------------------------------
                    'Actualiza los indices de las imagenes por código
                    '---------------------------------------------------------
                    Result = ClassEdtiScript.EjecutaEventoEnlaceDocumentosWorkflow(id_tarea,
                                                                                   id_actividad_usuario_logueado,
                                                                                   HttpContext.Current.Session("ENLASE"))
                    If Result <> "YES" Then
                        Seleccion_tarea_workflow = Result
                        Exit Function
                    End If
                Else
                    Result = ClassDaGabinete.ActualizaIndiceDocumentosGabineteRleacionadoTareaWorkflow(id_tarea,
                                                                                                       HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                                                       Radicado)
                    If Result <> "YES" Then
                        Seleccion_tarea_workflow = Result
                        Exit Function
                    End If
                End If
                '------------------------------------------------------------
                'Interrumpe la asignación de la tarea, opción sólo enlazar
                '------------------------------------------------------------
                If option_solo_actualiza_indices_documentos_enlace = 1 Then
                    Seleccion_tarea_workflow = "Se actualizaron los indices de los documentos correctamente "
                    Exit Function
                End If
            End If
            '-----------------------------------------------------
            'Verificar si el usuario o la actividad a asignar per
            'tenece al flujo de trabajo de lo contrario aborta 
            '
            '-----------------------------------------------------
            Dim id_flujo_trabajo As Integer = 0
            Dim Refclas_dat_adic As New Class_DAT_ADIC_TAR
            If evalua_pertenencia_flujo_trabajo = 1 Then
                Result = Refclas_dat_adic.SolicitaFlujoTareaWorkflow(HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                     id_tarea,
                                                                     id_flujo_trabajo)

                If Result <> "YES" Then
                    Seleccion_tarea_workflow = Result
                    Exit Function
                End If
                Dim Ref_class_registro_flujo As New Class_wf_registro_actividaes_flujos_trabajo
                Dim Estado_pertenencia As String = ""
                If id_flujo_trabajo <> 0 Then
                    Result = Ref_class_registro_flujo.Solicita_pertenencia_usuario_flujo_trabajo(id_actividad_usuario_logueado,
                                                                                                 HttpContext.Current.Session.Item("Id_Usuario_Workflow"),
                                                                                                 id_flujo_trabajo,
                                                                                                 Estado_pertenencia)
                    If Result <> "YES" Then
                        Seleccion_tarea_workflow = Result
                        Exit Function
                    End If
                    If Estado_pertenencia = "NO" Then
                        Seleccion_tarea_workflow = "El usuario no pertenece al flujo de trabajo asociado a la tarea que intenta recuperar. No es posible continuar con la recuperación de la tarea."
                        Exit Function
                    End If
                End If
            End If
            HttpContext.Current.Session("SELECCIONTEMPORAL") = id_tarea & "|" & id_actividad_tarea & "|" & Index & "|" & TipoActividad
            Dim SplitParan() As String
            Erase SplitParan
            '0-id_tarea
            '1-id_actividad
            '2-Index
            '3-TipoActividad
            SplitParan = Split(HttpContext.Current.Session.Item("SELECCIONTEMPORAL"), "|")
            If SplitParan Is Nothing Then
                Seleccion_tarea_workflow = "Lo paramentros id_tarea, id activdad index son nulos"
                Exit Function
            End If
            Dim Numero_Actividades As Integer = 0
            Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            '----Verifica que el usuario no tenga actvidades seleccionadas
            Result = Class_estados_tarea_workflow.SolicitaNumeroActividadesSelecionadasUsuario(id_actividad_usuario_logueado,
                                                                                               HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                                                               Numero_Actividades)
            If Result <> "YES" Then
                Seleccion_tarea_workflow = "Imposible obtener numero de actividades seleccionadas " + Result
                Exit Function
            End If
            If Numero_Actividades <> 0 Then
                Seleccion_tarea_workflow = "Usuario con tarea seleccionada "
                Exit Function
            End If
            '---------Evalua usuario con tareas asignadas
            Dim Objects As Object
            '---------Ejecuta los eventos PREINICIO Y TOMAR TAREA
            Result = Refclas.Inicializa_Tarea_Workflow(SplitParan(0),
                                                       id_actividad_usuario_logueado,
                                                       SplitParan(3),
                                                       Objects,
                                                       Objects,
                                                       mEval,
                                                       Objects)
            If Result <> "YES" Then
                Seleccion_tarea_workflow = Result
                Exit Function
            Else
                '--------Asigna el documento al usuario 
                Result = Refclas.Asigna_tarea(Val(SplitParan(0)),
                                                  0,
                                                  SplitParan(1),
                                                  determina_tipo_selecion,
                                                  Page,
                                                  id_pendiente,
                                                  id_respuesta,
                                                  "",
                                                  evalua_estado_tarea_recuperada,
                                                  stru_campo_tarea_)
                If Result <> "YES" Then
                    Seleccion_tarea_workflow = Result
                    Exit Function
                Else
                    HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = id_tarea
                    Hidden_id_tarea_selecionada.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                    UpdatePanel_general_variable.Update()

                End If
                HttpContext.Current.Session("SELECCIONTEMPORAL") = ""
                Hidden_resultado_selecion.Value = "YES"
                ModalPopupExtender_edition_admon_documentos.Hide()
                UpdatePanelseleccion.Update()
                Dim Refclas_seleccion As New Classselecciotarea
                Result = Refclas_seleccion.Actualiza_interface_estado_flujo_ruta(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                 HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                 HttpContext.Current.Session.Item("Id_Usuario_Workflow"),
                                                                                 HttpContext.Current.Session.Item("WF_ID_ACTIVIDAD"),
                                                                                 Page)
                If Result <> "YES" Then
                    Seleccion_tarea_workflow = Result
                    Exit Function
                End If
            End If
            Seleccion_tarea_workflow = "YES"
            Exit Function
        Catch ex As Exception
            Seleccion_tarea_workflow = "Iconsistencia general función Seleccion_tarea_workflow " & ex.Message
        End Try
    End Function
    Function Lista_imagenes_gestion_de_correspondencia(ByVal Id_tarea As Long, ByRef pag As Page) As String
        Try
            Dim Nombre_Ruta As String = ""
            Dim Result As String = ""
            Dim Datos_Tarea As String = ""
            Dim Datos_Gabientes As String = ""
            Dim Conta_Doc As Integer = 0
            Dim Ref_calss_wf_ruta As New Class_worflow_rutas
            Result = Ref_calss_wf_ruta.Solicita_nombre_ruta_workflow(HttpContext.Current.Session("Id_Ruta_Workflow").ToString,
                                                                     Nombre_Ruta)
            If Result <> "YES" Then
                Lista_imagenes_gestion_de_correspondencia = Result
                Exit Function
            End If
            '--------------------------------------
            'Obtener datos de la tarea
            '--------------------------------------
            'Matri_Datos_Tarea Informacion
            'Matri_Datos_Tarea(0)=ID DATOS TAREA
            'Matri_Datos_Tarea(1)=ID GABIENTE
            'Matri_Datos_Tarea(2)=ID_IMAGEN
            Result = ""
            Dim structure_datos_tarea_workflow As structure_datos_tarea_workflow = Nothing
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(Nombre_Ruta,
                                                                                            Id_tarea,
                                                                                            structure_datos_tarea_workflow)
            If Result <> "YES" Then
                Lista_imagenes_gestion_de_correspondencia = Result
                Exit Function
            End If
            Dim Refclas_seleccion_tarea As New Classselecciotarea
            Dim radicado As String = ""
            '------------------------------------------------------------------
            'Solicita el radicado relacionado al la tarea workflow y la ruta
            '------------------------------------------------------------------
            Result = Refclas_seleccion_tarea.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                                     Id_tarea,
                                                                                     radicado)
            If Result <> "YES" Then
                Lista_imagenes_gestion_de_correspondencia = Result
                Exit Function
            End If
            Dim ref_Class_configuracion_gabinete As New Class_configuracion_gabinete
            Dim structure_gabinete_workflow As structure_gabinete_workflow = Nothing
            Result = ref_Class_configuracion_gabinete.SolicitaDatosEstructuraGabineteWorkflow(structure_datos_tarea_workflow.ID_GABINETE,
                                                                                                  structure_gabinete_workflow)
            If Result <> "YES" Then
                Lista_imagenes_gestion_de_correspondencia = Result
                Exit Function
            End If

            Dim Refclas_trd As New ClassTrdDocumental
            Dim option_aplica_trd As Integer = 0
            Dim ref_Class_system1 As New Class_system1
            Result = ref_Class_system1.VerificaOpcionAplicarTablaRetencion(option_aplica_trd,
                                                                               structure_gabinete_workflow.NOMBRE_GABINETE)
            If Result <> "YES" Then
                Lista_imagenes_gestion_de_correspondencia = "Imposible encontrar opción aplicar trd gabinete (" + structure_gabinete_workflow.NOMBRE_GABINETE + ")"
                Exit Function
            End If
            Dim Ref_class_detalle_gabinete As New Class_DETALLE_GABIENETE
            Dim nombre_campo_radicado_gabinete As String = ""
            Result = Ref_class_detalle_gabinete.SolicitaNombreCampoRadicadoGabinete(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                                nombre_campo_radicado_gabinete)
            If Result <> "YES" Then
                Lista_imagenes_gestion_de_correspondencia = Result
                Exit Function
            End If
            Dim inventario_documental As Integer
            Dim aplica_trd As Integer
            Dim asigna_unidad As Integer
            Result = ref_Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                                                         inventario_documental,
                                                                                                         aplica_trd,
                                                                                                         asigna_unidad)
            If Result <> "YES" Then
                Lista_imagenes_gestion_de_correspondencia = Result
                Exit Function
            End If
            Dim ref_class_da_gabinete As New ClassDaGabinete
            Dim ref_Label_docu_relacionado_wf As Label = pag.FindControl("Label_docu_relacionado_wf")
            Dim ref_UpdatePanel_label_seleccion As UpdatePanel = pag.FindControl("UpdatePanel_label_seleccion")
            Dim ref_GridView_list_documento_relacion_wf As GridView = pag.FindControl("GridView_list_documento_relacion_wf")
            Dim ref_hiden_seleccion_documento_wf As Object = pag.FindControl("hiden_seleccion_documento_wf")
            Dim ref_UpdatePanelseleccion As UpdatePanel = pag.FindControl("UpdatePanelseleccion")
            Dim ref_Hidden_numero_doc_rel_wf As Object = pag.FindControl("Hidden_numero_doc_rel_wf")
            Dim ref_Webworkflow As Webworkflow = TryCast(pag, Webworkflow)
            Dim modernDocumentCountFormat As Boolean = ref_Webworkflow IsNot Nothing AndAlso ref_Webworkflow.WorkflowCentroTrabajoModernPresentationEnabled
            '-------------------------------------------------------------------------
            'Lista los documentos para la tarea asignada en workflow
            '-------------------------------------------------------------------------
            If Not ref_GridView_list_documento_relacion_wf Is Nothing Then
                Result = ref_class_da_gabinete.Lista_documentos_relacionados_a_tarea_workflow(nombre_campo_radicado_gabinete,
                                                                                               structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                                               radicado,
                                                                                               aplica_trd,
                                                                                               Id_tarea,
                                                                                               0,
                                                                                               ref_GridView_list_documento_relacion_wf,
                                                                                               ref_Label_docu_relacionado_wf,
                                                                                               ref_hiden_seleccion_documento_wf,
                                                                                               ref_UpdatePanelseleccion,
                                                                                               ref_UpdatePanel_label_seleccion,
                                                                                               Val(ref_Hidden_numero_doc_rel_wf.Value),
                                                                                               modernDocumentCountFormat)

                If Result <> "YES" Then
                    Lista_imagenes_gestion_de_correspondencia = Result
                    Exit Function
                End If

            End If
            Lista_imagenes_gestion_de_correspondencia = "YES"
        Catch ex As Exception
            Lista_imagenes_gestion_de_correspondencia = "Inconsistencia general funcion Lista_imagenes_gestion_de_correspondencia " & ex.Message
        End Try
    End Function
    Function Solicita_id_tipo_tramite_tarea_workflow(ByVal id_tarea As Long,
                                                     ByVal id_ruta As Integer,
                                                     ByVal nombre_ruta As String,
                                                     ByRef id_tipo_tramite As Integer) As String
        Try
            Dim Result As String = ""
            Dim nombre_campo_tramite As String = ""
            Dim Ref_class_cinfig_listado_ruta As New Class_configuracion_listado_ruta
            Result = Ref_class_cinfig_listado_ruta.SolicitaNombreCampoTramiteRuta(id_ruta,
                                                                                  nombre_campo_tramite)
            If Result <> "YES" Then
                Solicita_id_tipo_tramite_tarea_workflow = Result
                Exit Function
            End If
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim tramite As String = ""
            Dim estado_flujo As Integer = 0
            Result = Class_DAT_ADIC_TAR.SolicitaTramiteFlujoWorkflow(id_tarea,
                                                                     id_ruta,
                                                                     nombre_campo_tramite,
                                                                     nombre_ruta,
                                                                     tramite,
                                                                     estado_flujo)
            If Result <> "YES" Then
                Solicita_id_tipo_tramite_tarea_workflow = Result
                Exit Function
            End If
            Dim ref_class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Result = ref_class_tipo_doc_entrante.RetornaIdTipoTramitePorNombreTipo(tramite,
                                                                                   id_tipo_tramite)
            If Result <> "YES" Then
                Solicita_id_tipo_tramite_tarea_workflow = Result
                Exit Function
            Else
                Solicita_id_tipo_tramite_tarea_workflow = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Solicita_id_tipo_tramite_tarea_workflow = "Inconistencia general funcion Solicita_id_tipo_tramite_tarea_workflow " & ex.Message
        End Try
    End Function
    Function AsignaTareaWorkflow(ByVal IdTareaWorkflow As Long,
                                 ByVal IdActividadWorkflow As Integer,
                                 ByVal IdActividadUsuarioWorkflow As Integer,
                                 ByVal IdRutaWorkflow As Integer,
                                 ByVal NombreRutaWorkflow As String,
                                 ByVal Radicado As String,
                                 ByVal TipoAsignacion As Integer,
                                 ByVal IdUsuarioWorkflow As Integer,
                                 ByVal IdFlujoTrabajo As Integer,
                                 ByVal IdActividadFlujoTrabajo As Integer,
                                 ByVal IdUsuarioWorkflowFlujoTrabajo As Integer,
                                 ByVal IdPendiente As Integer,
                                 ByRef stru_campo_tarea_() As stru_campo_tarea) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Asgina tarea workflow 
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTareaWorkflow     : Representa la identificación de la tarea workflow
        'IdActividadWorkflow : Representa la identificación de la actividad workflow
        'IdRutaWorkflow      : Representa la identificacion de la ruta workflow
        'Radicado            : Representa el consecutivo de raiducado
        'TipoAsignacion      : Representa el tipo de asignación 0- Slección tarea curso normal 1- Selec
        'cion tarea sacando de pendiente   3-Selección tarea recuperada
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Nota: Remplaza fución "Asigna_tarea"
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim structure_datos_tarea_workflow As structure_datos_tarea_workflow = Nothing
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(NombreRutaWorkflow,
                                                                                   IdTareaWorkflow,
                                                                                   structure_datos_tarea_workflow)
            If Result <> "YES" Then
                AsignaTareaWorkflow = Result
                Exit Function
            End If
            If structure_datos_tarea_workflow.ID_GABINETE = 0 Then
                AsignaTareaWorkflow = "No es posible asignar la tarea, ya que no se ha definido un gabinete relacionado en el flujo de trabajo."
                Exit Function
            End If
            Dim Class_configuracion_gabinete As New Class_configuracion_gabinete
            Dim structure_gabinete_workflow As structure_gabinete_workflow = Nothing
            Result = Class_configuracion_gabinete.SolicitaDatosEstructuraGabineteWorkflow(structure_datos_tarea_workflow.ID_GABINETE,
                                                                                          structure_gabinete_workflow)
            If Result <> "YES" Then
                AsignaTareaWorkflow = Result
                Exit Function
            End If
            Dim OptionAplicaTrd As Integer = 0
            Dim Class_system1 As New Class_system1
            Result = Class_system1.VerificaOpcionAplicarTablaRetencion(OptionAplicaTrd,
                                                                       structure_gabinete_workflow.NOMBRE_GABINETE)
            If Result <> "YES" Then
                AsignaTareaWorkflow = Result
                Exit Function
            End If
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim IdImagenGabinete As Integer = 0
            '//--------Actualiza la imagen cuando no esta relacionda a la tarea-----/
            If structure_datos_tarea_workflow.ID_IMAGEN = 0 Then
                Result = ClassDaGabinete.SolicitaIdImagenGabinetePorRadicado(Radicado,
                                                                             structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                             IdImagenGabinete)
                If Result <> "YES" Then
                    AsignaTareaWorkflow = Result
                    Exit Function
                End If
                Result = Class_DAT_ADIC_TAR.ActualizaIdImagenTareaWorkflow(NombreRutaWorkflow,
                                                                           IdTareaWorkflow,
                                                                           IdImagenGabinete)
                If Result <> "YES" Then
                    AsignaTareaWorkflow = Result
                    Exit Function
                End If
            Else
                '///---Detecta que la imagen se elimne en el gabinete y relaciona con la nueva imagen---//
                Dim stru_paramter_image As stru_paramter_image = Nothing
                Result = ClassDaGabinete.SolicitaEtructuraImagenGabinete(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                         structure_datos_tarea_workflow.ID_IMAGEN,
                                                                         stru_paramter_image,
                                                                         OptionAplicaTrd,
                                                                         1)
                If Result <> "YES" Then
                    AsignaTareaWorkflow = Result
                    Exit Function
                End If
                If stru_paramter_image.ID = 0 Then
                    Result = ClassDaGabinete.SolicitaIdImagenGabinetePorRadicado(Radicado,
                                                                                 structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                                 IdImagenGabinete)
                    If Result <> "YES" Then
                        AsignaTareaWorkflow = Result
                        Exit Function
                    End If
                    If IdImagenGabinete = 0 Then
                        AsignaTareaWorkflow = "La tarea seleccionada no tiene un documento relacionado en el gabinete"
                        Exit Function
                    End If
                    Result = Class_DAT_ADIC_TAR.ActualizaIdImagenTareaWorkflow(NombreRutaWorkflow,
                                                                               IdTareaWorkflow,
                                                                               IdImagenGabinete)
                    If Result <> "YES" Then
                        AsignaTareaWorkflow = Result
                        Exit Function
                    End If
                End If
            End If
            Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            '//-------Valida tipo de asignación general------///
            If TipoAsignacion = 0 Then
                Result = Class_estados_tarea_workflow.ActualizaEstadoTareaWorkflow(IdUsuarioWorkflow,
                                                                                   IdTareaWorkflow,
                                                                                   IdActividadWorkflow,
                                                                                   IdPendiente)
                If Result <> "YES" Then
                    AsignaTareaWorkflow = Result
                    Exit Function
                End If
            End If
            '//-------Valida tipo de asignación tarea en pediente------///
            If TipoAsignacion = 1 Then
                Result = Class_estados_tarea_workflow.ActualizandoEstadoTareaWorkflowPendienteAtomica(IdUsuarioWorkflow,
                                                                                                     IdTareaWorkflow,
                                                                                                     IdActividadWorkflow,
                                                                                                     IdRutaWorkflow,
                                                                                                     Result = IdPendiente)
                If Result <> "YES" Then
                    AsignaTareaWorkflow = Result
                    Exit Function
                End If
            End If
            '//--------Actualiza los estados y asigna una tarea recuperada----///
            If TipoAsignacion = 2 Then
                Result = Class_estados_tarea_workflow.ActualizaEstadoTareaRecuperada(IdUsuarioWorkflow,
                                                                                     IdTareaWorkflow,
                                                                                     IdActividadWorkflow,
                                                                                     IdActividadUsuarioWorkflow,
                                                                                     IdFlujoTrabajo,
                                                                                     IdActividadFlujoTrabajo,
                                                                                     IdUsuarioWorkflowFlujoTrabajo,
                                                                                     IdPendiente)
                If Result <> "YES" Then
                    AsignaTareaWorkflow = Result
                    Exit Function
                End If
            End If
            '-------------------------------------------------------
            'Actualiza estado tarea en el cache o inserta registro si
            '------------------------------------------------------
            If TipoAsignacion = 2 And HttpContext.Current.Session.Item("UTIL_ITER_PENDIENTE") = 0 Then
                Result = Class_estados_tarea_workflow.Solicita_datos_lista_tarea_workflow_cache(IdTareaWorkflow,
                                                                                                stru_campo_tarea_)
                If Result <> "YES" Then
                    AsignaTareaWorkflow = Result
                    Exit Function
                End If
                Dim fila As Data.DataRow
                fila = HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").Tables(0).NewRow
                For i As Integer = 0 To stru_campo_tarea_.Length - 1
                    fila(stru_campo_tarea_(i).nombre_campo) = stru_campo_tarea_(i).valor_campo
                Next
                '--------------------------------------------------------------------
                'Eliminar la fila en clache si pertenece a la lista del usuario
                '--------------------------------------------------------------------
                If Not HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF") Is Nothing Then
                    For i As Integer = 0 To HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").Tables(0).Rows.Count - 1
                        If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").Tables(0).Rows(i).Item(0) = IdTareaWorkflow Then
                            HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").Tables(0).Rows(i).delete()
                            HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").AcceptChanges()
                            Exit For
                        End If
                    Next
                End If
                If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").GetType.ToString = "System.Data.DataSet" Then
                    For i As Integer = 0 To HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").Tables(0).Rows.Count - 1
                        If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").Tables(0).Rows(i).Item(0) = IdTareaWorkflow Then
                            HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").Tables(0).Rows(i).delete()
                            HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").AcceptChanges()
                            Exit For
                        End If
                    Next
                End If
                '--------------------------------------------------------------------
                'Inserta en el cache la nueva fila con de la tarea recuperada
                '---------------------------------------------------------------------
                If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").Tables(0).Rows.Count > 1 Then
                    HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").Tables(0).Rows.InsertAt(fila, 0)
                Else
                    HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").Tables(0).Rows.Add(fila)
                End If
                If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").GetType.ToString = "System.Data.DataSet" Then
                    fila = HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").Tables(0).NewRow
                    For i As Integer = 0 To stru_campo_tarea_.Length - 1
                        fila(stru_campo_tarea_(i).nombre_campo) = stru_campo_tarea_(i).valor_campo
                    Next
                    If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").Tables(0).Rows.Count > 1 Then
                        HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").Tables(0).Rows.InsertAt(fila, 0)
                    Else
                        HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").Tables(0).Rows.Add(fila)
                    End If
                End If
                HttpContext.Current.Session.Item("NUMEROACTIVIDADES") = HttpContext.Current.Session.Item("NUMEROACTIVIDADES") + 1
            Else
                If Not HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF") Is Nothing Then
                    If HttpContext.Current.Session.Item("UTIL_ITER_PENDIENTE") = 0 Then
                        For i As Integer = 0 To HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").Tables(0).Rows.Count - 1
                            If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").Tables(0).Rows(i).Item(0) = IdTareaWorkflow Then
                                HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").Tables(0).Rows(i).Item("ESTADO") = 1
                                HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").AcceptChanges()
                                Exit For
                            End If
                        Next
                    End If
                End If
                If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").GetType.ToString = "System.Data.DataSet" Then
                    If HttpContext.Current.Session.Item("UTIL_ITER_PENDIENTE") = 0 Then
                        For i As Integer = 0 To HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").Tables(0).Rows.Count - 1
                            If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").Tables(0).Rows(i).Item(0) = IdTareaWorkflow Then
                                HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").Tables(0).Rows(i).Item("ESTADO") = 1
                                HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").AcceptChanges()
                                Exit For
                            End If
                        Next
                    End If
                End If
            End If
            AsignaTareaWorkflow = "YES"
        Catch ex As Exception
            AsignaTareaWorkflow = "Iconsistencia genera funcion AsignaTareaWorkflow " & ex.Message
        End Try
    End Function
    Function Asigna_tarea(ByVal Id_tarea As Long,
                          ByVal evalua_asigna_tarea As Integer,
                          ByVal Id_actividad_tarea As Integer,
                          ByVal deter_tipo_selecion As Integer,
                          ByRef pag As Page,
                          Optional ByVal id_pendiente As Integer = -1,
                          Optional ByVal id_respuesta As Integer = 0,
                          Optional ByRef Resultado_correo As String = "",
                          Optional ByVal evalua_estado_tarea_recuperada As Integer = 0,
                          Optional ByRef stru_campo_tarea_() As stru_campo_tarea = Nothing) As String

        '----------------------------------------------------------
        'Funcion : Asigna_tarea
        'Funcion que selecicona y lista documentos relacionados
        'con una tarea si se envia la variable switch en 0 cambia
        'de estado la tarea y la lista en un listview
        'Fecha 2012-12-28 
        'Ingeniero : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------
        Try
            Dim Nombre_Ruta As String = ""
            Dim Result As String = ""
            Dim Datos_Tarea As String = ""
            Dim Datos_Gabientes As String = ""
            Dim Conta_Doc As Integer = 0
            Dim Ref_calss_wf_ruta As New Class_worflow_rutas
            Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            Result = Ref_calss_wf_ruta.Solicita_nombre_ruta_workflow(HttpContext.Current.Session("Id_Ruta_Workflow").ToString,
                                                                     Nombre_Ruta)
            If Result <> "YES" Then
                Asigna_tarea = Result
                Exit Function
            End If
            '---------------------------------------------
            'Solicita el tipo tramite de la tarea asignada
            '---------------------------------------------
            Dim id_tipo_tramite As Integer = -1
            Result = Me.Solicita_id_tipo_tramite_tarea_workflow(Id_tarea,
                                                                Val(HttpContext.Current.Session("Id_Ruta_Workflow")),
                                                                Nombre_Ruta,
                                                                id_tipo_tramite)
            If Result <> "YES" Then
                Asigna_tarea = Result
                Exit Function
            End If
            HttpContext.Current.Session("DG_TRAMITE_DIGITAIZACION") = id_tipo_tramite
            '--------------------------------------
            'Obtener datos de la tarea 
            '--------------------------------------
            'Matri_Datos_Tarea Informacion
            'Matri_Datos_Tarea(0)=ID DATOS TAREA
            'Matri_Datos_Tarea(1)=ID GABIENTE
            'Matri_Datos_Tarea(2)=ID_IMAGEN
            Result = ""
            Dim structure_datos_tarea_workflow As structure_datos_tarea_workflow = Nothing
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(Nombre_Ruta,
                                                                                       Id_tarea,
                                                                                       structure_datos_tarea_workflow)
            If Result <> "YES" Then
                Asigna_tarea = Result
                Exit Function
            End If

            If structure_datos_tarea_workflow.ID_DAT = 0 Then
                Asigna_tarea = "#25  Imposible encontrar id de la tarea en la tabla dat_adic_tar  (" & Id_tarea & ")"
                Exit Function
            End If
            If structure_datos_tarea_workflow.ID_GABINETE = 0 Then
                Asigna_tarea = "No es posible asignar la tarea (" & Id_tarea & "), ya que no se ha definido un gabinete relacionado en el flujo de trabajo. ya que no se ha definido un gabinete relacionado en el flujo de trabajo."
                Exit Function
            End If
            If structure_datos_tarea_workflow.ID_IMAGEN = 0 Then
                Asigna_tarea = "La tarea no puede ser asignada porque no se ha vinculado ningún documento."
                Exit Function
            End If
            '----------------------------------------
            'Obtener datos del gabinete
            '----------------------------------------
            'Datos_Gabientes_Matri
            'Datos_Gabientes_Matri(0)=Nombre_Gabienete
            'Datos_Gabientes_Matri(1)=RUTA BUSQUEDA IMAGEN
            'Datos_Gabientes_Matri(2)=BASE DE DATOS
            'Datos_Gabientes_Matri(3)=MOTOR BASE DE DATOS
            'Datos_Gabientes_Matri(4)=ODBC SERVIDOR
            'Datos_Gabientes_Matri(5)=USUARIO
            'Datos_Gabientes_Matri(6)=PASWORD 
            Dim ref_Class_configuracion_gabinete As New Class_configuracion_gabinete
            Dim structure_gabinete_workflow As structure_gabinete_workflow = Nothing
            Result = ref_Class_configuracion_gabinete.SolicitaDatosEstructuraGabineteWorkflow(structure_datos_tarea_workflow.ID_GABINETE,
                                                                                              structure_gabinete_workflow)
            If Result <> "YES" Then
                Asigna_tarea = "#28 SELECCIONA-WF " & Result
                Exit Function
            End If
            Dim Refclas_trd As New ClassTrdDocumental
            Dim option_aplica_trd As Integer = 0
            Dim ref_Class_system1 As New Class_system1
            Result = ref_Class_system1.VerificaOpcionAplicarTablaRetencion(option_aplica_trd,
                                                                           structure_gabinete_workflow.NOMBRE_GABINETE)
            If Result <> "YES" Then
                Asigna_tarea = "#31 SELECCIONA-WF Imposible encontrar opción aplicar trd gabinete (" + structure_gabinete_workflow.NOMBRE_GABINETE + ")"
                Exit Function
            End If
            Result = ""
            Dim ref_ClassDaGabinete As New ClassDaGabinete
            Dim stru_paramter_image As stru_paramter_image = Nothing
            Result = ref_ClassDaGabinete.SolicitaEtructuraImagenGabinete(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                         structure_datos_tarea_workflow.ID_IMAGEN,
                                                                         stru_paramter_image,
                                                                         option_aplica_trd,
                                                                         1)
            If Result <> "YES" Then
                Asigna_tarea = Result
                Exit Function
            End If
            '-----------------------------------------------------
            'Actualiza con la nueva imagen del radicado si se 
            'cambio de la tarea
            '-----------------------------------------------------
            Dim id_imagen_actualizar As Integer = 0
            Dim radicado_actualizar As String = ""
            If stru_paramter_image.ID = 0 Then
                Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(Id_tarea,
                                                                                        radicado_actualizar)
                If Result <> "YES" Then
                    Asigna_tarea = Result
                    Exit Function
                End If
                Result = ref_ClassDaGabinete.Solicita_id_documento_gabinete(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                            radicado_actualizar,
                                                                            0,
                                                                            id_imagen_actualizar)
                If Result <> "YES" Then
                    Asigna_tarea = Result
                    Exit Function
                End If
                If id_imagen_actualizar = 0 Then
                    Asigna_tarea = "La tarea (" & Id_tarea & ") no tiene documento relacionado en el gabinete (" & structure_gabinete_workflow.NOMBRE_GABINETE & ") imposible asignar la tarea"
                    Exit Function
                End If
                Result = ref_Class_DAT_ADIC_TAR.ActualizaIdImagenTareaWorkflow(Nombre_Ruta,
                                                                               Id_tarea,
                                                                               id_imagen_actualizar)
                If Result <> "YES" Then
                    Asigna_tarea = Result
                    Exit Function
                End If
                Result = ref_ClassDaGabinete.SolicitaEtructuraImagenGabinete(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                             id_imagen_actualizar,
                                                                             stru_paramter_image,
                                                                             option_aplica_trd,
                                                                             0)
                If Result <> "YES" Then
                    Asigna_tarea = Result
                    Exit Function
                End If
            End If
            '-------------------------------------------------------
            'Asigna tarea workflow y actualiza los estados
            '-------------------------------------------------------
            If evalua_asigna_tarea = 0 Then
                Dim id_actividad_user As Integer = 0
                Dim Ref_Class_grupos_workflow As New Class_grupos_workflow
                Result = Ref_Class_grupos_workflow.Solicita_id_actividad_usuario_workflow(id_actividad_user,
                                                                                          HttpContext.Current.Session("Id_Grupo_Workflow"))
                If Result <> "YES" Then
                    Asigna_tarea = "#46 SELECCIONA-WF Imposible Obtener Id actividad " & Result
                    Exit Function
                End If
                Result = ""
                Dim Numero_Actividades As Integer = 0
                '----Verifica que el usuario no tenga actvidades seleccionadas
                Result = Class_estados_tarea_workflow.SolicitaNumeroActividadesSelecionadasUsuario(id_actividad_user,
                                                                                                   HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                                                                   Numero_Actividades)
                If Result <> "YES" Then
                    Asigna_tarea = "#45 SELECCIONA-WF Imposible Obtener Numero de Actividades Selecciondas " + Result
                    Exit Function
                End If
                If Numero_Actividades <> 0 Then
                    Asigna_tarea = " Usuario con  actividades seleccionadas "
                    Exit Function
                End If
                '----Selecciona tarea workflow recuperada 
                If deter_tipo_selecion = 2 Then
                    Result = ""
                    Dim Refc As New ClassListandoTareas
                    '----------------------------------------------------------------------------
                    'Solicita los datos del flujo de trabajo anterior para caso flujo de trabajo
                    '----------------------------------------------------------------------------
                    Dim Refclas_flujo As New Class_flujo_trabajo_workflow
                    Dim id_actividad_flujo_trabjo As Integer = 0
                    Dim id_flujo_trabajo As Integer = 0
                    Dim id_usuarion_workflow_flujo_trabajo As Integer = 0
                    Dim ref_class_flujo As New Class_DAT_ADIC_TAR
                    Result = ref_class_flujo.SolicitaFlujoTareaWorkflow(Nombre_Ruta,
                                                                        Id_tarea,
                                                                        id_flujo_trabajo)
                    If Result <> "YES" Then
                        Asigna_tarea = Result
                        Exit Function
                    End If
                    Dim Refclas_registro_flujo As New Class_wf_registro_actividaes_flujos_trabajo
                    Dim stru_actividad_usuario_flujo() As stru_actividad_usuario_flujo = Nothing
                    Dim ref_clas_estados As New Class_estados_tarea_workflow
                    Dim stru_estados_flujo_tarea() As stru_estados_flujo_tarea = Nothing
                    Dim i_stru As Integer = 0
                    If id_flujo_trabajo <> 0 Then
                        Result = Refclas_registro_flujo.SolicitaIActividadesUsuarioWorkflowFlujoTrabajo(id_actividad_user,
                                                                                                         HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                                                                         id_flujo_trabajo,
                                                                                                         stru_actividad_usuario_flujo)
                        If Result <> "YES" Then
                            Asigna_tarea = Result
                            Exit Function
                        End If
                        For i As Integer = 0 To stru_actividad_usuario_flujo.Length - 1
                            Result = ref_clas_estados.SolicitaEstadosFlujoDocumentalIdTareaUsuarioFlujo(Id_tarea,
                                                                                                               stru_actividad_usuario_flujo(i).id_actividad_workflow_flujo,
                                                                                                               id_flujo_trabajo,
                                                                                                               stru_estados_flujo_tarea,
                                                                                                               i_stru)
                            If Result <> "YES" Then
                                Asigna_tarea = Result
                                Exit Function
                            End If
                        Next
                        If stru_estados_flujo_tarea Is Nothing Then
                            Asigna_tarea = "Usted no a participado en el flujo de trabajo, es posible que la tarea deba pasar previamente por otros usuarios antes de llegar a su actividad. Imposible recuperar"
                            Exit Function
                        Else
                            id_actividad_flujo_trabjo = stru_estados_flujo_tarea(UBound(stru_estados_flujo_tarea)).ID_ACTIVIDAD_FLUJO_TRABAJO
                            id_usuarion_workflow_flujo_trabajo = stru_estados_flujo_tarea(UBound(stru_estados_flujo_tarea)).ID_USUARIO_WORKFLOW_FLUJO_TRABAJO
                        End If
                    End If
                    Result = Class_estados_tarea_workflow.ActualizaEstadoTareaRecuperada(HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                                                         Id_tarea,
                                                                                         Id_actividad_tarea,
                                                                                         id_actividad_user,
                                                                                         id_flujo_trabajo,
                                                                                         id_actividad_flujo_trabjo,
                                                                                         id_usuarion_workflow_flujo_trabajo,
                                                                                         id_pendiente)
                    If Result <> "YES" Then
                        Asigna_tarea = " #47 SELECCIONA-WF " + Result
                        Exit Function
                    End If
                End If
                '----Selecciona tarea workflow 
                If deter_tipo_selecion = 0 Then
                    Result = Class_estados_tarea_workflow.ActualizaEstadoTareaWorkflow(HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                                                       Id_tarea,
                                                                                       Id_actividad_tarea,
                                                                                       id_pendiente)
                    If Result <> "YES" Then
                        Asigna_tarea = " #48 SELECCIONA-WF " + Result
                        Exit Function
                    End If
                End If
                '-----Selecciona tarea y saca de pendiente
                If deter_tipo_selecion = 1 Then
                    Result = Class_estados_tarea_workflow.ActualizandoEstadoTareaWorkflowPendienteAtomica(HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                                                                          Id_tarea,
                                                                                                          Id_actividad_tarea,
                                                                                                          HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                                                          id_pendiente)
                    If Result <> "YES" Then
                        Asigna_tarea = " #49 SELECCIONA-WF " + Result
                        Exit Function
                    End If
                End If
            End If
            Dim Refclas_seleccion_tarea As New Classselecciotarea
            Dim radicado As String = ""
            '------------------------------------------------------------------
            'Solicita el radicado relacionado al la tarea workflow y la ruta
            '------------------------------------------------------------------
            Result = Refclas_seleccion_tarea.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                                     Id_tarea,
                                                                                     radicado)
            If Result <> "YES" Then
                Asigna_tarea = Result
                Exit Function
            End If
            Dim Ref_class_detalle_gabinete As New Class_DETALLE_GABIENETE
            Dim nombre_campo_radicado_gabinete As String = ""
            Result = Ref_class_detalle_gabinete.SolicitaNombreCampoRadicadoGabinete(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                                    nombre_campo_radicado_gabinete)
            If Result <> "YES" Then
                Asigna_tarea = Result
                Exit Function
            End If
            Dim inventario_documental As Integer
            Dim aplica_trd As Integer
            Dim asigna_unidad As Integer
            Result = ref_Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                                                         inventario_documental,
                                                                                                         aplica_trd,
                                                                                                         asigna_unidad)
            If Result <> "YES" Then
                Asigna_tarea = Result
                Exit Function
            End If
            Dim ref_class_da_gabinete As New ClassDaGabinete
            Dim ref_Label_docu_relacionado_wf As Label = pag.FindControl("Label_docu_relacionado_wf")
            Dim ref_UpdatePanel_label_seleccion As UpdatePanel = pag.FindControl("UpdatePanel_label_seleccion")
            Dim ref_GridView_list_documento_relacion_wf As GridView = pag.FindControl("GridView_list_documento_relacion_wf")
            Dim ref_hiden_seleccion_documento_wf As Object = pag.FindControl("hiden_seleccion_documento_wf")
            Dim ref_UpdatePanelseleccion As UpdatePanel = pag.FindControl("UpdatePanelseleccion")
            Dim ref_Hidden_numero_doc_rel_wf As Object = pag.FindControl("Hidden_numero_doc_rel_wf")
            Dim UpdatePanelnumeroespera As UpdatePanel = pag.FindControl("UpdatePanelnumeroespera")
            Dim LabelEspera As Label = pag.FindControl("LabelEspera")
            Dim ref_Webworkflow As Webworkflow = TryCast(pag, Webworkflow)
            Dim modernDocumentCountFormat As Boolean = ref_Webworkflow IsNot Nothing AndAlso ref_Webworkflow.WorkflowCentroTrabajoModernPresentationEnabled
            '-------------------------------------------------------------------------
            'Lista los documentos para la tarea asignada en workflow CAMBIAR
            '-------------------------------------------------------------------------
            If Not ref_GridView_list_documento_relacion_wf Is Nothing Then
                Result = ref_class_da_gabinete.Lista_documentos_relacionados_a_tarea_workflow(nombre_campo_radicado_gabinete,
                                                                                              structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                                              radicado,
                                                                                              aplica_trd,
                                                                                              Id_tarea,
                                                                                              1,
                                                                                              ref_GridView_list_documento_relacion_wf,
                                                                                              ref_Label_docu_relacionado_wf,
                                                                                              ref_hiden_seleccion_documento_wf,
                                                                                              ref_UpdatePanelseleccion,
                                                                                              ref_UpdatePanel_label_seleccion,
                                                                                              Val(ref_Hidden_numero_doc_rel_wf.Value),
                                                                                              modernDocumentCountFormat)

                If Result <> "YES" Then
                    Asigna_tarea = Result
                    Exit Function
                End If
            End If
            '------------------------------------------------
            'Reasigna el tramite  NO
            '------------------------------------------------
            If id_respuesta <> 0 Then
                Dim Ref_respuesta As New Classgestionrespuesta
                Result = Ref_respuesta.Reasigna_respuesta_tarea_recuperda(Id_tarea,
                                                                          Resultado_correo,
                                                                          pag)
                If Result <> "YES" Then
                    Asigna_tarea = Result
                    Exit Function
                End If
            End If
            '-------------------------------------------------------
            'Actualiza estado tarea en el cache o inserta registro si
            '------------------------------------------------------
            If evalua_estado_tarea_recuperada = 1 And HttpContext.Current.Session.Item("UTIL_ITER_PENDIENTE") = 0 Then
                Result = Class_estados_tarea_workflow.Solicita_datos_lista_tarea_workflow_cache(Id_tarea,
                                                                                                stru_campo_tarea_)
                If Result <> "YES" Then
                    Asigna_tarea = Result
                    Exit Function
                End If
                Dim fila As Data.DataRow
                fila = HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").Tables(0).NewRow
                For i As Integer = 0 To stru_campo_tarea_.Length - 1
                    fila(stru_campo_tarea_(i).nombre_campo) = stru_campo_tarea_(i).valor_campo
                Next
                '--------------------------------------------------------------------
                'Eliminar la fila en clache si pertenece a la lista del usuario
                '--------------------------------------------------------------------
                If Not HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF") Is Nothing Then
                    For i As Integer = 0 To HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").Tables(0).Rows.Count - 1
                        If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").Tables(0).Rows(i).Item(0) = Id_tarea Then
                            HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").Tables(0).Rows(i).delete()
                            HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").AcceptChanges()
                            Exit For
                        End If
                    Next
                End If
                If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").GetType.ToString = "System.Data.DataSet" Then
                    For i As Integer = 0 To HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").Tables(0).Rows.Count - 1
                        If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").Tables(0).Rows(i).Item(0) = Id_tarea Then
                            HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").Tables(0).Rows(i).delete()
                            HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").AcceptChanges()
                            Exit For
                        End If
                    Next
                End If
                '--------------------------------------------------------------------
                'Inserta en el cache la nueva fila con de la tarea recuperada
                '---------------------------------------------------------------------
                If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").Tables(0).Rows.Count > 1 Then
                    HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").Tables(0).Rows.InsertAt(fila, 0)
                Else
                    HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").Tables(0).Rows.Add(fila)
                End If

                If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").GetType.ToString = "System.Data.DataSet" Then
                    fila = HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").Tables(0).NewRow
                    For i As Integer = 0 To stru_campo_tarea_.Length - 1
                        fila(stru_campo_tarea_(i).nombre_campo) = stru_campo_tarea_(i).valor_campo
                    Next
                    If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").Tables(0).Rows.Count > 1 Then
                        HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").Tables(0).Rows.InsertAt(fila, 0)
                    Else
                        HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").Tables(0).Rows.Add(fila)
                    End If
                End If
                HttpContext.Current.Session.Item("NUMEROACTIVIDADES") = HttpContext.Current.Session.Item("NUMEROACTIVIDADES") + 1
                LabelEspera.Text = "(" & HttpContext.Current.Session.Item("NUMEROACTIVIDADES") & ")"
                UpdatePanelnumeroespera.Update()
            Else
                If Not HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF") Is Nothing Then
                    If HttpContext.Current.Session.Item("UTIL_ITER_PENDIENTE") = 0 Then
                        For i As Integer = 0 To HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").Tables(0).Rows.Count - 1
                            If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").Tables(0).Rows(i).Item(0) = Id_tarea Then
                                HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").Tables(0).Rows(i).Item("ESTADO") = 1
                                HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").AcceptChanges()
                                Exit For
                            End If
                        Next
                    End If
                End If
                If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").GetType.ToString = "System.Data.DataSet" Then
                    If HttpContext.Current.Session.Item("UTIL_ITER_PENDIENTE") = 0 Then
                        For i As Integer = 0 To HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").Tables(0).Rows.Count - 1
                            If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").Tables(0).Rows(i).Item(0) = Id_tarea Then
                                HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").Tables(0).Rows(i).Item("ESTADO") = 1
                                HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").AcceptChanges()
                                Exit For
                            End If
                        Next
                    End If
                End If
            End If
            Asigna_tarea = "YES"
        Catch ex As Exception
            Asigna_tarea = "Inconsistencia general función Asigna_tarea " & ex.Message
        End Try
    End Function
    Function SolicitaListaImagensGabineteRelacionTareaWorkflow(ByVal IdTareaWorkflow As Long,
                                                               ByRef StruImagenGabineteWorkflow As stru_imagen_gabinete_workflow()) As String
        Try
            Dim Result As String = ""
            Dim structure_datos_tarea_workflow As structure_datos_tarea_workflow = Nothing
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                                       IdTareaWorkflow,
                                                                                       structure_datos_tarea_workflow)
            If Result <> "YES" Then
                SolicitaListaImagensGabineteRelacionTareaWorkflow = Result
                Exit Function
            End If
            If structure_datos_tarea_workflow.ID_DAT = 0 Then
                SolicitaListaImagensGabineteRelacionTareaWorkflow = "#255  Imposible encontrar id de la tarea en la tabla dat_adic_tar  (" & IdTareaWorkflow & ")"
                Exit Function
            End If
            If structure_datos_tarea_workflow.ID_GABINETE = 0 Then
                SolicitaListaImagensGabineteRelacionTareaWorkflow = "#266 tarea sin id gabinete asignado (" & IdTareaWorkflow & ")"
                Exit Function
            End If
            If structure_datos_tarea_workflow.ID_IMAGEN = 0 Then
                SolicitaListaImagensGabineteRelacionTareaWorkflow = "#277  La imagen de esta tarea fue cambiada o eliminada  tarea sin imagen adjunta  (" & IdTareaWorkflow & ")"
                Exit Function
            End If
            Dim ref_Class_configuracion_gabinete As New Class_configuracion_gabinete
            Dim structure_gabinete_workflow As structure_gabinete_workflow = Nothing
            Result = ref_Class_configuracion_gabinete.SolicitaDatosEstructuraGabineteWorkflow(structure_datos_tarea_workflow.ID_GABINETE,
                                                                                              structure_gabinete_workflow)
            If Result <> "YES" Then
                SolicitaListaImagensGabineteRelacionTareaWorkflow = Result
                Exit Function
            End If
            Dim ref_ClassDaGabinete As New ClassDaGabinete
            Dim stru_paramter_image As stru_paramter_image = Nothing
            Result = ref_ClassDaGabinete.SolicitaEtructuraImagenGabinete(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                         structure_datos_tarea_workflow.ID_IMAGEN,
                                                                         stru_paramter_image,
                                                                         0)
            If Result <> "YES" Then
                SolicitaListaImagensGabineteRelacionTareaWorkflow = Result
                Exit Function
            End If
            ReDim Preserve StruImagenGabineteWorkflow(0)
            StruImagenGabineteWorkflow(0).id_image = structure_datos_tarea_workflow.ID_IMAGEN
            StruImagenGabineteWorkflow(0).gabinete = structure_gabinete_workflow.NOMBRE_GABINETE
            Result = ref_ClassDaGabinete.SolicitaListaImagenesGabinete(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                          stru_paramter_image.ENLACE,
                                                                          StruImagenGabineteWorkflow,
                                                                          structure_datos_tarea_workflow.ID_IMAGEN)
            If Result <> "YES" Then
                SolicitaListaImagensGabineteRelacionTareaWorkflow = Result
                Exit Function
            End If
            If stru_paramter_image.ENLACE = "" Then
                SolicitaListaImagensGabineteRelacionTareaWorkflow = "La imagen principal (" & structure_datos_tarea_workflow.ID_IMAGEN & ") del gabinete (" &
                    structure_gabinete_workflow.NOMBRE_GABINETE & ") no relaciona datos campo ENLACE, tarea workflow (" & IdTareaWorkflow & ") por favor contace a su administrador de sistema"
                Exit Function
            End If
            SolicitaListaImagensGabineteRelacionTareaWorkflow = "YES"
        Catch ex As Exception
            SolicitaListaImagensGabineteRelacionTareaWorkflow = "Inconsistencia general funcion Solicita_lista_id_imagen_gabinete_relacion_tarea_workflow " & ex.Message
        End Try
    End Function
    Function Solicita_lista_id_producion_relacionados_tarea_workflow(ByVal Id_tarea As Long,
                                                                     ByRef stru_paramter_image_final As stru_paramter_image()) As String
        Try
            Dim Result As String = ""
            Dim structure_datos_tarea_workflow As structure_datos_tarea_workflow = Nothing
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                                            Id_tarea,
                                                                                            structure_datos_tarea_workflow)
            If Result <> "YES" Then
                Solicita_lista_id_producion_relacionados_tarea_workflow = Result
                Exit Function
            End If
            If structure_datos_tarea_workflow.ID_DAT = 0 Then
                Solicita_lista_id_producion_relacionados_tarea_workflow = "#255  Imposible encontrar id de la tarea en la tabla dat_adic_tar  (" & Id_tarea & ")"
                Exit Function
            End If
            If structure_datos_tarea_workflow.ID_GABINETE = 0 Then
                Solicita_lista_id_producion_relacionados_tarea_workflow = "#266 tarea sin id gabinete asignado (" & Id_tarea & ")"
                Exit Function
            End If
            If structure_datos_tarea_workflow.ID_IMAGEN = 0 Then
                Solicita_lista_id_producion_relacionados_tarea_workflow = "#277  La imagen de esta tarea fue cambiada o eliminada  tarea sin imagen adjunta  (" & Id_tarea & ")"
                Exit Function
            End If
            Dim ref_Class_configuracion_gabinete As New Class_configuracion_gabinete
            Dim structure_gabinete_workflow As structure_gabinete_workflow = Nothing
            Result = ref_Class_configuracion_gabinete.SolicitaDatosEstructuraGabineteWorkflow(structure_datos_tarea_workflow.ID_GABINETE,
                                                                                                  structure_gabinete_workflow)
            If Result <> "YES" Then
                Solicita_lista_id_producion_relacionados_tarea_workflow = Result
                Exit Function
            End If
            Dim Refclas_trd As New ClassTrdDocumental
            Dim option_aplica_trd As Integer = 0
            Dim ref_Class_system1 As New Class_system1
            Result = ref_Class_system1.VerificaOpcionAplicarTablaRetencion(option_aplica_trd,
                                                                               structure_gabinete_workflow.NOMBRE_GABINETE)
            If Result <> "YES" Then
                Solicita_lista_id_producion_relacionados_tarea_workflow = "#31 SELECCIONA-WF Imposible encontrar opción aplicar trd gabinete (" + structure_gabinete_workflow.NOMBRE_GABINETE + ")"
                Exit Function
            End If
            Dim ref_ClassDaGabinete As New ClassDaGabinete
            Dim stru_paramter_image As stru_paramter_image = Nothing
            Result = ref_ClassDaGabinete.Solicita_structura_imagen_gabinete_producion(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                                      structure_datos_tarea_workflow.ID_IMAGEN,
                                                                                      stru_paramter_image,
                                                                                      option_aplica_trd)
            If Result <> "YES" Then
                Solicita_lista_id_producion_relacionados_tarea_workflow = Result
                Exit Function
            End If
            ReDim Preserve stru_paramter_image_final(0)
            stru_paramter_image_final(0).ID = stru_paramter_image.ID
            stru_paramter_image_final(0).IDEX = stru_paramter_image.IDEX
            stru_paramter_image_final(0).ID_PRODUCCION = stru_paramter_image.ID_PRODUCCION
            stru_paramter_image_final(0).ID_TIPODOCUMENTO = stru_paramter_image.ID_TIPODOCUMENTO
            stru_paramter_image_final(0).PAG = stru_paramter_image.PAG
            stru_paramter_image_final(0).RADICADO = stru_paramter_image.RADICADO
            stru_paramter_image_final(0).TIPODOCUMENTO = stru_paramter_image.TIPODOCUMENTO
            stru_paramter_image_final(0).DBT_TIPO_IMAGEN = stru_paramter_image.DBT_TIPO_IMAGEN
            stru_paramter_image_final(0).DISC = stru_paramter_image.DISC
            stru_paramter_image_final(0).ENLACE = stru_paramter_image.ENLACE
            Result = ref_ClassDaGabinete.Solicita_imagenes_enlazadas_gabinete_produccion(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                                         stru_paramter_image.ENLACE,
                                                                                         stru_paramter_image_final,
                                                                                         stru_paramter_image.ID,
                                                                                         Id_tarea,
                                                                                         option_aplica_trd)
            If Result <> "YES" Then
                Solicita_lista_id_producion_relacionados_tarea_workflow = Result
                Exit Function
            End If
            Solicita_lista_id_producion_relacionados_tarea_workflow = "YES"
        Catch ex As Exception
            Solicita_lista_id_producion_relacionados_tarea_workflow = "Inconsistencia general funcion Solicita_lista_id_producion_relacionados_tarea_workflow " & ex.Message
        End Try
    End Function



    Function Activa_recupera_asigna_tarea(ByRef pag As Page,
                                         ByRef hdnEmailID As Object,
                                         ByRef mEval As Object,
                                         ByRef Hidden_id_tarea_selecionada As Object,
                                         ByRef ModalPopupExtenderRecuperar As Object,
                                         ByRef UpdatePanelintercambio As Object,
                                         ByRef UpdatePanelseleccion As Object,
                                         ByRef UpdatePanel_general_variable As Object) As String
        Try
            If HttpContext.Current.Session.Item("OPCIONSELECION") = "RECUPERARTAREA" Then
                Dim Result As String = ""
                If hdnEmailID.Value = "0" Then
                    HttpContext.Current.Session.Item("SESIONITERCAMBIO") = ""
                Else
                    HttpContext.Current.Session.Item("SESIONITERCAMBIO") = hdnEmailID.Value
                End If
                If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") <> "0" Then
                    Activa_recupera_asigna_tarea = "Usuario con tarea seleccionada imposible asignar el documento"
                    Exit Function
                End If
                If HttpContext.Current.Session.Item("SESIONITERCAMBIO") = "" Then
                    Activa_recupera_asigna_tarea = "Imposible asignar tarea por favor seleccione una tarea"
                    Exit Function
                End If
                'Valore split
                '0-id_tarea
                '1-id_actividad
                Dim Split() As String = HttpContext.Current.Session.Item("SESIONITERCAMBIO").ToString.Split("-")
                Dim Resultado As String = ""
                Dim Resultado_correo As String = ""
                'Result = Me.Seleccion_Tarea_Recuperada(Split(0), Split(1), Resultado, pag, mEval, 1, Resultado_correo, 0, 1)
                If Result <> "YES" Then
                    HttpContext.Current.Session.Item("SESIONITERCAMBIO") = ""
                    HttpContext.Current.Session.Item("OPCIONSELECION") = ""
                    ModalPopupExtenderRecuperar.Hide()
                    hdnEmailID.Value = "0"
                    Activa_recupera_asigna_tarea = Result
                    Exit Function
                Else
                    HttpContext.Current.Session.Item("SESIONITERCAMBIO") = ""
                    HttpContext.Current.Session.Item("OPCIONSELECION") = ""
                    hdnEmailID.Value = "0"
                    Hidden_id_tarea_selecionada.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                    UpdatePanel_general_variable.Update()
                    ModalPopupExtenderRecuperar.Hide()
                    UpdatePanelintercambio.Update()
                    UpdatePanelseleccion.Update()
                    If Resultado_correo <> "" Then
                        Activa_recupera_asigna_tarea = Resultado_correo
                        Exit Function
                    Else
                        Activa_recupera_asigna_tarea = "YES"
                        Exit Function
                    End If

                End If
            Else
                HttpContext.Current.Session.Item("SESIONITERCAMBIO") = ""
                HttpContext.Current.Session.Item("OPCIONSELECION") = ""
                hdnEmailID.Value = "0"
                Activa_recupera_asigna_tarea = "La opcion de seleccion global no coincide con la de RECUPERARTAREA se recomienda cerrar y iniciar sesion nuevamente"
                Exit Function
            End If
        Catch ex As Exception
            Activa_recupera_asigna_tarea = "Inconsistencia general función Activa_recupera_asigna_tarea " & ex.Message
        End Try
    End Function
    Public Shared Function Main(ByVal Conect_Wf As String,
                                ByVal Id_Usuario As String,
                                ByVal Id_Grupo As String,
                                ByVal Id_Actividad As String,
                                ByVal id_Tarea_sel As String,
                                ByVal Id_Ruta_Workflow As String) As String
        Try
            Dim Nombre_Ruta As String = ""
            Dim Numero_Actividades As String = ""
            Dim Fecha_Ini As String = ""
            Dim Fecha_Fin As String = ""
            Dim Matri_Campos_Lista() As String = Nothing
            Dim Sql_consulta As String = ""
            Dim CodigoSirep As String = ""
            Dim Result As String = ""
            Result = csfca_retorna_codigo_usuario_sirep_script(Conect_Wf,
                                                              Id_Usuario,
                                                              CodigoSirep)
            If Result <> "YES" Then
                Main = Result
                Exit Function
            End If
            '-------------------------------
            'Consulta id actividad
            '------------------------------
            Result = ""
            Result = csfca_solicita_id_Idactividad_Usuario_script(Id_Actividad,
                                                                  Id_Grupo,
                                                                  Conect_Wf)
            If Result <> "YES" Then
                Main = "Error Consultando Id Actividad " + Result
                Exit Function
            End If
            Dim campo_lista_tramite As String = ""
            Result = csfca_retorna_consulta_lita_tareas_script(Conect_Wf,
                                                               Id_Ruta_Workflow,
                                                               Matri_Campos_Lista,
                                                               campo_lista_tramite)
            If Result <> "YES" Then
                Main = Result
                Exit Function
            End If
            '-------------------------------
            'Consulta nombre de ruta
            '-------------------------------
            Result = csfca_Obtenr_Nombre_Ruta_Script(Nombre_Ruta,
                                                     Id_Ruta_Workflow,
                                                     Conect_Wf)
            If Result <> "YES" Then
                Main = Result
                Exit Function
            End If
            Dim tipo_actividad As String = ""
            Result = csfca_solicita_tipo_Actividad_General_escript(Conect_Wf,
                                                                   Id_Actividad,
                                                                   tipo_actividad)
            If Result <> "YES" Then
                Main = Result
                Exit Function
            End If
            Dim fitro_estado_modulo_correspondencia As String = ""
            If tipo_actividad = "ENLASE" Then
                fitro_estado_modulo_correspondencia = ""
            Else
                fitro_estado_modulo_correspondencia = " and estado_modulo_radicado = 0"
            End If
            Dim campos_obligatorios As String = "etw.Inicio_Tareas_Workflow_id_Tarea as id_tarea,etw.fecha_inicio,etw.estado_prioridad as prioridad,DAT.FLUJO_INTERNO_WF,"
            Sql_consulta = "Select " & campos_obligatorios &
                  campo_lista_tramite & ",wf_fl.NOMBRE_FLUJO_TRABAJO as FLUJO_TAREA,Fecha_Inicio as FECHAINICIOTRAMITE, Fecha_Fin AS FECHAFINALTRAMITE" & "  from " &
                  " estados_tarea_workflow etw " &
                  " inner join dat_adic_tar" & Nombre_Ruta & " as  DAT on " &
                  " (etw.Inicio_Tareas_Workflow_id_Tarea=DAT.INICIO_TAREAS_WORKFLOW_ID_TAREA  AND DAT.AUXILIAR='" & CodigoSirep & "')" &
                  " Left outer join wf_flujos_trabajo as wf_fl on (wf_fl.ID_WF_FLUJOS_TRABAJO=etw.ID_FLUJO_TRABAJO)"
            '" where (etw.id_actividad=" & Id_Actividad & _
            '" and etw.fecha_Seleccion is null and etw.fecha_fin is null  and etw.estado_tarea=0  " & limite_fecha & " and etw.id_usuario=" & Id_Usuario_Workflow & _
            '") or ( etw.id_actividad=" & Id_Actividad & " and etw.fecha_Seleccion is null and etw.fecha_fin is null  and etw.estado_tarea=0  and etw.id_usuario is null" & limite_fecha & _
            '") order by " & colum_order_name & " " & order_colum & limite_numero_lista_tareas
            'Sql_Consulta_Listado = Sql_Consulta_Listado & " etw.fecha_inicio,etw.estado_prioridad as prioridad,etw.Inicio_Tareas_Workflow_id_Tarea as id_tarea from " & _
            '"estados_tarea_workflow etw "
            ''-------------------------------
            ''Consulta nombre de ruta
            ''-------------------------------
            'Result = Obtenr_Nombre_RutaA(Nombre_Ruta, _
            '                             Id_Ruta_Workflow, _
            '                             Conect_Wf)
            'If Result <> "YES" Then
            '    Main = "Error Consultando Nombre de Ruta " + Result
            '    Exit Function

            'End If
            'Sql_Consulta_Listado = Sql_Consulta_Listado & "inner join dat_adic_tar" & Nombre_Ruta & " as  DAT on " & _
            '"(etw.Inicio_Tareas_Workflow_id_Tarea=DAT.INICIO_TAREAS_WORKFLOW_ID_TAREA AND DAT.AUXILIAR='" & CodigoSirep & "' )" & _
            '" inner join  configuracion_gabinete as cg on" & _
            '" (DAT.id_gabinete=cg.id_gabinete) "

            'Sql_Consulta_Listado = Sql_Consulta_Listado & "where etw.id_actividad=" & Id_Actividad & _
            '" and etw.fecha_fin is null and etw.id_usuario is null  and etw.estado_tarea=0"

            '---------------------------------
            'Consultando configuracion usuario
            '---------------------------------
            'Result = ""
            'Result = Leer_Datos_Configuracion_Usuario(Numero_Actividades, _
            '                                          Fecha_Ini, _
            '                                          Fecha_Fin, _
            '                                          Id_Usuario, _
            '                                          Conect_Wf)
            'If Result <> "YES" Then
            '    Main = "Error Consultando configuracion usuario " + Result
            '    Exit Function
            'End If
            'If Fecha_Ini <> "" And Fecha_Fin <> "" Then

            '    Sql_Consulta_Listado = Sql_Consulta_Listado & " and etw.fecha_inicio between '" & Fecha_Ini & _
            '    "' and '" & Fecha_Fin & "'"

            'End If
            'If Numero_Actividades <> "" Then
            '    Sql_Consulta_Listado = Sql_Consulta_Listado & " LIMIT " & Numero_Actividades
            'Else
            '    Sql_Consulta_Listado = Sql_Consulta_Listado & " LIMIT 2000"
            'End If
            Main = Sql_consulta
            Exit Function
        Catch ex As Exception
            Main = "Inconsistencia general función Main " & ex.Message
        End Try
    End Function
    Public Shared Function csfca_retorna_codigo_usuario_sirep_script(ByVal Conect_Wf As String,
                                                                     ByVal id_usuario As Object,
                                                                     ByRef CodigoSirep As Object) As String
        Try
            '********************************************************
            'Consulta el codigo de usuario sirep para el sirep para
            'asignacion del documento en workflow
            '********************************************************            
            Dim Sql_consulta = "Select CODIGO_CORTO_SIREP from RELACION_SIREP_WORKFLOW  " &
            " WHERE ID_UUSARIO_WORKFLOW=" & id_usuario
            Dim Dat_reader34 As MySqlDataReader
            Dim Re_sult34 As New MySqlCommand(Sql_consulta)
            Dim Data_Conexion_Mysql34 As New MySqlConnection(Conect_Wf)
            Data_Conexion_Mysql34.Open()
            Dim command34 As New MySqlCommand(Re_sult34.CommandText, Data_Conexion_Mysql34)
            Dat_reader34 = command34.ExecuteReader()
            If Dat_reader34 Is Nothing Then
                csfca_retorna_codigo_usuario_sirep_script = "Error funcion csfca_retorna_codigo_usuario_sirep_script  " & Sql_consulta
                Exit Function
            End If
            If Dat_reader34.HasRows = False Then
                Dat_reader34.Close()
                Data_Conexion_Mysql34.Close()
                csfca_retorna_codigo_usuario_sirep_script = "Usuario (" & id_usuario & ") sin usuario sirep relacionado en la tabla relacion sirep workflow funcion csfca_retorna_codigo_usuario_sirep_script"
                Exit Function
            Else
                Dat_reader34.Read()
                CodigoSirep = Dat_reader34.Item(0).ToString
                Dat_reader34.Close()
                Data_Conexion_Mysql34.Close()
                csfca_retorna_codigo_usuario_sirep_script = "YES"
                Exit Function
            End If
        Catch ex As Exception
            csfca_retorna_codigo_usuario_sirep_script = "Inconsistencia general funcion csfca_retorna_codigo_usuario_sirep " & ex.Message
        End Try
    End Function
    Public Shared Function csfca_solicita_id_Idactividad_Usuario_script(ByRef ID_ACTIVIDAD As String,
                                                                        ByVal id_Grupo As String,
                                                                        ByVal Conect_Wf As String) As String
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "Select ID_ACTIVIDAD from " &
            " GRUPOS_WORKFLOW WHERE ID_GRUPO =" & id_Grupo

            '*************************************************************************
            Dim Dat_reader32 As MySqlDataReader
            Dim Re_sult As New MySqlCommand(Sql_consulta)
            Dim Data_Conexion_Mysql As New MySqlConnection(Conect_Wf)
            Data_Conexion_Mysql.Open()
            Dim command As New MySqlCommand(Re_sult.CommandText, Data_Conexion_Mysql)
            Dat_reader32 = command.ExecuteReader()
            '**************************************************************************
            If Dat_reader32 Is Nothing Then
                csfca_solicita_id_Idactividad_Usuario_script = "Error funcion Solicita_id_Idactividad_Usuario_script " & Sql_consulta
                Exit Function
            End If
            If Dat_reader32.HasRows = False Then
                Dat_reader32.Close()
                Data_Conexion_Mysql.Close()
                csfca_solicita_id_Idactividad_Usuario_script = "Imposible Encontrar id actividad grupo (" & id_Grupo & ") funcion Solicita_id_Idactividad_Usuario_script"
                Exit Function
            Else
                Dat_reader32.Read()
                ID_ACTIVIDAD = Dat_reader32.Item(0).ToString
                Dat_reader32.Close()
                Data_Conexion_Mysql.Close()
                csfca_solicita_id_Idactividad_Usuario_script = "YES"
            End If
        Catch ex As Exception
            csfca_solicita_id_Idactividad_Usuario_script = "Inconsistencia general funcion Solicita_id_Idactividad_Usuario_script " & ex.Message
        End Try
    End Function
    Public Shared Function csfca_solicita_tipo_Actividad_General_escript(ByVal Conect_Wf As String,
                                                                         ByVal ID_ACTIVIDAD As String,
                                                                         ByRef Nombre_tipo_Actividad As String) As String
        '**********************************************************
        'Lista le tipo de actividad de usuario
        'Fecha Mod: 2012-11-16
        'Ing : Miguel Urueta Miranda 
        '**********************************************************
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT AGW.TIPO_ACTIVIDAD FROM LISTADO_ACTIVIDADES_WORKFLOW AS LAW " &
            " INNER JOIN ACTIVIDADES_GENERALES_WORKFLOW AS AGW " &
            " ON (LAW.ACTIVIDADES_GENERALES_WORKFLOW_ID_ACTIVIDAD_GENERAL = " &
            " AGW.ID_ACTIVIDAD_GENERAL) WHERE LAW.ID_ACTIVIDAD= " & ID_ACTIVIDAD
            '*************************************************************************
            Dim Dat_reader30 As MySqlDataReader
            Dim Re_sult As New MySqlCommand(Sql_consulta)
            Dim Data_Conexion_Mysql As New MySqlConnection(Conect_Wf)
            Data_Conexion_Mysql.Open()
            Dim command As New MySqlCommand(Re_sult.CommandText, Data_Conexion_Mysql)
            Dat_reader30 = command.ExecuteReader()
            '**************************************************************************
            If Dat_reader30 Is Nothing Then
                csfca_solicita_tipo_Actividad_General_escript = "Error funcion csfca_solicita_tipo_Actividad_General_escript " & Sql_consulta
                Exit Function
            End If
            If Dat_reader30.HasRows = False Then
                Dat_reader30.Close()
                Data_Conexion_Mysql.Close()
                csfca_solicita_tipo_Actividad_General_escript = "Inposible encontrar tipo actividad  (" & ID_ACTIVIDAD & "), funcion csfca_solicita_tipo_Actividad_General_escript "
                Exit Function
            Else
                Dat_reader30.Read()
                Nombre_tipo_Actividad = Dat_reader30.Item(0).ToString
                csfca_solicita_tipo_Actividad_General_escript = "YES"
                Dat_reader30.Close()
                Data_Conexion_Mysql.Close()
                Exit Function
            End If
        Catch ex As Exception
            csfca_solicita_tipo_Actividad_General_escript = "Inconsistencia general funcion csfca_solicita_tipo_Actividad_General_escript " & ex.Message
        End Try
    End Function
    Public Shared Function csfca_retorna_consulta_lita_tareas_script(ByVal Conect_Wf As String,
                                                                     ByVal id_ruta As Object,
                                                                     ByRef Matri_Campos_Lista() As String,
                                                                     ByRef campos_lista As String) As String
        Try
            Dim Sql_consulta As String = "Select NOMBRE_CAMPO from " &
            " CONFIGURACION_LISTADO_RUTA WHERE RUTAS_WORKFLOW_ID_RUTA =" & id_ruta &
            " AND LISTA_TAREA=1 order by id_campo"
            '*************************************************************************
            Dim Dat_reader30 As MySqlDataReader
            Dim Re_sult As New MySqlCommand(Sql_consulta)
            Dim Data_Conexion_Mysql As New MySqlConnection(Conect_Wf)
            Data_Conexion_Mysql.Open()
            Dim command As New MySqlCommand(Re_sult.CommandText, Data_Conexion_Mysql)
            Dat_reader30 = command.ExecuteReader()
            '**************************************************************************
            If Dat_reader30 Is Nothing Then
                csfca_retorna_consulta_lita_tareas_script = "Imposible Encontrar campos en la tabla CONFIGURACION_LISTADO_RUTA " & Sql_consulta
                Exit Function
            End If
            If Dat_reader30.HasRows = False Then
                Dat_reader30.Close()
                Data_Conexion_Mysql.Close()
                csfca_retorna_consulta_lita_tareas_script = "YES"
                Exit Function
            Else
                Erase Matri_Campos_Lista
                Dim i As Integer = 0
                While Dat_reader30.Read()
                    If campos_lista = "" Then
                        campos_lista = campos_lista & Dat_reader30.Item(0).ToString
                    Else
                        campos_lista = campos_lista & "," & Dat_reader30.Item(0).ToString
                    End If
                    i = i + 1
                End While
                Dat_reader30.Close()
                Data_Conexion_Mysql.Close()
                csfca_retorna_consulta_lita_tareas_script = "YES"
                Exit Function
            End If
        Catch ex As Exception
            csfca_retorna_consulta_lita_tareas_script = "Inconsistencia general función csfca_retorna_consulta_lita_tareas_script " & ex.Message
        End Try
    End Function
    Public Shared Function csfca_Obtenr_Nombre_Ruta_Script(ByRef Nombre_Ruta As String,
                                                           ByVal RutActividad As String,
                                                           ByVal Conect_Wf As String) As String
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "Select NOMBRE_RUTA from " &
            " RUTAS_WORKFLOW WHERE ID_RUTA =" & RutActividad
            '*************************************************************************
            Dim Dat_reader31 As MySqlDataReader
            Dim Re_sult As New MySqlCommand(Sql_consulta)
            Dim Data_Conexion_Mysql As New MySqlConnection(Conect_Wf)
            Data_Conexion_Mysql.Open()
            Dim command As New MySqlCommand(Re_sult.CommandText, Data_Conexion_Mysql)
            Dat_reader31 = command.ExecuteReader()
            '**************************************************************************
            If Dat_reader31 Is Nothing Then
                csfca_Obtenr_Nombre_Ruta_Script = "Error funcion csfca_Obtenr_Nombre_Ruta_Script " & Sql_consulta
                Exit Function
            End If
            If Dat_reader31.HasRows = False Then
                Dat_reader31.Close()
                Data_Conexion_Mysql.Close()
                csfca_Obtenr_Nombre_Ruta_Script = "Imposible Encontrar el nombre de codigo de ruta (" & RutActividad & ") funcion " & RutActividad
                Exit Function
            Else
                Dat_reader31.Read()
                Nombre_Ruta = Dat_reader31.Item(0).ToString
                csfca_Obtenr_Nombre_Ruta_Script = "YES"
                Dat_reader31.Close()
                Data_Conexion_Mysql.Close()
            End If
        Catch ex As Exception
            csfca_Obtenr_Nombre_Ruta_Script = "Inconsistencia general funcion funcion csfca_Obtenr_Nombre_Ruta_Script " & ex.Message
        End Try

    End Function


End Class
