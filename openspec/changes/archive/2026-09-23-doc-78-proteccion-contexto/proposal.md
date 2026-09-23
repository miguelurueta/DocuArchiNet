## Why

La importación moderna puede durar más que el contexto visible de la tarea. DOC-78 evita que una intención preparada para una tarea se ejecute o proyecte silenciosamente sobre otra, y permite recuperar su resultado autoritativo después de interrupciones.

## What Changes

- Capturar y verificar un contexto inmutable de intención/tarea.
- Revalidar mediante preflight justo antes del primer efecto.
- Bloquear acciones Workflow incompatibles durante escrituras y restaurarlas al terminar.
- Representar conflictos normativos y recuperar el estado mediante API moderna.
- Cubrir cambios de tarea en la misma pestaña y señales entre pestañas sin usar estado cliente como autoridad.

## Capabilities

### New Capabilities

- `proteccion-contexto`: protección de tarea, bloqueo de acciones y recuperación autoritativa de importaciones.

### Modified Capabilities

- Ninguna.

## Impact

- Nuevos módulos en `js/workflow/importar-servicio-web/` para guard y recuperación.
- Integración aditiva en `workflow/Webworkflow.aspx`, `importar-servicio-web-ui.js` y el `.vbproj`.
- Nuevas suites Node para contexto, recuperación y múltiples pestañas.
- Documentación canónica en `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-78-proteccion-contexto-recuperacion/`.
- Sin cambios en endpoints, almacenamiento, scripts globales ni acciones Workflow legacy.
