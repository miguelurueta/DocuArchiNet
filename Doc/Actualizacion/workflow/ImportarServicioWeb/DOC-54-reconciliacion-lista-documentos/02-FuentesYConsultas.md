# Fuentes y consultas

- Ticket: DOC-54
- Cambio OpenSpec: doc-54-reconciliacion-lista-documetos
- Clasificacion: cross_cutting

Las fuentes autoritativas son `workflow_import_intent`, `workflow_import_intent_item` y `registro_producion_documental`. La cabecera aporta propietario, tarea original, proveedor, fase y versión; el item aporta identidad externa, tarea destino, fase, conocimiento de persistencia y correlación; producción confirma que el identificador de almacenamiento existe y aporta el nombre documental. La evidencia de relación se calcula por cardinalidad de `document_id` y `target_task_id` dentro de la persistencia moderna.

Las consultas filtran primero `intent_id`, `user_id` y `task_id`. La consulta focal agrega `provider_id` y `external_key`. Todos los valores se envían como parámetros; la clase no ejecuta INSERT, UPDATE o DELETE y no requiere DDL.
