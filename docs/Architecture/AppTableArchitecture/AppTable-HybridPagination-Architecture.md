# Arquitectura Maestra: AppTable con Paginacion Hibrida y Busqueda Avanzada

## Objetivo

Definir una arquitectura reusable para evolucionar `AppTable` hacia un modelo de consulta de tabla que soporte, sin romper compatibilidad:

- paginacion cliente nativa de AG Grid
- paginacion servidor tipo Gmail
- busqueda simple local
- busqueda simple servidor
- busqueda avanzada servidor
- refresco manual de la consulta
- integracion visual reusable entre controles y tabla

Este documento busca reducir ambiguedad y servir como fuente unica de verdad para:

- prompts de IA
- tickets Jira
- implementacion frontend
- implementacion backend
- pruebas de regresion

## Alcance

Aplica a:

- `AppTable`
- tablas dinamicas basadas en `DynamicUiTableDto`
- `workflowInboxgestion`
- futuros modulos que reutilicen la misma infraestructura

No aplica a:

- rediseño visual general del sistema
- cambios de negocio especificos de un modulo
- reemplazo de AG Grid

## Estado actual

### Frontend

`AppTable` es un wrapper de AG Grid y hoy:

- renderiza `AgGridReact`
- soporta `ColDef[]`
- soporta seleccion, loading, overlays y actions dinamicas
- no trae toolbar propia de busqueda o paginacion
- no soporta aun una barra reusable tipo Gmail

La pantalla de `GestionCorrespondencia` ya tiene:

- input de busqueda
- selector de page size
- total visible
- boton `Actualizar`

Pero esos controles viven en la pagina, no en una capa reusable acoplada a la tabla.

### Backend

El endpoint `POST /api/workflowInboxgestion/inboxgestion` ya soporta parcialmente:

- `Page`
- `PageSize`
- `Search`
- `SearchType`
- `StructuredFilters`
- `SortField`
- `SortDir`
- `Pinned`
- `LockPinned`
- `MenuActions`
- `Children`
- `IsDivider`

Problemas detectados:

- el controller de `workflowInboxgestion` sigue hardcodeado
- `Pagination.Total` no es el total global real de la consulta
- el backend retorna total basado en `rows.Count`
- la paginacion servidor no esta lista para una UX tipo Gmail real
- la respuesta vacia no siempre mantiene una estructura de tabla completa y consistente

## Problema a resolver

Se requiere un modelo de tabla que permita coexistencia entre dos modos:

1. `client`
   - usa paginacion local de AG Grid
   - util para datasets pequeños o pantallas sin backend listo

2. `server`
   - usa paginacion tipo Gmail
   - depende de `page`, `pageSize`, `total`
   - debe convivir con busqueda avanzada y ordenamiento

La implementacion debe evitar:

- duplicacion de lógica por modulo
- mezclar paginacion cliente y servidor simultaneamente
- desacoplar `search`, `structuredFilters`, `sort` y `pagination`
- recalcular totals inconsistentes con la consulta activa

## Principios de arquitectura

### 1. AppTable sigue siendo renderer base

`AppTable` debe seguir siendo el wrapper base de AG Grid.

Responsabilidades de `AppTable`:

- renderizar grid
- exponer configuracion reusable
- soportar modos de paginacion
- soportar quick filter local si aplica

No debe asumir por defecto:

- toolbar fija
- layout completo de pagina
- buscador avanzado acoplado
- experiencia Gmail embebida de forma obligatoria

### 2. La experiencia tipo Gmail debe vivir en un wrapper/contenedor reusable

Se debe crear un componente superior, reutilizable, por ejemplo:

- `AppTableQueryWrapper`
- o `AppTableDataContainer`
- o `AppTableShell`

Este wrapper debe acoplar visualmente:

- busqueda
- refresh
- total/rango
- selector de page size
- navegacion anterior/siguiente
- tabla renderizada

### 3. La consulta debe modelarse como un estado unico

Para `server mode`, la tabla debe manejar un query state unificado:

```ts
type AppTableSearchType = 1 | 2 | 3;

type AppTableStructuredFilter = {
  field: string;
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
    | "isNotNull";
  value?: unknown;
  valueFrom?: unknown;
  valueTo?: unknown;
};

type AppTableQueryState = {
  page: number;
  pageSize: number;
  search: string;
  searchType?: AppTableSearchType;
  structuredFilters: AppTableStructuredFilter[];
  sortField?: string;
  sortDir?: "asc" | "desc";
};
```

Toda consulta backend debe derivarse de ese estado.

Regla:

- al cambiar `search`, `searchType`, `structuredFilters`, `sortField` o `sortDir`
- se debe resetear `page = 1`
- se conserva `pageSize`

## Arquitectura objetivo

```txt
Backend
  -> DynamicUiTableDto
  -> useDynamicUiTableQuery / hooks del modulo
  -> AppTableQueryWrapper
      -> barra de busqueda y acciones
      -> barra de paginacion tipo Gmail
      -> AppTable
          -> AG Grid
```

## Diseño frontend

### AppTable

Se recomienda evolucionar `AppTable` con estas capacidades:

```ts
type AppTablePaginationMode = "none" | "client" | "server";
```

Props nuevas sugeridas:

```ts
paginationMode?: AppTablePaginationMode;
quickFilterText?: string;
clientPaginationPageSize?: number;
```

Comportamiento:

- `none`
  - sin paginacion

- `client`
  - usa la paginacion nativa de AG Grid
  - usa `pagination: true`
  - usa `paginationPageSize`

- `server`
  - no depende de la paginacion interna del grid
  - recibe ya el subconjunto de filas de la pagina actual
  - la barra externa controla la consulta

### Wrapper reusable

Se recomienda crear un componente tipo:

`AppTableQueryWrapper`

Responsabilidades:

- renderizar un bloque visual unico que contenga controles + tabla
- mostrar:
  - input de busqueda
  - boton actualizar
  - selector de page size
  - rango visible tipo Gmail
  - botones anterior / siguiente
- aceptar children o renderizar internamente `AppTable`
- ajustarse visualmente con la tabla en un mismo contenedor

Posible contrato:

```ts
type AppTableQueryWrapperProps = {
  queryState: AppTableQueryState;
  onQueryChange: (patch: Partial<AppTableQueryState>) => void;
  onRefresh?: () => void;
  total: number;
  loading?: boolean;
  headerActions?: ReactNode;
  children: ReactNode;
};
```

### Contenedor visual

Se recomienda que el wrapper tenga un contenedor acoplado, por ejemplo:

- `headerControls`
- `paginationStrip`
- `tableBody`

Objetivo:

- que la UX se vea como una sola pieza
- no como toolbar suelta y tabla desconectada

### Refresh Button

Los controles accionables iconograficos asociados a tabla deben reutilizar `AppButton` como base del sistema UI.

Reglas:

- no crear un boton aislado fuera del sistema de componentes
- crear una base reusable oficial:
  - `AppIconActionButton`
- especializaciones permitidas:
  - `AppRefreshButton`
  - `AppTableCellActionButton`
  - `AppToolbarActionButton`
- debe soportar:
  - icono
  - `loading`
  - `disabled`
  - `aria-label`
  - tooltip opcional

### Lineamiento visual del refresh

La base reusable de acciones iconograficas y sus especializaciones deben aproximarse al patron visual de Gmail:

- apariencia ligera
- tamaño compacto
- prioridad visual baja o media
- apto para toolbar de tabla
- consistente con el sistema visual existente

No se recomienda usarlo como boton primario pesado.

### Integracion con AppDropdown

Cuando una accion dinamica use `Presentation = icon_button`, el trigger visual estandar debe reutilizar la misma base `AppIconActionButton`, incluso si la accion abre un `AppDropdown`.

Reglas:

- no crear triggers iconograficos paralelos para dropdowns de tabla
- `AppDropdown` puede recibir como trigger:
  - `AppIconActionButton`
  - o una especializacion directa basada en esa misma pieza
- el renderer de acciones de celda debe mantener consistencia visual entre:
  - refresh
  - accion directa
  - accion que abre dropdown

Objetivo:

- una sola familia visual para acciones compactas de tabla
- menor duplicacion de estilos y estados
- consistencia entre toolbar, celdas y menus contextuales

### Refresh

El control de actualizar debe tener una unica semantica reusable:

- ejecutar `onRefresh` con el estado de consulta actual si el callback existe
- no resetear filtros
- no resetear `page`
- no resetear `pageSize`
- no alterar `search`, `structuredFilters` ni `sort`

Reglas por modo:

- `server mode`
  - `onRefresh` debe volver a ejecutar la consulta backend con el query state actual

- `client mode`
  - `onRefresh` solo aplica si la pantalla provee una recarga externa del dataset
  - si no existe `onRefresh`, el control no debe renderizarse o debe quedar disabled

- `none mode`
  - aplica la misma regla que `client mode`

## Diseño backend

### Requerimientos minimos

Para soportar paginacion tipo Gmail real, el backend debe:

- recibir `Page`
- recibir `PageSize`
- recibir `Search`
- recibir `SearchType`
- recibir `StructuredFilters`
- recibir `SortField`
- recibir `SortDir`
- retornar `Pagination.Page`
- retornar `Pagination.PageSize`
- retornar `Pagination.Total` real

### Requisito critico

`Pagination.Total` debe representar el total global de la consulta filtrada, no solo los registros devueltos en la pagina actual.

### workflowInboxgestion

En `workflowInboxgestion` hace falta:

- quitar hardcode del controller
- validar claims reales
- propagar total real desde repository/query builder
- mantener consistencia entre filtros y paginacion
- devolver tabla vacia estructuralmente consistente cuando no haya filas

## Busqueda simple y avanzada

### Busqueda local

En `client mode`, puede usarse quick filter de AG Grid:

- util para refinar la pagina/local dataset
- no reemplaza busqueda backend

### Busqueda servidor

En `server mode`, la consulta debe usar el request real del backend.

Reglas:

- `search`
- `searchType`
- `structuredFilters`
- `sort`
- `page`
- `pageSize`

deben pertenecer al mismo query state.

### Regla de coherencia

Cuando cambia:

- `search`
- `searchType`
- `structuredFilters`
- `sortField`
- `sortDir`

entonces:

- `page` se resetea a `1`
- `pageSize` se conserva
- se recalcula `total`

Esto es obligatorio para no romper la paginacion.

## Decisiones explicitas

### Decision 1

No usar la paginacion nativa de AG Grid como unica solucion del sistema.

### Decision 2

Mantener AG Grid pagination disponible como modo opcional `client`.

### Decision 3

La experiencia tipo Gmail debe implementarse como `server mode` con barra externa reusable.

### Decision 4

`AppTable` no debe absorber por defecto toda la UX de consulta.  
El contenedor reusable debe encargarse del layout de controles.

### Decision 5

La busqueda avanzada debe integrarse al request backend y compartir el mismo query state que la paginacion.

## Plan de migracion

### Fase 1: Backend base de consulta

Objetivo:

- dejar `/api/workflowInboxgestion/inboxgestion` consistente para server mode

Entregables:

- claims reales en controller
- `Pagination.Total` real
- `Page` y `PageSize` consistentes
- vacio estructurado

### Fase 2: Query state reusable frontend

Objetivo:

- crear un modelo compartido de estado de consulta

Entregables:

- `AppTableQueryState`
- reglas de reset de pagina
- hooks reutilizables

### Fase 3: Wrapper y contenedor visual

Objetivo:

- crear el wrapper reusable tipo Gmail

Entregables:

- input de busqueda
- boton actualizar
- rango visible
- prev / next
- selector de page size
- acople visual con la tabla

### Fase 4: Modos de paginacion en AppTable

Objetivo:

- permitir `client`, `server`, `none`

Entregables:

- compatibilidad con AG Grid pagination local
- compatibilidad con wrapper server mode

### Fase 5: Integracion en GestionCorrespondencia

Objetivo:

- adoptar la arquitectura completa en el primer modulo real

Entregables:

- reemplazo de wiring actual
- integracion con query state
- prueba funcional del flujo

### Fase 6: Adopcion en otros modulos

Objetivo:

- reutilizar el modelo en otras tablas dinamicas

## Secuencia obligatoria de implementacion

La implementacion debe seguir esta secuencia para minimizar choques entre frontend y backend y evitar regresiones sobre `AppTable`, `workflowInboxgestion` y las acciones dinamicas ya existentes.

### Etapa 1: Backend primero

Orden obligatorio:

1. normalizar el endpoint `workflowInboxgestion`
2. exponer total real de la consulta filtrada
3. asegurar respuesta vacia estructurada

Razon:

- frontend no debe diseñar `server mode` sobre un backend que aun no devuelve `Pagination.Total` real
- el query state compartido depende de un contrato estable de API

### Etapa 2: Modelo compartido frontend

Orden obligatorio:

4. crear `AppTableQueryState`
5. definir las reglas de reset de pagina
6. adaptar los hooks dinamicos para consumir ese estado

Razon:

- el wrapper y la paginacion tipo Gmail no deben inventar su propio estado interno

### Etapa 3: Infraestructura visual reusable

Orden obligatorio:

7. crear `AppIconActionButton`
8. crear `AppTableQueryWrapper`
9. si hace falta, extraer `AppRefreshButton` y `AppTablePaginationBar`

Razon:

- primero debe existir la base reusable de acciones compactas
- luego se construye el contenedor visual que acopla controles y tabla

### Etapa 4: Evolucion de AppTable

Orden obligatorio:

10. agregar `paginationMode: "none" | "client" | "server"`
11. agregar `quickFilterText` para busqueda local
12. preservar compatibilidad hacia atras con usos actuales

Razon:

- `AppTable` debe soportar los modos, pero no debe absorber antes de tiempo la UX completa de consulta

### Etapa 5: Integracion funcional

Orden obligatorio:

13. integrar busqueda avanzada backend con el query state compartido
14. migrar `GestionCorrespondencia`
15. validar regresion sobre `MenuActions`, `Pinned`, overlays y seleccion

Razon:

- la primera pantalla real debe adoptarse solo cuando infraestructura y backend ya esten estables

## Artefactos por ticket

Los tickets detallados de esta iniciativa viven en esta misma carpeta:

- `01-BE-Normalizar-workflowInboxgestion-paginacion-consistente.md`
- `02-BE-Total-real-y-conteo-filtrado-workflowInboxgestion.md`
- `03-FE-AppTableQueryState-reusable.md`
- `04-FE-AppIconActionButton-y-AppTableQueryWrapper.md`
- `05-FE-Modos-paginacion-AppTable.md`
- `06-FE-Busqueda-avanzada-server-sin-romper-paginacion.md`
- `07-FE-Migracion-GestionCorrespondencia.md`

## Desglose de tickets Jira

### Backend

1. `Normalizar workflowInboxgestion para paginacion consistente y claims reales`
2. `Exponer total real y conteo filtrado en workflowInboxgestion`

### Frontend

3. `Crear AppTableQueryState reusable para tablas dinamicas`
4. `Crear AppIconActionButton y AppTableQueryWrapper`
5. `Agregar modos de paginacion client/server/none en AppTable`
6. `Integrar busqueda avanzada server sin romper paginacion`
7. `Migrar GestionCorrespondencia al modelo hibrido de tabla`

## Estrategia de pruebas

### Frontend unitarias

Cubrir:

- calculo de rango visible
- prev/next deshabilitado cuando aplica
- reset de pagina al cambiar filtros
- `client mode`
- `server mode`
- `none mode`
- quick filter local
- refresh con estado actual

### Frontend integracion

Cubrir:

- wrapper + AppTable
- wrapper + hook de consulta
- render con total real
- render con empty state
- render con loading

### Backend unitarias

Cubrir:

- total real de consulta
- page/pageSize
- busqueda simple
- busqueda avanzada
- structured filters
- sort
- claims invalidos

### Backend integracion

Cubrir:

- endpoint completo `workflowInboxgestion/inboxgestion`
- respuesta con paginacion
- respuesta con filtros
- respuesta vacia consistente

### Regresion

Cubrir:

- acciones dinamicas
- `Pinned/LockPinned`
- `MenuActions/Children/IsDivider`
- modulos que usan `AppTable` sin paginacion servidor

## Riesgos

- mezclar paginacion cliente y servidor en la misma pantalla
- recalcular mal `total`
- no resetear pagina al cambiar filtros
- acoplar demasiado `AppTable` a un caso de uso
- duplicar query state por modulo

## Recomendacion final

No implementar esta iniciativa como un solo ticket.

Debe tratarse como una migracion por fases con una arquitectura clara:

- `AppTable` como renderer base
- `AppTableQueryWrapper` como contenedor reusable
- backend consistente para server mode
- query state unificado

La experiencia tipo Gmail debe vivir en el wrapper/contenedor, no como una responsabilidad obligatoria y rigida de `AppTable`.
