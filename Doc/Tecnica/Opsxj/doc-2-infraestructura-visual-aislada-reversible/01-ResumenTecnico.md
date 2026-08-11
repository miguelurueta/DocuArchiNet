# DOC-2 — Infraestructura visual aislada y reversible

- Ticket: DOC-2
- Cambio OpenSpec: `doc-2-infraestructura-visual-aislada-reversible`
- Clasificación: cross-cutting

## Objetivo y resultado

DOC-2 introduce una capa visual de Centro de Trabajo Workflow que es opt-in, aislada y reversible. `workflow/Webworkflow.aspx` decide en servidor si un piloto recibe clase raíz y recursos DOC-2; no cambia la lógica WebForms.

## Superficie afectada

- Página: `workflow/Webworkflow.aspx`.
- Decisión: `workflow/Webworkflow.aspx.vb`, `ConfigurationManager.AppSettings` y `GA_LOGINUSUARIOGESTION` de sesión.
- Recursos nuevos: `Styles/workflow-centro-trabajo-moderno.css` y `js/workflow/centro-trabajo-visual.js`.
- Proyecto: ambos recursos están declarados como `Content` en `GestionDocumental-Docuarchi.net.vbproj`.

## Compatibilidad y reversión

No se renombra ni sustituye un control, `UpdatePanel`, hidden input, evento JavaScript o postback. La autorización continúa exclusivamente en servidor. Con flag apagado o sin piloto, no se entrega clase ni recursos DOC-2; la lista moderna de documentos y la reubicación manual de iconos siguen siendo la línea base visible.

El rollback total es `WorkflowCentroTrabajoModernEnabled=false`; el parcial elimina una subcapa de `WorkflowCentroTrabajoModernLayers`. No modifica datos, eventos ni lógica de negocio.
