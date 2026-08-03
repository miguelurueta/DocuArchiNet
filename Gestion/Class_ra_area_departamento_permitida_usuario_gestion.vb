Public Class Class_ra_area_departamento_permitida_usuario_gestion
    Function lista_areas_permitidas_usuario_gestion_organigrama_default_items(ByVal id_usuario_gestion As Integer, _
                                                                              ByVal id_organigrama As Integer, _
                                                                              ByVal id_area As Integer, _
                                                                              ByRef refcombo As DropDownList) As String
        '************************************************************
        'Funcion : Función lista las areas de gestion permitidas
        'para el usuario y asigna el area a la que pertenece la unidad
        'Fecha : 2018-08-29
        'Ingeniero : Miguel Angel Urueta Miranda
        '************************************************************
        Try
            refcombo.Items.Clear()
            Dim Parametro_Consulta As String = "Select adr.Codigo_Area,adr.Nombre_Area from ra_area_departamento_permitida_usuario_gestion as rdp " & _
            " inner join areas_depart_radicacion as adr on (adr.Codigo_Area=rdp.AREA_ARCHIVO_ID_AREA and adr.Registro_Organigrama_Id_Organigrama=" & id_organigrama & ") " & _
            " where remit_dest_interno_id_Remit_Dest_Int=" & id_usuario_gestion
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("ra_area_departamento_permitida_usuario_gestion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                lista_areas_permitidas_usuario_gestion_organigrama_default_items = "Función lista_areas_permitidas_usuario_gestion_organigrama_default_items  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Dim ilist As New ListItem
                ilist.Text = ""
                ilist.Value = 0
                refcombo.Items.Add(ilist)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilist = New ListItem
                    ilist.Text = Datset.Tables(0).Rows(i).Item(1)
                    ilist.Value = Datset.Tables(0).Rows(i).Item(0)
                    refcombo.Items.Add(ilist)
                Next
                For i As Integer = 0 To refcombo.Items.Count - 1
                    If refcombo.Items(i).Value = id_area Then
                        refcombo.Items(i).Selected = True
                        Exit For
                    End If
                Next
                lista_areas_permitidas_usuario_gestion_organigrama_default_items = "YES"
                Exit Function
            Else
                lista_areas_permitidas_usuario_gestion_organigrama_default_items = "YES"
                Exit Function
            End If
        Catch ex As Exception
            lista_areas_permitidas_usuario_gestion_organigrama_default_items = "Inconsistencia General Funcion lista_areas_permitidas_usuario_gestion_organigrama_default_items  : " & ex.Message
        End Try
    End Function
End Class
