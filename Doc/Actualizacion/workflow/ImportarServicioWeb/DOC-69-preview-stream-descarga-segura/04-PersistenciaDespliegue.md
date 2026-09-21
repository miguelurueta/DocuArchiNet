# Persistencia, despliegue y rollback

La tabla `workflow_import_preview_descriptor` se crea exclusivamente en `workflowdocument`. Usa `MEDIUMBLOB`, hash único, índices de autoridad/estado/expiración y no contiene llaves foráneas. El recurso externo se conserva como SHA-256, no como `ExternalKey`.

Orden de despliegue:

1. Ejecutar `001-create-workflow-import-preview-descriptor-mysql51.sql` en Workflow.
2. Desplegar binarios y handler manteniendo `WorkflowCentroTrabajoModernActive=false`.
3. Validar build y pruebas focales.
4. Habilitar únicamente mediante el procedimiento controlado existente.

Rollback: apagar el gate; retirar binarios si procede; ejecutar `002-rollback-workflow-import-preview-descriptor.sql` solo después de confirmar que no hay consumos activos. El rollback no modifica documentos, tareas, expedientes ni índices.

Fuentes: `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-69-lista-preview-incripciones/Sql/`, `web.config`.
