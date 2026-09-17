Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Text.RegularExpressions
Imports MySql.Data.MySqlClient

' Consulta nuevamente el gabinete después del almacenamiento; incluye documentos previos y nuevos.
Public NotInheritable Class MySqlImportRelatedDocumentRepository
    Implements IImportRelatedDocumentRepository

    Private ReadOnly _documentConnections As IModuleConnectionFactory
    Private ReadOnly _workflowConnections As IModuleConnectionFactory
    Private ReadOnly _executor As IDataExecutor

    Public Sub New(ByVal connections As IModuleConnectionFactory, ByVal executor As IDataExecutor)
        Me.New(connections, connections, executor)
    End Sub

    Public Sub New(ByVal documentConnections As IModuleConnectionFactory,
                   ByVal workflowConnections As IModuleConnectionFactory,
                   ByVal executor As IDataExecutor)
        If documentConnections Is Nothing OrElse workflowConnections Is Nothing OrElse executor Is Nothing Then Throw New ArgumentNullException("dependency")
        _documentConnections = documentConnections
        _workflowConnections = workflowConnections
        _executor = executor
    End Sub

    Public Function ObtenerPorEnlace(ByVal contexto As ContextoImportacionServicio,
                                     ByVal nombreGabinete As String,
                                     ByVal radicadoSii As String) As IList(Of DocumentoRelacionadoImportacion) Implements IImportRelatedDocumentRepository.ObtenerPorEnlace
        If contexto Is Nothing OrElse contexto.IdTarea <= 0 OrElse
           Not SafeIdentifier(nombreGabinete) OrElse String.IsNullOrWhiteSpace(radicadoSii) Then
            Throw New ImportRelatedDocumentRepositoryException("RELATED_DOCUMENT_QUERY_INVALID")
        End If

        Try
            Using connection = _documentConnections.CreateOpenConnection(ModuleContext(contexto))
                Dim sql = "SELECT ID,ENLASE FROM `" & nombreGabinete & "` WHERE ENLASE=@radicado ORDER BY ID"
                Dim rows = _executor.ExecuteReader(connection, Nothing, sql,
                    New List(Of IDataParameter) From {New MySqlParameter("@radicado", radicadoSii.Trim())},
                    AddressOf MapRows)
                Return Deduplicate(rows, contexto.IdTarea, nombreGabinete)
            End Using
        Catch ex As ImportRelatedDocumentRepositoryException
            Throw
        Catch ex As Exception
            ' El contrato externo recibe solo un código estable, nunca SQL ni detalles del proveedor.
            Throw New ImportRelatedDocumentRepositoryException("RELATED_DOCUMENT_QUERY_FAILED")
        End Try
    End Function

    Public Function Persistir(ByVal contexto As ContextoImportacionServicio,
                              ByVal intentId As String,
                              ByVal documento As DocumentoRelacionadoImportacion) As Boolean Implements IImportRelatedDocumentRepository.Persistir
        If contexto Is Nothing OrElse String.IsNullOrWhiteSpace(intentId) OrElse documento Is Nothing OrElse
           documento.IdImagen <= 0 OrElse Not SafeIdentifier(documento.NombreGabinete) Then Return False
        Const sql As String = "INSERT INTO workflow_import_related_document (intent_id,task_id,image_id,cabinet_name,sii_radicado,document_type_id,inscription_key,expected_expedient_id,discovery_status,destination_status,relation_status,cache_status,cabinet_index_status,electronic_index_status,xml_index_status,reconciliation_status,retryable,created_utc,updated_utc) VALUES (@intentId,@taskId,@imageId,@cabinet,@radicado,@documentTypeId,@inscriptionKey,@expedientId,'Descubierto',@destinationStatus,@relationStatus,@cacheStatus,@cabinetIndexStatus,@electronicIndexStatus,@xmlIndexStatus,@reconciliationStatus,0,@now,@now) ON DUPLICATE KEY UPDATE document_type_id=VALUES(document_type_id),inscription_key=VALUES(inscription_key),expected_expedient_id=VALUES(expected_expedient_id),destination_status=VALUES(destination_status),relation_status=VALUES(relation_status),cache_status=VALUES(cache_status),cabinet_index_status=VALUES(cabinet_index_status),electronic_index_status=VALUES(electronic_index_status),xml_index_status=VALUES(xml_index_status),reconciliation_status=VALUES(reconciliation_status),updated_utc=VALUES(updated_utc)"
        Dim parameters As New List(Of IDataParameter) From {
            New MySqlParameter("@intentId", intentId), New MySqlParameter("@taskId", contexto.IdTarea),
            New MySqlParameter("@imageId", documento.IdImagen), New MySqlParameter("@cabinet", documento.NombreGabinete),
            New MySqlParameter("@radicado", documento.RadicadoSii), New MySqlParameter("@documentTypeId", If(documento.IdTipoDocumental.HasValue, CType(documento.IdTipoDocumental.Value, Object), DBNull.Value)),
            New MySqlParameter("@inscriptionKey", If(String.IsNullOrWhiteSpace(documento.ClaveInscripcion), CType(DBNull.Value, Object), documento.ClaveInscripcion)),
            New MySqlParameter("@expedientId", If(documento.IdExpedienteEsperado.HasValue, CType(documento.IdExpedienteEsperado.Value, Object), DBNull.Value)),
            New MySqlParameter("@destinationStatus", documento.EstadoDestino.ToString()), New MySqlParameter("@relationStatus", documento.EstadoRelacion.ToString()),
            New MySqlParameter("@cacheStatus", documento.EstadoCache.ToString()), New MySqlParameter("@cabinetIndexStatus", documento.EstadoIndiceGabinete.ToString()),
            New MySqlParameter("@electronicIndexStatus", documento.EstadoIndiceSql.ToString()), New MySqlParameter("@xmlIndexStatus", documento.EstadoIndiceXml.ToString()),
            New MySqlParameter("@reconciliationStatus", documento.EstadoReconciliacion.ToString()), New MySqlParameter("@now", DateTime.UtcNow)}
        Try
            Using connection = _workflowConnections.CreateOpenConnection(ModuleContext(contexto))
                Return _executor.ExecuteNonQuery(connection, Nothing, sql, parameters) > 0
            End Using
        Catch
            Return False
        End Try
    End Function

    Private Shared Function MapRows(ByVal reader As IDataReader) As IList(Of RelatedDocumentRow)
        Dim rows As New List(Of RelatedDocumentRow)()
        While reader.Read()
            rows.Add(New RelatedDocumentRow With {
                .ImageId = Convert.ToInt64(reader("ID")),
                .Link = Convert.ToString(reader("ENLASE"))
            })
        End While
        Return rows
    End Function

    Private Shared Function Deduplicate(ByVal rows As IList(Of RelatedDocumentRow),
                                        ByVal taskId As Long,
                                        ByVal cabinet As String) As IList(Of DocumentoRelacionadoImportacion)
        Dim result As New List(Of DocumentoRelacionadoImportacion)()
        Dim seen As New HashSet(Of Long)()
        If rows Is Nothing Then Return result
        For Each row In rows
            If row.ImageId > 0 AndAlso seen.Add(row.ImageId) Then
                result.Add(New DocumentoRelacionadoImportacion With {
                    .IdTarea = taskId,
                    .IdImagen = row.ImageId,
                    .NombreGabinete = cabinet,
                    .RadicadoSii = row.Link,
                    .EstadoDestino = EstadoEfectoExpedienteImportacion.Pendiente,
                    .EstadoRelacion = EstadoRelacionDocumentoExpediente.NoConsultada
                })
            End If
        Next
        Return result
    End Function

    Private Shared Function SafeIdentifier(ByVal value As String) As Boolean
        Return Not String.IsNullOrWhiteSpace(value) AndAlso Regex.IsMatch(value, "^[A-Za-z_][A-Za-z0-9_]*$")
    End Function

    Private Shared Function ModuleContext(ByVal c As ContextoImportacionServicio) As ContextoModulo
        Return New ContextoModulo With {
            .CodigoModulo = "IMPORTAR_SERVICIO_WEB",
            .IdUsuario = c.IdUsuario,
            .IdGrupo = c.IdGrupo,
            .LoginUsuario = c.LoginUsuario
        }
    End Function

    Private NotInheritable Class RelatedDocumentRow
        Public Property ImageId As Long
        Public Property Link As String
    End Class
End Class

Public NotInheritable Class ImportRelatedDocumentRepositoryException
    Inherits Exception

    Public Sub New(ByVal safeCode As String)
        MyBase.New(safeCode)
        Codigo = safeCode
    End Sub

    Public ReadOnly Property Codigo As String
End Class
