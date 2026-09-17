Imports System
Imports System.Collections.Generic
Imports System.Data
Imports MySql.Data.MySqlClient

' Diario idempotente de vínculos confirmados; la relación física sigue siendo la autoridad.
Public NotInheritable Class MySqlImportDocumentLinkCacheRepository
    Implements IImportDocumentLinkCacheRepository

    Private ReadOnly _connections As IModuleConnectionFactory
    Private ReadOnly _executor As IDataExecutor

    Public Sub New(ByVal connections As IModuleConnectionFactory, ByVal executor As IDataExecutor)
        If connections Is Nothing OrElse executor Is Nothing Then Throw New ArgumentNullException("dependency")
        _connections = connections
        _executor = executor
    End Sub

    Public Function Obtener(ByVal contexto As ContextoImportacionServicio,
                            ByVal idImagen As Long,
                            ByVal nombreGabinete As String) As EntradaCacheVinculoDocumentoImportacion Implements IImportDocumentLinkCacheRepository.Obtener
        If Not ValidKey(contexto, idImagen, nombreGabinete) Then Return Nothing
        Try
            Using connection = _connections.CreateOpenConnection(ModuleContext(contexto))
                Return ReadOne(connection, contexto.IdTarea, idImagen, nombreGabinete)
            End Using
        Catch ex As Exception
            Throw New ImportDocumentLinkCacheException("DOCUMENT_LINK_CACHE_READ_FAILED")
        End Try
    End Function

    Public Function RegistrarVerificado(ByVal contexto As ContextoImportacionServicio,
                                        ByVal entrada As EntradaCacheVinculoDocumentoImportacion) As ResultadoEfectoExpedienteImportacion Implements IImportDocumentLinkCacheRepository.RegistrarVerificado
        If entrada Is Nothing OrElse
           Not ValidKey(contexto, entrada.IdImagen, entrada.NombreGabinete) OrElse
           entrada.IdTarea <> contexto.IdTarea OrElse entrada.IdExpedienteEsperado <= 0 OrElse
           String.IsNullOrWhiteSpace(entrada.RadicadoSii) OrElse
           entrada.EstadoRelacion <> EstadoRelacionDocumentoExpediente.Correcta Then
            Return Result(EstadoEfectoExpedienteImportacion.Fallido, "DOCUMENT_LINK_NOT_PHYSICALLY_CONFIRMED", False)
        End If

        Try
            Using connection = _connections.CreateOpenConnection(ModuleContext(contexto))
                Dim current = ReadOne(connection, entrada.IdTarea, entrada.IdImagen, entrada.NombreGabinete)
                If current IsNot Nothing Then Return Compare(current, entrada)

                Const insertSql As String =
                    "INSERT IGNORE INTO workflow_import_document_link_cache " &
                    "(task_id,image_id,cabinet_name,expected_expedient_id,sii_radicado,relation_status,created_utc,verified_utc) " &
                    "VALUES (@taskId,@imageId,@cabinetName,@expedientId,@radicado,@relationStatus,@createdUtc,@verifiedUtc)"
                _executor.ExecuteNonQuery(connection, Nothing, insertSql, Parameters(entrada))

                ' Relectura obligatoria: resuelve tanto inserción propia como carrera por la clave única.
                current = ReadOne(connection, entrada.IdTarea, entrada.IdImagen, entrada.NombreGabinete)
                If current Is Nothing Then
                    Return Result(EstadoEfectoExpedienteImportacion.ResultadoIncierto, "DOCUMENT_LINK_CACHE_WRITE_UNKNOWN", True)
                End If
                Return Compare(current, entrada)
            End Using
        Catch ex As Exception
            Return Result(EstadoEfectoExpedienteImportacion.Fallido, "DOCUMENT_LINK_CACHE_WRITE_FAILED", True)
        End Try
    End Function

    Private Function ReadOne(ByVal connection As IDbConnection,
                             ByVal taskId As Long,
                             ByVal imageId As Long,
                             ByVal cabinet As String) As EntradaCacheVinculoDocumentoImportacion
        Const sql As String =
            "SELECT task_id,image_id,cabinet_name,expected_expedient_id,sii_radicado,relation_status,created_utc,verified_utc " &
            "FROM workflow_import_document_link_cache " &
            "WHERE task_id=@taskId AND image_id=@imageId AND cabinet_name=@cabinetName LIMIT 2"
        Dim rows = _executor.ExecuteReader(connection, Nothing, sql,
            New List(Of IDataParameter) From {
                P("@taskId", taskId), P("@imageId", imageId), P("@cabinetName", cabinet)
            }, AddressOf MapRows)
        If rows.Count <> 1 Then Return Nothing
        Return rows(0)
    End Function

    Private Shared Function MapRows(ByVal reader As IDataReader) As IList(Of EntradaCacheVinculoDocumentoImportacion)
        Dim rows As New List(Of EntradaCacheVinculoDocumentoImportacion)()
        While reader.Read()
            Dim relation As EstadoRelacionDocumentoExpediente = EstadoRelacionDocumentoExpediente.NoConsultada
            [Enum].TryParse(Of EstadoRelacionDocumentoExpediente)(Convert.ToString(reader("relation_status")), True, relation)
            rows.Add(New EntradaCacheVinculoDocumentoImportacion With {
                .IdTarea = Convert.ToInt64(reader("task_id")),
                .IdImagen = Convert.ToInt64(reader("image_id")),
                .NombreGabinete = Convert.ToString(reader("cabinet_name")),
                .IdExpedienteEsperado = Convert.ToInt64(reader("expected_expedient_id")),
                .RadicadoSii = Convert.ToString(reader("sii_radicado")),
                .EstadoRelacion = relation,
                .FechaCreacionUtc = Convert.ToDateTime(reader("created_utc")),
                .FechaVerificacionUtc = NullableDate(reader("verified_utc"))
            })
        End While
        Return rows
    End Function

    Private Shared Function Compare(ByVal current As EntradaCacheVinculoDocumentoImportacion,
                                    ByVal expected As EntradaCacheVinculoDocumentoImportacion) As ResultadoEfectoExpedienteImportacion
        If current.IdExpedienteEsperado <> expected.IdExpedienteEsperado OrElse
           Not String.Equals(current.RadicadoSii, expected.RadicadoSii, StringComparison.Ordinal) OrElse
           current.EstadoRelacion <> EstadoRelacionDocumentoExpediente.Correcta Then
            Return Result(EstadoEfectoExpedienteImportacion.Conflicto, "DOCUMENT_LINK_CACHE_CONFLICT", False)
        End If
        Return Result(EstadoEfectoExpedienteImportacion.Confirmado, "DOCUMENT_LINK_CACHE_CONFIRMED", False)
    End Function

    Private Shared Function Parameters(ByVal value As EntradaCacheVinculoDocumentoImportacion) As IEnumerable(Of IDataParameter)
        Dim created = If(value.FechaCreacionUtc = DateTime.MinValue, DateTime.UtcNow, value.FechaCreacionUtc)
        Dim verified = If(value.FechaVerificacionUtc.HasValue, CType(value.FechaVerificacionUtc.Value, Object), DateTime.UtcNow)
        Return New List(Of IDataParameter) From {
            P("@taskId", value.IdTarea), P("@imageId", value.IdImagen), P("@cabinetName", value.NombreGabinete),
            P("@expedientId", value.IdExpedienteEsperado), P("@radicado", value.RadicadoSii),
            P("@relationStatus", EstadoRelacionDocumentoExpediente.Correcta.ToString()),
            P("@createdUtc", created), P("@verifiedUtc", verified)
        }
    End Function

    Private Shared Function ValidKey(ByVal c As ContextoImportacionServicio, ByVal imageId As Long, ByVal cabinet As String) As Boolean
        Return c IsNot Nothing AndAlso c.IdTarea > 0 AndAlso imageId > 0 AndAlso Not String.IsNullOrWhiteSpace(cabinet)
    End Function

    Private Shared Function NullableDate(ByVal value As Object) As Nullable(Of DateTime)
        If value Is Nothing OrElse Convert.IsDBNull(value) Then Return Nothing
        Return Convert.ToDateTime(value)
    End Function

    Private Shared Function P(ByVal name As String, ByVal value As Object) As IDataParameter
        Return New MySqlParameter(name, If(value, DBNull.Value))
    End Function

    Private Shared Function ModuleContext(ByVal c As ContextoImportacionServicio) As ContextoModulo
        Return New ContextoModulo With {
            .CodigoModulo = "IMPORTAR_SERVICIO_WEB", .IdUsuario = c.IdUsuario,
            .IdGrupo = c.IdGrupo, .LoginUsuario = c.LoginUsuario
        }
    End Function

    Private Shared Function Result(ByVal state As EstadoEfectoExpedienteImportacion,
                                  ByVal code As String,
                                  ByVal retryable As Boolean) As ResultadoEfectoExpedienteImportacion
        Return New ResultadoEfectoExpedienteImportacion With {
            .Estado = state, .Codigo = code, .Reintentable = retryable
        }
    End Function
End Class

Public NotInheritable Class ImportDocumentLinkCacheException
    Inherits Exception

    Public Sub New(ByVal safeCode As String)
        MyBase.New(safeCode)
        Codigo = safeCode
    End Sub

    Public ReadOnly Property Codigo As String
End Class
