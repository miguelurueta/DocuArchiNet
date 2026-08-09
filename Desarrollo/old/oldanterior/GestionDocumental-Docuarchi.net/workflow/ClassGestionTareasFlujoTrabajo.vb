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
Imports AjaxControlToolkit
Imports System.Web.Script.Serialization
Imports System.Security.Cryptography.X509Certificates
Imports GestionDocumental_Docuarchi.net.Class_config_general_service

Public Structure stru_ruta
    Dim id_ruta As Integer
    Dim nombre_ruta As String
End Structure
Public Structure stru_listado_actividades
    Dim Id_Actividad As Integer
    Dim Nombre_Actividad As String
End Structure



Public Class ClassGestionTareasFlujoTrabajo
    Function Solicita_rutas_workflow(ByRef stru_rutas() As stru_ruta) As String
        Try
            Erase stru_rutas
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("rutas_workflow")
            Dim sql_consulta As String = "Select id_Ruta,Nombre_Ruta from rutas_workflow where Estado_Ruta=1"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_rutas_workflow = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_rutas_workflow = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_rutas(i)
                    stru_rutas(i).id_ruta = Datset.Tables(0).Rows(i).Item(0)
                    stru_rutas(i).nombre_ruta = Datset.Tables(0).Rows(i).Item(1)
                Next
                Solicita_rutas_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_rutas_workflow = "Inconsistencia genera función Solicita_rutas_workflow " & ex.Message
        End Try
    End Function
    Function Lista_rutas_workflow_interface(ByVal stru_rutas() As stru_ruta,
                                            ByRef ref_droplist As DropDownList,
                                            ByRef up_dat As UpdatePanel) As String
        Try
            ref_droplist.Items.Clear()
            If stru_rutas Is Nothing Then
                Lista_rutas_workflow_interface = "YES"
                Exit Function
            End If
            Dim ilist_ As New ListItem("", "")
            ref_droplist.Items.Add(ilist_)
            For i As Integer = 0 To stru_rutas.Length - 1
                Dim ilist As New ListItem(stru_rutas(i).nombre_ruta, stru_rutas(i).id_ruta)
                ref_droplist.Items.Add(ilist)
            Next
            Lista_rutas_workflow_interface = "YES"
            Exit Function
        Catch ex As Exception
            Lista_rutas_workflow_interface = "Inconsistencia general función Lista_rutas_workflow_interface " & ex.Message
        Finally
            up_dat.Update()
        End Try
    End Function
    Function Solicita_listado_actividades(ByVal id_ruta As Integer,
                                          ByRef stru_listado() As stru_listado_actividades) As String
        Try
            Erase stru_listado
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("listado_actividades_workflow")
            Dim sql_consulta As String = "Select Id_Actividad,Nombre_Actividad from listado_actividades_workflow where Rutas_Workflow_id_Ruta=" & id_ruta &
                " order by Nombre_Actividad"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_listado_actividades = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_listado_actividades = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_listado(i)
                    stru_listado(i).Id_Actividad = Datset.Tables(0).Rows(i).Item(0)
                    stru_listado(i).Nombre_Actividad = Datset.Tables(0).Rows(i).Item(1)
                Next
                Solicita_listado_actividades = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_listado_actividades = "Inconsistencia general función Solicita_listado_actividades " & ex.Message
        End Try
    End Function
    Function Lista_actividades_Interface(ByVal stru_listado() As stru_listado_actividades,
                                            ByRef ref_droplist As DropDownList,
                                            ByRef up_dat As UpdatePanel) As String
        Try
            ref_droplist.Items.Clear()
            If stru_listado Is Nothing Then
                Lista_actividades_Interface = "YES"
                Exit Function
            End If
            For i As Integer = 0 To stru_listado.Length - 1
                Dim ilist As New ListItem(stru_listado(i).Nombre_Actividad, stru_listado(i).Id_Actividad)
                ref_droplist.Items.Add(ilist)
            Next
            Lista_actividades_Interface = "YES"
            Exit Function
        Catch ex As Exception
            Lista_actividades_Interface = "Inconsistencia general función Lista_actividades_Interface " & ex.Message
        Finally
            up_dat.Update()
        End Try
    End Function

    Function Default_Generar_Ceros_Recibo(ByRef Nombre_Recibo As String, ByVal parametro_letra As String) As String
        Try
            Dim ceros3 As String
            ceros3 = ""
            Select Case Len(Nombre_Recibo)
                Case "1"
                    ceros3 = "00000000"
                Case "2"
                    ceros3 = "0000000"
                Case "3"
                    ceros3 = "000000"
                Case "4"
                    ceros3 = "00000"
                Case "5"
                    ceros3 = "0000"
                Case "6"
                    ceros3 = "000"
                Case "7"
                    ceros3 = "00"
                Case "8"
                    ceros3 = "0"
            End Select
            Nombre_Recibo = parametro_letra & ceros3 & Nombre_Recibo
            Default_Generar_Ceros_Recibo = "YES"
        Catch ex As Exception
            Default_Generar_Ceros_Recibo = "Error formateando recibo " & ex.ToString()
        End Try
    End Function
    Function Solicita_datos_estructura_consulta_recibo_ruta_interfaz_SII(ByVal Parameter As Object,
                                                                         ByRef parameter_gestion As Object) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Función que solicita la estructura de un radicado SII para regitro ruta
        '          
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'parameter           : Representa la estructura del recibo a consultar
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Class_ConSultaRecibo_Service  : Retorna la estructura con los datos del recibo para la interfaz
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-05
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_ConSultaRecibo As New Class_ConSultaRecibo
            Result = Class_ConSultaRecibo.Solicita_datos_estructura_consulta_recibo_ruta_interfaz_SII(Parameter,
                                                                                                      parameter_gestion.Class_parram_consultarRecibo,
                                                                                                      parameter_gestion.Class_parram_consultarRadicado)
            If Result <> "YES" Then
                Solicita_datos_estructura_consulta_recibo_ruta_interfaz_SII = Result
                Exit Function
            End If
            Dim Class_ws_tipotramitesii_determina_gabinete As New Class_ws_tipotramitesii_determina_gabinete
            Dim Class_service_ilist_drowlist = New List(Of Class_service_ilist_drowlist)
            Result = Class_ws_tipotramitesii_determina_gabinete.Solicita_class_lista_tramites_SII(0, Class_service_ilist_drowlist)
            parameter_gestion.Class_service_ilist_drowlist = Class_service_ilist_drowlist
            If Result <> "YES" Then
                Solicita_datos_estructura_consulta_recibo_ruta_interfaz_SII = Result
                Exit Function
            End If
            Class_service_ilist_drowlist = New List(Of Class_service_ilist_drowlist)
            Dim Class_Listado_Actividades_workflow As New Class_Listado_Actividades_workflow
            Result = Class_Listado_Actividades_workflow.Solicita_class_actividades_workflow_ruta(Class_service_ilist_drowlist)
            parameter_gestion.Class_service_ilist_drowlist_actividad = Class_service_ilist_drowlist
            Solicita_datos_estructura_consulta_recibo_ruta_interfaz_SII = Result
            Exit Function
        Catch ex As Exception
            Solicita_datos_estructura_consulta_recibo_ruta_interfaz_SII = "Inconistencia general función Solicita_datos_estructura_consulta_recibo_ruta_interfaz_SII " & ex.Message
        End Try
    End Function
    Function Solicita_datos_estructura_consulta_recibo_flujo_interfaz_SII(ByVal Parameter As Object,
                                                                          ByRef parameter_gestion As Class_ConSultaRecibo_Service) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Función que solicita la estructura de un radicado SII para regitro flujo
        '          
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'parameter           : Representa la estructura del recibo a consultar
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Class_ConSultaRecibo_Service  : Retorna la estructura con los datos del recibo para la interfaz
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-05
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            '---------/// Consulta recibo SII y Radicado SII proceso de integración   ////Depende respuesta de integración////
            Dim Class_ConSultaRecibo As New Class_ConSultaRecibo
            Result = Class_ConSultaRecibo.Solicita_datos_estructura_consulta_recibo_ruta_interfaz_SII(Parameter,
                                                                                                      parameter_gestion.Class_parram_consultarRecibo,
                                                                                                      parameter_gestion.Class_parram_consultarRadicado)
            If Result <> "YES" Then
                Solicita_datos_estructura_consulta_recibo_flujo_interfaz_SII = Result
                Exit Function
            End If
            '---////Solicita la lista de tramites SII homologados en la tabla  servicio
            Dim Class_ws_tipotramitesii_determina_gabinete As New Class_ws_tipotramitesii_determina_gabinete
            Dim Class_service_ilist_drowlist = New List(Of Class_service_ilist_drowlist)
            Result = Class_ws_tipotramitesii_determina_gabinete.SolicitaListaTramiteSII(0,
                                                                                        parameter_gestion.Class_parram_consultarRecibo.tipotramite,
                                                                                        Class_service_ilist_drowlist)
            parameter_gestion.Class_service_ilist_drowlist = Class_service_ilist_drowlist
            If Result <> "YES" Then
                Solicita_datos_estructura_consulta_recibo_flujo_interfaz_SII = Result
                Exit Function
            End If
            '---////Solicita la identificación del tramite SII en la tabla tipo doc entrante  ----/////"
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim id_tipo_doc_entrante As Integer = 0
            Result = Class_tipo_doc_entrante.Solicita_identificacion_tipo_documento_entrante_externo_nombre(parameter_gestion.Class_parram_consultarRecibo.tipotramite,
                                                                                                            id_tipo_doc_entrante)
            If Result <> "YES" Then
                Solicita_datos_estructura_consulta_recibo_flujo_interfaz_SII = Result
                Exit Function
            End If
            '---////Solicita la identificación del flujo de trabajo relacionado al tipo tramite  ----/////"
            Dim id_flujo As Integer = 0
            Dim Class_ra_relacion_tramite_flujo_wokflow As New Class_ra_relacion_tramite_flujo_wokflow
            Dim stru_rutas() As stru_ruta = Nothing
            Result = Class_ra_relacion_tramite_flujo_wokflow.Solicita_id_flujo_relacion_flujo_tramite(id_tipo_doc_entrante,
                                                                                                      id_flujo)
            If Result <> "YES" Then
                Solicita_datos_estructura_consulta_recibo_flujo_interfaz_SII = Result
                Exit Function
            End If
            parameter_gestion.id_flujo = id_flujo
            If id_flujo = 0 Then
                Solicita_datos_estructura_consulta_recibo_flujo_interfaz_SII = "El recibo (" & Parameter & ") relacionado al servicio (" & parameter_gestion.Class_parram_consultarRecibo.tipotramite &
                    ")  no esta relacionado a un flujo de trabajo Imposible continuar"
                Exit Function
            End If
            '---////Solicita la lista de  flujos de trabajo relacionados con el tramite ----/////"
            Class_service_ilist_drowlist = New List(Of Class_service_ilist_drowlist)
            Result = Class_ra_relacion_tramite_flujo_wokflow.Solicita_relaciones_flujo_trabajo_tramite(id_tipo_doc_entrante,
                                                                                                       1,
                                                                                                       Class_service_ilist_drowlist)
            parameter_gestion.Class_service_ilist_drowlist_flujos = Class_service_ilist_drowlist
            If Result <> "YES" Then
                Solicita_datos_estructura_consulta_recibo_flujo_interfaz_SII = Result
                Exit Function
            End If
            If Class_service_ilist_drowlist.Count = 0 Then
                Solicita_datos_estructura_consulta_recibo_flujo_interfaz_SII = "El recibo (" & Parameter & ") relacionado al servicio (" & parameter_gestion.Class_parram_consultarRecibo.tipotramite &
                    ")  no tiene relacionado un flujo de trabajo al tipo tramite (" & id_tipo_doc_entrante & ")  Imposible continuar"
                Exit Function
            End If
            '---////Solicita actvidades de inicio relacionadas a flujo de trabajo  ----/////"
            Class_service_ilist_drowlist = New List(Of Class_service_ilist_drowlist)
            Dim Class_wf_registro_actividaes_flujos_trabajo As New Class_wf_registro_actividaes_flujos_trabajo
            Result = Class_wf_registro_actividaes_flujos_trabajo.Solicita_actividades_workflow_flujo_inicio(1,
                                                                                                            id_flujo,
                                                                                                            Class_service_ilist_drowlist)
            parameter_gestion.Class_service_ilist_drowlist_actividad_flujo = Class_service_ilist_drowlist
            Solicita_datos_estructura_consulta_recibo_flujo_interfaz_SII = Result
            Exit Function
        Catch ex As Exception
            Solicita_datos_estructura_consulta_recibo_flujo_interfaz_SII = "Inconistencia general función Solicita_datos_estructura_consulta_recibo_flujo_interfaz_SII " & ex.Message
        End Try
    End Function
    Function Solicita_datos_registro_flujo_virtual_sii(ByVal id_usuario_workflow As Integer,
                                                       ByVal class_row_virtual_sii As IList(Of class_row_virtual_sii),
                                                       ByRef Class_ConSultaRecibo_Service As Class_ConSultaRecibo_Service) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura para el registro de un flujo virtual en workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'id_usuario_workflow : Representa la identificación del usuario workflow
        'class_row_rue_sii   : Representa la estructura del registro rue
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Class_ConSultaRecibo_Service  : Retorna la estructura del registro
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-01-03
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Class_ConSultaRecibo_Service.id_usuario_workflow_transacion = id_usuario_workflow
            '---------/// Consulta recibo SII y Radicado SII proceso de integración   ////Depende respuesta de integración////
            Dim Class_ConsultarRadicado_sii As New Class_ConsultarRadicado_sii
            Result = Class_ConsultarRadicado_sii.Solicita_datos_estructura_radicado_recibo_SII(class_row_virtual_sii(0).CODIGOBARRAS,
                                                                                               Class_ConSultaRecibo_Service.Class_parram_consultarRecibo,
                                                                                               Class_ConSultaRecibo_Service.Class_parram_consultarRadicado)
            If Result <> "YES" Then
                Solicita_datos_registro_flujo_virtual_sii = Result
                Exit Function
            End If
            class_row_virtual_sii(0).RECIBO = Class_ConSultaRecibo_Service.Class_parram_consultarRadicado.recibo
            '---------/// Consulta la identificación de la ruta default   ////////
            Dim Class_worflow_rutas As New Class_worflow_rutas
            Result = Class_worflow_rutas.Solicita_id_ruta_nonbre_ruta_workflow("",
                                                                               Class_ConSultaRecibo_Service.id_ruta)
            If Result <> "YES" Then
                Solicita_datos_registro_flujo_virtual_sii = Result
                Exit Function
            End If
            Class_ConSultaRecibo_Service.id_usuario_workflow = id_usuario_workflow
            Dim class_row_virtual_sii_ = New List(Of class_row_virtual_sii)
            class_row_virtual_sii_.Add(class_row_virtual_sii(0))
            Class_ConSultaRecibo_Service.class_row_virtual_sii = class_row_virtual_sii_
            '---------/// Solicita la identificación del grupo al que pertenece el usuario   ////Sin grupo se sale del curso////
            Dim class_workflow_usuario As New ClassWorkflowUsuario
            Dim id_grupo_workflow As Integer = 0
            Result = class_workflow_usuario.Solicita_id_grupo_usuario_workflow(id_usuario_workflow,
                                                                               id_grupo_workflow)
            If Result <> "YES" Then
                Solicita_datos_registro_flujo_virtual_sii = Result
                Exit Function
            End If
            Class_ConSultaRecibo_Service.id_grupo_workflow = id_grupo_workflow
            Dim id_actividad_workflow As Integer = 0
            Dim Class_grupos_workflow As New Class_grupos_workflow
            '---------/// Solicita la identificación de la actividad relacionda al grupo   ////-Sin actividad se sale del curso////
            Result = Class_grupos_workflow.Solicita_id_actividad_grupo_workflow(id_grupo_workflow,
                                                                                id_actividad_workflow)
            If Result <> "YES" Then
                Solicita_datos_registro_flujo_virtual_sii = Result
                Exit Function
            End If
            Class_ConSultaRecibo_Service.id_actividad_workflow = id_actividad_workflow
            '---------/// Solicita nombre tramite por codigo rue    ////Relación del codigo rue con el tramite SII de la tabla tipo_doc_entrante////
            Dim Class_ws_tipotramitesii_determina_gabinete As New Class_ws_tipotramitesii_determina_gabinete
            Dim nombre_tramite As String = ""
            Result = Class_ws_tipotramitesii_determina_gabinete.Valida_registro_nombre_tramite_sii(Class_ConSultaRecibo_Service.Class_parram_consultarRecibo.tipotramite,
                                                                                                   nombre_tramite)
            If Result <> "YES" Then
                Solicita_datos_registro_flujo_virtual_sii = Result
                Exit Function
            End If
            Dim Class_service_ilist_drowlist = New List(Of Class_service_ilist_drowlist)
            '---------/// Determina si el codigo rue tiene relacionada un tramite de srvicio en el SII      ////Si no hay relación el sistema lista todos los tramites////
            If nombre_tramite = "" Then
                Result = Class_ws_tipotramitesii_determina_gabinete.Solicita_class_lista_tramites_SII(1,
                                                                                                      Class_service_ilist_drowlist)
                If Result <> "YES" Then
                    Solicita_datos_registro_flujo_virtual_sii = Result
                    Exit Function
                End If
                Class_ConSultaRecibo_Service.Class_service_ilist_drowlist = Class_service_ilist_drowlist
                Solicita_datos_registro_flujo_virtual_sii = "YES"
                Exit Function
            End If
            '---------/// Solicita la lista de tramite del tramite relacionado al codigo RUE  con el parametro nombre tramite   ////-////
            Class_service_ilist_drowlist = New List(Of Class_service_ilist_drowlist)
            Result = Class_ws_tipotramitesii_determina_gabinete.Solicita_class_lista_tramites_rue_default_SII(0,
                                                                                                              nombre_tramite,
                                                                                                              Class_service_ilist_drowlist)
            If Result <> "YES" Then
                Solicita_datos_registro_flujo_virtual_sii = Result
                Exit Function
            End If
            Class_ConSultaRecibo_Service.Class_service_ilist_drowlist = Class_service_ilist_drowlist
            '---------/// Solicita la identificación del tipo de tramite con el nombre del tramite   ////-////
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim id_tipo_doc_entrante As Integer = 0
            Result = Class_tipo_doc_entrante.RetornaIdTipoTramitePorNombreTipo(nombre_tramite,
                                                                                     id_tipo_doc_entrante)
            If Result <> "YES" Then
                Solicita_datos_registro_flujo_virtual_sii = Result
                Exit Function
            End If
            '---------/// Solicita la identificación del flujo de trabajo en la relación con el tipo tramite   ////-////
            Dim Class_ra_relacion_tramite_flujo_wokflow As New Class_ra_relacion_tramite_flujo_wokflow
            Dim id_flujo_trabajo As Integer = 0
            Result = Class_ra_relacion_tramite_flujo_wokflow.Solicita_id_flujo_relacion_flujo_tramite(id_tipo_doc_entrante,
                                                                                                      id_flujo_trabajo)
            If Result <> "YES" Then
                Solicita_datos_registro_flujo_virtual_sii = Result
                Exit Function
            End If
            Class_ConSultaRecibo_Service.id_flujo = id_flujo_trabajo
            '---------/// Valida si el tramite esta relacionado a flujo   ////-////
            Dim Class_wf_registro_actividaes_flujos_trabajo As New Class_wf_registro_actividaes_flujos_trabajo
            Dim id_actividad_flujo As Integer = 0
            If id_flujo_trabajo = 1 Then
                '---------/// Solicita la identiifcación de actvidad del flujo  de trabajo relacionada a la actividad workflow     ////-Solo lista actividades de inicio////
                Result = Class_wf_registro_actividaes_flujos_trabajo.Solicita_actividad_usuario_flujo_trabajo(id_actividad_workflow,
                                                                                                              id_flujo_trabajo,
                                                                                                              id_actividad_flujo)
                If Result <> "YES" Then
                    Solicita_datos_registro_flujo_virtual_sii = Result
                    Exit Function
                End If
                Class_ConSultaRecibo_Service.id_actividad_flujo = id_actividad_flujo
                Dim Class_flujo_trabajo_workflow As New Class_flujo_trabajo_workflow
                Class_service_ilist_drowlist = New List(Of Class_service_ilist_drowlist)
                '---------/// Solicita la lista flujo de trabajo del flujo de trabajo     ////En estrucutura de lista ////
                Result = Class_flujo_trabajo_workflow.Solicita_lista_flujo_trabajo_id_flujo(0,
                                                                                           id_flujo_trabajo,
                                                                                           Class_service_ilist_drowlist)
                If Result <> "YES" Then
                    Solicita_datos_registro_flujo_virtual_sii = Result
                    Exit Function
                End If
                Class_ConSultaRecibo_Service.Class_service_ilist_drowlist_flujos = Class_service_ilist_drowlist
                '---------/// Solicita la lista actvidades del flujo  de trabajo relacionada a la actividad workflow     ////-Solo lista actividades de inicio////
                Class_service_ilist_drowlist = New List(Of Class_service_ilist_drowlist)
                Result = Class_wf_registro_actividaes_flujos_trabajo.Solicita_actividades_usuario_flujo_trabajo(0,
                                                                                                               id_actividad_workflow,
                                                                                                               id_flujo_trabajo,
                                                                                                               Class_service_ilist_drowlist)
                If Result <> "YES" Then
                    Solicita_datos_registro_flujo_virtual_sii = Result
                    Exit Function
                End If
                Class_ConSultaRecibo_Service.Class_service_ilist_drowlist_actividad_flujo = Class_service_ilist_drowlist
            Else
                '---------/// Solicita la lista actvidades de ruta     ////-////
                Dim Class_Listado_Actividades_workflow As New Class_Listado_Actividades_workflow
                Class_service_ilist_drowlist = New List(Of Class_service_ilist_drowlist)
                Result = Class_Listado_Actividades_workflow.Solicita_class_actividades_workflow_ruta_default_actividad_usuario(0,
                                                                                                                               id_actividad_workflow,
                                                                                                                               Class_service_ilist_drowlist)
                If Result <> "YES" Then
                    Solicita_datos_registro_flujo_virtual_sii = Result
                    Exit Function
                End If
                Class_ConSultaRecibo_Service.Class_service_ilist_drowlist_rutas = Class_service_ilist_drowlist
            End If
            Solicita_datos_registro_flujo_virtual_sii = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_datos_registro_flujo_virtual_sii = "Inconsistencia general función Solicita_datos_registro_flujo_virtual_sii " & ex.Message
        End Try
    End Function
    Function Solicita_datos_registro_flujo_rue_sii(ByVal id_usuario_workflow As Integer,
                                                   ByVal class_row_rue_sii As IList(Of class_row_rue_sii),
                                                   ByRef Class_ConSultaRecibo_Service As Class_ConSultaRecibo_Service) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura para el registro de un fujo rue en workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'id_usuario_workflow : Representa la identificación del usuario workflow
        'class_row_rue_sii   : Representa la estructura del registro rue
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Class_ConSultaRecibo_Service  : Retorna la estructura del registro
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-24
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Class_ConSultaRecibo_Service.id_usuario_workflow_transacion = id_usuario_workflow
            '---------/// Consulta recibo SII y Radicado SII proceso de integración   ////Depende respuesta de integración////
            Dim Class_ConSultaRecibo As New Class_ConSultaRecibo
            Result = Class_ConSultaRecibo.Solicita_datos_estructura_consulta_recibo_ruta_interfaz_SII(class_row_rue_sii(0).RECIBO,
                                                                                                      Class_ConSultaRecibo_Service.Class_parram_consultarRecibo,
                                                                                                      Class_ConSultaRecibo_Service.Class_parram_consultarRadicado)
            If Result <> "YES" Then
                Solicita_datos_registro_flujo_rue_sii = Result
                Exit Function
            End If
            '---------/// Consulta la identificación de la ruta default   ////////
            Dim Class_worflow_rutas As New Class_worflow_rutas
            Result = Class_worflow_rutas.Solicita_id_ruta_nonbre_ruta_workflow("",
                                                                               Class_ConSultaRecibo_Service.id_ruta)
            If Result <> "YES" Then
                Solicita_datos_registro_flujo_rue_sii = Result
                Exit Function
            End If
            Class_ConSultaRecibo_Service.id_usuario_workflow = id_usuario_workflow
            Dim class_row_rue_si_ = New List(Of class_row_rue_sii)
            class_row_rue_si_.Add(class_row_rue_sii(0))
            Class_ConSultaRecibo_Service.class_row_rue_sii = class_row_rue_si_
            '---------/// Solicita la identificación del grupo al que pertenece el usuario   ////Sin grupo se sale del curso////
            Dim class_workflow_usuario As New ClassWorkflowUsuario
            Dim id_grupo_workflow As Integer = 0
            Result = class_workflow_usuario.Solicita_id_grupo_usuario_workflow(id_usuario_workflow,
                                                                               id_grupo_workflow)
            If Result <> "YES" Then
                Solicita_datos_registro_flujo_rue_sii = Result
                Exit Function
            End If
            Class_ConSultaRecibo_Service.id_grupo_workflow = id_grupo_workflow
            Dim id_actividad_workflow As Integer = 0
            Dim Class_grupos_workflow As New Class_grupos_workflow
            '---------/// Solicita la identificación de la actividad relacionda al grupo   ////-Sin actividad se sale del curso////
            Result = Class_grupos_workflow.Solicita_id_actividad_grupo_workflow(id_grupo_workflow,
                                                                                id_actividad_workflow)
            If Result <> "YES" Then
                Solicita_datos_registro_flujo_rue_sii = Result
                Exit Function
            End If
            Class_ConSultaRecibo_Service.id_actividad_workflow = id_actividad_workflow
            '---------/// Solicita nombre tramite por codigo rue    ////Relación del codigo rue con el tramite SII de la tabla tipo_doc_entrante////
            Dim Class_ws_tipotramitesii_determina_gabinete As New Class_ws_tipotramitesii_determina_gabinete
            Dim nombre_tramite As String = ""
            Result = Class_ws_tipotramitesii_determina_gabinete.Solicita_nombre_tramite_codigo_rue(class_row_rue_sii(0).CODIGOSERVCIORUE,
                                                                                                   nombre_tramite)
            If Result <> "YES" Then
                Solicita_datos_registro_flujo_rue_sii = Result
                Exit Function
            End If
            Dim Class_service_ilist_drowlist = New List(Of Class_service_ilist_drowlist)
            '---------/// Determina si el codigo rue tiene relacionada un tramite de srvicio en el SII      ////Si no hay relación el sistema lista todos los tramites////
            If nombre_tramite = "" Then
                Result = Class_ws_tipotramitesii_determina_gabinete.Solicita_class_lista_tramites_SII(1,
                                                                                                      Class_service_ilist_drowlist)
                If Result <> "YES" Then
                    Solicita_datos_registro_flujo_rue_sii = Result
                    Exit Function
                End If
                Class_ConSultaRecibo_Service.Class_service_ilist_drowlist = Class_service_ilist_drowlist
                Solicita_datos_registro_flujo_rue_sii = "YES"
                Exit Function
            End If
            '---------/// Solicita la lista de tramite del tramite relacionado al codigo RUE  con el parametro nombre tramite   ////-////
            Class_service_ilist_drowlist = New List(Of Class_service_ilist_drowlist)
            Result = Class_ws_tipotramitesii_determina_gabinete.Solicita_class_lista_tramites_rue_default_SII(0,
                                                                                                              nombre_tramite,
                                                                                                              Class_service_ilist_drowlist)
            If Result <> "YES" Then
                Solicita_datos_registro_flujo_rue_sii = Result
                Exit Function
            End If
            Class_ConSultaRecibo_Service.Class_service_ilist_drowlist = Class_service_ilist_drowlist
            '---------/// Solicita la identificación del tipo de tramite con el nombre del tramite   ////-////
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim id_tipo_doc_entrante As Integer = 0
            Result = Class_tipo_doc_entrante.RetornaIdTipoTramitePorNombreTipo(nombre_tramite,
                                                                               id_tipo_doc_entrante)
            If Result <> "YES" Then
                Solicita_datos_registro_flujo_rue_sii = Result
                Exit Function
            End If
            '---------/// Solicita la identificación del flujo de trabajo en la relación con el tipo tramite   ////-////
            Dim Class_ra_relacion_tramite_flujo_wokflow As New Class_ra_relacion_tramite_flujo_wokflow
            Dim id_flujo_trabajo As Integer = 0
            Result = Class_ra_relacion_tramite_flujo_wokflow.Solicita_id_flujo_relacion_flujo_tramite(id_tipo_doc_entrante,
                                                                                                      id_flujo_trabajo)
            If Result <> "YES" Then
                Solicita_datos_registro_flujo_rue_sii = Result
                Exit Function
            End If
            Class_ConSultaRecibo_Service.id_flujo = id_flujo_trabajo
            '---------/// Valida si el tramite esta relacionado a flujo   ////-////
            Dim Class_wf_registro_actividaes_flujos_trabajo As New Class_wf_registro_actividaes_flujos_trabajo
            Dim id_actividad_flujo As Integer = 0
            If id_flujo_trabajo = 1 Then
                '---------/// Solicita la identiifcación de actvidad del flujo  de trabajo relacionada a la actividad workflow     ////-Solo lista actividades de inicio////
                Result = Class_wf_registro_actividaes_flujos_trabajo.Solicita_actividad_usuario_flujo_trabajo(id_actividad_workflow,
                                                                                                              id_flujo_trabajo,
                                                                                                              id_actividad_flujo)
                If Result <> "YES" Then
                    Solicita_datos_registro_flujo_rue_sii = Result
                    Exit Function
                End If
                Class_ConSultaRecibo_Service.id_actividad_flujo = id_actividad_flujo
                Dim Class_flujo_trabajo_workflow As New Class_flujo_trabajo_workflow
                Class_service_ilist_drowlist = New List(Of Class_service_ilist_drowlist)
                '---------/// Solicita la lista flujo de trabajo del flujo de trabajo     ////En estrucutura de lista ////
                Result = Class_flujo_trabajo_workflow.Solicita_lista_flujo_trabajo_id_flujo(0,
                                                                                           id_flujo_trabajo,
                                                                                           Class_service_ilist_drowlist)
                If Result <> "YES" Then
                    Solicita_datos_registro_flujo_rue_sii = Result
                    Exit Function
                End If
                Class_ConSultaRecibo_Service.Class_service_ilist_drowlist_flujos = Class_service_ilist_drowlist
                '---------/// Solicita la lista actvidades del flujo  de trabajo relacionada a la actividad workflow     ////-Solo lista actividades de inicio////
                Class_service_ilist_drowlist = New List(Of Class_service_ilist_drowlist)
                Result = Class_wf_registro_actividaes_flujos_trabajo.Solicita_actividades_usuario_flujo_trabajo(0,
                                                                                                               id_actividad_workflow,
                                                                                                               id_flujo_trabajo,
                                                                                                               Class_service_ilist_drowlist)
                If Result <> "YES" Then
                    Solicita_datos_registro_flujo_rue_sii = Result
                    Exit Function
                End If
                Class_ConSultaRecibo_Service.Class_service_ilist_drowlist_actividad_flujo = Class_service_ilist_drowlist
            Else
                '---------/// Solicita la lista actvidades de ruta     ////-////
                Dim Class_Listado_Actividades_workflow As New Class_Listado_Actividades_workflow
                Class_service_ilist_drowlist = New List(Of Class_service_ilist_drowlist)
                Result = Class_Listado_Actividades_workflow.Solicita_class_actividades_workflow_ruta_default_actividad_usuario(0,
                                                                                                                               id_actividad_workflow,
                                                                                                                               Class_service_ilist_drowlist)
                If Result <> "YES" Then
                    Solicita_datos_registro_flujo_rue_sii = Result
                    Exit Function
                End If
                Class_ConSultaRecibo_Service.Class_service_ilist_drowlist_rutas = Class_service_ilist_drowlist
            End If
            Solicita_datos_registro_flujo_rue_sii = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_datos_registro_flujo_rue_sii = "Inconsistencia general función Solicita_datos_registro_flujo_rue_sii " & ex.Message
        End Try
    End Function
    Function Consulta_datos_recibo_sii(ByRef pag As Page) As String
        Try
            Dim TextBox_matricula As TextBox = pag.FindControl("TextBox_matricula")
            Dim UpdatePanel_text_matricula As UpdatePanel = pag.FindControl("UpdatePanel_text_matricula")
            Dim TextBox_razon_social As TextBox = pag.FindControl("TextBox_razon_social")
            Dim UpdatePanel_text_Razon_social As UpdatePanel = pag.FindControl("UpdatePanel_text_Razon_social")
            Dim DropDownList_tramites As DropDownList = pag.FindControl("DropDownList_tramites")
            Dim UpdatePanel_drow_tramites As UpdatePanel = pag.FindControl("UpdatePanel_drow_tramites")
            Dim TextBox_recibo_caja As TextBox = pag.FindControl("TextBox_recibo_caja")
            Dim DropDownList_ante_pone As DropDownList = pag.FindControl("DropDownList_ante_pone")
            Dim DropDownList_rutas As DropDownList = pag.FindControl("DropDownList_rutas")
            Dim UpdatePanel_drow_rutas As UpdatePanel = pag.FindControl("UpdatePanel_drow_rutas")
            Dim DropDownList_actividades As DropDownList = pag.FindControl("DropDownList_actividades")
            Dim UpdatePanel_drow_actividades As UpdatePanel = pag.FindControl("UpdatePanel_drow_actividades")
            Dim DropDownList_usurios As DropDownList = pag.FindControl("DropDownList_usurios")
            Dim UpdatePanel_drow_usuarios As UpdatePanel = pag.FindControl("UpdatePanel_drow_usuarios")
            Dim TextBox_codigo_barras As TextBox = pag.FindControl("TextBox_codigo_barras")
            Dim UpdatePanel_text_codigo_barras As UpdatePanel = pag.FindControl("UpdatePanel_text_codigo_barras")
            Dim stru_consulta_recibo As consultarRecibo = Nothing
            Dim recibo As String = TextBox_recibo_caja.Text
            Dim Result As String = Me.Default_Generar_Ceros_Recibo(recibo, DropDownList_ante_pone.Text)
            If Result <> "YES" Then
                Consulta_datos_recibo_sii = Result
                Exit Function
            End If
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim id_tipo_doc_entrante As Integer = 0
            Dim stru_consulta_radicado As ConsultarRadicado_sii = Nothing
            Result = Me.Lista_datos_sii_recibo(recibo,
                                               stru_consulta_recibo,
                                               stru_consulta_radicado)
            If Result <> "YES" Then
                TextBox_matricula.Text = ""
                UpdatePanel_text_matricula.Update()
                TextBox_razon_social.Text = ""
                UpdatePanel_text_Razon_social.Update()
                DropDownList_tramites.Text = ""
                UpdatePanel_drow_tramites.Update()
                TextBox_codigo_barras.Text = ""
                UpdatePanel_text_codigo_barras.Update()
                DropDownList_rutas.Items.Clear()
                UpdatePanel_drow_rutas.Update()
                DropDownList_usurios.Items.Clear()
                UpdatePanel_drow_usuarios.Update()
                Consulta_datos_recibo_sii = Result
                Exit Function
            Else
                TextBox_codigo_barras.Text = stru_consulta_recibo.radicado
                Dim Temp_Nombre As String = ""
                TextBox_razon_social.Text = stru_consulta_recibo.nombre
                TextBox_matricula.Text = stru_consulta_radicado.matricula
                For i As Integer = 0 To DropDownList_tramites.Items.Count - 1
                    If DropDownList_tramites.Items(i).Value <> "" Then
                        Dim split_tramite() As String = DropDownList_tramites.Items(i).Value.ToString.Split("|")
                        If UCase(split_tramite(1)) = UCase(stru_consulta_recibo.tipotramite) Then
                            DropDownList_tramites.SelectedIndex = i
                            Exit For
                        End If
                    End If
                Next
                DropDownList_rutas.Items.Clear()
                UpdatePanel_drow_rutas.Update()
                DropDownList_usurios.Items.Clear()
                UpdatePanel_drow_usuarios.Update()
                DropDownList_actividades.Items.Clear()
                UpdatePanel_drow_actividades.Update()
                UpdatePanel_text_matricula.Update()
                UpdatePanel_text_Razon_social.Update()
                UpdatePanel_drow_tramites.Update()
                UpdatePanel_text_codigo_barras.Update()
                Dim split() As String = DropDownList_tramites.SelectedItem.Value.Split("|")
                Dim value As String = split(1)
                Result = Class_tipo_doc_entrante.Solicita_identificacion_tipo_documento_entrante_externo_nombre(value,
                                                                                                                id_tipo_doc_entrante)
                If Result <> "YES" Then
                    Consulta_datos_recibo_sii = Result
                    Exit Function
                End If
                Dim existencia_relacion_flujo As Integer = 0
                Dim Class_ra_relacion_tramite_flujo_wokflow As New Class_ra_relacion_tramite_flujo_wokflow
                Dim stru_rutas() As stru_ruta = Nothing
                Result = Class_ra_relacion_tramite_flujo_wokflow.Solicita_existencia_relacion_flujo_tramite(id_tipo_doc_entrante,
                                                                                                            existencia_relacion_flujo)
                If Result <> "YES" Then
                    Consulta_datos_recibo_sii = Result
                    Exit Function
                End If
                If existencia_relacion_flujo = 1 Then
                    Result = Class_ra_relacion_tramite_flujo_wokflow.Solicita_relaciones_flujo_trabajo_tramite(id_tipo_doc_entrante,
                                                                                                               1,
                                                                                                               DropDownList_rutas)
                    If Result <> "YES" Then
                        Consulta_datos_recibo_sii = Result
                        Exit Function
                    End If
                    UpdatePanel_drow_rutas.Update()
                Else
                    Result = Me.Solicita_rutas_workflow(stru_rutas)
                    If Result <> "YES" Then
                        Consulta_datos_recibo_sii = Result
                        Exit Function
                    End If
                    Result = Me.Lista_rutas_workflow_interface(stru_rutas, DropDownList_rutas, UpdatePanel_drow_rutas)
                    If Result <> "YES" Then
                        Consulta_datos_recibo_sii = Result
                        Exit Function
                    End If
                End If
                Consulta_datos_recibo_sii = "YES"
            End If
        Catch ex As Exception
            Consulta_datos_recibo_sii = "Inconsistencia general función Consulta_datos_recibo_sii " & ex.Message
        End Try
    End Function
    Function Lista_datos_sii_recibo(ByVal numero_recibo As String,
                                    ByRef stru_consulta_recibo As consultarRecibo,
                                    ByRef stru_consulta_radicado As ConsultarRadicado_sii) As String
        Try
            Dim Result As String = ""
            Dim codigo_empresa As String = ""
            Dim usuario_sii As String = ""
            Dim clave_usuario_sii As String = ""
            Dim UrlBase As String = ""
            Dim Class_ws_usuarioworkflowsii As New Class_ws_usuarioworkflowsii
            Result = Class_ws_usuarioworkflowsii.solicita_usuario_validacion_sii(codigo_empresa,
                                                                                 usuario_sii,
                                                                                 clave_usuario_sii)
            If Result <> "YES" Then
                Lista_datos_sii_recibo = Result
                Exit Function
            End If
            Result = Class_ws_usuarioworkflowsii.Solicita_url_nombrefuncion_restfull(UrlBase,
                                                                                     "solicitarToken")
            If Result <> "YES" Then
                Lista_datos_sii_recibo = Result
                Exit Function
            End If
            Dim stru_token As SolicitaToken = Nothing
            Dim Class_ClassResfull As New Class_ClassResfull
            Result = Class_ClassResfull.Solicitar_token_general(codigo_empresa,
                                                               usuario_sii,
                                                               clave_usuario_sii,
                                                               UrlBase & "solicitarToken",
                                                               stru_token)
            If Result <> "YES" Then
                Lista_datos_sii_recibo = Result
                Exit Function
            End If
            If stru_token.mensajeerror <> "" Then
                Lista_datos_sii_recibo = stru_token.mensajeerror
                Exit Function
            End If
            Dim Class_ConSultaRecibo As New Class_ConSultaRecibo
            Result = Class_ConSultaRecibo.ConSultaRecibo(stru_consulta_recibo,
                                                         stru_consulta_radicado,
                                                         numero_recibo,
                                                         codigo_empresa,
                                                         usuario_sii,
                                                         clave_usuario_sii,
                                                         UrlBase)
            If Result <> "YES" Then
                Lista_datos_sii_recibo = Result
                Exit Function
            End If
            If stru_consulta_recibo.codigoerror <> "0000" Then
                Lista_datos_sii_recibo = "Imposible encontrar el recibo codigo error del sii " & stru_consulta_recibo.codigoerror
                Exit Function
            End If
            If stru_consulta_recibo.recibo = "" Then
                Lista_datos_sii_recibo = "Imposible encontrar el recibo " & numero_recibo
                Exit Function
            End If
            Lista_datos_sii_recibo = "YES"
        Catch ex As Exception
            Lista_datos_sii_recibo = "Inconsistencia general función Lista_datos_sii_recibo " & ex.Message
        End Try
    End Function
    Function Solicita_codigo_camara(ByVal nombre_empresa As String,
                                    ByRef codigo_camara As String) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("empresa_gestion_documental")
            Dim Sql_consulta As String = "Select CODIGO_CAMARA from empresa_gestion_documental " &
                " where RAZON_SOCIAL_EMPRESA='" & nombre_empresa & "'"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_codigo_camara = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_codigo_camara = "Imposible encontrar el código de la empresa " & nombre_empresa
                Exit Function
            Else
                codigo_camara = Datset.Tables(0).Rows(0).Item(0)
                Solicita_codigo_camara = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_codigo_camara = "Inconsistencia general función Solicita_codigo_camara " & ex.Message
        End Try
    End Function
    Function Registra_tarea_ruta_SII(ByVal class_registro_tarea_ccv_SII As class_registro_tarea_ccv_SII) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Asgina que registra la tarea de un radicado SII en una ruta de trabajo
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'class_registro_tarea_ccv_SII : Representa la estructurac de los datos
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-04
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim fecha_ini As String = Date.Now
            Dim Result As String = ""
            Dim id_ruta As Integer = 0
            Dim id_flujo_trabajo As Integer = 0
            Dim id_actividad_flujo_trabajo As Integer = 0
            Dim id_usuario_flujo_trabajo As Integer = 0
            Dim id_usuario_workflow As Integer = 0
            Dim codigo_sede_sii As String = ""
            Dim id_actividad_workflow As Integer = 0
            Dim refclas_gestion_fechas As New ClassGestionFechas
            Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(fecha_ini)
            If Result <> "YES" Then
                Registra_tarea_ruta_SII = "Imposible formatear fecha " & Result
                Exit Function
            End If
            Dim Class_ws_tipotramitesii_determina_gabinete As New Class_ws_tipotramitesii_determina_gabinete
            Dim stru_tipos_tramite_sii As stru_tipos_tramite_sii = Nothing
            Result = Class_ws_tipotramitesii_determina_gabinete.Solicita_estructura_tramite_SII(class_registro_tarea_ccv_SII.id_tramite,
                                                                                                stru_tipos_tramite_sii)
            If Result <> "YES" Then
                Registra_tarea_ruta_SII = Result
                Exit Function
            End If
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim nombre_gabinete As String = ""
            Dim Class_configuracion_gabinete As New Class_configuracion_gabinete
            Result = Class_configuracion_gabinete.Solicita_nombre_gabinete(stru_tipos_tramite_sii.tipo_gabinete,
                                                                           nombre_gabinete)
            If Result <> "YES" Then
                Registra_tarea_ruta_SII = Result
                Exit Function
            End If
            Dim codigo_gabinete As Integer = Val(stru_tipos_tramite_sii.tipo_gabinete)
            Dim descripcion_tramite As String = stru_tipos_tramite_sii.descripcion_tramite
            Dim nombre_servicio As String = stru_tipos_tramite_sii.nombre_tramite
            Dim Class_worflow_rutas As New Class_worflow_rutas
            Dim nombre_ruta As String = ""
            Result = Class_worflow_rutas.Solicita_id_ruta_nonbre_ruta_workflow(nombre_ruta,
                                                                               id_ruta)
            If Result <> "YES" Then
                Registra_tarea_ruta_SII = Result
                Exit Function
            End If
            Dim existencia As String = ""
            Result = Verificar_existencia_recibo(class_registro_tarea_ccv_SII.recibo,
                                                 nombre_ruta,
                                                 existencia)
            If Result <> "YES" Then
                Registra_tarea_ruta_SII = Result
                Exit Function
            End If
            If existencia = "YES" Then
                Registra_tarea_ruta_SII = "El sistema encontro el recibo (" & class_registro_tarea_ccv_SII.recibo & ") en la ruta de trabajo (" & nombre_ruta &
                ") imposible continuar con el registro de la tarea"
                Exit Function
            End If
            id_actividad_workflow = class_registro_tarea_ccv_SII.id_actividad
            Result = Me.Solicitar_codigo_sede(id_actividad_workflow,
                                              codigo_sede_sii)
            If Result <> "YES" Then
                Registra_tarea_ruta_SII = Result
                Exit Function
            End If
            Dim ClassRaRelacionRadicadoExternoExpediente As New ClassRaRelacionRadicadoExternoExpediente
            Result = ClassRaRelacionRadicadoExternoExpediente.RegistraValidaRelacionExpedienteRadicadoExterno(class_registro_tarea_ccv_SII.matricula,
                                                                                                              nombre_gabinete,
                                                                                                              class_registro_tarea_ccv_SII.recibo)
            If Result <> "YES" Then
                Registra_tarea_ruta_SII = Result
                Exit Function
            End If
            Result = Me.Registra_flujo_workflow_externo(id_ruta,
                                                        id_actividad_workflow,
                                                        id_usuario_workflow,
                                                        codigo_gabinete,
                                                        nombre_gabinete,
                                                        nombre_ruta,
                                                        0,
                                                        id_flujo_trabajo,
                                                        fecha_ini,
                                                        class_registro_tarea_ccv_SII.recibo,
                                                        class_registro_tarea_ccv_SII.codigo_barras,
                                                        class_registro_tarea_ccv_SII.rscocial,
                                                        class_registro_tarea_ccv_SII.matricula,
                                                        codigo_sede_sii,
                                                        2,
                                                        nombre_servicio,
                                                        descripcion_tramite,
                                                        id_actividad_flujo_trabajo,
                                                        id_usuario_flujo_trabajo,
                                                        class_registro_tarea_ccv_SII.option_registra_log,
                                                        class_registro_tarea_ccv_SII.id_usuario_workflow_transacion,
                                                        class_registro_tarea_ccv_SII.codigo_rue)
            If Result <> "YES" Then
                Registra_tarea_ruta_SII = Result
                Exit Function
            Else
                Registra_tarea_ruta_SII = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Registra_tarea_ruta_SII = "Inconsistencia general funcion Registra_tarea_ruta_SII " & ex.Message
        End Try
    End Function
    Function Registra_tarea_flujo_SII(ByVal class_registro_tarea_ccv_SII As class_registro_tarea_ccv_SII) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Asgina que registra la tarea de un radicado SII en una ruta de trabajo
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'class_registro_tarea_ccv_SII : Representa la estructurac de los datos
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-04
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------ az
        Try
            Dim fecha_ini As String = Date.Now
            Dim Result As String = ""
            Dim id_ruta As Integer = 0
            Dim id_flujo_trabajo As Integer = 0
            Dim id_actividad_flujo_trabajo As Integer = 0
            Dim id_usuario_flujo_trabajo As Integer = 0
            Dim id_usuario_workflow As Integer = 0
            Dim codigo_sede_sii As String = ""
            Dim id_actividad_workflow As Integer = 0
            Dim refclas_gestion_fechas As New ClassGestionFechas
            Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(fecha_ini)
            If Result <> "YES" Then
                Registra_tarea_flujo_SII = "Imposible formatear fecha " & Result
                Exit Function
            End If
            Dim Class_ws_tipotramitesii_determina_gabinete As New Class_ws_tipotramitesii_determina_gabinete
            Dim stru_tipos_tramite_sii As stru_tipos_tramite_sii = Nothing
            Result = Class_ws_tipotramitesii_determina_gabinete.Solicita_estructura_tramite_SII(class_registro_tarea_ccv_SII.id_tramite,
                                                                                                stru_tipos_tramite_sii)
            If Result <> "YES" Then
                Registra_tarea_flujo_SII = Result
                Exit Function
            End If
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim nombre_gabinete As String = ""
            Dim Class_configuracion_gabinete As New Class_configuracion_gabinete
            Result = Class_configuracion_gabinete.Solicita_nombre_gabinete(stru_tipos_tramite_sii.tipo_gabinete,
                                                                           nombre_gabinete)
            If Result <> "YES" Then
                Registra_tarea_flujo_SII = Result
                Exit Function
            End If
            Dim codigo_gabinete As Integer = Val(stru_tipos_tramite_sii.tipo_gabinete)
            Dim descripcion_tramite As String = stru_tipos_tramite_sii.descripcion_tramite
            Dim nombre_servicio As String = stru_tipos_tramite_sii.nombre_tramite
            Dim Class_worflow_rutas As New Class_worflow_rutas
            Dim nombre_ruta As String = ""
            Result = Class_worflow_rutas.Solicita_id_ruta_nonbre_ruta_workflow(nombre_ruta,
                                                                               id_ruta)
            If Result <> "YES" Then
                Registra_tarea_flujo_SII = Result
                Exit Function
            End If
            id_actividad_flujo_trabajo = class_registro_tarea_ccv_SII.id_actividad
            Dim nombre_flujo_trabajo As String = ""
            Dim Class_flujo_trabajo_workflow As New Class_flujo_trabajo_workflow
            Result = Class_flujo_trabajo_workflow.Solicita_nombre_flujos_trabajo_workflow_id_flujo(class_registro_tarea_ccv_SII.id_flujo,
                                                                                                   1,
                                                                                                   nombre_flujo_trabajo)
            If Result <> "YES" Then
                Registra_tarea_flujo_SII = Result
                Exit Function
            End If
            Dim existencia As String = ""
            Result = Verificar_existencia_recibo(class_registro_tarea_ccv_SII.recibo,
                                                 nombre_ruta,
                                                 existencia)
            If Result <> "YES" Then
                Registra_tarea_flujo_SII = Result
                Exit Function
            End If
            If existencia = "YES" Then
                Registra_tarea_flujo_SII = "El sistema encontro el recibo (" & class_registro_tarea_ccv_SII.recibo & ") en el flujo (" & nombre_flujo_trabajo &
                ") imposible continuar con el registro de la tarea"
                Exit Function
            End If
            Dim Class_wf_registro_actividaes_flujos_trabajo As New Class_wf_registro_actividaes_flujos_trabajo
            Dim struregistro_actividaes_flujos_trabajo As struregistro_actividaes_flujos_trabajo = Nothing
            Result = Class_wf_registro_actividaes_flujos_trabajo.Solicita_estructura_actividad_flujo_trabajo(id_actividad_flujo_trabajo,
                                                                                                             struregistro_actividaes_flujos_trabajo)
            If Result <> "YES" Then
                Registra_tarea_flujo_SII = Result
                Exit Function
            End If
            id_actividad_workflow = struregistro_actividaes_flujos_trabajo.listado_actividades_workflow_Id_Actividad
            id_flujo_trabajo = struregistro_actividaes_flujos_trabajo.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO
            Result = Me.Solicitar_codigo_sede(id_actividad_workflow,
                                              codigo_sede_sii)
            If Result <> "YES" Then
                Registra_tarea_flujo_SII = Result
                Exit Function
            End If
            id_actividad_flujo_trabajo = class_registro_tarea_ccv_SII.id_actividad
            id_usuario_flujo_trabajo = class_registro_tarea_ccv_SII.id_usuario
            id_usuario_workflow = class_registro_tarea_ccv_SII.id_usuario
            Dim ClassRaRelacionRadicadoExternoExpediente As New ClassRaRelacionRadicadoExternoExpediente
            Result = ClassRaRelacionRadicadoExternoExpediente.RegistraValidaRelacionExpedienteRadicadoExterno(class_registro_tarea_ccv_SII.matricula,
                                                                                                              nombre_gabinete,
                                                                                                              class_registro_tarea_ccv_SII.recibo)
            If Result <> "YES" Then
                Registra_tarea_flujo_SII = Result
                Exit Function
            End If
            Result = Me.Registra_flujo_workflow_externo(id_ruta,
                                                        id_actividad_workflow,
                                                        id_usuario_workflow,
                                                        codigo_gabinete,
                                                        nombre_gabinete,
                                                        nombre_ruta,
                                                        0,
                                                        class_registro_tarea_ccv_SII.id_flujo,
                                                        fecha_ini,
                                                        class_registro_tarea_ccv_SII.recibo,
                                                        class_registro_tarea_ccv_SII.codigo_barras,
                                                        class_registro_tarea_ccv_SII.rscocial,
                                                        class_registro_tarea_ccv_SII.matricula,
                                                        codigo_sede_sii,
                                                        2,
                                                        nombre_servicio,
                                                        descripcion_tramite,
                                                        id_actividad_flujo_trabajo,
                                                        id_usuario_flujo_trabajo,
                                                        class_registro_tarea_ccv_SII.option_registra_log,
                                                        class_registro_tarea_ccv_SII.id_usuario_workflow_transacion,
                                                        class_registro_tarea_ccv_SII.codigo_rue)
            If Result <> "YES" Then
                Registra_tarea_flujo_SII = Result
                Exit Function
            Else
                Registra_tarea_flujo_SII = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Registra_tarea_flujo_SII = "Inconsistencia general funcion Registra_tarea_flujo_SII " & ex.Message
        End Try
    End Function
    Function Inicia_Registra_flujo_trabajo(ByRef pag As Page) As String
        Try
            Dim TextBox_matricula As TextBox = pag.FindControl("TextBox_matricula")
            Dim UpdatePanel_text_matricula As UpdatePanel = pag.FindControl("UpdatePanel_text_matricula")
            Dim TextBox_razon_social As TextBox = pag.FindControl("TextBox_razon_social")
            Dim UpdatePanel_text_Razon_social As UpdatePanel = pag.FindControl("UpdatePanel_text_Razon_social")
            Dim DropDownList_tramites As DropDownList = pag.FindControl("DropDownList_tramites")
            Dim UpdatePanel_drow_tramites As UpdatePanel = pag.FindControl("UpdatePanel_drow_tramites")
            Dim TextBox_recibo_caja As TextBox = pag.FindControl("TextBox_recibo_caja")
            Dim DropDownList_ante_pone As DropDownList = pag.FindControl("DropDownList_ante_pone")
            Dim TextBox_codigo_barras As TextBox = pag.FindControl("TextBox_codigo_barras")
            Dim UpdatePanel_text_codigo_barras As UpdatePanel = pag.FindControl("UpdatePanel_text_codigo_barras")
            Dim DropDownList_rutas As DropDownList = pag.FindControl("DropDownList_rutas")
            Dim DropDownList_actividades As DropDownList = pag.FindControl("DropDownList_actividades")
            Dim DropDownList_usurios As DropDownList = pag.FindControl("DropDownList_usurios")
            If TextBox_recibo_caja.Text = "" Then
                Inicia_Registra_flujo_trabajo = "Informe el consecutivo de recibo de sii"
                Exit Function
            End If
            If TextBox_codigo_barras.Text = "" Then
                Inicia_Registra_flujo_trabajo = "Informe el consecutivo de código de barras o radicado del sii "
                Exit Function
            End If
            If DropDownList_rutas.Text = "" Then
                Inicia_Registra_flujo_trabajo = "Seleccione la ruta donde quiere crear la tarea o flujo de te trabajo "
                Exit Function
            End If
            If DropDownList_actividades.Text = "" Then
                Inicia_Registra_flujo_trabajo = "Seleccione la actividad donde se asignara la tarea o flujo de te trabajo "
                Exit Function
            End If
            If DropDownList_tramites.Text = "" Then
                Inicia_Registra_flujo_trabajo = "Seleccione el tipo de tramite a registrar "
                Exit Function
            End If
            If TextBox_razon_social.Text = "" Then
                Inicia_Registra_flujo_trabajo = "Informe el nombre o razón social para el flujo de trabajo "
                Exit Function
            End If
            Dim stru_consulta_recibo As consultarRecibo = Nothing
            Dim recibo As String = TextBox_recibo_caja.Text
            Dim Result As String = Me.Default_Generar_Ceros_Recibo(recibo,
                                                                   DropDownList_ante_pone.Text)
            If Result <> "YES" Then
                Inicia_Registra_flujo_trabajo = Result
                Exit Function
            End If


            Dim fecha_ini As String = Date.Now
            Result = ""
            Dim refclas_gestion_fechas As New ClassGestionFechas
            Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(fecha_ini)
            If Result <> "YES" Then
                Inicia_Registra_flujo_trabajo = "Imposible formatear fecha " & Result
                Exit Function
            End If

            Dim split() As String = DropDownList_tramites.SelectedValue.Split("|")
            Dim descripcion_tramite As String = split(3)
            Dim nombre_servicio As String = split(1)
            Dim codigo_gabinete As Integer = Val(split(2))
            Dim value As String = split(1)
            Dim nombre_gabinete As String = ""
            Dim Class_configuracion_gabinete As New Class_configuracion_gabinete
            Result = Class_configuracion_gabinete.Solicita_nombre_gabinete(codigo_gabinete.ToString,
                                                                           nombre_gabinete)
            If Result <> "YES" Then
                Inicia_Registra_flujo_trabajo = Result
                Exit Function
            End If
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim id_tipo_doc_entrante As Integer = 0
            Result = Class_tipo_doc_entrante.Solicita_identificacion_tipo_documento_entrante_externo_nombre(value,
                                                                                                            id_tipo_doc_entrante)
            If Result <> "YES" Then
                Inicia_Registra_flujo_trabajo = Result
                Exit Function
            End If
            Dim existencia_relacion_flujo As Integer = 0
            Dim Class_ra_relacion_tramite_flujo_wokflow As New Class_ra_relacion_tramite_flujo_wokflow
            Result = Class_ra_relacion_tramite_flujo_wokflow.Solicita_existencia_relacion_flujo_tramite(id_tipo_doc_entrante,
                                                                                                        existencia_relacion_flujo)
            If Result <> "YES" Then
                Inicia_Registra_flujo_trabajo = Result
                Exit Function
            End If
            Dim existencia As String = ""
            Dim nombre_ruta As String = ""
            Dim id_ruta As Integer = 0
            Dim id_flujo_trabajo As Integer = 0
            Dim id_actividad_flujo_trabajo As Integer = 0
            Dim id_usuario_flujo_trabajo As Integer = 0
            Dim id_usuario_workflow As Integer = 0
            Dim codigo_sede_sii As String = ""
            Dim id_actividad_workflow As Integer = 0
            Dim Class_Listado_Actividades_workflow As New Class_Listado_Actividades_workflow
            Dim Class_worflow_rutas As New Class_worflow_rutas
            Dim Class_wf_registro_actividaes_flujos_trabajo As New Class_wf_registro_actividaes_flujos_trabajo
            Dim struregistro_actividaes_flujos_trabajo As struregistro_actividaes_flujos_trabajo = Nothing
            If existencia_relacion_flujo = 1 Then
                id_actividad_flujo_trabajo = Val(DropDownList_rutas.SelectedValue)
                If DropDownList_usurios.SelectedItem.Text = "" Then
                    id_usuario_flujo_trabajo = 0
                    id_usuario_workflow = 0
                Else
                    id_usuario_flujo_trabajo = Val(DropDownList_usurios.SelectedValue)
                    id_usuario_workflow = Val(DropDownList_usurios.SelectedValue)
                End If
                Result = Class_wf_registro_actividaes_flujos_trabajo.Solicita_estructura_actividad_flujo_trabajo(id_actividad_flujo_trabajo,
                                                                                                                 struregistro_actividaes_flujos_trabajo)
                If Result <> "YES" Then
                    Inicia_Registra_flujo_trabajo = Result
                    Exit Function
                End If
                id_actividad_workflow = struregistro_actividaes_flujos_trabajo.listado_actividades_workflow_Id_Actividad
                id_flujo_trabajo = struregistro_actividaes_flujos_trabajo.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO
                Result = Class_Listado_Actividades_workflow.Solicita_id_ruta_id_actividad_workflow(id_actividad_workflow,
                                                                                                   id_ruta)
                If Result <> "YES" Then
                    Inicia_Registra_flujo_trabajo = Result
                    Exit Function
                End If
                Result = Class_worflow_rutas.Solicita_nombre_ruta_por_id_ruta(id_ruta,
                                                                              nombre_ruta)
                If Result <> "YES" Then
                    Inicia_Registra_flujo_trabajo = Result
                    Exit Function
                End If
                Result = Verificar_existencia_recibo(recibo,
                                                     nombre_ruta,
                                                     existencia)
                If Result <> "YES" Then
                    Inicia_Registra_flujo_trabajo = Result
                    Exit Function
                End If
                If existencia = "YES" Then
                    Inicia_Registra_flujo_trabajo = "El sistema encontro el recibo " & recibo & " en la ruta de trabajo " & nombre_ruta &
                    " imposible continuar"
                    Exit Function
                End If
                Result = Me.Solicitar_codigo_sede(id_actividad_workflow,
                                                  codigo_sede_sii)
                If Result <> "YES" Then
                    Inicia_Registra_flujo_trabajo = Result
                    Exit Function
                End If
            Else
                Result = Verificar_existencia_recibo(recibo,
                                                     DropDownList_rutas.SelectedItem.Text,
                                                     existencia)
                If Result <> "YES" Then
                    Inicia_Registra_flujo_trabajo = Result
                    Exit Function
                End If
                If existencia = "YES" Then
                    Inicia_Registra_flujo_trabajo = "El sistema encontro el recibo " & recibo & " en la ruta de trabajo " & DropDownList_rutas.SelectedItem.Text &
                    " imposible continuar"
                    Exit Function
                End If
                id_actividad_workflow = Val(DropDownList_actividades.SelectedValue)
                Result = Me.Solicitar_codigo_sede(id_actividad_workflow,
                                                  codigo_sede_sii)
                If Result <> "YES" Then
                    Inicia_Registra_flujo_trabajo = Result
                    Exit Function
                End If
                nombre_ruta = DropDownList_rutas.SelectedItem.Text
                id_ruta = Val(DropDownList_rutas.SelectedValue)
            End If
            Dim ClassRaRelacionRadicadoExternoExpediente As New ClassRaRelacionRadicadoExternoExpediente
            Result = ClassRaRelacionRadicadoExternoExpediente.RegistraValidaRelacionExpedienteRadicadoExterno(TextBox_matricula.Text,
                                                                                                              nombre_gabinete,
                                                                                                              recibo)
            If Result <> "YES" Then
                Inicia_Registra_flujo_trabajo = Result
                Exit Function
            End If
            Result = Me.Registra_flujo_workflow_externo(id_ruta,
                                                        id_actividad_workflow,
                                                        id_usuario_workflow,
                                                        codigo_gabinete,
                                                        nombre_gabinete,
                                                        nombre_ruta,
                                                        0,
                                                        id_flujo_trabajo,
                                                        fecha_ini,
                                                        recibo,
                                                        TextBox_codigo_barras.Text,
                                                        TextBox_razon_social.Text,
                                                        TextBox_matricula.Text,
                                                        codigo_sede_sii,
                                                        2,
                                                        nombre_servicio,
                                                        descripcion_tramite,
                                                        id_actividad_flujo_trabajo,
                                                        id_usuario_flujo_trabajo, 0, 0, "")
            If Result <> "YES" Then
                Inicia_Registra_flujo_trabajo = Result
                Exit Function
            Else
                Inicia_Registra_flujo_trabajo = "La tarea relacionada al recibo de sii (" & recibo & ") se creo correctamente "
                Exit Function
            End If
            Inicia_Registra_flujo_trabajo = "YES"
        Catch ex As Exception
            Inicia_Registra_flujo_trabajo = "Inconsistencia general función Inicia_Registra_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Solicitar_codigo_sede(ByVal id_actividad_workflow As Integer,
                                   ByRef codigo_sede As String) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("configuracion_gabinete")
            Dim Sql_consulta As String = "Select codigo_sede_sii from ws_sedes_workflow " &
                " where listado_actividades_workflow_id_actividad=" & id_actividad_workflow
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicitar_codigo_sede = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicitar_codigo_sede = "Imposible encontrar el código de la sede relacionado a la actividad de workflow  " & id_actividad_workflow
                Exit Function
            Else
                codigo_sede = Datset.Tables(0).Rows(0).Item(0)
                Solicitar_codigo_sede = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicitar_codigo_sede = "Inconsistencia general función Solicitar_codigo_sede " & ex.Message
        End Try
    End Function

    Function Verificar_existencia_recibo(ByVal recibo As String,
                                         ByVal nombre_ruta As String,
                                         ByRef existencia As String) As String

        Try
            Dim Sql_consulta As String = "Select datos_recibo from f_w_e_" & LCase(nombre_ruta) & " where datos_recibo='" & recibo & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet(nombre_ruta)
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Verificar_existencia_recibo = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia = "NO EXISTE"
                Verificar_existencia_recibo = "YES"
                Exit Function
            Else
                existencia = "YES"
                Verificar_existencia_recibo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verificar_existencia_recibo = "Inconsistencia general función  Verificar_existencia_recibo " & ex.Message
        End Try
    End Function
    Function Registra_flujo_workflow_externo(ByVal id_ruta As Integer,
                                             ByVal id_actividad As Integer,
                                             ByVal id_usuario_workflow As Integer,
                                             ByVal codigo_gabinete As Integer,
                                             ByVal nombre_gabinete As String,
                                             ByVal nombre_ruta As String, ByVal id_imagen As Integer,
                                             ByVal id_flujo_trabajo As Integer,
                                             ByVal fecha_ini As String,
                                             ByVal recibo As String, ByVal codigo_barras As String,
                                             ByVal rason_social As String,
                                             ByVal matricula As String,
                                             ByVal codigo_sede_sii As String,
                                             ByVal flujo_interno As Integer,
                                             ByVal tramite As String,
                                             ByVal descripcion_tramite As String,
                                             ByVal id_actividad_flujo_trabajo As Integer,
                                             ByVal id_usuario_flujo_trabajo As Integer,
                                             ByVal option_registro_log As Integer,
                                             ByVal id_usurio_workflow_registro_log As Integer,
                                             ByVal CODIGO_RUE As String) As String

        Dim myConnection As New MySqlConnection
        Dim myConnection_da As New conect.Dbase_Conction_Mysql
        myConnection_da.Returna_Conexion_Mysql(myConnection)
        Dim myCommand2 As MySqlCommand = myConnection.CreateCommand()
        Dim myTrans As MySqlTransaction
        Dim Switc As Integer = 0
        Dim ref_matricula As Object = "Null"
        If matricula <> "" Then
            ref_matricula = "'" & matricula & "'"
        End If
        Dim p_trans As Integer = 0
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim Campos_Insertcion As String = "DATOS_RECIBO,CODIGO_BARRAS," &
            "SECUENCIA_DOCUMENTO,RAZON_SOCIAL,MATRICULA,COD_SEDE,SECUENCIA_SERVICIO," &
            "TIPO_DOCUMENTO,FECHA_DOCUMENTO,INSCRIPCION_DOCUMENTO,ID_GABINETE,NOMBRE_GABINETE," &
            "ID_RUTA,FLAG"
            Dim Parametro_Insert As String = "INSERT INTO F_W_E_REGISTROPUBLICO ( " &
            Campos_Insertcion & ") Values (" &
            "'" & recibo & "','" & codigo_barras & "','" & 0 & "','" & Left(rason_social, 120) & "'," & ref_matricula & ",'" &
            codigo_sede_sii & "','" & "0" & "','" & Left(descripcion_tramite, 120) & "','" & fecha_ini & "','" & "0" & "','" &
            codigo_gabinete & "','" & nombre_gabinete & "','" & id_ruta & "','" & "1" & "')"
            myCommand.CommandText = Parametro_Insert
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Registra_flujo_workflow_externo = "csfc_r_f_w 30 Función csfcrp_registra_flujo_workflow Error de Conexión"
                myConnection.Close()
                Exit Function
            End If
            p_trans = 1
            Dim Sql_Insercion As String = "insert INICIO_TAREAS_WORKFLOW ( Rutas_Workflow_id_Ruta," &
               "Fecha_Ini_Workflow,Flag_sistema,id_dat_ext)" &
               " VALUES (" & id_ruta & ",'" &
               fecha_ini & "',1," & "0" & " )"
            myCommand.CommandText = Sql_Insercion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Registra_flujo_workflow_externo = "csfc_r_f_w 01 Función csfcrp_registra_flujo_workflow Error de Conexión"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim last_insert = myCommand.LastInsertedId
            '--------------------------------------------------------
            'Constuye sql inserción para tabla workflow
            '--------------------------------------------------------
            Dim sqlinsert As String = ""
            Dim campos_insert As String = " (DATOS_RECIBO,CODIGO_BARRAS,RAZON_SOCIAL,MATRICULA,SEDE,NOMBRE_GABINETE,FLUJO_INTERNO_WF,TRAMITE,DESCRIPCIONTRAMITE"
            Dim valores_insert As String = "('" & recibo & "','" & codigo_barras & "','" & Left(rason_social, 120) & "'," & ref_matricula &
                                           ",'" & codigo_sede_sii & "','" & nombre_gabinete & "','" & flujo_interno & "','" &
                                           Left(tramite, 120) & "','" & Left(descripcion_tramite, 120) & "','"

            '--------------------------------------
            'Inserta los datos del flujo documental
            '--------------------------------------
            Dim Ref_id_imagen As Object = "null"
            If id_imagen <> 0 Then
                Ref_id_imagen = id_imagen
            End If
            campos_insert = campos_insert & ",INICIO_TAREAS_WORKFLOW_ID_TAREA,ID_GABINETE,ID_IMAGEN,FLUJO_TRABAJO_WF)"
            valores_insert = valores_insert & last_insert & "','" & codigo_gabinete & "'," + Ref_id_imagen + "," & id_flujo_trabajo & ")"
            sqlinsert = "insert into DAT_ADIC_TAR" & nombre_ruta & " " & campos_insert & " values " & valores_insert
            myCommand.CommandText = sqlinsert
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Registra_flujo_workflow_externo = "csfc_r_f_w 02 Función csfcrp_registra_flujo_workflow Error de Conexión"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '----------------------------------------------
            'Registra los datos de estados tarea workflow
            '----------------------------------------------
            Dim ref_id_usuario_workflow As Object = "null"
            If id_usuario_workflow <> 0 Then
                ref_id_usuario_workflow = id_usuario_workflow
            End If
            Dim sqlinsert_dat As String = "INSERT INTO ESTADOS_TAREA_WORKFLOW (" &
                "Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta," &
                "Inicio_Tareas_Workflow_id_Tarea,Id_Actividad,FECHA_INICIO," &
                "ESTADO_PRIORIDAD,ESTADO_TAREA,Id_Usuario,ID_FLUJO_TRABAJO,ID_ACTIVIDAD_FLUJO_TRABAJO,ID_USUARIO_WORKFLOW_FLUJO_TRABAJO) VALUES (" &
                id_ruta & "," &
                last_insert & "," &
                id_actividad & ",'" &
                fecha_ini & "'," &
                "0,0," & ref_id_usuario_workflow & "," & id_flujo_trabajo & "," & id_actividad_flujo_trabajo & "," & id_usuario_flujo_trabajo & ")"
            myCommand.CommandText = sqlinsert_dat
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Registra_flujo_workflow_externo = "csfc_r_f_w 03 Función csfcrp_registra_flujo_workflow Error de Conexión"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim ref_codigo_rue As Object = "null"
            If CODIGO_RUE <> "" Then
                ref_codigo_rue = "'" & CODIGO_RUE & "'"
            End If
            Dim sql_reistro_log As String = "INSERT INTO wf_int_sii_registro_tarea_rue_virtual (inicio_tareas_workflow_id_Tarea,fecha_registro," &
                "id_usuario_workflow,CODIGO_RUE,DATOS_RECIBO,CODIGO_BARRAS,NOMBRE_TRAMITE) values (" &
                 last_insert & ",'" & fecha_ini & "'," & id_usurio_workflow_registro_log & "," & ref_codigo_rue & ",'" & recibo & "','" & codigo_barras &
                 "','" & tramite & "')"
            If option_registro_log = 1 Then
                myCommand.CommandText = sql_reistro_log
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Registra_flujo_workflow_externo = "csfc_r_f_w 03 Función csfcrp_registra_flujo_workflow Error de Conexión"
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            myTrans.Commit()
            myConnection.Close()
            Registra_flujo_workflow_externo = "YES"
            Exit Function
        Catch ex As Exception
            If p_trans = 1 Then
                myTrans.Rollback()
            End If
            Registra_flujo_workflow_externo = "csfc_a_d_c_p_r_r_w 020 Inconsistencia general función csfcrp_registra_flujo_workflow " & ex.Message
        Finally
            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
        End Try
    End Function
    Function Consulta_id_tarea_Workflow(ByVal Datos_Recibo As String, ByRef Id_tarea As Object) As String
        Try
            Dim Sql_consulta As String = "select INICIO_TAREAS_WORKFLOW_ID_TAREA " &
                  " from DAT_ADIC_TARREGISTROPUBLICO  WHERE DATOS_RECIBO='" & Datos_Recibo & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("configuracion_gabinete")
            Id_tarea = -1
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Consulta_id_tarea_Workflow = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Consulta_id_tarea_Workflow = "Imposible encontrar la tarea relacionada al recibo " & Datos_Recibo
                Exit Function
            Else
                Id_tarea = Datset.Tables(0).Rows(0).Item(0)
                Consulta_id_tarea_Workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Consulta_id_tarea_Workflow = "Error General Funcion Consulta_Id_Tarea_Workflow " & ex.Message
        End Try
    End Function
    Function Consulta_Estado_Tarea_Workflow(ByVal Id_tarea As Integer, ByRef Data_Tarea As String) As String
        Try
            Dim Sql_consulta As String = "SELECT  uw.login_Usuario, uw.nombre_usuario, uw.Cargo_Usuario " &
                                               "FROM estados_tarea_workflow etw " &
                                                "inner join usuario_workflow uw on " &
                                                "(uw.idU_suario=etw.id_usuario) " &
                                               " where etw.inicio_tareas_workflow_id_tarea=" & Id_tarea &
                                               " and etw.fecha_seleccion is not null and  etw.fecha_fin is null"

            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Consulta_Estado_Tarea_Workflow = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Data_Tarea = ""
                Consulta_Estado_Tarea_Workflow = "YES"
                Exit Function
            Else
                Data_Tarea = "La tarea esta seleccionada Por login " & Datset.Tables(0).Rows(0).Item(0) &
                " De nombre " & Datset.Tables(0).Rows(0).Item(1) & " con cargo " & Datset.Tables(0).Rows(0).Item(2)
                Consulta_Estado_Tarea_Workflow = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Consulta_Estado_Tarea_Workflow = "Error General Funcion Consulta_Estado_Tarea_Workflow " & ex.ToString
        End Try
    End Function
    Function Eliminar_flujo_workflow_SII(ByVal ReciboSII As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Elimina flujo de trabajo workflow para el sistema SII con el numero de recibo SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'ReciboSII           : Consecutovo recibo de caja SII
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-07
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Id_tarea As Integer = -1
            Result = Consulta_id_tarea_Workflow(ReciboSII,
                                                Id_tarea)
            If Result <> "YES" Then
                Eliminar_flujo_workflow_SII = Result
                Exit Function
            End If
            Dim Datos_Tarea As String = ""
            If Id_tarea <> -1 Then
                Result = ""
                Result = Consulta_Estado_Tarea_Workflow(Id_tarea,
                                                        Datos_Tarea)
                If Result <> "YES" Then
                    Eliminar_flujo_workflow_SII = "Imposible consultar estado tarea " & Result
                    Exit Function
                End If
                If Datos_Tarea <> "" Then
                    Eliminar_flujo_workflow_SII = Datos_Tarea
                    Exit Function
                End If
            End If
            '///-------------Valida  existencia autorización --------////  
            Dim Class_autoriza_tarea_worklfow As New Class_autoriza_tarea_worklfow
            Dim Estado_existencia_autorizacion As String = ""
            Dim Estado_elimina As Integer = 0
            Dim leyend_autorizacion As String = ""
            Result = Class_autoriza_tarea_worklfow.Solicita_existencia_autorizacion(Id_tarea,
                                                                                    Estado_existencia_autorizacion)
            If Result <> "YES" Then
                Eliminar_flujo_workflow_SII = Result
                Exit Function
            End If
            If Estado_existencia_autorizacion = "YES" Then
                Estado_elimina = 1
                leyend_autorizacion = " y  se eliminarón autorizaciones de flujo"
            End If
            '///-------------Elimina registro de flujo--------///
            Result = Eliminar_Registros_flujo(ReciboSII,
                                              Id_tarea,
                                              Estado_elimina)
            If Result <> "YES" Then
                Eliminar_flujo_workflow_SII = "Error Eliminando registros " & Result
                Exit Function
            End If
            Result = EliminaRegistrosRelacionTareaWorkflowSII(ReciboSII,
                                                              Id_tarea)
            If Result <> "YES" Then
                Eliminar_flujo_workflow_SII = "La tarea relacionada al recibo de sii (" & ReciboSII & ") se eliminó correctamente, pero no se pudo elimnar la relación de vinculaciones de la tarea  (" & Result & ")"
                Exit Function
            Else
                Eliminar_flujo_workflow_SII = "La tarea relacionada al recibo de sii (" & ReciboSII & ") se eliminó correctamente  " & leyend_autorizacion
                Exit Function
            End If
        Catch ex As Exception
            Eliminar_flujo_workflow_SII = "Inconsistencia general función Eliminar_flujo_workflow_SII " & ex.Message
        End Try
    End Function
    Function Eliminar_Flujo_Workflow(ByRef pag As Page) As String
        Try
            Dim TextBox_recibo_caja As TextBox = pag.FindControl("TextBox_recibo_elimina")
            Dim DropDownList_ante_pone As DropDownList = pag.FindControl("DropDownList_antepone_elimina")
            '***********************************
            'Validacion Campos
            '***********************************
            If TextBox_recibo_caja.Text = "" Then
                Eliminar_Flujo_Workflow = "Debe digitar le numero de recibo "
                Exit Function
            End If
            '***********************************
            'Formatea formato recibo
            '**********************************
            Dim Result As String = ""
            Dim Datos_Recibo As String = TextBox_recibo_caja.Text
            Result = Default_Generar_Ceros_Recibo(Datos_Recibo,
                                                  DropDownList_ante_pone.Text)
            If Result <> "YES" Then
                Eliminar_Flujo_Workflow = "Imposible formatear el recibo " & Result
                Exit Function
            End If
            '*********************************
            'Consulta id tarea workflow
            '*********************************
            Result = ""
            Dim Id_tarea As Integer = -1
            Result = Consulta_id_tarea_Workflow(Datos_Recibo,
                                                Id_tarea)
            If Result <> "YES" Then
                Eliminar_Flujo_Workflow = Result
                Exit Function
            End If
            '**********************************
            'Consulta existencia de estudio
            'de flujo
            '**********************************
            Dim Datos_Tarea As String = ""
            If Id_tarea <> -1 Then
                Result = ""
                Result = Consulta_Estado_Tarea_Workflow(Id_tarea,
                                                        Datos_Tarea)
                If Result <> "YES" Then
                    Eliminar_Flujo_Workflow = "Imposible consultar estado tarea " & Result
                    Exit Function
                End If
                If Datos_Tarea <> "" Then
                    Eliminar_Flujo_Workflow = Datos_Tarea
                    Exit Function
                End If
            End If
            '------------------------------------------
            'Solicita existencia autorización   
            '------------------------------------------
            Dim Class_autoriza_tarea_worklfow As New Class_autoriza_tarea_worklfow
            Dim Estado_existencia_autorizacion As String = ""
            Dim Estado_elimina As Integer = 0
            Dim leyend_autorizacion As String = ""
            Result = Class_autoriza_tarea_worklfow.Solicita_existencia_autorizacion(Id_tarea,
                                                                                    Estado_existencia_autorizacion)
            If Result <> "YES" Then
                Eliminar_Flujo_Workflow = Result
                Exit Function
            End If
            If Estado_existencia_autorizacion = "YES" Then
                Estado_elimina = 1
                leyend_autorizacion = " y  se eliminarón autorizaciones de flujo"
            End If
            '******************************************
            'eLIMINA RASTRO DEL FLUJO
            '******************************************
            Result = ""
            Result = Eliminar_Registros_flujo(Datos_Recibo,
                                              Id_tarea,
                                              Estado_elimina)
            If Result <> "YES" Then
                Eliminar_Flujo_Workflow = "Error Eliminado registros " & Result
                Exit Function
            Else
                Eliminar_Flujo_Workflow = "La tarea relacionada al recibo de sii (" & Datos_Recibo & ") se elimino correctamente en la ruta " & leyend_autorizacion
                Exit Function
            End If
            Eliminar_Flujo_Workflow = "YES"
        Catch ex As Exception
            Eliminar_Flujo_Workflow = "Inconsistencia general función Eliminar_Flujo_Workflow " & ex.Message
        End Try

    End Function
    Function EliminaRegistrosRelacionTareaWorkflowSII(ByVal ReciboSII As String,
                                                      ByVal IdTareaWorkflow As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Elimina los registros de relación de flujos de trabajo 
        'Elimina relacion de un radicado externo con un expediente
        'Elimina relacion de vinculación de documentos a expediente en una tarea workflow
        'Elimina cache vinculación de documentos a expediente
        'Elimina cache de inscrpción para inscripciones integración  SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'ReciboSII           : Consecutovo recibo de caja SII
        'IdTareaWorkflow     : Indentificación de la tarea workflow
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-02
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim myConnection As New MySqlConnection
        Dim myConnection_da As New conect.Dbase_Conction_Mysql_DA
        myConnection_da.Returna_Conexion_Mysql(myConnection)
        Dim myCommand2 As MySqlCommand = myConnection.CreateCommand()
        Dim myTrans As MySqlTransaction
        Dim Switc2 As Integer = 0
        Dim EstadoRolback As Integer = 0
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand2.Connection = myConnection
            myCommand2.Transaction = myTrans
            '///----Elimina relacion de un radicado externo con un expediente
            Dim SqlEliminaRelaconExpedienteExterno = "Delete from ra_relacion_radicado_externo_expediente  where RadicadoExterno='" &
              ReciboSII & "'"
            myCommand2.CommandText = SqlEliminaRelaconExpedienteExterno
            Switc2 = myCommand2.ExecuteNonQuery()
            EstadoRolback = 1
            '///----Elimina relacion de vinculación de documentos a expediente en una tarea workflow
            Dim SqlEliminaRelaconDocumentoExpediente = "Delete from ra_rel_copia_wf_produccion where id_tarea_wf=" & IdTareaWorkflow
            myCommand2.CommandText = SqlEliminaRelaconDocumentoExpediente
            Switc2 = myCommand2.ExecuteNonQuery()
            '///----Elimina cache vinculacion documentos a expiediente
            Dim SqlEliminaCacheVinculacion = "Delete from ra_sii_cache_vinculacion where RadicadoSII='" & ReciboSII & "'"
            myCommand2.CommandText = SqlEliminaCacheVinculacion
            Switc2 = myCommand2.ExecuteNonQuery()
            '///----Elimina cache de inscrpción para Inscripcion SII
            Dim SqlEliminaCacheIncripcion = "Delete from ra_sii_cahche_inscripcion where RadicadoSII='" & ReciboSII & "'"
            myCommand2.CommandText = SqlEliminaCacheIncripcion
            Switc2 = myCommand2.ExecuteNonQuery()
            '///----Elimina cache creación expediente Inscripcion SII
            Dim SqlEliminaCacheExpediente = "Delete from ra_sii_cache_exepediente where RadicadoSII='" & ReciboSII & "'"
            myCommand2.CommandText = SqlEliminaCacheExpediente
            Switc2 = myCommand2.ExecuteNonQuery()
            myTrans.Commit()
            myConnection.Close()
            EliminaRegistrosRelacionTareaWorkflowSII = "YES"
            Exit Function
        Catch ex As Exception
            EliminaRegistrosRelacionTareaWorkflowSII = "Se presentó una excepción de tipo  (" & ex.GetType().ToString() & ") con el siguiente mensaje de la transacción " & ex.Message
            If EstadoRolback = 1 Then
                myTrans.Rollback()
            End If
        Finally
            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
        End Try
    End Function
    Function Eliminar_Registros_flujo(ByVal Datos_Recibo As String,
                                      ByVal Id_tarea As Integer,
                                      ByVal elimina_registro As Integer) As String
        Dim myConnection As New MySqlConnection
        Dim myConnection_da As New conect.Dbase_Conction_Mysql
        myConnection_da.Returna_Conexion_Mysql(myConnection)
        Dim myCommand2 As MySqlCommand = myConnection.CreateCommand()
        Dim myTrans As MySqlTransaction
        Dim Switc2 As Integer = 0
        Try
            Dim Parametro_Delete_Registro As String = ""
            myTrans = myConnection.BeginTransaction()
            myCommand2.Connection = myConnection
            myCommand2.Transaction = myTrans
            '************************************
            'elimina rastro aprobación
            '*************************************
            If elimina_registro = 1 Then
                Parametro_Delete_Registro = "Delete from wf_autoriza_tarea where inicio_tareas_workflow_id_tarea=" & Id_tarea
                myCommand2.CommandText = Parametro_Delete_Registro
                Switc2 = myCommand2.ExecuteNonQuery()
                If Switc2 = 0 Then
                    Eliminar_Registros_flujo = "Imposible eliminar el registro de autorización"
                    'myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            '************************************
            'elimina rastro estados tarea
            '*************************************
            Parametro_Delete_Registro = "Delete from estados_tarea_workflow where inicio_tareas_workflow_id_tarea=" & Id_tarea
            myCommand2.CommandText = Parametro_Delete_Registro
            Switc2 = myCommand2.ExecuteNonQuery()
            If Switc2 = 0 Then
                Eliminar_Registros_flujo = "Imposible eliminar los estados de la tarea"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Parametro_Delete_Registro = "Delete from DAT_ADIC_TARREGISTROPUBLICO where datos_recibo='" & Datos_Recibo & "'"
            myCommand2.CommandText = Parametro_Delete_Registro
            Switc2 = myCommand2.ExecuteNonQuery()
            If Switc2 = 0 Then
                Eliminar_Registros_flujo = "Imposible eliminar los datos de la tarea"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If

            '*******************************************
            'Elimina rastro tabla f_w_e_registropublico
            '*******************************************
            Parametro_Delete_Registro = "Delete from f_w_e_registropublico where datos_recibo='" & Datos_Recibo & "'"
            myCommand2.CommandText = Parametro_Delete_Registro
            Switc2 = myCommand2.ExecuteNonQuery()
            If Switc2 = 0 Then
                Eliminar_Registros_flujo = "Imposible eliminar los datos de la tarea en la tabla f_w_e"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '********************************************************
            'Elimina registro de la tabla inicio_tareas_workflow
            '********************************************************
            Parametro_Delete_Registro = "Delete from inicio_tareas_workflow where id_tarea=" & Id_tarea
            myCommand2.CommandText = Parametro_Delete_Registro
            Switc2 = myCommand2.ExecuteNonQuery()
            If Switc2 = 0 Then
                Eliminar_Registros_flujo = "Imposible eliminar el registro del inicio de la tarea"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Eliminar_Registros_flujo = "YES"
        Catch ex As Exception
            Eliminar_Registros_flujo = " Inconsistencia general función Eliminar_Registros_flujo " & ex.Message
        Finally
            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
        End Try
    End Function
    Function Actualiza_datos_imagen_workflow_SII(ByVal recibo As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Actualiza los datos de la imagen docuarchi en los datos de la ruta WF
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'recibo           : Representa el recibo del sistema SII
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-09
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Result As String = ""
            Dim Class_worflow_rutas As New Class_worflow_rutas
            Dim nombre_ruta As String = ""
            Dim id_ruta As Integer = 0
            Result = Class_worflow_rutas.Solicita_id_ruta_nonbre_ruta_workflow(nombre_ruta,
                                                                               id_ruta)
            If Result <> "YES" Then
                Actualiza_datos_imagen_workflow_SII = Result
                Exit Function
            End If
            Dim Cod_barra As String = ""
            Dim Nom_Gabinete As String = ""
            Dim Id_gabinete As Integer = 0
            Dim secuencia As Integer = 0
            Dim id_tarea As Long = 0
            Dim Class_configuracion_gabinete As New Class_configuracion_gabinete
            Result = Class_configuracion_gabinete.Solicita_datos_ruta_workflow_gabinete_SII(recibo,
                                                                                            nombre_ruta,
                                                                                            Cod_barra,
                                                                                            Nom_Gabinete,
                                                                                            id_tarea,
                                                                                            Id_gabinete,
                                                                                            secuencia)
            If Result <> "YES" Then
                Actualiza_datos_imagen_workflow_SII = Result
                Exit Function
            End If
            Dim structure_gabinete_workflow As structure_gabinete_workflow = Nothing
            Result = Class_configuracion_gabinete.SolicitaDatosEstructuraGabineteWorkflow(Id_gabinete,
                                                                                              structure_gabinete_workflow)
            If Result <> "YES" Then
                Actualiza_datos_imagen_workflow_SII = Result
                Exit Function
            End If
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim Id_imagen As Integer = 0
            Dim Auxiliar As String = ""
            Dim Acto As String = ""
            Result = ClassDaGabinete.Solicita_structura_imagen_gabinete_SII(Nom_Gabinete,
                                                                            recibo,
                                                                            Cod_barra,
                                                                            Id_imagen,
                                                                            Auxiliar,
                                                                            Acto)
            If Result <> "YES" Then
                Actualiza_datos_imagen_workflow_SII = Result
                Exit Function
            End If
            If Id_imagen = 0 Then
                Actualiza_datos_imagen_workflow_SII = "Imposible encontrar documentos  en el  gabinete  (" & Nom_Gabinete & ") del recibo (" & recibo & ")"
                Exit Function
            End If
            If Auxiliar = "" Then
                Actualiza_datos_imagen_workflow_SII = "Documento sin campo auxiliar en el gabinete  (" & Nom_Gabinete & ") del recibo (" & recibo & ")"
                Exit Function
            End If
            If Acto = "" Then
                Actualiza_datos_imagen_workflow_SII = "Documento sin campo acto en el gabinete  (" & Nom_Gabinete & ") del recibo (" & recibo & ")"
                Exit Function
            End If
            '--------------------------------------------------------------------
            'Actauliza la id imagen en la tabla workflow
            '--------------------------------------------------------------------- 
            Dim SqlActualiza As String = "UPDATE DAT_ADIC_TAR" & nombre_ruta
            SqlActualiza = SqlActualiza & " SET ID_IMAGEN = " & Id_imagen
            SqlActualiza = SqlActualiza & ", AUXILIAR  ='" & Trim(Auxiliar) & "'"
            SqlActualiza = SqlActualiza & ", ACTO  =" & Trim(Acto)
            SqlActualiza = SqlActualiza & "  WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA ="
            SqlActualiza = SqlActualiza & id_tarea
            Dim ref As New conect.Dbase_Conction_Mysql
            Result = ref.SELECTION_INSERT_COMMAND(SqlActualiza)
            If Result <> "YES" Then
                Actualiza_datos_imagen_workflow_SII = " Error actualizando la imagen en loas campos de la ruta (" & Result & ")"
                Exit Function
            Else
                Actualiza_datos_imagen_workflow_SII = "Recibo (" & recibo & ") actualizado con la imagen (" & Id_imagen & ") del gabinete (" & Nom_Gabinete & ")"
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_datos_imagen_workflow_SII = "Inconsistencia general funcion Actualiza_datos_imagen_workflow_SII " & ex.Message
        End Try
    End Function
    Function Main_default_actualiza_datos_imagen(ByVal recibo As String, ByVal nombre_ruta As String,
                                                 ByVal parametro_letra As String) As String
        Dim Result As String = ""
        Dim SqlConsulta As String = ""
        Dim Id_tarea As String = ""
        Dim Numero_Actividades As String = "0"
        Dim Datos_Tarea As String = ""
        Dim Matri_Datos_Tarea() As String
        Dim Datos_Gabientes_Matri() As String
        Dim Datos_Gabientes As String = ""
        Dim Datos_Imagen As String = ""
        Dim Conta_Doc As Integer = 0
        Dim Iconta As Integer = 0
        Dim Cod_barra As String = ""
        Dim Nom_Gabinete As String = ""
        Dim Id_gabinete As String = ""
        Dim secuencia As String = ""
        Dim Copirecib As String = ""
        Try
            If nombre_ruta = "" Then
                Main_default_actualiza_datos_imagen = "Seleccione la ruta para actualizar el flujo de trabajo"
                Exit Function
            End If
            If recibo = "" Then
                Main_default_actualiza_datos_imagen = "Informe el dato del flujo a actualizar"
                Exit Function
            End If
            If parametro_letra = "" Then
                Main_default_actualiza_datos_imagen = "Seleccione el valor que ante pone el dato del flujo "
                Exit Function
            End If
            Result = ""
            Result = Default_Generar_Ceros_Recibo(recibo, parametro_letra)
            If Result <> "YES" Then
                Main_default_actualiza_datos_imagen = "Imposible Encontrar dato para el recibo  cerros workflow" + Result
                Exit Function
            End If
            Result = ""
            Result = Obtener_Id_Actividad_Recibo_Script(recibo, Cod_barra, Nom_Gabinete, nombre_ruta, Id_tarea, Id_gabinete, secuencia)
            If Result <> "YES" Then
                Main_default_actualiza_datos_imagen = "Imposible Encontrar datos para el recibo " + Result
                Exit Function
            End If
            '----------------------------------------
            'Obtener datos del gabiente
            '----------------------------------------
            'Datos_Gabientes_Matri
            'Datos_Gabientes_Matri(0)=Nombre_Gabienete
            'Datos_Gabientes_Matri(1)=RUTA BUSQUEDA IMAGEN
            'Datos_Gabientes_Matri(2)=BASE DE DATOS
            'Datos_Gabientes_Matri(3)=MOTOR BASE DE DATOS
            'Datos_Gabientes_Matri(4)=ODBC SERVIDOR
            'Datos_Gabientes_Matri(5)=USUARIO
            'Datos_Gabientes_Matri(6)=PASWORD 
            Result = ""
            Result = Default_Obtener_Datos_Gabinete_Script(Id_gabinete, Datos_Gabientes)
            If Result <> "YES" Then
                Main_default_actualiza_datos_imagen = "Eror Imposible Encontra Datos del gabiente " + Matri_Datos_Tarea(1) + Id_tarea + " " + Result
                Exit Function
            End If
            If Datos_Gabientes = "" Then
                Main_default_actualiza_datos_imagen = "Imposible Encontra Datos del gabiente " + Matri_Datos_Tarea(1) + Id_tarea + " " + Result
                Exit Function
            End If
            Erase Datos_Gabientes_Matri
            Datos_Gabientes_Matri = Datos_Gabientes.Split("|")
            If Datos_Gabientes_Matri Is Nothing Then
                Main_default_actualiza_datos_imagen = "Imposible Encontra Datos del gabiente " + Matri_Datos_Tarea(1) + Id_tarea + " " + Result
                Exit Function
            End If
            '--------------------------------------------------------------------------------------------------------------------------------
            'Consulta QUE LA IMAGEN EXUSTA EN DOCUARCHI
            '--------------------------------------------------------------------------------------------------------------------------------
            Cod_barra = Val(Cod_barra)
            'recibo = Val(recibo)
            Dim IdImag As String = ""
            Dim Auxiliar As String = ""
            Dim Acto As String = ""
            Dim SqlConsultaId As String = "Select ID, AUXILIAR,ACTOWF from " & Datos_Gabientes_Matri(0) & " Where RECIBOCAJA='" &
            recibo & "'" & " And  CODBARRAS='" & Cod_barra & "'"
            Result = ""
            Result = Default_Obtener_ID_Imagen_Script(SqlConsultaId,
                                                      IdImag,
                                                      Auxiliar,
                                                      Acto)
            If Result <> "YES" Then
                Main_default_actualiza_datos_imagen = Result
                Exit Function
            End If
            If IdImag = "" Then
                Main_default_actualiza_datos_imagen = "Documento no digitalizado en docuarchi .net  Gabinete  " & Nom_Gabinete
                Exit Function
            End If
            If Auxiliar = "" Then
                Main_default_actualiza_datos_imagen = "Documento sin campo auxiliar docuarchi .net  Gabinete  " & Nom_Gabinete
                Exit Function
            End If
            If Acto = "" Then
                Main_default_actualiza_datos_imagen = "Documento sin campo acto docuarchi .net  Gabinete  " & Nom_Gabinete
                Exit Function
            End If
            '--------------------------------------------------------------------
            'Actauliza la id imagen en la tabla workflow
            '--------------------------------------------------------------------- 
            Dim SqlActualiza As String = "UPDATE DAT_ADIC_TAR" & nombre_ruta
            SqlActualiza = SqlActualiza & " SET ID_IMAGEN = " & IdImag
            SqlActualiza = SqlActualiza & ", AUXILIAR  ='" & Trim(Auxiliar) & "'"
            SqlActualiza = SqlActualiza & ", ACTO  =" & Trim(Acto)
            SqlActualiza = SqlActualiza & "  WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA ="
            SqlActualiza = SqlActualiza & Id_tarea
            Result = ""
            Result = Default_Actualizando_Id_Tarea_Workflow_Script(SqlActualiza)
            If Result <> "YES" Then
                Main_default_actualiza_datos_imagen = Result
                Exit Function
            End If
            Main_default_actualiza_datos_imagen = "YES"
        Catch ex As Exception
            Main_default_actualiza_datos_imagen = "Inconsistencia general función  Main_default_actualiza_datos_imagen " & ex.Message
        End Try
    End Function
    Function Default_Actualizando_Id_Tarea_Workflow_Script(ByVal SqLupdate As String) As String

        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Result As String = ""
            Result = ref.SELECTION_INSERT_COMMAND(SqLupdate)
            If Result <> "YES" Then
                Default_Actualizando_Id_Tarea_Workflow_Script = " Error Actualizando Id imagen tabla workflow " & Result
                Exit Function
            Else
                Default_Actualizando_Id_Tarea_Workflow_Script = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Default_Actualizando_Id_Tarea_Workflow_Script = "Inconsistencia general función Default_Actualizando_Id_Tarea_Workflow_Script" & ex.Message
        End Try
    End Function
    Function Default_Obtener_ID_Imagen_Script(ByVal Sql_consulta As String, ByRef Id_Imagen As String,
                                              ByRef Auxiliar As String, ByRef Acto As String) As String
        Try

            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Default_Obtener_ID_Imagen_Script = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Default_Obtener_ID_Imagen_Script = "Imposible Encontrar Datos para enlazar en el gabiente "
                Exit Function
            Else

                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    Id_Imagen = ""
                Else
                    Id_Imagen = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) Then
                    Auxiliar = ""
                Else
                    Auxiliar = Datset.Tables(0).Rows(0).Item(1)
                End If

                If Datset.Tables(0).Rows(0).IsNull(2) Then
                    Acto = ""
                Else
                    Acto = Datset.Tables(0).Rows(0).Item(2)
                End If
                Default_Obtener_ID_Imagen_Script = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Default_Obtener_ID_Imagen_Script = "Inconsistencia general función Default_Obtener_ID_Imagen_Script " & ex.Message
        End Try
    End Function
    Function Default_Obtener_Datos_Gabinete_Script(ByVal Id_Gabinete As String,
                                                   ByRef Datos_Gabinete As String) As String

        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT NOMBRE_GABINETE,RUTA_BUSQUEDA_IMAGEN " &
            ",BASE_DATOS,MOTOR_BASE,ODBC_BASE,USUARIO_BASE,PASWORD_BASE" &
            " FROM CONFIGURACION_GABINETE" &
            " WHERE ID_GABINETE=" & Id_Gabinete
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Default_Obtener_Datos_Gabinete_Script = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Default_Obtener_Datos_Gabinete_Script = "YES"
                Exit Function
            Else
                Datos_Gabinete = Datset.Tables(0).Rows(0).Item(0).ToString & "|"
                Datos_Gabinete = Datos_Gabinete & Datset.Tables(0).Rows(0).Item(1).ToString & "|"
                Datos_Gabinete = Datos_Gabinete & Datset.Tables(0).Rows(0).Item(2).ToString & "|"
                Datos_Gabinete = Replace(Datos_Gabinete, "/", "")
                Datos_Gabinete = Datos_Gabinete & Datset.Tables(0).Rows(0).Item(3).ToString & "|"
                Datos_Gabinete = Datos_Gabinete & Datset.Tables(0).Rows(0).Item(4).ToString & "|"
                Datos_Gabinete = Datos_Gabinete & Datset.Tables(0).Rows(0).Item(5).ToString & "|"
                Datos_Gabinete = Datos_Gabinete & Datset.Tables(0).Rows(0).Item(6).ToString
                Default_Obtener_Datos_Gabinete_Script = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Default_Obtener_Datos_Gabinete_Script = "Error inconsistencia general función Default_Obtener_ID_Imagen_Script " & ex.Message
        End Try
    End Function
    Function Obtener_Id_Actividad_Recibo_Script(ByVal Datos_Recibo As String, ByRef Codigo_Barras As String,
                                                 ByRef Nombre_Gabinete As String, ByVal Nombre_Ruta As String,
                                                ByRef Id_Tarea As String, ByRef Id_gabinete As String,
                                                ByRef secuencia As String) As String
        Try
            Dim Sql_Consulta As String = ""
            Sql_Consulta = "SELECT CG.NOMBRE_GABINETE,dat.CODIGO_BARRAS, dat.INICIO_TAREAS_WORKFLOW_ID_TAREA,dat.ID_GABINETE,dat.SECUENCIAC FROM dat_adic_tar" & Nombre_Ruta &
                 " dat INNER JOIN configuracion_gabinete cg on (cg.id_Gabinete=dat.ID_GABINETE) " &
                 " WHERE DATOS_RECIBO ='" & Datos_Recibo & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Sql_Consulta, Datset)
            If Result <> "YES" Then
                Obtener_Id_Actividad_Recibo_Script = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Obtener_Id_Actividad_Recibo_Script = "Error Consultando en tabla   dat_adic_tar " & Nombre_Ruta
                Exit Function
            Else
                Codigo_Barras = Datset.Tables(0).Rows(0).Item(1)
                Id_Tarea = Datset.Tables(0).Rows(0).Item(2)
                Id_gabinete = Datset.Tables(0).Rows(0).Item(3)
                secuencia = Datset.Tables(0).Rows(0).Item(4)
                Obtener_Id_Actividad_Recibo_Script = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Obtener_Id_Actividad_Recibo_Script = "Inconistencia general función Obtener_Id_Actividad_Recibo_Script " & ex.Message
        End Try
    End Function

End Class


