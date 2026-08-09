Imports System.IO

Public Class ClassGagestorInicio
    Function Cambiar_Contraseña_Ra(ByVal Pawsuno As String, _
                                   ByVal paswdos As String) As String
        Try
            '*****************************************************
            'Verifica que los campos contraseña no esten vacios
            '*****************************************************
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Dim Result As String = ""
            If Pawsuno = "" Then
                Cambiar_Contraseña_Ra = "La primera contraseña debe informarse"
                Exit Function
            End If
            If paswdos = "" Then
                Cambiar_Contraseña_Ra = "La segunda contraseña debe informarse"
                Exit Function
            End If
            '******************************************************
            'La contraseña debe tener mas de ocho caracteres 
            '*****************************************************
            If Pawsuno.Length < 8 Then
                Cambiar_Contraseña_Ra = "La contraseña debe tener mínimo 8 caracteres"
                Exit Function
            End If
            '******************************************************
            'Compara las contraseña entre minusculas y mayusculas
            '******************************************************
            Dim compara As Integer = -2
            Dim Srcomuno As String = Pawsuno
            Dim Srcomdos As String = paswdos
            compara = StrComp(Srcomuno, Srcomdos, _
             CompareMethod.Binary)
            If compara = 0 Then
            Else
                Cambiar_Contraseña_Ra = "Las contraseñas no coinciden, tenga en cuenta que el sistema diferencia entre minúsculas y mayusculas"
                Exit Function
            End If
            '******************************************************
            'Encriptacion de contraseñas
            '******************************************************
            Dim Contraseña_Encript As String = Pawsuno
            Result = Encrip_Value(Contraseña_Encript)
            If Result <> "YES" Then
                Cambiar_Contraseña_Ra = "Imposible Encriptar la contraseña " & Result
                Exit Function
            End If
            '*******************************************************
            'Actualizacion de la base de datos
            '*******************************************************
            Dim Sqlupdate As String = "Update remit_dest_interno   " & _
                "set Pasw_Usuario='" & Pawsuno & "'" & _
                ", PASW_ENCRIPT='" & Contraseña_Encript & "'" & _
                " where id_Remit_Dest_Int=" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            Dim Resultado_Insertar As String = ref.SELECTION_INSERT_COMMAND(Sqlupdate)
            If Resultado_Insertar <> "YES" Then
                Cambiar_Contraseña_Ra = "Funcion update Error : " & Resultado_Insertar
                Exit Function
            End If
            Cambiar_Contraseña_Ra = "YES"
        Catch ex As Exception
            Cambiar_Contraseña_Ra = "Inconsistencia General Función Cambiar_Contraseña_Ra " & ex.Message
        End Try
    End Function 
    Function Tri_View_gestion(ByRef Tre_v As TreeView) As String
        '************************************
        'Funcion : Tri_View_gestion
        'Ing : Fidel Becerra 
        'Descripcion : Dibuja en tiempo de
        'ejecucion el listado de opciones del
        'usuario de gestion
        'fecha : 2016-8/29
        'Ing Miguel Angel Urueta Miranda
        '*************************************
        Try
            Dim atrrnodeprincipal As New TreeNode
            atrrnodeprincipal.Text = "Gestión documental"
            atrrnodeprincipal.Expand()
            Dim attrNode As New TreeNode
            attrNode.Text = "Unidades documentales"
            attrNode.ToolTip = "Gestión de expedientes y unidades de conservación"
            Dim AtrrNoderegexp As New TreeNode
            AtrrNoderegexp.Text = "Registrar expedientes"
            AtrrNoderegexp.Value = "GESTION|" & "REGISTRAEXP"
            AtrrNoderegexp.ToolTip = "Registrar unidad documental (Expedientes, carpetas, libros...)"
            AtrrNoderegexp.ImageUrl = "../workflow/imageneswf/layer-plus-light.png"
            Dim AtrrNodeConsultaexp As New TreeNode
            AtrrNodeConsultaexp.Text = "Consultar expedientes"
            AtrrNodeConsultaexp.Value = "GESTION|" & "GESTIONEXP"
            AtrrNodeConsultaexp.ToolTip = "Consultar unidades documentales (Expedientes, carpetas, libros...)"
            AtrrNodeConsultaexp.ImageUrl = "../workflow/imageneswf/search-regular.png"
            attrNode.ChildNodes.Add(AtrrNoderegexp)
            attrNode.ChildNodes.Add(AtrrNodeConsultaexp)
            Dim attrNode_gestion_conservacion As New TreeNode
            attrNode_gestion_conservacion.Text = "Consulta unidades de conservación"
            attrNode_gestion_conservacion.Value = "GESTION|" & "CONSERVACION"
            attrNode_gestion_conservacion.ToolTip = "Consulta unidades de conservación (cajas)"
            attrNode_gestion_conservacion.ImageUrl = "../workflow/imageneswf/search-regular.png"
            attrNode.ChildNodes.Add(attrNode_gestion_conservacion)
            Dim attrNode_consulta_toponimica As New TreeNode
            attrNode_consulta_toponimica.Text = "Gestión toponímica "
            attrNode_consulta_toponimica.Value = "GESTION|" & "TOPONIMICA"
            attrNode_consulta_toponimica.ToolTip = "Gestiona la Estructura física de archivo"
            attrNode_consulta_toponimica.ImageUrl = "../workflow/imageneswf/tools-light.png"
            attrNode.ChildNodes.Add(attrNode_consulta_toponimica)
            atrrnodeprincipal.ChildNodes.Add(attrNode)
            Dim attrNode_toponimica As New TreeNode
            'attrNode_toponimica.Text = "Gestión toponímica"
            'Dim attrNode_consulta_toponimica As New TreeNode
            'attrNode_consulta_toponimica.Text = "Consulta toponímica"
            'attrNode_consulta_toponimica.Value = "GESTION|" & "TOPONIMICA"
            'attrNode_consulta_toponimica.ImageUrl = "../workflow/imageneswf/iten_list_select.png"
            'attrNode_toponimica.ChildNodes.Add(attrNode_consulta_toponimica)
            'atrrnodeprincipal.ChildNodes.Add(attrNode_toponimica)
            Dim attrNode_organizacion_documental As New TreeNode
            attrNode_organizacion_documental.Text = "Instrumentos Archivísticos"
            Dim attrNode_admon_clasificacion As New TreeNode
            attrNode_admon_clasificacion.Text = "Administración cuadros de clasificación(CCD)"
            attrNode_admon_clasificacion.Value = "GESTION|" & "ORGANIZACIONDOCUMENTAL_ADMINISTRACION"
            attrNode_admon_clasificacion.ImageUrl = "../workflow/imageneswf/tools-light.png"
            If HttpContext.Current.Session("GA_ADMINISTRACION_CCD") = 1 Then
                attrNode_organizacion_documental.ChildNodes.Add(attrNode_admon_clasificacion)
            End If
            Dim attrNode_consulta_clasificacion As New TreeNode
            attrNode_consulta_clasificacion.Text = "Consulta cuadros de clasificación(CCD)"
            attrNode_consulta_clasificacion.Value = "GESTION|" & "ORGANIZACIONDOCUMENTAL_CONSULTA"
            attrNode_consulta_clasificacion.ImageUrl = "../workflow/imageneswf/search-regular.png"
            attrNode_organizacion_documental.ChildNodes.Add(attrNode_consulta_clasificacion)
            Dim attrNode_consulta_trd As New TreeNode
            attrNode_consulta_trd.Text = "Consulta de tablas de retención(TRD)"
            attrNode_consulta_trd.Value = "GESTION|" & "TABLARETENCIONDOCUMENTAL_CONSULTA"
            attrNode_consulta_trd.ImageUrl = "../workflow/imageneswf/search-regular.png"
            attrNode_organizacion_documental.ChildNodes.Add(attrNode_consulta_trd)
            Dim attrNode_consulta_org As New TreeNode
            attrNode_consulta_org.Text = "Administración de organigramas y estrucutura funcional"
            attrNode_consulta_org.Value = "GESTION|" & "GESTIONORGANIGRAMA_TRD"
            attrNode_consulta_org.ImageUrl = "../workflow/imageneswf/tools-light.png"
            If HttpContext.Current.Session.Item("GA_ADMINISTRACION_ORGANICA") = 1 Then
                attrNode_organizacion_documental.ChildNodes.Add(attrNode_consulta_org)
            End If
            Dim attrNode_gestion_instrumentos As New TreeNode
            attrNode_gestion_instrumentos.Text = "Administración de Instrumentos Archivisticos (TRD y TVD)"
            attrNode_gestion_instrumentos.Value = "GESTION|" & "GESTIONINSTRUMENTOS_ARCHIVI"
            attrNode_gestion_instrumentos.ImageUrl = "../workflow/imageneswf/tools-light.png"
            If HttpContext.Current.Session.Item("GA_ADMINISTRACION_TRD") = 1 Then
                attrNode_organizacion_documental.ChildNodes.Add(attrNode_gestion_instrumentos)
            End If
            Dim refclas_compartido As New ClassGaCompartirDocumento
            atrrnodeprincipal.ChildNodes.Add(attrNode_organizacion_documental)
            Dim attrNode_gestion_documentos As New TreeNode
            attrNode_gestion_documentos.Text = "Gestión de documentos"
            Dim attrNode_producion_documental As New TreeNode
            attrNode_producion_documental.Text = "Gestión de documentos electrónicos"
            attrNode_producion_documental.Value = "GESTION|" & "PRODUCCIONDOCUMENTAL_DOCUMENTOS"
            attrNode_producion_documental.ImageUrl = "../workflow/imageneswf/folder-tree-light.png"
            attrNode_gestion_documentos.ChildNodes.Add(attrNode_producion_documental)
            refclas_compartido.Retorna_numero_de_documentos_compartidos_de_un_usuario_para_otros_usuarios(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_NUMERO_COMPARTIDO"))
            Dim attrNode_mis_documentos_compartidos As New TreeNode
            attrNode_mis_documentos_compartidos.Text = "Mis documentos compartidos  (" & HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_NUMERO_COMPARTIDO") & ")"
            attrNode_mis_documentos_compartidos.Value = "GESTION|" & "COMPARTIDO_OTROS_USUARIOS"
            attrNode_mis_documentos_compartidos.ImageUrl = "../workflow/imageneswf/share-square-light.png"
            attrNode_gestion_documentos.ChildNodes.Add(attrNode_mis_documentos_compartidos)
            '--------------------------------------------------------------
            'Lista radicados internos enviados
            '--------------------------------------------------------------
            Dim Refclas_producion As New ClassGaProducionDocumental
            Dim Result_planti As String = ""
            Dim nombre_plantilla_radicado As String = ""
            Dim Id_Plantilla As Integer = 0
            Dim Class_system_plantilla_radicado As New Class_system_plantilla_radicado
            Result_planti = Class_system_plantilla_radicado.Solicita_nombre_id_plantilla_radicación_interna_default(nombre_plantilla_radicado,
                                                                                                                   Id_Plantilla,
                                                                                                                   0)
            If Result_planti <> "YES" Then
                'Tri_View_gestion = Result_planti
                'Exit Function
            End If
            If nombre_plantilla_radicado <> "" Then
                Dim attrNode_documentos_radicados As New TreeNode
                attrNode_documentos_radicados.Text = "Consultar mis radicaciones internas"
                attrNode_documentos_radicados.Value = "CONSULTAGESTION|" & Id_Plantilla.ToString & "|" & "RADICACION ENTRANTE" & "|" & "0" & "|" & nombre_plantilla_radicado
                attrNode_documentos_radicados.ImageUrl = "../workflow/imageneswf/search-regular.png"
                attrNode_gestion_documentos.ChildNodes.Add(attrNode_documentos_radicados)
                Dim attrNode_documentos_radicados_pendientes_enviar As New TreeNode
                attrNode_documentos_radicados_pendientes_enviar.Text = "Mis radicaciones internas pendientes por asignar"
                attrNode_documentos_radicados_pendientes_enviar.Value = "GESTIONPENDIENTES|" & Id_Plantilla.ToString & "|" & "RADICACION ENTRANTE" & "|" & "0" & "|" & nombre_plantilla_radicado
                attrNode_documentos_radicados_pendientes_enviar.ImageUrl = "../workflow/imageneswf/search-regular.png"
                attrNode_gestion_documentos.ChildNodes.Add(attrNode_documentos_radicados_pendientes_enviar)
            End If
            atrrnodeprincipal.ChildNodes.Add(attrNode_gestion_documentos)
            Dim attrNode_documentos_compartidos As New TreeNode
            attrNode_documentos_compartidos.Text = "Notificaciones y solicitudes"
            attrNode_documentos_compartidos.Expand()

            Dim result As String = refclas_compartido.Retorna_numero_de_documentos_compartidos_de_un_usuario(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_PENDIENTE_REVISION"))
            Dim attrNode_compartidos_para_revision As New TreeNode
            attrNode_compartidos_para_revision.Text = "Documentos compartidos pendientes por mi revisión (" & HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_PENDIENTE_REVISION") & ")"
            attrNode_compartidos_para_revision.Value = "GESTION|" & "COMPARTIDO_PENDIENTE_REVISION"
            attrNode_compartidos_para_revision.ImageUrl = "../workflow/imageneswf/bell-light.png"
            attrNode_documentos_compartidos.ChildNodes.Add(attrNode_compartidos_para_revision)
            refclas_compartido.Retorna_numero_de_documentos_compartidos_de_un_usuario_para_otros_usuarios(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_NUMERO_COMPARTIDO"))
            'Dim attrNode_mis_documentos_compartidos As New TreeNode
            'attrNode_mis_documentos_compartidos.Text = "Documentos compartidos a otros usuarios (" & HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_NUMERO_COMPARTIDO") & ")"
            'attrNode_mis_documentos_compartidos.Value = "GESTION|" & "COMPARTIDO_OTROS_USUARIOS"
            'attrNode_mis_documentos_compartidos.ImageUrl = "../workflow/imageneswf/bell-light.png"
            'attrNode_documentos_compartidos.ChildNodes.Add(attrNode_mis_documentos_compartidos)
            atrrnodeprincipal.ChildNodes.Add(attrNode_documentos_compartidos)
            Dim numero_slicitudes As Integer = 0
            Dim Refclas As New ClassRaSolicitudesAprobacion
            result = Refclas.Retorna_numero_de_solicitudes_aprobacion_de_un_usuario(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), HttpContext.Current.Session.Item("GA_NUMERO_SOLICITUDES_PENDIENTES_APROBAR_USUARIO"))
            Dim attrNode1Gru5 As New TreeNode
            'attrNode1Gru5.SelectAction = TreeNodeSelectAction.SelectExpand
            attrNode1Gru5.Text = "Respuestas pendientes por mi aprobación (" & HttpContext.Current.Session.Item("GA_NUMERO_SOLICITUDES_PENDIENTES_APROBAR_USUARIO") & ")"
            attrNode1Gru5.Value = "WF-SPA-06"
            attrNode1Gru5.ImageUrl = "../workflow/imageneswf/bell-light.png"
            attrNode_documentos_compartidos.ChildNodes.Add(attrNode1Gru5)
            Dim attrNode_usuario As New TreeNode
            attrNode_usuario.Text = "Usuario"
            Dim attrNode_pasword As New TreeNode
            attrNode_pasword.Text = "Contraseña usuario"
            attrNode_pasword.Value = "GESTION|" & "CONTRASEÑA"
            attrNode_pasword.ImageUrl = "../workflow/imageneswf/key-light.png"
            attrNode_usuario.ChildNodes.Add(attrNode_pasword)
            atrrnodeprincipal.ChildNodes.Add(attrNode_usuario)
            Tre_v.Nodes.Add(atrrnodeprincipal)
            Tri_View_gestion = "YES"
        Catch ex As Exception
            Tri_View_gestion = "Error General Funcion Tri_View_gestion : " & ex.Message
        End Try
    End Function
    Function Crea_Dir_Temporal_gestion() As String
        Try
            Dim Ruttempo As String = HttpContext.Current.Server.MapPath(HttpContext.Current.Session.Item("GA_RUTA_TEMPO") + HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString)
            If Directory.Exists(Ruttempo) = False Then
                Directory.CreateDirectory(Ruttempo)
            End If
            '--------------------------------
            'Crea ruta escaneo
            '--------------------------------
            Dim ruta_escaner As String = Ruttempo & "\ESCANER"
            If Directory.Exists(ruta_escaner) = False Then
                Directory.CreateDirectory(ruta_escaner)
            End If
            Dim ruta_impresioin As String = Ruttempo & "\IMPRESION"
            If Directory.Exists(ruta_impresioin) = False Then
                Directory.CreateDirectory(ruta_impresioin)
            End If
            Dim ruta_descarga As String = Ruttempo & "\DESCARGA"
            If Directory.Exists(ruta_descarga) = False Then
                Directory.CreateDirectory(ruta_descarga)
            End If
            Dim ruta_firma As String = Ruttempo & "\FIRMA"
            If Directory.Exists(ruta_firma) = False Then
                Directory.CreateDirectory(ruta_firma)
            End If
            Dim RutaTempGestion As String = Ruttempo & "\TEMP"
            If Directory.Exists(RutaTempGestion) = False Then
                Directory.CreateDirectory(RutaTempGestion)
            End If
            HttpContext.Current.Session.Item("GA_RUTA_TEMPO_IMPRESION") = ruta_impresioin
            HttpContext.Current.Session.Item("GA_RUTA_TEMPO_ESCANER") = ruta_escaner
            HttpContext.Current.Session.Item("GA_RUTA_TEMPO_DESCARGA") = ruta_descarga
            HttpContext.Current.Session.Item("GA_RUTA_FIRMA_GESTION") = ruta_firma
            HttpContext.Current.Session.Item("GA_RUTA_TEMP_GESTION") = RutaTempGestion
            HttpContext.Current.Session.Item("GA_RUTA_TEMP_GESTION_URL") = HttpContext.Current.Session.Item("GA_RUTA_TEMP_GESTION_URL") & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "/TEMP/"
            Crea_Dir_Temporal_gestion = "YES"
        Catch EX As Exception
            Crea_Dir_Temporal_gestion = EX.Message
        End Try
    End Function
End Class
