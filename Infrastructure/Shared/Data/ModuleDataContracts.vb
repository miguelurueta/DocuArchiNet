Imports System.Collections.Generic
Imports System.Data

'Contratos tecnicos compartidos. No conocen reglas, modelos ni codigos de Workflow.
Public Interface IModuleConnectionFactory
    Function CreateOpenConnection(ByVal contexto As ContextoModulo) As IDbConnection
End Interface

Public Interface IDataExecutor
    Function ExecuteNonQuery(ByVal connection As IDbConnection,
                             ByVal transaction As IDbTransaction,
                             ByVal commandText As String,
                             ByVal parameters As IEnumerable(Of IDataParameter)) As Integer
    Function ExecuteScalar(ByVal connection As IDbConnection,
                           ByVal transaction As IDbTransaction,
                           ByVal commandText As String,
                           ByVal parameters As IEnumerable(Of IDataParameter)) As Object
End Interface

Public Interface ITransactionFactory
    Function BeginTransaction(ByVal connection As IDbConnection) As IDbTransaction
End Interface

Public Class ResultadoDatos
    Public Property Exito As Boolean
    Public Property Codigo As String
    Public Property Referencia As String
End Class

Public Class SolicitudPaginacion
    Public Property Pagina As Integer
    Public Property TamanoPagina As Integer
End Class

Public Class PaginaResultado(Of T)
    Public Sub New()
        Elementos = New List(Of T)()
    End Sub

    Public Property Elementos As IList(Of T)
    Public Property TotalRegistros As Integer
    Public Property Pagina As Integer
    Public Property TamanoPagina As Integer
End Class
