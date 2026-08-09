Public Class Classactualizacionvisor
    
    Public Function Obtener_Id_Imagen_Enlasadas(ByVal Conectio As String, ByVal Nombre_Tabla As String, _
   ByVal Id_Enlase As String, ByRef Datos_Imagen() As String, ByVal Id_Imagen As String) As String
        Try
            Dim I As Integer = 0
            Dim Sql_consulta = ""

            Sql_consulta = "SELECT ID,DISC,PAG,IDEX,ENLASE,DBT " & _
            " FROM " & Nombre_Tabla & _
            " WHERE ENLASE='" & Id_Enlase & "' AND ID <> " & Id_Imagen
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DA_EXTENSION")
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Erase Datos_Imagen
                Obtener_Id_Imagen_Enlasadas = "Error Verifique haber digitado campo enlase en el gabinete Consultando en tabla GABIENETE " & " " & Nombre_Tabla & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Erase Datos_Imagen
                Obtener_Id_Imagen_Enlasadas = "YES"
                Exit Function
            Else

                For i2 As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    '---Recorrido de columnas
                    ReDim Preserve Datos_Imagen(i2)
                    For z As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                        Dim Tempvalor As Object = Datset.Tables(0).Rows(i2).Item(z)
                        If z = 5 Then
                            If IsDBNull(Tempvalor) Then
                                Datos_Imagen(i2) = Datos_Imagen(i2) & "0"
                            Else
                                Datos_Imagen(i2) = Datos_Imagen(i2) & Datset.Tables(0).Rows(i2).Item(z)
                            End If
                        Else
                            If IsDBNull(Tempvalor) Then
                                Datos_Imagen(i2) = Datos_Imagen(i2) & "0" & "|"
                            Else
                                Datos_Imagen(i2) = Datos_Imagen(i2) & Datset.Tables(0).Rows(i2).Item(z) & "|"
                            End If
                        End If
                        
                    Next
                Next
            End If
            'If Dat_reader32 Is Nothing Then
            '    Ref_Car_Conec32.CONEXION_MYSQL_C.Close()
            '    Ref_Car_Conec32 = Nothing
            '    Erase Datos_Imagen
            '    Obtener_Id_Imagen_Enlasadas = "Error Verifique haber digitado campo enlase en el gabinete Consultando en tabla GABIENETE " & " " & Nombre_Tabla & Sql_consulta
            '    Exit Function
            'End If
            'If Dat_reader32.HasRows = False Then
            '    Dat_reader32.Close()
            '    Ref_Car_Conec32.CONEXION_MYSQL_C.Close()
            '    Ref_Car_Conec32 = Nothing
            '    Erase Datos_Imagen
            '    Obtener_Id_Imagen_Enlasadas = "YES"
            '    Exit Function
            'Else

            '    While Dat_reader32.Read
            '        ReDim Preserve Datos_Imagen(I)
            '        If Dat_reader32.IsDBNull(0) = True Then
            '            Datos_Imagen(I) = Datos_Imagen(I) & Dat_reader32(0).ToString & "|"
            '        Else
            '            Datos_Imagen(I) = Datos_Imagen(I) & Dat_reader32(0).ToString & "|"
            '        End If
            '        If Dat_reader32.IsDBNull(1) = True Then
            '            Datos_Imagen(I) = Datos_Imagen(I) & Dat_reader32(1).ToString & "|"
            '        Else
            '            Datos_Imagen(I) = Datos_Imagen(I) & Dat_reader32(1).ToString & "|"
            '        End If
            '        If Dat_reader32.IsDBNull(2) = True Then
            '            Datos_Imagen(I) = Datos_Imagen(I) & Dat_reader32(2).ToString & "|"
            '        Else
            '            Datos_Imagen(I) = Datos_Imagen(I) & Dat_reader32(2).ToString & "|"
            '        End If
            '        If Dat_reader32.IsDBNull(3) = True Then
            '            Datos_Imagen(I) = Datos_Imagen(I) & Dat_reader32(3).ToString & "|"
            '        Else
            '            Datos_Imagen(I) = Datos_Imagen(I) & Dat_reader32(3).ToString & "|"
            '        End If
            '        If Dat_reader32.IsDBNull(5) = True Then
            '            Datos_Imagen(I) = Datos_Imagen(I) & Dat_reader32(5).ToString
            '        Else
            '            Datos_Imagen(I) = Datos_Imagen(I) & Dat_reader32(5).ToString
            '        End If
            '        I = I + 1
            '    End While


            '    Dat_reader32.Close()
            '    Ref_Car_Conec32.CONEXION_MYSQL_C.Close()
            '    Obtener_Id_Imagen_Enlasadas = "YES"
            'End If
            Obtener_Id_Imagen_Enlasadas = "YES"
        Catch ex As Exception
            Obtener_Id_Imagen_Enlasadas = "Error Consultando TABLA  de  Gabinete Verifique haber digitado campo enlase" & ex.Message
        End Try
    End Function
End Class
