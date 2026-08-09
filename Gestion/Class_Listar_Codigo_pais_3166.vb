Public Class Class_Listar_Codigo_pais_3166
    Function Listar_Codigo_pais_3166(ByRef Combo As DropDownList) As String
        '******************************************************
        'Funcion : Lista codigos de pais en base a la iso 3166
        'Fecha : 2017-01-13
        'Igeniero: Miguel Angel Urueta Miranda
        '******************************************************
        Try
            Combo.Items.Clear()
            Dim Parametro_Consulta As String = "select  NOMBRE_CODIGO " & _
                  " from ra_de_codigo_iso_3166 "
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("ra_de_codigo_iso_3166")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Listar_Codigo_pais_3166 = " Imposible conectar la base de datos del gestor documental " & Result
                Combo.Items.Clear()
                'update.Update()
                Exit Function
            End If
            Dim Paswuser As String = ""
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    Combo.Items.Add(Dat_reader.Tables(0).Rows(i).Item(0).ToString)
                Next
                'update.Update()
                Listar_Codigo_pais_3166 = "YES"
            Else
                Combo.Items.Clear()
                'update.Update()
                Listar_Codigo_pais_3166 = "YES"
            End If

        Catch ex As Exception
            Listar_Codigo_pais_3166 = "Inconsistencia General Funcion Listar_Codigo_pais_3166 " & ex.Message
        End Try
    End Function
    Function Listar_Codigo_pais_3166_seleccion(ByRef Combo As DropDownList, ByVal nombre_seleccion As String) As String
        '******************************************************
        'Funcion : Lista codigos de pais en base a la iso 3166
        'Fecha : 2017-01-13
        'Igeniero: Miguel Angel Urueta Miranda
        '******************************************************
        Try
            Combo.Items.Clear()
            Dim Parametro_Consulta As String = "select  NOMBRE_CODIGO " & _
                  " from ra_de_codigo_iso_3166 "

            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("ra_de_codigo_iso_3166")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Listar_Codigo_pais_3166_seleccion = "Función Listar_Codigo_pais_3166_seleccion Imposible conectar la base de datos del gestor documental " & Result
                Combo.Items.Clear()
                'update.Update()
                Exit Function
            End If
            Dim Paswuser As String = ""
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    Combo.Items.Add(Dat_reader.Tables(0).Rows(i).Item(0).ToString)
                Next
                'update.Update()
                For i As Integer = 0 To Combo.Items.Count - 1
                    If Combo.Items(i).Value = nombre_seleccion Then
                        Combo.Text = nombre_seleccion
                        Exit For
                    End If
                Next
                Listar_Codigo_pais_3166_seleccion = "YES"
            Else
                Combo.Items.Clear()
                'update.Update()
                Listar_Codigo_pais_3166_seleccion = "YES"
            End If

        Catch ex As Exception
            Listar_Codigo_pais_3166_seleccion = "Inconsistencia General Funcion Listar_Codigo_pais_3166_seleccion " & ex.Message
        End Try
    End Function
End Class
