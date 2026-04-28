# Design: SCRUMCORE-176 (AppEditorPdf - Auditoria y Optimizacion de Rerender)

## Goal

Reducir rerenders y recomputos innecesarios en `AppEditorPdf` sin tocar `AppEditor`.

## Hotspots Probables

- `metrics` recalculadas por dependencias amplias.
- `toolbarActions` recomposicion en cada render.
- `visualGuides` normalizacion y `style` objects recreados.
- Cambios en `data-*` attributes que no aportan a UI.

## Strategy

### Tight Dependencies

- Revisar `useMemo`/`useCallback` y acotar dependencias.
- Evitar crear objects inline en JSX cuando no es necesario.

### Stable Styles

- Mover variables CSS a un solo `style` memoizado:
  - zoom
  - guide inset

### Callback Hygiene

- `handlePageContextChange` y `handlePageBreakCommandReady` ya son callbacks; validar que no cambian por props irrelevantes.

## Instrumentation (Dev Only)

- Incluir `data-render-count` opcional (solo cuando `debugRenders` true) para ayudar QA, sin afectar prod.

## Non-Goals

- Cambiar engine, estado interno del editor, o infra de paginacion de `AppEditor`.

