Public Structure stru_permiso_nivel
    Dim id_permisos_niveles As Integer
    Dim remit_dest_interno_id_Remit_Dest_Int As Integer
    Dim ra_pro_niveles_id_nivel As Integer
    Dim carga_archivo As Integer
    Dim descarga_archivo As Integer
    Dim compartir_archivo As Integer
    Dim cambiar_nombre_archivo As Integer
    Dim elimiminar_archivo As Integer
    Dim radicar_archivo As Integer
    Dim visualizar_archivo As Integer
    Dim editar_expediente As Integer
    Dim eliminar_expediente As Integer
    Dim agregar_expediente As Integer
    Dim mover_expediente As Integer
    Dim copiar_archivo As Integer
    Dim crear_nivel_orgnizacion As Integer
End Structure
Public Class Class_ra_pro_permisos_niveles
    Function Solicita_numero_permisos_nivel(ByVal id_nivel As Integer, _
                                            ByRef numero_permiso As Integer) As String
        Try
            Dim Parametro_Consulta = "select id_permisos_niveles " & _
            " from ra_pro_permisos_niveles WHERE ra_pro_niveles_id_nivel=" & id_nivel
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_pro_permisos_niveles")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_numero_permisos_nivel = "Funcion  Solicita_numero_permisos_nivel dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                numero_permiso = 0
                Solicita_numero_permisos_nivel = "YES"
                Exit Function
            Else
                numero_permiso = Datset.Tables(0).Rows.Count
                Solicita_numero_permisos_nivel = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_numero_permisos_nivel = "Inconsistencia general función Solicita_numero_permisos_nivel " & ex.Message
        End Try
    End Function
    Function Solicita_datos_estrctura_permiso_nivel_usuario_gestion(ByVal id_nivel As Integer, _
                                                                    ByVal id_usuario_gestion As Integer, _
                                                                    ByRef stru_permiso_nivel As stru_permiso_nivel) As String
        Try
            Dim Parametro_Consulta = "select id_permisos_niveles,remit_dest_interno_id_Remit_Dest_Int," & _
            "ra_pro_niveles_id_nivel,carga_archivo,descarga_archivo," & _
            "compartir_archivo,elimiminar_archivo,radicar_archivo,visualizar_archivo,editar_expediente," & _
            "eliminar_expediente,agregar_expediente,cambiar_nombre_archivo,mover_expediente,copiar_archivo" & _
            " from ra_pro_permisos_niveles WHERE ra_pro_niveles_id_nivel=" & id_nivel & _
            " and remit_dest_interno_id_Remit_Dest_Int=" & id_usuario_gestion
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_pro_permisos_niveles")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_estrctura_permiso_nivel_usuario_gestion = "Funcion  Solicita_numero_permisos_nivel dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                stru_permiso_nivel = Nothing
                Solicita_datos_estrctura_permiso_nivel_usuario_gestion = "El usuario no tiene permisos sobre el nivel (" & id_nivel & ")"
                Exit Function
            Else
                stru_permiso_nivel.id_permisos_niveles = Datset.Tables(0).Rows(0).Item(0)
                stru_permiso_nivel.remit_dest_interno_id_Remit_Dest_Int = Datset.Tables(0).Rows(0).Item(1)
                stru_permiso_nivel.ra_pro_niveles_id_nivel = Datset.Tables(0).Rows(0).Item(2)
                stru_permiso_nivel.carga_archivo = Datset.Tables(0).Rows(0).Item(3)
                stru_permiso_nivel.descarga_archivo = Datset.Tables(0).Rows(0).Item(4)
                stru_permiso_nivel.compartir_archivo = Datset.Tables(0).Rows(0).Item(5)
                stru_permiso_nivel.elimiminar_archivo = Datset.Tables(0).Rows(0).Item(6)
                stru_permiso_nivel.radicar_archivo = Datset.Tables(0).Rows(0).Item(7)
                stru_permiso_nivel.visualizar_archivo = Datset.Tables(0).Rows(0).Item(8)
                stru_permiso_nivel.editar_expediente = Datset.Tables(0).Rows(0).Item(9)
                stru_permiso_nivel.eliminar_expediente = Datset.Tables(0).Rows(0).Item(10)
                stru_permiso_nivel.agregar_expediente = Datset.Tables(0).Rows(0).Item(11)
                stru_permiso_nivel.cambiar_nombre_archivo = Datset.Tables(0).Rows(0).Item(12)
                stru_permiso_nivel.mover_expediente = Datset.Tables(0).Rows(0).Item(13)
                stru_permiso_nivel.copiar_archivo = Datset.Tables(0).Rows(0).Item(14)
                Solicita_datos_estrctura_permiso_nivel_usuario_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_estrctura_permiso_nivel_usuario_gestion = "Inconsistencia general función Solicita_datos_estrctura_permiso_nivel_usuario_gestion " & ex.Message
        End Try
    End Function
    Function Solicita_matriz_datos_estrctura_permiso_nivel_usuario_gestion(ByVal id_nivel As Integer, _
                                                                           ByRef stru_permiso_nivel() As stru_permiso_nivel) As String
        Try
            Dim Parametro_Consulta = "select id_permisos_niveles,remit_dest_interno_id_Remit_Dest_Int," & _
            "ra_pro_niveles_id_nivel,carga_archivo,descarga_archivo," & _
            "compartir_archivo,elimiminar_archivo,radicar_archivo,visualizar_archivo,editar_expediente," & _
            "eliminar_expediente,agregar_expediente,cambiar_nombre_archivo,mover_expediente,copiar_archivo" & _
            " from ra_pro_permisos_niveles WHERE ra_pro_niveles_id_nivel=" & id_nivel 
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_pro_permisos_niveles")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_matriz_datos_estrctura_permiso_nivel_usuario_gestion = "Funcion  Solicita_numero_permisos_nivel dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                stru_permiso_nivel = Nothing
                Solicita_matriz_datos_estrctura_permiso_nivel_usuario_gestion = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_permiso_nivel(i)
                    stru_permiso_nivel(i).id_permisos_niveles = Datset.Tables(0).Rows(i).Item(0)
                    stru_permiso_nivel(i).remit_dest_interno_id_Remit_Dest_Int = Datset.Tables(0).Rows(i).Item(1)
                    stru_permiso_nivel(i).ra_pro_niveles_id_nivel = Datset.Tables(0).Rows(i).Item(2)
                    stru_permiso_nivel(i).carga_archivo = Datset.Tables(0).Rows(i).Item(3)
                    stru_permiso_nivel(i).descarga_archivo = Datset.Tables(0).Rows(i).Item(4)
                    stru_permiso_nivel(i).compartir_archivo = Datset.Tables(0).Rows(i).Item(5)
                    stru_permiso_nivel(i).elimiminar_archivo = Datset.Tables(0).Rows(i).Item(6)
                    stru_permiso_nivel(i).radicar_archivo = Datset.Tables(0).Rows(i).Item(7)
                    stru_permiso_nivel(i).visualizar_archivo = Datset.Tables(0).Rows(i).Item(8)
                    stru_permiso_nivel(i).editar_expediente = Datset.Tables(0).Rows(i).Item(9)
                    stru_permiso_nivel(i).eliminar_expediente = Datset.Tables(0).Rows(i).Item(10)
                    stru_permiso_nivel(i).agregar_expediente = Datset.Tables(0).Rows(i).Item(11)
                    stru_permiso_nivel(i).cambiar_nombre_archivo = Datset.Tables(0).Rows(i).Item(12)
                    stru_permiso_nivel(i).mover_expediente = Datset.Tables(0).Rows(i).Item(13)
                    stru_permiso_nivel(i).copiar_archivo = Datset.Tables(0).Rows(i).Item(14)
                Next             
                Solicita_matriz_datos_estrctura_permiso_nivel_usuario_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_matriz_datos_estrctura_permiso_nivel_usuario_gestion = "Inconsistencia general función Solicita_datos_estrctura_permiso_nivel_usuario_gestion " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_permiso_nivel(ByVal id_usuario_gestion As Integer, _
                                               ByVal id_nivel As Integer, _
                                               ByRef estado_nivel_compartido_usuario As String) As String
        Try
            Dim Parametro_Consulta = "select id_permisos_niveles " & _
           " from ra_pro_permisos_niveles WHERE ra_pro_niveles_id_nivel=" & id_nivel & _
           " and remit_dest_interno_id_Remit_Dest_Int=" & id_usuario_gestion
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_pro_permisos_niveles")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_permiso_nivel = "Funcion  Solicita_existencia_permiso_nivel dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_nivel_compartido_usuario = "NO"
                Solicita_existencia_permiso_nivel = "YES"
                Exit Function
            Else
                estado_nivel_compartido_usuario = "YES"
                Solicita_existencia_permiso_nivel = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_permiso_nivel = "Inconsistencia general función Solicita_existencia_permiso_nivel " & ex.Message
        End Try
    End Function
    Function Solicita_id_usuario_id_permiso(ByVal id_permiso As Integer, _
                                            ByRef id_usurio_permiso As Integer) As String
        Try
            Dim Parametro_Consulta = "select remit_dest_interno_id_Remit_Dest_Int " & _
       " from ra_pro_permisos_niveles WHERE id_permisos_niveles=" & id_permiso
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_pro_permisos_niveles")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_usuario_id_permiso = "Funcion  Solicita_id_usuario_id_permiso dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_usurio_permiso = 0
                Solicita_id_usuario_id_permiso = "Imposible encontrar el usuario relacionado al permiso (" & id_permiso & ")"
                Exit Function
            Else
                id_usurio_permiso = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_usuario_id_permiso = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_usuario_id_permiso = "Inconsistencia general función Solicita_id_usuario_id_permiso " & ex.Message
        End Try
      
    End Function
    Function Solicita_listado_usuario_permisos_nivel(ByVal id_nivel As Integer, _
                                                     ByRef grediview As GridView, _
                                                     ByRef reflabel As Label, _
                                                     ByRef hideselecion As Object, _
                                                     ByRef update As UpdatePanel, _
                                                     ByVal tipo_consulta As Integer, _
                                                     ByVal valor_consulta As String, _
                                                     ByRef colum_order_name As String, _
                                                     ByRef order_colum As String, _
                                                     ByRef UpdatePanel_title_permisos As UpdatePanel) As String
        Try
            Dim sql_consulta As String = ""
            If tipo_consulta = 1 Then
                sql_consulta = "SELECT rcs.id_permisos_niveles," & _
                   "rdi.Nombre_Remitente as USUARIO_COMPARTIDO,rdi.Cargo_Remite " _
                  & "as CARGO,carga_archivo as CARGAR_ARCHIVOS,rcs.descarga_archivo AS DESCAR_ARCHIVOS,rcs.compartir_archivo as COMPARTIR_ARCHIVOS," _
                  & "rcs.elimiminar_archivo AS ELIMINAR_ARCHIVOS,rcs.cambiar_nombre_archivo AS CAMBIAR_NOMBRES_ARCHIVO,rcs.radicar_archivo " & _
                  " as RADICAR_ARCHIVOS,rcs.visualizar_archivo as VISUALIZAR_ARCHIVOS,rcs.editar_expediente as EDITAR_EXPEDIENTES " & _
                  ",rcs.eliminar_expediente as ELIMINAR_EXPEDIENTES, rcs.agregar_expediente as AGREGAR_EXPEDIENTES, rcs.mover_expediente as MOVER_EXPEDIENTES " & _
                    ",rcs.copiar_archivo AS MOVER_ARCHIVOS_EXPEDIENTES " & _
                  " from ra_pro_permisos_niveles AS rcs " & _
                   " INNER JOIN remit_dest_interno AS rdi on (rdi.id_Remit_Dest_Int=rcs.Remit_Dest_Interno_id_remit_dest_Int) WHERE rcs.ra_pro_niveles_id_nivel= " & id_nivel & _
                    " order by " & colum_order_name & " " & order_colum
            Else
                sql_consulta = "SELECT rcs.id_permisos_niveles," & _
                    "rdi.Nombre_Remitente as USUARIO_COMPARTIDO,rdi.Cargo_Remite " _
                   & "as CARGO,carga_archivo as CARGAR_ARCHIVOS,rcs.descarga_archivo AS DESCAR_ARCHIVOS,rcs.compartir_archivo as COMPARTIR_ARCHIVOS," _
                   & "rcs.elimiminar_archivo AS ELIMINAR_ARCHIVOS,rcs.cambiar_nombre_archivo AS CAMBIAR_NOMBRES_ARCHIVO,rcs.radicar_archivo " & _
                   " as RADICAR_ARCHIVOS,rcs.visualizar_archivo as VISUALIZAR_ARCHIVOS,rcs.editar_expediente as EDITAR_EXPEDIENTES " & _
                   ",rcs.eliminar_expediente as ELIMINAR_EXPEDIENTES, rcs.agregar_expediente as AGREGAR_EXPEDIENTES , rcs.mover_expediente as MOVER_EXPEDIENTES" & _
                   ",rcs.copiar_archivo AS MOVER_ARCHIVOS_EXPEDIENTES " & _
                   " from ra_pro_permisos_niveles AS rcs " & _
                    " INNER JOIN remit_dest_interno AS rdi on (rdi.id_Remit_Dest_Int=rcs.Remit_Dest_Interno_id_remit_dest_Int) " & _
                   " WHERE  (rdi.Nombre_Remitente like '%" & valor_consulta & "%'" & _
                   " or rdi.Cargo_Remite like '%" & valor_consulta & "%')" & _
                   " and rcs.ra_pro_niveles_id_nivel= " & id_nivel & _
                   " order by " & colum_order_name & " " & order_colum
            End If
            HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_SOLICITUD_COLABORACION") = tipo_consulta
            HttpContext.Current.Session.Item("GA_DATO_CONSULTA_SOLICITUD_COLABORACION") = valor_consulta
            HttpContext.Current.Session.Item("Sort_matri_colum_colaboracion") = {"OPCIONES", "USUARIO_COMPARTIDO", "CARGO"}
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_listado_usuario_permisos_nivel = "Error listando documentos compartidos a otros usuarios " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                If tipo_consulta = 1 Then
                    reflabel.Text = "Se encontraron 0 registro(s) de permisos del nivel "
                Else
                    reflabel.Text = "Se encontraron 0 registro(s) de permisos del nivel"
                End If
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                UpdatePanel_title_permisos.Update()
                update.Update()
                Solicita_listado_usuario_permisos_nivel = "YES"
                Exit Function
            Else
                If tipo_consulta = 1 Then
                    reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) "
                Else
                    reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & "  registro(s) "
                End If
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                UpdatePanel_title_permisos.Update()
                update.Update()
                Dim matri_permisos() As String = {"", "id_permisos_niveles", "Nombre_Remitente", "Cargo_Remite", "carga_archivo", "descarga_archivo", _
                    "compartir_archivo", "elimiminar_archivo", "cambiar_nombre_archivo", "radicar_archivo", "visualizar_archivo", _
                    "editar_expediente", "eliminar_expediente", "agregar_expediente", "mover_expediente", "copiar_archivo"}
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    Dim imaga_buton As New HtmlInputImage
                    imaga_buton = New HtmlInputImage
                    imaga_buton.Attributes.Add("Class", "image_buton_clik_image_")
                    imaga_buton.Attributes.Add("onclick", "prevent_lista_permisos(event,this);")
                    imaga_buton.Attributes.Add("title", "Elimina usuario compartido")
                    imaga_buton.Src = "../workflow/imageneswf/trash-alt-light-20.png"
                    imaga_buton.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    imaga_buton.Attributes.Add("tip_event", "elimina_registro")
                    grediview.Rows(i).Cells(0).Controls.Add(imaga_buton)
                    For z As Integer = 0 To grediview.Rows(i).Cells.Count - 1
                        If z <= 3 Then
                            grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If

                        If z > 3 Then
                            Dim chek_ As New HtmlInputCheckBox
                            If grediview.Rows(i).Cells(z).Text = "1" Then
                                chek_.Checked = True
                            Else
                                chek_.Checked = False
                            End If
                            grediview.Rows(i).Cells(z).Style.Add("text-align", "center")
                            chek_.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                            chek_.Attributes.Add("DAcampoActualiza", matri_permisos(z))
                            chek_.Attributes.Add("DNtable", "ra_pro_permisos_niveles")
                            chek_.Attributes.Add("DAcampoCompara", "id_permisos_niveles")
                            chek_.Attributes.Add("DAcaponivel", id_nivel)
                            chek_.Attributes.Add("tip_event", "edita_registro_event")
                            chek_.Attributes.Add("onclick", "prevent_lista_permisos(event,this);")
                            grediview.Rows(i).Cells(z).Controls.Add(chek_)

                        End If
                    Next
                Next
                Dim Refclas As New ClassGredview
                Result = Refclas.add_clase_acender_decender(colum_order_name, _
                                                            HttpContext.Current.Session.Item("Sort_matri_colum_colaboracion"), _
                                                            order_colum, _
                                                            grediview)
                If Result <> "YES" Then
                    Solicita_listado_usuario_permisos_nivel = "Error add clase función  Solicita_listado_usuario_permisos_nivel " & Result
                    Exit Function
                End If
                Solicita_listado_usuario_permisos_nivel = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_listado_usuario_permisos_nivel = "Inconsistencia general función Solicita_listado_usuario_permisos_nivel " & ex.Message
        End Try
    End Function
    Function Lista_permisos_usuario_gestion_nivel(ByVal id_usuario_gestion As Integer, _
                                                  ByVal id_nivel As Integer, _
                                                  ByRef pag As Page) As String
        Try
            Dim CheckBox_cargar_archivo_lista As CheckBox = pag.FindControl("CheckBox_cargar_archivo_lista")
            Dim CheckBox_eliminar_archivos_lista As CheckBox = pag.FindControl("CheckBox_eliminar_archivos_lista")
            Dim CheckBox_cambiar_nombre_archivos_lista As CheckBox = pag.FindControl("CheckBox_cambiar_nombre_archivos_lista")
            Dim CheckBox_descargar_archivo_lista As CheckBox = pag.FindControl("CheckBox_descargar_archivo_lista")
            Dim CheckBox_compartir_archivo_lista As CheckBox = pag.FindControl("CheckBox_compartir_archivo_lista")
            Dim CheckBox_radicar_archivo_lista As CheckBox = pag.FindControl("CheckBox_radicar_archivo_lista")
            Dim CheckBox_visualizar_archivos_lista As CheckBox = pag.FindControl("CheckBox_visualizar_archivos_lista")
            Dim CheckBox_cambia_nombre_expediente_lista As CheckBox = pag.FindControl("CheckBox_cambia_nombre_expediente_lista")
            Dim CheckBox_agregar_expediente_lista As CheckBox = pag.FindControl("CheckBox_agregar_expediente_lista")
            Dim CheckBox_eliminar_expediente_lista As CheckBox = pag.FindControl("CheckBox_eliminar_expediente_lista")
            Dim CheckBox_mover_expediente_lista As CheckBox = pag.FindControl("CheckBox_mover_expediente_lista")
            Dim CheckBox_copiar_archivo_lista As CheckBox = pag.FindControl("CheckBox_copiar_archivo_lista")
            Dim UpdatePanel_lista_permisos_nivel As UpdatePanel = pag.FindControl("UpdatePanel_lista_permisos_nivel")
            Dim ModalPopupExtender_lista_permisos_nivel As AjaxControlToolkit.ModalPopupExtender = _
                pag.FindControl("ModalPopupExtender_lista_permisos_nivel")
            Dim Label_permisos_nivel_lista As Label = pag.FindControl("Label_permisos_nivel_lista")
            Dim nombre_propietario As String = ""
            Dim Nombre_cargo As String = ""
            Dim id_usuario_propietario_nivel As Integer = 0
            Dim Refclass_pronivel As New Class_ra_pro_niveles
            Dim Stru_permiso_nivel As stru_permiso_nivel = Nothing
            Dim Result As String = ""
            Result = Me.Solicita_datos_estrctura_permiso_nivel_usuario_gestion(id_nivel, _
                                                                               id_usuario_gestion, _
                                                                               Stru_permiso_nivel)
            If Result <> "YES" Then
                Lista_permisos_usuario_gestion_nivel = Result
                Exit Function
            End If

            Result = Refclass_pronivel.Solicita_usuario_propietario_nivel(id_nivel, _
                                                                        nombre_propietario, _
                                                                        Nombre_cargo, _
                                                                        id_usuario_propietario_nivel)
            If Result <> "YES" Then
                Lista_permisos_usuario_gestion_nivel = Result
                Exit Function
            End If
            Label_permisos_nivel_lista.Text = "Usuario que comparte el nivel (" & nombre_propietario & " - " & Nombre_cargo & ")"
            If Stru_permiso_nivel.carga_archivo = 1 Then
                CheckBox_cargar_archivo_lista.Checked = True
            Else
                CheckBox_cargar_archivo_lista.Checked = False
            End If
            If Stru_permiso_nivel.elimiminar_archivo = 1 Then
                CheckBox_eliminar_archivos_lista.Checked = True
            Else
                CheckBox_eliminar_archivos_lista.Checked = False
            End If
            If Stru_permiso_nivel.cambiar_nombre_archivo = 1 Then
                CheckBox_cambiar_nombre_archivos_lista.Checked = True
            Else
                CheckBox_cambiar_nombre_archivos_lista.Checked = False
            End If
            If Stru_permiso_nivel.descarga_archivo = 1 Then
                CheckBox_descargar_archivo_lista.Checked = True
            Else
                CheckBox_descargar_archivo_lista.Checked = False
            End If
            If Stru_permiso_nivel.compartir_archivo = 1 Then
                CheckBox_compartir_archivo_lista.Checked = True
            Else
                CheckBox_compartir_archivo_lista.Checked = False
            End If
            If Stru_permiso_nivel.radicar_archivo = 1 Then
                CheckBox_radicar_archivo_lista.Checked = True
            Else
                CheckBox_radicar_archivo_lista.Checked = False
            End If
            If Stru_permiso_nivel.visualizar_archivo = 1 Then
                CheckBox_visualizar_archivos_lista.Checked = True
            Else
                CheckBox_visualizar_archivos_lista.Checked = False
            End If
            If Stru_permiso_nivel.editar_expediente = 1 Then
                CheckBox_cambia_nombre_expediente_lista.Checked = True
            Else
                CheckBox_cambia_nombre_expediente_lista.Checked = False
            End If
            If Stru_permiso_nivel.agregar_expediente = 1 Then
                CheckBox_agregar_expediente_lista.Checked = True
            Else
                CheckBox_agregar_expediente_lista.Checked = False
            End If
            If Stru_permiso_nivel.eliminar_expediente = 1 Then
                CheckBox_eliminar_expediente_lista.Checked = True
            Else
                CheckBox_eliminar_expediente_lista.Checked = False
            End If
            If Stru_permiso_nivel.mover_expediente = 1 Then
                CheckBox_mover_expediente_lista.Checked = True
            Else
                CheckBox_mover_expediente_lista.Checked = False
            End If
            If Stru_permiso_nivel.copiar_archivo = 1 Then
                CheckBox_copiar_archivo_lista.Checked = True
            Else
                CheckBox_copiar_archivo_lista.Checked = False
            End If
            UpdatePanel_lista_permisos_nivel.Update()
            ModalPopupExtender_lista_permisos_nivel.Show()
            Lista_permisos_usuario_gestion_nivel = "YES"
            Exit Function
        Catch ex As Exception
            Lista_permisos_usuario_gestion_nivel = "Inconsistencia general función Lista_permisos_usuario_gestion_nivel " & ex.Message
        End Try
    End Function
End Class
