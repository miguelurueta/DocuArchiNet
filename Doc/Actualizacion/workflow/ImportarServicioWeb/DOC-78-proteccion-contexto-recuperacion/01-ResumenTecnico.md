# Resumen técnico

- Ticket: DOC-78
- Cambio OpenSpec: doc-78-proteccion-contexto
- Clasificacion: cross_cutting

## Objetivo

Impedir que una intención de importación cambie silenciosamente de tarea y recuperar su estado desde el backend después de conflictos, recargas o pérdida de conexión.

## Alcance y compatibilidad

La solución agrega un guard de contexto y un adaptador de recuperación al feature moderno. Conserva sin cambios endpoints, ASMX, almacenamiento, handlers globales, `AlmacenaDocumentoTareaWorkflow(...)`, `ClassAlmacenamiento` y la lista documental legacy. El backend continúa como única autoridad.
