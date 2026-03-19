## Why

La aplicacion no tiene hoy un manual tecnico reutilizable que explique como resolver navegacion interna entre componentes o vistas de un modulo sin perder el estado critico del flujo. Esa ausencia hace que decisiones como usar tabs locales, layouts persistentes o subrutas con `Outlet` dependan de criterio ad hoc y no de una referencia comun del proyecto.

El ticket `SCRUMCORE-7` busca cerrar esa brecha mediante un documento tecnico claro y reusable que sirva como guia de implementacion para futuros cambios, refactors modulares y tareas asistidas por IA.

## What Changes

- Se creara un manual tecnico en formato Markdown dentro de `docs/` para documentar una metodologia de preservacion de estado y navegacion interna en modulos SPA React.
- El manual describira dos enfoques:
  - Metodo A: contenedor persistente con vistas internas sin cambio de ruta.
  - Metodo B: layout persistente con subrutas internas y `Outlet`.
- El documento incluira objetivo, diferencias, casos de uso, arquitectura, flujo paso a paso, ejemplos practicos, buenas practicas, riesgos y lineamientos para reutilizar la metodologia en otros modulos.
- El manual cerrara con un prompt profesional para Jira/Codex/IA orientado a implementar la metodologia respetando la arquitectura actual del sistema.

## Capabilities

### New Capabilities
- `manual-persistencia-estado-navegacion-interna`: Documentacion tecnica reutilizable para decidir e implementar patrones de navegacion interna con preservacion de estado en la SPA.

### Modified Capabilities
- Ninguna.

## Impact

- Nuevo documento tecnico en `docs/` como referencia de arquitectura e implementacion.
- Mayor consistencia al decidir entre vistas internas persistentes y layouts con subrutas en futuros modulos.
- Mejor integracion del flujo Jira -> OpenSpec -> Codex mediante un prompt final reutilizable para implementacion asistida.
