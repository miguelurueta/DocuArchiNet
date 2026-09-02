# ESTABILIZACION-WORKFLOW

- Ticket: DOC-44
- Cambio OpenSpec: doc-44-estabilizacion-workflow
- Clasificacion: cross_cutting (Transversal)
## Servicios y reglas

`WebServiceWorkflowNotesModern.asmx` se valida y reutiliza sin modificaciones. Autorización, propiedad, tarea activa, cursor y versión permanecen en backend. `Class_anotacion_tarea` continúa únicamente como deuda del fallback para fase 06.

No se reactivan permisos comentados, endpoints legacy ni cambios de borrado, retención, supervisión o visibilidad.
