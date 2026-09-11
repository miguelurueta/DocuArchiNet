Imports System
Imports System.IO
Imports System.Threading
Imports System.Diagnostics
Imports System.Collections.Generic
Imports System.Linq

Public NotInheritable Class SystemImportacionServicioClock
    Implements IImportacionServicioClock
    Public Function UtcNow() As DateTime Implements IImportacionServicioClock.UtcNow
        Return DateTime.UtcNow
    End Function
End Class

Public NotInheritable Class SafeImportIntentTransitionAudit
    Implements IImportIntentTransitionAudit
    Public Sub Registrar(ByVal transicion As TransicionImportacion, ByVal aceptada As Boolean, ByVal codigo As String) Implements IImportIntentTransitionAudit.Registrar
        Dim intentId = If(transicion Is Nothing, String.Empty, transicion.IntentId)
        Dim clientItemId = If(transicion Is Nothing, String.Empty, transicion.ClientItemId)
        Trace.TraceInformation("ImportIntentTransition intent={0} item={1} accepted={2} code={3}", intentId, clientItemId, aceptada, If(codigo, String.Empty))
    End Sub
End Class

' Pasos productivos ordenados. La máquina de estados audita y fecha cada fase confirmada.
Public NotInheritable Class DownloadImportExecutionStep
    Implements IImportExecutionStep

    Private ReadOnly _clients As IRegistroClientesProveedoresImportacion

    Public Sub New(ByVal clients As IRegistroClientesProveedoresImportacion)
        If clients Is Nothing Then Throw New ArgumentNullException("clients")
        _clients = clients
    End Sub

    Public ReadOnly Property FaseConfirmada As FaseImportacionServicio Implements IImportExecutionStep.FaseConfirmada
        Get
            Return FaseImportacionServicio.RecursoObtenido
        End Get
    End Property

    Public Function Ejecutar(ByVal contexto As ContextoImportacionServicio,
                             ByVal intencion As IntencionImportacionServicio,
                             ByVal item As ResultadoElementoImportacion) As ResultadoFaseImportacion Implements IImportExecutionStep.Ejecutar
        If item Is Nothing OrElse item.IdentidadExterna Is Nothing Then Return Fallo("INVALID_EXTERNAL_ITEM", True)
        Dim resolved = _clients.Resolver(item.IdentidadExterna.ProviderId)
        If resolved Is Nothing OrElse Not resolved.Encontrado Then Return Fallo("PROVIDER_CLIENT_UNAVAILABLE", True)
        Try
            item.ContenidoDescargado = resolved.Cliente.DownloadAsync(item.IdentidadExterna.ExternalKey,
                item.CorrelationId, CancellationToken.None, intencion.Id, item.ClientItemId,
                If(intencion.ContextoOriginal Is Nothing, Nothing, intencion.ContextoOriginal.OperationId), contexto.IdTarea,
                If(intencion.ContextoOriginal Is Nothing, Nothing, intencion.ContextoOriginal.Radicado), item.IdentidadExterna.ExternalKey).GetAwaiter().GetResult()
            If item.ContenidoDescargado Is Nothing OrElse item.ContenidoDescargado.Length = 0 Then Return Fallo("EMPTY_EXTERNAL_RESOURCE", True)
            Dim sii = TryCast(resolved.Cliente, SiiImportProvider)
            If sii IsNot Nothing Then item.MetadatosSii = sii.ResolveStorageMetadataAsync(item.IdentidadExterna.ExternalKey,
                item.CorrelationId, CancellationToken.None, intencion.Id, item.ClientItemId,
                If(intencion.ContextoOriginal Is Nothing, Nothing, intencion.ContextoOriginal.OperationId), contexto.IdTarea,
                If(intencion.ContextoOriginal Is Nothing, Nothing, intencion.ContextoOriginal.Radicado), item.IdentidadExterna.ExternalKey).GetAwaiter().GetResult()
            Return Exito()
        Catch
            Return Fallo("EXTERNAL_DOWNLOAD_FAILED", True)
        End Try
    End Function

    Private Shared Function Exito() As ResultadoFaseImportacion
        Return New ResultadoFaseImportacion With {.Exitoso = True, .PersistenciaConocida = True}
    End Function

    Private Shared Function Fallo(ByVal codigo As String, ByVal reintentable As Boolean) As ResultadoFaseImportacion
        Return New ResultadoFaseImportacion With {.Codigo = codigo, .MensajeVisible = "No fue posible obtener el documento.", .PersistenciaConocida = True, .Reintentable = reintentable}
    End Function
End Class

Public NotInheritable Class PrepareImportExecutionStep
    Implements IImportExecutionStep

    Public ReadOnly Property FaseConfirmada As FaseImportacionServicio Implements IImportExecutionStep.FaseConfirmada
        Get
            Return FaseImportacionServicio.ExpedientePreparado
        End Get
    End Property

    Public Function Ejecutar(ByVal contexto As ContextoImportacionServicio,
                             ByVal intencion As IntencionImportacionServicio,
                             ByVal item As ResultadoElementoImportacion) As ResultadoFaseImportacion Implements IImportExecutionStep.Ejecutar
        If item Is Nothing OrElse item.ContenidoDescargado Is Nothing OrElse item.ContenidoDescargado.Length = 0 Then Return Fallo("RESOURCE_NOT_DOWNLOADED")
        Try
            Dim extension = ResolveTrustedExtension(item)
            If String.IsNullOrWhiteSpace(extension) Then Return Fallo("DOCUMENT_FILE_EXTENSION_UNSUPPORTED")
            item.RutaArchivoPreparado = Path.Combine(Path.GetTempPath(), "docuarchi-import-" & Guid.NewGuid().ToString("N") & extension)
            File.WriteAllBytes(item.RutaArchivoPreparado, item.ContenidoDescargado)
            item.ContenidoDescargado = Nothing
            Return New ResultadoFaseImportacion With {.Exitoso = True, .PersistenciaConocida = True}
        Catch
            Return Fallo("DOCUMENT_PREPARATION_FAILED")
        End Try
    End Function

    Private Shared Function Fallo(ByVal codigo As String) As ResultadoFaseImportacion
        Return New ResultadoFaseImportacion With {.Codigo = codigo, .MensajeVisible = "No fue posible preparar el documento.", .PersistenciaConocida = True, .Reintentable = True}
    End Function

    Private Shared Function ResolveTrustedExtension(ByVal item As ResultadoElementoImportacion) As String
        Dim format As String = If(item.MetadatosSii Is Nothing, String.Empty, item.MetadatosSii.Formato)
        Select Case If(format, String.Empty).Trim().TrimStart("."c).ToLowerInvariant()
            Case "pdf" : Return ".pdf"
            Case "tif", "tiff" : Return ".tif"
            Case "png" : Return ".png"
            Case "jpg", "jpeg" : Return ".jpg"
        End Select
        Select Case If(item.TipoContenido, String.Empty).Trim().ToLowerInvariant()
            Case "application/pdf" : Return ".pdf"
            Case "image/tiff" : Return ".tif"
            Case "image/png" : Return ".png"
            Case "image/jpeg" : Return ".jpg"
        End Select
        Return String.Empty
    End Function
End Class

Public NotInheritable Class PrepareImportIndicesExecutionStep
    Implements IImportExecutionStep

    Public ReadOnly Property FaseConfirmada As FaseImportacionServicio Implements IImportExecutionStep.FaseConfirmada
        Get
            Return FaseImportacionServicio.IndicesActualizados
        End Get
    End Property

    Public Function Ejecutar(ByVal contexto As ContextoImportacionServicio,
                             ByVal intencion As IntencionImportacionServicio,
                             ByVal item As ResultadoElementoImportacion) As ResultadoFaseImportacion Implements IImportExecutionStep.Ejecutar
        If item Is Nothing OrElse String.IsNullOrWhiteSpace(item.RutaArchivoPreparado) Then Return New ResultadoFaseImportacion With {.Codigo = "DOCUMENT_NOT_PREPARED", .MensajeVisible = "No fue posible preparar el documento.", .PersistenciaConocida = True}
        Return New ResultadoFaseImportacion With {.Exitoso = True, .PersistenciaConocida = True}
    End Function
End Class

Public NotInheritable Class StoreImportExecutionStep
    Implements IImportExecutionStep

    Private ReadOnly _metadata As IImportStorageMetadataRepository
    Private ReadOnly _documentTypes As IImportDocumentTypeResolver
    Private ReadOnly _storage As IImportDocumentStoragePort

    Public Sub New(ByVal metadata As IImportStorageMetadataRepository, ByVal documentTypes As IImportDocumentTypeResolver, ByVal storage As IImportDocumentStoragePort)
        If metadata Is Nothing OrElse documentTypes Is Nothing OrElse storage Is Nothing Then Throw New ArgumentNullException("dependency")
        _metadata = metadata
        _documentTypes = documentTypes
        _storage = storage
    End Sub

    Public ReadOnly Property FaseConfirmada As FaseImportacionServicio Implements IImportExecutionStep.FaseConfirmada
        Get
            Return FaseImportacionServicio.DocumentoAlmacenado
        End Get
    End Property

    Public Function Ejecutar(ByVal contexto As ContextoImportacionServicio,
                             ByVal intencion As IntencionImportacionServicio,
                             ByVal item As ResultadoElementoImportacion) As ResultadoFaseImportacion Implements IImportExecutionStep.Ejecutar
        If intencion Is Nothing OrElse intencion.ContextoOriginal Is Nothing OrElse item Is Nothing OrElse String.IsNullOrWhiteSpace(item.NombreTipoDocumental) Then Return Fallo("INVALID_STORAGE_CONTEXT", True)
        Dim command As ComandoAlmacenamientoImportacion = Nothing
        Try
            Dim metadata = _metadata.Resolver(contexto)
            If metadata Is Nothing Then Return Fallo("STORAGE_METADATA_UNAVAILABLE", True)
            If Not item.IdTipoDocumental.HasValue Then Return Fallo("INVALID_DOCUMENT_TYPE", True)
            Dim documentType As ResolucionTipoDocumentalImportacion
            Try
                documentType = _documentTypes.Resolver(contexto, item.IdTipoDocumental.Value, item.NombreTipoDocumental)
            Catch
                Return Fallo("DOCUMENT_TYPE_RESOLUTION_UNAVAILABLE", True)
            End Try
            If documentType Is Nothing OrElse Not documentType.Valida Then
                Return Fallo(If(documentType Is Nothing OrElse String.IsNullOrWhiteSpace(documentType.Codigo), "DOCUMENT_TYPE_RESOLUTION_FAILED", documentType.Codigo), True)
            End If
            command = New ComandoAlmacenamientoImportacion With {
            .RutaArchivo = item.RutaArchivoPreparado, .NombreGabinete = metadata.NombreGabinete,
            .Radicado = intencion.ContextoOriginal.Radicado, .NombreRutaWorkflow = metadata.NombreRutaWorkflow,
            .IdRutaWorkflow = intencion.ContextoOriginal.IdRuta, .IdTareaWorkflow = item.IdTareaDestino,
            .DescripcionTipo = documentType.NombreTipoDocumental,
            .IdTipoListaChequeo = documentType.IdTipoListaChequeo,
            .TipoAlmacenamiento = 2, .NombreClaseFormatoDocumento = metadata.NombreClaseFormatoDocumento,
            .NombreArchivoOrigen = item.NombreArchivo, .FormatoProveedor = If(item.MetadatosSii Is Nothing, Nothing, item.MetadatosSii.Formato),
            .TipoContenidoOrigen = item.TipoContenido}
            If item.MetadatosSii Is Nothing Then Return Fallo("SII_STORAGE_METADATA_UNAVAILABLE", True)
            Dim receipt As String = Nothing, taskBarcode As String = Nothing
            If New Class_DAT_ADIC_TAR().SolicitaReciboCodigoBarrasSII(item.IdTareaDestino, metadata.NombreRutaWorkflow,
                intencion.ContextoOriginal.IdRuta, receipt, taskBarcode) <> "YES" OrElse String.IsNullOrWhiteSpace(receipt) OrElse
                String.IsNullOrWhiteSpace(taskBarcode) Then Return Fallo("SII_RECEIPT_UNAVAILABLE", True)
            If String.IsNullOrWhiteSpace(item.MetadatosSii.RazonSocial) Then
                Dim subject As StruSiiCahcheInscripcion = Nothing
                Dim subjectResult = New ClassConsultaExpedienteSII().SolicitaEstructuraExpedienteSII(
                    item.MetadatosSii.Matricula, item.MetadatosSii.Proponente, metadata.NombreGabinete, subject)
                If Not String.Equals(subjectResult, "YES", StringComparison.OrdinalIgnoreCase) OrElse
                   String.IsNullOrWhiteSpace(subject.Rsocial) Then
                    Return FalloDiagnostico("SII_SUBJECT_METADATA_UNAVAILABLE", subjectResult)
                End If
                item.MetadatosSii.RazonSocial = subject.Rsocial
                item.MetadatosSii.NitCedula = subject.NitIdentificacion
            End If
            command.Radicado = receipt
            command.Campos = BuildSiiFields(metadata.NombreGabinete, taskBarcode, receipt, item.MetadatosSii)
        Catch ex As Exception
            Return FalloDiagnostico("DOCUMENT_STORAGE_PREPARATION_FAILED", ex.Message)
        End Try
        Try
            Return _storage.Almacenar(command)
        Finally
            Try
                If Not String.IsNullOrWhiteSpace(item.RutaArchivoPreparado) AndAlso File.Exists(item.RutaArchivoPreparado) Then File.Delete(item.RutaArchivoPreparado)
            Catch
                ' La limpieza no cambia el resultado persistente; no se expone la ruta temporal.
            End Try
            item.RutaArchivoPreparado = Nothing
        End Try
    End Function

    Private Shared Function BuildSiiFields(ByVal cabinet As String, ByVal barcode As String, ByVal receipt As String,
                                           ByVal sii As MetadatosDocumentoSii) As IList(Of CampoAlmacenamientoImportacion)
        Dim fields As New List(Of CampoAlmacenamientoImportacion)()
        AddField(fields, "CODBARRAS", Limit(barcode, 20)) : AddField(fields, "ENLASE", Limit(receipt, 20))
        Dim enrollment = If(String.Equals(cabinet, "RUP", StringComparison.OrdinalIgnoreCase), sii.Proponente, sii.Matricula)
        AddField(fields, "MATRICULA", Digits(enrollment))
        AddField(fields, "RAZONSOCIAL", Limit(sii.RazonSocial, If(String.Equals(cabinet, "RUP", StringComparison.OrdinalIgnoreCase), 40, 120)))
        AddField(fields, "NITCEDULA", Limit(sii.NitCedula, If(String.Equals(cabinet, "RUP", StringComparison.OrdinalIgnoreCase), 40, 20)))
        AddField(fields, "LIBRO", Digits(sii.Libro.Replace("RM", "").Replace("RE", "").Replace("RP", "")))
        AddField(fields, "INSCRIPCION", Digits(sii.Registro)) : AddField(fields, "RECIBOCAJA", Limit(receipt, 20))
        Dim dateField = If(String.Equals(cabinet, "ESAL", StringComparison.OrdinalIgnoreCase), "FECHAINSCRIP", "FECHAREGISTR")
        AddField(fields, dateField, SiiDate(sii.Fecha)) : AddField(fields, "ACTO", Digits(sii.Acto))
        Dim isMercantil = String.Equals(cabinet, "MERCANTIL", StringComparison.OrdinalIgnoreCase)
        AddField(fields, If(isMercantil, "DESCRIACTO", "DESCRIPCIONA"), Limit(sii.NombreActo, 40))
        Return fields
    End Function
    Private Shared Sub AddField(ByVal fields As IList(Of CampoAlmacenamientoImportacion), ByVal name As String, ByVal value As String)
        fields.Add(New CampoAlmacenamientoImportacion With {.Nombre = name, .Valor = If(value, String.Empty)})
    End Sub
    Private Shared Function Digits(ByVal value As String) As String
        Return New String(If(value, String.Empty).Where(Function(c) Char.IsDigit(c)).ToArray())
    End Function
    Private Shared Function Limit(ByVal value As String, ByVal maximum As Integer) As String
        Dim clean = If(value, String.Empty).Trim()
        Return If(clean.Length > maximum, clean.Substring(0, maximum), clean)
    End Function
    Private Shared Function SiiDate(ByVal value As String) As String
        Dim parsed As DateTime
        If DateTime.TryParseExact(If(value, String.Empty), "yyyyMMdd", Globalization.CultureInfo.InvariantCulture,
            Globalization.DateTimeStyles.None, parsed) Then Return parsed.ToString("yyyy-MM-dd", Globalization.CultureInfo.InvariantCulture)
        Return String.Empty
    End Function

    Private Shared Function Fallo(ByVal codigo As String, ByVal conocida As Boolean) As ResultadoFaseImportacion
        Return New ResultadoFaseImportacion With {.Codigo = codigo, .MensajeVisible = "No fue posible almacenar el documento.", .PersistenciaConocida = conocida, .Reintentable = conocida}
    End Function

    Private Shared Function FalloDiagnostico(ByVal codigo As String, ByVal value As String) As ResultadoFaseImportacion
        Dim diagnosticText As String = System.Text.RegularExpressions.Regex.Replace(If(value, String.Empty), "[\r\n\t]+", " ").Trim()
        If diagnosticText.Length = 0 Then diagnosticText = "La preparación del almacenamiento falló sin informar detalle."
        diagnosticText = System.Text.RegularExpressions.Regex.Replace(diagnosticText, "(?i)\b(password|pwd|contrase(?:ña|na)|token|authorization|cookie|connection\s*string|cadena\s+de\s+conexi(?:ó|o)n)\b\s*[:=]\s*[^;,\s]+", "$1=[REDACTED]")
        diagnosticText = System.Text.RegularExpressions.Regex.Replace(diagnosticText, "(?i)(https?://[^?\s]+)\?[^\s]+", "$1?[REDACTED]")
        If diagnosticText.Length > 500 Then diagnosticText = diagnosticText.Substring(0, 500) & "…"
        Return New ResultadoFaseImportacion With {.Codigo=codigo, .MensajeVisible=diagnosticText, .PersistenciaConocida=True, .Reintentable=True}
    End Function
End Class

Public NotInheritable Class CompleteImportExecutionStep
    Implements IImportExecutionStep
    Private ReadOnly _phase As FaseImportacionServicio

    Public Sub New(ByVal phase As FaseImportacionServicio)
        If phase <> FaseImportacionServicio.CacheActualizado AndAlso phase <> FaseImportacionServicio.Completada Then Throw New ArgumentOutOfRangeException("phase")
        _phase = phase
    End Sub

    Public ReadOnly Property FaseConfirmada As FaseImportacionServicio Implements IImportExecutionStep.FaseConfirmada
        Get
            Return _phase
        End Get
    End Property

    Public Function Ejecutar(ByVal contexto As ContextoImportacionServicio,
                             ByVal intencion As IntencionImportacionServicio,
                             ByVal item As ResultadoElementoImportacion) As ResultadoFaseImportacion Implements IImportExecutionStep.Ejecutar
        Return New ResultadoFaseImportacion With {.Exitoso = True, .PersistenciaConocida = True}
    End Function
End Class
