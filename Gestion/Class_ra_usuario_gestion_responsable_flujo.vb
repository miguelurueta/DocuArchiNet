Public Class Class_ra_usuario_gestion_responsable_flujo
    Function Solicita_lista_usuario_responsable_flujo(ByVal id_flujo_trabajo As Integer, _
                                                      ByRef drop_list As DropDownList, _
                                                      ByRef up_date As UpdatePanel) As String
        Try
            drop_list.Items.Clear()
            Dim Parametro_Consulta As String = "select  id_usu_responsable_flujo,rdi.Nombre_Remitente from ra_usuario_gestion_responsable_flujo rugr " & _
                "inner join remit_dest_interno as rdi on (rdi.id_Remit_Dest_Int=rugr.id_Remit_Dest_Int) " & _
                " where  rugr.id_flujo_trabajo ='" & id_flujo_trabajo & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_usuario_gestion_responsable_flujo")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_lista_usuario_responsable_flujo = "Error función Solicita_lista_usuario_responsable_flujo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then

                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim ilist As New ListItem
                    ilist.Value = Datset.Tables(0).Rows(i).Item(0)
                    ilist.Text = Datset.Tables(0).Rows(i).Item(1)
                    drop_list.Items.Add(ilist)
                Next

                Solicita_lista_usuario_responsable_flujo = "YES"
                Exit Function
            Else
                Solicita_lista_usuario_responsable_flujo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_usuario_responsable_flujo = "Inconsistencia general función Solicita_lista_usuario_responsable_flujo " & ex.Message
        Finally
            up_date.Update()
        End Try
    End Function
    Function Solicita_usuario_responsable_flujo(ByVal id_flujo As Integer, _
                                                ByRef id_usuario_responsable As Integer) As String
        Try
            Dim Parametro_Consulta As String = "select  id_Remit_Dest_Int from ra_usuario_gestion_responsable_flujo  " & _
               " where  id_flujo_trabajo =" & id_flujo
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_usuario_gestion_responsable_flujo")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_usuario_responsable_flujo = "Error función Solicita_usuario_responsable_flujo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_usuario_responsable = Datset.Tables(0).Rows(0).Item(0)
                Solicita_usuario_responsable_flujo = "YES"
                Exit Function
            Else
                id_usuario_responsable = 0
                Solicita_usuario_responsable_flujo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_usuario_responsable_flujo = "Inconsistencia general función Solicita_usuario_responsable_flujo " & ex.Message
        End Try
    End Function
    Function Registra_usuario_responsable_flujo(ByVal id_usuario_gestion As Integer, _
                                                ByVal id_area_departamento As Integer, _
                                                ByVal id_flujo As Integer, _
                                                ByVal nombre_usuario As String, _
                                                ByRef drowp As DropDownList, _
                                                ByRef update As UpdatePanel) As String
        Try
            If id_area_departamento = 0 Then
                Registra_usuario_responsable_flujo = "Debe seleccionar el area o departamento del usuario responsable"
                Exit Function
            End If
            If id_usuario_gestion = 0 Then
                Registra_usuario_responsable_flujo = "Debe seleccionar el usuario responsable"
                Exit Function
            End If
            If id_flujo = 0 Then
                Registra_usuario_responsable_flujo = "Debe seleccionar el flujo de trabajo"
                Exit Function
            End If
            Dim Parametro_insert As String = "INSERT INTO  ra_usuario_gestion_responsable_flujo   " & _
              " (id_flujo_trabajo,codigo_area,id_Remit_Dest_Int) VALUES (" & id_flujo & "," & id_area_departamento & "," & id_usuario_gestion & ")"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim ob_insert As Object = Nothing
            Result = ref.SELECTION_LAST_INSERT_COMMAND(Parametro_insert, _
                                                       ob_insert)
            If Result <> "YES" Then
                Registra_usuario_responsable_flujo = Result
                Exit Function
            End If
            Dim item_ As New ListItem
            item_.Value = ob_insert
            item_.Text = nombre_usuario
            drowp.Items.Add(item_)
            Registra_usuario_responsable_flujo = "YES"
            Exit Function
        Catch ex As Exception
            Registra_usuario_responsable_flujo = "Inconsistencia general función Registra_usuario_responsable_flujo " & ex.Message
        Finally
            update.Update()
        End Try
    End Function
    Function Eliminar_usuario_responsable_flujo(ByVal id_registro_usuario_responsable As Integer, _
                                                ByRef drowp As DropDownList, _
                                                ByRef update As UpdatePanel) As String
        Try
            If id_registro_usuario_responsable = 0 Then
                Eliminar_usuario_responsable_flujo = "Debe seleccionar el registro de usuario a eliminar"
                Exit Function
            End If
            Dim Parametro_insert As String = "DELETE  FROM ra_usuario_gestion_responsable_flujo  WHERE id_usu_responsable_flujo=" & _
                                              id_registro_usuario_responsable
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim ob_insert As Object = Nothing
            Result = ref.SELECTION_LAST_INSERT_COMMAND(Parametro_insert, _
                                                       ob_insert)
            If Result <> "YES" Then
                Eliminar_usuario_responsable_flujo = Result
                Exit Function
            Else
                drowp.Items.RemoveAt(drowp.SelectedIndex)
            End If
            Eliminar_usuario_responsable_flujo = "YES"
        Catch ex As Exception
            Eliminar_usuario_responsable_flujo = "Inconsistencia general función Eliminar_usuario_responsable_flujo " & ex.Message
        Finally
            update.Update()
        End Try
    End Function
    
End Class
