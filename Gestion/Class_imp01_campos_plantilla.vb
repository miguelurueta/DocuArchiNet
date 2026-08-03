Public Class class_stru_Row_plantilla_Generic
    Public Error_result As String
    Public Obj_ilist_row_generic As Object     'Seralizado DATA-SET
    Public Obj_ilist_fileds_generic As Object  'class_campos_table_bostra_table
End Class
Public Class class_row_rue_sii
    Property CODIGOBARRAS As String
    Property RECIBO As String
    Property NOMBRE As String
    Property SERVICIO As String
    Property CODIGOSERVCIORUE As String
    Property ESTADO As String
End Class
Public Class class_row_virtual_sii
    Property CODIGOBARRAS As String
    Property NOMBRE As String
    Property ESTADO As String
    Property RECIBO As String
End Class
Public Class Class_imp01_campos_plantilla
    Function Solicita_estructura_campos_dynamic_polantilla_externa_rue_SII(ByVal id_plantilla As Integer,
                                                                           ByRef class_campos_table_bostra_table As List(Of class_campos_table_bostra_table)) As String
        '--------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de campos de una plantilla externa para la carga
        '          de formatos de excel para la plantilla de rues ccv
        '        
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'id_plantilla                : Representa la identificación de la plantilla
        '                             
        '
        '
        '
        '---------------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------------
        'class_campos_table_bostra_table :Representa la estructura de campos  
        '---------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------------
        'Fecha                 : 2024-12-11
        'Modifica              : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------------
        Try
            class_campos_table_bostra_table = New List(Of class_campos_table_bostra_table)
            Dim Sql_consulta = "SELECT CAMPO_DESTINO,CAMPO_FUENTE FROM " &
           "imp01_campos_plantilla " &
           "WHERE IMP01_PLANTILLAIMP_ID_PLANTILLAIMP=" & id_plantilla
            Dim ref2 As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("imp01_campos_plantilla")
            Dim Resulta As String = ref2.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Resulta <> "YES" Then
                Solicita_estructura_campos_dynamic_polantilla_externa_rue_SII = "Funcion  Solicita_estructura_campos_dynamic_polantilla_externa : (" & Resulta & ")"
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estructura_campos_dynamic_polantilla_externa_rue_SII = "Imposible encontrar los campos para plantilla : (" & id_plantilla & ")"
                Exit Function
            Else
                Dim item As New class_campos_table_bostra_table
                item = New class_campos_table_bostra_table
                item.field = "state"
                item.checkbox = True
                item.visible = True
                item.viisble_sql = 0
                item.visible_like_sql = 0
                item.title = "Seleccione el registro"
                class_campos_table_bostra_table.Add(item)
                item = New class_campos_table_bostra_table
                item.field = "operate"
                item.title = "OPERATION"
                item.checkbox = False
                item.visible = True
                item.viisble_sql = 1
                item.clickToSelect = False
                item.visible_like_sql = 1
                item.align = "center"
                item.events = "window.operateEvents"
                item.formatter = "operateFormatter_ruesii"
                class_campos_table_bostra_table.Add(item)
                For y As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    item = New class_campos_table_bostra_table
                    item.title = Datset.Tables(0).Rows(y).Item(0)
                    item.field = Datset.Tables(0).Rows(y).Item(0)
                    item.field_destino = Datset.Tables(0).Rows(y).Item(1)
                    item.visible = True
                    item.viisble_sql = 1
                    item.visible_like_sql = 1
                    class_campos_table_bostra_table.Add(item)
                Next
                Solicita_estructura_campos_dynamic_polantilla_externa_rue_SII = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_campos_dynamic_polantilla_externa_rue_SII = "Inconsistencia general funcion Solicita_estructura_campos_dynamic_polantilla_externa_rue_SII " & ex.Message
        End Try
    End Function
    Function Solicita_estructura_campos_dynamic_polantilla_externa_virtual_SII(ByVal id_plantilla As Integer,
                                                                               ByRef class_campos_table_bostra_table As List(Of class_campos_table_bostra_table)) As String
        '--------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de campos de una plantilla externa para la carga
        '          de formatos de excel para la plantilla de tramites virtuales
        '        
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'id_plantilla                : Representa la identificación de la plantilla
        '                             
        '
        '
        '
        '---------------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------------
        'class_campos_table_bostra_table :Representa la estructura de campos  
        '---------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------------
        'Fecha                 : 2025-01-02
        'Modifica              : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------------
        Try
            class_campos_table_bostra_table = New List(Of class_campos_table_bostra_table)
            Dim Sql_consulta = "SELECT CAMPO_DESTINO,CAMPO_FUENTE FROM " &
           "imp01_campos_plantilla " &
           "WHERE IMP01_PLANTILLAIMP_ID_PLANTILLAIMP=" & id_plantilla
            Dim ref2 As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("imp01_campos_plantilla")
            Dim Resulta As String = ref2.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Resulta <> "YES" Then
                Solicita_estructura_campos_dynamic_polantilla_externa_virtual_SII = "Funcion  Solicita_estructura_campos_dynamic_polantilla_externa_virtual_SII : (" & Resulta & ")"
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estructura_campos_dynamic_polantilla_externa_virtual_SII = "Imposible encontrar los campos para plantilla : (" & id_plantilla & ")"
                Exit Function
            Else
                Dim item As New class_campos_table_bostra_table
                item = New class_campos_table_bostra_table
                item.field = "state"
                item.checkbox = True
                item.visible = True
                item.viisble_sql = 0
                item.visible_like_sql = 0
                item.title = "Seleccione el registro"
                class_campos_table_bostra_table.Add(item)
                item = New class_campos_table_bostra_table
                item.field = "operate"
                item.title = "OPERATION"
                item.checkbox = False
                item.visible = True
                item.viisble_sql = 1
                item.clickToSelect = False
                item.visible_like_sql = 1
                item.align = "center"
                item.events = "window.operateEvents"
                item.formatter = "operateFormatter_virtualsii"
                class_campos_table_bostra_table.Add(item)
                For y As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    item = New class_campos_table_bostra_table
                    item.title = Datset.Tables(0).Rows(y).Item(0)
                    item.field = Datset.Tables(0).Rows(y).Item(0)
                    item.field_destino = Datset.Tables(0).Rows(y).Item(1)
                    item.visible = True
                    item.viisble_sql = 1
                    item.visible_like_sql = 1
                    class_campos_table_bostra_table.Add(item)
                Next
                Solicita_estructura_campos_dynamic_polantilla_externa_virtual_SII = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_campos_dynamic_polantilla_externa_virtual_SII = "Inconsistencia general funcion Solicita_estructura_campos_dynamic_polantilla_externa_virtual_SII " & ex.Message
        End Try
    End Function
End Class
