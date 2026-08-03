Public Class Class_tipo_unidad_conservacion
    Function Retorna_id_tipo_unidad_conservacion_expediente(ByVal nombre_tipo As String, _
                                                            ByRef id_tipo_unidad_conservacion As Integer, _
                                                            ByVal tipo_unidad_conservacion As Integer) As String
        '-----------------------------------------------------------------
        'Fucion : Retorna id tipo unidad de conservacion expediente
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2016-08-29
        '------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "select ID_TIPO_UNIDAD  " & _
                " from tipo_unidad_conservacion where " & _
                " NOMBRE_TIPO_UNIDAD='" & nombre_tipo & "' and TIPO_UNIDAD=" & tipo_unidad_conservacion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_unidad_conservacion")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_tipo_unidad_conservacion_expediente = "Funcion Retorna_id_tipo_unidad_conservacion_expediente dice  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_tipo_unidad_conservacion = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_tipo_unidad_conservacion_expediente = "YES"
                Exit Function
            Else
                Retorna_id_tipo_unidad_conservacion_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_tipo_unidad_conservacion_expediente = "Inconsistencia general función Retorna_id_tipo_unidad_conservacion_expediente " & ex.Message
        End Try
    End Function
    Function Retorna_nombre_tipo_unidad_conservacion_expediente(ByVal id_tipo_unidad_conservacion As Integer,
                                                                ByRef tipo_unidad_conservacion As String) As String
        '-----------------------------------------------------------------
        'Fucion : Retorna id tipo unidad de conservacion expediente
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2016-08-29
        '------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "select NOMBRE_TIPO_UNIDAD  " &
                " from tipo_unidad_conservacion where " &
                " ID_TIPO_UNIDAD=" & id_tipo_unidad_conservacion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_unidad_conservacion")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_nombre_tipo_unidad_conservacion_expediente = "Funcion Retorna_nombre_tipo_unidad_conservacion_expediente dice  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                tipo_unidad_conservacion = Datset.Tables(0).Rows(0).Item(0)
                Retorna_nombre_tipo_unidad_conservacion_expediente = "YES"
                Exit Function
            Else
                tipo_unidad_conservacion = ""
                Retorna_nombre_tipo_unidad_conservacion_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_tipo_unidad_conservacion_expediente = "Inconsistencia general función Retorna_nombre_tipo_unidad_conservacion_expediente " & ex.Message
        End Try
    End Function
End Class
