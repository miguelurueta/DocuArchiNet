# SCRUMCORE-208 — Testing Enterprise

## Unit / Integration (Vitest + RTL)
- Se mockea `@embedpdf/plugin-scroll/react`:
  - `state.currentPage`, `state.totalPages`
  - `provides.scrollToPreviousPage`, `provides.scrollToNextPage`
- Escenarios:
  - Render indicador `X/Y` y `aria-label` `Página X de Y`.
  - Click `Página anterior` llama `scrollToPreviousPage`.
  - Click `Página siguiente` llama `scrollToNextPage`.
  - `provides = null` no produce crash.

## E2E (Playwright)
- Opcional según pipeline: smoke navegando la ruta de Playwright del visor.

