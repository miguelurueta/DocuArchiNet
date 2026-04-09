## MODIFIED Requirements

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

#### Scenario: Action and selection columns remain excluded
- **GIVEN** un `AppTable` configurado con `rowClickAffordance`
- **WHEN** la tabla renderiza una columna de acciones o la columna de seleccion
- **THEN** esas celdas MUST NOT recibir affordance navegable
- **AND** MUST conservar su comportamiento interactivo actual sin regresiones

#### Scenario: Internal interactive controls keep ownership of interaction
- **GIVEN** un `AppTable` configurado con `rowClickAffordance`
- **AND** una celda contiene un control interactivo interno como `button`, `a`, `input`, `textarea`, `select` o `[role="button"]`
- **WHEN** el usuario interactua con ese control
- **THEN** el sistema MUST NOT sobreescribir su cursor
- **AND** MUST NOT interceptar sus eventos
- **AND** MUST NOT degradar su accesibilidad existente

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
