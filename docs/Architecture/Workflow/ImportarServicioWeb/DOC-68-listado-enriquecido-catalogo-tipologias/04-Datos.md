# Datos y SQL

Catálogo: `ra_dig_tipos_docum_lista_chequeo` unido a `tipo_doc_series`, filtrado mediante `@procedureId`. Se publica ID TRD, descripción, `OBLIGATORIO` y `ORDEN_LISTA`; el ID de checklist permanece interno.

Estado: `workflow_import_intent` unido a `workflow_import_intent_item`, filtrado mediante `@taskId`, `@providerId` y `@externalKey`. Solo documento persistido, intención/item completados y relación/índice/caché confirmados produce `Importado`.

Ambas consultas son `SELECT` parametrizadas.
