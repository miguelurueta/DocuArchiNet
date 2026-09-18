Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Security.Cryptography
Imports System.Text
Imports MySql.Data.MySqlClient

' Migración moderna de la caché SII de creación. La tabla legacy se conserva para
' compatibilidad, pero esta ruta no invoca sus funciones ni acepta Rows(0) ambiguo.
Public NotInheritable Class MySqlImportExpedientCacheRepository
    Implements IImportExpedientCacheRepository

    Private ReadOnly _connections As IModuleConnectionFactory
    Private ReadOnly _executor As IDataExecutor
    Private ReadOnly _transactions As ITransactionFactory

    Public Sub New(ByVal connections As IModuleConnectionFactory,
                   ByVal executor As IDataExecutor,
                   ByVal transactions As ITransactionFactory)
        If connections Is Nothing OrElse executor Is Nothing OrElse transactions Is Nothing Then Throw New ArgumentNullException("dependency")
        _connections = connections
        _executor = executor
        _transactions = transactions
    End Sub

    Public Function Obtener(ByVal contexto As ContextoImportacionServicio,
                            ByVal inscripcion As InscripcionImportacion) As InscripcionImportacion Implements IImportExpedientCacheRepository.Obtener
        If Not Valid(contexto, inscripcion, False) Then Return FailureCopy(inscripcion, EstadoEfectoExpedienteImportacion.ResultadoIncierto)
        Try
            Using connection = _connections.CreateOpenConnection(ModuleContext(contexto))
                Return ReadOne(connection, Nothing, inscripcion)
            End Using
        Catch
            Return FailureCopy(inscripcion, EstadoEfectoExpedienteImportacion.ResultadoIncierto)
        End Try
    End Function

    Public Function RegistrarVerificado(ByVal contexto As ContextoImportacionServicio,
                                        ByVal inscripcion As InscripcionImportacion) As ResultadoEfectoExpedienteImportacion Implements IImportExpedientCacheRepository.RegistrarVerificado
        If Not Valid(contexto, inscripcion, True) Then Return Result(EstadoEfectoExpedienteImportacion.Fallido, "EXPEDIENT_CACHE_INPUT_INVALID", False)
        Dim lockName = BuildLockName(inscripcion)
        Try
            Using connection = _connections.CreateOpenConnection(ModuleContext(contexto))
                If Not AcquireLock(connection, lockName) Then Return Result(EstadoEfectoExpedienteImportacion.ResultadoIncierto, "EXPEDIENT_CACHE_LOCK_UNAVAILABLE", True)
                Try
                    Using transaction = _transactions.BeginTransaction(connection)
                        Dim current = ReadOne(connection, transaction, inscripcion)
                        If current IsNot Nothing Then
                            If current.EstadoCache = EstadoEfectoExpedienteImportacion.Conflicto Then
                                transaction.Rollback()
                                Return Result(EstadoEfectoExpedienteImportacion.Conflicto, "EXPEDIENT_CACHE_CONFLICT", False)
                            End If
                            If Not SameDestination(current, inscripcion) Then
                                transaction.Rollback()
                                Return Result(EstadoEfectoExpedienteImportacion.Conflicto, "EXPEDIENT_CACHE_CONFLICT", False)
                            End If
                            EnsureRadicadoRelation(connection, transaction, inscripcion)
                            transaction.Commit()
                            Return Result(EstadoEfectoExpedienteImportacion.Confirmado, "EXPEDIENT_CACHE_CONFIRMED", False)
                        End If

                        Const insertCache As String =
                            "INSERT INTO ra_sii_cache_exepediente " &
                            "(RadicadoSII,CodigoBarras,NitIdentificacion,Rsocial,NombreGabinete,EstadoVinculaDocumento,Matricula,IdExpediente,FechaRegistroCache,EstadoPadre) " &
                            "VALUES (@radicado,@barcode,@subjectId,@subjectName,@cabinet,0,@identity,@expedientId,NOW(),@parentState)"
                        Dim inserted = _executor.ExecuteNonQuery(connection, transaction, insertCache, CacheParameters(inscripcion))
                        If inserted <> 1 Then
                            transaction.Rollback()
                            Return Result(EstadoEfectoExpedienteImportacion.ResultadoIncierto, "EXPEDIENT_CACHE_WRITE_UNCERTAIN", True)
                        End If
                        EnsureRadicadoRelation(connection, transaction, inscripcion)
                        transaction.Commit()
                    End Using

                    Dim confirmed = ReadOne(connection, Nothing, inscripcion)
                    If confirmed Is Nothing OrElse confirmed.EstadoCache = EstadoEfectoExpedienteImportacion.Conflicto OrElse Not SameDestination(confirmed, inscripcion) Then
                        Return Result(EstadoEfectoExpedienteImportacion.ResultadoIncierto, "EXPEDIENT_CACHE_WRITE_UNCERTAIN", True)
                    End If
                    Return Result(EstadoEfectoExpedienteImportacion.Confirmado, "EXPEDIENT_CACHE_CONFIRMED", False)
                Finally
                    ReleaseLock(connection, lockName)
                End Try
            End Using
        Catch
            Return Result(EstadoEfectoExpedienteImportacion.ResultadoIncierto, "EXPEDIENT_CACHE_WRITE_UNCERTAIN", True)
        End Try
    End Function

    Private Function ReadOne(ByVal connection As IDbConnection,
                             ByVal transaction As IDbTransaction,
                             ByVal expected As InscripcionImportacion) As InscripcionImportacion
        Const sql As String =
            "SELECT RadicadoSII,NitIdentificacion,Rsocial,NombreGabinete,Matricula,IdExpediente,EstadoPadre " &
            "FROM ra_sii_cache_exepediente WHERE Matricula=@identity AND NombreGabinete=@cabinet " &
            "ORDER BY id_ra_sii_cache_exepediente LIMIT 2"
        Dim rows = _executor.ExecuteReader(connection, transaction, sql,
            New List(Of IDataParameter) From {P("@identity", Identity(expected)), P("@cabinet", expected.NombreGabinete.Trim())}, AddressOf MapRows)
        If rows.Count = 0 Then Return Nothing
        If rows.Count > 1 Then Return FailureCopy(expected, EstadoEfectoExpedienteImportacion.Conflicto)
        Return rows(0)
    End Function

    Private Sub EnsureRadicadoRelation(ByVal connection As IDbConnection,
                                       ByVal transaction As IDbTransaction,
                                       ByVal value As InscripcionImportacion)
        Const sql As String =
            "INSERT INTO ra_relacion_radicado_externo_expediente " &
            "(expediente_archivo_ID_EXPEDIENTE,RadicadoExterno,FechaRegistro) " &
            "SELECT @expedientId,@radicado,NOW() FROM DUAL WHERE NOT EXISTS " &
            "(SELECT 1 FROM ra_relacion_radicado_externo_expediente WHERE expediente_archivo_ID_EXPEDIENTE=@expedientId AND RadicadoExterno=@radicado)"
        _executor.ExecuteNonQuery(connection, transaction, sql,
            New List(Of IDataParameter) From {P("@expedientId", value.IdExpediente.Value), P("@radicado", value.RadicadoSii.Trim())})
        Const verifySql As String =
            "SELECT COUNT(*) FROM ra_relacion_radicado_externo_expediente " &
            "WHERE expediente_archivo_ID_EXPEDIENTE=@expedientId AND RadicadoExterno=@radicado"
        Dim count = Convert.ToInt32(_executor.ExecuteScalar(connection, transaction, verifySql,
            New List(Of IDataParameter) From {P("@expedientId", value.IdExpediente.Value), P("@radicado", value.RadicadoSii.Trim())}))
        If count <> 1 Then Throw New InvalidOperationException("EXPEDIENT_RADICADO_RELATION_CONFLICT")
    End Sub

    Private Shared Function MapRows(ByVal reader As IDataReader) As IList(Of InscripcionImportacion)
        Dim rows As New List(Of InscripcionImportacion)()
        While reader.Read()
            rows.Add(New InscripcionImportacion With {
                .RadicadoSii = Convert.ToString(reader("RadicadoSII")),
                .IdentificacionSujeto = Convert.ToString(reader("NitIdentificacion")),
                .RazonSocial = Convert.ToString(reader("Rsocial")),
                .NombreGabinete = Convert.ToString(reader("NombreGabinete")),
                .MatriculaNormalizada = Convert.ToString(reader("Matricula")),
                .IdExpediente = New Nullable(Of Long)(Convert.ToInt64(reader("IdExpediente"))),
                .RolExpediente = If(Convert.ToInt32(reader("EstadoPadre")) = 0, RolExpedienteImportacion.Secundario, RolExpedienteImportacion.Primario),
                .EstadoCache = EstadoEfectoExpedienteImportacion.Confirmado
            })
        End While
        Return rows
    End Function

    Private Shared Function CacheParameters(ByVal value As InscripcionImportacion) As IEnumerable(Of IDataParameter)
        Return New List(Of IDataParameter) From {
            P("@radicado", value.RadicadoSii.Trim()), P("@barcode", value.RadicadoSii.Trim()),
            P("@subjectId", Clean(value.IdentificacionSujeto)), P("@subjectName", Truncate(Clean(value.RazonSocial), 40)),
            P("@cabinet", value.NombreGabinete.Trim()), P("@identity", Identity(value)),
            P("@expedientId", value.IdExpediente.Value),
            P("@parentState", If(value.RolExpediente = RolExpedienteImportacion.Secundario, 0, 1))
        }
    End Function

    Private Function AcquireLock(ByVal connection As IDbConnection, ByVal lockName As String) As Boolean
        Dim value = _executor.ExecuteScalar(connection, Nothing, "SELECT GET_LOCK(@lockName,5)", New List(Of IDataParameter) From {P("@lockName", lockName)})
        Return value IsNot Nothing AndAlso Not Convert.IsDBNull(value) AndAlso Convert.ToInt32(value) = 1
    End Function

    Private Sub ReleaseLock(ByVal connection As IDbConnection, ByVal lockName As String)
        Try
            _executor.ExecuteScalar(connection, Nothing, "SELECT RELEASE_LOCK(@lockName)", New List(Of IDataParameter) From {P("@lockName", lockName)})
        Catch
            ' El cierre de la conexión también libera el advisory lock.
        End Try
    End Sub

    Private Shared Function BuildLockName(ByVal value As InscripcionImportacion) As String
        Using sha = SHA256.Create()
            Dim raw = Encoding.UTF8.GetBytes(value.NombreGabinete.Trim().ToUpperInvariant() & "|" & Identity(value))
            Dim hash = BitConverter.ToString(sha.ComputeHash(raw)).Replace("-", String.Empty)
            Return "DOC67_CACHE_" & hash.Substring(0, 48)
        End Using
    End Function

    Private Shared Function SameDestination(ByVal current As InscripcionImportacion, ByVal expected As InscripcionImportacion) As Boolean
        Return current IsNot Nothing AndAlso current.IdExpediente.HasValue AndAlso expected.IdExpediente.HasValue AndAlso
            current.IdExpediente.Value = expected.IdExpediente.Value
    End Function

    Private Shared Function Valid(ByVal context As ContextoImportacionServicio,
                                  ByVal value As InscripcionImportacion,
                                  ByVal requireDestination As Boolean) As Boolean
        Return context IsNot Nothing AndAlso context.IdTarea > 0 AndAlso value IsNot Nothing AndAlso
            Not String.IsNullOrWhiteSpace(value.NombreGabinete) AndAlso Not String.IsNullOrWhiteSpace(Identity(value)) AndAlso
            (Not requireDestination OrElse (value.IdExpediente.HasValue AndAlso value.IdExpediente.Value > 0 AndAlso Not String.IsNullOrWhiteSpace(value.RadicadoSii)))
    End Function

    Private Shared Function Identity(ByVal value As InscripcionImportacion) As String
        If value Is Nothing Then Return String.Empty
        Dim normalizedIdentity = If(String.IsNullOrWhiteSpace(value.MatriculaNormalizada), If(String.IsNullOrWhiteSpace(value.MatriculaPropietario), value.Matricula, value.MatriculaPropietario), value.MatriculaNormalizada)
        Return Clean(normalizedIdentity).Replace("S0", String.Empty)
    End Function

    Private Shared Function FailureCopy(ByVal source As InscripcionImportacion,
                                        ByVal state As EstadoEfectoExpedienteImportacion) As InscripcionImportacion
        Return New InscripcionImportacion With {
            .ClaveInscripcion = If(source Is Nothing, Nothing, source.ClaveInscripcion),
            .NombreGabinete = If(source Is Nothing, Nothing, source.NombreGabinete),
            .EstadoCache = state
        }
    End Function

    Private Shared Function Clean(ByVal value As String) As String
        Return If(value, String.Empty).Trim()
    End Function

    Private Shared Function Truncate(ByVal value As String, ByVal maximum As Integer) As String
        Return If(value.Length <= maximum, value, value.Substring(0, maximum))
    End Function

    Private Shared Function P(ByVal name As String, ByVal value As Object) As IDataParameter
        Return New MySqlParameter(name, If(value, DBNull.Value))
    End Function

    Private Shared Function ModuleContext(ByVal c As ContextoImportacionServicio) As ContextoModulo
        Return New ContextoModulo With {.CodigoModulo = "IMPORTAR_SERVICIO_WEB", .IdUsuario = c.IdUsuario, .IdGrupo = c.IdGrupo, .LoginUsuario = c.LoginUsuario}
    End Function

    Private Shared Function Result(ByVal state As EstadoEfectoExpedienteImportacion,
                                  ByVal code As String,
                                  ByVal retryable As Boolean) As ResultadoEfectoExpedienteImportacion
        Return New ResultadoEfectoExpedienteImportacion With {.Estado = state, .Codigo = code, .Reintentable = retryable}
    End Function
End Class
