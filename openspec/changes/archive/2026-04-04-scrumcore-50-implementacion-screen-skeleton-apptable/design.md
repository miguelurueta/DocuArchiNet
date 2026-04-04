# Design

## Summary

`AppTable` debe incorporar un `Skeleton Screen` reusable para mejorar la carga inicial tanto en `presentationMode="table"` como en `presentationMode="cards"`, sin degradar la experiencia de refetch cuando ya existen datos visibles.

## Decisions

### 1. El skeleton vive en la capa shared de `AppTable`

La responsabilidad de decidir entre skeleton, overlay, empty state o contenido visible debe quedar en `AppTable`, no distribuida en pantallas consumidoras.

Consecuencia:
- una pantalla no necesita implementar un loading placeholder propio para primera carga
- el comportamiento queda consistente entre grids y cards

### 2. El skeleton solo aplica a first load sin datos renderizables

La decisión de mostrar skeleton debe basarse en:
- `loading === true`
- no existen filas renderizables previas

No debe aplicarse cuando:
- hay datos previos visibles
- el estado es un refetch
- existe empty state real

### 3. Soporte explícito para `table` y `cards`

Se deben crear renderers de skeleton separados para conservar coherencia visual con cada presentation mode:
- `AppTableGridSkeleton`
- `AppTableCardSkeleton`

`AppTable` decide cuál usar según `presentationMode`.

### 4. Contrato configurable de modo de carga

Se agrega una opción shared, por ejemplo:

```ts
loadingMode?: "overlay" | "skeleton"
```

Regla recomendada:
- `skeleton` como default para first load
- `overlay` como override explícito si una pantalla lo requiere

### 5. No reemplazar contenido útil durante refetch

Cuando el usuario ya tiene filas visibles y se dispara un nuevo fetch:
- se mantiene el contenido actual
- no se reemplaza por skeleton
- la señal de loading sigue siendo no destructiva

Esto preserva la mejora introducida en el ticket de parpadeo de server pagination.

## Rendering Rules

### Caso A. First load sin filas
- mostrar skeleton
- no mostrar tabla vacía
- no mostrar overlay de loading duro

### Caso B. Refetch con filas previas
- mantener tabla o cards actuales
- no skeleton

### Caso C. Empty state real
- mostrar empty state existente
- no skeleton

### Caso D. Error
- mantener manejo de error existente
- no sustituir error por skeleton

## Affected Areas

- `src/app/Components/UI/AppTable/AppTable.tsx`
- `src/app/Components/UI/AppTable/AppTable.types.ts`
- `src/app/Components/UI/AppTable/renderers/AppTableGridRenderer.tsx`
- `src/app/Components/UI/AppTable/renderers/AppTableCardRenderer.tsx`
- nuevos renderers o componentes skeleton shared
- pruebas de `AppTable`

## Risks

- mostrar skeleton en refetch con datos visibles
- duplicar lógica de loading entre `AppTable` y consumidores
- romper la distinción entre loading, empty y error
- dejar cards con un skeleton distinto o inconsistente respecto a table

## Validation Strategy

Se debe cubrir al menos:
- first load sin datos muestra skeleton
- refetch con datos visibles no usa skeleton
- `presentationMode="cards"` soporta skeleton
- empty state real sigue funcionando
- error state no se reemplaza por skeleton
