<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09 -->
# Diseño técnico — DOC-92 Persistencia transaccional del segundo factor

## Contexto y evidencia

DOC-92 implementa únicamente la persistencia del challenge 2FA sobre la fundación de DOC-91. El diseño se apoya en artefactos versionados; en esta fase no se consulta ni modifica una base de datos.

Evidencia inspeccionada:

- `Doc/Actualizacion/Login/Exploracion/exploracion-doble-factor-autenticacion.md`: esquema físico registrado, estados, identidad canónica, compatibilidad y límites operativos.
- `Modelo/Login/SegundoFactor/SegundoFactorModels.vb`: modelos de challenge entregados por DOC-91.
- `Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb`: contrato actual `ISecondFactorChallengeRepository`.
- `Infrastructure/Shared/Data/ModuleDataContracts.vb`: `IModuleConnectionFactory`, `IDataExecutor` e `ITransactionFactory`.
- `Infrastructure/Shared/Data/AdoNetDataInfrastructure.vb`: infraestructura ADO.NET compartida.
- `Infrastructure/Shared/Data/WorkflowModuleConnectionFactory.vb`: `DocuarchiModuleConnectionFactory` y resolución del contexto central.
- `Infrastructure/Repositories/Workflow/MySqlNotasWorkflowRepository.vb` y `Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportIntentRepository.vb`: patrones transaccionales existentes.
- `GestionDocumental-Docuarchi.net.vbproj`: registro explícito de fuentes VB.NET.

## Inventario exacto del cambio

| Acción | Ruta | Símbolo o propósito |
| --- | --- | --- |
| Modificar | `Modelo/Login/SegundoFactor/SegundoFactorModels.vb` | Agregar `SecondFactorStoredChallenge` y `SecondFactorChallengeVerificationData` sin fabricar login al rehidratar |
| Modificar | `Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb` | Extender de forma compatible `ISecondFactorChallengeRepository` |
| Crear | `Infrastructure/Repositories/Login/SegundoFactor/MySqlSecondFactorChallengeRepository.vb` | Implementación MySQL transaccional |
| Crear | `Doc/Actualizacion/Login/Implementacion/DOC-92/Sql/00-preflight.sql` | Verificación no destructiva del esquema y motor |
| Crear | `Doc/Actualizacion/Login/Implementacion/DOC-92/Sql/01-apply.sql` | Extensión idempotente de tabla e índices |
| Crear | `Doc/Actualizacion/Login/Implementacion/DOC-92/Sql/02-postflight.sql` | Verificación estructural posterior |
| Crear | `Doc/Actualizacion/Login/Implementacion/DOC-92/Sql/03-rollback.sql` | Reversión explícita y advertencias |
| Crear | `Doc/Actualizacion/Login/Implementacion/DOC-92/Sql/04-cleanup-terminal.sql` | Limpieza manual parametrizada de terminales antiguos |
| Crear | `Doc/Actualizacion/Login/Implementacion/DOC-92/Sql/README.md` | Orden, precondiciones y operación segura |
| Crear/modificar | `tests/`, `tools/validation/` o harness MySQL existente | Pruebas focales y validación estructural; integración real solo con autorización vigente |
| Modificar | `GestionDocumental-Docuarchi.net.vbproj` | Incluir los nuevos `.vb` |

Quedan fuera: `webservice/WebServiceLoginSegundoFactor.asmx(.vb)`, UI, Controller/Service, envío SMTP, activación funcional y cualquier uso de `Session` o `HttpContext`.

## Decisiones

### D-01 — Límite físico y funcional cerrado

La tarea crea persistencia reutilizable e inactiva. No expone endpoints, no altera el login y no conecta todavía la orquestación 2FA. Esto impide que una migración de almacenamiento cambie módulos existentes.

### D-02 — Evolución aditiva compatible

Sobre `ra_auth_second_factor_challenge` se agregan, de forma idempotente y inicialmente nullable para tolerar filas legacy:

| Columna | Tipo propuesto | Regla para filas v1 |
| --- | --- | --- |
| `Purpose` | `VARCHAR(20)` | Obligatoria por aplicación |
| `SessionBindingHash` | `VARCHAR(200)` | Obligatoria; nunca se almacena el vínculo en claro |
| `State` | `VARCHAR(30)` | Obligatoria y perteneciente al grafo permitido |
| `KeyId` | `VARCHAR(100)` | Obligatoria; identifica la llave del HMAC protegido |
| `ResendCount` | `INT UNSIGNED` | Obligatoria, inicia en cero |
| `LastSentAtUtc` | `DATETIME` | Nullable hasta el envío |
| `TerminalAtUtc` | `DATETIME` | Nullable hasta estado terminal |
| `UpdatedAtUtc` | `DATETIME` | Obligatoria |
| `SchemaVersion` | `SMALLINT UNSIGNED` | `1` en filas nuevas |

Las filas nuevas dejan `AuthPayloadJson = NULL`. Las filas legacy, sin `SchemaVersion = 1`, no son elegibles para verificación. La obligatoriedad de v1 se valida en aplicación para no imponer una restricción destructiva a datos existentes.

### D-03 — Índices orientados a operaciones

Se preservan `uq_challengeid` e `IX_ra_auth_sfc_authuserid`. Se agregan:

- `IX_ra_auth_sfc_identity_purpose_state (AuthUserId, Purpose, State)`;
- `IX_ra_auth_sfc_session_state (SessionBindingHash, State)`;
- `IX_ra_auth_sfc_state_expiry (State, ExpiresAtUtc)`.

El preflight falla si la tabla no usa InnoDB, porque el diseño depende de bloqueos de fila y transacciones.

### D-04 — Dependencias de datos inyectadas

`MySqlSecondFactorChallengeRepository` recibe:

```vb
New(connections As IModuleConnectionFactory,
    executor As IDataExecutor,
    transactions As ITransactionFactory,
    centralContext As ContextoModulo,
    clock As ISecondFactorClock)
```

Todas las sentencias parametrizan valores. La conexión se obtiene del contexto central ya existente y el tiempo UTC proviene de `ISecondFactorClock`; no se usa reloj global en las transiciones. Infrastructure no lee configuración web, `Session`, `HttpContext`, cookies ni datos del request.

### D-05 — Extensión contractual compatible

No se eliminan las firmas de DOC-91. Persistencia no puede reconstruir honestamente `SegundoFactorIdentity`, porque la tabla guarda la identidad canónica y deliberadamente no guarda el login. Se agregan `SecondFactorStoredChallenge`, con `CanonicalIdentity` y los campos persistidos, y `SecondFactorChallengeVerificationData`, que reúne `Challenge As SecondFactorStoredChallenge` y `ProtectedCode As String`. Se incorporan operaciones explícitas:

```vb
Function GetVerificationData(challengeId As Guid, sessionBindingHash As String) As SecondFactorChallengeVerificationData
Function MarkSent(challengeId As Guid, sentAtUtc As DateTime) As Boolean
Function MarkDeliveryFailed(challengeId As Guid, failedAtUtc As DateTime) As Boolean
Function RegisterFailedAttemptData(challengeId As Guid, expectedAttempts As Integer) As SecondFactorStoredChallenge
Function ReplaceForResend(previousChallengeId As Guid, replacement As SegundoFactorChallenge, protectedCode As String, sessionBindingHash As String, requestedAtUtc As DateTime) As Boolean
Function Expire(challengeId As Guid, observedAtUtc As DateTime) As Boolean
```

`GetForVerification` y `RegisterFailedAttempt` se conservan para no romper el contrato de DOC-91, pero la implementación MySQL falla explícitamente con `NotSupportedException`: no inventa `LoginNormalizado`. Los consumidores nuevos deben usar `GetVerificationData` y `RegisterFailedAttemptData`; el login requerido para finalizar permanece en `PendingSecondFactorContext`. `KeyId` se deriva y valida desde `v1:keyId:mac`; no se duplica una llave secreta.

### D-06 — Mutaciones atómicas y control de concurrencia

Cada transición de estado abre transacción, selecciona la fila mediante `SELECT ... FOR UPDATE`, valida estado/versión/expiración/vínculo y ejecuta un `UPDATE` condicionado por el estado observado. El quinto intento fallido cambia atómicamente a `BLOCKED`. Ante verificaciones concurrentes, solo una transición `SENT -> FINALIZING` puede ganar.

### D-07 — Grafo de estados y semántica de consumo

Flujo principal: `CREATED -> SENT -> FINALIZING -> COMPLETED`. Alternos: `DELIVERY_FAILED`, `BLOCKED`, `EXPIRED`, `REVOKED` y `FINALIZATION_FAILED`. `Consumed = 1` únicamente en `COMPLETED`; adquirir finalización no consume el challenge. El reenvío revoca el challenge anterior y crea el reemplazo dentro de una sola transacción, con enfriamiento de 60 segundos y máximo dos reenvíos.

### D-08 — Operación SQL reversible y manual

La secuencia es preflight, apply, postflight. Los scripts consultan `information_schema` para ser reejecutables. El rollback elimina únicamente artefactos DOC-92 y advierte que puede perder datos v1. La limpieza elimina estados terminales con antigüedad mayor a 30 días, exige ejecución manual y no instala scheduler/evento.

### D-09 — Evidencia proporcional y autorización

Se ejecutan pruebas automáticas con dobles de `IDataExecutor`/transacción, compilación y regresiones DOC-91. La prueba de concurrencia contra MySQL real usa una base descartable y solo se ejecuta con autorización nueva y explícita; si no existe, se registra como bloqueada, nunca como aprobada. No aplica un E2E de navegador porque esta tarea no expone flujo de usuario.

## Flujo transaccional

```text
Caller
  -> ISecondFactorChallengeRepository
     -> MySqlSecondFactorChallengeRepository
        -> IModuleConnectionFactory.CreateOpenConnection(centralContext)
        -> ITransactionFactory.BeginTransaction(connection)
        -> SELECT ... FOR UPDATE
        -> validar SchemaVersion, State, expiración y SessionBindingHash
           -> inválido: ROLLBACK + resultado negativo
           -> válido: UPDATE ... WHERE ChallengeId = @id AND State = @expected
        -> COMMIT
  <- resultado sin datos secretos
```

## Riesgos y mitigaciones

- **Motor distinto de InnoDB:** el preflight bloquea la aplicación del DDL.
- **Filas legacy:** los campos nuevos permanecen nullable y `SchemaVersion = 1` es requisito de elegibilidad.
- **Carrera entre verificaciones:** bloqueo de fila más transición condicionada garantiza un único ganador.
- **Falla del finalizador futuro:** se conserva `FINALIZATION_FAILED`; no se afirma atomicidad distribuida con componentes fuera de DOC-92.
- **Rollback con datos nuevos:** el README exige respaldo y explicita la pérdida de atributos v1.
- **Pruebas sin MySQL real:** validan contrato y SQL emitido, pero no sustituyen evidencia del aislamiento del motor.

## Preguntas abiertas

Ninguna para iniciar implementación. Cualquier diferencia entre el esquema versionado y un ambiente real requiere detener la migración y obtener autorización vigente para una inspección `information_schema` de solo lectura.
