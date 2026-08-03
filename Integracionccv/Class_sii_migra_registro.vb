Public Class Class_sii_migra_registro
    Function Solicita_existencia_registro_codigo_sii(ByVal codigo_sii As String, _
                                                     ByRef estado_existencia As String) As String
        Try
            Dim Parametro_Consulta As String = " SELECT  id_migra_registro " & _
            " from sii_migra_registro where  codigo_sii='" & codigo_sii & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("sii_migra_registro")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_registro_codigo_sii = "Función Solicita_existencia_registro_codigo_sii dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_existencia = "NO"
                Solicita_existencia_registro_codigo_sii = "YES"
                Exit Function
            Else
                estado_existencia = "YES"
                Solicita_existencia_registro_codigo_sii = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_registro_codigo_sii = "Inconsistencia general funcion Solicita_existencia_registro_codigo_sii " & ex.Message
        End Try
    End Function
    Function Solicita_lista_registro_sii_migrados(ByVal fecha_ini As String, _
                                                  ByVal fecha_fin As String, _
                                                  ByVal codigo_sii As String, _
                                                  ByRef country As Object) As String
        Try
            Dim parameter As String = ""
            If fecha_ini <> "" And fecha_fin <> "" Then
                parameter = " where  CAST(fecha_migracion AS DATE)  between '" & fecha_ini & "' and '" & fecha_fin & "'"
            Else
                If fecha_ini <> "" Then
                    parameter = " where CAST(fecha_migracion AS DATE)  = '" & fecha_ini & "'"
                End If
                If fecha_fin <> "" Then
                    parameter = " where CAST(fecha_migracion AS DATE)  = '" & fecha_fin & "'"
                End If
            End If
            If codigo_sii <> "" Then
                If parameter <> "" Then
                    parameter = " and codigo_sii  = '" & codigo_sii & "'"
                Else
                    parameter = " where codigo_sii  = '" & codigo_sii & "'"
                End If
            End If
            Dim Parametro_Consulta As String = " SELECT  id_migra_registro,codigo_sii,fecha_migracion,usuario_migracion," & _
                "numero_registro_matri_imagenes as imagenes,matricula,nit_identificacion,recibo_sii " & _
            " from sii_migra_registro  " & parameter
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("sii_migra_registro")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_lista_registro_sii_migrados = "Función Solicita_lista_registro_sii_migrados dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim item As New WebService_integracion_sii.aray_item_registro
                    item.id_migra_registro = Datset.Tables(0).Rows(i).Item(0)
                    item.codigo_sii = Datset.Tables(0).Rows(i).Item(1)
                    item.fecha_migracion = Datset.Tables(0).Rows(i).Item(2)
                    item.usuario_migracion = Datset.Tables(0).Rows(i).Item(3)
                    item.imagenes = Datset.Tables(0).Rows(i).Item(4)
                    item.nit_identificacion = Datset.Tables(0).Rows(i).Item(5)
                    item.matricula = Datset.Tables(0).Rows(i).Item(6)
                    item.recibo_sii = Datset.Tables(0).Rows(i).Item(7)
                    country.Add(item)
                    
                Next
                Solicita_lista_registro_sii_migrados = "YES"
                Exit Function
            Else
                Dim item As New WebService_integracion_sii.aray_item_registro
                country.Add(item)
                Solicita_lista_registro_sii_migrados = "Imposible encontrar registros"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_registro_sii_migrados = "Inconsistencia general funcion Solicita_lista_registro_sii_migrados " & ex.Message
        End Try
    End Function
End Class
