# SCRUMCORE-214 — Comportamiento del Componente

## AppTreeTable

- Renderiza un listado jerárquico (árbol) con soporte de expand/collapse.
- Soporta dos modos:
  - `rows`: render directo desde props.
  - `load()`: carga backend-driven con estados `loading`, `empty`, `error`.
- En error (modo `load()`), muestra botón **Reintentar** (si `isRetryEnabled === true`).

## Integración en DocumentosWorkbench

- Se integra en:
  - `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`
- Ubicación:
  - Sección **Listado** dentro del `AppCollapseRail`.
- Reglas:
  - No afecta el comportamiento del visor PDF (`AppVisorEmbedPdf`).
  - No dispara navegación ni modifica estado global.

