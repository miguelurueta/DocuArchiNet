Public Structure stru_config_conector_ruta
    Dim Estado_evia_correo As Integer
    Dim Estado_soicita_autorizacion As Integer
    Dim Estado_soicita_autorizacion_firma_digital As Integer
    Dim Estado_copia_documento_estructura As Integer
    Dim Estado_asigna_expediente As Integer
    Dim Estado_firma_digital As Integer
    Dim estado_valida_balanceo As Integer
End Structure
Public Structure stru_actividades_envio
    Dim Id_Actividad_Siguiente As Integer
    Dim Listado_Actividades_Workflow_Id_Actividad As Integer
    Dim id_Ruta As Integer
    Dim Prioridad_Actividad As Integer
    Dim Ienti_Grafica_Actividad As Integer
    Dim id_actividades_disponibles_envio As Integer
    Dim Estado_evia_correo As Integer
    Dim Estado_soicita_autorizacion As Integer
    Dim Estado_soicita_autorizacion_firma_digital As Integer
    Dim Estado_copia_documento_estructura As Integer
    Dim Estado_asigna_expediente As Integer
End Structure
Public Class Class_actividades_disponibles_envio
    Function Solicita_estado_envio_correo_conector_ruta(ByVal id_conector As Integer, _
                                                        ByRef estado_envio_correo As String) As String
        Try
            If id_conector = 0 Then
                Solicita_estado_envio_correo_conector_ruta = "Por favor borre el cache de su navegador, para poder utilizar la opción de envío"
                Exit Function
            End If
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT  Estado_evia_correo " & _
              " from actividades_disponibles_envio where id_actividades_disponibles_envio= " & id_conector
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("actividades_disponibles_envio")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_envio_correo_conector_ruta = "Función Solicita_estado_envio_correo_conector_ruta dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estado_envio_correo_conector_ruta = "Imposible encontrar el estado de envió de correo el conector de ruta  (" & id_conector & ")"
                Exit Function
            Else
                estado_envio_correo = Datset.Tables(0).Rows(0).Item(0)
                Solicita_estado_envio_correo_conector_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_envio_correo_conector_ruta = "Inconsistencia general función Solicita_estado_envio_correo_conector_ruta " & ex.Message
        End Try
    End Function
    Function Actualiza_estado_envio_correo_conector_ruta(ByVal id_conector As Integer, _
                                                         ByVal estado_envio_correo As Integer) As String
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " UPDATE actividades_disponibles_envio set  Estado_evia_correo=" & estado_envio_correo & _
              "  where id_actividades_disponibles_envio=" & id_conector
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("actividades_disponibles_envio")
            Result = Ref_Car_Conec.SELECTION_INSERT_COMMAND(Parametro_Consulta)
            If Result <> "YES" Then
                Actualiza_estado_envio_correo_conector_ruta = "Función Actualiza_estado_envio_correo_conector_ruta dice " & Result
                Exit Function
            Else
                Actualiza_estado_envio_correo_conector_ruta = Result
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_estado_envio_correo_conector_ruta = "Inconsistencia general función Actualiza_estado_envio_correo_conector_ruta " & ex.Message
        End Try
    End Function
    Function Solicita_configuracion_conector_ruta(ByVal id_registro_actividad_envio As Integer,
                                                  ByRef stru_config_conector_ruta As stru_config_conector_ruta) As String
        Try
            If id_registro_actividad_envio = 0 Then
                id_registro_actividad_envio = "Por favor borre el cache de su navegador, para poder utilizar la opción de envío"
                Exit Function
            End If
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT  Estado_evia_correo,Estado_soicita_autorizacion,Estado_soicita_autorizacion_firma_digital" &
                ",Estado_copia_estructura,Estado_asigna_expediente,Estado_firma_digital,estado_valida_balanceo " &
              " from actividades_disponibles_envio where id_actividades_disponibles_envio= " & id_registro_actividad_envio
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("actividades_disponibles_envio")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                id_registro_actividad_envio = "Función Solicita_configuracion_conector_ruta dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_configuracion_conector_ruta = "Imposible encontrar la configuración del conector de ruta  (" & id_registro_actividad_envio & ")"
                Exit Function
            Else
                stru_config_conector_ruta.Estado_evia_correo = Datset.Tables(0).Rows(0).Item(0)
                stru_config_conector_ruta.Estado_soicita_autorizacion = Datset.Tables(0).Rows(0).Item(1)
                stru_config_conector_ruta.Estado_soicita_autorizacion_firma_digital = Datset.Tables(0).Rows(0).Item(2)
                stru_config_conector_ruta.Estado_copia_documento_estructura = Datset.Tables(0).Rows(0).Item(3)
                stru_config_conector_ruta.Estado_asigna_expediente = Datset.Tables(0).Rows(0).Item(4)
                stru_config_conector_ruta.Estado_firma_digital = Datset.Tables(0).Rows(0).Item(5)
                stru_config_conector_ruta.estado_valida_balanceo = Datset.Tables(0).Rows(0).Item(6)
                Solicita_configuracion_conector_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_configuracion_conector_ruta = "Inconsistencia general función Solicita_configuracion_conector_ruta " & ex.Message
        End Try
    End Function
    Function Actualiza_configuracion_conector_ruta(ByVal id_registro_actividad_envio As Integer,
                                                   ByVal stru_config_conector_ruta As stru_config_conector_ruta) As String
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " UPDATE actividades_disponibles_envio set  Estado_evia_correo=" &
                stru_config_conector_ruta.Estado_evia_correo &
                ",Estado_soicita_autorizacion=" & stru_config_conector_ruta.Estado_soicita_autorizacion &
                ",Estado_soicita_autorizacion_firma_digital=" & stru_config_conector_ruta.Estado_soicita_autorizacion_firma_digital &
                ",Estado_copia_estructura=" & stru_config_conector_ruta.Estado_copia_documento_estructura &
                ",Estado_asigna_expediente=" & stru_config_conector_ruta.Estado_asigna_expediente &
                ",estado_firma_digital=" & stru_config_conector_ruta.Estado_firma_digital &
                ",estado_valida_balanceo=" & stru_config_conector_ruta.estado_valida_balanceo &
              "  where id_actividades_disponibles_envio=" & id_registro_actividad_envio
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("actividades_disponibles_envio")
            Result = Ref_Car_Conec.SELECTION_INSERT_COMMAND(Parametro_Consulta)
            If Result <> "YES" Then
                Actualiza_configuracion_conector_ruta = "Función Actualiza_estado_envio_correo_conector_ruta dice " & Result
                Exit Function
            Else
                Actualiza_configuracion_conector_ruta = Result
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_configuracion_conector_ruta = "Inconsistencia general función Actualiza_configuracion_conector_ruta " & ex.Message
        End Try
    End Function
    Function Solicita_estructura_actividades_envio(ByVal id_registro_actividad_envio As Integer, _
                                                   ByRef stru_actividades_envio As stru_actividades_envio) As String
        Try

            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT  Id_Actividad_Siguiente,Listado_Actividades_Workflow_Id_Actividad,id_Ruta, " &
                "Prioridad_Actividad,Ienti_Grafica_Actividad,id_actividades_disponibles_envio,Estado_evia_correo,Estado_soicita_autorizacion," &
                "Estado_soicita_autorizacion_firma_digital,Estado_copia_estructura,Estado_asigna_expediente" &
              " from actividades_disponibles_envio where id_actividades_disponibles_envio= " & id_registro_actividad_envio
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("actividades_disponibles_envio")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estructura_actividades_envio = "Función Solicita_configuracion_conector_ruta dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estructura_actividades_envio = "Imposible encontrar la configuración del conector de ruta  (" & id_registro_actividad_envio & ")"
                Exit Function
            Else
                stru_actividades_envio.Id_Actividad_Siguiente = Datset.Tables(0).Rows(0).Item(0)
                stru_actividades_envio.Listado_Actividades_Workflow_Id_Actividad = Datset.Tables(0).Rows(0).Item(1)
                stru_actividades_envio.id_Ruta = Datset.Tables(0).Rows(0).Item(2)
                stru_actividades_envio.Prioridad_Actividad = Datset.Tables(0).Rows(0).Item(3)
                stru_actividades_envio.Ienti_Grafica_Actividad = Datset.Tables(0).Rows(0).Item(4)
                stru_actividades_envio.id_actividades_disponibles_envio = Datset.Tables(0).Rows(0).Item(5)
                stru_actividades_envio.Estado_evia_correo = Datset.Tables(0).Rows(0).Item(6)
                stru_actividades_envio.Estado_soicita_autorizacion = Datset.Tables(0).Rows(0).Item(7)
                stru_actividades_envio.Estado_soicita_autorizacion_firma_digital = Datset.Tables(0).Rows(0).Item(8)
                stru_actividades_envio.Estado_copia_documento_estructura = Datset.Tables(0).Rows(0).Item(9)
                stru_actividades_envio.Estado_asigna_expediente = Datset.Tables(0).Rows(0).Item(10)
                Solicita_estructura_actividades_envio = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_actividades_envio = "Inconsistencia general función Solicita_estructura_actividades_envio " & ex.Message
        End Try
    End Function

    Function Solicita_id_conector_actividad(ByVal id_actividad As Integer,
                                            ByRef id_conector As Integer) As String
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT  Id_Actividad_Siguiente " &
              " from actividades_disponibles_envio where Listado_Actividades_Workflow_Id_Actividad= " & id_actividad
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("actividades_disponibles_envio")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_conector_actividad = "Función Solicita_id_conector_actividad dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_conector_actividad = "Imposible encontrar el conector de la actividad  (" & id_actividad & ")"
                Exit Function
            Else
                id_conector = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_conector_actividad = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_conector_actividad = "Inconsistencia general función Solicita_id_conector_actividad " & ex.Message
        End Try
    End Function
    Function Solicita_Listado_actividades_anteriores_ruta(ByVal Id_actividad As Integer,
                                                          ByRef grediview As GridView,
                                                          ByRef reflabel As Label,
                                                          ByRef label_leyend As Label,
                                                          ByRef hideselecion As Object,
                                                          ByVal nombre_ruta As String,
                                                          ByRef update As UpdatePanel,
                                                          ByVal consulta_boot As Integer) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita el listado de actividades anteriores conectadas a una
        'ruta de trabajo
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Id_actividad           : Respresenta la id actividad de destino de las 
        'actividades anteriores
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'grediview              : Retorna el listado de las actividades anteriores
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-04-25
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            label_leyend.Text = "Ruta de trabajo de la tarea (" & nombre_ruta & ")"
            Dim Sql_consulta As String = "select law.id_actividad,ade.id_actividades_disponibles_envio,law.nombre_actividad AS NOMBRE,law.Descripcion_Actividad as DESCRIPCION  from actividades_disponibles_envio  as ade " &
            " inner join listado_actividades_workflow as law on (law.id_actividad=ade.Listado_Actividades_Workflow_Id_Actividad) " &
            " where ade.Id_Actividad_Siguiente=" & Id_actividad
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("grupos_workflow")
            Dim Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_Listado_actividades_anteriores_ruta = "Error Solicita_Listado_actividades_anteriores_ruta  " & Result
                Exit Function
            End If
            Datset.Tables(0).Columns.Add("DESTINO", GetType(String))
            If Datset.Tables(0).Rows.Count = 0 Then
                reflabel.Text = Datset.Tables(0).Rows.Count & " Grupo (s) "
                grediview.DataSource = Nothing
                hideselecion.value = ""
                grediview.DataBind()
                update.Update()
                Solicita_Listado_actividades_anteriores_ruta = "YES"
                Exit Function
            Else
                reflabel.Text = Datset.Tables(0).Rows.Count & " Grupo (s) "
                grediview.DataSource = Datset
                hideselecion.value = ""
                grediview.DataBind()
                update.Update()
                If consulta_boot = 0 Then
                    For i As Integer = 0 To grediview.Rows.Count - 1
                        grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                        Dim imaga_buton As New HtmlInputImage
                        imaga_buton.Attributes.Add("CssClass", "image_buton_clik_image")
                        imaga_buton.Attributes.Add("onclick", "prevent_ruta(event,this)")
                        imaga_buton.Src = "../workflow/imageneswf/share-all-solid.png"
                        imaga_buton.Attributes.Add("title", "Terminar la trea y enviar a la actividad " & grediview.Rows(i).Cells(2).Text)
                        imaga_buton.Attributes.Add("id_tar_sel", grediview.Rows(i).Cells(3).Text.ToString())
                        imaga_buton.Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                        imaga_buton.Attributes.Add("nombre_actividad", grediview.Rows(i).Cells(2).Text.ToString())
                        Dim imaga_buton_imagen As New HtmlInputImage
                        imaga_buton_imagen.Attributes.Add("Class", "image_buton_clik_image_no_alow_cursor")
                        grediview.Rows(i).Cells(2).Attributes.Add("CssClass", "image_buton_clik_image_no_alow_cursor")
                        imaga_buton_imagen.Attributes.Add("onclick", "prevent_blank(event,this);")
                        imaga_buton_imagen.Attributes.Add("height", "20px")
                        imaga_buton_imagen.Src = "../workflow/imageneswf/user-solid.png"
                        Dim imaga_buton_detalle As New HtmlInputImage
                        imaga_buton_detalle.Attributes.Add("CssClass", "image_buton_clik_image")
                        imaga_buton_detalle.Attributes.Add("onclick", "prevent_detalle_actividad_ruta(event,this)")
                        imaga_buton_detalle.Src = "../workflow/imageneswf/detalle.png"
                        imaga_buton_detalle.Attributes.Add("title", "Detalle de la actividad")
                        imaga_buton_detalle.Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                        grediview.Rows(i).Cells(0).Controls.Add(imaga_buton_imagen)
                        grediview.Rows(i).Cells(Datset.Tables(0).Columns.Count - 1).Controls.Add(imaga_buton_detalle)
                        grediview.Rows(i).Cells(Datset.Tables(0).Columns.Count).Controls.Add(imaga_buton)
                    Next
                    Solicita_Listado_actividades_anteriores_ruta = "YES"
                    Exit Function
                Else
                    For i As Integer = 0 To grediview.Rows.Count - 1
                        grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                        Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                        Dim divhtml_ As New HtmlControls.HtmlGenericControl("div")
                        Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                        ihtml.Style.Add("color", "white")
                        ihtml.Attributes.Add("class", "fad fa-share-all")
                        Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                        ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                        ahtml.Attributes.Add("onclick", "prevent_envio_ruta_actividad(event,this);")
                        ahtml.Attributes.Add("title", "Enviar a (" & grediview.Rows(i).Cells(3).Text.ToString() & ")")
                        ahtml.Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                        ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(2).Text.ToString())
                        ahtml.Style.Add("margin-left", "3px")
                        ahtml.Controls.Add(ihtml)
                        divhtml.Controls.Add(ahtml)
                        ihtml = New HtmlControls.HtmlGenericControl("i")
                        ihtml.Style.Add("color", "white")
                        ahtml = New HtmlControls.HtmlGenericControl("a")
                        ahtml.Attributes.Add("Class", "btn bg-info btn-sm")
                        ahtml.Attributes.Add("onclick", "prevent_detalle_actividad(event,this);")
                        ahtml.Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                        ahtml.Style.Add("margin-left", "3px")
                        ihtml.Attributes.Add("class", "fad fa-user-friends")
                        ahtml.Attributes.Add("title", "Actividad de grupo de usuarios")
                        ahtml.Controls.Add(ihtml)
                        divhtml_.Controls.Add(ahtml)
                        divhtml_.Style.Add("display", "inline-flex")
                        divhtml.Style.Add("display", "inline-flex")
                        grediview.Rows(i).Cells(Datset.Tables(0).Columns.Count).Controls.Add(divhtml)
                        grediview.Rows(i).Cells(0).Controls.Add(divhtml_)
                        For z As Integer = 0 To grediview.Rows(i).Cells.Count - 2
                            If z > 0 Then
                                grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                                grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                            End If
                        Next
                    Next
                    Solicita_Listado_actividades_anteriores_ruta = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Solicita_Listado_actividades_anteriores_ruta = "Incosistencia general función Solicita_Listado_actividades_anteriores_ruta " & ex.Message
        End Try
    End Function
End Class
