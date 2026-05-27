# SCRUMCORE-227 - Implementación ver_documento (Gestión Correspondencia)

## Objetivo

Integrar `AppDocumentViewerOrchestrator` como núcleo reusable dentro de `DocumentosWorkbench` para unificar el evento “visualizar documento” desde:

- `row_click`
- `menu_action` (`ver_documento`)

usando `DocumentResolveRequest` como contrato canónico, sin duplicar lógica de resolve/firma y manteniendo estabilidad del visor y selección múltiple intacta.

## Archivos clave

- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`
- `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentosTable.ts`
- `src/app/Components/UI/AppDocumentViewerOrchestrator/` (core reusable)

## Contrato canónico (source of truth)

`DocumentosWorkbench` obtiene `DocumentResolveRequest` desde:

- `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/action` (acción `ver_documento`)

y luego invoca:

- `useDocumentViewerOrchestrator().visualizarDocumento({ documentId, nombreGabinete, context? })`

> Nota (auth/descarga): el orquestador descarga el documento como `Blob` usando el cliente HTTP autenticado del proyecto y expone `documentoActivo.fileUrl` como un `blob:` URL en memoria. Esto evita `401` al consumir `download/{token}` como URL directa sin credenciales.

## Principios / restricciones (del prompt)

- NO modificar backend/endpoints.
- NO tocar permisos/policy internos de `AppVisorEmbedPdf`.
- NO duplicar lógica resolve/firma en Workbench.
- NO romper selección múltiple de `AppTreeTable`.
- En fallos, mantener el documento previamente visible (sin flicker).

## Manejo de errores y notificaciones (comportamiento UX enterprise)

### Qué se notifica

`DocumentosWorkbench` notifica errores **visibles al usuario** en dos momentos:

1) Si falla `action/ver_documento` (no se obtiene `DocumentResolveRequest`), se notifica un mensaje genérico:
   - `No fue posible abrir el documento.`

2) Si falla `visualizacion/resolve` dentro del orquestador, se notifica el **mensaje humano del backend** cuando existe:
   - Ejemplo real: `No existe carpeta física del documento`

> Nota: el orquestador expone el mensaje en `documentoActivo.errors[0]` (y además conserva códigos técnicos como `RESOLVE_FAILED` como respaldo).

### Canal de notificación

Se usa el sistema de notificaciones global del proyecto (`react-toastify`) desde:

- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`

### Toast persistente (no auto-close) y cierre por interacción global

Cuando se emite un error de visualización:

- Se crea un `toast.error(...)` **persistente**:
  - `autoClose: false` (no desaparece solo)
  - `closeOnClick: false` (no se cierra por click sobre el toast)
- El usuario puede cerrar manualmente con la **X** del toast.
- Además, el sistema cierra el toast si el usuario hace click en **cualquier otra parte de la UI**:
  - Se registra un listener global `pointerdown` (captura) para llamar `toast.dismiss(toastId)` y limpiar `viewerError`.
  - El listener se activa en el siguiente tick (`setTimeout(..., 0)`) para evitar cerrar el toast inmediatamente por el mismo click que disparó el error.

### Reintentos (mostrar error en cada click)

Si el usuario hace click nuevamente en la misma fila (reintento) y el backend devuelve el mismo error:

- El toast vuelve a aparecer.
- Esto se habilita reseteando la deduplicación (`lastNotifiedErrorRef`) al inicio de `openViewerFromRow(...)`.
