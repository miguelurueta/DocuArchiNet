Imports System
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions

Public NotInheritable Class ImportPreviewDescriptorService
    Private ReadOnly _repository As IImportPreviewDescriptorRepository
    Private ReadOnly _maximumBytes As Long
    Private ReadOnly _ttl As TimeSpan

    Public Sub New(ByVal repository As IImportPreviewDescriptorRepository, ByVal maximumBytes As Long, ByVal ttl As TimeSpan)
        If repository Is Nothing Then Throw New ArgumentNullException("repository")
        If maximumBytes <= 0 Then Throw New ArgumentOutOfRangeException("maximumBytes")
        If ttl <= TimeSpan.Zero OrElse ttl > TimeSpan.FromMinutes(30) Then Throw New ArgumentOutOfRangeException("ttl")
        _repository = repository : _maximumBytes = maximumBytes : _ttl = ttl
    End Sub

    Public Function Create(ByVal context As ContextoImportacionServicio, ByVal content As SiiPreviewContent,
                           ByVal utcNow As DateTime) As ImportPreviewCreationResult
        If context Is Nothing OrElse content Is Nothing OrElse content.Content Is Nothing OrElse
           String.IsNullOrWhiteSpace(content.ExternalKey) Then Throw New InvalidOperationException("PREVIEW_INVALID")
        If content.Content.LongLength <= 0 OrElse content.Content.LongLength > _maximumBytes Then Throw New InvalidOperationException("PREVIEW_SIZE_INVALID")
        Dim disposition = ImportPreviewContentService.ResolveDisposition(content.ContentType)
        If disposition.Length = 0 Then Throw New InvalidOperationException("PREVIEW_CONTENT_TYPE_INVALID")
        Dim tokenBytes(31) As Byte
        Using generator As RandomNumberGenerator = RandomNumberGenerator.Create()
            generator.GetBytes(tokenBytes)
        End Using
        Dim descriptor = ToBase64Url(tokenBytes)
        Dim snapshot As New ImportPreviewSnapshot With {
            .DescriptorHash = Hash(tokenBytes), .ResourceHash = Hash(Encoding.UTF8.GetBytes(content.ExternalKey.Trim())),
            .UserId = context.IdUsuario, .TaskId = context.IdTarea, .ProviderId = context.ProviderId.Trim().ToUpperInvariant(),
            .ContentType = content.ContentType.Trim().ToLowerInvariant(), .ContentLength = content.Content.LongLength,
            .ContentDisposition = disposition, .SafeFileName = ImportPreviewContentService.SafeFileName(content.FileName, content.ContentType),
            .Content = content.Content, .Status = "Disponible", .CreatedUtc = utcNow.ToUniversalTime(),
            .ExpiresUtc = utcNow.ToUniversalTime().Add(_ttl)}
        Try
            _repository.CleanupExpired(utcNow.ToUniversalTime())
        Catch
            'La limpieza oportunista no invalida un preview nuevo.
        End Try
        If Not _repository.Create(snapshot) Then Throw New InvalidOperationException("PREVIEW_PERSISTENCE_UNAVAILABLE")
        Return New ImportPreviewCreationResult With {.DescriptorId = descriptor, .Snapshot = snapshot}
    End Function

    Public Shared Function DescriptorHash(ByVal descriptorId As String) As Byte()
        Dim bytes As Byte() = Nothing
        If Not TryDecodeBase64Url(descriptorId, bytes) OrElse bytes.Length <> 32 Then Return Nothing
        Return Hash(bytes)
    End Function

    Private Shared Function Hash(ByVal value As Byte()) As Byte()
        Using sha As SHA256 = SHA256.Create()
            Return sha.ComputeHash(value)
        End Using
    End Function

    Private Shared Function ToBase64Url(ByVal value As Byte()) As String
        Return Convert.ToBase64String(value).TrimEnd("="c).Replace("+"c, "-"c).Replace("/"c, "_"c)
    End Function

    Private Shared Function TryDecodeBase64Url(ByVal value As String, ByRef result As Byte()) As Boolean
        result = Nothing
        Dim candidate = If(value, String.Empty).Trim()
        If candidate.Length <> 43 OrElse Not Regex.IsMatch(candidate, "^[A-Za-z0-9_-]+$") Then Return False
        Dim canonical = candidate
        candidate = candidate.Replace("-"c, "+"c).Replace("_"c, "/"c)
        candidate &= New String("="c, (4 - candidate.Length Mod 4) Mod 4)
        Try
            result = Convert.FromBase64String(candidate)
            Return String.Equals(ToBase64Url(result), canonical, StringComparison.Ordinal)
        Catch
            Return False
        End Try
    End Function
End Class
