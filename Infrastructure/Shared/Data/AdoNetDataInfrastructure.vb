Imports System
Imports System.Collections.Generic
Imports System.Data

'Ejecutor ADO.NET reutilizable. Los repositorios de cada modulo suministran SQL parametrizado.
Public Class AdoNetDataExecutor
    Implements IDataExecutor

    Public Function ExecuteNonQuery(ByVal connection As IDbConnection,
                                    ByVal transaction As IDbTransaction,
                                    ByVal commandText As String,
                                    ByVal parameters As IEnumerable(Of IDataParameter)) As Integer Implements IDataExecutor.ExecuteNonQuery
        Using command As IDbCommand = CrearComando(connection, transaction, commandText, parameters)
            Return command.ExecuteNonQuery()
        End Using
    End Function

    Public Function ExecuteScalar(ByVal connection As IDbConnection,
                                  ByVal transaction As IDbTransaction,
                                  ByVal commandText As String,
                                  ByVal parameters As IEnumerable(Of IDataParameter)) As Object Implements IDataExecutor.ExecuteScalar
        Using command As IDbCommand = CrearComando(connection, transaction, commandText, parameters)
            Return command.ExecuteScalar()
        End Using
    End Function

    Private Shared Function CrearComando(ByVal connection As IDbConnection,
                                         ByVal transaction As IDbTransaction,
                                         ByVal commandText As String,
                                         ByVal parameters As IEnumerable(Of IDataParameter)) As IDbCommand
        If connection Is Nothing OrElse connection.State <> ConnectionState.Open Then
            Throw New InvalidOperationException("MODULE_CONNECTION_NOT_OPEN")
        End If

        Dim command As IDbCommand = connection.CreateCommand()
        command.CommandText = commandText
        command.Transaction = transaction
        If parameters IsNot Nothing Then
            For Each parameter As IDataParameter In parameters
                command.Parameters.Add(parameter)
            Next
        End If
        Return command
    End Function
End Class

Public Class DbTransactionFactory
    Implements ITransactionFactory

    Public Function BeginTransaction(ByVal connection As IDbConnection) As IDbTransaction Implements ITransactionFactory.BeginTransaction
        If connection Is Nothing OrElse connection.State <> ConnectionState.Open Then
            Throw New InvalidOperationException("MODULE_CONNECTION_NOT_OPEN")
        End If
        Return connection.BeginTransaction()
    End Function
End Class
