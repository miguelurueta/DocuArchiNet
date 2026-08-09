Public Class WebFormDaExportArchivo
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim clasjava As New Classscrripjava
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        If Me.IsPostBack = False Then
            Dim spliter() As String = Me.Session.Item("RA_RUTA_IMPRESION_FINAL").ToString.Split(",")
            Dim archivo_extenxion As String = ""
            If Not spliter Is Nothing Then
                archivo_extenxion = spliter(0)
            Else
                archivo_extenxion = Me.Session.Item("RA_RUTA_IMPRESION_FINAL").ToString
            End If
            Dim fileinf As New IO.FileInfo(archivo_extenxion)
            If fileinf.Extension <> ".TIF" And fileinf.Extension <> ".BMP" And fileinf.Extension <> ".JPG" Then
                Me.Table1v.Visible = False
                Me.Panel_stamp_pdf.Visible = True
            Else
                Me.Panel_stamp_pdf.Visible = False
                Me.Table1v.Visible = True
            End If
        End If
    End Sub

    Private Sub Button_exportar_Click(sender As Object, e As EventArgs) Handles Button_exportar.Click
        Dim Result As String = ""
        Dim refjava As New Classscrripjava
        Dim refclasgabinete As New ClassDaGabinete
        Try
            Me.sid2.Value = ""
            Me.Hidden_respuesta.Value = "NO"
            If Me.Session.Item("RA_RUTA_IMPRESION_FINAL") = "OJO" Or Me.Session.Item("RA_RUTA_IMPRESION_FINAL") = "" Then
                refjava.Showscripman_menu("Imposible imprimir matriz vacia, es posible que el reguest este bloqueado", Me.updatepanel_imprimir, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim spliter() As String = Me.Session.Item("RA_RUTA_IMPRESION_FINAL").ToString.Split(",")
            Dim archivo_extenxion As String = ""
            If Not spliter Is Nothing Then
                archivo_extenxion = spliter(0)
            Else
                archivo_extenxion = Me.Session.Item("RA_RUTA_IMPRESION_FINAL").ToString
            End If
            Dim fileinf As New IO.FileInfo(archivo_extenxion)
            Me.hideextension.Value = UCase(fileinf.Extension)
            If Me.RadioButton_todo.Checked = True Then
                Me.sid2.Value = Session.Item("RA_RUTA_IMPRESION_FINAL")
                Me.Hidden_respuesta.Value = "YES"
            End If
            If Me.RadioButton_rango.Checked = True Then
                Dim splits() As String = Session.Item("RA_RUTA_IMPRESION_FINAL").ToString.Split(",")
                Dim matri_selec() As String
                Erase matri_selec
                If Not splits Is Nothing Then
                    If splits.Length > 1 Then
                        Result = refclasgabinete.Generando_Matriz_Imagenes_Sleccionadas(splits,
                                                                                        Me.TextBox_ini.Text,
                                                                                        matri_selec)
                        If Result <> "YES" Then
                            refjava.Showscripman_menu(Result, Me.updatepanel_imprimir, "ModalPopupExtender_mensaje_personalizado")
                            Me.Hidden_respuesta.Value = "NO"
                            Exit Sub
                        Else
                            If Not matri_selec Is Nothing Then
                                Me.sid2.Value = ""
                                For i As Integer = 0 To matri_selec.Length - 1
                                    If Me.sid2.Value = "" Then
                                        Me.sid2.Value = matri_selec(i)
                                    Else
                                        Me.sid2.Value = Me.sid2.Value & "," & matri_selec(i)
                                    End If
                                Next
                                Me.Hidden_respuesta.Value = "YES"
                            Else
                                refjava.Showscripman_menu("La selección no cumple los criterios, imposible imprimir", Me.updatepanel_imprimir, "ModalPopupExtender_mensaje_personalizado")
                                Me.Hidden_respuesta.Value = "NO"
                                Exit Sub
                            End If
                        End If
                    Else
                        Me.sid2.Value = Session.Item("RA_RUTA_IMPRESION_FINAL")
                        Me.Hidden_respuesta.Value = "YES"
                    End If

                End If
            End If
            Dim datos_log As String = ""
            Result = refclasgabinete.Retorna_Datos_Auditoria_Gabinete(Session.Item("DA_ID_IMAGEN_IMPRESION"),
                                                                      Session.Item("DA_GABINETE_IMPRESION"),
                                                                      datos_log)
            If Result <> "YES" Then
                refjava.Showscripman_menu("Imposible encontrar datos log ", Me.updatepanel_imprimir, "ModalPopupExtender_mensaje_personalizado")
                Me.Hidden_respuesta.Value = "NO"
                Exit Sub
            End If
            Dim selecion As String = ""
            If Me.RadioButton_todo.Checked = True Then
                selecion = "Todas las páginas (" & spliter.Length.ToString & ")"
            End If
           
            If Me.RadioButton_rango.Checked = True Then
                selecion = "Rango de selección " & Me.TextBox_ini.Text
            End If
            Result = refclasgabinete.Registra_Auditoria_Eventos(Session.Item("DA_GABINETE_IMPRESION"), selecion & " Imagen Principal " & spliter(0), Session.Item("DA_ID_IMAGEN_IMPRESION"), datos_log, "Guardar")
            If Result <> "YES" Then
                refjava.Showscripman_menu("Imposible encontrar datos log ", Me.updatepanel_imprimir, "ModalPopupExtender_mensaje_personalizado")
                Me.Hidden_respuesta.Value = "NO"
                Exit Sub
            End If
            If Me.RadioButton_docuarchi.Checked = True 
                Hidden_ruta_archivo.Value = Me.sid2.Value
            End If
            '------------------------------------------------
            'Fromatea en pdf
            '------------------------------------------------
            If Me.RadioButton_pdf.Checked = True Then
                Dim fileinff As New IO.FileInfo(spliter(0))
                If fileinff.Extension <> ".TIF" And fileinff.Extension <> ".BMP" And fileinff.Extension <> "JPG" Then
                    refjava.Showscripman_menu("El tipo de formato no se puede convertir en pdf ", Me.updatepanel_imprimir, "ModalPopupExtender_mensaje_personalizado")
                    Me.Hidden_respuesta.Value = "NO"
                    Exit Sub
                End If
                Dim option_marcar As Integer = 0
                If Me.CheckBoxEporta.Checked = True Then
                    option_marcar = 1
                End If
                Dim Refclas_report As New ClassReportesGestor
                Dim matri_archivo() As String
                Dim file_out() As String
                Erase file_out
                ReDim Preserve matri_archivo(0)
                Dim matri_envio() As String = Me.sid2.Value.ToString.Split(",")
                matri_archivo(0) = spliter(0)
                If HttpContext.Current.Session.Item("DA_Login_Usuario") = "CONSULTAPUBLICO" Then
                    option_marcar = 1
                End If
                Dim Class_ItexShare As New Class_ItexShare
                Result = Class_ItexShare.stamp_pdf_seguridad(matri_archivo,
                                                             file_out,
                                                             option_marcar,
                                                             matri_envio)
                If Result <> "YES" Then
                    refjava.Showscripman(Result.Replace("/", "-"), updatepanel_imprimir)
                    Exit Sub
                End If
                For i3 As Integer = 0 To file_out.Length - 1
                    If i3 = 0 Then
                        Me.sid2.Value = file_out(i3)
                    Else
                        Me.sid2.Value = Me.sid2.Value & "," & file_out(i3)
                    End If
                Next
                Hidden_ruta_archivo.Value = Me.sid2.Value
            End If
            'Estampa archivos pdf con image
            If CheckBox_stmp_pdf.Checked = True Then
                Dim option_marcar As Integer = 1
                If HttpContext.Current.Session.Item("DA_Login_Usuario") = "CONSULTAPUBLICO" Then
                    option_marcar = 1
                End If
                Dim Class_ItexShare As New Class_ItexShare
                Result = Class_ItexShare.Stamp_image_pdf_seguridad(archivo_extenxion,
                                                                   Me.sid2.Value,
                                                                   option_marcar)
                If Result <> "YES" Then
                    refjava.Showscripman(Result.Replace("/", "-"), updatepanel_imprimir)
                    Exit Sub
                End If
                Hidden_ruta_archivo.Value = Me.sid2.Value
            End If
            Me.Hidden_respuesta.Value = "YES"
            ifmExcel_.Attributes.Add("src", "..\Docuarchi\WebFormDaDescarga.aspx")
            updatapanel_iframe.Update()
        Catch ex As Exception
            refjava.Showscripman(ex.Message, updatepanel_imprimir)
            Me.Hidden_respuesta.Value = "NO"
        End Try
    End Sub
End Class