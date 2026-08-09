Imports System.IO

Public Class Classdescargapublico
    Function Descarga_documento_respuesta_pdf_public(ByVal id_respuesta As Integer,
                                                     ByRef ifmExcel As Object,
                                                     ByRef updatapanel_iframe As UpdatePanel,
                                                     ByRef Hidden_ruta_archivo As Object) As String
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassRaEnvioCorrespondencia
            Dim Refclasvisual As New ClassVisualisaDocumento
            Dim Refclasgestion As New Classgestionrespuesta
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Dim estru As stru_envio = Nothing
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta,
                                                                                        estru)
            If Result <> "YES" Then
                Descarga_documento_respuesta_pdf_public = Result
                Exit Function
            End If
            If estru.FECHA_RESPUETA = "" Then
                Descarga_documento_respuesta_pdf_public = "No hay una respuesta para el documento "
                Exit Function
            End If
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim Gabinete As String = ""
            Result = Class_tipo_doc_entrante.Solicita_nombre_gabinete_tramite(estru.TRAMITE_DOCUMENTO,
                                                                              estru.system_plantilla_radicado_id_plantilla,
                                                                              Gabinete)
            If Result <> "YES" Then
                Descarga_documento_respuesta_pdf_public = Result
                Exit Function
            End If
            Dim Matri_documentos() As String
            Erase Matri_documentos
            Result = Refclasvisual.Genera_Matris_Documentos_Almacenados(estru.ID_IMAGEN_RESPUESTA,
                                                                        Gabinete,
                                                                        Matri_documentos)

            If Result <> "YES" Then
                Descarga_documento_respuesta_pdf_public = Result
                Exit Function
            End If
            If Matri_documentos.Length >= 2 Then
                If HttpContext.Current.Session.Item("ACTIVA_WEB_SERVICE") = 0 Then
                    Descarga_documento_respuesta_pdf_public = "Por favor active web service para radicación"
                    Exit Function
                End If
                If HttpContext.Current.Session.Item("URL_WEB_SERVICE") = "" Then
                    Descarga_documento_respuesta_pdf_public = "Por favor informe la url web service para radicación"
                    Exit Function
                End If
                Dim ruta_local As String = HttpContext.Current.Server.MapPath("../Temp_Image/PUBLIC/DESCARGA/")
                If Directory.Exists(ruta_local) = False Then
                    Directory.CreateDirectory(ruta_local)
                End If
                Dim fileinf As New FileInfo(Matri_documentos(1))
                If File.Exists(Matri_documentos(1)) Then
                    If fileinf.Extension = ".DOCX" Then
                        Dim filecopia As String = ruta_local & "temp_" & "Dr" & HttpContext.Current.Session.SessionID & fileinf.Extension
                        If File.Exists(filecopia) = True Then
                            File.Delete(filecopia)
                        End If
                        File.Copy(Matri_documentos(1), filecopia)
                        If File.Exists(filecopia) = True Then
                            'Dim OB As New localhost.Service
                            'OB.Url = HttpContext.Current.Session.Item("URL_WEB_SERVICE")
                            Dim Conten As Object = Nothing
                            Dim Document As Object = Nothing
                            Dim ruta_final As String = filecopia
                            Hidden_ruta_archivo.Value = "../Temp_Image/PUBLIC/DESCARGA/" & "temp_" & "Dr" & HttpContext.Current.Session.SessionID & fileinf.Extension
                            ifmExcel.Attributes.Add("src", "..\radicador\WebFormDescargaRadicado.aspx")
                            updatapanel_iframe.Update()
                            Descarga_documento_respuesta_pdf_public = "YES"
                        End If
                    End If
                    If fileinf.Extension = ".PDF" Or fileinf.Extension = ".TIF" Then
                        Dim filecopia As String = ruta_local & "temp_" & "Dr" & HttpContext.Current.Session.SessionID & fileinf.Extension
                        If File.Exists(filecopia) = True Then
                            File.Delete(filecopia)
                        End If
                        File.Copy(Matri_documentos(1), filecopia)
                        If File.Exists(filecopia) = True Then
                            Hidden_ruta_archivo.Value = "../Temp_Image/PUBLIC/DESCARGA/" & "temp_" & "Dr" & HttpContext.Current.Session.SessionID & fileinf.Extension
                            ifmExcel.Attributes.Add("src", "..\radicador\WebFormDescargaRadicado.aspx")
                            updatapanel_iframe.Update()
                            Descarga_documento_respuesta_pdf_public = "YES"
                        End If
                    End If
                End If
            Else
                Descarga_documento_respuesta_pdf_public = "YES"
            End If
            Descarga_documento_respuesta_pdf_public = "YES"
        Catch ex As Exception
            Descarga_documento_respuesta_pdf_public = "Inconsistencia función Descarga_documento_respuesta_pdf_public " & ex.Message
        End Try
    End Function
End Class
