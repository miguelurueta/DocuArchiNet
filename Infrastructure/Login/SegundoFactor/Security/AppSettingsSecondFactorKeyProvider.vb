Imports System
Imports System.Configuration

Public Interface ISecondFactorSettings
    Function GetValue(ByVal name As String) As String
End Interface

Public NotInheritable Class ConfigurationManagerSecondFactorSettings
    Implements ISecondFactorSettings

    Public Function GetValue(ByVal name As String) As String Implements ISecondFactorSettings.GetValue
        Return ConfigurationManager.AppSettings(name)
    End Function
End Class

Public NotInheritable Class AppSettingsSecondFactorKeyProvider
    Implements ISecondFactorKeyProvider

    Public Const ActiveKeyIdSetting As String = "LoginSecondFactorHmacActiveKeyId"
    Public Const KeySettingPrefix As String = "LoginSecondFactorHmacKey."

    Private ReadOnly _settings As ISecondFactorSettings

    Public Sub New()
        Me.New(New ConfigurationManagerSecondFactorSettings())
    End Sub

    Public Sub New(ByVal settings As ISecondFactorSettings)
        If settings Is Nothing Then Throw New ArgumentNullException(NameOf(settings))
        _settings = settings
    End Sub

    Public Function GetActiveKey() As SecondFactorKeyMaterial Implements ISecondFactorKeyProvider.GetActiveKey
        Dim keyId As String = _settings.GetValue(ActiveKeyIdSetting)
        If String.IsNullOrWhiteSpace(keyId) Then Throw New InvalidOperationException("No existe una llave HMAC activa configurada.")
        Dim key As SecondFactorKeyMaterial = Nothing
        If Not TryGetKey(keyId.Trim(), key) Then Throw New InvalidOperationException("La llave HMAC activa no es válida.")
        Return key
    End Function

    Public Function TryGetKey(ByVal keyId As String, ByRef key As SecondFactorKeyMaterial) As Boolean Implements ISecondFactorKeyProvider.TryGetKey
        key = Nothing
        If String.IsNullOrWhiteSpace(keyId) OrElse keyId.Contains(":") Then Return False
        Dim encoded As String = _settings.GetValue(KeySettingPrefix & keyId.Trim())
        If String.IsNullOrWhiteSpace(encoded) Then Return False

        Try
            Dim bytes As Byte() = Convert.FromBase64String(encoded.Trim())
            If bytes.Length < 32 Then Return False
            key = New SecondFactorKeyMaterial(keyId.Trim(), bytes)
            Return True
        Catch ex As FormatException
            Return False
        Catch ex As ArgumentException
            Return False
        End Try
    End Function
End Class
