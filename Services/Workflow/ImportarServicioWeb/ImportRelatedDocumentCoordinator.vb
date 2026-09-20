Imports System

' Procesa secuencialmente el universo completo descubierto por gabinete y ENLASE.
Public NotInheritable Class ImportRelatedDocumentCoordinator
    Private ReadOnly _documents As IImportRelatedDocumentRepository
    Private ReadOnly _relations As IImportDocumentExpedientRelationPort
    Private ReadOnly _cache As IImportDocumentLinkCacheRepository
    Private ReadOnly _indices As IImportDocumentIndexUpdater
    Private ReadOnly _electronicIndex As IImportElectronicIndexVerifier
    Private ReadOnly _planBuilder As ImportRelatedDocumentPlan

    Public Sub New(ByVal documents As IImportRelatedDocumentRepository,
                   ByVal relations As IImportDocumentExpedientRelationPort,
                   ByVal cache As IImportDocumentLinkCacheRepository,
                   ByVal indices As IImportDocumentIndexUpdater,
                   ByVal electronicIndex As IImportElectronicIndexVerifier,
                   ByVal planBuilder As ImportRelatedDocumentPlan)
        If documents Is Nothing OrElse relations Is Nothing OrElse cache Is Nothing OrElse indices Is Nothing OrElse
           electronicIndex Is Nothing OrElse planBuilder Is Nothing Then Throw New ArgumentNullException("dependency")
        _documents = documents : _relations = relations : _cache = cache : _indices = indices
        _electronicIndex = electronicIndex : _planBuilder = planBuilder
    End Sub

    Public Function Procesar(ByVal contexto As ContextoImportacionServicio,
                             ByVal intencion As IntencionImportacionServicio,
                             ByVal planExpedientes As PlanExpedienteImportacion,
                             ByVal nombreGabinete As String,
                             ByVal radicadoSii As String) As PlanDocumentosRelacionadosImportacion
        Dim discovered = _documents.ObtenerPorEnlace(contexto, nombreGabinete, radicadoSii)
        If planExpedientes.Modo = ModoExpedienteImportacion.GestionarExpediente Then
            For Each document In discovered
                Dim cached = _cache.Obtener(contexto, document.IdImagen, document.NombreGabinete)
                If cached IsNot Nothing Then document.IdExpedienteEsperado = cached.IdExpedienteEsperado
            Next
        End If
        Dim plan = _planBuilder.Construir(intencion, planExpedientes, discovered)
        If plan.Estado <> EstadoEfectoExpedienteImportacion.Confirmado Then Return plan

        For Each document In plan.Documentos
            If Not _documents.Persistir(contexto, intencion.Id, document) Then
                plan.Estado = EstadoEfectoExpedienteImportacion.Fallido
                plan.Codigo = "RELATED_DOCUMENT_PERSISTENCE_FAILED"
                Return plan
            End If
        Next

        For Each document In plan.Documentos
            Dim inscription = FindInscription(planExpedientes, document.ClaveInscripcion)
            If inscription Is Nothing Then
                plan.Estado = EstadoEfectoExpedienteImportacion.Conflicto
                plan.Codigo = "RELATED_DOCUMENT_INSCRIPTION_UNRESOLVED"
                Return plan
            End If
            Dim failureCode As String = Nothing
            If Not ProcessOne(contexto, intencion.Id, document, inscription, planExpedientes.Modo, failureCode) Then
                plan.Estado = If(document.EstadoRelacion = EstadoRelacionDocumentoExpediente.ResultadoIncierto,
                                  EstadoEfectoExpedienteImportacion.ResultadoIncierto,
                                  EstadoEfectoExpedienteImportacion.Conflicto)
                plan.Codigo = If(String.IsNullOrWhiteSpace(failureCode), "RELATED_DOCUMENTS_NOT_CONFIRMED", failureCode)
                Return plan
            End If
        Next
        plan.Estado = EstadoEfectoExpedienteImportacion.Confirmado
        Return plan
    End Function

    Private Function ProcessOne(ByVal contexto As ContextoImportacionServicio,
                                ByVal intentId As String,
                                ByVal document As DocumentoRelacionadoImportacion,
                                ByVal inscription As InscripcionImportacion,
                                ByVal mode As ModoExpedienteImportacion,
                                ByRef failureCode As String) As Boolean
        If mode = ModoExpedienteImportacion.SinExpediente Then
            document.EstadoRelacion = EstadoRelacionDocumentoExpediente.NoConsultada
            document.EstadoCache = EstadoEfectoExpedienteImportacion.NoAplica
            document.EstadoIndiceSql = EstadoEfectoExpedienteImportacion.NoAplica
            document.EstadoIndiceXml = EstadoEfectoExpedienteImportacion.NoAplica
            Dim documentIndex = _indices.Actualizar(contexto, document, inscription)
            document.EstadoIndiceGabinete = documentIndex.Estado
            document.EstadoReconciliacion = documentIndex.Estado
            If Not _documents.Persistir(contexto, intentId, document) Then failureCode = "RELATED_DOCUMENT_PERSISTENCE_FAILED" : Return False
            If documentIndex.Estado <> EstadoEfectoExpedienteImportacion.Confirmado Then
                failureCode = If(String.IsNullOrWhiteSpace(documentIndex.Codigo), "DOCUMENT_INDEX_NOT_CONFIRMED", documentIndex.Codigo)
                Return False
            End If
            Return True
        End If
        Dim relation = _relations.Vincular(contexto, document)
        document.EstadoRelacion = relation.Relacion
        If Not _documents.Persistir(contexto, intentId, document) Then
            failureCode = "RELATED_DOCUMENT_PERSISTENCE_FAILED" : Return False
        End If
        If relation.Estado <> EstadoEfectoExpedienteImportacion.Confirmado OrElse
           relation.Relacion <> EstadoRelacionDocumentoExpediente.Correcta Then
            failureCode = If(relation.Relacion = EstadoRelacionDocumentoExpediente.ResultadoIncierto,
                             "RELATED_DOCUMENT_RELATION_UNCERTAIN", "RELATED_DOCUMENT_RELATION_NOT_CONFIRMED")
            Return False
        End If

        Dim cacheEntry As New EntradaCacheVinculoDocumentoImportacion With {
            .IdTarea = contexto.IdTarea, .IdImagen = document.IdImagen, .NombreGabinete = document.NombreGabinete,
            .IdExpedienteEsperado = document.IdExpedienteEsperado.Value, .RadicadoSii = document.RadicadoSii,
            .EstadoRelacion = EstadoRelacionDocumentoExpediente.Correcta, .FechaVerificacionUtc = DateTime.UtcNow
        }
        Dim cached = _cache.RegistrarVerificado(contexto, cacheEntry)
        document.EstadoCache = cached.Estado
        If Not _documents.Persistir(contexto, intentId, document) Then
            failureCode = "RELATED_DOCUMENT_PERSISTENCE_FAILED" : Return False
        End If
        If cached.Estado <> EstadoEfectoExpedienteImportacion.Confirmado Then
            failureCode = "RELATED_DOCUMENT_CACHE_NOT_CONFIRMED" : Return False
        End If

        Dim indexed = _indices.Actualizar(contexto, document, inscription)
        document.EstadoIndiceGabinete = indexed.Estado
        If Not _documents.Persistir(contexto, intentId, document) Then
            failureCode = "RELATED_DOCUMENT_PERSISTENCE_FAILED" : Return False
        End If
        If indexed.Estado <> EstadoEfectoExpedienteImportacion.Confirmado Then
            failureCode = If(String.IsNullOrWhiteSpace(indexed.Codigo), "RELATED_DOCUMENT_CABINET_INDEX_NOT_CONFIRMED", indexed.Codigo)
            Return False
        End If

        Dim evidence = _electronicIndex.Verificar(contexto, document)
        document.EstadoIndiceSql = If(evidence.SqlConfirmado, EstadoEfectoExpedienteImportacion.Confirmado, EstadoEfectoExpedienteImportacion.Ausente)
        document.EstadoIndiceXml = If(evidence.XmlConfirmado, EstadoEfectoExpedienteImportacion.Confirmado, EstadoEfectoExpedienteImportacion.Ausente)
        document.EstadoReconciliacion = evidence.Estado
        If Not _documents.Persistir(contexto, intentId, document) Then
            failureCode = "RELATED_DOCUMENT_PERSISTENCE_FAILED" : Return False
        End If
        If Not evidence.SqlConfirmado Then failureCode = "RELATED_DOCUMENT_SQL_INDEX_MISSING" : Return False
        If Not evidence.XmlConfirmado Then failureCode = "RELATED_DOCUMENT_XML_INDEX_MISSING" : Return False
        If evidence.Estado <> EstadoEfectoExpedienteImportacion.Confirmado Then
            failureCode = "RELATED_DOCUMENT_RECONCILIATION_UNCERTAIN" : Return False
        End If
        Return True
    End Function

    Private Shared Function FindInscription(ByVal plan As PlanExpedienteImportacion,
                                            ByVal key As String) As InscripcionImportacion
        For Each value In plan.Inscripciones
            If String.Equals(value.ClaveInscripcion, key, StringComparison.Ordinal) Then Return value
        Next
        Return Nothing
    End Function
End Class
