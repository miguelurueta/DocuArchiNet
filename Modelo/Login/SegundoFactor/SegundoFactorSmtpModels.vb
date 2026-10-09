Imports System

' Contratos SMTP internos e inmutables, independientes de presentación y adaptadores.
Public Enum SecondFactorSmtpDeliveryStatus
    Submitted = 1
    Disabled = 2
    InvalidConfiguration = 3
    AmbiguousConfiguration = 4
    Failed = 5
End Enum

Public NotInheritable Class SecondFactorSmtpConfiguration
    Public Sub New(ByVal host As String,
                   ByVal port As Integer,
                   ByVal senderAddress As String,
                   ByVal username As String,
                   ByVal password As String,
                   ByVal domain As String,
                   ByVal timeoutMilliseconds As Integer,
                   ByVal enableSsl As Boolean,
                   ByVal useDefaultCredentials As Boolean,
                   ByVal isBodyHtml As Boolean)
        Me.Host = host
        Me.Port = port
        Me.SenderAddress = senderAddress
        Me.Username = username
        Me.Password = password
        Me.Domain = domain
        Me.TimeoutMilliseconds = timeoutMilliseconds
        Me.EnableSsl = enableSsl
        Me.UseDefaultCredentials = useDefaultCredentials
        Me.IsBodyHtml = isBodyHtml
    End Sub

    Public ReadOnly Property Host As String
    Public ReadOnly Property Port As Integer
    Public ReadOnly Property SenderAddress As String
    Public ReadOnly Property Username As String
    Public ReadOnly Property Password As String
    Public ReadOnly Property Domain As String
    Public ReadOnly Property TimeoutMilliseconds As Integer
    Public ReadOnly Property EnableSsl As Boolean
    Public ReadOnly Property UseDefaultCredentials As Boolean
    Public ReadOnly Property IsBodyHtml As Boolean
End Class

Public NotInheritable Class SecondFactorSmtpConfigurationResolution
    Public Sub New(ByVal status As SecondFactorSmtpDeliveryStatus,
                   ByVal configuration As SecondFactorSmtpConfiguration)
        If Not [Enum].IsDefined(GetType(SecondFactorSmtpDeliveryStatus), status) Then Throw New ArgumentOutOfRangeException(NameOf(status))
        If status = SecondFactorSmtpDeliveryStatus.Submitted AndAlso configuration Is Nothing Then
            Throw New ArgumentNullException(NameOf(configuration))
        End If
        If status <> SecondFactorSmtpDeliveryStatus.Submitted AndAlso configuration IsNot Nothing Then
            Throw New ArgumentException("Solo una configuración válida puede incluir datos SMTP.", NameOf(configuration))
        End If
        Me.Status = status
        Me.Configuration = configuration
    End Sub

    Public ReadOnly Property Status As SecondFactorSmtpDeliveryStatus
    Public ReadOnly Property Configuration As SecondFactorSmtpConfiguration
End Class

Public NotInheritable Class SecondFactorSmtpDelivery
    Public Sub New(ByVal status As SecondFactorSmtpDeliveryStatus,
                   ByVal publicCode As String)
        If Not [Enum].IsDefined(GetType(SecondFactorSmtpDeliveryStatus), status) Then Throw New ArgumentOutOfRangeException(NameOf(status))
        If String.IsNullOrWhiteSpace(publicCode) Then Throw New ArgumentException("El código público es obligatorio.", NameOf(publicCode))
        Me.Status = status
        Me.PublicCode = publicCode.Trim()
    End Sub

    Public ReadOnly Property Status As SecondFactorSmtpDeliveryStatus
    Public ReadOnly Property PublicCode As String
End Class
