# SQL manual DOC-53

No ejecutar automáticamente. Requiere autorización del ambiente, respaldo y confirmación de que las tablas DOC-52 existen.

1. Aplicar `001-extend-import-intent-execution-state.sql` antes de habilitar el orquestador.
2. Verificar columnas de resultado e historial mediante consultas `SELECT`.
3. Para rollback, detener ejecuciones modernas y aplicar `002-rollback-import-intent-execution-state.sql`.

Solo se afectan tablas modernas `workflow_import_intent_*`; no se alteran tablas legacy.
