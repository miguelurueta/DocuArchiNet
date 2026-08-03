
Imports Dynamsoft.DotNet.TWAIN.Barcode
Public Class AutoCompleteRequest
    Public Property NameDbsAuto As String
    Public Property NameTableAuto As String
    Public Property NameCampoAuto As String
    Public Property Value As String
    Public Property FechaConsulta As String
    Public Property IdTable As Object
End Class
Public Class AutoResult
    Public Property Result As String
    Public Property Country = New List(Of String)()
End Class
'Representa los atributos de los botones awsome que interactuan en las tablas asp-net
Public Class Class_config_general_service_boton_atributes_awsome
    Public Property Element_i_name_atribute_color_awsome As String      ' Aloja el color del elemento  de la letra o dibujo del I "white"
    Public Property Element_i_name_atribute_fas_awsome As String        ' Aloja el atributo de awsome que determina el grosor de la figura "fas" o "fad"
    Public Property Element_i_name_atribute_boton_awsome As String      ' Aloja el atributo de awsome que determina la figura que montrara el boton "fa-folder-open"
    Public Property Element_i_name_atribute_dimension_awsome As String  ' Aloja el atributo de awsome que determina el tamaño de la figura  "fa-lg"
    Public Property Element_a_name_atribute_color_awsome As String      ' Aloja el atributo de awsome que determina el color del boton   "btn-primary"
    Public Property Element_a_name_atribute_fas_awsome As String        ' Aloja el atributo de awsome que determina el tamaño del boton  "fa-lg"
    Public Property Element_a_name_atribute_boton_awsome As String      ' Aloja el atributo de awsome que determina la figura que montrara el boton "no aplica"
    Public Property Element_a_name_atribute_dimension_awsome As String  ' Aloja el atributo de awsome que determina el tamaño de boton  "btn-sm"
    Public Property Element_a_name_atribute_onclclick_awsome As String  ' Aloja el atributo de awsome el evento del click   onclclick  "onclick" "prevent_lista_tareas(Event,this)"
    Public Property Element_a_name_atribute_title_awsome As String      ' Aloja el titulo del toltip del boton "Ver documentos"
    Public Property Element_a_name_atribute_tip_event_awsome As String  ' Aloja el tipo codigo del evento del boton  "documentos_tarea_list"
    Public Property Element_a_atribute_atributes_boton_awsome() As List(Of Class_config_general_atributes_awsome)  ' Aloja los atributos dinamicos o autonomos del boton "id" ""
    Public Property Element_a_atribute_atributes_row_awsome() As List(Of Class_config_general_atributes_awsome)
End Class
'Representa la descripcion de los atributos dinamicos o autonomos  para los botones awsome
Public Class Class_config_general_atributes_awsome
    Public Property name_atribute As String
    Public Property value_atribute As String
End Class
Public Class class_config_gneral_service_row_option_tom_select
    Property id_value As Integer
    Property tex_value As String
    Property text_value_descritipo As String

End Class
Public Class class_config_gneral_service_row_tom
    Property row_tom As List(Of class_config_gneral_service_row_option_tom_select)
    Property error_gestion As String

End Class
Public Class class_service_operatio_control
    Public Property dms_id_registro As String
    Public Property error_gestion As String
End Class
Public Class class_config_general_parmeter_interface_show
    Property id_registro As Long
    Property class_service As String
    Property name_service As String
    Property name_container As String
    Property name_control_padre As String
    Property name_space_campo As String
    Property asigna_valor As Integer
    Property apost_name_content As String
    Property add_check As Integer
    Property name_table As String
    Property id_script As Integer
    Property name_control_tittle As String
    Property value_control_tittle As String
End Class
Public Class Class_config_general_service
    Public Property Tupcae_label As String            'Aloja si aplica mayuscula al label
    Public Property label_input_class_font As String  'Aloja la clase font white del label
    Public Property Place_Holder As String            'Aloja el valor del palce holder del control IMPUT 
    Public Property control_input_class As String     'Aloja el valor de la clase del control IMPUT 
    Public Property name_campo As String              'Aloja el nombre del campo
    Public Property atrib_campo_n As String           'Aloja el nombre del campo como atributo
    Public Property aleas_campo As String             'Aloja el valor del campo aleas
    Public Property tipo_control As String            'Aloja el tipo de control
    Public Property value_campo As String             'Aloja el campo valor para los objetos tipo seleccion
    Public Property value_campo_old As String         'Aloja el valor anterior de un control para comparar los cambios
    Public Property texto_campo As String             'Aloja el texto mostrado por el control
    Public Property value_campo_beetwen As String     'Aloja el texto mostrado por el segundo control para los campos betwen control
    Public Property texto_campo_beetwen As String     'Aloja el texto mostrado por el segundo control para los campos betwen control
    Public Property campo_beetwen As Integer          'Aloja si el campo es beet de otro campo
    Public Property max_leng_campo As Integer         'Aloja el maximo numero de caracteres de un campo
    Public Property obligatorio_campo As Integer      'Aloja si el campo es obligatorio
    Public Property disable_campo As Integer          'Aloja si el campo esta inactivo o activo
    Public Property name_space_campo As String        'Aloja el espacio de nombre al que pertenece el control
    Public Property alow_null As Integer              'Aloja si el campo utilza tipo null 1 null 0 not null
    Public Property alow_tipo_value As Integer        'Aloja si el campo selecciona el text o el value 0 text 1 value
    Public Property campo_tip As String               'Aloja tipos de campo  (0) option  (1)text    (2)TEXTAREA   (3) DATE COR
    Public Property control_tip_correo As String      'Aloja si el campo es tipo correo
    Public Property tipo_campo As String              'INT LONG VARCHAR DATE TEXT
    Public Property error_gestion As String            'aloja el error del evento
    Public Property dbms_control As String            'Representa el conector dms "WF" "DA" 
    Public Property tbl_control As String             'Representa la tabla del control
    Public Property dms_id_registro As String         'Representa la idenitifcación del registro
    Public Property name_campo_id As String           'Representa el nombre del campo id o condición de la consulta
    Public Property clas_service_control As String    'Representa la clase del servicio
    Public Property service_control As String         'Representa el nombre del servicio o funcion web service
    Public Property result_service_control As String
    Public Property atrib_chek As Integer             'Representa si visualiza atri vhek
    Public Property campo_unico As Integer            'Representa el campo unico
    Public Property valida_capital_text As Integer    'Representa si se aplica capital text
    Public Property name_clase_general As String      'Representa la clase que identifica a todos los controles del formulario
    Public Property name_tab_control As String        'Representa el nombre del contendor del control
    Public Property event_control As List(Of Class_config_general_event_element)
    Public Property ilist_row_drowlist As List(Of Class_service_ilist_drowlist)
    Public Property config_service_drowlis As List(Of Class_config_general_service_drowlist)
    Public Property config_service_drowlis_destino As List(Of Class_config_general_service_drowlist)
    Public Property config_service_boton_atributes_awsome As List(Of Class_config_general_service_boton_atributes_awsome) ' Representa los atributos del botono awsome
    Public Property config_service_controls_error As List(Of class_service_control_error) ' Representa la matriz de cotroles con error
    Public Property drow_name_controls_destino As String 'Aloja el nombre del campo destino donde el control drowp list destino a llenar
    Public Property drow_name_padre_control As String    'Aloja el campo padre del control drowp
    Public Property drow_name_control_id As String       'Aloja el nombre del campo id que guarda la identifcación del indetificador del valor del control
    Public Property title_control As String              'Aloja el titulo del formulario
    Public Property type_cells_alow_control As String    'Aloja el tipo de celda que aloja el control  1- group  2- individual 
    Public Property tooltipAyuda As String               'Aloja el contenido del tooltip de ayuda
    Public Property Tipo_script As String                'Tipo escript para campos relacionados con validacion
    Public Property id_escript As Integer                'Identificación del script
    Public Property name_plantilla_validacion As String  'nombre de las plantillas de radicación
    Public Property id_plantilla As Integer              'Identificación de la plantillas de radicación
    Public Property campo_nombre_plantilla_val As String 'Representa el campo nombre de la plantilla de validacion
    Public Property campo_primary_plantilla_val As String 'Representa el campo primary de la plantilla de validacion
    Public Property url_class_service_control_plantilla As String    'Representa la clase del servicio y url
    Public Property service_control_plantilla As String              'Representa el nombre del servicio o funcion web service
    Public Property Tom_alow As String                    ' Representa el tipo campo  TOMSELCT
    Public Property Tom_option As String                  ' Representa el escape option que se lista en la lista del control TOMSELCT
    Public Property Tom_item As String                    ' Representa el escape option que se mustra cuando se agrega el control
    Public Property Item_Tom_Select As List(Of class_config_gneral_service_row_tom) 'Representa los datos de los campos TOM SELCT
    Public Property Item_Tom_row As List(Of class_config_gneral_service_row_option_tom_select) 'Representa los datos rows de los campos TOM SELCT
    Public Property CamposUpdateIndiceBach As List(Of CDCamposUpdateIndiceBach)

    Public Class CDCamposUpdateIndiceBach
        Property NombreCampo As String
        Property TipoCampo As String
        Property ValorCampo As String
    End Class
    Public Class CDTomParameter
            Public Property NombreCampo As String
            Public Property ValorCampo As String
        End Class
        Public Class class_service_control_error
            Public Property id_name_campo As String
            Public Property name_espace As String
        End Class

        Public Class Class_config_general_event_element
            Public Property name_event_control As String
            Public Property name_function_event_control As Object
        End Class
        Public Class Class_config_general_service_delete
            Public Property name_campo_condicion As String
            Public Property value_campo_condicion As String
        End Class
        Public Class Class_config_general_service_auto_complete
            Public Property name_dbs_auto As String
            Public Property name_table_auto As String
            Public Property name_campo_auto As String
            Public Property value_auto As String
            Public Property clas_service_control_auto As String
            Public Property service_control_auto As String
            Public Property control_destino As String
            Public Property name_plantilla_validacion As String  'nombre de las plantillas de radicación
            Public Property campo_nombre_plantilla_val As String 'Representa el campo nombre de la plantilla de validacion
            Public Property campo_primary_plantilla_val As String 'Representa el campo primary de la plantilla de validacion
            Public Property TomParameter As New List(Of CDTomParameter)
        End Class

        '-------------------Representa la clase de configuración del servicio dorwlis
        Public Class Class_config_general_service_drowlist
            Public Property name_dbs_auto As String            'Representa el conector dms "WF" "DA" 
            Public Property name_table_auto As String          'Rpresenta el nombre de la tabla referencia de la lista de datos del control
            Public Property name_campo_value As String         'Representa el nombre del campo que se lista en la tabla de la lista de datos del control
            Public Property name_campo_condicion As String     'Representa el nombre del campo que condiciona la conulta en la tabla referencia del drowlist
            Public Property name_campo_orden As String         'Representa el campo ordena el listado de la tabla referencia del drowlist
            Public Property tipo_orden As String               'Representa el tipo de orden para la tabla
            Public Property value_condicion As String          'Representa el valor de condición de la tabla
            Public Property value_default As String            'Representa el valor default que se selecciona con el elemento
            Public Property limit_rows As String               'Representa el limite de registro mostrados en un campo
            Public Property name_campo_primary As String       'Representa el nombre del campo primary key de la tabla
            Public Property addd_seleccion As Integer          'Representa el registro del primer campo vacion seleccion del drowlist agrega value=-1 y text=""
            Public Property value_seleccion As String          'Representa el valor seleccionado en el control
            Public Property campo_estado_auto_lista As Integer 'Representa si el control se auto lista desde la tabla cuando se inicia el dibujo de los controles
        End Class
        Public Class Class_service_ilist_drowlist
            Public Property id_value As String
            Public Property value_campo As String
            Public Property error_sistema As String
        End Class

        Function add_control_error_form(ByVal name_control As String,
                                        ByVal name_espace As String,
                                        ByRef class_service_control_error As List(Of class_service_control_error)) As String
            Try
                Dim class_service_control_error_ = New class_service_control_error
                class_service_control_error_.id_name_campo = name_control & "_" & name_espace
                class_service_control_error_.name_espace = name_espace
                class_service_control_error = New List(Of class_service_control_error)
                class_service_control_error.Add(class_service_control_error_)
                add_control_error_form = "YES"
            Catch ex As Exception
                add_control_error_form = "Incosnistencia general funcion add_control_error_form " & ex.Message
            End Try
        End Function
        Function add_event_control_form(ByVal name_event_control As String,
                                        ByVal name_function_event_control As String,
                                        ByRef Class_config_general_event_element As List(Of Class_config_general_event_element)) As String
            Try
                Dim Class_config_general_event_element_ = New Class_config_general_event_element
                Class_config_general_event_element_.name_event_control = name_event_control
                Class_config_general_event_element_.name_function_event_control = name_function_event_control
                Class_config_general_event_element = New List(Of Class_config_general_event_element)
                Class_config_general_event_element.Add(Class_config_general_event_element_)
                add_event_control_form = "YES"
            Catch ex As Exception
                add_event_control_form = ex.Message
            End Try
        End Function
        Function add_campo_form_control(ByVal m_name_campo As String,
                                        ByVal m_aleas_campo As String,
                                        ByVal m_tipo_control As String,
                                        ByVal m_value_campo As String,
                                        ByVal m_texto_campo As String,
                                        ByVal m_max_leng_campo As Integer,
                                        ByVal m_obligatorio_campo As Integer,
                                        ByVal m_name_space_campo As String,
                                        ByVal m_alow_null As Integer,
                                        ByVal m_alow_tipo_value As Integer,
                                        ByRef Class_config_general_service_ As List(Of Class_config_general_service)) As String
            Try

                Dim item As Class_config_general_service = New Class_config_general_service()
                item.name_campo = m_name_campo
                item.aleas_campo = m_aleas_campo
                item.tipo_control = m_tipo_control
                item.value_campo = m_value_campo
                item.texto_campo = m_texto_campo
                item.max_leng_campo = m_max_leng_campo
                item.obligatorio_campo = m_obligatorio_campo
                item.name_space_campo = m_name_campo
                item.alow_null = m_alow_null
                item.alow_tipo_value = m_alow_tipo_value
                item.campo_tip = ""
                Class_config_general_service_.Add(item)
                add_campo_form_control = "YES"
            Catch ex As Exception
                add_campo_form_control = "Inconsistencia general funcion add_campo_form_control " & ex.Message
            End Try
        End Function
        Function Solicita_index_campo_form_control(ByVal nombre_campo As String,
                                                   ByVal Class_config_general_service_ As List(Of Class_config_general_service),
                                                   ByRef index_campo As Integer) As String
            Try
                index_campo = -1
                For i As Integer = 0 To Class_config_general_service_.Count - 1
                    If UCase(Class_config_general_service_(i).name_campo) = UCase(nombre_campo) Then
                        index_campo = i
                        Exit For
                    End If
                Next
                Solicita_index_campo_form_control = "YES"
            Catch ex As Exception
                Solicita_index_campo_form_control = "Inconsistencia geeneral funcion Solicita_index_campo_form_control " & ex.Message
            End Try
        End Function
        Function Solicita_valor_campo_index_form_control(ByVal index_campo As String,
                                                         ByVal Class_config_general_service_ As List(Of Class_config_general_service),
                                                         ByRef valor_campo As String) As String
            Try
                valor_campo = Class_config_general_service_(index_campo).value_campo
                Solicita_valor_campo_index_form_control = "YES"
            Catch ex As Exception
                Solicita_valor_campo_index_form_control = "Inconsistencia geeneral funcion Solicita_index_campo_form_control " & ex.Message
            End Try
        End Function
        Function Create_insert_form_control(ByVal name_table As String,
                                            ByVal Class_config_general_service As List(Of Class_config_general_service),
                                            ByRef sql_insert As String) As String
            Try
                Dim sql_campos As String = ""
                Dim sql_insert_campos As String = ""
                Dim sql_table_insert As String = "insert into " & name_table & "  "
                Dim valor_campo As String = ""
                If Class_config_general_service Is Nothing Then
                    Create_insert_form_control = "La matriz de contorles esta vacia"
                    Exit Function
                End If
                For i As Integer = 0 To Class_config_general_service.Count - 1
                    If sql_campos = "" Then
                        sql_campos = "(" & Class_config_general_service(i).name_campo
                    Else
                        sql_campos = sql_campos & "," & Class_config_general_service(i).name_campo
                    End If
                    If Class_config_general_service(i).alow_tipo_value = 0 Then
                        valor_campo = Class_config_general_service(i).texto_campo
                    Else
                        valor_campo = Class_config_general_service(i).value_campo
                    End If
                    If Class_config_general_service(i).alow_null = 1 And valor_campo = "" Then
                        valor_campo = "Null"
                    Else
                        valor_campo = "'" & valor_campo & "'"
                    End If
                    If sql_insert_campos = "" Then
                        sql_insert_campos = "(" & valor_campo
                    Else
                        sql_insert_campos = sql_insert_campos & "," & valor_campo
                    End If
                Next
                sql_insert_campos = sql_insert_campos & ")"
                sql_campos = sql_campos & ")"
                sql_insert = sql_table_insert & sql_campos & " values " & sql_insert_campos
                Create_insert_form_control = "YES"
            Catch ex As Exception
                Create_insert_form_control = "Inconsistencia general funcion Create_insert_form_control " & ex.Message
            End Try
        End Function
        Function Create_update_form_control_auto(ByVal Class_config_general_service As List(Of Class_config_general_service),
                                                 ByRef sql_update As String) As String
            '---------------------------------------------------------------------------
            'Funcion : Crea comando sql para actualizacion autonomo
            '---------------------------------------------------------------------------
            '                           PARAMETROS  
            '---------------------------------------------------------------------------
            'Class_config_general_service  : Representa la estructura de los campos
            '---------------------------------------------------------------------------
            '                           RETORNO
            '---------------------------------------------------------------------------
            'sql_update            : Retorna el comando de actualizacion
            '---------------------------------------------------------------------------
            '                         CARACTERIZACIÓN
            '---------------------------------------------------------------------------
            'Fecha                 : 2023-04-01
            'Elabora               : Miguel Angel Urueta Miranda
            '----------------------------------------------------------------------------
            Try
                Dim campo_condicion As String = ""
                Dim sql_update_campos As String = ""
                Dim name_table As String = ""
                If Class_config_general_service(0).tbl_control = "" Then
                    Create_update_form_control_auto = "Imposible encontrar el nombre de la tabla "
                    Exit Function
                Else
                    name_table = Class_config_general_service(0).tbl_control
                End If
                If Class_config_general_service(0).name_campo_id = "" Then
                    Create_update_form_control_auto = "Imposible encontrar el campo condicion "
                    Exit Function
                Else
                    campo_condicion = Class_config_general_service(0).name_campo_id
                End If
                Dim valor_condicion As String = ""
                If Class_config_general_service(0).dms_id_registro = "" Then
                    Create_update_form_control_auto = "Imposible encontrar el valor condicion "
                    Exit Function
                Else
                    valor_condicion = Class_config_general_service(0).dms_id_registro
                End If
                Dim sql_table_update As String = "update " & name_table & "  "
                For i As Integer = 0 To Class_config_general_service.Count - 1
                    If Class_config_general_service(i).value_campo_old <> Class_config_general_service(i).texto_campo Then
                        Dim name_campo As String = Class_config_general_service(i).name_campo
                        Dim name_campo_value As String = Class_config_general_service(i).drow_name_control_id
                        Dim valor_campo_value As String = ""
                        Dim valor_campo As String = ""
                        '--------Asigna datos para campos
                        Select Case Class_config_general_service(i).campo_tip
                            Case 0  ' (1) SELECCION
                                valor_campo = Class_config_general_service(i).texto_campo
                                If name_campo_value <> "" Then
                                    valor_campo_value = Class_config_general_service(i).value_campo
                                End If
                            Case 1  '(1)text
                                valor_campo = Class_config_general_service(i).texto_campo
                            Case 2  '(2)TEXTAREA
                                valor_campo = Class_config_general_service(i).texto_campo
                            Case 3  '(3) DATE
                                valor_campo = Class_config_general_service(i).texto_campo
                        End Select
                        If Class_config_general_service(i).alow_null = 1 And valor_campo = "" Then
                            valor_campo = "Null"
                        Else
                            valor_campo = "'" & valor_campo & "'"
                        End If
                        '------Asigna valor a campo value
                        If name_campo_value <> "" Then
                            If Class_config_general_service(i).alow_null = 1 And valor_campo_value = "" Then
                                valor_campo_value = "Null"
                            Else
                                valor_campo_value = "'" & valor_campo_value & "'"
                            End If
                        End If
                        '-------Construye valor update
                        Dim set_update As String = name_campo & "=" & valor_campo
                        If name_campo_value <> "" Then
                            set_update = set_update & "," & name_campo_value & "=" & valor_campo_value
                        End If
                        If sql_update_campos = "" Then
                            sql_update_campos = " set " & set_update
                        Else
                            sql_update_campos = sql_update_campos & "," & set_update
                        End If
                    End If
                Next
                If sql_update_campos = "" Then
                    sql_update = ""
                Else
                    sql_update = sql_table_update & sql_update_campos & " where " & campo_condicion & "='" & valor_condicion & "'"
                End If
                Create_update_form_control_auto = "YES"
                Exit Function
            Catch ex As Exception
                Create_update_form_control_auto = "Inconstencia general funcion Create_update_form_control_auto " & ex.Message
            End Try
        End Function
        Function Create_update_form_control(ByVal name_table As String,
                                            ByVal Class_config_general_service As List(Of Class_config_general_service),
                                            ByVal campo_condicion As String,
                                            ByVal valor_condicion As String,
                                            ByRef sql_update As String) As String
            '---------------------------------------------------------------------------
            'Funcion : Crea comando sql para actualizacion 
            '---------------------------------------------------------------------------
            '                           PARAMETROS  
            '---------------------------------------------------------------------------
            'name_table            : Representa el nombre de la tabla
            'Class_config_general_service  : Representa la estructura de los campos
            'campo_condicion       : Represnta el nombre campo condicion
            'valor_condicion       : Representa el valor de la condición
            '---------------------------------------------------------------------------
            '                           RETORNO
            '---------------------------------------------------------------------------
            'sql_update            : Retorna el comando de actualizacion
            '---------------------------------------------------------------------------
            '                         CARACTERIZACIÓN
            '---------------------------------------------------------------------------
            'Fecha                 : 2023-04-01
            'Elabora               : Miguel Angel Urueta Miranda
            '----------------------------------------------------------------------------
            Try

                Dim sql_update_campos As String = ""
                Dim sql_table_update As String = "update " & name_table & "  "
                Dim valor_campo As String = ""
                If Class_config_general_service Is Nothing Then
                    Create_update_form_control = "La matriz de contorles esta vacia"
                    Exit Function
                End If
                For i As Integer = 0 To Class_config_general_service.Count - 1
                    Dim name_campo As String = Class_config_general_service(i).name_campo
                    If Class_config_general_service(i).alow_tipo_value = 0 Then
                        valor_campo = Class_config_general_service(i).texto_campo
                    Else
                        valor_campo = Class_config_general_service(i).value_campo
                    End If
                    If Class_config_general_service(i).alow_null = 1 And valor_campo = "" Then
                        valor_campo = "Null"
                    Else
                        valor_campo = "'" & valor_campo & "'"
                    End If
                    If sql_update_campos = "" Then
                        sql_update_campos = " set " & name_campo & "=" & valor_campo
                    Else
                        sql_update_campos = sql_update_campos & "," & name_campo & "=" & valor_campo
                    End If
                Next
                sql_update = sql_table_update & sql_update_campos & " where " & campo_condicion & "='" & valor_condicion & "'"
                Create_update_form_control = "YES"
            Catch ex As Exception
                Create_update_form_control = "Inconsistencia general funcion Create_update_form_control " & ex.Message
            End Try
        End Function
        Function Create_delete_form_control(ByVal name_table As String,
                                            ByVal Class_config_general_service_delete As List(Of Class_config_general_service_delete),
                                            ByRef sql_delete As String) As String
            Try
                Dim sql_condicion_campos As String = ""
                Dim sql_delte_table As String = "Delete from " & name_table & "  where "
                Dim valor_campo As String = ""
                If Class_config_general_service_delete Is Nothing Then
                    Create_delete_form_control = "La matriz de contorles esta vacia"
                    Exit Function
                End If
                For i As Integer = 0 To Class_config_general_service_delete.Count - 1
                    If sql_condicion_campos = "" Then
                        sql_condicion_campos = Class_config_general_service_delete(i).name_campo_condicion & "='" & Class_config_general_service_delete(i).value_campo_condicion & "'"
                    Else
                        sql_condicion_campos = sql_condicion_campos & " and " & Class_config_general_service_delete(i).name_campo_condicion & "='" & Class_config_general_service_delete(i).value_campo_condicion & "'"
                    End If
                Next
                sql_delete = sql_delte_table & sql_condicion_campos
                Create_delete_form_control = "YES"
                Exit Function
            Catch ex As Exception
                Create_delete_form_control = "Inconsistencia general función Create_delete_form_control " & ex.Message
            End Try
        End Function
        Function Solicita_datos_auto_complete_campos_form_control(ByVal name_dbs_auto As String,
                                                                  ByVal name_table_auto As String,
                                                                  ByVal name_campo_auto As String,
                                                                  ByVal value_auto As String,
                                                                  ByRef country As List(Of String)) As String
            Try
                Dim ref As Object
                Dim Result As String = ""
                Dim Sql_consulta As String = "Select DISTINCT (" & name_campo_auto & ") from " & name_table_auto & " where " & name_campo_auto & " like '%" & value_auto & "%' LIMIT 50"
                If name_dbs_auto = "WF" Then
                    ref = New conect.Dbase_Conction_Mysql
                Else
                    ref = New conect.Dbase_Conction_Mysql_RA
                End If
                Dim Datset As DataSet = New DataSet("DAT_ADIC")
                Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
                If Result <> "YES" Then
                    Solicita_datos_auto_complete_campos_form_control = Result
                    Exit Function
                End If
                If Datset.Tables(0).Rows.Count = 0 Then
                    country = Nothing
                    Solicita_datos_auto_complete_campos_form_control = "YES"
                    Exit Function
                Else
                    For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                        If Datset.Tables(0).Rows(i).IsNull(0) = False Then
                            Dim obsgetipe As Object = Datset.Tables(0).Rows(i).Item(0).GetType.ToString
                            If obsgetipe = "System.DateTime" Then
                                Dim subtrin As String = Datset.Tables(0).Rows(i).Item(0).ToString()
                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                                country.Add(splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0))
                            Else
                                country.Add(Datset.Tables(0).Rows(i).Item(0).ToString())
                            End If
                        End If
                    Next
                    Solicita_datos_auto_complete_campos_form_control = "YES"
                    Exit Function
                End If
            Catch ex As Exception
                Solicita_datos_auto_complete_campos_form_control = "Inconsistencia general funcion Solicita_datos_auto_complete_campos_form_control " & ex.Message
            End Try
        End Function
        Function Solicita_datos_drowlist_form_control(ByVal Class_config_general_service As Class_config_general_service,
                                                      ByRef Class_service_ilist_drowlist As List(Of Class_config_general_service.Class_service_ilist_drowlist)) As String
            '---------------------------------------------------------------------------
            'Funcion : Solicita lista de datos para el control drowlist con asignacion
            '---------------------------------------------------------------------------
            '                           PARAMETROS  
            '---------------------------------------------------------------------------
            'name_dbs_auto      : nombre del dms que realiza la consulta
            'name_table_auto    : nombre de la tabla de consulta
            'name_campo_value   : nombre del campo que lista el valor para el usuario
            'name_campo_primary : nombre del campo primary oculto del control
            'name_campo_condicion : Nombre del campo condicionante de la consulta
            'value_condicion    : Valor condicionante de la consulta
            'limit_rows         : Numero de record de la lista
            'name_campo_orden   : Name campo que ordena la lista
            'tipo_orden         : Tipo orden de lista
            'value_default      : Valor default de seleccion
            '---------------------------------------------------------------------------
            '                           RETORNO
            '---------------------------------------------------------------------------
            '
            '---------------------------------------------------------------------------
            '                         CARACTERIZACIÓN
            '---------------------------------------------------------------------------
            'Fecha                 : 2024-01-04
            'Elabora               : Miguel Angel Urueta Miranda
            '----------------------------------------------------------------------------
            Try

                Dim ref As Object
                Dim Result As String = ""
                Dim sql_condicion As String = ""
                Dim Class_config_general_service_drowlist As List(Of Class_config_general_service.Class_config_general_service_drowlist)
                Class_config_general_service_drowlist = Class_config_general_service.config_service_drowlis_destino
                If Class_config_general_service_drowlist(0).name_campo_condicion <> "" And Class_config_general_service_drowlist(0).value_condicion <> "" Then
                    sql_condicion = " where " & Class_config_general_service_drowlist(0).name_campo_condicion & " ='" &
                    Class_config_general_service_drowlist(0).value_condicion & "'"
                End If
                Dim Sql_consulta As String = "Select " & Class_config_general_service_drowlist(0).name_campo_primary & "," & Class_config_general_service_drowlist(0).name_campo_value & " from " &
                    Class_config_general_service_drowlist(0).name_table_auto & sql_condicion &
                    " order by " & Class_config_general_service_drowlist(0).name_campo_orden & " " & Class_config_general_service_drowlist(0).tipo_orden & " LIMIT " &
                    Class_config_general_service_drowlist(0).limit_rows
                If Class_config_general_service_drowlist(0).name_dbs_auto = "WF" Then
                    ref = New conect.Dbase_Conction_Mysql
                Else
                    ref = New conect.Dbase_Conction_Mysql_RA
                End If
                Dim Datset As DataSet = New DataSet("DAT_ADIC")
                Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
                If Result <> "YES" Then
                    Solicita_datos_drowlist_form_control = " function Solicita_datos_drowlist_form_control dice " & Result
                    Exit Function
                End If
                If Datset.Tables(0).Rows.Count = 0 Then
                    Class_service_ilist_drowlist = Nothing
                    Solicita_datos_drowlist_form_control = "YES"
                    Exit Function
                Else
                    '----Agrega campo vacio seleccion para capturar cambio
                    If Class_config_general_service_drowlist(0).addd_seleccion = 1 Then
                        Dim parameter_gestion As Class_config_general_service.Class_service_ilist_drowlist = New Class_config_general_service.Class_service_ilist_drowlist()
                        parameter_gestion.id_value = 0
                        parameter_gestion.value_campo = Class_config_general_service_drowlist(0).value_seleccion
                        Class_service_ilist_drowlist.Add(parameter_gestion)
                    End If
                    For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                        Dim parameter_gestion As Class_config_general_service.Class_service_ilist_drowlist = New Class_config_general_service.Class_service_ilist_drowlist()
                        parameter_gestion.id_value = Datset.Tables(0).Rows(i).Item(0)
                        If Datset.Tables(0).Rows(i).IsNull(1) = True Then
                            parameter_gestion.value_campo = ""
                        Else
                            Dim obsgetipe As Object = Datset.Tables(0).Rows(i).Item(1).GetType.ToString
                            If obsgetipe = "System.DateTime" Then
                                Dim subtrin As String = Datset.Tables(0).Rows(i).Item(0).ToString()
                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                                parameter_gestion.value_campo = splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0)
                            Else
                                parameter_gestion.value_campo = Datset.Tables(0).Rows(i).Item(1)
                            End If
                        End If
                        Class_service_ilist_drowlist.Add(parameter_gestion)
                    Next
                    '-----------------------------asigna el valor defautl------------------------------
                    For i As Integer = 0 To Class_service_ilist_drowlist.Count - 1
                        If Class_service_ilist_drowlist.Item(i).value_campo = Class_config_general_service.texto_campo Then
                            Class_config_general_service_drowlist(0).value_condicion = Class_service_ilist_drowlist.Item(i).id_value
                        End If
                    Next
                    Solicita_datos_drowlist_form_control = "YES"
                    Exit Function
                End If
            Catch ex As Exception
                Solicita_datos_drowlist_form_control = "Inconsistencia general funcion Solicita_datos_drowlist_form_control_plantilla " & ex.Message
            End Try
        End Function
        Function Solicita_datos_drowlist_form_control(ByVal Class_config_general_service_drowlist As List(Of Class_config_general_service_drowlist),
                                                      ByRef Class_service_ilist_drowlist As List(Of Class_service_ilist_drowlist)) As String
            '---------------------------------------------------------------------------
            'Funcion : Solicita lista de datos para el control drowlist sin asignacion
            '---------------------------------------------------------------------------
            '                           PARAMETROS  
            '---------------------------------------------------------------------------
            'name_dbs_auto      : nombre del dms que realiza la consulta
            'name_table_auto    : nombre de la tabla de consulta
            'name_campo_value   : nombre del campo que lista el valor para el usuario
            'name_campo_primary : nombre del campo primary oculto del control
            'name_campo_condicion : Nombre del campo condicionante de la consulta
            'value_condicion    : Valor condicionante de la consulta
            'limit_rows         : Numero de record de la lista
            'name_campo_orden   : Name campo que ordena la lista
            'tipo_orden         : Tipo orden de lista
            'value_default      : Valor default de seleccion
            '---------------------------------------------------------------------------
            '                           RETORNO
            '---------------------------------------------------------------------------
            'id_usuario_radicador  : Retorna la idnetificación del usuario radicador
            '---------------------------------------------------------------------------
            '                         CARACTERIZACIÓN
            '---------------------------------------------------------------------------
            'Fecha                 : 2023-10-27
            'Elabora               : Miguel Angel Urueta Miranda
            '----------------------------------------------------------------------------
            Try

                Dim ref As Object
                Dim Result As String = ""
                Dim sql_condicion As String = ""
                If Class_config_general_service_drowlist(0).name_campo_condicion <> "" And Class_config_general_service_drowlist(0).value_condicion <> "" Then
                    sql_condicion = " where " & Class_config_general_service_drowlist(0).name_campo_condicion & " ='" &
                    Class_config_general_service_drowlist(0).value_condicion & "'"
                End If
                Dim Sql_consulta As String = "Select " & Class_config_general_service_drowlist(0).name_campo_primary & "," & Class_config_general_service_drowlist(0).name_campo_value & " from " &
                    Class_config_general_service_drowlist(0).name_table_auto & sql_condicion &
                    " order by " & Class_config_general_service_drowlist(0).name_campo_orden & " " & Class_config_general_service_drowlist(0).tipo_orden & " LIMIT " &
                    Class_config_general_service_drowlist(0).limit_rows
                If Class_config_general_service_drowlist(0).name_dbs_auto = "WF" Then
                    ref = New conect.Dbase_Conction_Mysql
                Else
                    ref = New conect.Dbase_Conction_Mysql_RA
                End If
                Dim Datset As DataSet = New DataSet("DAT_ADIC")
                Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
                If Result <> "YES" Then
                    Solicita_datos_drowlist_form_control = " function Solicita_datos_drowlist_form_control dice " & Result
                    Exit Function
                End If
                If Datset.Tables(0).Rows.Count = 0 Then
                    Class_service_ilist_drowlist = Nothing
                    Solicita_datos_drowlist_form_control = "YES"
                    Exit Function
                Else
                    '----Agrega campo vacio seleccion para capturar cambio
                    If Class_config_general_service_drowlist(0).addd_seleccion = 1 Then
                        Dim parameter_gestion As Class_service_ilist_drowlist = New Class_service_ilist_drowlist()
                        parameter_gestion.id_value = 0
                        parameter_gestion.value_campo = Class_config_general_service_drowlist(0).value_seleccion
                        Class_service_ilist_drowlist.Add(parameter_gestion)
                    End If
                    For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                        Dim parameter_gestion As Class_service_ilist_drowlist = New Class_service_ilist_drowlist()
                        parameter_gestion.id_value = Datset.Tables(0).Rows(i).Item(0)
                        If Datset.Tables(0).Rows(i).IsNull(1) = True Then
                            parameter_gestion.value_campo = ""
                        Else
                            Dim obsgetipe As Object = Datset.Tables(0).Rows(i).Item(1).GetType.ToString
                            If obsgetipe = "System.DateTime" Then
                                Dim subtrin As String = Datset.Tables(0).Rows(i).Item(0).ToString()
                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                                parameter_gestion.value_campo = splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0)
                            Else
                                parameter_gestion.value_campo = Datset.Tables(0).Rows(i).Item(1)
                            End If
                        End If
                        Class_service_ilist_drowlist.Add(parameter_gestion)
                    Next
                    Solicita_datos_drowlist_form_control = "YES"
                    Exit Function
                End If
            Catch ex As Exception
                Solicita_datos_drowlist_form_control = "Inconsistencia general funcion Solicita_datos_drowlist_form_control " & ex.Message
            End Try
        End Function
    End Class

