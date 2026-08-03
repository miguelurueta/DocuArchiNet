Imports AjaxControlToolkit
Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Drawing
Imports System.Globalization
Public Structure stru_rangos_horas
    Dim id_per_horas As Integer
    Dim id_per_dias_radicacion As Integer
    Dim Hora_inicio As String
    Dim Hora_final As String
End Structure
Public Class ClassRe_Per_Horas_Dia_Radicacion
    Function Solicita_estructura_rangos_horas_dia(ByVal id_dia As Integer, _
                                                 ByRef stru_rango_hora() As stru_rangos_horas) As String
        Try
            Dim Parametro_Consulta As String = "Select id_per_horas," & _
                "id_per_dias_radicacion,Hora_inicio,Hora_final from ra_per_horas_dia_radicacion " & _
            " where id_per_dias_radicacion=" & id_dia
            Dim Datset As New DataSet
            Dim Result As String = ""
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estructura_rangos_horas_dia = " Función Solicita_estructura_rangos_horas_dia dice   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estructura_rangos_horas_dia = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_rango_hora(i)
                    stru_rango_hora(i).id_per_horas = Datset.Tables(0).Rows(i).Item(0)
                    stru_rango_hora(i).id_per_dias_radicacion = Datset.Tables(0).Rows(i).Item(1)
                    stru_rango_hora(i).Hora_inicio = Datset.Tables(0).Rows(i).Item(2)
                    stru_rango_hora(i).Hora_final = Datset.Tables(0).Rows(i).Item(3)
                Next
                Solicita_estructura_rangos_horas_dia = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_rangos_horas_dia = "Inconsistencia general función Solicita_estructura_rangos_horas_dia " & ex.Message
        End Try
    End Function
End Class
