# Metadata

- Ticket: DOC-74
- Rama: `feature/DOC-74`
- Fecha: 2026-09-22
- Estado: implementación local validada; rollout bloqueado por B10
- Gate: `WorkflowCentroTrabajoModernActive=false`
- Dependencias: B01, B02, B06, B10, ASMX y handler de preview
- Riesgo principal: descriptor de un solo uso y disponibilidad del mediador
- Adaptación del visor: `internalDocumentId` no se usa como selección. Se resuelve contra la lista autorizada renderizada por servidor, se verifica la tarea y se reutiliza el postback existente sin modificar el visor.

## Funciones creadas o modificadas

| Función | Ruta | Ubicación | Parámetros | Responsabilidad |
| --- | --- | --- | --- | --- |
| `initial`, `move` | `importar-servicio-web-preview-state.js` | estado | snapshot, estado, datos | Transiciones puras |
| `create`, `descriptorUrl` | `importar-servicio-web-preview.js` | controlador | opciones/descriptor | Solicitud, deduplicación y ruta segura |
| `renderPreview`, `openPreview`, `restoreListContext` | `importar-servicio-web-ui.js` | presentación | control/snapshot/trigger | Render y contexto accesible |
| `mapItem` | `sii/importar-servicio-web-sii-contract-mapper.js` | mapper | item | Identidades externa e interna |
| `renderItems` | `sii/importar-servicio-web-sii-adapter.js` | adapter | container/data | Acción de preview por fila |

No se modificaron funciones backend, almacenamiento ni visor documental.
