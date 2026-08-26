Imports System
Imports System.Collections.Generic
Imports System.Data
Imports MySql.Data.MySqlClient

'Guard exclusivo de Usuario anterior. El nombre depende solo de la tarea para serializar tokens distintos.
Public Class MySqlDevolverUsuarioAnteriorConcurrencyGuard
    Implements IDevolverUsuarioAnteriorConcurrencyGuard

    Private ReadOnly _connectionFactory As IModuleConnectionFactory
    Private ReadOnly _dataExecutor As IDataExecutor

    Public Sub New(ByVal connectionFactory As IModuleConnectionFactory, ByVal dataExecutor As IDataExecutor)
        If connectionFactory Is Nothing Then Throw New ArgumentNullException(NameOf(connectionFactory))
        If dataExecutor Is Nothing Then Throw New ArgumentNullException(NameOf(dataExecutor))
        _connectionFactory = connectionFactory
        _dataExecutor = dataExecutor
    End Sub

    Public Function Adquirir(ByVal contexto As ContextoModuloWorkflow,
                             ByVal idTarea As Long) As ResultadoGuardDevolverUsuarioAnterior Implements IDevolverUsuarioAnteriorConcurrencyGuard.Adquirir
        If contexto Is Nothing OrElse Not contexto.EsValido() OrElse idTarea <= 0 Then
            Return Bloquear(CodigosBloqueoDevolverUsuarioAnterior.ContextoInvalido, "No fue posible preparar la devolución de la tarea.")
        End If
        Dim conexion As IDbConnection = Nothing
        Try
            conexion = _connectionFactory.CreateOpenConnection(contexto)
            Dim nombre As String = "workflow-return-user-" & idTarea.ToString(Globalization.CultureInfo.InvariantCulture)
            Dim valor As Object = _dataExecutor.ExecuteScalar(conexion, Nothing, "SELECT GET_LOCK(@lockName, 0)",
                New List(Of IDataParameter) From {New MySqlParameter("@lockName", nombre)})
            If String.Equals(Convert.ToString(valor, Globalization.CultureInfo.InvariantCulture), "1", StringComparison.Ordinal) Then
                Dim resultado As New ResultadoGuardDevolverUsuarioAnterior With {
                    .Adquirido = True,
                    .Lease = New MySqlDevolverUsuarioAnteriorConcurrencyLease(conexion, _dataExecutor, nombre)}
                conexion = Nothing
                Return resultado
            End If
            Return Bloquear(CodigosBloqueoDevolverUsuarioAnterior.EnProgreso,
                            "La tarea ya se está devolviendo. Espere el resultado antes de reintentar.")
        Catch
            Return Bloquear(CodigosBloqueoDevolverUsuarioAnterior.NoDisponible, "No fue posible preparar la devolución de la tarea.")
        Finally
            If conexion IsNot Nothing Then conexion.Dispose()
        End Try
    End Function

    Private Shared Function Bloquear(ByVal codigo As String, ByVal mensaje As String) As ResultadoGuardDevolverUsuarioAnterior
        Return New ResultadoGuardDevolverUsuarioAnterior With {.Adquirido = False, .CodigoBloqueo = codigo, .MensajeFuncional = mensaje}
    End Function
End Class

Public NotInheritable Class MySqlDevolverUsuarioAnteriorConcurrencyLease
    Implements IDevolverUsuarioAnteriorConcurrencyLease

    Private _connection As IDbConnection
    Private ReadOnly _dataExecutor As IDataExecutor
    Private ReadOnly _nombre As String

    Public Sub New(ByVal connection As IDbConnection, ByVal dataExecutor As IDataExecutor, ByVal nombre As String)
        _connection = connection
        _dataExecutor = dataExecutor
        _nombre = nombre
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dim conexion As IDbConnection = _connection
        _connection = Nothing
        If conexion Is Nothing Then Return
        Try
            _dataExecutor.ExecuteScalar(conexion, Nothing, "SELECT RELEASE_LOCK(@lockName)",
                New List(Of IDataParameter) From {New MySqlParameter("@lockName", _nombre)})
        Catch
            'El cierre de conexión también libera GET_LOCK si la liberación explícita falla.
        Finally
            conexion.Dispose()
        End Try
    End Sub
End Class
