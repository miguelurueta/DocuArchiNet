<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09 -->
# Especificación — Persistencia transaccional del segundo factor

## ADDED Requirements

### Requirement: RQ-01 — Alcance aislado e inactivo (D-01)

El sistema SHALL incorporar la persistencia de challenges sin exponer endpoints, modificar el login, enviar correo ni depender del contexto HTTP.

#### Scenario: despliegue de DOC-92

- **WHEN** se compila y despliega el cambio sin una tarea posterior de integración
- **THEN** los flujos actuales de autenticación y los demás módulos conservan su comportamiento
- **AND** ningún ASMX, Controller, Service o componente visual nuevo queda expuesto.

### Requirement: RQ-02 — Esquema aditivo y compatibilidad legacy (D-02)

El script SHALL agregar de forma idempotente `Purpose`, `SessionBindingHash`, `State`, `KeyId`, `ResendCount`, `LastSentAtUtc`, `TerminalAtUtc`, `UpdatedAtUtc` y `SchemaVersion` con los tipos definidos en `design.md`.

#### Scenario: aplicación sobre tabla existente

- **GIVEN** una tabla con el contrato físico registrado en la exploración
- **WHEN** se ejecutan preflight, apply y postflight
- **THEN** las columnas nuevas existen sin eliminar ni renombrar columnas previas
- **AND** una segunda ejecución no duplica artefactos ni falla por existencia previa.

#### Scenario: fila legacy

- **GIVEN** una fila previa sin `SchemaVersion = 1`
- **WHEN** el repositorio intenta recuperarla para verificar
- **THEN** la fila no es elegible
- **AND** no se reinterpreta `AuthPayloadJson` como sesión o estado.

#### Scenario: challenge v1 nuevo

- **WHEN** el repositorio crea un challenge
- **THEN** persiste los campos v1 obligatorios
- **AND** deja `AuthPayloadJson` en `NULL`
- **AND** no almacena el vínculo de sesión en claro.

### Requirement: RQ-03 — Índices y motor transaccional (D-03)

El esquema SHALL conservar `uq_challengeid` e `IX_ra_auth_sfc_authuserid`, y SHALL crear los tres índices compuestos definidos por D-03.

#### Scenario: motor incompatible

- **GIVEN** que la tabla no utiliza InnoDB
- **WHEN** se ejecuta el preflight
- **THEN** la validación falla antes de aplicar DDL con un diagnóstico explícito.

#### Scenario: verificación posterior

- **WHEN** se ejecuta postflight después del apply
- **THEN** valida nombres, orden de columnas e índices requeridos.

### Requirement: RQ-04 — Infraestructura y SQL seguros (D-04)

El repositorio SHALL obtener la conexión central mediante `IModuleConnectionFactory`, ejecutar mediante `IDataExecutor`, delimitar transacciones mediante `ITransactionFactory` y parametrizar todos los valores.

#### Scenario: inspección estructural

- **WHEN** se valida `MySqlSecondFactorChallengeRepository`
- **THEN** su constructor recibe las cuatro dependencias definidas por D-04
- **AND** no referencia `HttpContext`, `Session`, cookies ni la clase legacy `conect`
- **AND** no concatena valores externos en SQL.

### Requirement: RQ-05 — Contrato compatible y lectura verificable (D-05)

`ISecondFactorChallengeRepository` SHALL conservar todas las firmas entregadas por DOC-91 y SHALL agregar las operaciones de D-05. `GetVerificationData` SHALL retornar el challenge y el código HMAC protegido necesarios para una verificación posterior.

#### Scenario: consumidor compilado contra DOC-91

- **WHEN** se compila la solución con la interfaz extendida
- **THEN** las firmas anteriores continúan presentes y sin cambios incompatibles.

#### Scenario: lectura autorizada

- **GIVEN** un challenge v1 con vínculo de sesión coincidente y estado elegible
- **WHEN** se invoca `GetVerificationData(challengeId, sessionBindingHash)`
- **THEN** retorna `SecondFactorChallengeVerificationData`
- **AND** incluye `ProtectedCode` sin exponer material de llave.

#### Scenario: formato HMAC inválido

- **WHEN** se intenta crear un challenge cuyo `protectedCode` no sigue `v1:keyId:mac`
- **THEN** el repositorio rechaza la operación sin insertar datos parciales.

### Requirement: RQ-06 — Transiciones y concurrencia atómicas (D-06)

Cada mutación SHALL bloquear la fila, validar el estado esperado y actualizar dentro de una transacción. La adquisición de finalización SHALL tener un único ganador.

#### Scenario: quinta verificación inválida

- **GIVEN** un challenge `SENT` con cuatro intentos fallidos
- **WHEN** se registra otro intento inválido
- **THEN** `Attempts` queda en cinco y `State` queda `BLOCKED` en el mismo commit
- **AND** ninguna verificación posterior puede adquirirlo.

#### Scenario: verificaciones concurrentes

- **GIVEN** un challenge `SENT` válido
- **WHEN** dos transacciones intentan `TryBeginFinalization` concurrentemente
- **THEN** exactamente una cambia el estado a `FINALIZING`
- **AND** la otra retorna falso sin alterar el estado ganador.

#### Scenario: transición desde estado inesperado

- **WHEN** una operación recibe una fila cuyo estado no permite la transición
- **THEN** retorna un resultado negativo
- **AND** la transacción no deja cambios parciales.

### Requirement: RQ-07 — Ciclo de vida, consumo y reenvío (D-07)

El repositorio SHALL respetar el grafo `CREATED -> SENT -> FINALIZING -> COMPLETED` y sus terminales alternos. `Consumed` SHALL ser `1` únicamente en `COMPLETED`.

#### Scenario: adquisición y finalización exitosa

- **WHEN** `TryBeginFinalization` adquiere un challenge enviado
- **THEN** el estado queda `FINALIZING` y `Consumed` permanece `0`
- **WHEN** `Complete` confirma la finalización
- **THEN** el estado queda `COMPLETED`, `Consumed` queda `1` y `TerminalAtUtc` queda informado.

#### Scenario: fallo de finalización

- **WHEN** `FailFinalization` procesa un challenge `FINALIZING`
- **THEN** queda `FINALIZATION_FAILED`, no consumido y terminal.

#### Scenario: reenvío válido

- **GIVEN** que transcurrieron al menos 60 segundos y existen menos de dos reenvíos
- **WHEN** `ReplaceForResend` es solicitado
- **THEN** revoca el challenge anterior y crea el reemplazo en una sola transacción
- **AND** nunca quedan ambos elegibles.

#### Scenario: reenvío fuera de política

- **WHEN** no transcurrió el enfriamiento o ya se alcanzaron dos reenvíos
- **THEN** no revoca ni crea filas y retorna falso.

#### Scenario: expiración

- **WHEN** `Expire` observa un challenge elegible cuyo `ExpiresAtUtc` ya pasó
- **THEN** registra `EXPIRED` y `TerminalAtUtc` atómicamente.

### Requirement: RQ-08 — Despliegue, rollback y retención (D-08)

El paquete SQL SHALL incluir preflight, apply, postflight, rollback, limpieza manual y README operativo.

#### Scenario: rollback controlado

- **WHEN** un operador autorizado ejecuta rollback
- **THEN** solo se eliminan índices y columnas introducidos por DOC-92
- **AND** la documentación advierte que los atributos v1 se perderán y exige respaldo previo.

#### Scenario: limpieza terminal

- **WHEN** se ejecuta manualmente la limpieza con retención de 30 días
- **THEN** solo elimina challenges terminales anteriores al límite
- **AND** no instala eventos, jobs ni schedulers.

### Requirement: RQ-09 — Verificación y evidencia honesta (D-09)

La implementación SHALL proporcionar pruebas automáticas focales, compilación y regresión de la fundación DOC-91. La concurrencia real SHALL probarse únicamente en MySQL descartable con autorización vigente.

#### Scenario: validación sin autorización de base real

- **WHEN** no existe autorización actual para ejecutar integración MySQL
- **THEN** se ejecutan las verificaciones sin conexión real
- **AND** la evidencia de concurrencia real se registra como bloqueada o pendiente, no como aprobada.

#### Scenario: integración autorizada

- **GIVEN** una base MySQL descartable y autorización explícita vigente
- **WHEN** se ejecuta el harness de integración
- **THEN** verifica idempotencia, transiciones, rollback y un único ganador concurrente
- **AND** no usa datos productivos ni presenta la prueba como E2E de usuario.
