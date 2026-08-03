Public Class Class_ra_tipo_expediente
    Function Retorna_tipo_expediente_requiere_unidad_conservacion(ByVal id_tipo_expediente As Integer, _
                                                                  ByRef requiere_unidad_conservacion As Integer) As String
        '-------------------------------------------------------------
        'Función : Reotorna si el tipo de expediente requiere de unidad
        'conservación física
        'Fecha : 2016-09-03
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------
        Try
            Dim sqlconsulta As String = "Select REQUIERE_UNIDAD_CONSERVACION from ra_tipo_expediente where ID_TIPO_EXPEDIENTE=" & _
                  id_tipo_expediente
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_tipo_expediente")
            Dim Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Retorna_tipo_expediente_requiere_unidad_conservacion = "Función Retorna_tipo_expediente_requiere_unidad_conservacion dice  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                requiere_unidad_conservacion = Datset.Tables(0).Rows(0).Item(0)
                Retorna_tipo_expediente_requiere_unidad_conservacion = "YES"
                Exit Function
            Else
                Retorna_tipo_expediente_requiere_unidad_conservacion = "Imposible encontrar el tipo de expediente es posible que lo aya eliminado  otro usuario"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_tipo_expediente_requiere_unidad_conservacion = "Inconsistencia general función Retorna_tipo_expediente_requiere_unidad_conservacion " & ex.Message
        End Try
    End Function
    Function Retorna_nombre_tipo_expediente_por_id_expediente(ByVal id_expediente As Integer, _
                                                             ByRef nombre_tipo_expediente As String) As String
        '*********************************************************************
        'Funcion : Retorna nombre tipo expediente con el iden
        'tificador del expediente registrada en la tabla
        'expediente archivo, haciendo cartesianidad con la tabla
        'tipo_unidad_conservacion con el filtro de id unidad de conservacion
        'Fecha 2015-01-27 Modíficado para web 2016-08-30
        'Ingeniero : Miguel Angel Urueta Miranda
        '*********************************************************************
        Try
            Dim sqlconsulta As String = "Select rte.NOMBRE_TIPO_EXPEDIENTE from expediente_archivo as ea " & _
            " inner join ra_tipo_expediente as rte on (rte.ID_TIPO_EXPEDIENTE=ea.RA_TIP_EXPE_ID_TIPO_EXPEDIENTE)" & _
            " where ID_EXPEDIENTE=" & id_expediente
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Retorna_nombre_tipo_expediente_por_id_expediente = "Función Retorna_id_configuracion_rotulo_por_nombre_plantilla Error dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_nombre_tipo_expediente_por_id_expediente = "Imposible el id del tipo de unidad de conservación "
                Exit Function
            Else
                nombre_tipo_expediente = Datset.Tables(0).Rows(0).Item(0)
                Retorna_nombre_tipo_expediente_por_id_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_tipo_expediente_por_id_expediente = "Inconsistencia Función " & vbCrLf & _
            "  Retorna_nombre_tipo_expediente_por_id_expediente " & ex.Message
        End Try
    End Function
    Function lista_tipos_expedientes_Combo(ByRef refcombo As DropDownList, _
                                              ByRef update As UpdatePanel, _
                                              Optional ByVal option_todos As Integer = 1) As String
        '**************************************************
        'Funcion : Lista los tipos de expedientes
        'disponibles
        'Fecha : 2015-01-05
        'Ingeniero : Miguel Angel Urueta Miranda
        '**************************************************
        Try

            Dim sqlconsulta As String = "Select NOMBRE_TIPO_EXPEDIENTE from ra_tipo_expediente  " & _
            " where ESTADO_TIPO=1"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_tipo_expediente")
            Dim Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                lista_tipos_expedientes_Combo = "Error listando datos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                refcombo.Items.Clear()
                If option_todos = 1 Then
                    refcombo.Items.Add("Todas")
                End If
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    refcombo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                update.Update()
                lista_tipos_expedientes_Combo = "YES"
                Exit Function
            Else
                refcombo.Items.Clear()
                If option_todos = 1 Then
                    refcombo.Items.Add("Todas")
                End If
                update.Update()
                lista_tipos_expedientes_Combo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            lista_tipos_expedientes_Combo = "Inconsistencia funcion lista_tipos_expedientes_Combo " & ex.Message
        End Try
    End Function
    Function Retorna_tipo_id_expediente(ByRef id_tipo_expediente As Integer, _
                                        ByVal nombre_tipo_expediente As String) As String
        '******************************************************
        'Funcion : Retorna el id del tipo expediente enviando
        'como parametro el nombre del expediente
        'Fecha : 2015-01-05
        'Ingeniero : Miguel Angel Urueta Miranda
        '******************************************************
        Try
            Dim sqlconsulta As String = "Select ID_TIPO_EXPEDIENTE from ra_tipo_expediente where " & _
               "  NOMBRE_TIPO_EXPEDIENTE='" & nombre_tipo_expediente & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_tipo_expediente")
            Dim Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Retorna_tipo_id_expediente = "Error listando datos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_tipo_expediente = Datset.Tables(0).Rows(0).Item(0)
                Retorna_tipo_id_expediente = "YES"
                Exit Function
            Else
                Retorna_tipo_id_expediente = "Imposible encontrar la identificacion del tipo de expediente"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_tipo_id_expediente = "Inconsistencia funcion Retorna_tipo_id_expediente " & ex.Message
        End Try
    End Function
    Function Solicita_la_identificacion_del_tipo_de_expediente_carpeta_electronica(ByRef id_tipo_carpeta_expediente As Integer) As String
        Try
            Dim tipo_expediente_carpeta As String = "ELECTRONICO"
            Dim Parametro_Consulta = "select ID_TIPO_EXPEDIENTE " & _
         " from ra_tipo_expediente WHERE NOMBRE_TIPO_EXPEDIENTE='" & tipo_expediente_carpeta & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_tipo_expediente")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_la_identificacion_del_tipo_de_expediente_carpeta_electronica = "Funcion  Solicita_la_identificacion_del_tipo_de_expediente_carpeta_electronica dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_la_identificacion_del_tipo_de_expediente_carpeta_electronica = "Imposible encontrar la identificacón del tipo de carpeta expediente ELECTRONICO"
                Exit Function
            Else
                id_tipo_carpeta_expediente = Datset.Tables(0).Rows(0).Item(0)
                Solicita_la_identificacion_del_tipo_de_expediente_carpeta_electronica = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_la_identificacion_del_tipo_de_expediente_carpeta_electronica = "Inconsistencia general función Solicita_la_identificacion_del_tipo_de_expediente_carpeta_electronica " & ex.Message
        End Try
    End Function
    Function Solicita_la_identificacion_del_tipo_de_expediente_carpeta_electronica(ByRef id_tipo_carpeta_expediente As Integer, _
                                                                                   ByVal estado_confirma As Integer) As String
        Try
            Dim tipo_expediente_carpeta As String = "ELECTRONICO"
            Dim Parametro_Consulta = "select ID_TIPO_EXPEDIENTE " & _
         " from ra_tipo_expediente WHERE NOMBRE_TIPO_EXPEDIENTE='" & tipo_expediente_carpeta & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_tipo_expediente")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_la_identificacion_del_tipo_de_expediente_carpeta_electronica = "Funcion  Solicita_la_identificacion_del_tipo_de_expediente_carpeta_electronica dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                If estado_confirma = 1 Then
                    Solicita_la_identificacion_del_tipo_de_expediente_carpeta_electronica = "Imposible encontrar la identificacón del tipo de carpeta expediente ELECTRONICO"
                    Exit Function
                Else
                    id_tipo_carpeta_expediente = 0
                    Solicita_la_identificacion_del_tipo_de_expediente_carpeta_electronica = "YES"
                End If

            Else
                id_tipo_carpeta_expediente = Datset.Tables(0).Rows(0).Item(0)
                Solicita_la_identificacion_del_tipo_de_expediente_carpeta_electronica = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_la_identificacion_del_tipo_de_expediente_carpeta_electronica = "Inconsistencia general función Solicita_la_identificacion_del_tipo_de_expediente_carpeta_electronica " & ex.Message
        End Try
    End Function
    Function Solicita_la_identificacion_del_tipo_de_expediente_carpeta_hibrido(ByRef id_tipo_carpeta_expediente As Integer, _
                                                                                  ByVal estado_confirma As Integer) As String
        Try
            Dim tipo_expediente_carpeta As String = "HIBRIDO"
            Dim Parametro_Consulta = "select ID_TIPO_EXPEDIENTE " & _
         " from ra_tipo_expediente WHERE NOMBRE_TIPO_EXPEDIENTE='" & tipo_expediente_carpeta & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_tipo_expediente")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_la_identificacion_del_tipo_de_expediente_carpeta_hibrido = "Funcion  Solicita_la_identificacion_del_tipo_de_expediente_carpeta_hibrido dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                If estado_confirma = 1 Then
                    Solicita_la_identificacion_del_tipo_de_expediente_carpeta_hibrido = "Imposible encontrar la identificacón del tipo de carpeta expediente HIBRIDO"
                    Exit Function
                Else
                    id_tipo_carpeta_expediente = 0
                    Solicita_la_identificacion_del_tipo_de_expediente_carpeta_hibrido = "YES"
                End If

            Else
                id_tipo_carpeta_expediente = Datset.Tables(0).Rows(0).Item(0)
                Solicita_la_identificacion_del_tipo_de_expediente_carpeta_hibrido = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_la_identificacion_del_tipo_de_expediente_carpeta_hibrido = "Inconsistencia general función Solicita_la_identificacion_del_tipo_de_expediente_carpeta_hibrido " & ex.Message
        End Try
    End Function
    Function Retorna_ayuda_clase_expediente(ByVal nombre_documento As String, _
                                                ByRef ayuda_documento As String) As String
        '************************************************************
        'Funcion : Retorna ayuda expediente
        'Ingeniero : Miguel Angel  Urueta
        'Fecha 2015-01-15
        '************************************************************
        Try

            Dim Parametro_Consulta As String = "SELECT EXPEDIENTE_AYUDA " & _
            " FROM ra_tipo_expediente " & _
             " where NOMBRE_TIPO_EXPEDIENTE='" & nombre_documento & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_tipo_expediente")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_ayuda_clase_expediente = "Funcion Retorna_ayuda_clase_expediente dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_ayuda_clase_expediente = "Imposible encontrar Retorna_ayuda_clase_expediente " & nombre_documento
                Exit Function
            Else
                ayuda_documento = Datset.Tables(0).Rows(0).Item(0)
                Retorna_ayuda_clase_expediente = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_ayuda_clase_expediente = "Inconsistencia funcion Retorna_ayuda_clase_expediente " & ex.Message
        End Try
    End Function
    Function Solicita_nombre_tipo_expediente(ByVal id_tipo_expediente As Integer, _
                                             ByVal nombre_tipo_expediente As String) As String
        Try
            Dim Parametro_Consulta = "select NOMBRE_TIPO_EXPEDIENTE " & _
            " from ra_tipo_expediente WHERE ID_TIPO_EXPEDIENTE=" & id_tipo_expediente
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_tipo_expediente")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombre_tipo_expediente = "Funcion  Solicita_nombre_tipo_expediente dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_nombre_tipo_expediente = "Imposible encontrar el nombre del tipo de expediente (" & id_tipo_expediente & ")"
                Exit Function
            Else
                nombre_tipo_expediente = Datset.Tables(0).Rows(0).Item(0)
                Solicita_nombre_tipo_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombre_tipo_expediente = "Inconsistencia general función Solicita_nombre_tipo_expediente " & ex.Message
        End Try
    End Function
End Class
