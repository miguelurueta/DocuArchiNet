Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Data.Odbc
Imports System.Globalization

' Resuelve empresa, módulo y configuración 2FA desde el catálogo central existente.
Public NotInheritable Class OdbcSecondFactorLoginModuleRepository
    Implements ISecondFactorLoginModuleRepository

    Private Const ResolveSql As String = "SELECT ge.ID_EMPRESA,gm.ID_MODULO,gm.NOMBRE_MODULO,gm.TIPO_MODULO,gm.RequiereSegundoFactor,gm.SecondFactorProviderType,gm.SegundoFactorTiempoExpira FROM empresa_gestion_documental ge INNER JOIN gestor_modulos gm ON gm.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=ge.ID_EMPRESA WHERE ge.RAZON_SOCIAL_EMPRESA=? AND gm.NOMBRE_MODULO=? AND gm.ESTADO_MODULO=1"
    Private ReadOnly _connections As IModuleConnectionFactory
    Private ReadOnly _executor As IDataExecutor
    Private ReadOnly _context As ContextoPreautenticacionModulo

    Public Sub New(ByVal connections As IModuleConnectionFactory,
                   ByVal executor As IDataExecutor,
                   ByVal context As ContextoPreautenticacionModulo)
        If connections Is Nothing Then Throw New ArgumentNullException(NameOf(connections))
        If executor Is Nothing Then Throw New ArgumentNullException(NameOf(executor))
        If context Is Nothing OrElse Not context.EsValido() Then Throw New ArgumentException("El contexto central es inválido.", NameOf(context))
        _connections = connections
        _executor = executor
        _context = context
    End Sub

    Public Function Resolve(ByVal companyName As String,
                            ByVal moduleName As String) As SecondFactorLoginModule Implements ISecondFactorLoginModuleRepository.Resolve
        If String.IsNullOrWhiteSpace(companyName) Then Throw New ArgumentException("La empresa es obligatoria.", NameOf(companyName))
        If String.IsNullOrWhiteSpace(moduleName) Then Throw New ArgumentException("El módulo es obligatorio.", NameOf(moduleName))

        Using connection As IDbConnection = _connections.CreateOpenConnection(_context)
            Return _executor.ExecuteReader(connection,
                                           Nothing,
                                           ResolveSql,
                                           Parameters(New OdbcParameter("@companyName", companyName.Trim()),
                                                      New OdbcParameter("@moduleName", moduleName.Trim())),
                                           AddressOf ProjectSingle)
        End Using
    End Function

    Private Shared Function ProjectSingle(ByVal reader As IDataReader) As SecondFactorLoginModule
        If reader Is Nothing OrElse Not reader.Read() Then Return Nothing
        Dim empresaId As Integer = RequiredInt32(reader, "ID_EMPRESA")
        Dim moduloId As Integer = RequiredInt32(reader, "ID_MODULO")
        Dim moduleName As String = RequiredString(reader, "NOMBRE_MODULO")
        Dim moduleType As String = RequiredString(reader, "TIPO_MODULO")
        Dim requiredFlag As Integer? = OptionalInt32(reader, "RequiereSegundoFactor")
        Dim provider As Integer? = OptionalInt32(reader, "SecondFactorProviderType")
        Dim expiration As Integer? = OptionalInt32(reader, "SegundoFactorTiempoExpira")
        If reader.Read() Then Throw New InvalidOperationException("LOGIN_MODULE_AMBIGUOUS")
        Return New SecondFactorLoginModule(empresaId, moduloId, moduleName, moduleType,
                                           New SegundoFactorConfiguration(requiredFlag, provider, expiration))
    End Function

    Private Shared Function RequiredInt32(ByVal record As IDataRecord, ByVal name As String) As Integer
        Dim value As Integer? = OptionalInt32(record, name)
        If Not value.HasValue OrElse value.Value <= 0 Then Throw New InvalidOperationException("LOGIN_MODULE_INVALID")
        Return value.Value
    End Function

    Private Shared Function OptionalInt32(ByVal record As IDataRecord, ByVal name As String) As Integer?
        Dim value As Object = record(name)
        If value Is Nothing OrElse Convert.IsDBNull(value) Then Return Nothing
        Dim parsed As Integer
        If Not Integer.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, parsed) Then
            Throw New InvalidOperationException("LOGIN_MODULE_INVALID")
        End If
        Return parsed
    End Function

    Private Shared Function RequiredString(ByVal record As IDataRecord, ByVal name As String) As String
        Dim value As String = Convert.ToString(record(name), CultureInfo.InvariantCulture)
        If String.IsNullOrWhiteSpace(value) Then Throw New InvalidOperationException("LOGIN_MODULE_INVALID")
        Return value.Trim()
    End Function

    Private Shared Function Parameters(ParamArray values As IDataParameter()) As IList(Of IDataParameter)
        Return New List(Of IDataParameter)(values)
    End Function
End Class
