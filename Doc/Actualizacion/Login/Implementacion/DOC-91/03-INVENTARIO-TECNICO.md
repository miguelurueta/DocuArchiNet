# Inventario técnico verificado

Todas las rutas son relativas a la raíz del repositorio `DocuArchiNet`.

## APIs, endpoints, servicios y repositorios

| Tipo | Ruta del archivo | Clase o interfaz | Nombre exacto | Parámetros y tipos | Retorno | Descripción | Relaciones o dependencias |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Endpoint | No aplica | No aplica | No aplica | Verbo HTTP: No aplica; ruta: No aplica; autorización: No aplica; DTO entrada/salida: No aplica | No aplica | DOC-91 no publica HTTP/ASMX. | No aplica |
| Service | No aplica | No aplica | No aplica | No aplica | No aplica | No hay orquestador implementado. | Los puertos serán consumidos por una entrega posterior. |
| Puerto de repositorio | `Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb` | `ISecondFactorConfigurationRepository` | `GetConfiguration` | `moduleId As Integer` | `SegundoFactorConfiguration` | Contrato para obtener configuración. | Sin implementación en DOC-91. |
| Puerto de repositorio | `Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb` | `ISecondFactorChallengeRepository` | `Create` | `challenge As SegundoFactorChallenge`, `protectedCode As String`, `sessionBindingHash As String` | `Boolean` | Contrato de creación. | Sin implementación MySQL. |
| Puerto de repositorio | misma ruta | `ISecondFactorChallengeRepository` | `GetForVerification` | `challengeId As Guid`, `sessionBindingHash As String` | `SegundoFactorChallenge` | Contrato de lectura verificable. | Sin implementación MySQL. |
| Puerto de repositorio | misma ruta | `ISecondFactorChallengeRepository` | `RegisterFailedAttempt` | `challengeId As Guid`, `expectedAttempts As Integer` | `SegundoFactorChallenge` | Contrato de incremento condicionado. | Sin implementación MySQL. |
| Puerto de repositorio | misma ruta | `ISecondFactorChallengeRepository` | `TryBeginFinalization` | `challengeId As Guid`, `expectedAttempts As Integer` | `Boolean` | Contrato de adquisición de finalización. | Sin implementación MySQL. |
| Puerto de repositorio | misma ruta | `ISecondFactorChallengeRepository` | `Complete` / `FailFinalization` / `Revoke` | `challengeId As Guid` | `Boolean` | Contratos terminales. | Sin implementación MySQL. |

## Clases, interfaces y enums

| Tipo | Ruta del archivo | Clase o interfaz | Nombre exacto | Parámetros y tipos | Retorno | Descripción | Relaciones o dependencias |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Enum | `Modelo/Login/SegundoFactor/SegundoFactorModels.vb` | No aplica | `SegundoFactorPurpose` | Miembro `LOGIN=1` | No aplica | Propósito soportado. | No aplica |
| Enum | misma ruta | No aplica | `SegundoFactorChallengeState` | `CREATED`, `SENT`, `FINALIZING`, `COMPLETED`, `DELIVERY_FAILED`, `BLOCKED`, `EXPIRED`, `REVOKED`, `FINALIZATION_FAILED` | No aplica | Catálogo de estados; no implementa transiciones. | No aplica |
| Clase de dominio | misma ruta | `SegundoFactorIdentity` | Constructor | `empresaId As Integer`, `moduloId As Integer`, `tipoUsuario As String`, `idInterno As String`, `login As String` | No aplica | Valida positivos/no vacíos y normaliza tipo/login a mayúscula invariant. | `System.Globalization` |
| Clase de dominio | misma ruta | `SegundoFactorConfiguration` | Constructor | `requiereSegundoFactor As Integer?`, `providerType As Integer?`, `expirationMinutes As Integer?` | No aplica | Aplica activación, EMAIL y rango de vigencia. | Constantes 5/60/2 |
| Clase de dominio | misma ruta | `SegundoFactorChallenge` | Constructor | `challengeId As Guid`, `identity As SegundoFactorIdentity`, `purpose As SegundoFactorPurpose`, `state As SegundoFactorChallengeState`, `attempts As Integer`, `resendCount As Integer`, `createdAtUtc As DateTime`, `expiresAtUtc As DateTime` | No aplica | Valida identidad, catálogo, límites y fechas UTC. | `ISecondFactorClock` |
| Clase de dominio | misma ruta | `SecondFactorProtectionContext` | Constructor | `purpose As SegundoFactorPurpose`, `challengeId As Guid`, `canonicalIdentity As String`, `sessionBinding As String` | No aplica | Contexto autenticado por HMAC. | No aplica |
| Clase de dominio | misma ruta | `SecondFactorKeyMaterial` | Constructor | `keyId As String`, `keyBytes As Byte()` | No aplica | Exige ID sin `:` y mínimo 32 bytes; clona el arreglo. | No aplica |
| Clase de dominio | misma ruta | `SecondFactorRecipient` | Constructor | `emailAddress As String`, `maskedDestination As String` | No aplica | Destinatario interno y representación enmascarada. | No envía correo. |
| Clase de dominio | misma ruta | `SecondFactorEmailMessage` | Constructor | `recipient As SecondFactorRecipient`, `code As String`, `expiresAtUtc As DateTime` | No aplica | Mensaje interno en memoria. | `SecondFactorContractValidation` |
| Resultado | misma ruta | `SecondFactorDeliveryResult` | No aplica | No aplica | No aplica | Resultado mutable `Success/PublicCode`. | Puerto de correo futuro. |
| Resultado | misma ruta | `LegacyLoginFinalizationResult` | No aplica | No aplica | No aplica | Resultado mutable `Success/LocalRoute/PublicCode`. | Finalizador futuro. |
| Clase de dominio | misma ruta | `PendingSecondFactorContext` | Constructor | `empresaId As Integer`, `moduloId As Integer`, `internalUserId As Long`, `normalizedLogin As String`, `maskedDestination As String`, `challengeId As Guid`, `sessionNonce As String`, `createdAtUtc As DateTime`, `expiresAtUtc As DateTime` | No aplica | Contexto inmutable permitido en Session. | `SessionPendingSecondFactorContextStore` |
| Utilidad | misma ruta | `SecondFactorContractValidation` | No aplica | No aplica | No aplica | Validación OTP y enmascarado. | Métodos `Shared`. |
| Interfaz | `Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb` | `ISecondFactorClock` | No aplica | No aplica | No aplica | Puerto UTC. | Implementada por `SystemSecondFactorClock`. |
| Interfaz | misma ruta | `ISecondFactorOtpGenerator` | No aplica | No aplica | No aplica | Puerto de generación. | Implementada por `CryptographicSecondFactorOtpGenerator`. |
| Interfaz | misma ruta | `ISecondFactorCodeProtector` | No aplica | No aplica | No aplica | Puerto HMAC. | Implementada por `HmacSecondFactorCodeProtector`. |
| Interfaz | misma ruta | `ISecondFactorKeyProvider` | No aplica | No aplica | No aplica | Puerto del anillo de llaves. | Implementada por `AppSettingsSecondFactorKeyProvider`. |
| Interfaces sin implementación | misma ruta | `ISecondFactorConfigurationRepository`, `ISecondFactorChallengeRepository`, `ISecondFactorRecipientResolver`, `ISecondFactorEmailSender`, `ILegacyLoginFinalizer` | No aplica | No aplica | No aplica | Puertos reservados para entregas posteriores. | Sin SQL, SMTP o Forms Authentication. |
| Interfaz | misma ruta | `IPendingSecondFactorContextStore` | No aplica | No aplica | No aplica | Puerto del contexto pendiente. | Implementada por el adaptador Session. |
| DTO | `DTOs/Login/SegundoFactor/SegundoFactorDtos.vb` | `SegundoFactorEstadoDto` | No aplica | No aplica | No aplica | Estado público serializable. | Usado por `SegundoFactorResultadoDto`. |
| DTO | misma ruta | `SegundoFactorResultadoDto` | No aplica | No aplica | No aplica | Resultado público serializable. | No está publicado por endpoint. |
| Interfaz técnica | `Infrastructure/Login/SegundoFactor/Security/AppSettingsSecondFactorKeyProvider.vb` | `ISecondFactorSettings` | No aplica | No aplica | No aplica | Abstrae lectura de configuración. | Implementación `ConfigurationManagerSecondFactorSettings`. |
| Clase técnica | misma ruta | `ConfigurationManagerSecondFactorSettings` | No aplica | No aplica | No aplica | Lee `ConfigurationManager.AppSettings`. | `System.Configuration` |
| Clase técnica | misma ruta | `AppSettingsSecondFactorKeyProvider` | Constructores | `()`; `(settings As ISecondFactorSettings)` | No aplica | Resuelve llave activa o identificada. | `ISecondFactorSettings`, `SecondFactorKeyMaterial` |
| Clase técnica | `Infrastructure/Login/SegundoFactor/Security/SystemSecondFactorClock.vb` | `SystemSecondFactorClock` | No aplica | No aplica | No aplica | Provee `DateTime.UtcNow`. | `ISecondFactorClock` |
| Clase técnica | `Infrastructure/Login/SegundoFactor/Security/CryptographicSecondFactorOtpGenerator.vb` | `CryptographicSecondFactorOtpGenerator` | Constructores | `()`; `(random As RandomNumberGenerator)` | No aplica | Genera OTP y permite RNG controlado en pruebas. | `ISecondFactorOtpGenerator`, `IDisposable` |
| Clase técnica | `Infrastructure/Login/SegundoFactor/Security/HmacSecondFactorCodeProtector.vb` | `HmacSecondFactorCodeProtector` | Constructor | `keys As ISecondFactorKeyProvider` | No aplica | Protege/verifica con HMAC-SHA256. | `ISecondFactorCodeProtector` |
| Adaptador | `webservice/Login/SegundoFactor/SessionPendingSecondFactorContextStore.vb` | `SessionPendingSecondFactorContextStore` | Constructores | `(session As HttpSessionStateBase)`; `(session As HttpSessionStateBase, clock As ISecondFactorClock)` | No aplica | Contexto pendiente con expiración. | `IPendingSecondFactorContextStore` |

## Métodos y funciones

| Tipo | Ruta del archivo | Clase o interfaz | Nombre exacto | Parámetros y tipos | Retorno | Descripción | Relaciones o dependencias |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Método | `Modelo/Login/SegundoFactor/SegundoFactorModels.vb` | `SegundoFactorChallenge` | `IsExpired` | `clock As ISecondFactorClock` | `Boolean` | Compara `UtcNow >= ExpiresAtUtc`. | Reloj inyectado. |
| Función `Shared` | misma ruta | `SecondFactorContractValidation` | `IsSixDigitCode` | `code As String` | `Boolean` | Exige seis caracteres entre `0` y `9`. | No aplica |
| Función `Shared` | misma ruta | `SecondFactorContractValidation` | `IsMaskedEmailDestination` | `destination As String` | `Boolean` | Exige un único `@`, asterisco en la parte local, dominio no enmascarado y ausencia de espacios. | `SecondFactorRecipient`, `PendingSecondFactorContext` |
| Función `Shared` | misma ruta | `SecondFactorContractValidation` | `MaskEmailAddress` | `emailAddress As String` | `String` | Valida un solo `@` y enmascara parte local. | Lanza `ArgumentException`. |
| Método privado | misma ruta | `SegundoFactorChallenge` | `EnsureUtc` | `value As DateTime`, `parameterName As String` | `Void` | Exige `DateTimeKind.Utc`. | Constructor de challenge. |
| Método | `Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb` | `ISecondFactorOtpGenerator` | `GenerateCode` | Ninguno | `String` | Generar OTP. | No aplica |
| Métodos | misma ruta | `ISecondFactorCodeProtector` | `Protect` / `Verify` | `(context As SecondFactorProtectionContext, code As String)` / los anteriores más `protectedCode As String` | `String` / `Boolean` | Proteger y verificar código. | No aplica |
| Métodos | misma ruta | `ISecondFactorKeyProvider` | `GetActiveKey` / `TryGetKey` | Ninguno / `keyId As String`, `key As SecondFactorKeyMaterial ByRef` | `SecondFactorKeyMaterial` / `Boolean` | Resolver llave activa o indicada. | No aplica |
| Método | misma ruta | `ISecondFactorRecipientResolver` | `Resolve` | `identity As SegundoFactorIdentity` | `SecondFactorRecipient` | Puerto sin implementación. | No aplica |
| Método | misma ruta | `ISecondFactorEmailSender` | `Send` | `message As SecondFactorEmailMessage` | `SecondFactorDeliveryResult` | Puerto sin implementación. | No aplica |
| Método | misma ruta | `ILegacyLoginFinalizer` | `FinalizeLogin` | `context As PendingSecondFactorContext` | `LegacyLoginFinalizationResult` | Puerto sin implementación. | No aplica |
| Métodos | misma ruta | `IPendingSecondFactorContextStore` | `Save` / `GetCurrent` / `Clear` | `context As PendingSecondFactorContext` / ninguno / ninguno | `Void` / `PendingSecondFactorContext` / `Void` | Contrato de Session. | No aplica |
| Método | `Infrastructure/Login/SegundoFactor/Security/SystemSecondFactorClock.vb` | `SystemSecondFactorClock` | propiedad getter `UtcNow` | Ninguno | `DateTime` | UTC del sistema. | `DateTime.UtcNow` |
| Métodos | `Infrastructure/Login/SegundoFactor/Security/CryptographicSecondFactorOtpGenerator.vb` | `CryptographicSecondFactorOtpGenerator` | `GenerateCode` / `Dispose` | Ninguno | `String` / `Void` | Muestreo por rechazo y liberación del RNG. | `RandomNumberGenerator` |
| Método | `Infrastructure/Login/SegundoFactor/Security/AppSettingsSecondFactorKeyProvider.vb` | `ISecondFactorSettings`, `ConfigurationManagerSecondFactorSettings` | `GetValue` | `name As String` | `String` | Contrato e implementación de appSettings. | `ConfigurationManager` |
| Métodos | misma ruta | `AppSettingsSecondFactorKeyProvider` | `GetActiveKey` / `TryGetKey` | Ninguno / `keyId As String`, `key As SecondFactorKeyMaterial ByRef` | `SecondFactorKeyMaterial` / `Boolean` | Valida ID, Base64 y longitud. | `ISecondFactorSettings` |
| Métodos | `Infrastructure/Login/SegundoFactor/Security/HmacSecondFactorCodeProtector.vb` | `HmacSecondFactorCodeProtector` | `Protect` / `Verify` | `(context As SecondFactorProtectionContext, code As String)` / los anteriores más `protectedCode As String` | `String` / `Boolean` | Contrato HMAC público. | Proveedor de llaves. |
| Funciones privadas | misma ruta | `HmacSecondFactorCodeProtector` | `ComputeMac` / `EncodeSegment` / `FixedTimeEquals` | `(key, context, code)` / `(value As String)` / `(expected As Byte(), supplied As Byte())` | `Byte()` / `String` / `Boolean` | Canonicalización, HMAC y XOR. | `HMACSHA256`, UTF-8. |
| Métodos | `webservice/Login/SegundoFactor/SessionPendingSecondFactorContextStore.vb` | `SessionPendingSecondFactorContextStore` | `Save` / `GetCurrent` / `Clear` | `context As PendingSecondFactorContext` / ninguno / ninguno | `Void` / `PendingSecondFactorContext` / `Void` | Guardar, recuperar/expirar y limpiar. | `HttpSessionStateBase`, reloj. |

## DTO: propiedades, obligatoriedad y validaciones

| DTO | Propiedad | Tipo | Obligatoria | Validación implementada |
| --- | --- | --- | --- | --- |
| `SegundoFactorEstadoDto` | `RequiereCodigo` | `Boolean` | Sí por valor | Ninguna adicional. |
| `SegundoFactorEstadoDto` | `DestinoEnmascarado` | `String` | No forzada por el DTO | Debe producirse previamente mediante contrato seguro; el DTO no valida. |
| `SegundoFactorEstadoDto` | `ExpiraEnSegundos` | `Integer` | Sí por valor | El DTO no valida rango. |
| `SegundoFactorEstadoDto` | `ReenvioDisponibleEnSegundos` | `Integer` | Sí por valor | El DTO no valida rango. |
| `SegundoFactorResultadoDto` | `Exito` | `Boolean` | Sí por valor | Ninguna adicional. |
| `SegundoFactorResultadoDto` | `Codigo` | `String` | No forzada | El DTO no valida catálogo. |
| `SegundoFactorResultadoDto` | `MensajeVisible` | `String` | No forzada | El DTO no sanitiza; el productor futuro debe entregar texto neutro. |
| `SegundoFactorResultadoDto` | `Estado` | `SegundoFactorEstadoDto` | No forzada | Puede ser `Nothing`; no existe endpoint consumidor todavía. |

## Propiedades de modelos

| Clase | Propiedades públicas exactas |
| --- | --- |
| `SegundoFactorIdentity` | `EmpresaId As Integer`, `ModuloId As Integer`, `TipoUsuario As String`, `IdInterno As String`, `LoginNormalizado As String`, `ClaveCanonica As String` |
| `SegundoFactorConfiguration` | `IsRequired As Boolean`, `ProviderType As Integer`, `ExpirationMinutes As Integer`; constantes `EmailProviderType=1`, `MaxAttempts=5`, `ResendCooldownSeconds=60`, `MaxResends=2` |
| `SegundoFactorChallenge` | `ChallengeId As Guid`, `Identity As SegundoFactorIdentity`, `Purpose As SegundoFactorPurpose`, `State As SegundoFactorChallengeState`, `Attempts As Integer`, `ResendCount As Integer`, `CreatedAtUtc As DateTime`, `ExpiresAtUtc As DateTime` |
| `SecondFactorProtectionContext` | `Purpose`, `ChallengeId`, `CanonicalIdentity`, `SessionBinding` |
| `SecondFactorKeyMaterial` | `KeyId As String`, `KeyBytes As Byte()`; el getter retorna copia |
| `SecondFactorRecipient` | `EmailAddress As String`, `MaskedDestination As String` |
| `SecondFactorEmailMessage` | `Recipient`, `Code`, `ExpiresAtUtc` |
| `SecondFactorDeliveryResult` | `Success`, `PublicCode` |
| `LegacyLoginFinalizationResult` | `Success`, `LocalRoute`, `PublicCode` |
| `PendingSecondFactorContext` | `EmpresaId`, `ModuloId`, `InternalUserId`, `NormalizedLogin`, `MaskedDestination`, `ChallengeId`, `SessionNonce`, `CreatedAtUtc`, `ExpiresAtUtc` |
