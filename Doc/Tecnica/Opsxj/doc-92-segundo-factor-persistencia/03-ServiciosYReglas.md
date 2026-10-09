# DOC-92 — Repositorio y reglas

`MySqlSecondFactorChallengeRepository` implementa `ISecondFactorChallengeRepository` y recibe `IModuleConnectionFactory`, `IDataExecutor`, `ITransactionFactory`, `ContextoModulo` e `ISecondFactorClock`.

Operaciones nuevas:

| Método | Resultado | Transición/regla |
| --- | --- | --- |
| `GetVerificationData(Guid, String)` | `SecondFactorChallengeVerificationData` | Solo fila v1 `SENT`, no consumida y ligada a sesión |
| `MarkSent(Guid, DateTime)` | `Boolean` | `CREATED -> SENT`; registra último envío |
| `MarkDeliveryFailed(Guid, DateTime)` | `Boolean` | `CREATED -> DELIVERY_FAILED` |
| `RegisterFailedAttemptData(Guid, Integer)` | `SecondFactorStoredChallenge` | Incrementa una vez; quinto intento -> `BLOCKED` |
| `TryBeginFinalization(Guid, Integer)` | `Boolean` | `SENT -> FINALIZING`; un único ganador |
| `Complete(Guid)` | `Boolean` | `FINALIZING -> COMPLETED`; único caso `Consumed=1` |
| `FailFinalization(Guid)` | `Boolean` | `FINALIZING -> FINALIZATION_FAILED` |
| `Revoke(Guid)` | `Boolean` | `CREATED/SENT -> REVOKED` |
| `ReplaceForResend(...)` | `Boolean` | Revoca y crea en una transacción; 60 segundos y máximo dos |
| `Expire(Guid, DateTime)` | `Boolean` | `CREATED/SENT -> EXPIRED` cuando vence |

Cada mutación bloquea la fila con `SELECT ... FOR UPDATE`, comprueba `SchemaVersion`, estado y datos esperados y actualiza con condición equivalente. Errores técnicos revierten la transacción y se propagan; una precondición funcional incumplida retorna `False` o `Nothing` sin cambios.
