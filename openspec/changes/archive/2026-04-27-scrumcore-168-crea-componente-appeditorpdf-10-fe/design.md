## Context

El ticket `SCRUMCORE-168` introduce la capability `app-appeditorpdf-10-fe` con foco en **toolbar responsive** para `AppEditorPdf` y con la restriccion explicita de **no exponer un toggle de tema** como parte del contrato reusable.

El repositorio ya cuenta con:

- `AppEditorToolbar` con modo compacto (`data-toolbar-mode="compact"`) que agrupa acciones cuando el ancho es reducido.
- CSS de toolbar con `flex-wrap` y secciones con wrapping, para evitar overflow duro.
- `AppEditor` soporta `themeMode`/`defaultThemeMode` via `data-theme`, pero actualmente no renderiza un control UI de toggle de tema en la toolbar.
- `AppEditorPdf` compone `toolbarActions` y mantiene acciones opcionales (p.ej. FE-09 `showPageBreakAction`) sin obligar a los consumidores.

La brecha observada para FE-10 es garantizar que el comportamiento responsive aplique de forma consistente en contenedores embebidos (no solo por `window.innerWidth`) y dejar el contrato claro: **sin toggle de tema por defecto**, pero con compatibilidad para que el consumidor controle el tema externamente.

## Goals / Non-Goals

**Goals**

- Mantener una toolbar usable en anchos pequenos (wrap/compact/overflow controlado).
- No introducir un control de cambio de tema en `AppEditorPdf` por defecto.
- Preservar compatibilidad con configuracion de tema via props (patron `themeMode` / `defaultThemeMode` del editor base).
- Mantener composicion con `toolbarActions` externos y acciones built-in opcionales (ej. FE-09).

**Non-Goals**

- Redisenar completo de la toolbar o migracion de UI kit.
- Implementar un sistema global de theming o persistencia de preferencias.
- Cambiar el conjunto de acciones de la toolbar (solo su adaptacion/responsividad).

## Decisions

1. La responsividad se implementa en la capa `AppEditorToolbar`, sin duplicarla en `AppEditorPdf`.
Rationale: `AppEditorPdf` delega la superficie de edicion a `AppEditor`; la toolbar pertenece a esa capa y cualquier mejora debe beneficiar a ambos consumidores.

2. El modo compacto debe decidirse por ancho real del contenedor (idealmente via `ResizeObserver`) y no solo por `window.innerWidth`.
Rationale: `AppEditorPdf` puede renderizarse dentro de layouts con sidebars, modales o paneles; el viewport no siempre refleja el ancho disponible para la toolbar.

3. No se introduce `ThemeToggle` en la toolbar por defecto.
Rationale: el ticket lo prohibe y los consumidores pueden controlar `themeMode` externamente si lo necesitan.

4. La composicion de acciones permanece: `toolbarActions` del consumidor + acciones opcionales internas (como FE-09) deben convivir y seguir siendo responsive.
Rationale: evita breaking changes y mantiene adopcion incremental de capacidades.

## Implementation Sketch

- Ajustar el detector de modo compacto de `AppEditorToolbar` para basarse en `ResizeObserver` sobre el root de toolbar, con fallback a `window.innerWidth` cuando el observer no este disponible.
- Mantener el contrato de `AppEditorPdf` sin exponer ninguna prop nueva para toggle de tema; en caso de existir `showThemeToggle` en tipos del editor base, se mantiene como no utilizado/false por defecto.
- Agregar pruebas que verifiquen:
  - ausencia de toggle de tema en toolbar por defecto (smoke),
  - el modo compacto se activa cuando el contenedor simula un ancho reducido (mock de `ResizeObserver`).

## Risks / Trade-offs

- [Riesgo] `ResizeObserver` no disponible en entornos de test/SSR.
Mitigacion: fallback a `window` y mocks de test controlados.

- [Riesgo] Cambios en modo compacto afecten snapshots/queries en tests existentes.
Mitigacion: pruebas focales a nivel de contract (atributos `data-toolbar-mode` y presencia/ausencia de elementos clave), no a layout pixel-perfect.

