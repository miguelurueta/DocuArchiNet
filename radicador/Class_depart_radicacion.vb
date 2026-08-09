Imports GestionDocumental_Docuarchi.net.WebServiceRadicacion

Public Class Class_depart_radicacion
    Function Lista_departamento_Paises(ByVal id_pais As Integer,
                                      ByVal id_departamento As Integer,
                                      ByVal drowp_list As DropDownList,
                                      ByRef update As UpdatePanel) As String
        Try
            drowp_list.Items.Clear()
            Dim Parametro_Consulta As String = "select Id_Depart_Radicacion,Nombre_Departamento from depart_radicacion " &
            " where pais_radicacion_id_pais_radicacion = " & id_pais & " and Estado_Departamento=1"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("depart_radicacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_departamento_Paises = " Función Lista_departamento_Paises dice " & Result
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
                        If id_departamento = drowp_list.Items(i).Value Then
                            drowp_list.Items(i).Selected = True
                        End If
                    Next
                End If
                Lista_departamento_Paises = "YES"
                Exit Function
            Else
                Lista_departamento_Paises = "YES"
                Exit Function
            End If
            Lista_departamento_Paises = "YES"
        Catch ex As Exception
            Lista_departamento_Paises = "Inconsistencia general función Lista_departamento_Paises " & ex.Message
        Finally
            update.Update()
        End Try

    End Function
    Function solicita_id_departamento_nombre(ByVal nombre As String,
                                            ByRef id As Integer) As String
        Try
            Dim Parametro_Consulta As String = "select Id_Depart_Radicacion from depart_radicacion " &
            " where Nombre_Departamento ='" & nombre & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("depart_radicacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                solicita_id_departamento_nombre = " Función solicita_id_departamento_nombre dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id = Datset.Tables(0).Rows(0).Item(0)
                solicita_id_departamento_nombre = "YES"
                Exit Function
            Else
                solicita_id_departamento_nombre = "Imposible encontrar la identificación del departamento por el siguiente nombre (" & nombre & ")"
                Exit Function
            End If
        Catch ex As Exception
            solicita_id_departamento_nombre = "Inconsistencia general función solicita_id_departamento_nombre " & ex.Message
        End Try
    End Function
    Function Service_lista_departamento_Paises(ByVal id_pais As Integer,
                                               ByRef rad_drow_lista_ As List(Of rad_drow_lista)) As String
        Try

            Dim Parametro_Consulta As String = "select Id_Depart_Radicacion,Nombre_Departamento from depart_radicacion " &
            " where pais_radicacion_id_pais_radicacion = " & id_pais & " and Estado_Departamento=1"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("depart_radicacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Service_lista_departamento_Paises = " Función Service_lista_departamento_Paises dice " & Result
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
                Service_lista_departamento_Paises = "YES"
                Exit Function
            Else
                Service_lista_departamento_Paises = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Service_lista_departamento_Paises = "Inconsistencia general función Service_lista_departamento_Paises " & ex.Message
        End Try

    End Function
End Class
