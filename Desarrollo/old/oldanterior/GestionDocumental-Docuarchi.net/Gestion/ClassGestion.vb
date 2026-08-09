Imports MySql.Data.MySqlClient
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports System.IO
Imports AjaxControlToolkit
Public Class ClassGestion
    
    Function Descarga_documento_plantilla_respuesta(ByRef pag As Page, _
                                                    ByVal ruta_fisica As String, _
                                                    ByVal ruta_virtual As String, _
                                                    ByVal id_respuesta As Integer, _
                                                    ByRef conten As Object, _
                                                    ByVal radicado As String) As String
        Try
            Dim Result As String = ""
            Dim Matri_pie() As String = Nothing
            Dim Refclas_remit_dest_interno As New Class_remit_dest_interno
            Result = Refclas_remit_dest_interno.solicita_datos_respuesta_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                         Matri_pie)
            If Result <> "YES" Then
                Descarga_documento_plantilla_respuesta = Result
                Exit Function
            End If
            If HttpContext.Current.Session.Item("WF_RUTA_FIRMA_FINAL") = "" Then
                Descarga_documento_plantilla_respuesta = "Imposible encontrar firma documento"
                Exit Function
            End If
            Dim docx As Object = Nothing
            Dim content As Object = Nothing
            Dim firma As Object = Nothing
            Dim document_plantilla As Object = Nothing
            Dim rut_firma As String = HttpContext.Current.Server.MapPath(HttpContext.Current.Session.Item("WF_RUTA_FIRMA_FINAL"))
            Result = ReadFile(rut_firma, firma)
            If Result <> "YES" Then
                Descarga_documento_plantilla_respuesta = Result
                Exit Function
            End If
            'Dim OB As New localhost.Service
            'OB.Url = HttpContext.Current.Session.Item("URL_WEB_SERVICE")
            'Dim datos_gestion() As Object = Nothing
            'Result = OB.Crea_Documento_plantilla_web(Matri_pie, _
            '                                         datos_gestion, _
            '                                         radicado, id_respuesta, _
            '                                         HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
            '                                         firma, _
            '                                         document_plantilla, _
            '                                         docx, _
            '                                         ".bmp")
            'If Result <> "YES" Then
            '    Descarga_documento_plantilla_respuesta = Result
            '    Exit Function
            'End If
            conten = docx
            Descarga_documento_plantilla_respuesta = "YES"
            Exit Function
        Catch ex As Exception
            Descarga_documento_plantilla_respuesta = "Inconsistencia general función Descarga_documento_plantilla_respuesta " & ex.Message
        End Try
    End Function
    Function Descarga_documento_plantilla_respuesta_radicado(ByVal ruta_fisica As String, _
                                                             ByVal ruta_virtual As String, _
                                                             ByVal id_respuesta As Integer, _
                                                             ByRef conten As Object, _
                                                             ByVal radicado As String,
                                                             ByVal radicado_respuesta As String, _
                                                             ByVal id_usuario_gestion_firma As Integer) As String
        Try
            Dim Result As String = ""
            Dim Matri_pie() As String = Nothing
            Dim Refclas_remit_dest_interno As New Class_remit_dest_interno
            Result = Refclas_remit_dest_interno.solicita_datos_respuesta_usuario_gestion(id_usuario_gestion_firma, _
                                                                                         Matri_pie)
            If Result <> "YES" Then
                Descarga_documento_plantilla_respuesta_radicado = Result
                Exit Function
            End If
            Dim docx As Object = Nothing
            Dim datos_gestion() As Object = Nothing
            Dim ref_class_gembox As New ClassGaGembox
            Dim Ruta_tempo_directorio As String = HttpContext.Current.Server.MapPath("../repositorio/plantillalibre_web.docx")
            Result = ref_class_gembox.Solicita_Documento_plantilla_radicado(Matri_pie, _
                                                                            radicado, _
                                                                            id_respuesta, _
                                                                            HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                            Ruta_tempo_directorio, _
                                                                            docx, _
                                                                            HttpContext.Current.Session("WF_RUTA_FIRMA_FINAL"), _
                                                                            radicado_respuesta, _
                                                                            HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                            id_usuario_gestion_firma)
            If Result <> "YES" Then
                Descarga_documento_plantilla_respuesta_radicado = Result
                Exit Function
            End If
            conten = docx
            Descarga_documento_plantilla_respuesta_radicado = "YES"
            Exit Function
        Catch ex As Exception
            Descarga_documento_plantilla_respuesta_radicado = "Inconsistencia general función Descarga_documento_plantilla_respuesta_radicado " & ex.Message
        End Try
    End Function

    Function ReadFile(ByVal FilePath1 As String, ByRef Filebyte1 As Byte()) As String
        Dim fs As FileStream = Nothing
        Try
            ' Read file and return contents
            If File.Exists(FilePath1) = True Then
                File.SetAttributes(FilePath1, FileAttributes.Normal)
            Else
                Return "imposible encontrar archivo temporal " & FilePath1
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
    Public Function SaveFile(ByVal Name As String, ByRef Content As Byte()) As String
        Dim objFstream As FileStream = Nothing
        Try
            objFstream = File.Open(Name, FileMode.Create, FileAccess.Write)
            Dim lngLen As Long = Content.Length
            objFstream.Write(Content, 0, CInt(lngLen))
            objFstream.Flush()
            objFstream.Close()
            Return "YES"
        Catch exp As Exception
            SaveFile = "Funcion Save file Exception: " & exp.Message

        Finally
            objFstream.Close()
        End Try
    End Function
    
End Class
