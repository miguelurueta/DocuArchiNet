# DOC-92 — Contratos e integración

No existen endpoints, handlers ni payloads HTTP en este cambio.

## Contrato físico v1

| Columna | Tipo | Regla |
| --- | --- | --- |
| `Purpose` | `VARCHAR(20)` | `LOGIN` para esta entrega |
| `SessionBindingHash` | `VARCHAR(200)` | Vínculo no reversible |
| `State` | `VARCHAR(30)` | Estado del grafo 2FA |
| `KeyId` | `VARCHAR(100)` | Extraído de `v1:keyId:mac` |
| `ResendCount` | `INT UNSIGNED` | 0 a 2 |
| `LastSentAtUtc` | `DATETIME` | Nullable hasta envío |
| `TerminalAtUtc` | `DATETIME` | Informado al terminar |
| `UpdatedAtUtc` | `DATETIME` | UTC de transición |
| `SchemaVersion` | `SMALLINT UNSIGNED` | `1` para filas nuevas |

Índices nuevos: identidad/propósito/estado, sesión/estado y estado/expiración. Se preservan `uq_challengeid` e `IX_ra_auth_sfc_authuserid`.

Orden operativo: preflight → respaldo → apply → postflight. Los scripts no fueron ejecutados en una base real durante DOC-92. La limpieza es manual y conserva 30 días; no instala scheduler.
