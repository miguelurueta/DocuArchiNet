# Contrato y estados

- Ticket: DOC-76
- Cambio OpenSpec: doc-76-progreso-integracion-sii
- Clasificacion: cross_cutting

## Solicitud y respuesta

El adaptador recibe `IntentId` obligatorio, `VersionToken`, `TaskId`, `ProviderId`, `OperationId`, `CorrelationId` y `SchemaVersion`. Consume `IntentId`, `Status`, `VersionToken`, `Items[]` y `Error`. Por item conserva los campos contractuales de identidad, fase, documento, tarea, mensaje, error, persistencia y correlación.

La salida contiene `phase`, `status`, `items`, `summary`, `isComplete` e `isTotalSuccess`. El resumen ofrece `saved/guardadas`, `skipped/omitidas`, `failed/fallidas` y `notProcessed/noProcesadas`.

## Mapeo normativo

| Fase backend | Estado visible |
|---|---|
| `Creada` | Disponible |
| `Validada` | Preparando |
| `RecursoObtenido`, `ExpedientePreparado`, `DocumentoAlmacenado`, `ÍndicesActualizados`, `CachéActualizado` | Procesando |
| `ResultadoIncierto` | Verificando |
| `Reconciliada` o `Completada` con `DocumentId` confirmado | Importada |
| `RequiereDecision` | Requiere decisión |
| `FallidaAntesDePersistir` | Fallida |
| `Parcial` | Parcial |
| `Detenida` | No procesada |
| `Omitida` | Omitida |
| Fase desconocida o final sin documento confirmado | Estado conservador; nunca Importada |

`isTotalSuccess` solo es verdadero si hay al menos un item y todos son Importada.
