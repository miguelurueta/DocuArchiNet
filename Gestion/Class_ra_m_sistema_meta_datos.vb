Public Class Class_ra_m_sistema_meta_datos
    Function Solicita_identificacion_sistema_meta_dato_default_archivo(ByVal id_tipo_meta_dato As Integer,
                                                                       ByRef id_sistema_meta_datos As Integer) As String
        '------------------------------------------------------
        'Funcion : Solicita la identificación del sistema
        'meta datos default con el parametro default
        'datos. Valores del sistema de meta datos
        '1- Meta dato archivo
        '2- Meta datos expediente
        'Fecha : 2020-01-31
        'Ing : Miguel Angel Urueta Miranda
        '------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "Select id_sistema_meta_datos  from ra_m_sistema_meta_datos where tipo_sistema_meta_datos=" & id_tipo_meta_dato &
                " and estado_registro_sistema_meta_datos=1 and meta_default=1 "
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_m_sistema_meta_datos")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_identificacion_sistema_meta_dato_default_archivo = "Función Solicita_identificacion_sistema_meta_dato_default_archivo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_sistema_meta_datos = Datset.Tables(0).Rows(0).Item(0)
                Solicita_identificacion_sistema_meta_dato_default_archivo = "YES"
                Exit Function
            Else
                id_sistema_meta_datos = 0
                Solicita_identificacion_sistema_meta_dato_default_archivo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_identificacion_sistema_meta_dato_default_archivo = "Inconsistencia general función Solicita_identificacion_sistema_meta_dato_default_archivo " & ex.Message
        End Try
    End Function
    Function Solicita_lista_sistema_meta_datos_archivo(ByVal id_sistema_meta_datos As Integer,
                                                       ByRef refcombo As DropDownList,
                                                       ByRef update As UpdatePanel) As String
        Try

            refcombo.Items.Clear()
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "select id_sistema_meta_datos,nombre_sistema_meta_datos  " &
                " from ra_m_sistema_meta_datos where tipo_sistema_meta_datos=1"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_m_sistema_meta_datos")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_lista_sistema_meta_datos_archivo = "Error Solicita_lista_sistema_meta_datos_archivo " & Result
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
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    If refcombo.Items(i).Value = id_sistema_meta_datos Then
                        refcombo.Items(i).Selected = True
                        Exit For
                    End If
                Next
                update.Update()
                Solicita_lista_sistema_meta_datos_archivo = "YES"
                Exit Function
            Else
                refcombo.Items.Clear()
                update.Update()
                Solicita_lista_sistema_meta_datos_archivo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_sistema_meta_datos_archivo = "Inconsistencia General Función Solicita_lista_sistema_meta_datos_archivo " & ex.Message
        End Try
    End Function
    Function Solicita_tipo_sistema_meta_datos(ByVal id_sistema_meta_dato As Integer,
                                              ByRef tipo_sistema_meta_datos As Integer) As String
        '------------------------------------------------------
        'Funcion : Solicita el tipo de sistema
        'meta datos con el parametro identificacion  del sistema 
        'de meta datos. Valores del sistema de meta datos
        '1- Meta dato archivo
        '2- Meta datos expediente
        'Fecha : 2022-02-08
        'Ing : Miguel Angel Urueta Miranda
        '------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "Select tipo_sistema_meta_datos  from ra_m_sistema_meta_datos where id_sistema_meta_datos=" & id_sistema_meta_dato &
               " and estado_registro_sistema_meta_datos=1 "
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_m_sistema_meta_datos")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_tipo_sistema_meta_datos = "Función Solicita_tipo_sistema_meta_datos dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                tipo_sistema_meta_datos = Datset.Tables(0).Rows(0).Item(0)
                Solicita_tipo_sistema_meta_datos = "YES"
                Exit Function
            Else
                tipo_sistema_meta_datos = 0
                Solicita_tipo_sistema_meta_datos = "Imposible emncontrar el registro del sistema meta datos (" & id_sistema_meta_dato & ")"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_tipo_sistema_meta_datos = "Inconsistencia general funcion Solicita_tipo_sistema_meta_datos " & ex.Message
        End Try
    End Function
    Function Solicita_nombre_sistema_meta_dato(ByVal id_sistema_meta_datos As Integer,
                                               ByRef nombre_sistema_meta_dato As String) As String
        '------------------------------------------------
        'Funcion : Solicita nombre sistema meta datos
        'con el identificador del meta dato
        'Fecha : 2022-02-17
        'Ing . Miguel Angel Urueta Miranda
        '------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "Select nombre_sistema_meta_datos  from ra_m_sistema_meta_datos where id_sistema_meta_datos=" & id_sistema_meta_datos
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_m_sistema_meta_datos")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombre_sistema_meta_dato = "Función Solicita_nombre_sistema_meta_dato dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                nombre_sistema_meta_dato = Datset.Tables(0).Rows(0).Item(0)
                Solicita_nombre_sistema_meta_dato = "YES"
                Exit Function
            Else
                nombre_sistema_meta_dato = ""
                Solicita_nombre_sistema_meta_dato = "Imposible emncontrar el nombre del sistema meta datos (" & id_sistema_meta_datos & ")"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombre_sistema_meta_dato = "Inconsistencia general funcion Solicita_nombre_sistema_meta_dato " & ex.Message
        End Try
    End Function
End Class
