# BACKEND-CONTRATOS-NOTAS

- Ticket: DOC-40
- Cambio OpenSpec: doc-40-backend-contratos-notas
- Clasificacion: cross_cutting (Transversal)

## Objetivo

DOC-40 implementa la base interna para modernizar Notas del módulo Workflow. Incluye contratos tipados, contexto de servidor, acceso explícito a tarea, coherencia de ruta y resultados funcionales. No contiene endpoint, configuración, datos, consumidor ni activación de funcionalidad.

## Alcance y compatibilidad

El alcance termina dentro de Workflow. Los patrones inspeccionados son el gate de sesión de previsualización, `ITareaWorkflowRepository` y los repositorios MySQL parametrizados. La ruta de negocio integra el contexto `IdRutaWorkflow` y el `IdRuta` de la tarea; no es una URL ni un dato que pueda aportar el navegador. Los metadatos se resuelven desde `rutas_workflow` con `@idRuta` y los identificadores técnicos derivados se validan en servidor. La referencia legacy `Class_anotacion_tarea` se conserva sin cambios y no será la base del diseño moderno. Páginas WebForms, JavaScript, `WorkflowCentroTrabajoModernActive`, migraciones y módulos externos quedan fuera.

La reversa no requiere acción técnica porque no hay comportamiento publicado. Cuando exista un consumidor autorizado, su reversa será devolver ese consumidor a la ruta legacy, sin doble escritura ni pérdida de notas. La futura escritura tiene una precondición adicional: cada esquema objetivo deberá aprobar y superar una migración de datos separada. Por compatibilidad con MySQL 5.1, esta fase conserva `utf8` y limita contenido a Unicode BMP; una futura adopción de `utf8mb4` requiere actualización de motor y cambio independiente.
