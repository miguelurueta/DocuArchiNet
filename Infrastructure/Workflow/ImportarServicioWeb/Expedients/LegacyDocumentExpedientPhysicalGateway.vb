Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Text.RegularExpressions
Imports MySql.Data.MySqlClient

' Adaptador transitorio en proceso. Conserva VinculaDocumentoExpediente y confirma por lectura del gabinete.
Public NotInheritable Class ModernDocumentExpedientPhysicalGateway
    Implements IDocumentExpedientPhysicalGateway

    Private Shared ReadOnly SafeCabinet As New Regex("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.CultureInvariant)
    Private ReadOnly _connections As IModuleConnectionFactory
    Private ReadOnly _executor As IDataExecutor

    Public Sub New(ByVal connections As IModuleConnectionFactory, ByVal executor As IDataExecutor)
        If connections Is Nothing OrElse executor Is Nothing Then Throw New ArgumentNullException("dependency")
        _connections = connections : _executor = executor
    End Sub

    Public Function ConsultarExpedientes(ByVal contexto As ContextoImportacionServicio,
                                         ByVal nombreGabinete As String,
                                         ByVal idImagen As Long) As IList(Of Long) Implements IDocumentExpedientPhysicalGateway.ConsultarExpedientes
        Validate(contexto, nombreGabinete, idImagen, Nothing)
        If Not SafeCabinet.IsMatch(nombreGabinete.Trim()) Then Throw New InvalidOperationException("DOCUMENT_CABINET_INVALID")
        Dim sql = "SELECT ID_EXPEDIENTE FROM `" & nombreGabinete.Trim() & "` WHERE ID=@imageId LIMIT 2"
        Using connection = _connections.CreateOpenConnection(ModuleContext(contexto))
            Return _executor.ExecuteReader(connection, Nothing, sql,
                New List(Of IDataParameter) From {New MySqlParameter("@imageId", idImagen)}, AddressOf MapRelations)
        End Using
    End Function

    Public Function VincularDocumento(ByVal contexto As ContextoImportacionServicio,
                                      ByVal nombreGabinete As String,
                                      ByVal idImagen As Long,
                                      ByVal idExpediente As Long,
                                      ByVal radicadoSii As String) As String Implements IDocumentExpedientPhysicalGateway.VincularDocumento
        Validate(contexto, nombreGabinete, idImagen, idExpediente)
        If contexto.IdUsuarioGestion <= 0 OrElse contexto.IdEmpresaGestion <= 0 OrElse contexto.IdUsuario <= 0 OrElse
           contexto.IdRuta <= 0 OrElse String.IsNullOrWhiteSpace(contexto.NombreRutaWorkflow) Then
            Throw New InvalidOperationException("DOCUMENT_RELATION_CONTEXT_INVALID")
        End If
        Using lockConnection = _connections.CreateOpenConnection(ModuleContext(contexto))
            Dim lockName = "DOC67_REL_" & nombreGabinete.Trim().ToUpperInvariant() & "_" & idImagen.ToString()
            If lockName.Length > 64 Then lockName = lockName.Substring(0, 64)
            Dim acquired = _executor.ExecuteScalar(lockConnection, Nothing, "SELECT GET_LOCK(@lockName,5)",
                New List(Of IDataParameter) From {New MySqlParameter("@lockName", lockName)})
            If acquired Is Nothing OrElse Convert.IsDBNull(acquired) OrElse Convert.ToInt32(acquired) <> 1 Then Throw New InvalidOperationException("DOCUMENT_RELATION_LOCK_UNAVAILABLE")
            Try
                Dim current = ConsultarExpedientes(contexto, nombreGabinete, idImagen)
                If current.Count > 1 Then Throw New InvalidOperationException("DOCUMENT_RELATION_DUPLICATED")
                If current.Count = 1 Then
                    If current(0) = idExpediente Then Return "YES"
                    Throw New InvalidOperationException("DOCUMENT_RELATION_CROSSED")
                End If
                Dim fieldValue As String = Nothing, relatedName As String = Nothing
                Return New ModernExpedientMutationBridge().VinculaDocumentoExpedienteConContexto(CInt(idExpediente), CInt(idImagen), nombreGabinete,
                    If(radicadoSii, String.Empty).Trim(), contexto.IdTarea, fieldValue, relatedName,
                    contexto.NombreRutaWorkflow, contexto.IdUsuarioGestion, contexto.IdEmpresaGestion,
                    contexto.IdUsuario, contexto.IdRuta)
            Finally
                Try
                    _executor.ExecuteScalar(lockConnection, Nothing, "SELECT RELEASE_LOCK(@lockName)",
                        New List(Of IDataParameter) From {New MySqlParameter("@lockName", lockName)})
                Catch
                    ' El cierre de la conexión también libera el lock.
                End Try
            End Try
        End Using
    End Function

    Private Shared Sub Validate(ByVal contexto As ContextoImportacionServicio, ByVal cabinet As String,
                                ByVal imageId As Long, ByVal expedientId As Nullable(Of Long))
        If contexto Is Nothing OrElse contexto.IdTarea <= 0 OrElse String.IsNullOrWhiteSpace(cabinet) OrElse
           imageId <= 0 OrElse imageId > Integer.MaxValue OrElse
           (expedientId.HasValue AndAlso (expedientId.Value <= 0 OrElse expedientId.Value > Integer.MaxValue)) Then
            Throw New InvalidOperationException("DOCUMENT_RELATION_INPUT_INVALID")
        End If
    End Sub

    Private Shared Function MapRelations(ByVal reader As IDataReader) As IList(Of Long)
        Dim values As New List(Of Long)()
        While reader.Read()
            If Not Convert.IsDBNull(reader("ID_EXPEDIENTE")) AndAlso Convert.ToInt64(reader("ID_EXPEDIENTE")) > 0 Then values.Add(Convert.ToInt64(reader("ID_EXPEDIENTE")))
        End While
        Return values
    End Function

    Private Shared Function ModuleContext(ByVal value As ContextoImportacionServicio) As ContextoModulo
        Return New ContextoModulo With {.CodigoModulo = "IMPORTAR_SERVICIO_WEB", .IdUsuario = value.IdUsuario, .IdGrupo = value.IdGrupo, .LoginUsuario = value.LoginUsuario}
    End Function
End Class
