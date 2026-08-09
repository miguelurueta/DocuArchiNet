Imports AjaxControlToolkit
Imports MySql.Data.MySqlClient
Imports Neodynamic.SDK.Web
Imports System.IO
Imports System.Drawing
Imports System.Globalization
Public Structure STRU_SOLICITUD_ARPBACION
    Dim Ra_Respuesta_Radicado_ID_RESPUESTA_RADICAD As Integer
    Dim Remit_Dest_Interno_id_remit_dest_Int As Integer
    Dim FECHA_REGISTRO_SOLICITUD As String
    Dim FECHA_REGISTRO_APROBACION As String
    Dim ESTADO_APROBACION As Integer
    Dim TIEMPO_RESPUESTA_APROBACION As Long
    Dim ESTADO_PRIORIDAD As Integer
    Dim NOTA_SOLICITUD As String
    Dim DESCRIPCION_ESTADO_APROBACION As String
    Dim FECHA_LIMITE_RESPUESTA As String
End Structure
Public Structure STRU_SOLICITUD_APROBACION_USUARIOS
    Dim Remit_Dest_Interno_id_remit_dest_Int As Integer
    Dim RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION As Integer
    Dim FECHA_REGISTRO_SOLICITUD As String
    Dim FECHA_RESPUESTA_SOLICITUD As String
    Dim ESTADO_RESPUESTA_SOLICITUD As Integer
    Dim TIEMPO_RESPUESTA_SOLICITUD As Long
    Dim ESTADO_VISTO_SOLICITANTE As Integer
    Dim DESCRIPCION_ESTADO_RESPUESTA As String
    Dim FECHA_LIMITE_RESPUESTA As String
End Structure
Public Class ClassRaSolicitudesAprobacion
    Function Lista_documentos_correcion_aprobacion_interface(ByRef Page_manager As Page, _
                                                             ByVal id_usuario_documento_compartido As Integer) As String
        '----------------------------------------------------------------
        'Función : Genera interface documentos relacionados de correción
        'ing : Miguel Angel Urueta Miranda
        'Fecha : 2017-06-05
        '----------------------------------------------------------------
        Try
            Dim pane As Panel = Page_manager.FindControl("Panel_seleccion_documento")
            Dim Update As UpdatePanel = Page_manager.FindControl("UpdatePanel_seleccion_documento")
            Dim Table As Table = Page_manager.FindControl("Table_seleccion_documento")
            Dim Label As Label = Page_manager.FindControl("Label_estado_doc_colaboracion")
            Dim Update_panel As UpdatePanel = Page_manager.FindControl("UpdatePanel_estado_doc_colaboracion")
            Dim objRow As TableRow
            Dim objCell As TableCell
            Dim label_nombre() As Label = {}
            Dim buton_decarga() As HtmlGenericControl = {}
            Dim Result As String = ""
            Dim stru() As stru_documentos_colaboracion = Nothing
            Result = Me.Lista_documentos_correcion_aprobacion_usuario(id_usuario_documento_compartido, stru, Label, Update_panel)
            If Result <> "YES" Then
                Lista_documentos_correcion_aprobacion_interface = Result
                Exit Function
            End If
            Dim i_conlabel As Integer = 0
            For i As Integer = 0 To stru.Length - 1
                If i = 0 Then
                    objRow = New TableRow
                    objCell = New TableCell
                    ReDim Preserve label_nombre(i_conlabel)
                    label_nombre(i_conlabel) = New Label
                    label_nombre(i_conlabel).Text = "DOCUMENTO"
                    objCell.Controls.Add(label_nombre(i_conlabel))
                    objCell.HorizontalAlign = HorizontalAlign.Center
                    'objCell.BorderWidth = 4
                    'objCell.BorderColor = Color.Blue
                    objRow.Cells.Add(objCell)
                    objCell = New TableCell
                    i_conlabel = i_conlabel + 1
                    ReDim Preserve label_nombre(i_conlabel)
                    label_nombre(i_conlabel) = New Label
                    label_nombre(i_conlabel).Text = "DESCARGA"
                    objCell.Controls.Add(label_nombre(i_conlabel))
                    objCell.HorizontalAlign = HorizontalAlign.Center
                    'objCell.BorderWidth = 4
                    'objCell.BorderColor = Color.Blue
                    objRow.Cells.Add(objCell)
                    Table.Rows.Add(objRow)
                    i_conlabel = i_conlabel + 1
                End If
                objRow = New TableRow
                objCell = New TableCell
                'objCell.HorizontalAlign = HorizontalAlign.Center
                ReDim Preserve label_nombre(i_conlabel)
                label_nombre(i_conlabel) = New Label
                label_nombre(i_conlabel).Text = stru(i).nombre_archivo
                objCell.Controls.Add(label_nombre(i_conlabel))
                objRow.Cells.Add(objCell)
                i_conlabel = i_conlabel + 1
                objCell = New TableCell
                objCell.HorizontalAlign = HorizontalAlign.Center
                ReDim Preserve buton_decarga(i)
                buton_decarga(i) = New HtmlGenericControl("input")
                buton_decarga(i).Attributes.Add("Type", "button")
                buton_decarga(i).Attributes.Add("width", "50px")
                buton_decarga(i).Attributes.Add("value", "Descarga")
                buton_decarga(i).Attributes.Add("class", "btn btn-success")
                buton_decarga(i).Attributes.Add("onclick", "dercaga_documento(this);")
                buton_decarga(i).ID = stru(i).ID_IMAGEN & "|" & stru(i).NOMBRE_GABINETE
                objCell.Controls.Add(buton_decarga(i))
                objRow.Cells.Add(objCell)
                Table.Rows.Add(objRow)
            Next
            Update.Update()
            Lista_documentos_correcion_aprobacion_interface = "YES"
        Catch ex As Exception
            Lista_documentos_correcion_aprobacion_interface = "Inconsistencia general función Lista_documentos_colaboracion_interface " & ex.Message
        End Try
    End Function
    Function Lista_documentos_correcion_aprobacion_interface_general(ByRef Page_manager As Page, _
                                                                     ByVal id_solictitud_aprobacion As Integer) As String
        '----------------------------------------------------------------
        'Función : Genera interface documentos relacionados de correción
        'ing : Miguel Angel Urueta Miranda
        'Fecha : 2017-06-05
        '----------------------------------------------------------------
        Try
            Dim pane As Panel = Page_manager.FindControl("Panel_seleccion_documento")
            Dim Update As UpdatePanel = Page_manager.FindControl("UpdatePanel_seleccion_documento")
            Dim Table As Table = Page_manager.FindControl("Table_seleccion_documento")
            Dim Label As Label = Page_manager.FindControl("Label_estado_doc_colaboracion")
            Dim Update_panel As UpdatePanel = Page_manager.FindControl("UpdatePanel_estado_doc_colaboracion")
            Dim objRow As TableRow
            Dim objCell As TableCell
            Dim label_nombre() As Label = {}
            Dim buton_decarga() As HtmlGenericControl = {}
            Dim Result As String = ""
            Dim stru() As stru_documentos_colaboracion = Nothing
            Result = Me.Lista_documentos_correcion_aprobacion_documento_compartido(id_solictitud_aprobacion, _
                                                                                   stru, _
                                                                                   Label, _
                                                                                   Update_panel)
            If Result <> "YES" Then
                Lista_documentos_correcion_aprobacion_interface_general = Result
                Exit Function
            End If
            Dim i_conlabel As Integer = 0
            For i As Integer = 0 To stru.Length - 1
                If i = 0 Then
                    objRow = New TableRow
                    objCell = New TableCell
                    ReDim Preserve label_nombre(i_conlabel)
                    label_nombre(i_conlabel) = New Label
                    label_nombre(i_conlabel).Text = "DOCUMENTO"
                    objCell.Controls.Add(label_nombre(i_conlabel))
                    objCell.HorizontalAlign = HorizontalAlign.Center
                    'objCell.BorderWidth = 4
                    'objCell.BorderColor = Color.Blue
                    objRow.Cells.Add(objCell)
                    objCell = New TableCell
                    i_conlabel = i_conlabel + 1
                    ReDim Preserve label_nombre(i_conlabel)
                    label_nombre(i_conlabel) = New Label
                    label_nombre(i_conlabel).Text = "DESCARGA"
                    objCell.Controls.Add(label_nombre(i_conlabel))
                    objCell.HorizontalAlign = HorizontalAlign.Center
                    'objCell.BorderWidth = 4
                    'objCell.BorderColor = Color.Blue
                    objRow.Cells.Add(objCell)
                    Table.Rows.Add(objRow)
                    i_conlabel = i_conlabel + 1
                End If
                objRow = New TableRow
                objCell = New TableCell
                'objCell.HorizontalAlign = HorizontalAlign.Center
                ReDim Preserve label_nombre(i_conlabel)
                label_nombre(i_conlabel) = New Label
                label_nombre(i_conlabel).Text = stru(i).nombre_archivo
                objCell.Controls.Add(label_nombre(i_conlabel))
                objRow.Cells.Add(objCell)
                i_conlabel = i_conlabel + 1
                objCell = New TableCell
                objCell.HorizontalAlign = HorizontalAlign.Center
                ReDim Preserve buton_decarga(i)
                buton_decarga(i) = New HtmlGenericControl("input")
                buton_decarga(i).Attributes.Add("Type", "button")
                buton_decarga(i).Attributes.Add("width", "50px")
                buton_decarga(i).Attributes.Add("value", "Descarga")
                buton_decarga(i).Attributes.Add("class", "btn btn-success")
                buton_decarga(i).Attributes.Add("onclick", "dercaga_documento(this);")
                buton_decarga(i).ID = stru(i).ID_IMAGEN & "|" & stru(i).NOMBRE_GABINETE
                objCell.Controls.Add(buton_decarga(i))
                objRow.Cells.Add(objCell)
                Table.Rows.Add(objRow)
            Next
            Update.Update()
            Lista_documentos_correcion_aprobacion_interface_general = "YES"
        Catch ex As Exception
            Lista_documentos_correcion_aprobacion_interface_general = "Inconsistencia general función Lista_documentos_correcion_aprobacion_interface_general " & ex.Message
        End Try
    End Function
    Function Elimina_documento_correcion_documento_colaboracion_dorw_list(ByVal nombre_documento As String, _
                                                                          ByRef drow_list As DropDownList, _
                                                                          ByRef update As UpdatePanel _
                                                                           ) As String
        '------------------------------------------------
        'Función : Elimina el documento de colaboración
        'seleccionando de la tabla de documentos de 
        'colabración
        'Ing Miguel Angel Urueta Miranda
        'Fecha : 2017-06-07
        '-------------------------------------------------
        Try
            Dim Result As String = ""
            Dim stru() As stru_documentos_colaboracion = Nothing
            Result = Me.Lista_documentos_correcion_aprobacion_por_nombre_archivo(nombre_documento, stru)
            If Result <> "YES" Then
                Elimina_documento_correcion_documento_colaboracion_dorw_list = Result
                Exit Function
            End If
            Dim Delte_sql As String = "Delete from ra_cd_documentos_corregidos_solicitudes_aprobacion where nombre_archivo='" & nombre_documento & "'"
            'Elimina_documento_colaboracion_documento_compartido_dorw_list = "YES"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Result = ref.SELECTION_INSERT_COMMAND(Delte_sql)
            If Result <> "YES" Then
                Elimina_documento_correcion_documento_colaboracion_dorw_list = Result
                Exit Function
            Else
                drow_list.Items.Remove(nombre_documento)
                update.Update()
                Dim ref_class As New ClassEliminarDocListResult
                Result = ref_class.EliminarDocumentosGabinete(stru(0).ID_IMAGEN,
                                                                    0,
                                                                    stru(0).NOMBRE_GABINETE,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    -1,
                                                                    "MODULOAPROBACION")
                If Result <> "YES" Then
                    Elimina_documento_correcion_documento_colaboracion_dorw_list = "El documento se elimino de la lista pero no  del gabinete por este error " & Result
                    Exit Function
                Else
                    Elimina_documento_correcion_documento_colaboracion_dorw_list = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Elimina_documento_correcion_documento_colaboracion_dorw_list = "Inconsistencia general función Elimina_documento_correcion_documento_colaboracion_dorw_list " & ex.Message
        End Try
    End Function
    Function Lista_documentos_correcion_aprobacion_por_nombre_archivo(ByVal nombre_archivo As String, ByRef stru() As stru_documentos_colaboracion) As String
        '--------------------------------------------
        'Función : Retorna lista de documentos
        'de corrección de un usuario 
        'Fecha : 2017-06-07
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------
        Try
            Dim sql_consulta As String = "SELECT ID_IMAGEN,NOMBRE_GABINETE,nombre_archivo  from ra_cd_documentos_corregidos_solicitudes_aprobacion " & _
                          " where nombre_archivo='" & nombre_archivo & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_documentos_correcion_aprobacion_por_nombre_archivo = "Error listando Lista_documentos_correcion_aprobacion_por_nombre_archivo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_documentos_correcion_aprobacion_por_nombre_archivo = "No se encontraron documentos de corección "
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru(i)
                    stru(i).ID_IMAGEN = Datset.Tables(0).Rows(i).Item(0)
                    stru(i).NOMBRE_GABINETE = Datset.Tables(0).Rows(i).Item(1)
                    stru(i).nombre_archivo = Datset.Tables(0).Rows(i).Item(2)
                Next
                Lista_documentos_correcion_aprobacion_por_nombre_archivo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_documentos_correcion_aprobacion_por_nombre_archivo = "Inconsistencia general función Lista_documentos_correcion_aprobacion_por_nombre_archivo " & ex.Message
        End Try
    End Function
    Function Lista_documentos_correcion_aprobacion_usuario(ByVal id_usuario_compartido As Integer, _
                                                           ByRef stru() As stru_documentos_colaboracion, _
                                                           ByRef ref_label_estado As Label, _
                                                           ByRef update_pane As UpdatePanel) As String
        '--------------------------------------------
        'Función : Retorna lista de documentos
        'de corrección de un usuario 
        'Fecha : 2017-06-06
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------
        Try
            Dim sql_consulta As String = "SELECT ID_IMAGEN,NOMBRE_GABINETE,nombre_archivo  from ra_cd_documentos_corregidos_solicitudes_aprobacion " & _
                          " where ID_CD_USUARIOS_SOLICITUDES_APROBACION=" & id_usuario_compartido
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_documentos_correcion_aprobacion_usuario = "Error listando Lista_documentos_correcion_aprobacion_usuario " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                ref_label_estado.Text = "Numero registro(s) " & Datset.Tables(0).Rows.Count
                update_pane.Update()
                Lista_documentos_correcion_aprobacion_usuario = "No se encontraron documentos de corrección "
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru(i)
                    stru(i).ID_IMAGEN = Datset.Tables(0).Rows(i).Item(0)
                    stru(i).NOMBRE_GABINETE = Datset.Tables(0).Rows(i).Item(1)
                    stru(i).nombre_archivo = Datset.Tables(0).Rows(i).Item(2)
                Next
                ref_label_estado.Text = "Numero registro(s) " & Datset.Tables(0).Rows.Count
                update_pane.Update()
                Lista_documentos_correcion_aprobacion_usuario = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_documentos_correcion_aprobacion_usuario = "Inconsistencia general función Lista_documentos_correcion_aprobacion_usuario " & ex.Message
        End Try
    End Function
    Function Lista_documentos_correcion_aprobacion_documento_compartido(ByVal id_documento_solicitud_aprobacion As Integer, ByRef stru() As stru_documentos_colaboracion, _
                                                           ByRef ref_label_estado As Label, ByRef update_pane As UpdatePanel) As String
        '--------------------------------------------
        'Función : Retorna lista de documentos
        'de corrección de un usuario 
        'Fecha : 2017-06-06
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------
        Try
            Dim sql_consulta As String = "SELECT rcd.ID_IMAGEN,rcd.NOMBRE_GABINETE,rcd.nombre_archivo  from ra_cd_usuarios_solicitudes_aprobacion as rcu  " & _
                "INNER JOIN ra_cd_documentos_corregidos_solicitudes_aprobacion AS rcd on (rcd.ID_CD_USUARIOS_SOLICITUDES_APROBACION=rcu.ID_CD_USUARIOS_SOLICITUDES_APROBACION) " & _
            " where rcu.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION=" & id_documento_solicitud_aprobacion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_documentos_correcion_aprobacion_documento_compartido = "Error listando Lista_documentos_correcion_aprobacion_documento_compartido " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                ref_label_estado.Text = "Numero registro(s) " & Datset.Tables(0).Rows.Count
                update_pane.Update()
                Lista_documentos_correcion_aprobacion_documento_compartido = "No se encontraron documentos de corrección "
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru(i)
                    stru(i).ID_IMAGEN = Datset.Tables(0).Rows(i).Item(0)
                    stru(i).NOMBRE_GABINETE = Datset.Tables(0).Rows(i).Item(1)
                    stru(i).nombre_archivo = Datset.Tables(0).Rows(i).Item(2)
                Next
                ref_label_estado.Text = "Numero registro(s) " & Datset.Tables(0).Rows.Count
                update_pane.Update()
                Lista_documentos_correcion_aprobacion_documento_compartido = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_documentos_correcion_aprobacion_documento_compartido = "Inconsistencia general función Lista_documentos_correcion_aprobacion_documento_compartido " & ex.Message
        End Try
    End Function
    Function Registra_documento_correcion_aprobacion(ByVal Nombre_Gabinete As String, _
        ByVal id_solicitud_aprobacion As Integer, ByVal ruta_document As String, ByRef Drowplist As DropDownList, ByVal id_usuario_gestion As Integer) As String
        Try
            Dim id_usuario_compartido As Integer = 0
            Dim Result As String = ""
            Result = Me.Retorna_id_usuario_solicitud_aprobacion(id_solicitud_aprobacion, id_usuario_compartido, id_usuario_gestion)
            If Result <> "YES" Then
                Registra_documento_correcion_aprobacion = Result
                Exit Function
            End If
            Dim id_imagen As Integer = 0
            Result = Me.Guardar_Documento_coreccion_solicitud_aprobacion(id_imagen, Nombre_Gabinete, id_solicitud_aprobacion, ruta_document)
            If Result <> "YES" Then
                Registra_documento_correcion_aprobacion = Result
                Exit Function
            End If
            Dim Refclas_radicador As New ClassRadicador
            Dim date1al As String = Date.Now
            Result = ""
            Dim refclas_gestion_fechas As New ClassGestionFechas
            Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(date1al)
            If Result <> "YES" Then
                Registra_documento_correcion_aprobacion = "Imposible formatear fecha " & Result
                Exit Function
            End If
            Dim file_inf As New FileInfo(ruta_document)
            Dim sql_insert As String = "Insert into ra_cd_documentos_corregidos_solicitudes_aprobacion (RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION," & _
                         "ID_IMAGEN,NOMBRE_GABINETE,ID_CD_USUARIOS_SOLICITUDES_APROBACION,FECHA_REGISTRO_DOCUMENTO,nombre_archivo) values (" &
                         id_solicitud_aprobacion & "," & id_imagen & ",'" & Nombre_Gabinete & "'," & id_usuario_compartido & ",'" & date1al & "','" & id_imagen & "|" & file_inf.Name & "')"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Result = ref.SELECTION_INSERT_COMMAND(sql_insert)
            If Result <> "YES" Then
                Registra_documento_correcion_aprobacion = "Error Registra_documento_correcion_aprobacion " & Result
                Exit Function
            Else
                Drowplist.Items.Add(id_imagen & "|" & file_inf.Name)
                Registra_documento_correcion_aprobacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Registra_documento_correcion_aprobacion = "Inconsistencia general función Registra_documento_correcion_aprobacion " & ex.Message
        End Try
    End Function
    Function Guardar_Documento_coreccion_solicitud_aprobacion(ByRef Id_imagen As Integer, _
                                                              ByVal Nombre_Gabinete As String, _
                                                              ByVal id_documento_compartido As Integer, _
                                                              ByVal ruta_document As String) As String
        Try
            Dim Refeclasaladir As New ClassAñadirDocumento
            Dim MatriDatosAlmacen() As String
            Erase MatriDatosAlmacen
            Dim Result As String = ""
            Dim Refalmacena As New ClassAlmacenamiento
            Dim option_unidad_conservacion As Integer = 0
            Dim ref_Class_system1 As New Class_system1
            Result = ref_Class_system1.Verfica_opcion_seleccion_unidad(option_unidad_conservacion, _
                                                                       Nombre_Gabinete)
            If Result <> "YES" Then
                Guardar_Documento_coreccion_solicitud_aprobacion = "Inconsistencia verficando opción asignación unidad y expediente codigo :  " & Result
                Exit Function
            End If
            Dim matri_datos() As Datos_Almacenamiento
            ReDim Preserve matri_datos(0)
            matri_datos(0).nombre_campo = "NUMERORADICA"
            matri_datos(0).valor_campo = ""
            ReDim Preserve matri_datos(1)
            matri_datos(1).nombre_campo = "ENLASE"
            matri_datos(1).valor_campo = ""
            ReDim Preserve matri_datos(2)
            matri_datos(2).nombre_campo = "IDREGAPRUEBA"
            matri_datos(2).valor_campo = id_documento_compartido
            If option_unidad_conservacion = 1 Then
                ReDim Preserve matri_datos(3)
                matri_datos(3).nombre_campo = "CLASEDOCUMENTO"
                matri_datos(3).valor_campo = "DOCUMENTO ELECTRONICO"
                Dim date1al As String = Date.Today
                Result = ""
                Dim ref_ClassGestionFechas As New ClassGestionFechas
                Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
                If Result <> "YES" Then
                    Guardar_Documento_coreccion_solicitud_aprobacion = "Error formatenado fecha alamcenamiento Funcion: Formatea_Fecha_Almacenamiento " & Result
                    Exit Function
                End If
                ReDim Preserve matri_datos(3)
                matri_datos(3).nombre_campo = "CLASEDOCUMENTO"
                matri_datos(3).valor_campo = "DOCUMENTO ELECTRONICO"
            End If
            Dim RefclasAñadir As New ClassAñadirDocumento
            Dim Refclaswfdigtializado As New ClassWorkflowDigitalizacion
            Dim Matri_Datos_Almacen() As String
            Erase Matri_Datos_Almacen
            Result = Refclaswfdigtializado.Obtiene_Valores_Campos_Documento_Enlazados(Matri_Datos_Almacen, Nombre_Gabinete, matri_datos)
            If Result <> "YES" Then
                Guardar_Documento_coreccion_solicitud_aprobacion = "Imposible general mariz datos almacenamiento "
                Exit Function
            End If
            If Matri_Datos_Almacen Is Nothing Then
                Guardar_Documento_coreccion_solicitud_aprobacion = "Matriz de datos de almacenamiento es nothing "
                Exit Function
            End If
            Dim Filein As New IO.FileInfo(ruta_document)
            Result = ""
            Dim Tipo_Doc_int As Integer = -1
            Dim Refclasvisor As New Classactualizacionvisor
            Result = ""
            Dim Class_da_extension As New Class_da_extension
            Result = Class_da_extension.SolicitaTipoArchivoDocuarchiExtension(UCase(Filein.Extension), Tipo_Doc_int)
            If Result <> "YES" Then
                Guardar_Documento_coreccion_solicitud_aprobacion = Result
                Exit Function
            End If
            '-------------------------------------------
            'Asigna datos gestion
            '-------------------------------------------
            Dim matri_gestion As estructure_gestion = Nothing
            'matri_gestion = Nothing
            matri_gestion.CLASE_DOCUMENTO = ""
            matri_gestion.EXPEDIENTE = ""
            matri_gestion.ID_AREA = 0
            matri_gestion.ID_CLASE_DOCUMENTO = 0
            matri_gestion.ID_EXPEDIENTE = 0
            matri_gestion.ID_SERIE = 0
            matri_gestion.ID_SUB_SERIE = 0
            matri_gestion.ID_TIPO_EXPEDIENTE = 0
            matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_TIPODOCUMENTO = 0
            matri_gestion.ID_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_USUARIO_GESTION = 0
            matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
            matri_gestion.UNIDAD_CONSERVACION = ""
            matri_gestion.FECHA_ELABORACION = ""
            Id_imagen = Tipo_Doc_int
            Dim radicado As String = ""
            Dim matri_documento() As String = {ruta_document}
            Result = Refalmacena.Almacenamiento("", "", Nombre_Gabinete, 0, Matri_Datos_Almacen, _
            2, 1, Tipo_Doc_int, matri_documento, 0, Id_imagen, Tipo_Doc_int, HttpContext.Current.Session.Item("GA_IDEMPRESA"), HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), matri_gestion.ID_AREA, matri_gestion.ID_SERIE, matri_gestion.ID_SUB_SERIE, _
            matri_gestion.ID_TIPODOCUMENTO, matri_gestion.ID_EXPEDIENTE, matri_gestion.ID_TIPO_EXPEDIENTE, _
            matri_gestion.ID_UNIDAD_CONSERVACION, matri_gestion.ID_TIPO_UNIDAD_CONSERVACION, _
            matri_gestion.ID_CLASE_DOCUMENTO, matri_gestion.EXPEDIENTE, matri_gestion.NOMBRE_SERIE, _
            matri_gestion.NOMBRE_SUB_SERIE, matri_gestion.TIPODOCUMENTO, matri_gestion.UNIDAD_CONSERVACION, _
            matri_gestion.CLASE_DOCUMENTO, matri_gestion.FECHA_ELABORACION, radicado)
            If Result <> "YES" Then
                Guardar_Documento_coreccion_solicitud_aprobacion = "Almacenando  dice " & Result
                Exit Function
            End If
            Guardar_Documento_coreccion_solicitud_aprobacion = "YES"
            Exit Function
        Catch ex As Exception
            Guardar_Documento_coreccion_solicitud_aprobacion = "Inconsistencia función Guardar_Documento_coreccion_solicitud_aprobacion " & ex.Message
        End Try
    End Function
    Function Retorna_id_usuario_solicitud_aprobacion(ByVal id_solicitud_aprobacion As Integer, ByRef id_usuario_solicitud_aprobacion As Integer, ByVal id_usuario_gestion As Integer) As String
        '--------------------------------------------
        'Función : Retorna id usuario relacionado
        'a la solicitud de aprobación
        'Fecha : 2017-06-07
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------
        Try
            Dim sql_consulta As String = "SELECT ID_CD_USUARIOS_SOLICITUDES_APROBACION  from ra_cd_usuarios_solicitudes_aprobacion " & _
                          " where RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION=" & id_solicitud_aprobacion & " and Remit_Dest_Interno_id_Remit_Dest_Int=" & id_usuario_gestion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_usuario_solicitud_aprobacion = "Error listando Retorna_id_usuario_solicitud_aprobacion " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_usuario_solicitud_aprobacion = "Imponsible encontrar el id usuario del documento compartido"
                Exit Function
            Else
                id_usuario_solicitud_aprobacion = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_usuario_solicitud_aprobacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_usuario_solicitud_aprobacion = "Inconsistencia general función Retorna_id_usuario_solicitud_aprobacion " & ex.Message
        End Try
    End Function
    Function Lista_documentos_de_correccion_aprobacion_drow_list(ByVal id_solicitud_aprobacion As Integer, _
                                                                 ByVal id_usuario_solicitud_aprobacion As Integer, _
                                                                 ByRef drow_list As DropDownList, _
                                                                 ByRef update As UpdatePanel) As String
        '----------------------------------------------------------------
        'Función : Lista los documentos de correción de un usuario es
        'pecifico
        'Ing :Miguel Angel Urueta Miranda
        'Fecha : 2017-06-07
        '-----------------------------------------------------------------
        Try
            drow_list.Items.Clear()
            Dim sql_consulta As String = "SELECT nombre_archivo  from ra_cd_documentos_corregidos_solicitudes_aprobacion " & _
                             " where RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION=" & id_solicitud_aprobacion & " and ID_CD_USUARIOS_SOLICITUDES_APROBACION=" & id_usuario_solicitud_aprobacion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_documentos_de_correccion_aprobacion_drow_list = "Error listando Lista_documentos_de_colaboracion_usuario_colaboracion " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_documentos_de_correccion_aprobacion_drow_list = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    drow_list.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                update.Update()
                Lista_documentos_de_correccion_aprobacion_drow_list = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_documentos_de_correccion_aprobacion_drow_list = "Inconsistencia general función Lista_documentos_de_correccion_aprobacion_drow_list " & ex.Message
        End Try
    End Function
    Function Retorna_estado_solicitudes_pendientes_por_aprobacion(ByVal id_usuario_gestion As Integer, _
                                                                  ByRef estado_solicitud As String) As String
        '-----------------------------------------------------------
        'Función : Lista estado solicitudes de aprobación pendientes
        'por aprobación
        'Fecha : 2017-03-03  
        'Ing Miguel Angel Urueta Miranda
        '-----------------------------------------------------------
        Try
            Dim sql_consulta As String = "Select ID_RESPUESTA_RADICADO from ra_respuesta_radicado where ID_REMIT_DEST_INT=" & id_usuario_gestion & _
                " and FECHA_REGISTRO_APROBACION is null and ID_SOLICITUDES_APROBACION_RESP is not  null"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_solicitudes_aprobacion")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_estado_solicitudes_pendientes_por_aprobacion = "Error Función Retorna_estado_solicitudes_pendientes_por_aprobacion dice  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_solicitud = "NO"
                Retorna_estado_solicitudes_pendientes_por_aprobacion = "YES"
                Exit Function
            Else
                estado_solicitud = "YES"
                Retorna_estado_solicitudes_pendientes_por_aprobacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_estado_solicitudes_pendientes_por_aprobacion = "Inconsistencia general función Retorna_estado_solicitudes_pendientes_por_aprobacion " & ex.Message
        End Try
    End Function
    Function Retorna_estado_solicitudes_pendientes_aprobadas(ByVal id_usuario_gestion As Integer, _
                                                             ByRef estado_solicitud As String) As String
        '-----------------------------------------------------------
        'Función : Lista estado solicitudes aprobadas
        'por aprobación
        'Fecha : 2017-03-03  
        'Ing Miguel Angel Urueta Miranda
        '-----------------------------------------------------------
        Try
            Dim sql_consulta As String = "Select ID_RESPUESTA_RADICADO from ra_respuesta_radicado where ID_REMIT_DEST_INT=" & id_usuario_gestion & _
                " and FECHA_REGISTRO_APROBACION  is not null and ID_SOLICITUDES_APROBACION_RESP is not  null and fecha_respueta is null"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_solicitudes_aprobacion")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_estado_solicitudes_pendientes_aprobadas = "Error Función Retorna_estado_solicitudes_pendientes_aprobadas dice  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_solicitud = "NO"
                Retorna_estado_solicitudes_pendientes_aprobadas = "YES"
                Exit Function
            Else
                estado_solicitud = "YES"
                Retorna_estado_solicitudes_pendientes_aprobadas = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_estado_solicitudes_pendientes_aprobadas = "Inconsistencia general función Retorna_estado_solicitudes_pendientes_aprobadas " & ex.Message
        End Try
    End Function

    Function Retorna_numero_solicitudes_pendientes_por_aprobacion(ByVal id_usuario_gestion As Integer, _
                                                                  ByVal radicado As String,
                                                                  ByRef numero_solicitud As Integer) As String
        '-----------------------------------------------------------
        'Función : Lista estado solicitudes de aprobación pendientes
        'por aprobación
        'Fecha : 2017-03-03
        'Ing Miguel Angel Urueta Miranda
        '-----------------------------------------------------------
        Try
            Dim sql_consulta As String = "Select ID_RESPUESTA_RADICADO from ra_respuesta_radicado where ID_REMIT_DEST_INT=" & id_usuario_gestion & _
                " and FECHA_REGISTRO_APROBACION is null and ID_SOLICITUDES_APROBACION_RESP is not  null and fecha_respueta is null and RADICADO='" & radicado & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_numero_solicitudes_pendientes_por_aprobacion = "Error Función Retorna_estado_solicitudes_pendientes_por_aprobacion dice  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                numero_solicitud = 0
                Retorna_numero_solicitudes_pendientes_por_aprobacion = "YES"
                Exit Function
            Else
                numero_solicitud = Datset.Tables(0).Rows.Count
                Retorna_numero_solicitudes_pendientes_por_aprobacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_numero_solicitudes_pendientes_por_aprobacion = "Inconsistencia general función Retorna_numero_solicitudes_pendientes_por_aprobacion " & ex.Message
        End Try
    End Function
    Function Retorna_numero_solicitudes_pendientes_por_aprobacion(ByVal id_usuario_gestion As Integer, _
                                                                  ByRef numero_solicitud As Integer) As String
        '-----------------------------------------------------------
        'Función : Lista estado solicitudes de aprobación pendientes
        'por aprobación
        'Fecha : 2017-03-03
        'Ing Miguel Angel Urueta Miranda
        '-----------------------------------------------------------
        Try
            Dim sql_consulta As String = "Select ID_RESPUESTA_RADICADO from ra_respuesta_radicado where ID_REMIT_DEST_INT=" & id_usuario_gestion & _
                " and FECHA_REGISTRO_APROBACION is null and ID_SOLICITUDES_APROBACION_RESP is not  null "
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_numero_solicitudes_pendientes_por_aprobacion = "Error Función Retorna_estado_solicitudes_pendientes_por_aprobacion dice  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                numero_solicitud = 0
                Retorna_numero_solicitudes_pendientes_por_aprobacion = "YES"
                Exit Function
            Else
                numero_solicitud = Datset.Tables(0).Rows.Count
                Retorna_numero_solicitudes_pendientes_por_aprobacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_numero_solicitudes_pendientes_por_aprobacion = "Inconsistencia general función Retorna_numero_solicitudes_pendientes_por_aprobacion " & ex.Message
        End Try
    End Function
    Function Retorna_estados_solicitudes_de_aprobacion_por_id_tarea_seleccionada(ByVal id_tarea_seleccionada As Integer, ByVal estado_inicial As Integer, ByRef estado_solicitud As String, ByVal id_usuario_gestion As Integer) As String
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassWorkflow
            Dim Radicado As String = ""
            Dim id_respuesta As Integer = 0
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(id_tarea_seleccionada, _
                                                                                 Radicado)
            If Result <> "YES" Then
                Retorna_estados_solicitudes_de_aprobacion_por_id_tarea_seleccionada = Result
                Exit Function
            End If
            If Radicado = "" Then
                Retorna_estados_solicitudes_de_aprobacion_por_id_tarea_seleccionada = "La tarea no tiene un radicado relacionado "
                Exit Function
            End If

            Dim Class_ra_respuesta_radicado As New Class_ra_respuesta_radicado
            Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado(Radicado,
                                                                               id_usuario_gestion,
                                                                               id_respuesta)
            If Result <> "YES" Then
                Retorna_estados_solicitudes_de_aprobacion_por_id_tarea_seleccionada = Result
                Exit Function
            End If
            If id_respuesta = 0 Then
                Retorna_estados_solicitudes_de_aprobacion_por_id_tarea_seleccionada = "Función Retorna_estados_solicitudes_de_aprobacion_por_id_tarea_seleccionada dice: imposible encontrar id respuesta radicado " & Radicado
                Exit Function
            End If
            Dim sql_consulta As String = "Select ESTADO_APROBACION from ra_cd_solicitudes_aprobacion where  Ra_Respuesta_Radicado_ID_RESPUESTA_RADICADO=" & id_respuesta & " and ESTADO_APROBACION=" & estado_inicial
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_solicitudes_aprobacion")
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_estados_solicitudes_de_aprobacion_por_id_tarea_seleccionada = "Error Función Retorna_estados_solicitudes_de_aprobacion_por_id_tarea_seleccionada dice  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_solicitud = "NO"
                Retorna_estados_solicitudes_de_aprobacion_por_id_tarea_seleccionada = "YES"
                Exit Function
            Else
                estado_solicitud = "YES"
                Retorna_estados_solicitudes_de_aprobacion_por_id_tarea_seleccionada = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_estados_solicitudes_de_aprobacion_por_id_tarea_seleccionada = "Inconsistencia general función Retorna_estados_solicitudes_de_aprobacion_por_id_tarea_seleccionada " & ex.Message
        End Try
    End Function

    Function Verifica_solicitudes_de_aprobacion_sin_desicion(ByVal id_tarea_seleccionada As Integer, _
                                                             ByRef estado_solicitud As String, _
                                                             ByVal id_usuario_gestion As Integer) As String
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassWorkflow
            Dim Radicado As String = ""
            Dim id_respuesta As Integer = 0
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(id_tarea_seleccionada, _
                                                                                 Radicado)
            If Result <> "YES" Then
                Verifica_solicitudes_de_aprobacion_sin_desicion = Result
                Exit Function
            End If
            If Radicado = "" Then
                estado_solicitud = "NO"
                Verifica_solicitudes_de_aprobacion_sin_desicion = "La tarea no tiene un radicado relacionado"
                Exit Function
            End If
            Dim Class_ra_respuesta_radicado As New Class_ra_respuesta_radicado
            Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado(Radicado,
                                                                              id_usuario_gestion,
                                                                              id_respuesta)
            If Result <> "YES" Then
                Verifica_solicitudes_de_aprobacion_sin_desicion = Result
                Exit Function
            End If
            If id_respuesta = 0 Then
                estado_solicitud = "NO"
                Verifica_solicitudes_de_aprobacion_sin_desicion = "YES"
                Exit Function
            End If
            Dim sql_consulta As String = "Select ID_SOLICITUDES_APROBACION from ra_cd_solicitudes_aprobacion where FECHA_REGISTRO_APROBACION is null and Ra_Respuesta_Radicado_ID_RESPUESTA_RADICADO=" & id_respuesta
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_solicitudes_aprobacion")
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Verifica_solicitudes_de_aprobacion_sin_desicion = "Error Función Verifica_solicitudes_de_aprobacion_sin_desicion  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_solicitud = "NO"
                Verifica_solicitudes_de_aprobacion_sin_desicion = "YES"
                Exit Function
            Else
                estado_solicitud = "YES"
                Verifica_solicitudes_de_aprobacion_sin_desicion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_solicitudes_de_aprobacion_sin_desicion = "Inconsistencia general función Verifica_solicitudes_de_aprobacion_sin_desicion " & ex.Message
        End Try
    End Function
    Function Verifica_solicitudes_de_aprobacion_sin_desicion_por_id_resuesta(ByVal id_respuesta As Integer, _
                                                                             ByRef estado_solicitud As String) As String
        Try
            Dim Result As String = ""
            Dim sql_consulta As String = "Select ID_SOLICITUDES_APROBACION from ra_cd_solicitudes_aprobacion " & _
                " where FECHA_REGISTRO_APROBACION is null and Ra_Respuesta_Radicado_ID_RESPUESTA_RADICADO=" & id_respuesta
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_solicitudes_aprobacion")
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Verifica_solicitudes_de_aprobacion_sin_desicion_por_id_resuesta = "Error Función Verifica_solicitudes_de_aprobacion_sin_desicion_por_id_resuesta  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_solicitud = "NO"
                Verifica_solicitudes_de_aprobacion_sin_desicion_por_id_resuesta = "YES"
                Exit Function
            Else
                estado_solicitud = "YES"
                Verifica_solicitudes_de_aprobacion_sin_desicion_por_id_resuesta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_solicitudes_de_aprobacion_sin_desicion_por_id_resuesta = "Inconsistencia general función Verifica_solicitudes_de_aprobacion_sin_desicion_por_id_resuesta " & ex.Message
        End Try
    End Function
    Function Retorna_nota_solicitud_de_aprobacion(ByVal id_nota As Integer, ByRef text_nota As String) As String
        '--------------------------------------------
        'Función : Retorna la nota de una solicitud 
        'de aprobación de un usuario especifico
        'Fecha : 2016-03-02
        'Ing. Miguel Angel Urueta Miranda
        '--------------------------------------------
        Try
            Dim sql_consulta As String = "select NOTA_SOLICITUD from ra_cd_notas_solicitudes_aprobacion_usuario where ID_NOTAS_SOLICITUDES_APROBACION_USUARIO=" & id_nota
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_notas_solicitudes_aprobacion_usuario")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_nota_solicitud_de_aprobacion = "Error Función Retorna_nota_solicitud_de_aprobacion " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                text_nota = ""
                Retorna_nota_solicitud_de_aprobacion = "No hay notas para esta solicitud "
                Exit Function
            Else
                text_nota = Datset.Tables(0).Rows(0).Item(0)
                Retorna_nota_solicitud_de_aprobacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nota_solicitud_de_aprobacion = "Inconsistencia general función Retorna_nota_solicitud_de_aprobacion " & ex.Message
        End Try
    End Function
    Function Lista_notas_solicitudes_especificas_de_aprobacion(ByVal id_solicitud_aprobacion_usuario As Integer, _
                                                               ByRef grediview As GridView, _
                                                               ByRef hideselecion As Object, _
                                                               ByRef update As UpdatePanel) As String
        '-------------------------------------------------------
        'Función lista las notas pertenecientes a una solicitud
        'de aprobación de una respuesta de un usuario espeficico
        'Fecha:2017-03-02
        'Ing : Miguel Angel Urueta Miranda
        '-------------------------------------------------------
        Try
            Dim sql_consulta As String = "Select rcn.ID_NOTAS_SOLICITUDES_APROBACION_USUARIO,rdi.Nombre_Remitente as NOMBRE, rdi.Cargo_Remite as CARGO,rcn.NOTA_SOLICITUD from ra_cd_usuarios_solicitudes_aprobacion as rcu " & _
                " inner join ra_cd_notas_solicitudes_aprobacion_usuario as rcn on (rcn.RA_CD_USUARIOS_SOLICITUDES_APROBACION_ID=rcu.ID_CD_USUARIOS_SOLICITUDES_APROBACION) " & _
                " inner join remit_dest_interno as rdi on (rdi.id_Remit_Dest_Int=rcu.Remit_Dest_Interno_id_remit_dest_Int)" & _
                " where ID_CD_USUARIOS_SOLICITUDES_APROBACION=" & id_solicitud_aprobacion_usuario
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_solicitudes_aprobacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_notas_solicitudes_especificas_de_aprobacion = "Error Función Lista_notas_solicitudes_generales_de_aprobacion " & Result
                Exit Function
            End If

            If Datset.Tables(0).Rows.Count = 0 Then
                grediview.DataSource = Nothing
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                Lista_notas_solicitudes_especificas_de_aprobacion = "YES"
                Exit Function
            Else
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-sticky-note fa-lg")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-primary btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Ver nota")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "vista_nota")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    grediview.Rows(i).Cells(0).Controls.Add(divhtml)
                    For z As Integer = 0 To grediview.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If
                    Next
                Next
               
                Lista_notas_solicitudes_especificas_de_aprobacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_notas_solicitudes_especificas_de_aprobacion = "Inconsistencia general función Lista_notas_solicitudes_especificas_de_aprobacion " & ex.Message
        End Try
    End Function
    Function Lista_notas_solicitudes_generales_de_aprobacion(ByVal id_solicitud_aprobacion_general As Integer, _
                                                             ByRef grediview As GridView, _
                                                             ByRef hideselecion As Object, _
                                                             ByRef update As UpdatePanel) As String
        '-------------------------------------------------------
        'Función lista las notas pertenecientes a una solicitud
        'de aprobación de una respuesta
        'Fecha:2017-03-02
        'Ing : Miguel Angel Urueta Miranda
        '-------------------------------------------------------
        Try
            Dim sql_consulta As String = "Select rcn.ID_NOTAS_SOLICITUDES_APROBACION_USUARIO,rdi.Nombre_Remitente as NOMBRE, rdi.Cargo_Remite as CARGO,rcn.NOTA_SOLICITUD from ra_cd_usuarios_solicitudes_aprobacion as rcu " & _
                " inner join ra_cd_notas_solicitudes_aprobacion_usuario as rcn on (rcn.RA_CD_USUARIOS_SOLICITUDES_APROBACION_ID=rcu.ID_CD_USUARIOS_SOLICITUDES_APROBACION) " & _
                " inner join remit_dest_interno as rdi on (rdi.id_Remit_Dest_Int=rcu.Remit_Dest_Interno_id_remit_dest_Int)" & _
                " where RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION=" & id_solicitud_aprobacion_general
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_solicitudes_aprobacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_notas_solicitudes_generales_de_aprobacion = "Error Función Lista_notas_solicitudes_generales_de_aprobacion " & Result
                Exit Function
            End If

            If Datset.Tables(0).Rows.Count = 0 Then
                grediview.DataSource = Nothing
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                Lista_notas_solicitudes_generales_de_aprobacion = "YES"
                Exit Function
            Else
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-sticky-note fa-lg")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-primary btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Ver nota")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "vista_nota")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    grediview.Rows(i).Cells(0).Controls.Add(divhtml)
                    For z As Integer = 0 To grediview.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If
                    Next
                Next
                Lista_notas_solicitudes_generales_de_aprobacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_notas_solicitudes_generales_de_aprobacion = "Inconsistencia general función Lista_notas_solicitudes_generales_de_aprobacion " & ex.Message
        End Try
    End Function
    Function Lista_solicitudes_generales_de_aprobacion_de_una_respuesta(ByVal id_respuesta As Integer, _
                                                                        ByRef grediview As GridView, _
                                                                        ByRef HiddenEmailconsulta As Object, _
                                                                        ByRef reflabel As Label, _
                                                                        ByRef hideselecion As Object, _
                                                                        ByRef update As UpdatePanel, _
                                                                        ByRef gred_view_datalle As GridView, _
                                                                        ByRef label_documento As Label, _
                                                                        ByRef up_date_detalle As UpdatePanel) As String
        Try
            'gred_view_datalle.DataSource = Nothing
            'gred_view_datalle.DataBind()
            'label_documento.Text = "Se encontraron 0 registro(s) "
            'up_date_detalle.Update()
            Dim sql_condicion As String = " where Ra_Respuesta_Radicado_ID_RESPUESTA_RADICADO=" & id_respuesta
            Dim sql_consulta As String = "SELECT ID_SOLICITUDES_APROBACION as NUMERO_SOLICITUD," & _
                    "FECHA_REGISTRO_SOLICITUD AS FECHA_SOLICITUD,FECHA_REGISTRO_APROBACION AS FECHA_APROBACION,DESCRIPCION_ESTADO_APROBACION AS ESTADO, NOTA_SOLICITUD as NOTA from ra_cd_solicitudes_aprobacion " & _
                     sql_condicion & " order by ID_SOLICITUDES_APROBACION desc"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_solicitudes_aprobacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_solicitudes_generales_de_aprobacion_de_una_respuesta = "Error listando datos " & Result
                Exit Function
            End If

            If Datset.Tables(0).Rows.Count = 0 Then
                'HiddenEmailconsulta.value = ""
                reflabel.Text = "Se encontraron 0 registro(s) "
                grediview.DataSource = Nothing
                'grediview.DataKeyNames = DataKey
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                Lista_solicitudes_generales_de_aprobacion_de_una_respuesta = "YES"
                Exit Function
            Else
                'HiddenEmailconsulta.value = sql_consulta
                reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) "
                'grediview.DataKeyNames = DataKey
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-archive fa-lg")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-warning btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Anular solicitud de aprobación")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "anular_sol")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-envelope fa-lg imag_crusor_da")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-primary btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Notificar al correo electrónico")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "noticor_sol")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-sticky-note fa-lg imag_crusor_da")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn   btn-info btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Ver notas de la solicitud")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "vernot_sol")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                   
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-folder-open fa-lg imag_crusor_da")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn   btn-primary btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Ver los anexos de la solicitud")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "veranex_sol")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-user-friends fa-lg imag_crusor_da")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn   btn-success btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Ver usuarios relacionados a la solicitud")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "ver_user_rel_sol")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    divhtml.Style.Add("display", "inline-flex")
                    grediview.Rows(i).Cells(0).Controls.Add(divhtml)
                    For z As Integer = 1 To grediview.Rows(i).Cells.Count - 1
                        'grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_corte_tr")
                        'grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this,'na');")
                        If z > 0 Then
                            grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If
                    Next
                Next
                Lista_solicitudes_generales_de_aprobacion_de_una_respuesta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_solicitudes_generales_de_aprobacion_de_una_respuesta = "Inconsistencia general función Lista_jerarquia_expedientes_unidades_simples_por_serie_area " & ex.Message
        End Try
    End Function
    Function Verfica_viabilidad_solicitud_aprobacion_respuesta(ByVal id_respuesta As Integer, _
                                                               ByRef fecha_limite As String) As String
        '-----------------------------------------------
        'Función : Verifica los requicitos mínimos para
        'la solicitud de aprobación de un documento
        'Fecha : 2017-02-15
        'Ingeniero : Miguel Angel Urueta Miranda
        '-----------------------------------------------
        Try
            Dim Result As String = ""
            Dim Refclas As New Classgestionrespuesta
            Dim ClassGestionFechas As New ClassGestionFechas
            Result = Refclas.Valida_usuario_propietario_respuesta(id_respuesta,
                                                                  HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If Result <> "YES" Then
                Verfica_viabilidad_solicitud_aprobacion_respuesta = Result
                Exit Function
            End If
            '-------------------------------------------------
            'Verifica si hay un documento de respuesta a 
            'modelo para solicitar la aprobación
            '-------------------------------------------------
            Dim estado_documento_respuesta As String = ""
            Result = Refclas.Verifica_existencia_documento_de_respuesta(id_respuesta,
                                                                        estado_documento_respuesta)
            If Result <> "YES" Then
                Verfica_viabilidad_solicitud_aprobacion_respuesta = Result
                Exit Function
            End If
            If estado_documento_respuesta = "NO" Then
                Verfica_viabilidad_solicitud_aprobacion_respuesta = "Debe adjuntar un documento previo con la posible respuesta al trámite"
                Exit Function
            End If
            '--------------------------------------------------
            'Verfica exitencia documento respuesta
            'permanente
            '--------------------------------------------------
            Dim estado_respuesta As String = ""
            Result = Refclas.Verfica_respuesta_con_fecha_respuesta(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                   id_respuesta,
                                                                   estado_respuesta)
            If Result <> "YES" Then
                Verfica_viabilidad_solicitud_aprobacion_respuesta = Result
                Exit Function
            End If
            If estado_respuesta = "YES" Then
                Verfica_viabilidad_solicitud_aprobacion_respuesta = "Existe una respuesta para este trámite, imposible generar una solicitud de aprobación"
                Exit Function
            End If
            '-------------------------------------------------
            'Solicita fecha limite de respuesta
            '-------------------------------------------------
            Dim stru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta,
                                                                               stru)
            If Result <> "YES" Then
                Verfica_viabilidad_solicitud_aprobacion_respuesta = Result
                Exit Function
            End If
            If stru.FECHA_VENCE <> "" Then
                fecha_limite = stru.FECHA_VENCE
                fecha_limite = ClassGestionFechas.formato_fecha_estructura(fecha_limite)
            End If
            Verfica_viabilidad_solicitud_aprobacion_respuesta = "YES"
        Catch ex As Exception
            Verfica_viabilidad_solicitud_aprobacion_respuesta = "Inconsistencia general función Verfica_viabilidad_solicitud_aprobacion_respuesta " & ex.Message
        End Try
    End Function

    Function verfica_existencia_respuesta_visto_solictud_aprobacion(ByVal id_solicitud_aprobacion As Integer, _
                                                                    ByRef estado_solicitud_aprobacion As Integer, _
                                                                    ByRef estado_visto As Integer) As String
        '------------------------------------------------------------
        'Función : Verifica que no tengan solicitudes vistas o con
        'respuestas.
        'Fecha : 2017-06-08
        'Ing : Miguel Angel Urueta Miranda
        '----------------------------------------------------------
        Try
            Dim strud() As STRU_SOLICITUD_APROBACION_USUARIOS = Nothing
            Dim Result As String = ""
            Result = Me.Retorna_datos_solictud_aprobacion_usuarios_por_id_solicitud(id_solicitud_aprobacion, strud)
            If Result <> "YES" Then
                verfica_existencia_respuesta_visto_solictud_aprobacion = Result
                Exit Function
            End If
            If Not strud Is Nothing Then
                For i As Integer = 0 To strud.Length - 1
                    If strud(i).ESTADO_VISTO_SOLICITANTE <> 0 Then
                        estado_visto = strud(i).ESTADO_VISTO_SOLICITANTE
                    End If
                    If strud(i).ESTADO_RESPUESTA_SOLICITUD <> 0 Then
                        estado_solicitud_aprobacion = strud(i).ESTADO_RESPUESTA_SOLICITUD
                    End If
                Next
                verfica_existencia_respuesta_visto_solictud_aprobacion = "YES"
                Exit Function
            End If
            verfica_existencia_respuesta_visto_solictud_aprobacion = "YES"
            Exit Function
        Catch ex As Exception
            verfica_existencia_respuesta_visto_solictud_aprobacion = "Inconsistencia función verfica_existencia_respuesta_visto_solictud_aprobacion " & ex.Message
        End Try
    End Function
    Function Actualiza_estado_anulado_general_aprobacion(ByVal id_solicitud_aprobcion As Integer, _
                                                         ByVal id_respuesta As Integer, _
                                                         ByVal nota_anulado As String, _
                                                         ByRef resultado_envio_correo As String) As String
        Dim Result As String = ""
        '------------------------------------------------
        'Retorna el estado general de la solicitud de
        'aprobación
        '------------------------------------------------
        Dim stru_ As STRU_SOLICITUD_ARPBACION = Nothing
        Result = Retorna_datos_solicitud_aprobación_documentos(id_solicitud_aprobcion, _
                                                               stru_)
        If Result <> "YES" Then
            Actualiza_estado_anulado_general_aprobacion = Result
            Exit Function
        End If
        '-----------------------------------------------
        'Retorna los datos de la respuesta del radicado
        '-----------------------------------------------
        Dim refclas_respuesta As New Classgestionrespuesta
        Dim stru_envi As stru_envio = Nothing
        Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
        Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta, _
                                                                                    stru_envi)
        If Result <> "YES" Then
            Actualiza_estado_anulado_general_aprobacion = Result
            Exit Function
        End If
        If stru_.ESTADO_APROBACION = 3 Then
            Actualiza_estado_anulado_general_aprobacion = "La solicitud de aprobación se encuentra en estado de archivado no se puede archivar la solicitud general"
            Exit Function
        End If
        'If stru_.ESTADO_APROBACION = 1 Then
        '    Actualiza_estado_anulado_general_aprobacion = "La solicitud de aprobación se encuentra en estado de aprobado no se puede archivar la solicitud general"
        '    Exit Function
        'End If
        'If stru_.ESTADO_APROBACION = 2 Then
        '    Actualiza_estado_anulado_general_aprobacion = "La solicitud de aprobación se encuentra en estado de desaprobado no se puede archivar la solicitud general"
        '    Exit Function
        'End If
        If stru_.ESTADO_APROBACION = 4 Then
            Actualiza_estado_anulado_general_aprobacion = "La solicitud de aprobación se encuentra en estado de anulado no se puede archivar la solicitud general"
            Exit Function
        End If
        '------------------------------------------------
        'Retorna los estados de todas las solicitudes
        'visto o solicitudes con alguana decisión
        '------------------------------------------------
        Dim estado_solictud_aprobacion_usuario As Integer = 0
        Dim estado_visto_solicitud_aprobacion_usuario As Integer = 0
        Result = verfica_existencia_respuesta_visto_solictud_aprobacion(id_solicitud_aprobcion, _
                                                                        estado_solictud_aprobacion_usuario, _
                                                                        estado_visto_solicitud_aprobacion_usuario)
        If Result <> "YES" Then
            Actualiza_estado_anulado_general_aprobacion = Result
            Exit Function
        End If
        If estado_solictud_aprobacion_usuario <> 0 Then
            'If estado_solictud_aprobacion_usuario = 1 Then
            '    Actualiza_estado_anulado_general_aprobacion = "Una de las solictudes de aprobación se encuentra en estado de aprobado no se puede archivar la solicitud general"
            '    Exit Function
            'End If
            'If estado_solictud_aprobacion_usuario = 2 Then
            '    Actualiza_estado_anulado_general_aprobacion = "Una de las solictudes de aprobación se encuentra en estado de desaprobado no se puede archivar la solicitud general"
            '    Exit Function
            'End If
            If estado_solictud_aprobacion_usuario = 3 Then

            End If

        End If
        '------------------------------------------------
        'Retorna correos electrónicos, nombre y id soli
        'situd de aprobación relacionadas a cada usuario
        '------------------------------------------------
        Dim correos_usuarios() As String = Nothing
        Dim id_solicitudes_aprobacion() As Integer = Nothing
        Dim nombre_usuario() As String = Nothing
        Result = Retorna_correos_usuarios_relacionados_solicitud_aprobacion(id_solicitud_aprobcion, _
                                                                            correos_usuarios, _
                                                                            id_solicitudes_aprobacion, _
                                                                            nombre_usuario)
        If Result <> "YES" Then
            Actualiza_estado_anulado_general_aprobacion = Result
            Exit Function
        End If
        Dim date1al As String = ""
        Dim ref_almacen As New ClassRadicador
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Result = refclas_gestion_fechas.Formatea_fecha_time_framework(Date.Now, date1al)
        If Result <> "YES" Then
            Actualiza_estado_anulado_general_aprobacion = Result
            Exit Function
        End If
        Dim stiempo As Object = Nothing
        Dim minuno As Object = Nothing
        Dim hora As Object = Nothing
        Dim dias_calendario As Object = Nothing
        Dim dias_no_habiles As Object = Nothing
        Result = refclas_gestion_fechas.Solicita_tiempo_respuesta_tramite(stru_.FECHA_REGISTRO_SOLICITUD, _
                                                                         stiempo, _
                                                                         hora, _
                                                                         minuno, _
                                                                         dias_calendario, _
                                                                         dias_no_habiles)
        If Result <> "YES" Then
            Actualiza_estado_anulado_general_aprobacion = Result
            Exit Function
        End If
        '-------------------------------------------------
        'Commando sql para la actualización en registro
        'general de solicitud de aprobación
        '-------------------------------------------------
        Dim sql_actualiza_registro_general As String = "update ra_cd_solicitudes_aprobacion set ESTADO_APROBACION=4,DESCRIPCION_ESTADO_APROBACION='Anulado', NOTA_SOLICITUD='" & nota_anulado & "'" & _
            ",FECHA_REGISTRO_APROBACION='" & date1al & "' " & _
            " where ID_SOLICITUDES_APROBACION=" & id_solicitud_aprobcion
        '------------------------------------------------------
        'Comando sql para actualizar los registros de usuarios
        'de solicitudes de aprobación
        '------------------------------------------------------
        Dim sql_actualizacion_registro_usuario As String = "Update ra_cd_usuarios_solicitudes_aprobacion set ESTADO_RESPUESTA_SOLICITUD=4,DESCRIPCION_ESTADO_RESPUESTA='Anulado'" & _
            ",FECHA_RESPUESTA_SOLICITUD='" & date1al & "' " & _
            " where RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION=" & id_solicitud_aprobcion
        '-----------------------------------------------------------------
        'Insertar los registros en el log de transacciones
        '-----------------------------------------------------------------
        Dim id_solicitudes_usuario As String = ""
        For i As Integer = 0 To id_solicitudes_aprobacion.Length - 1
            If i = 0 Then
                id_solicitudes_usuario = id_solicitudes_aprobacion(i)
            Else
                id_solicitudes_usuario = id_solicitudes_usuario & "-" & id_solicitudes_aprobacion(i)
            End If
        Next
        Dim nombre_usuario_relacionado As String = ""
        For i As Integer = 0 To nombre_usuario.Length - 1
            If i = 0 Then
                nombre_usuario_relacionado = nombre_usuario(i)
            Else
                nombre_usuario_relacionado = nombre_usuario_relacionado & "-" & nombre_usuario(i)
            End If
        Next
        Dim correos_usuarios_consolidado As String = ""
        For i As Integer = 0 To correos_usuarios.Length - 1
            If i = 0 Then
                correos_usuarios_consolidado = correos_usuarios(i)
            Else
                correos_usuarios_consolidado = correos_usuarios_consolidado & "," & correos_usuarios(i)
            End If
        Next
        Dim hor As String = Now
        Dim campos_trans As String = "Null"
        Dim isert_datos As String = ""
        campos_trans = UCase("Anulado") & " solicitud de aprobación de usuario numero (" & id_solicitud_aprobcion & _
      ")" & " Relacionada a la solicitudes de usuario " & id_solicitudes_usuario & vbCrLf & " Relacionados con los usuarios " & nombre_usuario_relacionado
        isert_datos = isert_datos & "('" & UCase("Anulado") & " SOLICITUD DE APROBACION" & "','" & HttpContext.Current.Session.Item("Login_Usuario_Workfow") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," & _
                     id_respuesta & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WORKFLOW WEB','" & campos_trans & "')"
        Dim sql_insert_log_solicitud = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" & _
                                             ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " & _
                                             isert_datos
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction = Nothing
        Dim sqlresultinsert As Integer = 0
        Try
            '-------------------------------------------------
            'Actualiza estado general solictud
            '-------------------------------------------------
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sql_actualiza_registro_general
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Actualiza_estado_anulado_general_aprobacion = "Imposible actualizar estado de la solictud de aprobación general"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '-----------------------------------------------------------------------
            'Actualiza estado solicitudes de aprobacion de usuarios relacionadas
            '-----------------------------------------------------------------------
            If sql_actualizacion_registro_usuario <> "" Then
                myCommand.CommandText = sql_actualizacion_registro_usuario
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Actualiza_estado_anulado_general_aprobacion = "Imposible actualizar la solicitud general de aprobación  "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If

            End If
            '-----------------------------------------------
            'Actualiza estado respuesta solicitud
            '-----------------------------------------------
            Dim update As String = "Update ra_respuesta_radicado set FECHA_REGISTRO_APROBACION='" & date1al & "'" & _
                 ",ESTADO_APROBACION=" & "4" & ",TIEMPO_RESPUESTA_APROBACION=" & stiempo & ",DESCRIPCION_ESTADO_APROBACION='" & "Anulado" & "'  where ID_RESPUESTA_RADICADO=" & id_respuesta
            myCommand.CommandText = update
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Actualiza_estado_anulado_general_aprobacion = "Imposible registar actualización respuesta  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '-----------------------------------------------
            'Registra log respuesta solicitud
            '-----------------------------------------------
            myCommand.CommandText = sql_insert_log_solicitud
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Actualiza_estado_anulado_general_aprobacion = "Imposible registar log de actualización solicitud de aprobación  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Dim Result_ As String = "YES"
            Dim id_tarea As Long = 0
            Dim Ref_class_wf_ruta As New Class_worflow_rutas
            Dim nombre_ruta As String = ""
            Result_ = Ref_class_wf_ruta.Retorna_nombre_ruta_workflow(nombre_ruta)
            If Result_ <> "YES" Then
                Actualiza_estado_anulado_general_aprobacion = Result_
                Exit Function
            End If
            Dim id_ruta As Integer = 0
            Result_ = Ref_class_wf_ruta.Retorna_id_ruta_workflow(nombre_ruta, _
                                                                id_ruta)
            If Result <> "YES" Then
                Actualiza_estado_anulado_general_aprobacion = Result_
                Exit Function
            End If
            Dim Refclas_confing_lista As New Class_configuracion_listado_ruta
            Dim nombre_campo_radicado As String = ""
            Result = Refclas_confing_lista.SolicitaNombreCampoRadicadoRuta(id_ruta,
                                                                           nombre_campo_radicado)
            If Result_ <> "YES" Then
                Actualiza_estado_anulado_general_aprobacion = Result_
                Exit Function
            End If

            Dim Refclas_dat As New Class_DAT_ADIC_TAR
            Result_ = Refclas_dat.Solicita_id_tarea_radicado(stru_envi.RADICADO, _
                                                            nombre_ruta, _
                                                            nombre_campo_radicado, _
                                                            id_tarea, _
                                                            0)
            If Result_ <> "YES" Then
                Actualiza_estado_anulado_general_aprobacion = Result_
                Exit Function
            End If

            Result_ = Refclas_dat.Actualiza_estado_tramite_tarea_workflow(nombre_ruta, _
                                                                          id_tarea, _
                                                                          "Solicitud anulada")
            If Not HttpContext.Current.Session.Item("dat_gred_cahce") Is Nothing Then
                For i As Integer = 0 To HttpContext.Current.Session.Item("dat_gred_cahce").Tables(0).Rows.Count - 1
                    If HttpContext.Current.Session.Item("dat_gred_cahce").Tables(0).Rows(i).Item(0) = id_tarea Then
                        HttpContext.Current.Session.Item("dat_gred_cahce").Tables(0).Rows(i).Item("ESTADO") = "Solicitud anulada"
                        HttpContext.Current.Session.Item("dat_gred_cahce").AcceptChanges()
                        Exit For
                    End If
                Next
            End If
            If HttpContext.Current.Session.Item("dat_gred_cahce_restore").GetType.ToString = "System.Data.DataSet" Then
                For i As Integer = 0 To HttpContext.Current.Session.Item("dat_gred_cahce_restore").Tables(0).Rows.Count - 1
                    If HttpContext.Current.Session.Item("dat_gred_cahce_restore").Tables(0).Rows(i).Item(0) = id_tarea Then
                        HttpContext.Current.Session.Item("dat_gred_cahce_restore").Tables(0).Rows(i).Item("ESTADO") = "Solicitud anulada"
                        HttpContext.Current.Session.Item("dat_gred_cahce_restore").AcceptChanges()
                        Exit For
                    End If
                Next
            End If
            Result = Me.Envio_correo_electronico_anula_solicitud_aprobacion(correos_usuarios_consolidado, _
                                                                            nota_anulado, _
                                                                            id_solicitud_aprobcion, _
                                                                            id_solicitudes_aprobacion, _
                                                                            nombre_usuario, _
                                                                            stru_envi.RADICADO, _
                                                                            stru_envi.DESTINATARIO)
            If Result_ <> "YES" Then
                Actualiza_estado_anulado_general_aprobacion = "Se actualizo el estado de la solcitud, pero no se pudo actualizar el estado del tramite en workflow " & Result_
                Exit Function
            End If
            resultado_envio_correo = Result
            Actualiza_estado_anulado_general_aprobacion = "YES"
        Catch ex As Exception
            myTrans.Rollback()
            If Not myTrans.Connection Is Nothing Then
                Actualiza_estado_anulado_general_aprobacion = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
            If myTrans.Connection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Actualiza_estado_anulado_general_aprobacion = "Error General " & ex.Message
            Exit Function
        End Try
    End Function
    Function Envio_correo_electronico_anula_solicitud_aprobacion(ByVal correos_relacionados As String, _
                                                                 ByVal nota_solicitud As String, _
                                                                 ByVal numero_solictud As Integer, _
                                                                 ByRef solictudes_usuario_relacionadas() As Integer, _
                                                                 ByRef usuarios_documentos_compartidos() As String, _
                                                                 ByRef radicado_respuesta As String, _
                                                                 ByVal mombre_remitente As String) As String
        '----------------------------------------------------------
        'Función : Envía correo electrónico usuario de solicitud
        'Fecha : 2016-02-20
        'Ing Miguel Angel Urueta Miranda
        '----------------------------------------------------------
        Try
            Dim refclas_gestion As New Classgestionrespuesta
            Dim Result As String = ""
            Dim nombre_area As String = ""
            Dim id_area As Integer = 0
            Dim nombre_usuario As String = ""
            Dim cargo_usuario As String = ""
            Result = refclas_gestion.Retorna_datos_asignacion_respuesta_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                        id_area, _
                                                                                        nombre_area, _
                                                                                        nombre_usuario, _
                                                                                        cargo_usuario)
            If Result <> "YES" Then
                Envio_correo_electronico_anula_solicitud_aprobacion = Result
                Exit Function
            End If
            Dim HTTEXT = "<p> Detalle usuarios relacionados </p>" & _
                    "<table>" & _
                       "<tr>" & _
                          "<td> CODIGO DOCUMENTO COMPARTIDO </td>" & _
                          "<td> NOMBRE USUARIO RELACIONADO </td>" & _
                       "</tr>"
            For i As Integer = 0 To usuarios_documentos_compartidos.Length - 1
                HTTEXT = HTTEXT & "<tr>" & _
                            "<td> " & solictudes_usuario_relacionadas(i) & " </td> " & _
                            "<td> " & usuarios_documentos_compartidos(i) & " </td> " & _
                       "</tr>"
            Next
            HTTEXT = HTTEXT & "</table>"
            Dim spli_texto_correo() As String = {"Anulación solicitud de aprobación de respuesta numero : " & numero_solictud & " radicado : " & radicado_respuesta, _
            "El usuario  " & nombre_usuario & " anuló la solicitud general de aprobacion numero " & numero_solictud, "Para dar respuesta al radicado " & radicado_respuesta, "Del remitente : " & mombre_remitente, _
            "EL sistema saco de la lista de pendiendientes por aprobacion los siguientes usuarios relacionados a continuación ", HTTEXT, _
            "Nombre de quien comparte : " & nombre_usuario, "Cargo de quien comparte : " & cargo_usuario, "Area de quien comparte : " & nombre_area}
            Dim Refclas As New ClassCorreo
            Result = Refclas.Envio_Correo_documento_compartido(spli_texto_correo, _
                                                               correos_relacionados, _
                                                               "Anulación solicitud de aprobación de respuesta numero : " & numero_solictud & " Radicado : " & radicado_respuesta)
            If Result <> "YES" Then
                Envio_correo_electronico_anula_solicitud_aprobacion = Result
                Exit Function
            Else
                Envio_correo_electronico_anula_solicitud_aprobacion = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Envio_correo_electronico_anula_solicitud_aprobacion = "Inconsistencia general función Envio_correo_electronico_anula_solicitud_aprobacion " & ex.Message
        End Try
    End Function
    Function Actualiza_estado_archivado_solicitud_aprobacion_usuario(ByVal id_solicitud_usuario As Integer, _
                                                                     ByVal tipo_aprobacion As Integer, _
                                                                     ByVal descripcion_tipo_aprobacion As String, _
                                                                     ByRef hiden_cambio_solicitud_general As String, _
                                                                     ByRef hiden_cambio_solicitud_usuario As String, _
                                                                     ByVal id_respuesta As Integer, _
                                                                     ByVal id_usuario_gestion As Integer, _
                                                                     ByVal correos_electronicos_relacionados As String, _
                                                                     ByRef resultado_envio_correo As String, _
                                                                     ByVal archiva_usuarios_solicitante As Integer) As String
        hiden_cambio_solicitud_general = ""
        hiden_cambio_solicitud_usuario = ""
        Dim Result As String = ""
        Dim stru As STRU_SOLICITUD_APROBACION_USUARIOS = Nothing
        Result = Me.Retorna_datos_solictud_aprobacion_usuarios(id_solicitud_usuario, stru)
        If Result <> "YES" Then
            Actualiza_estado_archivado_solicitud_aprobacion_usuario = Result
            Exit Function
        End If
        If stru.ESTADO_RESPUESTA_SOLICITUD = 3 Then
            Actualiza_estado_archivado_solicitud_aprobacion_usuario = "El sistema detecto que la solicitud se encuentra archivada"
            Exit Function
        End If
        If stru.ESTADO_VISTO_SOLICITANTE = 1 Then
            Actualiza_estado_archivado_solicitud_aprobacion_usuario = "El sistema detecto que la solicitud esta en estudio por el solicitado imposible archivar"
            Exit Function
        End If
        Dim stru_principal As STRU_SOLICITUD_ARPBACION = Nothing
        Result = Me.Retorna_datos_solicitud_aprobación_documentos(stru.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION, stru_principal)
        If Result <> "YES" Then
            Actualiza_estado_archivado_solicitud_aprobacion_usuario = Result
            Exit Function
        End If
        Dim correo_usuario_solicitante As String = ""
        Dim class_gestion_respuesta As New Classgestionrespuesta
        Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
        Result = Reclas_remit_dest_interno.Solicita_correo_usuario_gestion(stru_principal.Remit_Dest_Interno_id_remit_dest_Int, _
                                                                          correo_usuario_solicitante)
        If Result <> "YES" Then
            Actualiza_estado_archivado_solicitud_aprobacion_usuario = Result
            Exit Function
        End If
        Dim date1al As String = Date.Now
        Dim ref_almacen As New ClassRadicador
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(date1al)
        If Result <> "YES" Then
            Actualiza_estado_archivado_solicitud_aprobacion_usuario = Result
            Exit Function
        End If
        Dim stiempo As Object = Nothing
        Dim minuno As Object = Nothing
        Dim hora As Object = Nothing
        Dim dias_calendario As Object = Nothing
        Dim dias_no_habiles As Object = Nothing
        Result = refclas_gestion_fechas.Solicita_tiempo_respuesta_tramite(stru.FECHA_REGISTRO_SOLICITUD, _
                                                                         stiempo, _
                                                                         hora, _
                                                                         minuno, _
                                                                         dias_calendario, _
                                                                         dias_no_habiles)
        If Result <> "YES" Then
            Actualiza_estado_archivado_solicitud_aprobacion_usuario = Result
            Exit Function
        End If
        '-------------------------------------------
        'Retorna estado de solicitudes de aprobación
        '-1 Registra la solicitud
        '-2 No hay desición por que no se completo el
        'numero de aprobaciones
        '
        '-------------------------------------------
        Dim estado_solicitud As Integer = -3
        Dim estado_ref_solicitud As Integer = -3
        Dim decripcion_estado_solicitud As String = ""
        Result = Me.Retorna_estado_solicitud_aprobacion_final(stru.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION, _
                                                              estado_solicitud, _
                                                              decripcion_estado_solicitud)
        If Result <> "YES" Then
            Actualiza_estado_archivado_solicitud_aprobacion_usuario = Result
            Exit Function
        End If
        Dim sql_update_actualizacion_general As String = ""
        If estado_solicitud = -1 Then
            hiden_cambio_solicitud_general = descripcion_tipo_aprobacion
            decripcion_estado_solicitud = descripcion_tipo_aprobacion
            sql_update_actualizacion_general = "Update ra_cd_solicitudes_aprobacion set ESTADO_APROBACION=" & tipo_aprobacion & ",DESCRIPCION_ESTADO_APROBACION='" & _
                descripcion_tipo_aprobacion & "',FECHA_REGISTRO_APROBACION='" & date1al & "',TIEMPO_RESPUESTA_APROBACION=" & stiempo & " where ID_SOLICITUDES_APROBACION=" & stru.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION
            estado_ref_solicitud = tipo_aprobacion
        End If
        If estado_solicitud <> -1 And estado_solicitud <> -2 Then
            hiden_cambio_solicitud_general = decripcion_estado_solicitud
            sql_update_actualizacion_general = "Update ra_cd_solicitudes_aprobacion set ESTADO_APROBACION=" & estado_solicitud & ",DESCRIPCION_ESTADO_APROBACION='" & _
               decripcion_estado_solicitud & "',FECHA_REGISTRO_APROBACION='" & date1al & "',TIEMPO_RESPUESTA_APROBACION=" & stiempo & " where ID_SOLICITUDES_APROBACION=" & stru.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION
            estado_ref_solicitud = estado_solicitud
        End If
        Dim sql_update As String = "Update ra_cd_usuarios_solicitudes_aprobacion set ESTADO_RESPUESTA_SOLICITUD=" & tipo_aprobacion & ",DESCRIPCION_ESTADO_RESPUESTA='" & descripcion_tipo_aprobacion & "'" & _
            ",FECHA_RESPUESTA_SOLICITUD='" & date1al & "',TIEMPO_RESPUESTA_SOLICITUD=" & stiempo & " where ID_CD_USUARIOS_SOLICITUDES_APROBACION=" & id_solicitud_usuario
        hiden_cambio_solicitud_usuario = descripcion_tipo_aprobacion
        '-----------------------------------------------------------------
        'Insertar los registros en el log de transacciones
        '-----------------------------------------------------------------
        Dim hor As String = Now
        Dim campos_trans As String = "Null"
        Dim isert_datos As String = ""
        campos_trans = UCase(descripcion_tipo_aprobacion) & " solicitud de aprobación de usuario numero (" & id_solicitud_usuario & _
      ")" & " Relacionada a la solicitud principal : " & stru.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION
        isert_datos = isert_datos & "('" & UCase(tipo_aprobacion) & " SOLICITUD DE APROBACION" & "','" & HttpContext.Current.Session.Item("Login_Usuario_Workfow") & "','" & id_usuario_gestion & "','" & date1al & "'," & _
                     id_respuesta & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WORKFLOW WEB','" & campos_trans & "')"
        Dim sql_insert_log_solicitud = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" & _
                                             ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " & _
                                             isert_datos

        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction = Nothing
        Dim sqlresultinsert As Integer = 0
        Try
            '-------------------------------------------------
            'Registra solicitud de aprobación
            '-------------------------------------------------
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sql_update
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Actualiza_estado_archivado_solicitud_aprobacion_usuario = "Imposible actualizar la solicitud de aprobación del usuario  "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '------------------------------------------------
            'Actualiza solicitud general aprobacion
            '------------------------------------------------
            If sql_update_actualizacion_general <> "" Then
                myCommand.CommandText = sql_update_actualizacion_general
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Actualiza_estado_archivado_solicitud_aprobacion_usuario = "Imposible actualizar la solicitud general de aprobación  "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                '-----------------------------------------------
                'Actualiza estado respuesta solicitud
                '-----------------------------------------------
                Dim ref_estado_solicitud As Object = 0
                If estado_solicitud <> -1 And estado_solicitud <> -2 Then
                    ref_estado_solicitud = estado_solicitud
                Else
                    ref_estado_solicitud = tipo_aprobacion
                End If

                Dim update As String = "Update ra_respuesta_radicado set FECHA_REGISTRO_APROBACION='" & date1al & "'" & _
                     ",ESTADO_APROBACION=" & ref_estado_solicitud & ",TIEMPO_RESPUESTA_APROBACION=" & stiempo & ",DESCRIPCION_ESTADO_APROBACION='" & decripcion_estado_solicitud & "'  where ID_RESPUESTA_RADICADO=" & id_respuesta
                myCommand.CommandText = update
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Actualiza_estado_archivado_solicitud_aprobacion_usuario = "Imposible registar actualización respuesta  "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If

            '-----------------------------------------------
            'Registra log respuesta solicitud
            '-----------------------------------------------
            myCommand.CommandText = sql_insert_log_solicitud
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Actualiza_estado_archivado_solicitud_aprobacion_usuario = "Imposible registar log de solicitud de aprobación  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Dim Result_ As String = "YES"
            If sql_update_actualizacion_general <> "" Then
                Dim stru_envi As stru_envio = Nothing
                Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
                Result_ = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta, _
                                                                                            stru_envi)
                If Result_ <> "YES" Then
                    Actualiza_estado_archivado_solicitud_aprobacion_usuario = Result_
                    Exit Function
                End If
                Dim id_tarea As Long = 0
                Dim Ref_class_wf_ruta As New Class_worflow_rutas
                Dim nombre_ruta As String = ""
                Result_ = Ref_class_wf_ruta.Retorna_nombre_ruta_workflow(nombre_ruta)
                If Result_ <> "YES" Then
                    Actualiza_estado_archivado_solicitud_aprobacion_usuario = Result_
                    Exit Function
                End If
                Dim id_ruta As Integer = 0
                Result_ = Ref_class_wf_ruta.Retorna_id_ruta_workflow(nombre_ruta, _
                                                                    id_ruta)
                If Result <> "YES" Then
                    Actualiza_estado_archivado_solicitud_aprobacion_usuario = Result_
                    Exit Function
                End If
                Dim Refclas_confing_lista As New Class_configuracion_listado_ruta
                Dim nombre_campo_radicado As String = ""
                Result = Refclas_confing_lista.SolicitaNombreCampoRadicadoRuta(id_ruta,
                                                                               nombre_campo_radicado)
                If Result_ <> "YES" Then
                    Actualiza_estado_archivado_solicitud_aprobacion_usuario = Result_
                    Exit Function
                End If

                Dim Refclas_dat As New Class_DAT_ADIC_TAR
                Result_ = Refclas_dat.Solicita_id_tarea_radicado(stru_envi.RADICADO, _
                                                                nombre_ruta, _
                                                                nombre_campo_radicado, _
                                                                id_tarea, _
                                                                0)
                If Result_ <> "YES" Then
                    Actualiza_estado_archivado_solicitud_aprobacion_usuario = Result_
                    Exit Function
                End If
                Dim Descrip_solicitud As String = ""
                If estado_ref_solicitud = 1 Then
                    Descrip_solicitud = "Solicitud aprobada"
                End If
                If estado_ref_solicitud = 2 Then
                    Descrip_solicitud = "Solicitud desaprobada"
                End If
                If estado_ref_solicitud = 3 Then
                    Descrip_solicitud = "Solicitud archivada"
                End If
                If estado_ref_solicitud = 4 Then
                    Descrip_solicitud = "Solicitud anulada"
                End If
                Result_ = Refclas_dat.Actualiza_estado_tramite_tarea_workflow(nombre_ruta, _
                                                                              id_tarea, _
                                                                              Descrip_solicitud)
                If archiva_usuarios_solicitante = 1 Then
                    Result = Me.Envio_correo_electronico_respuesta_solicitud_aprobacion(correos_electronicos_relacionados, "", "", stru.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION, descripcion_tipo_aprobacion, stru.Remit_Dest_Interno_id_remit_dest_Int)
                End If
                If sql_update_actualizacion_general <> "" Then
                    Result = Me.Envio_correo_electronico_respuesta_solicitud_aprobacion(correo_usuario_solicitante, "", "", stru.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION, decripcion_estado_solicitud, stru_principal.Remit_Dest_Interno_id_remit_dest_Int)
                End If
            End If

            resultado_envio_correo = Result
            If Result_ <> "YES" Then
                Actualiza_estado_archivado_solicitud_aprobacion_usuario = "Se actualizo el estado de la solictud, pero no se pudo actualizar el estado del tramite en workflow " & Result_
                Exit Function
            End If
            Actualiza_estado_archivado_solicitud_aprobacion_usuario = "YES"
        Catch ex As Exception
            myTrans.Rollback()
            If Not myTrans.Connection Is Nothing Then
                Actualiza_estado_archivado_solicitud_aprobacion_usuario = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
            If myTrans.Connection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Actualiza_estado_archivado_solicitud_aprobacion_usuario = "Error General " & ex.Message
            Exit Function
        End Try
    End Function
    Function Actualiza_estado_solicitud_aprobacion_usuario(ByVal id_solicitud_usuario As Integer, _
                                                           ByVal tipo_aprobacion As Integer, _
                                                           ByVal descripcion_tipo_aprobacion As String, _
                                                           ByRef hiden_cambio_solicitud_general As String, _
                                                           ByRef hiden_cambio_solicitud_usuario As String, _
                                                           ByVal id_respuesta As Integer, _
                                                           ByVal id_usuario_gestion As Integer, _
                                                           ByVal correos_electronicos_relacionados As String, _
                                                           ByRef resultado_envio_correo As String, _
                                                           ByVal archiva_usuarios_solicitante As Integer, _
                                                           ByVal nota_aprobacion As String, _
                                                           ByVal estado_autoriza_firma As Integer) As String
        hiden_cambio_solicitud_general = ""
        hiden_cambio_solicitud_usuario = ""
        Dim Result As String = ""
        Dim stru As STRU_SOLICITUD_APROBACION_USUARIOS = Nothing
        Result = Me.Retorna_datos_solictud_aprobacion_usuarios(id_solicitud_usuario, _
                                                               stru)
        If Result <> "YES" Then
            Actualiza_estado_solicitud_aprobacion_usuario = Result
            Exit Function
        End If

        'If stru.ESTADO_RESPUESTA_SOLICITUD = 3 Then
        '    Actualiza_estado_solicitud_aprobacion_usuario = "El sistema detecto que la solicitud se encuentra archivada"
        '    Exit Function
        'End If
        'If stru.ESTADO_VISTO_SOLICITANTE = 1 Then
        '    Actualiza_estado_solicitud_aprobacion_usuario = "El sistema detecto que la solicitud esta en estudio por el solicitado imposible archivar" decisión
        '    Exit Function
        'End If
        If tipo_aprobacion <> 1 Then
            If nota_aprobacion = "" Then
                Actualiza_estado_solicitud_aprobacion_usuario = "Si su decisión es de " & descripcion_tipo_aprobacion & ", por favor digite la nota "
                Exit Function
            End If

        End If
        '-----------------------------------------------------
        'Verifica no este con desición la solicitud principal
        '-----------------------------------------------------
        Dim estado_solicitud_apro As Integer = 0
        Dim descripcion_estado As String = ""
        Result = Me.Retorna_estados_solicitudes_de_aprobacion_general_por_id_solicitud(stru.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION, _
                                                                                       estado_solicitud_apro, _
                                                                                       descripcion_estado)
        If Result <> "YES" Then
            Actualiza_estado_solicitud_aprobacion_usuario = Result
            Exit Function
        End If
        If estado_solicitud_apro <> 0 Then
            Actualiza_estado_solicitud_aprobacion_usuario = "La solicitud se encuentra en estado " & descripcion_estado & ", imposible actualizar el estado de la solicitud"
            Exit Function
        End If
        Dim stru_principal As STRU_SOLICITUD_ARPBACION = Nothing
        Result = Me.Retorna_datos_solicitud_aprobación_documentos(stru.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION, _
                                                                  stru_principal)
        If Result <> "YES" Then
            Actualiza_estado_solicitud_aprobacion_usuario = Result
            Exit Function
        End If
        Dim correo_usuario_solicitante As String = ""
        Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
        Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
        Result = Reclas_remit_dest_interno.Solicita_correo_usuario_gestion(stru_principal.Remit_Dest_Interno_id_remit_dest_Int, _
                                                                          correo_usuario_solicitante)
        If Result <> "YES" Then
            Actualiza_estado_solicitud_aprobacion_usuario = Result
            Exit Function
        End If
        Dim date1al As String = ""
        Dim ref_almacen As New ClassRadicador
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Result = refclas_gestion_fechas.Formatea_fecha_time_framework(Date.Now, date1al)
        If Result <> "YES" Then
            Actualiza_estado_solicitud_aprobacion_usuario = Result
            Exit Function
        End If
        Dim stiempo As Object = Nothing
        Dim minuno As Object = Nothing
        Dim hora As Object = Nothing
        Dim dias_calendario As Object = Nothing
        Dim dias_no_habiles As Object = Nothing
        Result = refclas_gestion_fechas.Solicita_tiempo_respuesta_tramite(stru.FECHA_REGISTRO_SOLICITUD, _
                                                                          stiempo, _
                                                                          hora, _
                                                                          minuno, _
                                                                          dias_calendario, _
                                                                          dias_no_habiles)
        If Result <> "YES" Then
            Actualiza_estado_solicitud_aprobacion_usuario = Result
            Exit Function
        End If

        '-------------------------------------------
        'Retorna estado de solicitudes de aprobación
        '-1 Registra la solicitud
        '-2 No hay desición por que no se completo el
        'numero de aprobaciones
        '
        '-------------------------------------------
        Dim estado_solicitud As Integer = -3
        Dim estado_ref_solicitud As Integer = -3
        Dim decripcion_estado_solicitud As String = ""
        Result = Me.Retorna_estado_solicitud_aprobacion_final(stru.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION, _
                                                              estado_solicitud, _
                                                              decripcion_estado_solicitud)
        If Result <> "YES" Then
            Actualiza_estado_solicitud_aprobacion_usuario = Result
            Exit Function
        End If

        Dim sql_update_actualizacion_general As String = ""
        If estado_solicitud = -1 Then
            hiden_cambio_solicitud_general = descripcion_tipo_aprobacion
            decripcion_estado_solicitud = descripcion_tipo_aprobacion
            sql_update_actualizacion_general = "Update ra_cd_solicitudes_aprobacion set ESTADO_APROBACION=" & tipo_aprobacion & ",DESCRIPCION_ESTADO_APROBACION='" & _
                descripcion_tipo_aprobacion & "',FECHA_REGISTRO_APROBACION='" & date1al & "',TIEMPO_RESPUESTA_APROBACION=" & stiempo & " where ID_SOLICITUDES_APROBACION=" & stru.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION
            estado_ref_solicitud = tipo_aprobacion
        End If
        If estado_solicitud <> -1 And estado_solicitud <> -2 Then
            hiden_cambio_solicitud_general = decripcion_estado_solicitud
            sql_update_actualizacion_general = "Update ra_cd_solicitudes_aprobacion set ESTADO_APROBACION=" & estado_solicitud & ",DESCRIPCION_ESTADO_APROBACION='" & _
               decripcion_estado_solicitud & "',FECHA_REGISTRO_APROBACION='" & date1al & "',TIEMPO_RESPUESTA_APROBACION=" & stiempo & " where ID_SOLICITUDES_APROBACION=" & stru.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION
            estado_ref_solicitud = estado_solicitud
        End If
        Dim sql_update As String = "Update ra_cd_usuarios_solicitudes_aprobacion set ESTADO_RESPUESTA_SOLICITUD=" & tipo_aprobacion & ",DESCRIPCION_ESTADO_RESPUESTA='" & descripcion_tipo_aprobacion & "'" & _
            ",FECHA_RESPUESTA_SOLICITUD='" & date1al & "',TIEMPO_RESPUESTA_SOLICITUD=" & stiempo & _
            ",ESTADO_AUTORIZACION_FIRMA=" & estado_autoriza_firma & _
            " where ID_CD_USUARIOS_SOLICITUDES_APROBACION=" & id_solicitud_usuario
        hiden_cambio_solicitud_usuario = descripcion_tipo_aprobacion
        '-----------------------------------------------------------------
        'Retorna datos de respuesta si la solictud general se cumple
        '-----------------------------------------------------------------
        Dim stru_respuesta As stru_envio = Nothing
        Dim refclass_gestion_respuesta As New Classgestionrespuesta
        If sql_update_actualizacion_general <> "" Then
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta, _
                                                                                        stru_respuesta)


        End If
        '-----------------------------------------------------------------
        'Insertar los registros en el log de transacciones
        '-----------------------------------------------------------------
        Dim hor As String = Now
        Dim campos_trans As String = "Null"
        Dim isert_datos As String = ""
        campos_trans = UCase(descripcion_tipo_aprobacion) & " solicitud de aprobación de usuario numero (" & id_solicitud_usuario & _
      ")" & " Relacionada a la solicitud principal : " & stru.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION
        isert_datos = isert_datos & "('" & UCase(tipo_aprobacion) & " SOLICITUD DE APROBACION" & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & id_usuario_gestion & "','" & date1al & "'," & _
                     id_respuesta & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','GESTOR DOCUMENTAL','" & campos_trans & "')"
        Dim sql_insert_log_solicitud = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" & _
                                             ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " & _
                                             isert_datos
        Dim sql_nota_aprobacion As String = ""
        If nota_aprobacion <> "" Then
            sql_nota_aprobacion = "Insert into ra_cd_notas_solicitudes_aprobacion_usuario (RA_CD_USUARIOS_SOLICITUDES_APROBACION_ID,NOTA_SOLICITUD,FECHA_NOTA_SOLICITUD) values (" & _
            id_solicitud_usuario & ",'" & nota_aprobacion & "','" & date1al & "')"
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction = Nothing
        Dim sqlresultinsert As Integer = 0
        Try
            '-------------------------------------------------
            'Registra solicitud de aprobación
            '-------------------------------------------------
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sql_update
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Actualiza_estado_solicitud_aprobacion_usuario = "Imposible actualizar la solicitud de aprobación del usuario  "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '------------------------------------------------
            'Actualiza solicitud general aprobacion
            '------------------------------------------------
            If sql_update_actualizacion_general <> "" Then
                myCommand.CommandText = sql_update_actualizacion_general
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Actualiza_estado_solicitud_aprobacion_usuario = "Imposible actualizar la solicitud general de aprobación  "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                '-----------------------------------------------
                'Actualiza estado respuesta solicitud
                '-----------------------------------------------
                Dim ref_estado_solicitud As Integer = 0
                If estado_solicitud <> -1 And estado_solicitud <> -2 Then
                    ref_estado_solicitud = estado_solicitud
                Else
                    ref_estado_solicitud = tipo_aprobacion
                End If
                Dim update As String = "Update ra_respuesta_radicado set FECHA_REGISTRO_APROBACION='" & date1al & "'" & _
                     ",ESTADO_APROBACION=" & ref_estado_solicitud & ",TIEMPO_RESPUESTA_APROBACION=" & stiempo & ",DESCRIPCION_ESTADO_APROBACION='" & decripcion_estado_solicitud & "'  where ID_RESPUESTA_RADICADO=" & id_respuesta
                myCommand.CommandText = update
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Actualiza_estado_solicitud_aprobacion_usuario = "Imposible registar actualización respuesta  "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If

            End If

            '-----------------------------------------------
            'Registra log respuesta solicitud
            '-----------------------------------------------
            myCommand.CommandText = sql_insert_log_solicitud
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Actualiza_estado_solicitud_aprobacion_usuario = "Imposible registar log de solicitud de aprobación  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '------------------------------------------------
            'Registra nota solicitud usuario
            '------------------------------------------------
            If sql_nota_aprobacion <> "" Then
                myCommand.CommandText = sql_nota_aprobacion
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Actualiza_estado_solicitud_aprobacion_usuario = "Imposible registar nota de solicitud de aprobación  "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            If HttpContext.Current.Session.Item("GA_NUMERO_SOLICITUDES_PENDIENTES_APROBAR_USUARIO") <> 0 Then
                HttpContext.Current.Session.Item("GA_NUMERO_SOLICITUDES_PENDIENTES_APROBAR_USUARIO") = Val(HttpContext.Current.Session.Item("GA_NUMERO_SOLICITUDES_PENDIENTES_APROBAR_USUARIO")) - 1
            End If
            myTrans.Commit()
            myConnection.Close()
            Dim Result_ As String = "YES"
            If sql_update_actualizacion_general <> "" Then
                Dim id_tarea As Long = 0
                Dim Ref_class_wf_ruta As New Class_worflow_rutas
                Dim nombre_ruta As String = ""
                Result = Ref_class_wf_ruta.Retorna_nombre_ruta_workflow(nombre_ruta)
                If Result <> "YES" Then
                    Actualiza_estado_solicitud_aprobacion_usuario = Result
                    Exit Function
                End If
                Dim id_ruta As Integer = 0
                Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(nombre_ruta, _
                                                                    id_ruta)
                If Result <> "YES" Then
                    Actualiza_estado_solicitud_aprobacion_usuario = Result
                    Exit Function
                End If
                Dim Refclas_confing_lista As New Class_configuracion_listado_ruta
                Dim nombre_campo_radicado As String = ""
                Result = Refclas_confing_lista.SolicitaNombreCampoRadicadoRuta(id_ruta,
                                                                               nombre_campo_radicado)
                If Result <> "YES" Then
                    Actualiza_estado_solicitud_aprobacion_usuario = Result
                    Exit Function
                End If

                Dim Refclas_dat As New Class_DAT_ADIC_TAR
                Result = Refclas_dat.Solicita_id_tarea_radicado(stru_respuesta.RADICADO, _
                                                                nombre_ruta, _
                                                                nombre_campo_radicado, _
                                                                id_tarea, _
                                                                0)
                If Result <> "YES" Then
                    Actualiza_estado_solicitud_aprobacion_usuario = Result
                    Exit Function
                End If
                Dim Descrip_solicitud As String = ""
                If estado_ref_solicitud = 1 Then
                    Descrip_solicitud = "Solicitud aprobada"
                End If
                If estado_ref_solicitud = 2 Then
                    Descrip_solicitud = "Solicitud desaprobada"
                End If
                If estado_ref_solicitud = 3 Then
                    Descrip_solicitud = "Solicitud archivada"
                End If
                If estado_ref_solicitud = 4 Then
                    Descrip_solicitud = "Solicitud anulada"
                End If
                Result_ = Refclas_dat.Actualiza_estado_tramite_tarea_workflow(nombre_ruta, _
                                                                              id_tarea, _
                                                                              Descrip_solicitud)

            End If
            If archiva_usuarios_solicitante = 1 Then
                Result = Me.Envio_correo_electronico_respuesta_solicitud_aprobacion(correos_electronicos_relacionados, "", "", id_solicitud_usuario, descripcion_tipo_aprobacion, stru.Remit_Dest_Interno_id_remit_dest_Int)
            End If
            If sql_update_actualizacion_general <> "" Then
                Dim nota As String = decripcion_estado_solicitud & " respuesta radicado " & stru_respuesta.RADICADO & " Solicitante " & stru_respuesta.DESTINATARIO & ".  El responsable agrego la siguiente anotación :  " & nota_aprobacion
                Result = Me.Envio_correo_electronico_respuesta_solicitud_aprobacion(correo_usuario_solicitante, nota, "", stru.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION, decripcion_estado_solicitud, stru_principal.Remit_Dest_Interno_id_remit_dest_Int)
            End If

            resultado_envio_correo = Result
            If Result_ <> "YES" Then
                Actualiza_estado_solicitud_aprobacion_usuario = "Se actualizo el estado de la solictud, pero no se pudo actualizar el estado del tramite en workflow " & Result_
                Exit Function
            End If

            Actualiza_estado_solicitud_aprobacion_usuario = "YES"
        Catch ex As Exception
            myTrans.Rollback()
            If Not myTrans.Connection Is Nothing Then
                Actualiza_estado_solicitud_aprobacion_usuario = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
            If myTrans.Connection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Actualiza_estado_solicitud_aprobacion_usuario = "Error General " & ex.Message
            Exit Function
        End Try
    End Function
    Function Agrega_usuario_a_la_solicitud_aprobacion( _
                                                      ByVal stru_user() As stru_usuario_gestion_compartido, _
                                                      ByVal id_respuesta As Integer, _
                                                      ByVal id_usuario_propietario As String, _
                                                      ByVal correos_electronicos_relacionados As String, _
                                                      ByRef resultado_envio_correo As String, _
                                                      ByVal id_solicitud_principal As Integer, _
                                                      ByRef valores_campo As String) As String

        resultado_envio_correo = "YES"
        Dim Result As String = ""
        Dim Refclas_ra_resp As New Class_ra_respuesta_radicado
        Dim stru_envi As stru_envio = Nothing
        Result = Refclas_ra_resp.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta, _
                                                                                  stru_envi)
        If Result <> "YES" Then
            Agrega_usuario_a_la_solicitud_aprobacion = Result
            Exit Function
        End If
        '-----------------------------------------------------
        'Verifica no este con desición la solicitud principal
        '-----------------------------------------------------
        Dim estado_solicitud As Integer = 0
        Dim descripcion_estado As String = ""
        Result = Me.Retorna_estados_solicitudes_de_aprobacion_general_por_id_solicitud(id_solicitud_principal, _
                                                                                       estado_solicitud, _
                                                                                       descripcion_estado)
        If Result <> "YES" Then
            Agrega_usuario_a_la_solicitud_aprobacion = Result
            Exit Function
        End If
        If estado_solicitud <> 0 Then
            Agrega_usuario_a_la_solicitud_aprobacion = "La solicitud se encuentra en estado " & descripcion_estado & ", imposible agregar el usuario a la solicitud de aprobación"
            Exit Function
        End If
       
        '----------------------------------------------
        'Solicita listar usuarios de gestión
        '----------------------------------------------
        Dim usuarios_relacionados As String = ""
        If Not stru_user Is Nothing Then
            For i As Integer = 0 To stru_user.Length - 1
                If i = 0 Then
                    usuarios_relacionados = stru_user(i).nombre_usuario
                Else
                    usuarios_relacionados = usuarios_relacionados & " - " & stru_user(i).nombre_usuario
                End If
            Next
        End If
        Dim estado_solicitud_estring As String = ""
        Dim nombre_usuario As String = ""
        Dim cargo_usuario As String = ""
        For i As Integer = 0 To stru_user.Length - 1
            If stru_user(i).id_usuario_gestion <> 0 Then
                Result = Verifica_existencia_usuario_relacionado_solicitud_principal(id_solicitud_principal, estado_solicitud_estring, nombre_usuario, cargo_usuario, stru_user(i).id_usuario_gestion)
                If Result <> "YES" Then
                    Agrega_usuario_a_la_solicitud_aprobacion = Result
                    Exit Function
                Else
                    If estado_solicitud_estring = "YES" Then
                        Agrega_usuario_a_la_solicitud_aprobacion = "El usuario " & nombre_usuario & " con el cargo " & cargo_usuario & ", ya tiene una solicitud registrada."
                        Exit Function
                    End If
                End If
            End If
        Next

        Dim date1al As String = ""
        Dim ref_almacen As New ClassRadicador
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Result = refclas_gestion_fechas.Formatea_fecha_time_framework(Date.Now, date1al)
        If Result <> "YES" Then
            Agrega_usuario_a_la_solicitud_aprobacion = Result
            Exit Function
        End If
        Dim stru_general As STRU_SOLICITUD_ARPBACION = Nothing
        Result = Me.Retorna_datos_solicitud_aprobación_documentos(id_solicitud_principal, stru_general)
        If Result <> "YES" Then
            Agrega_usuario_a_la_solicitud_aprobacion = Result
            Exit Function
        End If
        '-----------------------------------------------------------------
        'Formatea fecha limite solicitud de aprobación
        '-----------------------------------------------------------------
        Dim fecha_solicitud_aprobacion As String = stru_general.FECHA_LIMITE_RESPUESTA
        Result = refclas_gestion_fechas.formata_fecha_tipo_date(fecha_solicitud_aprobacion)
        If Result <> "YES" Then
            Agrega_usuario_a_la_solicitud_aprobacion = Result
            Exit Function
        End If
        '-----------------------------------------------------------------
        'Insertar los registros en el log de transacciones
        '-----------------------------------------------------------------
        Dim hor As String = Now
        Dim campos_trans As String = "Null"
        Dim isert_datos As String = ""
        campos_trans = "SOLICITA APROBACION DE RESPUESTA (" & id_respuesta & _
      ")" & "AL USUARIO(S) DE GESTION ID : " & usuarios_relacionados
        isert_datos = isert_datos & "('" & "SOLICITA APROBACION" & "','" & HttpContext.Current.Session.Item("Login_Usuario_Workfow") & "','" & id_usuario_propietario & "','" & date1al & "'," & _
                     id_respuesta & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WORKFLOW WEB','" & campos_trans & "')"
        Dim sql_insert_log_solicitud = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" & _
                                             ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " & _
                                             isert_datos
        Dim values_usuarios_solicitud As String = ""
        Dim sql_insert_registro_usuarios_solicitud As String = "insert into ra_cd_usuarios_solicitudes_aprobacion (Remit_Dest_Interno_id_remit_dest_Int," & _
            "RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION,FECHA_REGISTRO_SOLICITUD,FECHA_LIMITE_RESPUESTA) values "

        For i As Integer = 0 To stru_user.Length - 1
            If stru_user(i).id_usuario_gestion <> 0 Then
                If i = 0 Then
                    values_usuarios_solicitud = "(" & stru_user(i).id_usuario_gestion & "," & id_solicitud_principal & ",'" & date1al & "','" & fecha_solicitud_aprobacion & "')"
                Else
                    values_usuarios_solicitud = values_usuarios_solicitud & ",(" & stru_user(i).id_usuario_gestion & "," & id_solicitud_principal & ",'" & date1al & "','" & fecha_solicitud_aprobacion & "')"
                End If
            End If
        Next

        sql_insert_registro_usuarios_solicitud = sql_insert_registro_usuarios_solicitud & values_usuarios_solicitud
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction = Nothing
        Dim sqlresultinsert As Integer = 0
        Try
            '-------------------------------------------------
            'Registra solicitud de aprobación usuario
            '-------------------------------------------------
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sql_insert_registro_usuarios_solicitud
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Agrega_usuario_a_la_solicitud_aprobacion = "Imposible registrar la solicitud de aprobación para el usuario  "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim last_insert As Object = myCommand.LastInsertedId
            Dim spli_cargo() As String = stru_user(0).cargo_usuario.Split("(")
            Dim cargo As String = spli_cargo(1).Replace(")", "")
            valores_campo = last_insert & "|" & descripcion_estado & "|" & spli_cargo(0).Replace("|", "") & "|" _
             & cargo.Replace("|", "")
            '-----------------------------------------------
            'Registra log respuesta solicitud
            '-----------------------------------------------
            myCommand.CommandText = sql_insert_log_solicitud
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Agrega_usuario_a_la_solicitud_aprobacion = "Imposible registar log de solicitud de aprobación  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Dim correos_relacionados As String = ""
            Result = Me.Retorna_correos_usuarios_documento_aprobacion(id_solicitud_principal, _
                                                                      correos_relacionados)
            If Result <> "YES" Then
                resultado_envio_correo = Result
            Else
                Result = Me.Envio_correo_electronico_usuarios_solicitados(correos_relacionados, _
                                                                          stru_general.NOTA_SOLICITUD, _
                                                                          fecha_solicitud_aprobacion, _
                                                                          id_solicitud_principal, _
                                                                          stru_envi.RADICADO, _
                                                                          stru_envi.ID_AREA)
                resultado_envio_correo = Result
            End If
            'Result = Me.Envio_correo_electronico_usuarios_solicitados(correos_electronicos_relacionados, stru_general.NOTA_SOLICITUD, fecha_solicitud_aprobacion, last_insert)
            'resultado_envio_correo = Result
            Agrega_usuario_a_la_solicitud_aprobacion = "YES"
        Catch ex As Exception
            myTrans.Rollback()
            If Not myTrans.Connection Is Nothing Then
                Agrega_usuario_a_la_solicitud_aprobacion = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
            If myTrans.Connection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Agrega_usuario_a_la_solicitud_aprobacion = "Error General " & ex.Message
            Exit Function
        End Try
    End Function
    Function Retorna_estados_solicitudes_de_aprobacion_general_por_id_solicitud(ByVal id_solicitud As Integer, _
                                                                                ByVal estado_inicial As Integer, _
                                                                                ByRef estado_solicitud As String, _
                                                                                ByRef descripcion_estado As String) As String
        Try
            Dim Result As String = ""
            Dim sql_consulta As String = "Select DESCRIPCION_ESTADO_APROBACION from ra_cd_solicitudes_aprobacion where  ID_SOLICITUDES_APROBACION=" & id_solicitud & " and ESTADO_APROBACION=" & estado_inicial
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_solicitudes_aprobacion")
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_estados_solicitudes_de_aprobacion_general_por_id_solicitud = "Error Función Retorna_estados_solicitudes_de_aprobacion_por_id_tarea_seleccionada dice  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_solicitud = "NO"
                Retorna_estados_solicitudes_de_aprobacion_general_por_id_solicitud = "YES"
                Exit Function
            Else
                descripcion_estado = Datset.Tables(0).Rows(0).Item(0)
                estado_solicitud = "YES"
                Retorna_estados_solicitudes_de_aprobacion_general_por_id_solicitud = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_estados_solicitudes_de_aprobacion_general_por_id_solicitud = "Inconsistencia general función Retorna_estados_solicitudes_de_aprobacion_por_id_tarea_seleccionada " & ex.Message
        End Try
    End Function
    Function Retorna_correos_usuarios_relacionados_solicitud_aprobacion(ByVal id_solicitud_aprobacion As Integer, _
                                                                        ByRef correos_usuarios() As String, _
                                                                        ByRef id_solicitudes_aprobacion() As Integer, _
                                                                        ByRef nombre_usuario() As String) As String
        '--------------------------------------------------------------
        'Función : Retorna datos usuarios relacionados a una solicitud
        'de colaboración general, campos correo electrónico, nombre y 
        'id solictud usuario
        'Fecha : 2017-06-08
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim sql_consulta As String = "Select ID_CD_USUARIOS_SOLICITUDES_APROBACION,rdi.Nombre_Remitente,rdi.Correo_Electronico from ra_cd_usuarios_solicitudes_aprobacion as rcd " & _
                " inner join remit_dest_interno as rdi on (rdi.id_Remit_Dest_Int=rcd.Remit_Dest_Interno_id_remit_dest_Int)" & _
                "  where  rcd.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION=" & id_solicitud_aprobacion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_solicitudes_aprobacion")
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_correos_usuarios_relacionados_solicitud_aprobacion = "Error Función Retorna_correos_usuarios_relacionados_solicitud_aprobacion dice  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_correos_usuarios_relacionados_solicitud_aprobacion = "Imposible encontrar correos electrónicos de los usuarios relacionados a la solicitud  de aprobación " & id_solicitud_aprobacion
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve id_solicitudes_aprobacion(i)
                    id_solicitudes_aprobacion(i) = Datset.Tables(0).Rows(i).Item(0)
                    ReDim Preserve nombre_usuario(i)
                    nombre_usuario(i) = Datset.Tables(0).Rows(i).Item(1)
                    ReDim Preserve correos_usuarios(i)
                    correos_usuarios(i) = Datset.Tables(0).Rows(i).Item(2)
                Next
                Retorna_correos_usuarios_relacionados_solicitud_aprobacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_correos_usuarios_relacionados_solicitud_aprobacion = "Inconsistencia general función Retorna_correos_usuarios_relacionados_solicitud_aprobacion " & ex.Message
        End Try
    End Function
    Function Retorna_estados_solicitudes_de_aprobacion_general_por_id_solicitud(ByVal id_solicitud As Integer, _
                                                                                ByRef estado_solicitud As Integer, _
                                                                                ByRef descripcion_estado As String) As String
        Try
            Dim Result As String = ""
            Dim sql_consulta As String = "Select DESCRIPCION_ESTADO_APROBACION,ESTADO_APROBACION from ra_cd_solicitudes_aprobacion where  ID_SOLICITUDES_APROBACION=" & id_solicitud
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_solicitudes_aprobacion")
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_estados_solicitudes_de_aprobacion_general_por_id_solicitud = "Error Función Retorna_estados_solicitudes_de_aprobacion_por_id_tarea_seleccionada dice  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_estados_solicitudes_de_aprobacion_general_por_id_solicitud = "Imposible encontrar el estado de la solicitud"
                Exit Function
            Else
                descripcion_estado = Datset.Tables(0).Rows(0).Item(0)
                estado_solicitud = Datset.Tables(0).Rows(0).Item(1)
                Retorna_estados_solicitudes_de_aprobacion_general_por_id_solicitud = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_estados_solicitudes_de_aprobacion_general_por_id_solicitud = "Inconsistencia general función Retorna_estados_solicitudes_de_aprobacion_por_id_tarea_seleccionada " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_usuario_relacionado_solicitud_principal(ByVal id_solicitud_aprobacion_principal As Integer, _
                                                                         ByRef estado_solicitud As String, _
                                                                         ByRef nombre_usuario As String, _
                                                                         ByRef cargo_usuario As String, _
                                                                         ByVal id_usuario As Integer) As String
        Try
            Dim Result As String = ""
            Dim sql_consulta As String = "Select Nombre_Remitente,Cargo_Remite from ra_cd_usuarios_solicitudes_aprobacion as rcus " & _
                " Left outer join remit_dest_interno as rdi on (rdi.id_Remit_Dest_Int=rcus.Remit_Dest_Interno_id_remit_dest_Int)" &
                " where  RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION=" & id_solicitud_aprobacion_principal & " and ESTADO_RESPUESTA_SOLICITUD=" & 0 & _
                " and rcus.Remit_Dest_Interno_id_remit_dest_Int=" & id_usuario
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_solicitudes_aprobacion")
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_usuario_relacionado_solicitud_principal = "Error Función Verifica_existencia_usuario_relacionado_solicitud_principal dice  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_solicitud = "NO"
                Verifica_existencia_usuario_relacionado_solicitud_principal = "YES"
                Exit Function
            Else
                estado_solicitud = "YES"
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    nombre_usuario = ""
                Else
                    nombre_usuario = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    cargo_usuario = ""
                Else
                    cargo_usuario = Datset.Tables(0).Rows(0).Item(1)
                End If
                Verifica_existencia_usuario_relacionado_solicitud_principal = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_usuario_relacionado_solicitud_principal = "Inconsistencia general función Verifica_existencia_usuario_relacionado_solicitud_principal " & ex.Message
        End Try
    End Function
    'Function Registra_solicitud_aprobacion(ByVal prioridad_solcitud As String, _
    '                                       ByVal nota_solicitud_aprobacion As String, _
    '                                       ByVal fecha_solicitud_aprobacion As String, _
    '                                       ByVal usuarios_relacionados As String, _
    '                                       ByVal id_respuesta As Integer, _
    '                                       ByVal id_usuario_propietario As String, _
    '                                       ByVal correos_electronicos_relacionados As String, _
    '                                       ByRef resultado_envio_correo As String) As String
    '    resultado_envio_correo = "YES"
    '    Dim Result As String = ""
    '    If prioridad_solcitud = "" Then
    '        Registra_solicitud_aprobacion = "Seleccione la prioridad de la solicitud"
    '        Exit Function
    '    End If
    '    If fecha_solicitud_aprobacion = "" Then
    '        Registra_solicitud_aprobacion = "Seleccione la fecha de vencimiento de la solicitud de aprobación"
    '        Exit Function
    '    End If
    '    If usuarios_relacionados = "" Then
    '        Registra_solicitud_aprobacion = "Seleccione los usuarios a los cuales se les solicitara la aprobación de la respuesta"
    '        Exit Function
    '    End If
    '    If nota_solicitud_aprobacion = "" Then
    '        Registra_solicitud_aprobacion = "Digite la nota de la solicitud de aprobación"
    '        Exit Function
    '    End If
    '    Dim Ref_class_ra_respuesta As New Class_ra_respuesta_radicado
    '    Dim stru_env As stru_envio = Nothing
    '    Result = Ref_class_ra_respuesta.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta, _
    '                                                                                     stru_env)
    '    If Result <> "YES" Then
    '        Registra_solicitud_aprobacion = Result
    '        Exit Function
    '    End If
    '    If stru_env.ID_IMAGEN = 0 Then
    '        Registra_solicitud_aprobacion = "El sistema no registra un documento de respuesta para solicitar aprobación"
    '        Exit Function
    '    End If
    '    If stru_env.FECHA_RESPUETA <> "" Then
    '        Registra_solicitud_aprobacion = "El sistema registra una respuesta permanente imposible solicitar aprobación"
    '        Exit Function
    '    End If
    '    '----------------------------------------------
    '    'Solicita listar usuarios de gestión
    '    '----------------------------------------------
    '    Dim stru_user() As stru_usuario_gestion_compartido = Nothing
    '    Dim refclas_compartir As New ClassGaCompartirDocumento
    '    Result = refclas_compartir.Retorna_matriz_id_usuarios_gestion(usuarios_relacionados, _
    '                                                                  stru_user)
    '    If Result <> "YES" Then
    '        Registra_solicitud_aprobacion = Result
    '        Exit Function
    '    End If
    '    '-------------------------------------------------
    '    'Verifica existencia de solicitudes abiertas
    '    'o aprobadas
    '    '-------------------------------------------------
    '    Dim estado As String = ""
    '    Dim Ref_clas_rc_solicitudes As New ClassRaSolicitudesAprobacion
    '    Result = Ref_clas_rc_solicitudes.Retorna_estados_solicitudes_de_aprobacion_por_id_respuesta(id_respuesta, _
    '                                                                                                0, _
    '                                                                                                estado, _
    '                                                                                                id_usuario_propietario)
    '    If Result <> "YES" Then
    '        Registra_solicitud_aprobacion = Result
    '        Exit Function
    '    End If
    '    If estado = "YES" Then
    '        Registra_solicitud_aprobacion = "El sistema ha detectado solicitudes de aprobación pendientes por decisión, imposible registrar una nueva solicitud. "
    '        Exit Function
    '    End If
    '    estado = ""
    '    Result = Ref_clas_rc_solicitudes.Retorna_estados_solicitudes_de_aprobacion_por_id_respuesta(id_respuesta, _
    '                                                                                                1, _
    '                                                                                                estado, _
    '                                                                                                id_usuario_propietario)
    '    If Result <> "YES" Then
    '        Registra_solicitud_aprobacion = Result
    '        Exit Function
    '    End If
    '    If estado = "YES" Then
    '        Registra_solicitud_aprobacion = "El sistema ha detectado una solicitud donde se aprueba el documento de respuesta, imposible registrar una nueva solicitud."
    '        Exit Function
    '    End If
    '    '-------------------------------------------------
    '    'Solicita fecha limite de respuesta
    '    '-------------------------------------------------
    '    Dim Refclas As New Classgestionrespuesta
    '    Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
    '    Dim stru As stru_envio = Nothing
    '    Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta, _
    '                                                                                stru, _
    '                                                                                1)
    '    If Result <> "YES" Then
    '        Registra_solicitud_aprobacion = Result
    '        Exit Function
    '    End If
    '    Dim fecha_limite As String
    '    If stru.FECHA_VENCE <> "" Then
    '        fecha_limite = stru.FECHA_VENCE
    '        fecha_limite = Formatear_Fecha_Mysql(fecha_limite)
    '        Dim fechaActual As Date
    '        Dim bln As Boolean = DateTime.TryParse(fecha_limite, fechaActual)
    '        If (Not (bln)) Then
    '            Registra_solicitud_aprobacion = "La conversión no es correcta para la fecha actual"
    '            Exit Function
    '        End If
    '        Dim fecha_nueva As Date
    '        bln = DateTime.TryParse(fecha_solicitud_aprobacion, fecha_nueva)
    '        If (Not (bln)) Then
    '            Registra_solicitud_aprobacion = "La conversión no es correcta para nueva fecha"
    '            Exit Function
    '        End If
    '        If (fecha_nueva > fechaActual) Then
    '            Registra_solicitud_aprobacion = "La fecha no puede ser superior a la fecha de vencimiento  de respuesta del trámite"
    '            Exit Function
    '        End If
    '    End If
    '    '-----------------------------------------------
    '    'Registra solicitudes de aprobación
    '    '-----------------------------------------------
    '    Dim date1al As String = ""
    '    Dim ref_almacen As New ClassRadicador
    '    Dim refclas_gestion_fechas As New ClassGestionFechas
    '    Result = refclas_gestion_fechas.Formatea_fecha_time_framework(Date.Now, date1al)
    '    If Result <> "YES" Then
    '        Registra_solicitud_aprobacion = Result
    '        Exit Function
    '    End If
    '    Dim estado_solicitud As Integer = 0
    '    If prioridad_solcitud = "Normal" Then
    '        estado_solicitud = 1
    '    End If
    '    If prioridad_solcitud = "Urgente" Then
    '        estado_solicitud = 2
    '    End If
    '    Dim Ref_class_wf_ruta As New Class_worflow_rutas
    '    Dim nombre_ruta As String = ""
    '    Result = Ref_class_wf_ruta.Retorna_nombre_ruta_workflow(nombre_ruta)
    '    If Result <> "YES" Then
    '        Registra_solicitud_aprobacion = Result
    '        Exit Function
    '    End If
    '    Dim id_ruta As Integer = 0
    '    Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(nombre_ruta, _
    '                                                        id_ruta)
    '    If Result <> "YES" Then
    '        Registra_solicitud_aprobacion = Result
    '        Exit Function
    '    End If
    '    Dim Refclas_confing_lista As New Class_configuracion_listado_ruta
    '    Dim nombre_campo_radicado As String = ""
    '    Result = Refclas_confing_lista.Solicita_nombre_campo_radicado_ruta(id_ruta, _
    '                                                                       nombre_campo_radicado)
    '    If Result <> "YES" Then
    '        Registra_solicitud_aprobacion = Result
    '        Exit Function
    '    End If
    '    Dim id_tarea As Long = 0
    '    Dim Refclas_dat As New Class_DAT_ADIC_TAR
    '    Result = Refclas_dat.Solicita_id_tarea_radicado(stru.RADICADO, _
    '                                                    nombre_ruta, _
    '                                                    nombre_campo_radicado, _
    '                                                    id_tarea)
    '    If Result <> "YES" Then
    '        Registra_solicitud_aprobacion = Result
    '        Exit Function
    '    End If
    '    Dim sql_insert_registro_radicado As String = "insert into ra_cd_solicitudes_aprobacion (Ra_Respuesta_Radicado_ID_RESPUESTA_RADICADO," & _
    '        "Remit_Dest_Interno_id_remit_dest_Int,FECHA_REGISTRO_SOLICITUD,ESTADO_PRIORIDAD,NOTA_SOLICITUD,FECHA_LIMITE_RESPUESTA) VALUES (" & id_respuesta & "," & id_usuario_propietario & _
    '        ",'" & date1al & "'," & estado_solicitud & ",'" & nota_solicitud_aprobacion & "','" & fecha_solicitud_aprobacion & "')"


    '    '-----------------------------------------------------------------
    '    'Insertar los registros en el log de transacciones
    '    '-----------------------------------------------------------------
    '    Dim hor As String = Now
    '    Dim campos_trans As String = "Null"
    '    Dim isert_datos As String = ""
    '    campos_trans = "SOLICITA APROBACION DE RESPUESTA (" & id_respuesta & _
    '  ")" & "AL USUARIO(S) DE GESTION ID : " & usuarios_relacionados
    '    isert_datos = isert_datos & "('" & "SOLICITA APROBACION" & "','" & HttpContext.Current.Session.Item("Login_Usuario_Workfow") & "','" & id_usuario_propietario & "','" & date1al & "'," & _
    '                 id_respuesta & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WORKFLOW WEB','" & campos_trans & "')"
    '    Dim sql_insert_log_solicitud = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" & _
    '                                         ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " & _
    '                                         isert_datos

    '    Dim myConnection As New MySqlConnection
    '    Dim ref As New conect.Dbase_Conction_Mysql_RA
    '    ref.Returna_Conexion_Mysql(myConnection)
    '    Dim myTrans As MySqlTransaction = Nothing
    '    Dim sqlresultinsert As Integer = 0
    '    Try
    '        '-------------------------------------------------
    '        'Registra solicitud de aprobación
    '        '-------------------------------------------------
    '        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
    '        myTrans = myConnection.BeginTransaction()
    '        myCommand.Connection = myConnection
    '        myCommand.Transaction = myTrans
    '        myCommand.CommandText = sql_insert_registro_radicado
    '        sqlresultinsert = myCommand.ExecuteNonQuery()
    '        If sqlresultinsert = 0 Then
    '            Registra_solicitud_aprobacion = "Imposible registrar la solicitud de aprobación de la respuesta  "
    '            'myTrans.Rollback()
    '            myConnection.Close()
    '            Exit Function
    '        End If
    '        Dim id_solicitud_aprobacion As Object = Nothing
    '        id_solicitud_aprobacion = myCommand.LastInsertedId
    '        '----------------------------------------------------------------
    '        'Insertar los usuarios relacionados a la solicitud de aprobación
    '        '----------------------------------------------------------------
    '        Dim values_usuarios_solicitud As String = ""
    '        Dim sql_insert_registro_usuarios_solicitud As String = "insert into ra_cd_usuarios_solicitudes_aprobacion (Remit_Dest_Interno_id_remit_dest_Int," & _
    '            "RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION,FECHA_REGISTRO_SOLICITUD,FECHA_LIMITE_RESPUESTA) values "
    '        For i As Integer = 0 To stru_user.Length - 1
    '            If stru_user(i).id_usuario_gestion <> 0 Then
    '                If i = 0 Then
    '                    values_usuarios_solicitud = "(" & stru_user(i).id_usuario_gestion & "," & id_solicitud_aprobacion & ",'" & date1al & "','" & fecha_solicitud_aprobacion & "')"
    '                Else
    '                    values_usuarios_solicitud = values_usuarios_solicitud & ",(" & stru_user(i).id_usuario_gestion & "," & id_solicitud_aprobacion & ",'" & date1al & "','" & fecha_solicitud_aprobacion & "')"
    '                End If
    '            End If
    '        Next
    '        sql_insert_registro_usuarios_solicitud = sql_insert_registro_usuarios_solicitud & values_usuarios_solicitud
    '        myCommand.CommandText = sql_insert_registro_usuarios_solicitud
    '        sqlresultinsert = myCommand.ExecuteNonQuery()
    '        If sqlresultinsert = 0 Then
    '            Registra_solicitud_aprobacion = "Imposible registar los usuarios relacionados a la solicitud de aprobación  "
    '            myTrans.Rollback()
    '            myConnection.Close()
    '            Exit Function
    '        End If
    '        '-----------------------------------------------
    '        'Actualiza estado respuesta solicitud
    '        '-----------------------------------------------
    '        Dim update As String = "Update ra_respuesta_radicado set FECHA_REGISTRO_SOLICITUD='" & date1al & "',ID_SOLICITUDES_APROBACION_RESP=" & id_solicitud_aprobacion & _
    '             ",ESTADO_APROBACION=0,TIEMPO_RESPUESTA_APROBACION=null,FECHA_REGISTRO_APROBACION=null,NOTA_SOLICITUD='" & nota_solicitud_aprobacion & "',DESCRIPCION_ESTADO_APROBACION='POR CONFIRMAR'" & "  where ID_RESPUESTA_RADICADO=" & id_respuesta
    '        myCommand.CommandText = update
    '        sqlresultinsert = myCommand.ExecuteNonQuery()
    '        If sqlresultinsert = 0 Then
    '            Registra_solicitud_aprobacion = "Imposible registar actualización respuesta  "
    '            myTrans.Rollback()
    '            myConnection.Close()
    '            Exit Function
    '        End If
    '        '-----------------------------------------------
    '        'Registra log respuesta solicitud
    '        '-----------------------------------------------
    '        myCommand.CommandText = sql_insert_log_solicitud
    '        sqlresultinsert = myCommand.ExecuteNonQuery()
    '        If sqlresultinsert = 0 Then
    '            Registra_solicitud_aprobacion = "Imposible registar log de solicitud de aprobación  "
    '            myTrans.Rollback()
    '            myConnection.Close()
    '            Exit Function
    '        End If
    '        myTrans.Commit()
    '        myConnection.Close()
    '        Dim Result_ As String = Refclas_dat.Actualiza_estado_tramite_tarea_workflow(nombre_ruta, _
    '                                                                                   id_tarea, _
    '                                                                                   "Solicitud por aprobación")
    '        Dim correos_relacionados As String = ""
    '        Result = Me.Retorna_correos_usuarios_documento_aprobacion(id_solicitud_aprobacion, _
    '                                                                  correos_relacionados)
    '        If Result <> "YES" Then
    '            resultado_envio_correo = Result
    '        Else
    '            Result = Me.Envio_correo_electronico_usuarios_solicitados(correos_relacionados, _
    '                                                                      nota_solicitud_aprobacion, _
    '                                                                      fecha_solicitud_aprobacion, _
    '                                                                      id_solicitud_aprobacion, _
    '                                                                      stru_env.RADICADO, _
    '                                                                      stru_env.ID_IMAGEN)
    '            resultado_envio_correo = Result
    '        End If
    '        If Result_ <> "YES" Then
    '            Registra_solicitud_aprobacion = "Se reverso la respuesta, pero no se pudo actualizar el estado del tramite en workflow " & Result_
    '            Exit Function
    '        End If
    '        Registra_solicitud_aprobacion = "YES"
    '    Catch ex As Exception
    '        myTrans.Rollback()
    '        If Not myTrans.Connection Is Nothing Then
    '            Registra_solicitud_aprobacion = "An exception of type " + ex.GetType().ToString() + _
    '                              " was encountered while attempting to roll back the transaction."
    '            Exit Function
    '        End If
    '        If myTrans.Connection.State = ConnectionState.Open Then
    '            myConnection.Close()
    '        End If
    '        Registra_solicitud_aprobacion = "Error General " & ex.Message
    '        Exit Function
    '    End Try
    'End Function
    Function Registra_solicitud_aprobacion_new(ByVal prioridad_solcitud As String, _
                                               ByVal nota_solicitud_aprobacion As String, _
                                               ByVal fecha_solicitud_aprobacion As String, _
                                               ByVal stru_user() As stru_usuario_gestion_compartido, _
                                               ByVal id_respuesta As Integer, _
                                               ByVal id_usuario_propietario As String, _
                                               ByVal correos_electronicos_relacionados As String, _
                                               ByRef resultado_envio_correo As String, _
                                               ByRef valor_campos As String) As String
        resultado_envio_correo = "YES"
        Dim Result As String = ""
        If prioridad_solcitud = "" Then
            Registra_solicitud_aprobacion_new = "Seleccione la prioridad de la solicitud"
            Exit Function
        End If
        If fecha_solicitud_aprobacion = "" Then
            Registra_solicitud_aprobacion_new = "Seleccione la fecha de vencimiento de la solicitud de aprobación"
            Exit Function
        End If

        If nota_solicitud_aprobacion = "" Then
            Registra_solicitud_aprobacion_new = "Digite la nota de la solicitud de aprobación"
            Exit Function
        End If
        Dim Ref_class_ra_respuesta As New Class_ra_respuesta_radicado
        Dim stru_env As stru_envio = Nothing
        Result = Ref_class_ra_respuesta.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta, _
                                                                                         stru_env)
        If Result <> "YES" Then
            Registra_solicitud_aprobacion_new = Result
            Exit Function
        End If
        If stru_env.ID_IMAGEN = 0 Then
            Registra_solicitud_aprobacion_new = "El sistema no registra un documento de respuesta para solicitar aprobación"
            Exit Function
        End If
        If stru_env.FECHA_RESPUETA <> "" Then
            Registra_solicitud_aprobacion_new = "El sistema registra una respuesta permanente imposible solicitar aprobación"
            Exit Function
        End If
        '----------------------------------------------
        'Solicita listar usuarios de gestión
        '----------------------------------------------
        Dim usuarios_relacionados As String = ""
        If Not stru_user Is Nothing Then
            For i As Integer = 0 To stru_user.Length - 1
                If i = 0 Then
                    usuarios_relacionados = stru_user(i).nombre_usuario
                Else
                    usuarios_relacionados = usuarios_relacionados & " - " & stru_user(i).nombre_usuario
                End If
            Next
        End If
        '-------------------------------------------------
        'Verifica existencia de solicitudes abiertas
        'o aprobadas
        '-------------------------------------------------
        Dim estado As String = ""
        Dim Ref_clas_rc_solicitudes As New ClassRaSolicitudesAprobacion
        Result = Ref_clas_rc_solicitudes.Retorna_estados_solicitudes_de_aprobacion_por_id_respuesta(id_respuesta, _
                                                                                                    0, _
                                                                                                    estado, _
                                                                                                    id_usuario_propietario)
        If Result <> "YES" Then
            Registra_solicitud_aprobacion_new = Result
            Exit Function
        End If
        If estado = "YES" Then
            Registra_solicitud_aprobacion_new = "El sistema ha detectado solicitudes de aprobación pendientes por decisión, imposible registrar una nueva solicitud. "
            Exit Function
        End If
        estado = ""
        Result = Ref_clas_rc_solicitudes.Retorna_estados_solicitudes_de_aprobacion_por_id_respuesta(id_respuesta, _
                                                                                                    1, _
                                                                                                    estado, _
                                                                                                    id_usuario_propietario)
        If Result <> "YES" Then
            Registra_solicitud_aprobacion_new = Result
            Exit Function
        End If
        If estado = "YES" Then
            Registra_solicitud_aprobacion_new = "El sistema ha detectado una solicitud donde se aprueba el documento de respuesta, imposible registrar una nueva solicitud."
            Exit Function
        End If
        '-------------------------------------------------
        'Solicita fecha limite de respuesta
        '-------------------------------------------------
        Dim Refclas As New Classgestionrespuesta
        Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
        Dim stru As stru_envio = Nothing
        Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta, _
                                                                                    stru, _
                                                                                    1)
        If Result <> "YES" Then
            Registra_solicitud_aprobacion_new = Result
            Exit Function
        End If
        Dim fecha_limite As String
        Dim ClassGestionFechas As New ClassGestionFechas
        If stru.FECHA_VENCE <> "" Then
            fecha_limite = stru.FECHA_VENCE
            ClassGestionFechas.formato_fecha_estructura(fecha_limite)
            Dim fechaActual As Date
            Dim bln As Boolean = DateTime.TryParse(fecha_limite, fechaActual)
            If (Not (bln)) Then
                Registra_solicitud_aprobacion_new = "La conversión no es correcta para la fecha actual"
                Exit Function
            End If
            Dim fecha_nueva As Date
            bln = DateTime.TryParse(fecha_solicitud_aprobacion, fecha_nueva)
            If (Not (bln)) Then
                Registra_solicitud_aprobacion_new = "La conversión no es correcta para nueva fecha"
                Exit Function
            End If
            If (fecha_nueva > fechaActual) Then
                Registra_solicitud_aprobacion_new = "La fecha no puede ser superior a la fecha de vencimiento  de respuesta del trámite"
                Exit Function
            End If
        End If
        '-----------------------------------------------
        'Registra solicitudes de aprobación
        '-----------------------------------------------
        Dim date1al As String = ""
        Dim ref_almacen As New ClassRadicador
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Result = refclas_gestion_fechas.Formatea_fecha_time_framework(Date.Now, date1al)
        If Result <> "YES" Then
            Registra_solicitud_aprobacion_new = Result
            Exit Function
        End If
        Dim estado_solicitud As Integer = 0
        If prioridad_solcitud = "Normal" Then
            estado_solicitud = 1
        End If
        If prioridad_solcitud = "Urgente" Then
            estado_solicitud = 2
        End If
        Dim Ref_class_wf_ruta As New Class_worflow_rutas
        Dim nombre_ruta As String = ""
        Result = Ref_class_wf_ruta.Retorna_nombre_ruta_workflow(nombre_ruta)
        If Result <> "YES" Then
            Registra_solicitud_aprobacion_new = Result
            Exit Function
        End If
        Dim id_ruta As Integer = 0
        Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(nombre_ruta, _
                                                            id_ruta)
        If Result <> "YES" Then
            Registra_solicitud_aprobacion_new = Result
            Exit Function
        End If
        Dim Refclas_confing_lista As New Class_configuracion_listado_ruta
        Dim nombre_campo_radicado As String = ""
        Result = Refclas_confing_lista.SolicitaNombreCampoRadicadoRuta(id_ruta,
                                                                       nombre_campo_radicado)
        If Result <> "YES" Then
            Registra_solicitud_aprobacion_new = Result
            Exit Function
        End If
        Dim id_tarea As Long = 0
        Dim Refclas_dat As New Class_DAT_ADIC_TAR
        Result = Refclas_dat.Solicita_id_tarea_radicado(stru.RADICADO, _
                                                        nombre_ruta, _
                                                        nombre_campo_radicado, _
                                                        id_tarea, _
                                                        0)
        If Result <> "YES" Then
            Registra_solicitud_aprobacion_new = Result
            Exit Function
        End If
        Dim sql_insert_registro_radicado As String = "insert into ra_cd_solicitudes_aprobacion (Ra_Respuesta_Radicado_ID_RESPUESTA_RADICADO," & _
            "Remit_Dest_Interno_id_remit_dest_Int,FECHA_REGISTRO_SOLICITUD,ESTADO_PRIORIDAD,NOTA_SOLICITUD,FECHA_LIMITE_RESPUESTA) VALUES (" & id_respuesta & "," & id_usuario_propietario & _
            ",'" & date1al & "'," & estado_solicitud & ",'" & nota_solicitud_aprobacion & "','" & fecha_solicitud_aprobacion & "')"


        '-----------------------------------------------------------------
        'Insertar los registros en el log de transacciones
        '-----------------------------------------------------------------
        Dim hor As String = Now
        Dim campos_trans As String = "Null"
        Dim isert_datos As String = ""
        campos_trans = "SOLICITA APROBACION DE RESPUESTA (" & id_respuesta & _
      ")" & "AL USUARIO(S) DE GESTION ID : " & usuarios_relacionados
        isert_datos = isert_datos & "('" & "SOLICITA APROBACION" & "','" & HttpContext.Current.Session.Item("Login_Usuario_Workfow") & "','" & id_usuario_propietario & "','" & date1al & "'," & _
                     id_respuesta & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WORKFLOW WEB','" & campos_trans & "')"
        Dim sql_insert_log_solicitud = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" & _
                                             ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " & _
                                             isert_datos

        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction = Nothing
        Dim sqlresultinsert As Integer = 0
        Try
            '-------------------------------------------------
            'Registra solicitud de aprobación
            '-------------------------------------------------
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sql_insert_registro_radicado
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Registra_solicitud_aprobacion_new = "Imposible registrar la solicitud de aprobación de la respuesta  "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim id_solicitud_aprobacion As Object = Nothing
            id_solicitud_aprobacion = myCommand.LastInsertedId
            '----------------------------------------------------------------
            'Insertar los usuarios relacionados a la solicitud de aprobación
            '----------------------------------------------------------------
            Dim values_usuarios_solicitud As String = ""
            Dim sql_insert_registro_usuarios_solicitud As String = "insert into ra_cd_usuarios_solicitudes_aprobacion (Remit_Dest_Interno_id_remit_dest_Int," & _
                "RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION,FECHA_REGISTRO_SOLICITUD,FECHA_LIMITE_RESPUESTA) values "
            For i As Integer = 0 To stru_user.Length - 1
                If stru_user(i).id_usuario_gestion <> 0 Then
                    If i = 0 Then
                        values_usuarios_solicitud = "(" & stru_user(i).id_usuario_gestion & "," & id_solicitud_aprobacion & ",'" & date1al & "','" & fecha_solicitud_aprobacion & "')"
                    Else
                        values_usuarios_solicitud = values_usuarios_solicitud & ",(" & stru_user(i).id_usuario_gestion & "," & id_solicitud_aprobacion & ",'" & date1al & "','" & fecha_solicitud_aprobacion & "')"
                    End If
                End If
            Next
            sql_insert_registro_usuarios_solicitud = sql_insert_registro_usuarios_solicitud & values_usuarios_solicitud
            myCommand.CommandText = sql_insert_registro_usuarios_solicitud
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Registra_solicitud_aprobacion_new = "Imposible registar los usuarios relacionados a la solicitud de aprobación  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim fecha_tempo As String = ""
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            ref_ClassGestionFechas.FormateaFechaTimeDbDefault(Date.Now, _
                                                                  fecha_tempo)
            fecha_tempo = fecha_tempo.Replace("-", "/")
            valor_campos = id_solicitud_aprobacion & "|" & fecha_tempo & "|" & "" & "|" _
             & prioridad_solcitud & "|" & nota_solicitud_aprobacion.Replace("|", "")
            '-----------------------------------------------
            'Actualiza estado respuesta solicitud
            '-----------------------------------------------
            Dim update As String = "Update ra_respuesta_radicado set FECHA_REGISTRO_SOLICITUD='" & date1al & "',ID_SOLICITUDES_APROBACION_RESP=" & id_solicitud_aprobacion & _
                 ",ESTADO_APROBACION=0,TIEMPO_RESPUESTA_APROBACION=null,FECHA_REGISTRO_APROBACION=null,NOTA_SOLICITUD='" & nota_solicitud_aprobacion & "',DESCRIPCION_ESTADO_APROBACION='POR CONFIRMAR'" & "  where ID_RESPUESTA_RADICADO=" & id_respuesta
            myCommand.CommandText = update
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Registra_solicitud_aprobacion_new = "Imposible registar actualización respuesta  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '-----------------------------------------------
            'Registra log respuesta solicitud
            '-----------------------------------------------
            myCommand.CommandText = sql_insert_log_solicitud
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Registra_solicitud_aprobacion_new = "Imposible registar log de solicitud de aprobación  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Dim Result_ As String = Refclas_dat.Actualiza_estado_tramite_tarea_workflow(nombre_ruta,
                                                                                       id_tarea,
                                                                                       "Solicitud por aprobación")
            If Not HttpContext.Current.Session.Item("dat_gred_cahce") Is Nothing Then
                For i As Integer = 0 To HttpContext.Current.Session.Item("dat_gred_cahce").Tables(0).Rows.Count - 1
                    If HttpContext.Current.Session.Item("dat_gred_cahce").Tables(0).Rows(i).Item(0) = id_tarea Then
                        HttpContext.Current.Session.Item("dat_gred_cahce").Tables(0).Rows(i).Item("ESTADO") = "Solicitud por aprobación"
                        HttpContext.Current.Session.Item("dat_gred_cahce").AcceptChanges()
                        Exit For
                    End If
                Next
            End If
            If HttpContext.Current.Session.Item("dat_gred_cahce_restore").GetType.ToString = "System.Data.DataSet" Then
                For i As Integer = 0 To HttpContext.Current.Session.Item("dat_gred_cahce_restore").Tables(0).Rows.Count - 1
                    If HttpContext.Current.Session.Item("dat_gred_cahce_restore").Tables(0).Rows(i).Item(0) = id_tarea Then
                        HttpContext.Current.Session.Item("dat_gred_cahce_restore").Tables(0).Rows(i).Item("ESTADO") = "Solicitud por aprobación"
                        HttpContext.Current.Session.Item("dat_gred_cahce_restore").AcceptChanges()
                        Exit For
                    End If
                Next
            End If
            Dim correos_relacionados As String = ""
            Result = Me.Retorna_correos_usuarios_documento_aprobacion(id_solicitud_aprobacion, _
                                                                      correos_relacionados)
            If Result <> "YES" Then
                resultado_envio_correo = Result
            Else
                Result = Me.Envio_correo_electronico_usuarios_solicitados(correos_relacionados, _
                                                                          nota_solicitud_aprobacion, _
                                                                          fecha_solicitud_aprobacion, _
                                                                          id_solicitud_aprobacion, _
                                                                          stru_env.RADICADO, _
                                                                          stru_env.ID_IMAGEN)
                resultado_envio_correo = Result
            End If
            If Result_ <> "YES" Then
                Registra_solicitud_aprobacion_new = "Se reverso la respuesta, pero no se pudo actualizar el estado del tramite en workflow " & Result_
                Exit Function
            End If
            Registra_solicitud_aprobacion_new = "YES"
        Catch ex As Exception
            myTrans.Rollback()
            If Not myTrans.Connection Is Nothing Then
                Registra_solicitud_aprobacion_new = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
            If myTrans.Connection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Registra_solicitud_aprobacion_new = "Error General " & ex.Message
            Exit Function
        End Try
    End Function

    Function Retorna_correos_usuarios_documento_aprobacion(ByVal id_documento_compartido As Integer, _
                                                           ByRef correos_relacionados As String) As String
        '----------------------------------------------------
        'Función : Retorna correos electrónicos de usuarios
        'de documentos para aprobación
        'Fecha : 2017-03-28
        'Ing :Miguel Angel Urueta Miranda
        '----------------------------------------------------
        Try
            Dim sql_consulta As String = "SELECT rdi.Correo_Electronico " & _
                     " from ra_cd_usuarios_solicitudes_aprobacion as rcu " & _
                     "inner join remit_dest_interno as rdi on (rdi.id_Remit_Dest_Int=rcu.Remit_Dest_Interno_id_remit_dest_Int) where RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION=" & id_documento_compartido
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_solicitudes_aprobacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_correos_usuarios_documento_aprobacion = "Error listando correos electrónicos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_correos_usuarios_documento_aprobacion = "Imposible encontrar los correos electrónicos de la solicitud " & id_documento_compartido
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    If Datset.Tables(0).Rows(i).IsNull(0) = True Then
                    Else
                        If correos_relacionados = "" Then
                            correos_relacionados = Datset.Tables(0).Rows(i).Item(0)
                        Else
                            correos_relacionados = correos_relacionados & "," & Datset.Tables(0).Rows(i).Item(0)
                        End If
                    End If
                Next
                Retorna_correos_usuarios_documento_aprobacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_correos_usuarios_documento_aprobacion = "Inconsistencia general función Retorna_correos_usuarios_documento_aprobacion " & ex.Message
        End Try
    End Function
    Function Retorna_estado_solicitud_aprobacion_final(ByVal id_solicitud_aprobacion As Integer, _
                                                       ByRef estado_general_solicitud As Integer, _
                                                       ByRef descripcion_estado_general_solicitud As String) As String
        Try
            Dim numero_aprobado As Integer = 0
            Dim numero_no_aprobado As Integer = 0
            Dim numero_archivado As Integer = 0
            Dim numero_sin_desicion As Integer = 0
            Dim sql_consulta As String = "SELECT ESTADO_RESPUESTA_SOLICITUD " & _
                       " from ra_cd_usuarios_solicitudes_aprobacion  where RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION=" & id_solicitud_aprobacion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_solicitudes_aprobacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_estado_solicitud_aprobacion_final = "Error listando usuarios relacionados a solicitudes de información " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_estado_solicitud_aprobacion_final = "No se encontraron solicitudes de usuario relacionadas "
                Exit Function
            Else
                If Datset.Tables(0).Rows.Count > 1 Then
                    For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                        If Datset.Tables(0).Rows(i).Item(0) = 0 Then
                            numero_sin_desicion = numero_sin_desicion + 1
                        End If
                        If Datset.Tables(0).Rows(i).Item(0) = 1 Then
                            numero_aprobado = numero_aprobado + 1
                        End If
                        If Datset.Tables(0).Rows(i).Item(0) = 2 Then
                            numero_no_aprobado = numero_no_aprobado + 1
                        End If
                        If Datset.Tables(0).Rows(i).Item(0) = 3 Then
                            numero_archivado = numero_archivado + 1
                        End If
                    Next
                    '----------------------------------------
                    'Verifica documentos sin seleccion
                    '----------------------------------------
                    If numero_sin_desicion <= Datset.Tables(0).Rows.Count Then

                        If numero_sin_desicion > 1 Then
                            '-----------------------------------
                            'No desición
                            '-----------------------------------
                            estado_general_solicitud = -2
                        Else
                            Dim numero_general As Integer = 0
                            If numero_archivado >= numero_general Then
                                numero_general = numero_archivado
                                estado_general_solicitud = 3
                                descripcion_estado_general_solicitud = "Archivado"
                            End If

                            If numero_aprobado >= numero_general Then
                                numero_general = numero_aprobado
                                estado_general_solicitud = 1
                                descripcion_estado_general_solicitud = "Aprobado"
                            End If
                            If numero_no_aprobado >= numero_general Then
                                numero_general = numero_no_aprobado
                                estado_general_solicitud = 2
                                descripcion_estado_general_solicitud = "Desaprobado"
                            End If
                        End If

                    End If
                Else
                    '-------------------------------
                    'Caso unica solicitud
                    '-------------------------------
                    estado_general_solicitud = -1
                End If

                Retorna_estado_solicitud_aprobacion_final = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_estado_solicitud_aprobacion_final = "Inconsistencia general función Lista_numeros_estado_solicitud_aprobacion " & ex.Message
        End Try
    End Function
    Function Lista_usuarios_relacionados_a_solictud_de_aprobacion(ByVal id_solicitud_aprobacion As Integer, _
                                                                  ByRef grediview As GridView, _
                                                                  ByRef HiddenEmailconsulta As Object, _
                                                                  ByRef reflabel As Label, _
                                                                  ByRef hideselecion As Object, _
                                                                  ByRef update As UpdatePanel) As String
        Try

            Dim sql_consulta As String = "SELECT rcs.ID_CD_USUARIOS_SOLICITUDES_APROBACION," & _
                    "DESCRIPCION_ESTADO_RESPUESTA AS ESTADO,rdi.Nombre_Remitente as NOMBRE,rdi.Cargo_Remite as CARGO from ra_cd_usuarios_solicitudes_aprobacion AS rcs" & _
                    " INNER JOIN remit_dest_interno AS rdi on (rdi.id_Remit_Dest_Int=rcs.Remit_Dest_Interno_id_remit_dest_Int)  where rcs.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION=" & id_solicitud_aprobacion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_solicitudes_aprobacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_usuarios_relacionados_a_solictud_de_aprobacion = "Error listando usuarios relacionados a solicitudes de información " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                HiddenEmailconsulta.value = "YES"
                reflabel.Text = "Se encontraron 0 registro(s) "
                grediview.DataSource = Nothing
                'grediview.DataKeyNames = DataKey
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                Lista_usuarios_relacionados_a_solictud_de_aprobacion = "YES"
                Exit Function
            Else
                HiddenEmailconsulta.value = "YES"
                reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) "
                'grediview.DataKeyNames = DataKey
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-sticky-note fa-lg")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-info btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Lista de notas de usuario")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "notas_usuario_solicitud")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-folder-open fa-lg imag_crusor_da")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn   btn-primary btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Lista anexos usuario")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "anexos_usuario_solicitud")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-archive fa-lg imag_crusor_da")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn  btn-warning btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Archiva solicitud usuario")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "archiva_usuario_solicitud")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-envelope fa-lg imag_crusor_da")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-primary btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Notifica solicitud usuario al correo electrónico")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "notifica_usuario_solicitud")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    grediview.Rows(i).Cells(0).Controls.Add(divhtml)
                    For z As Integer = 1 To grediview.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If
                    Next
                Next
                Lista_usuarios_relacionados_a_solictud_de_aprobacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_usuarios_relacionados_a_solictud_de_aprobacion = "Inconsistencia general función Lista_usuarios_relacionados_a_solictud_de_aprobacion " & ex.Message
        End Try

    End Function
    Function Lista_usuarios_relacionados_a_solictud_de_aprobacion(ByVal id_solicitud_aprobacion As Integer, _
                                                                  ByRef grediview As GridView, _
                                                                  ByRef reflabel As Label, _
                                                                  ByRef update As UpdatePanel) As String
        Try

            Dim sql_consulta As String = "SELECT ID_CD_USUARIOS_SOLICITUDES_APROBACION," & _
                    "DESCRIPCION_ESTADO_RESPUESTA AS ESTADO,rdi.Nombre_Remitente as NOMBRE,rdi.Cargo_Remite as CARGO from ra_cd_usuarios_solicitudes_aprobacion AS rcs" & _
                    " INNER JOIN remit_dest_interno AS rdi on (rdi.id_Remit_Dest_Int=rcs.Remit_Dest_Interno_id_remit_dest_Int)  where rcs.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION=" & id_solicitud_aprobacion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_solicitudes_aprobacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_usuarios_relacionados_a_solictud_de_aprobacion = "Error listando usuarios relacionados a solicitudes de información " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                reflabel.Text = "Se encontraron 0 registro(s) "
                grediview.DataSource = Nothing
                grediview.DataBind()
                update.Update()
                Lista_usuarios_relacionados_a_solictud_de_aprobacion = "YES"
                Exit Function
            Else
                reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) "
                grediview.DataSource = Datset
                grediview.DataBind()
                update.Update()
                Lista_usuarios_relacionados_a_solictud_de_aprobacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_usuarios_relacionados_a_solictud_de_aprobacion = "Inconsistencia general función Lista_usuarios_relacionados_a_solictud_de_aprobacion " & ex.Message
        End Try

    End Function
    Function Retorna_numero_de_solicitudes_aprobacion_de_un_usuario(ByVal id_usuario As Integer, _
                                                                    ByRef numero_solicitud As Integer) As String
        Try
            Dim sql_consulta As String = "SELECT ID_CD_USUARIOS_SOLICITUDES_APROBACION  from ra_cd_usuarios_solicitudes_aprobacion AS rcs" & _
                       " where rcs.Remit_Dest_Interno_id_remit_dest_Int=" & id_usuario & _
                       " and rcs.ESTADO_RESPUESTA_SOLICITUD=0"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_solicitudes_aprobacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_numero_de_solicitudes_aprobacion_de_un_usuario = "Error listando Retorna_numero_de_solicitudes_aprobacion_de_un_usuario " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                numero_solicitud = Datset.Tables(0).Rows.Count
                Retorna_numero_de_solicitudes_aprobacion_de_un_usuario = "YES"
                Exit Function
            Else
                numero_solicitud = Datset.Tables(0).Rows.Count
                Retorna_numero_de_solicitudes_aprobacion_de_un_usuario = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_numero_de_solicitudes_aprobacion_de_un_usuario = "Inconsistencia general función Retorna_numero_de_solicitudes_aprobacion_de_un_usuario " & ex.Message
        End Try
    End Function
    Function Lista_usuarios_relacionados_a_solictud_de_aprobacion_pendiente(ByVal id_usuario_propietario As Integer, _
                                                                           ByRef grediview As GridView, _
                                                                           ByRef reflabel As Label, _
                                                                           ByRef hideselecion As Object, _
                                                                           ByRef update As UpdatePanel, _
                                                                           ByVal estado_solicitud As String, _
                                                                           ByVal tipo_consulta As Integer, _
                                                                           ByVal valor_consulta As String, _
                                                                           ByRef colum_order_name As String, _
                                                                           ByRef order_colum As String) As String
        Try
            Dim estado_ As Object
            If estado_solicitud = "Solicitudes Pendientes por aprobar" Then
                estado_ = "and rrr.ESTADO_APROBACION=" & 0
            End If
            If estado_solicitud = "Solicitudes Aprobadas" Then
                estado_ = "and rrr.ESTADO_APROBACION=" & 1
            End If
            If estado_solicitud = "Solicitudes Desaprobadas" Then
                estado_ = "and rrr.ESTADO_APROBACION=" & 2
            End If
            If estado_solicitud = "Solicitudes Archivadas" Then
                estado_ = "and rrr.ESTADO_APROBACION=" & 3
            End If
            If estado_solicitud = "Todas" Then
                estado_ = ""
            End If
            Dim sql_consulta As String = ""
            sql_consulta = ""
            If tipo_consulta = 1 Then
                sql_consulta = "SELECT rrr.ID_RESPUESTA_RADICADO as ID_RESPUESTA_RADICADO,rrr.ID_TAREA_WF as ID_TAREA_WF ,rrr.ID_SOLICITUDES_APROBACION_RESP AS SOLICITUD," & _
                   "rrr.DESCRIPCION_ESTADO_APROBACION AS ESTADO,rrr.DESTINATARIO as PETICIONARIO,rrr.RADICADO as RADICADO ,rrr.FECHA_VENCE As VENCIMIENTO " & _
                   ",rrr.ASUNTO as ASUNTO ,rrr.TRAMITE_DOCUMENTO as TRAMITE from ra_respuesta_radicado AS rrr " & _
                   "   where rrr.ID_REMIT_DEST_INT=" & id_usuario_propietario & _
                   "  " & estado_ & " and rrr.FECHA_RESPUETA is null and ID_SOLICITUDES_APROBACION_RESP is not null order by  " & colum_order_name & " " & order_colum
            Else
                sql_consulta = "SELECT rrr.ID_RESPUESTA_RADICADO as ID_RESPUESTA_RADICADO,rrr.ID_TAREA_WF as ID_TAREA_WF ,rrr.ID_SOLICITUDES_APROBACION_RESP AS SOLICITUD," & _
                   "rrr.DESCRIPCION_ESTADO_APROBACION AS ESTADO,rrr.DESTINATARIO as PETICIONARIO,rrr.RADICADO as RADICADO ,rrr.FECHA_VENCE As VENCIMIENTO " & _
                   ",rrr.ASUNTO as ASUNTO ,rrr.TRAMITE_DOCUMENTO as TRAMITE from ra_respuesta_radicado AS rrr " & _
                  "   where (" & _
                    " rrr.ID_SOLICITUDES_APROBACION_RESP like '%" & valor_consulta & "%'" & _
                    " or rrr.DESCRIPCION_ESTADO_APROBACION like '%" & valor_consulta & "%'" & _
                    " or rrr.RADICADO like '%" & valor_consulta & "%'" & _
                    " or rrr.FECHA_VENCE like '%" & valor_consulta & "%'" & _
                    " or rrr.DESTINATARIO like '%" & valor_consulta & "%') and " & _
                    " ID_REMIT_DEST_INT=" & id_usuario_propietario & _
                    " and rrr.FECHA_RESPUETA is null and ID_SOLICITUDES_APROBACION_RESP is not null order by  " & colum_order_name & " " & order_colum
            End If
            HttpContext.Current.Session.Item("Sort_matri_colum_solicitudes_apro") = {"OPCIONES", "ID_RESPUESTA_RADICADO", _
                                                                               "ID_TAREA_WF", "SOLICITUD", _
                                                                               "ESTADO", "PETICIONARIO", "RADICADO", _
                                                                               "VENCIMIENTO"}
            HttpContext.Current.Session.Item("SortExpression_solicitudes_apro") = colum_order_name
            HttpContext.Current.Session.Item("SortDirection_solicitudes_apro") = order_colum
            HttpContext.Current.Session.Item("Tipo_consulta_solicitudes_apro") = tipo_consulta
            HttpContext.Current.Session.Item("Tipo_dato_solicitudes_apro") = sql_consulta
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_solicitudes_aprobacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_usuarios_relacionados_a_solictud_de_aprobacion_pendiente = "Error listando  solicitudes de aprobación " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                'HiddenEmailconsulta.value = ""
                reflabel.Text = "Se encontraron 0 registro(s) "
                grediview.DataSource = Nothing
                'grediview.DataKeyNames = DataKey
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                Lista_usuarios_relacionados_a_solictud_de_aprobacion_pendiente = "YES"
                Exit Function
            Else
                'HiddenEmailconsulta.value = sql_consulta
                reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) "
                'grediview.DataKeyNames = DataKey
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(3).Text.ToString())
                    grediview.Rows(i).Attributes.Add("id_radicado", grediview.Rows(i).Cells(6).Text.ToString())
                    Dim imaga_buton As New HtmlInputImage
                    imaga_buton.Attributes.Add("Class", "image_buton_clik_image")
                    imaga_buton.Attributes.Add("onclick", "prevent(event,this);")
                    imaga_buton.Attributes.Add("title", "Completar gestión")
                    imaga_buton.Src = "../workflow/imageneswf/hand-point-down-light.png"
                    imaga_buton.Attributes.Add("idd", grediview.Rows(i).Cells(3).Text.ToString())
                    imaga_buton.Attributes.Add("id_radicado_", grediview.Rows(i).Cells(6).Text.ToString())
                    imaga_buton.Attributes.Add("tip_event", "comp_gestion_solic")
                    grediview.Rows(i).Cells(0).Controls.Add(imaga_buton)
                    imaga_buton = New HtmlInputImage
                    imaga_buton.Attributes.Add("Class", "image_buton_clik_image_")
                    imaga_buton.Attributes.Add("onclick", "prevent(event,this);")
                    imaga_buton.Attributes.Add("title", "Ver documentos del peticionario o solicitante")
                    imaga_buton.Src = "../workflow/imageneswf/images-light.png"
                    imaga_buton.Attributes.Add("idd", grediview.Rows(i).Cells(3).Text.ToString())
                    imaga_buton.Attributes.Add("id_radicado_", grediview.Rows(i).Cells(6).Text.ToString())
                    imaga_buton.Attributes.Add("tip_event", "ver_doc_solic")
                    grediview.Rows(i).Cells(0).Controls.Add(imaga_buton)
                    imaga_buton = New HtmlInputImage
                    imaga_buton.Attributes.Add("Class", "image_buton_clik_image_")
                    imaga_buton.Attributes.Add("onclick", "prevent(event,this);")
                    imaga_buton.Attributes.Add("title", "Descargar documento de respuesta a aprobar")
                    imaga_buton.Src = "../workflow/imageneswf/download-light.png"
                    imaga_buton.Attributes.Add("idd", grediview.Rows(i).Cells(3).Text.ToString())
                    imaga_buton.Attributes.Add("id_radicado_", grediview.Rows(i).Cells(6).Text.ToString())
                    imaga_buton.Attributes.Add("tip_event", "descargar_doc_resp_solic")
                    grediview.Rows(i).Cells(0).Controls.Add(imaga_buton)
                    imaga_buton = New HtmlInputImage
                    imaga_buton.Attributes.Add("Class", "image_buton_clik_image_")
                    imaga_buton.Attributes.Add("onclick", "prevent(event,this);")
                    imaga_buton.Attributes.Add("title", "Gestión solicitudes de aprobación")
                    imaga_buton.Src = "../workflow/imageneswf/check-light.png"
                    imaga_buton.Attributes.Add("idd", grediview.Rows(i).Cells(3).Text.ToString())
                    imaga_buton.Attributes.Add("id_radicado_", grediview.Rows(i).Cells(6).Text.ToString())
                    imaga_buton.Attributes.Add("tip_event", "gest_solic_aprobacion")
                    grediview.Rows(i).Cells(0).Controls.Add(imaga_buton)
                    imaga_buton = New HtmlInputImage
                    imaga_buton.Attributes.Add("Class", "image_buton_clik_image_")
                    imaga_buton.Attributes.Add("onclick", "prevent(event,this);")
                    imaga_buton.Attributes.Add("title", "Ver notas se la solicitud")
                    imaga_buton.Src = "../workflow/imageneswf/notas-regular.png"
                    imaga_buton.Attributes.Add("idd", grediview.Rows(i).Cells(3).Text.ToString())
                    imaga_buton.Attributes.Add("id_radicado_", grediview.Rows(i).Cells(6).Text.ToString())
                    imaga_buton.Attributes.Add("tip_event", "ver_not_solic_aprobacion")
                    grediview.Rows(i).Cells(0).Controls.Add(imaga_buton)
                    imaga_buton = New HtmlInputImage
                    imaga_buton.Attributes.Add("Class", "image_buton_clik_image_")
                    imaga_buton.Attributes.Add("onclick", "prevent(event,this);")
                    imaga_buton.Attributes.Add("title", "Ver usarios relacionados a la solicitud")
                    imaga_buton.Src = "../workflow/imageneswf/user-solid.png"
                    imaga_buton.Attributes.Add("idd", grediview.Rows(i).Cells(3).Text.ToString())
                    imaga_buton.Attributes.Add("id_radicado_", grediview.Rows(i).Cells(6).Text.ToString())
                    imaga_buton.Attributes.Add("tip_event", "ver_user_rel_sol")
                    grediview.Rows(i).Cells(0).Controls.Add(imaga_buton)
                    For z As Integer = 0 To grediview.Rows(i).Cells.Count - 1
                        grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_corte_tr")
                        grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                    Next
                Next
                Dim Refclas As New ClassGredview
                Result = Refclas.add_clase_acender_decender(colum_order_name, _
                                                            HttpContext.Current.Session.Item("Sort_matri_colum_solicitudes_apro"), _
                                                            order_colum, _
                                                            grediview)
                If Result <> "YES" Then
                    Lista_usuarios_relacionados_a_solictud_de_aprobacion_pendiente = "Error add clase funcion  Lista_solictudes_compartidos_de_un_usuario " & Result
                    Exit Function
                End If
                Lista_usuarios_relacionados_a_solictud_de_aprobacion_pendiente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_usuarios_relacionados_a_solictud_de_aprobacion_pendiente = "Inconsistencia general función Lista_usuarios_relacionados_a_solictud_de_aprobacion_pendiente " & ex.Message
        End Try

    End Function
    Function Lista_solictudes_de_aprobacion_de_un_usuario(ByVal id_usuario As Integer, _
                                                          ByRef grediview As GridView, _
                                                          ByRef HiddenEmailconsulta As Object, _
                                                          ByRef reflabel As Label, _
                                                          ByRef hideselecion As Object, _
                                                          ByRef update As UpdatePanel, _
                                                          ByVal estado_solicitud As String, _
                                                          ByVal tipo_consulta As Integer, _
                                                          ByVal valor_consulta As String, _
                                                          ByRef colum_order_name As String, _
                                                          ByRef order_colum As String, _
                                                          ByRef UpdatePanel_estado_listado_solicitud As UpdatePanel) As String
        Try
            Dim estado_ As Integer = 0
            If estado_solicitud = "Solicitudes Pendientes por responder" Then
                estado_ = 0
            End If
            If estado_solicitud = "Solicitudes Aprobadas" Then
                estado_ = 1
            End If
            If estado_solicitud = "Solicitudes Desaprobadas" Then
                estado_ = 2
            End If
            If estado_solicitud = "Solicitudes Archivadas" Then
                estado_ = 3
            End If
            Dim sql_consulta As String = ""
            If tipo_consulta = 1 Then
                sql_consulta = "SELECT ID_CD_USUARIOS_SOLICITUDES_APROBACION,ESTADO_PRIORIDAD,rcs.ESTADO_VISTO_SOLICITANTE,rcs.FECHA_LIMITE_RESPUESTA as FECHA_LIMITE," & _
                   "rcs.DESCRIPCION_ESTADO_RESPUESTA AS ESTADO,rdi.Nombre_Remitente as SOLICITANTE ,rdi.Cargo_Remite as CARGO_SOLICITANTE,rcsd.ID_SOLICITUDES_APROBACION " & _
                   " AS SOLICITUD,rrr.RADICADO as RADICADO,rrr.DESTINATARIO as PETICIONARIO, rcs.FECHA_REGISTRO_SOLICITUD as FECHA from ra_cd_usuarios_solicitudes_aprobacion AS rcs " & _
                   " INNER JOIN ra_cd_solicitudes_aprobacion AS rcsd on (rcsd.ID_SOLICITUDES_APROBACION=rcs.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION)" & _
                   " INNER JOIN ra_respuesta_radicado AS rrr on (rrr.ID_RESPUESTA_RADICADO=rcsd.Ra_Respuesta_Radicado_ID_RESPUESTA_RADICADO)" & _
                   " INNER JOIN remit_dest_interno AS rdi on (rdi.id_Remit_Dest_Int=rcsd.Remit_Dest_Interno_id_remit_dest_Int)  where rcs.Remit_Dest_Interno_id_remit_dest_Int=" & id_usuario & _
                   " and rcs.ESTADO_RESPUESTA_SOLICITUD=" & estado_ & " order by " & colum_order_name & " " & order_colum
            Else
                sql_consulta = "SELECT ID_CD_USUARIOS_SOLICITUDES_APROBACION,ESTADO_PRIORIDAD,rcs.ESTADO_VISTO_SOLICITANTE,rcs.FECHA_LIMITE_RESPUESTA as FECHA_LIMITE," & _
                   "rcs.DESCRIPCION_ESTADO_RESPUESTA AS ESTADO,rdi.Nombre_Remitente as SOLICITANTE ,rdi.Cargo_Remite as CARGO_SOLICITANTE,rcsd.ID_SOLICITUDES_APROBACION " & _
                   " AS SOLICITUD,rrr.RADICADO as RADICADO,rrr.DESTINATARIO as PETICIONARIO, rcs.FECHA_REGISTRO_SOLICITUD as FECHA from ra_cd_usuarios_solicitudes_aprobacion AS rcs " & _
                   " INNER JOIN ra_cd_solicitudes_aprobacion AS rcsd on (rcsd.ID_SOLICITUDES_APROBACION=rcs.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION)" & _
                   " INNER JOIN ra_respuesta_radicado AS rrr on (rrr.ID_RESPUESTA_RADICADO=rcsd.Ra_Respuesta_Radicado_ID_RESPUESTA_RADICADO)" & _
                   " INNER JOIN remit_dest_interno AS rdi on (rdi.id_Remit_Dest_Int=rcsd.Remit_Dest_Interno_id_remit_dest_Int)  where (" & _
                   "  rcs.DESCRIPCION_ESTADO_RESPUESTA like '%" & valor_consulta & "%'" & _
                   " or rrr.RADICADO like '%" & valor_consulta & "%'" & _
                    " or rrr.DESTINATARIO like '%" & valor_consulta & "%'" & _
                    " or  rdi.Nombre_Remitente like '%" & valor_consulta & "%'" & _
                    " or rdi.Cargo_Remite like '%" & valor_consulta & "%'" & _
                    " or rcs.FECHA_LIMITE_RESPUESTA like '%" & valor_consulta & "%'" & _
                    " or rcs.FECHA_REGISTRO_SOLICITUD like '%" & valor_consulta & "%'" & _
                   " ) and rcs.Remit_Dest_Interno_id_remit_dest_Int=" & id_usuario & " order by " & colum_order_name & " " & order_colum
            End If
            HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_SOLICITUD_APROBACION") = tipo_consulta
            HttpContext.Current.Session.Item("GA_DATO_CONSULTA_SOLICITUD_APROBACION") = valor_consulta
            HttpContext.Current.Session.Item("Sort_matri_colum_aprobacion") = {"OPCIONES", "ID_CD_USUARIOS_SOLICITUDES_APROBACION", _
                                                                               "ESTADO_PRIORIDAD", "rcs.ESTADO_VISTO_SOLICITANTE", _
                                                                               "FECHA_LIMITE", "ESTADO", "SOLICITANTE", _
                                                                               "CARGO_SOLICITANTE", "SOLICITUD", "RADICADO", "PETICIONARIO", "FECHA"}
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_solicitudes_aprobacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_solictudes_de_aprobacion_de_un_usuario = "Error listando solicitudes relacionadas a un usuario " & Result
                Exit Function
            End If
            Dim datakey() As String = {"ID_CD_USUARIOS_SOLICITUDES_APROBACION"}
            If Datset.Tables(0).Rows.Count = 0 Then
                'HiddenEmailconsulta.value = sql_consulta
                reflabel.Text = "Se encontraron 0 registro(s) "
                grediview.DataSource = Datset
                grediview.DataKeyNames = datakey
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                UpdatePanel_estado_listado_solicitud.Update()
                Lista_solictudes_de_aprobacion_de_un_usuario = "YES"
                Exit Function
            Else
                'HiddenEmailconsulta.value = sql_consulta
                reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) "
                grediview.DataKeyNames = datakey
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                UpdatePanel_estado_listado_solicitud.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    grediview.Rows(i).Attributes.Add("id_estado", grediview.Rows(i).Cells(2).Text.ToString())
                    grediview.Rows(i).Attributes.Add("id_estado_visto", grediview.Rows(i).Cells(3).Text.ToString())
                    grediview.Rows(i).Attributes.Add("id_solicitud", grediview.Rows(i).Cells(8).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-folder-open fa-lg")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-primary btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Ver documentos de la solicitud")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("id_solicitud", grediview.Rows(i).Cells(8).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "ver_doc_sol")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-long-arrow-down fa-lg ")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn   btn-info btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Descargar documento respuesta")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("id_solicitud", grediview.Rows(i).Cells(8).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "desc_doc_resp")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-check fa-lg ")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-info btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Decisión documento respuesta")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("id_solicitud", grediview.Rows(i).Cells(8).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "desicion_doc_resp")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    divhtml.Style.Add("display", "inline-flex")
                    grediview.Rows(i).Cells(0).Controls.Add(divhtml)
                    For z As Integer = 0 To grediview.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")

                        End If

                    Next


                Next
                Dim Refclas As New ClassGredview
                Result = Refclas.add_clase_acender_decender(colum_order_name, _
                                                            HttpContext.Current.Session.Item("Sort_matri_colum_aprobacion"), _
                                                            order_colum, _
                                                            grediview)
                If Result <> "YES" Then
                    Lista_solictudes_de_aprobacion_de_un_usuario = "Error add clase funcion  Lista_solictudes_de_aprobacion_de_un_usuario" & Result
                    Exit Function
                End If
                Lista_solictudes_de_aprobacion_de_un_usuario = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_solictudes_de_aprobacion_de_un_usuario = "Inconsistencia general función Lista_solictudes_de_aprobacion_de_un_usuario " & ex.Message
        End Try

    End Function

    Function Envio_correo_electronico_respuesta_solicitud_aprobacion(ByVal correos_relacionados As String, ByVal nota_solicitud As String, _
                                                           ByVal fecha_solicitud As String, ByVal numero_solictud As Integer, _
                                                           ByVal descripcion_tipo_aprobacion As String, ByVal id_usuario_gestion As Integer) As String
        '----------------------------------------------------------
        'Función : Envía correo electrónico usuario de solicitud
        'Fecha : 2016-02-20
        'Ing Miguel Angel Urueta Miranda 
        '----------------------------------------------------------
        Try
            Dim refclas_gestion As New Classgestionrespuesta
            Dim Result As String = ""
            Dim nombre_area As String = ""
            Dim id_area As Integer = 0
            Dim nombre_usuario As String = ""
            Dim cargo_usuario As String = ""
            Result = refclas_gestion.Retorna_datos_asignacion_respuesta_usuario_gestion(id_usuario_gestion, id_area, nombre_area, nombre_usuario, cargo_usuario)
            If Result <> "YES" Then
                Envio_correo_electronico_respuesta_solicitud_aprobacion = Result
                Exit Function
            End If
            Dim spli_texto_correo() As String = {descripcion_tipo_aprobacion & " la solicitud de aprobación número " & numero_solictud, _
            "Fecha límite de respuesta " & fecha_solicitud, "Nota Solicitud ", nota_solicitud, _
            "Nombre solicitante : " & nombre_usuario, "Cargo Solicitante : " & cargo_usuario, "Area Solicitante : " & nombre_area, "Por favor ingrese al sistema de gestión documental a la opción (Notificaciones y solicitudes - solicitudes de aprobación) y revise la solicitud"}
            Dim Refclas As New ClassCorreo
            Dim matri_documentos() As String = Nothing
            Result = Refclas.Envio_Correo_confirmacion_solicitud_aprobacion_respuesta(spli_texto_correo, _
                                                                                      correos_relacionados, _
                                                                                      descripcion_tipo_aprobacion & _
                                                                                      " solicitud de aprobación de documento de respuesta número " & _
                                                                                      numero_solictud, _
                                                                                      matri_documentos)
            If Result <> "YES" Then
                Envio_correo_electronico_respuesta_solicitud_aprobacion = Result
                Exit Function
            Else
                Envio_correo_electronico_respuesta_solicitud_aprobacion = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Envio_correo_electronico_respuesta_solicitud_aprobacion = "Inconsistencia general función Envio_correo_electronico_respuesta_solicitud_aprobacion " & ex.Message
        End Try
    End Function

    Function Envio_correo_electronico_usuarios_solicitados(ByVal correos_relacionados As String, _
                                                           ByVal nota_solicitud As String, _
                                                           ByVal fecha_solicitud As String, _
                                                           ByVal numero_solictud As Integer, _
                                                           ByVal numero_radicado As String, _
                                                           ByVal id_imagen As Integer) As String
        '----------------------------------------------------------
        'Función : Envía correo electrónico usuario de solicitud
        'Fecha : 2016-02-20
        'Ing Miguel Angel Urueta Miranda 
        '----------------------------------------------------------
        Try
            Dim refclas_gestion As New Classgestionrespuesta
            Dim Result As String = ""
            Dim nombre_area As String = ""
            Dim id_area As Integer = 0
            Dim nombre_usuario As String = ""
            Dim cargo_usuario As String = ""
            Result = refclas_gestion.Retorna_datos_asignacion_respuesta_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                        id_area, _
                                                                                        nombre_area, _
                                                                                        nombre_usuario, _
                                                                                        cargo_usuario)
            If Result <> "YES" Then
                Envio_correo_electronico_usuarios_solicitados = Result
                Exit Function
            End If
            Dim matri_documentos_almacenados() As String = Nothing
            Dim ref_almacenaminento As New ClassVisualisaDocumento
            If id_imagen <> 0 Then
                Result = ref_almacenaminento.Genera_Matris_Documentos_Almacenados(id_imagen, _
                                                                                  "IMP03GESTIONTMP", _
                                                                                   matri_documentos_almacenados)
                If Result <> "YES" Then
                    Envio_correo_electronico_usuarios_solicitados = Result
                    Exit Function
                End If
            End If
            Dim spli_texto_correo() As String = {"Solicitud aprobación Número : " & numero_solictud & " radicado : " & numero_radicado, " Con el siguiente correo electrónico le estamos notificando la asignación de una solicitud de aprobación de un documento de respuesta. ", _
            "Fecha límite de respuesta " & fecha_solicitud, " Nota Solicitud ", nota_solicitud, _
            "Nombre solicitante : " & nombre_usuario, "Cargo Solicitante : " & cargo_usuario, "Area Solicitante : " & nombre_area, "Por favor ingrese al sistema de gestión documental a la opción (Notificaciones y solicitudes - solicitudes de aprobación) y revise la solicitud"}
            Dim Refclas As New ClassCorreo
            Result = Refclas.Envio_Correo_confirmacion_solicitud_aprobacion_respuesta(spli_texto_correo, _
                                                                                      correos_relacionados, _
                                                                                      "Solicitud de aprobación número " _
                                                                                      & numero_solictud & " radicado : " & numero_radicado, _
                                                                                      matri_documentos_almacenados)
            If Result <> "YES" Then
                Envio_correo_electronico_usuarios_solicitados = Result
                Exit Function
            Else
                Envio_correo_electronico_usuarios_solicitados = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Envio_correo_electronico_usuarios_solicitados = "Inconsistencia general función Envio_correo_electronico_usuarios_solicitados " & ex.Message
        End Try
    End Function

    Function Retorna_datos_solictud_aprobacion_usuarios(ByVal id_solictud_aprobacion As Integer, _
                                                        ByRef stru As STRU_SOLICITUD_APROBACION_USUARIOS) As String
        Try
            Dim sql_consulta As String = "SELECT Remit_Dest_Interno_id_remit_dest_Int," & _
                     "RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION,FECHA_REGISTRO_SOLICITUD,FECHA_RESPUESTA_SOLICITUD," _
                    & "ESTADO_RESPUESTA_SOLICITUD,TIEMPO_RESPUESTA_SOLICITUD,ESTADO_VISTO_SOLICITANTE,DESCRIPCION_ESTADO_RESPUESTA,FECHA_LIMITE_RESPUESTA from ra_cd_usuarios_solicitudes_aprobacion where ID_CD_USUARIOS_SOLICITUDES_APROBACION=" & id_solictud_aprobacion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_solicitudes_aprobacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_datos_solictud_aprobacion_usuarios = "Error listando datos solicitudes de aprobación " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_datos_solictud_aprobacion_usuarios = "Imposible encontrar datos para la solicitud de aprobación número " & id_solictud_aprobacion
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    stru.Remit_Dest_Interno_id_remit_dest_Int = 0
                Else
                    stru.Remit_Dest_Interno_id_remit_dest_Int = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    stru.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION = 0
                Else
                    stru.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    stru.FECHA_REGISTRO_SOLICITUD = ""
                Else
                    stru.FECHA_REGISTRO_SOLICITUD = Datset.Tables(0).Rows(0).Item(2)
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) = True Then
                    stru.FECHA_RESPUESTA_SOLICITUD = ""
                Else
                    stru.FECHA_RESPUESTA_SOLICITUD = Datset.Tables(0).Rows(0).Item(3)
                End If
                If Datset.Tables(0).Rows(0).IsNull(4) = True Then
                    stru.ESTADO_RESPUESTA_SOLICITUD = 0
                Else
                    stru.ESTADO_RESPUESTA_SOLICITUD = Datset.Tables(0).Rows(0).Item(4)
                End If
                If Datset.Tables(0).Rows(0).IsNull(5) = True Then
                    stru.TIEMPO_RESPUESTA_SOLICITUD = 0
                Else
                    stru.TIEMPO_RESPUESTA_SOLICITUD = Datset.Tables(0).Rows(0).Item(5)
                End If
                If Datset.Tables(0).Rows(0).IsNull(6) = True Then
                    stru.ESTADO_VISTO_SOLICITANTE = 0
                Else
                    stru.ESTADO_VISTO_SOLICITANTE = Datset.Tables(0).Rows(0).Item(6)
                End If
                If Datset.Tables(0).Rows(0).IsNull(7) = True Then
                    stru.DESCRIPCION_ESTADO_RESPUESTA = ""
                Else
                    stru.DESCRIPCION_ESTADO_RESPUESTA = Datset.Tables(0).Rows(0).Item(7)
                End If
                If Datset.Tables(0).Rows(0).IsNull(8) = True Then
                    stru.FECHA_LIMITE_RESPUESTA = ""
                Else
                    stru.FECHA_LIMITE_RESPUESTA = Datset.Tables(0).Rows(0).Item(8)
                End If
                Retorna_datos_solictud_aprobacion_usuarios = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_datos_solictud_aprobacion_usuarios = "Inconsistencia general función Retorna_datos_solictud_aprobacion_usuarios " & ex.Message
        End Try
    End Function
    Function Retorna_datos_solictud_aprobacion_usuarios_por_id_solicitud(ByVal id_solicitud_aprobacion As Integer, ByRef stru() As STRU_SOLICITUD_APROBACION_USUARIOS) As String
        Try
            Dim sql_consulta As String = "SELECT Remit_Dest_Interno_id_remit_dest_Int," & _
                     "RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION,FECHA_REGISTRO_SOLICITUD,FECHA_RESPUESTA_SOLICITUD," _
                    & "ESTADO_RESPUESTA_SOLICITUD,TIEMPO_RESPUESTA_SOLICITUD,ESTADO_VISTO_SOLICITANTE,DESCRIPCION_ESTADO_RESPUESTA,FECHA_LIMITE_RESPUESTA from ra_cd_usuarios_solicitudes_aprobacion where RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION=" & id_solicitud_aprobacion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_solicitudes_aprobacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_datos_solictud_aprobacion_usuarios_por_id_solicitud = "Error listando datos solicitudes de aprobación " & Result
                Exit Function
            End If

            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_datos_solictud_aprobacion_usuarios_por_id_solicitud = "Imposible encontrar datos para la solicitud de aprobación número " & id_solicitud_aprobacion
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru(i)
                    If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                        stru(i).Remit_Dest_Interno_id_remit_dest_Int = 0
                    Else
                        stru(i).Remit_Dest_Interno_id_remit_dest_Int = Datset.Tables(0).Rows(0).Item(0)
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                        stru(i).RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION = 0
                    Else
                        stru(i).RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION = Datset.Tables(0).Rows(0).Item(1)
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                        stru(i).FECHA_REGISTRO_SOLICITUD = ""
                    Else
                        stru(i).FECHA_REGISTRO_SOLICITUD = Datset.Tables(0).Rows(0).Item(2)
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(3) = True Then
                        stru(i).FECHA_RESPUESTA_SOLICITUD = ""
                    Else
                        stru(i).FECHA_RESPUESTA_SOLICITUD = Datset.Tables(0).Rows(0).Item(3)
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(4) = True Then
                        stru(i).ESTADO_RESPUESTA_SOLICITUD = 0
                    Else
                        stru(i).ESTADO_RESPUESTA_SOLICITUD = Datset.Tables(0).Rows(0).Item(4)
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(5) = True Then
                        stru(i).TIEMPO_RESPUESTA_SOLICITUD = 0
                    Else
                        stru(i).TIEMPO_RESPUESTA_SOLICITUD = Datset.Tables(0).Rows(0).Item(5)
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(6) = True Then
                        stru(i).ESTADO_VISTO_SOLICITANTE = 0
                    Else
                        stru(i).ESTADO_VISTO_SOLICITANTE = Datset.Tables(0).Rows(0).Item(6)
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(7) = True Then
                        stru(i).DESCRIPCION_ESTADO_RESPUESTA = ""
                    Else
                        stru(i).DESCRIPCION_ESTADO_RESPUESTA = Datset.Tables(0).Rows(0).Item(7)
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(8) = True Then
                        stru(i).FECHA_LIMITE_RESPUESTA = ""
                    Else
                        stru(i).FECHA_LIMITE_RESPUESTA = Datset.Tables(0).Rows(0).Item(8)
                    End If
                Next
                Retorna_datos_solictud_aprobacion_usuarios_por_id_solicitud = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_datos_solictud_aprobacion_usuarios_por_id_solicitud = "Inconsistencia general función Retorna_datos_solictud_aprobacion_usuarios_por_id_solicitud " & ex.Message
        End Try
    End Function
    Function Retorna_datos_solicitud_aprobación_documentos(ByVal id_solictud_aprobacion As Integer, ByRef stru As STRU_SOLICITUD_ARPBACION) As String
        Try
            Dim sql_consulta As String = "SELECT Ra_Respuesta_Radicado_ID_RESPUESTA_RADICADO," & _
                     "Remit_Dest_Interno_id_remit_dest_Int,FECHA_REGISTRO_SOLICITUD,FECHA_REGISTRO_APROBACION," _
                    & "ESTADO_APROBACION,TIEMPO_RESPUESTA_APROBACION,ESTADO_PRIORIDAD,NOTA_SOLICITUD,DESCRIPCION_ESTADO_APROBACION,FECHA_LIMITE_RESPUESTA from ra_cd_solicitudes_aprobacion where ID_SOLICITUDES_APROBACION=" & id_solictud_aprobacion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_solicitudes_aprobacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_datos_solicitud_aprobación_documentos = "Error listando datos solicitudes de aprobación " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_datos_solicitud_aprobación_documentos = "Imposible encontrar datos para la solicitud de aprobación número " & id_solictud_aprobacion
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    stru.Ra_Respuesta_Radicado_ID_RESPUESTA_RADICAD = 0
                Else
                    stru.Ra_Respuesta_Radicado_ID_RESPUESTA_RADICAD = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    stru.Remit_Dest_Interno_id_remit_dest_Int = 0
                Else
                    stru.Remit_Dest_Interno_id_remit_dest_Int = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    stru.FECHA_REGISTRO_SOLICITUD = ""
                Else
                    stru.FECHA_REGISTRO_SOLICITUD = Datset.Tables(0).Rows(0).Item(2)
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) = True Then
                    stru.FECHA_REGISTRO_APROBACION = ""
                Else
                    stru.FECHA_REGISTRO_APROBACION = Datset.Tables(0).Rows(0).Item(3)
                End If
                If Datset.Tables(0).Rows(0).IsNull(4) = True Then
                    stru.ESTADO_APROBACION = 0
                Else
                    stru.ESTADO_APROBACION = Datset.Tables(0).Rows(0).Item(4)
                End If
                If Datset.Tables(0).Rows(0).IsNull(5) = True Then
                    stru.TIEMPO_RESPUESTA_APROBACION = 0
                Else
                    stru.TIEMPO_RESPUESTA_APROBACION = Datset.Tables(0).Rows(0).Item(5)
                End If

                If Datset.Tables(0).Rows(0).IsNull(6) = True Then
                    stru.ESTADO_PRIORIDAD = 0
                Else
                    stru.ESTADO_PRIORIDAD = Datset.Tables(0).Rows(0).Item(6)
                End If
                If Datset.Tables(0).Rows(0).IsNull(7) = True Then
                    stru.NOTA_SOLICITUD = ""
                Else
                    stru.NOTA_SOLICITUD = Datset.Tables(0).Rows(0).Item(7)
                End If
                If Datset.Tables(0).Rows(0).IsNull(8) = True Then
                    stru.DESCRIPCION_ESTADO_APROBACION = ""
                Else
                    stru.DESCRIPCION_ESTADO_APROBACION = Datset.Tables(0).Rows(0).Item(8)
                End If
                If Datset.Tables(0).Rows(0).IsNull(9) = True Then
                    stru.FECHA_LIMITE_RESPUESTA = ""
                Else
                    stru.FECHA_LIMITE_RESPUESTA = Datset.Tables(0).Rows(0).Item(9)
                End If
                Retorna_datos_solicitud_aprobación_documentos = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_datos_solicitud_aprobación_documentos = "Inconsistencia general función Retorna_datos_solicitud_aprobación_documentos " & ex.Message
        End Try
    End Function
    Function Retorna_correos_usuarios_solicitud_aprobacion(ByVal id_solicitud_aprobacion As Integer, _
                                                           ByRef correos_relacionados As String) As String
        Try
            Dim sql_consulta As String = "SELECT rdi.Correo_Electronico " & _
                     " from ra_cd_usuarios_solicitudes_aprobacion as rcu " & _
                     "inner join remit_dest_interno as rdi on (rdi.id_Remit_Dest_Int=rcu.Remit_Dest_Interno_id_remit_dest_Int) where RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION=" & id_solicitud_aprobacion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_solicitudes_aprobacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_correos_usuarios_solicitud_aprobacion = "Error listando correos electrónicos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_correos_usuarios_solicitud_aprobacion = "Imposible encontrar los correos electrónicos de la solicitud " & id_solicitud_aprobacion
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    If Datset.Tables(0).Rows(i).IsNull(0) = True Then
                    Else
                        If correos_relacionados = "" Then
                            correos_relacionados = Datset.Tables(0).Rows(i).Item(0)
                        Else
                            correos_relacionados = correos_relacionados & "," & Datset.Tables(0).Rows(i).Item(0)
                        End If
                    End If
                Next
                Retorna_correos_usuarios_solicitud_aprobacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_correos_usuarios_solicitud_aprobacion = "Inconsistencia general función Retorna_correos_usuarios_solicitud_aprobacion " & ex.Message
        End Try
    End Function
    Function Retorna_correos_usuarios_solicitud_aprobacion_sin_desicion(ByVal id_solicitud_aprobacion As Integer, _
                                                                        ByRef correos_relacionados As String) As String
        Try
            Dim sql_consulta As String = "SELECT rdi.Correo_Electronico " & _
                     " from ra_cd_usuarios_solicitudes_aprobacion as rcu " & _
                     "inner join remit_dest_interno as rdi on (rdi.id_Remit_Dest_Int=rcu.Remit_Dest_Interno_id_remit_dest_Int) " & _
                     "where RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION=" & id_solicitud_aprobacion & _
                     " and rcu.ESTADO_RESPUESTA_SOLICITUD=0"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_solicitudes_aprobacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_correos_usuarios_solicitud_aprobacion_sin_desicion = "Error listando correos electrónicos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_correos_usuarios_solicitud_aprobacion_sin_desicion = "Imposible encontrar los correos electrónicos de la solicitud " & id_solicitud_aprobacion
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    If Datset.Tables(0).Rows(i).IsNull(0) = True Then
                    Else
                        If correos_relacionados = "" Then
                            correos_relacionados = Datset.Tables(0).Rows(i).Item(0)
                        Else
                            correos_relacionados = correos_relacionados & "," & Datset.Tables(0).Rows(i).Item(0)
                        End If
                    End If
                Next
                Retorna_correos_usuarios_solicitud_aprobacion_sin_desicion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_correos_usuarios_solicitud_aprobacion_sin_desicion = "Inconsistencia general función Retorna_correos_usuarios_solicitud_aprobacion_sin_desicion " & ex.Message
        End Try
    End Function
    Function Solicita_solicitud_aprobacion_aprobada(ByVal id_respuesta As Integer, _
                                                    ByRef id_solicitud_aprobacion As Integer) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select ID_SOLICITUDES_APROBACION from ra_cd_solicitudes_aprobacion " & _
                " where Ra_Respuesta_Radicado_ID_RESPUESTA_RADICADO=" & id_respuesta & " and ESTADO_APROBACION=1"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_solicitud_aprobacion_aprobada = " Error Solicita_plantilla_default_respuesta  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_solicitud_aprobacion = 0
                Solicita_solicitud_aprobacion_aprobada = "YES"
                Exit Function
            Else
                id_solicitud_aprobacion = Datset.Tables(0).Rows(0).Item(0)
                Solicita_solicitud_aprobacion_aprobada = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_solicitud_aprobacion_aprobada = "Inconsistencia general función Solicita_solicitud_aprobacion_aprobada " & ex.Message
        End Try
    End Function
    Function Retorna_estados_solicitudes_de_aprobacion_por_id_respuesta(ByVal id_respuesta As Integer, _
                                                                        ByVal estado_inicial As Integer, _
                                                                        ByRef estado_solicitud As String, _
                                                                        ByVal id_usuario_gestion As Integer) As String
        Try
            Dim Result As String = ""
            Dim sql_consulta As String = "Select ESTADO_APROBACION from ra_cd_solicitudes_aprobacion " & _
                " where  Ra_Respuesta_Radicado_ID_RESPUESTA_RADICADO=" & _
                id_respuesta & " and ESTADO_APROBACION=" & estado_inicial
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_solicitudes_aprobacion")
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_estados_solicitudes_de_aprobacion_por_id_respuesta = "Error Función Retorna_estados_solicitudes_de_aprobacion_por_id_tarea_seleccionada dice  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_solicitud = "NO"
                Retorna_estados_solicitudes_de_aprobacion_por_id_respuesta = "YES"
                Exit Function
            Else
                estado_solicitud = "YES"
                Retorna_estados_solicitudes_de_aprobacion_por_id_respuesta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_estados_solicitudes_de_aprobacion_por_id_respuesta = "Inconsistencia general función Retorna_estados_solicitudes_de_aprobacion_por_id_tarea_seleccionada " & ex.Message
        End Try
    End Function
    Function Activa_notificacion_correo_solicitud_aprobacion(ByVal id_solicitud_aprobacion As Integer) As String
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassRaSolicitudesAprobacion
            Dim correos_relacionados As String = ""
            Dim stru As STRU_SOLICITUD_ARPBACION = Nothing
            Result = Refclas.Retorna_datos_solicitud_aprobación_documentos(id_solicitud_aprobacion, _
                                                                           stru)
            If Result <> "YES" Then
                Activa_notificacion_correo_solicitud_aprobacion = Result
                Exit Function
            End If
            If stru.ESTADO_APROBACION <> 0 Then
                Activa_notificacion_correo_solicitud_aprobacion = "La solicitud de aprobación se encuentra en estado (" & stru.DESCRIPCION_ESTADO_APROBACION & ") imposible confirmar a los correos electrónicos"
                Exit Function
            End If
            Result = Refclas.Retorna_correos_usuarios_solicitud_aprobacion_sin_desicion(id_solicitud_aprobacion, _
                                                                                        correos_relacionados)
            If Result <> "YES" Then
                Activa_notificacion_correo_solicitud_aprobacion = Result
                Exit Function
            End If
            Dim Refclas_ra_resp As New Class_ra_respuesta_radicado
            Dim stru_envi As stru_envio = Nothing
            Result = Refclas_ra_resp.Solicita_datos_estructura_envio_por_id_respuesta(stru.Ra_Respuesta_Radicado_ID_RESPUESTA_RADICAD, _
                                                                                      stru_envi)
            If Result <> "YES" Then
                Activa_notificacion_correo_solicitud_aprobacion = Result
                Exit Function
            End If
            Result = Refclas.Envio_correo_electronico_usuarios_solicitados(correos_relacionados, _
                                                                           stru.NOTA_SOLICITUD, _
                                                                           stru.FECHA_LIMITE_RESPUESTA, _
                                                                           id_solicitud_aprobacion, _
                                                                           stru_envi.RADICADO, _
                                                                           stru_envi.ID_IMAGEN)
            If Result <> "YES" Then
                Activa_notificacion_correo_solicitud_aprobacion = Result
                Exit Function
            End If
            Activa_notificacion_correo_solicitud_aprobacion = "YES"
        Catch ex As Exception
            Activa_notificacion_correo_solicitud_aprobacion = "Incosistencia general función Activa_notificacion_correo_solicitud_aprobacion " & ex.Message
        End Try
    End Function

    Function Activa_notificacion_correo_solicitud_aprobacion_usuario(ByVal id_solicitud_aprobacion_usuario As Integer) As String
        Try
            Dim stru As STRU_SOLICITUD_APROBACION_USUARIOS = Nothing
            Dim Result As String = ""
            Dim correos_relacionados As String = ""
            Result = Me.Retorna_datos_solictud_aprobacion_usuarios(id_solicitud_aprobacion_usuario, _
                                                                        stru)
            If Result <> "YES" Then
                Activa_notificacion_correo_solicitud_aprobacion_usuario = Result
                Exit Function
            End If
            If stru.ESTADO_RESPUESTA_SOLICITUD <> 0 Then
                Activa_notificacion_correo_solicitud_aprobacion_usuario = "La solictud de usuario esta en estado (" & stru.DESCRIPCION_ESTADO_RESPUESTA & ") imposible notificar al correo electrónico"
                Exit Function
            End If
            Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
            Result = Reclas_remit_dest_interno.Solicita_correo_usuario_gestion(stru.Remit_Dest_Interno_id_remit_dest_Int, _
                                                                               correos_relacionados)
            If Result <> "YES" Then
                Activa_notificacion_correo_solicitud_aprobacion_usuario = Result
                Exit Function
            End If
            Dim Refclas_ra_resp As New Class_ra_respuesta_radicado
            Dim stru_envi As stru_envio = Nothing
            Result = Refclas_ra_resp.Solicita_datos_estructura_envio_por_id_respuesta(HttpContext.Current.Session.Item("RA_ID_RESPUESTA_SELECCIONADA_SOLICITUD_APROBACION"), _
                                                                                      stru_envi)
            If Result <> "YES" Then
                Activa_notificacion_correo_solicitud_aprobacion_usuario = Result
                Exit Function
            End If
            Result = Me.Envio_correo_electronico_usuarios_solicitados(correos_relacionados, _
                                                                      "Se reitera la solicitud de su aprobación", _
                                                                      stru.FECHA_LIMITE_RESPUESTA, _
                                                                      stru.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION, _
                                                                      stru_envi.RADICADO, _
                                                                      stru_envi.ID_IMAGEN)
            If Result <> "YES" Then
                Activa_notificacion_correo_solicitud_aprobacion_usuario = Result
                Exit Function
            End If
            Activa_notificacion_correo_solicitud_aprobacion_usuario = "YES"
        Catch ex As Exception
            Activa_notificacion_correo_solicitud_aprobacion_usuario = "Inconsistencia general función Activa_notificacion_correo_solicitud_aprobacion_usuario " & ex.Message
        End Try
    End Function
End Class
