# DOC-115 — ContratosIntegracion

Ticket: DOC-115. Cambio: doc-115-produccion-importacion-carga-carpeta-lotes. Impacto: cross_cutting. Estado: implementación y validación completas, entrega pendiente de revisión/publicación.

## Contratos e integraciones

Se conservan DTOs y entradas de ConfirmarPlan y multipart loteId/archivoId/claveIntento más archivo. ConsultarConfirmacionLote es el nombre real de la fachada usado por las pruebas. No se publica reconciliación interna ni otro endpoint de prueba. 07-A consume el repositorio mediante la conexión operativa registrada y verifica autoridad/nivel/publicación por SELECT; no activa perfiles ni usa UI. Referencia normativa: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/02-contratos-operacion.md. La sobrecarga del helper de rollback conserva la firma anterior del test y evita un ID fijo en el modo nuevo.
