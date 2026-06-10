# SCRUMCORE-233 — Integración BackEnd

## Estado

**NO aplica cambios**: este ticket no modifica backend ni endpoints.

## Endpoints consumidos (existentes)

El flujo end‑to‑end (ya existente antes del ticket) consume:

- Action “ver_documento” (módulo de correspondencia):
  - ejecutado vía infraestructura de acciones del Workbench (sin cambios aquí).
- Visualización documental:
  - `POST /api/gestor-documental/documentos/visualizacion/resolve`
  - `GET /api/gestor-documental/documentos/visualizacion/download/{token}`
- Firma electrónica (solo PDF):
  - `GET /api/gestor-documental/documentos/{idArchivo}/firma-electronica?nombreGabinete={nombreGabinete}`

## Observación crítica (por qué NO fue backend)

La falla SCRUMCORE-233 se confirmó en el engine del visor:
- `openDocumentUrl` rechazaba con: `"Maximum number of documents (10) reached"`.

Esto es un límite interno del DocumentManager (cliente), no un error de backend. Por diseño, la solución se implementó en el frontend (cierre de documento previo + gating + cancelación), manteniendo backend intacto.

