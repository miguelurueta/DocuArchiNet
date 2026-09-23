# Impacto UI

- Ticket: DOC-78
- Cambio OpenSpec: doc-78-proteccion-contexto
- Clasificacion: cross_cutting

## Superficies UI

Durante `ExecuteImportIntent` se bloquean búsqueda/selección de tareas y los contenedores existentes `data-workflow-task-action` para devolver, enviar y continuar. El estado previo de cada control se restaura al finalizar. Cerrar el modal no cancela ni revierte la operación y la interfaz lo explica explícitamente.

## Validacion visual

Se debe verificar bloqueo reversible, mensaje durante escritura, foco del resultado, conflicto al cambiar de tarea y ausencia de inserción documental cruzada. El comportamiento permanece detrás de `WorkflowCentroTrabajoModernActive`.
