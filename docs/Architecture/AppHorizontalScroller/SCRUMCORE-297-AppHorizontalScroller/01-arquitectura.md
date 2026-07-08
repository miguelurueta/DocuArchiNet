# 01 Arquitectura

## Objetivo

Crear `AppHorizontalScroller` como primitive UI reutilizable para renderizar contenido en una fila horizontal responsive con scroll en X. El componente permite construir rails/banners horizontales para accesos rápidos, tarjetas resumidas, colecciones compactas o futuros listados documentales sin acoplarse a reglas de negocio.

## Alcance

- Componente compartido en `src/app/Components/UI/AppHorizontalScroller/`.
- Implementación con React 19, TypeScript y CSS Modules.
- API tipada para densidad, separación, ancho mínimo/máximo de items, scroll snap y edge fade.
- Región accesible con `role="region"` y `aria-label`.
- Pruebas unitarias con React Testing Library.

## No Objetivos

- No consumir APIs internas o externas.
- No usar `axios`, `fetch`, servicios HTTP ni hooks de dominio.
- No integrar `GestionCorrespondencia`.
- No implementar SCRUM-162.
- No crear cards documentales, visor, descarga, búsqueda, filtros, paginación, virtualización ni botones prev/next.
- No modificar `AppTable` ni `AppTreeTable`.
- No agregar dependencias nuevas.

## Mapa De Archivos

```txt
src/app/Components/UI/AppHorizontalScroller/
├── AppHorizontalScroller.tsx
├── AppHorizontalScroller.module.css
├── AppHorizontalScroller.test.tsx
└── index.ts

src/app/Components/UI/index.ts
└── export * from "./AppHorizontalScroller";

docs/Architecture/AppHorizontalScroller/SCRUMCORE-297-AppHorizontalScroller/
├── 00-indice.md
├── 01-arquitectura.md
├── 02-api-contrato-visual.md
├── 03-responsive-accesibilidad-css.md
├── 04-uso-e-integracion.md
├── 05-pruebas-validacion.md
└── 06-riesgos-checklist.md
```

## Responsabilidad Por Archivo

| Archivo | Responsabilidad |
|---|---|
| `AppHorizontalScroller.tsx` | API pública, normalización de dimensiones, composición DOM y clases. |
| `AppHorizontalScroller.module.css` | Layout horizontal, overflow, densidad, gap, snap y edge fade. |
| `AppHorizontalScroller.test.tsx` | Contrato de render, accesibilidad, variantes, dimensiones, snap y defensas. |
| `index.ts` | Export del componente y tipos públicos. |
| `src/app/Components/UI/index.ts` | Barrel compartido para consumidores que importan desde UI. |

## Composición General

```txt
Consumidor de dominio
  - obtiene datos
  - maneja loading/error/empty
  - renderiza items
  - define acciones
        |
        | children
        v
AppHorizontalScroller
  - role region + aria-label
  - layout horizontal
  - overflow-x
  - gap/density
  - item min/max width
  - scroll snap opcional
  - edge fade no bloqueante
```

`AppHorizontalScroller` no conoce endpoints, estados remotos, DTOs ni reglas funcionales. El consumidor es responsable de obtener datos y componer los hijos.

## Estructura DOM

```txt
AppHorizontalScroller
└── div.root
    └── div.viewport
        ├── role="region"
        ├── aria-label={ariaLabel}
        ├── data-testid={testId}
        └── div.content
            ├── style custom properties
            └── children
```

## Flujo De Layout

```txt
Props
  ├── density ───────────────> class densityCompact|densityComfortable
  ├── gap ───────────────────> class gapXS|gapSM|gapMD|gapLG
  ├── scrollSnap ────────────> snap + snapStart|snapCenter
  ├── edgeFade ──────────────> root edgeFade pseudo-elements
  ├── itemMinWidth ──────────> --app-horizontal-scroller-item-min-width
  └── itemMaxWidth ──────────> --app-horizontal-scroller-item-max-width
                                      |
                                      v
                              .content > * stable width
```
