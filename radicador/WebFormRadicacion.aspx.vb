Imports Neodynamic.SDK.Web

Public Class WebFormRadicacion
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.IsPostBack = False Then
            '**********************************************
            'Crea directori temporal workflow
            '**********************************************
            Dim Refclas As New ClassInicioRadicador
            Dim Result = Refclas.Crea_Dir_Temporal_ra()
            If Result <> "YES" Then
                HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "Directorio temporal : " & Result & vbCrLf
            Else
                HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "Directorio temporal : " & Result & vbCrLf
            End If
            'If HttpContext.Current.Session.Item("RA_TIPO_IMPRESION") = "2" Then
            '    Me.ifimpre.Attributes("SRC") = "../radicador/WebFormImprimir.aspx"
            'End If
            'If HttpContext.Current.Session.Item("RA_TIPO_IMPRESION") = "1" Then
            '    Me.ifimpre.Attributes("SRC") = "../radicador/WebFormImprimirfiles.aspx"
            'End If
        End If

        If Session.Item("RA_MODULO_SELECCIONADO") <> "" Then
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Dim Refclas As New ClassRadicador
            Dim Result As String = ""
            '------------------------------------------------------
            'Detecta la seleccion del modulo de radicacion
            '------------------------------------------------------
            If split(0) = "RADICACION" Then
                
                If split(2) = "RADICACION GUIA" Then
                    Result = Refclas.Genera_Interface_Radicacion_Guia(split(1), split(0), Me, split(4))
                    If Result <> "YES" Then

                    End If
                    If Me.IsPostBack = True Then
                        '-----------------------------------------------
                        'Asigna los valores del pais cuando se activa
                        'el evento postback carga los paises y seleccion
                        'el item seleccinado post evento
                        '-----------------------------------------------
                        Me.ComboBoxEditPaisDesExt.Items.Clear()
                        Dim ilist As New ListItem
                        ilist.Text = "SELECCIONE"
                        ilist.Value = "SELECCIONE"
                        Me.ComboBoxEditPaisDesExt.Items.Add(ilist)
                        ilist = New ListItem
                        ilist.Text = "COLOMBIA"
                        ilist.Value = "COLOMBIA"
                        '-------------------------------------------------
                        'Selecciona elemento post evento postback
                        '-------------------------------------------------
                        Me.ComboBoxEditPaisDesExt.Items.Add(ilist)
                        If Me.Hiddenselecionpais.Value <> "" Then
                            Me.ComboBoxEditPaisDesExt.SelectedIndex = 1
                        End If
                        Me.ComboBoxEditPaisDesExt.Text = Me.Hiddenselecionpais.Value
                        For i As Integer = 0 To Me.ComboBoxEditPaisDesExt.Items.Count - 1
                            If Me.ComboBoxEditPaisDesExt.Items(i).Text = Me.Hiddenselecionpais.Value Then
                                Me.ComboBoxEditPaisDesExt.SelectedIndex = i
                                Exit For
                            End If
                        Next

                        '------------------------------------------------
                        'Lista los departamentos en el evento postback
                        'y conserva el elemento seleccionado
                        '-----------------------------------------------
                        Dim Refclasra As New ClassRadicador
                        Dim Resulta As String = ""
                        Resulta = Refclas.Lista_Departamentos_Paises(Me.ComboBoxEditDepartDestExt, Me.Hiddenselecionpais.Value, Me.UpdatePanelContenido)
                        If Resulta = "YES" Then
                            Me.ComboBoxEditDepartDestExt.Text = Me.Hiddenselecionciudad.Value
                            For i As Integer = 0 To Me.ComboBoxEditDepartDestExt.Items.Count - 1
                                If Me.ComboBoxEditDepartDestExt.Items(i).Text = Me.Hiddenselecionciudad.Value Then
                                    Me.ComboBoxEditDepartDestExt.SelectedIndex = i
                                    Exit For
                                End If
                            Next

                        End If
                        '------------------------------------------------
                        'Lista los municipios en el evento postback
                        'y conserva el elemento seleccionado
                        '-----------------------------------------------
                        Resulta = Refclas.lista_Municipios_Departamentos(Me.DropDownListciudad, Me.Hiddenselecionciudad.Value)
                        If Resulta = "YES" Then
                            Me.DropDownListciudad.Text = Me.Hiddenmunicipio.Value
                            For i As Integer = 0 To Me.DropDownListciudad.Items.Count - 1
                                If Me.DropDownListciudad.Items(i).Text = Me.Hiddenmunicipio.Value Then
                                    Me.DropDownListciudad.SelectedIndex = i
                                    Exit For
                                End If
                            Next

                        End If
                    End If
                Else
                    'Result = Refclas.Genera_Interface_Radicacion(Matri_Split_Tag(1), Tipo_Plantilla)
                    'If Result <> "YES" Then
                    '    MsgBox(Result, MsgBoxStyle.Information)
                    '    Exit Sub
                    'End If
                End If
            End If

        End If
    End Sub

    Private Sub ComboBoxEditPaisDesExt_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxEditPaisDesExt.SelectedIndexChanged

    End Sub

    Protected Sub ComboBoxEditDepartDestExt_SelectedIndexChanged(sender As Object, e As EventArgs)

    End Sub

    Protected Sub ComboBoxEditPaisDesExt_SelectedIndexChanged1(sender As Object, e As EventArgs)

    End Sub

    Private Sub Buttonllenardepartamento_Click(sender As Object, e As EventArgs) Handles Buttonllenardepartamento.Click
        Dim Refclascript As New Classscrripjava
        Dim Result As String = ""
        Try
            If Me.Hiddenselecionpais.Value = "" Or Me.Hiddenselecionpais.Value = "SELECCIONE" Then
                Exit Sub
            End If
            Dim Refclas As New ClassRadicador
            Result = Refclas.Lista_Departamentos_Paises(Me.ComboBoxEditDepartDestExt, Me.Hiddenselecionpais.Value, Me.UpdatePanelContenido)
            If Result <> "YES" Then
                Refclascript.Showscripman(Result, Me.UpdatePanelContenido)
                'MsgBox(Result, MsgBoxStyle.Critical)
                Exit Sub
            End If

        Catch ex As Exception
            Refclascript.Showscripman(ex.Message, Me.UpdatePanelContenido)


        End Try
    End Sub

    Private Sub Buttonllenarciudad_Click(sender As Object, e As EventArgs) Handles Buttonllenarciudad.Click
        Dim Result As String = ""
        Dim Refclascript As New Classscrripjava
        Try

            If Me.Hiddenselecionciudad.Value = "" Or Me.Hiddenselecionciudad.Value = "SELECCIONE" Then
                Exit Sub
            End If
            Dim Refclas As New ClassRadicador
            Me.DropDownListciudad.Items.Clear()
            Result = Refclas.lista_Municipios_Departamentos(Me.DropDownListciudad, Me.Hiddenselecionciudad.Value)
            If Result <> "YES" Then
                Refclascript.Showscripman(Result, Me.UpdatePanelContenido)
                Exit Sub
            End If
        Catch ex As Exception
            Refclascript.Showscripman(ex.Message, Me.UpdatePanelContenido)
        End Try
    End Sub

    Private Sub BootonAgregar_Click(sender As Object, e As EventArgs) Handles BootonAgregar.Click
        Dim Refclas As New ClassRadicador
        Dim refclasjava As New Classscrripjava
        Dim Result As String = ""
        Result = Refclas.Agregar_destinatario_externo(Me, Me.UpdatePanelContenido)
        If Result <> "YES" Then
            refclasjava.Showscripman(Result, UpdatePanelContenido)
            Exit Sub
        End If
    End Sub

    Private Sub WebFormRadicacion_PreInit(sender As Object, e As EventArgs) Handles Me.PreInit
        'Me.ScriptManager1.RegisterPostBackControl(Me.ButtonImprimir)
        ''If (WebClientPrint.ProcessPrintJob(Request)) Then


        'Dim useDefaultPrinter As Boolean = (Request("useDefaultPrinter") = "checked")
        'Dim printerName As String = Server.UrlDecode(Request("printerName"))

        ''full path of the txt file to be printed
        'Dim txtFilePath As String = "C:\gayco\myFile.txt"

        ''create a temp file name for our txt file...
        'Dim fileName As String = Guid.NewGuid().ToString("N") + System.IO.Path.GetExtension(txtFilePath)

        ''Create a PrintFile object with the txt file
        'Dim file As New PrintFile(txtFilePath, fileName)
        ''Create a ClientPrintJob and send it back to the client!
        'Dim cpj As New ClientPrintJob()
        ''set file to print...
        'cpj.PrintFile = file
        ''set client printer...
        ''If (useDefaultPrinter OrElse printerName = "null") Then
        ''    cpj.ClientPrinter = New DefaultPrinter()
        ''Else
        ''    cpj.ClientPrinter = New InstalledPrinter(printerName)
        ''End If
        ''send it...
        'cpj.ClientPrinter = New UserSelectedPrinter()
        'cpj.SendToClient(Response)

        ''End If
    End Sub

    Private Sub WebFormRadicacion_PreRenderComplete(sender As Object, e As EventArgs) Handles Me.PreRenderComplete
        Dim script As [String] = "$(document).ready(function(){$('#" + TextBoxEditNombreDestRem.ClientID & "').autocomplete();});"
        ScriptManager.RegisterStartupScript(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", TextBoxEditNombreDestRem.ClientID), script, True)
    End Sub

    Private Sub ButtonConsulta_Click(sender As Object, e As EventArgs) Handles ButtonConsulta.Click
        Dim Refclas As New ClassRadicador
        Dim Clasescript As New Classscrripjava
        Dim Matri_Dat() As String
        Erase Matri_Dat
        Try
            Dim Result = Refclas.Retorna_Datos_Dest_Externo(Me.TextBoxEditNombreDestRem.Text, Matri_Dat)
            If Result <> "YES" Then
                Clasescript.Showscripman(Result, Me.UpdatePanelContenido)
                Exit Sub
            End If
            If Not Matri_Dat Is Nothing Then
                Me.TextBoxEditDirecionDestEst.Text = Matri_Dat(0).Trim
                Me.ComboBoxEditPaisDesExt.Text = Matri_Dat(1).Trim
                Me.Hiddenselecionpais.Value = Matri_Dat(1).Trim
                'Me.ComboBoxEditDepartDestExt.Text = Matri_Dat(2).Trim
                Me.Hiddenselecionciudad.Value = Matri_Dat(2).Trim
                'Me.DropDownListciudad.Text = Matri_Dat(3).Trim
                Me.Hiddenmunicipio.Value = Matri_Dat(3).Trim
                Me.TextBoxEditTelefonodestext.Text = Matri_Dat(4).Trim
                Me.TextBoxEditCodPostalDestExt.Text = Matri_Dat(5).Trim
                If Matri_Dat.Length > 6 Then
                    Me.TextBoxEntidadempresa.Text = Matri_Dat(6).Trim
                End If
                Me.HiddenIDdestinatario.Value = Matri_Dat(7).Trim
                '-----------------------------------------------
                'Asigna los valores del pais cuando se activa
                'el evento postback carga los paises y seleccion
                'el item seleccinado post evento
                '-----------------------------------------------
                Me.ComboBoxEditPaisDesExt.Items.Clear()
                Dim ilist As New ListItem
                ilist.Text = "SELECCIONE"
                ilist.Value = "SELECCIONE"
                Me.ComboBoxEditPaisDesExt.Items.Add(ilist)
                ilist = New ListItem
                ilist.Text = "COLOMBIA"
                ilist.Value = "COLOMBIA"
                '-------------------------------------------------
                'Selecciona elemento post evento postback
                '-------------------------------------------------
                Me.ComboBoxEditPaisDesExt.Items.Add(ilist)
                Me.ComboBoxEditPaisDesExt.Text = Me.Hiddenselecionpais.Value
                For i As Integer = 0 To Me.ComboBoxEditPaisDesExt.Items.Count - 1
                    If Me.ComboBoxEditPaisDesExt.Items(i).Text = Me.Hiddenselecionpais.Value Then
                        Me.ComboBoxEditPaisDesExt.SelectedIndex = i
                        Exit For
                    End If
                Next

                '------------------------------------------------
                'Lista los departamentos en el evento postback
                'y conserva el elemento seleccionado
                '-----------------------------------------------
                Dim Refclasra As New ClassRadicador
                Dim Resulta As String = ""
                Resulta = Refclas.Lista_Departamentos_Paises(Me.ComboBoxEditDepartDestExt, Me.Hiddenselecionpais.Value, Me.UpdatePanelContenido)
                If Resulta = "YES" Then
                    Me.ComboBoxEditDepartDestExt.Text = Me.Hiddenselecionciudad.Value
                    For i As Integer = 0 To Me.ComboBoxEditDepartDestExt.Items.Count - 1
                        If Me.ComboBoxEditDepartDestExt.Items(i).Text = Me.Hiddenselecionciudad.Value Then
                            Me.ComboBoxEditDepartDestExt.SelectedIndex = i
                            Exit For
                        End If
                    Next

                End If
                '------------------------------------------------
                'Lista los municipios en el evento postback
                'y conserva el elemento seleccionado
                '-----------------------------------------------
                Resulta = Refclas.lista_Municipios_Departamentos(Me.DropDownListciudad, Me.Hiddenselecionciudad.Value)
                If Resulta = "YES" Then
                    Me.DropDownListciudad.Text = Me.Hiddenmunicipio.Value
                    For i As Integer = 0 To Me.DropDownListciudad.Items.Count - 1
                        If Me.DropDownListciudad.Items(i).Text = Me.Hiddenmunicipio.Value Then
                            Me.DropDownListciudad.SelectedIndex = i
                            Exit For
                        End If
                    Next

                End If
            Else

            End If
        Catch ex As Exception
            Clasescript.Showscripman(ex.Message, Me.UpdatePanelContenido)
        End Try
    End Sub

    Private Sub ButtonActualizar_Click(sender As Object, e As EventArgs) Handles ButtonActualizar.Click
        Dim Result As String = ""
        Dim Refclas As New ClassRadicador
        Dim Refclasjava As New Classscrripjava
        If Me.HiddenIDdestinatario.Value = "" Then
            Refclasjava.Showscripman("Consulte el destinatario para actualizar", Me.UpdatePanelContenido)
            Exit Sub
        End If
        Result = Refclas.Actualizar_destinatario_externo(Me, Me.UpdatePanelContenido)
        If Result <> "YES" Then
            Refclasjava.Showscripman(Result, Me.UpdatePanelContenido)
        End If
    End Sub

    Private Sub ModalPopupExtenderdestinatario_Load(sender As Object, e As EventArgs) Handles ModalPopupExtenderdestinatario.Load

    End Sub

    Private Sub ButtonEliminar_Click(sender As Object, e As EventArgs) Handles ButtonEliminar.Click

        Dim Result As String = ""
        Dim Refclas As New ClassRadicador
        Dim Refclasjava As New Classscrripjava
        If Me.HiddenIDdestinatario.Value = "" Then
            Refclasjava.Showscripman("Consulte el destinatario para eliminar", Me.UpdatePanelContenido)
            Exit Sub
        End If
        If Me.HiddenPROMP.Value = "1" Then
            Exit Sub
        End If
        Result = Refclas.eliminar_destinatario_externo(Me, Me.UpdatePanelContenido)
        If Result <> "YES" Then
            Refclasjava.Showscripman(Result, Me.UpdatePanelContenido)
        End If
    End Sub
    'Cerrar el modal popup exten
    Private Sub Buttoncacerrar_Click(sender As Object, e As EventArgs) Handles Buttoncacerrar.Click

    End Sub

    Private Sub ModalPopupExtenderdestinatario_Unload(sender As Object, e As EventArgs) Handles ModalPopupExtenderdestinatario.Unload

    End Sub
    '*******Activa el los focus del campo destinatario
    Private Sub focusremitente_Click(sender As Object, e As EventArgs) Handles focusremitente.Click
        Try
            Dim Result As String = ""
            Dim Matri_Dat() As String
            Dim Refclas As New ClassRadicador
            Erase Matri_Dat
            Dim textbremitente As TextBox = sender.page.findcontrol("Remitente_Cor" & "|" & "Destinatario_Cor")
            Dim texbdireccion As TextBox = sender.page.findcontrol("Direccion_Dest" & "|" & "DIRECIONDESTINATARIO")
            Dim texbmunicipio As TextBox = sender.page.findcontrol("MUNICIPIO|MUNICIPIO")
            Dim texbtelefono As TextBox = sender.page.findcontrol("Telefono_Dest" & "|" & "TELEFONO")
            Dim textfecha As TextBox = sender.page.findcontrol("Fecha_Documento" + "|" + "Fecha_Documento")
            Dim updat As UpdatePanel = sender.page.findcontrol("ActualizaindiceImage")

            Result = Refclas.Verifica_Existencia_Destinatario_Ext_Guia(textbremitente.Text, Matri_Dat)
            If Result <> "YES" Then
                'MsgBox(Result, MsgBoxStyle.Critical)
                'textfecha.Focus()
                updat.Update()
                Exit Sub
            End If

            If Not Matri_Dat Is Nothing Then
                texbdireccion.Text = Matri_Dat(0).Trim
                texbmunicipio.Text = Matri_Dat(1).Trim
                texbtelefono.Text = Matri_Dat(2).Trim
                updat.Update()


            Else
                texbdireccion.Text = ""
                texbmunicipio.Text = ""
                texbtelefono.Text = ""
                'textfecha.Focus()
                updat.Update()



            End If


        Catch ex As Exception
            MsgBox("Error general evento  TexLosfocus_LosTfoc " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    'Private Sub ButtonImprimir_Click(sender As Object, e As EventArgs) Handles ButtonImprimir.Click
    '    Dim Reclasjava As New Classscrripjava
    '    Dim clasradicador As New ClassRadicador
    '    Dim Result As String = ""
    '    If Me.Hiddendatoradicacion.Value = "" Then
    '        Reclasjava.Showscripman("Imposible encontrar datos para impresion", Me.UpdatePanelContenido)
    '        Exit Sub
    '    End If
    '    Dim ob As New ClientPrintJob()
    '    Result = clasradicador.Genera_Rotulo_Impresion(Me.Hiddendatoradicacion.Value, "")
    '    If Result <> "YES" Then
    '        Reclasjava.Showscripman(Result, Me.UpdatePanelContenido)
    '        Exit Sub
    '    Else
    '        ob.SendToClient(Response)
    '    End If

    'End Sub
End Class