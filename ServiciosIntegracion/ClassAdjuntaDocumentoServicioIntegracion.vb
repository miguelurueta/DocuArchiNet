Public Class CservicioIntegracionAdjuntaDocumento
    Property ErrorService As String
    Property NameService As String
    Property IdServicioIntegracion As Integer
    Property CTipoDocEntrante As IList(Of CTipoDocEntrante)  'Estructura tipo tramite
End Class
Public Class ClassAdjuntaDocumentoServicioIntegracion
    Function ActivaAdjuntaDocumentoServicioIntegracion(ByVal IdTareaWorkflow As Long,
                                                       ByVal IdRuta As Integer,
                                                       ByVal NombreRutaWorkflow As String,
                                                       ByRef CTipoDocEntrante As CTipoDocEntrante) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura del tramamite y la eststuctura del servicio web
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTareaWorkflow     : Representa la identificación de la tarea workflow
        'IdRuta              : Representa la identificación de la ruta
        'NombreRutaWorkflow  : Representa el nombre de la ruta
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CTipoDocEntrante    : Retorna la estructura del tramite
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-10
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            If HttpContext.Current.Session.Item("ADJUNTAR_IMAGENES_PREDETERMINADA") = 0 Then
                ActivaAdjuntaDocumentoServicioIntegracion = "El usuario no tiene permisos para adjuntar documentos desde los servicios de integración."
                Exit Function
            End If
            If IdTareaWorkflow = 0 Or IdTareaWorkflow = -1 Then
                ActivaAdjuntaDocumentoServicioIntegracion = "Debe seleccionar una tarea para adjuntar un documento desde los servicios web."
                Exit Function
            End If
            Dim NombreCampoTramite As String = ""
            Dim Class_configuracion_listado_ruta As New Class_configuracion_listado_ruta
            Result = Class_configuracion_listado_ruta.SolicitaNombreCampoTramiteRuta(IdRuta,
                                                                                     NombreCampoTramite)
            If Result <> "YES" Then
                ActivaAdjuntaDocumentoServicioIntegracion = Result
                Exit Function
            End If
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim TramiteTarea As String = ""
            Dim EstadoFLujo As Integer = 0
            Result = Class_DAT_ADIC_TAR.SolicitaTramiteFlujoWorkflow(IdTareaWorkflow,
                                                                     IdRuta,
                                                                     NombreCampoTramite,
                                                                     NombreRutaWorkflow,
                                                                     TramiteTarea,
                                                                     EstadoFLujo)
            If Result <> "YES" Then
                ActivaAdjuntaDocumentoServicioIntegracion = Result
                Exit Function
            End If
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim IdTramite As Integer = 0
            Result = Class_tipo_doc_entrante.RetornaIdTipoTramitePorNombreTipo(TramiteTarea,
                                                                               IdTramite)
            If Result <> "YES" Then
                ActivaAdjuntaDocumentoServicioIntegracion = Result
                Exit Function
            End If
            Result = Class_tipo_doc_entrante.SolicitaEstructuraTramite(IdTramite,
                                                                       CTipoDocEntrante)
            If Result <> "YES" Then
                ActivaAdjuntaDocumentoServicioIntegracion = Result
                Exit Function
            End If
            Dim Class_ra_ser_servicioIntegracion As New Class_ra_ser_servicioIntegracion
            Result = Class_ra_ser_servicioIntegracion.SolicitaEstructuraServicioIntegracion(CTipoDocEntrante.Id_ser_servicioIntegracion,
                                                                                            CTipoDocEntrante.RaSerServicioInteracion)
            If Result <> "YES" Then
                ActivaAdjuntaDocumentoServicioIntegracion = Result
                Exit Function
            End If
            ActivaAdjuntaDocumentoServicioIntegracion = "YES"
            Exit Function
        Catch ex As Exception
            ActivaAdjuntaDocumentoServicioIntegracion = "Inconistencia general funcion ActivaAdjuntaDocumentoServicioIntegracion " & ex.Message
        End Try
    End Function
End Class
