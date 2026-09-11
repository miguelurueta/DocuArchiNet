# Pruebas y evidencia

- Ticket: DOC-56
- Cambio OpenSpec: doc-56-pruebas-integracion
- Clasificacion: cross_cutting

## Validación local vigente

| Capa | Resultado |
|---|---|
| pruebas focales del contrato de salida | 18/18 |
| `Verify-ImportarServicioWebModern.ps1` | 116/116; 31 archivos |
| MSBuild del proyecto VB | 0 errores; 309 advertencias heredadas |
| OpenSpec estricto | válido |

Las pruebas verifican que `ExecuteImportIntent` pasa por `ProjectExecutionResult`, que el fixture confirmado contiene los campos consumibles por frontend y que una salida no confirmada elimina la identidad documental.

## E2E real del contrato

La corrida RUP autorizada del 2026-09-11 llamó `assertSingleStoredDocument` sobre la respuesta directa de `ExecuteImportIntent` antes de `GetImportIntent` y `ReconcileImportIntent`. Después volvió a confirmar un solo documento y terminó sin código funcional de error.

Una tentativa previa con una cuenta sin tarea seleccionada se detuvo en `QueryItems` con `SESSION_TASK_UNAVAILABLE`, antes de crear intención o almacenar. La reserva se liberó y el gate se restauró. La repetición con la cuenta asociada al recurso fue exitosa.

## Pendiente

La carrera E2E de dos ejecuciones y la recuperación posterior siguen pendientes; no se consideran cubiertas por la ejecución individual.
