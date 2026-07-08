## Why

SCRUMCORE-297 requiere crear `AppHorizontalScroller` como primitive UI reutilizable para presentar colecciones horizontales responsive con scroll en X. Hoy el proyecto tiene componentes ricos para tablas, tabs, toolbars, inputs y contenedores, pero no existe un componente base desacoplado para rails/banners horizontales.

Separar este primitive evita que futuros consumidores, como SCRUM-162, acoplen layout horizontal a servicios HTTP, reglas de dominio o componentes tabulares.

## What Changes

- Crear el componente `AppHorizontalScroller` en la capa UI compartida.
- Exponer una API tipada para densidad, separacion, ancho minimo/maximo de items, scroll snap opcional y edge fade no bloqueante.
- Implementar estilos con CSS Modules, sin estilos globales ni dependencias nuevas.
- Agregar pruebas unitarias con React Testing Library.
- Documentar el componente en `docs/Architecture/AppHorizontalScroller/SCRUMCORE-297-AppHorizontalScroller.md`.

## Scope

Incluye:

- `src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.tsx`
- `src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.module.css`
- `src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.test.tsx`
- `src/app/Components/UI/AppHorizontalScroller/index.ts`
- Export barrel solo si el patron local lo requiere.
- Documentacion enterprise del primitive.

No incluye:

- Consumo de APIs internas o externas.
- `axios`, `fetch`, servicios HTTP o hooks de dominio.
- Integracion con `GestionCorrespondencia`.
- Integracion con SCRUM-162.
- Paginacion, busqueda, filtros, virtualizacion o botones prev/next.
- Cambios en `AppTable` o `AppTreeTable`.

## Jira Details

El ticket solicita un componente UI base `AppHorizontalScroller`, implementado con React 19, TypeScript y CSS Modules, sin acoplamiento a negocio ni consumo HTTP. Debe servir como base para listados tipo rail/banner y quedar listo para futuros consumidores.

## Capabilities

### New Capabilities

- `app-horizontal-scroller`: Primitive UI reusable para layout horizontal con overflow en X, accesibilidad base y controles visuales de densidad/ancho/snap/fade.

### Modified Capabilities

- Ninguna.

## Impact

- Nuevo componente compartido en `src/app/Components/UI/AppHorizontalScroller/`.
- Nueva documentacion enterprise en `docs/Architecture/AppHorizontalScroller/`.
- Nuevas pruebas unitarias enfocadas al contrato reusable.
- Sin impacto funcional sobre modulos de negocio existentes.
