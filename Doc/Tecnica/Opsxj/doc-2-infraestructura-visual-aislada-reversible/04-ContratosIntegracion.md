# DOC-2 — Recursos, cutover y compatibilidad

## Orden de carga

`workflow-centro-trabajo-moderno.css?v=20260810-doc2` y `centro-trabajo-visual.js?v=20260810-doc2` se emiten después de `Webworkflow.js` y los scripts legacy situados antes del `body`. Solo se entregan al piloto aprobado en servidor.

## Línea base visual preservada

Permanecen cargados en `Webworkflow.aspx`: `gridview-moderno.css`, `workflow-tareas-modernas.css`, `workflow-documentos-relacionados-modernos.css`, `workflow-documentos-relacionados-titulo.css`, `workflow-paginacion-visual.js`, `documentos-relacionados-visual.js` y `documentos-relacionados-titulo-visual.js`.

Esos recursos contienen la lista moderna de documentos y la reubicación de iconos/acciones creada antes de DOC-2; se preservan exactamente para que el modo apagado mantenga la línea base aprobada. DOC-2 no vuelve a cargarlos ni los replica: agrega sus recursos scoped después de la base y solo para piloto. No existe endpoint, payload, autenticación, esquema o integración externa nueva.

## Rollback

El rollback operativo de DOC-2 se hace exclusivamente con los `appSettings` de la capa nueva; la base manual nunca se retira ni se reintroduce durante dicho rollback.
