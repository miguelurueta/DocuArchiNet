# SCRUMCORE-233 — Metadata

- **Ticket**: SCRUMCORE-233
- **Nombre**: Estabilidad de carga en visor (click cancelable + latest‑wins + handshake ready + swap seguro)
- **Fecha**: 2026-05-27 (America/Bogota)
- **Autor**: Equipo Frontend (cambio realizado con Codex CLI)
- **Componentes tocados**:
  - `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx`
  - `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.types.ts`
  - `src/app/Components/UI/AppDocumentViewerOrchestrator/useDocumentViewerOrchestrator.ts`
  - `src/app/Components/UI/AppDocumentViewerOrchestrator/AppDocumentViewerOrchestrator.types.ts`
  - `src/app/Components/UI/AppDocumentViewerOrchestrator/AppDocumentViewerOrchestrator.adapter.ts`
  - `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`
  - `src/main.tsx`
- **Tipo**: Bugfix / hardening de estabilidad + concurrencia
- **Backend**: **NO modificado**
- **Endpoints**: **NO modificados**
- **Persistencia de URLs/tokens**: **NO** (`localStorage/sessionStorage/IndexedDB/caches persistentes`)

## Evidencia (error raíz)

- **Error real observado** (visor / EmbedPDF DocumentManager):
  - `reason.message`: `"Maximum number of documents (10) reached"`
  - Se emite como rechazo en `openDocumentUrl` (“outer task”).

## Diagnóstico final

- **Causa raíz**: el motor/DocumentManager acumulaba documentos abiertos por swaps rápidos (varios `load()` disparados por click + re-renders) sin cerrar el documento anterior → al llegar al límite 10, rechaza aperturas nuevas y el flujo queda “bloqueado” para el usuario.

## Solución aplicada (resumen)

- **Single-active document** en `AppVisorEmbedPdf`: cerrar (best-effort) el documento previo antes de abrir el nuevo.
- **Latest-wins + cancel chain** desde `DocumentosWorkbench`: `cancelCurrentLoad()` (visor) + `cancelCurrentRequest()` (orquestador).
- **Gate anti-duplicados** en `DocumentosWorkbench`: evita ejecutar `visorRef.load()` repetidamente para el mismo `(documentId, fileUrl, attemptId/documentKey)`.

