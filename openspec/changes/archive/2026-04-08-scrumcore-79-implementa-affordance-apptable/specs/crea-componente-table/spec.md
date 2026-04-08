# crea-componente-table Specification

## MODIFIED Requirements

### Requirement: AppTable presentacional con AG Grid

El sistema MUST mantener `AppTable<T extends Record<string, unknown>>` como componente shared presentacional, desacoplado de backend, dominio y navegacion de modulos consumidores, incluso cuando exponga affordance navegable reusable.

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
