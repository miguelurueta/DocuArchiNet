# DOC-2 — Trazabilidad de JIRA-02

## Objetivo verificable

JIRA-02 implementa una capa visual opt-in, aislada y reversible para el Centro de Trabajo Workflow. La modernización es de presentación: conserva controles ASP.NET WebForms, postbacks, permisos y datos; solo se entrega a un perfil piloto que el servidor aprueba.

## Mapa de alcance a implementación

| Alcance JIRA-02 | Evidencia de implementación | Resultado esperado |
| --- | --- | --- |
| Capa visual nueva | `Styles/workflow-centro-trabajo-moderno.css` y `js/workflow/centro-trabajo-visual.js` | Componentes y clases scoped, sin markup paralelo. |
| Activación segura | `workflow/Webworkflow.aspx.vb` y `Web.config` | Flag apagado por defecto, piloto por `GA_LOGINUSUARIOGESTION` y sin activación por cliente. |
| Carga ordenada | `workflow/Webworkflow.aspx` | Recursos DOC-2 versionados después de `Webworkflow.js` y de la línea base legacy. |
| Subcapas reversibles | `WorkflowCentroTrabajoModernLayers` | `layout`, `actions`, `documents` y `a11y`; `layout` depende de las demás. |
| Base previa | `04-ContratosIntegracion.md` | Los siete recursos manuales previos se preservan y no forman parte del rollback DOC-2. |
| Contrato visual | `02-ImpactoUI.md` y `CONTRATO-CSS-COMPONENTES-REUTILIZABLES.md` | Tokens, componentes, estados, breakpoint y `z-index` bajo la clase raíz. |

## Criterio de salida

Con flag apagado o usuario fuera de piloto, no se entregan recursos ni clase DOC-2 y la línea base queda idéntica. Con flag y piloto aprobados, solo `#div_content_general_wf` recibe la clase raíz y las subcapas calculadas. El rollback total usa `WorkflowCentroTrabajoModernEnabled=false`; el parcial retira una subcapa sin cambiar lógica, eventos o datos.

La promoción queda pendiente de QA manual con acceso TLS, cuentas piloto/no piloto, datos Workflow controlados y evidencia asociada al SHA desplegado. Un acceso bloqueado o una prueba no ejecutada no equivale a aprobación.
