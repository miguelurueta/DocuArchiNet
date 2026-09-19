# LISTA-PREVIEW-INCRIPCIONES

- Ticket: DOC-69
- Cambio OpenSpec: doc-69-lista-preview-incripciones
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Entregar el contenido de preview SII mediante un descriptor opaco, temporal, autorizado y de un solo uso. `GetPreview` descarga una vez, valida tipo y tamaño y conserva un snapshot compartido; el handler GET/HEAD entrega los bytes sin exponer URL, token, ruta ni autoridad del proveedor.

## Alcance y compatibilidad

- Backend afectado: ASMX moderno, servicios y repositorio de preview, cliente SII, handler `ImportarServicioWebPreview.ashx`, configuración y tabla temporal en `workflowdocument`.
- Infraestructura afectada: pruebas `node:test`, runner E2E existente y documentación técnica canónica.
- Compatibilidad preservada: no se modifican frontend, endpoints legacy, `ClassAlmacenamiento` ni ejecución DOC-67; el gate moderno permanece apagado por defecto.
- Reversa: apagar el gate y ejecutar el SQL de rollback cuando no existan consumidores activos.
