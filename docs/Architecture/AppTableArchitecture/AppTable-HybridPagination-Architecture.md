# Arquitectura Maestra: AppTable con Paginacion Hibrida, Busqueda y Exportacion Reusable

## Objetivo

Definir una arquitectura reusable para evolucionar `AppTable` hacia un modelo de consulta y exportacion que soporte, sin romper compatibilidad:

- paginacion cliente nativa de AG Grid
- paginacion servidor tipo Gmail
- busqueda simple local
- busqueda simple servidor
- busqueda avanzada servidor
- refresco manual de la consulta
- exportacion de pagina actual
- exportacion de registros seleccionados
- exportacion de resultados completos de la consulta
- integracion visual reusable entre controles, tabla y acciones de exportacion

Este documento busca reducir ambiguedad y servir como fuente unica de verdad para:

- prompts de IA
- tickets Jira
- implementacion frontend
- implementacion backend
- pruebas de regresion

## Alcance

Aplica a:

- `AppTable`
- `AppTableQueryWrapper`
- futuro `AppTableExport`
- tablas dinamicas basadas en `DynamicUiTableDto`
- `workflowInboxgestion`
- futuros modulos que reutilicen la misma infraestructura

No aplica a:

- rediseño visual general del sistema
- cambios de negocio especificos de un modulo
- reemplazo de AG Grid
- definiciones de layout especificas de una sola pantalla

## Estado actual

### Frontend

`AppTable` es un wrapper de AG Grid y hoy:

- renderiza `AgGridReact`
- soporta `ColDef[]`
- soporta seleccion, loading, overlays y actions dinamicas
- soporta `paginationMode = "none" | "client" | "server"`
- soporta `quickFilterText` solo en modos locales
- no trae exportacion reusable nativa
- no trae toolbar propia de exportacion embebida en el core

La pantalla de `GestionCorrespondencia` ya tiene:

- wrapper reusable de consulta
- boton `Actualizar`
- dropdown visual de `Exportar`
- paginacion server-side con `AppTableQueryState`

Pero hoy:

- las opciones de exportacion son solo UI
- no existe un contrato reusable de exportacion para `AppTable`
- no existe una estrategia comun para exportar pagina, seleccion o consulta completa

### Backend

El endpoint `POST /api/workflowInboxgestion/inboxgestion` ya soporta el modelo base de consulta:

- `Page`
- `PageSize`
- `Search`
- `SearchType`
- `StructuredFilters`
- `SortField`
- `SortDir`

El backend de lectura paginada existe, pero no hay un contrato transversal de exportacion definido para:

- exportar con la misma consulta activa
- exportar todo el matching set sin depender del page actual del front
- devolver archivos listos en formatos como `xlsx` o `pdf`

## Problema a resolver

Se requiere un modelo de tabla que permita coexistencia entre dos dimensiones:

1. consulta
   - `client`
   - `server`

2. exportacion
   - pagina actual
   - seleccionados
   - todo lo cargado en memoria
   - todos los resultados de la consulta activa

La implementacion debe evitar:

- duplicacion de logica por modulo
- mezclar exportacion local y server sin reglas explicitas
- acoplar `AppTable` a `GestionCorrespondencia`
- asumir que `rows` contiene todo el universo de datos cuando hay paginacion server-side
- usar iteracion de paginas desde el navegador como estrategia principal de exportacion total

## Principios de arquitectura

### 1. AppTable sigue siendo renderer base

`AppTable` debe seguir siendo el wrapper base de AG Grid.

Responsabilidades de `AppTable`:

- renderizar grid o cards
- exponer configuracion reusable
- soportar modos de paginacion
- soportar quick filter local si aplica
- exponer seleccion y datos visibles al contenedor

No debe asumir por defecto:

- toolbar fija
- exportacion directa contra backend
- experiencia Gmail embebida de forma obligatoria
- formatos de archivo
- logica de negocio de una pantalla

### 2. La experiencia tipo Gmail vive en un wrapper/contenedor reusable

La experiencia de consulta debe vivir en `AppTableQueryWrapper` o en un contenedor equivalente.

Este wrapper debe acoplar visualmente:

- busqueda
- refresh
- total/rango
- selector de page size
- navegacion anterior/siguiente
- acciones de exportacion
- tabla renderizada

Regla visual:

- las acciones de exportacion deben convivir en la misma franja de controles donde vive la paginacion
- no deben renderizarse como toolbar desconectada o bloque separado de la tabla
- el layout debe reacomodarse de forma responsive sin romper la lectura ni el acceso a paginacion y descarga

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

### 4. La exportacion debe ser una capacidad reusable y desacoplada

La exportacion no debe vivir hardcodeada en una pantalla.

Debe existir una pieza reusable, por ejemplo:

- `AppTableExport`
- o `AppTableExportMenu`
- o `useAppTableExport`

Esta pieza debe:

- reutilizar columnas y metadata visibles de `AppTable`
- soportar distintos modos de exportacion
- delegar la carga completa de datos a un proveedor configurable
- no conocer endpoints concretos de negocio

### 5. Exportar todo no debe depender del dataset visible en front

Cuando `paginationMode = "server"`, `rows` representa solo la pagina actual.

Por tanto:

- `currentPage` puede resolverse desde `rows`
- `selectedRows` puede resolverse desde seleccion local
- `allLoaded` solo aplica a datasets ya cargados en memoria
- `allMatching` requiere backend o una estrategia explicita de carga adicional

## Arquitectura objetivo

```txt
Backend
  -> endpoint de consulta paginada
  -> endpoint o servicio de exportacion total
  -> DynamicUiTableDto / archivo exportado

Frontend
  -> useDynamicUiTableQuery / hooks del modulo
  -> AppTableQueryWrapper
      -> busqueda, refresh, paginacion y exportacion
      -> AppTableExport
          -> currentPage
          -> selectedRows
          -> allLoaded
          -> allMatching
      -> AppTable
          -> AG Grid
```

## Diseño frontend

### AppTable

`AppTable` mantiene estas capacidades:

```ts
type AppTablePaginationMode = "none" | "client" | "server";
```

Props relevantes ya presentes:

```ts
paginationMode?: AppTablePaginationMode;
quickFilterText?: string;
clientPaginationPageSize?: number;
rowSelection?: "single" | "multiple";
onSelectionChanged?: (rows: T[]) => void;
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

`AppTableQueryWrapper` debe seguir siendo el contenedor visual de la experiencia de consulta.

Responsabilidades:

- renderizar un bloque visual unico que contenga controles + tabla
- mostrar:
  - input de busqueda
  - boton actualizar
  - selector de page size
  - rango visible tipo Gmail
  - botones anterior / siguiente
  - slot de acciones extra, incluida exportacion

Regla de composicion:

- `AppTableExport` debe ubicarse en la misma fila o banda funcional de los controles de paginacion
- en desktop deben verse como parte de una unica pieza visual
- en responsive pueden reflowear, pero deben seguir perteneciendo al mismo bloque de controles de tabla
- `headerActions` debe reservarse para acciones de cabecera genuinas y no para acciones operativas de tabla como exportacion
- las acciones operativas de tabla deben vivir en la banda compartida de paginacion/controles

Layout contractual de referencia:

```txt
+----------------------------------------------------------------------------------+
| Buscar | Actualizar | Total/Rango | Page size | Prev | Next | Exportar [v]      |
+----------------------------------------------------------------------------------+
| Tabla / Cards                                                                   |
+----------------------------------------------------------------------------------+
```

Regla responsive de referencia:

```txt
+--------------------------------------------------+
| Buscar | Actualizar | Exportar [v]               |
| Total/Rango | Page size | Prev | Next            |
+--------------------------------------------------+
| Tabla / Cards                                    |
+--------------------------------------------------+
```

Objetivo:

- que descarga y paginacion sigan leyendose como parte del mismo sistema de control de tabla
- que el reflow responsive reorganice, pero no separe conceptualmente las acciones

### Componente reusable de exportacion

Se recomienda crear un componente tipo:

`AppTableExport`

Responsabilidades:

- construir el menu o trigger de exportacion
- resolver modos disponibles segun capacidades del datasource
- ejecutar el flujo de generacion de archivo
- mantenerse desacoplado de cualquier modulo concreto

No debe:

- conocer `workflowInboxgestion`
- conocer `GestionCorrespondencia`
- asumir una API backend fija
- asumir que todas las tablas son server-side

### Estado visual durante la descarga

La exportacion debe tener un estado visual propio y separado del loading de datos de la tabla.

Reglas:

- no activar `Skeleton Screen` de `AppTable` durante una descarga
- no reemplazar filas ni cards mientras se genera un archivo
- la tabla debe mantenerse visible y estable
- el trigger de descarga debe reflejar `loading`
- el menu de exportacion debe quedar bloqueado o deshabilitado mientras la operacion esta en curso

Si la descarga es breve:

- basta con `loading` en el trigger `Exportar`

Si la descarga es perceptiblemente larga:

- puede mostrarse una señal no destructiva sobre la banda de controles
- no sobre el cuerpo completo de la tabla

Opciones validas:

- spinner en `AppDropdown`
- estado `loading` del trigger
- veil liviano o mensaje de progreso en la fila de controles

Opciones no validas:

- skeleton de filas
- ocultar la tabla
- overlay agresivo que haga parecer que la consulta de tabla esta recargando

### Formato profesional del reporte exportado

La salida de los reportes debe usar un formato profesional ejecutivo y no limitarse a una exportacion plana de datos.

Todo reporte generado debe incluir un encabezado con metadatos obligatorios:

- nombre del reporte
- usuario que genero el reporte
- modulo que genero el reporte
- tipo de reporte
- fecha y hora de generacion
- numero de filas exportadas
- descripcion del reporte

Adicionalmente:

- se debe incorporar la imagen corporativa de la empresa
- la imagen debe resolverse desde un asset versionado dentro del repo
- no debe depender de rutas locales del equipo del usuario
- la imagen no debe referenciarse como URL externa dentro del reporte final
- la imagen debe insertarse o incrustarse en el archivo exportado
- el diseño final debe verse consistente y ejecutivo tanto en formatos tabulares como imprimibles

Reglas de arquitectura:

- estos metadatos deben pertenecer al contrato de exportacion y no quedar hardcodeados por modulo
- el proveedor de exportacion debe poder recibir metadata contextual del reporte
- el nombre de archivo y el encabezado visual deben mantenerse coherentes

Contrato sugerido de metadata:

```ts
type AppTableExportReportMeta = {
  reportName: string;
  generatedBy: string;
  moduleName: string;
  reportType: string;
  generatedAt: string;
  rowCount: number;
  description: string;
  companyImageAsset: string;
};
```

Regla de origen del logo:

- `companyImageAsset` debe referenciar un recurso controlado por el repositorio
- si la exportacion ocurre server-side, el backend o adaptador debe resolver el asset y embebelo en el archivo
- si la exportacion ocurre client-side, el frontend debe cargar el asset y embebelo en el archivo generado
- el reporte final no debe depender de una URL para pintar el logo

Convencion recomendada para este repo:

- usar una ruta publica y estable para reportes exportados
- crear o reservar la carpeta `public/branding/reports/`
- ubicar alli el logo oficial corporativo del reporte
- nombre recomendado del asset:
  - `public/branding/reports/company-report-logo.png`

Justificacion:

- una ruta en `public/` evita ambiguedad entre import interno de React y acceso directo para generadores de archivos
- facilita tanto exportacion client-side como estrategias server-side que necesiten una referencia estable
- evita que cada modulo termine apuntando a assets distintos en `src/assets/`

Regla por formato de salida:

- `xlsx`
  - debe incrustar la imagen corporativa dentro de la hoja del reporte
  - debe incluir encabezado ejecutivo visible en la parte superior

- `pdf`
  - debe incrustar la imagen corporativa dentro del encabezado del documento
  - debe mantener composicion ejecutiva lista para impresion o distribucion

- `csv`
  - no requiere imagen incrustada
  - debe incluir solo metadata textual si el formato lo permite sin romper compatibilidad
  - si no existe una forma limpia de encabezado enriquecido, puede exportarse como dataset plano con convencion de nombre de archivo

Regla de consistencia:

- no se debe prometer el mismo nivel visual en `csv` que en `xlsx` o `pdf`
- el formato ejecutivo completo aplica prioritariamente a `xlsx` y `pdf`

### Contrato recomendado

```ts
type AppTableExportMode =
  | "currentPage"
  | "selectedRows"
  | "allLoaded"
  | "allMatching";

type AppTableExportFormat = "csv" | "xlsx" | "pdf";

type AppTableExportDataSource<T> = {
  getCurrentPageRows: () => T[];
  getSelectedRows?: () => T[];
  getAllLoadedRows?: () => T[];
  getAllMatchingRows?: () => Promise<T[]>;
};

type AppTableExportProps<T> = {
  columns: ColDef<T>[];
  dataSource: AppTableExportDataSource<T>;
  formats: AppTableExportFormat[];
  enabledModes?: AppTableExportMode[];
  fileName?: string;
  queryState?: AppTableQueryState;
};
```

### Semantica de cada modo

#### `currentPage`

- exporta los registros visibles en la pagina actual
- aplica tanto a `client` como a `server`

#### `selectedRows`

- exporta solo los registros seleccionados
- requiere seleccion habilitada
- si no hay seleccion, la opcion no debe aparecer o debe quedar disabled

#### `allLoaded`

- exporta todo lo ya cargado en memoria
- sirve para tablas `client`
- no debe ofrecerse por defecto en `server` si el front solo posee la pagina actual

#### `allMatching`

- exporta todos los registros que cumplen la consulta activa
- debe respetar:
  - `search`
  - `searchType`
  - `structuredFilters`
  - `sortField`
  - `sortDir`
- en `server mode` requiere `getAllMatchingRows` o un endpoint backend de exportacion

### Integracion visual

La exportacion debe vivir como accion de tabla reusable, no como detalle de una sola pagina.

Opciones validas:

- dentro del `AppTableQueryWrapper` como `headerActions`
- como componente hermano de `AppTable`
- como trigger reutilizable basado en `AppDropdown`

Regla:

- `AppTableExport` pertenece al ecosistema `AppTable`
- pero no al core de render de `AppTable.tsx`
- la implementacion grafica debe usar `AppDropdown` como patron visual estandar para mostrar las opciones de descarga

### Patron visual del menu de descarga

`AppDropdown` debe ser el componente oficial para exponer las opciones de exportacion.

Responsabilidades de `AppDropdown` en este contexto:

- actuar como trigger visual del menu
- renderizar jerarquia de opciones por formato y alcance
- reflejar estados disabled o loading cuando aplique

No debe:

- contener la logica de transformacion de archivos
- conocer backends concretos
- resolver por si mismo que modos estan disponibles

Ejemplo esperado de estructura visual:

- `Exportar en Excel`
- `Exportar en PDF`
- `Pagina actual`
- `Seleccionados`
- `Todo cargado`
- `Todos los resultados`

Regla de coherencia:

- las opciones visibles del `AppDropdown` deben depender estrictamente de las capacidades reales del datasource de `AppTableExport`
- no deben mostrarse opciones que el flujo no pueda resolver realmente

Regla de layout responsive:

- el trigger de descarga debe encajar visualmente con el grupo de paginacion
- si el ancho no alcanza, debe reubicarse sin quedar separado del contenedor de controles de tabla
- la solucion responsive debe priorizar continuidad visual sobre apilar toolbars inconexas

## Diseño backend

### Requerimientos minimos de consulta

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

### Requerimientos de exportacion

Para soportar `allMatching`, el backend debe ofrecer una de estas dos estrategias:

1. endpoint de exportacion directa
   - recibe el mismo query state
   - devuelve archivo final o URL temporal

2. endpoint de consulta total sin paginacion
   - recibe el mismo query state
   - devuelve todos los registros
   - el frontend genera el archivo

La estrategia recomendada es la primera para `xlsx` y `pdf`.

### Requisito critico

El backend de exportacion total debe reutilizar la misma semantica de filtros y ordenamiento de la consulta principal.

No se debe permitir que:

- la tabla muestre un conjunto
- y la exportacion total se resuelva con otra semantica distinta

## Busqueda simple y avanzada

### Busqueda local

En `client mode`, puede usarse quick filter de AG Grid:

- util para refinar el dataset local
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

Esto es obligatorio para no romper la paginacion y para mantener la exportacion total coherente con la consulta activa.

## Decisiones explicitas

### Decision 1

No usar la paginacion nativa de AG Grid como unica solucion del sistema.

### Decision 2

Mantener AG Grid pagination disponible como modo opcional `client`.

### Decision 3

La experiencia tipo Gmail debe implementarse como `server mode` con barra externa reusable.

### Decision 4

`AppTable` no debe absorber por defecto toda la UX de consulta ni la logica de exportacion total.

### Decision 5

La busqueda avanzada debe integrarse al request backend y compartir el mismo query state que la paginacion.

### Decision 6

`AppTableExport` debe ser reusable, pero alimentado por un datasource o strategy inyectable.

### Decision 7

`allMatching` no debe implementarse iterando paginas desde el navegador como estrategia principal del sistema.

### Decision 8

`csv` puede resolverse en frontend para casos locales; `xlsx` y `pdf` deben preferir backend cuando se exporta `allMatching`.

## Plan de migracion

### Fase 1: Backend base de consulta

Objetivo:

- dejar la consulta paginada consistente para `server mode`

Entregables:

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

- consolidar el wrapper reusable tipo Gmail

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

### Fase 5: Infraestructura reusable de exportacion

Objetivo:

- crear `AppTableExport` desacoplado de modulos concretos

Entregables:

- contrato `AppTableExportMode`
- datasource strategy
- integracion con `AppDropdown`
- soporte de `currentPage`, `selectedRows`, `allLoaded`, `allMatching`

### Fase 6: Adaptadores por pantalla

Objetivo:

- permitir que cada modulo conecte su backend sin romper la abstraccion reusable

Entregables:

- adapter de exportacion para pantallas server-side
- uso directo de `rows` para pantallas client-side
- naming consistente de archivos y formatos

## Secuencia obligatoria de implementacion

La implementacion debe seguir esta secuencia para minimizar choques entre frontend y backend y evitar regresiones sobre `AppTable` y modulos que lo reutilizan.

### Etapa 1: Contrato reusable primero

Orden obligatorio:

1. definir `AppTableExportMode`
2. definir `AppTableExportDataSource`
3. definir formatos soportados y reglas de disponibilidad

Razon:

- sin contrato reusable la exportacion termina acoplada a una sola pantalla

### Etapa 2: Capacidades locales

Orden obligatorio:

4. implementar `currentPage`
5. implementar `selectedRows`
6. implementar `allLoaded`

Razon:

- estos modos no dependen de backend y validan la API reusable

### Etapa 3: Capacidades server-side

Orden obligatorio:

7. definir `getAllMatchingRows` o endpoint server de exportacion
8. propagar `queryState` actual
9. asegurar coherencia con filtros y sort

Razon:

- `allMatching` no debe salir de suposiciones sobre `rows`

### Etapa 4: Integracion visual

Orden obligatorio:

10. integrar `AppTableExport` con `AppDropdown`
11. conectarlo al wrapper o toolbar reusable
12. mantener `AppTable.tsx` libre de logica backend

Razon:

- la UX debe verse integrada sin mover responsabilidades al renderer base

## Estrategia de pruebas

### Frontend unitarias

Cubrir:

- resolucion de modos habilitados
- exportacion de `currentPage`
- exportacion de `selectedRows`
- exportacion de `allLoaded`
- `allMatching` invoca el datasource correcto
- opciones disabled cuando faltan capacidades

### Frontend integracion

Cubrir:

- wrapper + AppTable + exportacion
- exportacion respetando columnas visibles
- exportacion con seleccion
- exportacion con `paginationMode = "server"`
- exportacion con `paginationMode = "client"`

### Backend

Cubrir:

- consistencia entre consulta visible y exportacion total
- filtros y ordenamiento aplicados en exportacion
- respuesta valida del endpoint de exportacion

### Regresion

Cubrir:

- tablas que no usan exportacion
- pantallas con exportacion local solamente
- pantallas con exportacion server-side
- acciones dinamicas ya existentes

## Riesgos

- acoplar `AppTableExport` a `GestionCorrespondencia`
- asumir que `server mode` conoce todos los registros
- iterar todas las paginas desde el navegador
- exportar con filtros distintos a los visibles
- mezclar semanticas `allLoaded` y `allMatching`
- poner demasiada responsabilidad dentro de `AppTable.tsx`

## Recomendacion final

Esta iniciativa debe tratarse como una arquitectura reusable del ecosistema `AppTable`, no como un feature puntual de una sola pantalla.

La solucion correcta es:

- `AppTable` como renderer base
- `AppTableQueryWrapper` como contenedor reusable de consulta
- `AppTableExport` como capacidad reusable de exportacion
- `AppTableQueryState` como fuente unica de verdad para consulta server-side
- backend como responsable de `allMatching` cuando los datos no estan completos en memoria

La exportacion de pagina y seleccion puede resolverse en frontend.

La exportacion de todos los resultados de la consulta debe resolverse con estrategia server-side o datasource inyectable, nunca asumiendo que la tabla visible contiene todo.
