Imports System.IO
Imports GemBox.Document
Imports GemBox.Document.Tables
Imports System.Xml

Public Class WebFormEdicionWordHtml
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Me.IsPostBack = False Then
                Me.htmlEditor.config.toolbar = New Object() {
              New Object() {"Cut", "Copy", "Paste", "PasteText", "PasteFromWord", "-", "Print", "SpellChecker", "Scayt"},
              New Object() {"Undo", "Redo", "-", "Find", "Replace", "-", "SelectAll", "RemoveFormat"},
              "/",
              New Object() {"Bold", "Italic", "Underline", "Strike", "-", "Subscript", "Superscript"},
              New Object() {"NumberedList", "BulletedList", "-", "Outdent", "Indent"},
              New Object() {"JustifyLeft", "JustifyCenter", "JustifyRight", "JustifyBlock"},
              New Object() {"Link", "Unlink"},
              New Object() {"Image", "Table", "SpecialChar", "PageBreak"},
              "/",
              New Object() {"Styles", "Format", "Font", "FontSize"},
              New Object() {"TextColor", "BGColor"},
              New Object() {"Maximize", "ShowBlocks"}}
                ComponentInfo.SetLicense("DTFX-JTBY-6RJK-Y101")
                Dim Result As String = ""
                Dim Refclas As New Classgestionrespuesta
                Result = Refclas.Asigna_respuesta_inicial_chkeditor(Me.htmlEditor, Me.Page, Hidden_id_respuesta.Value)
                If Result <> "YES" Then
                    label_result.Text = Result
                    Me.htmlEditor.Visible = False
                End If
            End If
        Catch ex As Exception
            label_result.Text = ex.Message
        End Try
    End Sub
    Protected Sub OnExportButtonClicked(sender As Object, e As EventArgs) Handles exportButton.Click
        Dim Result As String = ""
        If HttpContext.Current.Session.Item("ACTIVA_WEB_SERVICE") = 0 Then
            label_result.Text = "Por favor informe la url web service para workflow"
            Exit Sub
        End If
        If Session.Item("URL_WEB_SERVICE") = "" Then
            label_result.Text = "Por favor informe la url web service para workflow"
            Exit Sub
        End If
        Dim Refclas As New ClassWorkflow
        Dim Radicado As String = ""
        Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
        Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"), _
                                                                             Radicado)
        If Result <> "YES" Then
            Exit Sub
        End If
        If Radicado = "" Then
            label_result.Text = "La tarea seleccionada no tiene radicado relacionado "
            Exit Sub
        End If

        Dim Class_ra_respuesta_radicado As New Class_ra_respuesta_radicado
        Dim id_respuesta As Integer = 0
        Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado(Radicado,
                                                                           HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                           id_respuesta)
        If Result <> "YES" Then
            label_result.Text = Result
            Exit Sub
        End If
        Dim refclas_gestion As New ClassGestion
        Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/"
        Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
        Dim conten As Object = Nothing
        'Dim OB As New localhost.Service
        'OB.Url = HttpContext.Current.Session.Item("URL_WEB_SERVICE")
        'Result = OB.Donwload_archivo_plantilla(conten)
        'If Result <> "YES" Then
        '    label_result.Text = Result
        '    Exit Sub
        'End If
        Dim Refclas_sav_file As New ClassRaEnvioCorrespondencia
        Result = Refclas_sav_file.SaveFile(ruta_fisica & "template_rut" & ".docx", conten)
        If Result <> "YES" Then
            label_result.Text = Result
            Exit Sub
        End If
        Dim document As DocumentModel
        Using stream = File.OpenRead(ruta_fisica & "template_rut" & ".docx")
            document = DocumentModel.Load(stream, LoadOptions.DocxDefault)
        End Using
        For Each picture As Picture In document.GetChildElements(True, ElementType.Picture)
            picture.Layout = Layout.Inline(picture.Layout.Size)
        Next
        Dim str As String = Me.htmlEditor.Text.ToString().Replace("<title></title>", "")
        Dim g = "<meta content=" & """" & "text/html; charset=utf-8" & """" & "http-equiv=" & """" & "content-type" & """" & "/>"
        str = str.Replace("<meta content=" & """" & "text/html; charset=utf-8" & """" & "http-equiv=" & """" & "content-type" & """" & "/>", "")
        'str = str.Replace("margin-left:-27pt;margin-right:0pt;margin-top:0pt;margin-bottom:0pt;padding: 1pt 4pt;mso-pagination:lines-together;" & """" & ">", "")
        'str = str.Replace("<span>&nbsp;</span></p>", "")
        '<p style="margin-left:-27pt;margin-right:0pt;margin-top:0pt;margin-bottom:0pt;padding: 1pt 4pt;mso-pagination:lines-together;">
        '<span>&nbsp;</span></p>  style="margin: 0pt;">
        str = str.Replace("margin: 0pt", "margin-left:0pt;margin-right:0pt;margin-top:0pt;margin-bottom:0pt")
        For Each item As ContentRange In document.Content.Find("Resp_conte_web")
            item.LoadText(str, LoadOptions.HtmlDefault)
        Next
        Dim fileName = Path.ChangeExtension("Document", Me.outputFormatList.SelectedValue)    
        document.Save(Me.Response, fileName)

    End Sub

    
End Class