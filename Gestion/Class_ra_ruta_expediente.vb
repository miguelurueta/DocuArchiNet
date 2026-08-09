Public Structure stru_ruta_expediente
    Dim DISCO As Integer
    Dim RUTA As String
    Dim NUM_EXPEDIENTE As Integer
    Dim NUMERO_MAX_EXP As Integer
End Structure
Public Class Class_ra_ruta_expediente
    Function Solicita_datos_estructura_ruta_expediente(ByRef stru_ruta_expediente_ As stru_ruta_expediente) As String
        Try
            Dim Parametro_Consulta = "select DISCO,RUTA,NUM_EXPEDIENTE,NUMERO_MAX_EXP " & _
           " from ra_ruta_expediente "
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_ruta_expediente")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_estructura_ruta_expediente = "Funcion  Solicita_datos_estructura_ruta_expediente dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_estructura_ruta_expediente = "El sistema no registra la ruta de expedientes, contacte a su administador"
                Exit Function
            Else
                stru_ruta_expediente_.DISCO = Datset.Tables(0).Rows(0).Item(0)
                stru_ruta_expediente_.RUTA = Datset.Tables(0).Rows(0).Item(1)
                stru_ruta_expediente_.NUM_EXPEDIENTE = Datset.Tables(0).Rows(0).Item(2)
                stru_ruta_expediente_.NUMERO_MAX_EXP = Datset.Tables(0).Rows(0).Item(3)
                Solicita_datos_estructura_ruta_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_estructura_ruta_expediente = "Inconsistencia general función Solicita_datos_estructura_ruta_expediente " & ex.Message
        End Try
    End Function
End Class
