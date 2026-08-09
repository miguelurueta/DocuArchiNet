Imports MySql.Data.MySqlClient
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports System.IO
Imports AjaxControlToolkit

Public Structure stru_campos_destinatario
    Dim nombre_campo_fuente As String
    Dim valor_campo_fuente As String
    Dim nombre_campo_destino As String
    Dim valor_campo_destino As String
    Dim campo_identi As Integer
End Structure
Public Structure guia_envio
    Dim Id_guia_envio As Integer
    Dim areas_depart_radicacion_Codigo_Area As Integer
    Dim Remit_Dest_Interno_id_Remit_Dest_Int As Integer
    Dim ra_empresa_envio_ID_EMPRESA_ENVIO As Integer
    Dim Concecutivo_Guia As String
    Dim Fecha_Registro_Guia As String
    Dim NIT_IDENTIFICACION As String
    Dim NOMBRE_RAZON_SOCIAL As String
    Dim DIRECCION As String
    Dim TELEFONO As String
    Dim CORREO_ELECTRONICO As String
    Dim PAIS As String
    Dim DEPARTAMENTO As String
    Dim MUNICIPIO As String
    Dim Destinatario_Ext As Integer
    Dim ESTADO_CONFIRMACION_GUIA As Integer
    Dim FECHA_RECIBIDO_GUIA As String
    Dim NOTA_CLIENTE As String
    Dim RADICADO As String
    Dim ID_MENSAJERO_INTERNO As Integer
    Dim ID_TIPO_GUIA As Integer
    Dim ESTADO_GUIA As Integer
    Dim ANEXO As String
    Dim FECHA_ENVIO_GUIA As String
    Dim TIEMPO_RESPUESTA As Integer
    Dim ID_TIPO_MANUAL_AUTOMATICO As Integer
    Dim ID_USUARIO_GESTION_TRANSAC As Integer
End Structure
Public Class ClassRaEnvioCorrespondencia

    Function genera_documento_guia(ByVal id_guia As Integer _
                                       , ByRef archivo As String) As String
        Dim doc As Document
        Dim writer As PdfWriter
        Try
            Dim ref_clas_rad As New ClassRadicador
            Dim Ruta_Sesion = HttpContext.Current.Session("RA_RUTA_TEMPO_IMPRESION").ToString()
            Dim rutafinal As String = Ruta_Sesion & "\"
            Dim Rutatemp As String = ""
            Rutatemp = rutafinal & "TEMPREMISION" & "\"
            If Directory.Exists(Rutatemp) = False Then
                Directory.CreateDirectory(Rutatemp)
            End If
            Dim archivo_pdf As String = Rutatemp & "temp_" & "RA" & HttpContext.Current.Session.SessionID & ".pdf"
            doc = New Document(New Rectangle(792, 1200))
            doc.SetMargins(1.0F, 1.0F, 0.0F, 0.0F)
            'doc.SetPageSize(PageSize.Rotate())
            If archivo = "" Then
                writer = PdfWriter.GetInstance(doc, _
                               New FileStream(archivo_pdf, FileMode.Create))
                writer.AddViewerPreference(PdfName.PICKTRAYBYPDFSIZE, PdfBoolean.PDFTRUE)
            Else
                writer = PdfWriter.GetInstance(doc, _
                              New FileStream(archivo_pdf, FileMode.Append))
                writer.AddViewerPreference(PdfName.PICKTRAYBYPDFSIZE, PdfBoolean.PDFTRUE)
            End If
            Dim nombre_usuario As String = ""
            Dim cargo_usario As String = ""
            Dim Result As String = ""
            Dim strucs As guia_envio = Nothing
            Result = Me.Retorna_datos_estructura_guia(id_guia, strucs)
            If Result <> "YES" Then
                genera_documento_guia = Result
                Exit Function
            End If
            Dim refclas As New ClassRaConsultaRadicados
            Dim _plantilla() As String = Nothing
            Result = refclas.Lista_Nombre_Entidad(_plantilla)
            If Result <> "YES" Then
                genera_documento_guia = Result
                Exit Function
            End If
            Dim refconsulta As New ClassRaConsultaRadicados
            Result = refconsulta.Retorna_detalle_usuario_gestion(strucs.Remit_Dest_Interno_id_Remit_Dest_Int, nombre_usuario, cargo_usario)
            If Result <> "YES" Then
                genera_documento_guia = Result
                Exit Function
            End If
            Dim refclasgestor As New ClassGestorDocumental
            Dim nombre_operario_interno As String = ""
            If strucs.ID_MENSAJERO_INTERNO <> 0 Then
                Result = refclasgestor.Retorna_nombre_id_usuario_gestion(strucs.ID_MENSAJERO_INTERNO, nombre_operario_interno)
                If Result <> "YES" Then
                    genera_documento_guia = Result
                    Exit Function
                End If
            End If
            Dim nombre_operador_externo As String = ""
            Result = Me.retorna_nombre_empresa_mensajeria(strucs.ra_empresa_envio_ID_EMPRESA_ENVIO, nombre_operador_externo)
            If Result <> "YES" Then
                genera_documento_guia = Result
                Exit Function
            End If
            doc.Open()
            Dim _standardFont As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, _
               10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
            Dim _standardFont8 As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, _
               8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
            Dim _standardFont7 As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, _
              10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
            Dim _standardFont_tranps As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, _
               10, iTextSharp.text.Font.NORMAL, BaseColor.YELLOW)
            Dim tblrdatos As PdfPTable = New PdfPTable(2)
            'heigconetedor = $("#Contenedorderecho").height() - (($("#Contenedorderecho").height() * 96) / 100);
            Dim i1 As Single = doc.PageSize.Width - (doc.PageSize.Width * 80 / 100)
            Dim i2 As Single = doc.PageSize.Width - (doc.PageSize.Width * 20 / 100)
            tblrdatos.SetWidthPercentage(New Single() {i1, i2}, New Rectangle(doc.PageSize.Width, doc.PageSize.Height))
            Dim ruta_image As String = HttpContext.Current.Server.MapPath("../imagera/logo_trd.png")
            Dim imagen As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(ruta_image)
            imagen.BorderWidth = 0
            imagen.Alignment = Element.ALIGN_CENTER
            Dim percentage As Object = 0.0F
            percentage = 100 / imagen.Width
            imagen.ScalePercent(percentage * 40)
            Dim cltilogo As PdfPCell = New PdfPCell(imagen)
            cltilogo.HorizontalAlignment = 1
            cltilogo.Border = 0
            Dim tlbrempresa As PdfPTable = New PdfPTable(1)
            tlbrempresa.WidthPercentage = 30
            Dim celempresa As PdfPCell = New PdfPCell(New Paragraph(_plantilla(0) & vbCrLf & _plantilla(1), _standardFont7))
            celempresa.Border = 0
            celempresa.HorizontalAlignment = 1
            tlbrempresa.AddCell(cltilogo)
            tlbrempresa.AddCell(celempresa)
            tblrdatos.AddCell(tlbrempresa)
            '-----------------------------------------
            'tabla para detalle de la guia
            '-----------------------------------------
            Dim descripcion_tipo_guia As String = ""
            If strucs.ID_TIPO_MANUAL_AUTOMATICO = 2 Then
                descripcion_tipo_guia = " (Tipo guía : Manual) "
            Else
                descripcion_tipo_guia = " (Tipo guía : Automatica)"
            End If
            Dim tblrdatos_guia As PdfPTable = New PdfPTable(2)
            Dim codigo_unico_guia_title As PdfPCell = New PdfPCell(New Paragraph("Código único guía", _standardFont7))
            Dim codigo_unico_guia As PdfPCell = New PdfPCell(New Paragraph(strucs.Id_guia_envio & descripcion_tipo_guia, _standardFont7))
            tblrdatos_guia.AddCell(codigo_unico_guia_title)
            tblrdatos_guia.AddCell(codigo_unico_guia)
            Dim consecutivo_guia_title As PdfPCell = New PdfPCell(New Paragraph("Consecutivo guía", _standardFont7))
            Dim consecutivo_guia As PdfPCell = New PdfPCell(New Paragraph(strucs.Concecutivo_Guia, _standardFont7))
            tblrdatos_guia.AddCell(consecutivo_guia_title)
            tblrdatos_guia.AddCell(consecutivo_guia)
            Dim fecha_guia_title As PdfPCell = New PdfPCell(New Paragraph("Fecha guía", _standardFont7))
            Dim fecha_guia As PdfPCell = New PdfPCell(New Paragraph(strucs.Fecha_Registro_Guia, _standardFont7))
            tblrdatos_guia.AddCell(fecha_guia_title)
            tblrdatos_guia.AddCell(fecha_guia)
            Dim operador_guia_title As PdfPCell = New PdfPCell(New Paragraph("Mensajero que entrega la guía", _standardFont7))
            Dim operador_guia As PdfPCell = New PdfPCell(New Paragraph(UCase(nombre_operario_interno), _standardFont7))
            tblrdatos_guia.AddCell(operador_guia_title)
            tblrdatos_guia.AddCell(operador_guia)
            Dim radicado_guia_title As PdfPCell = New PdfPCell(New Paragraph("Operador Mensajeria", _standardFont7))
            Dim radicado_guia As PdfPCell = New PdfPCell(New Paragraph(UCase(nombre_operador_externo), _standardFont7))
            tblrdatos_guia.AddCell(radicado_guia_title)
            tblrdatos_guia.AddCell(radicado_guia)
            Dim radicado_rad_title As PdfPCell = New PdfPCell(New Paragraph("Radicado relacionado", _standardFont7))
            Dim radicado_rad As PdfPCell = New PdfPCell(New Paragraph(UCase(strucs.RADICADO), _standardFont7))
            tblrdatos_guia.AddCell(radicado_rad_title)
            tblrdatos_guia.AddCell(radicado_rad)
            Dim paragraf As New Paragraph
            paragraf = New Paragraph("", _standardFont7)
            paragraf.Alignment = Element.ALIGN_CENTER
            doc.Add(paragraf)
            tblrdatos.AddCell(tblrdatos_guia)
            doc.Add(tblrdatos)
            Dim tblrdatos_detalle As PdfPTable = New PdfPTable(2)
            Dim i3 As Single = doc.PageSize.Width - (doc.PageSize.Width * 50 / 100)
            Dim i4 As Single = doc.PageSize.Width - (doc.PageSize.Width * 50 / 100)
            tblrdatos_detalle.SetWidthPercentage(New Single() {i3, i4}, New Rectangle(doc.PageSize.Width, doc.PageSize.Height))
            Dim celremite As PdfPCell = New PdfPCell(New Paragraph("REMITENTE", _standardFont))
            celremite.HorizontalAlignment = 1
            Dim celdestinatario As PdfPCell = New PdfPCell(New Paragraph("DESTINATARIO", _standardFont7))
            celdestinatario.HorizontalAlignment = 1
            tblrdatos_detalle.AddCell(celremite)
            tblrdatos_detalle.AddCell(celdestinatario)
            '------------------------------------------------------
            'Asigna datos del remitente
            '------------------------------------------------------
            Dim tbldatos_detalle_remit As PdfPTable = New PdfPTable(2)
            Dim cel_nombre_title As PdfPCell = New PdfPCell(New Paragraph("Nombre Remitente", _standardFont7))
            Dim cel_nombre As PdfPCell = New PdfPCell(New Paragraph(UCase(nombre_usuario), _standardFont7))
            tbldatos_detalle_remit.AddCell(cel_nombre_title)
            tbldatos_detalle_remit.AddCell(cel_nombre)
            Dim cel_cargo_title As PdfPCell = New PdfPCell(New Paragraph("Cargo Remitente", _standardFont7))
            Dim cel_cargo As PdfPCell = New PdfPCell(New Paragraph(UCase(cargo_usario), _standardFont7))
            tbldatos_detalle_remit.AddCell(cel_cargo_title)
            tbldatos_detalle_remit.AddCell(cel_cargo)
            Dim cel_anexo_title As PdfPCell = New PdfPCell(New Paragraph("Anexo", _standardFont7))
            Dim cel_anexo As PdfPCell = New PdfPCell(New Paragraph(UCase(strucs.ANEXO), _standardFont7))
            tbldatos_detalle_remit.AddCell(cel_anexo_title)
            tbldatos_detalle_remit.AddCell(cel_anexo)

            tblrdatos_detalle.AddCell(tbldatos_detalle_remit)
            '------------------------------------------------------
            'Asigna datos destinatario
            '------------------------------------------------------
            Dim tbldatos_detalle_dest As PdfPTable = New PdfPTable(2)
            Dim cel_nombre_dest_title As PdfPCell = New PdfPCell(New Paragraph("Nombre/rsocial Destinatario", _standardFont7))
            Dim cel_dest_nombre As PdfPCell = New PdfPCell(New Paragraph(UCase(strucs.NOMBRE_RAZON_SOCIAL), _standardFont7))
            tbldatos_detalle_dest.AddCell(cel_nombre_dest_title)
            tbldatos_detalle_dest.AddCell(cel_dest_nombre)
            Dim cel_nit_dest_title As PdfPCell = New PdfPCell(New Paragraph("Nit/cedula Destinatario", _standardFont7))
            Dim cel_nit_dest As PdfPCell = New PdfPCell(New Paragraph(UCase(strucs.NIT_IDENTIFICACION), _standardFont7))
            tbldatos_detalle_dest.AddCell(cel_nit_dest_title)
            tbldatos_detalle_dest.AddCell(cel_nit_dest)
            Dim cel_cargo_dest_title As PdfPCell = New PdfPCell(New Paragraph("Teléfono Destinatario", _standardFont7))
            Dim cel_dest_cargo As PdfPCell = New PdfPCell(New Paragraph(strucs.TELEFONO, _standardFont8))
            tbldatos_detalle_dest.AddCell(cel_cargo_dest_title)
            tbldatos_detalle_dest.AddCell(cel_dest_cargo)
            Dim cel_dir_dest_title As PdfPCell = New PdfPCell(New Paragraph("Dirección Destinatario", _standardFont7))
            Dim cel_dir_dest As PdfPCell = New PdfPCell(New Paragraph(UCase(strucs.DIRECCION) & " " & strucs.MUNICIPIO, _standardFont7))
            tbldatos_detalle_dest.AddCell(cel_dir_dest_title)
            tbldatos_detalle_dest.AddCell(cel_dir_dest)
            Dim cel_tel_dest_title As PdfPCell = New PdfPCell(New Paragraph("Correo electrónico Destinatario", _standardFont7))
            Dim cel_tel_dest As PdfPCell = New PdfPCell(New Paragraph(UCase(strucs.CORREO_ELECTRONICO), _standardFont7))
            tbldatos_detalle_dest.AddCell(cel_tel_dest_title)
            tbldatos_detalle_dest.AddCell(cel_tel_dest)
            Dim cel_rec_dest_title As PdfPCell = New PdfPCell(New Paragraph("INFORMACION DE RECIBIDO", _standardFont7))
            cel_rec_dest_title.Colspan = 2
            cel_rec_dest_title.HorizontalAlignment = 1
            tblrdatos_detalle.AddCell(tbldatos_detalle_dest)
            tblrdatos_detalle.AddCell(cel_rec_dest_title)
            '-----------------------------------------------
            'Detalle de recibido
            '-----------------------------------------------
            Dim tblrdatorecibido As PdfPTable = New PdfPTable(4)
            Dim i5 As Single = doc.PageSize.Width - (doc.PageSize.Width * 75 / 100)
            Dim i6 As Single = doc.PageSize.Width - (doc.PageSize.Width * 75 / 100)
            Dim i7 As Single = doc.PageSize.Width - (doc.PageSize.Width * 75 / 100)
            Dim i8 As Single = doc.PageSize.Width - (doc.PageSize.Width * 75 / 100)
            tblrdatorecibido.SetWidthPercentage(New Single() {i5, i6, i7, i8}, New Rectangle(doc.PageSize.Width, doc.PageSize.Height))
            Dim cel_hora_rec_title As PdfPCell = New PdfPCell(New Paragraph("Fecha hora recibo (yyyy/mm/dd:hh)", _standardFont7))
            Dim cel_hora_rec As PdfPCell = New PdfPCell(New Paragraph("_______/____/____:_______" & vbCrLf & "YYYY MM  DD  HH               ", _standardFont))
            cel_hora_rec.VerticalAlignment = Element.ALIGN_MIDDLE
            cel_hora_rec.HorizontalAlignment = Element.ALIGN_CENTER
            tblrdatorecibido.AddCell(cel_hora_rec_title)
            tblrdatorecibido.AddCell(cel_hora_rec)
            Dim cel_motivo_rec_title As PdfPCell = New PdfPCell(New Paragraph("Motivo devolución", _standardFont7))
            'Dim cel_motivo_rec As PdfPCell = New PdfPCell(New Paragraph("", _standardFont))
            cel_motivo_rec_title.MinimumHeight = 90.0F
            cel_motivo_rec_title.Colspan = 2
            tblrdatorecibido.AddCell(cel_motivo_rec_title)
            'tblrdatorecibido.AddCell(cel_motivo_rec)
            Dim cel_firma_rec_title As PdfPCell = New PdfPCell(New Paragraph("Firma Recibido", _standardFont7))
            'cel_firma_rec_title.Border = 0
            'Dim cel_firma_rec As PdfPCell = New PdfPCell(New Paragraph("", _standardFont))
            'cel_firma_rec.Border = 0
            cel_firma_rec_title.Colspan = 2
            cel_firma_rec_title.MinimumHeight = 90.0F
            tblrdatorecibido.AddCell(cel_firma_rec_title)
            'tblrdatorecibido.AddCell(cel_firma_rec)
            Dim cel_nota_rec_title As PdfPCell = New PdfPCell(New Paragraph("Espacio nota destinatario", _standardFont7))
            'Dim cel_nota_rec As PdfPCell = New PdfPCell(New Paragraph("", _standardFont))
            cel_nota_rec_title.Colspan = 2
            cel_nota_rec_title.MinimumHeight = 90.0F
            tblrdatorecibido.AddCell(cel_nota_rec_title)
            Dim cel_ob_rec_title As PdfPCell = New PdfPCell(New Paragraph("Espacio nota mensajería", _standardFont7))
            'Dim cel_nota_rec As PdfPCell = New PdfPCell(New Paragraph("", _standardFont))
            cel_ob_rec_title.Colspan = 4
            cel_ob_rec_title.MinimumHeight = 90.0F
            tblrdatorecibido.AddCell(cel_ob_rec_title)
            'tblrdatorecibido.AddCell(cel_nota_rec)
            doc.Add(tblrdatos_detalle)
            doc.Add(tblrdatorecibido)
            doc.Close()
            writer.Close()
            archivo = archivo_pdf
            genera_documento_guia = "YES"
        Catch ex As Exception
            genera_documento_guia = "Inconsistencia general función genera_documento_guia " & ex.Message

        Finally

        End Try
    End Function
    Function Asigna_datos_interface_edicio_guia(ByVal stru As guia_envio, ByRef pag As Page) As String
        Try
            Dim TextBox_codigo_guia_envio As TextBox = pag.FindControl("TextBox_codigo_guia_envio")
            Dim TextBox_NOMBRE_RAZON_SOCIAL As TextBox = pag.FindControl("TextBox_NOMBRE_RAZON_SOCIAL")
            Dim TextBox_DIRECCION As TextBox = pag.FindControl("TextBox_DIRECCION")
            Dim TextBox_NIT_IDENTIFICACION As TextBox = pag.FindControl("TextBox_NIT_IDENTIFICACION")
            Dim TextBox_TELEFONO As TextBox = pag.FindControl("TextBox_TELEFONO")
            Dim TextBox_CORREO_ELECTRONICO As TextBox = pag.FindControl("TextBox_CORREO_ELECTRONICO")
            Dim TextBox_ANEXO As TextBox = pag.FindControl("TextBox_ANEXO")
            Dim TextBox_RADICADO As TextBox = Nothing
            TextBox_RADICADO = pag.FindControl("TextBox_RADICADO")
            If Not TextBox_RADICADO Is Nothing Then
                TextBox_RADICADO.Text = stru.RADICADO
            End If
            TextBox_codigo_guia_envio.Text = stru.Concecutivo_Guia
            TextBox_NOMBRE_RAZON_SOCIAL.Text = stru.NOMBRE_RAZON_SOCIAL
            TextBox_DIRECCION.Text = stru.DIRECCION
            TextBox_NIT_IDENTIFICACION.Text = stru.NIT_IDENTIFICACION
            TextBox_TELEFONO.Text = stru.TELEFONO
            TextBox_CORREO_ELECTRONICO.Text = stru.CORREO_ELECTRONICO
            TextBox_ANEXO.Text = stru.ANEXO
            Asigna_datos_interface_edicio_guia = "YES"
        Catch ex As Exception
            Asigna_datos_interface_edicio_guia = "Inconsistencia funcion Asigna_datos_interface_edicio_guia " & ex.Message
        End Try
    End Function
    Function asigna_remitente_destinatario_interface_guia(ByRef pag1 As Page) As String
        Try
            Dim modal As ModalPopupExtender = pag1.FindControl("ModalPopupExtender_valiacion_plantilla")
            Dim updat As UpdatePanel = pag1.FindControl("UpdatePanel_procesa_tramite_envio")
            Dim hidevalorselccion As Object = pag1.FindControl("Hidden_remitente_destinatario")
            Dim TextBox_NOMBRE_RAZON_SOCIAL As TextBox = pag1.FindControl("TextBox_NOMBRE_RAZON_SOCIAL")
            If hidevalorselccion.Value = "-1" Then
                asigna_remitente_destinatario_interface_guia = "Debe seleccionar el registro a asignar"
                Exit Function
            End If

            Dim Result As String = ""
            Dim stru_valores_campo() As stru_campos_destinatario = Nothing
            Result = Me.Retorna_datos_guia_envio_destinatario(stru_valores_campo, hidevalorselccion.Value)
            If Result <> "YES" Then
                asigna_remitente_destinatario_interface_guia = Result
                Exit Function
            End If
            Result = Me.Asigna_datos_interface_datos_externos(stru_valores_campo, pag1)
            If Result <> "YES" Then
                asigna_remitente_destinatario_interface_guia = Result
                Exit Function
            End If
            updat.Update()
            modal.Hide()
            asigna_remitente_destinatario_interface_guia = "YES"
        Catch ex As Exception
            asigna_remitente_destinatario_interface_guia = "Inconsistencia funcion asigna_remitente_destinatario_interface_guia " & ex.Message
        End Try
    End Function
    Function asigna_remitente_destinatario_id_dest(ByRef pag1 As Page, ByVal nombre_dest As String, ByRef id_dest As String) As String
        Try
            Dim updateid As UpdatePanel = pag1.FindControl("updatepanel_Asigana_datos_validacion_edicion")
            Dim updat As UpdatePanel = pag1.FindControl("UpdatePanel_procesa_tramite_envio")
            Dim hidevalorselccion As Object = pag1.FindControl("Hidden_remitente_destinatario")
            Dim Result As String = ""
            Dim stru_valores_campo() As stru_campos_destinatario = Nothing
            Result = Me.Retorna_datos_guia_envio_destinatario_nombre(stru_valores_campo, nombre_dest)
            If Result <> "YES" Then
                asigna_remitente_destinatario_id_dest = "YES"
                Exit Function
            End If
            For i As Integer = 0 To stru_valores_campo.Length - 1
                If stru_valores_campo(i).campo_identi = 1 Then
                    id_dest = stru_valores_campo(i).valor_campo_fuente
                End If
            Next
            'updateid.Update()
            asigna_remitente_destinatario_id_dest = "YES"
        Catch ex As Exception
            asigna_remitente_destinatario_id_dest = "Inconsistencia funcion aasigna_remitente_destinatario_id_dest " & ex.Message
        End Try
    End Function
    Function asigna_remitente_destinatario_interface_guia_manual(ByRef pag1 As Page) As String
        Try
            Dim updateid As UpdatePanel = pag1.FindControl("updatepanel_Asigana_datos_validacion_edicion")
            Dim updat As UpdatePanel = pag1.FindControl("UpdatePanel_procesa_tramite_envio")
            Dim hidevalorselccion As Object = pag1.FindControl("Hidden_remitente_destinatario")
            Dim TextBox_NOMBRE_RAZON_SOCIAL As TextBox = pag1.FindControl("TextBox_NOMBRE_RAZON_SOCIAL")
            If TextBox_NOMBRE_RAZON_SOCIAL.Text = "" Then
                asigna_remitente_destinatario_interface_guia_manual = "Debe informar el nombre del destinatario"
                Exit Function
            End If
            Dim Result As String = ""
            Dim stru_valores_campo() As stru_campos_destinatario = Nothing
            Result = Me.Retorna_datos_guia_envio_destinatario_nombre(stru_valores_campo, TextBox_NOMBRE_RAZON_SOCIAL.Text)
            If Result <> "YES" Then
                asigna_remitente_destinatario_interface_guia_manual = Result
                Exit Function
            End If
            Result = Me.Asigna_datos_interface_datos_externos(stru_valores_campo, pag1)
            If Result <> "YES" Then
                asigna_remitente_destinatario_interface_guia_manual = Result
                Exit Function
            End If
            For i As Integer = 0 To stru_valores_campo.Length - 1
                If stru_valores_campo(i).campo_identi = 1 Then
                    hidevalorselccion.value = stru_valores_campo(i).valor_campo_fuente
                End If
            Next
            updateid.Update()
            asigna_remitente_destinatario_interface_guia_manual = "YES"
        Catch ex As Exception
            asigna_remitente_destinatario_interface_guia_manual = "Inconsistencia funcion asigna_remitente_destinatario_interface_guia_manual " & ex.Message
        End Try
    End Function
    Function Retorna_datos_estructura_guia(ByVal id_guia As Integer, ByRef stru As guia_envio) As String
        Try
            Dim sql_consulta As String = "Select Id_guia_envio,areas_depart_radicacion_Codigo_Area,Remit_Dest_Interno_id_Remit_Dest_Int," & _
            "ra_empresa_envio_ID_EMPRESA_ENVIO,Concecutivo_Guia,Fecha_Registro_Guia,NIT_IDENTIFICACION,NOMBRE_RAZON_SOCIAL,DIRECCION," & _
            "TELEFONO,CORREO_ELECTRONICO,PAIS,DEPARTAMENTO,MUNICIPIO,Destinatario_Ext,ESTADO_CONFIRMACION_GUIA,FECHA_RECIBIDO_GUIA," & _
            "NOTA_CLIENTE,RADICADO,ID_MENSAJERO_INTERNO,ID_TIPO_GUIA,ESTADO_GUIA,ANEXO,FECHA_ENVIO_GUIA,TIEMPO_RESPUESTA,ID_TIPO_MANUAL_AUTOMATICO,ID_USUARIO_GESTION_TRANSAC FROM ra_guia_interna WHERE Id_guia_envio=" & id_guia
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_datos_estructura_guia = "función Retorna_datos_estructura_guia dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    stru.Id_guia_envio = 0
                Else
                    stru.Id_guia_envio = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    stru.areas_depart_radicacion_Codigo_Area = 0
                Else
                    stru.areas_depart_radicacion_Codigo_Area = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    stru.Remit_Dest_Interno_id_Remit_Dest_Int = 0
                Else
                    stru.Remit_Dest_Interno_id_Remit_Dest_Int = Datset.Tables(0).Rows(0).Item(2)
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) = True Then
                    stru.ra_empresa_envio_ID_EMPRESA_ENVIO = 0
                Else
                    stru.ra_empresa_envio_ID_EMPRESA_ENVIO = Datset.Tables(0).Rows(0).Item(3)
                End If
                If Datset.Tables(0).Rows(0).IsNull(4) = True Then
                    stru.Concecutivo_Guia = ""
                Else
                    stru.Concecutivo_Guia = Datset.Tables(0).Rows(0).Item(4)
                End If
                If Datset.Tables(0).Rows(0).IsNull(5) = True Then
                    stru.Fecha_Registro_Guia = ""
                Else
                    stru.Fecha_Registro_Guia = Datset.Tables(0).Rows(0).Item(5)
                End If
                If Datset.Tables(0).Rows(0).IsNull(6) = True Then
                    stru.NIT_IDENTIFICACION = ""
                Else
                    stru.NIT_IDENTIFICACION = Datset.Tables(0).Rows(0).Item(6)
                End If
                If Datset.Tables(0).Rows(0).IsNull(7) = True Then
                    stru.NOMBRE_RAZON_SOCIAL = ""
                Else
                    stru.NOMBRE_RAZON_SOCIAL = Datset.Tables(0).Rows(0).Item(7)
                End If
                If Datset.Tables(0).Rows(0).IsNull(8) = True Then
                    stru.DIRECCION = ""
                Else
                    stru.DIRECCION = Datset.Tables(0).Rows(0).Item(8)
                End If
                If Datset.Tables(0).Rows(0).IsNull(9) = True Then
                    stru.TELEFONO = ""
                Else
                    stru.TELEFONO = Datset.Tables(0).Rows(0).Item(9)
                End If
                If Datset.Tables(0).Rows(0).IsNull(10) = True Then
                    stru.CORREO_ELECTRONICO = ""
                Else
                    stru.CORREO_ELECTRONICO = Datset.Tables(0).Rows(0).Item(10)
                End If
                If Datset.Tables(0).Rows(0).IsNull(11) = True Then
                    stru.PAIS = ""
                Else
                    stru.PAIS = Datset.Tables(0).Rows(0).Item(11)
                End If
                If Datset.Tables(0).Rows(0).IsNull(12) = True Then
                    stru.DEPARTAMENTO = ""
                Else
                    stru.DEPARTAMENTO = Datset.Tables(0).Rows(0).Item(12)
                End If
                If Datset.Tables(0).Rows(0).IsNull(13) = True Then
                    stru.MUNICIPIO = ""
                Else
                    stru.MUNICIPIO = Datset.Tables(0).Rows(0).Item(13)
                End If
                If Datset.Tables(0).Rows(0).IsNull(14) = True Then
                    stru.Destinatario_Ext = 0
                Else
                    stru.Destinatario_Ext = Datset.Tables(0).Rows(0).Item(14)
                End If
                If Datset.Tables(0).Rows(0).IsNull(15) = True Then
                    stru.ESTADO_CONFIRMACION_GUIA = 0
                Else
                    stru.ESTADO_CONFIRMACION_GUIA = Datset.Tables(0).Rows(0).Item(15)
                End If
                If Datset.Tables(0).Rows(0).IsNull(16) = True Then
                    stru.FECHA_RECIBIDO_GUIA = ""
                Else
                    stru.FECHA_RECIBIDO_GUIA = Datset.Tables(0).Rows(0).Item(16)
                End If
                If Datset.Tables(0).Rows(0).IsNull(17) = True Then
                    stru.NOTA_CLIENTE = ""
                Else
                    stru.NOTA_CLIENTE = Datset.Tables(0).Rows(0).Item(17)
                End If
                If Datset.Tables(0).Rows(0).IsNull(18) = True Then
                    stru.RADICADO = ""
                Else
                    stru.RADICADO = Datset.Tables(0).Rows(0).Item(18)
                End If
                If Datset.Tables(0).Rows(0).IsNull(19) = True Then
                    stru.ID_MENSAJERO_INTERNO = 0
                Else
                    stru.ID_MENSAJERO_INTERNO = Datset.Tables(0).Rows(0).Item(19)
                End If
                If Datset.Tables(0).Rows(0).IsNull(20) = True Then
                    stru.ID_TIPO_GUIA = 0
                Else
                    stru.ID_TIPO_GUIA = Datset.Tables(0).Rows(0).Item(20)
                End If
                If Datset.Tables(0).Rows(0).IsNull(21) = True Then
                    stru.ESTADO_GUIA = 0
                Else
                    stru.ESTADO_GUIA = Datset.Tables(0).Rows(0).Item(21)
                End If
                If Datset.Tables(0).Rows(0).IsNull(22) = True Then
                    stru.ANEXO = ""
                Else
                    stru.ANEXO = Datset.Tables(0).Rows(0).Item(22)
                End If
                If Datset.Tables(0).Rows(0).IsNull(23) = True Then
                    stru.FECHA_ENVIO_GUIA = ""
                Else
                    stru.FECHA_ENVIO_GUIA = Datset.Tables(0).Rows(0).Item(23)
                End If
                If Datset.Tables(0).Rows(0).IsNull(24) = True Then
                    stru.TIEMPO_RESPUESTA = 0
                Else
                    stru.TIEMPO_RESPUESTA = Datset.Tables(0).Rows(0).Item(24)
                End If
                If Datset.Tables(0).Rows(0).IsNull(25) = True Then
                    stru.ID_TIPO_MANUAL_AUTOMATICO = 0
                Else
                    stru.ID_TIPO_MANUAL_AUTOMATICO = Datset.Tables(0).Rows(0).Item(25)
                End If
                If Datset.Tables(0).Rows(0).IsNull(26) = True Then
                    stru.ID_USUARIO_GESTION_TRANSAC = 0
                Else
                    stru.ID_USUARIO_GESTION_TRANSAC = Datset.Tables(0).Rows(0).Item(26)
                End If
                Retorna_datos_estructura_guia = "YES"
                Exit Function
            Else
                Retorna_datos_estructura_guia = "Imposible encontrar registro para el tramite"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_datos_estructura_guia = "Inconsistencia función Retorna_datos_estructura_guia " & ex.Message
        End Try
    End Function
    Function Retorna_id_imagen_correspondencia_por_enviar(ByVal id_tramite_envio As Integer, ByRef id_imagen As Long) As String
        Try
            Dim sql_consulta As String = "Select ID_IMAGEN from ra_respuesta_radicado where ID_RESPUESTA_RADICADO=" & id_tramite_envio
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_imagen_correspondencia_por_enviar = "función Retorna_id_imagen_correspondencia_por_enviar dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    Retorna_id_imagen_correspondencia_por_enviar = "El registro no tiene documento asociado"
                    Exit Function
                End If

                id_imagen = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_imagen_correspondencia_por_enviar = "YES"
                Exit Function
            Else
                Retorna_id_imagen_correspondencia_por_enviar = "Imposible encontrar registro para el tramite"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_imagen_correspondencia_por_enviar = "Inconsistencia general función Retorna_id_imagen_correspondencia_por_enviar " & ex.Message
        End Try
    End Function
    Function Retorna_id_area_permitida_para_envio(ByVal Nombre_area As String, _
                                                  ByVal id_organigrama As Integer, ByRef id_area_permitida As Integer) As String
        '----------------------------------------------------------------
        'Funcion :Retorna id area permitida usuario de gestion, con los
        'parametros nombre area y id organigrama
        'Fecha : 2016-04-16
        'Ing : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------
        Try
            Dim sql_consulta As String = "select Codigo_Area from  areas_depart_radicacion as adr " & _
                " where adr.Nombre_Area='" & Nombre_area & "' and Registro_Organigrama_Id_Organigrama=" & id_organigrama
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_area_permitida_para_envio = "Función Retorna_areas_permitidas_para_envio_usuario_gestion_id_area dice " & Result
                Exit Function
            End If

            If Datset.Tables(0).Rows.Count > 0 Then
                id_area_permitida = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_area_permitida_para_envio = "YES"
                Exit Function
            Else
                Retorna_id_area_permitida_para_envio = "Función Retorna_areas_permitidas_para_envio_usuario_gestion_id_area dice imposible encontrar id area " & Nombre_area
            End If
        Catch ex As Exception
            Retorna_id_area_permitida_para_envio = "Inconsistencia funcion Retorna_id_area_permitida_para_envio " & ex.Message
        End Try
    End Function
    Function Retorna_areas_permitidas_para_envio_usuario_gestion_id_area(ByVal id_usuario_gestion As Integer, ByRef id_areas_permitidas() As Integer) As String
        Try
            Dim sql_consulta As String = "select AREA_ARCHIVO_ID_AREA from  ra_area_departamento_permitida_usuario_gestion_resp as rdp " & _
                " where rdp.remit_dest_interno_id_Remit_Dest_Int=" & id_usuario_gestion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_areas_permitidas_para_envio_usuario_gestion_id_area = "Función Retorna_areas_permitidas_para_envio_usuario_gestion_id_area dice " & Result
                Exit Function
            End If
            Erase id_areas_permitidas
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve id_areas_permitidas(i)
                    id_areas_permitidas(i) = Datset.Tables(0).Rows(i).Item(0)
                Next
                Retorna_areas_permitidas_para_envio_usuario_gestion_id_area = "YES"
            Else
                Retorna_areas_permitidas_para_envio_usuario_gestion_id_area = "YES"
            End If
        Catch ex As Exception
            Retorna_areas_permitidas_para_envio_usuario_gestion_id_area = "Inconsistencia funcion Retorna_areas_permitidas_para_envio_usuario_gestion_id_area " & ex.Message
        End Try
    End Function
    Function Retorna_areas_permitidas_para_envio_usuario_gestion(ByVal id_usuario_gestion As Integer, ByRef drowlist As DropDownList, ByRef update As UpdatePanel) As String
        Try
            Dim sql_consulta As String = "select adr.Nombre_Area from  ra_area_departamento_permitida_usuario_gestion_resp as rdp " & _
                " inner join areas_depart_radicacion as adr on (adr.Codigo_Area=rdp.AREA_ARCHIVO_ID_AREA) " & _
                " where rdp.remit_dest_interno_id_Remit_Dest_Int=" & id_usuario_gestion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_areas_permitidas_para_envio_usuario_gestion = "Función Retorna_areas_permitidas_para_envio_usuario_gestion dice " & Result
                Exit Function
            End If
            drowlist.Items.Clear()
            If Datset.Tables(0).Rows.Count > 0 Then
                drowlist.Items.Add("Todas")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    drowlist.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                drowlist.Text = "Todas"
                Retorna_areas_permitidas_para_envio_usuario_gestion = "YES"
            Else
                Retorna_areas_permitidas_para_envio_usuario_gestion = "YES"
            End If
        Catch ex As Exception
            Retorna_areas_permitidas_para_envio_usuario_gestion = "Inconsistencia funcion Retorna_areas_permitidas_para_envio_usuario_gestion " & ex.Message
        End Try
    End Function
    Function Retorna_correo_electronico_usuario_radicador(ByVal id_usuario_radicador As Integer, ByRef correo_usuario As String) As String
        Try
            Dim sql_consulta As String = "Select Correo_Usuario from usuario_radicador where id_usuario=" & id_usuario_radicador
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_correo_electronico_usuario_radicador = "Función retorna_correo_electronico_usuario_radicador dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    correo_usuario = ""
                Else
                    correo_usuario = Datset.Tables(0).Rows(0).Item(0)
                End If
                Retorna_correo_electronico_usuario_radicador = "YES"
            Else
                correo_usuario = ""
                Retorna_correo_electronico_usuario_radicador = "YES"
            End If
        Catch ex As Exception
            Retorna_correo_electronico_usuario_radicador = "Inconsistencia funcion Retorna_correo_electronico_usuario_radicador " & ex.Message
        End Try
    End Function
    Function Asigna_nueva_cuenta_correo(ByRef lista_correo As String, ByVal nuevo_correo As String) As String
        Try
            Dim split() As String = Nothing
            If lista_correo = "" Then
                lista_correo = nuevo_correo
                Asigna_nueva_cuenta_correo = "YES"
                Exit Function
            End If
            split = lista_correo.Split(",")
            If Not split Is Nothing Then
                For i As Integer = 0 To split.Length - 1
                    If nuevo_correo = split(i) Then
                        Asigna_nueva_cuenta_correo = "La dirección " & nuevo_correo & " se encuentra en la lista de destinatarios"
                        Exit Function
                    End If
                Next
                If lista_correo = "" Then
                    lista_correo = nuevo_correo
                Else
                    lista_correo = lista_correo & "," & nuevo_correo
                End If
            Else
                If lista_correo = "" Then
                    lista_correo = nuevo_correo
                Else
                    lista_correo = lista_correo & "," & nuevo_correo
                End If

            End If
            Asigna_nueva_cuenta_correo = "YES"
        Catch ex As Exception
            Asigna_nueva_cuenta_correo = "Inconsistencia función Asigna_nueva_cuenta_correo " & ex.Message
        End Try
    End Function
    Function Retorna_correo_electronico_usuario_gestion(ByVal id_usuario_gestion As Integer, _
                     ByRef correo_usuario_gestion As String) As String
        Try

            Dim sql_consulta As String = "Select Correo_Electronico from remit_dest_interno where id_Remit_Dest_Int=" & id_usuario_gestion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_correo_electronico_usuario_gestion = "Función Retorna_id_usuario_gestion_respuesta dice " & Result
                Exit Function
            End If

            If Datset.Tables(0).Rows.Count > 0 Then

                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    correo_usuario_gestion = ""
                Else
                    correo_usuario_gestion = Datset.Tables(0).Rows(0).Item(0)
                End If
                Retorna_correo_electronico_usuario_gestion = "YES"
            Else
                correo_usuario_gestion = ""
                Retorna_correo_electronico_usuario_gestion = "YES"
            End If

        Catch ex As Exception
            Retorna_correo_electronico_usuario_gestion = "Inconsistencia función Retorna_correo_electronico_usuario_gestion " & ex.Message
        End Try
    End Function
    Function Retorna_id_usuario_gestion_respuesta(ByVal id_tramite_respuesta As Integer, _
                                                  ByRef id_usuario_gestion As Integer) As String

        Try
            Dim sql_consulta As String = "Select ID_REMIT_DEST_INT from ra_respuesta_radicado where ID_RESPUESTA_RADICADO=" & id_tramite_respuesta
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_usuario_gestion_respuesta = "Función Retorna_id_usuario_gestion_respuesta dice " & Result
                Exit Function
            End If

            If Datset.Tables(0).Rows.Count > 0 Then

                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    id_usuario_gestion = 0
                Else
                    id_usuario_gestion = Datset.Tables(0).Rows(0).Item(0)
                End If
                Retorna_id_usuario_gestion_respuesta = "YES"
            Else
                id_usuario_gestion = 0
                Retorna_id_usuario_gestion_respuesta = "YES"
            End If
        Catch ex As Exception
            Retorna_id_usuario_gestion_respuesta = "Inconsistencia función Retorna_id_usuario_gestion_respuesta " & ex.Message
        End Try
    End Function
    Function Retorna_id_destinatario_guia_correspondencia(ByVal id_guia_envio As Integer, ByRef id_destinatario_guia As Integer) As String
        Try
            Dim sql_consulta As String = "Select Destinatario_Ext from ra_guia_interna where Id_guia_envio=" & id_guia_envio
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_destinatario_guia_correspondencia = "Función Retorna_id_destinatario_guia_correspondencia dice " & Result
                Exit Function
            End If

            If Datset.Tables(0).Rows.Count > 0 Then

                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    id_destinatario_guia = 0
                Else
                    id_destinatario_guia = Datset.Tables(0).Rows(0).Item(0)
                End If
                Retorna_id_destinatario_guia_correspondencia = "YES"
            Else
                id_destinatario_guia = 0
                Retorna_id_destinatario_guia_correspondencia = "YES"
            End If
        Catch ex As Exception
            Retorna_id_destinatario_guia_correspondencia = "Inconsistencia función Retorna_id_destinatario_guia_correspondencia " & ex.Message
        End Try
    End Function
    Function Retorna_id_destinatario_gestion_respuesta(ByVal id_tramite_respuesta As Integer, _
                                                  ByRef id_usuario_externo As Integer) As String

        Try
            Dim sql_consulta As String = "Select codigo_dest_externo from ra_respuesta_radicado where ID_RESPUESTA_RADICADO=" & id_tramite_respuesta
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_destinatario_gestion_respuesta = "Función Retorna_id_destinatario_gestion_respuesta dice " & Result
                Exit Function
            End If

            If Datset.Tables(0).Rows.Count > 0 Then

                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    id_usuario_externo = 0
                Else
                    id_usuario_externo = Datset.Tables(0).Rows(0).Item(0)
                End If
                Retorna_id_destinatario_gestion_respuesta = "YES"
            Else
                id_usuario_externo = 0
                Retorna_id_destinatario_gestion_respuesta = "YES"
            End If
        Catch ex As Exception
            Retorna_id_destinatario_gestion_respuesta = "Inconsistencia función Retorna_id_destinatario_gestion_respuesta " & ex.Message
        End Try
    End Function
    Function Lista_tramites_envios_por_archivar(ByRef page1 As Page) As String

        Try
            Dim hdnEmailID_VAL As Object = page1.FindControl("hdnEmailID_VAL")
            Dim scripma As GridView = page1.FindControl("GridView_val_radicacion")
            Dim labetitle As Label = page1.FindControl("titulo_label_val_radicacion")
            Dim updatelabel As UpdatePanel = page1.FindControl("UpdatePanelabel_val_radicacion")
            Dim DropDownList_areas_depart As DropDownList = page1.FindControl("DropDownList_areas_depart")
            Dim DropDownList_empresa_envio As DropDownList = page1.FindControl("DropDownList_empresa_envio")
            Dim TextBox_fecha_ini As TextBox = page1.FindControl("TextBox_fecha_ini")
            Dim TextBox_fecha_fin As TextBox = page1.FindControl("TextBox_fecha_fin")
            Dim TextBox_fecha_resp_ini As TextBox = page1.FindControl("TextBox_fecha_resp_ini")
            Dim TextBox_fecha_resp_fin As TextBox = page1.FindControl("TextBox_fecha_resp_fin")
            Dim TextBoxRadicado As TextBox = page1.FindControl("TextBoxRadicado")
            Dim TextBoxRadicado_respuesta As TextBox = page1.FindControl("TextBoxRadicado_respuesta")
            Dim TextBoxUSUARIO_RESPONSABLE As TextBox = page1.FindControl("TextBoxUSUARIO_RESPONSABLE")
            Dim TextBoxDESTINATARIO As TextBox = page1.FindControl("TextBoxDESTINATARIO")
            Dim TextBox_GUIA_ENVIO As TextBox = page1.FindControl("TextBox_GUIA_ENVIO")
            Dim TextBox_FECHA_ENVIO_INI As TextBox = page1.FindControl("TextBox_FECHA_ENVIO_INI")
            Dim TextBox_FECHA_ENVIO_FIN As TextBox = page1.FindControl("TextBox_FECHA_ENVIO_FIN")
            If scripma Is Nothing Then
                Lista_tramites_envios_por_archivar = "Imposible encontrar datagrid GridView_val_radicacion"
                Exit Function
            End If
            If labetitle Is Nothing Then
                Lista_tramites_envios_por_archivar = "Imposible encontrar el control   titulo_label"
                Exit Function
            End If
            If updatelabel Is Nothing Then
                Lista_tramites_envios_por_archivar = "Imposible encontrar el control  UpdatePanelabel_validacion"
                Exit Function
            End If
            Dim updat As UpdatePanel = page1.Page.FindControl("UpdatePanel_conenido_grid_val_radicacion")
            If updatelabel Is Nothing Then
                Lista_tramites_envios_por_archivar = "Imposible encontrar el control  UpdatePanel_conenido_grid_val_radicacion"
                Exit Function
            End If
            Dim Result As String = ""
            Dim sql_condicion As String = ""
            Dim sql_condicion_iner As String = ""
            Dim id_organigrama As Integer = 0
            Dim id_area_permitida As Integer = 0
            Dim id_areas_permitidas() As Integer
            Erase id_areas_permitidas
            Dim Reclas_registro_organigrama As New Class_registro_organigrama
            If DropDownList_empresa_envio.Text <> "" Then
                sql_condicion = sql_condicion & " and EMPRESA_ENVIO='" & DropDownList_empresa_envio.Text & "'"
            End If
            If DropDownList_areas_depart.Text = "" Then
                Lista_tramites_envios_por_archivar = "El usuarios de gestión no tiene relacionas áreas para archivar, consulte a su administrador"
                Exit Function
            End If
            If DropDownList_areas_depart.Text <> "Todas" Then
                Result = Reclas_registro_organigrama.Retorna_id_organigrama_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                            id_organigrama)
                If Result <> "YES" Then
                    Lista_tramites_envios_por_archivar = Result
                    Exit Function
                End If
                Result = Me.Retorna_id_area_permitida_para_envio(DropDownList_areas_depart.Text, id_organigrama, id_area_permitida)
                If Result <> "YES" Then
                    Lista_tramites_envios_por_archivar = Result
                    Exit Function
                End If
                sql_condicion = sql_condicion & " and ID_AREA=" & id_area_permitida
            Else
                sql_condicion_iner = "  INNER JOIN ra_area_departamento_permitida_usuario_gestion_resp AS ADP ON " & _
                " (ADP.AREA_ARCHIVO_ID_AREA=RRR.ID_AREA AND ADP.remit_dest_interno_id_Remit_Dest_Int=" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & ")"
            End If

            If TextBox_fecha_ini.Text <> "" And TextBox_fecha_fin.Text <> "" Then
                sql_condicion = sql_condicion & " and CAST(FECHA_VENCE AS DATE) between '" & TextBox_fecha_ini.Text & "' AND '" & TextBox_fecha_fin.Text & "'"
            Else
                If TextBox_fecha_ini.Text <> "" Then
                    sql_condicion = sql_condicion & " and CAST(FECHA_VENCE AS DATE) = '" & TextBox_fecha_ini.Text & "'"
                End If
                If TextBox_fecha_fin.Text <> "" Then
                    sql_condicion = sql_condicion & " and CAST(FECHA_VENCE AS DATE) = '" & TextBox_fecha_fin.Text & "'"
                End If
            End If
            If TextBox_fecha_resp_ini.Text <> "" And TextBox_fecha_resp_fin.Text <> "" Then
                sql_condicion = sql_condicion & " and CAST(FECHA_RESPUETA AS DATE) between '" & TextBox_fecha_resp_ini.Text & "' AND '" & TextBox_fecha_resp_fin.Text & "'"
            Else
                If TextBox_fecha_resp_ini.Text <> "" Then
                    sql_condicion = sql_condicion & " and CAST(FECHA_RESPUETA AS DATE) = '" & TextBox_fecha_resp_ini.Text & "'"
                End If
                If TextBox_fecha_resp_fin.Text <> "" Then
                    sql_condicion = sql_condicion & " and CAST(FECHA_RESPUETA AS DATE) = '" & TextBox_fecha_resp_fin.Text & "'"
                End If
            End If
            If TextBox_FECHA_ENVIO_INI.Text <> "" And TextBox_FECHA_ENVIO_FIN.Text <> "" Then
                sql_condicion = sql_condicion & " and CAST(FECHA_ENVIO AS DATE) between '" & TextBox_FECHA_ENVIO_INI.Text & "' AND '" & TextBox_FECHA_ENVIO_FIN.Text & "'"
            Else
                If TextBox_FECHA_ENVIO_INI.Text <> "" Then
                    sql_condicion = sql_condicion & " and CAST(FECHA_ENVIO AS DATE) = '" & TextBox_FECHA_ENVIO_INI.Text & "'"
                End If
                If TextBox_FECHA_ENVIO_FIN.Text <> "" Then
                    sql_condicion = sql_condicion & " and CAST(FECHA_ENVIO AS DATE) = '" & TextBox_FECHA_ENVIO_FIN.Text & "'"
                End If
            End If
            If TextBoxRadicado.Text <> "" Then
                sql_condicion = sql_condicion & " and RADICADO='" & TextBoxRadicado.Text & "'"
            End If
            If TextBoxRadicado_respuesta.Text <> "" Then
                sql_condicion = sql_condicion & " and RADICADO_RESPUESTA='" & TextBoxRadicado_respuesta.Text & "'"
            End If
            If TextBoxUSUARIO_RESPONSABLE.Text <> "" Then
                sql_condicion = sql_condicion & " and USUARIO_RESPONSABLE='" & TextBoxUSUARIO_RESPONSABLE.Text & "'"
            End If
            If TextBoxDESTINATARIO.Text <> "" Then
                sql_condicion = sql_condicion & " and DESTINATARIO='" & TextBoxDESTINATARIO.Text & "'"
            End If
            If TextBox_GUIA_ENVIO.Text <> "" Then
                sql_condicion = sql_condicion & " and GUIA_ENVIO='" & TextBox_GUIA_ENVIO.Text & "'"
            End If
            Dim sql_consulta As String = "SELECT RRR.ID_RESPUESTA_RADICADO,RRR.TRAMITE_DOCUMENTO,RRR.RADICADO," & _
            "RRR.RADICADO_RESPUESTA,RRR.GUIA_ENVIO,RRR.EMPRESA_ENVIO,RRR.FECHA_VENCE,RRR.FECHA_RESPUETA,RRR.FECHA_ENVIO,RRR.HORA_ENVIO,RDI.Nombre_Remitente as NOMBRE_REMITENTE,RDI.Cargo_Remite as CARGO_REMITENTE,ADR.Nombre_Area as NOMBRE_AREA," & _
            "RRR.DESTINATARIO,RRR.GABINETE,RRR.ID_IMAGEN " & _
            " FROM  ra_respuesta_radicado AS RRR " & _
            " INNER JOIN remit_dest_interno AS RDI ON  (RRR.ID_REMIT_DEST_INT=RDI.id_Remit_Dest_Int) " &
            " INNER JOIN areas_depart_radicacion AS ADR ON (RRR.ID_AREA=ADR.Codigo_Area) " & sql_condicion_iner & "  where ESTADO_ENVIO=1 AND ESTADO_RESPUESTA=2" & sql_condicion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_tramites_envios_por_archivar = "Imposible Encontrar datos para listar " & Result
                labetitle.Text = "Se encontraron " & "0" & " registro(s) enviados por archivar  "
                scripma.DataSource = Nothing
                'scripma.DataKeyNames = DataKey
                hdnEmailID_VAL.value = "-1"
                scripma.DataBind()
                updat.Update()
                updatelabel.Update()
                Exit Function

            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                labetitle.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) enviados por archivar "
                scripma.DataSource = Nothing
                'scripma.DataKeyNames = DataKey
                hdnEmailID_VAL.value = "-1"
                scripma.DataBind()
                updat.Update()
                updatelabel.Update()
                Lista_tramites_envios_por_archivar = "YES"
                Exit Function
            Else
                HttpContext.Current.Session.Item("RA_DATO_CONSULTA") = sql_consulta
                labetitle.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) enviados por archivar "
                'scripma.DataKeyNames = DataKey
                hdnEmailID_VAL.value = "-1"
                scripma.DataSource = Datset
                scripma.DataBind()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    scripma.Rows(i).Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                Next
                updat.Update()
                updatelabel.Update()
                Lista_tramites_envios_por_archivar = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Lista_tramites_envios_por_archivar = ex.Message
        End Try

    End Function
    Function Limpiar_campos_consulta_guia(ByRef page1 As Page) As String
        Try
            Dim DropDownList_areas_depart As DropDownList = page1.FindControl("DropDownList_areas_depart")
            Dim DropDownList_nombre_remitente As DropDownList = page1.FindControl("DropDownList_nombre_remitente")
            Dim DropDownList_empresa_envio As DropDownList = page1.FindControl("DropDownList_empresa_envio")
            Dim DropDownList_mensajero_inerno As DropDownList = page1.FindControl("DropDownList_mensajero_inerno")
            Dim TextBox_Id_guia_envio As TextBox = page1.FindControl("TextBox_Id_guia_envio")
            Dim TextBox_Concecutivo_Guia As TextBox = page1.FindControl("TextBox_Concecutivo_Guia")
            Dim TextBox_Fecha_Registro_Guia_ini As TextBox = page1.FindControl("TextBox_Fecha_Registro_Guia_ini")
            Dim TextBox_fecha_Registro_Guia_fin As TextBox = page1.FindControl("TextBox_fecha_Registro_Guia_fin")
            Dim TextBox_FECHA_ENVIO_GUIA_ini As TextBox = page1.FindControl("TextBox_FECHA_ENVIO_GUIA_ini")
            Dim TextBox_FECHA_ENVIO_GUIA_fin As TextBox = page1.FindControl("TextBox_FECHA_ENVIO_GUIA_fin")
            Dim TextBox_NOMBRE_RAZON_SOCIA As TextBox = page1.FindControl("TextBox_NOMBRE_RAZON_SOCIA")
            Dim TextBox_NIT_IDENTIFICACION_2 As TextBox = page1.FindControl("TextBox_NIT_IDENTIFICACION_2")
            Dim DropDownList_ESTADO_CONFIRMACION_GUIA As DropDownList = page1.FindControl("DropDownList_ESTADO_CONFIRMACION_GUIA")
            Dim DropDownList_ESTADO_GUIA As DropDownList = page1.FindControl("DropDownList_ESTADO_GUIA")
            Dim updatelabel As UpdatePanel = page1.FindControl("UpdatePanelContenido_val_radicacion")
            Dim TextBox_RECIBIDO_GUIA_ini As TextBox = page1.FindControl("TextBox_RECIBIDO_GUIA_ini")
            Dim TextBox_FECHA_RECIBIDO_GUIA_fin As TextBox = page1.FindControl("TextBox_FECHA_RECIBIDO_GUIA_fin")
            TextBox_Id_guia_envio.Text = ""
            Limpiar_campos_consulta_guia = ""
            TextBox_Concecutivo_Guia.Text = ""
            TextBox_Fecha_Registro_Guia_ini.Text = ""
            TextBox_fecha_Registro_Guia_fin.Text = ""
            TextBox_FECHA_ENVIO_GUIA_ini.Text = ""
            TextBox_FECHA_ENVIO_GUIA_fin.Text = ""
            TextBox_NOMBRE_RAZON_SOCIA.Text = ""
            TextBox_NIT_IDENTIFICACION_2.Text = ""
            DropDownList_areas_depart.Text = "Todas"
            DropDownList_nombre_remitente.Text = ""
            DropDownList_empresa_envio.Text = ""
            DropDownList_mensajero_inerno.Text = ""
            DropDownList_ESTADO_CONFIRMACION_GUIA.Text = ""
            DropDownList_ESTADO_GUIA.Text = ""
            If Not TextBox_RECIBIDO_GUIA_ini Is Nothing Then
                TextBox_RECIBIDO_GUIA_ini.Text = ""
            End If
            If Not TextBox_FECHA_RECIBIDO_GUIA_fin Is Nothing Then
                TextBox_FECHA_RECIBIDO_GUIA_fin.Text = ""
            End If
            Limpiar_campos_consulta_guia = "YES"
            updatelabel.Update()
        Catch ex As Exception
            Limpiar_campos_consulta_guia = "Inconsistencia genera función Limpiar_campos_consulta_guia " & ex.Message
        End Try
    End Function
    Function Limpiar_Campos_registro_guia(ByVal page1 As Page, ByVal opcion As Integer) As String
        Try
            Dim TextBox_NOMBRE_RAZON_SOCIAL As TextBox = page1.FindControl("TextBox_NOMBRE_RAZON_SOCIAL")
            Dim TextBox_DIRECCION As TextBox = page1.FindControl("TextBox_DIRECCION")
            Dim TextBox_NIT_IDENTIFICACION As TextBox = page1.FindControl("TextBox_NIT_IDENTIFICACION")
            Dim TextBox_TELEFONO As TextBox = page1.FindControl("TextBox_TELEFONO")
            Dim TextBox_CORREO_ELECTRONICO As TextBox = page1.FindControl("TextBox_CORREO_ELECTRONICO")
            Dim TextBox_ANEXO As TextBox = page1.FindControl("TextBox_ANEXO")
            Dim UpdatePanel_procesa_tramite_envio As UpdatePanel = page1.FindControl("UpdatePanel_procesa_tramite_envio")
            TextBox_NOMBRE_RAZON_SOCIAL.Text = ""
            TextBox_DIRECCION.Text = ""
            TextBox_NIT_IDENTIFICACION.Text = ""
            TextBox_TELEFONO.Text = ""
            TextBox_CORREO_ELECTRONICO.Text = ""
            TextBox_ANEXO.Text = ""
            If opcion = 1 Then
                UpdatePanel_procesa_tramite_envio.Update()
            End If
            Limpiar_Campos_registro_guia = "YES"
        Catch ex As Exception
            Limpiar_Campos_registro_guia = "Función Limpiar_Campos_registro_guia " & ex.Message
        End Try
    End Function
    Function Limpiar_campos_consulta(ByRef page1 As Page) As String
        '-----------------------------------------------------
        'Funcion : Limpia los campos de consulta 
        'Fecha 2016-04-22
        'Ing : Miguel Angel Urueta Miranda
        '-----------------------------------------------------
        Try
            Dim updatelabel As UpdatePanel = page1.FindControl("UpdatePanelContenido_val_radicacion")
            Dim DropDownList_areas_depart As DropDownList = page1.FindControl("DropDownList_areas_depart")
            Dim DropDownList_empresa_envio As DropDownList = page1.FindControl("DropDownList_empresa_envio")
            Dim TextBox_fecha_ini As TextBox = page1.FindControl("TextBox_fecha_ini")
            Dim TextBox_fecha_fin As TextBox = page1.FindControl("TextBox_fecha_fin")
            Dim TextBox_fecha_resp_ini As TextBox = page1.FindControl("TextBox_fecha_resp_ini")
            Dim TextBox_fecha_resp_fin As TextBox = page1.FindControl("TextBox_fecha_resp_fin")
            Dim TextBoxRadicado As TextBox = page1.FindControl("TextBoxRadicado")
            Dim TextBoxRadicado_respuesta As TextBox = page1.FindControl("TextBoxRadicado_respuesta")
            Dim TextBoxUSUARIO_RESPONSABLE As TextBox = page1.FindControl("TextBoxUSUARIO_RESPONSABLE")
            Dim TextBoxDESTINATARIO As TextBox = page1.FindControl("TextBoxDESTINATARIO")
            Dim TextBox_GUIA_ENVIO As TextBox = page1.FindControl("TextBox_GUIA_ENVIO")
            If DropDownList_areas_depart.Items.Count > 0 Then
                DropDownList_areas_depart.Text = "Todas"
            End If
            If DropDownList_empresa_envio.Items.Count > 0 Then
                DropDownList_empresa_envio.Text = ""
            End If
            TextBox_fecha_ini.Text = ""
            TextBox_fecha_fin.Text = ""
            TextBox_fecha_resp_ini.Text = ""
            TextBox_fecha_resp_fin.Text = ""
            TextBoxRadicado.Text = ""
            TextBoxRadicado_respuesta.Text = ""
            TextBoxUSUARIO_RESPONSABLE.Text = ""
            TextBoxDESTINATARIO.Text = ""
            TextBox_GUIA_ENVIO.Text = ""
            updatelabel.Update()
            Limpiar_campos_consulta = "YES"
        Catch ex As Exception
            Limpiar_campos_consulta = "Inconsistencia función Limpiar_campos_consulta " & ex.Message
        End Try
    End Function
    Public Function SaveFile(ByVal Name As String, ByRef Content As Byte()) As String
        Dim objFstream As FileStream
        Try
            objFstream = File.Open(Name, FileMode.Create, FileAccess.Write)
            Dim lngLen As Long = Content.Length
            objFstream.Write(Content, 0, CInt(lngLen))
            objFstream.Flush()
            objFstream.Close()
            Return "YES"
        Catch exp As Exception
            SaveFile = "Funcion Save file Exception: " & exp.ToString()

        Finally

            objFstream.Close()
        End Try
    End Function
    Function ReadFile(ByVal FilePath1 As String, _
                      ByRef Filebyte1 As Byte()) As String
        Dim fs As FileStream
        Try
            ' Read file and return contents
            If File.Exists(FilePath1) = True Then
                File.SetAttributes(FilePath1, FileAttributes.Normal)
            Else
                Return "imposible encontrar archivo temporal, revice firma " & FilePath1
                Exit Function
            End If
            fs = File.Open(FilePath1, FileMode.Open, FileAccess.Read)
            Dim lngLen As Long = fs.Length
            Dim abytBuffer(CInt(lngLen - 1)) As Byte
            fs.Read(abytBuffer, 0, CInt(lngLen))
            Filebyte1 = abytBuffer
            Return "YES"
        Catch exp As Exception
            Return "Funcion ReadFile " & exp.Message
        Finally
            If Not fs Is Nothing Then
                fs.Close()
            End If
        End Try
    End Function

    Function Lista_guia_envio_correspondencia(ByRef page1 As Page) As String
        Try
            Dim hdnEmailID_VAL As Object = page1.FindControl("hdnEmailID_VAL")
            Dim scripma As GridView = page1.FindControl("GridView_val_radicacion")
            Dim labetitle As Label = page1.FindControl("titulo_label_val_radicacion")
            Dim updatelabel As UpdatePanel = page1.FindControl("UpdatePanelabel_val_radicacion")
            Dim DropDownList_areas_depart As DropDownList = page1.FindControl("DropDownList_areas_depart")
            Dim DropDownList_nombre_remitente As DropDownList = page1.FindControl("DropDownList_nombre_remitente")
            Dim DropDownList_empresa_envio As DropDownList = page1.FindControl("DropDownList_empresa_envio")
            Dim DropDownList_mensajero_inerno As DropDownList = page1.FindControl("DropDownList_mensajero_inerno")
            Dim TextBox_Id_guia_envio As TextBox = page1.FindControl("TextBox_Id_guia_envio")
            Dim TextBox_Concecutivo_Guia As TextBox = page1.FindControl("TextBox_Concecutivo_Guia")
            Dim TextBox_Fecha_Registro_Guia_ini As TextBox = page1.FindControl("TextBox_Fecha_Registro_Guia_ini")
            Dim TextBox_fecha_Registro_Guia_fin As TextBox = page1.FindControl("TextBox_fecha_Registro_Guia_fin")
            Dim TextBox_FECHA_ENVIO_GUIA_ini As TextBox = page1.FindControl("TextBox_FECHA_ENVIO_GUIA_ini")
            Dim TextBox_FECHA_ENVIO_GUIA_fin As TextBox = page1.FindControl("TextBox_FECHA_ENVIO_GUIA_fin")
            Dim TextBox_NOMBRE_RAZON_SOCIA As TextBox = page1.FindControl("TextBox_NOMBRE_RAZON_SOCIA")
            Dim TextBox_NIT_IDENTIFICACION_2 As TextBox = page1.FindControl("TextBox_NIT_IDENTIFICACION_2")
            Dim DropDownList_ESTADO_CONFIRMACION_GUIA As DropDownList = page1.FindControl("DropDownList_ESTADO_CONFIRMACION_GUIA")
            Dim DropDownList_ESTADO_GUIA As DropDownList = page1.FindControl("DropDownList_ESTADO_GUIA")
            Dim updat As UpdatePanel = page1.Page.FindControl("UpdatePanel_conenido_grid_val_radicacion")
            Dim DropDownList_tipo_guia As DropDownList = page1.FindControl("DropDownList_tipo_guia")
            Dim TextBox_RECIBIDO_GUIA_ini As TextBox = page1.FindControl("TextBox_RECIBIDO_GUIA_ini")
            Dim TextBox_FECHA_RECIBIDO_GUIA_fin As TextBox = page1.FindControl("TextBox_FECHA_RECIBIDO_GUIA_fin")
            Dim sql_condicion As String = ""
            Dim sql_condicion_iner As String = ""
            Dim id_organigrama As Integer = 0
            Dim id_area_permitida As Integer = 0
            Dim id_areas_permitidas() As Integer
            Erase id_areas_permitidas
            Dim Reclas_registro_organigrama As New Class_registro_organigrama
            Dim Refradicado As New ClassRadicador
            Dim Result As String = ""
            If DropDownList_empresa_envio.Text <> "" Then
                Dim id_empresa As Integer = 0
                Result = Me.retorna_id_empresa_mensajeria(DropDownList_empresa_envio.Text, id_empresa)
                If Result <> "YES" Then
                    Lista_guia_envio_correspondencia = Result
                    Exit Function
                End If
                sql_condicion = sql_condicion & " and ra_empresa_envio_ID_EMPRESA_ENVIO=" & id_empresa
            End If

            If DropDownList_areas_depart.Text <> "Todas" And DropDownList_areas_depart.Text <> "" Then
                Result = Reclas_registro_organigrama.Retorna_id_organigrama_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                            id_organigrama)
                If Result <> "YES" Then
                    Lista_guia_envio_correspondencia = Result
                    Exit Function
                End If
                Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
                Result = ref_Class_areas_depart_radicacion.Retorna_id_area_usuario_gestion(id_organigrama, _
                                                                                           DropDownList_areas_depart.Text, _
                                                                                           id_area_permitida)
                If Result <> "YES" Then
                    Lista_guia_envio_correspondencia = Result
                    Exit Function
                End If
                sql_condicion = sql_condicion & " and areas_depart_radicacion_Codigo_Area=" & id_area_permitida
            Else

            End If
            Dim Ref_Class_remit_dest_interno As New Class_remit_dest_interno
            If DropDownList_nombre_remitente.Text <> "" Then
                Dim id_codigo_remitente As Integer = 0
                Result = Ref_Class_remit_dest_interno.Retorna_Id_Destinatario(DropDownList_nombre_remitente.Text, _
                                                                              id_codigo_remitente)
                If Result <> "YES" Then
                    Lista_guia_envio_correspondencia = Result
                    Exit Function
                End If
                sql_condicion = sql_condicion & " and Remit_Dest_Interno_id_Remit_Dest_Int=" & id_codigo_remitente
            End If
            If DropDownList_mensajero_inerno.Text <> "" Then
                Dim id_mensajero As Integer = 0
                Result = Ref_Class_remit_dest_interno.Retorna_Id_Destinatario(DropDownList_mensajero_inerno.Text, _
                                                                              id_mensajero)
                If Result <> "YES" Then
                    Lista_guia_envio_correspondencia = Result
                    Exit Function
                End If
                sql_condicion = sql_condicion & " and ID_MENSAJERO_INTERNO=" & id_mensajero
            End If
            If TextBox_Id_guia_envio.Text <> "" Then
                sql_condicion = sql_condicion & " and Id_guia_envio=" & Val(TextBox_Id_guia_envio.Text)
            End If
            If TextBox_Concecutivo_Guia.Text <> "" Then
                sql_condicion = sql_condicion & " and Concecutivo_Guia='" & TextBox_Concecutivo_Guia.Text & "'"
            End If
            If TextBox_Fecha_Registro_Guia_ini.Text <> "" And TextBox_fecha_Registro_Guia_fin.Text <> "" Then
                sql_condicion = sql_condicion & " and CAST(Fecha_Registro_Guia AS DATE) between '" & TextBox_Fecha_Registro_Guia_ini.Text & "' AND '" & TextBox_fecha_Registro_Guia_fin.Text & "'"
            Else
                If TextBox_Fecha_Registro_Guia_ini.Text <> "" Then
                    sql_condicion = sql_condicion & " and CAST(Fecha_Registro_Guia AS DATE) = '" & TextBox_Fecha_Registro_Guia_ini.Text & "'"
                End If
                If TextBox_fecha_Registro_Guia_fin.Text <> "" Then
                    sql_condicion = sql_condicion & " and CAST(Fecha_Registro_Guia AS DATE) = '" & TextBox_fecha_Registro_Guia_fin.Text & "'"
                End If
            End If
            If TextBox_FECHA_ENVIO_GUIA_ini.Text <> "" And TextBox_FECHA_ENVIO_GUIA_fin.Text <> "" Then
                sql_condicion = sql_condicion & " and CAST(FECHA_ENVIO_GUIA AS DATE) between '" & TextBox_FECHA_ENVIO_GUIA_ini.Text & "' AND '" & TextBox_FECHA_ENVIO_GUIA_fin.Text & "'"
            Else
                If TextBox_FECHA_ENVIO_GUIA_ini.Text <> "" Then
                    sql_condicion = sql_condicion & " and CAST(FECHA_ENVIO_GUIA AS DATE) = '" & TextBox_FECHA_ENVIO_GUIA_ini.Text & "'"
                End If
                If TextBox_FECHA_ENVIO_GUIA_fin.Text <> "" Then
                    sql_condicion = sql_condicion & " and CAST(FECHA_ENVIO_GUIA AS DATE) = '" & TextBox_FECHA_ENVIO_GUIA_fin.Text & "'"
                End If
            End If
            If TextBox_RECIBIDO_GUIA_ini.Text <> "" And TextBox_FECHA_RECIBIDO_GUIA_fin.Text <> "" Then
                sql_condicion = sql_condicion & " and CAST(FECHA_RECIBIDO_GUIA AS DATE) between '" & TextBox_RECIBIDO_GUIA_ini.Text & "' AND '" & TextBox_FECHA_RECIBIDO_GUIA_fin.Text & "'"
            Else
                If TextBox_RECIBIDO_GUIA_ini.Text <> "" Then
                    sql_condicion = sql_condicion & " and CAST(FECHA_RECIBIDO_GUIA AS DATE) = '" & TextBox_RECIBIDO_GUIA_ini.Text & "'"
                End If
                If TextBox_FECHA_RECIBIDO_GUIA_fin.Text <> "" Then
                    sql_condicion = sql_condicion & " and CAST(FECHA_RECIBIDO_GUIA AS DATE) = '" & TextBox_FECHA_RECIBIDO_GUIA_fin.Text & "'"
                End If
            End If
            If TextBox_NOMBRE_RAZON_SOCIA.Text <> "" Then
                sql_condicion = sql_condicion & " and NOMBRE_RAZON_SOCIAL='" & TextBox_NOMBRE_RAZON_SOCIA.Text & "'"
            End If
            If TextBox_NIT_IDENTIFICACION_2.Text <> "" Then
                sql_condicion = sql_condicion & " and NIT_IDENTIFICACION='" & TextBox_NIT_IDENTIFICACION_2.Text & "'"
            End If
            If DropDownList_ESTADO_CONFIRMACION_GUIA.Text <> "" Then
                Dim estado_guia As Integer = 0
                If DropDownList_ESTADO_CONFIRMACION_GUIA.Text = "Guías pendientes por enviar" Then
                    estado_guia = 1
                End If
                If DropDownList_ESTADO_CONFIRMACION_GUIA.Text = "Guías enviadas" Then
                    estado_guia = 2
                End If
                If DropDownList_ESTADO_CONFIRMACION_GUIA.Text = "Guías archivadas" Then
                    estado_guia = 3
                End If
                If DropDownList_ESTADO_CONFIRMACION_GUIA.Text = "Devolución Permanente" Then
                    estado_guia = 4
                End If
                sql_condicion = sql_condicion & " and ESTADO_CONFIRMACION_GUIA='" & estado_guia & "'"
            End If
            If DropDownList_ESTADO_GUIA.Text <> "" Then
                Dim estado_guia As Integer = 0
                If DropDownList_ESTADO_GUIA.Text = "Guías anuladas" Then
                    estado_guia = 1
                End If
                If DropDownList_ESTADO_GUIA.Text = "Guías activas" Then
                    estado_guia = 0
                End If
                sql_condicion = sql_condicion & " and RRR.ESTADO_GUIA='" & estado_guia & "'"
            Else
                sql_condicion = sql_condicion & " and RRR.ESTADO_GUIA='" & 0 & "'"
            End If
            If DropDownList_tipo_guia.Text <> "" Then
                Dim id_tipo_guia As Integer = 0
                If DropDownList_tipo_guia.Text = "Manual" Then
                    id_tipo_guia = 1
                End If
                If DropDownList_tipo_guia.Text = "Automatica" Then
                    id_tipo_guia = 2
                End If
                sql_condicion = sql_condicion & " and ID_TIPO_MANUAL_AUTOMATICO='" & id_tipo_guia & "'"
            End If
            Dim sql_consulta As String = "SELECT RRR.ID_GUIA_ENVIO,RRR.CONCECUTIVO_GUIA,RRR.NOMBRE_RAZON_SOCIAL," & _
               "REE.NOMBRE_EMPRESA,RDA.NOMBRE_REMITENTE,RDI.NOMBRE_REMITENTE AS MENSAJERO,REG.Descripcion_estado_guia AS ESTADO_ENVIO,CAST(RRR.FECHA_RECIBIDO_GUIA AS DATE) AS FECHA_RECIBIDO_GUIA,RRR.FECHA_ENVIO_GUIA,RRR.TIEMPO_RESPUESTA" & _
               " FROM  ra_guia_interna AS RRR " & _
               " LEFT OUTER JOIN ra_empresa_envio AS REE ON  (REE.ID_EMPRESA_ENVIO=RRR.ra_empresa_envio_ID_EMPRESA_ENVIO) " & _
               " LEFT OUTER JOIN remit_dest_interno AS RDA ON (RDA.id_Remit_Dest_Int=RRR.Remit_Dest_Interno_id_Remit_Dest_Int) " & _
               " LEFT OUTER JOIN ra_estados_guia_interna AS REG ON (REG.Estado_guia=RRR.ESTADO_CONFIRMACION_GUIA) " & _
               " LEFT OUTER JOIN remit_dest_interno AS RDI ON (RDI.id_Remit_Dest_Int=RRR.ID_MENSAJERO_INTERNO) " & sql_condicion_iner & "  where not Id_guia_envio is null  " & sql_condicion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_guia_envio_correspondencia = "Imposible Encontrar datos para listar " & Result
                labetitle.Text = "Se encontraron " & "0" & " registro(s) de guías  "
                scripma.DataSource = Nothing
                'scripma.DataKeyNames = DataKey
                hdnEmailID_VAL.value = "-1"
                scripma.DataBind()
                updat.Update()
                updatelabel.Update()
                Exit Function

            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                labetitle.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) de guías "
                scripma.DataSource = Nothing
                'scripma.DataKeyNames = DataKey
                hdnEmailID_VAL.value = "-1"
                scripma.DataBind()
                updat.Update()
                updatelabel.Update()
                Lista_guia_envio_correspondencia = "YES"
                Exit Function
            Else
                HttpContext.Current.Session.Item("RA_DATO_CONSULTA") = sql_consulta
                labetitle.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) de guías "
                'scripma.DataKeyNames = DataKey
                hdnEmailID_VAL.value = "-1"
                scripma.DataSource = Datset
                scripma.DataBind()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    scripma.Rows(i).Attributes.Add("id", scripma.Rows(i).Cells(0).Text.ToString())
                Next
                updat.Update()
                updatelabel.Update()
                Lista_guia_envio_correspondencia = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_guia_envio_correspondencia = "Inconsistencia función Lista_guia_envio_correspondencia " & ex.Message
        End Try
    End Function
    Function Lista_guia_envio_correspondencia_por_procesar(ByRef page1 As Page) As String
        Try
            Dim hdnEmailID_VAL As Object = page1.FindControl("hdnEmailID_VAL")
            Dim scripma As GridView = page1.FindControl("GridView_val_radicacion")
            Dim labetitle As Label = page1.FindControl("titulo_label_val_radicacion")
            Dim updatelabel As UpdatePanel = page1.FindControl("UpdatePanelabel_val_radicacion")
            Dim DropDownList_areas_depart As DropDownList = page1.FindControl("DropDownList_areas_depart")
            Dim DropDownList_nombre_remitente As DropDownList = page1.FindControl("DropDownList_nombre_remitente")
            Dim DropDownList_empresa_envio As DropDownList = page1.FindControl("DropDownList_empresa_envio")
            Dim DropDownList_mensajero_inerno As DropDownList = page1.FindControl("DropDownList_mensajero_inerno")
            Dim TextBox_Id_guia_envio As TextBox = page1.FindControl("TextBox_Id_guia_envio")
            Dim TextBox_Concecutivo_Guia As TextBox = page1.FindControl("TextBox_Concecutivo_Guia")
            Dim TextBox_Fecha_Registro_Guia_ini As TextBox = page1.FindControl("TextBox_Fecha_Registro_Guia_ini")
            Dim TextBox_fecha_Registro_Guia_fin As TextBox = page1.FindControl("TextBox_fecha_Registro_Guia_fin")
            Dim TextBox_FECHA_ENVIO_GUIA_ini As TextBox = page1.FindControl("TextBox_FECHA_ENVIO_GUIA_ini")
            Dim TextBox_FECHA_ENVIO_GUIA_fin As TextBox = page1.FindControl("TextBox_FECHA_ENVIO_GUIA_fin")
            Dim TextBox_NOMBRE_RAZON_SOCIA As TextBox = page1.FindControl("TextBox_NOMBRE_RAZON_SOCIA")
            Dim TextBox_NIT_IDENTIFICACION_2 As TextBox = page1.FindControl("TextBox_NIT_IDENTIFICACION_2")
            Dim DropDownList_ESTADO_CONFIRMACION_GUIA As DropDownList = page1.FindControl("DropDownList_ESTADO_CONFIRMACION_GUIA")
            Dim DropDownList_ESTADO_GUIA As DropDownList = page1.FindControl("DropDownList_ESTADO_GUIA")
            Dim updat As UpdatePanel = page1.Page.FindControl("UpdatePanel_conenido_grid_val_radicacion")
            Dim sql_condicion As String = ""
            Dim sql_condicion_iner As String = ""
            Dim id_organigrama As Integer = 0
            Dim id_area_permitida As Integer = 0
            Dim id_areas_permitidas() As Integer
            Erase id_areas_permitidas
            Dim Reclas_registro_organigrama As New Class_registro_organigrama
            Dim Refradicado As New ClassRadicador
            Dim Result As String = ""
            If DropDownList_empresa_envio.Text <> "" Then
                Dim id_empresa As Integer = 0
                Result = Me.retorna_id_empresa_mensajeria(DropDownList_empresa_envio.Text, id_empresa)
                If Result <> "YES" Then
                    Lista_guia_envio_correspondencia_por_procesar = Result
                    Exit Function
                End If
                sql_condicion = sql_condicion & " and ra_empresa_envio_ID_EMPRESA_ENVIO=" & id_empresa
            End If

            If DropDownList_areas_depart.Text <> "Todas" And DropDownList_areas_depart.Text <> "" Then
                Result = Reclas_registro_organigrama.Retorna_id_organigrama_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                            id_organigrama)
                If Result <> "YES" Then
                    Lista_guia_envio_correspondencia_por_procesar = Result
                    Exit Function
                End If
                Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
                Result = ref_Class_areas_depart_radicacion.Retorna_id_area_usuario_gestion(id_organigrama, _
                                                                                          DropDownList_areas_depart.Text, _
                                                                                          id_area_permitida)
                If Result <> "YES" Then
                    Lista_guia_envio_correspondencia_por_procesar = Result
                    Exit Function
                End If
                sql_condicion = sql_condicion & " and areas_depart_radicacion_Codigo_Area=" & id_area_permitida
            Else

            End If
            Dim Ref_Class_remit_dest_interno As New Class_remit_dest_interno
            If DropDownList_nombre_remitente.Text <> "" Then
                Dim id_codigo_remitente As Integer = 0
                Result = Ref_Class_remit_dest_interno.Retorna_Id_Destinatario(DropDownList_nombre_remitente.Text, _
                                                                              id_codigo_remitente)
                If Result <> "YES" Then
                    Lista_guia_envio_correspondencia_por_procesar = Result
                    Exit Function
                End If
                sql_condicion = sql_condicion & " and Remit_Dest_Interno_id_Remit_Dest_Int=" & id_codigo_remitente
            End If
            If DropDownList_mensajero_inerno.Text <> "" Then
                Dim id_mensajero As Integer = 0
                Result = Ref_Class_remit_dest_interno.Retorna_Id_Destinatario(DropDownList_mensajero_inerno.Text, _
                                                                              id_mensajero)
                If Result <> "YES" Then
                    Lista_guia_envio_correspondencia_por_procesar = Result
                    Exit Function
                End If
                sql_condicion = sql_condicion & " and ID_MENSAJERO_INTERNO=" & id_mensajero
            End If
            If TextBox_Id_guia_envio.Text <> "" Then
                sql_condicion = sql_condicion & " and Id_guia_envio=" & Val(TextBox_Id_guia_envio.Text)
            End If
            If TextBox_Concecutivo_Guia.Text <> "" Then
                sql_condicion = sql_condicion & " and Concecutivo_Guia='" & TextBox_Concecutivo_Guia.Text & "'"
            End If
            If TextBox_Fecha_Registro_Guia_ini.Text <> "" And TextBox_fecha_Registro_Guia_fin.Text <> "" Then
                sql_condicion = sql_condicion & " and CAST(Fecha_Registro_Guia AS DATE) between '" & TextBox_Fecha_Registro_Guia_ini.Text & "' AND '" & TextBox_fecha_Registro_Guia_fin.Text & "'"
            Else
                If TextBox_Fecha_Registro_Guia_ini.Text <> "" Then
                    sql_condicion = sql_condicion & " and CAST(Fecha_Registro_Guia AS DATE) = '" & TextBox_Fecha_Registro_Guia_ini.Text & "'"
                End If
                If TextBox_fecha_Registro_Guia_fin.Text <> "" Then
                    sql_condicion = sql_condicion & " and CAST(Fecha_Registro_Guia AS DATE) = '" & TextBox_fecha_Registro_Guia_fin.Text & "'"
                End If
            End If
            If TextBox_FECHA_ENVIO_GUIA_ini.Text <> "" And TextBox_FECHA_ENVIO_GUIA_fin.Text <> "" Then
                sql_condicion = sql_condicion & " and CAST(FECHA_ENVIO_GUIA AS DATE) between '" & TextBox_FECHA_ENVIO_GUIA_ini.Text & "' AND '" & TextBox_FECHA_ENVIO_GUIA_fin.Text & "'"
            Else
                If TextBox_FECHA_ENVIO_GUIA_ini.Text <> "" Then
                    sql_condicion = sql_condicion & " and CAST(FECHA_ENVIO_GUIA AS DATE) = '" & TextBox_FECHA_ENVIO_GUIA_ini.Text & "'"
                End If
                If TextBox_FECHA_ENVIO_GUIA_fin.Text <> "" Then
                    sql_condicion = sql_condicion & " and CAST(FECHA_ENVIO_GUIA AS DATE) = '" & TextBox_FECHA_ENVIO_GUIA_fin.Text & "'"
                End If
            End If
            If TextBox_NOMBRE_RAZON_SOCIA.Text <> "" Then
                sql_condicion = sql_condicion & " and NOMBRE_RAZON_SOCIAL='" & TextBox_NOMBRE_RAZON_SOCIA.Text & "'"
            End If
            If TextBox_NIT_IDENTIFICACION_2.Text <> "" Then
                sql_condicion = sql_condicion & " and NIT_IDENTIFICACION='" & TextBox_NIT_IDENTIFICACION_2.Text & "'"
            End If
            If DropDownList_ESTADO_CONFIRMACION_GUIA.Text <> "" Then
                Dim estado_guia As Integer = 0
                If DropDownList_ESTADO_CONFIRMACION_GUIA.Text = "Guías pendientes por enviar" Then
                    estado_guia = 1
                End If
                If DropDownList_ESTADO_CONFIRMACION_GUIA.Text = "Guías enviadas" Then
                    estado_guia = 2
                End If
                If DropDownList_ESTADO_CONFIRMACION_GUIA.Text = "Guías archivadas" Then
                    estado_guia = 3
                End If
                sql_condicion = sql_condicion & " and ESTADO_CONFIRMACION_GUIA='" & estado_guia & "'"
            End If
            If DropDownList_ESTADO_GUIA.Text <> "" Then
                Dim estado_guia As Integer = 0
                If DropDownList_ESTADO_GUIA.Text = "Guías anuladas" Then
                    estado_guia = 1
                End If
                If DropDownList_ESTADO_GUIA.Text = "Guías activas" Then
                    estado_guia = 0
                End If
                sql_condicion = sql_condicion & " and ESTADO_GUIA='" & estado_guia & "'"
            End If
            Dim sql_consulta As String = "SELECT RRR.ID_GUIA_ENVIO,RRR.CONCECUTIVO_GUIA,RRR.NOMBRE_RAZON_SOCIAL," & _
               "REE.NOMBRE_EMPRESA,RDA.NOMBRE_REMITENTE,RDI.NOMBRE_REMITENTE AS MENSAJERO,REG.Descripcion_estado_guia AS ESTADO_ENVIO,CAST(RRR.FECHA_RECIBIDO_GUIA AS DATE) AS FECHA_RECIBIDO_GUIA" & _
               " FROM  ra_guia_interna AS RRR " & _
               " LEFT OUTER JOIN ra_empresa_envio AS REE ON  (REE.ID_EMPRESA_ENVIO=RRR.ra_empresa_envio_ID_EMPRESA_ENVIO) " & _
               " LEFT OUTER JOIN remit_dest_interno AS RDA ON (RDA.id_Remit_Dest_Int=RRR.Remit_Dest_Interno_id_Remit_Dest_Int) " & _
               " LEFT OUTER JOIN ra_estados_guia_interna AS REG ON (REG.Estado_guia=RRR.ESTADO_CONFIRMACION_GUIA) " & _
               " LEFT OUTER JOIN remit_dest_interno AS RDI ON (RDI.id_Remit_Dest_Int=RRR.ID_MENSAJERO_INTERNO) " & sql_condicion_iner & "  where not Id_guia_envio is null and ID_TIPO_MANUAL_AUTOMATICO=2 and ESTADO_CONFIRMACION_GUIA > 1 and RRR.ESTADO_GUIA=0 AND TIEMPO_RESPUESTA IS NULL " & sql_condicion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_guia_envio_correspondencia_por_procesar = "Imposible Encontrar datos para listar " & Result
                labetitle.Text = "Se encontraron " & "0" & " registro(s) de guías  "
                scripma.DataSource = Nothing
                'scripma.DataKeyNames = DataKey
                hdnEmailID_VAL.value = "-1"
                scripma.DataBind()
                updat.Update()
                updatelabel.Update()
                Exit Function

            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                labetitle.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) de guías "
                scripma.DataSource = Nothing
                'scripma.DataKeyNames = DataKey
                hdnEmailID_VAL.value = "-1"
                scripma.DataBind()
                updat.Update()
                updatelabel.Update()
                Lista_guia_envio_correspondencia_por_procesar = "YES"
                Exit Function
            Else
                HttpContext.Current.Session.Item("RA_DATO_CONSULTA") = sql_consulta
                labetitle.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) de guías "
                'scripma.DataKeyNames = DataKey
                hdnEmailID_VAL.value = "-1"
                scripma.DataSource = Datset
                scripma.DataBind()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    scripma.Rows(i).Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                Next
                updat.Update()
                updatelabel.Update()
                Lista_guia_envio_correspondencia_por_procesar = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_guia_envio_correspondencia_por_procesar = "Inconsistencia función Lista_guia_envio_correspondencia_por_procesar " & ex.Message
        End Try
    End Function
    Function Lista_tramites_por_enviar(ByRef page1 As Page) As String

        Try
            Dim Result As String = ""
            Dim hdnEmailID_VAL As Object = page1.FindControl("hdnEmailID_VAL")
            Dim scripma As GridView = page1.FindControl("GridView_val_radicacion")
            Dim labetitle As Label = page1.FindControl("titulo_label_val_radicacion")
            Dim updatelabel As UpdatePanel = page1.FindControl("UpdatePanelabel_val_radicacion")
            Dim DropDownList_areas_depart As DropDownList = page1.FindControl("DropDownList_areas_depart")
            Dim DropDownList_empresa_envio As DropDownList = page1.FindControl("DropDownList_empresa_envio")
            Dim TextBox_fecha_ini As TextBox = page1.FindControl("TextBox_fecha_ini")
            Dim TextBox_fecha_fin As TextBox = page1.FindControl("TextBox_fecha_fin")
            Dim TextBox_fecha_resp_ini As TextBox = page1.FindControl("TextBox_fecha_resp_ini")
            Dim TextBox_fecha_resp_fin As TextBox = page1.FindControl("TextBox_fecha_resp_fin")
            Dim TextBoxRadicado As TextBox = page1.FindControl("TextBoxRadicado")
            Dim TextBoxRadicado_respuesta As TextBox = page1.FindControl("TextBoxRadicado_respuesta")
            Dim TextBoxUSUARIO_RESPONSABLE As TextBox = page1.FindControl("TextBoxUSUARIO_RESPONSABLE")
            Dim TextBoxDESTINATARIO As TextBox = page1.FindControl("TextBoxDESTINATARIO")
            Dim TextBox_GUIA_ENVIO As TextBox = page1.FindControl("TextBox_GUIA_ENVIO")
            If scripma Is Nothing Then
                Lista_tramites_por_enviar = "Imposible encontrar datagrid GridView_val_radicacion"
                Exit Function
            End If
            If labetitle Is Nothing Then
                Lista_tramites_por_enviar = "Imposible encontrar el control   titulo_label"
                Exit Function
            End If
            If updatelabel Is Nothing Then
                Lista_tramites_por_enviar = "Imposible encontrar el control  UpdatePanelabel_validacion"
                Exit Function
            End If
            Dim updat As UpdatePanel = page1.Page.FindControl("UpdatePanel_conenido_grid_val_radicacion")
            If updatelabel Is Nothing Then
                Lista_tramites_por_enviar = "Imposible encontrar el control  UpdatePanel_conenido_grid_val_radicacion"
                Exit Function
            End If
            Dim sql_condicion As String = ""
            Dim sql_condicion_iner As String = ""
            Dim id_organigrama As Integer = 0
            Dim id_area_permitida As Integer = 0
            Dim id_areas_permitidas() As Integer
            Erase id_areas_permitidas
            Dim Refclasgestiond As New ClassGestionDocumental
            Dim Reclas_registro_organigrama As New Class_registro_organigrama
            If DropDownList_empresa_envio.Text <> "" Then
                sql_condicion = sql_condicion & " and EMPRESA_ENVIO='" & DropDownList_empresa_envio.Text & "'"
            End If
            If DropDownList_areas_depart.Text = "" Then
                Lista_tramites_por_enviar = "El usuarios de gestión no tiene relacionas áreas  para enviar, consulte a su administrador"
                Exit Function
            End If
            If DropDownList_areas_depart.Text <> "Todas" Then
                Result = Reclas_registro_organigrama.Retorna_id_organigrama_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                            id_organigrama)
                If Result <> "YES" Then
                    Lista_tramites_por_enviar = Result
                    Exit Function
                End If
                Result = Me.Retorna_id_area_permitida_para_envio(DropDownList_areas_depart.Text, id_organigrama, id_area_permitida)
                If Result <> "YES" Then
                    Lista_tramites_por_enviar = Result
                    Exit Function
                End If
                sql_condicion = sql_condicion & " and ID_AREA=" & id_area_permitida
            Else
                sql_condicion_iner = "  INNER JOIN ra_area_departamento_permitida_usuario_gestion_resp AS ADP ON " & _
                " (ADP.AREA_ARCHIVO_ID_AREA=RRR.ID_AREA AND ADP.remit_dest_interno_id_Remit_Dest_Int=" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & ")"
            End If

            If TextBox_fecha_ini.Text <> "" And TextBox_fecha_fin.Text <> "" Then
                sql_condicion = sql_condicion & " and CAST(FECHA_VENCE AS DATE) between '" & TextBox_fecha_ini.Text & "' AND '" & TextBox_fecha_fin.Text & "'"
            Else
                If TextBox_fecha_ini.Text <> "" Then
                    sql_condicion = sql_condicion & " and CAST(FECHA_VENCE AS DATE) = '" & TextBox_fecha_ini.Text & "'"
                End If
                If TextBox_fecha_fin.Text <> "" Then
                    sql_condicion = sql_condicion & " and CAST(FECHA_VENCE AS DATE) = '" & TextBox_fecha_fin.Text & "'"
                End If
            End If
            If TextBox_fecha_resp_ini.Text <> "" And TextBox_fecha_resp_fin.Text <> "" Then
                sql_condicion = sql_condicion & " and CAST(FECHA_RESPUETA AS DATE) between '" & TextBox_fecha_resp_ini.Text & "' AND '" & TextBox_fecha_resp_fin.Text & "'"
            Else
                If TextBox_fecha_resp_ini.Text <> "" Then
                    sql_condicion = sql_condicion & " and CAST(FECHA_RESPUETA AS DATE) = '" & TextBox_fecha_resp_ini.Text & "'"
                End If
                If TextBox_fecha_resp_fin.Text <> "" Then
                    sql_condicion = sql_condicion & " and CAST(FECHA_RESPUETA AS DATE) = '" & TextBox_fecha_resp_fin.Text & "'"
                End If
            End If
            If TextBoxRadicado.Text <> "" Then
                sql_condicion = sql_condicion & " and RADICADO='" & TextBoxRadicado.Text & "'"
            End If
            If TextBoxRadicado_respuesta.Text <> "" Then
                sql_condicion = sql_condicion & " and RADICADO_RESPUESTA='" & TextBoxRadicado_respuesta.Text & "'"
            End If
            If TextBoxUSUARIO_RESPONSABLE.Text <> "" Then
                sql_condicion = sql_condicion & " and USUARIO_RESPONSABLE='" & TextBoxUSUARIO_RESPONSABLE.Text & "'"
            End If
            If TextBoxDESTINATARIO.Text <> "" Then
                sql_condicion = sql_condicion & " and DESTINATARIO='" & TextBoxDESTINATARIO.Text & "'"
            End If
            If TextBox_GUIA_ENVIO.Text <> "" Then
                sql_condicion = sql_condicion & " and GUIA_ENVIO='" & TextBox_GUIA_ENVIO.Text & "'"
            End If
            Dim sql_consulta As String = "SELECT RRR.ID_RESPUESTA_RADICADO,RRR.TRAMITE_DOCUMENTO,RRR.RADICADO," & _
            "RRR.RADICADO_RESPUESTA,RRR.GUIA_ENVIO,RRR.EMPRESA_ENVIO,RRR.FECHA_VENCE,RRR.FECHA_RESPUETA,RDI.Nombre_Remitente as NOMBRE_REMITENTE,RDI.Cargo_Remite as CARGO_REMITENTE,ADR.Nombre_Area as NOMBRE_AREA," & _
            "RRR.DESTINATARIO,RRR.GABINETE,RRR.ID_IMAGEN " & _
            " FROM  ra_respuesta_radicado AS RRR " & _
            " INNER JOIN remit_dest_interno AS RDI ON  (RRR.ID_REMIT_DEST_INT=RDI.id_Remit_Dest_Int) " &
            " INNER JOIN areas_depart_radicacion AS ADR ON (RRR.ID_AREA=ADR.Codigo_Area) " & sql_condicion_iner & "  where ESTADO_RESPUESTA=1 " & sql_condicion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_tramites_por_enviar = "Imposible Encontrar datos para listar " & Result
                labetitle.Text = "Se encontraron " & "0" & " registro(s) por enviar  "
                scripma.DataSource = Nothing
                'scripma.DataKeyNames = DataKey
                hdnEmailID_VAL.value = "-1"
                scripma.DataBind()
                updat.Update()
                updatelabel.Update()
                Exit Function

            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                labetitle.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) por enviar "
                scripma.DataSource = Nothing
                'scripma.DataKeyNames = DataKey
                hdnEmailID_VAL.value = "-1"
                scripma.DataBind()
                updat.Update()
                updatelabel.Update()
                Lista_tramites_por_enviar = "YES"
                Exit Function
            Else
                HttpContext.Current.Session.Item("RA_DATO_CONSULTA") = sql_consulta
                labetitle.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) por enviar "
                'scripma.DataKeyNames = DataKey
                hdnEmailID_VAL.value = "-1"
                scripma.DataSource = Datset
                scripma.DataBind()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    scripma.Rows(i).Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                Next
                updat.Update()
                updatelabel.Update()
                Lista_tramites_por_enviar = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Lista_tramites_por_enviar = ex.Message
        End Try

    End Function
    Function retorna_consecutivo_guia_respuesta_radicado(ByVal id_tramite_respuesta As Integer, ByRef id_guia_interno As String) As String
        Try
            Dim sql_consulta As String = "Select Numero_Guia_Interna from ra_respuesta_radicado where ID_RESPUESTA_RADICADO='" & id_tramite_respuesta & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                retorna_consecutivo_guia_respuesta_radicado = " función retorna_consecutivo_guia_respuesta_radicado dice  " & Result
                Exit Function
            End If
            Dim TempoEnsamble As String = ""
            If Datset.Tables(0).Rows.Count > 0 Then
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    id_guia_interno = 0
                Else
                    id_guia_interno = Datset.Tables(0).Rows(0).Item(0)
                End If
                retorna_consecutivo_guia_respuesta_radicado = "YES"
            Else
                retorna_consecutivo_guia_respuesta_radicado = "El consecutivo " & id_tramite_respuesta & " de respuesta no existe "
            End If
        Catch ex As Exception
            retorna_consecutivo_guia_respuesta_radicado = "Inconsistencia general función retorna_consecutivo_guia_respuesta_radicado " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_guia_respuesta_radicado(ByVal id_tramite_respuesta As Integer, ByRef estado_respuesta As String) As String
        Try
            Dim sql_consulta As String = "Select GUIA_ENVIO from ra_respuesta_radicado where ID_RESPUESTA_RADICADO='" & id_tramite_respuesta & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_guia_respuesta_radicado = " función Verifica_existencia_guia_respuesta_radicado dice  " & Result
                Exit Function
            End If
            Dim TempoEnsamble As String = ""
            If Datset.Tables(0).Rows.Count > 0 Then
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    estado_respuesta = "NO"
                Else
                    estado_respuesta = "YES"
                End If
                Verifica_existencia_guia_respuesta_radicado = "YES"
            Else
                Verifica_existencia_guia_respuesta_radicado = "El consecutivo " & id_tramite_respuesta & " de respuesta no existe "
            End If
        Catch ex As Exception
            Verifica_existencia_guia_respuesta_radicado = "Inconsistencia general función Verifica_existencia_guia_respuesta_radicado " & ex.Message
        End Try
    End Function
    Function verifica_existencia_radicado_respuesta_envio(ByVal consecutivo_radicado As String) As String
        Try
            Dim sql_consulta As String = "Select Flag_Flow from ra_registro_general_radicacion where Consecutivo_Rad='" & consecutivo_radicado & "'" & _
                " and Flag_Flow=777"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                verifica_existencia_radicado_respuesta_envio = " función verifica_existencia_radicado_respuesta_envio dice  " & Result
                Exit Function
            End If
            Dim TempoEnsamble As String = ""
            If Datset.Tables(0).Rows.Count > 0 Then

                verifica_existencia_radicado_respuesta_envio = "YES"
            Else
                verifica_existencia_radicado_respuesta_envio = "El consecutivo radicado no existen en la tabla general"
            End If
        Catch ex As Exception
            verifica_existencia_radicado_respuesta_envio = "Inconsistencia general función verifica_existencia_radicado_respuesta_envio " & ex.Message
        End Try
    End Function

    Function Retorna_nombre_plantilla_destinatario_guia(ByRef nombre_plantilla_guia As String, ByRef id_plantilla As Integer) As String
        '-----------------------------------------------------------
        'Funcion : Lista nombre plantilla guia destinatario 
        'Fecha : 2016-04-27
        'Ing : Miguel Angel Urueta Miranda
        '------------------------------------------------------------
        Try
            Dim sql_consulta As String = "Select Nombre_Plantilla,Id_Plantilla_Validacion from plantilla_validacion " & _
                  " where PLANTILLA_DEFAULT_GUIA=1"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_nombre_plantilla_destinatario_guia = " Error Listando nombre plantilla guía   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                nombre_plantilla_guia = Datset.Tables(0).Rows(0).Item(0)
                id_plantilla = Datset.Tables(0).Rows(0).Item(1)
                Retorna_nombre_plantilla_destinatario_guia = "YES"
            Else
                Retorna_nombre_plantilla_destinatario_guia = "Imposible encontrar plantilla validacion por favor active la opcion defalut de la plantilla"
            End If
        Catch ex As Exception
            Retorna_nombre_plantilla_destinatario_guia = "Inconsistencia función Retorna_nombre_plantilla_destinatario_guia " & ex.Message
        End Try
    End Function
    Function Retorna_campos_plantilla_vald_guia(ByRef stru_valores_campo() As stru_campos_destinatario, ByVal id_plantilla As Integer) As String
        Try
            Dim sql_consulta As String = "Select Nombre_Campo,CAMPO_RELACION_GUIA,Campo_Primari_key from campos_plantilla_validacion " & _
                     " where Plantilla_Validacion_Id_Plantilla_Validacion=" & id_plantilla & " and not CAMPO_RELACION_GUIA  is null "
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_campos_plantilla_vald_guia = " Error Listando nombre plantilla guía   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_valores_campo(i)
                    stru_valores_campo(i).nombre_campo_fuente = Datset.Tables(0).Rows(i).Item(0)
                    stru_valores_campo(i).nombre_campo_destino = Datset.Tables(0).Rows(i).Item(1)
                    stru_valores_campo(i).campo_identi = Datset.Tables(0).Rows(i).Item(2)
                Next

                Retorna_campos_plantilla_vald_guia = "YES"
                Exit Function
            Else
                Retorna_campos_plantilla_vald_guia = "Imposible encontrar campos plantilla validacion por favor active la opcion defalut de la plantilla y agregue los campos"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_campos_plantilla_vald_guia = "Inconsistencia función Retorna_campos_plantilla_vald_guia " & ex.Message
        End Try
    End Function
    Function Retorna_dato_guia_envio_plantilla(ByRef stru_valores_campo() As stru_campos_destinatario, _
                                               ByVal id_dest As Integer, ByVal nombre_plantilla As String) As String
        Try
            Dim campos As String = ""
            Dim campo_identi As String = ""
            For i As Integer = 0 To stru_valores_campo.Length - 1
                If i = 0 Then
                    campos = stru_valores_campo(i).nombre_campo_fuente
                Else
                    campos = campos & "," & stru_valores_campo(i).nombre_campo_fuente
                End If
                If stru_valores_campo(i).campo_identi = 1 Then
                    campo_identi = stru_valores_campo(i).nombre_campo_fuente
                End If
            Next
            If campo_identi = "" Then
                Retorna_dato_guia_envio_plantilla = "Imposible encontrar campo identi en plantilla validación " & nombre_plantilla
                Exit Function
            End If
            Dim sqlconsulta As String = "Select " & campos & " from " & nombre_plantilla & " where " & campo_identi & "=" & id_dest
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Retorna_dato_guia_envio_plantilla = " Error Listando nombre plantilla guía   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To stru_valores_campo.Length - 1
                    If Datset.Tables(0).Rows(0).IsNull(i) = False Then
                        stru_valores_campo(i).valor_campo_fuente = Datset.Tables(0).Rows(0).Item(i)
                    Else
                        stru_valores_campo(i).valor_campo_fuente = ""
                    End If

                Next
            End If
            Retorna_dato_guia_envio_plantilla = "YES"
            Exit Function
        Catch ex As Exception
            Retorna_dato_guia_envio_plantilla = "Inconsistencia general función Retorna_dato_guia_envio_plantilla " & ex.Message
        End Try
    End Function
    Function Retorna_dato_guia_envio_plantilla_nombre(ByRef stru_valores_campo() As stru_campos_destinatario, _
                                              ByVal nombre_remitente As String, ByVal nombre_plantilla As String) As String
        Try
            Dim campos As String = ""
            Dim campo_identi As String = ""
            For i As Integer = 0 To stru_valores_campo.Length - 1
                If i = 0 Then
                    campos = stru_valores_campo(i).nombre_campo_fuente
                Else
                    campos = campos & "," & stru_valores_campo(i).nombre_campo_fuente
                End If
                If stru_valores_campo(i).campo_identi = 1 Then
                    campo_identi = stru_valores_campo(i).nombre_campo_fuente
                End If
            Next
            If campo_identi = "" Then
                Retorna_dato_guia_envio_plantilla_nombre = "Imposible encontrar campo idneti en plantilla validación " & nombre_plantilla
                Exit Function
            End If
            Dim sqlconsulta As String = "Select " & campos & " from " & nombre_plantilla & " where " & " Nombre_Remitente " & "='" & nombre_remitente & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Retorna_dato_guia_envio_plantilla_nombre = " Error Listando nombre plantilla guía   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To stru_valores_campo.Length - 1
                    stru_valores_campo(i).valor_campo_fuente = Datset.Tables(0).Rows(0).Item(i)
                Next
            End If
            Retorna_dato_guia_envio_plantilla_nombre = "YES"
            Exit Function
        Catch ex As Exception
            Retorna_dato_guia_envio_plantilla_nombre = "Inconsistencia general función Retorna_dato_guia_envio_plantilla_nombre " & ex.Message
        End Try
    End Function

    Function Auto_completar_registrar_guia(ByRef pag1 As Page) As String
        Try
            Dim nombre_plantilla As String = ""
            Dim id_plantilla As Integer = 0
            Dim Result As String = ""
            Dim stru_valores_campo() As stru_campos_destinatario = Nothing
            Dim _Panelvalidacion_val_radicacion As Panel = pag1.FindControl("_Panelvalidacion_val_radicacion")
            Result = Me.Retorna_nombre_plantilla_destinatario_guia(nombre_plantilla, id_plantilla)
            If Result <> "YES" Then
                Auto_completar_registrar_guia = Result
                Exit Function
            End If
            Result = Me.Retorna_campos_plantilla_vald_guia(stru_valores_campo, id_plantilla)
            If Result <> "YES" Then
                Auto_completar_registrar_guia = Result
                Exit Function
            End If
            For i As Integer = 0 To stru_valores_campo.Length - 1
                Dim reftext As TextBox = Nothing
                reftext = pag1.FindControl("TextBox_" & stru_valores_campo(i).nombre_campo_destino)
                If Not reftext Is Nothing Then
                    Dim refclas_radic As New ClassRadicador
                    Result = refclas_radic.agregar_auto_complete(reftext.ID.ToString, _Panelvalidacion_val_radicacion, "GetGuiaRadicaconasp", nombre_plantilla, stru_valores_campo(i).nombre_campo_fuente)
                    If Result <> "YES" Then
                        Auto_completar_registrar_guia = Result
                        Exit Function
                    End If
                End If
            Next
            Auto_completar_registrar_guia = "YES"
        Catch ex As Exception
            Auto_completar_registrar_guia = "Inconsistencia función Auto_completar_registrar_guia " & ex.Message
        End Try
    End Function
    Function Auto_completar_editar_guia(ByRef pag1 As Page) As String
        Try
            Dim nombre_plantilla As String = ""
            Dim id_plantilla As Integer = 0
            Dim Result As String = ""
            Dim stru_valores_campo() As stru_campos_destinatario = Nothing
            Dim _Panelvalidacion_val_radicacion As Panel = pag1.FindControl("_Panelvalidacion_val_radicacion_")
            Result = Me.Retorna_nombre_plantilla_destinatario_guia(nombre_plantilla, id_plantilla)
            If Result <> "YES" Then
                Auto_completar_editar_guia = Result
                Exit Function
            End If
            Result = Me.Retorna_campos_plantilla_vald_guia(stru_valores_campo, id_plantilla)
            If Result <> "YES" Then
                Auto_completar_editar_guia = Result
                Exit Function
            End If
            For i As Integer = 0 To stru_valores_campo.Length - 1
                Dim reftext As TextBox = Nothing
                reftext = pag1.FindControl("TextBox_" & stru_valores_campo(i).nombre_campo_destino)
                If Not reftext Is Nothing Then
                    Dim refclas_radic As New ClassRadicador
                    Result = refclas_radic.agregar_auto_complete(reftext.ID.ToString, _Panelvalidacion_val_radicacion, "GetGuiaRadicaconasp", nombre_plantilla, stru_valores_campo(i).nombre_campo_fuente)
                    If Result <> "YES" Then
                        Auto_completar_editar_guia = Result
                        Exit Function
                    End If
                End If
            Next
            Auto_completar_editar_guia = "YES"
        Catch ex As Exception
            Auto_completar_editar_guia = "Inconsistencia función Auto_completar_editar_guia " & ex.Message
        End Try
    End Function
    Function Retorna_datos_guia_envio_destinatario(ByRef stru_valores_campo() As stru_campos_destinatario, ByVal id_destinatario As Integer) As String
        Try
            Dim nombre_plantilla As String = ""
            Dim id_plantilla As Integer = 0
            Dim Result As String = ""
            Result = Me.Retorna_nombre_plantilla_destinatario_guia(nombre_plantilla, id_plantilla)
            If Result <> "YES" Then
                Retorna_datos_guia_envio_destinatario = Result
                Exit Function
            End If
            Result = Me.Retorna_campos_plantilla_vald_guia(stru_valores_campo, id_plantilla)
            If Result <> "YES" Then
                Retorna_datos_guia_envio_destinatario = Result
                Exit Function
            End If
            Result = Retorna_dato_guia_envio_plantilla(stru_valores_campo, id_destinatario, nombre_plantilla)
            If Result <> "YES" Then
                Retorna_datos_guia_envio_destinatario = Result
                Exit Function
            End If
            Retorna_datos_guia_envio_destinatario = "YES"
        Catch ex As Exception
            Retorna_datos_guia_envio_destinatario = "Inconsistencia función Retorna_datos_guia_envio_destinatario " & ex.Message
        End Try
    End Function
    Function Retorna_datos_guia_envio_destinatario_nombre(ByRef stru_valores_campo() As stru_campos_destinatario, ByVal nombre_destinatario As String) As String
        Try
            Dim nombre_plantilla As String = ""
            Dim id_plantilla As Integer = 0
            Dim Result As String = ""
            Result = Me.Retorna_nombre_plantilla_destinatario_guia(nombre_plantilla, id_plantilla)
            If Result <> "YES" Then
                Retorna_datos_guia_envio_destinatario_nombre = Result
                Exit Function
            End If
            Result = Me.Retorna_campos_plantilla_vald_guia(stru_valores_campo, id_plantilla)
            If Result <> "YES" Then
                Retorna_datos_guia_envio_destinatario_nombre = Result
                Exit Function
            End If
            Result = Retorna_dato_guia_envio_plantilla_nombre(stru_valores_campo, nombre_destinatario, nombre_plantilla)
            If Result <> "YES" Then
                Retorna_datos_guia_envio_destinatario_nombre = Result
                Exit Function
            End If

            Retorna_datos_guia_envio_destinatario_nombre = "YES"
        Catch ex As Exception
            Retorna_datos_guia_envio_destinatario_nombre = "Inconsistencia función Retorna_datos_guia_envio_destinatario_nombre " & ex.Message
        End Try
    End Function
    Function Asigna_datos_interface_datos_externos(ByRef stru_valores_campo() As stru_campos_destinatario, ByRef page As Page) As String
        Try
            For i As Integer = 0 To stru_valores_campo.Length - 1
                Dim reftext As TextBox = Nothing
                reftext = page.FindControl("TextBox_" & stru_valores_campo(i).nombre_campo_destino)
                If Not reftext Is Nothing Then
                    reftext.Text = stru_valores_campo(i).valor_campo_fuente
                End If
            Next
            Asigna_datos_interface_datos_externos = "YES"
        Catch ex As Exception
            Asigna_datos_interface_datos_externos = "Inconsistencia función Asigna_datos_interface_datos_externos " & ex.Message
        End Try
    End Function
    Function retorna_id_empresa_mensajeria(ByVal nombre_empresa As String, ByRef id_empresa_mensajeria As Integer) As String
        Try
            Dim sql_consulta As String = "Select ID_EMPRESA_ENVIO from ra_empresa_envio where NOMBRE_EMPRESA='" & nombre_empresa & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                retorna_id_empresa_mensajeria = " Error Listando id mensajeria   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_empresa_mensajeria = Datset.Tables(0).Rows(0).Item(0)
                retorna_id_empresa_mensajeria = "YES"
                Exit Function
            Else
                retorna_id_empresa_mensajeria = "Imposible encontrar el tipo de servicio "
                Exit Function
            End If
        Catch ex As Exception
            retorna_id_empresa_mensajeria = "Inconsistencia funcion retorna_id_empresa_mensajeria " & ex.Message
        End Try
    End Function
    Function retorna_nombre_empresa_mensajeria(ByVal id_empresa_mensajeria As Integer, ByRef nombre_empresa As String) As String
        Try
            Dim sql_consulta As String = "Select NOMBRE_EMPRESA from ra_empresa_envio where ID_EMPRESA_ENVIO='" & id_empresa_mensajeria & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                retorna_nombre_empresa_mensajeria = " Error Listando nombre empresa mensajeria   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                nombre_empresa = Datset.Tables(0).Rows(0).Item(0)
                retorna_nombre_empresa_mensajeria = "YES"
                Exit Function
            Else
                retorna_nombre_empresa_mensajeria = "Imposible encontrar el nombre del operador de mensajería "
                Exit Function
            End If
        Catch ex As Exception
            retorna_nombre_empresa_mensajeria = "Inconsistencia funcion retorna_nombre_empresa_mensajeria " & ex.Message
        End Try
    End Function
    Function retorna_tipo_empresa_registro_guia(ByVal nombre_empresa As String, ByRef id_tipo_empresa As Integer) As String
        Try
            Dim sql_consulta As String = "Select TIPO_EMPRESA_INTERNA from ra_empresa_envio where NOMBRE_EMPRESA='" & nombre_empresa & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                retorna_tipo_empresa_registro_guia = " Error Listando nombre plantilla guía   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_tipo_empresa = Datset.Tables(0).Rows(0).Item(0)
                retorna_tipo_empresa_registro_guia = "YES"
                Exit Function
            Else
                retorna_tipo_empresa_registro_guia = "Imposible encontrar el tipo de servicio "
                Exit Function
            End If
        Catch ex As Exception
            retorna_tipo_empresa_registro_guia = "Inconsistencia funcion retorna_tipo_empresa_registro_guia " & ex.Message
        End Try
    End Function
    Function Actualiza_datos_estado_envio(ByVal id_guia_envio As Integer, ByVal pag As Page) As String
        Try
            Dim DropDownListESTADO_CONFIRMACION_GUIA As DropDownList = pag.FindControl("DropDownListESTADO_CONFIRMACION_GUIA")
            Dim TextBox_FECHA_RECIBIDO_GUIA As TextBox = pag.FindControl("TextBox_FECHA_RECIBIDO_GUIA")
            Dim TextBox_NOTA_CLIENTE As TextBox = pag.FindControl("TextBox_NOTA_CLIENTE")
            Dim Hidden_campos_validacion As Object = pag.FindControl("Hidden_campos_validacion_procesa")
            Dim Hidden_valores_validacion As Object = pag.FindControl("Hidden_valores_validacion_procesa")
            Dim estado_guia As Integer = 0
            If DropDownListESTADO_CONFIRMACION_GUIA.Text = "Enviada" Then
                estado_guia = 2
            End If
            If DropDownListESTADO_CONFIRMACION_GUIA.Text = "Entregada" Then
                estado_guia = 3
            End If
            If DropDownListESTADO_CONFIRMACION_GUIA.Text = "Devolucion Permanente" Then
                estado_guia = 4
            End If
            If estado_guia <> 2 And TextBox_FECHA_RECIBIDO_GUIA.Text = "" Then
                Actualiza_datos_estado_envio = "Debe seleccionar la fecha de recibido o devolucion de la guía de envío"
                Exit Function
            End If
            Dim ref_fecha As Object = ""
            If TextBox_FECHA_RECIBIDO_GUIA.Text = "" Then
                ref_fecha = "Null"
            Else
                ref_fecha = "'" & TextBox_FECHA_RECIBIDO_GUIA.Text & "'"
            End If
            If estado_guia = 2 Then
                ref_fecha = "Null"
                TextBox_FECHA_RECIBIDO_GUIA.Text = ""
            End If
            Dim ref_nota As Object = ""
            If TextBox_NOTA_CLIENTE.Text = "" Then
                ref_nota = "Null"
            Else
                ref_nota = "'" & TextBox_NOTA_CLIENTE.Text & "'"
            End If
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_DA
            Dim result As String = ""
            Dim sql_update As String = "Update ra_guia_interna set ESTADO_CONFIRMACION_GUIA=" & estado_guia & ",FECHA_RECIBIDO_GUIA=" & ref_fecha & ",NOTA_CLIENTE=" & ref_nota & _
                " where Id_guia_envio=" & id_guia_envio
            result = Ref_Car_Conec.SELECTION_INSERT_COMMAND(sql_update)
            If result <> "YES" Then
                Actualiza_datos_estado_envio = result
                Exit Function
            Else
                Hidden_campos_validacion.Value = "ESTADO_ENVIO¬FECHA_RECIBIDO_GUIA"
                Hidden_valores_validacion.Value = DropDownListESTADO_CONFIRMACION_GUIA.Text & "¬" & TextBox_FECHA_RECIBIDO_GUIA.Text
                Actualiza_datos_estado_envio = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_datos_estado_envio = "Inconsistencia general función Actualiza_datos_estado_envio " & ex.Message
        End Try
    End Function
    Function Asigna_datos_interface_guia_envio(ByVal estado_guia As Integer, _
        ByVal fecha_recibido_guia As String, ByVal nota_guia As String, ByRef pag As Page) As String
        Try
            Dim DropDownListESTADO_CONFIRMACION_GUIA As DropDownList = pag.FindControl("DropDownListESTADO_CONFIRMACION_GUIA")
            Dim TextBox_FECHA_RECIBIDO_GUIA As TextBox = pag.FindControl("TextBox_FECHA_RECIBIDO_GUIA")
            Dim TextBox_NOTA_CLIENTE As TextBox = pag.FindControl("TextBox_NOTA_CLIENTE")
            Dim UpdatePanel_procesa_archivo_tramite As UpdatePanel = pag.FindControl("UpdatePanel_procesa_archivo_tramite")
            If estado_guia = 2 Then
                DropDownListESTADO_CONFIRMACION_GUIA.Text = "Enviada"
            End If
            If estado_guia = 3 Then
                DropDownListESTADO_CONFIRMACION_GUIA.Text = "Entregada"
            End If
            If estado_guia = 4 Then
                DropDownListESTADO_CONFIRMACION_GUIA.Text = "Devolucion Permanente"
            End If
            TextBox_FECHA_RECIBIDO_GUIA.Text = fecha_recibido_guia
            TextBox_NOTA_CLIENTE.Text = nota_guia
            UpdatePanel_procesa_archivo_tramite.Update()
            Asigna_datos_interface_guia_envio = "YES"
        Catch ex As Exception
            Asigna_datos_interface_guia_envio = "Inconsistencia general función Asigna_datos_interface_guia_envio " & ex.Message
        End Try
    End Function
    Function Retorna_datos_estado_guia_envio(ByVal id_guia_envio As Integer, ByRef estado_guia As Integer, _
        ByRef fecha_recibido_guia As String, ByRef nota_guia As String) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select ESTADO_CONFIRMACION_GUIA,CAST(FECHA_RECIBIDO_GUIA AS DATE),NOTA_CLIENTE from ra_guia_interna where Id_guia_envio='" & _
            id_guia_envio & "'"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_datos_estado_guia_envio = " Error Listando existencia guia   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_datos_estado_guia_envio = "Imposible encontrar el estado de la guía " & id_guia_envio
                Exit Function
            Else
                estado_guia = Datset.Tables(0).Rows(0).Item(0)
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    fecha_recibido_guia = ""
                Else
                    Dim ref_ClassGestionFechas As New ClassGestionFechas
                    fecha_recibido_guia = Datset.Tables(0).Rows(0).Item(1).ToString
                    Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(fecha_recibido_guia)
                    If Result <> "YES" Then
                        Retorna_datos_estado_guia_envio = Result
                        Exit Function
                    End If
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    nota_guia = ""
                Else
                    Dim refclasalamcenamiento As New ClassAlmacenamiento
                    nota_guia = Datset.Tables(0).Rows(0).Item(2).ToString
                End If
                Retorna_datos_estado_guia_envio = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_datos_estado_guia_envio = "Inconsistencia general función Retorna_datos_estado_guia_envio  " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_destina_ext_guia(ByVal id_dext_esterno As Integer, ByRef estado As String) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select Destinatario_Ext from ra_guia_interna where Destinatario_Ext='" & _
            id_dext_esterno & "'"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_destina_ext_guia = " Error Listando existencia destinatario externo   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado = "NO"
                Verifica_existencia_destina_ext_guia = "YES"
                Exit Function
            Else
                estado = "YES"
                Verifica_existencia_destina_ext_guia = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_destina_ext_guia = "Inconsistencia general función Verifica_existencia_destina_ext_guia = " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_radicado_en_guia(ByVal radicado As String) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select Id_guia_envio from ra_guia_interna where RADICADO='" & _
            radicado & "'"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_radicado_en_guia = " Error Listando existencia radicado   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Verifica_existencia_radicado_en_guia = "YES"
                Exit Function
            Else
                Verifica_existencia_radicado_en_guia = "El radicado se encuentra relacionado con el numero unico de guía " & Datset.Tables(0).Rows(0).Item(0)
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_radicado_en_guia = "Inconsistencia general función Verifica_existencia_radicado_en_guia = " & ex.Message
        End Try
    End Function
    Function Retorna_operarios_mensajeria_gestion(ByRef combo As DropDownList, ByVal nombre_empresa As String) As String
        Try
            combo.Items.Clear()
            Dim sql_consulta As String = "Select Nombre_Remitente from remit_dest_interno_perfil_produccion AS rdip " & _
                "INNER JOIN remit_dest_interno AS rdi ON (rdi.id_Remit_Dest_Int=rdip.remit_dest_interno_idremit_dest_interno)" & _
                " where PERFIL_USUARIO_MENSAJERIA=1"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_operarios_mensajeria_gestion = " Error Listando operarios de mensajería   " & Result
                Exit Function
            End If
            Dim TempoEnsamble As String = ""
            If Datset.Tables(0).Rows.Count > 0 Then
                combo.Items.Add("")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    combo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                For i As Integer = 0 To combo.Items.Count - 1
                    If nombre_empresa <> "" Then
                        If combo.Items(i).Text = nombre_empresa Then
                            combo.Text = nombre_empresa
                        End If

                    End If
                Next
                
                Retorna_operarios_mensajeria_gestion = "YES"
            Else
                Retorna_operarios_mensajeria_gestion = "YES"
            End If
        Catch ex As Exception
            Retorna_operarios_mensajeria_gestion = "Inconsistencia general función Retorna_operarios_mensajeria_gestion " & ex.Message
        End Try
    End Function
    Function Retorna_datos_envio_respuesta(ByVal id_respuesta As Integer, ByRef nombre_empresa As String, _
                                           ByRef numero_guia As String, ByRef radicado_respuesta As String, ByRef id_guia As Integer) As String
        Try
            Dim sql_consulta As String = "Select EMPRESA_ENVIO,GUIA_ENVIO,RADICADO_RESPUESTA,Numero_Guia_Interna from ra_respuesta_radicado where ID_RESPUESTA_RADICADO=" & id_respuesta
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_datos_envio_respuesta = " Error Listando datos basicos envío   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    nombre_empresa = ""
                Else
                    nombre_empresa = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    numero_guia = ""
                Else
                    numero_guia = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    radicado_respuesta = ""
                Else
                    radicado_respuesta = Datset.Tables(0).Rows(0).Item(2)
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) = True Then
                    id_guia = 0
                Else
                    id_guia = Datset.Tables(0).Rows(0).Item(3)
                End If
                Retorna_datos_envio_respuesta = "YES"
            Else
                Retorna_datos_envio_respuesta = "YES"
            End If
        Catch ex As Exception
            Retorna_datos_envio_respuesta = "Inconsistencia función Retorna_datos_envio_respuesta " & ex.Message
        End Try
    End Function
    Function Lista_empresa_envio(ByRef combo As DropDownList, ByVal nombre_empresa As String) As String
        Try
            combo.Items.Clear()
            Dim sql_consulta As String = "Select NOMBRE_EMPRESA from ra_empresa_envio where ESTADO_EMPRESA=1 "
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_empresa_envio = " Error Listando empresas de envío   " & Result
                Exit Function
            End If
            Dim TempoEnsamble As String = ""
            If Datset.Tables(0).Rows.Count > 0 Then
                combo.Items.Add("")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    combo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                combo.Text = nombre_empresa
                Lista_empresa_envio = "YES"
            Else
                Lista_empresa_envio = "YES"
            End If
        Catch ex As Exception
            Lista_empresa_envio = "Inconsistencia función Lista_empresa_envio " & ex.Message
        End Try
    End Function
    Function Actualiza_guia_envio_manual(ByVal empresa_envio As String, _
        ByRef guia_envio As String, ByVal pag As Page, ByVal nombre_destinatario As String, _
        ByVal operador_mensajeria_interna As String, _
         ByVal nit_identificacion As String, _
        ByVal direccion As String, ByVal telefono As String, ByVal correo_electrnico As String, _
        ByVal id_tipo As Integer, ByVal id_guia As Integer, _
        ByVal anexo As String, ByVal radicado_guia As String, ByVal nombre_remitente As String) As String


        If nombre_destinatario = "" Then
            Actualiza_guia_envio_manual = "Por favor informe el nombre del destinatario de la correspondencia"
            Exit Function
        End If
        If direccion = "" Then
            Actualiza_guia_envio_manual = "Por favor informe la dirección del destinatario de la correspondencia"
            Exit Function
        End If
        If nombre_remitente = "" Then
            Actualiza_guia_envio_manual = "Por favor seleccione el nombre del remitente"
            Exit Function
        End If
        If id_tipo = 0 Then
            If guia_envio = "" Then
                Actualiza_guia_envio_manual = "Por favor informe el consecutivo de guía del operador externo"
                Exit Function
            End If
        End If
        If id_tipo = 1 Then
            If operador_mensajeria_interna = "" Then
                Actualiza_guia_envio_manual = "Por favor seleccione el usuario de mensajeria interna"
                Exit Function
            End If
        End If
        Dim strus As guia_envio = Nothing
        Dim Result As String = ""
        Result = Me.Retorna_datos_estructura_guia(id_guia, strus)
        If Result <> "YES" Then
            Actualiza_guia_envio_manual = Result
            Exit Function
        End If
        Dim consecutivo_guia As String = ""
        consecutivo_guia = guia_envio
        '--------------------------------------------------------------
        'Retorna id empresa envio, detecta cambio operador mensajería
        '--------------------------------------------------------------
        Dim id_empresa_envio As Integer = 0
        Result = Me.retorna_id_empresa_mensajeria(empresa_envio, id_empresa_envio)
        If Result <> "YES" Then
            Actualiza_guia_envio_manual = Result
            Exit Function
        End If
        Dim estado_cambio_empresa_envio As Integer = 0
        If id_empresa_envio <> strus.ra_empresa_envio_ID_EMPRESA_ENVIO Then
            estado_cambio_empresa_envio = 1
        End If
        Dim id_remitente As Integer = 0
        Dim refclasrad As New ClassRadicador
        Dim Ref_Class_remit_dest_interno As New Class_remit_dest_interno
        Result = Ref_Class_remit_dest_interno.Retorna_Id_Destinatario(nombre_remitente, _
                                                                      id_remitente)
        If Result <> "YES" Then
            Actualiza_guia_envio_manual = Result
            Exit Function
        End If
        Dim Estado_cambio_remitente As Integer = 0
        Dim id_area As Integer = 0
        If id_remitente <> strus.Remit_Dest_Interno_id_Remit_Dest_Int Then
            Estado_cambio_remitente = 1
            Dim Ref_class_remit_dest_int As New Class_remit_dest_interno
            Result = Ref_class_remit_dest_int.Solicita_id_area_nombre_area_destinatario(id_remitente, _
                                                                         id_area, _
                                                                         "")
            If Result <> "YES" Then
                Actualiza_guia_envio_manual = Result
                Exit Function
            End If
        End If
        If radicado_guia <> strus.RADICADO Then
            Dim id_plantilla_default As Integer = 0
            Dim nombre_plantilla_default As String = ""
            If radicado_guia <> "" Then
                Dim refclas_rad As New ClassRadicador
                Dim ref_calss_system As New Class_system_plantilla_radicado
                Result = ref_calss_system.Solicita_plantilla_default_respuesta(id_plantilla_default, nombre_plantilla_default)
                If Result <> "YES" Then
                    Actualiza_guia_envio_manual = Result
                    Exit Function
                End If
                '-----Verifica la existencia del radicado en la plantilla
                Result = refclas_rad.Verifica_existencia_radicado_en_plantilla(nombre_plantilla_default, radicado_guia)
                If Result <> "YES" Then
                    Actualiza_guia_envio_manual = Result
                    Exit Function
                End If
                '----Verfica exisencia del radicado como parte de una guía
                Result = Me.Verifica_existencia_radicado_en_guia(radicado_guia)
                If Result <> "YES" Then
                    Actualiza_guia_envio_manual = Result
                    Exit Function
                End If
            End If
        End If
        '---------------------------------------------------------
        'Detecta cambio destinatario externo
        '----------------------------------------------------------
        Dim estado_cambio_destinatario As Integer = 0
        Dim id_destinatario As Integer = 0
        If strus.NOMBRE_RAZON_SOCIAL <> nombre_destinatario Then
            estado_cambio_destinatario = 1
            Dim refradic As New ClassRadicador
            Dim id_plantilla As Integer = 0
            Dim nombre_plantilla_default As String = ""
            Dim ref_calss_system As New Class_system_plantilla_radicado
            Result = ref_calss_system.Solicita_plantilla_default_respuesta(id_plantilla, nombre_plantilla_default)
            If Result <> "YES" Then
                Actualiza_guia_envio_manual = Result
                Exit Function
            End If
            Dim id_escrip As Integer = 0
            Result = refradic.Retorna_id_script_validacion(id_plantilla, _
                                                           "DINAMICOEXTERNO", _
                                                           "REMITENTE_COR", _
                                                           id_escrip)
            If Result <> "YES" Then
                Actualiza_guia_envio_manual = Result
                Exit Function
            End If
            Dim Ref_Class_relacion_script_plantilla As New Class_relacion_script_plantilla
            Dim campo_comparacion As String = ""
            Dim nombre_plantilla_validacion As String = ""
            Dim id_plantilla_validacion As Integer = 0
            Result = Ref_Class_relacion_script_plantilla.retorna_campo_compracion_plantilla(id_escrip, _
                                                                                            campo_comparacion, _
                                                                                            nombre_plantilla_validacion, _
                                                                                            id_plantilla_validacion)
            If Result <> "YES" Then
                Actualiza_guia_envio_manual = Result
                Exit Function
            End If
            Dim Ref_Class_destinatario_externo As New Class_destinatario_externo
            Result = Ref_Class_destinatario_externo.verifica_existencia_destinatario_externo(nombre_destinatario, _
                                                                                            id_destinatario, _
                                                                                            nombre_plantilla_validacion, _
                                                                                            campo_comparacion)
            If Result <> "YES" Then
                Actualiza_guia_envio_manual = Result
                Exit Function
            End If

        End If
        '---------------------------------------------------------
        'Detecta cambio de operador interno
        '---------------------------------------------------------
        Dim Refclasradicado As New ClassRadicador
        Dim estado_operador_interno As Integer = 0
        Dim id_operador_interno As Integer = 0
        If id_tipo = 1 Then
            '------------------------------------------------------
            'Retorna usuario de getion perfil mensajero
            '------------------------------------------------------

            Result = Ref_Class_remit_dest_interno.Retorna_Id_Destinatario(operador_mensajeria_interna, _
                                                                          id_operador_interno)
            If Result <> "YES" Then
                Actualiza_guia_envio_manual = Result
                Exit Function
            End If

            If id_operador_interno <> strus.ID_MENSAJERO_INTERNO Then
                estado_operador_interno = 1
            End If
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim mySqldatReader As MySqlDataReader
        Dim sqlresultinsert As Integer = 0
        Dim Class_zero_fill As New Class_zero_fill
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim Parametro_Select_System1 As String = " SELECT conse_cutivo_guia FROM registro_plantilla_guia " & _
            " where Nombre_Plantilla_Guia='" & "Ra_Guia_Interna" & "' for update"
            Dim Parametro_Actualiza_System1 As String = ""
            If id_tipo = 0 Then
                consecutivo_guia = guia_envio
            End If
            If estado_cambio_empresa_envio = 1 Then

                If id_tipo = 1 Then
                    myCommand.CommandText = Parametro_Select_System1
                    mySqldatReader = myCommand.ExecuteReader()
                    If mySqldatReader Is Nothing Then
                        Actualiza_guia_envio_manual = "Imposible Encontrar Registro En Tabla registro_plantilla_guia Error Conexion"
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader.HasRows = False Then
                        Actualiza_guia_envio_manual = "Imposible Encontrar Registro En Tabla registro_plantilla_guia"
                        'myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    '***********************************************************************
                    'Valores recuperados de la consulta de la tabla registro_plantilla_guia
                    '***********************************************************************
                    Dim id_incremento As Integer = 0
                    mySqldatReader.Read()
                    id_incremento = mySqldatReader.Item(0)
                    mySqldatReader.Close()
                    id_incremento = id_incremento + 1
                    consecutivo_guia = id_incremento.ToString
                    Result = Class_zero_fill.zero_fill(consecutivo_guia, 6, "0")
                    If Result <> "YES" Then
                        Actualiza_guia_envio_manual = "Imposible agregar zerofill " & Result
                        'myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    Dim año_radic As String = Now.Year.ToString.Substring(2, 2)
                    consecutivo_guia = año_radic & consecutivo_guia
                    guia_envio = consecutivo_guia
                    Parametro_Actualiza_System1 = "update registro_plantilla_guia set conse_cutivo_guia = " & "'" & id_incremento & "'" & _
                    " where  Nombre_Plantilla_Guia='" & "Ra_Guia_Interna'"
                    myCommand.CommandText = Parametro_Actualiza_System1
                    sqlresultinsert = myCommand.ExecuteNonQuery()
                    If sqlresultinsert = 0 Then
                        Actualiza_guia_envio_manual = "Imposible actualizar el consecutivo de guía  "
                        myConnection.Close()
                        Exit Function
                    End If
                End If
            End If
            Dim ref_nit_identificacion As String = ""
            If nit_identificacion = "" Then
                ref_nit_identificacion = "null"
            Else
                ref_nit_identificacion = "'" & nit_identificacion & "'"
            End If
            Dim ref_telefono As String = ""
            If telefono = "" Then
                ref_telefono = "null"
            Else
                ref_telefono = "'" & telefono & "'"
            End If
            Dim ref_correo_electrnico As String = ""
            If correo_electrnico = "" Then
                ref_correo_electrnico = "Null"
            Else
                ref_correo_electrnico = "'" & correo_electrnico & "'"
            End If
            Dim ref_anexo As String = ""
            If anexo = "" Then
                ref_anexo = "Null"
            Else
                ref_anexo = "'" & anexo & "'"
            End If
            Dim ref_radicado_guia As String = ""
            If radicado_guia = "" Then
                ref_radicado_guia = "Null"
            Else
                ref_radicado_guia = "'" & radicado_guia & "'"
            End If
            Dim Parametro_Insercio As String = "update ra_guia_interna "
            '-----------------------------------------
            'Cambia el estado del operador de envio
            '----------------------------------------
            If estado_cambio_empresa_envio = 1 Then
                If Parametro_Insercio = "update ra_guia_interna " Then
                    Parametro_Insercio = Parametro_Insercio & " set ra_empresa_envio_ID_EMPRESA_ENVIO=" & id_empresa_envio & ",ID_TIPO_GUIA=" & id_tipo
                Else
                    Parametro_Insercio = Parametro_Insercio & ", ra_empresa_envio_ID_EMPRESA_ENVIO=" & id_empresa_envio & ",ID_TIPO_GUIA=" & id_tipo
                End If
            End If
            '---------------------------------------------
            'Cambia estado de operador interno
            '---------------------------------------------
            If estado_operador_interno = 1 Then
                If Parametro_Insercio = "update ra_guia_interna " Then
                    Parametro_Insercio = Parametro_Insercio & " set ID_MENSAJERO_INTERNO=" & id_operador_interno
                Else
                    Parametro_Insercio = Parametro_Insercio & ", ID_MENSAJERO_INTERNO=" & id_operador_interno
                End If
            End If
            '-----------------------------------------------
            'Remitente guía
            '-----------------------------------------------
            If Estado_cambio_remitente = 1 Then
                If Parametro_Insercio = "update ra_guia_interna " Then
                    Parametro_Insercio = Parametro_Insercio & " set Remit_Dest_Interno_id_Remit_Dest_Int=" & id_remitente & ",areas_depart_radicacion_Codigo_Area=" & id_area
                Else
                    Parametro_Insercio = Parametro_Insercio & ", Remit_Dest_Interno_id_Remit_Dest_Int=" & id_remitente & ",areas_depart_radicacion_Codigo_Area=" & id_area
                End If
            End If
            '-----------------------------------------------
            'Cambia destinatario externo
            '-----------------------------------------------
            If estado_cambio_destinatario = 1 Then
                If Parametro_Insercio = "update ra_guia_interna " Then
                    Parametro_Insercio = Parametro_Insercio & " set Destinatario_Ext=" & id_destinatario
                Else
                    Parametro_Insercio = Parametro_Insercio & ", Destinatario_Ext=" & id_destinatario
                End If
            End If
            '---------------------------------------------
            'Cambia datos generales guia
            '---------------------------------------------
            If Parametro_Insercio = "update ra_guia_interna " Then
                Parametro_Insercio = Parametro_Insercio & " set Concecutivo_Guia='" & consecutivo_guia & "',NIT_IDENTIFICACION=" & ref_nit_identificacion & _
                ",NOMBRE_RAZON_SOCIAL='" & nombre_destinatario & "',DIRECCION='" & direccion & "',TELEFONO=" & ref_telefono & ",CORREO_ELECTRONICO=" & ref_correo_electrnico & _
                ",ANEXO=" & ref_anexo & ",RADICADO=" & ref_radicado_guia
            Else
                Parametro_Insercio = Parametro_Insercio & ", Concecutivo_Guia='" & consecutivo_guia & "',NIT_IDENTIFICACION=" & ref_nit_identificacion & _
                ",NOMBRE_RAZON_SOCIAL='" & nombre_destinatario & "',DIRECCION='" & direccion & "',TELEFONO=" & ref_telefono & ",CORREO_ELECTRONICO=" & ref_correo_electrnico & _
                ",ANEXO=" & ref_anexo & ",RADICADO=" & ref_radicado_guia
            End If
            If Parametro_Insercio <> "update ra_guia_interna " Then
                Parametro_Insercio = Parametro_Insercio & " where Id_guia_envio=" & id_guia
            End If
            myCommand.CommandText = Parametro_Insercio
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Actualiza_guia_envio_manual = "Imposible actualizar la guía de envío "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Dim Hidden_campos_validacion As Object = pag.FindControl("Hidden_campos_validacion")
            Dim Hidden_valores_validacion As Object = pag.FindControl("Hidden_valores_validacion")
            Hidden_campos_validacion.Value = "CONCECUTIVO_GUIA¬NOMBRE_RAZON_SOCIAL¬NOMBRE_EMPRESA¬MENSAJERO¬NOMBRE_REMITENTE"
            Hidden_valores_validacion.Value = consecutivo_guia & "¬" & nombre_destinatario & "¬" _
            & empresa_envio & "¬" & operador_mensajeria_interna & "¬" & nombre_remitente
            Actualiza_guia_envio_manual = "YES"

        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Actualiza_guia_envio_manual = "An exception of type " + ex.GetType().ToString() + _
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Actualiza_guia_envio_manual = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Actualiza_guia_envio(ByVal id_tramite As Integer, ByVal empresa_envio As String, _
        ByRef guia_envio As String, ByVal pag As Page, ByVal nombre_destinatario As String, _
        ByVal operador_mensajeria_interna As String, _
         ByVal nit_identificacion As String, _
        ByVal direccion As String, ByVal telefono As String, ByVal correo_electrnico As String, ByVal id_tipo As Integer, ByVal id_guia As Integer, ByVal anexo As String) As String
        If nombre_destinatario = "" Then
            Actualiza_guia_envio = "Por favor informe el nombre del destinatario de la correspondencia"
            Exit Function
        End If
        If direccion = "" Then
            Actualiza_guia_envio = "Por favor informe la dirección del destinatario de la correspondencia"
            Exit Function
        End If
        If id_tipo = 0 Then
            If guia_envio = "" Then
                Actualiza_guia_envio = "Por favor informe el consecutivo de guía del operador externo"
                Exit Function
            End If
        End If
        If id_tipo = 1 Then
            If operador_mensajeria_interna = "" Then
                Actualiza_guia_envio = "Por favor seleccione el usuario de mensajeria interna"
                Exit Function
            End If
        End If
        Dim strus As guia_envio = Nothing
        Dim Result As String = ""
        Result = Me.Retorna_datos_estructura_guia(id_guia, strus)
        If Result <> "YES" Then
            Actualiza_guia_envio = Result
            Exit Function
        End If

        '--------------------------------------------------------------
        'Retorna id empresa envio, detecta cambio operador mensajería
        '--------------------------------------------------------------
        Dim id_empresa_envio As Integer = 0
        Result = Me.retorna_id_empresa_mensajeria(empresa_envio, id_empresa_envio)
        If Result <> "YES" Then
            Actualiza_guia_envio = Result
            Exit Function
        End If
        Dim estado_cambio_empresa_envio As Integer = 0
        If id_empresa_envio <> strus.ra_empresa_envio_ID_EMPRESA_ENVIO Then
            estado_cambio_empresa_envio = 1
        End If
        '---------------------------------------------------------
        'Detecta cambio de operador interno
        '---------------------------------------------------------
        Dim Ref_Class_remit_dest_interno As New Class_remit_dest_interno
        Dim Refclasradicado As New ClassRadicador
        Dim estado_operador_interno As Integer = 0
        Dim id_operador_interno As Integer = 0
        If id_tipo = 1 Then
            '------------------------------------------------------
            'Retorna usuario de getion perfil mensajero
            '------------------------------------------------------
            If id_tipo = 1 Then
                Result = Ref_Class_remit_dest_interno.Retorna_Id_Destinatario(operador_mensajeria_interna, _
                                                                              id_operador_interno)
                If Result <> "YES" Then
                    Actualiza_guia_envio = Result
                    Exit Function
                End If
            End If
            If id_operador_interno <> strus.ID_MENSAJERO_INTERNO Then
                estado_operador_interno = 1
            End If
        End If
        '----------------------------------------------------------
        'Estado cambio consecutivo radicado
        '----------------------------------------------------------

        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim mySqldatReader As MySqlDataReader
        Dim sqlresultinsert As Integer = 0
        Dim Class_zero_fill As New Class_zero_fill
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim Parametro_Select_System1 As String = " SELECT conse_cutivo_guia FROM registro_plantilla_guia " & _
            " where Nombre_Plantilla_Guia='" & "Ra_Guia_Interna" & "' for update"
            Dim Parametro_Actualiza_System1 As String = ""
            Dim consecutivo_guia As String = ""
            consecutivo_guia = guia_envio
            '----------------------------------------------
            'Actualiza tipo empresa de envío
            '----------------------------------------------
            If estado_cambio_empresa_envio = 1 Then
                If id_tipo = 1 Then
                    myCommand.CommandText = Parametro_Select_System1
                    mySqldatReader = myCommand.ExecuteReader()
                    If mySqldatReader Is Nothing Then
                        Actualiza_guia_envio = "Imposible Encontrar Registro En Tabla registro_plantilla_guia Error Conexion"
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader.HasRows = False Then
                        Actualiza_guia_envio = "Imposible Encontrar Registro En Tabla registro_plantilla_guia"
                        'myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    '***********************************************************************
                    'Valores recuperados de la consulta de la tabla registro_plantilla_guia
                    '***********************************************************************
                    Dim id_incremento As Integer = 0
                    mySqldatReader.Read()
                    id_incremento = mySqldatReader.Item(0)
                    mySqldatReader.Close()
                    id_incremento = id_incremento + 1
                    consecutivo_guia = id_incremento.ToString
                    Result = Class_zero_fill.zero_fill(consecutivo_guia, 6, "0")
                    If Result <> "YES" Then
                        Actualiza_guia_envio = "Imposible agregar zerofill " & Result
                        'myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    Dim año_radic As String = Now.Year.ToString.Substring(2, 2)
                    consecutivo_guia = año_radic & consecutivo_guia
                    guia_envio = consecutivo_guia
                    Parametro_Actualiza_System1 = "update registro_plantilla_guia set conse_cutivo_guia = " & "'" & id_incremento & "'" & _
                    " where  Nombre_Plantilla_Guia='" & "Ra_Guia_Interna'"
                    myCommand.CommandText = Parametro_Actualiza_System1
                    sqlresultinsert = myCommand.ExecuteNonQuery()
                    If sqlresultinsert = 0 Then
                        Actualiza_guia_envio = "Imposible actualizar el consecutivo de guía  "
                        myConnection.Close()
                        Exit Function
                    End If
                End If
            End If
            Dim ref_nit_identificacion As String = ""
            If nit_identificacion = "" Then
                ref_nit_identificacion = "null"
            Else
                ref_nit_identificacion = "'" & nit_identificacion & "'"
            End If
            Dim ref_telefono As String = ""
            If telefono = "" Then
                ref_telefono = "null"
            Else
                ref_telefono = "'" & telefono & "'"
            End If
            Dim ref_correo_electrnico As String = ""
            If correo_electrnico = "" Then
                ref_correo_electrnico = "Null"
            Else
                ref_correo_electrnico = "'" & correo_electrnico & "'"
            End If
            Dim ref_anexo As String = ""
            If anexo = "" Then
                ref_anexo = "Null"
            Else
                ref_anexo = "'" & anexo & "'"
            End If
            Dim Parametro_Insercio As String = "update ra_guia_interna "
            '-----------------------------------------
            'Cambia el estado del operador de envio
            '----------------------------------------
            If estado_cambio_empresa_envio = 1 Then
                If Parametro_Insercio = "update ra_guia_interna " Then
                    Parametro_Insercio = Parametro_Insercio & " set ra_empresa_envio_ID_EMPRESA_ENVIO=" & id_empresa_envio & ",ID_TIPO_GUIA=" & id_tipo
                Else
                    Parametro_Insercio = Parametro_Insercio & ", ra_empresa_envio_ID_EMPRESA_ENVIO=" & id_empresa_envio & ",ID_TIPO_GUIA=" & id_tipo
                End If
            End If
            '---------------------------------------------
            'Cambia estado de operador interno
            '---------------------------------------------
            If estado_operador_interno = 1 Then
                If Parametro_Insercio = "update ra_guia_interna " Then
                    Parametro_Insercio = Parametro_Insercio & " set ID_MENSAJERO_INTERNO=" & id_operador_interno
                Else
                    Parametro_Insercio = Parametro_Insercio & ", ID_MENSAJERO_INTERNO=" & id_operador_interno
                End If
            End If
            '---------------------------------------------
            'Cambia datos generales guia
            '---------------------------------------------
            If Parametro_Insercio = "update ra_guia_interna " Then
                Parametro_Insercio = Parametro_Insercio & " set Concecutivo_Guia='" & consecutivo_guia & "',NIT_IDENTIFICACION=" & ref_nit_identificacion & _
                ",NOMBRE_RAZON_SOCIAL='" & nombre_destinatario & "',DIRECCION='" & direccion & "',TELEFONO=" & ref_telefono & ",CORREO_ELECTRONICO=" & ref_correo_electrnico & _
                ",ANEXO=" & ref_anexo
            Else
                Parametro_Insercio = Parametro_Insercio & ", Concecutivo_Guia='" & consecutivo_guia & "',NIT_IDENTIFICACION=" & ref_nit_identificacion & _
                ",NOMBRE_RAZON_SOCIAL='" & nombre_destinatario & "',DIRECCION='" & direccion & "',TELEFONO=" & ref_telefono & ",CORREO_ELECTRONICO=" & ref_correo_electrnico & _
                ",ANEXO=" & ref_anexo
            End If
            If Parametro_Insercio <> "update ra_guia_interna " Then
                Parametro_Insercio = Parametro_Insercio & " where Id_guia_envio=" & id_guia
            End If
            myCommand.CommandText = Parametro_Insercio
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Actualiza_guia_envio = "Imposible registrar la guía de envío "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            'Dim obisert = myCommand.LastInsertedId
            Dim update_respuesta As String = "update ra_respuesta_radicado set EMPRESA_ENVIO='" & empresa_envio & "',GUIA_ENVIO='" & consecutivo_guia & "' " & _
                " where ID_RESPUESTA_RADICADO=" & id_tramite
            myCommand.CommandText = update_respuesta
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Actualiza_guia_envio = "Imposible actualizar guía en la tabla respuesta  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Actualiza_guia_envio = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Actualiza_guia_envio = "An exception of type " + ex.GetType().ToString() + _
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Actualiza_guia_envio = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Registra_guia_envio(ByVal empresa_envio As String, _
        ByRef guia_envio As String, ByVal pag As Page, ByVal nombre_destinatario As String, _
        ByVal operador_mensajeria_interna As String, _
         ByVal nit_identificacion As String, _
        ByVal direccion As String, ByVal telefono As String, ByVal correo_electrnico As String, _
        ByVal anexo As String, ByRef id_guia As Integer, ByVal nombre_remitente As String, ByVal id_destinatario_externo As Integer, _
        ByVal id_usuario_registro As Integer, ByVal id_tipo_manual_automatico As Integer, ByVal radicado As String, ByVal estado_envio_guia As Integer) As String
        '---------------------------------------------------------
        'Función : Asigna la guía de envío al tramite
        'Fecha : 2016-04-18
        'Ing : Miguel Angel Urueta
        '---------------------------------------------------------
        'If radicado <> "" Then
        '    Registra_guia_envio = "Por favor informe el consecutivo radicado"
        '    Exit Function
        'End If
        If empresa_envio = "" Then
            Registra_guia_envio = "Por favor seleccione el operador de envío"
            Exit Function
        End If
        If nombre_destinatario = "" Then
            Registra_guia_envio = "Por favor informe el nombre del destinatario de la correspondencia"
            Exit Function
        End If
        If direccion = "" Then
            Registra_guia_envio = "Por favor informe la dirección del destinatario de la correspondencia"
            Exit Function
        End If
        If nombre_remitente = "" Then
            Registra_guia_envio = "Por favor seleccione el remitente de la correspondencia"
            Exit Function
        End If
        Dim refclas As New Classgestionrespuesta
        Dim refclas_rad As New ClassRadicador
        Dim ref_calss_system As New Class_system_plantilla_radicado
        Dim Result As String = ""
        Dim id_tipo As Integer = 0
        '-------------------------------------------------------
        'Retorna nombre plantilla radicado
        '-------------------------------------------------------
        Dim id_plantilla_default As Integer = 0
        Dim nombre_plantilla_default As String = ""
        If radicado <> "" Then
            Result = ref_calss_system.Solicita_plantilla_default_respuesta(id_plantilla_default, nombre_plantilla_default)
            If Result <> "YES" Then
                Registra_guia_envio = Result
                Exit Function
            End If
            '-----Verifica la existencia del radicado en la plantilla
            Result = refclas_rad.Verifica_existencia_radicado_en_plantilla(nombre_plantilla_default, radicado)
            If Result <> "YES" Then
                Registra_guia_envio = Result
                Exit Function
            End If
            '----Verfica exisencia del radicado como parte de una guía
            Result = Me.Verifica_existencia_radicado_en_guia(radicado)
            If Result <> "YES" Then
                Registra_guia_envio = Result
                Exit Function
            End If
        End If
        Result = Me.retorna_tipo_empresa_registro_guia(empresa_envio, id_tipo)
        If Result <> "YES" Then
            Registra_guia_envio = Result
            Exit Function
        End If
        If id_tipo = 0 Then
            If guia_envio = "" Then
                Registra_guia_envio = "Por favor informe el consecutivo de guía del operador externo"
                Exit Function
            End If
        End If
        If id_tipo = 1 Then
            If operador_mensajeria_interna = "" Then
                Registra_guia_envio = "Por favor seleccione el usuario de mensajeria interna"
                Exit Function
            End If
        End If

        '------------------------------------------------------
        'Retorna usuario de getion perfil mensajero
        '------------------------------------------------------
        Dim id_operador_interno As Integer = 0
        Dim Refclasradicado As New ClassRadicador
        Dim Ref_Class_remit_dest_interno As New Class_remit_dest_interno
        If id_tipo = 1 Then
            Result = Ref_Class_remit_dest_interno.Retorna_Id_Destinatario(operador_mensajeria_interna, _
                                                                          id_operador_interno)
            If Result <> "YES" Then
                Registra_guia_envio = Result
                Exit Function
            End If
        End If

        '-------------------------------------------------------
        'Retorna id del remitente de la guía
        '-------------------------------------------------------
        Dim id_remitente As Integer = 0
        Result = Ref_Class_remit_dest_interno.Retorna_Id_Destinatario(nombre_remitente, _
                                                                      id_remitente)
        If Result <> "YES" Then
            Registra_guia_envio = Result
            Exit Function
        End If
        Dim id_area As Integer = 0
        Dim Ref_class_remit_dest_int As New Class_remit_dest_interno
        Result = Ref_class_remit_dest_int.Solicita_id_area_nombre_area_destinatario(id_remitente, _
                                                                     id_area, _
                                                                     "")
        If Result <> "YES" Then
            Registra_guia_envio = Result
            Exit Function
        End If
        '------------------------------------------------------
        'Retorna id empresa envio 
        '------------------------------------------------------
        Dim id_empresa_envio As Integer = 0
        Result = Me.retorna_id_empresa_mensajeria(empresa_envio, id_empresa_envio)
        If Result <> "YES" Then
            Registra_guia_envio = Result
            Exit Function
        End If
        '------------------------------------------------------
        'Asignar datos radicacion
        '------------------------------------------------------
        Dim refclasalmacen As New ClassAlmacenamiento
        Dim date1al As String = Date.Now
        Result = ""
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(date1al)
        If Result <> "YES" Then
            Registra_guia_envio = "Imposible formatear fecha " & Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim mySqldatReader As MySqlDataReader
        Dim sqlresultinsert As Integer = 0
        Dim Class_zero_fill As New Class_zero_fill
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim Parametro_Select_System1 As String = " SELECT conse_cutivo_guia FROM registro_plantilla_guia " & _
            " where Nombre_Plantilla_Guia='" & "Ra_Guia_Interna" & "' for update"
            Dim Parametro_Actualiza_System1 As String = ""
            Dim consecutivo_guia As String = ""
            If id_tipo = 0 Then
                consecutivo_guia = guia_envio
            End If
            If id_tipo = 1 Then
                myCommand.CommandText = Parametro_Select_System1
                mySqldatReader = myCommand.ExecuteReader()
                If mySqldatReader Is Nothing Then
                    Registra_guia_envio = "Imposible Encontrar Registro En Tabla registro_plantilla_guia Error Conexion"
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                If mySqldatReader.HasRows = False Then
                    Registra_guia_envio = "Imposible Encontrar Registro En Tabla registro_plantilla_guia"
                    'myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                '***********************************************************************
                'Valores recuperados de la consulta de la tabla registro_plantilla_guia
                '***********************************************************************
                Dim id_incremento As Integer = 0
                mySqldatReader.Read()
                id_incremento = mySqldatReader.Item(0)
                mySqldatReader.Close()
                id_incremento = id_incremento + 1
                consecutivo_guia = id_incremento.ToString
                Result = Class_zero_fill.zero_fill(consecutivo_guia, 6, "0")
                If Result <> "YES" Then
                    Registra_guia_envio = "Imposible agregar zerofill " & Result
                    'myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                Dim año_radic As String = Now.Year.ToString.Substring(2, 2)
                consecutivo_guia = año_radic & consecutivo_guia
                guia_envio = consecutivo_guia
                Parametro_Actualiza_System1 = "update registro_plantilla_guia set conse_cutivo_guia = " & "'" & id_incremento & "'" & _
                " where  Nombre_Plantilla_Guia='" & "Ra_Guia_Interna'"
                myCommand.CommandText = Parametro_Actualiza_System1
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Registra_guia_envio = "Imposible actualizar el consecutivo de guía  "
                    myConnection.Close()
                    Exit Function
                End If
            End If
            Dim ref_nit_identificacion As String = ""
            If nit_identificacion = "" Then
                ref_nit_identificacion = "null"
            Else
                ref_nit_identificacion = "'" & nit_identificacion & "'"
            End If
            Dim ref_telefono As String = ""
            If telefono = "" Then
                ref_telefono = "null"
            Else
                ref_telefono = "'" & telefono & "'"
            End If
            Dim ref_correo_electrnico As String = ""
            If correo_electrnico = "" Then
                ref_correo_electrnico = "Null"
            Else
                ref_correo_electrnico = "'" & correo_electrnico & "'"
            End If
            Dim ref_anexo As String = ""
            If anexo = "" Then
                ref_anexo = "Null"
            Else
                ref_anexo = "'" & anexo & "'"
            End If
            Dim ref_radicado As String = ""
            If radicado = "" Then
                ref_radicado = "Null"
            Else
                ref_radicado = "'" & radicado & "'"
            End If

            Dim Parametro_Insercio As String = "insert into ra_guia_interna (areas_depart_radicacion_Codigo_Area,Remit_Dest_Interno_id_Remit_Dest_Int," & _
           "ra_empresa_envio_ID_EMPRESA_ENVIO,Concecutivo_Guia,Fecha_Registro_Guia,NIT_IDENTIFICACION,NOMBRE_RAZON_SOCIAL,DIRECCION,TELEFONO," & _
           "CORREO_ELECTRONICO,Destinatario_Ext,RADICADO,ID_MENSAJERO_INTERNO, ID_TIPO_GUIA,ANEXO,ESTADO_GUIA,FECHA_ENVIO_GUIA,ID_TIPO_MANUAL_AUTOMATICO,ID_USUARIO_GESTION_TRANSAC,ESTADO_CONFIRMACION_GUIA) values (" & id_area & "," & id_remitente & "," & _
           id_empresa_envio & ",'" & consecutivo_guia & "','" & date1al & "'," & ref_nit_identificacion & ",'" & nombre_destinatario & "','" & _
           direccion & "'," & ref_telefono & "," & ref_correo_electrnico & "," & id_destinatario_externo & "," & ref_radicado & "," & _
           id_operador_interno & "," & id_tipo & "," & ref_anexo & ",0,'" & date1al & "'," & id_tipo_manual_automatico & "," & id_usuario_registro & "," & estado_envio_guia & ")"
            myCommand.CommandText = Parametro_Insercio
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Registra_guia_envio = "Imposible registrar la guía de envío "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim obisert = myCommand.LastInsertedId
            id_guia = myCommand.LastInsertedId
            myTrans.Commit()
            myConnection.Close()
            Registra_guia_envio = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Registra_guia_envio = "An exception of type " + ex.GetType().ToString() + _
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Registra_guia_envio = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Asigna_guia_envio(ByVal id_tramite As Integer, ByVal empresa_envio As String, _
        ByRef guia_envio As String, ByVal pag As Page, ByVal nombre_destinatario As String, _
        ByVal operador_mensajeria_interna As String, _
         ByVal nit_identificacion As String, _
        ByVal direccion As String, ByVal telefono As String, ByVal correo_electrnico As String, ByVal id_tipo As Integer, ByVal anexo As String, ByRef id_guia As Integer, _
        ByVal id_usuario_registro As Integer, ByVal id_tipo_manual_automatico As Integer) As String
        '---------------------------------------------------------
        'Función : Asigna la guía de envío al tramite
        'Fecha : 2016-04-18
        'Ing : Miguel Angel Urueta
        '---------------------------------------------------------
        If nombre_destinatario = "" Then
            Asigna_guia_envio = "Por favor informe el nombre del destinatario de la correspondencia"
            Exit Function
        End If
        If direccion = "" Then
            Asigna_guia_envio = "Por favor informe la dirección del destinatario de la correspondencia"
            Exit Function
        End If
        If id_tipo = 0 Then
            If guia_envio = "" Then
                Asigna_guia_envio = "Por favor informe el consecutivo de guía del operador externo"
                Exit Function
            End If
        End If
        If id_tipo = 1 Then
            If operador_mensajeria_interna = "" Then
                Asigna_guia_envio = "Por favor seleccione el usuario de mensajeria interna"
                Exit Function
            End If
        End If
        Dim refclas As New Classgestionrespuesta
        Dim Result As String = ""
        Dim stru_envio As stru_envio = Nothing
        Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
        Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_tramite, stru_envio)
        If Result <> "YES" Then
            Asigna_guia_envio = Result
            Exit Function
        End If

        '------------------------------------------------------
        'Retorna usuario de getion perfil mensajero
        '------------------------------------------------------
        Dim id_operador_interno As Integer = 0
        Dim Refclasradicado As New ClassRadicador
        Dim Ref_Class_remit_dest_interno As New Class_remit_dest_interno
        If id_tipo = 1 Then
            Result = Ref_Class_remit_dest_interno.Retorna_Id_Destinatario(operador_mensajeria_interna, _
                                                                          id_operador_interno)
            If Result <> "YES" Then
                Asigna_guia_envio = Result
                Exit Function
            End If
        End If
        '------------------------------------------------------
        'Retorna id empresa envio 
        '------------------------------------------------------
        Dim id_empresa_envio As Integer = 0
        Result = Me.retorna_id_empresa_mensajeria(empresa_envio, id_empresa_envio)
        If Result <> "YES" Then
            Asigna_guia_envio = Result
            Exit Function
        End If

        '------------------------------------------------------
        'Asignar datos radicacion
        '------------------------------------------------------
        Dim refclasalmacen As New ClassAlmacenamiento
        Dim date1al As String = Date.Now
        Result = ""
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(date1al)
        If Result <> "YES" Then
            Asigna_guia_envio = "Imposible formatear fecha " & Result
            Exit Function
        End If

        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim mySqldatReader As MySqlDataReader
        Dim sqlresultinsert As Integer = 0
        Dim Class_zero_fill As New Class_zero_fill
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim Parametro_Select_System1 As String = " SELECT conse_cutivo_guia FROM registro_plantilla_guia " & _
            " where Nombre_Plantilla_Guia='" & "Ra_Guia_Interna" & "' for update"
            Dim Parametro_Actualiza_System1 As String = ""
            Dim consecutivo_guia As String = ""
            If id_tipo = 0 Then
                consecutivo_guia = guia_envio
            End If
            If id_tipo = 1 Then
                myCommand.CommandText = Parametro_Select_System1
                mySqldatReader = myCommand.ExecuteReader()
                If mySqldatReader Is Nothing Then
                    Asigna_guia_envio = "Imposible Encontrar Registro En Tabla registro_plantilla_guia Error Conexion"
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                If mySqldatReader.HasRows = False Then
                    Asigna_guia_envio = "Imposible Encontrar Registro En Tabla registro_plantilla_guia"
                    'myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                '***********************************************************************
                'Valores recuperados de la consulta de la tabla registro_plantilla_guia
                '***********************************************************************
                Dim id_incremento As Integer = 0
                mySqldatReader.Read()
                id_incremento = mySqldatReader.Item(0)
                mySqldatReader.Close()
                id_incremento = id_incremento + 1
                consecutivo_guia = id_incremento.ToString
                Result = Class_zero_fill.zero_fill(consecutivo_guia, 6, "0")
                If Result <> "YES" Then
                    Asigna_guia_envio = "Imposible agregar zerofill " & Result
                    'myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                Dim año_radic As String = Now.Year.ToString.Substring(2, 2)
                consecutivo_guia = año_radic & consecutivo_guia
                guia_envio = consecutivo_guia
                Parametro_Actualiza_System1 = "update registro_plantilla_guia set conse_cutivo_guia = " & "'" & id_incremento & "'" & _
                " where  Nombre_Plantilla_Guia='" & "Ra_Guia_Interna'"
                myCommand.CommandText = Parametro_Actualiza_System1
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Asigna_guia_envio = "Imposible actualizar el consecutivo de guía  "
                    myConnection.Close()
                    Exit Function
                End If
            End If
            Dim ref_nit_identificacion As String = ""
            If nit_identificacion = "" Then
                ref_nit_identificacion = "null"
            Else
                ref_nit_identificacion = "'" & nit_identificacion & "'"
            End If
            Dim ref_telefono As String = ""
            If telefono = "" Then
                ref_telefono = "null"
            Else
                ref_telefono = "'" & telefono & "'"
            End If
            Dim ref_correo_electrnico As String = ""
            If correo_electrnico = "" Then
                ref_correo_electrnico = "Null"
            Else
                ref_correo_electrnico = "'" & correo_electrnico & "'"
            End If
            Dim ref_anexo As String = ""
            If anexo = "" Then
                ref_anexo = "Null"
            Else
                ref_anexo = "'" & anexo & "'"
            End If
            Dim Parametro_Insercio As String = "insert into ra_guia_interna (areas_depart_radicacion_Codigo_Area,Remit_Dest_Interno_id_Remit_Dest_Int," & _
           "ra_empresa_envio_ID_EMPRESA_ENVIO,Concecutivo_Guia,Fecha_Registro_Guia,NIT_IDENTIFICACION,NOMBRE_RAZON_SOCIAL,DIRECCION,TELEFONO," & _
           "CORREO_ELECTRONICO,Destinatario_Ext,RADICADO,ID_MENSAJERO_INTERNO, ID_TIPO_GUIA,ANEXO,ID_TIPO_MANUAL_AUTOMATICO,ID_USUARIO_GESTION_TRANSAC) values (" & stru_envio.ID_AREA & "," & stru_envio.ID_REMIT_DEST_INT & "," & _
           id_empresa_envio & ",'" & consecutivo_guia & "','" & date1al & "'," & ref_nit_identificacion & ",'" & nombre_destinatario & "','" & _
           direccion & "'," & ref_telefono & "," & ref_correo_electrnico & "," & stru_envio.codigo_dest_externo & ",'" & stru_envio.RADICADO_RESPUESTA & "'," & _
           id_operador_interno & "," & id_tipo & "," & ref_anexo & "," & id_tipo_manual_automatico & "," & id_usuario_registro & ")"
            myCommand.CommandText = Parametro_Insercio
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Asigna_guia_envio = "Imposible registrar la guía de envío "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim obisert = myCommand.LastInsertedId
            id_guia = myCommand.LastInsertedId
            Dim update_respuesta As String = "update ra_respuesta_radicado set EMPRESA_ENVIO='" & empresa_envio & "',GUIA_ENVIO='" & consecutivo_guia & "'," & _
                "  Numero_Guia_Interna=" & obisert & " where ID_RESPUESTA_RADICADO=" & id_tramite
            myCommand.CommandText = update_respuesta
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Asigna_guia_envio = "Imposible actualizar guía en la tabla respuesta  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Asigna_guia_envio = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Asigna_guia_envio = "An exception of type " + ex.GetType().ToString() + _
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Asigna_guia_envio = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function anula_guia_envio_manual(ByVal id_guia As Integer) As String
        Try
            Dim Result As String = ""
            Dim stru As guia_envio = Nothing
            Result = Me.Retorna_datos_estructura_guia(id_guia, stru)
            If Result <> "YES" Then
                anula_guia_envio_manual = Result
                Exit Function
            End If
            If stru.ESTADO_CONFIRMACION_GUIA > 2 Then
                If stru.ESTADO_CONFIRMACION_GUIA = 3 Then
                    anula_guia_envio_manual = "La guía se encuentra en estado entregado imposible anular"
                    Exit Function
                End If
                If stru.ESTADO_CONFIRMACION_GUIA = 4 Then
                    anula_guia_envio_manual = "La guía se encuentra en estado de devolución permanente imposible anular"
                    Exit Function
                End If
            End If
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim update_guia As String = "update ra_guia_interna set ESTADO_GUIA=1 where Id_guia_envio=" & id_guia
            Result = ref.SELECTION_INSERT_COMMAND(update_guia)
            If Result <> "YES" Then
                anula_guia_envio_manual = Result
                Exit Function
            Else
                anula_guia_envio_manual = "YES"
            End If
        Catch ex As Exception
            anula_guia_envio_manual = "Inconsistencia función anula_guia_envio_manual " & ex.Message
        End Try
    End Function
    Function anula_guia_envio_respuesta(ByVal id_tramite_respuesta As Integer, _
                                        ByVal id_guia As Integer, _
                                        ByRef operador_mensajeria As String, _
                                        ByRef numero_guia As String) As String
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim update_guia As String = "update ra_guia_interna set ESTADO_GUIA=1 where Id_guia_envio=" & id_guia
            myCommand.CommandText = update_guia
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                anula_guia_envio_respuesta = "Imposible actualizar guía en la tabla respuesta  "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim update_respuesta As String = "update ra_respuesta_radicado set EMPRESA_ENVIO=" & "null" & ",GUIA_ENVIO=" & "null" & "," & _
               "  Numero_Guia_Interna=" & "null" & " where ID_RESPUESTA_RADICADO=" & id_tramite_respuesta
            myCommand.CommandText = update_respuesta
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                anula_guia_envio_respuesta = "Imposible actualizar guía en la tabla respuesta  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            operador_mensajeria = ""
            numero_guia = ""
            anula_guia_envio_respuesta = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    anula_guia_envio_respuesta = "An exception of type " + ex.GetType().ToString() + _
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            anula_guia_envio_respuesta = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Envia_respuesta_radicado(ByVal id_tramite As Integer,
                                      ByVal empresa_envio As String,
                                      ByVal guia_envio As String,
                                      ByVal radicado_respuesta As String,
                                      ByVal id_guia_envio As Integer) As String

        Dim refclas As New ClassRadicador
        Dim date1al As String = Date.Now
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Dim Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(date1al)
        If Result <> "YES" Then
            Envia_respuesta_radicado = Result
            Exit Function
        End If
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
        Dim logi_user As String = HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION")
        Dim hour As String = Date.Now.Hour
        Dim datehora As String = Date.Now.Hour.ToString
        Dim ref_radicado_respuesta As String = ""
        If radicado_respuesta = "" Then
            ref_radicado_respuesta = "null"
        Else
            ref_radicado_respuesta = "'" & radicado_respuesta & "'"
        End If
        Dim update As String = "Update ra_respuesta_radicado set ESTADO_ENVIO=1, ID_USUARIO_RADICADO=" &
            id_user & ",FECHA_ENVIO='" & date1al & "', HORA_ENVIO='" & datehora & "',ESTADO_RESPUESTA=2  " &
            ",EMPRESA_ENVIO='" & empresa_envio & "', GUIA_ENVIO='" & guia_envio & "'" &
            ",RADICADO_RESPUESTA=" & ref_radicado_respuesta & ",ID_USUARIO_GESTION_ENVIA=" & HttpContext.Current.Session("GA_IDUSUARIOGESTION") &
            "  where ID_RESPUESTA_RADICADO=" & id_tramite
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        Try

            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = update
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Envia_respuesta_radicado = "Imposible actualizar envio de respuesta  "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If

            Dim cambios_campos As String = "Asigna a la empresa : " & empresa_envio & "Con codigo de guía : " & guia_envio & " relacionado al radicado : " & radicado_respuesta
            update = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO,CAMPOS,IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO) VALUES (" &
                "'" & "ENVIA RESPUESTA" & "','" & logi_user & "','" & id_user & "','" & date1al & "','" & id_tramite & "','" & cambios_campos &
                "','" & iphost & "','" & hour.ToString & "','" & "RADICACION WEB" & "')"
            myCommand.CommandText = update
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Envia_respuesta_radicado = "Imposible actualizar fecha limite de respuesta  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim sql_update_guia As String = ""
            If id_guia_envio <> 0 Then
                sql_update_guia = "update ra_guia_interna set ESTADO_CONFIRMACION_GUIA=" & 2 & ",FECHA_ENVIO_GUIA='" & date1al & "' " &
                " where Id_guia_envio=" & id_guia_envio
                myCommand.CommandText = sql_update_guia
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Envia_respuesta_radicado = "Imposible actualizar guia envío  "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            myTrans.Commit()
            myConnection.Close()
            Envia_respuesta_radicado = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Envia_respuesta_radicado = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Envia_respuesta_radicado = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Devolucion_envio_respuesta(ByVal id_tramite As Integer, ByVal id_guia_envio As Integer) As String

        Dim refclas As New ClassRadicador
        Dim date1al As String = Date.Now
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Dim Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(date1al)
        If Result <> "YES" Then
            Devolucion_envio_respuesta = Result
            Exit Function
        End If
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("RA_ID_USUARIO")
        Dim logi_user As String = HttpContext.Current.Session.Item("RA_LOGIN_USER")
        Dim hour As String = Date.Now.Hour
        Dim datehora As String = Date.Now.Hour.ToString
        Dim ref_radicado_respuesta As String = ""
        Dim update As String = "Update ra_respuesta_radicado set ESTADO_ENVIO=0, ID_USUARIO_RADICADO=" & _
           id_user & ",FECHA_ENVIO=" & "null" & ", HORA_ENVIO=" & "null" & ",ESTADO_RESPUESTA=1  " & _
           "  where ID_RESPUESTA_RADICADO=" & id_tramite
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        Try

            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = update
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Devolucion_envio_respuesta = "Imposible actualizar tipo documento tipo tramite  "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If

            Dim cambios_campos As String = "Asigna a la empresa : " & "" & "Con codigo de guía : " & "" & " relacionado al radicado : " & ""
            update = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO,CAMPOS,IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO) VALUES (" & _
                "'" & "ENVIA A PENDIENTES POR ENVIAR" & "','" & logi_user & "','" & id_user & "','" & date1al & "','" & id_tramite & "','" & cambios_campos & _
                "','" & iphost & "','" & hour.ToString & "','" & "RADICACION WEB" & "')"
            myCommand.CommandText = update
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Devolucion_envio_respuesta = "Imposible actualizar fecha limite de respuesta  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim sql_update_guia As String = ""
            If id_guia_envio <> 0 Then
                sql_update_guia = "update ra_guia_interna set ESTADO_CONFIRMACION_GUIA=" & 1 & ",FECHA_ENVIO_GUIA=" & "null" & " " & _
                " where Id_guia_envio=" & id_guia_envio
                myCommand.CommandText = sql_update_guia
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Devolucion_envio_respuesta = "Imposible actualizar guia envío  "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            myTrans.Commit()
            myConnection.Close()
            Devolucion_envio_respuesta = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Devolucion_envio_respuesta = "An exception of type " + ex.GetType().ToString() + _
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Devolucion_envio_respuesta = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Asigna_centro_envio_respuesta(ByVal radicado_respuesta As String) As String

        Dim refclas As New ClassRadicador
        Dim date1al As String = Date.Now
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Dim Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(date1al)
        If Result <> "YES" Then
            Asigna_centro_envio_respuesta = Result
            Exit Function
        End If
        '----------------------------------------------------------
        'Retorna id respuesta con radicado de respuesta
        '----------------------------------------------------------
        Dim refclasgestion As New Classgestionrespuesta
        Dim id_respuesta As Integer = 0
        Result = refclasgestion.Reorna_id_respuesta_radicado_respuesta(radicado_respuesta, id_respuesta)
        If Result <> "YES" Then
            Asigna_centro_envio_respuesta = Result
            Exit Function
        End If
        If id_respuesta = -1 Then
            Asigna_centro_envio_respuesta = "El radicado de respuesta no tiene un asignado una identificación de respuesta "
            Exit Function
        End If
        Dim stru As stru_envio = Nothing
        Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
        Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta, stru)
        If Result <> "YES" Then
            Asigna_centro_envio_respuesta = Result
            Exit Function
        End If

        '---------------------------------------------------------
        'Verifica que el radicado tenga una respuesta en firme
        '---------------------------------------------------------
        If stru.FECHA_RESPUETA = "" Then
            Asigna_centro_envio_respuesta = "El radicado no tiene una respuesta para realizar el envío"
            Exit Function
        End If
        '--------------------------------------------------------
        'Verifica que el radicado tengo un documento de respuesta
        '--------------------------------------------------------
        If stru.ID_IMAGEN = 0 Then
            Asigna_centro_envio_respuesta = "El radicado no tiene un documento de respuesta para realizar el envío"
            Exit Function
        End If
        '--------------------------------------------------------
        'Verifica la existencia de una guía de respuesta
        '--------------------------------------------------------
        If stru.GUIA_ENVIO <> "" Then
            Asigna_centro_envio_respuesta = "El radicado registra la guía de envío " & stru.GUIA_ENVIO & " impodible asignar manualmente para envío"
            Exit Function
        End If
        '---------------------------------------------------------
        'Verifica que el sistema este en estado de envío  ESTADO_ENVIO=1 AND ESTADO_RESPUESTA=2"
        '---------------------------------------------------------
        If stru.ESTADO_ENVIO <> 0 Then
            Asigna_centro_envio_respuesta = "El sistema registra una aignación de envio para el radicado de respuesta"
            Exit Function
        End If
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("RA_ID_USUARIO")
        Dim logi_user As String = HttpContext.Current.Session.Item("RA_LOGIN_USER")
        Dim hour As String = Date.Now.Hour
        Dim datehora As String = Date.Now.Hour.ToString
        Dim ref_radicado_respuesta As String = ""
        Dim update As String = "Update ra_respuesta_radicado set ESTADO_ENVIO=0, ID_USUARIO_RADICADO=" & _
           id_user & ",FECHA_ENVIO=" & "null" & ", HORA_ENVIO=" & "null" & ",ESTADO_RESPUESTA=1  " & _
           "  where ID_RESPUESTA_RADICADO=" & id_respuesta
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        Try

            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = update
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Asigna_centro_envio_respuesta = "Imposible actualizar tipo documento tipo tramite  "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If

            Dim cambios_campos As String = "Asigna a la empresa : " & "" & "Con codigo de guía : " & "" & " relacionado al radicado : " & ""
            update = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO,CAMPOS,IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO) VALUES (" & _
                "'" & "ENVIA A PENDIENTES POR ENVIAR" & "','" & logi_user & "','" & id_user & "','" & date1al & "','" & id_respuesta & "','" & cambios_campos & _
                "','" & iphost & "','" & hour.ToString & "','" & "RADICACION WEB" & "')"
            myCommand.CommandText = update
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Asigna_centro_envio_respuesta = "Imposible actualizar log de respuesta radicado "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If

            myTrans.Commit()
            myConnection.Close()
            Asigna_centro_envio_respuesta = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Asigna_centro_envio_respuesta = "An exception of type " + ex.GetType().ToString() + _
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Asigna_centro_envio_respuesta = "Error General función Asigna_centro_envio_respuesta " & e.Message
            Exit Function
        End Try
    End Function
    Function Archiva_guia_manual(ByVal id_guia As Integer, ByRef estado_actualizacion As String) As String
        Try
            Dim refclas As New ClassRadicador
            Dim ref_clas_gestion As New Classgestionrespuesta
            Dim stru As guia_envio = Nothing
            Dim Result As String = ""
            Result = Me.Retorna_datos_estructura_guia(id_guia, stru)
            If Result <> "YES" Then
                Archiva_guia_manual = Result
                Exit Function
            End If
            If stru.ESTADO_CONFIRMACION_GUIA <= 2 Then
                estado_actualizacion = "NO"
                Archiva_guia_manual = "YES"
                Exit Function
            End If
            Dim refclas_gestion_fechas As New ClassGestionFechas
            Dim stiempo As Object = Nothing
            Dim minuno As Object = Nothing
            Dim hora As Object = Nothing
            Dim dias_calendario As Object = Nothing
            Dim dias_no_habiles As Object = Nothing
            Result = refclas_gestion_fechas.Solicita_tiempo_respuesta_tramite(stru.FECHA_ENVIO_GUIA, _
                                                                             stiempo, _
                                                                             hora, _
                                                                             minuno, _
                                                                             dias_calendario, _
                                                                             dias_no_habiles, _
                                                                             stru.FECHA_RECIBIDO_GUIA)
            If Result <> "YES" Then
                Archiva_guia_manual = Result
                Exit Function
            End If
            Dim sql_update_guia As String = ""
            sql_update_guia = "update ra_guia_interna set TIEMPO_RESPUESTA=" & Val(sTiempo) & "  where Id_guia_envio=" & id_guia
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Result = ref.SELECTION_INSERT_COMMAND(sql_update_guia)
            If Result <> "YES" Then
                Archiva_guia_manual = Result
                Exit Function
            Else
                estado_actualizacion = "YES"
                Archiva_guia_manual = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Archiva_guia_manual = "Inconsistencia función Archiva_guia_manual " & ex.Message
        End Try
    End Function
    Function Archiva_respuesta(ByVal id_tramite As Integer, ByVal fecha_respuesta_usuario As String, _
        ByVal hora_respuesta_envio As String, ByVal id_guia_envio As Integer, ByVal nota_cliente As String, _
        ByVal descripcion_confirmacion_guia As String) As String
        If id_guia_envio <> 0 Then
            If descripcion_confirmacion_guia = "" Then
                Archiva_respuesta = "Seleccione el estado de la guia de envío "
                Exit Function
            End If
            If fecha_respuesta_usuario = "" Then
                Archiva_respuesta = "Debe informar la fecha de recibido "
                Exit Function
            End If
        End If
        Dim refclas As New ClassRadicador
        Dim date1al As String = Date.Now
        Dim hor As String = TimeOfDay
        Dim ref_fecha_respuesta_usuario As String = fecha_respuesta_usuario
        Dim matri() As String = ref_fecha_respuesta_usuario.Split("-")
        ref_fecha_respuesta_usuario = matri(2) & "/" & matri(1) & "/" & matri(0)
        ref_fecha_respuesta_usuario = ref_fecha_respuesta_usuario & " " & hor
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Dim Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(date1al)
        If Result <> "YES" Then
            Archiva_respuesta = Result
            Exit Function
        End If
        Dim stru As guia_envio = Nothing
        Result = Me.Retorna_datos_estructura_guia(id_guia_envio, stru)
        If Result <> "YES" Then
            Archiva_respuesta = Result
            Exit Function
        End If
        Dim numero_dias_no_habil As Integer = 0
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("RA_ID_USUARIO")
        Dim logi_user As String = HttpContext.Current.Session.Item("RA_LOGIN_USER")
        Dim hour As String = Date.Now.Hour
        Dim datehora As String = Date.Now.Hour.ToString
        Dim ref_radicado_respuesta As String = ""
        Dim ref_clas_gestion As New Classgestionrespuesta
        Dim update As String = "Update ra_respuesta_radicado set ESTADO_ENVIO=2,ESTADO_RESPUESTA=3, FECHA_RECIBO_FISICO='" & _
                    fecha_respuesta_usuario & "',HORA_RECIBO_FISICO='" & hora_respuesta_envio & "',ID_USUARIO_GESTION_ARCHIVA=" & HttpContext.Current.Session("GA_IDUSUARIOGESTION") & _
                    "  where ID_RESPUESTA_RADICADO=" & id_tramite
        Dim stiempo As Object = Nothing
        Dim minuno As Object = Nothing
        Dim hora As Object = Nothing
        Dim dias_calendario As Object = Nothing
        Dim dias_no_habiles As Object = Nothing
        Result = refclas_gestion_fechas.Solicita_tiempo_respuesta_tramite(stru.FECHA_ENVIO_GUIA, _
                                                                         stiempo, _
                                                                         hora, _
                                                                         minuno, _
                                                                         dias_calendario, _
                                                                         dias_no_habiles, _
                                                                         ref_fecha_respuesta_usuario)
        If Result <> "YES" Then
            Archiva_respuesta = Result
            Exit Function
        End If
        Dim refnota_cliente As String = ""
        If nota_cliente = "" Then
            refnota_cliente = "null"
        Else
            refnota_cliente = "'" & nota_cliente & "'"
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        Try

            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = update
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Archiva_respuesta = "Imposible actualizar tipo documento tipo tramite  "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If

            Dim cambios_campos As String = "Asigna FECHA_RECIBO_FISICO : " & fecha_respuesta_usuario & "Con HORA_RECIBO_FISICO : " & hora_respuesta_envio
            update = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO,CAMPOS,IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO) VALUES (" & _
                "'" & "ARCHIVA ENVIO" & "','" & logi_user & "','" & id_user & "','" & date1al & "','" & id_tramite & "','" & cambios_campos & _
                "','" & iphost & "','" & hour.ToString & "','" & "RADICACION" & "')"
            myCommand.CommandText = update
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Archiva_respuesta = "Imposible actualizar fecha limite de respuesta  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim sql_update_guia As String = ""
            If id_guia_envio <> 0 Then
                Dim estado_confirmacion_guia As Integer = 3
                If descripcion_confirmacion_guia = "Entregada" Then
                    estado_confirmacion_guia = 3
                End If
                If descripcion_confirmacion_guia = "Devolucion Permanente" Then
                    estado_confirmacion_guia = 4
                End If
                sql_update_guia = "update ra_guia_interna set ESTADO_CONFIRMACION_GUIA=" & estado_confirmacion_guia & ",FECHA_RECIBIDO_GUIA='" & fecha_respuesta_usuario & "',NOTA_CLIENTE=" & _
                refnota_cliente & ",TIEMPO_RESPUESTA=" & Val(sTiempo) & "  where Id_guia_envio=" & id_guia_envio
                myCommand.CommandText = sql_update_guia
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Archiva_respuesta = "Imposible actualizar guia envío  "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            myTrans.Commit()
            myConnection.Close()
            Archiva_respuesta = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Archiva_respuesta = "An exception of type " + ex.GetType().ToString() + _
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Archiva_respuesta = "Error General " & e.Message
            Exit Function
        End Try
    End Function
End Class
