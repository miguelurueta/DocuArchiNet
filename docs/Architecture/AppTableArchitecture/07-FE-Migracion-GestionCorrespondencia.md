# Ticket 07 FE

## Titulo

Migrar `GestionCorrespondencia` al modelo hibrido de tabla

## Objetivo

Adoptar la arquitectura final de tabla dinámica en el primer módulo real (`GestionCorrespondencia`), usando:

- `workflowInboxgestion`
- `AppTableQueryWrapper`
- `AppTable` con `paginationMode="server"`
- `AppTableQueryState` como única fuente de verdad
- backend ya normalizado

La pantalla debe migrar a la infraestructura reusable ya construida, sin reimplementar lógica base.

## Problema actual

- `GestionCorrespondencia` aún contiene wiring propio de controles
- no consume completamente la infraestructura reusable
- existe riesgo de duplicación entre wrapper, pantalla y hook de módulo

## Alcance

- migrar la pantalla a `AppTableQueryWrapper`
- integrar `AppIconActionButton` y `AppRefreshButton` si aplica
- usar `AppTable` con `paginationMode="server"`
- usar backend ya normalizado, entendiendo por ello:
  - `Pagination.Total` real
  - `Page`
  - `PageSize`
  - empty state estructurado
  - claims reales ya restaurados
- preservar:
  - actions
  - dropdowns
  - `MenuActions`
  - pinned columns
  - empty states
  - loading states

## No alcance

- no migrar aún otros módulos
- no redefinir dominio de correspondencia
- no crear infraestructura nueva paralela a `AppTable`

## Dependencias

- Tickets 01 y 02 BE completos
- Tickets 03 a 06 FE completos

## Archivos frontend esperados

- `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx`
- `src/modules/gestionCorrespondencia/pages/GestionCorrespondenciaRoutePage.tsx`
- `src/modules/gestionCorrespondencia/hooks/useGestionCorrespondenciaTable.ts`
- `src/modules/gestionCorrespondencia/components/` si requiere refactor menor
- tests del módulo y ruta

## Contrato esperado del hook del módulo

```ts
type UseGestionCorrespondenciaTableResult<T extends AppTableRow = AppTableRow> = {
  rows: T[]
  columns: ColDef<T>[]
  total: number
  page: number
  pageSize: number
  loading: boolean
  error: Error | null
  isEmpty: boolean
  refetch: () => void
  onQueryChange: (patch: Partial<AppTableQueryState>) => void
  queryState: AppTableQueryState
}
```

## Responsabilidades por pieza

### `useGestionCorrespondenciaTable.ts`

- consumir infraestructura reusable existente
- usar `AppTableQueryState` como única fuente de verdad
- exponer datos finales para pantalla
- no llamar `clienteApi` directamente si ya existe hook o servicio reusable
- no duplicar serialización de request

### `GestionCorrespondencia.tsx`

- actuar como componente de composición
- usar `AppTableQueryWrapper + AppTable`
- no implementar barra paralela
- no serializar requests
- no duplicar action layer
- no construir adapters manuales

### `GestionCorrespondenciaRoutePage.tsx`

- preservar el patrón actual de ruta para loading, error y success
- no absorber dentro de la página principal la responsabilidad total de loading inicial si el wrapper de ruta ya la cubre
- mantener compatibilidad con el esquema actual del módulo

## Reglas de implementación

- la pantalla no debe reimplementar otra barra de tabla
- debe usar el wrapper reusable
- debe usar server mode
- refresh debe ejecutar `refetch` con el query state actual
- `page`, `pageSize`, `total` y rango visible deben ser consistentes
- `AppTableQueryState` debe ser la única fuente de verdad
- `prev / next` y `pageSize` deben operar desde el wrapper reusable
- no mantener estado paralelo de paginación, búsqueda o sort
- no romper el patrón `Outlet + Drawer`
- no romper la subruta `respuesta`

## Filtros actuales del módulo

Si la pantalla mantiene controles adicionales como `category`, debe definirse explícitamente una de estas opciones:

- integrarlo formalmente al modelo del módulo sin romper `AppTableQueryState`
- o dejarlo fuera de este ticket con comportamiento preservado pero sin mezclarlo con el query state reusable base

No debe eliminarse silenciosamente un filtro existente del módulo.

## Riesgos a evitar

- lógica duplicada entre wrapper y pantalla
- romper `MenuActions` y acciones de columna
- romper `Pinned/LockPinned`
- romper loading y empty states
- romper navegación secundaria o drawer actual
- introducir `any` en el contrato final del hook

## Pruebas obligatorias

- render completo de pantalla
- refresh
- prev / next
- cambio de page size
- búsqueda y búsqueda avanzada
- total y rango visibles
- acciones dinámicas siguen funcionando
- pinned columns siguen funcionando
- no regresión del drawer o ruta secundaria
- loading y empty state siguen consistentes con el patrón actual del módulo

## Criterios de aceptación

- `GestionCorrespondencia` adopta la arquitectura nueva
- no reimplementa infraestructura base
- `AppTableQueryState` es la única fuente de verdad
- `AppTableQueryWrapper` compone controles y tabla
- `AppTable` sigue siendo el renderer final
- se mantiene estable la experiencia actual
- no se rompe `GestionCorrespondenciaRoutePage`
- queda lista para reutilización en otros módulos

## Instrucción final

Antes de implementar:

- validar estado actual de `GestionCorrespondencia`
- validar `GestionCorrespondenciaRoutePage`
- validar integración actual con `workflowInboxgestion`
- validar filtros adicionales existentes en la pantalla

Luego:

- implementar con TypeScript estricto
- mantener separación de capas
- evitar duplicación de wiring en la pantalla

Finalmente reportar:

- decisiones de integración
- compatibilidad preservada con ruta y drawer
- estado final del hook del módulo
- filtros preservados o formalmente excluidos
- preparación para reutilización en futuros módulos
