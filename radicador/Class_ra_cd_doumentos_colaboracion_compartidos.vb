Public Class Class_ra_cd_doumentos_colaboracion_compartidos
    Function SolicitaEstadoSolicitudAprobacionDocumentoCompartido(ByVal idSolicitudAprobacion As Integer,
                                                                  ByRef EstadoGeneralSolicitud As Integer,
                                                                  ByRef DescripcionEstadoGeneralSolicitud As String,
                                                                  ByVal DescripcionTipoAprobacion As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita estado de solicitud de aprobación de un documento compartido para aprobación
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'idSolicitudAprobacion      : Representa la identificación de la solictud general de aprobación
        'DescripcionTipoAprobacion  : Representa la la decisión de la solcitud de aprobacion  -Aprobado
        '                             -Desaprobado  -Archivado 
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'EstadoGeneralSolicitud  : Retorna el estado general de la solicitud  (-1) Representa el caso de
        'una única solicitud  (-2)  No hay desición por que no se completo el numero total de decisiones
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim numero_aprobado As Integer = 0
            Dim numero_no_aprobado As Integer = 0
            Dim numero_archivado As Integer = 0
            Dim numero_sin_desicion As Integer = 0
            Dim tipo_aprobacion As Integer = 0
            Dim descripcion_estado_aprobacion As String = ""
            '-------Asigna el valor del tipo de decisión
            If DescripcionTipoAprobacion = "Aprobado" Then
                numero_aprobado = 1
            End If
            If DescripcionTipoAprobacion = "Desaprobado" Then
                numero_no_aprobado = 1
            End If
            If DescripcionTipoAprobacion = "Archivado" Then
                numero_archivado = 1
            End If
            Dim sql_consulta As String = "SELECT ESTADO_RESPUESTA_SOLICITUD " &
                       " from ra_cd_usuarios_documentos_compartidos  where ID_RA_CD_DOCUMENTOS_COMPARTIDOS=" & idSolicitudAprobacion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaEstadoSolicitudAprobacionDocumentoCompartido = "Error SolicitaEstadoSolicitudAprobacionDocumentoCompartido " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaEstadoSolicitudAprobacionDocumentoCompartido = "No se encontraron documentos compartidos para aprobación relacionados a usuario relacionados "
                Exit Function
            Else
                If Datset.Tables(0).Rows.Count > 1 Then
                    '-------Asigna los valores de los registros de decisión
                    For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                        If Datset.Tables(0).Rows(i).Item(0) = 0 Then
                            numero_sin_desicion = numero_sin_desicion + 1
                        End If
                        If Datset.Tables(0).Rows(i).Item(0) = 1 Then
                            numero_aprobado = numero_aprobado + 1
                        End If
                        If Datset.Tables(0).Rows(i).Item(0) = 2 Then
                            numero_no_aprobado = numero_no_aprobado + 1
                        End If
                        If Datset.Tables(0).Rows(i).Item(0) = 3 Then
                            numero_archivado = numero_archivado + 1
                        End If
                    Next
                    '----------Descuenta las archivadas
                    Dim numero_total_solcitudes As Integer = Datset.Tables(0).Rows.Count
                    If numero_archivado > 0 Then
                        numero_total_solcitudes = numero_total_solcitudes - numero_archivado
                    End If
                    '----------Caso unica solicitud por solicitudes archivadas que no cuentan en la decisión
                    If numero_total_solcitudes = 1 Then
                        EstadoGeneralSolicitud = -1
                        SolicitaEstadoSolicitudAprobacionDocumentoCompartido = "YES"
                        Exit Function
                    End If
                    Dim diferencia As Integer = 0
                    '----Determina decisión por la diferencia de la mitad mas uno
                    diferencia = numero_total_solcitudes - numero_aprobado
                    If numero_aprobado >= diferencia Then
                        EstadoGeneralSolicitud = 1
                        DescripcionEstadoGeneralSolicitud = "Aprobado"
                        SolicitaEstadoSolicitudAprobacionDocumentoCompartido = "YES"
                        Exit Function
                    End If
                    diferencia = numero_total_solcitudes - numero_no_aprobado
                    If numero_no_aprobado >= diferencia Then
                        EstadoGeneralSolicitud = 2
                        DescripcionEstadoGeneralSolicitud = "Desaprobado"
                        SolicitaEstadoSolicitudAprobacionDocumentoCompartido = "YES"
                        Exit Function
                    End If
                    EstadoGeneralSolicitud = -2
                Else
                    '-------------------------------
                    'Caso unica solicitud
                    '-------------------------------
                    EstadoGeneralSolicitud = -1
                End If
                SolicitaEstadoSolicitudAprobacionDocumentoCompartido = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaEstadoSolicitudAprobacionDocumentoCompartido = "Inconsistencia general función SolicitaEstadoSolicitudAprobacionDocumentoCompartido " & ex.Message
        End Try
    End Function
End Class
