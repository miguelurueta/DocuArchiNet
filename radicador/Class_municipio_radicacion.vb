Imports GestionDocumental_Docuarchi.net.WebServiceRadicacion

Public Class Class_municipio_radicacion
    Function Lista_municipios_departamento(ByVal id_departamento As Integer,
                                           ByVal id_municipio As Integer,
                                           ByVal drowp_list As DropDownList,
                                           ByRef update As UpdatePanel) As String
        Try
            drowp_list.Items.Clear()
            Dim Parametro_Consulta As String = "select id_Municipio_Radicacion,Nombre_Municipio from municipio_radicacion " &
            " where Depart_Radicacion_Id_Depart_Radicacion = " & id_departamento
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("depart_radicacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_municipios_departamento = " Función Lista_municipios_departamento dice " & Result
                Exit Function
            End If
            Dim ilist As ListItem
            ilist = New ListItem
            ilist.Text = ""
            ilist.Value = 0
            drowp_list.Items.Add(ilist)
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilist = New ListItem
                    ilist.Text = Datset.Tables(0).Rows(i).Item(1)
                    ilist.Value = Datset.Tables(0).Rows(i).Item(0)
                    drowp_list.Items.Add(ilist)
                Next
                If id_departamento <> 0 Then
                    For i As Integer = 0 To drowp_list.Items.Count - 1
                        If id_municipio = drowp_list.Items(i).Value Then
                            drowp_list.Items(i).Selected = True
                        End If
                    Next
                End If
                Lista_municipios_departamento = "YES"
                Exit Function
            Else
                Lista_municipios_departamento = "YES"
                Exit Function
            End If
            Lista_municipios_departamento = "YES"
        Catch ex As Exception
            Lista_municipios_departamento = "Inconsistencia general función Lista_municipios_departamento " & ex.Message
        Finally
            update.Update()
        End Try

    End Function
    Function Service_lista_municipio_departamento(ByVal id_departamento As Integer,
                                                  ByRef rad_drow_lista_ As List(Of rad_drow_lista)) As String
        Try

            Dim Parametro_Consulta As String = "select id_Municipio_Radicacion,Nombre_Municipio from municipio_radicacion " &
            " where Depart_Radicacion_Id_Depart_Radicacion = " & id_departamento
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("municipio_radicacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Service_lista_municipio_departamento = " Función Service_lista_departamento_Paises dice " & Result
                Exit Function
            End If
            Dim item As rad_drow_lista
            If Datset.Tables(0).Rows.Count > 0 Then
                item = New rad_drow_lista
                item.value = "0"
                item.text = ""
                rad_drow_lista_.Add(item)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    item = New rad_drow_lista
                    item.value = Datset.Tables(0).Rows(i).Item(0)
                    item.text = Datset.Tables(0).Rows(i).Item(1)
                    rad_drow_lista_.Add(item)
                Next
                Service_lista_municipio_departamento = "YES"
                Exit Function
            Else
                Service_lista_municipio_departamento = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Service_lista_municipio_departamento = "Inconsistencia general función Service_lista_municipio_departamento " & ex.Message

        End Try

    End Function
    Function solicita_id_municipio_ciudad_nombre(ByVal nombre As String,
                                                 ByRef id As Integer) As String
        Try
            Dim Parametro_Consulta As String = "select id_Municipio_Radicacion from municipio_radicacion " &
            " where Nombre_Municipio ='" & nombre & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("depart_radicacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                solicita_id_municipio_ciudad_nombre = "Función solicita_id_municipio_ciudad_nombre dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id = Datset.Tables(0).Rows(0).Item(0)
                solicita_id_municipio_ciudad_nombre = "YES"
                Exit Function
            Else
                solicita_id_municipio_ciudad_nombre = "Imposible encontrar la identificación del municipio/ciudad por el siguiente nombre (" & nombre & ")"
                Exit Function
            End If
        Catch ex As Exception
            solicita_id_municipio_ciudad_nombre = "Inconsistencia general función solicita_id_municipio_ciudad_nombre " & ex.Message
        End Try
    End Function
End Class
