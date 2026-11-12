# DOC-119 — contratos de seguimiento

- Cambio: `doc-119-produccion-importacion-carga-carpeta-seguimiento`.
- Clasificación: `cross_cutting`.

## Contratos e integraciones

Sin DTO/endpoint/ASMX/ASHX nuevos. El cliente usa ConsultarRecursosLote con loteId/tipo/pagina/tamanoPagina. ARCHIVOS incorpora raizId/estado/estadoRecepcion y RAICES estado, omitiendo filtros vacíos. Los códigos de opciones presentados corresponden a estados existentes; no se crea catálogo de negocio.

ExportarPaginaLote copia la consulta efectiva de archivos: conserva parámetros y no extiende el CSV a todo el lote. No promete misma revisión global entre consultas concurrentes. La exportación no se ofrece después de una consulta rechazada.

No se cambia AbrirImportacion, autoridad/ruta/sello ni ConfirmarPlan/ConsultarConfirmacionLote. Los runners REC y DOC-113 no cubren automáticamente SEG-01/02; sus modos con efectos no se ejecutan por inferencia. Plan de aceptación real en doc119-plan-aceptacion-20261111.md y autorizaciones posteriores registradas por escenario. SEG-01 ccb25ec9 real aprobado y SEG-0229623218 con cobertura mixta expresamente aceptada conservan sus SHA/ambiente/límites. Los helpers existentes se adaptaron al menú sin crear autenticador, transporte o coordinador nuevo. La UI final solo tiene regresión local en este corte.
