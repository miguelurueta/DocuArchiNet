# gestion-correspondencia Specification

## MODIFIED Requirements

### Requirement: El modulo Gestion Correspondencia SHALL incluir pruebas de comportamiento del flujo estructural

El sistema SHALL mantener cobertura observable del shell y del flujo contextual del modulo, incluyendo la adopcion de affordance navegable reusable sin regresiones en navegacion, acciones ni seleccion.

#### Scenario: La accion de toolbar ya no abre el detalle contextual
- **WHEN** `GestionCorrespondencia` renderiza las acciones principales del toolbar
- **THEN** el flujo de apertura de `GestionRespuesta` MUST depender de la accion contextual de fila y MUST NOT exponerse tambien como boton global redundante

### Requirement: Gestion Correspondencia adopta affordance navegable reusable de AppTable

The system SHALL adopt the shared `AppTable` row click affordance contract in `GestionCorrespondencia`, replacing the module-local CSS implementation without changing the current navigation flow.

#### Scenario: GestionCorrespondencia activates shared affordance contract
- **WHEN** `GestionCorrespondencia` renders its main table
- **THEN** it MUST pass `rowClickAffordance` to `AppTable`
- **AND** it MUST preserve `onCellClicked` and `onActionTriggered` as the module-owned navigation layer

#### Scenario: Module-local navigable grid class is removed
- **WHEN** the shared affordance contract is active in `GestionCorrespondencia`
- **THEN** the page MUST NOT depend on `gridClassName={styles.navigableGrid}` for navigable cell affordance

#### Scenario: Local affordance CSS is removed
- **WHEN** `GestionCorrespondencia` adopts the shared affordance contract
- **THEN** `GestionCorrespondencia.module.css` MUST NOT keep cursor, hover, or equivalent visual rules for navigable table cells

#### Scenario: Data-cell navigation remains unchanged
- **WHEN** the user clicks a navigable data cell in `GestionCorrespondencia`
- **THEN** the module MUST keep navigating to `respuesta/:id` through its existing cell-click flow

#### Scenario: Actions column still does not trigger accidental navigation
- **WHEN** the user clicks the `acciones` column cell surface in `GestionCorrespondencia`
- **THEN** the module MUST NOT navigate through the generic cell-click flow
- **AND** the contextual action menu MUST remain usable

#### Scenario: Selection behavior remains unchanged
- **WHEN** the user interacts with row selection in `GestionCorrespondencia`
- **THEN** the module MUST preserve the existing selection behavior and MUST NOT add navigable affordance to the selection column

#### Scenario: Keyboard support comes from AppTable
- **WHEN** the user uses `Enter` on a navigable data cell in `GestionCorrespondencia`
- **THEN** the module MUST preserve the expected row action behavior through the shared `AppTable` implementation
- **AND** `GestionCorrespondencia` MUST NOT implement a second keyboard-specific navigation layer
