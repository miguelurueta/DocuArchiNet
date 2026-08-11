## Context

DOC-2 afecta `workflow/Webworkflow.aspx`, una página ASP.NET WebForms con `UpdatePanel`, postbacks y permisos resueltos en servidor. El único contenedor autorizado para la capa es `#div_content_general_wf`.

La revisión encontró recursos visuales manuales aprobados antes de DOC-2: `gridview-moderno.css`, `workflow-tareas-modernas.css`, `workflow-documentos-relacionados-modernos.css`, `workflow-documentos-relacionados-titulo.css`, `workflow-paginacion-visual.js`, `documentos-relacionados-visual.js` y `documentos-relacionados-titulo-visual.js`. Dan forma a la lista moderna de documentos y reubican sus iconos/acciones; son la línea base que debe conservarse con el flag DOC-2 apagado.

## Goals / Non-Goals

**Goals**

- Aislar una capa visual que el servidor active solo para pilotos.
- Entregar tokens y componentes del contrato CSS sin alterar controles WebForms.
- Permitir rollback maestro y parcial por configuración.

**Non-Goals**

- Cambiar permisos, reglas Workflow, postbacks, documentos, visor o datos.
- Crear consumidores `AppResponses<T>` o una infraestructura frontend ajena a esta aplicación WebForms.

## Decisions

1. `WorkflowCentroTrabajoModernEnabled` se lee con `ConfigurationManager.AppSettings`; su valor efectivo predeterminado es `false`.
2. `WorkflowCentroTrabajoModernPilotProfiles` es una lista exacta, separada por coma o punto y coma, de valores servidor de `Session("GA_LOGINUSUARIOGESTION")`. No acepta parámetros, cookies, clases añadidas por cliente ni comodines.
3. La página emite `workflow-centro-trabajo-moderno` y los recursos DOC-2 solo si están activos flag maestro y perfil piloto. En cualquier otra combinación no añade clase ni entrega CSS/JavaScript nuevo.
4. `WorkflowCentroTrabajoModernLayers` controla `layout`, `actions`, `documents` y `a11y`; `layout` es dependencia de las demás. El valor predeterminado para un piloto es `layout,actions,documents,a11y`.
5. `workflow-centro-trabajo-moderno.css` y `centro-trabajo-visual.js` se cargan después de `Webworkflow.js` y el resto de scripts legacy antes del `body`, con versión de caché explícita.
6. Los siete recursos manuales inventariados permanecen en la ruta de carga inalterados como baseline aprobado. DOC-2 no los duplica ni los mueve: su hoja empieza todos los selectores con `.workflow-centro-trabajo-moderno` y su adaptador solo añade clases nuevas dentro del contenedor cuando el servidor habilita el piloto. Así la capa nueva se carga después de la base, con precedencia explícita y sin una segunda ruta visual no controlada.
7. La política `AppResponses<T>` sembrada por el flujo no es aplicable: DOC-2 no crea esos consumidores y el repositorio no contiene `src/shared/api`.

## Risks / Trade-offs

- ASP.NET relee `appSettings` con el reciclado normal de aplicación; no hay redeploy de lógica de negocio, datos ni eventos.
- Un piloto vacío bloquea deliberadamente la capa aunque el flag maestro sea verdadero.
- La base manual ya tiene adaptadores que cambian clases y reubican acciones. DOC-2 no los ejecuta por segunda vez; solo añade clases `ctw-*` para el piloto. Cualquier migración futura de la base a CSS scoped se hará en un ticket dedicado para no alterar la apariencia aprobada.

## Migration Plan

1. Publicar los recursos DOC-2 con flag maestro `false` y lista de pilotos vacía.
2. Comprobar que una sesión normal conserva los recursos y la apariencia baseline, pero no entrega clase ni recursos DOC-2.
3. Configurar temporalmente flag `true` y un login de gestión controlado; comprobar clase raíz, recursos y capas en HTML.
4. Retirar `actions`, `documents` o `a11y` de la configuración y comprobar rollback parcial. Para rollback total, restaurar el flag en `false`.

## Open Questions

- La evidencia visual requiere ambiente, cuenta piloto y datos de Workflow controlados; no están disponibles en el workspace local.
