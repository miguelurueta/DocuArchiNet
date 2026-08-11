## Context

DOC-2: INFRAESTRUCTURA-VISUAL-AISLADA-REVERSIBLE

## Jira Details

> # JIRA-02 — Infraestructura visual aislada y reversible
> 
> ## Prompt para Jira
> 
> **Rol:** Actúa como arquitecto frontend senior para aplicaciones ASP.NET WebForms, experto en CSS incremental, carga de recursos legacy y mecanismos de feature flag reversibles.
> 
> Implementa la infraestructura opt-in de modernización del centro de trabajo Workflow. Debe cargar una capa visual aislada sin modificar el comportamiento existente y debe poder desactivarse sin despliegue de lógica de negocio. La implementación parte de la decisión de corte de JIRA-00: no se añade una segunda capa que coexista sin control con recursos visuales ya activos.
> 
> ### Alcance
> 
> - Crear `Styles/workflow-centro-trabajo-moderno.css` y `js/workflow/centro-trabajo-visual.js`.
> - Migrar, aislar o retirar de la ruta de carga los recursos preexistentes que JIRA-00 haya clasificado como incompatibles; ninguna capa moderna debe tener efectos cuando el modo esté apagado.
> - Cargarlos después de todos los recursos CSS y JavaScript legacy relevantes, en particular después de `Webworkflow.js`, con versión de caché explícita y orden verificado en el HTML entregado.
> - Definir una bandera de entorno/configuración evaluada en servidor: `WorkflowCentroTrabajoModernEnabled`, con valor predeterminado `false`, y una lista de perfiles piloto evaluada en servidor. La página emite la clase raíz `workflow-centro-trabajo-moderno` solo cuando ambas condiciones estén aprobadas.
> - Definir clases de subcapa emitidas por el mismo mecanismo: `ctw-layer-layout`, `ctw-layer-actions`, `ctw-layer-documents` y `ctw-layer-a11y`. Deben tener dependencias y valores predeterminados documentados para permitir rollback parcial sin retirar una hoja compartida.
> - Definir contrato de selectores, variables CSS, capas `z-index` y clases de estado.
> - Implementar los tokens y componentes de `CONTRATO-CSS-COMPONENTES-REUTILIZABLES.md`; el resultado visual debe reproducir el HTML base, no una interpretación libre.
> 
> ### Restricciones no negociables
> 
> - Sin la clase de activación la interfaz debe ser idéntica a la actual.
> - No sustituir HTML ni alterar controles ASP.NET.
> - El adaptador no puede modificar DOM, clases, atributos ni foco fuera del contenedor moderno; una clase agregada manualmente desde cliente no constituye segmentación válida del piloto.
> - No habilitar por CSS una acción que el servidor haya ocultado por permisos.
> - El adaptador no puede lanzar errores que bloqueen scripts existentes.
> 
> ### Entregables técnicos
> 
> 1. `01-ArquitecturaActivacion.md`: bandera, perfiles piloto, punto de evaluación servidor, carga, selectores permitidos, subcapas y rollback.
> 2. `02-ContratoCSS.md`: variables, breakpoints y `z-index`.
> 3. `03-PlanCutoverCapasPrevias.md`: destino de cada recurso inventariado en JIRA-00 y prueba de ausencia de efectos con modo apagado.
> 4. `04-PruebasActivacion.md`: evidencia modo apagado/encendido y de cada rollback parcial.
> 5. Hoja CSS con componentes scoped: `.ctw-btn`, `.ctw-icon-btn`, `.ctw-menu`, `.ctw-menu__panel`, `.ctw-badge`, `.ctw-action-bar` y `.ctw-document-bar`.
> 
> ### Criterios de aceptación
> 
> - Con el flag maestro apagado no hay diferencia visual, mutación DOM ni error respecto a la línea base aprobada de JIRA-01.
> - Activar/desactivar la clase raíz o una subcapa cambia solo la presentación de su alcance documentado.
> - No hay errores JavaScript en consola durante carga ni postback.
> - La reversión maestra o parcial no requiere revertir eventos, redeploy de lógica ni modificación de datos.
> 
> ### Pruebas requeridas
> 
> - Comparación visual y funcional con bandera en `0` y `1`.
> - Validar usuario fuera/dentro de perfil piloto; el modo moderno no se habilita mediante parámetro o manipulación de cliente.
> - Validar rollback maestro y de cada subcapa que haya sido habilitada.
> - Validar caché tras recarga forzada y navegación interna.
> 
> ### Reversión
> 
> Desactivar `WorkflowCentroTrabajoModernEnabled` para recuperación total o la subcapa documentada para recuperación parcial; mantener recursos publicados e inertes para no generar referencias rotas.

## Goals / Non-Goals

**Goals**
- Refinar alcance tecnico usando el contexto completo de Jira.
- Definir decisiones arquitectonicas, riesgos y plan de migracion.

**Non-Goals**
- Cambios fuera del alcance descrito por el ticket.

## Decisions

1. Aplicar politica central de AppResponses<T> para evitar parsers locales y filtrado de mensajes tecnicos en UI.

## Politica AppResponses<T>

- Los tickets que consuman `AppResponses<T>` deben centralizar mensajes visibles en `src/shared/api/appResponseError.ts`.
- No se deben duplicar parsers locales para resolver `UserMessage`, `requestId`, `code` o sanitizacion de mensajes tecnicos.
- `response.message` se considera potencialmente tecnico y solo puede mostrarse si el helper confirma que no contiene senales internas.
- El diagnostico completo queda limitado a `logAppResponseErrorDiagnostic` con `window.__APP_RESPONSE_DEBUG__ = true`; la consola puede activarse con `errorsDebugOn()` y apagarse con `errorsDebugOff()`.
- Esta politica es gradual: el bloqueo estricto de nuevos consumidores aplica cuando el helper existe fisicamente.

## Risks / Trade-offs

- Tickets existentes pueden tener parsers locales; la migracion debe ser gradual y enfocada en nuevos consumidores o cambios tocados por cada ticket.

## Migration Plan

1. Sembrar reglas AppResponses<T> en nuevos artefactos `opsxj:new`.
2. Usar `src/shared/api/appResponseError.ts` cuando el ticket consuma APIs con envelope AppResponses<T>.
3. Evitar bloqueo estricto hasta que el helper exista en la rama objetivo.

## Open Questions

- TBD
