## Why

La fundación de Notas Workflow ya separa contratos, servicio, repositorio y contexto de sesión, pero el repositorio moderno responde fail-closed. Las lecturas actuales permanecen en código legacy con SQL concatenado y tarea tomada de sesión, lo que impide ofrecer una consulta segura, paginada y aislada por tarea.

## What Changes

- Habilitar únicamente listado, consulta de contenido y contador de notas en el límite moderno ASMX.
- Implementar lectura MySQL parametrizada detrás de `INotasWorkflowRepository`.
- Incorporar cursor protegido y orden determinista para el listado operativo.
- Garantizar que el contenido y el contador respeten la misma tarea autorizada y que no revelen información entre contextos.
- Añadir pruebas con fakes, pruebas de consultas y documentación técnica bajo `Doc/Actualizacion/workflow/Notas/`.
- Reutilizar el control ODBC de solo lectura de `tools/e2e`, con DSN no sensible y credenciales efímeras, para las huellas E2E antes/después.

## Non-goals

- Crear, editar o eliminar notas.
- Cambiar páginas Web Forms, migrar consumidores legacy o modificar `WorkflowCentroTrabajoModernActive`.
- Activar o exponer lectura histórica sin una política de negocio posterior.
- Ejecutar E2E autenticada sin autorización expresa del ambiente y sus datos de prueba.

## Impact

- Afecta los modelos, DTOs, servicio y repositorio modernos de Notas, además de `WebServiceWorkflowNotesModern.asmx.vb`.
- Preserva `workflow/Class_anotacion_tarea.vb` y los endpoints legacy sin alteración.
- Agrega la capacidad `lectura-listado-contador` con trazabilidad D-01 a D-06 y RQ-01 a RQ-06.
