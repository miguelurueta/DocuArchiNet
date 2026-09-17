# SQL manual DOC-67 — planificación y estados

Estos scripts no se ejecutan automáticamente. Requieren autorización explícita del ambiente, respaldo verificado y confirmación de que las migraciones DOC-52, DOC-53 y DOC-56 ya fueron aplicadas.

## Aplicación

Ejecutar, en orden, `001-add-expedient-planning-state.sql` y `003-create-document-link-cache.sql`. Los scripts son reejecutables: usan `CREATE TABLE IF NOT EXISTS` y el primero consulta `information_schema` antes de añadir columnas, índice o clave foránea. Las columnas nuevas de items aceptan los registros anteriores mediante valores `NULL` o `Pendiente`.

La caché documental usa la identidad global `(task_id,image_id,cabinet_name)`, sin `intent_id`, y conserva el expediente esperado, radicado, estado y fechas de creación/verificación. Su restricción única es la protección final ante carreras; la aplicación deberá releer la fila ganadora y compararla con la relación física.

Validar únicamente con consultas `SELECT`: existencia de tablas/columnas, claves, conteos anteriores y que los items preexistentes conserven sus valores originales.

## Rollback

Detener primero las ejecuciones modernas. Ejecutar `004-rollback-document-link-cache.sql` antes de `002-rollback-expedient-planning-state.sql`. Ambos rollbacks son reejecutables, pero se detienen con `SIGNAL` si encuentran caché verificada, inscripciones, documentos relacionados o estados DOC-67 materializados en items. No borran expedientes, documentos ni relaciones legacy. Si una guarda se activa, conservar las tablas o acordar una exportación antes de cualquier retiro.

No ejecutar estos scripts como parte de pruebas automatizadas.

## Consolidado sin procedures

`005-create-doc67-consolidated-workflow-mysql51.sql` es el camino consolidado y
no debe combinarse con `001`/`003` en la misma puesta en producción. Sirve tanto
para una instalación limpia como para una instalación antigua o parcialmente
actualizada: crea las siete tablas modernas faltantes y completa de forma
idempotente las columnas, índices y la relación de inscripción de
`workflow_import_intent` y `workflow_import_intent_item`. Usa sentencias
temporales `PREPARE/EXECUTE`, pero no crea procedures. Todos los objetos se
crean exclusivamente en `workflowdocument`; no altera tablas físicas de
`docuarchi` ni crea claves foráneas entre bases de datos.
