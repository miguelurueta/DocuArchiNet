# 02 API Y Contrato Visual

## Tipos Públicos

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

## Defaults

```ts
density = "comfortable";
gap = "md";
scrollSnap = "none";
edgeFade = false;
```

## Tabla De Props

| Prop | Tipo | Default | Obligatoria | Uso |
|---|---|---:|---|---|
| `children` | `ReactNode` | N/A | Sí | Contenido que el consumidor quiere renderizar horizontalmente. |
| `ariaLabel` | `string` | N/A | Sí | Nombre accesible de la región scrolleable. |
| `className` | `string` | `undefined` | No | Clase adicional para el root. |
| `viewportClassName` | `string` | `undefined` | No | Clase adicional para el viewport scrolleable. |
| `contentClassName` | `string` | `undefined` | No | Clase adicional para el contenedor flex de items. |
| `density` | `"compact" \| "comfortable"` | `comfortable` | No | Controla padding vertical. |
| `gap` | `"xs" \| "sm" \| "md" \| "lg"` | `md` | No | Controla separación entre hijos directos. |
| `itemMinWidth` | `number \| string` | `undefined` | No | Define ancho mínimo de hijos directos. Number se convierte a px. |
| `itemMaxWidth` | `number \| string` | `undefined` | No | Define ancho máximo de hijos directos. Number se convierte a px. |
| `scrollSnap` | `"none" \| "start" \| "center"` | `none` | No | Activa snap horizontal de proximidad. |
| `edgeFade` | `boolean` | `false` | No | Activa fade visual en bordes sin bloquear interacción. |
| `testId` | `string` | `undefined` | No | Se aplica como `data-testid` en el viewport. |

## Normalización De Dimensiones

| Entrada | Resultado |
|---|---|
| `220` | `"220px"` |
| `"14rem"` | `"14rem"` |
| `"min(18rem, 80vw)"` | `"min(18rem, 80vw)"` |
| `""` | Ignorado |
| `" "` | Ignorado |
| `0` | Ignorado |
| `-1` | Ignorado |
| `Number.NaN` | Ignorado |
| `Number.POSITIVE_INFINITY` | Ignorado |
| `"-1px"` | Ignorado |
| `"-10rem"` | Ignorado |

## Contrato Visual

- El root ocupa `width: 100%` y no genera overflow horizontal de página.
- El viewport usa scroll horizontal nativo.
- El content usa una fila flex horizontal sin wrap.
- Los hijos directos usan `flex: 0 0 auto`.
- `itemMinWidth` y `itemMaxWidth` se aplican por custom properties.
- `edgeFade` es solo una ayuda visual y no comunica información crítica.

## Custom Properties

```css
--app-horizontal-scroller-item-min-width
--app-horizontal-scroller-item-max-width
--app-horizontal-scroller-fade-surface
```

`--app-horizontal-scroller-fade-surface` permite adaptar el fade al color del contenedor padre sin cambiar la implementación del componente.
