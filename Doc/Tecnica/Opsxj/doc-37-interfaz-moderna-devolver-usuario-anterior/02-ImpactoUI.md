# Impacto UI — Devolver a usuario anterior

- Ticket: DOC-37
- Cambio OpenSpec: doc-37-interfaz-moderna-devolver-usuario-anterior
- Clasificacion: cross_cutting

## Superficies UI

El menú Devolver reemplaza el enlace heredado de Usuario anterior por `workflow-return-user-previous-trigger`. El modal propio muestra actividad y usuario históricos resueltos por servidor, o el bloqueo funcional del preview. La confirmación reutiliza `ConfirmationDialog`; no se crea un modal, lista, búsqueda o selector alterno. El estado propio protege apertura, carga, bloqueo, ejecución y recuperación con timeout de quince segundos, sin compartir datos con devolución de actividad o envíos.

## Validacion visual

Las pruebas CJS comprobaron estructura, ARIA, foco inicial, trampa de foco, Escape, backdrop, cancelación, doble clic, bloqueo de cierre durante ejecución y restauración de la bandeja. Con autorización expresa, la QA/E2E de GESTOR completó por separado el preview, la ejecución y el bloqueo UI. Cada sesión seleccionó exclusivamente su tarea autorizada mediante la bandeja Workflow; el preview conservó estado y auditoría, y las dos etapas mutantes verificaron la transición y sus protecciones de cierre sin exponer datos de la respuesta.
