# Arquitectura y diagramas verificados

## Alcance de repositorios y módulos

| Repositorio | Módulos incluidos | Nivel de verificación |
| --- | --- | --- |
| `DocuArchiNet` | `Modelo/Login/SegundoFactor`, `Infrastructure/Repositories/Login/SegundoFactor`, `Infrastructure/Shared/Data`, `Domain/Shared` | Código inspeccionado y símbolos comprobados estructuralmente por Roslyn. |
| `DocuArchiCore` | Modelo y repositorio de `ra_auth_second_factor_challenge` usados como referencia durante la exploración | Referencia externa; no forma parte del checkout ni de la validación CI DOC-92. |

No se incluyen endpoints, controllers, servicios de aplicación, SMTP, UI ni finalización del login porque DOC-92 no implementa esas capas. El recorrido real es:

`EXT: consumidor interno futuro → CODE: ISecondFactorChallengeRepository → CODE: MySqlSecondFactorChallengeRepository → CODE: abstracciones ADO.NET → EXT: MySQL docuarchi → retorno al consumidor`.

## Convención

- `CODE:` identifica una clase, interfaz o método que debe resolverse en el código del repositorio.
- `EXT:` identifica un actor o dependencia externa que no se resuelve como símbolo local.
- `CONCEPT:` identifica una tabla, estado o regla conceptual; tampoco se resuelve contra código.
- Solo `EXT:` y `CONCEPT:` pueden aparecer en `excludedKinds` del manifiesto.

## Diagramas requeridos

| Diagrama | Propósito | Fuentes principales |
| --- | --- | --- |
| `Diagramas/01-componentes-clases.mmd` | Clases, puertos y dependencias reales. | Interfaces, modelos, repositorio y contratos ADO.NET. |
| `Diagramas/02-secuencia-crear-consultar.mmd` | Crear y consultar un challenge elegible. | `Create`, `GetVerificationData` y ejecutores. |
| `Diagramas/03-secuencia-intentos-finalizacion.mmd` | Intentos fallidos y adquisición exclusiva. | `RegisterFailedAttemptData`, `TryBeginFinalization`, `Complete`, `FailFinalization`. |
| `Diagramas/04-secuencia-reenvio-expiracion.mmd` | Reenvío, revocación, expiración y fallo de entrega. | `ReplaceForResend`, `Revoke`, `Expire`, `MarkDeliveryFailed`. |

Cada archivo declara mediante comentarios `Fuentes` y `Referencias CODE`. `diagram-contract.json` es el inventario autoritativo que permite detectar archivos faltantes y referencias divergentes.

## Relaciones implementadas

`MySqlSecondFactorChallengeRepository` implementa `ISecondFactorChallengeRepository`. Recibe por constructor `IModuleConnectionFactory`, `IDataExecutor`, `ITransactionFactory`, `ContextoModulo` e `ISecondFactorClock`. El repositorio no instancia conexiones ni transacciones concretas y no lee `HttpContext` o `Session`.

`SegundoFactorChallenge` es la entrada completa para creación/reemplazo. La lectura no puede reconstruir `SegundoFactorIdentity.LoginNormalizado`, por lo que retorna `SecondFactorChallengeVerificationData`, compuesto por `SecondFactorStoredChallenge` y el valor protegido de `CodeHash`.

## Condiciones y errores relevantes

- Los identificadores vacíos, fechas no UTC, vínculo de sesión vacío y HMAC con formato inválido producen excepciones de argumento antes de la operación correspondiente.
- `GetForVerification` y `RegisterFailedAttempt` lanzan `NotSupportedException`; sus variantes implementadas son `GetVerificationData` y `RegisterFailedAttemptData`.
- Una fila es activa si no está consumida, tiene menos de cinco intentos y no está vencida.
- Las operaciones compuestas bloquean por `SELECT ... FOR UPDATE`, hacen `Commit` cuando terminan normalmente y `Rollback` cuando se propaga una excepción.
- `TryBeginFinalization` consume la fila. El esquema existente no permite diferenciar después éxito y fallo de finalización.
- `MarkSent` solo comprueba que la fila siga activa; no persiste una fecha o estado de envío.
- `sessionBindingHash` se valida como entrada obligatoria, pero no se guarda ni se compara en SQL; la vinculación criptográfica corresponde al protector HMAC de DOC-91 y al futuro consumidor.

## Inconsistencias y límites constatados

- La interfaz de DOC-91 expone estados más ricos que las columnas físicas; DOC-92 deriva únicamente `SENT`, `BLOCKED`, `EXPIRED` y `COMPLETED`.
- `Complete` y `FailFinalization` ejecutan la misma comprobación de `Consumed=1`; no pueden distinguir resultados terminales.
- `Expire` actualiza por `ChallengeId` sin predicado `Provider`; la unicidad de `ChallengeId` reduce el riesgo, pero la diferencia frente a otras operaciones queda registrada.
- No existe Controller o Service que conecte estas operaciones al login. Dibujarlos sería inventar funcionalidad.
