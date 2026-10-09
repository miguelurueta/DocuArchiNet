Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports MySql.Data.MySqlClient

'Persistencia central de Login 2FA sobre el contrato físico compartido con DocuArchiCore.
Public NotInheritable Class MySqlSecondFactorChallengeRepository
    Implements ISecondFactorChallengeRepository

    Private Const ProviderEmail As String = "EMAIL"
    Private Const SelectColumns As String = "ChallengeId,AuthUserId,Provider,CodeHash,ExpiresAtUtc,Consumed,Attempts,CreatedAtUtc,AuthPayloadJson"

    Private ReadOnly _connections As IModuleConnectionFactory
    Private ReadOnly _executor As IDataExecutor
    Private ReadOnly _transactions As ITransactionFactory
    Private ReadOnly _centralContext As ContextoModulo
    Private ReadOnly _clock As ISecondFactorClock

    Public Sub New(ByVal connections As IModuleConnectionFactory,
                   ByVal executor As IDataExecutor,
                   ByVal transactions As ITransactionFactory,
                   ByVal centralContext As ContextoModulo,
                   ByVal clock As ISecondFactorClock)
        If connections Is Nothing Then Throw New ArgumentNullException(NameOf(connections))
        If executor Is Nothing Then Throw New ArgumentNullException(NameOf(executor))
        If transactions Is Nothing Then Throw New ArgumentNullException(NameOf(transactions))
        If centralContext Is Nothing OrElse Not centralContext.EsValido() Then Throw New ArgumentException("El contexto central es inválido.", NameOf(centralContext))
        If clock Is Nothing Then Throw New ArgumentNullException(NameOf(clock))
        _connections = connections
        _executor = executor
        _transactions = transactions
        _centralContext = New ContextoModulo With {
            .CodigoModulo = centralContext.CodigoModulo,
            .IdUsuario = centralContext.IdUsuario,
            .IdGrupo = centralContext.IdGrupo,
            .LoginUsuario = centralContext.LoginUsuario}
        _clock = clock
    End Sub

    Public Function Create(ByVal challenge As SegundoFactorChallenge,
                           ByVal protectedCode As String,
                           ByVal sessionBindingHash As String) As Boolean Implements ISecondFactorChallengeRepository.Create
        ValidateNewChallenge(challenge, protectedCode, sessionBindingHash)
        Return InTransaction(Function(connection, transaction) InsertChallenge(connection, transaction, challenge, protectedCode))
    End Function

    Public Function GetForVerification(ByVal challengeId As Guid,
                                       ByVal sessionBindingHash As String) As SegundoFactorChallenge Implements ISecondFactorChallengeRepository.GetForVerification
        Throw New NotSupportedException("Use GetVerificationData; la tabla compartida no almacena LoginNormalizado.")
    End Function

    Public Function GetVerificationData(ByVal challengeId As Guid,
                                        ByVal sessionBindingHash As String) As SecondFactorChallengeVerificationData Implements ISecondFactorChallengeRepository.GetVerificationData
        ValidateChallengeId(challengeId)
        ValidateSessionBindingHash(sessionBindingHash)
        Dim observedAtUtc As DateTime = ClockUtcNow()
        Using connection As IDbConnection = _connections.CreateOpenConnection(_centralContext)
            Const sql As String = "SELECT " & SelectColumns & " FROM ra_auth_second_factor_challenge WHERE ChallengeId=@challengeId AND Provider=@provider AND Consumed=0 AND Attempts<@maxAttempts AND ExpiresAtUtc>@observedAt LIMIT 1"
            Dim row As PersistenceRow = _executor.ExecuteReader(connection, Nothing, sql,
                Parameters(P("@challengeId", challengeId.ToString("D")), P("@provider", ProviderEmail), P("@maxAttempts", SegundoFactorConfiguration.MaxAttempts), P("@observedAt", observedAtUtc)), AddressOf MapSingleRow)
            If row Is Nothing Then Return Nothing
            Return New SecondFactorChallengeVerificationData(row.ToStoredChallenge(observedAtUtc), row.ProtectedCode)
        End Using
    End Function

    Public Function MarkSent(ByVal challengeId As Guid,
                             ByVal sentAtUtc As DateTime) As Boolean Implements ISecondFactorChallengeRepository.MarkSent
        ValidateChallengeId(challengeId)
        EnsureUtc(sentAtUtc, NameOf(sentAtUtc))
        Return ExistsActive(challengeId, sentAtUtc)
    End Function

    Public Function MarkDeliveryFailed(ByVal challengeId As Guid,
                                       ByVal failedAtUtc As DateTime) As Boolean Implements ISecondFactorChallengeRepository.MarkDeliveryFailed
        ValidateChallengeId(challengeId)
        EnsureUtc(failedAtUtc, NameOf(failedAtUtc))
        Return ConsumeIfActive(challengeId, failedAtUtc)
    End Function

    Public Function RegisterFailedAttempt(ByVal challengeId As Guid,
                                          ByVal expectedAttempts As Integer) As SegundoFactorChallenge Implements ISecondFactorChallengeRepository.RegisterFailedAttempt
        Throw New NotSupportedException("Use RegisterFailedAttemptData; la tabla compartida no almacena LoginNormalizado.")
    End Function

    Public Function RegisterFailedAttemptData(ByVal challengeId As Guid,
                                              ByVal expectedAttempts As Integer) As SecondFactorStoredChallenge Implements ISecondFactorChallengeRepository.RegisterFailedAttemptData
        ValidateChallengeId(challengeId)
        If expectedAttempts < 0 OrElse expectedAttempts >= SegundoFactorConfiguration.MaxAttempts Then Throw New ArgumentOutOfRangeException(NameOf(expectedAttempts))
        Return InTransaction(
            Function(connection, transaction)
                Dim row As PersistenceRow = LockRow(connection, transaction, challengeId)
                Dim observedAtUtc As DateTime = ClockUtcNow()
                If Not IsActive(row, observedAtUtc) OrElse row.Attempts <> expectedAttempts Then Return Nothing
                Dim nextAttempts As Integer = expectedAttempts + 1
                Const sql As String = "UPDATE ra_auth_second_factor_challenge SET Attempts=@nextAttempts WHERE ChallengeId=@challengeId AND Consumed=0 AND Attempts=@expectedAttempts AND ExpiresAtUtc>@observedAt"
                If _executor.ExecuteNonQuery(connection, transaction, sql,
                    Parameters(P("@nextAttempts", nextAttempts), P("@challengeId", challengeId.ToString("D")), P("@expectedAttempts", expectedAttempts), P("@observedAt", observedAtUtc))) <> 1 Then Return Nothing
                row.Attempts = nextAttempts
                Return row.ToStoredChallenge(observedAtUtc)
            End Function)
    End Function

    Public Function TryBeginFinalization(ByVal challengeId As Guid,
                                         ByVal expectedAttempts As Integer) As Boolean Implements ISecondFactorChallengeRepository.TryBeginFinalization
        ValidateChallengeId(challengeId)
        If expectedAttempts < 0 OrElse expectedAttempts >= SegundoFactorConfiguration.MaxAttempts Then Throw New ArgumentOutOfRangeException(NameOf(expectedAttempts))
        Return InTransaction(
            Function(connection, transaction)
                Dim row As PersistenceRow = LockRow(connection, transaction, challengeId)
                Dim observedAtUtc As DateTime = ClockUtcNow()
                If Not IsActive(row, observedAtUtc) OrElse row.Attempts <> expectedAttempts Then Return False
                Const sql As String = "UPDATE ra_auth_second_factor_challenge SET Consumed=1 WHERE ChallengeId=@challengeId AND Consumed=0 AND Attempts=@expectedAttempts AND ExpiresAtUtc>@observedAt"
                Return _executor.ExecuteNonQuery(connection, transaction, sql,
                    Parameters(P("@challengeId", challengeId.ToString("D")), P("@expectedAttempts", expectedAttempts), P("@observedAt", observedAtUtc))) = 1
            End Function)
    End Function

    Public Function Complete(ByVal challengeId As Guid) As Boolean Implements ISecondFactorChallengeRepository.Complete
        ValidateChallengeId(challengeId)
        Return IsConsumed(challengeId)
    End Function

    Public Function FailFinalization(ByVal challengeId As Guid) As Boolean Implements ISecondFactorChallengeRepository.FailFinalization
        ValidateChallengeId(challengeId)
        Return IsConsumed(challengeId)
    End Function

    Public Function Revoke(ByVal challengeId As Guid) As Boolean Implements ISecondFactorChallengeRepository.Revoke
        ValidateChallengeId(challengeId)
        Return ConsumeIfActive(challengeId, ClockUtcNow())
    End Function

    Public Function ReplaceForResend(ByVal previousChallengeId As Guid,
                                     ByVal replacement As SegundoFactorChallenge,
                                     ByVal protectedCode As String,
                                     ByVal sessionBindingHash As String,
                                     ByVal requestedAtUtc As DateTime) As Boolean Implements ISecondFactorChallengeRepository.ReplaceForResend
        ValidateChallengeId(previousChallengeId)
        EnsureUtc(requestedAtUtc, NameOf(requestedAtUtc))
        ValidateNewChallenge(replacement, protectedCode, sessionBindingHash)
        If replacement.ResendCount < 1 OrElse replacement.ResendCount > SegundoFactorConfiguration.MaxResends Then Return False
        Return InTransaction(
            Function(connection, transaction)
                Dim previous As PersistenceRow = LockRow(connection, transaction, previousChallengeId)
                If Not IsActive(previous, requestedAtUtc) OrElse requestedAtUtc < previous.CreatedAtUtc.AddSeconds(SegundoFactorConfiguration.ResendCooldownSeconds) Then Return False
                If replacement.Identity.ClaveCanonica <> previous.CanonicalIdentity Then Return False
                Const revokeSql As String = "UPDATE ra_auth_second_factor_challenge SET Consumed=1 WHERE ChallengeId=@challengeId AND Consumed=0 AND Attempts<@maxAttempts AND ExpiresAtUtc>@observedAt"
                If _executor.ExecuteNonQuery(connection, transaction, revokeSql,
                    Parameters(P("@challengeId", previousChallengeId.ToString("D")), P("@maxAttempts", SegundoFactorConfiguration.MaxAttempts), P("@observedAt", requestedAtUtc))) <> 1 Then Return False
                If Not InsertChallenge(connection, transaction, replacement, protectedCode) Then Throw New InvalidOperationException("SECOND_FACTOR_REPLACEMENT_NOT_INSERTED")
                Return True
            End Function)
    End Function

    Public Function Expire(ByVal challengeId As Guid,
                           ByVal observedAtUtc As DateTime) As Boolean Implements ISecondFactorChallengeRepository.Expire
        ValidateChallengeId(challengeId)
        EnsureUtc(observedAtUtc, NameOf(observedAtUtc))
        Using connection As IDbConnection = _connections.CreateOpenConnection(_centralContext)
            Const sql As String = "UPDATE ra_auth_second_factor_challenge SET Consumed=1 WHERE ChallengeId=@challengeId AND Consumed=0 AND ExpiresAtUtc<=@observedAt"
            Return _executor.ExecuteNonQuery(connection, Nothing, sql, Parameters(P("@challengeId", challengeId.ToString("D")), P("@observedAt", observedAtUtc))) = 1
        End Using
    End Function

    Private Function InsertChallenge(ByVal connection As IDbConnection,
                                     ByVal transaction As IDbTransaction,
                                     ByVal challenge As SegundoFactorChallenge,
                                     ByVal protectedCode As String) As Boolean
        Const sql As String = "INSERT INTO ra_auth_second_factor_challenge (ChallengeId,AuthUserId,Provider,CodeHash,ExpiresAtUtc,Consumed,Attempts,CreatedAtUtc,AuthPayloadJson) VALUES (@challengeId,@authUserId,@provider,@codeHash,@expiresAt,0,@attempts,@createdAt,NULL)"
        Return _executor.ExecuteNonQuery(connection, transaction, sql,
            Parameters(P("@challengeId", challenge.ChallengeId.ToString("D")), P("@authUserId", challenge.Identity.ClaveCanonica), P("@provider", ProviderEmail), P("@codeHash", protectedCode.Trim()), P("@expiresAt", challenge.ExpiresAtUtc), P("@attempts", challenge.Attempts), P("@createdAt", challenge.CreatedAtUtc))) = 1
    End Function

    Private Function LockRow(ByVal connection As IDbConnection,
                             ByVal transaction As IDbTransaction,
                             ByVal challengeId As Guid) As PersistenceRow
        Const sql As String = "SELECT " & SelectColumns & " FROM ra_auth_second_factor_challenge WHERE ChallengeId=@challengeId AND Provider=@provider FOR UPDATE"
        Return _executor.ExecuteReader(connection, transaction, sql, Parameters(P("@challengeId", challengeId.ToString("D")), P("@provider", ProviderEmail)), AddressOf MapSingleRow)
    End Function

    Private Function ExistsActive(ByVal challengeId As Guid, ByVal observedAtUtc As DateTime) As Boolean
        Using connection As IDbConnection = _connections.CreateOpenConnection(_centralContext)
            Const sql As String = "SELECT COUNT(*) FROM ra_auth_second_factor_challenge WHERE ChallengeId=@challengeId AND Provider=@provider AND Consumed=0 AND Attempts<@maxAttempts AND ExpiresAtUtc>@observedAt"
            Return Convert.ToInt32(_executor.ExecuteScalar(connection, Nothing, sql,
                Parameters(P("@challengeId", challengeId.ToString("D")), P("@provider", ProviderEmail), P("@maxAttempts", SegundoFactorConfiguration.MaxAttempts), P("@observedAt", observedAtUtc))), CultureInfo.InvariantCulture) = 1
        End Using
    End Function

    Private Function IsConsumed(ByVal challengeId As Guid) As Boolean
        Using connection As IDbConnection = _connections.CreateOpenConnection(_centralContext)
            Const sql As String = "SELECT COUNT(*) FROM ra_auth_second_factor_challenge WHERE ChallengeId=@challengeId AND Provider=@provider AND Consumed=1"
            Return Convert.ToInt32(_executor.ExecuteScalar(connection, Nothing, sql, Parameters(P("@challengeId", challengeId.ToString("D")), P("@provider", ProviderEmail))), CultureInfo.InvariantCulture) = 1
        End Using
    End Function

    Private Function ConsumeIfActive(ByVal challengeId As Guid, ByVal observedAtUtc As DateTime) As Boolean
        Using connection As IDbConnection = _connections.CreateOpenConnection(_centralContext)
            Const sql As String = "UPDATE ra_auth_second_factor_challenge SET Consumed=1 WHERE ChallengeId=@challengeId AND Provider=@provider AND Consumed=0 AND Attempts<@maxAttempts AND ExpiresAtUtc>@observedAt"
            Return _executor.ExecuteNonQuery(connection, Nothing, sql,
                Parameters(P("@challengeId", challengeId.ToString("D")), P("@provider", ProviderEmail), P("@maxAttempts", SegundoFactorConfiguration.MaxAttempts), P("@observedAt", observedAtUtc))) = 1
        End Using
    End Function

    Private Function InTransaction(Of T)(ByVal operation As Func(Of IDbConnection, IDbTransaction, T)) As T
        Using connection As IDbConnection = _connections.CreateOpenConnection(_centralContext)
            Using transaction As IDbTransaction = _transactions.BeginTransaction(connection)
                Try
                    Dim result As T = operation(connection, transaction)
                    transaction.Commit()
                    Return result
                Catch
                    Try
                        transaction.Rollback()
                    Catch
                    End Try
                    Throw
                End Try
            End Using
        End Using
    End Function

    Private Shared Function MapSingleRow(ByVal reader As IDataReader) As PersistenceRow
        If Not reader.Read() Then Return Nothing
        Return New PersistenceRow With {
            .ChallengeId = Guid.Parse(Convert.ToString(reader("ChallengeId"), CultureInfo.InvariantCulture)),
            .CanonicalIdentity = Convert.ToString(reader("AuthUserId"), CultureInfo.InvariantCulture),
            .ProtectedCode = Convert.ToString(reader("CodeHash"), CultureInfo.InvariantCulture),
            .ExpiresAtUtc = Utc(reader, "ExpiresAtUtc"),
            .Consumed = Convert.ToBoolean(reader("Consumed"), CultureInfo.InvariantCulture),
            .Attempts = Convert.ToInt32(reader("Attempts"), CultureInfo.InvariantCulture),
            .CreatedAtUtc = Utc(reader, "CreatedAtUtc")}
    End Function

    Private Shared Function IsActive(ByVal row As PersistenceRow, ByVal observedAtUtc As DateTime) As Boolean
        Return row IsNot Nothing AndAlso Not row.Consumed AndAlso row.Attempts < SegundoFactorConfiguration.MaxAttempts AndAlso row.ExpiresAtUtc > observedAtUtc
    End Function

    Private Shared Sub ValidateNewChallenge(ByVal challenge As SegundoFactorChallenge,
                                            ByVal protectedCode As String,
                                            ByVal sessionBindingHash As String)
        If challenge Is Nothing Then Throw New ArgumentNullException(NameOf(challenge))
        If challenge.State <> SegundoFactorChallengeState.CREATED Then Throw New ArgumentException("Un challenge nuevo debe iniciar en CREATED.", NameOf(challenge))
        ValidateSessionBindingHash(sessionBindingHash)
        ValidateProtectedCode(protectedCode)
    End Sub

    Private Shared Sub ValidateProtectedCode(ByVal protectedCode As String)
        If String.IsNullOrWhiteSpace(protectedCode) Then Throw New ArgumentException("El código protegido es obligatorio.", NameOf(protectedCode))
        Dim parts As String() = protectedCode.Trim().Split(":"c)
        If parts.Length <> 3 OrElse parts(0) <> "v1" OrElse String.IsNullOrWhiteSpace(parts(1)) Then Throw New ArgumentException("El código protegido debe usar v1:keyId:mac.", NameOf(protectedCode))
        Try
            If Convert.FromBase64String(parts(2)).Length <> 32 Then Throw New FormatException()
        Catch ex As FormatException
            Throw New ArgumentException("El MAC protegido es inválido.", NameOf(protectedCode), ex)
        End Try
    End Sub

    Private Shared Sub ValidateChallengeId(ByVal challengeId As Guid)
        If challengeId = Guid.Empty Then Throw New ArgumentException("El challenge es obligatorio.", NameOf(challengeId))
    End Sub

    Private Shared Sub ValidateSessionBindingHash(ByVal sessionBindingHash As String)
        If String.IsNullOrWhiteSpace(sessionBindingHash) Then Throw New ArgumentException("El vínculo de sesión protegido es obligatorio.", NameOf(sessionBindingHash))
    End Sub

    Private Shared Sub EnsureUtc(ByVal value As DateTime, ByVal parameterName As String)
        If value.Kind <> DateTimeKind.Utc Then Throw New ArgumentException("La fecha debe expresarse en UTC.", parameterName)
    End Sub

    Private Function ClockUtcNow() As DateTime
        Dim value As DateTime = _clock.UtcNow
        EnsureUtc(value, "UtcNow")
        Return value
    End Function

    Private Shared Function Utc(ByVal reader As IDataReader, ByVal name As String) As DateTime
        Return DateTime.SpecifyKind(Convert.ToDateTime(reader(name), CultureInfo.InvariantCulture), DateTimeKind.Utc)
    End Function

    Private Shared Function P(ByVal name As String, ByVal value As Object) As IDataParameter
        Return New MySqlParameter(name, If(value, DBNull.Value))
    End Function

    Private Shared Function Parameters(ParamArray values As IDataParameter()) As IList(Of IDataParameter)
        Return New List(Of IDataParameter)(values)
    End Function

    Private NotInheritable Class PersistenceRow
        Public Property ChallengeId As Guid
        Public Property CanonicalIdentity As String
        Public Property ProtectedCode As String
        Public Property ExpiresAtUtc As DateTime
        Public Property Consumed As Boolean
        Public Property Attempts As Integer
        Public Property CreatedAtUtc As DateTime

        Public Function ToStoredChallenge(ByVal observedAtUtc As DateTime) As SecondFactorStoredChallenge
            Dim derivedState As SegundoFactorChallengeState
            If Consumed Then
                derivedState = SegundoFactorChallengeState.COMPLETED
            ElseIf Attempts >= SegundoFactorConfiguration.MaxAttempts Then
                derivedState = SegundoFactorChallengeState.BLOCKED
            ElseIf ExpiresAtUtc <= observedAtUtc Then
                derivedState = SegundoFactorChallengeState.EXPIRED
            Else
                derivedState = SegundoFactorChallengeState.SENT
            End If
            Return New SecondFactorStoredChallenge(ChallengeId, CanonicalIdentity, SegundoFactorPurpose.LOGIN, derivedState, Attempts, CreatedAtUtc, ExpiresAtUtc)
        End Function
    End Class
End Class
