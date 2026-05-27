# SCRUMCORE-227 - Integración BackEnd (solo consumo)

Este ticket no modifica backend. Consume:

## 1) Action ver_documento (source of truth)

- Endpoint: `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/action`
- Resultado esperado: `data.DocumentResolveRequest` con `{ NombreGabinete, IdDocumento }`

## 2) Resolve + Firma (delegado al orquestador)

Una vez `DocumentResolveRequest` es obtenido, el flujo se delega a `AppDocumentViewerOrchestrator` (ver documentación del core):

- `POST /api/gestor-documental/documentos/visualizacion/resolve`
- `GET /api/gestor-documental/documentos/{idArchivo}/firma-electronica?...` (solo PDF)

