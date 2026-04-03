## ADDED Requirements

### Requirement: La consulta server se deriva de AppTableQueryState
El sistema SHALL usar `AppTableQueryState` como única fuente de verdad para construir la consulta server de tablas dinámicas, incluyendo búsqueda simple, búsqueda avanzada, paginación y sort.

#### Scenario: Búsqueda simple resetea página y participa del request
- **WHEN** cambia `search` en `AppTableQueryState`
- **THEN** el sistema resetea `page = 1`
- **AND** serializa `Search` en el request backend

#### Scenario: Filtros estructurados participan del request
- **WHEN** `AppTableQueryState` contiene `structuredFilters`
- **THEN** el sistema serializa `StructuredFilters` en el shape requerido por el backend

#### Scenario: Sort participa del mismo request
- **WHEN** `sortField` o `sortDir` cambian en `AppTableQueryState`
- **THEN** el sistema serializa `SortField` y `SortDir` en el mismo request server

### Requirement: El mapper backend-compatible es único y reusable
El sistema SHALL centralizar en un único helper o mapper la transformación desde `AppTableQueryState` hacia el request real del endpoint backend.

#### Scenario: Request compatible con workflowInboxgestion
- **WHEN** el query layer construye una consulta server
- **THEN** produce un payload que incluye `Page`, `PageSize`, `Search`, `SearchType`, `StructuredFilters`, `SortField` y `SortDir`

#### Scenario: El mapper evita serialización manual por pantalla
- **WHEN** una pantalla o hook de módulo consume el query layer compartido
- **THEN** no necesita reserializar manualmente la consulta base

### Requirement: El total server no se recalcula localmente
El sistema SHALL usar el total devuelto por backend como fuente de verdad en `server mode` y no debe recalcularlo localmente a partir de las filas visibles.

#### Scenario: Total filtrado viene del backend
- **WHEN** el query layer recibe una respuesta paginada del backend
- **THEN** expone `Pagination.Total` como total de la consulta activa
- **AND** no usa `rows.length` como reemplazo del total

### Requirement: quickFilterText no afecta la consulta server
El sistema SHALL ignorar `quickFilterText` cuando la tabla opera en `paginationMode = "server"` y la consulta se resuelve vía backend.

#### Scenario: Server mode mantiene coherencia entre rows y total
- **WHEN** una tabla opera en `server mode`
- **THEN** el request backend se construye solo desde `AppTableQueryState`
- **AND** `quickFilterText` no altera localmente la consulta ni el total mostrado
