# SCRUMCORE-216 - Proposal

## Why
`AppTreeTable` hoy funciona con logica/render propio y no reutiliza el motor maduro de `AppTable`, lo que incrementa el riesgo de divergencia UX, deuda tecnica y dificulta la evolucion hacia una UI dinamica backend-driven.

Esta tarea estandariza el arbol sobre `AppTable` para mejorar consistencia, mantenibilidad y preparar compatibilidad futura sin romper consumidores actuales.

## What Changes
- Refactorizar `AppTreeTable` para que use `AppTable` internamente como engine (render, eventos, acciones, estados base, accesibilidad).
- Mantener 100% compatibilidad con la API publica actual de `AppTreeTable` (`rows`, `load`, `loadChildren`, `onSelectRow`, `emptyMessage`, `isRetryEnabled`, `className`, etc.).
- Introducir estructura interna por capas (hooks/adapters/types) para:
  - Flattening del arbol (Tree -> Table rows).
  - Estado de expansion local.
  - Calculo de filas visibles e indentacion.
- Mantener estados legacy (loading / empty / error / retry) sin cambiar mensajes en espanol.
- Dejar listo el punto de extension para evolucion futura (Dynamic UI / metadata / acciones), sin implementar integracion backend-driven adicional en este ticket.

## Capabilities
### New Capabilities
- `actualizacion-componente-apptreetable` (refactorizacion enterprise de `AppTreeTable` como wrapper/adaptador sobre `AppTable`).

### Modified Capabilities
- (vacio)

## Impact
- UI Shared: `src/app/Components/UI/AppTreeTable/*` (component, styles, types, nuevos hooks/adapters).
- Dependencias: `AppTable` (engine base), AG Grid (via `AppTable`).
- Consumers: `DocumentosWorkbench` y cualquier consumidor de `AppTreeTable` (sin cambios requeridos por compatibilidad).
- Testing: Vitest/RTL: unit tests (adapters/hooks) + integracion `AppTreeTable -> AppTable` + smoke en `DocumentosWorkbench`.

