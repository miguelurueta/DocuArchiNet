Imports System

Public NotInheritable Class ImportPreviewSnapshot
    Public Property Id As Long
    Public Property DescriptorHash As Byte()
    Public Property ResourceHash As Byte()
    Public Property UserId As Integer
    Public Property TaskId As Long
    Public Property ProviderId As String
    Public Property ContentType As String
    Public Property ContentLength As Long
    Public Property ContentDisposition As String
    Public Property SafeFileName As String
    Public Property Content As Byte()
    Public Property Status As String
    Public Property ExpiresUtc As DateTime
    Public Property CreatedUtc As DateTime
End Class

Public NotInheritable Class ImportPreviewAuthority
    Public Property UserId As Integer
    Public Property TaskId As Long
    Public Property ProviderId As String
End Class

Public Interface IImportPreviewDescriptorRepository
    Function Create(ByVal snapshot As ImportPreviewSnapshot) As Boolean
    Function GetAvailableMetadata(ByVal descriptorHash As Byte(), ByVal authority As ImportPreviewAuthority,
                                  ByVal utcNow As DateTime) As ImportPreviewSnapshot
    Function ClaimAndLoad(ByVal descriptorHash As Byte(), ByVal authority As ImportPreviewAuthority,
                          ByVal utcNow As DateTime) As ImportPreviewSnapshot
    Function MarkConsumed(ByVal id As Long, ByVal utcNow As DateTime) As Boolean
    Function CleanupExpired(ByVal utcNow As DateTime) As Integer
End Interface

Public NotInheritable Class ImportPreviewCreationResult
    Public Property DescriptorId As String
    Public Property Snapshot As ImportPreviewSnapshot
End Class

Public NotInheritable Class SiiPreviewContent
    Public Property ExternalKey As String
    Public Property ContentType As String
    Public Property FileName As String
    Public Property Content As Byte()
End Class
