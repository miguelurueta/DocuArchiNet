Imports AjaxControlToolkit
Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Drawing
Imports System.Globalization
Public Structure stru_dias
    Dim id_per_dias_radicacion As Integer
    Dim system_plantilla_radicado_id_Plantilla As Integer
    Dim Nombre_dia As String
    Dim Numero_dia As Integer
    Dim Estado_Habil As Integer
End Structure
Public Class ClassRa_per_dias_radicacion
    Function Solicita_estado_autorizacion_dia_habil(ByVal numero_dia As Integer, _
                                                    ByVal id_plantilla As Integer, _
                                                    ByRef estado_autorizacion As Integer, _
                                                    ByRef nombre_dia As String) As String

        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "Select Estado_Habil,Nombre_dia " & _
                " from ra_per_dias_radicacion where system_plantilla_radicado_id_Plantilla=" & id_plantilla & " and Numero_dia=" & numero_dia
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_autorizacion_dia_habil = " Función Solicita_estado_autorizacion_dia_habil dice   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_autorizacion = 1
                Solicita_estado_autorizacion_dia_habil = "YES"
                Exit Function
            Else
                estado_autorizacion = Datset.Tables(0).Rows(0).Item(0)
                nombre_dia = Datset.Tables(0).Rows(0).Item(1)
                Solicita_estado_autorizacion_dia_habil = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_autorizacion_dia_habil = "Inconsistencia general funcion Solicita_estado_autorizacion_dia_habil " & ex.Message
        End Try
    End Function
    Function Solicita_identificacion_dia_habil(ByVal numero_dia As Integer, _
                                               ByVal id_plantilla As Integer, _
                                               ByRef id_dia As Integer) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "Select id_per_dias_radicacion " & _
                " from ra_per_dias_radicacion where system_plantilla_radicado_id_Plantilla=" & id_plantilla & " and Numero_dia=" & numero_dia
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_identificacion_dia_habil = " Función Solicita_identificacion_dia_habil dice   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_dia = 1
                Solicita_identificacion_dia_habil = "YES"
                Exit Function
            Else
                id_dia = Datset.Tables(0).Rows(0).Item(0)
                Solicita_identificacion_dia_habil = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_identificacion_dia_habil = "Inconsistencia general función Solicita_identificacion_dia_habil " & ex.Message
        End Try
    End Function
    Function Solicita_estructura_dias_plantilla(ByVal id_plantilla As Integer, _
                                                ByRef stru_dia() As stru_dias) As String
        Try
            Dim Parametro_Consulta As String = "Select id_per_dias_radicacion," & _
                "system_plantilla_radicado_id_Plantilla,Nombre_dia,Numero_dia,Estado_Habil from ra_per_dias_radicacion " & _
            " where system_plantilla_radicado_id_Plantilla=" & id_plantilla
            Dim Datset As New DataSet
            Dim Result As String = ""
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estructura_dias_plantilla = " Función Solicita_estructura_dias_plantilla dice   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estructura_dias_plantilla = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_dia(i)
                    stru_dia(i).id_per_dias_radicacion = Datset.Tables(0).Rows(i).Item(0)
                    stru_dia(i).system_plantilla_radicado_id_Plantilla = Datset.Tables(0).Rows(i).Item(1)
                    stru_dia(i).Nombre_dia = Datset.Tables(0).Rows(i).Item(2)
                    stru_dia(i).Numero_dia = Datset.Tables(0).Rows(i).Item(3)
                    stru_dia(i).Estado_Habil = Datset.Tables(0).Rows(i).Item(4)
                Next
                Solicita_estructura_dias_plantilla = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_dias_plantilla = "Inconsistencia general función Solicita_estructura_dias_plantilla " & ex.Message
        End Try
    End Function
End Class
