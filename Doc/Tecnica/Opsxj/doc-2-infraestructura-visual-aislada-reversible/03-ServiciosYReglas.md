# DOC-2 — Activación y reglas de servidor

## Regla de activación

`Webworkflow.WorkflowCentroTrabajoModernActive` exige dos condiciones: `WorkflowCentroTrabajoModernEnabled` con `true`, `1` o `yes`, y coincidencia exacta sin distinción de mayúsculas entre `GA_LOGINUSUARIOGESTION` y `WorkflowCentroTrabajoModernPilotProfiles`. La lista permite coma, punto y coma o salto de línea; vacía nunca habilita la capa.

`WorkflowCentroTrabajoModernLayers` acepta `layout`, `actions`, `documents` y `a11y`. El valor predeterminado para piloto es `layout,actions,documents,a11y`; si falta `layout`, no se emite ninguna subcapa. El flag predetermina `false`.

## Integridad funcional

No se consulta ni modifica permiso, tarea, documento, visor o sesión. La bandera no puede habilitarse por URL, cookie o clase de navegador. El adaptador solo usa `classList` dentro de `#div_content_general_wf` y se reaplica en `PageRequestManager.endRequest`.

## AppResponses

No aplica: DOC-2 no consume `AppResponses<T>` y el repositorio no contiene `src/shared/api`. No se añadió helper, parser ni registro de errores frontend artificial.
