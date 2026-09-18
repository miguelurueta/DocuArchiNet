Imports System

Public NotInheritable Class ImportItemResultMapper
    Public Function Map(ByVal item As SnapshotItemReconciliacionImportacion, ByVal taskId As Long) As ImportItemResultDto
        If item Is Nothing Then Throw New ArgumentNullException("item")
        Dim consistency = Classify(item, taskId)
        Dim dto As New ImportItemResultDto With {.ClientItemId=item.ClientItemId,.ExternalKey=item.ExternalKey,.DocumentId=item.IdDocumento,.TaskId=taskId,.ReachedPhase=item.Fase.ToString(),.PersistenceKnown=item.PersistenciaConocida,.CorrelationId=item.CorrelationId,.DocumentName=item.NombreDocumento,.ContentType=item.TipoContenido,.Retryable=item.Reintentable AndAlso item.PersistenciaConocida AndAlso Not item.IdDocumento.HasValue}
        Select Case consistency
            Case ConsistenciaDocumentoImportacion.Confirmado
                dto.Status="Disponible" : dto.Message="Documento disponible."
            Case ConsistenciaDocumentoImportacion.ResultadoIncierto
                dto.Status=If(item.Fase=FaseImportacionServicio.ResultadoIncierto,"ResultadoIncierto","Verificando") : dto.ErrorCode="IMPORT_RESULT_UNCERTAIN" : dto.Message="Se está verificando el resultado."
            Case ConsistenciaDocumentoImportacion.RelacionDuplicada
                dto.Status="Inconsistente" : dto.ErrorCode="DOCUMENT_RELATION_DUPLICATED" : dto.Message="La relación documental requiere revisión."
            Case ConsistenciaDocumentoImportacion.TareaDistinta
                dto.Status="Inconsistente" : dto.ErrorCode="DOCUMENT_TASK_MISMATCH" : dto.Message="La relación documental requiere revisión."
            Case ConsistenciaDocumentoImportacion.RelacionAusente
                dto.Status="Inconsistente" : dto.ErrorCode="DOCUMENT_RELATION_MISSING" : dto.Message="La relación documental requiere revisión."
            Case Else
                dto.Status=VisibleStatus(item.Fase) : dto.ErrorCode=SafeCode(item.CodigoError) : dto.Message=SafeMessage(item.MensajeVisible, item.Fase, dto.ErrorCode)
        End Select
        If dto.Status<>"Disponible" Then dto.DocumentId=Nothing : dto.DocumentName=Nothing : dto.ContentType=Nothing
        Return dto
    End Function

    Public Function MapExpedientEffects(ByVal clientItemId As String,
                                        ByVal documento As DocumentoRelacionadoImportacion) As ImportItemExpedientEffectsDto
        If documento Is Nothing Then Throw New ArgumentNullException("documento")
        Dim dto As New ImportItemExpedientEffectsDto With {
            .ClientItemId = clientItemId,
            .InscriptionKey = documento.ClaveInscripcion,
            .ExpedientId = documento.IdExpedienteEsperado,
            .ExpedientStatus = If(documento.IdExpedienteEsperado.HasValue, "Confirmado", "Pendiente"),
            .DestinationStatus = documento.EstadoDestino.ToString(),
            .RelationStatus = documento.EstadoRelacion.ToString(),
            .LinkCacheStatus = documento.EstadoCache.ToString(),
            .CabinetIndexStatus = documento.EstadoIndiceGabinete.ToString(),
            .ElectronicIndexSqlStatus = documento.EstadoIndiceSql.ToString(),
            .ElectronicIndexXmlStatus = documento.EstadoIndiceXml.ToString(),
            .ReconciliationStatus = documento.EstadoReconciliacion.ToString()
        }
        dto.ResultCode = ExpedientEffectCode(documento)
        dto.Retryable = dto.ResultCode = ImportExpedientResultCodes.CacheWriteUncertain OrElse
                        dto.ResultCode = ImportExpedientResultCodes.IndexUpdateFailed
        dto.Message = ExpedientEffectMessage(dto.ResultCode)
        If dto.ResultCode <> ImportExpedientResultCodes.Confirmed Then dto.ExpedientId = Nothing
        Return dto
    End Function

    Private Shared Function ExpedientEffectCode(ByVal documento As DocumentoRelacionadoImportacion) As String
        If Not documento.IdExpedienteEsperado.HasValue Then Return ImportExpedientResultCodes.ExpedientUnresolved
        If documento.EstadoDestino = EstadoEfectoExpedienteImportacion.Conflicto Then Return ImportExpedientResultCodes.DestinationConflict
        Select Case documento.EstadoRelacion
            Case EstadoRelacionDocumentoExpediente.Ausente : Return ImportExpedientResultCodes.RelationMissing
            Case EstadoRelacionDocumentoExpediente.Duplicada : Return ImportExpedientResultCodes.RelationDuplicated
            Case EstadoRelacionDocumentoExpediente.Cruzada : Return ImportExpedientResultCodes.RelationCrossed
            Case EstadoRelacionDocumentoExpediente.ResultadoIncierto : Return ImportExpedientResultCodes.EffectUncertain
        End Select
        If documento.EstadoCache = EstadoEfectoExpedienteImportacion.Conflicto Then Return ImportExpedientResultCodes.CacheConflict
        If documento.EstadoCache = EstadoEfectoExpedienteImportacion.ResultadoIncierto Then Return ImportExpedientResultCodes.CacheWriteUncertain
        If documento.EstadoIndiceGabinete = EstadoEfectoExpedienteImportacion.Fallido Then Return ImportExpedientResultCodes.IndexUpdateFailed
        If documento.EstadoIndiceSql <> EstadoEfectoExpedienteImportacion.Confirmado Then Return ImportExpedientResultCodes.ElectronicIndexSqlMissing
        If documento.EstadoIndiceXml <> EstadoEfectoExpedienteImportacion.Confirmado Then Return ImportExpedientResultCodes.ElectronicIndexXmlMissing
        If documento.EstadoReconciliacion <> EstadoEfectoExpedienteImportacion.Confirmado Then Return ImportExpedientResultCodes.EffectUncertain
        Return ImportExpedientResultCodes.Confirmed
    End Function

    Private Shared Function ExpedientEffectMessage(ByVal code As String) As String
        If code = ImportExpedientResultCodes.Confirmed Then Return "Expediente y efectos documentales confirmados."
        If code = ImportExpedientResultCodes.ExpedientUnresolved Then Return "No fue posible resolver el expediente requerido."
        If code = ImportExpedientResultCodes.DestinationConflict Then Return "El destino documental requiere revisión."
        If code = ImportExpedientResultCodes.RelationMissing OrElse
           code = ImportExpedientResultCodes.RelationDuplicated OrElse
           code = ImportExpedientResultCodes.RelationCrossed Then Return "La relación documental requiere revisión."
        If code = ImportExpedientResultCodes.CacheConflict OrElse code = ImportExpedientResultCodes.CacheWriteUncertain Then Return "La verificación de caché requiere revisión."
        If code = ImportExpedientResultCodes.IndexUpdateFailed OrElse
           code = ImportExpedientResultCodes.ElectronicIndexSqlMissing OrElse
           code = ImportExpedientResultCodes.ElectronicIndexXmlMissing Then Return "La indexación documental requiere verificación."
        Return "El resultado requiere reconciliación."
    End Function

    Private Shared Function Classify(ByVal item As SnapshotItemReconciliacionImportacion, ByVal taskId As Long) As ConsistenciaDocumentoImportacion
        If item.Fase=FaseImportacionServicio.ResultadoIncierto OrElse (item.IdDocumento.HasValue AndAlso Not item.PersistenciaConocida) Then Return ConsistenciaDocumentoImportacion.ResultadoIncierto
        If item.IdTareaDestino<>taskId OrElse item.CantidadRelacionesOtraTarea>0 Then Return ConsistenciaDocumentoImportacion.TareaDistinta
        If item.IdDocumento.HasValue AndAlso item.PersistenciaConocida Then
            If item.CantidadDocumentos<>1 Then Return ConsistenciaDocumentoImportacion.RelacionAusente
            If item.CantidadRelaciones=0 Then Return ConsistenciaDocumentoImportacion.RelacionAusente
            If item.CantidadRelaciones>1 Then Return ConsistenciaDocumentoImportacion.RelacionDuplicada
            Return ConsistenciaDocumentoImportacion.Confirmado
        End If
        Return ConsistenciaDocumentoImportacion.NoConfirmado
    End Function

    Private Shared Function VisibleStatus(ByVal phase As FaseImportacionServicio) As String
        Select Case phase
            Case FaseImportacionServicio.Completada, FaseImportacionServicio.Reconciliada : Return "Completado"
            Case FaseImportacionServicio.Parcial : Return "Parcial"
            Case FaseImportacionServicio.Detenida : Return "Detenido"
            Case FaseImportacionServicio.FallidaAntesDePersistir : Return "Fallido"
            Case FaseImportacionServicio.ResultadoIncierto : Return "ResultadoIncierto"
            Case Else : Return "Verificando"
        End Select
    End Function
    Private Shared Function SafeCode(ByVal value As String) As String
        If String.IsNullOrWhiteSpace(value) Then Return Nothing
        If value.Length>64 OrElse Not Text.RegularExpressions.Regex.IsMatch(value,"^[A-Z0-9_]+$") Then Return "IMPORT_RECONCILIATION_ERROR"
        Return value
    End Function
    Private Shared Function SafeMessage(ByVal value As String, ByVal phase As FaseImportacionServicio, ByVal code As String) As String
        If Not String.IsNullOrWhiteSpace(value) AndAlso Not String.IsNullOrWhiteSpace(code) AndAlso code.StartsWith("DOCUMENT_STORAGE_", StringComparison.Ordinal) Then Return value
        If phase=FaseImportacionServicio.FallidaAntesDePersistir Then Return "No fue posible completar la importación."
        If phase=FaseImportacionServicio.Detenida Then Return "La importación fue detenida."
        If phase=FaseImportacionServicio.Parcial Then Return "La importación se completó parcialmente."
        Return "El resultado de importación está en proceso."
    End Function
End Class

Public NotInheritable Class ImportExpedientResultCodes
    Public Const Confirmed As String = "EXPEDIENT_EFFECTS_CONFIRMED"
    Public Const ExpedientUnresolved As String = "EXPEDIENT_UNRESOLVED"
    Public Const DestinationConflict As String = "DOCUMENT_DESTINATION_CONFLICT"
    Public Const RelationMissing As String = "DOCUMENT_RELATION_MISSING"
    Public Const RelationDuplicated As String = "DOCUMENT_RELATION_DUPLICATED"
    Public Const RelationCrossed As String = "DOCUMENT_RELATION_CROSSED"
    Public Const CacheConflict As String = "DOCUMENT_LINK_CACHE_CONFLICT"
    Public Const CacheWriteUncertain As String = "DOCUMENT_LINK_CACHE_WRITE_UNCERTAIN"
    Public Const IndexUpdateFailed As String = "DOCUMENT_INDEX_UPDATE_FAILED"
    Public Const ElectronicIndexSqlMissing As String = "ELECTRONIC_INDEX_SQL_MISSING"
    Public Const ElectronicIndexXmlMissing As String = "ELECTRONIC_INDEX_XML_MISSING"
    Public Const EffectUncertain As String = "EXPEDIENT_EFFECT_UNCERTAIN"
End Class
