Public Class Class_tipo_unidad_documental
    Function Retorna_id_tipo_unidad_documental_por_nombre(ByVal nombre_tipo_unidad As String, _
                                                          ByRef id_tipo_unidad_documental As Integer) As String
        '-----------------------------------------------------------------
        'Funcion : Retorna la identificacion del tipo de unidad documental
        'con el paramentro nombre unidad documental
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2016-10-20
        '------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "select ID_TIPO_UNIDAD_DOCUMENTAL " & _
                " from TIPOS_UNIDAD_DOCUMENTAL  WHERE NOMBRE_TIPO_UNIDAD_DOCUMENTAL='" & Trim(nombre_tipo_unidad) & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_unidad_conservacion")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_tipo_unidad_documental_por_nombre = "Funcion  Retorna_id_tipo_unidad_documental_por_nombre " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_tipo_unidad_documental = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_tipo_unidad_documental_por_nombre = "YES"
                Exit Function
            Else
                Retorna_id_tipo_unidad_documental_por_nombre = "Imposible encontrar el id tipo unidad documental por el nombre (" & nombre_tipo_unidad & ")"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_tipo_unidad_documental_por_nombre = "Inconsistencia general función Retorna_id_tipo_unidad_documental_por_nombre " & ex.Message
        End Try
    End Function
End Class
