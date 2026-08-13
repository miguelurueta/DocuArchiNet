# DOC-2 — Recursos, cutover y compatibilidad

## Orden de carga

`Webworkflow.js?v=20260812-taskclose53`, `workflow-centro-trabajo-moderno.css?v=20260812-taskclose40` y `centro-trabajo-visual.js?v=20260812-taskclose12` se emiten en ese orden después de los scripts legacy relevantes y antes del `body`. `documentos-relacionados-titulo-visual.js?v=20260810-title3` se mantiene antes de esos recursos; solo CSS y el adaptador DOC-2 se entregan según la decisión de servidor del piloto. La versión de `Webworkflow.js` invalida caché al restaurar la transición legacy de cierre.

La misma condición de servidor hace visible al inicio de `Page_Load` el `HtmlMeta` estático y tipado `workflowCentroTrabajoModernViewport`, inicialmente invisible, solo para DOC-2 activo. No es un recurso ni una decisión de cliente: permite que los media queries ya publicados usen el ancho CSS real del dispositivo, sin introducir bloques ejecutables `<% If %>` ni modificar la colección de `head`, lo que impediría registrar recursos de AjaxControlToolkit. Una sesión baseline no recibe la etiqueta.

## Línea base visual preservada

Permanecen cargados en `Webworkflow.aspx`: `gridview-moderno.css`, `workflow-tareas-modernas.css`, `workflow-documentos-relacionados-modernos.css`, `workflow-documentos-relacionados-titulo.css`, `workflow-paginacion-visual.js`, `documentos-relacionados-visual.js` y `documentos-relacionados-titulo-visual.js`.

Esos recursos contienen la lista moderna de documentos y la reubicación de iconos/acciones creada antes de DOC-2; se preservan para que el modo apagado mantenga la línea base aprobada. DOC-2 no vuelve a cargarlos ni los replica. El helper de título de documentos conserva la reubicación baseline fuera del piloto y, cuando detecta la raíz emitida por servidor con `ctw-layer-documents`, deja en su posición original los enlaces existentes de carga y actualización para reproducir la barra contextual sin cambiar sus IDs o manejadores. No existe endpoint, payload, autenticación, esquema o integración externa nueva.

## Rollback

El rollback operativo de DOC-2 se hace exclusivamente con los `appSettings` de la capa nueva; la base manual nunca se retira ni se reintroduce durante dicho rollback.
