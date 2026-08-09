
Public Class class_ra_itex_config_stamp
    Public Property id_user_config As Integer
    Public Property itex_value_stamp_transparent As Integer
    Public Property itex_aplicate_transparent_stamp As Integer
    Public Property item_itext As List(Of class_control_drow_lista_itex)
    Public Property error_sistema As String
End Class
Public Class class_control_drow_lista_itex
    Public Property value As String
    Public Property text As String
End Class
Public Class Class_ra_config_itex_stamp
    Function Solicita_datos_configuracion_itex_stamp_user(ByVal id_usuario As Integer,
                                                          ByRef class_ra_itex_config_stamp As List(Of class_ra_itex_config_stamp)) As String
        Try
            Dim parameter_gestion As class_ra_itex_config_stamp = New class_ra_itex_config_stamp()
            Dim Result As String = ""
            Dim ref = New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_config_itex_stamp")
            Dim Sql_consulta As String = "select remit_dest_interno_id_Remit_Dest_Int,itex_value_stamp_transparent,itex_aplicate_transparent_stamp " &
                " from ra_config_itex_stamp where remit_dest_interno_id_Remit_Dest_Int=" & id_usuario
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_configuracion_itex_stamp_user = " function Solicita_datos_configuracion_itex_stamp_user dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                parameter_gestion.id_user_config = id_usuario
                parameter_gestion.itex_aplicate_transparent_stamp = 0
                parameter_gestion.itex_value_stamp_transparent = 10
                parameter_gestion.item_itext = New List(Of class_control_drow_lista_itex)
                Solicita_valores_item_stamp(parameter_gestion.item_itext)
                parameter_gestion.error_sistema = "YES"
                class_ra_itex_config_stamp.Add(parameter_gestion)
                Solicita_datos_configuracion_itex_stamp_user = "YES"
            Else
                parameter_gestion.id_user_config = Datset.Tables(0).Rows(0).Item(0)
                parameter_gestion.itex_aplicate_transparent_stamp = Datset.Tables(0).Rows(0).Item(2)
                parameter_gestion.itex_value_stamp_transparent = Datset.Tables(0).Rows(0).Item(1)
                parameter_gestion.item_itext = New List(Of class_control_drow_lista_itex)
                Solicita_valores_item_stamp(parameter_gestion.item_itext)
                parameter_gestion.error_sistema = "YES"
                class_ra_itex_config_stamp.Add(parameter_gestion)
                Solicita_datos_configuracion_itex_stamp_user = "YES"
            End If
        Catch ex As Exception
            Solicita_datos_configuracion_itex_stamp_user = "Inconsistencia general funcion Solicita_datos_configuracion_itex_stamp_user " & ex.Message
        End Try
    End Function
    Function Solicita_valores_item_stamp(ByRef class_control_drow_lista_itex As List(Of class_control_drow_lista_itex)) As String
        Try
            For i As Integer = 5 To 200 Step 5
                Dim _class_control_drow_lista_itex As class_control_drow_lista_itex = New class_control_drow_lista_itex
                _class_control_drow_lista_itex.value = i
                _class_control_drow_lista_itex.text = i
                class_control_drow_lista_itex.Add(_class_control_drow_lista_itex)
            Next
            Solicita_valores_item_stamp = "YES"
        Catch ex As Exception
            Solicita_valores_item_stamp = "Inconsistencia general funcion Solicita_valores_item_stamp " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_datos_configuracion_itex_stamp_user(ByVal id_usuario As Integer,
                                                                     ByRef estado_existencia As String) As String
        Try
            Dim parameter_gestion As class_ra_itex_config_stamp = New class_ra_itex_config_stamp()
            Dim Result As String = ""
            Dim ref = New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_config_itex_stamp")
            Dim Sql_consulta As String = "select remit_dest_interno_id_Remit_Dest_Int " &
                " from ra_config_itex_stamp where remit_dest_interno_id_Remit_Dest_Int=" & id_usuario
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_datos_configuracion_itex_stamp_user = " function Solicita_existencia_datos_configuracion_itex_stamp_user dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_existencia = "NO"
                Solicita_existencia_datos_configuracion_itex_stamp_user = "YES"
            Else
                estado_existencia = "YES"
                Solicita_existencia_datos_configuracion_itex_stamp_user = "YES"
            End If
        Catch ex As Exception
            Solicita_existencia_datos_configuracion_itex_stamp_user = "Inconsistencia general funcion Solicita_existencia_datos_configuracion_itex_stamp_user " & ex.Message
        End Try
    End Function
    Function Insert_datos_configuracion_itex_stamp_user(ByVal id_usuario As Integer,
                                                        ByRef class_ra_itex_config_stamp As class_ra_itex_config_stamp) As String
        Try
            Dim Result As String = ""
            Dim ref = New conect.Dbase_Conction_Mysql_DA
            Dim sql_insert As String = "insert into ra_config_itex_stamp (remit_dest_interno_id_Remit_Dest_Int,itex_value_stamp_transparent,itex_aplicate_transparent_stamp) values " &
                " (" & id_usuario & "," & class_ra_itex_config_stamp.itex_value_stamp_transparent & "," & class_ra_itex_config_stamp.itex_aplicate_transparent_stamp & ")"
            Result = ref.SELECTION_INSERT_COMMAND(sql_insert)
            Insert_datos_configuracion_itex_stamp_user = Result
        Catch ex As Exception
            Insert_datos_configuracion_itex_stamp_user = "Iconsistencia general funcion Insert_datos_configuracion_itex_stamp_user " & ex.Message
        End Try
    End Function
    Function Update_datos_configuracion_itex_stamp_user(ByVal id_usuario As Integer,
                                                        ByRef class_ra_itex_config_stamp As class_ra_itex_config_stamp) As String
        Try
            Dim Result As String = ""
            Dim ref = New conect.Dbase_Conction_Mysql_DA
            Dim update_insert As String = "update ra_config_itex_stamp set itex_value_stamp_transparent=" & class_ra_itex_config_stamp.itex_value_stamp_transparent &
            ",itex_aplicate_transparent_stamp=" & class_ra_itex_config_stamp.itex_aplicate_transparent_stamp & " where remit_dest_interno_id_Remit_Dest_Int=" & id_usuario
            Result = ref.SELECTION_INSERT_COMMAND(update_insert)
            Update_datos_configuracion_itex_stamp_user = Result
        Catch ex As Exception
            Update_datos_configuracion_itex_stamp_user = "Iconsistencia general funcion Update_datos_configuracion_itex_stamp_user " & ex.Message
        End Try
    End Function
End Class

