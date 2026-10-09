# DOC-92 — Contratos e integración

- Ticket: DOC-92
- Cambio OpenSpec: doc-92-segundo-factor-persistencia
- Clasificacion: cross_cutting

## Contratos e integraciones

No existen endpoints, handlers ni payloads HTTP en este cambio. El único contrato externo es la tabla existente `ra_auth_second_factor_challenge` compartida con DocuArchiCore.

| Columna existente | Uso en DocuArchiNet |
| --- | --- |
| `Id` | Clave autoincremental administrada por MySQL. |
| `ChallengeId` | UUID público único del challenge. |
| `AuthUserId` | Identidad canónica del módulo, grupo, tipo e identificador. |
| `Provider` | Valor fijo `EMAIL`. |
| `CodeHash` | Valor protegido `v1:keyId:mac`; no contiene el OTP en claro. |
| `ExpiresAtUtc` | Caducidad UTC. |
| `Consumed` | Bandera atómica de inutilización/adquisición. |
| `Attempts` | Número de verificaciones fallidas. |
| `CreatedAtUtc` | Creación UTC y referencia de cooldown. |
| `AuthPayloadJson` | `NULL` en filas emitidas por DocuArchiNet. |

Se preservan los índices existentes: clave primaria de `Id`, unicidad de `ChallengeId` e índice de `AuthUserId`. DOC-92 no añade ni altera ningún artefacto físico.

DocuArchiCore usa la misma forma de tabla, pero su `CodeHash` y `AuthPayloadJson` responden a otro contrato. Por seguridad y consistencia, los challenges no son intercambiables entre aplicaciones: la compatibilidad buscada es coexistencia sobre el mismo esquema, no validación cruzada.
