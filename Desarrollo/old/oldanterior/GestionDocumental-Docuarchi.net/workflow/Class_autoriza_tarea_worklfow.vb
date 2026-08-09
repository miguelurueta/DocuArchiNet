Imports System.IO
Imports System.IO.IsolatedStorage
Imports System.Xml
Imports MySql.Data.MySqlClient
Imports iTextSharp.text
Imports iTextSharp.text.pdf

Public Structure stru_autorizacion
    Dim Id_autorizacion As Long
    Dim estados_tarea_workflow_id_Estado As Long
    Dim rutas_workflow_id_Ruta As Integer
    Dim inicio_tareas_workflow_id_Tarea As Long
    Dim usuario_workflow_idU_suario As Integer
    Dim id_actividad_flujo As Integer
    Dim feha_autorizacion As String
    Dim nombre_usuario_worlflow As String
    Dim cargo_usuario_workflow As String
    Dim nombre_actividad_workflow As String
    Dim nombre_ruta_workflow As String
    Dim nombre_flujo_trabajo As String
    Dim Beneficiario_tramite As String
    Dim nombre_tramite As String
    Dim radicado As String
    Dim estado_firma_digital As Integer
    Dim estado_autorizacion As Integer
    Dim fecha_anula_aprobacion As String
    Dim has_huella As String
    Dim clave_has_huella As String
    Dim tipo_has_huella As String
    Dim Id_Actividad As Integer
    Dim ID_FLUJO_TRABAJO As Integer
    Dim nombre_actividad_flujo As String
End Structure
Public Class Class_autoriza_tarea_worklfow
    Function Autoriza_tarea(ByVal id_tarea As Long, _
                            ByVal id_ruta As Integer, _
                            ByVal id_usuario_wf As String, _
                            ByVal id_actividad As Integer) As String

        Dim Result As String = ""
        Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
        Dim stru_estado As stru_estado = Nothing
        Result = Class_estados_tarea_workflow.Solicita_datos_estructura_tareas_seleccionada(id_actividad, _
                                                                                            id_usuario_wf, _
                                                                                            id_tarea, _
                                                                                            stru_estado)
        If Result <> "YES" Then
            Autoriza_tarea = Result
            Exit Function
        End If
        Dim estado_existencia_aprobacion As String = ""
        If stru_estado.ID_FLUJO_TRABAJO = 0 Then
            Result = SolicitaExistenciaAutorizacion(id_tarea,
                                                      id_actividad,
                                                      id_usuario_wf,
                                                      estado_existencia_aprobacion)
            If Result <> "YES" Then
                Autoriza_tarea = Result
                Exit Function
            End If
        Else
            Result = Solicita_existencia_autorizacion_flujo_trabajo(id_tarea,
                                                                    stru_estado.ID_ACTIVIDAD_FLUJO_TRABAJO,
                                                                    id_usuario_wf,
                                                                    estado_existencia_aprobacion)
            If Result <> "YES" Then
                Autoriza_tarea = Result
                Exit Function
            End If
        End If
        If estado_existencia_aprobacion = "YES" Then
            Autoriza_tarea = "Existe una autorización para la tarea, imposible continuar"
            Exit Function
        End If
        Dim nombre_actividad As String = ""
        Dim nombre_actividad_flujo As Object = "Null"
        Dim Class_Listado_Actividades_workflow As New Class_Listado_Actividades_workflow
        Result = Class_Listado_Actividades_workflow.Retorna_Nombre_Actividad_id_actividad(id_actividad,
                                                                                          nombre_actividad)
        If Result <> "YES" Then
            Autoriza_tarea = Result
            Exit Function
        End If
        Dim class_usuario_workflow As New Class_usuario_workflow
        Dim nombre_usuario As String = ""
        Dim cargo_usuario As String = ""
        class_usuario_workflow.Solicita_nombre_cargo_usuario_workflow(id_usuario_wf,
                                                                      nombre_usuario,
                                                                      cargo_usuario)
        If Result <> "YES" Then
            Autoriza_tarea = Result
            Exit Function
        End If
        If stru_estado.ID_FLUJO_TRABAJO <> 0 Then
            nombre_actividad_flujo = "'" & nombre_usuario & " (" & cargo_usuario & ")'"
        End If
        Dim class_workflow_rutas As New Class_worflow_rutas
        Dim nombre_ruta_workflow As String = ""
        Result = class_workflow_rutas.Solicita_nombre_ruta_workflow(id_ruta,
                                                                   nombre_ruta_workflow)

        If Result <> "YES" Then
            Autoriza_tarea = Result
            Exit Function
        End If
        Dim class_flujo_trabajo_workflow As New Class_flujo_trabajo_workflow
        Dim nombre_flujo_trabajo As Object = "Null"
        If stru_estado.ID_FLUJO_TRABAJO <> 0 Then
            Result = class_flujo_trabajo_workflow.SolicitaNombreFlujoTrabajoPorIdFlujo(stru_estado.ID_FLUJO_TRABAJO,
                                                                                            nombre_flujo_trabajo)
            If Result <> "YES" Then
                Autoriza_tarea = Result
                Exit Function
            Else
                nombre_flujo_trabajo = "'" & nombre_flujo_trabajo & "'"
            End If
        End If
        Dim nombre_campo_radicado As String = ""
        Dim Class_configuracion_listado_ruta As New Class_configuracion_listado_ruta
        Result = Class_configuracion_listado_ruta.SolicitaNombreCampoRadicadoRuta(id_ruta,
                                                                                  nombre_campo_radicado)
        If Result <> "YES" Then
            Autoriza_tarea = Result
            Exit Function
        End If
        Dim campo_tramite As String = ""
        Result = Class_configuracion_listado_ruta.SolicitaNombreCampoTramiteRuta(id_ruta,
                                                                                 campo_tramite)
        If Result <> "YES" Then
            Autoriza_tarea = Result
            Exit Function
        End If
        Dim campo_beneficiario As String = ""
        Result = Class_configuracion_listado_ruta.SolicitaNombreCampoBenificiarioRuta(id_ruta,
                                                                                         campo_beneficiario)
        If Result <> "YES" Then
            Autoriza_tarea = Result
            Exit Function
        End If
        Dim class_dat_adic_tar As New Class_DAT_ADIC_TAR
        Dim Radicado As String = ""
        class_dat_adic_tar.Solicita_valor_campo_dinamico_ruta(nombre_ruta_workflow,
                                                              nombre_campo_radicado,
                                                              id_tarea,
                                                              Radicado)
        If Result <> "YES" Then
            Autoriza_tarea = Result
            Exit Function
        End If
        Dim tramite As Object = "Null"
        class_dat_adic_tar.Solicita_valor_campo_dinamico_ruta(nombre_ruta_workflow,
                                                              campo_tramite,
                                                              id_tarea,
                                                              tramite)
        If Result <> "YES" Then
            Autoriza_tarea = Result
            Exit Function
        End If
        If tramite = "" Then
            tramite = "Null"
        Else
            tramite = "'" & tramite & "'"
        End If
        Dim beneficiario As Object = "Null"
        class_dat_adic_tar.Solicita_valor_campo_dinamico_ruta(nombre_ruta_workflow,
                                                              campo_beneficiario,
                                                              id_tarea,
                                                              beneficiario)
        If Result <> "YES" Then
            Autoriza_tarea = Result
            Exit Function
        End If
        If beneficiario = "" Then
            beneficiario = "Null"
        Else
            beneficiario = "'" & beneficiario & "'"
        End If
        Dim fecha_autorizacion As String = ""
        Dim class_gestion_fechas As New ClassGestionFechas
        Result = class_gestion_fechas.Formatea_fecha_time_framework(Now,
                                                                  fecha_autorizacion)
        If Result <> "YES" Then
            Autoriza_tarea = Result
            Exit Function
        End If
        Dim has_huella As String = "Indentificacion de la ruta :" & id_ruta & "|" &
            "identificacion del estado de tarea que autoriza: " & stru_estado.id_Estado & "|" &
            "Identificacion de la tarea workflow : " & id_tarea & "|" &
            "Identificacion del usuario workflow : " & id_usuario_wf & "|" &
            "Identificacion de la actividad de flujo de trabajo : " & stru_estado.ID_ACTIVIDAD_FLUJO_TRABAJO & "|" &
            "Identificacion del flujo de trabajo : " & stru_estado.ID_FLUJO_TRABAJO & "|" &
            "Fecha de la autorizacion : " & fecha_autorizacion & "|" &
            "Nombre usuario  que autoriza : " & nombre_usuario & "|" &
            "Cargo usuario  que autoriza : " & cargo_usuario & "|" &
            "Nombre actividad workflow : " & nombre_actividad & "|" &
            "Nombre actividad flujo trabajo : " & nombre_actividad_flujo & "|" &
            "Nombre ruta de trabajo : " & nombre_ruta_workflow & "|" &
            "Nombre flujo de trabajo : " & nombre_flujo_trabajo & "|" &
            "Usuario beneficiario de la tarea : " & beneficiario & "|" &
            "Nombre tramite de la tarea : " & tramite & "|" &
            "Numero radicado de la tarea : " & Radicado & "|" &
            "Estado firma tramite : " & "0" & "|" &
            "Clave has : " & "7894561230!" & "|" &
            "Tipo has : " & "md5" & "|"
        Dim has_huella_md5_escript As String = ""
        Result = encriptacion.encript_md5(has_huella,
                                        "7894561230!",
                                        has_huella_md5_escript)

        If Result <> "YES" Then
            Autoriza_tarea = Result
            Exit Function
        End If
        Dim stru_autorizacion As stru_autorizacion
        stru_autorizacion.rutas_workflow_id_Ruta = id_ruta
        stru_autorizacion.estados_tarea_workflow_id_Estado = stru_estado.id_Estado
        stru_autorizacion.inicio_tareas_workflow_id_Tarea = id_tarea
        stru_autorizacion.usuario_workflow_idU_suario = id_usuario_wf
        stru_autorizacion.id_actividad_flujo = stru_estado.ID_ACTIVIDAD_FLUJO_TRABAJO
        stru_autorizacion.ID_FLUJO_TRABAJO = stru_estado.ID_FLUJO_TRABAJO
        stru_autorizacion.feha_autorizacion = fecha_autorizacion
        stru_autorizacion.nombre_usuario_worlflow = nombre_usuario
        stru_autorizacion.cargo_usuario_workflow = cargo_usuario
        stru_autorizacion.nombre_actividad_workflow = nombre_actividad
        stru_autorizacion.nombre_ruta_workflow = nombre_ruta_workflow
        stru_autorizacion.nombre_flujo_trabajo = nombre_flujo_trabajo
        stru_autorizacion.Beneficiario_tramite = beneficiario
        stru_autorizacion.nombre_tramite = tramite
        stru_autorizacion.radicado = Radicado
        stru_autorizacion.clave_has_huella = "7894561230!"
        stru_autorizacion.tipo_has_huella = "md5"
        stru_autorizacion.has_huella = has_huella_md5_escript
        Dim ruta_archivo_xml As String = ""
        Dim estado_obliga_firma_digital As Integer = 0
        Dim Class_wf_configura_autoriza_tarea As New Class_wf_configura_autoriza_tarea
        Result = Class_wf_configura_autoriza_tarea.Solicita_configuracion_autorizacion("AUTORIZA_TAREA_WORKFLOW",
                                                                                       ruta_archivo_xml,
                                                                                       estado_obliga_firma_digital)
        If Result <> "YES" Then
            Autoriza_tarea = Result
            Exit Function
        End If
        ruta_archivo_xml = ruta_archivo_xml.Replace("/", "\")
        Dim Class_Crea_directory As New Class_Crea_directory
        If Directory.Exists(ruta_archivo_xml) = False Then
            Result = Class_Crea_directory.Crea_directory(ruta_archivo_xml)
            If Result <> "YES" Then
                Autoriza_tarea = Result
                Exit Function
            End If
        End If
        Dim Parametro_registra_autorizacion As String = "Insert into wf_autoriza_tarea (estados_tarea_workflow_id_Estado," &
       "rutas_workflow_id_Ruta,inicio_tareas_workflow_id_Tarea,usuario_workflow_idU_suario,id_actividad_flujo,feha_autorizacion," &
       "nombre_usuario_worlflow,cargo_usuario_workflow,nombre_actividad_workflow,nombre_ruta_ruta_workflow,nombre_flujo_trabajo," &
       "Beneficiario_tramite,nombre_tramite,radicado,estado_firma_digital,estado_autorizacion,has_huella,clave_has_huella,tipo_has_huella," &
       "Id_Actividad,nombre_actividad_flujo) values (" &
       stru_estado.id_Estado & "," & id_ruta & "," & id_tarea & "," & id_usuario_wf & "," & stru_estado.ID_ACTIVIDAD_FLUJO_TRABAJO &
       ",'" & fecha_autorizacion & "','" & nombre_usuario & "','" & cargo_usuario & "','" & nombre_actividad & "','" & nombre_ruta_workflow &
       "'," & nombre_flujo_trabajo & "," & beneficiario & "," & tramite & ",'" & Radicado & "'," & 0 & "," & 1 & ",'" & has_huella_md5_escript &
       "','" & stru_autorizacion.clave_has_huella & "','" & stru_autorizacion.tipo_has_huella & "'," & id_actividad & "," & nombre_actividad_flujo & ")"
        Dim Class_zero_fill As New Class_zero_fill
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = Parametro_registra_autorizacion
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Autoriza_tarea = "Imposible registrar la autorización de la tarea   "
                myConnection.Close()
                Exit Function
            End If
            Dim id_aprobacion As Object = myCommand.LastInsertedId
            Dim id_String = id_aprobacion.ToString
            Result = Class_zero_fill.zero_fill(id_String,
                                               20,
                                               "0")
            If Result <> "YES" Then
                myTrans.Rollback()
                myConnection.Close()
                Autoriza_tarea = "Error generando zero fill del archivo xml " & Result
                Exit Function
            End If
            Dim nombre_archivo As String = id_String & ".xml"
            Dim Ruta_archivo As String = ruta_archivo_xml & nombre_archivo
            Result = Me.Genera_archivo_xml_aprobacion(id_aprobacion,
                                                      stru_autorizacion,
                                                      Ruta_archivo,
                                                      nombre_actividad_flujo,
                                                      nombre_archivo)

            If Result <> "YES" Then
                myTrans.Rollback()
                myConnection.Close()
                Autoriza_tarea = "Error generando archivo xml " & Result
                Exit Function
            End If
            myTrans.Commit()
            Autoriza_tarea = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Autoriza_tarea = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Autoriza_tarea = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Genera_archivo_xml_aprobacion(ByVal id_aprobacion As Long,
                                           ByVal stru_aprobacion As stru_autorizacion,
                                           ByVal ruta_archivo As String,
                                           ByVal nombre_actividad_flujo As String,
                                           ByVal nombre_archivo As String) As String
        Try
            If File.Exists(ruta_archivo) = True Then
                Kill(ruta_archivo)
            End If
        Catch ex As Exception

        End Try

        Dim myXmlTextWriter As XmlTextWriter = New XmlTextWriter(ruta_archivo,
                                                                  System.Text.Encoding.UTF8)
        Try
            myXmlTextWriter.Formatting = System.Xml.Formatting.Indented
            myXmlTextWriter.WriteStartDocument(False)
            myXmlTextWriter.WriteStartElement("Autoriza_tareas")
            myXmlTextWriter.WriteStartElement("Autoriza_tarea")
            myXmlTextWriter.WriteAttributeString("Identificacion_autorizacion", id_aprobacion)
            myXmlTextWriter.WriteAttributeString("Indentificacion_de_la_ruta", stru_aprobacion.rutas_workflow_id_Ruta)
            myXmlTextWriter.WriteAttributeString("Identificacion_del_estado_de_tarea_que_autoriza", stru_aprobacion.estados_tarea_workflow_id_Estado)
            myXmlTextWriter.WriteAttributeString("Identificacion_de_la_tarea_workflow", stru_aprobacion.inicio_tareas_workflow_id_Tarea)
            myXmlTextWriter.WriteAttributeString("Identificacion_del_usuario_workflow", stru_aprobacion.usuario_workflow_idU_suario)
            myXmlTextWriter.WriteAttributeString("Identificacion_de_la_actividad_de_flujo_de_trabajo", stru_aprobacion.id_actividad_flujo)
            myXmlTextWriter.WriteAttributeString("Identificacion_del_flujo_de_trabajo", stru_aprobacion.ID_FLUJO_TRABAJO)
            myXmlTextWriter.WriteAttributeString("Fecha_de_la_autorizacion", stru_aprobacion.feha_autorizacion)
            myXmlTextWriter.WriteAttributeString("Nombre_usuario_que_autoriza", stru_aprobacion.nombre_usuario_worlflow)
            myXmlTextWriter.WriteAttributeString("Cargo_usuario_que_autoriza", stru_aprobacion.cargo_usuario_workflow)
            myXmlTextWriter.WriteAttributeString("Nombre_actividad_workflow", stru_aprobacion.nombre_actividad_workflow)
            myXmlTextWriter.WriteAttributeString("Nombre_actividad_flujo_trabajo", nombre_actividad_flujo)
            myXmlTextWriter.WriteAttributeString("Nombre_ruta_de_trabajo", stru_aprobacion.nombre_ruta_workflow)
            myXmlTextWriter.WriteAttributeString("Nombre_flujo_de_trabajo", stru_aprobacion.nombre_flujo_trabajo)
            myXmlTextWriter.WriteAttributeString("Usuario_beneficiario_de_la_tarea", stru_aprobacion.Beneficiario_tramite)
            myXmlTextWriter.WriteAttributeString("Nombre_tramite_de_la_tarea", stru_aprobacion.nombre_tramite)
            myXmlTextWriter.WriteAttributeString("Numero_radicado_de_la_tarea", stru_aprobacion.radicado)
            myXmlTextWriter.WriteAttributeString("Has_huella", stru_aprobacion.has_huella)
            myXmlTextWriter.WriteAttributeString("Clave_has", stru_aprobacion.clave_has_huella)
            myXmlTextWriter.WriteAttributeString("Tipo_has", stru_aprobacion.tipo_has_huella)
            myXmlTextWriter.WriteAttributeString("Nombre_archivo_xml", nombre_archivo)
            myXmlTextWriter.WriteEndElement()
            myXmlTextWriter.WriteEndElement()
            myXmlTextWriter.Flush()
            myXmlTextWriter.Close()
            Genera_archivo_xml_aprobacion = "YES"
        Catch ex As Exception
            If Not myXmlTextWriter Is Nothing Then
                myXmlTextWriter.Close()
            End If
            Genera_archivo_xml_aprobacion = "Error general Genera_archivo_xml_aprobacion " & ex.Message
        End Try
    End Function

    Function SolicitaExistenciaAutorizacion(ByVal IdTareaWorkflow As Long,
                                            ByVal idActividadWorkflow As Integer,
                                            ByVal IdUsuarioWorkflow As Integer,
                                            ByRef ExistenciaAutorizacion As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : La función verifica la existencia de la aprobación de la tarea workflow en el registro
        'de la tarea aprobadas, la función verifica que el usuario workflow, la actividad
        'y la tarea no halla aprobado la tarea
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTareaWorkflow     : Representa la identificación de la tarea workflow
        'idActividadWorkflow : Representa la identiicaicón de la actividad workflow
        'IdUsuarioWorkflow   : Representa la identiicación del usuario workflow
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'ExistenciaAutorizacion     : Retorna el estado de existencia de aprobacion
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2019-10-24
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT Id_autorizacion FROM wf_autoriza_tarea" &
            " WHERE inicio_tareas_workflow_id_Tarea=" & IdTareaWorkflow &
            " AND Id_Actividad=" & idActividadWorkflow & " AND " &
            " usuario_workflow_idU_suario = " & IdUsuarioWorkflow &
            " and estado_autorizacion=1 "
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_autoriza_tarea")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaExistenciaAutorizacion = "Se presentó una inconsistencia en la función  SolicitaExistenciaAutorizacion : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                ExistenciaAutorizacion = "NO"
                SolicitaExistenciaAutorizacion = "YES"
                Exit Function
            Else
                ExistenciaAutorizacion = "YES"
                SolicitaExistenciaAutorizacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaExistenciaAutorizacion = "Inconsistencia general función SolicitaExistenciaAutorizacion " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_firma_autorizacion(ByVal id_tarea As Long, _
                                                    ByVal id_actividad As Integer, _
                                                    ByVal id_usuario_wf As Integer, _
                                                    ByRef existencia As String) As String
        '----------------------------------------------------------------
        'Funcion : La función verifica la existencia de la aprobación
        'de la tarea workflow en el registro de la tarea aprobadas
        'la función verifica que el usuario workflow, la actividad
        'y la tarea no halla aprobado la tarea
        'id_tarea : Representa la tarea workflow
        'id_actividad : Representa la actividad workflow
        'id_usuario_wf : Representa el usuario worlflow
        'existencia : Retorna la exitencia del registro de la aprobación
        'Fecha : 2019-10-24
        'Ing : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT estado_firma_digital FROM wf_autoriza_tarea" & _
            " WHERE inicio_tareas_workflow_id_Tarea=" & id_tarea & _
            " AND Id_Actividad=" & id_actividad & " AND " & _
            " usuario_workflow_idU_suario = " & id_usuario_wf & _
            " and estado_autorizacion=1 "
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_autoriza_tarea")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_firma_autorizacion = "Funcion  Solicita_existencia_autorizacion " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia = "NO"
                Solicita_existencia_firma_autorizacion = "YES"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).Item(0) = 1 Then
                    existencia = "YES"
                Else
                    existencia = "NO"
                End If

                Solicita_existencia_firma_autorizacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_firma_autorizacion = "Inconsistencia general función Solicita_existencia_firma_autorizacion " & ex.Message
        End Try
    End Function
    Function Solicita_id_autorizacion(ByVal id_tarea As Long, _
                                      ByVal id_actividad As Integer, _
                                      ByVal id_usuario_wf As Integer, _
                                      ByRef id_autorizacion As Long) As String
        '----------------------------------------------------------------
        'Funcion : La función solicita la identificacion de la autorizacion
        'de la tarea workflow en el registro de la tarea aprobadas
        'la función verifica que el usuario workflow, la actividad
        'y la tarea no halla aprobado la tarea
        'id_tarea : Representa la tarea workflow
        'id_actividad : Representa la actividad workflow
        'id_usuario_wf : Representa el usuario worlflow
        'existencia : Retorna la exitencia del registro de la aprobación
        'Fecha : 2019-10-26
        'Ing : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT Id_autorizacion FROM wf_autoriza_tarea" & _
            " WHERE inicio_tareas_workflow_id_Tarea=" & id_tarea & _
            " AND Id_Actividad=" & id_actividad & " AND " & _
            " usuario_workflow_idU_suario = " & id_usuario_wf & _
            " and estado_autorizacion=1"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_autoriza_tarea")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_autorizacion = "Funcion  Solicita_id_autorizacion " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_autorizacion = "Imposible encontrar la autorización de la tarea (" & id_tarea & ")"
                Exit Function
            Else
                id_autorizacion = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_autorizacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_autorizacion = "Inconsistencia general función Solicita_id_autorizacion " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_autorizacion_flujo_trabajo(ByVal id_tarea As Long, _
                                                            ByVal id_actividad_flujo_trabajo As Integer, _
                                                            ByVal id_usuario_wf As Integer, _
                                                            ByRef existencia As String) As String
        '----------------------------------------------------------------------------------
        'Funcion : La función verifica la existencia de la aprobación
        'de la tarea workflow en el registro de la tarea aprobadas
        'la función verifica que el usuario workflow, la actividad de FELUJO DE TRABAJO
        'y la tarea no halla aprobado la tarea
        'id_tarea : Representa la tarea workflow
        'id_actividad_flujo_trabajo : Representa la actividad del flujo de trabajo
        'id_usuario_wf : Representa el usuario worlflow
        'existencia : Retorna la exitencia del registro de la aprobación
        'Fecha : 2019-10-24
        'Ing : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT Id_autorizacion FROM wf_autoriza_tarea" & _
            " WHERE inicio_tareas_workflow_id_Tarea=" & id_tarea & _
            " AND id_actividad_flujo=" & id_actividad_flujo_trabajo & " AND " & _
            " usuario_workflow_idU_suario = " & id_usuario_wf & _
            " and estado_autorizacion=1 "
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_autoriza_tarea")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_autorizacion_flujo_trabajo = "Funcion  Solicita_existencia_autorizacion_flujo_trabajo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia = "NO"
                Solicita_existencia_autorizacion_flujo_trabajo = "YES"
                Exit Function
            Else
                existencia = "YES"
                Solicita_existencia_autorizacion_flujo_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_autorizacion_flujo_trabajo = "Inconsistencia general función Solicita_existencia_autorizacion_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Lista_autorizaciones_tarea(ByVal id_tarea As Long, _
                                        ByVal tipo_consulta As Integer, _
                                        ByVal valor_consulta As String, _
                                        ByRef colum_order_name As String, _
                                        ByRef order_colum As String, _
                                        ByRef label_title As Label, _
                                        ByRef scripma As GridView, _
                                        ByRef update_panel As UpdatePanel) As String
        Try
            Dim sql_consulta As String = ""
            If tipo_consulta = 1 Then
                sql_consulta = "SELECT Id_autorizacion,nombre_usuario_worlflow as AUTORIZANTE,cargo_usuario_workflow as CARGO," & _
                    "feha_autorizacion AS FECHA,radicado as RADICADO,nombre_tramite as TRAMITE," _
                    & "nombre_actividad_workflow AS ACTIVIDAD,nombre_actividad_flujo AS ACTIVIDAD_USUARIO," & _
                    "nombre_flujo_trabajo as FLUJO,Beneficiario_tramite as BENEFICIARIO " & _
                    " from wf_autoriza_tarea " & _
                    " where inicio_tareas_workflow_id_Tarea=" & id_tarea & _
                    " and estado_autorizacion=1" & _
                    "  order by  " & colum_order_name & " " & order_colum
            End If
            Dim Result As String = ""
            HttpContext.Current.Session.Item("Sort_matri_colum_lista_autoriza") = {"OPCIONES", "Id_autorizacion", "AUTORIZANTE", "CARGO", "FECHA", "RADICADO", _
                                                                                   "TRAMITE", "ACTIVIDAD", "ACTIVIDAD_USUARIO", "FLUJO", "BENEFICIARIO"}
            HttpContext.Current.Session.Item("SortExpression_lista_autoriza") = colum_order_name
            HttpContext.Current.Session.Item("SortDirection_produccion_lista_autoriza") = order_colum
            HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_LISTA_AUTORIZA") = tipo_consulta
            HttpContext.Current.Session.Item("GA_DATO_CONSULTA_LISTA_AUTORIZA") = sql_consulta
            Dim Datset_consulta As DataSet = New DataSet("wf_autoriza_tarea")
            Dim Dat_reader As MySql.Data.MySqlClient.MySqlDataReader = Nothing
            Dim ref As New conect.Dbase_Conction_Mysql
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset_consulta)
            If Result <> "YES" Then
                Lista_autorizaciones_tarea = "Error listando datos " & Result
                Exit Function
            End If
            If Datset_consulta.Tables(0).Rows.Count = 0 Then
                label_title.Text = "(" & Datset_consulta.Tables(0).Rows.Count & ") registro(s) de autorización "
                scripma.DataSource = Datset_consulta
                scripma.DataBind()
                update_panel.Update()
                Lista_autorizaciones_tarea = "YES"
                Exit Function
            Else
                label_title.Text = "(" & Datset_consulta.Tables(0).Rows.Count & ") registro(s) de autorización "
                scripma.DataSource = Datset_consulta
                scripma.DataBind()
                update_panel.Update()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fas fa-file-download")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent_autoriza_xx(event,this);")
                    ahtml.Attributes.Add("title", "Descarga xml")
                    ahtml.Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "descarga_xml")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    divhtml.Style.Add("display", "inline-flex")
                    scripma.Rows(i).Cells(0).Controls.Add(divhtml)
                    For z As Integer = 0 To scripma.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            scripma.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            scripma.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If
                    Next
                Next
                Dim Refclas_ As New ClassGredview
                Result = Refclas_.add_clase_acender_decender(colum_order_name, _
                                                             HttpContext.Current.Session.Item("Sort_matri_colum_lista_autoriza"), _
                                                             order_colum, _
                                                             scripma)
                If Result <> "YES" Then
                    Lista_autorizaciones_tarea = "Error add clase función  add_clase_acender_decender " & Result
                    Exit Function
                End If
                Lista_autorizaciones_tarea = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_autorizaciones_tarea = "Inconsistencia general función Lista_autorizaciones_tarea " & ex.Message
        End Try
    End Function
    Function Anular_autorizacion_tarea(ByVal id_usuario_wf As Integer, _
                                       ByVal id_actividad As Integer, _
                                       ByVal id_tarea As Long) As String
        Try
            Dim Result As String = ""
            Dim id_autorizacion As Long = 0
            Result = Me.Solicita_id_autorizacion(id_tarea, _
                                                 id_actividad, _
                                                 id_usuario_wf, _
                                                 id_autorizacion)
            If Result <> "YES" Then
                Anular_autorizacion_tarea = Result
                Exit Function
            End If
            Dim stru_autorizacion As stru_autorizacion = Nothing
            Result = Solicita_datos_estructura_autorizacion(id_autorizacion, _
                                                           stru_autorizacion)
            If Result <> "YES" Then
                Anular_autorizacion_tarea = Result
                Exit Function
            End If
            Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            Dim stru_estado As stru_estado = Nothing
            Result = Class_estados_tarea_workflow.Solicita_datos_estructura_tareas_seleccionada(id_actividad, _
                                                                                                id_usuario_wf, _
                                                                                                id_tarea, _
                                                                                                stru_estado)
            If Result <> "YES" Then
                Anular_autorizacion_tarea = Result
                Exit Function
            End If
            If stru_estado.Id_Usuario <> stru_autorizacion.usuario_workflow_idU_suario Then
                Anular_autorizacion_tarea = "El usuario no es el propietario de la aprobación, imposible anular"
                Exit Function
            End If
            If stru_estado.id_Estado <> stru_autorizacion.estados_tarea_workflow_id_Estado Then
                Anular_autorizacion_tarea = "Esta tratando de anular un aprobación en un estado de asignación diferente al estado de aprobación orignal, imposible anular"
                Exit Function
            End If
            Dim fecha_anula As String = ""
            Dim ClassGestionFechas As New ClassGestionFechas
            Result = ClassGestionFechas.Formatea_fecha_time_framework(Now, _
                                                                      fecha_anula)
            If Result <> "YES" Then
                Anular_autorizacion_tarea = Result
                Exit Function
            End If
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim sql_update As String = "Update wf_autoriza_tarea set estado_autorizacion=0 where Id_autorizacion=" & id_autorizacion
            Result = ref.SELECTION_INSERT_COMMAND(sql_update)
            If Result <> "YES" Then
                Anular_autorizacion_tarea = Result
                Exit Function
            End If
            Anular_autorizacion_tarea = "YES"
            Exit Function
        Catch ex As Exception
            Anular_autorizacion_tarea = "Inconsistencia general función Anular_autorizacion_tarea " & ex.Message
        End Try
    End Function
    Function Elimnar_autorizacion_tarea(ByVal id_usuario_wf As Integer, _
                                        ByVal id_actividad As Integer, _
                                        ByVal id_tarea As Long) As String
        Try
            Dim Result As String = ""
            Dim id_autorizacion As Long = 0
            Result = Me.Solicita_id_autorizacion(id_tarea, _
                                                 id_actividad, _
                                                 id_usuario_wf, _
                                                 id_autorizacion)
            If Result <> "YES" Then
                Elimnar_autorizacion_tarea = Result
                Exit Function
            End If
            Dim stru_autorizacion As stru_autorizacion = Nothing
            Result = Solicita_datos_estructura_autorizacion(id_autorizacion, _
                                                           stru_autorizacion)
            If Result <> "YES" Then
                Elimnar_autorizacion_tarea = Result
                Exit Function
            End If
            Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            Dim stru_estado As stru_estado = Nothing
            Result = Class_estados_tarea_workflow.Solicita_datos_estructura_tareas_seleccionada(id_actividad, _
                                                                                                id_usuario_wf, _
                                                                                                id_tarea, _
                                                                                                stru_estado)
            If Result <> "YES" Then
                Elimnar_autorizacion_tarea = Result
                Exit Function
            End If
            If stru_estado.Id_Usuario <> stru_autorizacion.usuario_workflow_idU_suario Then
                Elimnar_autorizacion_tarea = "El usuario no es el propietario de la aprobación, imposible eliminar"
                Exit Function
            End If
            
            
            If stru_estado.id_Estado <> stru_autorizacion.estados_tarea_workflow_id_Estado Then
                Elimnar_autorizacion_tarea = "Esta tratando de anular un aprobación en un estado de asignación diferente al estado de aprobación orignal, imposible anular"
                Exit Function
            End If
            Dim ruta_archivo_xml As String = ""
            Dim estado_obliga_firma_digital As Integer = 0
            Dim Class_wf_configura_autoriza_tarea As New Class_wf_configura_autoriza_tarea
            Result = Class_wf_configura_autoriza_tarea.Solicita_configuracion_autorizacion("AUTORIZA_TAREA_WORKFLOW", _
                                                                                           ruta_archivo_xml, _
                                                                                           estado_obliga_firma_digital)
            If Result <> "YES" Then
                Elimnar_autorizacion_tarea = Result
                Exit Function
            End If
            ruta_archivo_xml = ruta_archivo_xml.Replace("/", "\")
            Dim id_String = id_autorizacion.ToString
            Dim Class_zero_fill As New Class_zero_fill
            Result = Class_zero_fill.zero_fill(id_String, _
                                               20, _
                                               "0")
            If Result <> "YES" Then
                Elimnar_autorizacion_tarea = Result
                Exit Function
            End If
            Dim nombre_archivo As String = id_String & ".xml"
            Dim Ruta_archivo As String = ruta_archivo_xml & nombre_archivo
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim sql_update As String = "delete from wf_autoriza_tarea  where Id_autorizacion=" & id_autorizacion
            Result = ref.SELECTION_INSERT_COMMAND(sql_update)
            If Result <> "YES" Then
                Elimnar_autorizacion_tarea = Result
                Exit Function
            End If
            If File.Exists(Ruta_archivo) = True Then
                Kill(Ruta_archivo)
            End If
            Elimnar_autorizacion_tarea = "YES"
            Exit Function
        Catch ex As Exception
            Elimnar_autorizacion_tarea = "Inconsistencia general función Elimnar_autorizacion_tarea " & ex.Message
        End Try
    End Function
    Function Solicita_datos_estructura_autorizacion(ByVal id_autorizacion As Long, _
                                                    ByRef stru_autorizacion As stru_autorizacion) As String
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT estados_tarea_workflow_id_Estado, " & _
            "rutas_workflow_id_Ruta,ID_FLUJO_TRABAJO,inicio_tareas_workflow_id_Tarea,usuario_workflow_idU_suario," & _
            "id_actividad_flujo,feha_autorizacion,nombre_usuario_worlflow,cargo_usuario_workflow,nombre_actividad_workflow," & _
            "nombre_actividad_flujo,nombre_ruta_ruta_workflow,nombre_flujo_trabajo,Beneficiario_tramite,nombre_tramite," & _
            "radicado,estado_firma_digital,estado_autorizacion,fecha_anula_aprobacion,has_huella,clave_has_huella,tipo_has_huella," & _
            "Id_Actividad" & _
            " FROM wf_autoriza_tarea " &
            " WHERE Id_autorizacion=" & id_autorizacion
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_autoriza_tarea")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_estructura_autorizacion = "Funcion  Solicita_datos_estructura_autorizacion " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_estructura_autorizacion = "Imposible encontrar los datos de estructura del numero de autorización  (" & id_autorizacion & ")"
                Exit Function
            Else
                stru_autorizacion.estados_tarea_workflow_id_Estado = Datset.Tables(0).Rows(0).Item(0)
                stru_autorizacion.rutas_workflow_id_Ruta = Datset.Tables(0).Rows(0).Item(1)
                stru_autorizacion.ID_FLUJO_TRABAJO = Datset.Tables(0).Rows(0).Item(2)
                stru_autorizacion.inicio_tareas_workflow_id_Tarea = Datset.Tables(0).Rows(0).Item(3)
                stru_autorizacion.usuario_workflow_idU_suario = Datset.Tables(0).Rows(0).Item(4)
                stru_autorizacion.id_actividad_flujo = Datset.Tables(0).Rows(0).Item(5)
                stru_autorizacion.feha_autorizacion = Datset.Tables(0).Rows(0).Item(6)
                stru_autorizacion.nombre_usuario_worlflow = Datset.Tables(0).Rows(0).Item(7)
                stru_autorizacion.cargo_usuario_workflow = Datset.Tables(0).Rows(0).Item(8)
                stru_autorizacion.nombre_actividad_workflow = Datset.Tables(0).Rows(0).Item(9)
                If Datset.Tables(0).Rows(0).IsNull(10) Then
                    stru_autorizacion.nombre_actividad_flujo = ""
                Else
                    stru_autorizacion.nombre_actividad_flujo = Datset.Tables(0).Rows(0).Item(10)
                End If
                stru_autorizacion.nombre_ruta_workflow = Datset.Tables(0).Rows(0).Item(11)
                If Datset.Tables(0).Rows(0).IsNull(12) Then
                    stru_autorizacion.nombre_flujo_trabajo = ""
                Else
                    stru_autorizacion.nombre_flujo_trabajo = Datset.Tables(0).Rows(0).Item(12)
                End If
                If Datset.Tables(0).Rows(0).IsNull(13) Then
                    stru_autorizacion.Beneficiario_tramite = ""
                Else
                    stru_autorizacion.Beneficiario_tramite = Datset.Tables(0).Rows(0).Item(13)
                End If
                If Datset.Tables(0).Rows(0).IsNull(14) Then
                    stru_autorizacion.nombre_tramite = ""
                Else
                    stru_autorizacion.nombre_tramite = Datset.Tables(0).Rows(0).Item(14)
                End If
                stru_autorizacion.radicado = Datset.Tables(0).Rows(0).Item(15)
                stru_autorizacion.estado_firma_digital = Datset.Tables(0).Rows(0).Item(16)
                stru_autorizacion.estado_autorizacion = Datset.Tables(0).Rows(0).Item(17)
                If Datset.Tables(0).Rows(0).IsNull(18) Then
                    stru_autorizacion.fecha_anula_aprobacion = ""
                Else
                    stru_autorizacion.fecha_anula_aprobacion = Datset.Tables(0).Rows(0).Item(18)
                End If
                stru_autorizacion.has_huella = Datset.Tables(0).Rows(0).Item(19)
                stru_autorizacion.clave_has_huella = Datset.Tables(0).Rows(0).Item(20)
                stru_autorizacion.tipo_has_huella = Datset.Tables(0).Rows(0).Item(21)
                stru_autorizacion.Id_Actividad = Datset.Tables(0).Rows(0).Item(22)
                Solicita_datos_estructura_autorizacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_estructura_autorizacion = "Inconsistencia general función Solicita_datos_estructura_autorizacion " & ex.Message
        End Try
    End Function
    Function Descarga_archivo_xml(ByVal id_autorizacion As Long, _
                                  ByRef iframe As Object, _
                                  ByRef Hidden_ruta_archivo As Object, _
                                  ByRef update_panel As UpdatePanel) As String
        Try
            Dim Class_wf_configura_autoriza_tarea As New Class_wf_configura_autoriza_tarea
            Dim Result As String = ""
            Dim Ruta_archivo As String = ""
            Dim estado_firma As Integer = 0
            Result = Class_wf_configura_autoriza_tarea.Solicita_configuracion_autorizacion("AUTORIZA_TAREA_WORKFLOW", _
                                                                                           Ruta_archivo, _
                                                                                           estado_firma)
            If Result <> "YES" Then
                Descarga_archivo_xml = Result
                Exit Function
            End If
            If Ruta_archivo = "" Then
                Descarga_archivo_xml = "El sistema detecto que no hay una ruta de archivo en la configuración"
                Exit Function
            End If
            Ruta_archivo = Ruta_archivo.Replace("/", "\")
            Dim Class_Crea_directory As New Class_Crea_directory
            If Directory.Exists(Ruta_archivo) = False Then
                Descarga_archivo_xml = "Imposible encontrar el directorio (" & Ruta_archivo & ")"
                Exit Function
            End If
            Dim Class_zero_fill As New Class_zero_fill
            Dim id_String = id_autorizacion.ToString
            Result = Class_zero_fill.zero_fill(id_String, _
                                               20, _
                                               "0")
            If Result <> "YES" Then
                Descarga_archivo_xml = Result
                Exit Function
            End If
            Dim nombre_archivo As String = id_String & ".xml"
            Dim Ruttempo As String = HttpContext.Current.Server.MapPath(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_WF") + _
                                                                        HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString)
            If Directory.Exists(Ruttempo) = False Then
                Directory.CreateDirectory(Ruttempo)
            End If
            Dim ruta_descarga As String = Ruttempo & "\DESCARGA"
            If Directory.Exists(ruta_descarga) = False Then
                Directory.CreateDirectory(ruta_descarga)
            End If
            Dim filecopia As String = Ruta_archivo & id_String & ".xml"
            Dim file_destino As String = ruta_descarga & "\" & HttpContext.Current.Session.Item("Id_Usuario_Workflow") & ".xml"
            If File.Exists(filecopia) = False Then
                Descarga_archivo_xml = "Impsobile encontrar el archivo (" & filecopia & ")"
                Exit Function
            End If
            If File.Exists(file_destino) Then
                Kill(file_destino)
            End If
            FileCopy(filecopia, file_destino)
            If File.Exists(file_destino) = True Then
                Hidden_ruta_archivo.Value = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_WF") & "/" & _
                HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/" & _
                HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & ".xml"
                iframe.Attributes.Add("src", "../radicador/WebFormDescargaRadicado.aspx")
                update_panel.Update()
                Descarga_archivo_xml = "YES"
                Exit Function
            Else
                Descarga_archivo_xml = "Imposible encontrar el archivo (" & file_destino & ")"
                Exit Function
            End If
        Catch ex As Exception
            Descarga_archivo_xml = "Inconsistencia general función Descarga_archivo_xml " & ex.Message
        End Try
    End Function
    Function Solicita_consolidado_autorizacion(ByVal id_tarea As Long, _
                                               ByRef iframe As Object, _
                                               ByRef Hidden_ruta_archivo As Object, _
                                               ByRef update_panel As UpdatePanel) As String

        Dim Result As String = ""
        Dim Class_configuracion_listado_ruta As New Class_configuracion_listado_ruta
        Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
        Dim id_ruta As Integer = 0
        Result = Class_estados_tarea_workflow.Solicita_id_ruta_tarea(id_tarea, _
                                                                    id_ruta)
        If Result <> "YES" Then
            Solicita_consolidado_autorizacion = Result
            Exit Function
        End If
        Dim campo_radicado As String = ""
        Result = Class_configuracion_listado_ruta.SolicitaNombreCampoRadicadoRuta(id_ruta,
                                                                                      campo_radicado)
        If Result <> "YES" Then
            Solicita_consolidado_autorizacion = Result
            Exit Function
        End If
        Dim campo_tramite As String = ""
        Result = Class_configuracion_listado_ruta.SolicitaNombreCampoTramiteRuta(id_ruta,
                                                                                 campo_tramite)
        If Result <> "YES" Then
            Solicita_consolidado_autorizacion = Result
            Exit Function
        End If
        Dim campo_beneficiario As String = ""
        Result = Class_configuracion_listado_ruta.SolicitaNombreCampoBenificiarioRuta(id_ruta, _
                                                                                         campo_beneficiario)
        If Result <> "YES" Then
            Solicita_consolidado_autorizacion = Result
            Exit Function
        End If
        Dim Class_worflow_rutas As New Class_worflow_rutas
        Dim nombre_ruta As String = ""
        Result = Class_worflow_rutas.Retorna_nombre_ruta_por_id_ruta(id_ruta, _
                                                                     nombre_ruta)
        If Result <> "YES" Then
            Solicita_consolidado_autorizacion = Result
            Exit Function
        End If
        Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
        Dim Radicado As String = ""
        Result = Class_DAT_ADIC_TAR.Solicita_valor_campo_dinamico_ruta(nombre_ruta, _
                                                                       campo_radicado,
                                                                       id_tarea, _
                                                                       Radicado)
        If Result <> "YES" Then
            Solicita_consolidado_autorizacion = Result
            Exit Function
        End If
        Dim tramite As String = ""
        Result = Class_DAT_ADIC_TAR.Solicita_valor_campo_dinamico_ruta(nombre_ruta, _
                                                                       campo_tramite,
                                                                       id_tarea, _
                                                                       tramite)
        If Result <> "YES" Then
            Solicita_consolidado_autorizacion = Result
            Exit Function
        End If
        Dim beneficiario As String = ""
        Result = Class_DAT_ADIC_TAR.Solicita_valor_campo_dinamico_ruta(nombre_ruta, _
                                                                       campo_beneficiario,
                                                                       id_tarea, _
                                                                       beneficiario)
        If Result <> "YES" Then
            Solicita_consolidado_autorizacion = Result
            Exit Function
        End If
        Dim id_flujo_tarea As Integer = 0
        Result = Class_DAT_ADIC_TAR.SolicitaFlujoTareaWorkflow(nombre_ruta, _
                                                                id_tarea, _
                                                                id_flujo_tarea)
        If Result <> "YES" Then
            Solicita_consolidado_autorizacion = Result
            Exit Function
        End If
        Dim Class_flujo_trabajo_workflow As New Class_flujo_trabajo_workflow
        Dim nombre_flujo_trabajo As String = ""
        If id_flujo_tarea <> 0 Then
            Result = Class_flujo_trabajo_workflow.SolicitaNombreFlujoTrabajoPorIdFlujo(id_flujo_tarea, _
                                                                                            nombre_flujo_trabajo)
            If Result <> "YES" Then
                Solicita_consolidado_autorizacion = Result
                Exit Function
            End If
        End If
        Dim Class_empresa_gestion_documental As New Class_empresa_gestion_documental
        Dim nit_empresa As String = ""
        Dim nombre_empresa As String = ""
        Result = Class_empresa_gestion_documental.Solicita_nombre_identificacion_empresa(nit_empresa, _
                                                                                         nombre_empresa)
        If Result <> "YES" Then
            Solicita_consolidado_autorizacion = Result
            Exit Function
        End If
        Dim ClassGestionFechas As New ClassGestionFechas
        Dim fecha_reporte As String = ""
        Result = ClassGestionFechas.Formatea_fecha_time_framework(Now, _
                                                                  fecha_reporte)
        If Result <> "YES" Then
            Solicita_consolidado_autorizacion = Result
            Exit Function
        End If
        Dim stru_autorizacion() As stru_autorizacion = Nothing
        Result = Solicita_matriz_datos_estructura_autorizacion(id_tarea, _
                                                             stru_autorizacion)
        If Result <> "YES" Then
            Solicita_consolidado_autorizacion = Result
            Exit Function
        End If
        If stru_autorizacion Is Nothing Then
            Solicita_consolidado_autorizacion = "No hay autorizaciones para la tarea "
            Exit Function
        End If
        Dim doc As Document
        Dim writer As PdfWriter = Nothing
        Dim ruta_image As String = HttpContext.Current.Server.MapPath("../imagera/" & "logo_trd.png")
        If File.Exists(ruta_image) = False Then
            Solicita_consolidado_autorizacion = "El sistema no registra el archivo de logo para integrar al detalle contacte a su administrador  " & _
            ruta_image
            Exit Function
        End If
        Dim file_destino As String = ""
        Try
            Dim nombre_archivo As String = "DOC" & HttpContext.Current.Session.Item("Id_Usuario_Workflow") & ".pdf"
            Dim Ruttempo As String = HttpContext.Current.Server.MapPath(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_WF") + _
                                                                        HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString)
            If Directory.Exists(Ruttempo) = False Then
                Directory.CreateDirectory(Ruttempo)
            End If
            Dim ruta_descarga As String = Ruttempo & "\DESCARGA"
            If Directory.Exists(ruta_descarga) = False Then
                Directory.CreateDirectory(ruta_descarga)
            End If
            file_destino = ruta_descarga & "\" & nombre_archivo
            If File.Exists(file_destino) = True Then
                Kill(file_destino)
            End If
            doc = New Document(PageSize.LETTER)
            doc.SetPageSize(PageSize.LETTER.Rotate())
            writer = PdfWriter.GetInstance(doc, _
                               New FileStream(file_destino, FileMode.Create))
            writer.AddViewerPreference(PdfName.PICKTRAYBYPDFSIZE, PdfBoolean.PDFTRUE)
            doc.Open()
            Dim _standardFont As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, _
               12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
            Dim _standardFont_datos_unidad_conservacion As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, _
            12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
            'doc.NewPage()
            Dim imagen As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(ruta_image)
            imagen.BorderWidth = 0
            imagen.Alignment = Element.ALIGN_LEFT
            Dim percentage As Object = 0.0F
            percentage = 100 / imagen.Width
            imagen.ScalePercent(percentage * 80)
            'Insertamos la imagen en el documento  doc.PageNumber = 1
            doc.Add(imagen)
            Dim paragraf As New Paragraph
            paragraf = New Paragraph("CONSOLIDADO DE AUTORIZACIONES", _standardFont)
            paragraf.Alignment = Element.ALIGN_CENTER
            doc.Add(paragraf)
            paragraf = New Paragraph(nombre_empresa & " " & nit_empresa, _standardFont)
            paragraf.Alignment = Element.ALIGN_CENTER
            doc.Add(paragraf)
            paragraf = New Paragraph("Fecha emisión " & " " & fecha_reporte, _standardFont)
            paragraf.Alignment = Element.ALIGN_CENTER
            doc.Add(paragraf)
            doc.Add(Chunk.NEWLINE)
            Dim tblrdatos As PdfPTable = New PdfPTable(2)
            tblrdatos.WidthPercentage = 100
            Dim cltitle_ident_radic As PdfPCell = New PdfPCell(New Phrase("RADICADO ", _standardFont_datos_unidad_conservacion))
            Dim cltival_ident_radic As PdfPCell = New PdfPCell(New Phrase(Radicado, _standardFont_datos_unidad_conservacion))
            Dim cltitle_tipo_tramite As PdfPCell = New PdfPCell(New Phrase("TRAMITE ", _standardFont_datos_unidad_conservacion))
            Dim cltival_tipo_tramite As PdfPCell = New PdfPCell(New Phrase(tramite, _standardFont_datos_unidad_conservacion))
            Dim cltitle_beneficiario As PdfPCell = New PdfPCell(New Phrase("BENEFICIARIO ", _standardFont_datos_unidad_conservacion))
            Dim cltival_beneficiario As PdfPCell = New PdfPCell(New Phrase(beneficiario, _standardFont_datos_unidad_conservacion))
            Dim cltitle_ruta As PdfPCell = New PdfPCell(New Phrase("RUTA GENERAL ", _standardFont_datos_unidad_conservacion))
            Dim cltival_ruta As PdfPCell = New PdfPCell(New Phrase(nombre_ruta, _standardFont_datos_unidad_conservacion))
            Dim cltitle_flujo As PdfPCell = New PdfPCell(New Phrase("FLUJO ", _standardFont_datos_unidad_conservacion))
            Dim cltival_flujo As PdfPCell = New PdfPCell(New Phrase(nombre_flujo_trabajo, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitle_ident_radic)
            tblrdatos.AddCell(cltival_ident_radic)
            tblrdatos.AddCell(cltitle_tipo_tramite)
            tblrdatos.AddCell(cltival_tipo_tramite)
            tblrdatos.AddCell(cltitle_beneficiario)
            tblrdatos.AddCell(cltival_beneficiario)
            tblrdatos.AddCell(cltitle_ruta)
            tblrdatos.AddCell(cltival_ruta)
            tblrdatos.AddCell(cltitle_flujo)
            tblrdatos.AddCell(cltival_flujo)
            doc.Add(tblrdatos)
            doc.Add(Chunk.NEWLINE)
            paragraf = New Paragraph("USUARIOS QUE AUTORIZARON LA TAREA  (" & id_tarea & ") ", _standardFont)
            paragraf.Alignment = Element.ALIGN_CENTER
            doc.Add(paragraf)
            doc.Add(New Paragraph(3, vbCrLf))
            Dim tblrdatos_ As PdfPTable = New PdfPTable(6)
            tblrdatos_.WidthPercentage = 100
            Dim cltitle_ident_transac_user As PdfPCell = New PdfPCell(New Phrase("NUM AUTORIZACION ", _standardFont_datos_unidad_conservacion))
            Dim cltitle_nombre_transac_user As PdfPCell = New PdfPCell(New Phrase("NOMBRE", _standardFont_datos_unidad_conservacion))
            Dim cltitle_cargo_transac_user As PdfPCell = New PdfPCell(New Phrase("CARGO ", _standardFont_datos_unidad_conservacion))
            Dim cltitle_fecha_reg_transac_user As PdfPCell = New PdfPCell(New Phrase("FECHA REGISTRO AUTORIZACION", _standardFont_datos_unidad_conservacion))
            Dim cltitle_fecha_resp_transac_user As PdfPCell = New PdfPCell(New Phrase("ACTIVIDAD QUE AUTORIZO", _standardFont_datos_unidad_conservacion))
            Dim cltitle_estado_transac_user As PdfPCell = New PdfPCell(New Phrase("ACTIVIDAD USUARIO QUE AUTORIZO", _standardFont_datos_unidad_conservacion))
            tblrdatos_.AddCell(cltitle_ident_transac_user)
            tblrdatos_.AddCell(cltitle_nombre_transac_user)
            tblrdatos_.AddCell(cltitle_cargo_transac_user)
            tblrdatos_.AddCell(cltitle_fecha_reg_transac_user)
            tblrdatos_.AddCell(cltitle_fecha_resp_transac_user)
            tblrdatos_.AddCell(cltitle_estado_transac_user)
            For i As Integer = 0 To stru_autorizacion.Length - 1
                Dim cltvalue_ident_transac_user As PdfPCell = New PdfPCell(New Phrase(stru_autorizacion(i).Id_autorizacion, _standardFont_datos_unidad_conservacion))
                Dim cltvalue_nombre_transac_user As PdfPCell = New PdfPCell(New Phrase(stru_autorizacion(i).nombre_usuario_worlflow, _standardFont_datos_unidad_conservacion))
                Dim cltvalue_cargo_transac_user As PdfPCell = New PdfPCell(New Phrase(stru_autorizacion(i).cargo_usuario_workflow, _standardFont_datos_unidad_conservacion))
                Dim cltvalue_fecha_reg_transac_user As PdfPCell = New PdfPCell(New Phrase(stru_autorizacion(i).feha_autorizacion, _standardFont_datos_unidad_conservacion))
                Dim cltvalue_fecha_resp_transac_user As PdfPCell = New PdfPCell(New Phrase(stru_autorizacion(i).nombre_actividad_workflow, _standardFont_datos_unidad_conservacion))
                Dim cltvalue_estado_transac_user As PdfPCell = New PdfPCell(New Phrase(stru_autorizacion(i).nombre_actividad_flujo, _standardFont_datos_unidad_conservacion))
                tblrdatos_.AddCell(cltvalue_ident_transac_user)
                tblrdatos_.AddCell(cltvalue_nombre_transac_user)
                tblrdatos_.AddCell(cltvalue_cargo_transac_user)
                tblrdatos_.AddCell(cltvalue_fecha_reg_transac_user)
                tblrdatos_.AddCell(cltvalue_fecha_resp_transac_user)
                tblrdatos_.AddCell(cltvalue_estado_transac_user)
            Next
            doc.Add(tblrdatos_)
            If tblrdatos_.TotalHeight >= 448.0 And doc.PageNumber = 1 Then
                doc.NewPage()
            End If
            
            Solicita_consolidado_autorizacion = "YES"
        Catch ex As Exception
            Solicita_consolidado_autorizacion = "Inconistencia general función  Solicita_consolidado_autorizacion " & ex.Message
        Finally
            doc.Close()
            If Not writer Is Nothing Then
                writer.Close()
            End If
            If File.Exists(file_destino) = True Then
                Hidden_ruta_archivo.Value = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_WF") & "/" & _
                HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/DOC" & _
                HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & ".pdf"
                iframe.Attributes.Add("src", "../radicador/WebFormDescargaRadicado.aspx")
                update_panel.Update()
            Else
                Solicita_consolidado_autorizacion = "Imposible encontrar el archivo (" & file_destino & ")"

            End If
        End Try
    End Function
    Function Solicita_matriz_datos_estructura_autorizacion(ByVal id_tarea As Long,
                                                           ByRef stru_autorizacion() As stru_autorizacion) As String
        Try
            stru_autorizacion = Nothing
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT estados_tarea_workflow_id_Estado, " &
            "rutas_workflow_id_Ruta,ID_FLUJO_TRABAJO,inicio_tareas_workflow_id_Tarea,usuario_workflow_idU_suario," &
            "id_actividad_flujo,feha_autorizacion,nombre_usuario_worlflow,cargo_usuario_workflow,nombre_actividad_workflow," &
            "nombre_actividad_flujo,nombre_ruta_ruta_workflow,nombre_flujo_trabajo,Beneficiario_tramite,nombre_tramite," &
            "radicado,estado_firma_digital,estado_autorizacion,fecha_anula_aprobacion,has_huella,clave_has_huella,tipo_has_huella," &
            "Id_Actividad,Id_autorizacion" &
            " FROM wf_autoriza_tarea " &
            " WHERE inicio_tareas_workflow_id_Tarea=" & id_tarea &
            " and estado_autorizacion=1"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_autoriza_tarea")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_matriz_datos_estructura_autorizacion = "Funcion  Solicita_datos_estructura_autorizacion " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_matriz_datos_estructura_autorizacion = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_autorizacion(i)
                    stru_autorizacion(i).estados_tarea_workflow_id_Estado = Datset.Tables(0).Rows(i).Item(0)
                    stru_autorizacion(i).rutas_workflow_id_Ruta = Datset.Tables(0).Rows(i).Item(1)
                    stru_autorizacion(i).ID_FLUJO_TRABAJO = Datset.Tables(0).Rows(i).Item(2)
                    stru_autorizacion(i).inicio_tareas_workflow_id_Tarea = Datset.Tables(0).Rows(i).Item(3)
                    stru_autorizacion(i).usuario_workflow_idU_suario = Datset.Tables(0).Rows(i).Item(4)
                    stru_autorizacion(i).id_actividad_flujo = Datset.Tables(0).Rows(i).Item(5)
                    stru_autorizacion(i).feha_autorizacion = Datset.Tables(0).Rows(i).Item(6)
                    stru_autorizacion(i).nombre_usuario_worlflow = Datset.Tables(0).Rows(i).Item(7)
                    stru_autorizacion(i).cargo_usuario_workflow = Datset.Tables(0).Rows(i).Item(8)
                    stru_autorizacion(i).nombre_actividad_workflow = Datset.Tables(0).Rows(i).Item(9)
                    If Datset.Tables(0).Rows(i).IsNull(10) Then
                        stru_autorizacion(i).nombre_actividad_flujo = ""
                    Else
                        stru_autorizacion(i).nombre_actividad_flujo = Datset.Tables(0).Rows(i).Item(10)
                    End If
                    stru_autorizacion(i).nombre_ruta_workflow = Datset.Tables(0).Rows(i).Item(11)
                    If Datset.Tables(0).Rows(i).IsNull(12) Then
                        stru_autorizacion(i).nombre_flujo_trabajo = ""
                    Else
                        stru_autorizacion(i).nombre_flujo_trabajo = Datset.Tables(0).Rows(i).Item(12)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(13) Then
                        stru_autorizacion(i).Beneficiario_tramite = ""
                    Else
                        stru_autorizacion(i).Beneficiario_tramite = Datset.Tables(0).Rows(i).Item(13)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(14) Then
                        stru_autorizacion(i).nombre_tramite = ""
                    Else
                        stru_autorizacion(i).nombre_tramite = Datset.Tables(0).Rows(i).Item(14)
                    End If
                    stru_autorizacion(i).radicado = Datset.Tables(0).Rows(i).Item(15)
                    stru_autorizacion(i).estado_firma_digital = Datset.Tables(0).Rows(i).Item(16)
                    stru_autorizacion(i).estado_autorizacion = Datset.Tables(0).Rows(i).Item(17)
                    If Datset.Tables(0).Rows(i).IsNull(18) Then
                        stru_autorizacion(i).fecha_anula_aprobacion = ""
                    Else
                        stru_autorizacion(i).fecha_anula_aprobacion = Datset.Tables(0).Rows(i).Item(18)
                    End If
                    stru_autorizacion(i).has_huella = Datset.Tables(0).Rows(i).Item(19)
                    stru_autorizacion(i).clave_has_huella = Datset.Tables(0).Rows(i).Item(20)
                    stru_autorizacion(i).tipo_has_huella = Datset.Tables(0).Rows(i).Item(21)
                    stru_autorizacion(i).Id_Actividad = Datset.Tables(0).Rows(i).Item(22)
                    stru_autorizacion(i).Id_autorizacion = Datset.Tables(0).Rows(i).Item(23)
                Next
                Solicita_matriz_datos_estructura_autorizacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_matriz_datos_estructura_autorizacion = "Inconsistencia general función Solicita_matriz_datos_estructura_autorizacion " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_autorizacion(ByVal id_tarea_workflow As Long,
                                              ByRef Estado_existencia_autorizacion As String) As String
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT Id_autorizacion FROM wf_autoriza_tarea" &
            " WHERE inicio_tareas_workflow_id_Tarea=" & id_tarea_workflow
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_autoriza_tarea")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_autorizacion = "Funcion  Solicita_existencia_autorizacion_flujo_trabajo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Estado_existencia_autorizacion = "NO"
                Solicita_existencia_autorizacion = "YES"
                Exit Function
            Else
                Estado_existencia_autorizacion = "YES"
                Solicita_existencia_autorizacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_autorizacion = "Inconsistencia general funcion Solicita_existencia_autorizacion " & ex.Message
        End Try
    End Function
End Class
