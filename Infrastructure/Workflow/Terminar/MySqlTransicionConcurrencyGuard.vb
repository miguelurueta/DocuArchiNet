Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Security.Cryptography
Imports System.Text
Imports MySql.Data.MySqlClient

'Bloqueo cooperativo por tarea y versión. No cambia estados ni abre transacciones de negocio.
Public Class MySqlTransicionConcurrencyGuard
    Implements ITransicionConcurrencyGuard

    Private ReadOnly _connectionFactory As IModuleConnectionFactory
    Private ReadOnly _dataExecutor As IDataExecutor

    Public Sub New(ByVal connectionFactory As IModuleConnectionFactory, ByVal dataExecutor As IDataExecutor)
        If connectionFactory Is Nothing Then Throw New ArgumentNullException(NameOf(connectionFactory))
        If dataExecutor Is Nothing Then Throw New ArgumentNullException(NameOf(dataExecutor))

        _connectionFactory = connectionFactory
        _dataExecutor = dataExecutor
    End Sub

    Public Function Adquirir(ByVal contexto As ContextoModuloWorkflow,
                             ByVal idTarea As Long,
                             ByVal tokenVersion As String) As ResultadoGuardTransicion Implements ITransicionConcurrencyGuard.Adquirir
        If contexto Is Nothing OrElse Not contexto.EsValido() OrElse idTarea <= 0 OrElse String.IsNullOrWhiteSpace(tokenVersion) Then
            Return Bloqueado(CodigosBloqueoPrevisualizacion.TransicionInconsistente,
                             "No fue posible preparar el envio de la tarea.")
        End If

        Dim connection As IDbConnection = Nothing
        Try
            connection = _connectionFactory.CreateOpenConnection(contexto)
            Dim lockName As String = CrearNombreLock(idTarea, tokenVersion)
            Dim valor As Object = _dataExecutor.ExecuteScalar(connection,
                                                               Nothing,
                                                               "SELECT GET_LOCK(@lockName, 0)",
                                                               New List(Of IDataParameter) From {Parametro("@lockName", lockName)})
            If String.Equals(Convert.ToString(valor, CultureInfo.InvariantCulture), "1", StringComparison.Ordinal) Then
                Dim resultado As New ResultadoGuardTransicion With {
                    .Adquirido = True,
                    .Lease = New MySqlTransicionConcurrencyLease(connection, _dataExecutor, lockName)
                }
                connection = Nothing
                Return resultado
            End If

            Return Bloqueado(CodigosBloqueoPrevisualizacion.TransicionEnProgreso,
                             "La tarea ya se esta enviando. Espere el resultado antes de reintentar.")
        Catch
            Return Bloqueado(CodigosBloqueoPrevisualizacion.TransicionNoDisponible,
                             "No fue posible preparar el envio de la tarea.")
        Finally
            If connection IsNot Nothing Then connection.Dispose()
        End Try
    End Function

    Private Shared Function Bloqueado(ByVal codigo As String, ByVal mensaje As String) As ResultadoGuardTransicion
        Return New ResultadoGuardTransicion With {
            .Adquirido = False,
            .CodigoBloqueo = codigo,
            .MensajeFuncional = mensaje
        }
    End Function

    Private Shared Function Parametro(ByVal nombre As String, ByVal valor As Object) As IDataParameter
        Return New MySqlParameter(nombre, If(valor, DBNull.Value))
    End Function

    Private Shared Function CrearNombreLock(ByVal idTarea As Long, ByVal tokenVersion As String) As String
        Dim entrada As String = idTarea.ToString(CultureInfo.InvariantCulture) & "|" & tokenVersion
        Using sha As SHA256 = SHA256.Create()
            Dim bytes As Byte() = sha.ComputeHash(Encoding.UTF8.GetBytes(entrada))
            Dim hash As New StringBuilder()
            For indice As Integer = 0 To 19
                hash.Append(bytes(indice).ToString("x2", CultureInfo.InvariantCulture))
            Next
            Return "workflow-modern-" & hash.ToString()
        End Using
    End Function
End Class

Public NotInheritable Class MySqlTransicionConcurrencyLease
    Implements ITransicionConcurrencyLease

    Private _connection As IDbConnection
    Private ReadOnly _dataExecutor As IDataExecutor
    Private ReadOnly _lockName As String

    Public Sub New(ByVal connection As IDbConnection, ByVal dataExecutor As IDataExecutor, ByVal lockName As String)
        _connection = connection
        _dataExecutor = dataExecutor
        _lockName = lockName
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dim connection As IDbConnection = _connection
        _connection = Nothing
        If connection Is Nothing Then Return

        Try
            _dataExecutor.ExecuteScalar(connection,
                                        Nothing,
                                        "SELECT RELEASE_LOCK(@lockName)",
                                        New List(Of IDataParameter) From {New MySqlParameter("@lockName", _lockName)})
        Catch
            'La conexión se cierra de todos modos; MySQL libera el bloqueo asociado.
        Finally
            connection.Dispose()
        End Try
    End Sub
End Class
