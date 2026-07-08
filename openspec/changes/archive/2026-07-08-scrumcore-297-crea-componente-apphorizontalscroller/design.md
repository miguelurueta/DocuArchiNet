## Context

SCRUMCORE-297 crea `AppHorizontalScroller`, un primitive UI puro para renderizar contenido en una fila horizontal responsive con scroll en X. El componente debe poder ser usado por futuros componentes de dominio sin conocer sus datos, endpoints, estados de carga o reglas funcionales.

El proyecto ya cuenta con componentes UI compartidos en `src/app/Components/UI/`. Este cambio debe seguir ese patron y mantenerse separado de `gestionCorrespondencia`, `AppTable` y `AppTreeTable`.

## Goals

- Crear un componente UI reusable, estable y accesible.
- Mantener el componente libre de consumo HTTP y logica de dominio.
- Soportar React 19, TypeScript y CSS Modules.
- Permitir configurar densidad, gap, ancho minimo/maximo de items, scroll snap y edge fade.
- Evitar layout shift mediante dimensiones estables.
- Documentar API, accesibilidad, responsive behavior y restricciones.

## Non-Goals

- No consumir el contrato SCRUM-162.
- No implementar servicios HTTP, hooks de carga ni estados de dominio.
- No crear cards documentales ni acciones sobre documentos.
- No implementar paginacion, filtros, busqueda, virtualizacion ni botones de navegacion.
- No modificar `AppTable`, `AppTreeTable` ni modulos de negocio.
- No agregar dependencias nuevas.

## Component API

```ts
export type AppHorizontalScrollerDensity = "compact" | "comfortable";
export type AppHorizontalScrollerGap = "xs" | "sm" | "md" | "lg";
export type AppHorizontalScrollerSnap = "none" | "start" | "center";

export interface AppHorizontalScrollerProps {
  children: React.ReactNode;
  ariaLabel: string;
  className?: string;
  viewportClassName?: string;
  contentClassName?: string;
  density?: AppHorizontalScrollerDensity;
  gap?: AppHorizontalScrollerGap;
  itemMinWidth?: number | string;
  itemMaxWidth?: number | string;
  scrollSnap?: AppHorizontalScrollerSnap;
  edgeFade?: boolean;
  testId?: string;
}
```

Defaults:

```ts
density = "comfortable";
gap = "md";
scrollSnap = "none";
edgeFade = false;
```

## Architecture

```txt
Domain consumer
  - fetches data
  - handles loading/error/empty
  - renders items
  - defines actions
        |
        | children
        v
AppHorizontalScroller
  - role region + aria-label
  - horizontal layout
  - overflow-x
  - gap/density
  - item min/max width
  - optional scroll snap
  - non-blocking edge fade
```

## Technical Decisions

1. Use a pure function component without internal state.
   - Reason: the component only solves layout and should not own workflow behavior.

2. Use CSS custom properties for item dimensions.
   - Reason: `itemMinWidth` and `itemMaxWidth` must be configurable without mutating children.

3. Use `role="region"` with required `ariaLabel`.
   - Reason: a horizontally scrollable area needs a clear accessible name.

4. Do not add `tabIndex` to the container.
   - Reason: keyboard interaction should remain with child controls unless explicit keyboard navigation is added later.

5. Implement edge fade with `pointer-events: none`.
   - Reason: visual affordance must not block buttons, links, text selection or child interaction.

6. Use `scroll-snap-type: x proximity` for snap modes.
   - Reason: `mandatory` can feel rigid in long content rails.

7. Keep the component free from Ant Design dependency.
   - Reason: it is a low-level layout primitive; consumers may render Ant Design elements inside it.

## Styling Rules

- CSS Modules only.
- No global styles.
- No hardcoded business palette.
- No decorative dominant gradients.
- No internal cards.
- No nested cards.
- No `position: fixed`.
- Use `box-sizing: border-box`, `max-width: 100%` and `min-width: 0`.
- Use native `overflow-x: auto`.
- Use `-webkit-overflow-scrolling: touch`.
- Respect `prefers-reduced-motion` if smooth behavior is introduced.

## Dimension Normalization

`itemMinWidth` and `itemMaxWidth` accept `number | string`.

- Positive numbers are converted to px.
- Non-empty strings are passed through.
- Empty strings, zero, negative numbers, `NaN` and infinities are ignored.
- Children are not cloned or mutated.

## Risks / Trade-offs

- Native scrollbars differ across browsers.
  - Mitigation: rely on native scrolling and avoid custom scrollbar logic.

- Edge fade can interfere with interaction if implemented as overlay incorrectly.
  - Mitigation: enforce `pointer-events: none`.

- Consumers may expect navigation buttons.
  - Mitigation: document prev/next buttons as out of scope for this first primitive.

- Item width rules apply to direct children.
  - Mitigation: document direct-child layout contract and test custom properties.

## Migration Plan

1. Add `AppHorizontalScroller` as a new isolated component.
2. Add tests and documentation.
3. Export from component index if local pattern requires it.
4. Leave future domain integration, including SCRUM-162, to a separate ticket.

## Open Questions

- None blocking. If the component barrel strategy differs from nearby UI components, follow the local folder pattern during implementation.
