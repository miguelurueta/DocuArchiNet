Imports System
Imports System.Collections.Generic

' Contrato v1 compartido, independiente de infraestructura web y persistencia.
<Serializable()> Public Class ErrorImportacionServicioDto
    Public Property Codigo As String
    Public Property MensajeVisible As String
    Public Property ReferenciaTrazabilidad As String
    Public Property EsReintentable As Boolean
End Class

<Serializable()> Public MustInherit Class SolicitudImportacionServicioDto
    Public Sub New()
        SchemaVersion = "1.0"
    End Sub
    Public Property SchemaVersion As String
    Public Property OperationId As String
    Public Property CorrelationId As String
    Public Property TaskId As Long
    Public Property ProviderId As String
End Class

<Serializable()> Public MustInherit Class RespuestaImportacionServicioDto
    Public Sub New()
        SchemaVersion = "1.0"
    End Sub
    Public Property SchemaVersion As String
    Public Property OperationId As String
    Public Property CorrelationId As String
    Public Property [Error] As ErrorImportacionServicioDto
End Class

<Serializable()> Public Class ResolveCapabilitiesRequestDto
    Inherits SolicitudImportacionServicioDto
End Class

<Serializable()> Public Class ProviderCapabilityDto
    Public Property Codigo As String
    Public Property Habilitada As Boolean
    Public Property TimeoutSeconds As Nullable(Of Integer)
End Class

<Serializable()> Public Class ResolveCapabilitiesResponseDto
    Inherits RespuestaImportacionServicioDto
    Public Sub New()
        Capabilities = New List(Of ProviderCapabilityDto)()
    End Sub
    Public Property ProviderId As String
    Public Property ContextAllowed As Boolean
    Public Property Capabilities As IList(Of ProviderCapabilityDto)
End Class

<Serializable()> Public Class QueryItemsRequestDto
    Inherits SolicitudImportacionServicioDto
    Public Property ContinuationToken As String
    Public Property PageSize As Nullable(Of Integer)
End Class

<Serializable()> Public Class ExternalItemDto
    Public Property ExternalKey As String
    Public Property DisplayName As String
    Public Property ContentType As String
    Public Property Length As Nullable(Of Long)
    Public Property PreviewAvailable As Boolean
End Class

<Serializable()> Public Class QueryItemsResponseDto
    Inherits RespuestaImportacionServicioDto
    Public Sub New()
        Items = New List(Of ExternalItemDto)()
    End Sub
    Public Property Items As IList(Of ExternalItemDto)
    Public Property ContinuationToken As String
End Class

<Serializable()> Public Class GetPreviewRequestDto
    Inherits SolicitudImportacionServicioDto
    Public Property ExternalKey As String
End Class

<Serializable()> Public Class GetPreviewResponseDto
    Inherits RespuestaImportacionServicioDto
    Public Property ExternalKey As String
    Public Property DescriptorId As String
    Public Property ContentType As String
    Public Property Length As Nullable(Of Long)
    Public Property Disposition As String
    Public Property ExpiresAtUtc As Nullable(Of DateTime)
End Class

<Serializable()> Public Class ImportItemSelectionDto
    Public Property ExternalKey As String
    Public Property ClientItemId As String
End Class

<Serializable()> Public Class PreflightImportRequestDto
    Inherits SolicitudImportacionServicioDto
    Public Sub New()
        Items = New List(Of ImportItemSelectionDto)()
    End Sub
    Public Property Items As IList(Of ImportItemSelectionDto)
End Class

<Serializable()> Public Class ImportRequirementDto
    Public Property Codigo As String
    Public Property Satisfecho As Boolean
    Public Property MensajeVisible As String
End Class

<Serializable()> Public Class DocumentCommandDto
    Public Property ClientItemId As String
    Public Property ExternalKey As String
    Public Property DocumentTypeId As Nullable(Of Integer)
    Public Property FileName As String
    Public Property ContentType As String
End Class

<Serializable()> Public Class ImportItemResultDto
    Public Property ClientItemId As String
    Public Property ExternalKey As String
    Public Property Status As String
    Public Property DocumentId As Nullable(Of Long)
    Public Property ErrorCode As String
    Public Property Message As String
End Class

<Serializable()> Public Class PreflightImportResponseDto
    Inherits RespuestaImportacionServicioDto
    Public Sub New()
        Requirements = New List(Of ImportRequirementDto)()
        Commands = New List(Of DocumentCommandDto)()
    End Sub
    Public Property IsValid As Boolean
    Public Property Requirements As IList(Of ImportRequirementDto)
    Public Property Commands As IList(Of DocumentCommandDto)
End Class

<Serializable()> Public Class CreateImportIntentRequestDto
    Inherits SolicitudImportacionServicioDto
    Public Sub New()
        Items = New List(Of ImportItemSelectionDto)()
    End Sub
    Public Property IdempotencyKey As String
    Public Property Items As IList(Of ImportItemSelectionDto)
End Class

<Serializable()> Public Class CreateImportIntentResponseDto
    Inherits RespuestaImportacionServicioDto
    Public Property IntentId As String
    Public Property Status As String
    Public Property VersionToken As String
    Public Property Reused As Boolean
End Class

<Serializable()> Public Class ExecuteImportIntentRequestDto
    Inherits SolicitudImportacionServicioDto
    Public Property IntentId As String
    Public Property VersionToken As String
End Class

<Serializable()> Public Class ExecuteImportIntentResponseDto
    Inherits RespuestaImportacionServicioDto
    Public Property IntentId As String
    Public Property Accepted As Boolean
    Public Property Status As String
    Public Property VersionToken As String
End Class

<Serializable()> Public Class GetImportIntentRequestDto
    Inherits SolicitudImportacionServicioDto
    Public Property IntentId As String
End Class

<Serializable()> Public Class GetImportIntentResponseDto
    Inherits RespuestaImportacionServicioDto
    Public Sub New()
        Items = New List(Of ImportItemResultDto)()
    End Sub
    Public Property IntentId As String
    Public Property Status As String
    Public Property VersionToken As String
    Public Property Items As IList(Of ImportItemResultDto)
End Class

<Serializable()> Public Class ReconcileImportIntentRequestDto
    Inherits SolicitudImportacionServicioDto
    Public Property IntentId As String
End Class

<Serializable()> Public Class ReconcileImportIntentResponseDto
    Inherits RespuestaImportacionServicioDto
    Public Sub New()
        Items = New List(Of ImportItemResultDto)()
    End Sub
    Public Property IntentId As String
    Public Property Status As String
    Public Property ConfirmedDocumentCount As Integer
    Public Property Items As IList(Of ImportItemResultDto)
End Class
