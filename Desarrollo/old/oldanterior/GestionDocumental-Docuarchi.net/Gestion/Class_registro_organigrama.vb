Public Class Class_registro_organigrama
    
    Function Retorna_id_organigrama_usuario_gestion(ByVal id_usuario_gestion As Integer, _
                                                    ByRef id_organigrama As Integer) As String
        '-----------------------------------------------------------------
        'Funcion : Retorna organigrama id usuario de gestion 
        'Fecha : 2016-04-16
        'Ing : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "Select  ro.ID_ORGANIGRAMA from  remit_dest_interno as rdi" & _
                       " inner join registro_organigrama as ro on (ro.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=rdi.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA)" & _
                       "where id_Remit_Dest_Int='" & id_usuario_gestion & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("remit_dest_interno")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_organigrama_usuario_gestion = "Función Retorna_id_organigrama_usuario_gestion  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_organigrama = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_organigrama_usuario_gestion = "YES"
                Exit Function
            Else
                Retorna_id_organigrama_usuario_gestion = "Imposible encontrar la identificacion del organigrama función Retorna_id_organigrama_usuario_gestion"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_organigrama_usuario_gestion = "Inconsistencia General Funcion Retorna_id_organigrama_usuario_gestion : " & ex.Message
        End Try
    End Function

    'Function Retorna_Id_Organigrama_activo_empresa(ByVal id_empresa As Integer, _
    '                                               ByVal Nombre_Organigrama As String, _
    '                                               ByRef id_organigrama As Integer) As String
    '    '************************************************************
    '    'Funcion : Retorna id organigrama de la empresa activa
    '    'Fecha : 2013-11-20
    '    'Ingeniero : Miguel Angel Urueta Miranda
    '    '************************************************************
    '    Try
    '        Dim Parametro_Consulta As String = "Select  ID_ORGANIGRAMA from  registro_organigrama " & _
    '               "where NOMBRE_ORGANIGRAMA='" & Nombre_Organigrama & "' AND ESTADO_ORGANIGRAMA=1 " & _
    '               " and EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA='" & id_empresa & "'"
    '        Dim ref As New conect.Dbase_Conction_Mysql
    '        Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
    '        Dim Result As String = ""
    '        Dim Datset As DataSet = New DataSet("remit_dest_interno")
    '        Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
    '        If Result <> "YES" Then
    '            Retorna_Id_Organigrama_activo_empresa = "Función Retorna_Id_Organigrama_activo_empresa  Imposible conectar la base de datos del gestor documental " & Result
    '            Exit Function
    '        End If
    '        If Datset.Tables(0).Rows.Count > 0 Then
    '            id_organigrama = Datset.Tables(0).Rows(0).Item(0)
    '            Retorna_Id_Organigrama_activo_empresa = "YES"
    '        Else
    '            Retorna_Id_Organigrama_activo_empresa = "Imposible encontrar la identificacion del organigrama función Retorna_Id_Organigrama_activo_empresa"
    '        End If

    '    Catch ex As Exception
    '        Retorna_Id_Organigrama_activo_empresa = "Inconsistencia General Funcion Retorna_Id_Organigrama_activo_empresa  : " & ex.Message
    '    End Try
    'End Function
    Function Retorna_Id_Organigrama_activo_empresa(ByVal id_empresa As Integer, _
                                                   ByRef id_organigrama As Integer) As String
        '************************************************************
        'Funcion : Retorna id organigrama de la empresa activa
        'Fecha : 2013-11-20
        'Ingeniero : Miguel Angel Urueta Miranda
        '************************************************************
        Try
            Dim Parametro_Consulta As String = "Select  ID_ORGANIGRAMA from  registro_organigrama " & _
                   "where ESTADO_ORGANIGRAMA=1 " & _
                   " and EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA='" & id_empresa & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("remit_dest_interno")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_Id_Organigrama_activo_empresa = "Función Retorna_Id_Organigrama_activo_empresa  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_organigrama = Datset.Tables(0).Rows(0).Item(0)
                Retorna_Id_Organigrama_activo_empresa = "YES"
            Else
                Retorna_Id_Organigrama_activo_empresa = "Imposible encontrar la identificacion del organigrama función Retorna_Id_Organigrama_activo_empresa"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_Id_Organigrama_activo_empresa = "Inconsistencia General Funcion Retorna_Id_Organigrama_activo_empresa  : " & ex.Message
        End Try
    End Function
    Function Retorna_id_organigrama(ByVal nombre_organigrama As String, _
                                    ByVal id_empresa As Integer, _
                                    ByRef id_organigrama As Integer) As String
        '************************************************************
        'Funcion : Retorna id organigrama de la empresa activa
        'Fecha : 2013-11-20
        'Ingeniero : Miguel Angel Urueta Miranda
        '************************************************************
        Try
            Dim Parametro_Consulta As String = "Select  ID_ORGANIGRAMA from  registro_organigrama " & _
                   "where NOMBRE_ORGANIGRAMA='" & nombre_organigrama & "'" & _
                   " and EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=" & id_empresa
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("remit_dest_interno")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_organigrama = "Función Retorna_id_organigrama  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_organigrama = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_organigrama = "YES"
                Exit Function
            Else
                Retorna_id_organigrama = "Imposible encontrar la identificacion del organigrama (" & nombre_organigrama & ")"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_id_organigrama = "Inconsistencia General Funcion Retorna_id_organigrama  : " & ex.Message
        End Try
    End Function
    Function Solicita_datos_caracterizacion_organigrama_activo(ByVal id_empresa_usuario_gestion As Integer, _
                                                               ByRef id_organigrama As Integer, _
                                                               ByRef nombre_organigrama As String) As String
        Try
            Dim Parametro_Consulta As String = "SELECT ID_ORGANIGRAMA,NOMBRE_ORGANIGRAMA FROM registro_organigrama WHERE " & _
           "  ESTADO_ORGANIGRAMA=1 and EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=" & id_empresa_usuario_gestion
            Dim Ref_Car_Conec33 As New conect.Dbase_Conction_Mysql_RA
            Dim datset As DataSet = New DataSet("remit_dest_interno")
            Dim Result As String = ""
            Result = Ref_Car_Conec33.SELECTION_SELECT_FIELD(Parametro_Consulta, datset)
            If Result <> "YES" Then
                Solicita_datos_caracterizacion_organigrama_activo = "Función Solicita_datos_caracterizacion_organigrama_activo dice " & Result
                Exit Function
            End If
            If datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_caracterizacion_organigrama_activo = "Imposible encontrar el organigrama predeterminado"
                Exit Function
            Else
                If datset.Tables(0).Rows(0).IsNull(0) Then
                    id_organigrama = 0
                Else
                    id_organigrama = datset.Tables(0).Rows(0).Item(0)
                End If
                nombre_organigrama = datset.Tables(0).Rows(0).Item(1)
                Solicita_datos_caracterizacion_organigrama_activo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_caracterizacion_organigrama_activo = "Inconsistencia general función Solicita_datos_caracterizacion_organigrama_activo " & ex.Message
        End Try
    End Function
    Function Listar_Organigramas_Empresa_Combo(ByVal id_empresa As Integer, _
                                              ByRef refcombo As DropDownList, _
                                              ByRef update As UpdatePanel) As String
        Try

            refcombo.Items.Clear()
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "select NOMBRE_ORGANIGRAMA  " & _
                " from REGISTRO_ORGANIGRAMA where " & _
                " EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=" & id_empresa '& " and ESTADO_ORGANIGRAMA=1 "
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("REGISTRO_ORGANIGRAMA")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Listar_Organigramas_Empresa_Combo = "Error listando datos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    refcombo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                update.Update()
                Listar_Organigramas_Empresa_Combo = "YES"
                Exit Function
            Else
                refcombo.Items.Clear()
                update.Update()
                Listar_Organigramas_Empresa_Combo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Listar_Organigramas_Empresa_Combo = "Inconsistencia General Funcion Listar_Organigramas_Empresa_Combo " & ex.Message
        End Try
    End Function
    Function Listar_Organigramas_Empresa_Combo_Items(ByVal id_empresa As Integer, _
                                                     ByRef refcombo As DropDownList, _
                                                     ByRef update As UpdatePanel) As String
        Try

            refcombo.Items.Clear()
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "select ID_ORGANIGRAMA,NOMBRE_ORGANIGRAMA  " & _
                " from REGISTRO_ORGANIGRAMA where " & _
                " EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=" & id_empresa '& " and ESTADO_ORGANIGRAMA=1 "
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("REGISTRO_ORGANIGRAMA")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Listar_Organigramas_Empresa_Combo_Items = "Error Listar_Organigramas_Empresa_Combo_Items " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Dim ilis As System.Web.UI.WebControls.ListItem
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilis = New System.Web.UI.WebControls.ListItem
                    ilis.Text = Datset.Tables(0).Rows(i).Item(1)
                    ilis.Value = Datset.Tables(0).Rows(i).Item(0)
                    refcombo.Items.Add(ilis)
                Next
                Listar_Organigramas_Empresa_Combo_Items = "YES"
                Exit Function
            Else
                refcombo.Items.Clear()
                Listar_Organigramas_Empresa_Combo_Items = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Listar_Organigramas_Empresa_Combo_Items = "Inconsistencia General Función Listar_Organigramas_Empresa_Combo_Items " & ex.Message
        Finally

        End Try
    End Function
    Function Listar_Organigramas_Empresa_Combo_Default(ByVal id_empresa As Integer, _
                                                       ByVal nombre_organigrama As String, _
                                                       ByRef refcombo As DropDownList, _
                                                       ByRef update As UpdatePanel) As String
        Try

            refcombo.Items.Clear()
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "select NOMBRE_ORGANIGRAMA  " & _
                " from REGISTRO_ORGANIGRAMA where " & _
                " EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=" & id_empresa
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("REGISTRO_ORGANIGRAMA")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Listar_Organigramas_Empresa_Combo_Default = "Función Listar_Organigramas_Empresa_Combo_Default dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                refcombo.Items.Add("")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    refcombo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                For i As Integer = 0 To refcombo.Items.Count - 1
                    If refcombo.Items(i).Text = nombre_organigrama Then
                        refcombo.Text = nombre_organigrama
                        Exit For
                    End If
                Next
                update.Update()
                Listar_Organigramas_Empresa_Combo_Default = "YES"
                Exit Function
            Else
                refcombo.Items.Clear()
                update.Update()
                Listar_Organigramas_Empresa_Combo_Default = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Listar_Organigramas_Empresa_Combo_Default = "Inconsistencia General Funcion Listar_Organigramas_Empresa_Combo_Default " & ex.Message
        End Try
    End Function
    Function Listar_Organigramas_Empresa_Combo_Default_Items(ByVal id_empresa As Integer, _
                                                             ByVal id_organigrama As Integer, _
                                                             ByRef refcombo As DropDownList, _
                                                             ByRef update As UpdatePanel) As String
        Try

            refcombo.Items.Clear()
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "select ID_ORGANIGRAMA,NOMBRE_ORGANIGRAMA  " & _
                " from REGISTRO_ORGANIGRAMA where " & _
                " EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=" & id_empresa '& " and ESTADO_ORGANIGRAMA=1 "
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("REGISTRO_ORGANIGRAMA")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Listar_Organigramas_Empresa_Combo_Default_Items = "Función Listar_Organigramas_Empresa_Combo_Default_Items dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                'For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                '    refcombo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                'Next

                Dim ilis As System.Web.UI.WebControls.ListItem
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilis = New System.Web.UI.WebControls.ListItem
                    ilis.Text = Datset.Tables(0).Rows(i).Item(1)
                    ilis.Value = Datset.Tables(0).Rows(i).Item(0)
                    refcombo.Items.Add(ilis)
                Next
                For i As Integer = 0 To refcombo.Items.Count - 1
                    If refcombo.Items(i).Value = id_organigrama Then
                        refcombo.Items(i).Selected = True
                        Exit For
                    End If
                Next
                Listar_Organigramas_Empresa_Combo_Default_Items = "YES"
                Exit Function
            Else
                refcombo.Items.Clear()
                Listar_Organigramas_Empresa_Combo_Default_Items = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Listar_Organigramas_Empresa_Combo_Default_Items = "Inconsistencia General Funcion Listar_Organigramas_Empresa_Combo_Default_Items " & ex.Message
        Finally
            update.Update()
        End Try
    End Function
    Function Lista_organigrama_activo(ByVal id_empresa As Integer, _
                                      ByRef refcombo As DropDownList, _
                                      ByRef update As UpdatePanel) As String
        Try
            refcombo.Items.Clear()
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "select ID_ORGANIGRAMA,NOMBRE_ORGANIGRAMA  " & _
                " from REGISTRO_ORGANIGRAMA where " & _
                " EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=" & id_empresa & " and ESTADO_ORGANIGRAMA=1 "
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("REGISTRO_ORGANIGRAMA")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_organigrama_activo = "Función Listar_Organigramas_Empresa_Combo_Default_Items dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Dim ilis As System.Web.UI.WebControls.ListItem
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilis = New System.Web.UI.WebControls.ListItem
                    ilis.Text = Datset.Tables(0).Rows(i).Item(1)
                    ilis.Value = Datset.Tables(0).Rows(i).Item(0)
                    refcombo.Items.Add(ilis)
                Next
                Lista_organigrama_activo = "YES"
                Exit Function
            Else
                refcombo.Items.Clear()
                Lista_organigrama_activo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_organigrama_activo = "Inconsistencia General Funcion Lista_organigrama_activo " & ex.Message
        Finally
            update.Update()
        End Try
    End Function
    
End Class
