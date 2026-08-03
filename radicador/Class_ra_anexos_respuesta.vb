Public Structure stru_anexos
    Dim id_anexo_respuesta As Integer
    Dim ID_RESPUESTA_RADICADO As Integer
    Dim id_imagen_gabinete As Long
    Dim nombre_gabinete As String
    Dim nombre_archivo As String
End Structure
Public Class Class_ra_anexos_respuesta
    Function Registra_anexo_respuesta(ByVal id_respuesta As Integer,
                                      ByVal id_imagen_gabinete As Integer,
                                      ByVal nombre_gabinete As String,
                                      ByVal nombre_archivo As String,
                                      ByRef id_anexo As Object) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim sql_insert As String = "Insert into ra_anexos_respuesta (ra_respuesta_radicado_ID_RESPUESTA_RADICADO," &
                                       "id_imagen_gabinete,nombre_gabinete,nombre_archivo) values (" & id_respuesta & "," & id_imagen_gabinete &
                                       ",'" & nombre_gabinete & "','" & nombre_archivo & "')"

            Result = Ref_Car_Conec.SELECTION_LAST_INSERT_COMMAND(sql_insert, id_anexo)
            If Result <> "YES" Then
                Registra_anexo_respuesta = Result
                Exit Function
            Else
                Registra_anexo_respuesta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Registra_anexo_respuesta = "Inconsistencia general función Registra_anexo_respuesta " & ex.Message
        End Try
    End Function
    Function Solicita_lista_documentos_anexos(ByVal id_respuesta As Integer,
                                              ByRef resultList_ As Object)
        Try
            Dim resultList As Gestion_respuesta_enexos() = New Gestion_respuesta_enexos() {}
            Dim res_list = resultList.ToList
            Dim Parametro_Consulta As String = "SELECT id_anexo_respuesta,nombre_archivo" &
                  " FROM  ra_anexos_respuesta " &
                  " where ra_respuesta_radicado_ID_RESPUESTA_RADICADO=" & id_respuesta
            Dim Result As String = ""
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_anexos_respuesta")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_lista_documentos_anexos = "Función Solicita_lista_documentos_anexos dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_lista_documentos_anexos = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    'ReDim Preserve resultList(i)
                    'resultList(i).id_anexo = Datset.Tables(0).Rows(i).Item(0)
                    'resultList(i).nombre_anexo = Datset.Tables(0).Rows(i).Item(1)
                    Dim Gestion_respuesta_enexos_ As Gestion_respuesta_enexos = New Gestion_respuesta_enexos()
                    Gestion_respuesta_enexos_.id_anexo = Datset.Tables(0).Rows(i).Item(0)
                    Gestion_respuesta_enexos_.nombre_anexo = Datset.Tables(0).Rows(i).Item(1)
                    res_list.Add(Gestion_respuesta_enexos_)


                Next
                resultList_ = res_list
                Solicita_lista_documentos_anexos = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_documentos_anexos = "Inconsistencia general funcion Solicita_lista_documentos_anexos " & ex.Message
        End Try
    End Function
    Function Solicita_lista_anexos_respuesta(ByVal id_respuesta As Integer, _
                                             ByRef stru_anexos() As stru_anexos) As String
        Try
            Erase stru_anexos
            Dim Parametro_Consulta As String = "SELECT id_anexo_respuesta," & _
                   " ra_respuesta_radicado_ID_RESPUESTA_RADICADO," & _
                   "id_imagen_gabinete,nombre_gabinete,nombre_archivo" & _
                   " FROM  ra_anexos_respuesta " & _
                   " where ra_respuesta_radicado_ID_RESPUESTA_RADICADO=" & id_respuesta
            Dim Result As String = ""
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_anexos_respuesta")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_lista_anexos_respuesta = "Función Solicita_lista_anexos_respuesta dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_lista_anexos_respuesta = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_anexos(i)
                    stru_anexos(i).id_anexo_respuesta = Datset.Tables(0).Rows(i).Item(0)
                    stru_anexos(i).ID_RESPUESTA_RADICADO = Datset.Tables(0).Rows(i).Item(1)
                    stru_anexos(i).id_imagen_gabinete = Datset.Tables(0).Rows(i).Item(2)
                    stru_anexos(i).nombre_gabinete = Datset.Tables(0).Rows(i).Item(3)
                    stru_anexos(i).nombre_archivo = Datset.Tables(0).Rows(i).Item(4)
                Next
                Solicita_lista_anexos_respuesta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_anexos_respuesta = "Inconsistencia general función Solicita_lista_anexos_respuesta " & ex.Message
        End Try
    End Function
    Function Lista_anexos_respuesta_interface(ByVal stru_anexos() As stru_anexos, _
                                              ByRef drop_lis As DropDownList, _
                                              ByRef update As UpdatePanel, _
                                              ByRef drop_lis_simple As DropDownList, _
                                              ByRef update_simple As UpdatePanel) As String
        Try
            drop_lis.Items.Clear()
            drop_lis_simple.Items.Clear()
            Dim ilist As ListItem
            If Not stru_anexos Is Nothing Then
                For i As Integer = 0 To stru_anexos.Length - 1
                    ilist = New ListItem
                    ilist.Text = stru_anexos(i).nombre_archivo
                    ilist.Value = stru_anexos(i).id_anexo_respuesta
                    drop_lis.Items.Add(ilist)
                    drop_lis_simple.Items.Add(ilist)
                Next
                Lista_anexos_respuesta_interface = "YES"
                Exit Function
            Else
                Lista_anexos_respuesta_interface = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_anexos_respuesta_interface = "Inconistencia general función Lista_anexos_respuesta_interface " & ex.Message
        Finally
            update.Update()
        End Try
    End Function
    Function listar_anexos_respuesta_solicitud(ByVal id_respuesta As Integer, _
                                               ByRef drow_list As DropDownList, _
                                               ByRef update As UpdatePanel, _
                                               ByRef drop_lis_simple As DropDownList, _
                                               ByRef update_simple As UpdatePanel) As String
        Try
            Dim stru_anexo() As stru_anexos = Nothing
            Dim Result As String = ""
            Result = Me.Solicita_lista_anexos_respuesta(id_respuesta, _
                                                        stru_anexo)
            If Result <> "YES" Then
                listar_anexos_respuesta_solicitud = Result
                Exit Function
            End If
            Result = Me.Lista_anexos_respuesta_interface(stru_anexo, _
                                                       drow_list, _
                                                       update, _
                                                       drop_lis_simple, _
                                                       update_simple)
            If Result <> "YES" Then
                listar_anexos_respuesta_solicitud = Result
                Exit Function
            End If
            listar_anexos_respuesta_solicitud = "YES"
            Exit Function
        Catch ex As Exception
            listar_anexos_respuesta_solicitud = "Inconsistencia general función listar_anexos_respuesta_solicitud " & ex.Message
        End Try
    End Function
    Function Solicita_datos_estructura_anexo(ByVal id_anexo As Integer, _
                                             ByRef stru_anexos As stru_anexos) As String
        Try

            Dim Parametro_Consulta As String = "SELECT id_anexo_respuesta," & _
                   " ra_respuesta_radicado_ID_RESPUESTA_RADICADO," & _
                   "id_imagen_gabinete,nombre_gabinete,nombre_archivo" & _
                   " FROM  ra_anexos_respuesta " & _
                   " where id_anexo_respuesta=" & id_anexo
            Dim Result As String = ""
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_anexos_respuesta")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_estructura_anexo = "Función Solicita_datos_estructura_anexo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_estructura_anexo = "Imposible encontrar la estructura del anexo (" & id_anexo & ")"
                Exit Function
            Else
                stru_anexos.id_anexo_respuesta = Datset.Tables(0).Rows(0).Item(0)
                stru_anexos.ID_RESPUESTA_RADICADO = Datset.Tables(0).Rows(0).Item(1)
                stru_anexos.id_imagen_gabinete = Datset.Tables(0).Rows(0).Item(2)
                stru_anexos.nombre_gabinete = Datset.Tables(0).Rows(0).Item(3)
                stru_anexos.nombre_archivo = Datset.Tables(0).Rows(0).Item(4)    
                Solicita_datos_estructura_anexo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_estructura_anexo = "Inconsistencia general función Solicita_datos_estructura_anexo " & ex.Message
        End Try
    End Function
    Function Elimina_anexo_respuesta(ByVal id_anexo As Integer) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim sql_insert As String = "Delete from ra_anexos_respuesta where  id_anexo_respuesta=" & id_anexo
            Result = Ref_Car_Conec.SELECTION_LAST_INSERT_COMMAND(sql_insert, id_anexo)
            If Result <> "YES" Then
                Elimina_anexo_respuesta = Result
                Exit Function
            Else
                Elimina_anexo_respuesta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Elimina_anexo_respuesta = "Inconsistencia genreral función Elimina_anexo_respuesta " & ex.Message
        End Try
    End Function
End Class
