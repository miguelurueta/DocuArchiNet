Public Class Class_ra_de_fondo_documental
    Function Retorna_id_fondo_documental_nombre(ByVal nombre_fondo_documental As String,
                                                ByRef id_fondo_documental As Integer) As String
        '**************************************************
        'Funcion : Retorna id fondo documental
        'Fecha : 2017-02-02
        'Ingeniero : Miguel Angel Urueta Miranda
        '*************************************************
        Try
            Dim sqlconsulta As String = "Select Id_fondo_documental from ra_de_fondo_documental  " &
                          " where Nombre_fondo_documental='" & nombre_fondo_documental & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_de_fondo_documental")
            Dim Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Retorna_id_fondo_documental_nombre = "Función Retorna_id_fondo_documental_nombre Error de  conexión " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_fondo_documental = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_fondo_documental_nombre = "YES"
                Exit Function
            Else
                Retorna_id_fondo_documental_nombre = "Imposible encontrar la identificación del nombre del fondo " & nombre_fondo_documental
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_fondo_documental_nombre = "Inconsistencia función Retorna_id_fondo_documental_nombre " & ex.Message
        End Try
    End Function
    Function Retorna_nombre_fondo_documental(ByVal id_fondo_documental As Integer,
                                             ByRef nombre_fondo_documental As String) As String
        '--------------------------------------------------
        'Funcion : Retorna nombre fondo documental como
        'parametro el id
        'Fecha : 2022-06-14
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------------
        Try
            Dim sqlconsulta As String = "Select nombre_fondo_documental  from ra_de_fondo_documental  " &
                          " where Id_fondo_documental='" & id_fondo_documental & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_de_fondo_documental")
            Dim Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Retorna_nombre_fondo_documental = "Función Retorna_nombre_fondo_documental Error de  conexión " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                nombre_fondo_documental = Datset.Tables(0).Rows(0).Item(0)
                Retorna_nombre_fondo_documental = "YES"
                Exit Function
            Else
                Retorna_nombre_fondo_documental = "Imposible encontrar el nombre del fondo con la idetificacion(" & id_fondo_documental & ")"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_fondo_documental = "Inconsistencia función Retorna_nombre_fondo_documental " & ex.Message
        End Try
    End Function
End Class
