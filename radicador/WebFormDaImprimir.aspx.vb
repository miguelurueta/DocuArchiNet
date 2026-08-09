Public Class WebFormDaImprimir
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        Try
            If Me.Session.Item("RA_RUTA_IMPRESION_FINAL") <> "OJO" Then
                'Dim SES As String = Me.Session.Item("RA_RUTA_IMPRESION_FINAL")
                'Me.sid2.Value = SES
                'Dim p = 1
            End If
            '***************************
            '0-codigo destinatario
            '1-codigo remitente
            '2-id usuario
            '3-codigo plantilla
            '4-Consecutivo Radicado
            '5-Consecutivo codigo barra
            '***************************
            'Dim splitdatos() As String
            'Erase splitdatos
            'splitdatos = HttpContext.Current.Session("RA_DATO_IMPRESION").ToString.Split("¬")
            'If Not splitdatos Is Nothing Then
            '    Me.usuario_datos.InnerText = splitdatos(2)
            '    Me.radicad_datos.InnerText = splitdatos(4)
            '    Me.barr_datos.InnerText = splitdatos(5)
            '    Me.fech_datos.InnerText = splitdatos(6)
            'End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Button_imprimir_Click(sender As Object, e As EventArgs) Handles Button_imprimir.Click
        Dim Result As String = ""
        Dim refjava As New Classscrripjava
        Dim refclasgabinete As New ClassRaImpresion
        Try
            Result = refclasgabinete.Imprimir_da_documentos_visor(Me.Page)
            If Result <> "YES" Then
                refjava.Showscripman(Result, Me.updatepanel_imprimir)
                Me.Hidden_respuesta.Value = "NO"
                Exit Sub
            End If
            'Me.sid2.Value = ""
            'Me.Hidden_respuesta.Value = "NO"
            'If Me.Session.Item("RA_RUTA_IMPRESION_FINAL") = "OJO" Or Me.Session.Item("RA_RUTA_IMPRESION_FINAL") = "" Then
            '    refjava.Showscripman("Imposible imprimir matriz vacia, es posible que el reguest este bloqueado", Me.updatepanel_imprimir)
            '    Exit Sub
            'End If
            'Dim spliter() As String = Me.Session.Item("RA_RUTA_IMPRESION_FINAL").ToString.Split(",")
            'Dim archivo_extenxion As String = ""
            'If Not spliter Is Nothing Then
            '    archivo_extenxion = spliter(0)
            'Else
            '    archivo_extenxion = Me.Session.Item("RA_RUTA_IMPRESION_FINAL").ToString
            'End If
            'Dim fileinf As New IO.FileInfo(archivo_extenxion)
            'Me.hideextension.Value = UCase(fileinf.Extension)
            'If Me.RadioButton_seleccion.Checked = True Then
            '    If Session.Item("RA_RUTA_IMPRESION_FINAL_DOC_ACTUAL") = "" Then
            '        refjava.Showscripman("Imposible imprimir documento actual vacio, es posible que el reguest este bloqueado", Me.updatepanel_imprimir)
            '        Exit Sub
            '    End If
            '    Me.sid2.Value = Session.Item("RA_RUTA_IMPRESION_FINAL_DOC_ACTUAL")
            '    Me.Hidden_respuesta.Value = "YES"
            'End If
            'If Me.RadioButton_todo.Checked = True Then
            '    Me.sid2.Value = Session.Item("RA_RUTA_IMPRESION_FINAL")
            '    Me.Hidden_respuesta.Value = "YES"
            'End If
            'If Me.RadioButton_rango.Checked = True Then
            '    Dim splits() As String = Session.Item("RA_RUTA_IMPRESION_FINAL").ToString.Split(",")
            '    Dim matri_selec() As String
            '    Erase matri_selec
            '    If Not splits Is Nothing Then
            '        If splits.Length > 1 Then
            '            Result = refclasgabinete.Generando_Matriz_Imagenes_Sleccionadas(splits, Me.TextBox_ini.Text, matri_selec)
            '            If Result <> "YES" Then
            '                refjava.Showscripman(Result, updatepanel_imprimir)
            '                Me.Hidden_respuesta.Value = "NO"
            '                Exit Sub
            '            Else
            '                If Not matri_selec Is Nothing Then
            '                    Me.sid2.Value = ""
            '                    For i As Integer = 0 To matri_selec.Length - 1
            '                        If Me.sid2.Value = "" Then
            '                            Me.sid2.Value = matri_selec(i)
            '                        Else
            '                            Me.sid2.Value = Me.sid2.Value & "," & matri_selec(i)
            '                        End If
            '                    Next
            '                    Me.Hidden_respuesta.Value = "YES"
            '                Else
            '                    refjava.Showscripman("La selección no cumple los criterios, imposible imprimir", updatepanel_imprimir)
            '                    Me.Hidden_respuesta.Value = "NO"
            '                    Exit Sub
            '                End If
            '            End If
            '        Else
            '            Me.sid2.Value = Session.Item("RA_RUTA_IMPRESION_FINAL")
            '            Me.Hidden_respuesta.Value = "YES"
            '        End If

            '    End If
            'End If
            'Dim datos_log As String = ""

            'Result = refclasgabinete.Retorna_Datos_Auditoria_Gabinete(Session.Item("DA_ID_IMAGEN_IMPRESION"), Session.Item("DA_GABINETE_IMPRESION"), datos_log)
            'If Result <> "YES" Then
            '    refjava.Showscripman("Imposible encontrar datos log " & Result, updatepanel_imprimir)
            '    Me.Hidden_respuesta.Value = "NO"
            '    Exit Sub
            'End If
            'Dim selecion As String = ""
            'If Me.RadioButton_todo.Checked = True Then
            '    selecion = "Todas las páginas (" & spliter.Length.ToString & ")"
            'End If
            'If Me.RadioButton_seleccion.Checked = True Then
            '    selecion = "Pagina actual (1)" & Me.sid2.Value
            'End If
            'If Me.RadioButton_rango.Checked = True Then
            '    selecion = "Rango de selección " & Me.TextBox_ini.Text
            'End If
            'Result = refclasgabinete.Registra_Auditoria_Eventos(Session.Item("DA_GABINETE_IMPRESION"), selecion & " Imagen Principal " & spliter(0), Session.Item("DA_ID_IMAGEN_IMPRESION"), datos_log, "Imprimir")
            'If Result <> "YES" Then
            '    refjava.Showscripman("Imposible registrar datos log " & Result, updatepanel_imprimir)
            '    Me.Hidden_respuesta.Value = "NO"
            '    Exit Sub
            'End If
            'Me.sid2.Value = Me.sid2.Value & "|" & UCase(fileinf.Extension)
        Catch ex As Exception
            refjava.Showscripman(ex.Message, updatepanel_imprimir)
            Me.Hidden_respuesta.Value = "NO"
        End Try
    End Sub
End Class