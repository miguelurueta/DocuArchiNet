Public Class RaSerServicioInteracion
    Property Id_ser_servicioIntegracion As Integer
    Property NombreServicio As String
    Property FechaRegistro As String
    Property EstadoServicio As Integer
End Class
Public Class CDParameterValoresCamposIndiceGabinete
    Property CDParmeterValoresCamposGabineteSII As CDParmeterValoresCamposGabineteSII
    Property CDParmeterValoresCamposGabineteDatAdicTar As CDParmeterValoresCamposGabineteDatAdicTar
    Property CDParmeterValoresCamposGabinete As CDParmeterValoresCamposGabinete
End Class
Public Class Class_ra_ser_servicioIntegracion
    Function SolicitaDatosCamposIndiceGabineteIntegracion(ByVal CDParameterValoresCamposIndiceGabinete As CDParameterValoresCamposIndiceGabinete,
                                                          ByVal NombreServicioIntegracion As String,
                                                          ByRef Radicado As String,
                                                          ByRef CDcamposAsignaAlmacenamiento As List(Of CDcamposAsignaAlmacenamiento)) As String
        '--------------------------------------------------------------------------------------------------
        'Funcion : Asgina campos y datos de alamacenamiento para indice de gabinete para flujos externos
        '          no identiifcados, el sistema solo solicta el valor del campo radicado desde la tabla
        '          de registro de descripcion de tareas DAT_ADIC_TAR
        '          
        '--------------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------------------------
        'CDParameterValoresCamposIndiceGabinete  : Representa la estructura con los parmetros para consulta
        'de los datos del indice del gabinete procedente de una integración
        'NombreServicioIntegracion               : Representa el nombre del servicio de integración 
        '
        '
        '--------------------------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------------------------
        'Radicado                     : Retorna el consecutivo de recibo del sistema SII
        'CDcamposAsignaAlmacenamiento : Retorna de los valores y los campos de almacenamiento
        '--------------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------------------------
        'Fecha                 : 2025-07-18
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ClassIntegracionSII As New Class_Integracion_SII
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Select Case NombreServicioIntegracion
                Case "INTEGRACIONSII"
                    Result = ClassIntegracionSII.SolicitaDatosCamposIndiceGabineteSII(CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabineteSII,
                                                                                      Radicado,
                                                                                      CDcamposAsignaAlmacenamiento)
                    If Result <> "YES" Then
                        SolicitaDatosCamposIndiceGabineteIntegracion = Result
                        Exit Function
                    End If
                Case Else
                    Result = Class_DAT_ADIC_TAR.SolicitaDatosCamposIndiceGabineteFlujoExterno(CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabineteDatAdicTar,
                                                                                              Radicado,
                                                                                              CDcamposAsignaAlmacenamiento)
                    If Result <> "YES" Then
                        SolicitaDatosCamposIndiceGabineteIntegracion = Result
                        Exit Function
                    End If
            End Select
            SolicitaDatosCamposIndiceGabineteIntegracion = "YES"
            Exit Function
        Catch ex As Exception
            SolicitaDatosCamposIndiceGabineteIntegracion = "Inconsistencia general funcion SolicitaDatosCamposIndiceGabineteIntegracion " & ex.Message
        End Try
    End Function
    Function SolicitaEstructuraServicioIntegracion(ByVal IdSerServicioIntegracion As Integer,
                                                   ByRef RaSerServicioInteracion As RaSerServicioInteracion) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de un servicio de integración relacionado a un tipo tramite
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTipoDocEntrante   : Representa la identificación del tipo tramite
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'RaSerServicioInteracion  : Retorna la identificación del usuario radicador
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-07-16
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Try
            Dim SqlConsulta As String = "Select Id_ser_servicioIntegracion," &
                "NombreServicio,FechaRegistro,EstadoServicio from ra_ser_servicioIntegracion " &
                " where Id_ser_servicioIntegracion=" & IdSerServicioIntegracion & " and EstadoServicio=1"
            Dim ConexDB As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_ser_servicioIntegracion")
            Dim Result As String = ConexDB.SELECTION_SELECT_FIELDA(SqlConsulta,
                                                                   Datset)
            If Result <> "YES" Then
                SolicitaEstructuraServicioIntegracion = "Error funcion  SolicitaEstructuraServicioIntegracionPorTipo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                RaSerServicioInteracion.EstadoServicio = 0
                RaSerServicioInteracion.FechaRegistro = ""
                RaSerServicioInteracion.Id_ser_servicioIntegracion = 0
                RaSerServicioInteracion.NombreServicio = ""
                SolicitaEstructuraServicioIntegracion = "YES"
                Exit Function
            Else
                RaSerServicioInteracion.EstadoServicio = Datset.Tables(0).Rows(0).Item("EstadoServicio")
                RaSerServicioInteracion.FechaRegistro = Datset.Tables(0).Rows(0).Item("FechaRegistro")
                RaSerServicioInteracion.Id_ser_servicioIntegracion = Datset.Tables(0).Rows(0).Item("Id_ser_servicioIntegracion")
                RaSerServicioInteracion.NombreServicio = Datset.Tables(0).Rows(0).Item("NombreServicio")
                SolicitaEstructuraServicioIntegracion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaEstructuraServicioIntegracion = "Inconsistencia general funcion SolicitaEstructuraServicioIntegracionPorTipo " & ex.Message
        End Try
    End Function
End Class
