# PROMPT ARQUITECTÓNICO
Implementar Skeleton Screen reusable para `AppTable`

## Rol esperado

Arquitecto de software senior y desarrollador frontend React
(React 19 + TypeScript estricto + arquitectura enterprise)

## Objetivo

Incorporar un `Skeleton Screen` reusable en `AppTable` para mejorar la experiencia de carga inicial, tanto en la vista tabular como en la vista cards, evitando overlays duros o pantallas vacías durante el primer render.

## Problema actual

Hoy `AppTable` depende principalmente de overlays o estados simples de carga.
Eso genera una experiencia menos refinada cuando:

- la tabla carga por primera vez
- el usuario entra a una pantalla y todavía no hay datos
- el grid o la vista cards todavía no tienen contenido útil para mostrar

Además, ya existe un esfuerzo previo para:
- preservar datos previos durante refetch
- evitar parpadeos
- alinear layout y presentación responsive

Falta completar esa UX con una estrategia de skeleton coherente.

## Objetivo funcional

`AppTable` debe poder mostrar un skeleton reusable cuando:

- está cargando por primera vez
- todavía no tiene filas renderizables

Y debe evitar reemplazar contenido útil ya visible durante refetch.

## Reglas funcionales

### Mostrar skeleton cuando:
- `loading === true`
- y no hay datos previos útiles

### No mostrar skeleton cuando:
- hay filas ya visibles
- el estado actual es un refetch corto
- existe un empty state real
- existe un error

## Comportamiento esperado

### Caso 1. First load sin datos
- mostrar skeleton
- no mostrar overlay duro ni tabla vacía

### Caso 2. Refetch con datos previos
- mantener tabla o cards visibles
- no reemplazarlas con skeleton

### Caso 3. Empty state real
- mostrar estado vacío
- no skeleton

### Caso 4. Error
- mantener tratamiento actual
- no skeleton como sustituto de error

## Alcance

- skeleton reusable para `presentationMode="table"`
- skeleton reusable para `presentationMode="cards"`
- comportamiento shared en `AppTable`
- coherencia visual con tipografía y layout ya estandarizados

## No alcance

- no rediseñar backend
- no mezclar con paginación o filtros
- no usar skeleton para tapar refetch con datos visibles
- no resolver aquí estados de error

## Contrato sugerido

```ts
loadingMode?: "overlay" | "skeleton"
```

O equivalente, con una semántica clara.

Recomendación:
- default `skeleton` para first load
- fallback configurable a overlay si una pantalla lo necesita

## Arquitectura sugerida

Separar renderers de carga:

- `AppTableGridSkeleton`
- `AppTableCardSkeleton`

Y dejar a `AppTable` como orquestador de:
- `loading`
- `rows`
- `presentationMode`
- `layoutMode`

## Criterios de decisión

Si:
- `loading && !hasRenderableRows`
  - skeleton

Si:
- `loading && hasRenderableRows`
  - mantener contenido actual

Si:
- `!loading && empty`
  - empty state

## Archivos esperados

- `src/app/Components/UI/AppTable/AppTable.tsx`
- `src/app/Components/UI/AppTable/AppTable.types.ts`
- `src/app/Components/UI/AppTable/renderers/AppTableGridRenderer.tsx`
- `src/app/Components/UI/AppTable/renderers/AppTableCardRenderer.tsx`
- nuevos componentes skeleton si aplica
- pruebas de `AppTable`

## Riesgos a evitar

- mostrar skeleton durante refetch con datos visibles
- duplicar lógica de loading en pantallas consumidoras
- crear skeleton acoplado a una pantalla concreta
- romper empty state real
- romper `presentationMode="cards"`

## Pruebas obligatorias

- first load sin datos muestra skeleton
- refetch con datos previos no reemplaza contenido por skeleton
- `cards` también soporta skeleton
- empty state real sigue funcionando
- error no se confunde con skeleton

## Criterios de aceptación

- `AppTable` tiene skeleton reusable
- aplica a tabla y cards
- mejora carga inicial sin degradar refetch
- no rompe empty state ni error state
- queda como comportamiento shared del componente

## Conclusión

Sí, este trabajo tiene valor y encaja bien en la evolución actual de `AppTable`.
Debe ir como ticket separado y reusable, no como ajuste ad hoc de una sola pantalla.
