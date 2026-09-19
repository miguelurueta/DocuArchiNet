# LISTADO-TIPOLOGIAS

- Ticket: DOC-68
- Cambio OpenSpec: doc-68-listado-tipologias
- Clasificacion: cross_cutting (Transversal)
## Superficies UI

- [x] DOC-68 no modifica páginas WebForms, UserControls, modales, tablas ni estilos.
- [x] No aplica validación de foco, `hover`, selección, responsive o accesibilidad porque el alcance es exclusivamente backend/ASMX.

El frontend futuro consume los campos aditivos `Metadata`, `ImportStatus`, `AllowedActions` y el catálogo de tipologías; esta entrega no altera su representación visual.

## Validacion visual

No aplica captura visual. La validación real se realizó a nivel de contrato autenticado mediante la plataforma E2E compartida para MERCANTIL, ESAL y RUP. Las páginas legacy no presentaron diferencias y el gate quedó apagado después de cada corrida.
