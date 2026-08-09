Public Structure stru_ra_de_registro_relaciones_jerarquia
    Dim ID_REGISTRO_RELACIONES_JERARQUIA As Integer
    Dim RA_DE_CUADRO_CLASIFICACION_ID_DE_CUADRO_CLASIFICACION As Integer
    Dim ID_REGISTRO_JERARQUIA_PADRE As Integer
    Dim ID_REGISTRO_JERARQUIA_HIJO As Integer
End Structure
Public Class Class_ra_de_registro_relaciones_jerarquia
    Function Solicita_niveles_rleacionados_a_cuadro_de_clasificacion(ByVal id_cuadro_clasficacion As Integer, _
                                                                     ByRef stru() As stru_ra_de_registro_relaciones_jerarquia) As String
        Try
            stru = Nothing
            Dim Parametro_Consulta As String = "select  ID_REGISTRO_RELACIONES_JERARQUIA, " & _
                "RA_DE_CUADRO_CLASIFICACION_ID_DE_CUADRO_CLASIFICACION,ID_REGISTRO_JERARQUIA_PADRE,ID_REGISTRO_JERARQUIA_HIJO" & _
                   " from ra_de_registro_relaciones_jerarquia where  RA_DE_CUADRO_CLASIFICACION_ID_DE_CUADRO_CLASIFICACION=" & id_cuadro_clasficacion
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("ra_de_registro_jerarquia")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Solicita_niveles_rleacionados_a_cuadro_de_clasificacion = "Función Solicita_niveles_rleacionados_a_cuadro_de_clasificacion Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    ReDim Preserve stru(i)
                    stru(i).ID_REGISTRO_RELACIONES_JERARQUIA = Dat_reader.Tables(0).Rows(i).Item(0)
                    stru(i).RA_DE_CUADRO_CLASIFICACION_ID_DE_CUADRO_CLASIFICACION = Dat_reader.Tables(0).Rows(i).Item(1)
                    stru(i).ID_REGISTRO_JERARQUIA_PADRE = Dat_reader.Tables(0).Rows(i).Item(2)
                    stru(i).ID_REGISTRO_JERARQUIA_HIJO = Dat_reader.Tables(0).Rows(i).Item(3)
                Next
                Solicita_niveles_rleacionados_a_cuadro_de_clasificacion = "YES"
                Exit Function
            Else
                Solicita_niveles_rleacionados_a_cuadro_de_clasificacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_niveles_rleacionados_a_cuadro_de_clasificacion = "Inconsistencia gneral función Solicita_niveles_rleacionados_a_cuadro_de_clasificacion " & ex.Message
        End Try
    End Function
End Class
