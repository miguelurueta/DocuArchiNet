Public Structure Stru_ra_cert_ente_certificador
    Dim id_ente_certificador As Integer
    Dim Nombre_ente_certificador As String
    Dim nit_ente_certificador As String
    Dim direc_ente_certificador As String
    Dim correo_electronico As String
    Dim estado_ente As Integer
    Dim Login_service As String
    Dim password_service As String
    Dim Login_tsa_service As String
    Dim password_tsa_service As String
End Structure
Public Class Class_ra_cert_ente_certificador
    Function Solicita_estructura_ente_certificador(ByVal id_ente_certificador As Integer,
                                                   ByRef Stru_ra_cert_ente_certificador As Stru_ra_cert_ente_certificador) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita los datos de la estructura del ente certificador
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'id_ente_certificador: Representa la identificación del ente certificador
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Stru_ra_cert_ente_certificador  : Retorna la estructura del ente certificador
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-11-09
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Parametro_Consulta As String = "SELECT  id_ente_certificador,Nombre_ente_certificador," &
            "nit_ente_certificador,direc_ente_certificador,correo_electronico,estado_ente,Login_service," &
            "password_service,Login_tsa_service,password_tsa_service" &
            " from ra_cert_ente_certificador where id_ente_certificador=" & id_ente_certificador
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cert_servicio_certificado")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estructura_ente_certificador = "Función Solicita_estructura_ente_certificador dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estructura_ente_certificador = "Imposible encontrar el ente certificador"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    Stru_ra_cert_ente_certificador.id_ente_certificador = 0
                Else
                    Stru_ra_cert_ente_certificador.id_ente_certificador = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) Then
                    Stru_ra_cert_ente_certificador.Nombre_ente_certificador = ""
                Else
                    Stru_ra_cert_ente_certificador.Nombre_ente_certificador = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) Then
                    Stru_ra_cert_ente_certificador.nit_ente_certificador = ""
                Else
                    Stru_ra_cert_ente_certificador.nit_ente_certificador = Datset.Tables(0).Rows(0).Item(2)
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) Then
                    Stru_ra_cert_ente_certificador.direc_ente_certificador = ""
                Else
                    Stru_ra_cert_ente_certificador.direc_ente_certificador = Datset.Tables(0).Rows(0).Item(3)
                End If
                If Datset.Tables(0).Rows(0).IsNull(4) Then
                    Stru_ra_cert_ente_certificador.correo_electronico = ""
                Else
                    Stru_ra_cert_ente_certificador.correo_electronico = Datset.Tables(0).Rows(0).Item(4)
                End If
                If Datset.Tables(0).Rows(0).IsNull(5) Then
                    Stru_ra_cert_ente_certificador.estado_ente = 1
                Else
                    Stru_ra_cert_ente_certificador.estado_ente = Datset.Tables(0).Rows(0).Item(5)
                End If
                If Datset.Tables(0).Rows(0).IsNull(6) Then
                    Stru_ra_cert_ente_certificador.Login_service = ""
                Else
                    Stru_ra_cert_ente_certificador.Login_service = Datset.Tables(0).Rows(0).Item(6)
                End If
                If Datset.Tables(0).Rows(0).IsNull(7) Then
                    Stru_ra_cert_ente_certificador.password_service = ""
                Else
                    Stru_ra_cert_ente_certificador.password_service = Datset.Tables(0).Rows(0).Item(7)
                End If
                If Datset.Tables(0).Rows(0).IsNull(8) Then
                    Stru_ra_cert_ente_certificador.Login_tsa_service = ""
                Else
                    Stru_ra_cert_ente_certificador.Login_tsa_service = Datset.Tables(0).Rows(0).Item(8)
                End If
                If Datset.Tables(0).Rows(0).IsNull(9) Then
                    Stru_ra_cert_ente_certificador.password_tsa_service = ""
                Else
                    Stru_ra_cert_ente_certificador.password_tsa_service = Datset.Tables(0).Rows(0).Item(9)
                End If
                Solicita_estructura_ente_certificador = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_ente_certificador = "Inconsistencia general funcion Solicita_estructura_ente_certificador " & ex.Message
        End Try
    End Function
End Class
