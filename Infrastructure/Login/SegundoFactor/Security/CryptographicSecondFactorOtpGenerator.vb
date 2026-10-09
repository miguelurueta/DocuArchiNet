Imports System
Imports System.Globalization
Imports System.Security.Cryptography

Public NotInheritable Class CryptographicSecondFactorOtpGenerator
    Implements ISecondFactorOtpGenerator
    Implements IDisposable

    Private Const CodeSpace As UInteger = 1000000UI
    Private Const AcceptanceLimit As UInteger = UInteger.MaxValue - (UInteger.MaxValue Mod CodeSpace)
    Private ReadOnly _random As RandomNumberGenerator
    Private _disposed As Boolean

    Public Sub New()
        Me.New(RandomNumberGenerator.Create())
    End Sub

    Public Sub New(ByVal random As RandomNumberGenerator)
        If random Is Nothing Then Throw New ArgumentNullException(NameOf(random))
        _random = random
    End Sub

    Public Function GenerateCode() As String Implements ISecondFactorOtpGenerator.GenerateCode
        If _disposed Then Throw New ObjectDisposedException(NameOf(CryptographicSecondFactorOtpGenerator))
        Dim buffer(3) As Byte
        Dim sample As UInteger
        Do
            _random.GetBytes(buffer)
            sample = BitConverter.ToUInt32(buffer, 0)
        Loop While sample >= AcceptanceLimit
        Return (sample Mod CodeSpace).ToString("D6", CultureInfo.InvariantCulture)
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        If _disposed Then Return
        _random.Dispose()
        _disposed = True
    End Sub
End Class
