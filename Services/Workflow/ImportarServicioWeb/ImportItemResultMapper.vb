Imports System

Public NotInheritable Class ImportItemResultMapper
    Public Function Map(ByVal item As SnapshotItemReconciliacionImportacion, ByVal taskId As Long) As ImportItemResultDto
        If item Is Nothing Then Throw New ArgumentNullException("item")
        Dim consistency = Classify(item, taskId)
        Dim dto As New ImportItemResultDto With {.ClientItemId=item.ClientItemId,.ExternalKey=item.ExternalKey,.DocumentId=item.IdDocumento,.TaskId=taskId,.ReachedPhase=item.Fase.ToString(),.PersistenceKnown=item.PersistenciaConocida,.CorrelationId=item.CorrelationId,.DocumentName=item.NombreDocumento,.ContentType=item.TipoContenido,.Retryable=False}
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
                dto.Status=VisibleStatus(item.Fase) : dto.ErrorCode=SafeCode(item.CodigoError) : dto.Message=SafeMessage(item.MensajeVisible, item.Fase) : dto.Retryable=item.Reintentable AndAlso Not item.PersistenciaConocida
        End Select
        If dto.Status<>"Disponible" Then dto.DocumentId=Nothing : dto.DocumentName=Nothing : dto.ContentType=Nothing
        Return dto
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
    Private Shared Function SafeMessage(ByVal value As String, ByVal phase As FaseImportacionServicio) As String
        If phase=FaseImportacionServicio.FallidaAntesDePersistir Then Return "No fue posible completar la importación."
        If phase=FaseImportacionServicio.Detenida Then Return "La importación fue detenida."
        If phase=FaseImportacionServicio.Parcial Then Return "La importación se completó parcialmente."
        Return "El resultado de importación está en proceso."
    End Function
End Class
