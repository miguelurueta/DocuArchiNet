Imports System
Imports System.Collections.Generic
Imports System.Text.RegularExpressions

Public NotInheritable Class ImportPreviewContentService
    Private Shared ReadOnly InlineTypes As ISet(Of String) = New HashSet(Of String)(
        New String() {"application/pdf", "image/png", "image/jpeg", "image/tiff"}, StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _repository As IImportPreviewDescriptorRepository

    Public Sub New(ByVal repository As IImportPreviewDescriptorRepository)
        If repository Is Nothing Then Throw New ArgumentNullException("repository")
        _repository = repository
    End Sub

    Public Function Head(ByVal descriptorId As String, ByVal authority As ImportPreviewAuthority,
                         ByVal utcNow As DateTime) As ImportPreviewSnapshot
        Dim hash = ImportPreviewDescriptorService.DescriptorHash(descriptorId)
        If hash Is Nothing Then Return Nothing
        Return _repository.GetAvailableMetadata(hash, authority, utcNow.ToUniversalTime())
    End Function

    Public Function Claim(ByVal descriptorId As String, ByVal authority As ImportPreviewAuthority,
                          ByVal utcNow As DateTime) As ImportPreviewSnapshot
        Dim hash = ImportPreviewDescriptorService.DescriptorHash(descriptorId)
        If hash Is Nothing Then Return Nothing
        Return _repository.ClaimAndLoad(hash, authority, utcNow.ToUniversalTime())
    End Function

    Public Function Complete(ByVal snapshot As ImportPreviewSnapshot, ByVal utcNow As DateTime) As Boolean
        Return snapshot IsNot Nothing AndAlso _repository.MarkConsumed(snapshot.Id, utcNow.ToUniversalTime())
    End Function

    Public Shared Function ResolveDisposition(ByVal contentType As String) As String
        Dim normalized = If(contentType, String.Empty).Trim()
        If InlineTypes.Contains(normalized) Then Return "inline"
        Return String.Empty
    End Function

    Public Shared Function SafeFileName(ByVal value As String, ByVal contentType As String) As String
        Dim extension = If(String.Equals(contentType, "application/pdf", StringComparison.OrdinalIgnoreCase), ".pdf",
            If(String.Equals(contentType, "image/png", StringComparison.OrdinalIgnoreCase), ".png",
            If(String.Equals(contentType, "image/jpeg", StringComparison.OrdinalIgnoreCase), ".jpg", ".tiff")))
        Dim candidate = Regex.Replace(If(value, String.Empty), "[^A-Za-z0-9._-]", "_").Trim("."c, "_"c)
        If candidate.Length = 0 Then candidate = "preview" & extension
        If candidate.Length > 120 Then candidate = candidate.Substring(0, 120)
        If Not candidate.EndsWith(extension, StringComparison.OrdinalIgnoreCase) Then candidate &= extension
        Return candidate
    End Function
End Class
