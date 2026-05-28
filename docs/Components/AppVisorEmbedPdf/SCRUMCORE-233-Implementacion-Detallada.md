# SCRUMCORE-233 — Implementación detallada

## 0) Resumen técnico (qué se cambió)

1) Se instrumentó trazabilidad temporal con `window.__DV_DEBUG__` para correlacionar: click → action → resolve → blob → visor.open.
2) Se confirmó el error raíz del engine: **DocumentManager alcanzando el máximo de 10 documentos abiertos**.
3) Se implementó una solución enterprise “sostenible” basada en:
   - **single-active document** en el visor (cerrar antes de abrir),
   - **cancel chain** (visor + orquestador) sin lock agresivo,
   - **gate anti-duplicados** para evitar `load()` repetido por re-renders.

## 1) Cambios por archivo (qué se tocó y por qué)

### 1.1 `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx`

Objetivo: estabilizar `openDocumentUrl` bajo swaps rápidos y evitar límites internos del engine.

Cambios clave:
- **Observabilidad** (temporal): logs bajo `window.__DV_DEBUG__` con prefijo `[DV][visor]`.
- **Captura del error real**: al fallar `openDocumentUrl` se loguea `err.reason.message` (por ejemplo: `"Maximum number of documents (10) reached"`).
- **Single-active document**:
  - se conserva el `documentId` abierto previamente,
  - antes de abrir un documento nuevo se ejecuta `closeDocument(prevId)` (best‑effort) para no acumular.
- **Handshake “ready”**:
  - tras `openDocumentUrl`, el visor espera `task.wait()` para considerar la carga usable.
- **Hardening del prompt**:
  - si el error es genérico (p. ej. `OPEN_FAILED`/outer task reject), **no** se muestra prompt de contraseña;
  - el prompt solo se activa cuando el engine reporta explícitamente `PdfErrorCode.Password`.

Resultado esperado:
- se evita llegar a `maxDocuments=10` durante navegación normal y estrés de clicks.

### 1.2 `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`

Objetivo: evitar tormentas de carga por re-renders y hacer visible el error real cuando la acción falla.

Cambios clave:
- **Cancel chain best‑effort por click**:
  - `visorRef.current?.cancelCurrentLoad()`
  - `documentViewer.cancelCurrentRequest()`
- **Gate de carga hacia el visor** (`lastVisorLoadKeyRef`):
  - si la clave `(documentId + fileUrl + attemptId/documentKey)` no cambia, no se llama `visorRef.load()` de nuevo.
  - esto corta aperturas duplicadas provocadas por estados intermedios (p. ej. firma resuelta después).
- **Error en action `ver_documento`**:
  - se extrae causa de error (incluyendo errores Axios) y se muestra por toast,
  - se añade timeout (10s) para hacer visible un “cuelgue” del action (sin error en consola).
- **Ajuste de dismiss**: se evita que el toast se cierre inmediatamente durante ráfagas de clicks.

### 1.3 `src/app/Components/UI/AppDocumentViewerOrchestrator/useDocumentViewerOrchestrator.ts`

Objetivo: facilitar diagnóstico sin alterar responsabilidades.

Cambios clave:
- logs bajo `window.__DV_DEBUG__` con prefijo `[DV][attempt:...][req:...]` para:
  - resolve start/ok,
  - download blob start/ok,
  - blobUrl created (+ previous),
  - firma start/ok.
- sin cambios en endpoints ni contrato de backend.

### 1.4 `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.types.ts`

Objetivo: permitir trazabilidad / estado de carga en la API managed (sin romper legacy).

Cambios clave:
- se añadieron campos opcionales:
  - `attemptId?`, `documentKey?`
  - `loadStatus?: "loaded" | "failed" | "cancelled"`

### 1.5 `src/app/Components/UI/AppDocumentViewerOrchestrator/AppDocumentViewerOrchestrator.types.ts`
### 1.6 `src/app/Components/UI/AppDocumentViewerOrchestrator/AppDocumentViewerOrchestrator.adapter.ts`

Objetivo: propagar identidad del intento (cuando el consumidor la provea) sin romper consumidores.

Cambios clave:
- se extendieron inputs/estado runtime con `attemptId?` / `documentKey?` como opcionales.

### 1.7 `src/main.tsx`

Objetivo: asegurar visibilidad del toast.

Cambio clave:
- import del CSS de `react-toastify`:
  - `react-toastify/dist/ReactToastify.css`

## 2) Cómo operar el diagnóstico temporal

En la consola del navegador:

```js
window.__DV_DEBUG__ = true
```

Qué buscar:
- Punto de quiebre del engine:
  - `[DV][visor] openDocumentUrl failed (outer task) { err: { reason: { message: ... }}}`
- Correlación con click:
  - `[DV][attempt:N][seq:S] ...` (Workbench)
- Correlación con red:
  - `[DV][attempt:N][req:R] ...` (Orquestador)

## 3) Invariantes / garantías logradas

- El visor **no acumula** documentos abiertos indefinidamente (close-before-open best‑effort).
- El Workbench no invoca `visorRef.load(...)` (se usa legacy `fileUrl`), evitando doble carga; el hint textual inicial se removiÃ³ y queda solo skeleton diferido.
- Cancelación es un estado normal: no se presenta como error.
- Prompt de contraseña no se dispara por fallos genéricos del engine.
