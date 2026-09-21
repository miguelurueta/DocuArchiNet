# Contrato, campos y errores

| Campo | Autoridad | Significado |
| --- | --- | --- |
| `ClientItemId` | Selección validada | Correlación del item |
| `TargetTaskId` | Contexto Workflow | Tarea lógica confirmada |
| `DocumentTypeId/Name` | Resolver TRD | Tipología confirmada |
| `DestinationMode` | Configuración local | `Single` o `Multiple` |
| `ExpedientRequired` | Configuración local | Requisito lógico |
| `Effects` | Builder servidor | Efectos `Planned` |
| `Executable` | Servicio | Todos los requisitos confirmados |

Nunca se publican `ExpedientId`, gabinete físico, tabla, SQL, ruta o credenciales.

Errores estables: `EFFECT_CONFIGURATION_UNAVAILABLE`, `DOCUMENT_TYPE_INVALID`, `EFFECT_DESTINATION_AMBIGUOUS`, `PREFLIGHT_UNAVAILABLE` y `PREFLIGHT_STALE`.
