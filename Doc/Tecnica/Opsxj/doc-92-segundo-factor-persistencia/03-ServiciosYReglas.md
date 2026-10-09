# DOC-92 — Repositorio y reglas

- Ticket: DOC-92
- Cambio OpenSpec: doc-92-segundo-factor-persistencia
- Clasificacion: cross_cutting

## Servicios y reglas

`MySqlSecondFactorChallengeRepository` implementa `ISecondFactorChallengeRepository` e inyecta `IModuleConnectionFactory`, `IDataExecutor`, `ITransactionFactory`, `ContextoModulo` e `ISecondFactorClock`. No accede a `HttpContext`, sesión WebForms ni a la conexión global `conect`.

| Método | Comportamiento con el esquema existente |
| --- | --- |
| `Create` | Inserta las nueve columnas funcionales; `Id` es autoincremental y `AuthPayloadJson` queda `NULL`. |
| `GetVerificationData` | Lee un challenge `EMAIL` no consumido, vigente y con menos de cinco intentos. |
| `MarkSent` | Confirma que la fila ya creada continúa activa; no inventa una columna de envío. |
| `MarkDeliveryFailed` | Consume el challenge activo para impedir su uso. |
| `RegisterFailedAttemptData` | Bloquea con `SELECT ... FOR UPDATE` e incrementa `Attempts` con valor esperado. |
| `TryBeginFinalization` | Adquiere un único ganador cambiando atómicamente `Consumed` de `0` a `1`. |
| `Complete` / `FailFinalization` | Comprueban la adquisición ya consumida; no persisten estados inexistentes. |
| `Revoke` / `Expire` | Marcan `Consumed=1` bajo sus condiciones de vigencia. |
| `ReplaceForResend` | En una transacción consume el anterior e inserta el reemplazo; usa `CreatedAtUtc` para el cooldown. |

Los estados expuestos por la proyección son derivados: `Consumed=1` produce `COMPLETED`, cinco intentos producen `BLOCKED`, la fecha vencida produce `EXPIRED` y una fila utilizable produce `SENT`. No se almacenan `State`, `Purpose`, vínculo de sesión, contador de reenvíos ni fechas terminales.

El límite de reenvíos permanece en el contexto de dominio/sesión porque la tabla no tiene un contador. Después de que `TryBeginFinalization` consume la fila, un fallo posterior obliga a reiniciar el login: el esquema actual no permite representar `FINALIZING` o revertir con seguridad una adquisición.
