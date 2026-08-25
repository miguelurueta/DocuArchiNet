Imports System
Imports System.Collections.Generic
Imports System.Data
Imports MySql.Data.MySqlClient

'Lock cooperativo de devolución: la identidad está ligada exclusivamente a la tarea, nunca al token de preview.
Public Class MySqlDevolverActividadConcurrencyGuard
    Implements IDevolverActividadConcurrencyGuard

    Private ReadOnly _connectionFactory As IModuleConnectionFactory
    Private ReadOnly _dataExecutor As IDataExecutor

    Public Sub New(ByVal connectionFactory As IModuleConnectionFactory, ByVal dataExecutor As IDataExecutor)
        If connectionFactory Is Nothing Then Throw New ArgumentNullException(NameOf(connectionFactory))
        If dataExecutor Is Nothing Then Throw New ArgumentNullException(NameOf(dataExecutor))
        _connectionFactory = connectionFactory
        _dataExecutor = dataExecutor
    End Sub

    Public Function Adquirir(ByVal contexto As ContextoModuloWorkflow,
                             ByVal idTarea As Long) As ResultadoGuardDevolverActividad Implements IDevolverActividadConcurrencyGuard.Adquirir
        If contexto Is Nothing OrElse Not contexto.EsValido() OrElse idTarea <= 0 Then
            Return Bloquear(CodigosBloqueoDevolverActividad.ContextoInvalido,
                            "No fue posible preparar la devolución de la tarea.")
        End If

        Dim connection As IDbConnection = Nothing
        Try
            connection = _connectionFactory.CreateOpenConnection(contexto)
            Dim lockName As String = CrearNombreLock(idTarea)
            Dim valor As Object = _dataExecutor.ExecuteScalar(connection, Nothing, "SELECT GET_LOCK(@lockName, 0)",
                                                               New List(Of IDataParameter) From {New MySqlParameter("@lockName", lockName)})
            If String.Equals(Convert.ToString(valor, Globalization.CultureInfo.InvariantCulture), "1", StringComparison.Ordinal) Then
                Dim resultado As New ResultadoGuardDevolverActividad With {
                    .Adquirido = True,
                    .Lease = New MySqlDevolverActividadConcurrencyLease(connection, _dataExecutor, lockName)
                }
                connection = Nothing
                Return resultado
            End If
            Return Bloquear(CodigosBloqueoDevolverActividad.EnProgreso,
                            "La tarea ya se está devolviendo. Espere el resultado antes de reintentar.")
        Catch
            Return Bloquear(CodigosBloqueoDevolverActividad.NoDisponible,
                            "No fue posible preparar la devolución de la tarea.")
        Finally
            If connection IsNot Nothing Then connection.Dispose()
        End Try
    End Function

    Private Shared Function CrearNombreLock(ByVal idTarea As Long) As String
        Return "workflow-return-" & idTarea.ToString(Globalization.CultureInfo.InvariantCulture)
    End Function

    Private Shared Function Bloquear(ByVal codigo As String, ByVal mensaje As String) As ResultadoGuardDevolverActividad
        Return New ResultadoGuardDevolverActividad With {.Adquirido = False, .CodigoBloqueo = codigo, .MensajeFuncional = mensaje}
    End Function
End Class

Public NotInheritable Class MySqlDevolverActividadConcurrencyLease
    Implements IDevolverActividadConcurrencyLease

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
            _dataExecutor.ExecuteScalar(connection, Nothing, "SELECT RELEASE_LOCK(@lockName)",
                                        New List(Of IDataParameter) From {New MySqlParameter("@lockName", _lockName)})
        Catch
            'MySQL libera el lock al cerrar la conexión aun si la liberación explícita falla.
        Finally
            connection.Dispose()
        End Try
    End Sub
End Class
