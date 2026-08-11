# DOC-2 — Recursos, cutover y compatibilidad

## Orden de carga

`workflow-centro-trabajo-moderno.css?v=20260810-doc2` y `centro-trabajo-visual.js?v=20260810-doc2` se emiten después de `Webworkflow.js` y los scripts legacy situados antes del `body`. Solo se entregan al piloto aprobado en servidor.

## Corte de recursos previos

Se retiraron de `Webworkflow.aspx`, sin borrar archivos, estas capas no aisladas: `gridview-moderno.css`, `workflow-tareas-modernas.css`, `workflow-documentos-relacionados-modernos.css`, `workflow-documentos-relacionados-titulo.css`, `workflow-paginacion-visual.js`, `documentos-relacionados-visual.js` y `documentos-relacionados-titulo-visual.js`.

Son incompatibles con DOC-2 porque usaban selectores globales o scripts que cambiaban clases y, en un caso, movían nodos. No existe endpoint, payload, autenticación, esquema o integración externa nueva.

## Rollback

Los archivos retirados permanecen para auditoría. El rollback operativo de DOC-2 se hace exclusivamente con los `appSettings` de la capa nueva, sin reintroducirlos ni cambiar eventos o datos.
