Imports AjaxControlToolkit
Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Drawing
Imports System.Globalization
Public Class ClassRea_001_feriados
    Function Solicita_confirmacion_dia_feriado(ByVal pais As String, _
                                              ByVal fecha_actual As String, _
                                              ByRef estado_confirmacion As String, _
                                              ByRef descripcion_feriado As String) As String
        '-------------------------------------------------------
        'Función : Solicita si la fecha informada en formato
        'YYYY-MM-DD Corresponde a un día festivo del pais
        'informado
        'Fecha : 2018-09-18
        'Ingeniero : Miguel Angel Urueta Miranda
        '-------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "Select DESCRIPCION_FERIADO " & _
                " from rea_001_feriados where FECHA_FERIADO='" & fecha_actual & "' and PAIS_FERIADO='" & pais & "' and ESTADO_DIA=1"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_confirmacion_dia_feriado = " Función Solicita_confirmacion_dia_feriado dice   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_confirmacion = "NO"
                Solicita_confirmacion_dia_feriado = "YES"
                Exit Function
            Else
                descripcion_feriado = Datset.Tables(0).Rows(0).Item(0)
                estado_confirmacion = "YES"
                Solicita_confirmacion_dia_feriado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_confirmacion_dia_feriado = "Inconsistencia general función Solicita_confirmacion_dia_feriado " & ex.Message
        End Try
    End Function
End Class
