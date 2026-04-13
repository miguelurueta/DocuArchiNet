# Ticket 03 FE

## Titulo

Integracion avanzada, accesibilidad y pruebas de `AppCollapseRail`

## Objetivo

Completar accesibilidad avanzada, helpers de integracion y cobertura de tests
para garantizar un componente enterprise-ready.

## Contexto existente

- Arquitectura: `docs/Architecture/AppCollapseRail/AppCollapseRail-Architecture.md`
- Implementacion core en `src/app/Components/UI/AppCollapseRail/`

## Restricciones (obligatorio)

- No romper API del core
- No agregar dependencias extra
- Mantener tipado estricto

## Reglas de accesibilidad (obligatorio)

- `aria-label` obligatorio en panel.
- `aria-expanded` + `aria-controls` en toggle.
- `tabIndex` correcto en rail button.
- Focus visible en header toggle y rail button.

## Integracion (obligatorio)

- Exportar desde `src/app/Components/UI/index.ts`.
- README breve en `src/app/Components/UI/AppCollapseRail/README.md`
  con ejemplos de uso.

## Pruebas obligatorias

- Render basico con titulo y children.
- Toggle cambia `aria-expanded`.
- Rail aparece al colapsar.
- `aria-controls` conecta panel con toggle.
- Responsive: clases para `variant="overlay"` activas en mobile.

## Criterios de aceptacion

- Tests pasan en Vitest.
- README incluye ejemplo desktop y mobile.
- Accesibilidad validada.
