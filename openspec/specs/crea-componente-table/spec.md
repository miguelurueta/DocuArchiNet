# crea-componente-table Specification

## Purpose
TBD - created by archiving change scrumcore-27-crea-componente-table. Update Purpose after archive.
## Requirements
### Requirement: AppTable presentacional con AG Grid
El sistema MUST proveer un componente `AppTable<T extends Record<string, unknown>>` que renderice AG Grid Community, no conozca backend ni DTOs y permanezca desacoplado de dominio y navegacion de modulos consumidores.

#### Scenario: Render basico sin backend
- **WHEN** se renderiza `AppTable` con `rows` y `columns`
- **THEN** se muestra la grilla sin realizar llamadas a APIs

#### Scenario: Row click affordance is opt-in
- **GIVEN** un `AppTable` sin configuracion explicita de affordance
- **WHEN** la tabla se renderiza con filas y columnas
- **THEN** el sistema MUST NOT aplicar cursor navegable, hover navegable ni comportamiento extra de teclado asociado a affordance

#### Scenario: Row click affordance is enabled declaratively
- **GIVEN** un `AppTable` configurado con `rowClickAffordance`
- **WHEN** la tabla se renderiza en modo `table`
- **THEN** el sistema MUST aplicar una affordance visual reusable a celdas de datos navegables
- **AND** MUST NOT ejecutar navegacion automaticamente dentro del shared component

#### Scenario: Row click tooltip is opt-in and depends on affordance
- **GIVEN** un `AppTable` sin `rowClickTooltip`
- **WHEN** la tabla se renderiza con `rowClickAffordance`
- **THEN** el sistema MUST NOT mostrar ninguna pista textual adicional por defecto
- **AND** solo MUST activar tooltip reusable cuando el consumidor configure `rowClickTooltip`

#### Scenario: Row click tooltip remains inactive without affordance
- **GIVEN** un `AppTable` configurado con `rowClickTooltip`
- **AND** `rowClickAffordance` no esta activo
- **WHEN** la tabla se renderiza
- **THEN** el sistema MUST NOT asumir navegabilidad
- **AND** MUST NOT mostrar tooltip de affordance navegable

#### Scenario: Action and selection columns remain excluded
- **GIVEN** un `AppTable` configurado con `rowClickAffordance`
- **WHEN** la tabla renderiza una columna de acciones o la columna de seleccion
- **THEN** esas celdas MUST NOT recibir affordance navegable
- **AND** MUST conservar su comportamiento interactivo actual sin regresiones

#### Scenario: Tooltip excludes action and selection surfaces
- **GIVEN** un `AppTable` configurado con `rowClickAffordance` y `rowClickTooltip`
- **WHEN** el usuario pasa el cursor o enfoca la columna de acciones o la columna de seleccion
- **THEN** el sistema MUST NOT mostrar tooltip de affordance
- **AND** MUST preservar la interaccion actual de esas superficies

#### Scenario: Internal interactive controls keep ownership of interaction
- **GIVEN** un `AppTable` configurado con `rowClickAffordance`
- **AND** una celda contiene un control interactivo interno como `button`, `a`, `input`, `textarea`, `select` o `[role="button"]`
- **WHEN** el usuario interactua con ese control
- **THEN** el sistema MUST NOT sobreescribir su cursor
- **AND** MUST NOT interceptar sus eventos
- **AND** MUST NOT degradar su accesibilidad existente

#### Scenario: Tooltip excludes internal interactive controls
- **GIVEN** un `AppTable` configurado con `rowClickAffordance` y `rowClickTooltip`
- **AND** una celda contiene botones, links, inputs o menus interactivos
- **WHEN** el cursor o el foco pertenecen a uno de esos controles internos
- **THEN** el sistema MUST NOT mostrar tooltip de affordance sobre ese control
- **AND** el control interactivo MUST conservar su foco visible y semantica propia

#### Scenario: Enter reuses the primary consumer interaction flow
- **GIVEN** un `AppTable` configurado con `rowClickAffordance`
- **AND** el foco del grid esta sobre una celda navegable de datos
- **WHEN** el usuario presiona `Enter`
- **THEN** el sistema MUST disparar la accion primaria equivalente al flujo observable del consumidor
- **AND** MUST NOT hardcodear navegacion ni conocimiento de modulo dentro de `AppTable`

#### Scenario: Enter does not activate excluded surfaces
- **GIVEN** un `AppTable` configurado con `rowClickAffordance`
- **WHEN** el foco esta en la columna de acciones, seleccion o un control interactivo interno
- **AND** el usuario presiona `Enter`
- **THEN** el sistema MUST NOT disparar la accion primaria de fila o celda navegable

#### Scenario: Existing event contracts remain unchanged
- **GIVEN** un `AppTable` configurado con `rowClickAffordance`
- **WHEN** el usuario hace click o interactua por teclado
- **THEN** el sistema MUST preservar el contrato observable de `onRowClicked`, `onCellClicked` y `onActionTriggered`
- **AND** MUST NOT cambiar bubbling ni prioridades de eventos existentes

#### Scenario: Tooltip remains presentational and does not alter events
- **GIVEN** un `AppTable` configurado con `rowClickAffordance` y `rowClickTooltip`
- **WHEN** el usuario interactua con una superficie navegable valida
- **THEN** el sistema MUST tratar el tooltip como una capa presentacional
- **AND** MUST NOT interceptar click
- **AND** MUST NOT alterar bubbling
- **AND** MUST NOT cambiar el contrato observable de `onRowClicked`, `onCellClicked` ni `onActionTriggered`

#### Scenario: Cell focus remains functional without appearing as a second selection
- **GIVEN** un `AppTable` configurado con `rowClickAffordance`
- **WHEN** una celda navegable recibe foco dentro del grid
- **THEN** el sistema MUST mantener el foco funcional necesario para teclado y `Enter`
- **AND** MUST desacoplar la decoracion visual del foco de la semantica de seleccion de fila

#### Scenario: Focus visual override remains scoped to reusable affordance
- **GIVEN** un `AppTable` configurado con `rowClickAffordance`
- **WHEN** el shared component aplica overrides visuales para estados de foco de AG Grid
- **THEN** esos overrides MUST estar limitados al scope visual activado por `rowClickAffordance`
- **AND** MUST NOT afectar tablas que no usan ese contrato reusable

#### Scenario: Interactive children keep visible focus ownership
- **GIVEN** un `AppTable` configurado con `rowClickAffordance`
- **AND** una celda contiene botones, links, inputs o menus interactivos
- **WHEN** uno de esos elementos recibe foco
- **THEN** el sistema MUST conservar el foco visible del elemento interactivo
- **AND** MUST NOT neutralizarlo por el override visual aplicado al foco de celda

#### Scenario: Cards support reusable row click tooltip
- **GIVEN** un `AppTable` configurado con `presentationMode="cards"`, `rowClickAffordance` y `rowClickTooltip`
- **WHEN** el usuario pasa el cursor o enfoca una card navegable valida
- **THEN** el sistema MUST mostrar la pista textual reusable definida por el consumidor
- **AND** MUST preservar la accion primaria observable actual de la card

#### Scenario: Grid tooltip strategy remains lightweight
- **GIVEN** un `AppTable` configurado con `rowClickAffordance` y `rowClickTooltip` en modo `table`
- **WHEN** la grilla renderiza multiples celdas navegables
- **THEN** el sistema MUST implementar la pista textual con una estrategia liviana para AG Grid
- **AND** MUST evitar un wrapper React costoso por cada celda navegable si eso degrada el rendimiento observable

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

