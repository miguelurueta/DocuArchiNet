Public Structure Stru_ra_cert_certificado
    Dim id_certificado As Integer
    Dim ra_cert_tipo_certficacion_tipo_certificacion As Integer
    Dim ra_cert_tipo_certificado_tipo_certificado As Integer
    Dim ra_cert_ente_certificador_id_ente_certificador As Integer
    Dim Contador_registro_de_firmas As Long
    Dim numero_serial As String
    Dim algortimo_de_firma As String
    Dim algortimo_has_firma As String
    Dim valido_desde As String
    Dim valido_hasta As String
    Dim numero_identificacion_suscriptor As String
    Dim correo_electronico_suscriptor As String
    Dim cargo_suscriptor As String
    Dim pais_suscriptor As String
    Dim departamento_suscriptor As String
    Dim municipio_suscriptor As String
    Dim Direccion_suscriptor As String
    Dim llave_publica As String
    Dim algoritmo_identificacion As String
    Dim huella_digital As String
    Dim uso_llave As String
    Dim llave_privada As String
    Dim estado_revocado As Integer
    Dim numero_firmas_certificado As Integer
    Dim nombre_suscriptor As String
    Dim default_certificado As Integer
    Dim ra_cert_servicio_certificado_id_cert_sevcio_certificado As Integer
    Dim util_tsa_certificado As Integer
End Structure
Public Class Class_ra_cert_certificado

    Function Solicita_identificacion_cert_default(ByRef id_certificado As Integer) As String
        '------------------------------------------------------
        'Funcion : Solicita la identificación del certificado
        'Ing . Miguel Angel Urueta Miranda 
        'Fecha : 2022-03-11
        '-------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = " SELECT  id_certificado " &
            " from ra_cert_certificado where default_certificado=1"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cert_certificado")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_identificacion_cert_default = "Función Solicita_identificacion_cert_default dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_certificado = 0
                Solicita_identificacion_cert_default = "YES"
                Exit Function
            Else
                id_certificado = Datset.Tables(0).Rows(0).Item(0)
                Solicita_identificacion_cert_default = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_identificacion_cert_default = "Inconsistencia general funcion Solicita_identificacion_cert_default  " & ex.Message
        End Try
    End Function
    Function Solicita_estructura_certificado(ByVal id_certificado As Integer,
                                             ByRef Stru_ra_cert_certificado As Stru_ra_cert_certificado) As String
        '---------------------------------------------------------
        'Funcion : Solicita la estructura de un certicado digital
        'Ing . Miguel Angel Urueta Miranda
        'Fecha : 2022-03-11
        '---------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = " SELECT  id_certificado,ra_cert_tipo_certficacion_tipo_certificacion, " &
            "ra_cert_tipo_certificado_tipo_certificado,ra_cert_ente_certificador_id_ente_certificador," &
            "Contador_registro_de_firmas,numero_serial,algortimo_de_firma,algortimo_has_firma,valido_desde,valido_hasta," &
            "numero_identificacion_suscriptor,correo_electronico_suscriptor,cargo_suscriptor,pais_suscriptor,departamento_suscriptor," &
            "municipio_suscriptor,Direccion_suscriptor,llave_publica,algoritmo_identificacion,huella_digital,uso_llave,llave_privada" &
            ",estado_revocado,numero_firmas_certificado,nombre_suscriptor,default_certificado,ra_cert_servicio_certificado_id_cert_sevcio_certificado,util_tsa_certificado " &
           " from ra_cert_certificado where id_certificado=" & id_certificado
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cert_certificado")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estructura_certificado = "Función Solicita_estructura_certificado dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_certificado = 0
                Solicita_estructura_certificado = "Imposible encontrar la estructura del certifcado (" & id_certificado & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    Stru_ra_cert_certificado.id_certificado = 0
                Else
                    Stru_ra_cert_certificado.id_certificado = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) Then
                    Stru_ra_cert_certificado.ra_cert_tipo_certficacion_tipo_certificacion = 0
                Else
                    Stru_ra_cert_certificado.ra_cert_tipo_certficacion_tipo_certificacion = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) Then
                    Stru_ra_cert_certificado.ra_cert_tipo_certificado_tipo_certificado = 0
                Else
                    Stru_ra_cert_certificado.ra_cert_tipo_certificado_tipo_certificado = Datset.Tables(0).Rows(0).Item(2)
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) Then
                    Stru_ra_cert_certificado.ra_cert_ente_certificador_id_ente_certificador = 0
                Else
                    Stru_ra_cert_certificado.ra_cert_ente_certificador_id_ente_certificador = Datset.Tables(0).Rows(0).Item(3)
                End If
                If Datset.Tables(0).Rows(0).IsNull(4) Then
                    Stru_ra_cert_certificado.Contador_registro_de_firmas = 0
                Else
                    Stru_ra_cert_certificado.Contador_registro_de_firmas = Datset.Tables(0).Rows(0).Item(4)
                End If
                If Datset.Tables(0).Rows(0).IsNull(5) Then
                    Stru_ra_cert_certificado.numero_serial = ""
                Else
                    Stru_ra_cert_certificado.numero_serial = Datset.Tables(0).Rows(0).Item(5)
                End If
                If Datset.Tables(0).Rows(0).IsNull(6) Then
                    Stru_ra_cert_certificado.algortimo_de_firma = ""
                Else
                    Stru_ra_cert_certificado.algortimo_de_firma = Datset.Tables(0).Rows(0).Item(6)
                End If
                If Datset.Tables(0).Rows(0).IsNull(7) Then
                    Stru_ra_cert_certificado.algortimo_has_firma = ""
                Else
                    Stru_ra_cert_certificado.algortimo_has_firma = Datset.Tables(0).Rows(0).Item(7)
                End If
                If Datset.Tables(0).Rows(0).IsNull(8) Then
                    Stru_ra_cert_certificado.valido_desde = ""
                Else
                    Stru_ra_cert_certificado.valido_desde = Datset.Tables(0).Rows(0).Item(8)
                End If
                If Datset.Tables(0).Rows(0).IsNull(9) Then
                    Stru_ra_cert_certificado.valido_hasta = ""
                Else
                    Stru_ra_cert_certificado.valido_hasta = Datset.Tables(0).Rows(0).Item(9)
                End If
                If Datset.Tables(0).Rows(0).IsNull(10) Then
                    Stru_ra_cert_certificado.numero_identificacion_suscriptor = ""
                Else
                    Stru_ra_cert_certificado.numero_identificacion_suscriptor = Datset.Tables(0).Rows(0).Item(10)
                End If
                If Datset.Tables(0).Rows(0).IsNull(11) Then
                    Stru_ra_cert_certificado.correo_electronico_suscriptor = ""
                Else
                    Stru_ra_cert_certificado.correo_electronico_suscriptor = Datset.Tables(0).Rows(0).Item(11)
                End If
                If Datset.Tables(0).Rows(0).IsNull(12) Then
                    Stru_ra_cert_certificado.cargo_suscriptor = ""
                Else
                    Stru_ra_cert_certificado.cargo_suscriptor = Datset.Tables(0).Rows(0).Item(12)
                End If
                If Datset.Tables(0).Rows(0).IsNull(13) Then
                    Stru_ra_cert_certificado.pais_suscriptor = ""
                Else
                    Stru_ra_cert_certificado.pais_suscriptor = Datset.Tables(0).Rows(0).Item(13)
                End If
                If Datset.Tables(0).Rows(0).IsNull(14) Then
                    Stru_ra_cert_certificado.departamento_suscriptor = ""
                Else
                    Stru_ra_cert_certificado.departamento_suscriptor = Datset.Tables(0).Rows(0).Item(14)
                End If
                If Datset.Tables(0).Rows(0).IsNull(15) Then
                    Stru_ra_cert_certificado.municipio_suscriptor = ""
                Else
                    Stru_ra_cert_certificado.municipio_suscriptor = Datset.Tables(0).Rows(0).Item(15)
                End If
                If Datset.Tables(0).Rows(0).IsNull(16) Then
                    Stru_ra_cert_certificado.Direccion_suscriptor = ""
                Else
                    Stru_ra_cert_certificado.Direccion_suscriptor = Datset.Tables(0).Rows(0).Item(16)
                End If
                If Datset.Tables(0).Rows(0).IsNull(17) Then
                    Stru_ra_cert_certificado.llave_publica = ""
                Else
                    Stru_ra_cert_certificado.llave_publica = Datset.Tables(0).Rows(0).Item(17)
                End If
                If Datset.Tables(0).Rows(0).IsNull(18) Then
                    Stru_ra_cert_certificado.algoritmo_identificacion = ""
                Else
                    Stru_ra_cert_certificado.algoritmo_identificacion = Datset.Tables(0).Rows(0).Item(18)
                End If
                If Datset.Tables(0).Rows(0).IsNull(19) Then
                    Stru_ra_cert_certificado.huella_digital = ""
                Else
                    Stru_ra_cert_certificado.huella_digital = Datset.Tables(0).Rows(0).Item(19)
                End If
                If Datset.Tables(0).Rows(0).IsNull(20) Then
                    Stru_ra_cert_certificado.uso_llave = ""
                Else
                    Stru_ra_cert_certificado.uso_llave = Datset.Tables(0).Rows(0).Item(20)
                End If
                If Datset.Tables(0).Rows(0).IsNull(21) Then
                    Stru_ra_cert_certificado.llave_privada = ""
                Else
                    Stru_ra_cert_certificado.llave_privada = Datset.Tables(0).Rows(0).Item(21)
                End If
                If Datset.Tables(0).Rows(0).IsNull(22) Then
                    Stru_ra_cert_certificado.estado_revocado = 0
                Else
                    Stru_ra_cert_certificado.estado_revocado = Datset.Tables(0).Rows(0).Item(22)
                End If
                If Datset.Tables(0).Rows(0).IsNull(23) Then
                    Stru_ra_cert_certificado.numero_firmas_certificado = 0
                Else
                    Stru_ra_cert_certificado.numero_firmas_certificado = Datset.Tables(0).Rows(0).Item(23)
                End If
                If Datset.Tables(0).Rows(0).IsNull(24) Then
                    Stru_ra_cert_certificado.nombre_suscriptor = ""
                Else
                    Stru_ra_cert_certificado.nombre_suscriptor = Datset.Tables(0).Rows(0).Item(24)
                End If
                If Datset.Tables(0).Rows(0).IsNull(25) Then
                    Stru_ra_cert_certificado.default_certificado = 0
                Else
                    Stru_ra_cert_certificado.default_certificado = Datset.Tables(0).Rows(0).Item(25)
                End If
                If Datset.Tables(0).Rows(0).IsNull(26) Then
                    Stru_ra_cert_certificado.ra_cert_servicio_certificado_id_cert_sevcio_certificado = 0
                Else
                    Stru_ra_cert_certificado.ra_cert_servicio_certificado_id_cert_sevcio_certificado = Datset.Tables(0).Rows(0).Item(26)
                End If
                If Datset.Tables(0).Rows(0).IsNull(27) Then
                    Stru_ra_cert_certificado.util_tsa_certificado = 0
                Else
                    Stru_ra_cert_certificado.util_tsa_certificado = Datset.Tables(0).Rows(0).Item(27)
                End If
                Solicita_estructura_certificado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_certificado = "Incosnsitencia general funcion Solicita_estructura_certificado " & ex.Message
        End Try
    End Function
End Class
