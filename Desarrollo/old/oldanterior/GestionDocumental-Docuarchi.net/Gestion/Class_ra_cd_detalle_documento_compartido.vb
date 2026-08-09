Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Public Class Class_ra_cd_detalle_documento_compartido
    Function SolicitaCertificadoDesicionDocumentoCompartido(ByVal IdDcoumentoCompartido As Integer,
                                                            ByVal IdUsuarioGestionDesicion As Integer,
                                                            ByVal DescripcionDesicion As String,
                                                            ByVal stru_compartido_general As STRU_DOCUMENTO_COMPARTIDO_GENERAL,
                                                            ByVal EstructuraDocumentoCompartido() As stru_docu_compartido,
                                                            ByVal MatriDocumentoGabinete() As String,
                                                            ByRef FileCertificado As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita el archivo en formato padf con la certificacion de la decisción del documento
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdDcoumentoCompartido           : Representa la identificación de la tarea compartida
        'IdUsuarioGestionDesicion        : Representa la identificación del usuario de gestión
        'DescripcionDesicion             : Representa la descripcion de la decisión
        'stru_compartido_general         : Representa la estructura general del documento compartido
        'EstructuraDocumentoCompartido   : Representa la estructura de documentos compartidos
        'MatriDocumentoGabinete          : Representa la estructura de la matriz de documentos del gabinete
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'FileCertificado                 : Retorna la ruta del documento del certificado
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim Ref_clas_docu_compart_usuario As New Class_ra_cd_usuarios_documentos_compartidos
        Dim DetalleRegistroUsuarioCompartido() As Detalle_registro_compartido_usuario = Nothing
        Dim Result As String = ""
        Result = Ref_clas_docu_compart_usuario.SolicitaRegistroDetalleUsuariosRelacionadosADocumentoCompartido(IdDcoumentoCompartido,
                                                                                                               DetalleRegistroUsuarioCompartido)
        If Result <> "YES" Then
            SolicitaCertificadoDesicionDocumentoCompartido = Result
            Exit Function
        End If
        For i As Integer = 0 To DetalleRegistroUsuarioCompartido.Length - 1
            If DetalleRegistroUsuarioCompartido(i).Remit_Dest_Interno_id_Remit_Dest_Int = IdUsuarioGestionDesicion Then
                DetalleRegistroUsuarioCompartido(i).DESCRIPCION_ESTADO_RESPUESTA = DescripcionDesicion
            End If
        Next
        Dim docu_relacion As String = ""
        For i2 As Integer = 0 To EstructuraDocumentoCompartido.Length - 1
            docu_relacion = docu_relacion & "ID : " & EstructuraDocumentoCompartido(i2).ID_IMAGEN & vbCrLf
            docu_relacion = docu_relacion & "GABINETE : " & EstructuraDocumentoCompartido(i2).NOMBRE_GABINETE & vbCrLf
            docu_relacion = docu_relacion & "DOCUMENTO : " & EstructuraDocumentoCompartido(i2).RUTA_DOCUMENTO & vbCrLf
        Next
        Dim NitEmpresa As String = ""
        Dim NombreEmpresa As String = ""
        Dim Class_empresa_gestion_documental As New Class_empresa_gestion_documental
        Result = Class_empresa_gestion_documental.Solicita_nombre_identificacion_empresa(NitEmpresa,
                                                                                         NombreEmpresa)
        If Result <> "YES" Then
            SolicitaCertificadoDesicionDocumentoCompartido = Result
            Exit Function
        End If
        Dim NombreSolicitante As String = ""
        Dim CargoSolicitante As String = ""
        Dim ref_class_remit_dest_int As New Class_remit_dest_interno
        Result = ref_class_remit_dest_int.Retorna_datos_caracterizacion_usuario_gestion(stru_compartido_general.Remit_Dest_Interno_id_remit_dest_Int,
                                                                                        NombreSolicitante,
                                                                                        CargoSolicitante,
                                                                                        "")
        If Result <> "YES" Then
            SolicitaCertificadoDesicionDocumentoCompartido = Result
            Exit Function
        End If
        Dim ClassGestionFechas As New ClassGestionFechas
        Dim dat As Date = Now
        Dim fecha As String = ""
        Result = ClassGestionFechas.Formatea_fecha_time_framework(dat,
                                                                  fecha)
        If Result <> "YES" Then
            SolicitaCertificadoDesicionDocumentoCompartido = Result
            Exit Function
        End If
        Dim RutaLogo As String = HttpContext.Current.Server.MapPath("../imagera/" & "logo_trd.png")
        If File.Exists(RutaLogo) = False Then
            SolicitaCertificadoDesicionDocumentoCompartido = "El sistema no registra el archivo de logo para integrar al detalle contacte a su administrador  " &
            RutaLogo
            Exit Function
        End If
        Dim Rutatemp As String = HttpContext.Current.Session.Item("GA_RUTA_TEMP_GESTION") & "\"
        If Directory.Exists(Rutatemp) = False Then
            Directory.CreateDirectory(Rutatemp)
        End If
        FileCertificado = Rutatemp & "temp_" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & ".pdf"
        If File.Exists(FileCertificado) = True Then
            Kill(FileCertificado)
        End If
        Dim doc As Document
        Dim writer As PdfWriter = Nothing
        Try
            doc = New Document(PageSize.LETTER)
            doc.SetPageSize(PageSize.LETTER.Rotate())
            writer = PdfWriter.GetInstance(doc,
                               New FileStream(FileCertificado, FileMode.Create))
            writer.AddViewerPreference(PdfName.PICKTRAYBYPDFSIZE, PdfBoolean.PDFTRUE)
            doc.Open()
            Dim _standardFont As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA,
               12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
            Dim _standardFont_datos_unidad_conservacion As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA,
            12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
            'doc.NewPage()
            Dim imagen As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(RutaLogo)
            imagen.BorderWidth = 0
            imagen.Alignment = Element.ALIGN_LEFT
            Dim percentage As Object = 0.0F
            percentage = 100 / imagen.Width
            imagen.ScalePercent(percentage * 80)
            'Insertamos la imagen en el documento  doc.PageNumber = 1
            doc.Add(imagen)
            Dim paragraf As New Paragraph
            paragraf = New Paragraph("CERTIFICADO DE DECISION DOCUMENTO COMPARTIDO", _standardFont)
            paragraf.Alignment = Element.ALIGN_CENTER
            doc.Add(paragraf)
            'paragraf = New Paragraph("Se emite certificado de decisión de documento compartido a la fecha " & fecha & " como constancia de decision " & stru_compartido_general.ESTADO_APROBACION, _standardFont)
            'paragraf.Alignment = Element.ALIGN_CENTER
            'doc.Add(paragraf)
            paragraf = New Paragraph(NombreEmpresa, _standardFont)
            paragraf.Alignment = Element.ALIGN_CENTER
            doc.Add(paragraf)
            paragraf = New Paragraph(NitEmpresa, _standardFont)
            paragraf.Alignment = Element.ALIGN_CENTER
            doc.Add(paragraf)
            paragraf = New Paragraph("Se emite certificado de decisión del documento compartido con fecha " & fecha & ", como constancia de  " & stru_compartido_general.DESCRIPCION_ESTADO_APROBACION, _standardFont)
            paragraf.Alignment = Element.ALIGN_LEFT
            doc.Add(paragraf)
            doc.Add(Chunk.NEWLINE)
            Dim tblrdatos As PdfPTable = New PdfPTable(2)
            tblrdatos.WidthPercentage = 100
            Dim cltitle_ident_transac As PdfPCell = New PdfPCell(New Phrase("Identifiación transacción ", _standardFont_datos_unidad_conservacion))
            Dim cltival_ident_transac As PdfPCell = New PdfPCell(New Phrase(stru_compartido_general.ID_RA_CD_DOCUMENTOS_COMPARTIDOS, _standardFont_datos_unidad_conservacion))
            Dim cltitle_tipo_transac As PdfPCell = New PdfPCell(New Phrase("Tipo compartido ", _standardFont_datos_unidad_conservacion))
            Dim cltival_tipo_transac As PdfPCell = New PdfPCell(New Phrase(stru_compartido_general.DESCRIPCION_TIPO_COMPARTIDO, _standardFont_datos_unidad_conservacion))
            Dim cltitle_fecha_transac As PdfPCell = New PdfPCell(New Phrase("Fecha registro solicitud ", _standardFont_datos_unidad_conservacion))
            Dim cltival_fecha_transac As PdfPCell = New PdfPCell(New Phrase(stru_compartido_general.FECHA_REGISTRO_SOLICITUD, _standardFont_datos_unidad_conservacion))
            Dim cltitle_fecha_apro_transac As PdfPCell = New PdfPCell(New Phrase("Fecha respuesta solicitud ", _standardFont_datos_unidad_conservacion))
            Dim cltival_fecha_apro_transac As PdfPCell = New PdfPCell(New Phrase(stru_compartido_general.FECHA_REGISTRO_APROBACION, _standardFont_datos_unidad_conservacion))
            Dim cltitle_estado_transac As PdfPCell = New PdfPCell(New Phrase("Estado ", _standardFont_datos_unidad_conservacion))
            Dim cltival_estado_transac As PdfPCell = New PdfPCell(New Phrase(stru_compartido_general.DESCRIPCION_ESTADO_APROBACION, _standardFont_datos_unidad_conservacion))
            Dim cltitle_asunto_transac As PdfPCell = New PdfPCell(New Phrase("Asunto ", _standardFont_datos_unidad_conservacion))
            Dim cltival_asunto_transac As PdfPCell = New PdfPCell(New Phrase(stru_compartido_general.ASUNTO_DOCUMENTO, _standardFont_datos_unidad_conservacion))
            Dim cltitle_nota_transac As PdfPCell = New PdfPCell(New Phrase("Nota ", _standardFont_datos_unidad_conservacion))
            Dim cltival_nota_transac As PdfPCell = New PdfPCell(New Phrase(stru_compartido_general.NOTA_SOLICITUD, _standardFont_datos_unidad_conservacion))
            Dim cltitle_solicitante_transac As PdfPCell = New PdfPCell(New Phrase("Solicitante ", _standardFont_datos_unidad_conservacion))
            Dim cltival_solicitante_transac As PdfPCell = New PdfPCell(New Phrase(NombreSolicitante & "  " & CargoSolicitante, _standardFont_datos_unidad_conservacion))
            Dim cltitle_documento_transac As PdfPCell = New PdfPCell(New Phrase("Documento relacionado ", _standardFont_datos_unidad_conservacion))
            Dim cltival_documento_transac As PdfPCell = New PdfPCell(New Phrase(docu_relacion, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitle_ident_transac)
            tblrdatos.AddCell(cltival_ident_transac)
            tblrdatos.AddCell(cltitle_tipo_transac)
            tblrdatos.AddCell(cltival_tipo_transac)
            tblrdatos.AddCell(cltitle_fecha_transac)
            tblrdatos.AddCell(cltival_fecha_transac)
            tblrdatos.AddCell(cltitle_fecha_apro_transac)
            tblrdatos.AddCell(cltival_fecha_apro_transac)
            tblrdatos.AddCell(cltitle_estado_transac)
            tblrdatos.AddCell(cltival_estado_transac)
            tblrdatos.AddCell(cltitle_asunto_transac)
            tblrdatos.AddCell(cltival_asunto_transac)
            tblrdatos.AddCell(cltitle_nota_transac)
            tblrdatos.AddCell(cltival_nota_transac)
            tblrdatos.AddCell(cltitle_solicitante_transac)
            tblrdatos.AddCell(cltival_solicitante_transac)
            tblrdatos.AddCell(cltitle_documento_transac)
            tblrdatos.AddCell(cltival_documento_transac)
            doc.Add(tblrdatos)
            doc.Add(Chunk.NEWLINE)
            paragraf = New Paragraph("USUARIOS RELACIONADOS A LA SOLICITUD  ", _standardFont)
            paragraf.Alignment = Element.ALIGN_CENTER
            doc.Add(paragraf)
            doc.Add(New Paragraph(3, vbCrLf))
            Dim tblrdatos_ As PdfPTable = New PdfPTable(6)
            tblrdatos_.WidthPercentage = 100
            Dim cltitle_ident_transac_user As PdfPCell = New PdfPCell(New Phrase("IDENTIFICACION ", _standardFont_datos_unidad_conservacion))
            Dim cltitle_nombre_transac_user As PdfPCell = New PdfPCell(New Phrase("NOMBRE", _standardFont_datos_unidad_conservacion))
            Dim cltitle_cargo_transac_user As PdfPCell = New PdfPCell(New Phrase("CARGO ", _standardFont_datos_unidad_conservacion))
            Dim cltitle_fecha_reg_transac_user As PdfPCell = New PdfPCell(New Phrase("FECHA REGISTRO", _standardFont_datos_unidad_conservacion))
            Dim cltitle_fecha_resp_transac_user As PdfPCell = New PdfPCell(New Phrase("FECHA RESPUESTA", _standardFont_datos_unidad_conservacion))
            Dim cltitle_estado_transac_user As PdfPCell = New PdfPCell(New Phrase("ESTADO", _standardFont_datos_unidad_conservacion))
            tblrdatos_.AddCell(cltitle_ident_transac_user)
            tblrdatos_.AddCell(cltitle_nombre_transac_user)
            tblrdatos_.AddCell(cltitle_cargo_transac_user)
            tblrdatos_.AddCell(cltitle_fecha_reg_transac_user)
            tblrdatos_.AddCell(cltitle_fecha_resp_transac_user)
            tblrdatos_.AddCell(cltitle_estado_transac_user)
            For i As Integer = 0 To DetalleRegistroUsuarioCompartido.Length - 1
                Dim cltvalue_ident_transac_user As PdfPCell = New PdfPCell(New Phrase(DetalleRegistroUsuarioCompartido(i).ID_USUARIOS_DOCUMENTOS_COMPARTIDOS, _standardFont_datos_unidad_conservacion))
                Dim cltvalue_nombre_transac_user As PdfPCell = New PdfPCell(New Phrase(DetalleRegistroUsuarioCompartido(i).nombre_usuario, _standardFont_datos_unidad_conservacion))
                Dim cltvalue_cargo_transac_user As PdfPCell = New PdfPCell(New Phrase(DetalleRegistroUsuarioCompartido(i).cargo_usuario, _standardFont_datos_unidad_conservacion))
                Dim cltvalue_fecha_reg_transac_user As PdfPCell = New PdfPCell(New Phrase(DetalleRegistroUsuarioCompartido(i).FECHA_REGISTRO_SOLICITUD, _standardFont_datos_unidad_conservacion))
                Dim cltvalue_fecha_resp_transac_user As PdfPCell = New PdfPCell(New Phrase(fecha, _standardFont_datos_unidad_conservacion))
                Dim cltvalue_estado_transac_user As PdfPCell = New PdfPCell(New Phrase(DetalleRegistroUsuarioCompartido(i).DESCRIPCION_ESTADO_RESPUESTA, _standardFont_datos_unidad_conservacion))
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
            SolicitaCertificadoDesicionDocumentoCompartido = "YES"
        Catch ex As Exception
            SolicitaCertificadoDesicionDocumentoCompartido = "Inconsistencia general función SolicitaCertificadoDesicionDocumentoCompartido " & ex.Message
        Finally
            doc.Close()
            If Not writer Is Nothing Then
                writer.Close()
            End If
        End Try
    End Function
    Function Genera_detalle_documento_compartido(ByVal id_documento_compartido As Long,
                                                 ByRef file_documento_compartido As String) As String

        Dim Result As String = ""
        Dim Ref_clas_docu_compartido As New Class_ra_Cd_Documentos_Compartidos
        Dim stru_compartido_general As STRU_DOCUMENTO_COMPARTIDO_GENERAL = Nothing
        Result = Ref_clas_docu_compartido.SolicitaEstructuraGeneraldocumentosCompartido(id_documento_compartido,
                                                                                        stru_compartido_general)
        If Result <> "YES" Then
            Genera_detalle_documento_compartido = Result
            Exit Function
        End If
        Dim Ref_clas_docu_compart_usuario As New Class_ra_cd_usuarios_documentos_compartidos
        Dim stru_compartido_usuario() As Detalle_registro_compartido_usuario = Nothing
        Result = Ref_clas_docu_compart_usuario.SolicitaRegistroDetalleUsuariosRelacionadosADocumentoCompartido(id_documento_compartido,
                                                                                                              stru_compartido_usuario)
        If Result <> "YES" Then
            Genera_detalle_documento_compartido = Result
            Exit Function
        End If
        Dim stru_docu_compartido() As stru_docu_compartido = Nothing
        Dim Ref_clas_docu_comp As New Class_ra_cd_documentos_gabinete_compartido
        Result = Ref_clas_docu_comp.SolicitaDatosEstructuraDocumentoCompartido(id_documento_compartido,
                                                                               stru_docu_compartido)
        If Result <> "YES" Then
            Genera_detalle_documento_compartido = Result
            Exit Function
        End If
        Dim ref_class_workflow As New ClassWorflowVisor
        Dim ClassDaGabinete As New ClassDaGabinete
        For i As Integer = 0 To stru_docu_compartido.Length - 1
            Dim matri_documentos() As String = Nothing
            Result = ClassDaGabinete.Solicita_matriz_documentos_almacenados_gabinete(stru_docu_compartido(i).ID_IMAGEN,
                                                                                     stru_docu_compartido(i).NOMBRE_GABINETE,
                                                                                     matri_documentos)
            If Result <> "YES" Then
                Genera_detalle_documento_compartido = Result
                Exit Function
            End If
            stru_docu_compartido(i).RUTA_DOCUMENTO = matri_documentos(1)
        Next
        Dim docu_relacion As String = ""
        For i2 As Integer = 0 To stru_docu_compartido.Length - 1
            docu_relacion = docu_relacion & "ID : " & stru_docu_compartido(i2).ID_IMAGEN & vbCrLf
            docu_relacion = docu_relacion & "GABINETE : " & stru_docu_compartido(i2).NOMBRE_GABINETE & vbCrLf
            docu_relacion = docu_relacion & "DOCUMENTO : " & stru_docu_compartido(i2).RUTA_DOCUMENTO & vbCrLf
        Next
        Dim nit_empresa As String = ""
        Dim nombre_empresa As String = ""
        Dim ref_class_empresa_gestion As New Class_empresa_gestion_documental
        Result = ref_class_empresa_gestion.Solicita_nombre_identificacion_empresa(nit_empresa,
                                                                                  nombre_empresa)
        If Result <> "YES" Then
            Genera_detalle_documento_compartido = Result
            Exit Function
        End If
        Dim Nombre_solicitante As String = ""
        Dim Cargo_solicitante As String = ""
        Dim ref_class_remit_dest_int As New Class_remit_dest_interno
        Result = ref_class_remit_dest_int.Retorna_datos_caracterizacion_usuario_gestion(stru_compartido_general.Remit_Dest_Interno_id_remit_dest_Int,
                                                                                      Nombre_solicitante,
                                                                                      Cargo_solicitante,
                                                                                      "")
        If Result <> "YES" Then
            Genera_detalle_documento_compartido = Result
            Exit Function
        End If
        Dim Ref_clas_gestion_fechas As New ClassGestionFechas
        Dim dat As Date = Now
        Dim fecha As String = ""
        Result = Ref_clas_gestion_fechas.Formatea_fecha_time_framework(dat,
                                                                       fecha)
        If Result <> "YES" Then
            Genera_detalle_documento_compartido = Result
            Exit Function
        End If
        Dim ruta_image As String = HttpContext.Current.Server.MapPath("../imagera/" & "logo_trd.png")
        If File.Exists(ruta_image) = False Then
            Genera_detalle_documento_compartido = "El sistema no registra el archivo de logo para integrar al detalle contacte a su administrador  " &
            ruta_image
            Exit Function
        End If
        Dim Rutatemp As String = HttpContext.Current.Session.Item("GA_RUTA_TEMP_GESTION") & "\"
        If Directory.Exists(Rutatemp) = False Then
            Directory.CreateDirectory(Rutatemp)
        End If
        file_documento_compartido = Rutatemp & "Certificado-" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & ".pdf"
        If File.Exists(file_documento_compartido) = True Then
            Kill(file_documento_compartido)
        End If
        Dim doc As Document
        Dim writer As PdfWriter = Nothing
        Try
            doc = New Document(PageSize.LETTER)
            doc.SetPageSize(PageSize.LETTER.Rotate())
            writer = PdfWriter.GetInstance(doc,
                               New FileStream(file_documento_compartido, FileMode.Create))
            writer.AddViewerPreference(PdfName.PICKTRAYBYPDFSIZE, PdfBoolean.PDFTRUE)
            doc.Open()
            Dim _standardFont As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA,
               12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
            Dim _standardFont_datos_unidad_conservacion As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA,
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
            paragraf = New Paragraph("DETALLE DOCUMENTO COMPARTIDO", _standardFont)
            paragraf.Alignment = Element.ALIGN_CENTER
            doc.Add(paragraf)
            paragraf = New Paragraph(nombre_empresa & " " & nit_empresa, _standardFont)
            paragraf.Alignment = Element.ALIGN_CENTER
            doc.Add(paragraf)
            paragraf = New Paragraph("Fecha emisión " & " " & fecha, _standardFont)
            paragraf.Alignment = Element.ALIGN_CENTER
            doc.Add(paragraf)
            doc.Add(Chunk.NEWLINE)
            Dim tblrdatos As PdfPTable = New PdfPTable(2)
            tblrdatos.WidthPercentage = 100
            Dim cltitle_ident_transac As PdfPCell = New PdfPCell(New Phrase("Identifiación transacción ", _standardFont_datos_unidad_conservacion))
            Dim cltival_ident_transac As PdfPCell = New PdfPCell(New Phrase(stru_compartido_general.ID_RA_CD_DOCUMENTOS_COMPARTIDOS, _standardFont_datos_unidad_conservacion))
            Dim cltitle_tipo_transac As PdfPCell = New PdfPCell(New Phrase("Tipo compartido ", _standardFont_datos_unidad_conservacion))
            Dim cltival_tipo_transac As PdfPCell = New PdfPCell(New Phrase(stru_compartido_general.DESCRIPCION_TIPO_COMPARTIDO, _standardFont_datos_unidad_conservacion))
            Dim cltitle_fecha_transac As PdfPCell = New PdfPCell(New Phrase("Fecha registro solicitud ", _standardFont_datos_unidad_conservacion))
            Dim cltival_fecha_transac As PdfPCell = New PdfPCell(New Phrase(stru_compartido_general.FECHA_REGISTRO_SOLICITUD, _standardFont_datos_unidad_conservacion))
            Dim cltitle_fecha_apro_transac As PdfPCell = New PdfPCell(New Phrase("Fecha respuesta solicitud ", _standardFont_datos_unidad_conservacion))
            Dim cltival_fecha_apro_transac As PdfPCell = New PdfPCell(New Phrase(stru_compartido_general.FECHA_REGISTRO_APROBACION, _standardFont_datos_unidad_conservacion))
            Dim cltitle_estado_transac As PdfPCell = New PdfPCell(New Phrase("Estado ", _standardFont_datos_unidad_conservacion))
            Dim cltival_estado_transac As PdfPCell = New PdfPCell(New Phrase(stru_compartido_general.DESCRIPCION_ESTADO_APROBACION, _standardFont_datos_unidad_conservacion))
            Dim cltitle_asunto_transac As PdfPCell = New PdfPCell(New Phrase("Asunto ", _standardFont_datos_unidad_conservacion))
            Dim cltival_asunto_transac As PdfPCell = New PdfPCell(New Phrase(stru_compartido_general.ASUNTO_DOCUMENTO, _standardFont_datos_unidad_conservacion))
            Dim cltitle_nota_transac As PdfPCell = New PdfPCell(New Phrase("Nota ", _standardFont_datos_unidad_conservacion))
            Dim cltival_nota_transac As PdfPCell = New PdfPCell(New Phrase(stru_compartido_general.NOTA_SOLICITUD, _standardFont_datos_unidad_conservacion))
            Dim cltitle_solicitante_transac As PdfPCell = New PdfPCell(New Phrase("Solicitante ", _standardFont_datos_unidad_conservacion))
            Dim cltival_solicitante_transac As PdfPCell = New PdfPCell(New Phrase(Nombre_solicitante & "  " & Cargo_solicitante, _standardFont_datos_unidad_conservacion))
            Dim cltitle_documento_transac As PdfPCell = New PdfPCell(New Phrase("Documento relacionado ", _standardFont_datos_unidad_conservacion))
            Dim cltival_documento_transac As PdfPCell = New PdfPCell(New Phrase(docu_relacion, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitle_ident_transac)
            tblrdatos.AddCell(cltival_ident_transac)
            tblrdatos.AddCell(cltitle_tipo_transac)
            tblrdatos.AddCell(cltival_tipo_transac)
            tblrdatos.AddCell(cltitle_fecha_transac)
            tblrdatos.AddCell(cltival_fecha_transac)
            tblrdatos.AddCell(cltitle_fecha_apro_transac)
            tblrdatos.AddCell(cltival_fecha_apro_transac)
            tblrdatos.AddCell(cltitle_estado_transac)
            tblrdatos.AddCell(cltival_estado_transac)
            tblrdatos.AddCell(cltitle_asunto_transac)
            tblrdatos.AddCell(cltival_asunto_transac)
            tblrdatos.AddCell(cltitle_nota_transac)
            tblrdatos.AddCell(cltival_nota_transac)
            tblrdatos.AddCell(cltitle_solicitante_transac)
            tblrdatos.AddCell(cltival_solicitante_transac)
            tblrdatos.AddCell(cltitle_documento_transac)
            tblrdatos.AddCell(cltival_documento_transac)
            doc.Add(tblrdatos)
            doc.Add(Chunk.NEWLINE)
            paragraf = New Paragraph("USUARIOS RELACIONADOS A LA SOLICITUD   ", _standardFont)
            paragraf.Alignment = Element.ALIGN_CENTER
            doc.Add(paragraf)
            doc.Add(New Paragraph(3, vbCrLf))
            Dim tblrdatos_ As PdfPTable = New PdfPTable(6)
            tblrdatos_.WidthPercentage = 100
            Dim cltitle_ident_transac_user As PdfPCell = New PdfPCell(New Phrase("IDENTIFICACION ", _standardFont_datos_unidad_conservacion))
            Dim cltitle_nombre_transac_user As PdfPCell = New PdfPCell(New Phrase("NOMBRE", _standardFont_datos_unidad_conservacion))
            Dim cltitle_cargo_transac_user As PdfPCell = New PdfPCell(New Phrase("CARGO ", _standardFont_datos_unidad_conservacion))
            Dim cltitle_fecha_reg_transac_user As PdfPCell = New PdfPCell(New Phrase("FECHA REGISTRO", _standardFont_datos_unidad_conservacion))
            Dim cltitle_fecha_resp_transac_user As PdfPCell = New PdfPCell(New Phrase("FECHA RESPUESTA", _standardFont_datos_unidad_conservacion))
            Dim cltitle_estado_transac_user As PdfPCell = New PdfPCell(New Phrase("ESTADO", _standardFont_datos_unidad_conservacion))
            tblrdatos_.AddCell(cltitle_ident_transac_user)
            tblrdatos_.AddCell(cltitle_nombre_transac_user)
            tblrdatos_.AddCell(cltitle_cargo_transac_user)
            tblrdatos_.AddCell(cltitle_fecha_reg_transac_user)
            tblrdatos_.AddCell(cltitle_fecha_resp_transac_user)
            tblrdatos_.AddCell(cltitle_estado_transac_user)
            For i As Integer = 0 To stru_compartido_usuario.Length - 1
                Dim cltvalue_ident_transac_user As PdfPCell = New PdfPCell(New Phrase(stru_compartido_usuario(i).ID_USUARIOS_DOCUMENTOS_COMPARTIDOS, _standardFont_datos_unidad_conservacion))
                Dim cltvalue_nombre_transac_user As PdfPCell = New PdfPCell(New Phrase(stru_compartido_usuario(i).nombre_usuario, _standardFont_datos_unidad_conservacion))
                Dim cltvalue_cargo_transac_user As PdfPCell = New PdfPCell(New Phrase(stru_compartido_usuario(i).cargo_usuario, _standardFont_datos_unidad_conservacion))
                Dim cltvalue_fecha_reg_transac_user As PdfPCell = New PdfPCell(New Phrase(stru_compartido_usuario(i).FECHA_REGISTRO_SOLICITUD, _standardFont_datos_unidad_conservacion))
                Dim cltvalue_fecha_resp_transac_user As PdfPCell = New PdfPCell(New Phrase(stru_compartido_usuario(i).FECHA_RESPUESTA_SOLICITUD, _standardFont_datos_unidad_conservacion))
                Dim cltvalue_estado_transac_user As PdfPCell = New PdfPCell(New Phrase(stru_compartido_usuario(i).DESCRIPCION_ESTADO_RESPUESTA, _standardFont_datos_unidad_conservacion))
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
            Genera_detalle_documento_compartido = "YES"
        Catch ex As Exception
            Genera_detalle_documento_compartido = "Inconsistencia general función genera_detalle_documento_compartido " & ex.Message
        Finally
            doc.Close()
            If Not writer Is Nothing Then
                writer.Close()
            End If
        End Try
    End Function
End Class
