Imports System.IO

Public Class WebFormRemisionCorrespondencia
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.IsPostBack = False Then
            Dim Refcls As New ClassRadicador
            Dim Refclas_consulta As New ClassRaConsultaRadicados
            Dim Result As String = ""
            Dim id_empresa As Integer = 0
            Result = Refcls.Retorna_ID_Empresa_Usuario_Radicador(Session.Item("RA_ID_USUARIO"), id_empresa)
            If Result <> "YES" Then
                Me.Label5.Text = Result
                Exit Sub
            End If
            Result = Refclas_consulta.Lista_plantilla_radicado_combo(id_empresa, Me.DropDownList_plantilla)
            If Result <> "YES" Then
                Me.Label5.Text = Result
                Exit Sub
            End If
            Result = Refcls.Retorna_Areas_Departamento_Radicacion(id_empresa, _
                                                                  Me.DropDownList_area)
            If Result <> "YES" Then
                Me.Label5.Text = Result
                Exit Sub
            End If
            Dim matr() As String = {"00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10" _
            , "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24"}
            For i As Integer = 0 To matr.Length - 1
                Me.DropDownList_hora_ini.Items.Add(matr(i))
                Me.DropDownList_hora_fin.Items.Add(matr(i))
            Next
            Me.update_general.Update()
            '**********************************************
            'Crea directori temporal workflow
            '**********************************************
            Dim Refclas As New ClassInicioRadicador
            Result = Refclas.Crea_Dir_Temporal_ra()
            If Result <> "YES" Then
                Me.Label5.Text = Result
            End If
        End If
    End Sub

    Protected Sub DropDownList_area_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_area.SelectedIndexChanged
        Dim id_empresa As Integer = -1
        Dim refclas As New ClassRadicador
        Dim clasjava As New Classscrripjava
        Dim result As String = ""
        Try
            If Me.DropDownList_area.Text = "SELECCIONE" Then
                Me.DropDownList_dest_tremit.Items.Clear()
                Me.update_general.Update()
                Exit Sub
            End If
            result = refclas.Retorna_ID_Empresa_Usuario_Radicador(HttpContext.Current.Session.Item("RA_ID_USUARIO"), id_empresa)
            If result <> "YES" Then
                clasjava.Showscripman(result, Me.update_general)
                Me.DropDownList_area.Focus()
                Exit Sub
            Else
                Dim id_organigrama As Integer = -1
                Dim ref_clas_empresa As New Class_registro_organigrama
                result = ref_clas_empresa.Retorna_Id_Organigrama_activo_empresa(id_empresa, _
                                                                                id_organigrama)
                If result <> "YES" Then
                    clasjava.Showscripman(result, Me.update_general)
                    Me.DropDownList_area.Focus()
                    Exit Sub
                Else
                    result = refclas.Lista_usuarios_gestion_internos_por_area(id_organigrama, Me.DropDownList_area.Text, Me.DropDownList_dest_tremit, id_empresa, Me.update_general, "")
                    If result <> "YES" Then
                        clasjava.Showscripman(result, Me.update_general)
                        Me.DropDownList_area.Focus()
                        Exit Sub
                    End If
                    Me.update_general.Update()
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman("Inconsistencia general " & ex.Message, Me.update_general)
        End Try
    End Sub

    Private Sub Button_generar_Click(sender As Object, e As EventArgs) Handles Button_generar.Click
        Dim Result As String = ""
        Dim Refclas As New ClassRaConsultaRadicados
        Dim clasjava As New Classscrripjava
        Try
            If Me.DropDownList_plantilla.Text = "" Then
                clasjava.Showscripman_menu("Debe selecionar la plantilla ", Me.UpdatePanel_boton_generar, "ModalPopupExtender_mensaje_personalizado")
                Me.DropDownList_plantilla.Focus()
                Exit Sub
            End If
            If Me.DropDownList_area.Text = "" Then
                clasjava.Showscripman_menu("Debe selecionar el área ", Me.UpdatePanel_boton_generar, "ModalPopupExtender_mensaje_personalizado")
                Me.DropDownList_area.Focus()
                Exit Sub
            End If
            If Me.DropDownList_area.Text = "SELECCIONE" Then
                clasjava.Showscripman_menu("Debe selecionar el área ", Me.UpdatePanel_boton_generar, "ModalPopupExtender_mensaje_personalizado")
                Me.DropDownList_area.Focus()
                Exit Sub
            End If
            If Me.DropDownList_area.Text = "TODAS LAS AREAS" Then
                clasjava.Showscripman_menu("Debe selecionar el área ", Me.UpdatePanel_boton_generar, "ModalPopupExtender_mensaje_personalizado")
                Me.DropDownList_area.Focus()
                Exit Sub
            End If

            If Me.TextBox_fecha_ini.Text = "" Then
                clasjava.Showscripman_menu("Debe selecionarla fecha incial ", Me.UpdatePanel_boton_generar, "ModalPopupExtender_mensaje_personalizado")
                Me.TextBox_fecha_ini.Focus()
                Exit Sub
            End If
            If Me.TextBox_fin.Text = "" Then
                clasjava.Showscripman_menu("Debe selecionarla fecha final ", Me.UpdatePanel_boton_generar, "ModalPopupExtender_mensaje_personalizado")
                Me.TextBox_fecha_ini.Focus()
                Exit Sub
            End If
            Dim archivo As String = ""
            Result = Refclas.Lista_remisiones_tramites_radicados(Me.DropDownList_plantilla.Text, _
                                                                 Me.DropDownList_area.Text, _
                                                                 Me.DropDownList_dest_tremit.Text, _
                                                                 Me.TextBox_fecha_ini.Text, _
                                                                 Me.TextBox_fin.Text, archivo, _
                                                                 Me.DropDownList_hora_ini.Text, _
                                                                 Me.DropDownList_hora_fin.Text)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_generar, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If archivo <> "" Then
                Dim fileinf As New FileInfo(archivo)
                If File.Exists(archivo) Then
                    Dim filecopia As String = HttpContext.Current.Session.Item("RA_RUTA_TEMPO_DESCARGA") & "\" & "file_temp" & fileinf.Extension
                    If File.Exists(filecopia) = True Then
                        File.Delete(filecopia)
                    End If
                    File.Copy(archivo, filecopia)
                    File.Delete(archivo)
                    If File.Exists(filecopia) = True Then
                        Hidden_ruta_archivo.Value = "../Temp_Radicacion/" & HttpContext.Current.Session.Item("RA_ID_USUARIO").ToString & "/DESCARGA/" & "file_temp" & fileinf.Extension
                        ifmExcel_.Attributes.Add("src", "..\radicador\WebFormDescargaRadicado.aspx")
                        updatapanel_iframe.Update()
                    End If
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu("Inconsistencia general " & ex.Message, Me.UpdatePanel_boton_generar, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
End Class