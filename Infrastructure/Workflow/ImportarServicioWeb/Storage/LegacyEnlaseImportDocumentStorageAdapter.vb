Imports System
Imports System.IO
Imports System.Web

' Adaptador exclusivo de ANEXOS_RADICADO_ENLASE. Reutiliza una sola vez la función legacy,
' pero le entrega el archivo ya descargado y validado por el proveedor moderno.
Public NotInheritable Class LegacyEnlaseImportDocumentStorageAdapter
    Implements IImportDocumentStoragePort

    Public Function Almacenar(ByVal comando As ComandoAlmacenamientoImportacion) As ResultadoFaseImportacion Implements IImportDocumentStoragePort.Almacenar
        If Not Validar(comando) Then Return Fallo("INVALID_ENLASE_STORAGE_COMMAND", True)
        If Not ContextoSesionCoincide(comando) Then Return Fallo("ENLASE_SESSION_CONTEXT_MISMATCH", True)
        Dim idImagen As Integer = 0
        Try
            Dim imagen As stru_datos_image_lista = Nothing
            Dim anexo = CrearAnexo(comando)
            Dim respuesta = New ClassAlmacenamiento().PreAlmacenaDocumentoAnexosEnlaceIntegracionSII(
                comando.IdTipoListaChequeo,
                comando.DescripcionTipo,
                comando.IdTramite,
                0,
                comando.NombreGabinete,
                comando.ProviderReference,
                comando.Radicado,
                anexo,
                comando.NombreClaseFormatoDocumento,
                idImagen,
                imagen,
                comando.RutaArchivo)
            If String.Equals(respuesta, "YES", StringComparison.OrdinalIgnoreCase) Then
                Dim proyeccion = CrearProyeccion(comando, imagen, idImagen)
                If proyeccion Is Nothing Then Return FalloIncierto(idImagen)
                Return New ResultadoFaseImportacion With {
                    .Exitoso = True,
                    .PersistenciaConocida = False,
                    .IdDocumento = idImagen,
                    .EvidenciaFisicaConfirmada = False,
                    .Reintentable = False,
                    .ProyeccionDocumentoEnlase = proyeccion}
            End If
            Return MapearRechazo(respuesta, idImagen)
        Catch
            Return FalloIncierto(idImagen)
        End Try
    End Function

    Private Shared Function CrearProyeccion(ByVal comando As ComandoAlmacenamientoImportacion,
                                             ByVal imagen As stru_datos_image_lista,
                                             ByVal idImagen As Integer) As ProyeccionDocumentoEnlaseImportacion
        If comando Is Nothing OrElse idImagen <= 0 Then Return Nothing
        Dim gabinete = If(String.IsNullOrWhiteSpace(imagen.nombre_gabinete), comando.NombreGabinete, imagen.nombre_gabinete)
        Dim radicado = If(String.IsNullOrWhiteSpace(imagen.radicado), comando.Radicado, imagen.radicado)
        ' DBT no es poblado por todos los caminos legacy (en particular, el anexo sin
        ' tipología). La extensión sí pertenece al resultado del almacenamiento y es el
        ' descriptor físico autoritativo disponible sin una segunda consulta.
        Dim tipoFisico = If(String.IsNullOrWhiteSpace(Convert.ToString(imagen.DBT)), imagen.extension, Convert.ToString(imagen.DBT))
        Dim nombre = If(String.IsNullOrWhiteSpace(imagen.notipodocumento), comando.DescripcionTipo, imagen.notipodocumento)
        If String.IsNullOrWhiteSpace(nombre) Then nombre = comando.NombreArchivoOrigen
        Dim icono = If(String.IsNullOrWhiteSpace(imagen.icono_icono_awe_some), "fa-file", imagen.icono_icono_awe_some)
        If String.IsNullOrWhiteSpace(gabinete) OrElse String.IsNullOrWhiteSpace(radicado) OrElse
           String.IsNullOrWhiteSpace(tipoFisico) OrElse String.IsNullOrWhiteSpace(nombre) OrElse
           comando.IdTareaWorkflow <= 0 Then Return Nothing
        Return New ProyeccionDocumentoEnlaseImportacion With {
            .NombreGabinete = gabinete, .IdDocumento = idImagen, .Radicado = radicado,
            .TipoFisico = tipoFisico, .NombreDocumento = nombre, .IdTarea = comando.IdTareaWorkflow,
            .EstadoFirma = imagen.estado_firma_digital, .ClaseIcono = icono}
    End Function

    Private Shared Function Validar(ByVal comando As ComandoAlmacenamientoImportacion) As Boolean
        Return comando IsNot Nothing AndAlso
            String.Equals(comando.Capability, SiiImportProvider.AnnexesEnlaseCapability, StringComparison.OrdinalIgnoreCase) AndAlso
            comando.IdTareaWorkflow > 0 AndAlso comando.IdRutaWorkflow > 0 AndAlso comando.IdTramite > 0 AndAlso
            ((Not comando.DocumentTypeRequired AndAlso comando.IdTipoListaChequeo = 0 AndAlso String.IsNullOrWhiteSpace(comando.DescripcionTipo)) OrElse
             (comando.IdTipoListaChequeo > 0 AndAlso Not String.IsNullOrWhiteSpace(comando.DescripcionTipo))) AndAlso
            Not String.IsNullOrWhiteSpace(comando.NombreGabinete) AndAlso Not String.IsNullOrWhiteSpace(comando.NombreRutaWorkflow) AndAlso
            Not String.IsNullOrWhiteSpace(comando.Radicado) AndAlso Not String.IsNullOrWhiteSpace(comando.ProviderReference) AndAlso
            Not String.IsNullOrWhiteSpace(comando.ExternalKey) AndAlso comando.MetadatosSii IsNot Nothing AndAlso
            String.Equals(comando.ExternalKey.Trim(), If(comando.MetadatosSii.IdAnexo, String.Empty).Trim(), StringComparison.Ordinal) AndAlso
            Not String.IsNullOrWhiteSpace(comando.RutaArchivo) AndAlso File.Exists(comando.RutaArchivo)
    End Function

    Private Shared Function ContextoSesionCoincide(ByVal comando As ComandoAlmacenamientoImportacion) As Boolean
        Try
            Dim current = HttpContext.Current
            If current Is Nothing OrElse current.Session Is Nothing Then Return False
            Dim selectedTask = Convert.ToInt64(current.Session.Item("ID_TAREA_SELECCIONDA_ENLACE"))
            Dim selectedRoute = Convert.ToInt32(current.Session.Item("Id_Ruta_Workflow"))
            Dim routeName = Convert.ToString(current.Session.Item("WF_RUTAWORKFLOW"))
            Return selectedTask = comando.IdTareaWorkflow AndAlso selectedRoute = comando.IdRutaWorkflow AndAlso
                String.Equals(routeName, comando.NombreRutaWorkflow, StringComparison.OrdinalIgnoreCase)
        Catch
            Return False
        End Try
    End Function

    Private Shared Function CrearAnexo(ByVal comando As ComandoAlmacenamientoImportacion) As CDlistaAnexosSII
        Dim metadata = comando.MetadatosSii
        Return New CDlistaAnexosSII With {
            .idanexo = comando.ExternalKey.Trim(),
            .formato = metadata.Formato,
            .tipo = metadata.TipoImagen,
            .observaciones = metadata.Observaciones,
            .url = String.Empty,
            .tiposirep = metadata.TipoSirep,
            .tipodigitalizacion = metadata.TipoDigitalizacion,
            .identificador = metadata.IdentificadorImagen,
            .nombre = metadata.RazonSocial,
            .matricula = metadata.Matricula,
            .proponente = metadata.Proponente,
            .fechadocumento = metadata.FechaDocumento,
            .origen = metadata.Origen,
            .identificacion = metadata.NitCedula}
    End Function

    Private Shared Function MapearRechazo(ByVal respuesta As String, ByVal idDocumento As Integer) As ResultadoFaseImportacion
        Dim texto = If(respuesta, String.Empty).ToUpperInvariant()
        If idDocumento > 0 Then Return FalloIncierto(idDocumento)
        If texto.Contains("TIPO DOCUMENTAL") OrElse texto.Contains("LISTA DE CHEQUEO") Then
            Return Fallo("ENLASE_DOCUMENT_TYPE_REQUIRED", True)
        End If
        If texto.Contains("ARCHIVO PREPARADO") OrElse texto.Contains("ARCHIVO SOLICITADO") OrElse
           texto.Contains("IMPOSIBLE ACCEDER AL ARCHIVO") Then
            Return Fallo("ENLASE_STORAGE_FILE_UNAVAILABLE", True)
        End If
        If texto.Contains("CONFIGURACION DIGITALIZACION") OrElse texto.Contains("CONFIGURACIÓN DIGITALIZACIÓN") Then
            Return Fallo("ENLASE_STORAGE_CONFIGURATION_UNAVAILABLE", True)
        End If
        If texto.Contains("MATRICULA") OrElse texto.Contains("PROPONENTE") OrElse texto.Contains("EXPEDIENTE SII") Then
            Return Fallo("ENLASE_STORAGE_SUBJECT_UNAVAILABLE", True)
        End If
        If texto.Contains("CONEXION") OrElse texto.Contains("CONECTAR") OrElse texto.Contains("MYSQL") OrElse
           texto.Contains("BASE DE DATOS") OrElse texto.Contains("ACCESS DENIED") Then
            Return Fallo("ENLASE_STORAGE_DEPENDENCY_UNAVAILABLE", True)
        End If
        ' La función puede haber almacenado y fallado al actualizar ID_IMAGEN; una salida no
        ' clasificable nunca se eleva a rechazo previo ni habilita reintento automático.
        Return FalloIncierto()
    End Function

    Private Shared Function FalloIncierto(Optional ByVal idDocumento As Integer = 0) As ResultadoFaseImportacion
        Return New ResultadoFaseImportacion With {
            .Codigo = "ENLASE_STORAGE_UNCERTAIN",
            .MensajeVisible = "No fue posible confirmar el resultado del almacenamiento; se requiere reconciliación.",
            .PersistenciaConocida = False,
            .Reintentable = False,
            .EvidenciaFisicaConfirmada = False,
            .IdDocumento = If(idDocumento > 0, New Nullable(Of Long)(idDocumento), Nothing)}
    End Function
    Private Shared Function Fallo(ByVal codigo As String, ByVal persistenciaConocida As Boolean) As ResultadoFaseImportacion
        Return New ResultadoFaseImportacion With {
            .Codigo = codigo,
            .MensajeVisible = "No fue posible almacenar el anexo.",
            .PersistenciaConocida = persistenciaConocida,
            .Reintentable = persistenciaConocida}
    End Function
End Class
