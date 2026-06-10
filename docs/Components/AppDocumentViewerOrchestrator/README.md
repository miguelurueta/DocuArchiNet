# AppDocumentViewerOrchestrator (Docs)

Documentación del núcleo reusable creado para `SCRUMCORE-226`.

## ¿Qué es?

`AppDocumentViewerOrchestrator` es un **núcleo reusable (sin UI)** para orquestar la visualización documental. Su responsabilidad es consolidar en un solo lugar el flujo común que antes podía duplicarse entre módulos:

- `visualizacion/resolve`: resolver el documento y obtener la URL temporal correcta para el visor.
- Seleccionar la URL final: prioridad `UrlTemporalAbsoluta`, fallback `UrlTemporal`.
- Detectar si el documento es PDF.
- Consultar firma electrónica **solo si es PDF** (`firma-electronica`) y sin bloquear la visualización.
- Consolidar un **estado runtime estable** para que consumidores como `AppVisorEmbedPdf` rendericen sin implementar lógica de resolve/firma.

El orquestador existe para evitar:

- Duplicación de lógica.
- Race conditions / respuestas stale por clicks rápidos.
- Manejo desigual de errores y divergencias entre módulos.
- Pérdida del documento visible ante errores (estabilidad del visor).

## ¿Qué NO es?

- No es UI, no renderiza nada.
- No decide permisos, toolbars, ni edición/anotaciones.
- No arma `DocumentResolveRequest` desde “rows DTO” ni infiere datos: eso es del módulo consumidor.
- No persiste URLs temporales en storage/caches.
- No cambia backend/endpoints.

## ¿Cómo se usa? (alto nivel)

El consumidor:

1. Obtiene el contrato canónico `{ documentId, nombreGabinete }` (y opcional `context` solo trazabilidad).
2. Invoca `visualizarDocumento(input)`.
3. Renderiza el visor (p.ej. `AppVisorEmbedPdf`) usando `documentoActivo.fileUrl` + estados (`loading/error`).

Ver detalles de API y semántica en `SCRUMCORE-226-Implementacion-Detallada.md`.

## Referencias cruzadas

- Arquitectura y diagramas Mermaid: `SCRUMCORE-226-Arquitectura.md`
- Contratos, flujo, trazabilidad y pendientes: `SCRUMCORE-226-Implementacion-Detallada.md`
- Endpoints y contratos backend: `SCRUMCORE-226-Integracion-BackEnd.md`
- Tests y evidencias: `SCRUMCORE-226-Pruebas.md`
- Control de cambios y lista de archivos: `SCRUMCORE-226-Metadata.md`

- `SCRUMCORE-226-Arquitectura.md`
- `SCRUMCORE-226-Implementacion-Detallada.md`
- `SCRUMCORE-226-Integracion-BackEnd.md`
- `SCRUMCORE-226-Pruebas.md`
- `SCRUMCORE-226-Metadata.md`
