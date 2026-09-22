<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## Context

DOC-74 agrega al modal moderno una vista segura para recursos SII aún no almacenados. Ya existen `GetPreview`, descriptor temporal y handler defensivo; falta la composición frontend, sus estados y la conservación del contexto de lista.

## Goals / Non-Goals

**Goals**: usar la frontera mediada; distinguir preview y documento importado; modelar expiración, fallback, foco y navegación responsive; mantener una descarga por apertura.

**Non-Goals**: modificar almacenamiento o visores; crear otro cliente HTTP o raíz frontend; habilitar gates o simular un backend ausente.

## Decisions

### D-01 — Descriptor mediado como única autoridad

`importar-servicio-web-preview.js` invoca `api.getPreview` con contexto, proveedor e identidad externa. La ruta same-origin se deriva solo del descriptor codificado. Se descarta insertar URL de fila o usar `window.open` como recorrido principal.

### D-02 — Estado aislado y solicitud única

`importar-servicio-web-preview-state.js` es una máquina pura. Conserva la solicitud de la apertura actual; foco, resize y rerender no la reinician. Estados: preparando, disponible, formato no visualizable, recurso vencido, proveedor no disponible, no autorizado y bloqueado. Renovar es una acción explícita sin mutación.

### D-03 — Panel aditivo y restauración

El panel vive dentro del diálogo. En escritorio es lateral y en ancho reducido es subvista con `Volver a la lista`. Antes de abrir captura fila, filtros, scroll y foco; cerrar o volver los restaura sin otra consulta o descarga.

### D-04 — Documento importado separado

El preview se rotula como temporal. `Ver documento importado` solo aparece con identidad interna reconciliada y autorizada y delega al visor vigente, sin construir rutas desde datos externos.

### D-05 — Fallo cerrado y B10

Gate, autorización, expiración y proveedor se traducen a estados sin respuestas completas. Si el mediador falta, la UI queda bloqueada. El fallback reutiliza el handler seguro.

### D-06 — Límites y evidencia

Solo se crean los dos scripts canónicos. Markup, code-behind, CSS y `.vbproj` reciben cambios aditivos. Las tres pruebas indicadas cubren flujo, seguridad y accesibilidad. La documentación vive bajo `docs/modulos/workflow/importar-servicio-web/DOC-74-vista-segura-recursos-externos/`.

## Risks / Trade-offs

- El descriptor puede ser de un solo uso; no habrá recargas implícitas.
- MIME no visualizables usan descarga mediada.
- Sin identidad interna se omite la acción del visor.
- El rollout queda bloqueado aunque las pruebas locales pasen si B10 no está disponible.

## Migration Plan

1. Agregar módulos y pruebas sin activar gates.
2. Integrar markup, estilos y scripts aditivamente.
3. Ejecutar pruebas focales y regresión relacionada.
4. Confirmar B10 con evidencia autorizada antes de rollout.
5. Rollback retirando panel y registros aditivos.

## Open Questions

- El ambiente objetivo deberá confirmar B10; la implementación local permanece cerrada por defecto.
