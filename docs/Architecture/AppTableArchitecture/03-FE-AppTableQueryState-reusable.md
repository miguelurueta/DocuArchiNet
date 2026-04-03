# Ticket 03 FE

## Titulo

Crear `AppTableQueryState` reusable para tablas dinamicas

## Objetivo

Implementar un modelo centralizado, reusable y tipado de estado de consulta para tablas dinamicas (`AppTableQueryState`) que unifique:

- paginacion
- busqueda simple
- busqueda avanzada
- ordenamiento

Evitar duplicacion de lógica por pantalla y garantizar consistencia en:

- resets de pagina
- serializacion hacia backend
- consumo por hooks reutilizables

## Contexto existente

Ya existe:

- infraestructura de tablas dinamicas (`AppTable`)
- adapters y query layer (`useDynamicUiTableQuery`)
- contrato backend estable para paginacion y filtros estructurados

Problema actual:

- cada pantalla puede manejar su propio estado
- riesgo de inconsistencias y regresiones
- lógica de reset duplicada
- serializacion del request no centralizada

## Restricciones (obligatorio)

- no acoplar a ninguna pantalla
- no duplicar lógica en componentes visuales
- no usar `unknown[]` como contrato final de filtros
- no introducir lógica de dominio
- no romper contratos existentes de query layer
- no incluir `refresh` dentro del state
- no usar `any`

## Ubicación (obligatoria)

```txt
src/app/Components/UI/AppTable/
  types/
  hooks/
  utils/
  tests/
```

## Contratos (obligatorios)

### Search type

Si el backend mantiene los valores reales `1 | 2 | 3`, el frontend debe modelarlos explícitamente:

```ts
type AppTableSearchType = 1 | 2 | 3
```

### Filtros estructurados

El contrato interno debe poder representar completamente lo que backend ya soporta, incluyendo `between`.

```ts
type AppTableStructuredFilter = {
  field: string
  operator:
    | "eq"
    | "neq"
    | "contains"
    | "startsWith"
    | "endsWith"
    | "gt"
    | "gte"
    | "lt"
    | "lte"
    | "between"
    | "isNull"
    | "isNotNull"
  value?: unknown
  valueFrom?: unknown
  valueTo?: unknown
}
```

### Query state

```ts
type AppTableQueryState = {
  page: number
  pageSize: number
  search: string
  searchType?: AppTableSearchType
  structuredFilters: AppTableStructuredFilter[]
  sortField?: string
  sortDir?: "asc" | "desc"
}
```

## Valores por defecto (obligatorio)

```ts
page = 1
pageSize = 25
search = ""
structuredFilters = []
sortField = undefined
sortDir = undefined
searchType = undefined
```

## Helpers y hook mínimos (obligatorio)

Debe implementarse exactamente:

```ts
getDefaultAppTableQueryState(): AppTableQueryState

updateAppTableQueryState(
  prev: AppTableQueryState,
  patch: Partial<AppTableQueryState>
): AppTableQueryState

serializeAppTableQueryState(
  state: AppTableQueryState
): Record<string, unknown>

useAppTableQueryState(
  initialState?: Partial<AppTableQueryState>
)
```

## Reglas de implementación (obligatorio)

### Reglas de reset de página

El sistema debe resetear `page = 1` cuando cambie efectivamente:

- `search`
- `searchType`
- `structuredFilters`
- `sortField`
- `sortDir`
- `pageSize`

### Reglas adicionales

- cambios en `page` no afectan otros campos
- `refresh` no forma parte del state
- `onRefresh` no modifica el state
- la comparación debe ser por valor efectivo, no solo por referencia
- arrays y objetos deben evaluarse correctamente para evitar resets espurios

## Serialización (obligatorio)

- la transformación del state a request backend debe ser centralizada
- ningún módulo debe serializar manualmente
- el resultado debe ser compatible con `useDynamicUiTableQuery`
- debe existir capa de adaptación
- no acoplar el state interno a una pantalla específica
- pero sí mantener compatibilidad semántica completa con backend:
  - `page`
  - `pageSize`
  - `search`
  - `searchType`
  - `structuredFilters`
  - `sortField`
  - `sortDir`

## Inmutabilidad (obligatorio)

- no mutar `prev`
- siempre retornar nuevo objeto
- helpers deben ser funciones puras

## Riesgos a evitar

- estados paralelos por módulo
- lógica de reset en UI
- serialización duplicada
- inconsistencias entre pantallas
- resets incorrectos por comparación por referencia
- contrato de filtros incompleto para operadores soportados por backend

## Pruebas (obligatorio)

Cubrir mínimo:

- reset de página al cambiar `search`
- no resetear si `search` no cambia efectivamente
- reset de página al cambiar `structuredFilters`
- reset de página al cambiar `sort`
- reset de página al cambiar `pageSize`
- no resetear en `refresh`
- serialización correcta del query state
- serialización correcta de filtros `between`
- persistencia de estado al cambiar solo `page`
- valores por defecto correctos

## Criterios de aceptación

- existe un query state reusable y centralizado
- las reglas de reset están encapsuladas, no en UI
- la serialización está centralizada
- el modelo es consumible por hooks y wrappers
- ningún módulo reimplementa lógica base
- no existe acoplamiento a pantalla
- el contrato de filtros representa correctamente lo soportado por backend
