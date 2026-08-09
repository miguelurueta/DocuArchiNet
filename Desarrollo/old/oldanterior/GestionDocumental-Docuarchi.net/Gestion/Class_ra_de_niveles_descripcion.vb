Public Class Class_ra_de_niveles_descripcion
    Function Solicita_id_nivel_de_clasificacion(ByVal nombre_nivel_clasificacion As String, _
                                                ByRef id_nivel_clasificacion As Integer) As String
        '---------------------------------------------------------------
        'Función : Solicita la identificación del nivel de clasificación
        'con el nombre del nivel de clasificación
        'Fecha : 2017-01-13
        'Ing Miguel Angel Urueta Miranda
        '---------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  ID_NIVELES_DESCRIPCION " & _
                      " from ra_de_niveles_descripcion where  NOMBRE_NIVEL_DESCRIPCION='" & nombre_nivel_clasificacion & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("ra_de_niveles_descripcion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Solicita_id_nivel_de_clasificacion = "Función Solicita_id_nivel_de_clasificacion Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                id_nivel_clasificacion = Dat_reader.Tables(0).Rows(0).Item(0)
                Solicita_id_nivel_de_clasificacion = "YES"
                Exit Function
            Else
                Solicita_id_nivel_de_clasificacion = "Imposible encontrar el nivel de clasificacón documental de " & nombre_nivel_clasificacion & " comunique a su administrador"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_nivel_de_clasificacion = "Inconsistencia general función Solicita_id_nivel_de_clasificacion " & ex.Message
        End Try
    End Function
End Class
