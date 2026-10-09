Imports System
Imports System.Globalization
Imports System.Security.Cryptography
Imports System.Text

Public NotInheritable Class HmacSecondFactorCodeProtector
    Implements ISecondFactorCodeProtector

    Private Const FormatVersion As String = "v1"
    Private ReadOnly _keys As ISecondFactorKeyProvider

    Public Sub New(ByVal keys As ISecondFactorKeyProvider)
        If keys Is Nothing Then Throw New ArgumentNullException(NameOf(keys))
        _keys = keys
    End Sub

    Public Function Protect(ByVal context As SecondFactorProtectionContext, ByVal code As String) As String Implements ISecondFactorCodeProtector.Protect
        If context Is Nothing Then Throw New ArgumentNullException(NameOf(context))
        If Not SecondFactorContractValidation.IsSixDigitCode(code) Then Throw New ArgumentException("El OTP debe tener seis dígitos.", NameOf(code))
        Dim key As SecondFactorKeyMaterial = _keys.GetActiveKey()
        Dim mac As Byte() = ComputeMac(key, context, code)
        Return String.Concat(FormatVersion, ":", key.KeyId, ":", Convert.ToBase64String(mac))
    End Function

    Public Function Verify(ByVal context As SecondFactorProtectionContext,
                           ByVal code As String,
                           ByVal protectedCode As String) As Boolean Implements ISecondFactorCodeProtector.Verify
        If context Is Nothing OrElse Not SecondFactorContractValidation.IsSixDigitCode(code) OrElse String.IsNullOrWhiteSpace(protectedCode) Then Return False
        Dim parts As String() = protectedCode.Split(":"c)
        If parts.Length <> 3 OrElse Not String.Equals(parts(0), FormatVersion, StringComparison.Ordinal) Then Return False

        Dim key As SecondFactorKeyMaterial = Nothing
        If Not _keys.TryGetKey(parts(1), key) OrElse key Is Nothing Then Return False
        Dim suppliedMac As Byte()
        Try
            suppliedMac = Convert.FromBase64String(parts(2))
        Catch ex As FormatException
            Return False
        End Try
        Dim expectedMac As Byte() = ComputeMac(key, context, code)
        Return FixedTimeEquals(expectedMac, suppliedMac)
    End Function

    Private Shared Function ComputeMac(ByVal key As SecondFactorKeyMaterial,
                                       ByVal context As SecondFactorProtectionContext,
                                       ByVal code As String) As Byte()
        Dim payload As String = String.Join("|", New String() {
            EncodeSegment(context.Purpose.ToString()),
            EncodeSegment(context.ChallengeId.ToString("D", CultureInfo.InvariantCulture)),
            EncodeSegment(context.CanonicalIdentity),
            EncodeSegment(context.SessionBinding),
            EncodeSegment(code)
        })
        Using hmac As New HMACSHA256(key.KeyBytes)
            Return hmac.ComputeHash(Encoding.UTF8.GetBytes(payload))
        End Using
    End Function

    Private Shared Function EncodeSegment(ByVal value As String) As String
        Dim bytes As Byte() = Encoding.UTF8.GetBytes(If(value, String.Empty))
        Return bytes.Length.ToString(CultureInfo.InvariantCulture) & ":" & Convert.ToBase64String(bytes)
    End Function

    Private Shared Function FixedTimeEquals(ByVal expected As Byte(), ByVal supplied As Byte()) As Boolean
        If expected Is Nothing OrElse supplied Is Nothing OrElse expected.Length <> supplied.Length Then Return False
        Dim difference As Integer = 0
        For index As Integer = 0 To expected.Length - 1
            difference = difference Or (expected(index) Xor supplied(index))
        Next
        Return difference = 0
    End Function
End Class
