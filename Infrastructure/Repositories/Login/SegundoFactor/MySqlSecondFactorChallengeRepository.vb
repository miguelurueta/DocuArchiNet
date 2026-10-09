Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports MySql.Data.MySqlClient

'Persistencia central de Login 2FA. No conoce presentación, correo ni finalización de login.
Public NotInheritable Class MySqlSecondFactorChallengeRepository
    Implements ISecondFactorChallengeRepository

    Private Const SchemaVersion As Integer = 1
    Private Const ProviderEmail As String = "EMAIL"
    Private Const SelectColumns As String = "ChallengeId,AuthUserId,Purpose,State,Attempts,ResendCount,CreatedAtUtc,ExpiresAtUtc,LastSentAtUtc,TerminalAtUtc,UpdatedAtUtc,SchemaVersion,SessionBindingHash,CodeHash,Consumed,KeyId"

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
        Return InTransaction(Function(connection, transaction) InsertChallenge(connection, transaction, challenge, protectedCode, sessionBindingHash))
    End Function

    Public Function GetForVerification(ByVal challengeId As Guid,
                                       ByVal sessionBindingHash As String) As SegundoFactorChallenge Implements ISecondFactorChallengeRepository.GetForVerification
        Throw New NotSupportedException("Use GetVerificationData; la persistencia no almacena LoginNormalizado.")
    End Function

    Public Function GetVerificationData(ByVal challengeId As Guid,
                                        ByVal sessionBindingHash As String) As SecondFactorChallengeVerificationData Implements ISecondFactorChallengeRepository.GetVerificationData
        ValidateChallengeId(challengeId)
        ValidateSessionBindingHash(sessionBindingHash)
        Dim observedAtUtc As DateTime = ClockUtcNow()
        Using connection As IDbConnection = _connections.CreateOpenConnection(_centralContext)
            Const sql As String = "SELECT " & SelectColumns & " FROM ra_auth_second_factor_challenge WHERE ChallengeId=@challengeId AND SessionBindingHash=@sessionBindingHash AND SchemaVersion=@schemaVersion AND State=@state AND Consumed=0 AND ExpiresAtUtc>@observedAt LIMIT 1"
            Dim row As PersistenceRow = _executor.ExecuteReader(connection, Nothing, sql,
                Parameters(P("@challengeId", challengeId.ToString("D")), P("@sessionBindingHash", sessionBindingHash.Trim()), P("@schemaVersion", SchemaVersion), P("@state", SegundoFactorChallengeState.SENT.ToString()), P("@observedAt", observedAtUtc)),
                AddressOf MapSingleRow)
            If row Is Nothing Then Return Nothing
            Return New SecondFactorChallengeVerificationData(row.ToStoredChallenge(), row.ProtectedCode)
        End Using
    End Function

    Public Function MarkSent(ByVal challengeId As Guid,
                             ByVal sentAtUtc As DateTime) As Boolean Implements ISecondFactorChallengeRepository.MarkSent
        ValidateChallengeId(challengeId)
        EnsureUtc(sentAtUtc, NameOf(sentAtUtc))
        Return Transition(challengeId, SegundoFactorChallengeState.CREATED, SegundoFactorChallengeState.SENT, sentAtUtc, False, True)
    End Function

    Public Function MarkDeliveryFailed(ByVal challengeId As Guid,
                                       ByVal failedAtUtc As DateTime) As Boolean Implements ISecondFactorChallengeRepository.MarkDeliveryFailed
        ValidateChallengeId(challengeId)
        EnsureUtc(failedAtUtc, NameOf(failedAtUtc))
        Return Transition(challengeId, SegundoFactorChallengeState.CREATED, SegundoFactorChallengeState.DELIVERY_FAILED, failedAtUtc, True, False)
    End Function

    Public Function RegisterFailedAttempt(ByVal challengeId As Guid,
                                          ByVal expectedAttempts As Integer) As SegundoFactorChallenge Implements ISecondFactorChallengeRepository.RegisterFailedAttempt
        Throw New NotSupportedException("Use RegisterFailedAttemptData; la persistencia no almacena LoginNormalizado.")
    End Function

    Public Function RegisterFailedAttemptData(ByVal challengeId As Guid,
                                              ByVal expectedAttempts As Integer) As SecondFactorStoredChallenge Implements ISecondFactorChallengeRepository.RegisterFailedAttemptData
        ValidateChallengeId(challengeId)
        If expectedAttempts < 0 OrElse expectedAttempts >= SegundoFactorConfiguration.MaxAttempts Then Throw New ArgumentOutOfRangeException(NameOf(expectedAttempts))
        Return InTransaction(
            Function(connection, transaction)
                Dim row As PersistenceRow = LockRow(connection, transaction, challengeId)
                Dim observedAtUtc As DateTime = _clock.UtcNow
                EnsureUtc(observedAtUtc, "UtcNow")
                If row Is Nothing OrElse row.SchemaVersion <> SchemaVersion OrElse row.State <> SegundoFactorChallengeState.SENT OrElse row.Attempts <> expectedAttempts OrElse row.ExpiresAtUtc <= observedAtUtc Then Return Nothing
                Dim nextAttempts As Integer = expectedAttempts + 1
                Dim nextState As SegundoFactorChallengeState = If(nextAttempts >= SegundoFactorConfiguration.MaxAttempts, SegundoFactorChallengeState.BLOCKED, SegundoFactorChallengeState.SENT)
                Const sql As String = "UPDATE ra_auth_second_factor_challenge SET Attempts=@nextAttempts,State=@nextState,TerminalAtUtc=@terminalAt,UpdatedAtUtc=@updatedAt WHERE ChallengeId=@challengeId AND SchemaVersion=@schemaVersion AND State=@expectedState AND Attempts=@expectedAttempts"
                Dim terminalAt As Object = If(nextState = SegundoFactorChallengeState.BLOCKED, CType(observedAtUtc, Object), DBNull.Value)
                Dim affected As Integer = _executor.ExecuteNonQuery(connection, transaction, sql,
                    Parameters(P("@nextAttempts", nextAttempts), P("@nextState", nextState.ToString()), P("@terminalAt", terminalAt), P("@updatedAt", observedAtUtc), P("@challengeId", challengeId.ToString("D")), P("@schemaVersion", SchemaVersion), P("@expectedState", SegundoFactorChallengeState.SENT.ToString()), P("@expectedAttempts", expectedAttempts)))
                If affected <> 1 Then Return Nothing
                row.Attempts = nextAttempts
                row.State = nextState
                Return row.ToStoredChallenge()
            End Function)
    End Function

    Public Function TryBeginFinalization(ByVal challengeId As Guid,
                                         ByVal expectedAttempts As Integer) As Boolean Implements ISecondFactorChallengeRepository.TryBeginFinalization
        ValidateChallengeId(challengeId)
        If expectedAttempts < 0 OrElse expectedAttempts >= SegundoFactorConfiguration.MaxAttempts Then Throw New ArgumentOutOfRangeException(NameOf(expectedAttempts))
        Return InTransaction(
            Function(connection, transaction)
                Dim row As PersistenceRow = LockRow(connection, transaction, challengeId)
                Dim observedAtUtc As DateTime = _clock.UtcNow
                EnsureUtc(observedAtUtc, "UtcNow")
                If row Is Nothing OrElse row.SchemaVersion <> SchemaVersion OrElse row.State <> SegundoFactorChallengeState.SENT OrElse row.Attempts <> expectedAttempts OrElse row.ExpiresAtUtc <= observedAtUtc Then Return False
                Const sql As String = "UPDATE ra_auth_second_factor_challenge SET State=@nextState,UpdatedAtUtc=@updatedAt WHERE ChallengeId=@challengeId AND SchemaVersion=@schemaVersion AND State=@expectedState AND Attempts=@expectedAttempts AND Consumed=0"
                Return _executor.ExecuteNonQuery(connection, transaction, sql,
                    Parameters(P("@nextState", SegundoFactorChallengeState.FINALIZING.ToString()), P("@updatedAt", observedAtUtc), P("@challengeId", challengeId.ToString("D")), P("@schemaVersion", SchemaVersion), P("@expectedState", SegundoFactorChallengeState.SENT.ToString()), P("@expectedAttempts", expectedAttempts))) = 1
            End Function)
    End Function

    Public Function Complete(ByVal challengeId As Guid) As Boolean Implements ISecondFactorChallengeRepository.Complete
        ValidateChallengeId(challengeId)
        Return Transition(challengeId, SegundoFactorChallengeState.FINALIZING, SegundoFactorChallengeState.COMPLETED, ClockUtcNow(), True, False, True)
    End Function

    Public Function FailFinalization(ByVal challengeId As Guid) As Boolean Implements ISecondFactorChallengeRepository.FailFinalization
        ValidateChallengeId(challengeId)
        Return Transition(challengeId, SegundoFactorChallengeState.FINALIZING, SegundoFactorChallengeState.FINALIZATION_FAILED, ClockUtcNow(), True, False)
    End Function

    Public Function Revoke(ByVal challengeId As Guid) As Boolean Implements ISecondFactorChallengeRepository.Revoke
        ValidateChallengeId(challengeId)
        Return InTransaction(
            Function(connection, transaction)
                Dim row As PersistenceRow = LockRow(connection, transaction, challengeId)
                If row Is Nothing OrElse row.SchemaVersion <> SchemaVersion OrElse (row.State <> SegundoFactorChallengeState.CREATED AndAlso row.State <> SegundoFactorChallengeState.SENT) Then Return False
                Dim observedAtUtc As DateTime = ClockUtcNow()
                Const sql As String = "UPDATE ra_auth_second_factor_challenge SET State=@nextState,TerminalAtUtc=@terminalAt,UpdatedAtUtc=@updatedAt WHERE ChallengeId=@challengeId AND SchemaVersion=@schemaVersion AND State=@expectedState"
                Return _executor.ExecuteNonQuery(connection, transaction, sql,
                    Parameters(P("@nextState", SegundoFactorChallengeState.REVOKED.ToString()), P("@terminalAt", observedAtUtc), P("@updatedAt", observedAtUtc), P("@challengeId", challengeId.ToString("D")), P("@schemaVersion", SchemaVersion), P("@expectedState", row.State.ToString()))) = 1
            End Function)
    End Function

    Public Function ReplaceForResend(ByVal previousChallengeId As Guid,
                                     ByVal replacement As SegundoFactorChallenge,
                                     ByVal protectedCode As String,
                                     ByVal sessionBindingHash As String,
                                     ByVal requestedAtUtc As DateTime) As Boolean Implements ISecondFactorChallengeRepository.ReplaceForResend
        ValidateChallengeId(previousChallengeId)
        EnsureUtc(requestedAtUtc, NameOf(requestedAtUtc))
        ValidateNewChallenge(replacement, protectedCode, sessionBindingHash)
        Return InTransaction(
            Function(connection, transaction)
                Dim previous As PersistenceRow = LockRow(connection, transaction, previousChallengeId)
                If previous Is Nothing OrElse previous.SchemaVersion <> SchemaVersion OrElse previous.State <> SegundoFactorChallengeState.SENT OrElse Not previous.LastSentAtUtc.HasValue OrElse previous.ExpiresAtUtc <= requestedAtUtc Then Return False
                If previous.ResendCount >= SegundoFactorConfiguration.MaxResends OrElse requestedAtUtc < previous.LastSentAtUtc.Value.AddSeconds(SegundoFactorConfiguration.ResendCooldownSeconds) Then Return False
                If replacement.Identity.ClaveCanonica <> previous.CanonicalIdentity OrElse replacement.Purpose <> previous.Purpose OrElse replacement.ResendCount <> previous.ResendCount + 1 Then Return False

                Const revokeSql As String = "UPDATE ra_auth_second_factor_challenge SET State=@nextState,TerminalAtUtc=@terminalAt,UpdatedAtUtc=@updatedAt WHERE ChallengeId=@challengeId AND SchemaVersion=@schemaVersion AND State=@expectedState"
                If _executor.ExecuteNonQuery(connection, transaction, revokeSql,
                    Parameters(P("@nextState", SegundoFactorChallengeState.REVOKED.ToString()), P("@terminalAt", requestedAtUtc), P("@updatedAt", requestedAtUtc), P("@challengeId", previousChallengeId.ToString("D")), P("@schemaVersion", SchemaVersion), P("@expectedState", SegundoFactorChallengeState.SENT.ToString()))) <> 1 Then Return False
                If Not InsertChallenge(connection, transaction, replacement, protectedCode, sessionBindingHash) Then
                    Throw New InvalidOperationException("SECOND_FACTOR_REPLACEMENT_NOT_INSERTED")
                End If
                Return True
            End Function)
    End Function

    Public Function Expire(ByVal challengeId As Guid,
                           ByVal observedAtUtc As DateTime) As Boolean Implements ISecondFactorChallengeRepository.Expire
        ValidateChallengeId(challengeId)
        EnsureUtc(observedAtUtc, NameOf(observedAtUtc))
        Return InTransaction(
            Function(connection, transaction)
                Dim row As PersistenceRow = LockRow(connection, transaction, challengeId)
                If row Is Nothing OrElse row.SchemaVersion <> SchemaVersion OrElse row.ExpiresAtUtc > observedAtUtc OrElse (row.State <> SegundoFactorChallengeState.CREATED AndAlso row.State <> SegundoFactorChallengeState.SENT) Then Return False
                Const sql As String = "UPDATE ra_auth_second_factor_challenge SET State=@nextState,TerminalAtUtc=@terminalAt,UpdatedAtUtc=@updatedAt WHERE ChallengeId=@challengeId AND SchemaVersion=@schemaVersion AND State=@expectedState AND ExpiresAtUtc<=@observedAt"
                Return _executor.ExecuteNonQuery(connection, transaction, sql,
                    Parameters(P("@nextState", SegundoFactorChallengeState.EXPIRED.ToString()), P("@terminalAt", observedAtUtc), P("@updatedAt", observedAtUtc), P("@challengeId", challengeId.ToString("D")), P("@schemaVersion", SchemaVersion), P("@expectedState", row.State.ToString()), P("@observedAt", observedAtUtc))) = 1
            End Function)
    End Function

    Private Function Transition(ByVal challengeId As Guid,
                                ByVal expectedState As SegundoFactorChallengeState,
                                ByVal nextState As SegundoFactorChallengeState,
                                ByVal occurredAtUtc As DateTime,
                                ByVal terminal As Boolean,
                                ByVal marksSent As Boolean,
                                Optional ByVal consumes As Boolean = False) As Boolean
        Return InTransaction(
            Function(connection, transaction)
                Dim row As PersistenceRow = LockRow(connection, transaction, challengeId)
                If row Is Nothing OrElse row.SchemaVersion <> SchemaVersion OrElse row.State <> expectedState Then Return False
                If marksSent AndAlso (occurredAtUtc < row.CreatedAtUtc OrElse occurredAtUtc >= row.ExpiresAtUtc) Then Return False
                Const sql As String = "UPDATE ra_auth_second_factor_challenge SET State=@nextState,Consumed=@consumed,LastSentAtUtc=CASE WHEN @marksSent=1 THEN @occurredAt ELSE LastSentAtUtc END,TerminalAtUtc=@terminalAt,UpdatedAtUtc=@occurredAt WHERE ChallengeId=@challengeId AND SchemaVersion=@schemaVersion AND State=@expectedState"
                Return _executor.ExecuteNonQuery(connection, transaction, sql,
                    Parameters(P("@nextState", nextState.ToString()), P("@consumed", If(consumes, 1, 0)), P("@marksSent", If(marksSent, 1, 0)), P("@occurredAt", occurredAtUtc), P("@terminalAt", If(terminal, CType(occurredAtUtc, Object), DBNull.Value)), P("@challengeId", challengeId.ToString("D")), P("@schemaVersion", SchemaVersion), P("@expectedState", expectedState.ToString()))) = 1
            End Function)
    End Function

    Private Function InsertChallenge(ByVal connection As IDbConnection,
                                     ByVal transaction As IDbTransaction,
                                     ByVal challenge As SegundoFactorChallenge,
                                     ByVal protectedCode As String,
                                     ByVal sessionBindingHash As String) As Boolean
        Const sql As String = "INSERT INTO ra_auth_second_factor_challenge (ChallengeId,AuthUserId,Provider,Purpose,SessionBindingHash,State,KeyId,CodeHash,ExpiresAtUtc,Consumed,Attempts,ResendCount,CreatedAtUtc,LastSentAtUtc,TerminalAtUtc,UpdatedAtUtc,SchemaVersion,AuthPayloadJson) VALUES (@challengeId,@authUserId,@provider,@purpose,@sessionBindingHash,@state,@keyId,@codeHash,@expiresAt,0,@attempts,@resendCount,@createdAt,NULL,NULL,@updatedAt,@schemaVersion,NULL)"
        Return _executor.ExecuteNonQuery(connection, transaction, sql,
            Parameters(P("@challengeId", challenge.ChallengeId.ToString("D")), P("@authUserId", challenge.Identity.ClaveCanonica), P("@provider", ProviderEmail), P("@purpose", challenge.Purpose.ToString()), P("@sessionBindingHash", sessionBindingHash.Trim()), P("@state", challenge.State.ToString()), P("@keyId", ExtractKeyId(protectedCode)), P("@codeHash", protectedCode.Trim()), P("@expiresAt", challenge.ExpiresAtUtc), P("@attempts", challenge.Attempts), P("@resendCount", challenge.ResendCount), P("@createdAt", challenge.CreatedAtUtc), P("@updatedAt", challenge.CreatedAtUtc), P("@schemaVersion", SchemaVersion))) = 1
    End Function

    Private Function LockRow(ByVal connection As IDbConnection,
                             ByVal transaction As IDbTransaction,
                             ByVal challengeId As Guid) As PersistenceRow
        Const sql As String = "SELECT " & SelectColumns & " FROM ra_auth_second_factor_challenge WHERE ChallengeId=@challengeId FOR UPDATE"
        Return _executor.ExecuteReader(connection, transaction, sql, Parameters(P("@challengeId", challengeId.ToString("D"))), AddressOf MapSingleRow)
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
        Dim purpose As SegundoFactorPurpose
        Dim state As SegundoFactorChallengeState
        If Not [Enum].TryParse(Convert.ToString(reader("Purpose"), CultureInfo.InvariantCulture), True, purpose) Then Return Nothing
        If Not [Enum].TryParse(Convert.ToString(reader("State"), CultureInfo.InvariantCulture), True, state) Then Return Nothing
        Return New PersistenceRow With {
            .ChallengeId = Guid.Parse(Convert.ToString(reader("ChallengeId"), CultureInfo.InvariantCulture)),
            .CanonicalIdentity = Convert.ToString(reader("AuthUserId"), CultureInfo.InvariantCulture),
            .Purpose = purpose,
            .State = state,
            .Attempts = Convert.ToInt32(reader("Attempts"), CultureInfo.InvariantCulture),
            .ResendCount = Convert.ToInt32(reader("ResendCount"), CultureInfo.InvariantCulture),
            .CreatedAtUtc = Utc(reader, "CreatedAtUtc"),
            .ExpiresAtUtc = Utc(reader, "ExpiresAtUtc"),
            .LastSentAtUtc = NullableUtc(reader, "LastSentAtUtc"),
            .SchemaVersion = Convert.ToInt32(reader("SchemaVersion"), CultureInfo.InvariantCulture),
            .ProtectedCode = Convert.ToString(reader("CodeHash"), CultureInfo.InvariantCulture)}
    End Function

    Private Shared Sub ValidateNewChallenge(ByVal challenge As SegundoFactorChallenge,
                                            ByVal protectedCode As String,
                                            ByVal sessionBindingHash As String)
        If challenge Is Nothing Then Throw New ArgumentNullException(NameOf(challenge))
        If challenge.State <> SegundoFactorChallengeState.CREATED Then Throw New ArgumentException("Un challenge nuevo debe iniciar en CREATED.", NameOf(challenge))
        ValidateSessionBindingHash(sessionBindingHash)
        ExtractKeyId(protectedCode)
    End Sub

    Private Shared Function ExtractKeyId(ByVal protectedCode As String) As String
        If String.IsNullOrWhiteSpace(protectedCode) Then Throw New ArgumentException("El código protegido es obligatorio.", NameOf(protectedCode))
        Dim parts As String() = protectedCode.Trim().Split(":"c)
        If parts.Length <> 3 OrElse parts(0) <> "v1" OrElse String.IsNullOrWhiteSpace(parts(1)) OrElse parts(1).Contains(":") Then Throw New ArgumentException("El código protegido debe usar v1:keyId:mac.", NameOf(protectedCode))
        Try
            If Convert.FromBase64String(parts(2)).Length <> 32 Then Throw New FormatException()
        Catch ex As FormatException
            Throw New ArgumentException("El MAC protegido es inválido.", NameOf(protectedCode), ex)
        End Try
        Return parts(1)
    End Function

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

    Private Shared Function NullableUtc(ByVal reader As IDataReader, ByVal name As String) As Nullable(Of DateTime)
        Dim ordinal As Integer = reader.GetOrdinal(name)
        If reader.IsDBNull(ordinal) Then Return Nothing
        Return New Nullable(Of DateTime)(Utc(reader, name))
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
        Public Property Purpose As SegundoFactorPurpose
        Public Property State As SegundoFactorChallengeState
        Public Property Attempts As Integer
        Public Property ResendCount As Integer
        Public Property CreatedAtUtc As DateTime
        Public Property ExpiresAtUtc As DateTime
        Public Property LastSentAtUtc As Nullable(Of DateTime)
        Public Property SchemaVersion As Integer
        Public Property ProtectedCode As String

        Public Function ToStoredChallenge() As SecondFactorStoredChallenge
            Return New SecondFactorStoredChallenge(ChallengeId, CanonicalIdentity, Purpose, State, Attempts, ResendCount, CreatedAtUtc, ExpiresAtUtc, LastSentAtUtc)
        End Function
    End Class
End Class
