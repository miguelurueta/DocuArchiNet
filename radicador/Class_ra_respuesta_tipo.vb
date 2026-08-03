Imports GestionDocumental_Docuarchi.net.WebServiceRadicacion

Public Class Class_ra_respuesta_tipo
    Function Service_lista_tipo_respuesta(ByRef rad_drow_lista_ As List(Of rad_drow_lista)) As String
        Try
            Dim Parametro_Consulta As String = "select id_tipo_respuesta,Tipo_respuesta from ra_respuesta_tipo " &
            " where estado_tipo_respuesta_envia=1"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_respuesta_tipo")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Service_lista_tipo_respuesta = " Función Service_lista_tipo_respuesta dice " & Result
                Exit Function
            End If
            Dim item As rad_drow_lista
            If Datset.Tables(0).Rows.Count > 0 Then
                item = New rad_drow_lista
                item.value = "0"
                item.text = ""
                rad_drow_lista_.Add(item)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    item = New rad_drow_lista
                    item.value = Datset.Tables(0).Rows(i).Item(0)
                    item.text = Datset.Tables(0).Rows(i).Item(1)
                    rad_drow_lista_.Add(item)
                Next
                Service_lista_tipo_respuesta = "YES"
                Exit Function
            Else
                Service_lista_tipo_respuesta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Service_lista_tipo_respuesta = "Inconsistencia general función Service_lista_tipo_respuesta " & ex.Message
        End Try

    End Function
End Class
