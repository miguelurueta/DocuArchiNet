Imports MySql.Data.MySqlClient
Imports System
Imports System.IO
Imports System.Drawing
Imports System.Web.Script.Serialization
Imports MindFusion.Diagramming
Imports MindFusion.Diagramming.WebForms
Imports MindFusion.Diagramming.Import.VisioImporter
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports Graphics = System.Drawing.Graphics
Imports MindFusion.Diagramming.Components

Public Structure Matri_Typo_campo_Mysql
    Dim Tipo_Campo As String
    Dim Longitud_Campo As String
End Structure
Public Structure Estru_campo_indice
    Dim id_campo_indice As Integer
    Dim indice_campo As Integer
End Structure
Public Structure stru_estado_tarea
    Dim id_Estado As Long
    Dim Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta As Integer
    Dim Inicio_Tareas_Workflow_id_Tarea As Long
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
Public Structure stru_conector_estado
    Dim id_Estado As Long
    Dim Id_Actividad As Long
End Structure

Public Class Class_worflow_rutas
    Function Importar_ruta_workflow_general(ByVal archivo_ruta_temporal As String, _
                                            ByRef ruta_archivo As String, _
                                            ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView, _
                                            ByRef nombre_ruta As String, _
                                            ByRef ref_update As UpdatePanel, _
                                            ByRef ref_droplist_ruta As DropDownList, _
                                            ByRef ref_upda_panel_droptlist As UpdatePanel) As String
        Try
            If nombre_ruta = "" Then
                Importar_ruta_workflow_general = "Por favor seleccione el nombre de la ruta"
                Exit Function
            End If
            If archivo_ruta_temporal = "" Then
                Importar_ruta_workflow_general = "Por favor seleccione el archivo a importar"
                Exit Function
            End If
            Dim ob As New MindFusion.Diagramming.Import.VisioImporter
            Dim diagran As New MindFusion.Diagramming.DiagramDocument
            diagran = ob.Import(archivo_ruta_temporal)
            ref_diagram.Diagram = diagran.Pages(0)
            '--------------------------------------------
            'Retorna el id de la ruta
            '--------------------------------------------
            Dim Refclasworkflow As New ClassWorkflow
            Dim Result As String = ""
            Dim id_ruta As Integer = 0
            Dim Ref_class_wf_ruta As New Class_worflow_rutas
            Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(nombre_ruta, _
                                                                id_ruta)
            If Result <> "YES" Then
                Importar_ruta_workflow_general = Result
                Exit Function
            End If
            '--------------------------------------------
            'Recorrer el diagrama para listar los shapes
            '--------------------------------------------
            For Each sha As Object In ref_diagram.Diagram.Items
                Dim obdi As Object = sha.GetType
                If obdi.Fullname = "MindFusion.Diagramming.ShapeNode" Then
                    Dim id_actividad As Integer = 0
                    Dim re_trim = Trim(sha.Text)
                    re_trim = Trim(re_trim)
                    re_trim = re_trim.Replace(vbCrLf, "")
                    '---Retorna el id de la actividad por nombre
                    Result = Retorna_Id_Actividad_por_id_ruta(re_trim, id_ruta, id_actividad)
                    If Result <> "YES" Then
                        Importar_ruta_workflow_general = Result
                        Exit Function
                    End If
                    sha.id = id_actividad
                    '----Retorna tipo actividad
                    Dim nombre_tipo_actividad As String = ""
                    Result = Retorna_tipo_actividad(id_actividad, nombre_tipo_actividad)
                    If Result <> "YES" Then
                        Importar_ruta_workflow_general = Result
                        Exit Function
                    End If
                    '----Aplica estilo al shape
                    Result = Aplica_estilo_shape(sha, nombre_tipo_actividad)
                    If Result <> "YES" Then
                        Importar_ruta_workflow_general = Result
                        Exit Function
                    End If
                End If

            Next
            '-----------------------------------------------------
            'Función recorre los conectores link
            '-----------------------------------------------------
            For Each sha As Object In ref_diagram.Diagram.Items
                Dim obdi As Object = sha.GetType
                If obdi.Fullname = "MindFusion.Diagramming.DiagramLink" Then
                    Dim re_trim = Trim(sha.Text)
                    re_trim = Trim(re_trim)
                    re_trim = re_trim.Replace(vbCrLf, "")
                    Dim spli_link_text() As String = re_trim.Split("|")
                    '-----Retorna el id del diagrama link
                    Dim identicacion_conector As Integer = 0
                    Result = Solicita_id_conector_identificacion_grafica(spli_link_text(0), id_ruta, identicacion_conector)
                    If Result <> "YES" Then
                        Importar_ruta_workflow_general = Result
                        Exit Function
                    End If
                    sha.id = id_ruta.ToString & "_" & identicacion_conector.ToString
                    Result = Aplica_estilo_conector(sha)
                    If Result <> "YES" Then
                        Importar_ruta_workflow_general = Result
                        Exit Function
                    End If
                End If
            Next
            ref_diagram.ZoomFactor = 75
            Dim Ruta_archivo_guardado As String = ""
            ''--------------------------------------------------
            ''Guarda el archivo en el sistema de archivo
            ''--------------------------------------------------
            'Result = Salva_archivo_file_sistem(ref_diagram, Ruta_archivo_guardado, nombre_ruta)
            'If Result <> "YES" Then
            '    Importar_ruta_workflow_general = Result
            '    Exit Function
            'End If
            '-------------------------------------------------
            'Guarda el archivo en la base de datos
            '-------------------------------------------------
            Dim string_plantilla As String = ref_diagram.SaveToString(SaveToStringFormat.Base64, True)
            Result = Guarda_archivo_base_datos_string(string_plantilla, id_ruta)
            If Result <> "YES" Then
                Importar_ruta_workflow_general = Result
                Exit Function
            End If
            HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA") = nombre_ruta
            ref_update.Update()
            '-------------------------------------------------
            'Lista las rutas en el droplist principal
            '-------------------------------------------------
            If ref_droplist_ruta.Items.Count = 0 Then
                ref_droplist_ruta.Items.Add("")
                ref_droplist_ruta.Items.Add(nombre_ruta)
            Else
                ref_droplist_ruta.Items.Add(nombre_ruta)
            End If
            ref_upda_panel_droptlist.Update()
            Importar_ruta_workflow_general = "YES"
        Catch ex As Exception
            Importar_ruta_workflow_general = "Inconsistencia general función Importar_ruta_workflow_general " & ex.Message
        End Try
    End Function
    Function Eliminar_elemento_diagrama_web(ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView, _
                                            ByRef ref_update As UpdatePanel, _
                                            ByVal nombre_ruta As String) As String
        Try
            Dim Result As String = ""
            If ref_diagram.Diagram.Selection.Items.Count = 0 Then
                Eliminar_elemento_diagrama_web = "Por favor seleccione el elemento del diagrama "
                Exit Function
            End If
            If ref_diagram.Diagram.Selection.Items.Count > 1 Then
                Eliminar_elemento_diagrama_web = "Solo se puede eliminar un elemento del diagrama"
                Exit Function
            End If
            '-----------------------------------------------------------
            'Elimina elementos del digrama que pertenecen a actividades
            '-----------------------------------------------------------
            Dim sha As Object = Nothing
            For Each sha In ref_diagram.Diagram.Selection.Items
                If ref_diagram.Diagram.Selection.Items(0).GetType.FullName = "MindFusion.Diagramming.ShapeNode" Then
                    Result = Me.Elimina_actividad_workflow(ref_diagram, _
                                                           ref_update, _
                                                           nombre_ruta, _
                                                           sha)
                    If Result <> "YES" Then
                        Eliminar_elemento_diagrama_web = Result
                        Exit Function
                    Else
                        Exit For
                    End If
                End If
            Next
            '----------------------------------------------------------
            'Elimina elementos conectores del diagrama 
            '----------------------------------------------------------
            sha = Nothing
            For Each sha In ref_diagram.Diagram.Selection.Items
                If ref_diagram.Diagram.Selection.Items(0).GetType.FullName = "MindFusion.Diagramming.DiagramLink" Then
                    Result = Me.Elimina_conector_actividades_workflow(ref_diagram, _
                                                                      ref_update, _
                                                                      nombre_ruta, _
                                                                      sha)
                    If Result <> "YES" Then
                        Eliminar_elemento_diagrama_web = Result
                        Exit Function
                    Else
                        Exit For
                    End If
                End If
            Next
            Eliminar_elemento_diagrama_web = "YES"
        Catch ex As Exception
            Eliminar_elemento_diagrama_web = "Inconsistencia general función Eliminar_elemento_diagrama_web " & ex.Message
        End Try

    End Function
    Function Elimina_conector_actividades_workflow(ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView, _
                                                   ByRef ref_update As UpdatePanel, _
                                                   ByVal nombre_ruta As String, _
                                                   ByRef shape As MindFusion.Diagramming.DiagramLink) As String
        Dim Result As String = ""
        If shape.Id Is Nothing Then
            Elimina_conector_actividades_workflow = "El id del DiagramLink se ecuentra en estado nothing "
            Exit Function
        End If
        '----------------------------------------
        'Función retorna id ruta
        '----------------------------------------
        Dim Refclas_workflow As New ClassWorkflow
        Dim id_ruta As Integer = 0
        Dim Ref_class_wf_ruta As New Class_worflow_rutas
        Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(nombre_ruta, _
                                                            id_ruta)
        If Result <> "YES" Then
            Elimina_conector_actividades_workflow = Result
            Exit Function
        End If
        Dim matri_id_link() As String = shape.Id.ToString.Split("_")
        Dim id_link As Integer = Val(matri_id_link(1))
        Dim Sql_eliminar As String = "delete from ACTIVIDADES_DISPONIBLES_ENVIO where id_actividades_disponibles_envio=" & id_link & _
            " and id_Ruta=" & id_ruta
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        ref.Returna_Conexion_Mysql(myConnection)
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = Sql_eliminar
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Elimina_conector_actividades_workflow = "Imposible eliminar el conector  "
                myConnection.Close()
                Exit Function
            End If
            '----------------------------------
            'Elimina el link del diagrama
            '----------------------------------
            ref_diagram.Diagram.Items.Remove(shape)
            Result = Me.Guardar_ruta_workflow(ref_diagram, nombre_ruta)
            If Result <> "YES" Then
                If ref_diagram.Diagram.UndoManager.History.Commands.Count > 0 Then
                    ref_diagram.Diagram.UndoManager.Undo()
                End If
                ref_update.Update()
                Elimina_conector_actividades_workflow = Result
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            ref_update.Update()
            myTrans.Commit()
            myConnection.Close()
            Elimina_conector_actividades_workflow = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Elimina_conector_actividades_workflow = "An exception of type " + ex.GetType().ToString() + _
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                End If
            End Try
            myConnection.Close()
            Elimina_conector_actividades_workflow = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Elimina_actividad_workflow(ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView, _
                                        ByRef ref_update As UpdatePanel, _
                                        ByVal nombre_ruta As String, _
                                        ByRef shape As MindFusion.Diagramming.ShapeNode) As String
        If shape.Id Is Nothing Then
            Elimina_actividad_workflow = "El id del shape se ecuentra en estado nothing "
            Exit Function
        End If

        Dim id_actividad As Integer = shape.Id
        '----------------------------------------
        'Función retorna id ruta
        '----------------------------------------
        Dim Result As String = ""
        Dim Refclas_workflow As New ClassWorkflow
        Dim id_ruta As Integer = 0
        Dim Ref_class_wf_ruta As New Class_worflow_rutas
        Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(nombre_ruta, _
                                                            id_ruta)
        If Result <> "YES" Then
            Elimina_actividad_workflow = Result
            Exit Function
        End If
        '---------------------------------------------
        'Retorna id grupo relacionado a la actividad
        '---------------------------------------------
        Dim id_grupo_relacionado As Integer = 0
        Result = Me.Solicita_id_grupo_relacionado_ruta(id_actividad, id_ruta, id_grupo_relacionado)
        If Result <> "YES" Then
            Elimina_actividad_workflow = Result
            Exit Function
        End If
        If id_grupo_relacionado <> 0 Then
            Dim estado_resultado_existencia_usuario As String = "YES"
            Result = Me.Verifica_existencia_usuario_relacionado_grupo_workflow(id_grupo_relacionado, id_ruta, estado_resultado_existencia_usuario)
            If Result <> "YES" Then
                Elimina_actividad_workflow = Result
                Exit Function
            End If
            If estado_resultado_existencia_usuario = "YES" Then
                Elimina_actividad_workflow = "El sistema detecto que la actividad relacionada tiene un grupo con usuario relacionados, imposible continuar "
                Exit Function
            End If

        End If
        '----------------------------------------------
        'Verifica la existencia de actividades
        '----------------------------------------------
        Dim estado_existencia_registro_actividades_ruta As String = "YES"
        Result = Me.Verifica_existencia_registros_estados_tareas(id_actividad, id_ruta, estado_existencia_registro_actividades_ruta)
        If Result <> "YES" Then
            Elimina_actividad_workflow = Result
            Exit Function
        End If
        '---------------------------------------------------------
        'Verfica existencia conector relacionado a la actividad
        '---------------------------------------------------------
        Dim estado_existencia_conexion_actividad As String = "YES"
        Result = Me.Verifica_existencia_conexion_como_fuente(id_actividad, id_ruta, estado_existencia_conexion_actividad)
        If Result <> "YES" Then
            Elimina_actividad_workflow = Result
            Exit Function
        End If
        If estado_existencia_conexion_actividad = "YES" Then
            Elimina_actividad_workflow = "La actividad esta conectada a otra actividad como actividad inicial, imposible eliminar "
            Exit Function
        End If
        Result = Me.Verifica_existencia_conexion_como_destino(id_actividad, id_ruta, estado_existencia_conexion_actividad)
        If Result <> "YES" Then
            Elimina_actividad_workflow = Result
            Exit Function
        End If
        If estado_existencia_conexion_actividad = "YES" Then
            Elimina_actividad_workflow = "La actividad esta conectada a otra actividad como actividad destino, imposible eliminar "
            Exit Function
        End If
        Dim Sql_Insercion_actividad As String = "delete from  LISTADO_ACTIVIDADES_WORKFLOW where RUTAS_WORKFLOW_ID_RUTA=" _
           & id_ruta & " and Id_Actividad=" & id_actividad

        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        ref.Returna_Conexion_Mysql(myConnection)
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = Sql_Insercion_actividad
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Elimina_actividad_workflow = "Imposible eliminar actividad  "
                myConnection.Close()
                Exit Function
            End If
            Dim sql_insert_eventos As String = ""
            sql_insert_eventos = "Delete from  SCRIPT_ACTIVIDADES " _
            & "where Listado_Actividades_Workflow_Id_Actividad=" & id_actividad
            myCommand.CommandText = sql_insert_eventos
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Elimina_actividad_workflow = "Imposible eliminar los eventos de la actividad  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If id_grupo_relacionado <> 0 Then
                Dim SqlInsert = "Delete from grupos_workflow " _
                   & "where Rutas_Workflow_id_Ruta=" & _
                    id_ruta & " and Id_Grupo=" & id_grupo_relacionado & " and id_Actividad=" & id_actividad
                myCommand.CommandText = SqlInsert
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Elimina_actividad_workflow = "Imposible eliminar grupo worokflow  "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            '----------------------------------
            'Agrega el shape al diagrama
            '----------------------------------
            ref_diagram.Diagram.Items.Remove(shape)
            Result = Me.Guardar_ruta_workflow(ref_diagram, nombre_ruta)
            If Result <> "YES" Then
                If ref_diagram.Diagram.UndoManager.History.Commands.Count > 0 Then
                    ref_diagram.Diagram.UndoManager.Undo()
                End If
                ref_update.Update()
                Elimina_actividad_workflow = Result
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            ref_update.Update()
            myTrans.Commit()
            myConnection.Close()
            Elimina_actividad_workflow = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Elimina_actividad_workflow = "An exception of type " + ex.GetType().ToString() + _
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                End If
            End Try
            myConnection.Close()
            Elimina_actividad_workflow = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Solicita_id_grupo_relacionado_ruta(ByVal id_actividad As Integer,
                                                ByVal id_ruta As Integer,
                                                ByRef id_grupo_relacionado As Integer) As String
        '-----------------------------------------------
        'Función : El sistema solicita el id del 
        'grupo relacionado
        'Fecha : 2017-07-28
        'Ing : Miguel Angel Urueta Miranda
        '-----------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select Id_Grupo from grupos_workflow where id_Actividad =" & id_actividad & " and Rutas_Workflow_id_Ruta=" & id_ruta
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("grupos_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_grupo_relacionado_ruta = "Función Solicita_id_grupo_relacionado_ruta dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_grupo_relacionado = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_grupo_relacionado_ruta = "YES"
                Exit Function
            Else
                id_grupo_relacionado = 0
                Solicita_id_grupo_relacionado_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_grupo_relacionado_ruta = "Inconsistencia general función Solicita_id_grupo_relacionado_ruta " & ex.Message
        End Try

    End Function
    Function Verifica_existencia_usuario_relacionado_grupo_workflow(ByVal id_grupo As Integer, ByVal id_ruta As Integer, _
                                                                    ByRef estado_resultado As String) As String
        '------------------------------------------------
        'Funcion : Retorna existencia del resultado
        'Fecha : 2017-07-28
        'Ingeniero : Miguel Angel Urueta Miranda
        '------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select Grupos_Workflow_Id_Grupo from usuario_workflow where Grupos_Workflow_Id_Grupo =" & id_grupo & " and Grupos_Workflow_Rutas_Workflow_id_Ruta=" & id_ruta
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("usuario_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_usuario_relacionado_grupo_workflow = "Función Verifica_existencia_usuario_relacionado_grupo_workflow dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                estado_resultado = "YES"
                Verifica_existencia_usuario_relacionado_grupo_workflow = "YES"
                Exit Function
            Else
                estado_resultado = "NO"
                Verifica_existencia_usuario_relacionado_grupo_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_usuario_relacionado_grupo_workflow = "Inconsistencia general función Verifica_existencia_usuario_relacionado_grupo_workflow " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_registros_estados_tareas(ByVal id_actividad As Integer, ByVal id_ruta As Integer, _
                                                          ByRef estado_existencia_registro_actividades_ruta As String) As String
        '----------------------------------------------------------------------------------------
        'Funcion : Retorna existencia del resultado tareas de la actividad registras en la ruta
        'Fecha : 2017-07-28
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select id_Estado from estados_tarea_workflow where Id_Actividad =" & id_actividad & " and Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta=" & id_ruta
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_registros_estados_tareas = "Función Verifica_existencia_registros_estados_tareas dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                estado_existencia_registro_actividades_ruta = "YES"
                Verifica_existencia_registros_estados_tareas = "YES"
                Exit Function
            Else
                estado_existencia_registro_actividades_ruta = "NO"
                Verifica_existencia_registros_estados_tareas = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_registros_estados_tareas = "Inconsistencia general función Verifica_existencia_registros_estados_tareas " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_conexion_como_fuente(ByVal id_actividad As Integer, ByVal id_ruta As Integer, ByRef estado_existencia_conexion_actividad As String) As String
        '----------------------------------------------------------------------------------------
        'Funcion : Retorna existencia conexión como actividad fuente
        'Fecha : 2017-07-28
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select id_Ruta from actividades_disponibles_envio where Listado_Actividades_Workflow_Id_Actividad =" & id_actividad & " and id_Ruta=" & id_ruta
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("actividades_disponibles_envio")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_conexion_como_fuente = "Función Verifica_existencia_conexion_como_fuente dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                estado_existencia_conexion_actividad = "YES"
                Verifica_existencia_conexion_como_fuente = "YES"
                Exit Function
            Else
                estado_existencia_conexion_actividad = "NO"
                Verifica_existencia_conexion_como_fuente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_conexion_como_fuente = "Inconsistencia general función Verifica_existencia_conexion_actividad_otras_actividades " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_conexion_como_destino(ByVal id_actividad As Integer, ByVal id_ruta As Integer, ByRef estado_existencia_conexion_actividad As String) As String
        '----------------------------------------------------------------------------------------
        'Funcion : Retorna existencia conexión como actividad destino
        'Fecha : 2017-07-28
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select id_Ruta from actividades_disponibles_envio where Id_Actividad_Siguiente =" & id_actividad & " and id_Ruta=" & id_ruta
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("actividades_disponibles_envio")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_conexion_como_destino = "Función Verifica_existencia_conexion_como_destino dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                estado_existencia_conexion_actividad = "YES"
                Verifica_existencia_conexion_como_destino = "YES"
                Exit Function
            Else
                estado_existencia_conexion_actividad = "NO"
                Verifica_existencia_conexion_como_destino = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_conexion_como_destino = "Inconsistencia general función Verifica_existencia_conexion_como_destino " & ex.Message
        End Try
    End Function
    Function Lista_ruta_trabajo_tarea_worflow_grafico(ByVal id_ruta As Integer, ByVal id_tarea As Integer, ByVal radicado As String, ByRef diagramView As Object, _
                                                       ByRef UpdatePanel_diagran_view As Object, ByRef CheckBox_Grid_alineamiento As Object, _
                                                       ByRef Label_nombre_flujo_trabjo As Label) As String
        Try
            Dim Refclas As New ClassWorkflow
            Dim Result As String = ""
            Dim nombre_ruta As String = ""
            Dim Refclas_rad As New ClassRadicador
            Dim Ref_class_ruta As New Class_worflow_rutas
            Result = Ref_class_ruta.Retorna_nombre_ruta_por_id_ruta(id_ruta.ToString, _
                                                                    nombre_ruta)
            If Result <> "YES" Then
                Lista_ruta_trabajo_tarea_worflow_grafico = Result
                Exit Function
            End If
            Result = Abre_ruta_workflow(nombre_ruta, diagramView, UpdatePanel_diagran_view, CheckBox_Grid_alineamiento)
            If Result <> "YES" Then
                Lista_ruta_trabajo_tarea_worflow_grafico = Result
                Exit Function
            End If
            Dim Matri_actividades_recorri() As Integer = Nothing
            Result = Me.Retorna_actividades_recorridas_unicas_ruta_trabajo(id_tarea, _
                                                                           id_ruta, _
                                                                           Matri_actividades_recorri)
            If Result <> "YES" Then
                Lista_ruta_trabajo_tarea_worflow_grafico = Result
                Exit Function
            End If
            If Not Matri_actividades_recorri Is Nothing Then
                Result = Me.Marca_Actividad_ruta_workflow_recorridas(Matri_actividades_recorri, diagramView, UpdatePanel_diagran_view)
                If Result <> "YES" Then
                    Lista_ruta_trabajo_tarea_worflow_grafico = Result
                    Exit Function
                End If
                Dim id_actividad As Integer = 0
                Result = Me.Solicita_id_actividad_tarea_seleccionada_ruta_workflow(id_ruta, id_tarea, id_actividad)
                If Result <> "YES" Then
                    Lista_ruta_trabajo_tarea_worflow_grafico = Result
                    Exit Function
                End If
                If id_actividad <> 0 Then
                    Result = Me.Marca_Actividad_ruta_workflow(id_actividad, diagramView, UpdatePanel_diagran_view)
                    If Result <> "YES" Then
                        Lista_ruta_trabajo_tarea_worflow_grafico = Result
                        Exit Function
                    End If
                End If
            End If
            Dim matri_conector() As stru_conector_estado = Nothing
            Result = Me.Retorna_actividades_recorridas_ruta_trabajo(id_tarea, _
                                                                    id_ruta, _
                                                                    matri_conector)
            If Result <> "YES" Then
                Lista_ruta_trabajo_tarea_worflow_grafico = Result
                Exit Function
            End If
            If Not matri_conector Is Nothing Then
                If matri_conector.Length > 1 Then
                    For i As Integer = 0 To matri_conector.Length - 1
                        If i < matri_conector.Length - 1 Then
                            Result = Me.Conecta_actividades_recorrido_ruta(matri_conector(i).Id_Actividad, matri_conector(i + 1).Id_Actividad, _
                                                                           (i + 1), diagramView, UpdatePanel_diagran_view, matri_conector(i).id_Estado)
                            If Result <> "YES" Then
                                Lista_ruta_trabajo_tarea_worflow_grafico = Result
                                Exit Function
                            End If
                        End If
                    Next
                End If
            End If
            Create_label_diagrama(diagramView, UpdatePanel_diagran_view, nombre_ruta, radicado, id_tarea, 1)
            Label_nombre_flujo_trabjo.Text = "Ruta de trabajo (" & nombre_ruta & ")  Radicado relacionado (" & radicado & ") Indentificación de la tarea (" & id_tarea & ")"
            Lista_ruta_trabajo_tarea_worflow_grafico = "YES"
            Exit Function
        Catch ex As Exception
            Lista_ruta_trabajo_tarea_worflow_grafico = "Inconsistencia general función Lista_ruta_trabajo_tarea_worflow_grafico " & ex.Message
        End Try
    End Function
    Function Create_label_diagrama(ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView, _
                                    ByRef ref_update As UpdatePanel, _
                                    ByVal nombre_ruta As String, _
                                    ByVal radicado As String, _
                                    ByVal id_tarea As Integer, _
                                    ByVal marca_descripcion As Integer) As String
        Try

            Dim sap As New MindFusion.Diagramming.ShapeNode
            Dim Stroke = New MindFusion.Drawing.Pen(Color.Black, 0)
            Dim x As Integer = 0
            Dim y As Integer = 0
            '-------------------------------------------
            'Posiciona el shape en el grid del diagrama
            '-------------------------------------------
            x = 2
            y = (ref_diagram.Diagram.Bounds.Height) - 1
            Dim Rect = New RectangleF(x, y, 400, 5)
            sap.Bounds = Rect
            Dim sysdra = New System.Drawing.Font("Bold", 12)
            sap.Font = sysdra
            Dim penBrush As MindFusion.Drawing.Brush = New MindFusion.Drawing.LinearGradientBrush( _
             Color.White, Color.White, 0)
            sap.Brush = penBrush
            sap.Transparent = True
            sap.Locked = True
            sap.Text = "Ruta de trabajo (" & nombre_ruta & ")  Radicado relacionado (" & radicado & ") Indentificación de la tarea (" & id_tarea & ") fecha diagrama " & Now.ToString
            ref_diagram.Diagram.Items.Add(sap)
            If marca_descripcion = 1 Then
                sap = New MindFusion.Diagramming.ShapeNode
                Stroke = New MindFusion.Drawing.Pen(Color.Black, 0)
                sap.Transparent = True
                sap.Locked = True
                x = 1
                y = (ref_diagram.Diagram.Bounds.Height) - 10
                Rect = New RectangleF(x, y, 215, 30)
                sap.Bounds = Rect
                sap.ImageUrl = "../workflow/imageneswf/contenido_recto.png"
                ref_diagram.Diagram.Items.Add(sap)
            End If
            ref_update.Update()
            Create_label_diagrama = "YES"
        Catch ex As Exception
            Create_label_diagrama = "Inconsistencia genera función Create_label_diagrama " & ex.Message
        End Try
    End Function
    Function Conecta_actividades_recorrido_ruta(ByVal id_actividad_fuente As Integer, _
                                                ByVal id_actividad_destino As Integer, _
                                                ByVal identificador_conector As Integer, _
                                                ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView, _
                                                ByRef ref_update As UpdatePanel, _
                                                ByVal id_estado As Long) As String
        Try
            Dim sha_fuente As MindFusion.Diagramming.ShapeNode = Nothing
            Dim sha_destino As MindFusion.Diagramming.ShapeNode = Nothing
            For Each sha_ As Object In ref_diagram.Diagram.Items
                Dim ob As Object = sha_.GetType
                If ob.Fullname = "MindFusion.Diagramming.ShapeNode" Then
                    If sha_.id = id_actividad_fuente Then
                        sha_fuente = sha_
                    End If
                    If sha_.id = id_actividad_destino Then
                        sha_destino = sha_
                    End If
                End If
            Next
            If Not sha_fuente Is Nothing And Not sha_destino Is Nothing Then
                Dim link As MindFusion.Diagramming.DiagramLink
                Dim Stroke = New MindFusion.Drawing.Pen(Color.Red, 0)
                link = ref_diagram.Diagram.Factory.CreateDiagramLink(sha_fuente, sha_destino)
                link.AutoRoute = True
                link.AutoSnapToNode = False
                'EVITA QUE SE MUEVA EL CONECTOR FINAL
                link.AllowMoveEnd = False
                link.AllowMoveStart = False
                link.DrawCrossings = False
                link.CrossingRadius = 1
                link.DrawCrossings = True
                link.Pen = Stroke
                link.AddLabel(identificador_conector.ToString)
                link.Id = id_estado
                link.Tag = "Traza"
                Dim Fill = New MindFusion.Drawing.SolidBrush(Color.Yellow)
                link.HeadBrush = Fill
            End If
            Conecta_actividades_recorrido_ruta = "YES"
        Catch ex As Exception
            Conecta_actividades_recorrido_ruta = "Inconsistencia general función Conecta_actividades_recorrido_ruta " & ex.Message
        Finally
            ref_update.Update()
        End Try
    End Function
    Function Abre_ruta_workflow(ByVal nombre_ruta As String, ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView, _
                                ByRef ref_update As UpdatePanel, ByRef ref_CheckBox_Grid_alineamiento As CheckBox) As String
        Try

            If nombre_ruta = "" Then
                Abre_ruta_workflow = "Debe seleccionar una ruta"
                Exit Function
            End If
            Dim Parametro_Consulta As String = "select Archivo_Plantilla_Mindifucion from RUTAS_WORKFLOW where NOMBRE_RUTA ='" & nombre_ruta & "' and ESTADO_RUTA = 1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("rutas_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Abre_ruta_workflow = Result
                Exit Function
            End If
            Dim bDatos() As Byte = Nothing
            Dim string_plantilla As String = ""
            If Datset.Tables(0).Rows.Count > 0 Then
                string_plantilla = Datset.Tables(0).Rows(0).Item(0)
                Dim path_temporal As String = ""
                If string_plantilla = "" Then
                    Abre_ruta_workflow = "Imposible extraer el diagrama de la base de datos"
                    Exit Function
                Else
                    HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA") = nombre_ruta
                    ref_diagram.Diagram.LoadFromString(string_plantilla)
                    ref_diagram.ZoomFactor = 75
                    If ref_diagram.Diagram.ShowGrid = True Then
                        ref_CheckBox_Grid_alineamiento.Checked = True
                    Else
                        ref_CheckBox_Grid_alineamiento.Checked = False
                    End If
                    ref_update.Update()
                    Abre_ruta_workflow = "YES"
                End If
            Else
                Abre_ruta_workflow = "Imposible encontrar la ruta "
                Exit Function
            End If
        Catch ex As Exception
            Abre_ruta_workflow = "Inconsistencia general función Abre_ruta_workflow " & ex.Message
        End Try
    End Function
    Function Retorna_actividades_recorridas_unicas_ruta_trabajo(ByVal id_tarea As Integer, _
                                                               ByVal id_ruta As Integer, _
                                                                ByRef matri_id_actividades() As Integer) As String
        Try
            matri_id_actividades = Nothing
            Dim Parametro_Consulta As String = "SELECT distinct(Id_Actividad) from estados_tarea_workflow where inicio_tareas_workflow_id_tarea ='" & _
                id_tarea & "' and Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta=" & id_ruta & " order by id_Estado "
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_actividades_recorridas_unicas_ruta_trabajo = "Error función Retorna_actividades_recorridas_unicas_ruta_trabajo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve matri_id_actividades(i)
                    matri_id_actividades(i) = Datset.Tables(0).Rows(i).Item(0)
                Next
                Retorna_actividades_recorridas_unicas_ruta_trabajo = "YES"
                Exit Function
            Else
                Retorna_actividades_recorridas_unicas_ruta_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_actividades_recorridas_unicas_ruta_trabajo = "Inconsistencia general función Retorna_actividades_recorridas_unicas_ruta_trabajo " & ex.Message
        End Try
    End Function
    Function Retorna_actividades_recorridas_ruta_trabajo(ByVal id_tarea As Integer, _
                                                         ByVal id_ruta As Integer, _
                                                         ByRef matri_id_actividades() As stru_conector_estado) As String
        Try
            matri_id_actividades = Nothing
            Dim Parametro_Consulta As String = "SELECT id_Estado,Id_Actividad from estados_tarea_workflow where inicio_tareas_workflow_id_tarea =" & _
                id_tarea & " and Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta=" & id_ruta & " order by id_Estado "
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_actividades_recorridas_ruta_trabajo = "Error función Retorna_actividades_recorridas_ruta_trabajo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve matri_id_actividades(i)
                    matri_id_actividades(i).id_Estado = Datset.Tables(0).Rows(i).Item(0)
                    matri_id_actividades(i).Id_Actividad = Datset.Tables(0).Rows(i).Item(1)
                Next
                Retorna_actividades_recorridas_ruta_trabajo = "YES"
                Exit Function
            Else
                Retorna_actividades_recorridas_ruta_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_actividades_recorridas_ruta_trabajo = "Inconsistencia general función Retorna_actividades_recorridas_ruta_trabajo " & ex.Message
        End Try
    End Function
    Function Marca_Actividad_ruta_workflow_recorridas(ByVal matri_id_actividades() As Integer, ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView, _
                               ByRef ref_update As UpdatePanel)
        Try
            For Each sha_ As Object In ref_diagram.Diagram.Items
                Dim ob As Object = sha_.GetType
                If ob.Fullname = "MindFusion.Diagramming.ShapeNode" Then
                    For i As Integer = 0 To matri_id_actividades.Length - 1
                        If sha_.id = matri_id_actividades(i).ToString Then
                            Dim penBrush As MindFusion.Drawing.Brush = New MindFusion.Drawing.LinearGradientBrush( _
                            Color.Yellow, Color.Yellow, 0)
                            sha_.Brush = penBrush
                            Exit For
                        End If
                    Next
                End If
            Next
            Marca_Actividad_ruta_workflow_recorridas = "YES"
            ref_update.Update()
        Catch ex As Exception
            Marca_Actividad_ruta_workflow_recorridas = "Inconsistencia general función Marca_Actividad_ruta_workflow_recorridas " & ex.Message
        End Try
    End Function
    Function Solicita_id_actividad_tarea_seleccionada_ruta_workflow(ByVal id_ruta As Integer, _
                                                                    ByVal id_tarea_selecionada As Integer, _
                                                                    ByRef id_actividad As Integer _
                                                                           ) As String
        '-----------------------------------------------------------------
        'Funcion : Solicita el id de la actividad 
        'la cual tiene la tarea seleccionada
        'Fecha : 2017-09-29
        '------------------------------------------------------------------
        Try
            id_actividad = 0
            Dim sqlconsulta As String = "Select Id_Actividad from estados_tarea_workflow " & _
                " where Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta=" & id_ruta & _
                " and  fecha_fin is null and Inicio_Tareas_Workflow_Id_Tarea=" & id_tarea_selecionada
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Solicita_id_actividad_tarea_seleccionada_ruta_workflow = "Error función Solicita_id_actividad_tarea_seleccionada_ruta_workflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_actividad_tarea_seleccionada_ruta_workflow = "YES"
                Exit Function
            Else
                id_actividad = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_actividad_tarea_seleccionada_ruta_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_actividad_tarea_seleccionada_ruta_workflow = "Inconsistencia general funcion Solicita_id_actividad_tarea_seleccionada_ruta_workflow " & ex.Message
        End Try

    End Function
    Function Marca_Actividad_ruta_workflow(ByVal id_actividad As Integer, _
                                           ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView, _
                                           ByRef ref_update As UpdatePanel)
        Try
            For Each sha_ As Object In ref_diagram.Diagram.Items
                Dim ob As Object = sha_.GetType
                If ob.Fullname = "MindFusion.Diagramming.ShapeNode" Then
                    If sha_.id = id_actividad.ToString Then
                        Dim penBrush As MindFusion.Drawing.Brush = New MindFusion.Drawing.LinearGradientBrush( _
                        Color.LightSteelBlue, Color.LightSteelBlue, 0)
                        sha_.Brush = penBrush
                        Exit For
                    End If
                End If
            Next
            Marca_Actividad_ruta_workflow = "YES"
        Catch ex As Exception
            Marca_Actividad_ruta_workflow = "Inconsistencia general función Marca_Actividad_ruta_workflow " & ex.Message
        Finally
            ref_update.Update()
        End Try
    End Function
    Function Diagrama_trazabilidad_ruta_trabajo_por_usuario(ByVal id_tarea As Integer, ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView, _
                              ByRef ref_update As UpdatePanel, ByRef ref_CheckBox_Grid_alineamiento As CheckBox) As String
        Try
            Dim ruta_plantilla As String = HttpContext.Current.Server.MapPath("../configuracion/dlc_flujo.vdx")
            If File.Exists(ruta_plantilla) = False Then
                Diagrama_trazabilidad_ruta_trabajo_por_usuario = "Imposible encontrar el archivo " & ruta_plantilla
                Exit Function
            End If
            Dim ob As New MindFusion.Diagramming.Import.VisioImporter
            Dim diagran As New MindFusion.Diagramming.DiagramDocument
            Dim digran_page As New MindFusion.Diagramming.DiagramPage
            diagran = ob.Import(ruta_plantilla)
            If diagran.Pages.Count = 0 Then
                Diagrama_trazabilidad_ruta_trabajo_por_usuario = "La plantilla base no tiene el formato correcto, no contiene por lo menos un diagrama "
                Exit Function
            End If
            ref_diagram.Diagram.Items.Clear()
            ref_diagram.Diagram = diagran.Pages(0)
            ref_diagram.ZoomFactor = 75
            Dim stru_shape_actividad() As stru_shape_actividad = Nothing
            Dim Result As String = ""
            Dim ref_class As New Class_flujo_trabajo_workflow
            Result = Me.Solicita_listado_estructura_usuario_recorridos_ruta(id_tarea, stru_shape_actividad)
            If Result <> "YES" Then
                Diagrama_trazabilidad_ruta_trabajo_por_usuario = Result
                Exit Function
            End If
            If Not stru_shape_actividad Is Nothing Then
                For i As Integer = 0 To stru_shape_actividad.Length - 1
                    Result = ref_class.Agrega_shape_flujo_trabajo_trazabilidad(ref_diagram, ref_update, stru_shape_actividad(i).nombre_tipo_actividad, stru_shape_actividad(i).id_estado, stru_shape_actividad(i).nombre_actividad, 0, i + 1, stru_shape_actividad(i).cargo_usuario)
                    If Result <> "YES" Then
                        Diagrama_trazabilidad_ruta_trabajo_por_usuario = Result
                        Exit Function
                    End If
                Next
                Result = ref_class.Conecta_shape_trazabilidad_flujo_trabajo(ref_diagram, ref_update)
                If Result <> "YES" Then
                    Diagrama_trazabilidad_ruta_trabajo_por_usuario = Result
                    Exit Function
                End If
            End If
            Create_label_diagrama(ref_diagram, ref_update, HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"), _
                                 HttpContext.Current.Session.Item("RU_RADICADO_RUTA_TRABAJO"), id_tarea, 0)
            Diagrama_trazabilidad_ruta_trabajo_por_usuario = "YES"
        Catch ex As Exception
            Diagrama_trazabilidad_ruta_trabajo_por_usuario = "Inconsistencia general función Diagrama_trazabilidad_ruta_trabajo_por_usuario " & ex.Message
        Finally
            ref_update.Update()
        End Try
    End Function
    Function Solicita_listado_estructura_usuario_recorridos_ruta(ByVal id_tarea As Integer, ByRef stru_shape_actividad() As stru_shape_actividad) As String
        Try
            stru_shape_actividad = Nothing
            Dim sqlconsulta As String = "SELECT  etw.id_Estado,uw.Nombre_Usuario,'USUARIOINDIVIDUAL' as colum_virtual, uw.Cargo_Usuario  from estados_tarea_workflow as etw " & _
            "inner join listado_actividades_workflow as law on (etw.Id_Actividad=law.Id_Actividad) " & _
            "inner join usuario_workflow as uw on (etw.Id_usuario=uw.Idu_suario) " & _
            "where inicio_tareas_workflow_id_tarea=" & id_tarea
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Solicita_listado_estructura_usuario_recorridos_ruta = "Error función Solicita_listado_estructura_usuario_recorridos_ruta " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_listado_estructura_usuario_recorridos_ruta = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_shape_actividad(i)
                    stru_shape_actividad(i).id_estado = Datset.Tables(0).Rows(i).Item(0)
                    stru_shape_actividad(i).nombre_actividad = Datset.Tables(0).Rows(i).Item(1)
                    stru_shape_actividad(i).nombre_tipo_actividad = Datset.Tables(0).Rows(i).Item(2)
                    stru_shape_actividad(i).cargo_usuario = Datset.Tables(0).Rows(i).Item(3)
                Next
                Solicita_listado_estructura_usuario_recorridos_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_listado_estructura_usuario_recorridos_ruta = "Iconsistencia general función Solicita_listado_estructura_usuario_recorridos_ruta " & ex.Message
        End Try
    End Function
    Function Diagrama_trazabilidad_ruta_trabajo_por_actividad(ByVal id_tarea As Integer, ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView, _
                               ByRef ref_update As UpdatePanel, ByRef ref_CheckBox_Grid_alineamiento As CheckBox) As String
        Try
            Dim ruta_plantilla As String = HttpContext.Current.Server.MapPath("../configuracion/dlc_flujo.vdx")
            If File.Exists(ruta_plantilla) = False Then
                Diagrama_trazabilidad_ruta_trabajo_por_actividad = "Imposible encontrar el archivo " & ruta_plantilla
                Exit Function
            End If
            Dim ob As New MindFusion.Diagramming.Import.VisioImporter
            Dim diagran As New MindFusion.Diagramming.DiagramDocument
            Dim digran_page As New MindFusion.Diagramming.DiagramPage
            diagran = ob.Import(ruta_plantilla)
            If diagran.Pages.Count = 0 Then
                Diagrama_trazabilidad_ruta_trabajo_por_actividad = "La plantilla base no tiene el formato correcto, no contiene por lo menos un diagrama "
                Exit Function
            End If
            ref_diagram.Diagram.Items.Clear()
            ref_diagram.Diagram = diagran.Pages(0)
            ref_diagram.ZoomFactor = 75
            Dim stru_shape_actividad() As stru_shape_actividad = Nothing
            Dim Result As String = ""
            Dim ref_class As New Class_flujo_trabajo_workflow
            Result = Me.Solicita_listado_estructura_actividades_recorridas_ruta(id_tarea, stru_shape_actividad)
            If Result <> "YES" Then
                Diagrama_trazabilidad_ruta_trabajo_por_actividad = Result
                Exit Function
            End If
            If Not stru_shape_actividad Is Nothing Then
                For i As Integer = 0 To stru_shape_actividad.Length - 1
                    Result = ref_class.Agrega_shape_flujo_trabajo_trazabilidad(ref_diagram, ref_update, _
                                                                               stru_shape_actividad(i).nombre_tipo_actividad, _
                                                                               stru_shape_actividad(i).id_estado, _
                                                                               stru_shape_actividad(i).nombre_actividad, _
                                                                               0, i + 1, stru_shape_actividad(i).cargo_usuario)
                    If Result <> "YES" Then
                        Diagrama_trazabilidad_ruta_trabajo_por_actividad = Result
                        Exit Function
                    End If
                Next
                Result = ref_class.Conecta_shape_trazabilidad_flujo_trabajo(ref_diagram, ref_update)
                If Result <> "YES" Then
                    Diagrama_trazabilidad_ruta_trabajo_por_actividad = Result
                    Exit Function
                End If
            End If
            Create_label_diagrama(ref_diagram, ref_update, HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"), _
                                  HttpContext.Current.Session.Item("RU_RADICADO_RUTA_TRABAJO"), id_tarea, 0)
            Diagrama_trazabilidad_ruta_trabajo_por_actividad = "YES"
        Catch ex As Exception
            Diagrama_trazabilidad_ruta_trabajo_por_actividad = "Inconsistencia general función Diagrama_trazabilidad_ruta_trabajo_por_actividad " & ex.Message
        Finally
            ref_update.Update()
        End Try
    End Function
    Function Solicita_listado_estructura_actividades_recorridas_ruta(ByVal id_tarea As Integer, ByRef stru_shape_actividad() As stru_shape_actividad) As String
        Try
            stru_shape_actividad = Nothing
            Dim sqlconsulta As String = "SELECT  etw.id_Estado,law.Nombre_Actividad,agw.Tipo_Actividad, '' as colum  from estados_tarea_workflow as etw " & _
            "inner join listado_actividades_workflow as law on (etw.Id_Actividad=law.Id_Actividad) " & _
             "inner join Actividades_Generales_Workflow as agw on (agw.Id_Actividad_General=law.Actividades_Generales_Workflow_Id_Actividad_General) " & _
             " where inicio_tareas_workflow_id_tarea =" & id_tarea & " order by etw.id_Estado"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Solicita_listado_estructura_actividades_recorridas_ruta = "Error función Solicita_listado_estructura_actividades_recorridas_ruta " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_listado_estructura_actividades_recorridas_ruta = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_shape_actividad(i)
                    stru_shape_actividad(i).id_estado = Datset.Tables(0).Rows(i).Item(0)
                    stru_shape_actividad(i).nombre_actividad = Datset.Tables(0).Rows(i).Item(1)
                    stru_shape_actividad(i).nombre_tipo_actividad = Datset.Tables(0).Rows(i).Item(2)
                    stru_shape_actividad(i).cargo_usuario = Datset.Tables(0).Rows(i).Item(3)
                Next
                Solicita_listado_estructura_actividades_recorridas_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_listado_estructura_actividades_recorridas_ruta = "Iconsistencia general función Solicita_listado_estructura_actividades_recorridas_ruta " & ex.Message
        End Try
    End Function
    Function Guarda_archivo_base_datos_string(ByVal string_archivo As String, ByVal id_ruta As Integer) As String
        Try
            Dim Vss_Bynary() As Byte = Nothing
            Dim Result As String = ""
            Dim sql_atualiza_ruta As String = "update rutas_workflow set Archivo_Plantilla_Mindifucion = ?imagen  where " _
            & "id_ruta =" & id_ruta
            Dim myConnection As New MySqlConnection
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim sqlresultinsert As Integer = 0
            ref.Returna_Conexion_Mysql(myConnection)
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myCommand.Connection = myConnection
            myCommand.CommandText = sql_atualiza_ruta
            myCommand.Parameters.AddWithValue("?imagen", string_archivo)
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Guarda_archivo_base_datos_string = "Imposible crear una nueva ruta de trabajo  "
                myConnection.Close()
                Exit Function
            End If
            myConnection.Close()
            Guarda_archivo_base_datos_string = "YES"
        Catch ex As Exception
            Guarda_archivo_base_datos_string = "Inconsistencia general función Guarda_archivo_base_datos " & ex.Message
        End Try
    End Function
    Function Salva_archivo_file_sistem(ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView, ByRef ruta_archivo As String, ByVal nombre_ruta As String) As String
        Try
            ruta_archivo = HttpContext.Current.Server.MapPath(HttpContext.Current.Session.Item("WF_RUTA_FIRMA") & HttpContext.Current.Session.Item("Id_Usuario_Workflow") & "\")
            If Directory.Exists(ruta_archivo) = False Then
                Directory.CreateDirectory(ruta_archivo)
            End If
            ruta_archivo = ruta_archivo & nombre_ruta
            ref_diagram.SaveToFile(ruta_archivo, True)
            Salva_archivo_file_sistem = "YES"
        Catch ex As Exception
            Salva_archivo_file_sistem = "Inconsistencia general función Salva_archivo_file_sistem " & ex.Message
        End Try
    End Function
    Function VSS_Bytes(ByVal Path As String) As Byte()
        'Funcion Convierte archivos as formato binario
        Dim sPath As String
        Try
            sPath = Path
            Dim Ruta As New FileStream(sPath, FileMode.Open, FileAccess.Read)
            Dim Binario(CInt(Ruta.Length)) As Byte
            Ruta.Read(Binario, 0, CInt(Ruta.Length))
            Ruta.Close()
            Return Binario
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Function Bytes_VSS(ByVal Bin As Byte(), ByVal Nombre_Ruta As String, ByRef pathTemporal As String) As String
        Try
            Dim oFileStream As FileStream
            pathTemporal = HttpContext.Current.Server.MapPath(HttpContext.Current.Session.Item("WF_RUTA_FIRMA") & HttpContext.Current.Session.Item("Id_Usuario_Workflow") & "\")
            If Directory.Exists(pathTemporal) = False Then
                Directory.CreateDirectory(pathTemporal)
            End If
            pathTemporal = pathTemporal & Nombre_Ruta
            If File.Exists(pathTemporal) Then
                File.Delete(pathTemporal)
            End If
            oFileStream = New FileStream(pathTemporal, FileMode.CreateNew)
            oFileStream.Write(Bin, 0, Bin.Length)
            oFileStream.Close()
            oFileStream = Nothing
            Bytes_VSS = "YES"
        Catch ex As Exception
            Bytes_VSS = ex.Message
        End Try
    End Function
    Function Agrega_shape_ruta_worokflow(ByRef DiagramView As MindFusion.Diagramming.WebForms.DiagramView,
                                         ByRef UpdatePanel_diagran_view As UpdatePanel,
                                         ByVal nombre_tipo_actividad As String,
                                         ByVal identificador As Integer,
                                         ByVal nombre_actividad As String,
                                         ByVal nombre_ruta As String,
                                         ByVal id_ruta As Integer) As String
        '---------------------------------------------------------
        'Función : Agrega un shape a la interface del diagrama      
        'Fecha : 2017-07-27
        'Ing : Miguel Angel Urueta Miranda
        '---------------------------------------------------------
        Try
            Dim sap As New MindFusion.Diagramming.ShapeNode
            'sap.AllowOutgoingLinks = False
            'Dim Stroke = New MindFusion.Drawing.Pen(Color.Black, 0)
            'Dim Fill = New MindFusion.Drawing.SolidBrush(Color.White)
            'sap.PolygonalTextLayout = True
            'sap.EnableStyledText = True
            Dim x As Integer = 0
            Dim y As Integer = 0
            '-------------------------------------------
            'Posiciona el shape en el grid del diagrama
            '-------------------------------------------
            x = DiagramView.Diagram.Bounds.Width / 2
            y = (DiagramView.Diagram.Bounds.Height / 2) - 100
            Dim Rect = New RectangleF(x, y, 25, 25)
            sap.Bounds = Rect
            'sap.Text = UCase(nombre_actividad)
            sap.Id = identificador
            '-----------------------------------
            'Aplica estilo al shape
            '-----------------------------------
            Dim Result As String = ""
            Result = Aplica_estilo_shape_add(sap,
                                             nombre_tipo_actividad,
                                             nombre_actividad)
            If Result <> "YES" Then
                Agrega_shape_ruta_worokflow = "Imposible aplicar el estilo al shape, mensaje " & Result
                Exit Function
            End If
            DiagramView.Diagram.Items.Add(sap)
            Dim Ruta_archivo_guardado As String = ""
            '--------------------------------------------------
            'Guarda el archivo en el sistema de archivo
            '--------------------------------------------------
            'Result = Salva_archivo_file_sistem(DiagramView, Ruta_archivo_guardado, nombre_ruta)
            'If Result <> "YES" Then
            '    DiagramView.Diagram.Items.Remove(sap)
            '    Agrega_shape_ruta_worokflow = Result
            '    Exit Function
            'End If
            Dim string_diagrama As String = DiagramView.SaveToString(SaveToStringFormat.Base64, True)
            '-------------------------------------------------
            'Guarda el archivo en la base de datos
            '-------------------------------------------------
            Result = Guarda_archivo_base_datos_string(string_diagrama, id_ruta)
            If Result <> "YES" Then
                DiagramView.Diagram.Items.Remove(sap)
                Agrega_shape_ruta_worokflow = Result
                Exit Function
            End If
            UpdatePanel_diagran_view.Update()
            Agrega_shape_ruta_worokflow = "YES"
        Catch ex As Exception
            Agrega_shape_ruta_worokflow = "Inconsistencia general función Agrega_shape_ruta_worokflow " & ex.Message
        End Try

    End Function
    Function Aplica_estilo_shape_add(ByRef shape As MindFusion.Diagramming.ShapeNode,
                                     ByVal nombre_tipo_actividad As String,
                                     ByVal nombre_sape As String) As String
        '--------------------------------------------------
        'Función : Aplica estilo del shape
        'Fecha : 2017-06-30
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------------
        Try
            shape.AllowOutgoingLinks = False
            Dim Stroke = New MindFusion.Drawing.Pen(Color.Black, 0)
            Dim Fill = New MindFusion.Drawing.SolidBrush(Color.White)
            shape.PolygonalTextLayout = True
            shape.EnableStyledText = True
            Dim form = New StringAlignment
            shape.TextFormat.Alignment = StringAlignment.Center
            Dim ref_tink As MindFusion.Diagramming.Thickness
            ref_tink.Top = 15
            Dim re_trim = Trim(nombre_sape)
            re_trim = Trim(re_trim)
            re_trim = re_trim.Replace(vbCrLf, "")
            shape.TextPadding = ref_tink
            shape.Text = re_trim
            Dim sysdra = New System.Drawing.Font("Bold", 9)
            shape.Font = sysdra
            Dim penBrush As MindFusion.Drawing.Brush = New MindFusion.Drawing.LinearGradientBrush(
             Color.White, Color.White, 0)
            shape.Brush = penBrush
            shape.ImageAlign = MindFusion.Drawing.ImageAlign.TopCenter
            If nombre_tipo_actividad = "SISTEMA" Then
                shape.ImageUrl = "../workflow/imageneswf/ActividadSistema.png"
            End If
            If nombre_tipo_actividad = "ENLASE" Then
                shape.ImageUrl = "../workflow/imageneswf/Actividad_Enlace_Digitalizacion_dos.png"
            End If
            If nombre_tipo_actividad = "USUARIO" Then
                shape.ImageUrl = "../workflow/imageneswf/Actividad_grupo_usuario.png"
            End If
            If nombre_tipo_actividad = "USUARIORESPONSABLE" Then
                shape.ImageUrl = "../workflow/imageneswf/Actividad_grupo_usuario_responsable.png"
            End If
            If nombre_tipo_actividad = "USUARIORESPONSABLERADICADOR" Then
                shape.ImageUrl = "../workflow/imageneswf/Actividad_grupo_usuario_responsable_radicador.png"
            End If
            If nombre_tipo_actividad = "USUARIOINDIVIDUAL" Then
                shape.ImageUrl = "../workflow/imageneswf/actividad_usuario.png"
            End If
            Aplica_estilo_shape_add = "YES"
        Catch ex As Exception
            Aplica_estilo_shape_add = "Inconsistencia general función Aplica_estilo_shape " & ex.Message
        End Try
    End Function
    Function Aplica_estilo_shape_add_trazabilidad(ByRef shape As MindFusion.Diagramming.ShapeNode, _
                                                  ByVal nombre_tipo_actividad As String, _
                                                  ByVal nombre_sape As String, _
                                                  ByVal cargo As String) As String
        '--------------------------------------------------
        'Función : Aplica estilo del shape
        'Fecha : 2017-06-30
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------------
        Try
            shape.AllowOutgoingLinks = False
            Dim Stroke = New MindFusion.Drawing.Pen(Color.Black, 0)
            Dim Fill = New MindFusion.Drawing.SolidBrush(Color.White)
            shape.PolygonalTextLayout = True
            shape.EnableStyledText = True
            Dim form = New StringAlignment
            shape.TextFormat.Alignment = StringAlignment.Center
            Dim ref_tink As MindFusion.Diagramming.Thickness
            ref_tink.Top = 15
            Dim re_trim = Trim(nombre_sape)
            re_trim = Trim(re_trim)
            re_trim = re_trim.Replace(vbCrLf, "")
            shape.TextPadding = ref_tink
            If nombre_tipo_actividad = "USUARIOINDIVIDUAL" Then
                shape.Text = re_trim & "(" & cargo & ")"
            Else
                shape.Text = re_trim
            End If
            Dim sysdra = New System.Drawing.Font("Bold", 9)
            shape.Font = sysdra
            Dim penBrush As MindFusion.Drawing.Brush = New MindFusion.Drawing.LinearGradientBrush( _
             Color.White, Color.White, 0)
            shape.Brush = penBrush
            shape.ImageAlign = MindFusion.Drawing.ImageAlign.TopCenter
            If nombre_tipo_actividad = "SISTEMA" Then
                shape.ImageUrl = "../workflow/imageneswf/ActividadSistema.png"
            End If
            If nombre_tipo_actividad = "ENLASE" Then
                shape.ImageUrl = "../workflow/imageneswf/Actividad_Enlace_Digitalizacion_dos.png"
            End If
            If nombre_tipo_actividad = "USUARIO" Then
                shape.ImageUrl = "../workflow/imageneswf/Actividad_grupo_usuario.png"
            End If
            If nombre_tipo_actividad = "USUARIOINDIVIDUAL" Then
                shape.ImageUrl = "../workflow/imageneswf/actividad_usuario.png"
            End If
            Aplica_estilo_shape_add_trazabilidad = "YES"
        Catch ex As Exception
            Aplica_estilo_shape_add_trazabilidad = "Inconsistencia general función  Aplica_estilo_shape_add_trazabilidad " & ex.Message
        End Try
    End Function
    Function Aplica_estilo_shape(ByRef shape As MindFusion.Diagramming.ShapeNode, ByVal nombre_tipo_actividad As String) As String
        '--------------------------------------------------
        'Función : Aplica estilo del shape
        'Fecha : 2017-06-30
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------------
        Try
            shape.AllowOutgoingLinks = False
            Dim Stroke = New MindFusion.Drawing.Pen(Color.Black, 0)
            Dim Fill = New MindFusion.Drawing.SolidBrush(Color.White)
            shape.PolygonalTextLayout = True
            shape.EnableStyledText = True
            Dim form = New StringAlignment
            shape.TextFormat.Alignment = StringAlignment.Center
            Dim ref_tink As MindFusion.Diagramming.Thickness
            ref_tink.Top = 10
            Dim re_trim = Trim(shape.Text)
            re_trim = Trim(re_trim)
            re_trim = re_trim.Replace(vbCrLf, "")
            shape.Text = re_trim
            shape.TextPadding = ref_tink
            Dim sysdra = New System.Drawing.Font("Bold", 9)
            shape.Font = sysdra
            Dim penBrush As MindFusion.Drawing.Brush = New MindFusion.Drawing.LinearGradientBrush( _
             Color.White, Color.White, 0)
            shape.Brush = penBrush
            shape.ImageAlign = MindFusion.Drawing.ImageAlign.TopCenter
            If nombre_tipo_actividad = "SISTEMA" Then
                shape.ImageUrl = "../workflow/imageneswf/ActividadSistema.png"
            End If
            If nombre_tipo_actividad = "ENLASE" Then
                shape.ImageUrl = "../workflow/imageneswf/Actividad_Enlace_Digitalizacion_dos.png"
            End If
            If nombre_tipo_actividad = "USUARIO" Then
                shape.ImageUrl = "../workflow/imageneswf/Actividad_grupo_usuario.png"
            End If
            If nombre_tipo_actividad = "USUARIOINDIVIDUAL" Then
                shape.ImageUrl = "../workflow/imageneswf/actividad_usuario.png"
            End If
            Aplica_estilo_shape = "YES"
        Catch ex As Exception
            Aplica_estilo_shape = "Inconsistencia general función Aplica_estilo_shape " & ex.Message
        End Try
    End Function
    Function Aplica_estilo_shape_marcado(ByRef shape As MindFusion.Diagramming.ShapeNode) As String
        '--------------------------------------------------
        'Función : Aplica estilo del shape
        'Fecha : 2017-06-30
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------------
        Try
            shape.AllowOutgoingLinks = False
            Dim Stroke = New MindFusion.Drawing.Pen(Color.Black, 0)
            Dim Fill = New MindFusion.Drawing.SolidBrush(Color.White)
            shape.PolygonalTextLayout = True
            shape.EnableStyledText = True
            Dim form = New StringAlignment
            shape.TextFormat.Alignment = StringAlignment.Center
            Dim ref_tink As MindFusion.Diagramming.Thickness
            ref_tink.Top = 10
            Dim re_trim = Trim(shape.Text)
            re_trim = Trim(re_trim)
            re_trim = re_trim.Replace(vbCrLf, "")
            shape.Text = re_trim
            shape.TextPadding = ref_tink
            Dim sysdra = New System.Drawing.Font("Bold", 9)
            shape.Font = sysdra
            Dim penBrush As MindFusion.Drawing.Brush = New MindFusion.Drawing.LinearGradientBrush( _
             Color.Blue, Color.Blue, 0)
            shape.Brush = penBrush
            shape.ImageAlign = MindFusion.Drawing.ImageAlign.TopCenter
            Aplica_estilo_shape_marcado = "YES"
        Catch ex As Exception
            Aplica_estilo_shape_marcado = "Inconsistencia general función Aplica_estilo_shape_marcado " & ex.Message
        End Try
    End Function
    Function Aplica_zon_factor_diagranview(ByRef ref_diagran_view As MindFusion.Diagramming.WebForms.DiagramView, ByVal zon As Object, _
                                           ByRef ref_update_panel As UpdatePanel) As String
        Try
            ref_diagran_view.ZoomFactor = zon
            ref_update_panel.Update()
            Aplica_zon_factor_diagranview = "YES"
        Catch ex As Exception
            Aplica_zon_factor_diagranview = "Inconsistencia general función Aplica_zon_factor_diagranview " & ex.Message
        End Try
    End Function
    Function lista_zon_interface(ByRef ref_droplist As DropDownList, _
                                           ByRef ref_update_panel As UpdatePanel) As String
        Try
            ref_droplist.Items.Clear()
            For i As Integer = 5 To 100 Step 5
                ref_droplist.Items.Add(i)
            Next
            ref_droplist.Text = "75"
            ref_update_panel.Update()
            lista_zon_interface = "YES"
        Catch ex As Exception
            lista_zon_interface = "Inconsistencia general función lista_zon_interface " & ex.Message
        End Try
    End Function
    Function Retorna_Id_Actividad_por_id_ruta(ByVal Nombre_Actividad As String, _
                                              ByVal id_ruta As Integer, _
                                              ByRef id_actividad As Integer) As String
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT Id_Actividad FROM LISTADO_ACTIVIDADES_WORKFLOW " & _
            " WHERE Nombre_Actividad='" & Trim(Nombre_Actividad) & "' and Rutas_Workflow_id_Ruta=" & id_ruta
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("LISTADO_ACTIVIDADES_WORKFLOW")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                id_actividad = 0
                Retorna_Id_Actividad_por_id_ruta = "Función Retorna_Id_Actividad_por_id_ruta dice " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_Id_Actividad_por_id_ruta = "Imposible encontrar el id de la actividad por el nombre " & Nombre_Actividad & " y por id " & id_actividad
                Exit Function
            Else
                id_actividad = Datset.Tables(0).Rows(0).Item(0)
                Retorna_Id_Actividad_por_id_ruta = "YES"
            End If

        Catch ex As Exception
            Retorna_Id_Actividad_por_id_ruta = "Inconsistencia general función Retorna_Id_Actividad_por_id_ruta " & ex.Message
        End Try
    End Function
    Function Retorna_id_ruta_por_id_actividad(ByVal id_actividad As Integer, _
                                              ByRef id_ruta As Integer) As String
        '---------------------------------------------------------------
        'Función : Retorna el id de la ruta relacionado a la actividad
        'informada
        'Fecha : 2017-12-04
        'Ing : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT Rutas_Workflow_id_Ruta FROM LISTADO_ACTIVIDADES_WORKFLOW " & _
            " WHERE Id_Actividad=" & id_actividad
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("LISTADO_ACTIVIDADES_WORKFLOW")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                id_actividad = 0
                Retorna_id_ruta_por_id_actividad = "Función Retorna_id_ruta_por_id_actividad dice " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_ruta_por_id_actividad = "Imposible encontrar el id de ruta relacionada a la actividad " & id_actividad
                Exit Function
            Else
                id_ruta = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_ruta_por_id_actividad = "YES"
            End If

        Catch ex As Exception
            Retorna_id_ruta_por_id_actividad = "Inconsistencia general función Retorna_id_ruta_por_id_actividad " & ex.Message
        End Try
    End Function
    Function Retorna_tipo_actividad(ByVal id_actividad As Integer, ByRef nombre_tipo_actividad As String) As String
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT Tipo_Actividad FROM LISTADO_ACTIVIDADES_WORKFLOW as law " & _
                " INNER JOIN actividades_generales_workflow as agw on (agw.Id_Actividad_General=law.Actividades_Generales_Workflow_Id_Actividad_General)" & _
            " WHERE Id_Actividad=" & id_actividad
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("LISTADO_ACTIVIDADES_WORKFLOW")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                id_actividad = 0
                Retorna_tipo_actividad = "Función Retorna_tipo_actividad dice " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_tipo_actividad = "Imposible encontrar nombre del tipo de actividad del id " & id_actividad
                Exit Function
            Else
                nombre_tipo_actividad = Datset.Tables(0).Rows(0).Item(0)
                Retorna_tipo_actividad = "YES"
            End If

        Catch ex As Exception
            Retorna_tipo_actividad = "Inconsistencia general función Retorna_tipo_actividad " & ex.Message
        End Try
    End Function
    Function Solicita_id_conector_identificacion_grafica(ByVal identificacion_grafica As Integer, ByVal id_ruta As Integer, _
                                                         ByRef identificacion_conector As Integer) As String
        '-------------------------------------------------------------
        'Función : Solicita el numero de identificación del conector
        'con la identificacion grafica y la ruta
        'Fecha : 2017-06-30
        'Ing : Miguel Angel Urueta Miranda
        '-------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT id_actividades_disponibles_envio FROM actividades_disponibles_envio  " & _
            " WHERE Ienti_Grafica_Actividad=" & identificacion_grafica & " and id_Ruta=" & id_ruta
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("LISTADO_ACTIVIDADES_WORKFLOW")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                identificacion_conector = 0
                Solicita_id_conector_identificacion_grafica = "Función Solicita_id_conector_identificacion_grafica dice " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_conector_identificacion_grafica = "Imposible encontrar el id del conector " & identificacion_grafica
                Exit Function
            Else
                identificacion_conector = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_conector_identificacion_grafica = "YES"
            End If
        Catch ex As Exception
            Solicita_id_conector_identificacion_grafica = "Inconsistencia general función Solicita_id_conector_identificacion_grafica " & ex.Message
        End Try
    End Function
    Function Aplica_estilo_conector(ByRef link As MindFusion.Diagramming.DiagramLink) As String
        Try
            link.AutoRoute = False
            link.AutoSnapToNode = False
            'EVITA QUE SE MUEVA EL CONECTOR FINAL
            link.AllowMoveEnd = False
            link.DrawCrossings = False
            link.Text = ""
            link.CrossingRadius = 1
            link.DrawCrossings = True
            Dim Fill = New MindFusion.Drawing.SolidBrush(Color.Yellow)
            Dim penn = New MindFusion.Drawing.Pen(Color.Black)
            link.Pen = penn
            link.HeadBrush = Fill
            Aplica_estilo_conector = "YES"
        Catch ex As Exception
            Aplica_estilo_conector = "Inconsistencia general función Aplica_estilo_conector " & ex.Message
        End Try
    End Function
    Function Seleccion_menu_pricipal(ByVal valor_seleccion As String, _
                                     ByRef pag As Page) As String
        Dim Result As String = ""
        '----------------------------------------
        'Importa ruta plataforma antigua
        '----------------------------------------
        If valor_seleccion = "I-R-WF" Then
            Dim ref_DropDownList_rutas_disponibles As DropDownList = pag.FindControl("DropDownList_rutas_disponibles")
            Dim ref_UpdatePanel_importa_ruta As UpdatePanel = pag.FindControl("UpdatePanel_importa_ruta")
            Dim ref_ModalPopupExtender_edition_importa_ruta As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_edition_importa_ruta")
            Dim ref_TextBox_archivo_import As TextBox = pag.FindControl("TextBox_archivo_import")
            Dim ref_UpdatePanel_adunta_archivo_ruta As UpdatePanel = pag.FindControl("UpdatePanel_adunta_archivo_ruta")
            Dim rutas() As String = Nothing
            Result = Me.Solicita_nombres_rutas_workflow_sin_importacion(rutas)
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
            Result = Me.Lista_rutas_interface_importacion(rutas, ref_DropDownList_rutas_disponibles, ref_UpdatePanel_importa_ruta)
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
            ref_TextBox_archivo_import.Text = ""
            ref_UpdatePanel_adunta_archivo_ruta.Update()
            ref_ModalPopupExtender_edition_importa_ruta.Show()
        End If
        '------------------------------------------
        'Lista rutas disponibles
        '------------------------------------------
        If valor_seleccion = "A-RD-WF" Then
            Dim ref_DropDownListrutasdisponibles As DropDownList = pag.FindControl("DropDownListrutasdisponibles")
            Dim ref_UpdatePanel_abrir_rutas_disponibles As UpdatePanel = pag.FindControl("UpdatePanel_abrir_rutas_disponibles")
            Dim ref_ModalPopupExtender_edition_abrir_rutas_disponibles As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_edition_abrir_rutas_disponibles")
            Dim rutas() As String = Nothing
            Result = Me.Solicita_nombres_rutas_workflow(rutas)
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
            Result = Me.Lista_rutas_interface_importacion(rutas, ref_DropDownListrutasdisponibles, ref_UpdatePanel_abrir_rutas_disponibles)
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
            ref_ModalPopupExtender_edition_abrir_rutas_disponibles.Show()

        End If
        '-------------------------------------------
        'Administración campos de la ruta
        '-------------------------------------------
        If valor_seleccion = "S-GAU" Then
            Dim ref_ModalPopupExtender_edition_paginas_externas_popou As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_edition_paginas_externas_popou")
            If ref_ModalPopupExtender_edition_paginas_externas_popou Is Nothing Then
                Seleccion_menu_pricipal = "Imposible encontrar el control ( ModalPopupExtender_edition_paginas_externas_popou )"
                Exit Function
            End If
            Dim ref_UpdatePanel_paginas_externas_popou As UpdatePanel = pag.FindControl("UpdatePanel_paginas_externas_popou")
            If ref_UpdatePanel_paginas_externas_popou Is Nothing Then
                Seleccion_menu_pricipal = "Imposible encontrar el control ( UpdatePanel_paginas_externas_popou )"
                Exit Function
            End If
            Dim ref_Iframe_paginas_externas_popup As Object = pag.FindControl("Iframe_paginas_externas_popup__")
            If ref_Iframe_paginas_externas_popup Is Nothing Then
                Seleccion_menu_pricipal = "Imposible encontrar el control ( Iframe_paginas_externas_popup_ )"
                Exit Function
            End If
            ref_Iframe_paginas_externas_popup.Attributes.Add("src", "../workflow/WebFormWorkflowEditarCamposRuta.aspx")
            ref_UpdatePanel_paginas_externas_popou.Update()
            ref_ModalPopupExtender_edition_paginas_externas_popou.Show()
        End If
        '------------------------------------------------------------------
        'Administración gabinetes agregar un gabinete a la configuración
        '------------------------------------------------------------------
        If valor_seleccion = "A-GAW" Then
            Dim ref_DropDownList_Nombre_Gabinete_Agrega As DropDownList = pag.FindControl("DropDownList_Nombre_Gabinete_Agrega")
            Dim ref_ModalPopupExtender_agrega_nuevo_gabinete As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_agrega_nuevo_gabinete")
            Dim ref_UpdatePanel_nombre_gabinete_agrega As UpdatePanel = pag.FindControl("UpdatePanel_nombre_gabinete_agrega")
            Dim ref_TextBox_ruta_fisica_gab_agrega As TextBox = pag.FindControl("TextBox_ruta_fisica_gab_agrega")
            Dim ref_TextBox_ruta_almacena_gab_agrega As TextBox = pag.FindControl("TextBox_ruta_almacena_gab_agrega")
            Dim ref_TextBox_ruta_busqueda_gab_agrega As TextBox = pag.FindControl("TextBox_ruta_busqueda_gab_agrega")
            Dim ref_DropDownList_base_datos_gabinete_agrega As DropDownList = pag.FindControl("DropDownList_base_datos_gabinete_agrega")
            Dim ref_DropDownList_dbms_gabinete_agrega As DropDownList = pag.FindControl("DropDownList_dbms_gabinete_agrega")
            Dim ref_TextBox_unc_gabinete_agrega As TextBox = pag.FindControl("TextBox_unc_gabinete_agrega")
            Dim ref_TextBox_usuario_db_gabinete_agrega As TextBox = pag.FindControl("TextBox_usuario_db_gabinete_agrega")
            Dim ref_TextBox_pasword_db_gabinete_agrega As TextBox = pag.FindControl("TextBox_pasword_db_gabinete_agrega")
            Dim ref_UpdatePanel_parametros_gabinete_agrega As UpdatePanel = pag.FindControl("UpdatePanel_parametros_gabinete_agrega")
            Dim ref_clas_workflow As New Class_worflow_rutas
            Result = ref_clas_workflow.Inicializa_datos_configuracion_interface(ref_TextBox_ruta_fisica_gab_agrega.Text, ref_TextBox_ruta_busqueda_gab_agrega.Text, _
                ref_TextBox_ruta_almacena_gab_agrega.Text, ref_DropDownList_base_datos_gabinete_agrega, ref_DropDownList_dbms_gabinete_agrega, _
                 ref_TextBox_unc_gabinete_agrega.Text, ref_TextBox_usuario_db_gabinete_agrega.Text, ref_TextBox_pasword_db_gabinete_agrega.Text, _
                 ref_UpdatePanel_parametros_gabinete_agrega)
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
            Dim ref_clas_gabinete As New ClassDaGabinete
            Dim Class_system1 As New Class_system1
            Dim matri_gabinetes() As String = Nothing
            'Retorna_gabinetes_disponibles
            Result = Class_system1.Retorna_gabinetes_disponibles(matri_gabinetes)
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function

            End If
            Result = ref_clas_gabinete.Asigna_gabinetes_disponibles_interface_droplist(matri_gabinetes, ref_DropDownList_Nombre_Gabinete_Agrega, ref_UpdatePanel_nombre_gabinete_agrega)
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            Else
                ref_ModalPopupExtender_agrega_nuevo_gabinete.Show()
            End If
        End If
        '--------------------------------------------------------------------
        'Administración de gabinetes editar gabinetes disponibles
        '--------------------------------------------------------------------
        If valor_seleccion = "A-EDGW" Then
            Dim ref_DropDownList_Nombre_Gabinete_edita As DropDownList = pag.FindControl("DropDownList_Nombre_Gabinete_edita")
            Dim ref_ModalPopupExtender_edita_configuracion_gabinete As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_edita_configuracion_gabinete")
            Dim ref_UpdatePanel_nombre_gabinete_edita As UpdatePanel = pag.FindControl("UpdatePanel_nombre_gabinete_edita")
            Dim ref_TextBox_ruta_fisica_gab_edita As TextBox = pag.FindControl("TextBox_ruta_fisica_gab_edita")
            Dim ref_TextBox_ruta_almacena_gab_edita As TextBox = pag.FindControl("TextBox_ruta_almacena_gab_edita")
            Dim ref_TextBox_ruta_busqueda_gab_edita As TextBox = pag.FindControl("TextBox_ruta_busqueda_gab_edita")
            Dim ref_DropDownList_base_datos_gabinete_edita As DropDownList = pag.FindControl("DropDownList_base_datos_gabinete_edita")
            Dim ref_DropDownList_dbms_gabinete_edita As DropDownList = pag.FindControl("DropDownList_dbms_gabinete_edita")
            Dim ref_TextBox_unc_gabinete_edita As TextBox = pag.FindControl("TextBox_unc_gabinete_edita")
            Dim ref_TextBox_usuario_db_gabinete_edita As TextBox = pag.FindControl("TextBox_usuario_db_gabinete_edita")
            Dim ref_TextBox_pasword_db_gabinete_edita As TextBox = pag.FindControl("TextBox_pasword_db_gabinete_edita")
            Dim ref_UpdatePanel_parametros_gabinete_edita As UpdatePanel = pag.FindControl("UpdatePanel_parametros_gabinete_edita")
            Dim ref_clas_gabinete As New Class_worflow_rutas
            Result = ref_clas_gabinete.Inicializa_datos_configuracion_interface(ref_TextBox_ruta_fisica_gab_edita.Text, ref_TextBox_ruta_busqueda_gab_edita.Text, _
                ref_TextBox_ruta_almacena_gab_edita.Text, ref_DropDownList_base_datos_gabinete_edita, ref_DropDownList_dbms_gabinete_edita, _
                 ref_TextBox_unc_gabinete_edita.Text, ref_TextBox_usuario_db_gabinete_edita.Text, ref_TextBox_pasword_db_gabinete_edita.Text, _
                 ref_UpdatePanel_parametros_gabinete_edita)
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
            Dim matri_gabinetes() As String = Nothing
            Result = ref_clas_gabinete.Retorna_gabinetes_disponibles_configuracion(matri_gabinetes)
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
            Result = ref_clas_gabinete.Asigna_gabinetes_disponibles_interface_configuracion_droplist(matri_gabinetes, ref_DropDownList_Nombre_Gabinete_edita, ref_UpdatePanel_nombre_gabinete_edita)
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            Else
                ref_ModalPopupExtender_edita_configuracion_gabinete.Show()
            End If
        End If
        '---------------------------------------------------------------------
        'Administración de eventos web de cada actividad
        '---------------------------------------------------------------------
        'INICIO-WEB
        If valor_seleccion = "M-INICIO-WEB" Then
            Result = Abrir_menu_eventos("INICIO-WEB", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "WEB")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'PREINICIO-WEB
        If valor_seleccion = "M-PREINICIO-WEB" Then
            Result = Abrir_menu_eventos("PREINICIO-WEB", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "WEB")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'TOMARTAREA-WEB
        If valor_seleccion = "M-TOMARTAREA-WEB" Then
            Result = Abrir_menu_eventos("TOMARTAREA-WEB", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "WEB")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'PRETERMINARACTIVIAD-WEB
        If valor_seleccion = "M-PRETERMINARACTIVIDAD-WEB" Then
            Result = Abrir_menu_eventos("PRETERMINARACTIVIAD-WEB", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "WEB")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'TERMINARACTIVIDAD-WEB
        If valor_seleccion = "M-TERMINARACTIVIDAD-WEB" Then
            Result = Abrir_menu_eventos("TERMINARACTIVIDAD-WEB", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "WEB")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'PENDIENTE-WEB
        If valor_seleccion = "M-PENDIENTE-WEB" Then
            Result = Abrir_menu_eventos("PENDIENTE-WEB", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "WEB")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'ADJUNTOS-WEB
        If valor_seleccion = "M-ADJUNTOS-WEB" Then
            Result = Abrir_menu_eventos("ADJUNTOS-WEB", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "WEB")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'ADJUNTARIMAGENES-WEB
        If valor_seleccion = "M-ADJUNTARIMAGENES-WEB" Then
            Result = Abrir_menu_eventos("ADJUNTARIMAGENES-WEB", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "WEB")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'CREARIMAGENES-WEB
        If valor_seleccion = "M-CREARIMAGENES-WEB" Then
            Result = Abrir_menu_eventos("CREARIMAGENES-WEB", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "WEB")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'DEFAULTSCRIPT-WEB
        If valor_seleccion = "M-DEFAULTESCRIPT-WEB" Then
            Result = Abrir_menu_eventos("DEFAULTSCRIPT-WEB", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "WEB")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'M-ENLACE-WB
        If valor_seleccion = "M-ENLACE-WEB" Then
            Result = Abrir_menu_eventos("ENLASE-WEB", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "WEB")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'M-PREACTUALIZAR-WEB
        If valor_seleccion = "M-PREACTUALIZAR-WEB" Then
            Result = Abrir_menu_eventos("PREACTUALIZAR", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "WEB")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'M-ACTUALIZAR-WEB
        If valor_seleccion = "M-ACTUALIZAR-WEB" Then
            Result = Abrir_menu_eventos("ACTUALIZAR", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "WEB")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'M-FINALIZAR-WEB
        If valor_seleccion = "M-FINALIZAR-WEB" Then
            Result = Abrir_menu_eventos("FINALIZAR", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "WEB")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'M-ADJUNTARIMAGENES_SISTEMA-WE
        If valor_seleccion = "M-ADJUNTARIMAGENES_SISTEMA-WEB" Then
            Result = Abrir_menu_eventos("ADJUNTARIMAGENES_SISTEMA", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "WEB")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'M-CREARIMAGENES_SISTEMA-WE
        If valor_seleccion = "M-CREARIMAGENES_SISTEMA-WEB" Then
            Result = Abrir_menu_eventos("CREARIMAGENES_SISTEMA", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "WEB")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'M-DEFAULTSCRIPT_SISTEMA-WE
        If valor_seleccion = "M-DEFAULTSCRIPT_SISTEMA-WEB" Then
            Result = Abrir_menu_eventos("DEFAULTSCRIPT_SISTEMA", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "WEB")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        '---------------------------------------------------------------------
        'Administración de eventos ESCRITORIO de cada actividad
        '---------------------------------------------------------------------
        'INICIO-ESCRITORIO
        If valor_seleccion = "M-INICIO-WE" Then
            Result = Abrir_menu_eventos("INICIO", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "ESCRITORIO")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'PREINICIO-ESCRITORIO
        If valor_seleccion = "M-PREINICIO-WE" Then
            Result = Abrir_menu_eventos("PREINICIO", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "ESCRITORIO")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'TOMARTAREA-ESCRITORIO
        If valor_seleccion = "M-TOMARTAREA-WE" Then
            Result = Abrir_menu_eventos("TOMARTAREA", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "ESCRITORIO")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'PRETERMINARACTIVIAD-ESCRITORIO
        If valor_seleccion = "M-PRETERMINARACTIVIDAD-WE" Then
            Result = Abrir_menu_eventos("PRETERMINARACTIVIAD", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "ESCRITORIO")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'TERMINARACTIVIDAD-ESCRITORIO
        If valor_seleccion = "M-TERMINARACTIVIDAD-WE" Then
            Result = Abrir_menu_eventos("TERMINARACTIVIDAD", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "ESCRITORIO")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'PENDIENTE-ESCRITORIO
        If valor_seleccion = "M-PENDIENTE-WE" Then
            Result = Abrir_menu_eventos("PENDIENTE", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "ESCRITORIO")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'ADJUNTOS-ESCRITORIO
        If valor_seleccion = "M-ADJUNTOS-WE" Then
            Result = Abrir_menu_eventos("ADJUNTOS", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "ESCRITORIO")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'ADJUNTARIMAGENES-ESCRITORIO
        If valor_seleccion = "M-ADJUNTARIMAGENES-WE" Then
            Result = Abrir_menu_eventos("ADJUNTARIMAGENES", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "ESCRITORIO")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'CREARIMAGENES-ESCRITORIO
        If valor_seleccion = "M-CREARIMAGENES-WE" Then
            Result = Abrir_menu_eventos("CREARIMAGENES", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "ESCRITORIO")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'DEFAULTSCRIPT-ESCRITORIO
        If valor_seleccion = "M-DEFAULTESCRIPT-WE" Then
            Result = Abrir_menu_eventos("DEFAULTSCRIPT", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "ESCRITORIO")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'M-ENLACE-WE
        If valor_seleccion = "M-ENLACE-WE" Then
            Result = Abrir_menu_eventos("ENLASE", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "ESCRITORIO")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'M-PREACTUALIZAR-ESCRITORIO
        If valor_seleccion = "M-PREACTUALIZAR-WE" Then
            Result = Abrir_menu_eventos("PREACTUALIZAR", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "ESCRITORIO")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'M-ACTUALIZAR-ESCRITORIO
        If valor_seleccion = "M-ACTUALIZAR-WE" Then
            Result = Abrir_menu_eventos("ACTUALIZAR", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "ESCRITORIO")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'M-FINALIZAR-ESCRITORIO
        If valor_seleccion = "M-FINALIZAR-WE" Then
            Result = Abrir_menu_eventos("FINALIZAR", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "ESCRITORIO")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'M-ADJUNTARIMAGENES_SISTEMA-WE
        If valor_seleccion = "M-ADJUNTARIMAGENES_SISTEMA-WE" Then
            Result = Abrir_menu_eventos("ADJUNTARIMAGENES_SISTEMA", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "ESCRITORIO")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'M-CREARIMAGENES_SISTEMA-WE
        If valor_seleccion = "M-CREARIMAGENES_SISTEMA-WE" Then
            Result = Abrir_menu_eventos("CREARIMAGENES_SISTEMA", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "ESCRITORIO")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'M-DEFAULTSCRIPT_SISTEMA-WE
        If valor_seleccion = "M-DEFAULTSCRIPT_SISTEMA-WE" Then
            Result = Abrir_menu_eventos("DEFAULTSCRIPT_SISTEMA", HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), pag, "ESCRITORIO")
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        'RELACIONAR ACTIVIDAD GRUPO WORKFLOW
        If valor_seleccion = "R-GRUPO-WB" Then
            Dim diagramView As MindFusion.Diagramming.WebForms.DiagramView = pag.FindControl("diagramView")
            If diagramView.Diagram.Selection.Items.Count = 0 Then
                Seleccion_menu_pricipal = "Debe seleccionar la actividad"
                Exit Function
            End If
            If diagramView.Diagram.Selection.Items.Count > 1 Then
                Seleccion_menu_pricipal = "Por favor seleccione una sola actividad"
                Exit Function
            End If
            Dim obshape As Object = diagramView.Diagram.Selection.Items(0)
            Dim ob As Object = obshape.GetType
            If ob.Fullname = "MindFusion.Diagramming.ShapeNode" Then

            Else
                Seleccion_menu_pricipal = "Debe seleccionar un elemento de actividad"
                Exit Function
            End If
            Dim ref_DropDownList_grupos_disponibles_asignacion As DropDownList = pag.FindControl("DropDownList_grupos_disponibles_asignacion")
            Dim ref_UpdatePanel_grupos_disponibles_asignacion As UpdatePanel = pag.FindControl("UpdatePanel_grupos_disponibles_asignacion")
            Dim ref_ModalPopupExtender_edition_grupos_disponibles_asignacion As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_edition_grupos_disponibles_asignacion")
            If HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA") = "" Then
                Seleccion_menu_pricipal = "Debe seleccionar la ruta "
                Exit Function
            End If
            Dim id_ruta As Integer = 0
            Dim Refclas As New ClassWorkflow
            Dim Ref_class_wf_ruta As New Class_worflow_rutas
            Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), _
                                                                id_ruta)
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
            Result = Me.Retorna_grupos_disponibles_worokflow_actividad(id_ruta, _
                                                                       ref_DropDownList_grupos_disponibles_asignacion, _
                                                                      ref_UpdatePanel_grupos_disponibles_asignacion)
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            Else
                ref_ModalPopupExtender_edition_grupos_disponibles_asignacion.Show()
            End If
        End If
        'LISTA GRUPOS WORKFLOW RELACIONADO A LA ACTIVIDAD WORKFLOW
        If valor_seleccion = "L-GRUPO-WB" Then
            Dim ref_DropDownList_lista_grupo_workflow_relacion As DropDownList = pag.FindControl("DropDownList_lista_grupo_workflow_relacion")
            Dim ref_UpdatePanel_lista_grupo_workflow_relacion As UpdatePanel = pag.FindControl("UpdatePanel_lista_grupo_workflow_relacion")
            Dim ref_ModalPopupExtender_edition_lista_grupo_workflow_relacion As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_edition_lista_grupo_workflow_relacion")
            If HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA") = "" Then
                Seleccion_menu_pricipal = "Debe seleccionar la ruta "
                Exit Function
            End If
            Dim id_actividad As Integer = 0
            Result = Retorna_id_actividad_interface(pag, id_actividad)
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
            Dim id_ruta As Integer = 0
            Dim Refclas As New ClassWorkflow
            Dim Ref_class_wf_ruta As New Class_worflow_rutas
            Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), _
                                                                id_ruta)
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
            Retorna_Grupos_Workflow_relacionados_actividad(pag, _
                                                           id_ruta, _
                                                           id_actividad, _
                                                           ref_DropDownList_lista_grupo_workflow_relacion, _
                                                           ref_UpdatePanel_lista_grupo_workflow_relacion)
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
            ref_ModalPopupExtender_edition_lista_grupo_workflow_relacion.Show()
        End If

        If valor_seleccion = "A-EXP-WF" Then
            If HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA") = "" Then
                Seleccion_menu_pricipal = "Debe seleccionar la ruta "
                Exit Function
            End If
            Dim ref_ifmExcel As Object = pag.FindControl("ifmExcel_")
            Dim ref_Hidden_ruta_archivo As Object = pag.FindControl("Hidden_ruta_archivo")
            Dim ref_updatapanel_iframe As Object = pag.FindControl("updatapanel_iframe")
            Dim diagramView As MindFusion.Diagramming.WebForms.DiagramView = pag.FindControl("diagramView")
            Result = Me.Exporta_pdf_mindifucion(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_RUTA_DESCARGA"), _
                                                diagramView, _
                                                ref_ifmExcel, _
                                                ref_Hidden_ruta_archivo, _
                                                ref_updatapanel_iframe)
            If Result <> "YES" Then
                Seleccion_menu_pricipal = Result
                Exit Function
            End If
        End If
        '---------------------------------
        'Configuración conector ruta
        '---------------------------------
        If valor_seleccion = "C-CONECTOR-WB" Then
            Dim estado_envio_correo_electronico As Integer = 0
            HttpContext.Current.Session.Item("DR_RUTASELECCION_ID_CONECTOR") = 0
            Dim ref_Class_actividades_disponibles_envio As New Class_actividades_disponibles_envio
            If HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA") = "" Then
                Seleccion_menu_pricipal = "Debe seleccionar la ruta "
                Exit Function
            End If
            Dim diagramView As MindFusion.Diagramming.WebForms.DiagramView = pag.FindControl("diagramView")
            For Each sha In diagramView.Diagram.Selection.Items
                If diagramView.Diagram.Selection.Items(0).GetType.FullName = "MindFusion.Diagramming.DiagramLink" Then
                    Dim split_id() As String = sha.Id.ToString.Split("_")
                    HttpContext.Current.Session.Item("DR_RUTASELECCION_ID_CONECTOR") = split_id(1)
                    Exit For
                End If
            Next
            Dim stru_config_conector_ruta As stru_config_conector_ruta = Nothing
            If HttpContext.Current.Session.Item("DR_RUTASELECCION_ID_CONECTOR") <> 0 Then
                Result = ref_Class_actividades_disponibles_envio.Solicita_configuracion_conector_ruta(HttpContext.Current.Session.Item("DR_RUTASELECCION_ID_CONECTOR"), _
                                                                                                      stru_config_conector_ruta)
                If Result <> "YES" Then
                    Seleccion_menu_pricipal = Result
                    Exit Function
                End If
                Dim CheckBox_estado_correo_conector As CheckBox = pag.FindControl("CheckBox_estado_correo_conector")
                Dim CheckBox_autoriza_tarea As CheckBox = pag.FindControl("CheckBox_autoriza_tarea")
                Dim CheckBox_estado_copia_estructura As CheckBox = pag.FindControl("CheckBox_estado_copia_estructura")
                Dim CheckBox_Estado_asigna_expediente As CheckBox = pag.FindControl("CheckBox_Estado_asigna_expediente")
                Dim CheckBox_autoriza_tarea_firma_digital As CheckBox = pag.FindControl("CheckBox_autoriza_tarea_firma_digital")
                Dim CheckBox_estado_firma_digital As CheckBox = pag.FindControl("CheckBox_estado_firma_digital")
                Dim CheckBox_estado_valida_balanceo As CheckBox = pag.FindControl("CheckBox_estado_valida_balanceo")
                Dim UpdatePanel_configura_envi_correo_conector As UpdatePanel = pag.FindControl("UpdatePanel_configura_envi_correo_conector")
                Dim ModalPopupExtender_edition_configura_envi_correo_conector As AjaxControlToolkit.ModalPopupExtender = _
                    pag.FindControl("ModalPopupExtender_edition_configura_envi_correo_conector")
                If stru_config_conector_ruta.Estado_evia_correo = 1 Then
                    CheckBox_estado_correo_conector.Checked = True
                Else
                    CheckBox_estado_correo_conector.Checked = False
                End If
                If stru_config_conector_ruta.Estado_soicita_autorizacion = 1 Then
                    CheckBox_autoriza_tarea.Checked = True
                Else
                    CheckBox_autoriza_tarea.Checked = False
                End If
                If stru_config_conector_ruta.Estado_soicita_autorizacion_firma_digital = 1 Then
                    CheckBox_autoriza_tarea_firma_digital.Checked = True
                Else
                    CheckBox_autoriza_tarea_firma_digital.Checked = False
                End If
                If stru_config_conector_ruta.Estado_copia_documento_estructura = 1 Then
                    CheckBox_estado_copia_estructura.Checked = True
                Else
                    CheckBox_estado_copia_estructura.Checked = False
                End If
                If stru_config_conector_ruta.Estado_asigna_expediente = 1 Then
                    CheckBox_Estado_asigna_expediente.Checked = True
                Else
                    CheckBox_Estado_asigna_expediente.Checked = False
                End If
                If stru_config_conector_ruta.Estado_firma_digital = 1 Then
                    CheckBox_estado_firma_digital.Checked = True
                Else
                    CheckBox_estado_firma_digital.Checked = False
                End If
                If stru_config_conector_ruta.estado_valida_balanceo = 1 Then
                    CheckBox_estado_valida_balanceo.Checked = True
                Else
                    CheckBox_estado_valida_balanceo.Checked = False
                End If
                UpdatePanel_configura_envi_correo_conector.Update()
                ModalPopupExtender_edition_configura_envi_correo_conector.Show()
            End If
        End If
        Seleccion_menu_pricipal = "YES"
    End Function
    Function Exporta_pdf_mindifucion(ByVal Ruta_Archivo As String, _
                                     ByRef diagramView As MindFusion.Diagramming.WebForms.DiagramView, _
                                     ByRef ref_iframe As Object, _
                                     ByRef ref_hiden As Object, _
                                     ByRef updatapanel_iframe As UpdatePanel) As String
        Try
            Dim pdfExp As New MindFusion.Diagramming.Export.PdfExporter
            If Directory.Exists(Ruta_Archivo) = False Then
                Directory.CreateDirectory(Ruta_Archivo)
                Directory.CreateDirectory(Ruta_Archivo & "\" & HttpContext.Current.Session.Item("Id_Usuario_Workflow"))
            Else
                If Directory.Exists(Ruta_Archivo & "\" & HttpContext.Current.Session.Item("Id_Usuario_Workflow")) = False Then
                    Directory.CreateDirectory(Ruta_Archivo & "\" & HttpContext.Current.Session.Item("Id_Usuario_Workflow"))
                End If
            End If
            ref_hiden.value = Ruta_Archivo & "\" & HttpContext.Current.Session.Item("Id_Usuario_Workflow") & "\export_" & HttpContext.Current.Session.Item("Id_Usuario_Workflow") & ".pdf"
            If File.Exists(ref_hiden.value) = True Then
                Kill(ref_hiden.value)
            End If
            Dim obshape As Object = Nothing
            Dim obshape_diagran As MindFusion.Diagramming.ShapeNode = Nothing
            For Each obshape In diagramView.Diagram.Items
                Dim ob As Object = obshape.GetType
                If ob.Fullname = "MindFusion.Diagramming.ShapeNode" Then
                    If obshape.ImageUrl <> "" Then
                        Dim localPath = HttpContext.Current.Server.MapPath(obshape.ImageUrl)
                        obshape.Image = Image.FromFile(localPath)
                        obshape_diagran = obshape
                        obshape_diagran.ImageAlign = MindFusion.Drawing.ImageAlign.TopCenter
                    End If

                End If
            Next
            'Dim archivo_sever As String = HttpContext.Current.Server.MapPath("../Temp_Image/134")
            'diagramView.Diagram.SaveToXml(archivo_sever & "\salida.xml")
            pdfExp.JpegImageEncoding = False
            pdfExp.Export(diagramView.Diagram, ref_hiden.value)
            ref_iframe.Attributes.Add("src", "..\Docuarchi\WebFormDaDescarga.aspx")
            updatapanel_iframe.Update()
            Exporta_pdf_mindifucion = "YES"
        Catch ex As Exception
            Exporta_pdf_mindifucion = "Inconsistencia general función Exporta_pdf_mindifucion " & ex.Message
        End Try

    End Function
    Function Abrir_menu_eventos(ByVal nombre_evento As String,
                                ByVal nombre_ruta As String,
                                ByRef pag As Page,
                                ByVal tipo_aplicacion As String) As String
        Try
            Dim Result As String = ""
            Dim ref_TextBox_contenido_edita_escript_evento As TextBox = pag.FindControl("TextBox_contenido_edita_escript_evento")
            Dim ref_UpdatePanel_contenido_edita_escript_evento As UpdatePanel = pag.FindControl("UpdatePanel_contenido_edita_escript_evento")
            Dim ref_HiddenField_nom_event As Object = pag.FindControl("HiddenField_nom_event")
            Dim ref_ref_HiddenField_result_edit_script As Object = pag.FindControl("HiddenField_result_edit_script")
            Dim ref_ModalPopupExtender_edition_edita_escript_evento As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_edition_edita_escript_evento")
            If nombre_ruta = "" Then
                Abrir_menu_eventos = "Debe seleccionar una ruta de actividades"
                Exit Function
            End If
            Dim diagramView As MindFusion.Diagramming.WebForms.DiagramView = pag.FindControl("diagramView")
            If diagramView.Diagram.Selection.Items.Count = 0 Then
                Abrir_menu_eventos = "Debe seleccionar la actividad"
                Exit Function
            End If
            Dim obshape As Object = diagramView.Diagram.Selection.Items(0)
            Dim ob As Object = obshape.GetType
            If ob.Fullname = "MindFusion.Diagramming.ShapeNode" Then
                Result = Me.Inicializa_interface_edita_escrip_actividad(obshape.text,
                                                                        nombre_evento,
                                                                        nombre_ruta,
                                                                        ref_TextBox_contenido_edita_escript_evento,
                                                                        ref_UpdatePanel_contenido_edita_escript_evento,
                                                                        tipo_aplicacion)
                If Result <> "YES" Then
                    Abrir_menu_eventos = Result
                    Exit Function
                Else
                    ref_HiddenField_nom_event.value = nombre_evento
                    nombre_evento = nombre_evento.Replace("ENLASE", "ENLACE")
                    ref_ref_HiddenField_result_edit_script.value = "ACTIVIDAD (" & obshape.text & ") EVENTO  (" & nombre_evento & ")"
                    ref_ModalPopupExtender_edition_edita_escript_evento.Show()
                    Abrir_menu_eventos = "YES"
                    Exit Function
                End If
            Else
                Abrir_menu_eventos = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Abrir_menu_eventos = "Inconsistencia general función Abrir_menu_eventos " & ex.Message
        End Try

    End Function
    Function Compila_evento_escript(ByRef pag As Page) As String
        Try
            Dim diagramView As MindFusion.Diagramming.WebForms.DiagramView = pag.FindControl("diagramView")
            Dim ref_TextBox_contenido_edita_escript_evento As TextBox = pag.FindControl("TextBox_contenido_edita_escript_evento")
            Dim ref_HiddenField_result_edit_script As Object = pag.FindControl("HiddenField_result_edit_script")
            Dim Result As String = ""
            If diagramView.Diagram.Selection.Items.Count = 0 Then
                Compila_evento_escript = "Debe seleccionar la actividad"
                Exit Function
            End If

            Dim mEval As New ClassEdtiScript
            Dim mExpresion As String = ref_TextBox_contenido_edita_escript_evento.Text
            'Creamos un objeto StringCollection y agregamos los parámetros de entrada que usará el método eval StringCollection
            Dim mParameters As New System.Collections.Specialized.StringCollection()
            mParameters.Add("ByVal X as Double")
            mParameters.Add("ByVal Y as Double")
            Dim mNameSpaces As New System.Collections.Specialized.StringCollection()
            Dim Matris() As String = {5, 6}
            Dim NombreFunction As String = "Main"
            If InStr(ref_HiddenField_result_edit_script.value, "-WEB") > 0 Then
                Result = mEval.PrecompilarAssembly_web(mExpresion, mParameters)
                If Result = "YES" Then
                    Compila_evento_escript = "YES"
                    mEval = Nothing
                    Exit Function
                Else
                    Compila_evento_escript = "No se ha generado el Assembly " & Result
                    Exit Function
                End If
            Else
                Result = mEval.PrecompilarAssembly_web(mExpresion, mParameters)
                If Result = "YES" Then
                    Compila_evento_escript = "YES"
                    mEval = Nothing
                    Exit Function

                Else
                    Compila_evento_escript = "No se ha generado el Assembly " & Result
                    Exit Function
                End If
            End If


        Catch ex As Exception
            Compila_evento_escript = "Inconsistencia general función Compila_evento_escript " & ex.Message
        End Try

    End Function
    Function Actualiza_escript_actividad_seleccionada(ByRef pag As Page) As String
        Try
            Dim diagramView As MindFusion.Diagramming.WebForms.DiagramView = pag.FindControl("diagramView")
            Dim ref_TextBox_contenido_edita_escript_evento As TextBox = pag.FindControl("TextBox_contenido_edita_escript_evento")
            Dim ref_HiddenField_result_edit_script As Object = pag.FindControl("HiddenField_result_edit_script")
            Dim ref_HiddenField_nom_event As Object = pag.FindControl("HiddenField_nom_event")
            Dim Result As String = ""
            If diagramView.Diagram.Selection.Items.Count = 0 Then
                Actualiza_escript_actividad_seleccionada = "Debe seleccionar la actividad"
                Exit Function
            End If
            If ref_HiddenField_nom_event.value = "" Then
                Actualiza_escript_actividad_seleccionada = "Debe sistema no detecto el evento a modificar"
                Exit Function
            End If
            Dim id_ruta As Integer = 0
            Dim refclas_workflow As New ClassWorkflow
            Dim Ref_class_wf_ruta As New Class_worflow_rutas
            Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA"), _
                                                                id_ruta)
            If Result <> "YES" Then
                Actualiza_escript_actividad_seleccionada = Result
                Exit Function
            End If
            Dim obshape As Object = diagramView.Diagram.Selection.Items(0)
            Dim ob As Object = obshape.GetType
            Dim ref_class As New ClassEdtiScript
            Dim TextScript As String = Replace(ref_TextBox_contenido_edita_escript_evento.Text, "'", "ï")
            Result = ref_class.Actualiza_Script(obshape.text, ref_HiddenField_nom_event.value, TextScript, id_ruta)
            If Result <> "YES" Then
                Actualiza_escript_actividad_seleccionada = Result
                Exit Function
            End If
            Actualiza_escript_actividad_seleccionada = "YES"
        Catch ex As Exception
            Actualiza_escript_actividad_seleccionada = "Inconsistencia general función Actualiza_escript_actividad_seleccionada " & ex.Message
        End Try

    End Function
    Function Inicializa_interface_edita_escrip_actividad(ByVal Nombre_Actividad As String,
                                                         ByVal nombre_evento As String,
                                                         ByVal nombre_ruta As String,
                                                         ByRef ref_texbox As TextBox,
                                                         ByRef ref_update As UpdatePanel,
                                                         ByVal tipo_aplicacion As String) As String
        Try
            Dim Result As String = ""
            '------Retorna id de ruta workflow
            Dim Refclas_workflow As New ClassWorkflow
            Dim id_ruta As Integer = 0
            Dim Ref_class_wf_ruta As New Class_worflow_rutas
            Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(nombre_ruta,
                                                                id_ruta)
            If Result <> "YES" Then
                Inicializa_interface_edita_escrip_actividad = Result
                Exit Function
            End If
            '------Retorna el valor del evento selecionado
            Dim Contenido_esript As String = ""
            Dim Estado_existencia As String = "NO"
            Result = Me.Consulta_valor_Script(Nombre_Actividad,
                                              nombre_evento,
                                              id_ruta, Contenido_esript,
                                              Estado_existencia)
            If Result <> "YES" Then
                Inicializa_interface_edita_escrip_actividad = Result
                Exit Function
            End If
            Dim id_actividad_general As Integer = 0
            Dim tipo_actividad As String = ""
            Result = Retorna_id_actividad_general_ruta(Nombre_Actividad,
                                                       id_ruta,
                                                       id_actividad_general,
                                                       tipo_actividad)
            If Result <> "YES" Then
                Inicializa_interface_edita_escrip_actividad = Result
                Exit Function
            End If
            '-----Determina evento si se ejecuta
            Dim Resultado_pertenencia_evento As String = ""
            If tipo_aplicacion = "WEB" Then
                If tipo_actividad = "USUARIO" Then
                    Dim matri_usuario() As String = HttpContext.Current.Session.Item("MatriScriptUusarioWEB")
                    For i As Integer = 0 To matri_usuario.Length - 1
                        If nombre_evento = matri_usuario(i) Then
                            Resultado_pertenencia_evento = "YES"
                            Exit For
                        End If
                    Next
                End If
                If tipo_actividad = "ENLASE" Then
                    Dim matri_usuario() As String = HttpContext.Current.Session.Item("MatriScriptEnlaseWEB")
                    For i As Integer = 0 To matri_usuario.Length - 1
                        If nombre_evento = matri_usuario(i) Then
                            Resultado_pertenencia_evento = "YES"
                            Exit For
                        End If
                    Next
                End If
                If tipo_actividad = "SISTEMA" Then
                    Dim matri_usuario() As String = HttpContext.Current.Session.Item("MatriScriptSistema")
                    For i As Integer = 0 To matri_usuario.Length - 1
                        If nombre_evento = matri_usuario(i) Then
                            Resultado_pertenencia_evento = "YES"
                            Exit For
                        End If
                    Next
                End If
            End If
            If tipo_aplicacion = "ESCRITORIO" Then
                If tipo_actividad = "USUARIO" Then
                    Dim matri_usuario() As String = HttpContext.Current.Session.Item("MatriScriptUusario")
                    For i As Integer = 0 To matri_usuario.Length - 1
                        If nombre_evento = matri_usuario(i) Then
                            Resultado_pertenencia_evento = "YES"
                            Exit For
                        End If
                    Next
                End If
                If tipo_actividad = "ENLASE" Then
                    Dim matri_usuario() As String = HttpContext.Current.Session.Item("MatriScriptEnlace")
                    For i As Integer = 0 To matri_usuario.Length - 1
                        If nombre_evento = matri_usuario(i) Then
                            Resultado_pertenencia_evento = "YES"
                            Exit For
                        End If
                    Next
                End If
                If tipo_actividad = "SISTEMA" Then
                    Dim matri_usuario() As String = HttpContext.Current.Session.Item("MatriScriptSistema")
                    For i As Integer = 0 To matri_usuario.Length - 1
                        If nombre_evento = matri_usuario(i) Then
                            Resultado_pertenencia_evento = "YES"
                            Exit For
                        End If
                    Next
                End If
            End If
            If Resultado_pertenencia_evento = "" Then
                Dim REF_NOMBRE As String = nombre_evento
                REF_NOMBRE = REF_NOMBRE.Replace("ENLASE", "ENLACE")
                Inicializa_interface_edita_escrip_actividad = "La actividad " & Nombre_Actividad & " no dispone del evento " & REF_NOMBRE
                Exit Function
            End If
            '-----Crea el evento si el evento no existe
            If Estado_existencia = "NO" Then
                '------Retorna id actividad seleccionada
                Dim id_actividad As Integer = 0
                Result = Retorna_Id_Actividad_por_id_ruta(Nombre_Actividad,
                                                          id_ruta,
                                                          id_actividad)
                If Result <> "YES" Then
                    Inicializa_interface_edita_escrip_actividad = Result
                    Exit Function
                End If
                Result = Me.Crea_evento_actividad_individual(id_actividad,
                                                             id_actividad_general,
                                                             nombre_evento)
                If Result <> "YES" Then
                    Inicializa_interface_edita_escrip_actividad = Result
                    Exit Function
                End If
            End If
            Dim stri As String = ""
            If Contenido_esript = "" Then
                stri = "Public Shared Function Main(ByVal Conect_Wf As string,ByVal Id_User_WF As string) As string"
                stri = stri + vbCr + "End Function" & vbCr
                Select Case nombre_evento
                    Case "DEFAULTSCRIPT-WEB"
                        stri = "Public Shared Function Main(ByVal Conect_Wf As string,ByVal Id_Usuario As string,ByVal Id_Grupo As String, ByVal Id_Actividad As String,ByVal id_Tarea_sel As  String,Byval Id_Ruta_Workflow as String) As string"
                        stri = stri + vbCr + "End Function" & vbCr
                    Case "CREARIMAGENES-WEB"
                        stri = "Public Shared Function Main(ByVal Conect_Wf As string,ByVal Id_User_WF As string, ByVal id_Tarea_sel As  String,Byval Id_Ruta_Workflow as String) As string"
                        stri = stri + vbCr + "End Function" & vbCr
                    Case "ENLASE-WEB"
                        stri = "Public Shared Function Main(ByVal Conect_Wf As string,ByVal Id_Usuario As string,ByVal Id_Grupo As String" +
                        ",ByVal Id_Actividad As String," +
                        "ByVal id_Tarea_sel As  String,Byval Id_Ruta_Workflow as String) As string"
                        stri = stri + vbCr + "End Function" & vbCr
                    Case "INICIO-WEB"
                        stri = "Public Shared Function Main(ByVal Conect_Wf As string,ByVal Id_Usuario As string,ByVal Id_Grupo As String, ByVal Id_Actividad As String,ByVal id_Tarea_sel As  String,Byval Id_Ruta_Workflow as String) As string"
                        stri = stri + vbCr + "End Function" & vbCr
                    Case "PRETERMINARACTIVIAD-WEB"
                        stri = "Public Shared Function Main(ByVal Conect_Wf As string,ByVal Id_User_WF As Object, ByVal id_tarea As Object, Byval id_ruta as string, Byval Id_Usuario_Destino As Object, Byval Id_Actividad_Destino As object, Byval id_flujo_trabajo As Object, byval id_actividad_flujo_trabajo As Object, Byval id_usuario_workflow_flujo_trabajo As Object) As string"
                        stri = stri + vbCr + "End Function" & vbCr
                    Case "TERMINARACTIVIDAD-WEB"
                        stri = "Public Shared Function Main(ByVal Conect_Wf As string,ByVal Id_User_WF As string,ByVal id_tarea As Object, Byval id_ruta As Object, Byval Id_Usuario_Destino As Object, Byval Id_Actividad_Destino As object, Byval id_flujo_trabajo As Object, byval id_actividad_flujo_trabajo As Object, Byval id_usuario_workflow_flujo_trabajo As Object) As string"
                        stri = stri + vbCr + "End Function" & vbCr
                    Case "ADJUNTARIMAGENES-WEB"
                        stri = "Public Shared Function Main(ByVal Conect_Wf As string,ByVal Id_User_WF As string,ByVal id_Tarea As string,Byval Id_Activida as string, Byval Id_Imagen as String) As string"
                        stri = stri + vbCr + "End Function" & vbCr
                    Case "ADJUNTOS-WEB"
                        stri = "Public Shared Function Main(ByVal Conect_Wf As string,ByVal Id_User_WF As string,ByVal id_Tarea As string,Byval Id_Activida as string, Byval Id_Imagen as String) As string"
                        stri = stri + vbCr + "End Function" & vbCr
                    Case "TOMARTAREA-WEB"
                        stri = "Public Shared Function Main(ByVal Conect_Wf As String, ByVal Id_User_WF As Object,ByVal id_Tarea As Object, ByVal Id_Activida As Object) As string"
                        stri = stri + vbCr + "End Function" & vbCr
                End Select
                ref_texbox.Text = stri
                ref_update.Update()
                Inicializa_interface_edita_escrip_actividad = "YES"
                Exit Function
            Else
                ref_texbox.Text = Replace(Contenido_esript, "ï", "'")
                ref_update.Update()
                Inicializa_interface_edita_escrip_actividad = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Inicializa_interface_edita_escrip_actividad = "Inconsistencia general función Inicializa_interface_edita_escrip_actividad " & ex.Message
        End Try

    End Function
    Function SolicitaperfilDiagramadorUsuarioWorkflow(ByVal id_usuario_workflow As Integer,
                                                      ByRef importador_ruta As Integer,
                                                      ByRef crea_flujo_trabajo As Integer,
                                                      ByRef agrega_actividad As Integer,
                                                      ByRef conecta_actividad As Integer,
                                                      ByRef elimina_actividad As Integer,
                                                      ByRef elimina_conector As Integer,
                                                      ByRef diagramador As Integer,
                                                      ByRef migracion As Integer) As String
        '------------------------------------------------------
        'Función : Retorna perfil diagramador usuario worokflow
        'Fecha : 2017-06-28
        'Ing : Miguel Angel Urueta Miranda
        '------------------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_perfil_diagrmador")
            Dim sql_consulta As String = "Select IMPORTADOR_RUTA,CREA_FLUJO_TRABAJO,AGREGA_ACTIVIDAD," &
                "CONECTA_ACTIVIDAD,ELIMINA_ACTIVIDAD,ELIMINA_CONECTOR,DIAGRAMADOR,MIGRACION from wf_perfil_diagramador " &
                " where usuario_workflow_idu_suario=" & id_usuario_workflow
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaperfilDiagramadorUsuarioWorkflow = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                importador_ruta = 0
                crea_flujo_trabajo = 0
                agrega_actividad = 0
                conecta_actividad = 0
                elimina_actividad = 0
                elimina_conector = 0
                diagramador = 0
                SolicitaperfilDiagramadorUsuarioWorkflow = "YES"
                Exit Function
            Else
                importador_ruta = Datset.Tables(0).Rows(0).Item(0)
                crea_flujo_trabajo = Datset.Tables(0).Rows(0).Item(1)
                agrega_actividad = Datset.Tables(0).Rows(0).Item(2)
                conecta_actividad = Datset.Tables(0).Rows(0).Item(3)
                elimina_actividad = Datset.Tables(0).Rows(0).Item(4)
                elimina_conector = Datset.Tables(0).Rows(0).Item(5)
                diagramador = Datset.Tables(0).Rows(0).Item(6)
                migracion = Datset.Tables(0).Rows(0).Item(7)
                SolicitaperfilDiagramadorUsuarioWorkflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaperfilDiagramadorUsuarioWorkflow = "Inconsistencia general función Función SolicitaperfilDiagramadorUsuarioWorkflow " & ex.Message
        End Try
    End Function
    Function Solicita_nombres_rutas_workflow_sin_importacion(ByRef rutas() As String) As String
        '----------------------------------------------
        'Función : Retorna nombre de rutas workflow sin
        'importar
        'Fecha : 2017-06-29
        'Ing : Miguel Angel Urueta Miranda
        '----------------------------------------------
        Try
            Erase rutas
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("rutas_workflow")
            Dim sql_consulta As String = "Select Nombre_Ruta from rutas_workflow " &
                " where Archivo_Plantilla_Mindifucion is null"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombres_rutas_workflow_sin_importacion = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_nombres_rutas_workflow_sin_importacion = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve rutas(i)
                    rutas(i) = Datset.Tables(0).Rows(i).Item(0)
                Next
                Solicita_nombres_rutas_workflow_sin_importacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombres_rutas_workflow_sin_importacion = "Inconsistencia general función Solicita_nombres_rutas_workflow_sin_importacion " & ex.Message
        End Try
    End Function
    Function Solicita_nombres_rutas_workflow(ByRef rutas() As String) As String
        '----------------------------------------------
        'Función : Retorna nombre de rutas workflow 
        'Fecha : 2017-07-04
        'Ing : Miguel Angel Urueta Miranda
        '----------------------------------------------
        Try
            Erase rutas
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("rutas_workflow")
            Dim sql_consulta As String = "Select Nombre_Ruta from rutas_workflow " &
                " where not Archivo_Plantilla_Mindifucion  is null"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombres_rutas_workflow = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_nombres_rutas_workflow = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve rutas(i)
                    rutas(i) = Datset.Tables(0).Rows(i).Item(0)
                Next
                Solicita_nombres_rutas_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombres_rutas_workflow = "Inconsistencia general función Solicita_nombres_rutas_workflow " & ex.Message
        End Try
    End Function

    Function Lista_rutas_interface_importacion(ByVal rutas() As String,
                                               ByRef ref_droplis As DropDownList,
                                               ByRef ref_update As UpdatePanel,
                                               Optional ByVal estado_limpia As Integer = 0) As String
        '----------------------------------------------
        'Función : Lista el nombre de rutas disponibles
        'en la interface
        'Fecha : 2017-06-29
        'Ing : Miguel Angel Urueta Miranda
        '----------------------------------------------
        Try
            If estado_limpia = 0 Then
                ref_droplis.Items.Clear()
            End If
            If Not rutas Is Nothing Then
                For i As Integer = 0 To rutas.Length - 1
                    ref_droplis.Items.Add(rutas(i))
                Next
                ref_update.Update()
                Lista_rutas_interface_importacion = "YES"
            Else
                ref_update.Update()
                Lista_rutas_interface_importacion = "YES"
            End If
        Catch ex As Exception
            Lista_rutas_interface_importacion = "Inconsistencia general función Lista_rutas_interface_importacion " & ex.Message
        End Try
    End Function

    Function Lista_campos_disponibles_ruta_tabla(ByVal nombre_ruta As String, ByRef grediview As GridView, ByRef HiddenEmailconsulta As Object,
        ByRef reflabel As Label, ByRef hideselecion As Object, ByRef update As UpdatePanel) As String
        Try
            If nombre_ruta = "Seleccione" Then
                HiddenEmailconsulta.value = ""
                reflabel.Text = "Se encontraron 0 registro(s) "
                grediview.DataSource = Nothing
                'grediview.DataKeyNames = DataKey
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                Lista_campos_disponibles_ruta_tabla = "YES"
                Exit Function
            End If
            Dim Sql_consulta As String = " DESCRIBE dat_adic_tar" & nombre_ruta
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("dat_adic_tar" & nombre_ruta)
            Dim Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_campos_disponibles_ruta_tabla = "Error listando descripción tabla  " & "dat_adic_tar" & nombre_ruta & " " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                HiddenEmailconsulta.value = ""
                reflabel.Text = "Se encontraron 0 registro(s) "
                grediview.DataSource = Nothing
                'grediview.DataKeyNames = DataKey
                hideselecion.value = ""
                grediview.DataBind()
                update.Update()
                Lista_campos_disponibles_ruta_tabla = "YES"
                Exit Function
            Else
                reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) "
                grediview.DataSource = Datset
                hideselecion.value = ""
                grediview.DataBind()
                HiddenEmailconsulta.value = "YES"
                update.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(0).Text.ToString())
                    grediview.Rows(i).Attributes.Add("id_nombre", grediview.Rows(i).Cells(1).Text.ToString())
                Next
                Lista_campos_disponibles_ruta_tabla = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_campos_disponibles_ruta_tabla = "Inconsistencia general función Lista_campos_disponibles_ruta " & ex.Message
        End Try

    End Function
    Function Lista_campos_disponibles_ruta(ByVal id_ruta As String, ByRef grediview As GridView,
                                           ByRef HiddenEmailconsulta As Object,
                                           ByRef reflabel As Label,
                                           ByRef hideselecion As Object,
                                           ByRef update As UpdatePanel) As String
        Try

            Dim Sql_consulta As String = " SELECT Id_Configuracion,Nombre_Campo,Tipo_Campo,Lista_Tarea," &
                "Ordena_Tarea,Prioridad,campo_radicado,campo_tramite,Campo_beneficiario,campo_fecha_vence FROM CONFIGURACION_LISTADO_RUTA  " _
           & " WHERE RUTAS_WORKFLOW_ID_RUTA = " & id_ruta & " Order by  id_campo ASC"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_LISTADO_RUTA")
            Dim Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_campos_disponibles_ruta = "Error listando campos disponible ruta, función Lista_campos_disponibles_ruta dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                HiddenEmailconsulta.value = ""
                reflabel.Text = "Se encontraron 0 registro(s) "
                grediview.DataSource = Nothing
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                Lista_campos_disponibles_ruta = "YES"
                Exit Function
            Else
                HiddenEmailconsulta.value = "YES"
                reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) "
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(0).Text.ToString())
                Next
                Lista_campos_disponibles_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_campos_disponibles_ruta = "Inconsistencia general función Lista_campos_disponibles_ruta " & ex.Message
        End Try

    End Function

    Function Typo_Campo_Resutl_Mysql(ByRef Matri_Typo_Mysql() As Matri_Typo_campo_Mysql) As String
        Try

            Dim i As Integer = 0
            ReDim Preserve Matri_Typo_Mysql(i)
            Matri_Typo_Mysql(i).Tipo_Campo = "INT"
            Matri_Typo_Mysql(i).Longitud_Campo = "10"
            i = i + 1
            ReDim Preserve Matri_Typo_Mysql(i)
            Matri_Typo_Mysql(i).Tipo_Campo = "INTEGER"
            Matri_Typo_Mysql(i).Longitud_Campo = "10"
            i = i + 1
            ReDim Preserve Matri_Typo_Mysql(i)
            Matri_Typo_Mysql(i).Tipo_Campo = "BIGINT"
            Matri_Typo_Mysql(i).Longitud_Campo = ""
            i = i + 1
            ReDim Preserve Matri_Typo_Mysql(i)
            Matri_Typo_Mysql(i).Tipo_Campo = "DATE"
            Matri_Typo_Mysql(i).Longitud_Campo = ""
            i = i + 1
            ReDim Preserve Matri_Typo_Mysql(i)
            Matri_Typo_Mysql(i).Tipo_Campo = "DATETIME"
            Matri_Typo_Mysql(i).Longitud_Campo = ""
            i = i + 1
            ReDim Preserve Matri_Typo_Mysql(i)
            Matri_Typo_Mysql(i).Tipo_Campo = "YEAR"
            Matri_Typo_Mysql(i).Longitud_Campo = "2"
            i = i + 1
            ReDim Preserve Matri_Typo_Mysql(i)
            Matri_Typo_Mysql(i).Tipo_Campo = "VARCHAR"
            Matri_Typo_Mysql(i).Longitud_Campo = "120"
            i = i + 1
            ReDim Preserve Matri_Typo_Mysql(i)
            Matri_Typo_Mysql(i).Tipo_Campo = "TEXT"
            Matri_Typo_Mysql(i).Longitud_Campo = ""
            Typo_Campo_Resutl_Mysql = "YES"
        Catch ex As Exception
            Typo_Campo_Resutl_Mysql = ex.Message
        End Try
    End Function
    Function Lista_tipos_campo_interface(ByVal Matri_Typo_Mysql() As Matri_Typo_campo_Mysql, ByRef ref_drolist As DropDownList, ByRef ref_update As UpdatePanel) As String
        Try
            ref_drolist.Items.Clear()
            ref_drolist.Items.Add("")
            For i As Integer = 0 To Matri_Typo_Mysql.Length - 1
                ref_drolist.Items.Add(Matri_Typo_Mysql(i).Tipo_Campo)
            Next
            ref_update.Update()
            Lista_tipos_campo_interface = "YES"
        Catch ex As Exception
            Lista_tipos_campo_interface = "Inconsistencia general función Lista_tipos_campo_interface " & ex.Message
        End Try
    End Function
    Function Lista_numero_indices(ByRef ref_drop_list As DropDownList, ByRef ref_update_panel As UpdatePanel) As String
        Try
            For i As Integer = 1 To 255
                ref_drop_list.Items.Add(i)
            Next
            ref_update_panel.Update()
            Lista_numero_indices = "YES"
        Catch ex As Exception
            Lista_numero_indices = "Inconsistencia general función Lista_numero_indices " & ex.Message
        End Try
    End Function
    Function Selecion_tipo_campo(ByVal ref_drop_list As DropDownList, ByRef Text_list_logitud As TextBox, ByRef ref_update As UpdatePanel) As String
        Try
            Dim Matris_Campo() As Matri_Typo_campo_Mysql
            Erase Matris_Campo
            Dim Result As String = Me.Typo_Campo_Resutl_Mysql(Matris_Campo)
            Dim i As Integer = 0
            If Not Matris_Campo Is Nothing Then
                For i = 0 To UBound(Matris_Campo)
                    If Matris_Campo(i).Tipo_Campo = ref_drop_list.Text And Matris_Campo(i).Longitud_Campo <> "" Then
                        Text_list_logitud.Enabled = True
                        Text_list_logitud.Text = Matris_Campo(i).Longitud_Campo
                        ref_update.Update()
                        Selecion_tipo_campo = "YES"
                        Exit Function
                    End If
                    If Matris_Campo(i).Tipo_Campo = ref_drop_list.Text And Matris_Campo(i).Longitud_Campo = "" Then
                        Text_list_logitud.Enabled = False
                        Text_list_logitud.Text = Matris_Campo(i).Longitud_Campo
                        ref_update.Update()
                        Selecion_tipo_campo = "YES"
                        Exit Function
                    End If
                Next
            End If
            Selecion_tipo_campo = "YES"
        Catch ex As Exception
            Selecion_tipo_campo = "Inconsistencia general función Selecion_tipo_campo " & ex.Message
        End Try
    End Function
    Function Adiciona_Nuevo_Campo_Ruta_workflow(ByVal Nombre_Ruta As String, ByVal Nombre_Campo As String,
    ByVal Typo_Campo As String, ByVal Longitud As String, ByVal oblitorio As Integer, ByVal TextBox_longitud_campo As TextBox) As String
        '****************************************************
        'Funcion   : Adiciona_Nuevo_Campo_Dato_Tarea
        'Ingeniero : Miguel Angel Urueta Miranda
        'Fecha     : 2009-05-27
        'Descripcion : Adiciona nuevo campo a la tabla 
        'DAT_ADIC_TAR + NOMBRE RUTA con el proposito
        'de tener campos para los datos adiocionales de las
        'tareas
        'Modificado para la versión web por el ingeniero
        'Miguel Angel Urueta Miranda
        'Fecha 2017-07-11
        '****************************************************
        Try
            If Nombre_Ruta = "" Then
                Adiciona_Nuevo_Campo_Ruta_workflow = "Por favor seleccione el nombre de la ruta "
                Exit Function
            End If
            If Nombre_Campo = "" Then
                Adiciona_Nuevo_Campo_Ruta_workflow = "Por favor informe el nombre del campo "
                Exit Function
            End If
            If Typo_Campo = "" Then
                Adiciona_Nuevo_Campo_Ruta_workflow = "Por favor seleccione el tipo de campo "
                Exit Function
            End If

            If TextBox_longitud_campo.Enabled = True Then
                If TextBox_longitud_campo.Text = "" Then
                    Adiciona_Nuevo_Campo_Ruta_workflow = "Por favor informe la longitud del campo"
                    Exit Function
                End If
            End If
            Dim SqlAterTable As String = "ALTER TABLE DAT_ADIC_TAR" & Nombre_Ruta & " Add " & Nombre_Campo
            If Longitud <> "" Then
                SqlAterTable = SqlAterTable & " " & Typo_Campo & "(" & Longitud & ")"
            Else
                SqlAterTable = SqlAterTable & " " & Typo_Campo
            End If
            If oblitorio = 1 Then
                SqlAterTable = SqlAterTable & " NOT NULL"
            Else
                SqlAterTable = SqlAterTable & " NULL"
            End If
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Result As String = ""
            Result = ref.SELECTION_INSERT_COMMAND(SqlAterTable)
            If Result <> "YES" Then
                Adiciona_Nuevo_Campo_Ruta_workflow = Result
                Exit Function
            Else
                Adiciona_Nuevo_Campo_Ruta_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Adiciona_Nuevo_Campo_Ruta_workflow = ex.Message
        End Try
    End Function
    Function Adiciona_campo_listado_ruta_workflow(ByVal id_ruta As Integer, ByVal nombre_campo As String, ByVal nombre_tipo_campo As String) As String
        Try
            Dim Result As String = ""
            Dim Refclas As New Class_worflow_rutas
            Dim estado_existencia As String = ""
            Result = Refclas.Verifica_existencia_Nombre_campo_listado_ruta(id_ruta, nombre_campo, estado_existencia)
            If Result <> "YES" Then
                Adiciona_campo_listado_ruta_workflow = "Inconsistencia general función Adiciona_campo_listado_ruta_workflow " & Result
                Exit Function
            End If
            If estado_existencia = "YES" Then
                Adiciona_campo_listado_ruta_workflow = "El campo a importar ya se encuentra registrado en el listado de campos disponibles "
                Exit Function
            End If
            Dim id_campo As Integer = 0
            Result = Me.Retorna_valor_indice_ultimo_campo_ordenacion(id_ruta, id_campo)
            If Result <> "YES" Then
                Adiciona_campo_listado_ruta_workflow = Result
                Exit Function
            End If
            id_campo = id_campo + 1
            '-----------------------------------------------
            'Inserta el registro del campo en la lista
            '-----------------------------------------------
            Dim sql_insert As String = "Insert Into CONFIGURACION_lISTADO_RUTA(RUTAS_WORKFLOW_ID_RUTA," _
                        & "NOMBRE_CAMPO,TIPO_CAMPO,LISTA_TAREA,ORDENA_TAREA,PRIORIDAD,id_campo)VALUES (" _
                        & id_ruta & ",'" & nombre_campo & "','" & nombre_tipo_campo & "',0,0,0," & id_campo & ")"
            Dim ref As New conect.Dbase_Conction_Mysql
            Result = ref.SELECTION_INSERT_COMMAND(sql_insert)
            If Result <> "YES" Then
                Adiciona_campo_listado_ruta_workflow = Result
                Exit Function
            Else
                Adiciona_campo_listado_ruta_workflow = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Adiciona_campo_listado_ruta_workflow = "Inconsistencia general función Adiciona_campo_listado_ruta_workflow " & ex.Message
        End Try
    End Function

    Function Verifica_existencia_Nombre_campo_listado_ruta(ByVal id_ruta As String, ByVal Nombre_Campo As String, ByRef estado_existencia As String) As String
        '------------------------------------------------------------
        'Funcíon : Verifica la existencia del campo en el listado de
        'la ruta con el parametro id ruta y nombre ruta
        'Fecha : 2017-07-11
        'Ing : Miguel Angel Urueta Miranda
        '------------------------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_LISTADO_RUTA")
            Dim Sql_consulta As String = "Select * from CONFIGURACION_LISTADO_RUTA WHERE RUTAS_WORKFLOW_ID_RUTA = '" & id_ruta & "'" _
            & " AND NOMBRE_CAMPO = '" & Nombre_Campo & "'"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_Nombre_campo_listado_ruta = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_existencia = "NO"
                Verifica_existencia_Nombre_campo_listado_ruta = "YES"
                Exit Function
            Else
                estado_existencia = "YES"
                Verifica_existencia_Nombre_campo_listado_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_Nombre_campo_listado_ruta = "Inconsistencia general función Verifica_existencia_Nombre_campo_listado_ruta " & ex.Message
        End Try
    End Function
    Function Eliminar_campo_listado_ruta(ByVal id_campo As Integer) As String
        '-------------------------------------------------
        'Función : Elimina el campo del listado de campos
        'de ruta workflow
        'Fecha : 2017-07-13
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------------
        Try
            Dim SqlInsert As String = "Delete from CONFIGURACION_lISTADO_RUTA WHERE Id_Configuracion = " _
                 & id_campo
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Result As String = ref.SELECTION_INSERT_COMMAND(SqlInsert)
            If Result <> "YES" Then
                Eliminar_campo_listado_ruta = Result
                Exit Function
            Else
                Eliminar_campo_listado_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Eliminar_campo_listado_ruta = "Inconsistencia general función Eliminar_campo_listado_ruta " & ex.Message
        End Try
    End Function
    Function Asigna_datos_interface_configuracion_campo(ByVal id_configuracion_campo As Integer,
        ByRef ref_CheckBox_Lista_Campo_ruta As CheckBox, ByRef ref_CheckBox_Ordena_La_lista As CheckBox,
        ByRef ref_CheckBox_Campo_Prioridad_Lista As CheckBox, ByRef ref_update As UpdatePanel) As String
        '------------------------------------------------------
        'Función : Asigna datos interface edición con el id
        'configuración del listado
        'Fecha : 2017-07-13
        'Ing : Miguel Angel Urueta Miranda
        '-------------------------------------------------------
        Try
            Dim sqlconsulta As String = "Select Lista_Tarea,Ordena_Tarea,Prioridad from configuracion_listado_ruta where Id_Configuracion=" & id_configuracion_campo
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_LISTADO_RUTA")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Asigna_datos_interface_configuracion_campo = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Asigna_datos_interface_configuracion_campo = "Imposible encontrar datos del campo con la identificación " & id_configuracion_campo
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).Item(0) = 0 Then
                    ref_CheckBox_Lista_Campo_ruta.Checked = False
                Else
                    ref_CheckBox_Lista_Campo_ruta.Checked = True
                End If
                If Datset.Tables(0).Rows(0).Item(1) = 0 Then
                    ref_CheckBox_Ordena_La_lista.Checked = False
                Else
                    ref_CheckBox_Ordena_La_lista.Checked = True
                End If
                If Datset.Tables(0).Rows(0).Item(2) = 0 Then
                    ref_CheckBox_Campo_Prioridad_Lista.Checked = False
                Else
                    ref_CheckBox_Campo_Prioridad_Lista.Checked = True
                End If
                ref_update.Update()
                Asigna_datos_interface_configuracion_campo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Asigna_datos_interface_configuracion_campo = "Inconsistencia general función Asigna_datos_interface_configuracion_campo " & ex.Message
        End Try
    End Function
    Function Actualiza_configuracion_campo_listado_ruta(ByVal id_configuracion_campo As Integer,
        ByRef ref_CheckBox_Lista_Campo_ruta As CheckBox, ByRef ref_CheckBox_Ordena_La_lista As CheckBox,
        ByRef ref_CheckBox_Campo_Prioridad_Lista As CheckBox,
        ByRef ref_Hidden_estado_configura_campo_lista As Object, ByRef up_date As UpdatePanel) As String
        '---------------------------------------------------------
        'Función : Actualiza la configuración de los campos
        'Fecha : 2017-07-14
        'Ing : Miguel Angel Urueta Miranda
        '---------------------------------------------------------
        Try
            Dim Lista_Campo_Ruta As Integer = 0
            If ref_CheckBox_Lista_Campo_ruta.Checked = True Then
                Lista_Campo_Ruta = 1
            Else
                Lista_Campo_Ruta = 0
            End If
            Dim Ordena_La_lista As Integer = 0
            If ref_CheckBox_Ordena_La_lista.Checked = True Then
                Ordena_La_lista = 1
            Else
                Ordena_La_lista = 0
            End If
            Dim Campo_Prioridad_Lista As Integer = 0
            If ref_CheckBox_Campo_Prioridad_Lista.Checked = True Then
                Campo_Prioridad_Lista = 1
            Else
                Campo_Prioridad_Lista = 0
            End If
            Dim Update As String = "update configuracion_listado_ruta set Lista_Tarea=" & Lista_Campo_Ruta &
                ",Ordena_Tarea=" & Ordena_La_lista & ",Prioridad=" & Campo_Prioridad_Lista & " where Id_Configuracion=" & id_configuracion_campo
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Result As String = ref.SELECTION_INSERT_COMMAND(Update)
            If Result <> "YES" Then
                Actualiza_configuracion_campo_listado_ruta = Result
                Exit Function
            Else
                Actualiza_configuracion_campo_listado_ruta = "YES"
                ref_Hidden_estado_configura_campo_lista.value = "YES"
                up_date.Update()
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_configuracion_campo_listado_ruta = "Inconsistencia general función Actualiza_configuracion_campo_listado_ruta " & ex.Message
        End Try
    End Function
    Function Lista_orden_ordenacion_listado_ruta(ByVal id_ruta As Integer, ByRef ref_DropDownList_configuracion_listado_ruta As DropDownList,
                                                 ByRef ref_UpdatePanel_configura_listado_ruta As UpdatePanel) As String
        Try
            Dim sqlconsulta As String = "Select TIPO_ORDENACION_CAMPO from configuracion_listado_ruta_parametros where rutas_workflow_id_Ruta=" & id_ruta
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("configuracion_listado_ruta_parametros")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Lista_orden_ordenacion_listado_ruta = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                ref_DropDownList_configuracion_listado_ruta.Items.Clear()
                ref_DropDownList_configuracion_listado_ruta.Items.Add("ASCENDENTE")
                ref_DropDownList_configuracion_listado_ruta.Items.Add("DESCENDENTE")
                ref_UpdatePanel_configura_listado_ruta.Update()
                Lista_orden_ordenacion_listado_ruta = "YES"
                Exit Function
            Else
                ref_DropDownList_configuracion_listado_ruta.Items.Clear()
                ref_DropDownList_configuracion_listado_ruta.Items.Add("ASCENDENTE")
                ref_DropDownList_configuracion_listado_ruta.Items.Add("DESCENDENTE")
                Dim tipo_orden As String = Datset.Tables(0).Rows(0).Item(0)
                For i As Integer = 0 To ref_DropDownList_configuracion_listado_ruta.Items.Count - 1
                    If ref_DropDownList_configuracion_listado_ruta.Items(i).Text = tipo_orden Then
                        ref_DropDownList_configuracion_listado_ruta.Text = tipo_orden
                    End If
                Next
                ref_UpdatePanel_configura_listado_ruta.Update()
                Lista_orden_ordenacion_listado_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_orden_ordenacion_listado_ruta = "Inconsistencia general función Lista_orden_ordenacion_listado_ruta " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_lista_ordenacion_listado_ruta(ByVal id_ruta As Integer, ByRef estado_configuracion As String) As String
        Try
            Dim sqlconsulta As String = "Select idconfiguracion_listado_ruta_parametros from configuracion_listado_ruta_parametros where rutas_workflow_id_Ruta=" & id_ruta
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("configuracion_listado_ruta_parametros")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_lista_ordenacion_listado_ruta = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_configuracion = "NO"
                Verifica_existencia_lista_ordenacion_listado_ruta = "YES"
                Exit Function
            Else
                estado_configuracion = "YES"
                Verifica_existencia_lista_ordenacion_listado_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_lista_ordenacion_listado_ruta = "Inconsistencia general función Verifica_existencia_lista_ordenacion_listado_ruta " & ex.Message
        End Try

    End Function
    Function Registra_configuracion_listado_ordenacion_ruta(ByVal id_ruta As Integer, ByVal estado_configuracion As String) As String

        Try
            Dim SqlInsert As String = "Insert into configuracion_listado_ruta_parametros (rutas_workflow_id_Ruta,TIPO_ORDENACION_CAMPO) values (" &
            id_ruta & ",'" & estado_configuracion & "')"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Result As String = ref.SELECTION_INSERT_COMMAND(SqlInsert)
            If Result <> "YES" Then
                Registra_configuracion_listado_ordenacion_ruta = Result
                Exit Function
            Else
                Registra_configuracion_listado_ordenacion_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Registra_configuracion_listado_ordenacion_ruta = "Inconsistencia general funcion Registra_configuracion_listado_ordenacion_ruta " & ex.Message
        End Try
    End Function
    Function Actualiza_configuracion_listado_ordenacion_ruta(ByVal id_ruta As Integer, ByVal estado_configuracion As String) As String

        Try
            Dim SqlInsert As String = "Update configuracion_listado_ruta_parametros set TIPO_ORDENACION_CAMPO='" & estado_configuracion & "'" &
            " WHERE rutas_workflow_id_Ruta=" & id_ruta
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Result As String = ref.SELECTION_INSERT_COMMAND(SqlInsert)
            If Result <> "YES" Then
                Actualiza_configuracion_listado_ordenacion_ruta = Result
                Exit Function
            Else
                Actualiza_configuracion_listado_ordenacion_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_configuracion_listado_ordenacion_ruta = "Inconsistencia general funcion Actualiza_configuracion_listado_ordenacion_ruta " & ex.Message
        End Try
    End Function
    Function Bajar_Indice_campo_lista(ByVal id_campo_indice As Integer, ByVal id_ruta As Integer,
        ByRef ref_Hidden_id_idex_config_siguiente As Object, ByRef ref_Hidden_ide_orden_siguiente As Object,
        ByRef ref_Hidden_id_orden_seleccion As Object, ByRef ref_Hidden_resultado_aprobacion As Object) As String
        Dim Result As String = ""
        Dim Stru_campos() As Estru_campo_indice = Nothing
        '-----------------------------------------------
        'Lista los campos con los indices
        '-----------------------------------------------
        Result = Me.Retorna_listado_campo_indice_ordenacion(id_ruta, Stru_campos)
        If Result <> "YES" Then
            Bajar_Indice_campo_lista = Result
            Exit Function
        End If
        '-----------------------------------------------
        'Restriccion indice campo final de la lista
        '-----------------------------------------------
        If Stru_campos(Stru_campos.Length - 1).id_campo_indice = id_campo_indice Then
            Bajar_Indice_campo_lista = "Imposible bajar al siguiente indice, final de la fila"
            Exit Function
        End If
        '----------------------------------------------------------------
        'Asigna datos a los campos para actualizacion campo seleccionado
        '----------------------------------------------------------------
        Dim Indice_campo_seleccion As Integer = 0
        Dim index_selecion As Integer = -1
        For i As Integer = 0 To Stru_campos.Length - 1
            If id_campo_indice = Stru_campos(i).id_campo_indice Then
                Indice_campo_seleccion = Stru_campos(i).indice_campo
                index_selecion = i
                Exit For
            End If
        Next
        ref_Hidden_id_orden_seleccion.value = Indice_campo_seleccion
        If index_selecion = -1 Then
            Bajar_Indice_campo_lista = "Imposible econtrar el siguiente campo de la fila"
            Exit Function
        End If
        index_selecion = index_selecion + 1
        Dim indice_campo_siguiente As Integer = Stru_campos(index_selecion).indice_campo
        Dim id_indice_campo_siguiente As Integer = Stru_campos(index_selecion).id_campo_indice
        ref_Hidden_id_idex_config_siguiente.value = Stru_campos(index_selecion).id_campo_indice
        ref_Hidden_ide_orden_siguiente.value = Stru_campos(index_selecion).indice_campo
        Dim update_Actualiza_seleccionado As String = "Update CONFIGURACION_LISTADO_RUTA set id_Campo=" & indice_campo_siguiente & " where Id_Configuracion=" & id_campo_indice
        Dim update_actualza_siguiente As String = "Update CONFIGURACION_LISTADO_RUTA set id_Campo=" & Indice_campo_seleccion & " where Id_Configuracion=" & id_indice_campo_siguiente
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans

            myCommand.CommandText = update_Actualiza_seleccionado
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Bajar_Indice_campo_lista = "Actualizar campo indice seleccionado  : "
                errorM = "Imposible archivar expediente  : " & update_Actualiza_seleccionado
                myConnection.Close()
                Exit Function
            End If

            myCommand.CommandText = update_actualza_siguiente
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible actualizar indice siguiente  : " & update_actualza_siguiente
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If

            myTrans.Commit()
            myConnection.Close()
            Bajar_Indice_campo_lista = errorM
            ref_Hidden_resultado_aprobacion.value = errorM
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myTrans.Rollback()
                myConnection.Close()
                Bajar_Indice_campo_lista = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Bajar_Indice_campo_lista = errorM

        End Try
    End Function
    Function Subir_Indice_campo_lista(ByVal id_campo_indice As Integer, ByVal id_ruta As Integer,
        ByRef ref_Hidden_id_idex_config_siguiente As Object, ByRef ref_Hidden_ide_orden_siguiente As Object,
        ByRef ref_Hidden_id_orden_seleccion As Object, ByRef ref_Hidden_resultado_aprobacion As Object) As String
        Dim Result As String = ""
        Dim Stru_campos() As Estru_campo_indice = Nothing
        '-----------------------------------------------
        'Lista los campos con los indices
        '-----------------------------------------------
        Result = Me.Retorna_listado_campo_indice_ordenacion(id_ruta, Stru_campos)
        If Result <> "YES" Then
            Subir_Indice_campo_lista = Result
            Exit Function
        End If
        '-----------------------------------------------
        'Restriccion indice campo final de la lista
        '-----------------------------------------------
        If Stru_campos(0).id_campo_indice = id_campo_indice Then
            Subir_Indice_campo_lista = "Imposible subir al siguiente indice, inicio de fila"
            Exit Function
        End If
        '----------------------------------------------------------------
        'Asigna datos a los campos para actualizacion campo seleccionado
        '----------------------------------------------------------------
        Dim Indice_campo_seleccion As Integer = 0
        Dim index_selecion As Integer = -1
        For i As Integer = 0 To Stru_campos.Length - 1
            If id_campo_indice = Stru_campos(i).id_campo_indice Then
                Indice_campo_seleccion = Stru_campos(i).indice_campo
                index_selecion = i
                Exit For
            End If
        Next
        ref_Hidden_id_orden_seleccion.value = Indice_campo_seleccion
        If index_selecion = -1 Then
            Subir_Indice_campo_lista = "Imposible econtrar el siguiente campo de la fila"
            Exit Function
        End If
        index_selecion = index_selecion - 1
        Dim indice_campo_anterior As Integer = Stru_campos(index_selecion).indice_campo
        Dim id_indice_campo_anterior As Integer = Stru_campos(index_selecion).id_campo_indice
        ref_Hidden_id_idex_config_siguiente.value = Stru_campos(index_selecion).id_campo_indice
        ref_Hidden_ide_orden_siguiente.value = Stru_campos(index_selecion).indice_campo
        Dim update_Actualiza_seleccionado As String = "Update CONFIGURACION_LISTADO_RUTA set id_Campo=" & indice_campo_anterior & " where Id_Configuracion=" & id_campo_indice
        Dim update_actualza_siguiente As String = "Update CONFIGURACION_LISTADO_RUTA set id_Campo=" & Indice_campo_seleccion & " where Id_Configuracion=" & id_indice_campo_anterior
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans

            myCommand.CommandText = update_Actualiza_seleccionado
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Subir_Indice_campo_lista = "Actualizar campo indice seleccionado  : "
                errorM = "Imposible archivar expediente  : " & update_Actualiza_seleccionado
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_actualza_siguiente
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible actualizar indice siguiente  : " & update_actualza_siguiente
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Subir_Indice_campo_lista = errorM
            ref_Hidden_resultado_aprobacion.value = errorM
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myTrans.Rollback()
                myConnection.Close()
                Subir_Indice_campo_lista = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Subir_Indice_campo_lista = errorM

        End Try
    End Function
    Function Retorna_listado_campo_indice_ordenacion(ByVal id_ruta As Integer, ByRef stru_Campo() As Estru_campo_indice) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_LISTADO_RUTA")
            Dim Sql_consulta As String = "Select Id_Configuracion,id_Campo,Nombre_Campo from CONFIGURACION_LISTADO_RUTA WHERE RUTAS_WORKFLOW_ID_RUTA = '" & id_ruta & "' order by id_Campo"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_listado_campo_indice_ordenacion = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_listado_campo_indice_ordenacion = "Imposible encontrar campos asociados a la ruta"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    If Datset.Tables(0).Rows(i).IsNull(1) Then
                        Retorna_listado_campo_indice_ordenacion = "El campo " & Datset.Tables(0).Rows(i).Item(1) & " no tiene un indice para el orden consulte al administrador para agregar este indice "
                        Exit Function
                    End If
                    ReDim Preserve stru_Campo(i)
                    stru_Campo(i).id_campo_indice = Datset.Tables(0).Rows(i).Item(0)
                    stru_Campo(i).indice_campo = Datset.Tables(0).Rows(i).Item(1)
                Next
                Retorna_listado_campo_indice_ordenacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_listado_campo_indice_ordenacion = "Inconsistencia general funcion Retorna_listado_campo_indice_ordenacion " & ex.Message
        End Try

    End Function
    Function Retorna_valor_indice_ultimo_campo_ordenacion(ByVal id_ruta As Integer, ByRef id_campo As Integer) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_LISTADO_RUTA")
            Dim Sql_consulta As String = "Select id_Campo from CONFIGURACION_LISTADO_RUTA WHERE RUTAS_WORKFLOW_ID_RUTA = '" & id_ruta & "' order by id_Campo desc limit 1"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_valor_indice_ultimo_campo_ordenacion = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_campo = 0
                Retorna_valor_indice_ultimo_campo_ordenacion = "YES"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    Retorna_valor_indice_ultimo_campo_ordenacion = "El ultimo campo de la lista tiene indice null "
                    Exit Function
                Else
                    id_campo = Datset.Tables(0).Rows(0).Item(0)
                    Retorna_valor_indice_ultimo_campo_ordenacion = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Retorna_valor_indice_ultimo_campo_ordenacion = "Inconsistencia general funcion Retorna_listado_campo_indice_ordenacion " & ex.Message
        End Try

    End Function
    Function Verifica_Nombre_Gabinete(ByVal Nombre_Gabinete As String, ByRef estado_existencia_gabinete As String) As String
        '------------------------------------------------------------------
        'Fución : Verifica la existencia del gabinete en la configuración
        'de gabinetes de workflow
        'Fecha : 2017-07-20
        'Ing : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Dim Sql_consulta As String = "Select * from CONFIGURACION_GABINETE WHERE NOMBRE_GABINETE = '" & Nombre_Gabinete & "'"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Verifica_Nombre_Gabinete = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_existencia_gabinete = "NO"
                Verifica_Nombre_Gabinete = "YES"
                Exit Function
            Else
                estado_existencia_gabinete = "YES"
                Verifica_Nombre_Gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_Nombre_Gabinete = "Inconsistencia general función Verifica_Nombre_Gabinete " & ex.Message
        End Try
    End Function
    Function Agregar_nuevo_gabinete_configuracion_workflow(ByVal nombre_gabinete As String, ByRef ruta_fisica As String,
        ByRef ruta_busqueda As String, ByRef ruta_almacena As String, ByVal base_datos As String, ByVal dbms As String,
        ByVal unc_servidor As String, ByVal usuario_base_datos As String, ByVal pasword As String) As String
        '--------------------------------------------------------------------
        'Función : Agrega nuevo gabinete a la configuración de workflow con 
        'todos los datos de configuración necesarios
        'Ingeniero : Miguel Angel Urueta Miranda
        'Fecha : 2017-07-20
        '---------------------------------------------------------------------
        Try
            If nombre_gabinete = "" Then
                Agregar_nuevo_gabinete_configuracion_workflow = "Por favor seleccione el nombre del gabinete "
                Exit Function
            End If
            If ruta_fisica = "" Then
                Agregar_nuevo_gabinete_configuracion_workflow = "Por favor digite la ruta fisica del gabinete "
                Exit Function
            End If
            If ruta_busqueda = "" Then
                Agregar_nuevo_gabinete_configuracion_workflow = "Por favor digite la ruta de busqueda del gabinete "
                Exit Function
            End If
            If ruta_almacena = "" Then
                Agregar_nuevo_gabinete_configuracion_workflow = "Por favor digite la ruta de almacenamiento del gabinete "
                Exit Function
            End If
            If base_datos = "" Then
                Agregar_nuevo_gabinete_configuracion_workflow = "Por favor seleccione la base de datos del gabinete "
                Exit Function
            End If
            If dbms = "" Then
                Agregar_nuevo_gabinete_configuracion_workflow = "Por favor seleccione el motor de base de datos del gabinete "
                Exit Function
            End If
            If unc_servidor = "" Then
                Agregar_nuevo_gabinete_configuracion_workflow = "Por favor digite la dirección del servidor del gabinete "
                Exit Function
            End If
            If usuario_base_datos = "" Then
                Agregar_nuevo_gabinete_configuracion_workflow = "Por favor digite el usuario de base de datos "
                Exit Function
            End If
            If pasword = "" Then
                Agregar_nuevo_gabinete_configuracion_workflow = "Por favor digite el pasword de base de datos "
                Exit Function
            End If
            ruta_busqueda = ruta_busqueda.Replace("\", "/")
            ruta_almacena = ruta_almacena.Replace("\", "/")
            ruta_fisica = ruta_fisica.Replace("\", "/")
            Dim Existencia As String = "YES"
            Dim Result As String = ""
            Result = Me.Verifica_Nombre_Gabinete(nombre_gabinete, Existencia)
            If Result <> "YES" Then
                Agregar_nuevo_gabinete_configuracion_workflow = Result
                Exit Function
            End If
            If Existencia = "YES" Then
                Agregar_nuevo_gabinete_configuracion_workflow = "El gabinete que esta tratando de agregar ya existe en la configuración"
                Exit Function
            End If
            Dim SqlInsert As String = "Insert Into CONFIGURACION_GABINETE(NOMBRE_GABINETE," _
                        & "RUTA_FISICA_GABINETE,RUTA_ALMACENA_IMAGEN,RUTA_BUSQUEDA_IMAGEN,BASE_DATOS," _
                        & " MOTOR_BASE,ODBC_BASE,USUARIO_BASE,PASWORD_BASE)VALUES ('" _
                        & nombre_gabinete & "','" & ruta_fisica & "','" & ruta_almacena & "','" _
                        & ruta_busqueda & "','" & base_datos & "','" & dbms & "','" _
                        & unc_servidor & "','" & usuario_base_datos & "','" _
                        & pasword & "' )"
            Dim ref As New conect.Dbase_Conction_Mysql
            Result = ref.SELECTION_INSERT_COMMAND(SqlInsert)
            If Result <> "YES" Then
                Agregar_nuevo_gabinete_configuracion_workflow = Result
                Exit Function
            Else
                Agregar_nuevo_gabinete_configuracion_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Agregar_nuevo_gabinete_configuracion_workflow = "Inconsistencia general función Agregar_nuevo_gabinete_configuracion_workflow " & ex.Message
        End Try
    End Function
    Function Retorna_gabinetes_disponibles_configuracion(
     ByRef Matri_Datos() As String) As String
        '--------------------------------------------------------------
        'Funcion : Solicita los datos de los gabinetes disponibles
        'en configuración
        'Fecha : 2017-07-22
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  Nombre_Gabinete  from  configuracion_gabinete "
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("configuracion_gabinete")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_gabinetes_disponibles_configuracion = " La funcion Retorna_gabinetes_disponibles_configuracion dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_gabinetes_disponibles_configuracion = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Matri_Datos(i)
                    Matri_Datos(i) = Datset.Tables(0).Rows(i).Item(0)
                Next
                Retorna_gabinetes_disponibles_configuracion = "YES"
            End If

        Catch ex As Exception
            Retorna_gabinetes_disponibles_configuracion = "Inconsistencia función  Retorna_gabinetes_disponibles_configuracion " & ex.Message
        End Try
    End Function
    Function Asigna_gabinetes_disponibles_interface_configuracion_droplist(ByVal matri_gabinetes() As String, ByRef ref_droplist As DropDownList, ByRef ref_update As UpdatePanel) As String
        '-----------------------------------------------------------
        'Función : Agrega a la interface items con los gabinetes
        'disponibles
        'Fecha : 2017-07-22
        'Ing Miguel Angel Urueta Miranda
        '-----------------------------------------------------------
        Try
            ref_droplist.Items.Clear()
            ref_droplist.Items.Add("")
            If Not matri_gabinetes Is Nothing Then
                For i As Integer = 0 To matri_gabinetes.Length - 1
                    ref_droplist.Items.Add(matri_gabinetes(i))
                Next
            End If
            ref_update.Update()
            Asigna_gabinetes_disponibles_interface_configuracion_droplist = "YES"
        Catch ex As Exception
            Asigna_gabinetes_disponibles_interface_configuracion_droplist = "Inconsistencia general función Asigna_gabinetes_disponibles_interface_configuracion_droplist " & ex.Message
        End Try
    End Function
    Function Lista_datos_configuracion_gabinete_seleccionado(ByVal nombre_gabinete As String, ByRef ruta_fisica As String,
        ByRef ruta_busqueda As String, ByRef ruta_almacena As String, ByRef ref_DropDownList_base_datos_gabinete_edita As DropDownList,
        ByRef DropDownList_dbms_gabinete_edita As DropDownList, ByRef unc_servidor As String, ByRef usuario_bs As String,
        ByRef pasword_bs As String, ByRef ref_update As UpdatePanel) As String
        '--------------------------------------------------------------
        'Función : Solicita los datos de configuración del gabinete
        'Fecha : 2017-07-20
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------
        ref_DropDownList_base_datos_gabinete_edita.Items.Clear()
        ref_DropDownList_base_datos_gabinete_edita.Items.Add("DOCUARCHI")
        DropDownList_dbms_gabinete_edita.Items.Clear()
        DropDownList_dbms_gabinete_edita.Items.Add("MYSQL")
        Try
            Dim Parametro_Consulta As String = "select Ruta_Busqueda_Imagen,Ruta_Almacena_Imagen,Ruta_Fisica_Gabinete,Base_Datos,Motor_Base,Odbc_base,Usuario_Base,Pasword_Base from  configuracion_gabinete " &
                  " where Nombre_Gabinete='" & nombre_gabinete & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("configuracion_gabinete")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_datos_configuracion_gabinete_seleccionado = " La funcion Lista_datos_configuracion_gabinete_seleccionado dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_datos_configuracion_gabinete_seleccionado = "Imposible encontrar datos de configuración del gabinete " & nombre_gabinete
                Exit Function
            Else
                ruta_busqueda = Datset.Tables(0).Rows(0).Item(0)
                ruta_busqueda = ruta_busqueda.Replace("/", "\")
                ruta_almacena = Datset.Tables(0).Rows(0).Item(1)
                ruta_almacena = ruta_almacena.Replace("/", "\")
                ruta_fisica = Datset.Tables(0).Rows(0).Item(2)
                ruta_fisica = ruta_fisica.Replace("/", "\")
                unc_servidor = Datset.Tables(0).Rows(0).Item(5)
                usuario_bs = Datset.Tables(0).Rows(0).Item(6)
                pasword_bs = Datset.Tables(0).Rows(0).Item(7)
                Lista_datos_configuracion_gabinete_seleccionado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_datos_configuracion_gabinete_seleccionado = "Inconsistencia función Lista_datos_configuracion_gabinete_seleccionado " & ex.Message
        Finally
            ref_update.Update()
        End Try
    End Function
    Function Actualiza_gabinete_configuracion_workflow(ByVal nombre_gabinete As String, ByRef ruta_fisica As String,
        ByRef ruta_busqueda As String, ByRef ruta_almacena As String, ByVal base_datos As String, ByVal dbms As String,
        ByVal unc_servidor As String, ByVal usuario_base_datos As String, ByVal pasword As String) As String
        '--------------------------------------------------------------------
        'Función : Actualiza la configuración de workflow con 
        'todos los datos de configuración necesarios
        'Ingeniero : Miguel Angel Urueta Miranda
        'Fecha : 2017-07-20
        '---------------------------------------------------------------------
        Try
            If nombre_gabinete = "" Then
                Actualiza_gabinete_configuracion_workflow = "Por favor seleccione el nombre del gabinete "
                Exit Function
            End If
            If ruta_fisica = "" Then
                Actualiza_gabinete_configuracion_workflow = "Por favor digite la ruta fisica del gabinete "
                Exit Function
            End If
            If ruta_busqueda = "" Then
                Actualiza_gabinete_configuracion_workflow = "Por favor digite la ruta de busqueda del gabinete "
                Exit Function
            End If
            If ruta_almacena = "" Then
                Actualiza_gabinete_configuracion_workflow = "Por favor digite la ruta de almacenamiento del gabinete "
                Exit Function
            End If
            If base_datos = "" Then
                Actualiza_gabinete_configuracion_workflow = "Por favor seleccione la base de datos del gabinete "
                Exit Function
            End If
            If dbms = "" Then
                Actualiza_gabinete_configuracion_workflow = "Por favor seleccione el motor de base de datos del gabinete "
                Exit Function
            End If
            If unc_servidor = "" Then
                Actualiza_gabinete_configuracion_workflow = "Por favor digite la dirección del servidor del gabinete "
                Exit Function
            End If
            If usuario_base_datos = "" Then
                Actualiza_gabinete_configuracion_workflow = "Por favor digite el usuario de base de datos "
                Exit Function
            End If
            If pasword = "" Then
                Actualiza_gabinete_configuracion_workflow = "Por favor digite el pasword de base de datos "
                Exit Function
            End If
            ruta_busqueda = ruta_busqueda.Replace("\", "/")
            ruta_almacena = ruta_almacena.Replace("\", "/")
            ruta_fisica = ruta_fisica.Replace("\", "/")
            Dim Existencia As String = "YES"
            Dim Result As String = ""
            Dim SqlInsert As String = "Update CONFIGURACION_GABINETE set RUTA_FISICA_GABINETE='" & ruta_fisica & "'" &
            ",RUTA_ALMACENA_IMAGEN='" & ruta_almacena & "',RUTA_BUSQUEDA_IMAGEN='" & ruta_busqueda & "'" &
            ",BASE_DATOS='" & base_datos & "',MOTOR_BASE='" & dbms & "',ODBC_BASE='" & unc_servidor & "'" &
            ",USUARIO_BASE='" & usuario_base_datos & "',PASWORD_BASE='" & pasword & "' where NOMBRE_GABINETE='" & nombre_gabinete & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Result = ref.SELECTION_INSERT_COMMAND(SqlInsert)
            If Result <> "YES" Then
                Actualiza_gabinete_configuracion_workflow = Result
                Exit Function
            Else
                Actualiza_gabinete_configuracion_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_gabinete_configuracion_workflow = "Inconsistencia general función Actualiza_gabinete_configuracion_workflow " & ex.Message
        End Try
    End Function
    Function Inicializa_datos_configuracion_interface(ByRef ruta_fisica As String,
       ByRef ruta_busqueda As String, ByRef ruta_almacena As String, ByRef ref_DropDownList_base_datos_gabinete_edita As DropDownList,
       ByRef DropDownList_dbms_gabinete_edita As DropDownList, ByRef unc_servidor As String, ByRef usuario_bs As String,
       ByRef pasword_bs As String, ByRef ref_update As UpdatePanel) As String
        '----------------------------------------------------------------
        'Función : Inicializa_datos_configuracion_interface
        'Ingeniero : Miguel Angel Urueta Miranda
        'Fecha : 2017-07-22
        '----------------------------------------------------------------
        Try
            ref_DropDownList_base_datos_gabinete_edita.Items.Clear()
            ref_DropDownList_base_datos_gabinete_edita.Items.Add("DOCUARCHI")
            DropDownList_dbms_gabinete_edita.Items.Clear()
            DropDownList_dbms_gabinete_edita.Items.Add("MYSQL")
            unc_servidor = ""
            usuario_bs = ""
            pasword_bs = ""
            ruta_fisica = ""
            ruta_busqueda = ""
            ruta_almacena = ""
            ref_update.Update()
            Inicializa_datos_configuracion_interface = "YES"
        Catch ex As Exception
            Inicializa_datos_configuracion_interface = "Inconsistencia general fución Inicializa_datos_configuracion_interface " & ex.Message
        End Try
    End Function
    Function Consulta_valor_Script(ByVal Nombre_Actividad As String, ByVal nombre_evento As String, ByVal id_ruta As Integer,
                                   ByRef valor_escript As String, ByRef estado_existencia As String) As String
        '***********************************************************
        'Nombre Funcion  : Consulta_Script 
        'Ing de Programa : Miguel Angel Urueta Miranda
        'Fecha           : 2009-05-04
        'Descripcion     : El sistema consulta de la base
        'de datos script_actividades con el los parametors
        'nombre de script y nombre actvidad, la funcion devuelbe
        'el codigo del script
        'Modificado : So modifica para desarrollo web el medotod de
        'conexión a base de datos y se agrega un nuevo parametro
        'de identificación de la actividad
        'Fecha : 2017-07-24
        'Ingeniero :Miguel Angel Urueta Miranda
        '***********************************************************
        Try
            Dim Parametro_Consulta As String = " SELECT SA.VALOR_SCRIPT FROM LISTADO_ACTIVIDADES_WORKFLOW as SC " _
            & "inner JOIN SCRIPT_ACTIVIDADES AS SA ON " _
            & "(SA.listado_Actividades_Workflow_Id_Actividad= " _
            & "SC.ID_ACTIVIDAD AND SA.NOMBRE_EVENTO= '" & nombre_evento & "')" _
            & " WHERE SC.NOMBRE_ACTIVIDAD='" & Nombre_Actividad & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("system1")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Consulta_valor_Script = " La función Consulta_valor_Script dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Consulta_valor_Script = "YES"
                estado_existencia = "NO"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    valor_escript = ""
                Else
                    valor_escript = Datset.Tables(0).Rows(0).Item(0)
                End If
                estado_existencia = "YES"
                Consulta_valor_Script = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Consulta_valor_Script = "Inconsistencia general función Consulta_valor_Script " & ex.Message
        End Try
    End Function
    Function Retorna_id_actividad_general_ruta(ByVal nombre_actividad As String, ByVal id_ruta As Integer, ByRef id_actividad_general As Integer,
                                               ByRef tipo_actividad As String) As String
        '-------------------------------------------------------
        'Función : Retorna el id de actividad general con el 
        'nombre de la actividad y la ruta
        'Fecha : 2017-04-24
        'Ing : Miguel Angel Urueta Miranda
        '-------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "SELECT LAW.ID_ACTIVIDAD,AG.ID_ACTIVIDAD_GENERAL,AG.Tipo_Actividad " _
           & "FROM LISTADO_ACTIVIDADES_WORKFLOW AS LAW " _
           & "INNER JOIN  ACTIVIDADES_GENERALES_WORKFLOW  AS AG ON " _
           & "(AG.ID_ACTIVIDAD_GENERAL = LAW.ACTIVIDADES_GENERALES_WORKFLOW_ID_ACTIVIDAD_GENERAL)" _
           & " where NOMBRE_ACTIVIDAD=" & "'" & nombre_actividad & "' and Rutas_Workflow_id_Ruta=" & id_ruta
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("LISTADO_ACTIVIDADES_WORKFLOW")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_actividad_general_ruta = "La funcion Retorna_id_actividad_general_ruta dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_actividad_general_ruta = "Imposible encontrar el id actividad general de la actividad " & nombre_actividad
                Exit Function
            Else
                id_actividad_general = Datset.Tables(0).Rows(0).Item(1)
                tipo_actividad = Datset.Tables(0).Rows(0).Item(2)
                Retorna_id_actividad_general_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_actividad_general_ruta = "Inconsisrencia general función Retorna_id_actividad_general_ruta " & ex.Message
        End Try

    End Function
    Function Crea_evento_actividad_individual(ByVal Id_Actividad As Integer, ByVal Id_Actividad_General As Integer, ByVal nombre_evento As String) As String
        '----------------------------------------------------
        'Funcion : Crear evento individual para la actividad
        'Fecha : 2017-04-27
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------------------------------------------------
        Try
            Dim SqlInsert = "insert into SCRIPT_ACTIVIDADES(LISTADO_ACTIVIDADES_WORKFLOW_ID_ACTIVIDAD," _
                   & "ACTIVIDADES_GENERALES_WORKFLOW_ID_ACTIVIDAD_GENERAL,NOMBRE_EVENTO,ESTADO_SCRIPT) VALUES " _
                   & "(" & Id_Actividad & "," & Id_Actividad_General & ",'" & nombre_evento & "'," & 1 & ")"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Result As String = ref.SELECTION_INSERT_COMMAND(SqlInsert)
            If Result <> "YES" Then
                Crea_evento_actividad_individual = Result
                Exit Function
            Else
                Crea_evento_actividad_individual = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Crea_evento_actividad_individual = "Inconsistencia general función Crea_evento_actividad_individual " & ex.Message
        End Try
    End Function
    Function Crear_conexion_actividades_workflow(
                                                 ByVal nombre_ruta As String, ByRef DiagramView As MindFusion.Diagramming.WebForms.DiagramView,
                                                 ByRef UpdatePanel_diagran_view As UpdatePanel) As String
        Dim HiddenField_value_selecion As Object = Nothing
        HiddenField_value_selecion = UpdatePanel_diagran_view.FindControl("HiddenField_value_selecion")
        If HiddenField_value_selecion Is Nothing Then
            Crear_conexion_actividades_workflow = "Imposible encontrar el control selección HiddenField_value_selecion"
            Exit Function
        End If
        Dim id_actividad_fuente As Integer = 0
        Dim id_actividad_destino As Integer = 0
        Dim Result As String = ""
        Dim Refclas_workflow As New ClassWorkflow
        Dim Ref_class_wf_ruta As New Class_worflow_rutas
        Dim id_ruta As Integer = 0
        Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(nombre_ruta,
                                                            id_ruta)
        If Result <> "YES" Then
            Crear_conexion_actividades_workflow = Result
            Exit Function
        End If
        If id_ruta = 0 Then
            Crear_conexion_actividades_workflow = "Imposible encontrar el id de la ruta " & nombre_ruta
            Exit Function
        End If
        Dim sha As Object = Nothing
        Dim Matshape() As Object
        Erase Matshape
        If HiddenField_value_selecion.Value = "" Then
            Crear_conexion_actividades_workflow = "Debe seleccionar dos actividades en el diagrama como mínimo para conectar "
            Exit Function
        End If
        If DiagramView.Diagram.Selection.Items.Count = 0 Then
            Crear_conexion_actividades_workflow = "Debe seleccionar dos actividades en el diagrama como mínimo para conectar "
            Exit Function
        End If
        If DiagramView.Diagram.Selection.Items.Count = 1 Then
            Crear_conexion_actividades_workflow = "Debe seleccionar dos actividades en el diagrama como mínimo para conectar "
            Exit Function
        End If
        Dim split() As String = HiddenField_value_selecion.Value.Split("|")
        For i As Integer = 0 To split.Length - 1
            If split(0) = split(1) Then
                Crear_conexion_actividades_workflow = "La actividad destino no puede ser la misma actividad de inicio "
                Exit Function
            End If
            For Each sha_ As Object In DiagramView.Diagram.Selection.Items
                Dim ob As Object = sha_.GetType
                If ob.Fullname = "MindFusion.Diagramming.ShapeNode" Then
                    If sha_.id = split(i) Then
                        ReDim Preserve Matshape(i)
                        Matshape(i) = sha_
                    End If
                End If

            Next
        Next
        If Matshape Is Nothing Then
            Crear_conexion_actividades_workflow = "Debe seleccionar dos actividades como mínimo para conectar "
            Exit Function
        End If
        If Matshape.Length = 1 Then
            Crear_conexion_actividades_workflow = "Debe seleccionar dos actividades como mínimo para conectar "
            Exit Function
        End If
        If Matshape.Length > 2 Then
            Crear_conexion_actividades_workflow = "Solo  debe seleccionar dos actividades al mismo tiempo para conectar "
            Exit Function
        End If
        id_actividad_destino = Val(Matshape(1).id)
        id_actividad_fuente = Val(Matshape(0).id)
        '-------------------------------------
        'Verfica la existencia conector
        '-------------------------------------
        Result = Me.Verifica_existencia_conector_actividades(id_actividad_fuente, id_actividad_destino, id_ruta)
        If Result <> "YES" Then
            Crear_conexion_actividades_workflow = Result
            Exit Function
        End If
        Dim Sql_Insercion As String = "insert into ACTIVIDADES_DISPONIBLES_ENVIO (ID_ACTIVIDAD_SIGUIENTE," _
           & "Listado_Actividades_Workflow_Id_Actividad,Ienti_Grafica_Actividad,id_ruta) values " _
           & "(" & id_actividad_destino & "," & id_actividad_fuente & "," _
           & 0 & "," & id_ruta & " )"
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        ref.Returna_Conexion_Mysql(myConnection)
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = Sql_Insercion
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Crear_conexion_actividades_workflow = "Imposible crear conector de actividad  "
                myConnection.Close()
                Exit Function
            End If
            Dim ident = myCommand.LastInsertedId
            Dim link As MindFusion.Diagramming.DiagramLink
            link = DiagramView.Diagram.Factory.CreateDiagramLink(Matshape(0), Matshape(1))
            'link.Text = Matshape(0).Text & "->" & Matshape(1).text
            link.AutoRoute = True
            link.AutoSnapToNode = False
            'EVITA QUE SE MUEVA EL CONECTOR FINAL
            link.AllowMoveEnd = False
            link.AllowMoveStart = False
            link.DrawCrossings = False
            link.CrossingRadius = 1
            'link.DrawCrossings = True
            link.Id = id_ruta.ToString & "_" & ident
            Dim Fill = New MindFusion.Drawing.SolidBrush(Color.Yellow)
            link.HeadBrush = Fill
            Dim Ruta_archivo_guardado As String = ""
            '--------------------------------------------------
            'Guarda el archivo en el sistema de archivo
            '--------------------------------------------------
            'Result = Salva_archivo_file_sistem(DiagramView, Ruta_archivo_guardado, nombre_ruta)
            'If Result <> "YES" Then
            '    HiddenField_value_selecion.Value = ""
            '    DiagramView.Diagram.Selection.Clear()
            '    If DiagramView.Diagram.UndoManager.History.Commands.Count > 0 Then
            '        DiagramView.Diagram.UndoManager.Undo()
            '    End If
            '    UpdatePanel_diagran_view.Update()
            '    Crear_conexion_actividades_workflow = Result
            '    Exit Function
            'End If
            Dim string_plantilla As String = ""
            string_plantilla = DiagramView.SaveToString(SaveToStringFormat.Base64, True)
            '-------------------------------------------------
            'Guarda el archivo en la base de datos
            '-------------------------------------------------
            Result = Guarda_archivo_base_datos_string(string_plantilla, id_ruta)
            If Result <> "YES" Then
                HiddenField_value_selecion.Value = ""
                DiagramView.Diagram.Selection.Clear()
                If DiagramView.Diagram.UndoManager.History.Commands.Count > 0 Then
                    DiagramView.Diagram.UndoManager.Undo()
                End If
                UpdatePanel_diagran_view.Update()
                Crear_conexion_actividades_workflow = Result
                Exit Function
            End If
            DiagramView.Diagram.Selection.Clear()
            HiddenField_value_selecion.Value = ""
            UpdatePanel_diagran_view.Update()
            myTrans.Commit()
            myConnection.Close()
            Crear_conexion_actividades_workflow = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Crear_conexion_actividades_workflow = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                End If
            End Try
            myConnection.Close()
            Crear_conexion_actividades_workflow = "Error General " & e.Message
            Exit Function



        End Try
    End Function
    Function Verifica_existencia_conector_actividades(ByVal Id_Actividad_fuente As Integer, ByVal Id_Actividad_destino As Integer, ByVal id_ruta As Integer) As String
        '-------------------------------------------------------
        'Función : Verifica la existencia de conectores entre
        'actividades
        'Fecha : 2017-08-03
        'Ingeniero : Miguel Angel Urueta Miranda
        '-------------------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ACTIVIDADES_DISPONIBLES_ENVIO")
            Dim Sql_consulta As String = "Select * from ACTIVIDADES_DISPONIBLES_ENVIO " _
            & " where ID_ACTIVIDAD_SIGUIENTE= " & Id_Actividad_destino & " and Listado_Actividades_Workflow_Id_Actividad = " & Id_Actividad_fuente &
            " and id_Ruta=" & id_ruta
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_conector_actividades = "Función Verifica_existencia_conector_actividades dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Verifica_existencia_conector_actividades = "YES"
                Exit Function
            Else
                Verifica_existencia_conector_actividades = "El conector de actividades ya se encuentra registrado, imposible conectar las actividades"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_conector_actividades = "Inconsistencia general función Verifica_existencia_conector_actividades " & ex.Message
        End Try
    End Function
    Function Retorna_grupos_disponibles_worokflow_actividad(ByVal id_ruta As Integer, ByRef ref_droplist As DropDownList, ByRef ref_update_panel As UpdatePanel) As String
        '-------------------------------------------------------
        'Función : Retorna grupos workflow disponibles para
        'realcionar con la actividad
        'Fecha : 2017-08-04
        'Ingeniero : Miguel Angel Urueta Miranda
        '-------------------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("grupos_workflow")
            Dim Sql_consulta As String = "Select Nombre_Grupo from grupos_workflow " _
            & " where id_Actividad is null " &
            " and Rutas_Workflow_id_Ruta=" & id_ruta
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_grupos_disponibles_worokflow_actividad = "Función Retorna_grupos_disponibles_worokflow_actividad dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                ref_droplist.Items.Clear()
                ref_update_panel.Update()
                Retorna_grupos_disponibles_worokflow_actividad = "YES"
                Exit Function
            Else
                ref_droplist.Items.Clear()
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ref_droplist.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                ref_update_panel.Update()
                Retorna_grupos_disponibles_worokflow_actividad = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_grupos_disponibles_worokflow_actividad = "Inconsistencia general función Retorna_grupos_disponibles_worokflow_actividad " & ex.Message
        End Try
    End Function
    Function Retorna_Grupos_Workflow_relacionados_actividad(ByVal pag As Page, ByVal id_ruta As Integer, ByVal id_actividad As Integer,
                                                            ByRef ref_droplist As DropDownList, ByRef ref_updatepanel As UpdatePanel) As String
        '-------------------------------------------------------
        'Función : Retorna grupos de workflow relacionado
        'a la ruta.
        'Fecha : 2017-08-05
        'Ingeniero : Miguel Angel Urueta Miranda
        '-------------------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("grupos_workflow")
            Dim Sql_consulta As String = "Select Nombre_Grupo from grupos_workflow " _
            & " where id_Actividad=" & id_actividad &
            " and Rutas_Workflow_id_Ruta=" & id_ruta
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_Grupos_Workflow_relacionados_actividad = "Función Retorna_Grupos_Workflow_relacionados_actividad dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                ref_droplist.Items.Clear()
                ref_updatepanel.Update()
                Retorna_Grupos_Workflow_relacionados_actividad = "YES"
                Exit Function
            Else
                ref_droplist.Items.Clear()
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ref_droplist.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                ref_updatepanel.Update()
                Retorna_Grupos_Workflow_relacionados_actividad = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_Grupos_Workflow_relacionados_actividad = "Inconsistencia general función Retorna_Grupos_Workflow_relacionados_actividad " & ex.Message
        End Try
    End Function
    Function Retorna_id_grupo_workflow(ByVal nombre_grupo As String, ByVal id_ruta As Integer, ByRef id_grupo As Integer) As String
        '-------------------------------------------------------
        'Función : Retorna id del grupo workflow
        'con el nombre del grupo y el is de la ruta
        'Fecha : 2017-08-04
        'Ingeniero : Miguel Angel Urueta Miranda
        '-------------------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("grupos_workflow")
            Dim Sql_consulta As String = "Select Id_Grupo from grupos_workflow " _
            & " where Nombre_Grupo='" & nombre_grupo & "'" &
            " and Rutas_Workflow_id_Ruta=" & id_ruta
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_grupo_workflow = "Función Retorna_id_grupo_workflow dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_grupo_workflow = "YES"
                Exit Function
            Else
                id_grupo = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_grupo_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_grupo_workflow = "Inconsistencia general función Retorna_id_grupo_workflow " & ex.Message
        End Try
    End Function
    Function Retorna_id_actividad_interface(ByRef pag As Page, ByRef id_actividad As Integer) As String
        '------------------------------------------------------------------------
        'Función : Retorna id actividad seleccionada en la interface
        'Fecha : 201-08-05
        'Ingeniero : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------
        Try
            Dim diagramView As MindFusion.Diagramming.WebForms.DiagramView = pag.FindControl("diagramView")
            If diagramView.Diagram.Selection.Items.Count = 0 Then
                Retorna_id_actividad_interface = "Debe seleccionar la actividad"
                Exit Function
            End If
            If diagramView.Diagram.Selection.Items.Count > 1 Then
                Retorna_id_actividad_interface = "Por favor seleccione una sola actividad"
                Exit Function
            End If
            Dim obshape As Object = diagramView.Diagram.Selection.Items(0)
            Dim ob As Object = obshape.GetType
            If ob.Fullname = "MindFusion.Diagramming.ShapeNode" Then
                id_actividad = obshape.id
                Retorna_id_actividad_interface = "YES"
                Exit Function
            Else
                Retorna_id_actividad_interface = "Debe seleccionar un elemento de actividad"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_actividad_interface = "Inconsistencia general función Retorna_id_actividad_interface " & ex.Message
        End Try

    End Function
    Function Actualiza_relacion_grupo_workflow_actividad(ByRef pag As Page, ByVal nombre_ruta As String, ByVal nombre_grupo As String) As String
        '-------------------------------------------------------------------------
        'Función : Actualiza la relación del grupo workflow con la actividad
        'de workflow
        'Fecha : 2017-08-05
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------
        Try
            Dim diagramView As MindFusion.Diagramming.WebForms.DiagramView = pag.FindControl("diagramView")
            If diagramView.Diagram.Selection.Items.Count = 0 Then
                Actualiza_relacion_grupo_workflow_actividad = "Debe seleccionar la actividad"
                Exit Function
            End If
            If diagramView.Diagram.Selection.Items.Count > 1 Then
                Actualiza_relacion_grupo_workflow_actividad = "Por favor seleccione una sola actividad"
                Exit Function
            End If
            Dim obshape As Object = diagramView.Diagram.Selection.Items(0)
            Dim ob As Object = obshape.GetType
            Dim id_actividad As Integer = 0
            If ob.Fullname = "MindFusion.Diagramming.ShapeNode" Then
                id_actividad = obshape.id
            Else
                Actualiza_relacion_grupo_workflow_actividad = "Debe seleccionar un elemento de actividad"
                Exit Function
            End If
            Dim Result As String = ""
            Dim id_ruta As Integer = 0
            Dim Refclas_workflow As New ClassWorkflow
            Dim Ref_class_wf_ruta As New Class_worflow_rutas
            Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(nombre_ruta,
                                                                id_ruta)
            If Result <> "YES" Then
                Actualiza_relacion_grupo_workflow_actividad = Result
                Exit Function
            End If
            Dim id_grupo As Integer = 0
            Result = Me.Retorna_id_grupo_workflow(nombre_grupo, id_ruta, id_grupo)
            If Result <> "YES" Then
                Actualiza_relacion_grupo_workflow_actividad = Result
                Exit Function
            End If
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim sqlupdate As String = "Update grupos_workflow set id_Actividad=" & id_actividad & " where Id_Grupo=" & id_grupo &
                " and Rutas_Workflow_id_Ruta=" & id_ruta
            Result = ref.SELECTION_INSERT_COMMAND(sqlupdate)
            If Result <> "YES" Then
                Actualiza_relacion_grupo_workflow_actividad = Result
                Exit Function
            Else
                Actualiza_relacion_grupo_workflow_actividad = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_relacion_grupo_workflow_actividad = "Inconsistencia general función Actualiza_relacion_grupo_workflow_actividad " & ex.Message
        End Try
    End Function
    Function Elimina_relacion_grupo_workflow_actividad(ByRef pag As Page, ByVal nombre_ruta As String, ByVal nombre_grupo As String) As String
        '-------------------------------------------------------------------------
        'Función : Elimina la relación del grupo workflow con la actividad
        'de workflow
        'Fecha : 2017-08-05
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------
        Try
            Dim diagramView As MindFusion.Diagramming.WebForms.DiagramView = pag.FindControl("diagramView")
            If diagramView.Diagram.Selection.Items.Count = 0 Then
                Elimina_relacion_grupo_workflow_actividad = "Debe seleccionar la actividad"
                Exit Function
            End If
            If diagramView.Diagram.Selection.Items.Count > 1 Then
                Elimina_relacion_grupo_workflow_actividad = "Por favor seleccione una sola actividad"
                Exit Function
            End If
            Dim obshape As Object = diagramView.Diagram.Selection.Items(0)
            Dim ob As Object = obshape.GetType
            Dim id_actividad As Integer = 0
            If ob.Fullname = "MindFusion.Diagramming.ShapeNode" Then
                id_actividad = obshape.id
            Else
                Elimina_relacion_grupo_workflow_actividad = "Debe seleccionar un elemento de actividad"
                Exit Function
            End If
            Dim Result As String = ""
            Dim id_ruta As Integer = 0
            Dim Refclas_workflow As New ClassWorkflow
            Dim Ref_class_wf_ruta As New Class_worflow_rutas
            Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(nombre_ruta, id_ruta)
            If Result <> "YES" Then
                Elimina_relacion_grupo_workflow_actividad = Result
                Exit Function
            End If
            Dim id_grupo As Integer = 0
            Result = Me.Retorna_id_grupo_workflow(nombre_grupo,
                                                  id_ruta,
                                                  id_grupo)
            If Result <> "YES" Then
                Elimina_relacion_grupo_workflow_actividad = Result
                Exit Function
            End If
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim sqlupdate As String = "Update grupos_workflow set id_Actividad=" & "null" & " where Id_Grupo=" & id_grupo &
                " and Rutas_Workflow_id_Ruta=" & id_ruta
            Result = ref.SELECTION_INSERT_COMMAND(sqlupdate)
            If Result <> "YES" Then
                Elimina_relacion_grupo_workflow_actividad = Result
                Exit Function
            Else
                Elimina_relacion_grupo_workflow_actividad = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Elimina_relacion_grupo_workflow_actividad = "Inconsistencia general función Elimina_relacion_grupo_workflow_actividad " & ex.Message
        End Try
    End Function
    Function Crear_actividad_script_ruta(ByVal id_tipo_actividad As String,
                                         ByVal Nombre_Actividad As String,
                                         ByVal nombre_ruta As String,
                                         ByVal descripcion As String,
        ByRef DiagramView As MindFusion.Diagramming.WebForms.DiagramView, ByRef UpdatePanel_diagran_view As UpdatePanel, ByVal option_crea_asocia_grupo_workflow As Integer) As String
        Dim Result As String = ""
        Dim Id_ruta As Integer = 0
        If Nombre_Actividad = "" Then
            Crear_actividad_script_ruta = "Debe informar el nombre de la nueva actividad"
            Exit Function
        End If
        If descripcion = "" Then
            Crear_actividad_script_ruta = "Por favor informe la descripción de la actividad"
            Exit Function
        End If
        Dim Class_actividades_generales_workflow As New Class_actividades_generales_workflow
        Dim actividades_generales_workflow_ As actividades_generales_workflow = Nothing
        Result = Class_actividades_generales_workflow.Solicita_estructura_tipo_actividad_workflow(id_tipo_actividad,
                                                                                                  actividades_generales_workflow_)
        If Result <> "YES" Then
            Crear_actividad_script_ruta = Result
            Exit Function
        End If
        '--------------------------------------------
        'Verifica la existencia de un grupo workflow
        '--------------------------------------------
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Dim Fecha_Format As String = Now.ToString
        Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(Fecha_Format)
        If Result <> "YES" Then
            Crear_actividad_script_ruta = Result
            Exit Function
        End If
        Dim estado_existencia_grupo As String = "YES"
        If option_crea_asocia_grupo_workflow = 1 Then
            Result = Verifica_existencia_grupo_workflow(Nombre_Actividad, Id_ruta, estado_existencia_grupo)
            If Result <> "YES" Then
                Crear_actividad_script_ruta = Result
                Exit Function
            End If
            If estado_existencia_grupo = "YES" Then
                If Result <> "YES" Then
                    Crear_actividad_script_ruta = "El grupo " & Nombre_Actividad & " ya se encuentra registrado, por favor inactive la opción crear grupo workflow "
                    Exit Function
                End If
            End If
        End If
        '---------------------------------------
        'Retorna nombre id ruta
        '---------------------------------------
        Dim Refclas As New ClassWorkflow
        Dim Ref_class_wf_ruta As New Class_worflow_rutas
        Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(nombre_ruta,
                                                            Id_ruta)
        If Result <> "YES" Then
            Crear_actividad_script_ruta = Result
            Exit Function
        End If
        '---------------------------------------
        'Retorna id tipo de actividad general
        '---------------------------------------
        Dim id_tipo_actividad_general As Integer = actividades_generales_workflow_.Id_Actividad_General
        'Result = Me.Retorna_id_tipo_actividad_general(Tipo_Actividad,
        '                                              id_tipo_actividad_general)
        'If Result <> "YES" Then
        '    Crear_actividad_script_ruta = Result
        '    Exit Function
        'End If
        '------------------------------------------
        'Verifica existencia actividad en la ruta
        '------------------------------------------
        Dim estado_existencia_ruta As String = ""
        Result = Verifica_existencia_actividad_ruta(Nombre_Actividad,
                                                    Id_ruta,
                                                    estado_existencia_ruta)
        If Result <> "YES" Then
            Crear_actividad_script_ruta = Result
            Exit Function
        End If
        If estado_existencia_ruta = "YES" Then
            Crear_actividad_script_ruta = "La actividad " & Nombre_Actividad & " se ecuentra creada en la ruta "
            Exit Function
        End If
        '-----------------------------------------------
        'Solicita matriz de los eventos de la actividad
        '-----------------------------------------------
        Dim matri_eventos() As String = Nothing
        Result = Me.Retorna_Matriz_evento_actividad(actividades_generales_workflow_.Tipo_Actividad,
                                                    matri_eventos)
        If Result <> "YES" Then
            Crear_actividad_script_ruta = Result
            Exit Function
        End If
        If matri_eventos Is Nothing Then
            Crear_actividad_script_ruta = "Imposible recuperar la matriz general de eventos"
            Exit Function
        End If

        Dim ref_descripcion As String = ""
        If descripcion = "" Then
            ref_descripcion = "null"
        Else
            ref_descripcion = "'" & descripcion & "'"
        End If
        Dim Sql_Insercion_actividad As String = "insert into LISTADO_ACTIVIDADES_WORKFLOW (RUTAS_WORKFLOW_ID_RUTA," _
            & "ACTIVIDADES_GENERALES_WORKFLOW_ID_ACTIVIDAD_GENERAL,NOMBRE_ACTIVIDAD,DESCRIPCION_ACTIVIDAD" _
            & ",TIEMPO_CADUCIDAD,EVALUA_CANTIDAD,EVALUA_USUARIO,EVALUA_POLITICA) values " _
            & "('" & Id_ruta & "','" & id_tipo_actividad_general & "','" _
            & Nombre_Actividad & "'," & ref_descripcion & ",'" & 0 & "','" & 0 & "','" & 0 & "','" & 0 & "')"
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        ref.Returna_Conexion_Mysql(myConnection)
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = Sql_Insercion_actividad
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Crear_actividad_script_ruta = "Imposible crear la nueva actividad  "
                myConnection.Close()
                Exit Function
            End If
            Dim sql_insert_eventos As String = ""
            Dim ident = myCommand.LastInsertedId
            sql_insert_eventos = "insert into SCRIPT_ACTIVIDADES(LISTADO_ACTIVIDADES_WORKFLOW_ID_ACTIVIDAD," _
            & "ACTIVIDADES_GENERALES_WORKFLOW_ID_ACTIVIDAD_GENERAL,NOMBRE_EVENTO,ESTADO_SCRIPT) VALUES "
            For i = 0 To matri_eventos.Length - 1
                If i = 0 Then
                    sql_insert_eventos = sql_insert_eventos & "(" & ident & "," & id_tipo_actividad_general & ",'" & matri_eventos(i) & "'," & 1 & ")"
                Else
                    sql_insert_eventos = sql_insert_eventos & ", (" & ident & "," & id_tipo_actividad_general & ",'" & matri_eventos(i) & "'," & 1 & ")"
                End If
            Next
            myCommand.CommandText = sql_insert_eventos
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Crear_actividad_script_ruta = "Imposible registrar los eventos de la actividad  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim SqlInsert = "Insert Into grupos_workflow(rutas_workflow_id_ruta," _
               & "nombre_grupo,fecha_creacion,estado_grupo,id_Actividad) VALUES (" _
               & Id_ruta & ",'" & Nombre_Actividad & "','" & Fecha_Format & "',1," & ident & " )"
            If option_crea_asocia_grupo_workflow = 1 Then
                myCommand.CommandText = SqlInsert
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Crear_actividad_script_ruta = "Imposible registrar grupo worokflow  "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            '----------------------------------
            'Agrega el shape al diagrama
            '----------------------------------
            Dim tipo_actividad As String = ""
            If actividades_generales_workflow_.Nombre_tipo_actividad = "ENLACE" Then
                tipo_actividad = "ENLASE"
            Else
                tipo_actividad = actividades_generales_workflow_.Nombre_tipo_actividad
            End If
            Result = Me.Agrega_shape_ruta_worokflow(DiagramView,
                                                    UpdatePanel_diagran_view,
                                                    tipo_actividad,
                                                    ident,
                                                    Nombre_Actividad,
                                                    nombre_ruta,
                                                    Id_ruta)
            If Result <> "YES" Then
                Crear_actividad_script_ruta = Result
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Crear_actividad_script_ruta = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Crear_actividad_script_ruta = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                End If
            End Try
            myConnection.Close()
            Crear_actividad_script_ruta = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Retorna_Matriz_evento_actividad(ByVal tipo_actividad As String,
                                             ByRef matri_usuario_salida() As String) As String
        Try
            If tipo_actividad = "USUARIO" Then
                Dim i_contador As Integer = 0
                Dim matri_usuario() As String = HttpContext.Current.Session.Item("MatriScriptUusarioWEB")
                For i As Integer = 0 To matri_usuario.Length - 1
                    ReDim Preserve matri_usuario_salida(i_contador)
                    matri_usuario_salida(i_contador) = matri_usuario(i)
                    i_contador = i_contador + 1
                Next
                Dim matri_usuario_escritorio() As String = HttpContext.Current.Session.Item("MatriScriptUusario")
                For i As Integer = 0 To matri_usuario_escritorio.Length - 1
                    ReDim Preserve matri_usuario_salida(i_contador)
                    matri_usuario_salida(i_contador) = matri_usuario_escritorio(i)
                    i_contador = i_contador + 1
                Next
            End If
            If tipo_actividad = "ENLASE" Then
                Dim i_contador As Integer = 0
                Dim matri_usuario() As String = HttpContext.Current.Session.Item("MatriScriptEnlaseWEB")
                For i As Integer = 0 To matri_usuario.Length - 1
                    ReDim Preserve matri_usuario_salida(i_contador)
                    matri_usuario_salida(i_contador) = matri_usuario(i)
                    i_contador = i_contador + 1
                Next
                Dim matri_usuario_escritorio() As String = HttpContext.Current.Session.Item("MatriScriptEnlace")
                For i As Integer = 0 To matri_usuario_escritorio.Length - 1
                    ReDim Preserve matri_usuario_salida(i_contador)
                    matri_usuario_salida(i_contador) = matri_usuario_escritorio(i)
                    i_contador = i_contador + 1
                Next
            End If
            If tipo_actividad = "SISTEMA" Then
                Dim i_contador As Integer = 0
                Dim matri_usuario() As String = HttpContext.Current.Session.Item("MatriScriptSistema")
                For i As Integer = 0 To matri_usuario.Length - 1
                    ReDim Preserve matri_usuario_salida(i_contador)
                    matri_usuario_salida(i_contador) = matri_usuario(i)
                    i_contador = i_contador + 1
                Next
            End If
            Retorna_Matriz_evento_actividad = "YES"
        Catch ex As Exception
            Retorna_Matriz_evento_actividad = "Inconsistencia general función Retorna_Matriz_evento_actividad " & ex.Message
        End Try

    End Function
    Function Verifica_existencia_grupo_workflow(ByVal nombre_grupo As String, ByVal id_ruta As Integer, ByRef estado_existencia_grupo As String) As String
        '------------------------------------------------
        'Función : Verfica la existencia de un grupo 
        'dentro de la ruta especifica
        'Ing : Miguel Ange Urueta Miranda
        'Fecha : 2017-07-27
        '-------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "Select Id_Grupo from grupos_workflow " _
         & " where Nombre_Grupo= '" & nombre_grupo & "' and Rutas_Workflow_id_Ruta=" & id_ruta
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("from grupos_workflow")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_grupo_workflow = "La funcion Verifica_existencia_grupo_workflow dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_existencia_grupo = "NO"
                Verifica_existencia_grupo_workflow = "YES"
                Exit Function
            Else
                estado_existencia_grupo = "YES"
                Verifica_existencia_grupo_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_grupo_workflow = "Inconsistencia general función Verifica_existencia_grupo_workflow " & ex.Message
        End Try

    End Function
    Function Verifica_existencia_actividad_ruta(ByVal Nombre_Actividad As String, ByVal id_ruta As Integer, ByRef Estado_servicio As String) As String
        '------------------------------------------------------
        'Función : Verifica la existencia de la actividad
        'en la ruta 
        'Fecha : 2017-07-26
        'Ingeniero : Miguel Angel Urueta Miranda
        '------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "Select Id_Actividad from listado_actividades_workflow " _
           & " where Nombre_Actividad= '" & Nombre_Actividad & "' and Rutas_Workflow_id_Ruta=" & id_ruta
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ACTIVIDADES_GENERALES_WORKFLOW")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_actividad_ruta = "La funcion Verifica_existencia_actividad_ruta dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Estado_servicio = "NO"
                Verifica_existencia_actividad_ruta = "YES"
                Exit Function
            Else
                Estado_servicio = "YES"
                Verifica_existencia_actividad_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_actividad_ruta = "Inconsistencia general función Verifica_existencia_actividad_ruta " & ex.Message
        End Try

    End Function
    Function Retorna_id_tipo_actividad_general(ByVal Tipo_Actividad As String,
                                               ByRef id_tipo_actividad_general As Integer) As String
        '---------------------------------------------------------------
        'Función : Retorna id tipo actividad general con el nombre
        'del tipo de actividad general 
        'Fecha : 2017-07-26
        'Ing : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "Select Id_Actividad_General from ACTIVIDADES_GENERALES_WORKFLOW" _
           & " where TIPO_ACTIVIDAD= '" & Tipo_Actividad & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ACTIVIDADES_GENERALES_WORKFLOW")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_tipo_actividad_general = "La funcion Retorna_id_tipo_actividad_general dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_tipo_actividad_general = "Imposible encontrar el id actividad general del tipo " & Tipo_Actividad
                Exit Function
            Else
                id_tipo_actividad_general = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_tipo_actividad_general = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_tipo_actividad_general = "Inconsistencia general función Retorna_id_tipo_actividad_general " & ex.Message
        End Try
    End Function
    Function Guardar_ruta_workflow(ByVal DiagramView As MindFusion.Diagramming.WebForms.DiagramView, ByVal nombre_ruta As String) As String
        '---------------------------------------------------------
        'Función : Guarda el archivo y actualiza la ruta
        'en la base de datos
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2017-07-27
        '---------------------------------------------------------
        Try
            Dim Ruta_archivo_guardado As String = ""
            Dim id_ruta As Integer = 0
            Dim Result As String = ""
            '--------------------------------------------------
            'Retorna id ruta por nombre
            '--------------------------------------------------
            Dim Ref_class_wf_ruta As New Class_worflow_rutas
            Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(nombre_ruta,
                                                                id_ruta)
            If Result <> "YES" Then
                Guardar_ruta_workflow = Result
                Exit Function
            End If
            '--------------------------------------------------
            'Guarda el archivo en el sistema de archivo
            '--------------------------------------------------
            'Result = Salva_archivo_file_sistem(DiagramView, Ruta_archivo_guardado, nombre_ruta)
            'If Result <> "YES" Then
            '    Guardar_ruta_workflow = Result
            '    Exit Function
            'End If
            Dim string_diagrama As String = DiagramView.SaveToString(SaveToStringFormat.Base64, True)
            '-------------------------------------------------
            'Guarda el archivo en la base de datos
            '-------------------------------------------------
            Result = Guarda_archivo_base_datos_string(string_diagrama, id_ruta)
            If Result <> "YES" Then
                Guardar_ruta_workflow = Result
                Exit Function
            End If
            Guardar_ruta_workflow = "YES"
        Catch ex As Exception
            Guardar_ruta_workflow = "Inconsistencia general función Guardar_ruta_workflow " & ex.Message
        End Try

    End Function
    Function Actualiza_estado_campo_listado_ruta(ByVal id_configuracion As Integer,
                                                 ByVal nombre_campo As String) As String
        '------------------------------------------------------
        'Función : Actualiza el estado del campo de la lista
        'de configuración con el argumento 
        'Fecha : 2017-01-24
        'Ing : Miguel Angel Urueta Miranda
        '------------------------------------------------------
        Dim sql_update_seteo As String = "Update configuracion_listado_ruta set " & nombre_campo & "= 0"
        Dim sql_update_actualizacion As String = "Update configuracion_listado_ruta set " & nombre_campo & "= 1 where Id_Configuracion=" & id_configuracion
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        ref.Returna_Conexion_Mysql(myConnection)
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sql_update_seteo
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Actualiza_estado_campo_listado_ruta = "Imposible setear los campos de la tabla  "
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = sql_update_actualizacion
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Actualiza_estado_campo_listado_ruta = "Imposible registrar los eventos de la actividad  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Actualiza_estado_campo_listado_ruta = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Actualiza_estado_campo_listado_ruta = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                End If
            End Try
            myConnection.Close()
            Actualiza_estado_campo_listado_ruta = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Solicita_datos_estructura_estado_tarea(ByVal id_estado_tarea As Integer,
                                                    ByRef stru_estado_tarea As stru_estado_tarea) As String
        Try
            Dim Sql_consulta As String = " SELECT id_Estado,Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta,Inicio_Tareas_Workflow_id_Tarea,Id_Actividad,Id_Usuario,Fecha_Inicio, " &
                "Fecha_Seleccion,Fecha_Fin,Duracion_Inicio_Seleccion,Duracion_Seleccion_Fin,Estado_Prioridad,Estado_Tarea,ID_FLUJO_TRABAJO," &
                " ID_ACTIVIDAD_FLUJO_TRABAJO,ID_USUARIO_WORKFLOW_FLUJO_TRABAJO,ESTADO_RECUPERACION_FLUJO_TRABAJO " &
                " FROM  estados_tarea_workflow  " _
                & " WHERE id_Estado = " & id_estado_tarea
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim ref_clas_gestion_fecha As New ClassGestionFechas
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_estructura_estado_tarea = "función Solicita_datos_estructura_estado_tarea dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                stru_estado_tarea = Nothing
                Solicita_datos_estructura_estado_tarea = "YES"
                Exit Function
            Else
                stru_estado_tarea.id_Estado = Datset.Tables(0).Rows(0).Item(0)
                stru_estado_tarea.Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta = Datset.Tables(0).Rows(0).Item(1)
                stru_estado_tarea.Inicio_Tareas_Workflow_id_Tarea = Datset.Tables(0).Rows(0).Item(2)
                stru_estado_tarea.Id_Actividad = Datset.Tables(0).Rows(0).Item(3)
                If Datset.Tables(0).Rows(0).IsNull(4) Then
                    stru_estado_tarea.Id_Usuario = 0
                Else
                    stru_estado_tarea.Id_Usuario = Datset.Tables(0).Rows(0).Item(4)
                End If
                If Datset.Tables(0).Rows(0).IsNull(5) Then
                    stru_estado_tarea.Fecha_Inicio = ""
                Else
                    Dim fech_ref_date_db As Date = Datset.Tables(0).Rows(0).Item(5)
                    Result = ref_clas_gestion_fecha.Formatea_fecha_time_db(fech_ref_date_db, stru_estado_tarea.Fecha_Inicio)
                    If Result <> "YES" Then
                        Solicita_datos_estructura_estado_tarea = "Funcion Solicita_datos_estructura_estado_tarea dice error formatendo fecha (" & fech_ref_date_db & ")" & Result
                        Exit Function
                    End If
                End If
                If Datset.Tables(0).Rows(0).IsNull(6) Then
                    stru_estado_tarea.Fecha_Seleccion = ""
                Else
                    Dim fech_ref_date_db As Date = Datset.Tables(0).Rows(0).Item(6)
                    Result = ref_clas_gestion_fecha.Formatea_fecha_time_db(fech_ref_date_db, stru_estado_tarea.Fecha_Seleccion)
                    If Result <> "YES" Then
                        Solicita_datos_estructura_estado_tarea = "Funcion Solicita_datos_estructura_estado_tarea dice error formatendo fecha (" & fech_ref_date_db & ")" & Result
                        Exit Function
                    End If
                End If
                If Datset.Tables(0).Rows(0).IsNull(7) Then
                    stru_estado_tarea.Fecha_Fin = ""
                Else
                    Dim fech_ref_date_db As Date = Datset.Tables(0).Rows(0).Item(7)
                    Result = ref_clas_gestion_fecha.Formatea_fecha_time_db(fech_ref_date_db, stru_estado_tarea.Fecha_Fin)
                    If Result <> "YES" Then
                        Solicita_datos_estructura_estado_tarea = "Funcion Solicita_datos_estructura_estado_tarea dice error formatendo fecha (" & fech_ref_date_db & ")" & Result
                        Exit Function
                    End If
                End If
                If Datset.Tables(0).Rows(0).IsNull(8) Then
                    stru_estado_tarea.Duracion_Inicio_Seleccion = 0
                Else
                    stru_estado_tarea.Duracion_Inicio_Seleccion = Datset.Tables(0).Rows(0).Item(8)
                End If
                If Datset.Tables(0).Rows(0).IsNull(9) Then
                    stru_estado_tarea.Duracion_Seleccion_Fin = 0
                Else
                    stru_estado_tarea.Duracion_Seleccion_Fin = Datset.Tables(0).Rows(0).Item(9)

                End If
                stru_estado_tarea.Estado_Prioridad = Datset.Tables(0).Rows(0).Item(10)
                stru_estado_tarea.Estado_Tarea = Datset.Tables(0).Rows(0).Item(11)
                stru_estado_tarea.ID_FLUJO_TRABAJO = Datset.Tables(0).Rows(0).Item(12)
                stru_estado_tarea.ID_ACTIVIDAD_FLUJO_TRABAJO = Datset.Tables(0).Rows(0).Item(13)
                stru_estado_tarea.ID_USUARIO_WORKFLOW_FLUJO_TRABAJO = Datset.Tables(0).Rows(0).Item(14)
                stru_estado_tarea.ESTADO_RECUPERACION_FLUJO_TRABAJO = Datset.Tables(0).Rows(0).Item(15)
                Solicita_datos_estructura_estado_tarea = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_estructura_estado_tarea = "Inconsistencia general función Solicita_datos_estructura_estado_tarea " & ex.Message
        End Try
    End Function
    Function Dibuja_detalle_conector_trazabilidad(ByVal id_estado_tarea As Long,
                                                  ByRef fecha_inicio As String,
                                                  ByRef Fecha_Seleccion As String,
                                                  ByRef Fecha_Fin As String,
                                                  ByRef Duracion_Inicio_Seleccion As String,
                                                  ByRef Duracion_Seleccion_Fin As String,
                                                  ByRef usuario_asignado As String,
                                                  ByRef cargo_usuario_asignado As String,
                                                  ByRef id_Estado As String,
                                                  ByRef upanel As UpdatePanel) As String
        Try
            Dim Result As String = ""
            Dim Refclas_GestionFechas As New ClassGestionFechas
            Dim Stru As stru_estado_tarea = Nothing
            fecha_inicio = ""
            Fecha_Seleccion = ""
            Fecha_Fin = ""
            Duracion_Inicio_Seleccion = ""
            Duracion_Seleccion_Fin = ""
            usuario_asignado = ""
            cargo_usuario_asignado = ""
            id_Estado = ""
            Result = Me.Solicita_datos_estructura_estado_tarea(id_estado_tarea, Stru)
            If Result <> "YES" Then
                Dibuja_detalle_conector_trazabilidad = Result
                Exit Function
            End If
            If Stru.Fecha_Fin Is Nothing Then
                Dibuja_detalle_conector_trazabilidad = "Imposible encontrar el detalle del estado ( " & id_estado_tarea & " )"
                Exit Function
            End If
            Dim Refclas As New ClassWorkflowUsuario
            If Stru.Id_Usuario <> 0 Then
                Result = Refclas.Solicita_nombre_cargo_usuario_workflow(Stru.Id_Usuario, usuario_asignado, cargo_usuario_asignado)
                If Result <> "YES" Then
                    Dibuja_detalle_conector_trazabilidad = Result
                    Exit Function
                End If
            End If
            fecha_inicio = Stru.Fecha_Inicio.ToString
            Fecha_Seleccion = Stru.Fecha_Seleccion.ToString
            Fecha_Fin = Stru.Fecha_Fin
            Dim tiempo_trabajo_tarea As Object = 0
            Dim tiempo_trabajo_tarea_sel As Object = 0
            If Stru.Fecha_Seleccion <> "" Then
                Dim Fecha_Fromat_ini = fecha_inicio
                Dim Fecha_Fromat_fin = Fecha_Seleccion
                Result = Refclas_GestionFechas.Resta_fechas_db(Fecha_Fromat_ini,
                                                               Fecha_Fromat_fin,
                                                               tiempo_trabajo_tarea_sel)
                If Result <> "YES" Then
                    Dibuja_detalle_conector_trazabilidad = Result
                    Exit Function
                End If
                Dim hora As Object = (Val(tiempo_trabajo_tarea_sel) * 1) / 60
                Dim dia As Object = (hora * 1) / 24
                Duracion_Inicio_Seleccion = "Minutos (" & tiempo_trabajo_tarea_sel & ")  Horas (" & Left(hora.ToString, 6) & ") Dias (" & Left(dia.ToString, 6) & ")"
            Else
                Duracion_Inicio_Seleccion = "Minutos (" & tiempo_trabajo_tarea_sel & ") Horas (0) Dias (0)"
            End If

            If Stru.Fecha_Fin <> "" Then
                Dim Fecha_Fromat_ini = fecha_inicio
                Dim Fecha_Fromat_fin = Fecha_Fin
                Result = Refclas_GestionFechas.Resta_fechas_db(Fecha_Fromat_ini,
                                                               Fecha_Fromat_fin,
                                                               tiempo_trabajo_tarea)
                If Result <> "YES" Then
                    Dibuja_detalle_conector_trazabilidad = Result
                    Exit Function
                End If
                Dim hora As Object = (Val(tiempo_trabajo_tarea) * 1) / 60
                Dim dia As Object = (hora * 1) / 24
                Duracion_Seleccion_Fin = "Minutos (" & tiempo_trabajo_tarea & ")  Horas (" & Left(hora.ToString, 6) & ") Dias (" & Left(dia.ToString, 6) & ")"
            Else
                Duracion_Seleccion_Fin = "Minutos (" & tiempo_trabajo_tarea & ")  Horas (0) Dias (0)"
            End If
            id_Estado = id_estado_tarea
            Dibuja_detalle_conector_trazabilidad = "YES"
        Catch ex As Exception
            Dibuja_detalle_conector_trazabilidad = "Incosistencia general función Dibuja_detalle_conector_trazabilidad " & ex.Message
        Finally
            upanel.Update()
        End Try
    End Function

    Function Mostrar_detalle_elemento_diagrama(ByVal ref_diagram As MindFusion.Diagramming.WebForms.DiagramView,
                                               ByRef pag As Page) As String
        Try
            Dim Result As String = ""
            If ref_diagram.Diagram.Selection.Items.Count = 0 Then
                Mostrar_detalle_elemento_diagrama = "Por favor seleccione el elemento del diagrama "
                Exit Function
            End If
            If ref_diagram.Diagram.Selection.Items.Count > 1 Then
                Mostrar_detalle_elemento_diagrama = "Solo se puede eliminar un elemento del diagrama "
                Exit Function
            End If
            Dim Shap_link As MindFusion.Diagramming.DiagramLink = Nothing
            For Each sha In ref_diagram.Diagram.Selection.Items
                If ref_diagram.Diagram.Selection.Items(0).GetType.FullName = "MindFusion.Diagramming.DiagramLink" Then
                    Shap_link = ref_diagram.Diagram.Selection.Items(0)
                    Exit For
                End If
            Next

            If Not Shap_link Is Nothing Then
                If Shap_link.Tag = "Traza" Then
                    Dim TextBox_fecha_inicio As TextBox = pag.FindControl("TextBox_fecha_inicio")
                    Dim TextBox_Fecha_Seleccion As TextBox = pag.FindControl("TextBox_Fecha_Seleccion")
                    Dim TextBox_Fecha_Fin As TextBox = pag.FindControl("TextBox_Fecha_Fin")
                    Dim TextBox_Duracion_Inicio_Seleccion As TextBox = pag.FindControl("TextBox_Duracion_Inicio_Seleccion")
                    Dim TextBox_Duracion_Seleccion_Fin As TextBox = pag.FindControl("TextBox_Duracion_Seleccion_Fin")
                    Dim TextBox_usuario_asignado As TextBox = pag.FindControl("TextBox_usuario_asignado")
                    Dim TextBox_cargo_usuario_asignado As TextBox = pag.FindControl("TextBox_cargo_usuario_asignado")
                    Dim TextBox_id_Estado As TextBox = pag.FindControl("TextBox_id_Estado")
                    Dim UpdatePanel_detalle_conector_trazabilidad As UpdatePanel = pag.FindControl("UpdatePanel_detalle_conector_trazabilidad")
                    Dim ModalPopupExtender_detalle_conector_trazabilidad As AjaxControlToolkit.ModalPopupExtender =
                        pag.FindControl("ModalPopupExtender_detalle_conector_trazabilidad")
                    Result = Me.Dibuja_detalle_conector_trazabilidad(Shap_link.Id, TextBox_fecha_inicio.Text, TextBox_Fecha_Seleccion.Text,
                                                                     TextBox_Fecha_Fin.Text, TextBox_Duracion_Inicio_Seleccion.Text,
                                                                     TextBox_Duracion_Seleccion_Fin.Text, TextBox_usuario_asignado.Text,
                                                                     TextBox_cargo_usuario_asignado.Text, TextBox_id_Estado.Text,
                                                                     UpdatePanel_detalle_conector_trazabilidad)
                    If Result <> "YES" Then
                        Mostrar_detalle_elemento_diagrama = Result
                        Exit Function
                    Else
                        ModalPopupExtender_detalle_conector_trazabilidad.Show()
                        Mostrar_detalle_elemento_diagrama = "YES"
                        Exit Function
                    End If
                End If
            End If
            Mostrar_detalle_elemento_diagrama = "YES"
        Catch ex As Exception
            Mostrar_detalle_elemento_diagrama = "Inconsistencia general fución Mostrar_detalle_elemento_diagrama " & ex.Message
        End Try
    End Function
    Function Solicita_nombre_ruta_workflow(ByVal RutActividad As String,
                                           ByRef Nombre_Ruta As String) As String
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "Select NOMBRE_RUTA from " &
            " RUTAS_WORKFLOW WHERE ID_RUTA =" & Val(RutActividad)
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("RUTAS_WORKFLOW")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombre_ruta_workflow = "# 02 Error Consultando en tabla " & "RUTAS_WORKFLOW" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_nombre_ruta_workflow = "Imposible Encontrar Nombre Ruta (" & RutActividad & ")"
                Exit Function
            Else
                Nombre_Ruta = Datset.Tables(0).Rows(0).Item(0).ToString
                Solicita_nombre_ruta_workflow = "YES"
            End If
        Catch ex As Exception
            Solicita_nombre_ruta_workflow = "# 02 Error Consultando nombre ruta" & ex.Message
        End Try
    End Function
    Function Solicita_nombre_ruta_por_id_ruta(ByVal id_ruta As Integer,
                                              ByRef Nombre_Ruta As String) As String
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "Select NOMBRE_RUTA from " &
            " RUTAS_WORKFLOW WHERE ID_RUTA =" & id_ruta
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("RUTAS_WORKFLOW")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombre_ruta_por_id_ruta = "Error funcion Solicita_nombre_ruta_por_id_ruta  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_nombre_ruta_por_id_ruta = "Imposible Encontrar nombre de ruta por id (" & id_ruta & ")"
                Exit Function
            Else
                Nombre_Ruta = Datset.Tables(0).Rows(0).Item(0).ToString
                Solicita_nombre_ruta_por_id_ruta = "YES"
            End If
        Catch ex As Exception
            Solicita_nombre_ruta_por_id_ruta = "Inconsistencia general funcion Solicita_nombre_ruta_por_id_ruta " & ex.Message
        End Try
    End Function
    Function SolicitaEstadoRutaCerradoAbierto(ByVal IdtareaWorkflow As Long,
                                              ByVal IdRutaWorkflow As Integer,
                                              ByVal RadicadoTarea As String,
                                              ByVal NombreRutaWorkflow As String,
                                              ByRef EstadoRuta As Integer,
                                              ByRef Tramite As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Soliciata estado de ruta cerrada o abierta segun el tramite relacionado
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdtareaWorkflow     : Representa la identificación de la tarea workflow
        'IdRutaWorkflow      : Representa la identiicacion de la ruta
        'RadicadoTarea       : Presenta el conscutvo de radicado
        'NombreRutaWorkflow  : Representa el nombre de la ruta
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'EstadoRuta  : Retorna el estado de la ruta 1-Cerrado 0- abierto
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-09
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Ref_class_cinfig_listado_ruta As New Class_configuracion_listado_ruta
            Dim NombrecampoTramite As String = ""
            Result = Ref_class_cinfig_listado_ruta.SolicitaNombreCampoTramiteRuta(IdRutaWorkflow,
                                                                                  NombrecampoTramite)
            If Result <> "YES" Then
                SolicitaEstadoRutaCerradoAbierto = Result
                Exit Function
            End If
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = Class_DAT_ADIC_TAR.SolicitaTramiteFlujoWorkflow(IdtareaWorkflow,
                                                                    IdRutaWorkflow,
                                                                    NombrecampoTramite,
                                                                    NombreRutaWorkflow,
                                                                    Tramite,
                                                                    0)
            If Result <> "YES" Then
                SolicitaEstadoRutaCerradoAbierto = Result
                Exit Function
            End If
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Result = Class_tipo_doc_entrante.SolicitaEstadoTramiteRutaAbiertaCerrada(Tramite,
                                                                                     EstadoRuta)
            If Result <> "YES" Then
                SolicitaEstadoRutaCerradoAbierto = Result
                Exit Function
            End If
            SolicitaEstadoRutaCerradoAbierto = "YES"
            Exit Function
        Catch ex As Exception
            SolicitaEstadoRutaCerradoAbierto = "Inconsistencia general funcion SolicitaEstadoRutaCerradoAbierto " & ex.Message
        End Try
    End Function
    Function Solicita_etado_abierto_cerrado_ruta_tarea(ByVal id_tarea_workflow As Long,
                                                       ByVal id_ruta As Integer,
                                                       ByRef estado_ruta As Integer,
                                                       ByRef tramite As String) As String
        Try
            Dim Radicado As String = ""
            Dim Result As String = ""
            Dim estado_flujo As Integer = 0
            Dim nombre_campo_tramite As String = ""
            Dim nombre_ruta As String = ""
            Dim estado_ruta_abierta As Integer = 0
            Dim Refclass_workflow As New ClassWorkflow
            Dim Ref_class_seleccion As New Classselecciotarea
            Dim ref_class_dat As New Class_DAT_ADIC_TAR
            Dim Ref_class_tipo_doc_antrante As New Class_tipo_doc_entrante
            '-------------------------------------------------------
            'Solicita el nombre radicado de la tarea seleccionada
            '-------------------------------------------------------
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(id_tarea_workflow,
                                                                                    Radicado)
            If Result <> "YES" Then
                Solicita_etado_abierto_cerrado_ruta_tarea = Result
                Exit Function
            End If
            '--------------------------------
            'Retorna nombre ruta
            '--------------------------------
            Dim Ref_class_ruta As New Class_worflow_rutas
            Result = Ref_class_ruta.Retorna_nombre_ruta_por_id_ruta(id_ruta.ToString,
                                                                    nombre_ruta)
            If Result <> "YES" Then
                Solicita_etado_abierto_cerrado_ruta_tarea = Result
                Exit Function
            End If
            '--------------------------------
            'Retorna campo tramite documento
            '--------------------------------
            Dim Ref_class_cinfig_listado_ruta As New Class_configuracion_listado_ruta
            Result = Ref_class_cinfig_listado_ruta.SolicitaNombreCampoTramiteRuta(id_ruta,
                                                                                  nombre_campo_tramite)
            If Result <> "YES" Then
                Solicita_etado_abierto_cerrado_ruta_tarea = Result
                Exit Function
            End If
            '-------------------------------------------
            'Retorna estado cerrado abierta ruta tarea  
            '-------------------------------------------
            Result = ref_class_dat.SolicitaTramiteFlujoWorkflow(id_tarea_workflow,
                                                                id_ruta,
                                                                nombre_campo_tramite,
                                                                nombre_ruta,
                                                                tramite,
                                                                estado_flujo)
            If Result <> "YES" Then
                Solicita_etado_abierto_cerrado_ruta_tarea = Result
                Exit Function
            End If
            '-------------------------------------------------
            'Verifica que la taarea no pertenezca a un flujo
            '-------------------------------------------------
            If estado_flujo <> 0 Then
                estado_ruta = 0
                Solicita_etado_abierto_cerrado_ruta_tarea = "YES"
                Exit Function
            Else
                Result = Ref_class_tipo_doc_antrante.SolicitaEstadoTramiteRutaAbiertaCerrada(tramite,
                                                                                                  estado_ruta)
                If Result <> "YES" Then
                    Solicita_etado_abierto_cerrado_ruta_tarea = Result
                    Exit Function
                End If
            End If
            Solicita_etado_abierto_cerrado_ruta_tarea = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_etado_abierto_cerrado_ruta_tarea = "Inconsistencia general función Solicita_etado_abierto_cerrado_ruta_tarea " & ex.Message
        End Try
    End Function
    Function Retorna_nombre_ruta_workflow(ByRef nombre_ruta As String) As String
        Try
            Dim Sql_consulta As String = "SELECT Nombre_Ruta" & _
               " FROM rutas_workflow " & _
               " WHERE Estado_Ruta=1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("rutas_workflow")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_nombre_ruta_workflow = " #12 Inconsistencia función Retorna_nombre_ruta_workflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_nombre_ruta_workflow = "No hay ruta activa para trazabilidad "
                Exit Function
            Else
                nombre_ruta = Datset.Tables(0).Rows(0).Item(0).ToString
                Retorna_nombre_ruta_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_ruta_workflow = "Inconsistencia función Retorna_nombre_ruta_workflow " & ex.Message
        End Try
    End Function
    Function Retorna_nombre_ruta_por_id_ruta(ByVal id_ruta As Object, _
                                             ByRef nombre_ruta As String) As String
        Try
            Dim Sql_consulta As String = "SELECT Nombre_Ruta" & _
               " FROM rutas_workflow " & _
               " WHERE id_Ruta=" & id_ruta
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("rutas_workflow")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_nombre_ruta_por_id_ruta = " #12 Inconsistencia función Retorna_nombre_ruta_workflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_nombre_ruta_por_id_ruta = "No hay ruta activa para trazabilidad "
                Exit Function
            Else
                nombre_ruta = Datset.Tables(0).Rows(0).Item(0).ToString
                Retorna_nombre_ruta_por_id_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_ruta_por_id_ruta = "Inconsistencia función Retorna_nombre_ruta_por_id_ruta " & ex.Message
        End Try
    End Function

    Function Retorna_id_ruta_workflow(ByVal nombre_ruta As String, _
                                      ByRef id_ruta As Integer) As String
        Try
            Dim Sql_consulta As String = "SELECT id_Ruta " & _
               " FROM rutas_workflow " & _
               " WHERE Nombre_Ruta='" & nombre_ruta & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("rutas_workflow")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_ruta_workflow = " #12 Inconsistencia función Retorna_id_ruta_workflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_ruta_workflow = "No hay ruta activa para trazabilidad con el nombre " & nombre_ruta
                Exit Function
            Else
                id_ruta = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_ruta_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_ruta_workflow = "Inconsistencia función Retorna_id_ruta_workflow " & ex.Message
        End Try
    End Function
    Function Listar_Rutas_Workflow(ByRef Matri_Reportes() As String) As String
        Try
            Dim Sql_consulta = "select ID_RUTA,NOMBRE_RUTA FROM RUTAS_WORKFLOW "
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Listar_Rutas_Workflow = "Imposible listar rutas workflow" & Result
                Exit Function
            End If
            If Datset Is Nothing Or Datset.Tables.Count = 0 Then
                Listar_Rutas_Workflow = " Imposible Encontrar id ruta "
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Listar_Rutas_Workflow = " Imposible Encontrar id ruta no record tabla (0)"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Matri_Reportes(i)
                    Matri_Reportes(i) = Datset.Tables(0).Rows(i).Item(0).ToString & "|" & Datset.Tables(0).Rows(i).Item(1).ToString
                Next
                Listar_Rutas_Workflow = "YES"
                Exit Function
            End If
            Listar_Rutas_Workflow = "YES"
        Catch ex As Exception
            Listar_Rutas_Workflow = ex.Message
        End Try
    End Function
    Function Solicita_id_ruta_nonbre_ruta_workflow(ByRef nombre_ruta As String,
                                                   ByRef id_ruta As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita el nombre de la ruta y la identificación de la ruta activa
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        '
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'nombre_ruta  : Retorna el nombre de la ruta
        'id_ruta      : Retorna la identificación de la ruta
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-04
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Sql_consulta As String = "SELECT id_Ruta,Nombre_Ruta " &
               " FROM rutas_workflow  WHERE Estado_Ruta=1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("rutas_workflow")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_ruta_nonbre_ruta_workflow = " Error función Solicita_id_ruta_nonbre_ruta_workflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_ruta_nonbre_ruta_workflow = "Imposible encontrar rutas activas "
                Exit Function
            Else
                id_ruta = Datset.Tables(0).Rows(0).Item(0)
                nombre_ruta = Datset.Tables(0).Rows(0).Item(1)
                Solicita_id_ruta_nonbre_ruta_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_ruta_nonbre_ruta_workflow = "Inconsistencia función Solicita_id_ruta_nonbre_ruta_workflow " & ex.Message
        End Try
    End Function
End Class
