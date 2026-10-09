# Casos de uso implementados

Todos los actores son consumidores internos futuros; no existe actor HTTP ni usuario final conectado en DOC-92.

## UC-01 — Crear challenge persistido

| Campo | Detalle |
| --- | --- |
| Actor | `EXT: consumidor interno futuro` |
| Precondiciones | Challenge `CREATED`, identidad válida, fechas UTC, HMAC `v1:keyId:mac` y vínculo no vacío. |
| Flujo principal | `Create` valida, abre conexión/transacción, ejecuta `InsertChallenge` y confirma. |
| Alternativos | Un `INSERT` con cero filas retorna `False` y confirma la transacción sin cambios. |
| Errores | Argumentos inválidos; error ADO.NET produce rollback y se propaga. |
| Resultado | `Boolean`. `AuthPayloadJson` queda `NULL`. |
| Endpoint / Controller / Service / Repository | No aplica / No aplica / No aplica / `MySqlSecondFactorChallengeRepository.Create`. |

## UC-02 — Consultar challenge verificable

| Campo | Detalle |
| --- | --- |
| Actor | Consumidor interno. |
| Precondiciones | `challengeId` y vínculo no vacíos. |
| Flujo principal | `GetVerificationData` consulta proveedor `EMAIL`, no consumido, menos de cinco intentos y vigencia futura. |
| Alternativos | Sin fila elegible retorna `Nothing`. |
| Errores | Identificador o vínculo inválidos; errores de conexión/lectura se propagan. |
| Resultado | `SecondFactorChallengeVerificationData` o `Nothing`. |
| Endpoint / Controller / Service / Repository | No aplica / No aplica / No aplica / `GetVerificationData`. |

## UC-03 — Confirmar disponibilidad después del envío

| Campo | Detalle |
| --- | --- |
| Actor | Consumidor interno posterior al envío. |
| Precondiciones | Identificador válido y `sentAtUtc` UTC. |
| Flujo principal | `MarkSent` ejecuta `ExistsActive`. |
| Alternativos | Fila ausente, consumida, bloqueada o vencida retorna `False`. |
| Errores | Fecha no UTC o identificador vacío. |
| Resultado | `Boolean`; no modifica la fila. |
| Endpoint / Controller / Service / Repository | No aplica / No aplica / No aplica / `MarkSent`. |

## UC-04 — Invalidar por fallo de entrega

| Campo | Detalle |
| --- | --- |
| Actor | Consumidor interno posterior al proveedor de correo. |
| Precondiciones | Identificador válido y `failedAtUtc` UTC. |
| Flujo principal | `MarkDeliveryFailed` cambia `Consumed=1` si la fila sigue activa. |
| Alternativos | Si ya no es activa, retorna `False`. |
| Errores | Fecha o identificador inválidos; error ADO.NET propagado. |
| Resultado | `Boolean`. |
| Endpoint / Controller / Service / Repository | No aplica / No aplica / No aplica / `MarkDeliveryFailed`. |

## UC-05 — Registrar intento fallido

| Campo | Detalle |
| --- | --- |
| Actor | Verificador interno. |
| Precondiciones | `expectedAttempts` entre 0 y 4. |
| Flujo principal | Bloquea fila, confirma estado activo y contador esperado, incrementa una vez y confirma. |
| Alternativos | Precondición perdida retorna `Nothing`; el quinto intento se proyecta `BLOCKED`. |
| Errores | Rango/identificador inválido; error técnico revierte. |
| Resultado | `SecondFactorStoredChallenge` o `Nothing`. |
| Endpoint / Controller / Service / Repository | No aplica / No aplica / No aplica / `RegisterFailedAttemptData`. |

## UC-06 — Adquirir finalización exclusiva

| Campo | Detalle |
| --- | --- |
| Actor | Verificador interno tras OTP correcto. |
| Precondiciones | Challenge activo y contador esperado entre 0 y 4. |
| Flujo principal | Bloquea la fila y cambia condicionalmente `Consumed` de 0 a 1. |
| Alternativos | Fila no elegible o carrera perdida retorna `False`; una sola transacción puede ganar. |
| Errores | Entrada inválida o fallo transaccional. |
| Resultado | `Boolean`. |
| Endpoint / Controller / Service / Repository | No aplica / No aplica / No aplica / `TryBeginFinalization`. |

## UC-07 — Consultar finalización consumida

| Campo | Detalle |
| --- | --- |
| Actor | Finalizador interno. |
| Precondiciones | Identificador válido. |
| Flujo principal | `Complete` o `FailFinalization` consulta existencia de fila `EMAIL` con `Consumed=1`. |
| Alternativos | Si no existe, retorna `False`. |
| Errores | Identificador vacío o error ADO.NET. |
| Resultado | `Boolean`; ambos métodos son indistinguibles físicamente. |
| Endpoint / Controller / Service / Repository | No aplica / No aplica / No aplica / `Complete`, `FailFinalization`. |

## UC-08 — Revocar challenge

| Campo | Detalle |
| --- | --- |
| Actor | Consumidor interno. |
| Precondiciones | Identificador válido. |
| Flujo principal | `Revoke` usa el reloj y consume una fila todavía activa. |
| Alternativos | Cero filas afectadas retorna `False`. |
| Errores | Identificador inválido, reloj no UTC o fallo ADO.NET. |
| Resultado | `Boolean`. |
| Endpoint / Controller / Service / Repository | No aplica / No aplica / No aplica / `Revoke`. |

## UC-09 — Reemplazar para reenvío

| Campo | Detalle |
| --- | --- |
| Actor | Orquestador interno futuro. |
| Precondiciones | Challenge anterior activo; reemplazo `CREATED`, misma identidad, reenvío 1 o 2, al menos 60 segundos y fechas UTC. |
| Flujo principal | Bloquea anterior, lo consume, inserta reemplazo y confirma en una transacción. |
| Alternativos | Precondición funcional incumplida retorna `False` sin cambios confirmados relevantes. |
| Errores | Si la inserción no afecta una fila lanza `InvalidOperationException`; toda excepción revierte también la revocación. |
| Resultado | `Boolean`. |
| Endpoint / Controller / Service / Repository | No aplica / No aplica / No aplica / `ReplaceForResend`. |

## UC-10 — Expirar challenge vencido

| Campo | Detalle |
| --- | --- |
| Actor | Proceso interno futuro. |
| Precondiciones | Identificador válido y observación UTC. |
| Flujo principal | `Expire` establece `Consumed=1` si no estaba consumido y `ExpiresAtUtc <= observedAtUtc`. |
| Alternativos | Sin fila elegible retorna `False`. |
| Errores | Entrada inválida o error ADO.NET. |
| Resultado | `Boolean`. |
| Endpoint / Controller / Service / Repository | No aplica / No aplica / No aplica / `Expire`. |

## Operaciones declaradas pero no soportadas por el adaptador

`GetForVerification` y `RegisterFailedAttempt` pertenecen al puerto heredado de DOC-91, pero el repositorio MySQL lanza `NotSupportedException` porque la tabla no almacena `LoginNormalizado`. No constituyen casos exitosos implementados; deben usarse `GetVerificationData` y `RegisterFailedAttemptData`.
