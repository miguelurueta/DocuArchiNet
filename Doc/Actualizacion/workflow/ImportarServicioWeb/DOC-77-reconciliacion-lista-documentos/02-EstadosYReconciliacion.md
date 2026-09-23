# Estados y reconciliación

- Ticket: DOC-77
- Cambio OpenSpec: doc-77-reconciliacion-lista-documentos
- Clasificacion: cross_cutting

## Mapeo conservador

| Backend | Visible DOC-77 | Confirmado para lista |
| --- | --- | --- |
| `Disponible` | Disponible | Sí, con tarea coincidente y `DocumentId` positivo |
| `Verificando` | Verificando | No |
| `ResultadoIncierto` | ResultadoIncierto | No; permite reconciliación explícita |
| `Inconsistente` | Inconsistente | No |
| `Completado` / `Completada` | Completado | No por sí solo; manda el estado del item |
| `Parcial` | Parcial | No por sí solo |
| `Detenido` / `Detenida` | Detenido | No |
| `Fallido` / `FallidaAntesDePersistir` | Fallido | No |
| Ausente o desconocido | Verificando | No |

## Recuperación

Una reapertura autorizada usa una llamada a `GetImportIntent`. Un item incierto usa una llamada a `ReconcileImportIntent` con `IntentId`, contexto de tarea/proveedor e `ExternalKey`. No existen intervalos, sondeo periódico ni consultas directas a SII.

Las identidades inciertas se deduplican antes de reconciliar. La respuesta sustituye únicamente el item de la misma `ExternalKey`; un estado desconocido no puede promoverse a éxito.
