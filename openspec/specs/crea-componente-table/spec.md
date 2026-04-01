# crea-componente-table Specification

## Purpose
TBD - created by archiving change scrumcore-27-crea-componente-table. Update Purpose after archive.
## Requirements
### Requirement: AppTable presentacional con AG Grid
El sistema MUST proveer un componente `AppTable<T extends Record<string, unknown>>` que renderice AG Grid Community y no conozca backend ni DTOs.

#### Scenario: Render basico sin backend
- **WHEN** se renderiza `AppTable` con `rows` y `columns`
- **THEN** se muestra la grilla sin realizar llamadas a APIs

### Requirement: Props tipadas y callbacks
El sistema SHALL exponer props tipadas para seleccion, eventos de filas/celdas y `getRowId` opcional con fallback a `row.id`.

#### Scenario: Seleccion y callbacks
- **WHEN** el usuario selecciona filas o hace click en una celda
- **THEN** los callbacks tipados se ejecutan con datos de la fila

### Requirement: Configuracion base reusable
El sistema MUST centralizar defaults en `agGridDefaultConfig` y componer configuracion final en `useAgGridBaseConfig`.

#### Scenario: Defaults aplicados
- **WHEN** no se pasan overrides de configuracion
- **THEN** la grilla aplica defaults de seleccion multiple, columnas resizables y filtros

### Requirement: Loading y empty state
El sistema SHALL mostrar estados de loading y empty state cuando aplique.

#### Scenario: Loading activo
- **WHEN** `loading` es `true`
- **THEN** se muestra overlay de carga

#### Scenario: Sin filas
- **WHEN** `rows` esta vacio y `loading` es `false`
- **THEN** se muestra overlay de estado vacio

### Requirement: Documentacion obligatoria
El sistema MUST incluir documentacion de `AppTable` en `docs/Components/AppTable/README.md`.

#### Scenario: README disponible
- **WHEN** se consulta la documentacion
- **THEN** existen descripcion, API, ejemplos y limites del componente

