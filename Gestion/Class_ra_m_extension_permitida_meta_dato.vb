Public Class Class_ra_m_extension_permitida_meta_dato
    Function Solicita_extension_activa_meta_dato(ByVal id_sistema_meta_dato As Integer,
                                                 ByRef matri_extensiones() As String) As String
        '---------------------------------------------------------
        'Funcion : Solicita extensiones del sistema de meta datos
        'informado
        'Fecha : 2022-02-22
        'Ing . Miguel Angel Urueta Miranda
        '---------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "SELECT extension_archivo " &
            " FROM ra_m_extension_permitida_meta_dato  " &
            " where ra_m_sistema_meta_datos_id_sistema_meta_datos=" & id_sistema_meta_dato
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_m_extension_permitida_meta_dato")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_extension_activa_meta_dato = "Función Solicita_extension_activa_meta_dato dice " & Result
                Exit Function
            End If
            Erase matri_extensiones
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve matri_extensiones(i)
                    matri_extensiones(i) = Datset.Tables(0).Rows(0).Item(0)
                Next
                Solicita_extension_activa_meta_dato = "YES"
                Exit Function
            Else
                Solicita_extension_activa_meta_dato = "Imosible encontrar extensiones activas para el sistema de meta datos (" & id_sistema_meta_dato & ")"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_extension_activa_meta_dato = "Inconsistencia general funcion Solicita_extension_activa_meta_dato " & ex.Message
        End Try
    End Function
End Class
