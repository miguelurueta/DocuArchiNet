Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Net.Mail
Imports MySql.Data.MySqlClient

' Lee la configuración SMTP activa usando exclusivamente la conexión de Radicación inyectada.
Public NotInheritable Class MySqlSecondFactorSmtpConfigurationRepository
    Implements ISecondFactorSmtpConfigurationRepository

    Private Const MaximumTimeoutMilliseconds As Integer = 120000
    Private Const LegacyTimeoutMultiplier As Long = 100000L
    Private Const SelectConfigurationSql As String = "SELECT SERV_SMTP, PUERTO_SERV_SMTP, USUARIO_SMTP, PASW_SMTP, DOMINIO_SMTP, SMTP_TIEMPO, ESTADO_SSL, ESTADO_ENVIO, ESTADO_BODY, ESTADO_CREDENCIAL FROM Config_Smpt_Side WHERE ESTADO_ENVIO=@enabled"

    Private ReadOnly _connections As IModuleConnectionFactory
    Private ReadOnly _executor As IDataExecutor
    Private ReadOnly _radicacionContext As ContextoModulo

    Public Sub New(ByVal connections As IModuleConnectionFactory,
                   ByVal executor As IDataExecutor,
                   ByVal radicacionContext As ContextoModulo)
        If connections Is Nothing Then Throw New ArgumentNullException(NameOf(connections))
        If executor Is Nothing Then Throw New ArgumentNullException(NameOf(executor))
        If radicacionContext Is Nothing OrElse Not radicacionContext.EsValido() Then Throw New ArgumentException("El contexto de Radicación es inválido.", NameOf(radicacionContext))
        _connections = connections
        _executor = executor
        _radicacionContext = New ContextoModulo With {
            .CodigoModulo = radicacionContext.CodigoModulo,
            .IdUsuario = radicacionContext.IdUsuario,
            .IdGrupo = radicacionContext.IdGrupo,
            .LoginUsuario = radicacionContext.LoginUsuario}
    End Sub

    Public Function Resolve() As SecondFactorSmtpConfigurationResolution Implements ISecondFactorSmtpConfigurationRepository.Resolve
        Try
            Using connection As IDbConnection = _connections.CreateOpenConnection(_radicacionContext)
                Return _executor.ExecuteReader(connection,
                                               Nothing,
                                               SelectConfigurationSql,
                                               Parameters(New MySqlParameter("@enabled", 1)),
                                               AddressOf ProjectConfiguration)
            End Using
        Catch
            Return Resolution(SecondFactorSmtpDeliveryStatus.Failed)
        End Try
    End Function

    Private Shared Function ProjectConfiguration(ByVal reader As IDataReader) As SecondFactorSmtpConfigurationResolution
        If reader Is Nothing OrElse Not reader.Read() Then Return Resolution(SecondFactorSmtpDeliveryStatus.Disabled)

        Dim row As SmtpConfigurationRow = ReadRow(reader)
        If reader.Read() Then Return Resolution(SecondFactorSmtpDeliveryStatus.AmbiguousConfiguration)

        Dim configuration As SecondFactorSmtpConfiguration = Nothing
        If Not TryBuildConfiguration(row, configuration) Then Return Resolution(SecondFactorSmtpDeliveryStatus.InvalidConfiguration)
        Return New SecondFactorSmtpConfigurationResolution(SecondFactorSmtpDeliveryStatus.Submitted, configuration)
    End Function

    Private Shared Function ReadRow(ByVal reader As IDataReader) As SmtpConfigurationRow
        Return New SmtpConfigurationRow With {
            .Host = DbString(reader, "SERV_SMTP"),
            .Port = DbValue(reader, "PUERTO_SERV_SMTP"),
            .Username = DbString(reader, "USUARIO_SMTP"),
            .Password = DbString(reader, "PASW_SMTP"),
            .Domain = DbString(reader, "DOMINIO_SMTP"),
            .Timeout = DbValue(reader, "SMTP_TIEMPO"),
            .Ssl = DbValue(reader, "ESTADO_SSL"),
            .Enabled = DbValue(reader, "ESTADO_ENVIO"),
            .BodyHtml = DbValue(reader, "ESTADO_BODY"),
            .Credential = DbValue(reader, "ESTADO_CREDENCIAL")}
    End Function

    Private Shared Function TryBuildConfiguration(ByVal row As SmtpConfigurationRow,
                                                  ByRef configuration As SecondFactorSmtpConfiguration) As Boolean
        Dim port As Integer
        Dim timeoutUnits As Long
        Dim sslFlag As Integer
        Dim enabledFlag As Integer
        Dim bodyFlag As Integer
        Dim credentialFlag As Integer

        If row Is Nothing OrElse String.IsNullOrWhiteSpace(row.Host) Then Return False
        If Not TryInt32(row.Port, port) OrElse port < 1 OrElse port > 65535 Then Return False
        If String.IsNullOrWhiteSpace(row.Username) OrElse Not IsValidEmail(row.Username) Then Return False
        If Not TryFlag(row.Ssl, sslFlag) OrElse Not TryFlag(row.Enabled, enabledFlag) OrElse enabledFlag <> 1 Then Return False
        If Not TryFlag(row.BodyHtml, bodyFlag) OrElse Not TryFlag(row.Credential, credentialFlag) Then Return False
        If Not TryInt64(row.Timeout, timeoutUnits) OrElse timeoutUnits <= 0 Then Return False
        If timeoutUnits > Integer.MaxValue \ LegacyTimeoutMultiplier Then Return False
        Dim convertedTimeout As Long = timeoutUnits * LegacyTimeoutMultiplier
        Dim effectiveTimeout As Integer = CInt(Math.Min(convertedTimeout, CLng(MaximumTimeoutMilliseconds)))
        If credentialFlag = 1 AndAlso (String.IsNullOrWhiteSpace(row.Username) OrElse String.IsNullOrWhiteSpace(row.Password)) Then Return False

        configuration = New SecondFactorSmtpConfiguration(row.Host.Trim(),
                                                          port,
                                                          row.Username.Trim(),
                                                          If(row.Username, String.Empty).Trim(),
                                                          If(row.Password, String.Empty),
                                                          If(row.Domain, String.Empty).Trim(),
                                                          effectiveTimeout,
                                                          sslFlag = 1,
                                                          credentialFlag = 0,
                                                          bodyFlag = 1)
        Return True
    End Function

    Private Shared Function IsValidEmail(ByVal value As String) As Boolean
        Try
            Dim address As New MailAddress(value.Trim())
            Return String.Equals(address.Address, value.Trim(), StringComparison.OrdinalIgnoreCase)
        Catch
            Return False
        End Try
    End Function

    Private Shared Function TryFlag(ByVal value As Object, ByRef result As Integer) As Boolean
        Return TryInt32(value, result) AndAlso (result = 0 OrElse result = 1)
    End Function

    Private Shared Function TryInt32(ByVal value As Object, ByRef result As Integer) As Boolean
        If value Is Nothing OrElse Convert.IsDBNull(value) Then Return False
        Return Integer.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, result)
    End Function

    Private Shared Function TryInt64(ByVal value As Object, ByRef result As Long) As Boolean
        If value Is Nothing OrElse Convert.IsDBNull(value) Then Return False
        Return Long.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, result)
    End Function

    Private Shared Function DbString(ByVal reader As IDataRecord, ByVal name As String) As String
        Dim value As Object = reader(name)
        If value Is Nothing OrElse Convert.IsDBNull(value) Then Return Nothing
        Return Convert.ToString(value, CultureInfo.InvariantCulture)
    End Function

    Private Shared Function DbValue(ByVal reader As IDataRecord, ByVal name As String) As Object
        Dim value As Object = reader(name)
        Return If(value Is Nothing OrElse Convert.IsDBNull(value), Nothing, value)
    End Function

    Private Shared Function Resolution(ByVal status As SecondFactorSmtpDeliveryStatus) As SecondFactorSmtpConfigurationResolution
        Return New SecondFactorSmtpConfigurationResolution(status, Nothing)
    End Function

    Private Shared Function Parameters(ParamArray values As IDataParameter()) As IList(Of IDataParameter)
        Return New List(Of IDataParameter)(values)
    End Function

    Private NotInheritable Class SmtpConfigurationRow
        Public Property Host As String
        Public Property Port As Object
        Public Property Username As String
        Public Property Password As String
        Public Property Domain As String
        Public Property Timeout As Object
        Public Property Ssl As Object
        Public Property Enabled As Object
        Public Property BodyHtml As Object
        Public Property Credential As Object
    End Class
End Class
