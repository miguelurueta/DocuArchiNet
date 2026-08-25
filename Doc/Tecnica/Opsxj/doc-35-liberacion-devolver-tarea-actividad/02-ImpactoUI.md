# LIBERACION-DEVOLVER-TAREA-ACTIVIDAD

- Ticket: DOC-35
- Cambio OpenSpec: doc-35-liberacion-devolver-tarea-actividad
- Clasificacion: cross_cutting

## Superficies UI

- DOC-35 no modifica Webworkflow.aspx, modales, tablas, estilos ni scripts.
- Se conserva la ruta moderna única de Devolver a actividad anterior, incluida la selección, confirmación, bloqueo durante envío, foco y estado accesible.
- No se reactiva postback, UpdatePanel, GridView, ModalPopupExtender ni una ruta UI alternativa.

## Validacion visual

Se reutiliza la QA no autenticada documentada en DOC-34: respuesta HTTP 200 en escritorio y móvil, modal inicialmente oculto, marcado de diálogo y controles accesibles. No se ejecuta QA autenticada, E2E ni operación de ambiente en DOC-35.
