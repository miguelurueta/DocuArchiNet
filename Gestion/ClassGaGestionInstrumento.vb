Imports MySql.Data.MySqlClient
Imports System
Imports System.IO
Public Structure stru_instrumento
    Dim id_organigrama As Integer
    Dim id_tipo_instrumento As Integer
    Dim fecha_instrumento As String
    Dim version_instrumento As String
    Dim Estado_instrumento As Integer
    Dim nombre_instrumento As String
    Dim Descripcion As String
    Dim Justificacion As String
    Dim fecha_registro As String
End Structure
Public Class ClassGaGestionInstrumento
    Function Seleccion_menu_pricipal(ByVal valor_seleccion As String, _
                                     ByRef pag As Page) As String
        Try
            Dim ModalPopupExtender_agregar_instrumento As AjaxControlToolkit.ModalPopupExtender = _
                pag.FindControl("ModalPopupExtender_agregar_instrumento")
            Dim UpdatePane_agregar_instrumento As UpdatePanel = pag.FindControl("UpdatePane_agregar_instrumento")
            Dim DropDownList_tipo_instrumento As DropDownList = pag.FindControl("DropDownList_tipo_instrumento")
            Dim DropDownList_organigrama As DropDownList = pag.FindControl("DropDownList_organigrama")
            Dim DropDownList_tipo_instrumento_editar As DropDownList = pag.FindControl("DropDownList_tipo_instrumento_editar")
            Dim TextBox_nombre_instrumento_editar As TextBox = pag.FindControl("TextBox_nombre_instrumento_editar")
            Dim TextBox_fecha_instrumento_editar As TextBox = pag.FindControl("TextBox_fecha_instrumento_editar")
            Dim TextBox_descripcion_instrumento_editar As TextBox = pag.FindControl("TextBox_descripcion_instrumento_editar")
            Dim TextBox_version_instrumento_editar As TextBox = pag.FindControl("TextBox_version_instrumento_editar")
            Dim TextBox_Justificacion_instrumento_editar As TextBox = pag.FindControl("TextBox_Justificacion_instrumento_editar")
            Dim UpdatePanel_editar_instrumento As UpdatePanel = pag.FindControl("UpdatePanel_editar_instrumento")
            Dim DropDownList_instrumento As DropDownList = pag.FindControl("DropDownList_instrumento")
            Dim ModalPopupExtender_editar_instrumento As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_editar_instrumento")
            Dim Check_activa_instrumento As CheckBox = pag.FindControl("Check_activa_instrumento")
            Dim CheckBox_inactiva_instrumento As CheckBox = pag.FindControl("CheckBox_inactiva_instrumento")
            Dim ModalPopupExtender_activar_inactivar As  _
            AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_activar_inactivar")
            Dim UpdatePanel_activar_inactivar As UpdatePanel = pag.FindControl("UpdatePanel_activar_inactivar")
            Dim ModalPopupExtender_confirmar_eliminar As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_confirmar_eliminar")
            Dim UpdatePanel_confirmar_eliminar As UpdatePanel = pag.FindControl("UpdatePanel_confirmar_eliminar")
            Dim Label_Confirmado As Label = pag.FindControl("Label_Confirmado")
            Dim HiddenField_estado_operacion As Object = pag.FindControl("HiddenField_estado_operacion")
            Dim ModalPopupExtender_agregar_serie As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_agregar_serie")
            Dim UpdatePanel_agregar_serie As UpdatePanel = pag.FindControl("UpdatePanel_agregar_serie")
            Dim DropDownList_tiempo_retencion_gestion As DropDownList = pag.FindControl("DropDownList_tiempo_retencion_gestion")
            Dim DropDownList_tiempo_retencion_central As DropDownList = pag.FindControl("DropDownList_tiempo_retencion_central")
            Dim DropDownList_areas_departamento As DropDownList = pag.FindControl("DropDownList_areas_departamento")
            Dim TreeViewInstrumento As TreeView = pag.FindControl("TreeViewInstrumento")
            Dim UpdatePanel_title_agregar_serie As UpdatePanel = pag.FindControl("UpdatePanel_title_agregar_serie")
            Dim Label_title_agregar_serie As Label = pag.FindControl("Label_title_agregar_serie")
            Dim ModalPopupExtender_seleccion_agregar As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_seleccion_agregar")
            Dim ModalPopupExtender_activar_inactivar_elemento As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_activar_inactivar_elemento")
            Dim UpdatePanel_activar_inactivar_elemento As UpdatePanel = pag.FindControl("UpdatePanel_activar_inactivar_elemento")
            Dim Check_activa_elemento As CheckBox = pag.FindControl("Check_activa_elemento")
            Dim CheckBox_inactiva_elemento As CheckBox = pag.FindControl("CheckBox_inactiva_elemento")
            Dim HiddenField_oper As Object = pag.FindControl("HiddenField_oper")
            Dim Check_agrega_sub_serie As CheckBox _
                = pag.FindControl("Check_agrega_sub_serie")
            Dim CheckBox_agrega_tipo As CheckBox _
                = pag.FindControl("CheckBox_agrega_tipo")
            Dim UpdatePanel_seleccion_agregar As UpdatePanel = pag.FindControl("UpdatePanel_seleccion_agregar")
            Dim ModalPopupExtender_agregar_tipo_documento As AjaxControlToolkit.ModalPopupExtender = _
                pag.FindControl("ModalPopupExtender_agregar_tipo_documento")
            Dim UpdatePanel_agregar_tipo_documento As UpdatePanel = pag.FindControl("UpdatePanel_agregar_tipo_documento")
            Dim HiddenField_agrega_tipo As Object = pag.FindControl("HiddenField_agrega_tipo")
            Dim ModalPopupExtender_agregar_tipo_documento_sub_serie As AjaxControlToolkit.ModalPopupExtender = _
                pag.FindControl("ModalPopupExtender_agregar_tipo_documento_sub_serie")
            Dim UpdatePanel_title_agregar_tipo_documento_sub_serie As UpdatePanel = pag.FindControl("UpdatePanel_title_agregar_tipo_documento_sub_serie")
            Dim Label_title_agregar_tipo_documento_sub_serie As Label = pag.FindControl("Label_title_agregar_tipo_documento_sub_serie")
            Dim ref_clas_organigrama As New ClassGaOrganigrama
            Dim ref_class_trd As New ClassTrdDocumental
            Dim Result As String = ""
            If valor_seleccion = "IAC-ADD-INSTRUMENTO" Then
                If DropDownList_organigrama.SelectedValue Is Nothing Then
                    Seleccion_menu_pricipal = "Debe seleccionar el organigrama para agregar el instrumento archivístico"
                    Exit Function
                End If
                If DropDownList_organigrama.SelectedItem.Text = "" Then
                    Seleccion_menu_pricipal = "Debe seleccionar el organigrama para agregar el instrumento archivístico"
                    Exit Function
                End If
                DropDownList_tipo_instrumento.Items.Clear()
                Dim ilis As New ListItem
                ilis.Text = "Tabla de retención documental"
                ilis.Value = "1"
                DropDownList_tipo_instrumento.Items.Add(ilis)
                ilis = New ListItem
                ilis.Text = "Tabla de valoración documental"
                ilis.Value = "2"
                DropDownList_tipo_instrumento.Items.Add(ilis)
                UpdatePane_agregar_instrumento.Update()
                ModalPopupExtender_agregar_instrumento.Show()
            End If
            If valor_seleccion = "IAC-EDIDA-INSTRUMENTO" Then
                Dim stru_instrumento As stru_instrumento = Nothing
                If DropDownList_instrumento.SelectedValue Is Nothing Then
                    Seleccion_menu_pricipal = "Debe seleccionar el instrumento a editar"
                    Exit Function
                End If
                If DropDownList_instrumento.SelectedValue = "0" Or DropDownList_instrumento.SelectedValue = "" Then
                    Seleccion_menu_pricipal = "Debe seleccionar el instrumento a editar"
                    Exit Function
                End If
                Result = Me.Asigna_datos_estructura_instrumentos_archivisticos(DropDownList_instrumento.SelectedValue, _
                                                                               stru_instrumento)
                If Result <> "YES" Then
                    Seleccion_menu_pricipal = Result
                    Exit Function
                End If
                Result = Me.Crea_interface_caracterizacion_instrumento_archivistico(stru_instrumento, _
                                                                     DropDownList_tipo_instrumento_editar, _
                                                                     TextBox_nombre_instrumento_editar, _
                                                                     TextBox_fecha_instrumento_editar, _
                                                                     TextBox_descripcion_instrumento_editar, _
                                                                     TextBox_version_instrumento_editar, _
                                                                     TextBox_Justificacion_instrumento_editar, _
                                                                     UpdatePanel_editar_instrumento)
                If Result <> "YES" Then
                    Seleccion_menu_pricipal = Result
                    Exit Function
                End If
                ModalPopupExtender_editar_instrumento.Show()
            End If
            If valor_seleccion = "IAC-ACTIVA-INSTRUMENTO" Then
                If DropDownList_instrumento.SelectedValue Is Nothing Then
                    Seleccion_menu_pricipal = "Debe seleccionar el instrumento a cambiar el estado"
                    Exit Function
                End If
                If DropDownList_instrumento.SelectedValue = "0" Or DropDownList_instrumento.SelectedValue = "" Then
                    Seleccion_menu_pricipal = "Debe seleccionar el instrumento a cambiar el estado"
                    Exit Function
                End If
                Result = Me.Activa_cambio_estado_instrumento(DropDownList_instrumento.SelectedValue, _
                                                            Check_activa_instrumento, _
                                                            CheckBox_inactiva_instrumento, _
                                                            ModalPopupExtender_activar_inactivar, _
                                                            UpdatePanel_activar_inactivar)
                If Result <> "YES" Then
                    Seleccion_menu_pricipal = Result
                    Exit Function
                End If
            End If
            If valor_seleccion = "IAC-ELIM-INSTRUMENTO" Then
                If DropDownList_instrumento.SelectedValue Is Nothing Then
                    Seleccion_menu_pricipal = "Debe seleccionar el instrumento a eliminar"
                    Exit Function
                End If
                If DropDownList_instrumento.SelectedValue = "0" Or DropDownList_instrumento.SelectedValue = "" Then
                    Seleccion_menu_pricipal = "Debe seleccionar el instrumento a eliminar"
                    Exit Function
                End If
                HiddenField_estado_operacion.Value = "IAC-ELIM-INSTRUMENTO"
                Label_Confirmado.Text = "Desea eliminar el instrumento " & DropDownList_instrumento.SelectedItem.Text
                UpdatePanel_confirmar_eliminar.Update()
                ModalPopupExtender_confirmar_eliminar.Show()
            End If

            If valor_seleccion = "IAC-ADD-TABLA" Then
                If DropDownList_instrumento.Text = "" Then
                    Seleccion_menu_pricipal = "Debe seleccionar el instrumento para agregar un nuevo elemento a la tabla"
                    Exit Function
                End If

                If DropDownList_areas_departamento.SelectedValue Is Nothing Then
                    Seleccion_menu_pricipal = "Debe seleccionar el área o departamento para agregar un nuevo elemento a la tabla"
                    Exit Function
                End If
                If DropDownList_areas_departamento.SelectedValue = "" Then
                    Seleccion_menu_pricipal = "Debe seleccionar el área para o departamento para agregar un nuevo elemento a la tabla"
                    Exit Function
                End If
                If TreeViewInstrumento.SelectedNode Is Nothing Then
                    Seleccion_menu_pricipal = "Debe seleccionar un elmento de la estructura para agregar un nuevo elemento a la tabla"
                    Exit Function
                End If
                If TreeViewInstrumento.SelectedNode.Value = "" Then
                    Label_title_agregar_serie.Text = "Agregar serie documental "
                Else
                    Dim spli_t() As String = TreeViewInstrumento.SelectedNode.Value.ToString.Split("|")
                    If spli_t(2) = 1 Then
                        CheckBox_agrega_tipo.Enabled = True
                        Check_agrega_sub_serie.Enabled = True
                        UpdatePanel_seleccion_agregar.Update()
                        ModalPopupExtender_seleccion_agregar.Show()
                        Seleccion_menu_pricipal = "YES"
                        Exit Function
                    End If
                    If spli_t(2) = 2 Then
                        UpdatePanel_title_agregar_tipo_documento_sub_serie.Update()
                        Label_title_agregar_tipo_documento_sub_serie.Text = "Agregar tipo documento a sub serie documental"
                        ModalPopupExtender_agregar_tipo_documento_sub_serie.Show()
                        Seleccion_menu_pricipal = "YES"
                        Exit Function
                    End If
                    If spli_t(2) = 3 Then

                        Seleccion_menu_pricipal = "No se puede agregar un nuevo elemento a un tipo documental"
                        Exit Function
                    End If
                    If spli_t(2) = 4 Then
                        Seleccion_menu_pricipal = "No se puede agregar un nuevo elemento a un tipo documental"
                        Exit Function
                    End If
                End If
                Label_title_agregar_serie.Text = "Agregar serie documental "
                DropDownList_tiempo_retencion_gestion.Items.Clear()
                DropDownList_tiempo_retencion_central.Items.Clear()
                For i As Integer = 0 To 100
                    DropDownList_tiempo_retencion_gestion.Items.Add(i)
                    DropDownList_tiempo_retencion_central.Items.Add(i)
                Next
                Dim id_tipo_instrumento As Integer = 0
                Dim Ref_class_gestion_instrumento As New Class_ra_registro_instrumento_archivistico
                Result = Ref_class_gestion_instrumento.Retorna_id_tipo_instrumento(Val(DropDownList_instrumento.SelectedValue), _
                                                                                   id_tipo_instrumento)
                If Result <> "YES" Then
                    Seleccion_menu_pricipal = Result
                    Exit Function
                End If
              
                UpdatePanel_agregar_serie.Update()
                UpdatePanel_title_agregar_serie.Update()
                ModalPopupExtender_agregar_serie.Show()
            End If
            If valor_seleccion = "IAC-EDIDA-TABLA" Then
                If TreeViewInstrumento.SelectedNode Is Nothing Then
                    Seleccion_menu_pricipal = "Debe seleccionar un elemento de la estructura para editar"
                    Exit Function
                End If
                If TreeViewInstrumento.SelectedNode.Value = "" Then
                    Seleccion_menu_pricipal = "YES"
                    Exit Function
                Else
                    Dim spli_t() As String = TreeViewInstrumento.SelectedNode.Value.ToString.Split("|")
                    If spli_t(2) = 1 Then
                        Result = ref_class_trd.Activa_editar_serie_documental(spli_t(1), _
                                                                              pag)
                        If Result <> "YES" Then
                            Seleccion_menu_pricipal = Result
                            Exit Function
                        Else
                            Seleccion_menu_pricipal = "YES"
                            Exit Function
                        End If
                    End If
                    If spli_t(2) = 2 Then
                        Result = ref_class_trd.Activa_editar_sub_serie_documental(spli_t(1), pag)
                        If Result <> "YES" Then
                            Seleccion_menu_pricipal = Result
                            Exit Function
                        Else
                            Seleccion_menu_pricipal = "YES"
                            Exit Function
                        End If
                    End If
                    If spli_t(2) = 4 Then
                        Result = ref_class_trd.Activa_editar_tipo_documento(spli_t(1), "TextBox_nombre_tipo_documento_sub_serie", _
                                                                            "TextBox_ruta_documento_sub_serie", _
                                                                            "TextBoxCodigoDocumento_sub_serie", _
                                                                             "UpdatePanel_agregar_tipo_documento_sub_serie", _
                                                                             "ModalPopupExtender_agregar_tipo_documento_sub_serie", _
                                                                             "Label_title_agregar_tipo_documento_sub_serie", _
                                                                             "Edita tipo documento sub serie", _
                                                                             "UpdatePanel_title_agregar_tipo_documento_sub_serie", _
                                                                             pag, _
                                                                             "CheckBox_trasv_sub_serie")
                        If Result <> "YES" Then
                            Seleccion_menu_pricipal = Result
                            Exit Function
                        Else
                            Seleccion_menu_pricipal = "YES"
                            Exit Function
                        End If
                    End If
                    If spli_t(2) = 3 Then
                        Result = ref_class_trd.Activa_editar_tipo_documento(spli_t(1), "TextBox_nombre_tipo_documento", _
                                                                             "TextBox_ruta_documento", _
                                                                             "TextBoxCodigoDocumento", _
                                                                              "UpdatePanel_agregar_tipo_documento", _
                                                                              "ModalPopupExtender_agregar_tipo_documento", _
                                                                              "Label_title_agregar_tipo_documento", _
                                                                              "Edita tipo documento serie", _
                                                                              "UpdatePanel_title_agregar_tipo_documento", _
                                                                              pag, _
                                                                              "CheckBox_trasv_serie")
                        If Result <> "YES" Then
                            Seleccion_menu_pricipal = Result
                            Exit Function
                        Else
                            Seleccion_menu_pricipal = "YES"
                            Exit Function
                        End If
                    End If
                End If
            End If
            If valor_seleccion = "IAC-ELIM-TABLA" Then
                If DropDownList_instrumento.Text = "" Then
                    Seleccion_menu_pricipal = "Debe seleccionar el insturmento para eliminar un elemento de la tabla"
                    Exit Function
                End If

                If DropDownList_areas_departamento.SelectedValue Is Nothing Then
                    Seleccion_menu_pricipal = "Debe seleccionar el área o departamento para eliminar un  elemento de la tabla"
                    Exit Function
                End If
                If DropDownList_areas_departamento.SelectedValue = "" Then
                    Seleccion_menu_pricipal = "Debe seleccionar el área o departamento para eliminar un  elemento de la tabla"
                    Exit Function
                End If
                If TreeViewInstrumento.SelectedNode Is Nothing Then
                    Seleccion_menu_pricipal = "Debe seleccionar el elemento de la tabla a eliminar"
                    Exit Function
                End If
                If TreeViewInstrumento.SelectedNode.Value = "" Then
                    Seleccion_menu_pricipal = "Imposible eliminar el elemento raiz"
                    Exit Function
                Else
                    Dim spli_t() As String = TreeViewInstrumento.SelectedNode.Value.ToString.Split("|")
                    If spli_t(2) = 1 Then
                        HiddenField_estado_operacion.Value = "IAC-ELIM-INSTRUMENTO1"
                        Label_Confirmado.Text = "Desea eliminar la serie " & TreeViewInstrumento.SelectedNode.Text
                        UpdatePanel_confirmar_eliminar.Update()
                        ModalPopupExtender_confirmar_eliminar.Show()
                        Seleccion_menu_pricipal = "YES"
                        Exit Function
                    End If
                    If spli_t(2) = 2 Then
                        HiddenField_estado_operacion.Value = "IAC-ELIM-INSTRUMENTO2"
                        Label_Confirmado.Text = "Desea eliminar la sub serie " & TreeViewInstrumento.SelectedNode.Text
                        UpdatePanel_confirmar_eliminar.Update()
                        ModalPopupExtender_confirmar_eliminar.Show()
                        Seleccion_menu_pricipal = "YES"
                        Exit Function
                    End If
                    If spli_t(2) = 3 Then
                        HiddenField_estado_operacion.Value = "IAC-ELIM-INSTRUMENTO3"
                        Label_Confirmado.Text = "Desea eliminar el tipo documental " & TreeViewInstrumento.SelectedNode.Text & " de la  serie "
                        UpdatePanel_confirmar_eliminar.Update()
                        ModalPopupExtender_confirmar_eliminar.Show()
                        Seleccion_menu_pricipal = "YES"
                        Exit Function
                    End If
                    If spli_t(2) = 4 Then
                        HiddenField_estado_operacion.Value = "IAC-ELIM-INSTRUMENTO3"
                        Label_Confirmado.Text = "Desea eliminar el tipo documental " & TreeViewInstrumento.SelectedNode.Text & " de la sub serie "
                        UpdatePanel_confirmar_eliminar.Update()
                        ModalPopupExtender_confirmar_eliminar.Show()
                        Seleccion_menu_pricipal = "YES"
                        Exit Function
                    End If
                End If
            End If
            If valor_seleccion = "IAC-ACTIVA-TABLA" Then
                If DropDownList_instrumento.Text = "" Then
                    Seleccion_menu_pricipal = "Debe seleccionar el insturmento para cambiar el estado de un elemento de la tabla"
                    Exit Function
                End If

                If DropDownList_areas_departamento.SelectedValue Is Nothing Then
                    Seleccion_menu_pricipal = "Debe seleccionar el área o departamento para cambiar el estado de un  elemento de la tabla"
                    Exit Function
                End If
                If DropDownList_areas_departamento.SelectedValue = "" Then
                    Seleccion_menu_pricipal = "Debe seleccionar el área o departamento para cambiar el estado de un  elemento de la tabla"
                    Exit Function
                End If
                If TreeViewInstrumento.SelectedNode Is Nothing Then
                    Seleccion_menu_pricipal = "Debe seleccionar el elemento de la tabla a cambiar el estado"
                    Exit Function
                End If
                If TreeViewInstrumento.SelectedNode.Value = "" Then
                    Seleccion_menu_pricipal = "Imposible cambiar el elemento raiz"
                    Exit Function
                Else
                    Dim Estado_ As Integer = 0
                    Dim spli_t() As String = TreeViewInstrumento.SelectedNode.Value.ToString.Split("|")
                    If spli_t(2) = 1 Then
                        Result = ref_class_trd.Solicita_estado_serie_documental(spli_t(1), Estado_)
                        If Result <> "YES" Then
                            Seleccion_menu_pricipal = Result
                            Exit Function
                        End If
                        HiddenField_oper.Value = "IAC-ACTIVA-TABLA1"
                        Result = ref_class_trd.Asigna_cambio_estado_elemento(Estado_, Check_activa_elemento, CheckBox_inactiva_elemento, _
                                                                              ModalPopupExtender_activar_inactivar_elemento, _
                                                                              UpdatePanel_activar_inactivar_elemento)
                        If Result <> "YES" Then
                            Seleccion_menu_pricipal = Result
                            Exit Function
                        End If

                        Seleccion_menu_pricipal = "YES"
                        Exit Function
                    End If
                    If spli_t(2) = 2 Then
                        Result = ref_class_trd.Solicita_estado_sub_serie_documental(spli_t(1), Estado_)
                        If Result <> "YES" Then
                            Seleccion_menu_pricipal = Result
                            Exit Function
                        End If
                        HiddenField_oper.Value = "IAC-ACTIVA-TABLA2"
                        Result = ref_class_trd.Asigna_cambio_estado_elemento(Estado_, Check_activa_elemento, CheckBox_inactiva_elemento, _
                                                                              ModalPopupExtender_activar_inactivar_elemento, _
                                                                              UpdatePanel_activar_inactivar_elemento)
                        If Result <> "YES" Then
                            Seleccion_menu_pricipal = Result
                            Exit Function
                        End If
                        Seleccion_menu_pricipal = "YES"
                        Exit Function
                    End If
                    If spli_t(2) = 3 Then
                        Result = ref_class_trd.Solicita_estado_tipo_documento(spli_t(1), Estado_)
                        If Result <> "YES" Then
                            Seleccion_menu_pricipal = Result
                            Exit Function
                        End If
                        HiddenField_oper.Value = "IAC-ACTIVA-TABLA3"
                        Result = ref_class_trd.Asigna_cambio_estado_elemento(Estado_, Check_activa_elemento, CheckBox_inactiva_elemento, _
                                                                              ModalPopupExtender_activar_inactivar_elemento, _
                                                                              UpdatePanel_activar_inactivar_elemento)
                        If Result <> "YES" Then
                            Seleccion_menu_pricipal = Result
                            Exit Function
                        End If
                        Seleccion_menu_pricipal = "YES"
                        Exit Function
                    End If
                    If spli_t(2) = 4 Then
                        Result = ref_class_trd.Solicita_estado_tipo_documento(spli_t(1), Estado_)
                        If Result <> "YES" Then
                            Seleccion_menu_pricipal = Result
                            Exit Function
                        End If
                        HiddenField_oper.Value = "IAC-ACTIVA-TABLA3"
                        Result = ref_class_trd.Asigna_cambio_estado_elemento(Estado_, Check_activa_elemento, CheckBox_inactiva_elemento, _
                                                                              ModalPopupExtender_activar_inactivar_elemento, _
                                                                              UpdatePanel_activar_inactivar_elemento)
                        If Result <> "YES" Then
                            Seleccion_menu_pricipal = Result
                            Exit Function
                        End If
                        Seleccion_menu_pricipal = "YES"
                        Exit Function
                    End If
                End If
            End If
            If valor_seleccion = "IAC-EXPORTA-INSTRUMENTO" Then
                If DropDownList_instrumento.Text = "" Then
                    Seleccion_menu_pricipal = "Debe seleccionar el insturmento para exportar la tabla del departamento"
                    Exit Function
                End If

                If DropDownList_areas_departamento.SelectedValue Is Nothing Then
                    Seleccion_menu_pricipal = "Debe seleccionar el área o departamento para exportar la tabla"
                    Exit Function
                End If
                If DropDownList_areas_departamento.SelectedValue = "" Then
                    Seleccion_menu_pricipal = "Debe seleccionar el área o departamento para exportar la tabla"
                    Exit Function
                End If
                Result = ref_class_trd.Export_Serie(DropDownList_areas_departamento.SelectedValue, 1, _
                                                     DropDownList_areas_departamento.SelectedItem.Text, pag)
                If Result <> "YES" Then
                    Seleccion_menu_pricipal = Result
                    Exit Function
                End If
            End If
            Seleccion_menu_pricipal = "YES"
        Catch ex As Exception
            Seleccion_menu_pricipal = "Inconsistencia general función Seleccion_menu_pricipal " & ex.Message
        End Try
    End Function
    Function Agregar_instrumento_archivistico(ByVal id_organigrama As Integer, _
                                              ByVal nombre_instrumento As String, _
                                              ByVal id_tipo_instrumento As Integer,
                                              ByVal fecha_instrumento As String, _
                                              ByVal descripcion_instrumento As String, _
                                              ByVal version_instrumento As String, _
                                              ByVal justificacion_instrumento As String, _
                                              ByRef drop_list As DropDownList, _
                                              ByRef update As UpdatePanel) As String
        Try
            If id_organigrama = 0 Then
                Agregar_instrumento_archivistico = "Debe seleccionar el organigrama al que pertenecerá el instrumento archivístico"
                Exit Function
            End If
            If nombre_instrumento = "" Then
                Agregar_instrumento_archivistico = "Debe informar el nombre del instrumento archivístico"
                Exit Function
            End If
            If id_tipo_instrumento = 0 Then
                Agregar_instrumento_archivistico = "Debe seleccionar el tipo de instrumento archivístico"
                Exit Function
            End If
            If fecha_instrumento = "" Then
                Agregar_instrumento_archivistico = "Debe informar la fecha de creación del instrumento archivístico"
                Exit Function
            End If
            If descripcion_instrumento = "" Then
                Agregar_instrumento_archivistico = "Debe informar la descripción del instrumento archivístico"
                Exit Function
            End If
            Dim date_time As String = ""
            Dim Result As String = ""
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Result = ref_ClassGestionFechas.Retorna_fecha_registro(date_time)
            If Result <> "YES" Then
                Agregar_instrumento_archivistico = Result
                Exit Function
            End If
            Dim existencia_documento As String = ""
            Result = Me.Verifica_instrumentos_relacionados_al_organigrama(id_tipo_instrumento, id_organigrama, _
                                                                        nombre_instrumento, existencia_documento)
            If Result <> "YES" Then
                Agregar_instrumento_archivistico = Result
                Exit Function
            End If
            Dim ref_version_instrumento As String = Left(date_time.ToString, 10)
            ref_version_instrumento = ref_version_instrumento.ToString.Replace("/", "")
            ref_version_instrumento = ref_version_instrumento.ToString.Replace("-", "")
            If version_instrumento <> "" Then
                ref_version_instrumento = version_instrumento
            End If
            Dim ref_jutificacion As String = "null"
            If justificacion_instrumento <> "" Then
                ref_jutificacion = "'" & justificacion_instrumento & "'"
            End If
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim sqlinsertion As String = "Insert into ra_registro_instrumento_archivistico (registro_organigrama_ID_ORGANIGRAMA," & _
                "id_tipo_instrumento,fecha_instrumento,version_instrumento,Estado_instrumento,nombre_instrumento,Descripcion," & _
                "Justificacion,fecha_registro) values (" & _
                id_organigrama & "," & id_tipo_instrumento & ",'" & fecha_instrumento & "','" & ref_version_instrumento & "',0,'" & _
               nombre_instrumento & "','" & descripcion_instrumento & "'," & ref_jutificacion & ",'" & date_time & "')"
            Dim last_insert As Object = 0
            Result = ref.SELECTION_LAST_INSERT_COMMAND(sqlinsertion, last_insert)
            If Result <> "YES" Then
                Agregar_instrumento_archivistico = Result
                Exit Function
            Else
                Dim tipo_descripcion As String = ""
                If id_tipo_instrumento = 1 Then
                    tipo_descripcion = "(TRD) -"
                Else
                    tipo_descripcion = "(TVD) -"
                End If
                Dim ilis As New ListItem
                ilis.Text = tipo_descripcion & nombre_instrumento
                ilis.Value = last_insert
                drop_list.Items.Add(ilis)
                For i As Integer = 0 To drop_list.Items.Count - 1
                    If drop_list.Items(i).Value = last_insert Then
                        drop_list.Items(i).Selected = True
                        Exit For
                    End If
                Next
                update.Update()
                Agregar_instrumento_archivistico = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Agregar_instrumento_archivistico = "Inconsistencia general función Agregar_instrumento_archivistico " & ex.Message
        End Try
    End Function
    Function Verifica_instrumentos_relacionados_al_organigrama(ByVal id_tipo_instrumento As Integer, _
                                                             ByVal id_organigrama As Integer, _
                                                             ByVal nombre_instrumento As String, _
                                                             ByRef existencia As String) As String
        '--------------------------------------------------
        'Función : Solicita la existencia del organigrama
        'con el nombre del organigrama, la identificación
        'del organigrama y del tipo de instrumento
        'Fecha : 2018-06-27
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_registro_instrumento_archivistico")
            Dim sql_consulta As String = "Select id_instrumento from ra_registro_instrumento_archivistico " & _
                " where registro_organigrama_ID_ORGANIGRAMA=" & id_organigrama & _
                " and id_tipo_instrumento=" & id_tipo_instrumento & _
                " and nombre_instrumento='" & nombre_instrumento & "'"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Verifica_instrumentos_relacionados_al_organigrama = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia = "NO"
                Verifica_instrumentos_relacionados_al_organigrama = "YES"
                Exit Function
            Else
                existencia = "YES"
                Verifica_instrumentos_relacionados_al_organigrama = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_instrumentos_relacionados_al_organigrama = "Inconsistencia general función Verifica_instrumentos_relacionados_al_organigrama " & ex.Message
        End Try

    End Function
    
    Function Asigna_datos_estructura_instrumentos_archivisticos(ByVal id_instrumento As Integer, _
                                                                ByRef stru_instrumento As stru_instrumento) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_registro_instrumento_archivistico")
            Dim sql_consulta As String = "Select registro_organigrama_ID_ORGANIGRAMA,id_tipo_instrumento,fecha_instrumento," & _
                "version_instrumento,Estado_instrumento,nombre_instrumento,Descripcion,Justificacion " & _
                " from ra_registro_instrumento_archivistico where id_instrumento=" & id_instrumento
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Asigna_datos_estructura_instrumentos_archivisticos = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Asigna_datos_estructura_instrumentos_archivisticos = "Imposible encontrar datos del instrumento (" & id_instrumento & ")"
                Exit Function
            Else
                stru_instrumento.id_organigrama = Datset.Tables(0).Rows(0).Item(0)
                stru_instrumento.id_tipo_instrumento = Datset.Tables(0).Rows(0).Item(1)
                stru_instrumento.fecha_instrumento = Datset.Tables(0).Rows(0).Item(2)
                Dim SplitWf() As String = Left(stru_instrumento.fecha_instrumento, 10).Split("/")
                stru_instrumento.fecha_instrumento = SplitWf(2) & "-" & SplitWf(1) & "-" & SplitWf(0)
                stru_instrumento.version_instrumento = Datset.Tables(0).Rows(0).Item(3)
                stru_instrumento.Estado_instrumento = Datset.Tables(0).Rows(0).Item(4)
                stru_instrumento.nombre_instrumento = Datset.Tables(0).Rows(0).Item(5)
                If Datset.Tables(0).Rows(0).IsNull(6) Then
                    stru_instrumento.Descripcion = ""
                Else
                    stru_instrumento.Descripcion = Datset.Tables(0).Rows(0).Item(6)
                End If
                If Datset.Tables(0).Rows(0).IsNull(7) Then
                    stru_instrumento.Justificacion = ""
                Else
                    stru_instrumento.Justificacion = Datset.Tables(0).Rows(0).Item(7)
                End If
                Asigna_datos_estructura_instrumentos_archivisticos = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Asigna_datos_estructura_instrumentos_archivisticos = "Inconsistencia general función Asigna_datos_estructura_instrumentos_archivisticos " & ex.Message
        End Try
    End Function
    Function Crea_interface_caracterizacion_instrumento_archivistico(ByVal stru_instrumento As stru_instrumento, _
                                                                     ByRef DropDownList_tipo_instrumento_editar As DropDownList, _
                                                                     ByRef TextBox_nombre_instrumento_editar As TextBox, _
                                                                     ByRef TextBox_fecha_instrumento_editar As TextBox, _
                                                                     ByRef TextBox_descripcion_instrumento_editar As TextBox, _
                                                                     ByRef TextBox_version_instrumento_editar As TextBox, _
                                                                     ByRef TextBox_Justificacion_instrumento_editar As TextBox, _
                                                                     ByRef UpdatePanel_editar_instrumento As UpdatePanel) As String
        Try
            DropDownList_tipo_instrumento_editar.Items.Clear()
            Dim ilis As New ListItem
            ilis.Text = "Tabla de retención documental"
            ilis.Value = "1"
            DropDownList_tipo_instrumento_editar.Items.Add(ilis)
            ilis = New ListItem
            ilis.Text = "Tabla de valoración documental"
            ilis.Value = "2"
            DropDownList_tipo_instrumento_editar.Items.Add(ilis)
            If stru_instrumento.id_tipo_instrumento = 1 Then
                DropDownList_tipo_instrumento_editar.Text = "1"
            Else
                DropDownList_tipo_instrumento_editar.Text = "2"
            End If
            TextBox_nombre_instrumento_editar.Text = stru_instrumento.nombre_instrumento
            TextBox_fecha_instrumento_editar.Text = stru_instrumento.fecha_instrumento
            TextBox_descripcion_instrumento_editar.Text = stru_instrumento.Descripcion
            TextBox_version_instrumento_editar.Text = stru_instrumento.version_instrumento
            TextBox_Justificacion_instrumento_editar.Text = stru_instrumento.Justificacion
            UpdatePanel_editar_instrumento.Update()
            Crea_interface_caracterizacion_instrumento_archivistico = "YES"
            Exit Function
        Catch ex As Exception
            Crea_interface_caracterizacion_instrumento_archivistico = "Inconsistencia general Crea_interface_caracterizacion_instrumento_archivistico " & ex.Message
        End Try
    End Function
    Function Lista_instrumentos_archivisticos(ByVal id_organigrama As Integer, _
                                              ByRef drop_list As DropDownList, _
                                              ByRef update As UpdatePanel) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_registro_instrumento_archivistico")
            Dim sql_consulta As String = "Select id_instrumento,nombre_instrumento,id_tipo_instrumento,Estado_instrumento from ra_registro_instrumento_archivistico " & _
                " where registro_organigrama_ID_ORGANIGRAMA=" & id_organigrama & _
                " order by id_tipo_instrumento,id_instrumento"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_instrumentos_archivisticos = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_instrumentos_archivisticos = "YES"
                Exit Function
            Else
                Dim ilis_ As New ListItem
                ilis_.Text = ""
                ilis_.Value = 0
                drop_list.Items.Add(ilis_)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim nombre_instrumento As String = Datset.Tables(0).Rows(i).Item(1)
                    Dim activo As String = ""
                    If Datset.Tables(0).Rows(i).Item(3) = 1 Then
                        activo = "►"
                    End If
                    If Datset.Tables(0).Rows(i).Item(2) = 1 Then
                        nombre_instrumento = activo & "(TRD) - " & nombre_instrumento
                    Else
                        nombre_instrumento = activo & "(TVD) - " & nombre_instrumento
                    End If
                    Dim ilis As New ListItem
                    ilis.Text = nombre_instrumento
                    ilis.Value = Datset.Tables(0).Rows(i).Item(0)
                    drop_list.Items.Add(ilis)
                Next
                Lista_instrumentos_archivisticos = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_instrumentos_archivisticos = "Inconsistencia general función Lista_instrumentos_archivisticos " & ex.Message
        End Try
    End Function
    Function Lista_instrumentos_archivisticos_activos(ByVal id_organigrama As Integer, _
                                                      ByRef drop_list As DropDownList, _
                                                      ByRef update As UpdatePanel) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_registro_instrumento_archivistico")
            Dim sql_consulta As String = "Select id_instrumento,nombre_instrumento,id_tipo_instrumento,Estado_instrumento from ra_registro_instrumento_archivistico " & _
                " where registro_organigrama_ID_ORGANIGRAMA=" & id_organigrama & _
                " and Estado_instrumento=1 " & _
                " order by id_tipo_instrumento,id_instrumento"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_instrumentos_archivisticos_activos = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                drop_list.Items.Clear()
                Lista_instrumentos_archivisticos_activos = "YES"
                Exit Function
            Else
                Dim ilis_ As ListItem
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim nombre_instrumento As String = Datset.Tables(0).Rows(i).Item(1)
                    Dim activo As String = ""
                    If Datset.Tables(0).Rows(i).Item(3) = 1 Then
                        activo = "►"
                    End If
                    If Datset.Tables(0).Rows(i).Item(2) = 1 Then
                        nombre_instrumento = "(TRD) - " & nombre_instrumento
                    Else
                        nombre_instrumento = "(TVD) - " & nombre_instrumento
                    End If
                    ilis_ = New ListItem
                    ilis_.Text = nombre_instrumento
                    ilis_.Value = Datset.Tables(0).Rows(i).Item(0)
                    drop_list.Items.Add(ilis_)
                Next
                ilis_ = New ListItem
                ilis_.Text = ""
                ilis_.Value = 0
                drop_list.Items.Add(ilis_)
                Lista_instrumentos_archivisticos_activos = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_instrumentos_archivisticos_activos = "Inconsistencia general función Lista_instrumentos_archivisticos_activos " & ex.Message
        End Try
    End Function
    Function Lista_instrumentos_archivisticos(ByVal id_organigrama As Integer, _
                                              ByVal id_instrumento As Integer, _
                                              ByRef drop_list As DropDownList, _
                                              ByRef update As UpdatePanel) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_registro_instrumento_archivistico")
            Dim sql_consulta As String = "Select id_instrumento,nombre_instrumento,id_tipo_instrumento,Estado_instrumento from ra_registro_instrumento_archivistico " & _
                " where registro_organigrama_ID_ORGANIGRAMA=" & id_organigrama & _
                " and Estado_instrumento=1 or  id_instrumento=" & id_instrumento & _
                " order by id_tipo_instrumento,id_instrumento"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_instrumentos_archivisticos = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                drop_list.Items.Clear()
                Lista_instrumentos_archivisticos = "YES"
                Exit Function
            Else
                Dim ilis_ As ListItem
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim nombre_instrumento As String = Datset.Tables(0).Rows(i).Item(1)
                    Dim activo As String = ""
                    If Datset.Tables(0).Rows(i).Item(3) = 1 Then
                        activo = "►"
                    End If
                    If Datset.Tables(0).Rows(i).Item(2) = 1 Then
                        nombre_instrumento = "(TRD) - " & nombre_instrumento
                    Else
                        nombre_instrumento = "(TVD) - " & nombre_instrumento
                    End If
                    ilis_ = New ListItem
                    ilis_.Text = nombre_instrumento
                    ilis_.Value = Datset.Tables(0).Rows(i).Item(0)
                    drop_list.Items.Add(ilis_)
                Next
                ilis_ = New ListItem
                ilis_.Text = ""
                ilis_.Value = 0
                drop_list.Items.Add(ilis_)
                For i As Integer = 0 To drop_list.Items.Count - 1
                    If drop_list.Items(i).Value = id_instrumento Then
                        drop_list.Items(i).Selected = True
                        Exit For
                    End If
                Next
                Lista_instrumentos_archivisticos = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_instrumentos_archivisticos = "Inconsistencia general función Lista_instrumentos_archivisticos " & ex.Message
        End Try
    End Function
    Function Lista_instrumentos_archivisticos_activos_default(ByVal id_organigrama As Integer, _
                                                              ByVal id_instrumento As Integer, _
                                                              ByRef drop_list As DropDownList, _
                                                              ByRef update As UpdatePanel) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_registro_instrumento_archivistico")
            Dim sql_consulta As String = "Select id_instrumento,nombre_instrumento," & _
                "id_tipo_instrumento,Estado_instrumento from ra_registro_instrumento_archivistico " & _
                " where registro_organigrama_ID_ORGANIGRAMA=" & id_organigrama & _
                " and Estado_instrumento=1 " & _
                " order by id_tipo_instrumento,id_instrumento"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_instrumentos_archivisticos_activos_default = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                drop_list.Items.Clear()
                Lista_instrumentos_archivisticos_activos_default = "YES"
                Exit Function
            Else
                Dim ilis_ As ListItem
                ilis_ = New ListItem
                ilis_.Text = ""
                ilis_.Value = 0
                drop_list.Items.Add(ilis_)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim nombre_instrumento As String = Datset.Tables(0).Rows(i).Item(1)
                    Dim activo As String = ""
                    If Datset.Tables(0).Rows(i).Item(3) = 1 Then
                        activo = "►"
                    End If
                    If Datset.Tables(0).Rows(i).Item(2) = 1 Then
                        nombre_instrumento = "(TRD) - " & nombre_instrumento
                    Else
                        nombre_instrumento = "(TVD) - " & nombre_instrumento
                    End If
                    ilis_ = New ListItem
                    ilis_.Text = nombre_instrumento
                    ilis_.Value = Datset.Tables(0).Rows(i).Item(0)
                    drop_list.Items.Add(ilis_)
                Next
                For i As Integer = 0 To drop_list.Items.Count - 1
                    If drop_list.Items(i).Value = id_instrumento Then
                        drop_list.Items(i).Selected = True
                        Exit For
                    End If
                Next
                Lista_instrumentos_archivisticos_activos_default = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_instrumentos_archivisticos_activos_default = "Inconsistencia general función Lista_instrumentos_archivisticos_activos_default " & ex.Message
        End Try
    End Function
    Function Lista_instrumentos_producion_documental_manager(ByRef drop_list As DropDownList, _
                                                             ByRef update As UpdatePanel, _
                                                             ByVal id_instrumento As Integer) As String
        Try
            drop_list.Items.Clear()
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_registro_instrumento_archivistico")
            Dim sql_consulta As String = "Select id_instrumento,nombre_instrumento," & _
                "id_tipo_instrumento,Estado_instrumento from ra_registro_instrumento_archivistico " & _
                " where Estado_instrumento=1 " & _
                " order by id_tipo_instrumento,id_instrumento"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_instrumentos_producion_documental_manager = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                drop_list.Items.Clear()
                Lista_instrumentos_producion_documental_manager = "YES"
                Exit Function
            Else
                Dim ilis_ As ListItem
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim nombre_instrumento As String = Datset.Tables(0).Rows(i).Item(1)
                    If Datset.Tables(0).Rows(i).Item(2) = 1 Then
                        nombre_instrumento = "(TRD) - " & nombre_instrumento
                    Else
                        nombre_instrumento = "(TVD) - " & nombre_instrumento
                    End If
                    ilis_ = New ListItem
                    ilis_.Text = nombre_instrumento
                    ilis_.Value = Datset.Tables(0).Rows(i).Item(0)
                    drop_list.Items.Add(ilis_)
                Next
                ilis_ = New ListItem
                ilis_.Text = ""
                ilis_.Value = 0
                drop_list.Items.Add(ilis_)
                For i As Integer = 0 To drop_list.Items.Count - 1
                    If drop_list.Items(i).Value = id_instrumento Then
                        drop_list.Items(i).Selected = True
                        Exit For
                    End If
                Next
                Lista_instrumentos_producion_documental_manager = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_instrumentos_producion_documental_manager = "Inconsistencia general función Lista_instrumentos_producion_documental_manager " & ex.Message
        Finally
            update.Update()
        End Try
    End Function
    
    Function Lista_instrumentos_producion_documental(ByRef drop_list As DropDownList, _
                                                     ByRef update As UpdatePanel, _
                                                     ByVal id_instrumento As Integer) As String
        Try
            drop_list.Items.Clear()
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_registro_instrumento_archivistico")
            Dim sql_consulta As String = "Select id_instrumento,nombre_instrumento,id_tipo_instrumento,Estado_instrumento from ra_registro_instrumento_archivistico " & _
                " where Estado_instrumento=1 and id_tipo_instrumento=1" & _
                " order by id_tipo_instrumento,id_instrumento"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_instrumentos_producion_documental = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                drop_list.Items.Clear()
                Lista_instrumentos_producion_documental = "YES"
                Exit Function
            Else
                Dim ilis_ As ListItem        
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim nombre_instrumento As String = Datset.Tables(0).Rows(i).Item(1)
                    If Datset.Tables(0).Rows(i).Item(2) = 1 Then
                        nombre_instrumento = "(TRD) - " & nombre_instrumento
                    Else
                        nombre_instrumento = "(TVD) - " & nombre_instrumento
                    End If
                    ilis_ = New ListItem
                    ilis_.Text = nombre_instrumento
                    ilis_.Value = Datset.Tables(0).Rows(i).Item(0)
                    drop_list.Items.Add(ilis_)
                Next
                ilis_ = New ListItem
                ilis_.Text = ""
                ilis_.Value = 0
                drop_list.Items.Add(ilis_)
                Lista_instrumentos_producion_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_instrumentos_producion_documental = "Inconsistencia general función Lista_instrumentos_producion_documental " & ex.Message
        Finally
            update.Update()
        End Try
    End Function
    Function Lista_instrumentos_producion_documental_edita(ByRef drop_list As DropDownList, _
                                                           ByRef update As UpdatePanel, _
                                                           ByVal id_instrumento As Integer) As String
        Try
            drop_list.Items.Clear()
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_registro_instrumento_archivistico")
            Dim sql_consulta As String = "Select id_instrumento,nombre_instrumento,id_tipo_instrumento,Estado_instrumento from ra_registro_instrumento_archivistico " & _
                " where Estado_instrumento=1 and id_tipo_instrumento=1" & _
                " order by id_tipo_instrumento,id_instrumento"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_instrumentos_producion_documental_edita = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                drop_list.Items.Clear()
                Lista_instrumentos_producion_documental_edita = "YES"
                Exit Function
            Else
                Dim ilis_ As ListItem
                ilis_ = New ListItem
                ilis_.Text = ""
                ilis_.Value = 0
                drop_list.Items.Add(ilis_)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim nombre_instrumento As String = Datset.Tables(0).Rows(i).Item(1)
                    If Datset.Tables(0).Rows(i).Item(2) = 1 Then
                        nombre_instrumento = "(TRD) - " & nombre_instrumento
                    Else
                        nombre_instrumento = "(TVD) - " & nombre_instrumento
                    End If
                    ilis_ = New ListItem
                    ilis_.Text = nombre_instrumento
                    ilis_.Value = Datset.Tables(0).Rows(i).Item(0)
                    drop_list.Items.Add(ilis_)
                Next
                For i As Integer = 0 To drop_list.Items.Count - 1
                    If drop_list.Items(i).Value = id_instrumento Then
                        drop_list.Items(i).Selected = True
                        Exit For
                    End If
                Next
                Lista_instrumentos_producion_documental_edita = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_instrumentos_producion_documental_edita = "Inconsistencia general función Lista_instrumentos_producion_documental_edita " & ex.Message
        Finally
            update.Update()
        End Try
    End Function
    Function Lista_instrumentos_producion_documental_default(ByRef drop_list As DropDownList, _
                                                             ByRef update As UpdatePanel, _
                                                             ByVal id_instrumento As Integer) As String
        Try
            drop_list.Items.Clear()
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_registro_instrumento_archivistico")
            Dim sql_consulta As String = "Select id_instrumento,nombre_instrumento," & _
                "id_tipo_instrumento,Estado_instrumento from ra_registro_instrumento_archivistico " & _
                " where Estado_instrumento=1 and id_tipo_instrumento=1" & _
                " order by id_tipo_instrumento,id_instrumento"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_instrumentos_producion_documental_default = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                drop_list.Items.Clear()
                Lista_instrumentos_producion_documental_default = "YES"
                Exit Function
            Else
                Dim ilis_ As ListItem
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim nombre_instrumento As String = Datset.Tables(0).Rows(i).Item(1)
                    If Datset.Tables(0).Rows(i).Item(2) = 1 Then
                        nombre_instrumento = "(TRD) - " & nombre_instrumento
                    Else
                        nombre_instrumento = "(TVD) - " & nombre_instrumento
                    End If
                    ilis_ = New ListItem
                    ilis_.Text = nombre_instrumento
                    ilis_.Value = Datset.Tables(0).Rows(i).Item(0)
                    drop_list.Items.Add(ilis_)
                Next
                ilis_ = New ListItem
                ilis_.Text = ""
                ilis_.Value = 0
                drop_list.Items.Add(ilis_)
                For i As Integer = 0 To drop_list.Items.Count - 1
                    If drop_list.Items(i).Value = id_instrumento Then
                        drop_list.Items(i).Selected = True
                        Exit For
                    End If
                Next

                Lista_instrumentos_producion_documental_default = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_instrumentos_producion_documental_default = "Inconsistencia general función Lista_instrumentos_producion_documental_default " & ex.Message
        Finally
            update.Update()
        End Try
    End Function
    Function Lista_instrumentos_producion_documental_por_id(ByRef drop_list As DropDownList, _
                                                            ByRef update As UpdatePanel, _
                                                            ByVal id_instrumento As Integer) As String
        Try
            drop_list.Items.Clear()
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_registro_instrumento_archivistico")
            Dim sql_consulta As String = "Select id_instrumento,nombre_instrumento," & _
                "id_tipo_instrumento,Estado_instrumento from ra_registro_instrumento_archivistico " & _
                " where id_instrumento=" & id_instrumento & _
                " order by id_tipo_instrumento,id_instrumento"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_instrumentos_producion_documental_por_id = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                drop_list.Items.Clear()
                Lista_instrumentos_producion_documental_por_id = "YES"
                Exit Function
            Else
                Dim ilis_ As ListItem
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim nombre_instrumento As String = Datset.Tables(0).Rows(i).Item(1)
                    If Datset.Tables(0).Rows(i).Item(2) = 1 Then
                        nombre_instrumento = "(TRD) - " & nombre_instrumento
                    Else
                        nombre_instrumento = "(TVD) - " & nombre_instrumento
                    End If
                    ilis_ = New ListItem
                    ilis_.Text = nombre_instrumento
                    ilis_.Value = Datset.Tables(0).Rows(i).Item(0)
                    drop_list.Items.Add(ilis_)
                Next
                ilis_ = New ListItem
                ilis_.Text = ""
                ilis_.Value = 0
                drop_list.Items.Add(ilis_)
                For i As Integer = 0 To drop_list.Items.Count - 1
                    If drop_list.Items(i).Value = id_instrumento Then
                        drop_list.Items(i).Selected = True
                        Exit For
                    End If
                Next
                Lista_instrumentos_producion_documental_por_id = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_instrumentos_producion_documental_por_id = "Inconsistencia general función Lista_instrumentos_producion_documental_por_id " & ex.Message
        Finally
            update.Update()
        End Try
    End Function
    Function Edita_instrumento_archivistico( _
                                              ByVal id_instrumento As Integer, _
                                              ByVal nombre_instrumento As String, _
                                              ByVal id_tipo_instrumento As Integer,
                                              ByVal fecha_instrumento As String, _
                                              ByVal descripcion_instrumento As String, _
                                              ByVal version_instrumento As String, _
                                              ByVal justificacion_instrumento As String, _
                                              ByRef drop_list As DropDownList, _
                                              ByRef update As UpdatePanel) As String
       
        If nombre_instrumento = "" Then
            Edita_instrumento_archivistico = "Debe informar el nombre del instrumento archivístico"
            Exit Function
        End If
        If id_tipo_instrumento = 0 Then
            Edita_instrumento_archivistico = "Debe seleccionar el tipo de instrumento archivístico"
            Exit Function
        End If
        If fecha_instrumento = "" Then
            Edita_instrumento_archivistico = "Debe informar la fecha de creación del instrumento archivístico"
            Exit Function
        End If
        If descripcion_instrumento = "" Then
            Edita_instrumento_archivistico = "Debe informar la descripción del instrumento archivístico"
            Exit Function
        End If
        If version_instrumento = "" Then
            Edita_instrumento_archivistico = "Debe informar la versión del instrumento archivístico"
            Exit Function
        End If
       
        Dim stru_instrumento As stru_instrumento = Nothing
        Dim Result As String = ""
        Result = Me.Asigna_datos_estructura_instrumentos_archivisticos(id_instrumento, _
                                                                       stru_instrumento)
        If Result <> "YES" Then
            Edita_instrumento_archivistico = Result
            Exit Function
        End If
        Dim confirm As Boolean = True
        Dim Cambios As String = ""
        Dim existencia As String = ""
        Dim sigla As String = ""
        Dim update_registro As String = "Update ra_registro_instrumento_archivistico "
        If nombre_instrumento <> stru_instrumento.nombre_instrumento Then
            Result = Me.Verifica_instrumentos_relacionados_al_organigrama(id_tipo_instrumento, stru_instrumento.id_organigrama, _
                                                                        nombre_instrumento, existencia)
            If Result <> "YES" Then
                Edita_instrumento_archivistico = Result
                Exit Function
            End If
            If existencia = "YES" Then
                Edita_instrumento_archivistico = "Esta intentado editar el nombre del instrumento con uno ya existente, imposible continuar"
                Exit Function
            End If
            Cambios = Cambios & " Cambio de nombre del instrumento valor actual (" & stru_instrumento.nombre_instrumento & ") Nuevo valor (" & nombre_instrumento & ")"
            If update_registro = "Update ra_registro_instrumento_archivistico " Then
                update_registro = update_registro & " set nombre_instrumento='" & nombre_instrumento & "'"
            Else
                update_registro = update_registro & " , nombre_instrumento='" & nombre_instrumento & "'"
            End If
            If stru_instrumento.Estado_instrumento = 1 Then
                sigla = "►"
            End If
            If id_tipo_instrumento = 1 Then
                sigla = sigla & "(TRD) - "
            Else
                sigla = sigla & "(TVD) - "
            End If
        End If
        If id_tipo_instrumento <> stru_instrumento.id_tipo_instrumento Then
            Dim tipo_instrumento_cambio As String = ""
            Dim tipo_instrumento_actual As String = ""
            If id_tipo_instrumento = 1 Then
                tipo_instrumento_cambio = "Tabla de retención documental"
                sigla = "(TRD) - "
            Else
                tipo_instrumento_cambio = "Tabla de valoración documental"
                sigla = "(TVD) - "
            End If
            If stru_instrumento.id_tipo_instrumento = 1 Then
                tipo_instrumento_actual = "Tabla de retención documental"
            Else
                tipo_instrumento_actual = "Tabla de valoración documental"
            End If
            Cambios = Cambios & " Cambio de tipo de instrumento valor actual (" & tipo_instrumento_actual & ") Nuevo valor (" & tipo_instrumento_cambio & ")"
            If update_registro = "Update ra_registro_instrumento_archivistico " Then
                update_registro = update_registro & " set id_tipo_instrumento='" & id_tipo_instrumento & "'"
            Else
                update_registro = update_registro & " , id_tipo_instrumento='" & id_tipo_instrumento & "'"
            End If
        End If
        Dim fecha_instrumento_ As String = stru_instrumento.fecha_instrumento
        If fecha_instrumento <> fecha_instrumento_ Then
            Cambios = Cambios & " Cambio de fecha del instrumento valor actual (" & fecha_instrumento_ & ") Nuevo valor (" & fecha_instrumento & ")"
            If update_registro = "Update ra_registro_instrumento_archivistico " Then
                update_registro = update_registro & " set fecha_instrumento='" & fecha_instrumento & "'"
            Else
                update_registro = update_registro & " , fecha_instrumento='" & fecha_instrumento & "'"
            End If
        End If
        If descripcion_instrumento <> stru_instrumento.Descripcion Then
            Cambios = Cambios & " Cambio de descrición de instrumento valor actual (" & stru_instrumento.Descripcion & ") Nuevo valor (" & descripcion_instrumento & ")"
            If update_registro = "Update ra_registro_instrumento_archivistico " Then
                update_registro = update_registro & " set Descripcion='" & descripcion_instrumento & "'"
            Else
                update_registro = update_registro & " , Descripcion='" & descripcion_instrumento & "'"
            End If
        End If
        If version_instrumento <> stru_instrumento.version_instrumento Then
            Cambios = Cambios & " Cambio de versión del instrumento valor actual (" & stru_instrumento.version_instrumento & ") Nuevo valor (" & version_instrumento & ")"
            If update_registro = "Update ra_registro_instrumento_archivistico " Then
                update_registro = update_registro & " set version_instrumento='" & version_instrumento & "'"
            Else
                update_registro = update_registro & " , version_instrumento='" & version_instrumento & "'"
            End If
        End If
        If justificacion_instrumento <> stru_instrumento.Justificacion Then
            Cambios = Cambios & " Cambio de justificación del instrumento valor actual (" & stru_instrumento.Justificacion & ") Nuevo valor (" & justificacion_instrumento & ")"
            Dim ref_jutificacion As String = "Null"
            If justificacion_instrumento <> "" Then
                ref_jutificacion = "'" & justificacion_instrumento & "'"
            End If
            If update_registro = "Update ra_registro_instrumento_archivistico " Then
                update_registro = update_registro & " set Justificacion=" & ref_jutificacion
            Else
                update_registro = update_registro & " , Justificacion=" & ref_jutificacion
            End If
        End If
        If update_registro <> "Update ra_registro_instrumento_archivistico " Then
            update_registro = update_registro & " where id_instrumento=" & id_instrumento
        End If
        If update_registro = "Update ra_registro_instrumento_archivistico " Then
            Edita_instrumento_archivistico = "No se detectaron cambios para actualizar el instrumento"
            Exit Function
        End If
        Dim date_time As String = ""
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Result = ref_ClassGestionFechas.Retorna_fecha_registro(date_time)
        If Result <> "YES" Then
            Edita_instrumento_archivistico = Result
            Exit Function
        End If
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
        Dim logi_user As String = HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION")
        Dim hor As String = Now
        Dim detalle_trans As String = "EDITA INSTRUMENTO"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        campos_trans = "EDITA INSTRUMENTO " & id_instrumento & "-" & stru_instrumento.nombre_instrumento & "  (" & _
        " EDITA INSTRUMENTO CON LOS SIGUIENTES CAMBIOS " & Cambios & ")"
        isert_datos = isert_datos & "(" & id_instrumento & _
            ",'" & detalle_trans & "','" & logi_user & "','" & id_user & "','" & date_time & "'," & _
                    "'" & iphost & "','GESTOR-WEB','" & campos_trans & "')"

        Dim update_gestion = "INSERT INTO ra_log_instrumentos_archivisticos (Ra_registro_instrumento_archivistico_id_instrumento,desc_op,USER_OPER,ID_USER,DATE_TRANS" & _
                                             ",IP_TRANS,MODULO_REGISTRO,CAMPOS) values " & _
                                             isert_datos
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Edita_instrumento_archivistico = ""
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = update_registro
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Edita_instrumento_archivistico = "Imposible registrar cambios en el instrumento"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Edita_instrumento_archivistico = "Imposible registrar log del instrumento"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If nombre_instrumento <> stru_instrumento.nombre_instrumento Then
                For i As Integer = 0 To drop_list.Items.Count - 1
                    If drop_list.Items.Item(i).Value = id_instrumento.ToString Then
                        drop_list.Items.Item(i).Text = sigla & nombre_instrumento
                        update.Update()
                        Exit For
                    End If
                Next
            End If
            If id_tipo_instrumento <> stru_instrumento.id_tipo_instrumento Then
                For i As Integer = 0 To drop_list.Items.Count - 1
                    If drop_list.Items.Item(i).Value = id_instrumento.ToString Then
                        drop_list.Items.Item(i).Text = sigla & nombre_instrumento
                        update.Update()
                        Exit For
                    End If
                Next
            End If
            myTrans.Commit()
            Edita_instrumento_archivistico = "YES"
            Exit Function
        Catch ex As MySqlException
            If myConnection.State = ConnectionState.Open Then
                myTrans.Rollback()
                myConnection.Close()
                Edita_instrumento_archivistico = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function


            End If
        Finally
            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Edita_instrumento_archivistico = Edita_instrumento_archivistico
        End Try
    End Function
    Function Activa_cambio_estado_instrumento(ByVal id_instrumento As Integer, _
                                              ByRef Check_activa_instrumento As CheckBox, _
                                              ByRef CheckBox_inactiva_instrumento As CheckBox, _
                                              ByRef ModalPopupExtender_activar_inactivar As  _
                                              AjaxControlToolkit.ModalPopupExtender, _
                                              ByRef UpdatePanel_activar_inactivar As UpdatePanel) As String
        '---------------------------------------------------------------
        'Función : Activa cambio de estado del instrumento archivístico
        'Fecha : 2018-06-29
        'Ing : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------
        Try
            Dim estado_instrumento As Integer = 0
            Dim Result As String = ""
            Result = Solicita_cambio_de_estado_instrumento(id_instrumento, estado_instrumento)
            If Result <> "YES" Then
                Activa_cambio_estado_instrumento = Result
                Exit Function
            End If
            Result = Asigna_cambio_estado_instrumento(id_instrumento, _
                                                  Check_activa_instrumento, _
                                                  CheckBox_inactiva_instrumento, _
                                                  ModalPopupExtender_activar_inactivar, _
                                                  UpdatePanel_activar_inactivar)
            If Result <> "YES" Then
                Activa_cambio_estado_instrumento = Result
                Exit Function
            End If
            Activa_cambio_estado_instrumento = "YES"
            Exit Function
        Catch ex As Exception
            Activa_cambio_estado_instrumento = "Inconsistencia general función Activa_cambio_estado_instrumento " & ex.Message
        End Try

    End Function
    Function Solicita_cambio_de_estado_instrumento(ByVal id_instrumento As Integer, _
                                                   ByRef estado_instrumento As Integer) As String
        '------------------------------------------------------
        'Función : Retorna los el estado del instrumento
        'archivístico
        'Fecha : 2018-08-26
        'Ingeniero : Miguel Angel Urueta Miranda
        '------------------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_registro_instrumento_archivistico")
            Dim sql_consulta As String = "Select Estado_instrumento from ra_registro_instrumento_archivistico " & _
                " where id_instrumento=" & id_instrumento
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_cambio_de_estado_instrumento = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_cambio_de_estado_instrumento = "Imposible encontrar el estado del instrumento archivístico (" & id_instrumento & ")"
                Exit Function
            Else
                estado_instrumento = Datset.Tables(0).Rows(0).Item(0)
                Solicita_cambio_de_estado_instrumento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_cambio_de_estado_instrumento = "Inconsistencia general función Solicita_cambio_de_estado_instrumento " & ex.Message
        End Try
    End Function
    Function Asigna_cambio_estado_instrumento(ByVal id_instrumento As Integer, _
                                              ByRef Check_activa_instrumento As CheckBox, _
                                              ByRef CheckBox_inactiva_instrumento As CheckBox, _
                                              ByRef ModalPopupExtender_activar_inactivar As  _
                                              AjaxControlToolkit.ModalPopupExtender, _
                                              ByRef UpdatePanel_activar_inactivar As UpdatePanel) As String
        '---------------------------------------------------------------
        'Función : Asigna cambio de estado del instrumento archivístico
        'a la interface
        'Fecha : 2018-06-29
        'Ing : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------
        Try
            Dim estado_instrumento As Integer = 0
            Dim Result As String = ""
            Result = Me.Solicita_cambio_de_estado_instrumento(id_instrumento, estado_instrumento)
            If Result <> "YES" Then
                Asigna_cambio_estado_instrumento = Result
                Exit Function
            Else
                If estado_instrumento = 1 Then
                    Check_activa_instrumento.Checked = True
                    CheckBox_inactiva_instrumento.Checked = False
                Else
                    Check_activa_instrumento.Checked = False
                    CheckBox_inactiva_instrumento.Checked = True
                End If
                UpdatePanel_activar_inactivar.Update()
                ModalPopupExtender_activar_inactivar.Show()
                Asigna_cambio_estado_instrumento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Asigna_cambio_estado_instrumento = "Inconsistencia general función Activa_cambia_estado_organigrama " & ex.Message
        End Try
    End Function
    Function cambiar_estado_instrumento_archivistico(ByVal id_organigrama As Integer, _
                                                     ByVal id_instrumento As Integer, _
                                                     ByRef Check_activa_instrumento As CheckBox, _
                                                     ByRef CheckBox_inactiva_instrumento As CheckBox, _
                                                     ByVal nombre_instrumento As String, _
                                                     ByRef DropDownList_instrumento As DropDownList, _
                                                     ByRef UpdatePanel_instrumentos As UpdatePanel) As String
        '---------------------------------------------------------------
        'Función : Cambia de estado del instrumento archivístico
        'a la interface
        'Fecha : 2018-06-29
        'Ing : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------
        Dim estado_instrumento As Integer = 0
        Dim Result As String = ""
        Result = Solicita_cambio_de_estado_instrumento(id_instrumento, _
                                                       estado_instrumento)
        If Result <> "YES" Then
            cambiar_estado_instrumento_archivistico = Result
            Exit Function
        End If
        Dim nuevo_estado_instruemento As Integer = 0
        If Check_activa_instrumento.Checked = True Then
            nuevo_estado_instruemento = 1
        Else
            nuevo_estado_instruemento = 0
        End If
        If nuevo_estado_instruemento = estado_instrumento Then
            cambiar_estado_instrumento_archivistico = "YES"
            Exit Function
        End If
        Dim id_tipo_instrumento As Integer = 0
        Dim Ref_class_gestion_instrumento As New Class_ra_registro_instrumento_archivistico
        Result = Ref_class_gestion_instrumento.Retorna_id_tipo_instrumento(id_instrumento, _
                                                                           id_tipo_instrumento)
        If Result <> "YES" Then
            cambiar_estado_instrumento_archivistico = Result
            Exit Function
        End If
        Dim id_instrumento_activo_actual As Integer = 0
        Dim Ref_class_registro_instrumento As New Class_ra_registro_instrumento_archivistico
        Result = Ref_class_registro_instrumento.Retorna_id_instrumento_activo(id_tipo_instrumento, _
                                                                              id_organigrama, _
                                                                              id_instrumento_activo_actual)
        If Result <> "YES" Then
            cambiar_estado_instrumento_archivistico = Result
            Exit Function
        End If
        Dim date_time As String = ""
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Result = ref_ClassGestionFechas.Retorna_fecha_registro(date_time)
        If Result <> "YES" Then
            cambiar_estado_instrumento_archivistico = Result
            Exit Function
        End If
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
        Dim logi_user As String = HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION")
        Dim hor As String = Now
        Dim detalle_trans As String = "CAMBIA ESTADO INSTRUMENTO"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        campos_trans = "CAMBIA ESTADO INSTRUMENTO " & id_instrumento & "-" & nombre_instrumento & "  (" & _
        " CAMBIA DE ESTADO " & estado_instrumento & " A ESTADO " & nuevo_estado_instruemento & ")"
        isert_datos = isert_datos & "(" & id_instrumento & _
            ",'" & detalle_trans & "','" & logi_user & "','" & id_user & "','" & date_time & "'," & _
                    "'" & iphost & "','GESTOR-WEB','" & campos_trans & "')"

        Dim update_gestion = "INSERT INTO ra_log_instrumentos_archivisticos (Ra_registro_instrumento_archivistico_id_instrumento,desc_op,USER_OPER,ID_USER,DATE_TRANS" & _
                                              ",IP_TRANS,MODULO_REGISTRO,CAMPOS) values " & _
                                              isert_datos
        Dim update_inactiva_todos As String = "Update ra_registro_instrumento_archivistico set Estado_instrumento=0 where id_tipo_instrumento=" & id_tipo_instrumento
        Dim update_cambia_estado As String = "Update ra_registro_instrumento_archivistico set Estado_instrumento=" & nuevo_estado_instruemento & " where id_instrumento=" & id_instrumento
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        cambiar_estado_instrumento_archivistico = ""
        Try
            Dim Switc As Integer
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = update_inactiva_todos
            If id_tipo_instrumento = 1 Then
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    cambiar_estado_instrumento_archivistico = "Imposible inactivar los instrumentos para balance "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If      
            myCommand.CommandText = update_cambia_estado
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                cambiar_estado_instrumento_archivistico = "Imposible cambiar el estado del intrumento"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                cambiar_estado_instrumento_archivistico = "Imposible registrar log del instrumento"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If id_instrumento_activo_actual <> 0 Then
                For i As Integer = 0 To DropDownList_instrumento.Items.Count - 1
                    If DropDownList_instrumento.Items(i).Value = id_instrumento_activo_actual Then
                        DropDownList_instrumento.Items(i).Text = DropDownList_instrumento.Items(i).Text.ToString.Replace("►", "")
                    End If
                Next
            End If
            For i As Integer = 0 To DropDownList_instrumento.Items.Count - 1
                If DropDownList_instrumento.Items(i).Value = id_instrumento Then
                    If nuevo_estado_instruemento = 1 Then
                        DropDownList_instrumento.Items(i).Text = "►" & DropDownList_instrumento.Items(i).Text
                    End If
                End If
            Next
            UpdatePanel_instrumentos.Update()
            myTrans.Commit()
            cambiar_estado_instrumento_archivistico = "YES"
            Exit Function
        Catch ex As MySqlException
            If myConnection.State = ConnectionState.Open Then
                myTrans.Rollback()
                myConnection.Close()
                cambiar_estado_instrumento_archivistico = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally
            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            cambiar_estado_instrumento_archivistico = cambiar_estado_instrumento_archivistico
        End Try
    End Function
    
    
    Function Eliminar_instrumento_archivistico(ByVal id_instrumento As Integer, _
                                               ByRef DropDownList_instrumento As DropDownList, _
                                               ByRef UpdatePanel_instrumentos As UpdatePanel, _
                                               ByRef DropDownList_areas_departamento As DropDownList, _
                                               ByRef TreeViewInstrumento As TreeView, _
                                               ByRef UpdatePanel_treview_instrumento As UpdatePanel) As String

        Dim Result As String = ""
        Dim id_serie As Integer = 0
        Dim Refclass_series As New Class_series_documentales
        Result = Refclass_series.Verifica_serie_relacionada_instrumento_archivistico(id_instrumento, _
                                                                                     id_serie)
        If Result <> "YES" Then
            Eliminar_instrumento_archivistico = Result
            Exit Function
        End If
        If id_serie <> 0 Then
            Eliminar_instrumento_archivistico = "El instrumento archivístico tiene series relacionadas"
            Exit Function
        End If
        Dim Refclass As New Class_ra_log_instrumentos_archivisticos
        Dim existencia As String = "NO"
        Result = Refclass.Solicita_existencia_registro_instrumento(id_instrumento, _
                                                                   existencia)
        If Result <> "YES" Then
            Eliminar_instrumento_archivistico = Result
            Exit Function
        End If
        Dim sql_delete As String = "Delete from ra_registro_instrumento_archivistico where id_instrumento=" & id_instrumento
        Dim sql_delete_log As String = "Delete from ra_log_instrumentos_archivisticos where Ra_registro_instrumento_archivistico_id_instrumento=" & id_instrumento
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Eliminar_instrumento_archivistico = ""
        Try
            Dim Switc As Integer
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sql_delete_log
            Switc = myCommand.ExecuteNonQuery()
            If existencia = "YES" Then
                If Switc = 0 Then
                    Eliminar_instrumento_archivistico = "Imposible eliminar log del instrumento archivístico"
                    'myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            myCommand.CommandText = sql_delete
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Eliminar_instrumento_archivistico = "Imposible eliminar instrumento archivístico"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            DropDownList_areas_departamento.Items.Clear()
            DropDownList_instrumento.Items.Remove(DropDownList_instrumento.SelectedItem)
            TreeViewInstrumento.Nodes.Clear()
            UpdatePanel_treview_instrumento.Update()
            UpdatePanel_instrumentos.Update()
            myTrans.Commit()
            Eliminar_instrumento_archivistico = "YES"
            Exit Function
        Catch ex As MySqlException
            If myConnection.State = ConnectionState.Open Then
                myTrans.Rollback()
                myConnection.Close()
                Eliminar_instrumento_archivistico = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally
            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Eliminar_instrumento_archivistico = Eliminar_instrumento_archivistico
        End Try

    End Function
    Function Lista_areas_organigrama_instrumento(ByVal id_organigrama As Integer, _
                                              ByRef drop_list As DropDownList, _
                                              ByRef update As UpdatePanel) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("areas_depart_radicacion")
            Dim sql_consulta As String = "Select Codigo_Area,Nombre_Area from areas_depart_radicacion " & _
                " where Registro_Organigrama_Id_Organigrama=" & id_organigrama & _
                " order by Nombre_Area"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_areas_organigrama_instrumento = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_areas_organigrama_instrumento = "YES"
                Exit Function
            Else
                Dim ilis_ As New ListItem
                ilis_.Text = ""
                ilis_.Value = 0
                drop_list.Items.Add(ilis_)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim ilis As New ListItem
                    ilis.Text = Datset.Tables(0).Rows(i).Item(1)
                    ilis.Value = Datset.Tables(0).Rows(i).Item(0)
                    drop_list.Items.Add(ilis)
                Next
                Lista_areas_organigrama_instrumento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_areas_organigrama_instrumento = "Inconsistencia general función Lista_areas_organigrama_instrumento " & ex.Message
        End Try
    End Function
    Function Solicita_id_instrumento_default_organigrama(ByVal id_organigrama As Integer, _
                                                         ByVal id_tipo_instrumento As Integer, _
                                                         ByRef id_instrumento As Integer) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_registro_instrumento_archivistico")
            Dim sql_consulta As String = "Select id_instrumento from ra_registro_instrumento_archivistico " & _
                " where registro_organigrama_ID_ORGANIGRAMA=" & id_organigrama & _
                " and id_tipo_instrumento=" & id_tipo_instrumento & " and Estado_instrumento=1"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_instrumento_default_organigrama = "Función Solicita_id_instrumento_default_organigrama doce " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_instrumento = 0
                Solicita_id_instrumento_default_organigrama = "YES"
                Exit Function
            Else
                id_instrumento = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_instrumento_default_organigrama = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_instrumento_default_organigrama = "Inconsistencia general función Solicita_id_instrumento_default_organigrama " & ex.Message
        End Try
    End Function
    Function Solicita_id_insturmento_serie_documental(ByVal id_serie As Integer, _
                                                      ByRef id_instrumento_archivistico As Integer) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("series_documentales")
            Dim sql_consulta As String = "Select Ra_registro_instrumento_archivistico_id_instrumento from series_documentales " & _
                " where Id_Series=" & id_serie
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_insturmento_serie_documental = "Función Solicita_id_insturmento_serie_documental dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_instrumento_archivistico = 0
                Solicita_id_insturmento_serie_documental = "YES"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    id_instrumento_archivistico = 0
                Else
                    id_instrumento_archivistico = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_id_insturmento_serie_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_insturmento_serie_documental = "Inconsistencia general función Solicita_id_insturmento_serie_documental " & ex.Message
        End Try
    End Function
    Function Solicita_id_area_serie_documental(ByVal id_serie As Integer, _
                                                      ByRef id_area_departamento As Integer) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("series_documentales")
            Dim sql_consulta As String = "Select Areas_Depart_Radicacion_Codigo_Area from series_documentales " & _
                " where Id_Series=" & id_serie
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_area_serie_documental = "Función Solicita_id_area_serie_documental dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_area_departamento = 0
                Solicita_id_area_serie_documental = "YES"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    id_area_departamento = 0
                Else
                    id_area_departamento = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_id_area_serie_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_area_serie_documental = "Inconsistencia general función Solicita_id_area_serie_documental " & ex.Message
        End Try
    End Function
    Function Solicita_id_organigrama_area_departamento(ByVal id_area As Integer, _
                                                       ByRef id_organigrama As Integer) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("areas_depart_radicacion")
            Dim sql_consulta As String = "Select Registro_Organigrama_Id_Organigrama from areas_depart_radicacion " & _
                " where Codigo_Area=" & id_area
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_organigrama_area_departamento = "Función Solicita_id_organigrama_area_departamento dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_organigrama = 0
                Solicita_id_organigrama_area_departamento = "YES"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    id_organigrama = 0
                Else
                    id_organigrama = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_id_organigrama_area_departamento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_organigrama_area_departamento = "Inconsistencia general función Solicita_id_organigrama_area_departamento " & ex.Message
        End Try
    End Function
    Function Solicita_id_organigrama_instrumento(ByVal id_instrumento_archivistico As Integer, _
                                                 ByRef id_organigrama As Integer) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_registro_instrumento_archivistico")
            Dim sql_consulta As String = "Select registro_organigrama_ID_ORGANIGRAMA from ra_registro_instrumento_archivistico " & _
                " where id_instrumento=" & id_instrumento_archivistico
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_organigrama_instrumento = "Función Solicita_id_organigrama_instrumento dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_organigrama = 0
                Solicita_id_organigrama_instrumento = "YES"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    id_organigrama = 0
                Else
                    id_organigrama = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_id_organigrama_instrumento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_organigrama_instrumento = "Inconsistencia general función Solicita_id_organigrama_instrumento " & ex.Message
        End Try
    End Function
    Function Solicita_nombre_organigrama_por_identidad_organigrama(ByVal id_organigrama As Integer, _
                                         ByRef nombre_organigrama As String) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("registro_organigrama")
            Dim sql_consulta As String = "Select NOMBRE_ORGANIGRAMA from registro_organigrama " & _
                " where ID_ORGANIGRAMA=" & id_organigrama
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombre_organigrama_por_identidad_organigrama = "Función Solicita_nombre_organigrama_por_identidad_organigrama dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                nombre_organigrama = ""
                Solicita_nombre_organigrama_por_identidad_organigrama = "YES"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    nombre_organigrama = ""
                Else
                    nombre_organigrama = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_nombre_organigrama_por_identidad_organigrama = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombre_organigrama_por_identidad_organigrama = "Inconsistencia general función Solicita_nombre_organigrama_por_identidad_organigrama " & ex.Message
        End Try
    End Function
    Function Solicita_datos_instrumento_rotulo(ByVal id_instrumento_archivistico As Integer, _
                                                 ByRef id_tipo_instrumento As Integer, _
                                                 ByRef nombre_instrumento As String, _
                                                 ByRef version_instrumento As String) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_registro_instrumento_archivistico")
            Dim sql_consulta As String = "Select id_tipo_instrumento,nombre_instrumento,version_instrumento from ra_registro_instrumento_archivistico " & _
                " where id_instrumento=" & id_instrumento_archivistico
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_instrumento_rotulo = "Función Solicita_datos_instrumento_rotulo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_tipo_instrumento = 0
                nombre_instrumento = ""
                version_instrumento = ""
                Solicita_datos_instrumento_rotulo = "YES"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    id_tipo_instrumento = 0
                Else
                    id_tipo_instrumento = Datset.Tables(0).Rows(0).Item(0)
                End If
                nombre_instrumento = Datset.Tables(0).Rows(0).Item(1)
                version_instrumento = Datset.Tables(0).Rows(0).Item(2)
                Solicita_datos_instrumento_rotulo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_instrumento_rotulo = "Inconsistencia general función Solicita_datos_instrumento_rotulo " & ex.Message
        End Try
    End Function
End Class
