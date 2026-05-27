# SCRUMCORE-227 - Arquitectura

## Mental model

1 handler unificado → 1 contrato canónico (`DocumentResolveRequest`) → 1 orquestador (`AppDocumentViewerOrchestrator`) → 1 visor consumidor (`AppVisorEmbedPdf`) → 0 lógica duplicada.

## Capas y dependencias

- `DocumentosWorkbench.tsx` (UI/State host): consume hook de tabla + orquestador.
- `useGestionRespuestaDocumentosTable.ts` (data/adapters): ejecuta `action/ver_documento` y devuelve `DocumentResolveRequest` (no resuelve URL aquí).
- `AppDocumentViewerOrchestrator` (core reusable): resolve URL, PDF detection, firma (solo PDF), anti-race, estabilidad.
- `AppVisorEmbedPdf` (visor): renderiza el documento usando `fileUrl` (sin tocar permisos aquí).

Dependencias:

`DocumentosWorkbench` → `useGestionRespuestaDocumentosTable` → `ListaDocumentosRadicados/action`

`DocumentosWorkbench` → `useDocumentViewerOrchestrator` → `visualizacion/resolve` (+ firma si PDF)

## Concurrencia y estabilidad

- Workbench ignora resultados stale del `action/ver_documento` usando un `seq` local.
- Orquestador cancela requests previos y evita out-of-order en resolve/firma.
- Si falla action/resolve/firma, el documento previamente visible se mantiene.

