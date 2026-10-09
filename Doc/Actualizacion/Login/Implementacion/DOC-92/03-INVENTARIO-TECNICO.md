# Inventario técnico verificado

Las rutas son relativas a la raíz indicada antes del guion. El alcance implementado pertenece a `DocuArchiNet`; `DocuArchiCore` no aporta clases ejecutables a esta entrega.

## APIs, endpoints, servicios y repositorios

| Tipo | Ruta del archivo | Clase o interfaz | Nombre exacto | Parámetros y tipos | Retorno | Descripción | Relaciones o dependencias |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Endpoint | `DocuArchiNet — No aplica` | No aplica | No aplica | Verbo HTTP: No aplica; ruta completa: No aplica; autorización: No aplica; DTO entrada/salida: No aplica | No aplica | DOC-92 no publica HTTP ni ASMX. | No aplica |
| Controller | `DocuArchiNet — No aplica` | No aplica | No aplica | No aplica | No aplica | No existe controller para persistencia 2FA. | No aplica |
| Service | `DocuArchiNet — No aplica` | No aplica | No aplica | No aplica | No aplica | No existe orquestador de aplicación en DOC-92. | Consumidor futuro. |
| Puerto de repositorio | `DocuArchiNet — Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb` | `ISecondFactorChallengeRepository` | `Create` | `challenge As SegundoFactorChallenge`, `protectedCode As String`, `sessionBindingHash As String` | `Boolean` | Crear challenge. | Implementado por `MySqlSecondFactorChallengeRepository`. |
| Puerto de repositorio | misma ruta | `ISecondFactorChallengeRepository` | `GetForVerification` | `challengeId As Guid`, `sessionBindingHash As String` | `SegundoFactorChallenge` | Firma heredada no rehidratable. | La implementación lanza `NotSupportedException`. |
| Puerto de repositorio | misma ruta | `ISecondFactorChallengeRepository` | `GetVerificationData` | `challengeId As Guid`, `sessionBindingHash As String` | `SecondFactorChallengeVerificationData` | Leer proyección verificable. | Implementación MySQL. |
| Puerto de repositorio | misma ruta | `ISecondFactorChallengeRepository` | `MarkSent` | `challengeId As Guid`, `sentAtUtc As DateTime` | `Boolean` | Comprobar que continúa activo. | Implementación MySQL. |
| Puerto de repositorio | misma ruta | `ISecondFactorChallengeRepository` | `MarkDeliveryFailed` | `challengeId As Guid`, `failedAtUtc As DateTime` | `Boolean` | Consumir tras fallo de entrega. | Implementación MySQL. |
| Puerto de repositorio | misma ruta | `ISecondFactorChallengeRepository` | `RegisterFailedAttempt` | `challengeId As Guid`, `expectedAttempts As Integer` | `SegundoFactorChallenge` | Firma heredada no rehidratable. | La implementación lanza `NotSupportedException`. |
| Puerto de repositorio | misma ruta | `ISecondFactorChallengeRepository` | `RegisterFailedAttemptData` | `challengeId As Guid`, `expectedAttempts As Integer` | `SecondFactorStoredChallenge` | Incremento condicionado. | Implementación MySQL. |
| Puerto de repositorio | misma ruta | `ISecondFactorChallengeRepository` | `TryBeginFinalization` | `challengeId As Guid`, `expectedAttempts As Integer` | `Boolean` | Adquisición exclusiva por consumo. | Implementación MySQL. |
| Puerto de repositorio | misma ruta | `ISecondFactorChallengeRepository` | `Complete` | `challengeId As Guid` | `Boolean` | Consultar fila consumida. | Misma señal física que `FailFinalization`. |
| Puerto de repositorio | misma ruta | `ISecondFactorChallengeRepository` | `FailFinalization` | `challengeId As Guid` | `Boolean` | Consultar fila consumida. | Misma señal física que `Complete`. |
| Puerto de repositorio | misma ruta | `ISecondFactorChallengeRepository` | `Revoke` | `challengeId As Guid` | `Boolean` | Consumir fila activa. | Reloj inyectado. |
| Puerto de repositorio | misma ruta | `ISecondFactorChallengeRepository` | `ReplaceForResend` | `previousChallengeId As Guid`, `replacement As SegundoFactorChallenge`, `protectedCode As String`, `sessionBindingHash As String`, `requestedAtUtc As DateTime` | `Boolean` | Revocar e insertar atómicamente. | Transacción, cooldown y misma identidad. |
| Puerto de repositorio | misma ruta | `ISecondFactorChallengeRepository` | `Expire` | `challengeId As Guid`, `observedAtUtc As DateTime` | `Boolean` | Consumir si venció. | Implementación MySQL. |

## Implementación del repositorio

| Tipo | Ruta del archivo | Clase o interfaz | Nombre exacto | Parámetros y tipos | Retorno | Descripción | Relaciones o dependencias |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Repositorio | `DocuArchiNet — Infrastructure/Repositories/Login/SegundoFactor/MySqlSecondFactorChallengeRepository.vb` | `MySqlSecondFactorChallengeRepository` | Constructor | `connections As IModuleConnectionFactory`, `executor As IDataExecutor`, `transactions As ITransactionFactory`, `centralContext As ContextoModulo`, `clock As ISecondFactorClock` | No aplica | Valida e inmoviliza dependencias/contexto. | Implementa `ISecondFactorChallengeRepository`. |
| Método público | misma ruta | misma clase | `Create` | `SegundoFactorChallenge`, `String`, `String` | `Boolean` | Valida e inserta en transacción. | `InsertChallenge`, `InTransaction`. |
| Método público | misma ruta | misma clase | `GetForVerification` | `Guid`, `String` | `SegundoFactorChallenge` | No soportado. | Siempre `NotSupportedException`. |
| Método público | misma ruta | misma clase | `GetVerificationData` | `Guid`, `String` | `SecondFactorChallengeVerificationData` | Consulta fila elegible y proyecta. | Factory, executor, reloj. |
| Método público | misma ruta | misma clase | `MarkSent` | `Guid`, `DateTime` | `Boolean` | Comprueba actividad. | `ExistsActive`. |
| Método público | misma ruta | misma clase | `MarkDeliveryFailed` | `Guid`, `DateTime` | `Boolean` | Consume si activo. | `ConsumeIfActive`. |
| Método público | misma ruta | misma clase | `RegisterFailedAttempt` | `Guid`, `Integer` | `SegundoFactorChallenge` | No soportado. | Siempre `NotSupportedException`. |
| Método público | misma ruta | misma clase | `RegisterFailedAttemptData` | `Guid`, `Integer` | `SecondFactorStoredChallenge` | Bloquea, compara e incrementa. | `LockRow`, `IsActive`, `InTransaction`. |
| Método público | misma ruta | misma clase | `TryBeginFinalization` | `Guid`, `Integer` | `Boolean` | Cambia `Consumed=1` de forma condicionada. | `LockRow`, `InTransaction`. |
| Método público | misma ruta | misma clase | `Complete` | `Guid` | `Boolean` | Consulta consumo. | `IsConsumed`. |
| Método público | misma ruta | misma clase | `FailFinalization` | `Guid` | `Boolean` | Consulta consumo. | `IsConsumed`. |
| Método público | misma ruta | misma clase | `Revoke` | `Guid` | `Boolean` | Consume si activo usando UTC actual. | `ConsumeIfActive`, reloj. |
| Método público | misma ruta | misma clase | `ReplaceForResend` | `Guid`, `SegundoFactorChallenge`, `String`, `String`, `DateTime` | `Boolean` | Reemplazo transaccional. | `LockRow`, `InsertChallenge`, rollback. |
| Método público | misma ruta | misma clase | `Expire` | `Guid`, `DateTime` | `Boolean` | Consume fila vencida. | Executor. |
| Función privada | misma ruta | misma clase | `InsertChallenge` | `connection As IDbConnection`, `transaction As IDbTransaction`, `challenge As SegundoFactorChallenge`, `protectedCode As String` | `Boolean` | INSERT de nueve columnas, `AuthPayloadJson=NULL`. | `IDataExecutor`. |
| Función privada | misma ruta | misma clase | `LockRow` | `IDbConnection`, `IDbTransaction`, `Guid` | `PersistenceRow` | `SELECT ... FOR UPDATE`. | Provider `EMAIL`. |
| Función privada | misma ruta | misma clase | `ExistsActive` | `Guid`, `DateTime` | `Boolean` | Cuenta fila elegible. | `ExecuteScalar`. |
| Función privada | misma ruta | misma clase | `IsConsumed` | `Guid` | `Boolean` | Cuenta fila `EMAIL` consumida. | `ExecuteScalar`. |
| Función privada | misma ruta | misma clase | `ConsumeIfActive` | `Guid`, `DateTime` | `Boolean` | UPDATE condicionado. | `ExecuteNonQuery`. |
| Función privada genérica | misma ruta | misma clase | `InTransaction(Of T)` | `operation As Func(Of IDbConnection, IDbTransaction, T)` | `T` | Abre, confirma o revierte transacción. | Connection/transaction factories. |
| Función privada `Shared` | misma ruta | misma clase | `MapSingleRow` | `reader As IDataReader` | `PersistenceRow` | Mapea una fila o `Nothing`. | Nueve columnas seleccionadas. |
| Función privada `Shared` | misma ruta | misma clase | `IsActive` | `row As PersistenceRow`, `observedAtUtc As DateTime` | `Boolean` | Evalúa consumo, intentos y vencimiento. | Máximo cinco. |
| Método privado `Shared` | misma ruta | misma clase | `ValidateNewChallenge` | `challenge As SegundoFactorChallenge`, `protectedCode As String`, `sessionBindingHash As String` | `Void` | Exige estado `CREATED`, vínculo y HMAC. | Validadores privados. |
| Método privado `Shared` | misma ruta | misma clase | `ValidateProtectedCode` | `protectedCode As String` | `Void` | Exige `v1:keyId:base64` y MAC de 32 bytes. | `Convert.FromBase64String`. |
| Método privado `Shared` | misma ruta | misma clase | `ValidateChallengeId` | `challengeId As Guid` | `Void` | Rechaza `Guid.Empty`. | No aplica |
| Método privado `Shared` | misma ruta | misma clase | `ValidateSessionBindingHash` | `sessionBindingHash As String` | `Void` | Rechaza vacío. | No persiste el valor. |
| Método privado `Shared` | misma ruta | misma clase | `EnsureUtc` | `value As DateTime`, `parameterName As String` | `Void` | Exige UTC. | No aplica |
| Función privada | misma ruta | misma clase | `ClockUtcNow` | Ninguno | `DateTime` | Lee reloj y exige UTC. | `ISecondFactorClock`. |
| Función privada `Shared` | misma ruta | misma clase | `Utc` | `reader As IDataReader`, `name As String` | `DateTime` | Convierte y marca UTC. | `CultureInfo.InvariantCulture`. |
| Función privada `Shared` | misma ruta | misma clase | `P` | `name As String`, `value As Object` | `IDataParameter` | Crea `MySqlParameter`. | MySql.Data. |
| Función privada `Shared` | misma ruta | misma clase | `Parameters` | `ParamArray values As IDataParameter()` | `IList(Of IDataParameter)` | Materializa parámetros. | `List(Of IDataParameter)`. |
| Función anidada | misma ruta | `PersistenceRow` | `ToStoredChallenge` | `observedAtUtc As DateTime` | `SecondFactorStoredChallenge` | Deriva estado. | `Consumed`, `Attempts`, `ExpiresAtUtc`. |

## Infraestructura y modelos participantes

| Tipo | Ruta del archivo | Clase o interfaz | Nombre exacto | Parámetros y tipos | Retorno | Descripción | Relaciones o dependencias |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Interfaz técnica | `DocuArchiNet — Infrastructure/Shared/Data/ModuleDataContracts.vb` | `IModuleConnectionFactory` | `CreateOpenConnection` | `contexto As ContextoModulo` | `IDbConnection` | Abre conexión del módulo. | Implementación compartida fuera del repositorio DOC-92. |
| Interfaz técnica | misma ruta | `IDataExecutor` | `ExecuteNonQuery` | `IDbConnection`, `IDbTransaction`, `String`, `IEnumerable(Of IDataParameter)` | `Integer` | Ejecuta escritura parametrizada. | ADO.NET. |
| Interfaz técnica | misma ruta | `IDataExecutor` | `ExecuteScalar` | `connection As IDbConnection`, `transaction As IDbTransaction`, `commandText As String`, `parameters As IEnumerable(Of IDataParameter)` | `Object` | Ejecuta escalar. | ADO.NET. |
| Interfaz técnica | misma ruta | `IDataExecutor` | `ExecuteReader(Of T)` | `connection As IDbConnection`, `transaction As IDbTransaction`, `commandText As String`, `parameters As IEnumerable(Of IDataParameter)`, `projector As Func(Of IDataReader,T)` | `T` | Ejecuta lectura y proyección. | ADO.NET. |
| Interfaz técnica | misma ruta | `ITransactionFactory` | `BeginTransaction` | `connection As IDbConnection` | `IDbTransaction` | Inicia transacción. | ADO.NET. |
| Interfaz de dominio | `DocuArchiNet — Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb` | `ISecondFactorClock` | Getter `UtcNow` | No aplica | `DateTime` | Fuente UTC. | Inyectada al repositorio. |
| Implementación técnica | `DocuArchiNet — Infrastructure/Shared/Data/ModuleConnectionFactory.vb` | `ModuleConnectionFactory` | `CreateOpenConnection` | `contexto As ContextoModulo` | `IDbConnection` | Valida contexto/configuración y abre `MySqlConnection`. | Implementa `IModuleConnectionFactory`. |
| Implementación técnica | `DocuArchiNet — Infrastructure/Shared/Data/AdoNetDataInfrastructure.vb` | `AdoNetDataExecutor` | `ExecuteNonQuery` | `IDbConnection`, `IDbTransaction`, `String`, `IEnumerable(Of IDataParameter)` | `Integer` | Ejecuta comando parametrizado. | Implementa `IDataExecutor`. |
| Implementación técnica | misma ruta | `AdoNetDataExecutor` | `ExecuteScalar` | `IDbConnection`, `IDbTransaction`, `String`, `IEnumerable(Of IDataParameter)` | `Object` | Ejecuta escalar parametrizado. | Implementa `IDataExecutor`. |
| Implementación técnica | misma ruta | `AdoNetDataExecutor` | `ExecuteReader(Of T)` | `IDbConnection`, `IDbTransaction`, `String`, `IEnumerable(Of IDataParameter)`, `Func(Of IDataReader,T)` | `T` | Ejecuta lector y projector. | Implementa `IDataExecutor`. |
| Implementación técnica | misma ruta | `AdoNetDataExecutor` | `CrearComando` | `IDbConnection`, `IDbTransaction`, `String`, `IEnumerable(Of IDataParameter)` | `IDbCommand` | Valida conexión, asigna transacción y agrega parámetros. | Método privado compartido. |
| Implementación técnica | misma ruta | `DbTransactionFactory` | `BeginTransaction` | `connection As IDbConnection` | `IDbTransaction` | Valida conexión abierta e inicia transacción. | Implementa `ITransactionFactory`. |
| Implementación técnica | `DocuArchiNet — Infrastructure/Login/SegundoFactor/Security/SystemSecondFactorClock.vb` | `SystemSecondFactorClock` | Getter `UtcNow` | No aplica | `DateTime` | Retorna `DateTime.UtcNow`. | Implementa `ISecondFactorClock`. |
| Modelo | `DocuArchiNet — Modelo/Login/SegundoFactor/SegundoFactorModels.vb` | `SegundoFactorChallenge` | Constructor | `Guid`, `SegundoFactorIdentity`, `SegundoFactorPurpose`, `SegundoFactorChallengeState`, `Integer`, `Integer`, `DateTime`, `DateTime` | No aplica | Entrada de creación/reemplazo validada. | DOC-91. |
| Proyección | misma ruta | `SecondFactorStoredChallenge` | Constructor | `Guid`, `String`, `SegundoFactorPurpose`, `SegundoFactorChallengeState`, `Integer`, `DateTime`, `DateTime` | No aplica | Estado reconstruible desde la tabla. | Retorno de intento fallido. |
| Proyección | misma ruta | `SecondFactorChallengeVerificationData` | Constructor | `challenge As SecondFactorStoredChallenge`, `protectedCode As String` | No aplica | Agrupa proyección y hash protegido. | Retorno de consulta. |
| Contexto técnico | `DocuArchiNet — Domain/Shared/ContextoModulo.vb` | `ContextoModulo` | `EsValido` | Ninguno | `Boolean` | Exige código, usuario positivo y login. | Connection factory. |

## Propiedades, obligatoriedad y validaciones

| Clase/proyección | Propiedad | Tipo | Obligatoria | Validación implementada |
| --- | --- | --- | --- | --- |
| `SegundoFactorChallenge` | `ChallengeId` | `Guid` | Sí | Distinto de vacío. |
| misma | `Identity` | `SegundoFactorIdentity` | Sí | No `Nothing`; la identidad valida sus campos. |
| misma | `Purpose` | `SegundoFactorPurpose` | Sí | Solo `LOGIN`. |
| misma | `State` | `SegundoFactorChallengeState` | Sí | Miembro definido; `Create` exige `CREATED`. |
| misma | `Attempts` | `Integer` | Sí | 0 a 5. |
| misma | `ResendCount` | `Integer` | Sí | 0 a 2; reemplazo exige 1 a 2. |
| misma | `CreatedAtUtc`, `ExpiresAtUtc` | `DateTime` | Sí | UTC y expiración posterior. |
| `SecondFactorStoredChallenge` | `ChallengeId` | `Guid` | Sí | Distinto de vacío. |
| misma | `CanonicalIdentity` | `String` | Sí | No vacío; se recorta. |
| misma | `Purpose`, `State`, `Attempts` | enums / `Integer` | Sí | LOGIN, enum definido e intentos 0 a 5. |
| misma | `CreatedAtUtc`, `ExpiresAtUtc` | `DateTime` | Sí | UTC y orden temporal. |
| `SecondFactorChallengeVerificationData` | `Challenge` | `SecondFactorStoredChallenge` | Sí | No `Nothing`. |
| misma | `ProtectedCode` | `String` | Sí | No vacío; se recorta. |
| `ContextoModulo` | `CodigoModulo` | `String` | Sí para el repositorio | `EsValido` exige no vacío. |
| misma | `IdUsuario` | `Integer` | Sí | `EsValido` exige positivo. |
| misma | `IdGrupo` | `Integer` | No | Sin restricción en `EsValido`. |
| misma | `LoginUsuario` | `String` | Sí | `EsValido` exige no vacío. |

## DTOs

| DTO | Propiedades | Entrada/salida HTTP | Estado |
| --- | --- | --- | --- |
| No aplica | No aplica | No aplica | DOC-92 no define ni consume DTOs; las clases `SecondFactorStoredChallenge` y `SecondFactorChallengeVerificationData` son proyecciones internas, no contratos HTTP. |
