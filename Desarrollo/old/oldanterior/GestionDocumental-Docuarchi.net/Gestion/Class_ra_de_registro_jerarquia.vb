Public Class Class_ra_de_registro_jerarquia
    Function Retorna_id_cuadro_registro_jerarquia(ByVal id_registro_jerarquia As Integer, _
                                                  ByRef id_cuadro_clasficacion As Integer) As String
        Try
            Dim Parametro_Consulta As String = "select  RA_DE_CUADRO_CLASIFICACION_ID_DE_CUADRO_CLASIFICACION " & _
                   " from ra_de_registro_jerarquia where  ID_REGISTRO_JERARQUIA=" & id_registro_jerarquia
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("ra_de_registro_jerarquia")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Retorna_id_cuadro_registro_jerarquia = "Función Retorna_id_cuadro_registro_jerarquia Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                id_cuadro_clasficacion = Dat_reader.Tables(0).Rows(0).Item(0)
                Retorna_id_cuadro_registro_jerarquia = "YES"
                Exit Function
            Else
                Retorna_id_cuadro_registro_jerarquia = "Imposible encontrar el identificador del cuadro de clasificación en la jerarquia  (" & id_registro_jerarquia & ")"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_cuadro_registro_jerarquia = "Inconistencia general funcion Retorna_id_cuadro_registro_jerarquia " & ex.Message
        End Try
    End Function
    Function Retorna_listado_relaciones_jerarquia(ByVal id_registro_jerarquia_padre As Integer, _
                                                  ByRef matri_id_registro_jerarquia_hijo() As Integer) As String
        '---------------------------------------------------------------
        'Función : Retorna las relaciones de jerarquia de un nodo 
        'padre con el parámetro id jerarquía
        'Fecha : 2017-01-19
        'Ing Miguel Angel Urueta Miranda
        '---------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  ID_REGISTRO_JERARQUIA_HIJO " & _
                         " from ra_de_registro_relaciones_jerarquia  " & _
                         " where  ID_REGISTRO_JERARQUIA_PADRE='" & id_registro_jerarquia_padre & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("ra_de_registro_relaciones_jerarquia")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Retorna_listado_relaciones_jerarquia = "Función Retorna_listado_relaciones_jerarquia Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    ReDim Preserve matri_id_registro_jerarquia_hijo(i)
                    matri_id_registro_jerarquia_hijo(i) = Dat_reader.Tables(0).Rows(i).Item(0)
                Next
                Retorna_listado_relaciones_jerarquia = "YES"
                Exit Function
            Else
                Retorna_listado_relaciones_jerarquia = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_listado_relaciones_jerarquia = "Inconsistencia genera función Retorna_listado_relaciones_jerarquia " & ex.Message
        End Try
    End Function
End Class
