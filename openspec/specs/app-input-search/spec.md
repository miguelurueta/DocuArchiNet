# app-input-search Specification

## Purpose
Define el componente de busqueda reusable `AppInputSearch` para la capa UI compartida, preservando el contrato base de `AppInput` y evitando que los consumidores repitan semantica, iconografia y accesibilidad de campos de busqueda.

## Requirements

### Requirement: Reusable search input component
The system SHALL provide an `AppInputSearch` component in the shared UI layer that renders a text search control through the existing `AppInput` contract.

#### Scenario: Render controlled search value
- **WHEN** a consumer renders `AppInputSearch` with `value`, `placeholder`, and `aria-label`
- **THEN** the component MUST render a textbox with the provided accessible name, placeholder, and controlled value

#### Scenario: Notify text changes
- **WHEN** the user types in `AppInputSearch`
- **THEN** the component MUST notify the consumer through the input change handler without owning query state internally

#### Scenario: Preserve input states
- **WHEN** a consumer renders `AppInputSearch` as disabled or with an error state
- **THEN** the component MUST preserve the disabled and error behavior delegated by `AppInput`

### Requirement: Search input accessibility and visual semantics
The system SHALL expose `AppInputSearch` as an accessible search text control while keeping search iconography decorative.

#### Scenario: Accessible name is provided by consumer
- **WHEN** `AppInputSearch` is rendered with a label or `aria-label`
- **THEN** assistive technologies MUST identify the textbox by that accessible name rather than by decorative icon content

#### Scenario: Search icon is decorative
- **WHEN** `AppInputSearch` renders a search icon
- **THEN** the icon MUST NOT create an additional interactive control or replace the textbox accessible name

### Requirement: AppTableQueryWrapper uses AppInputSearch for table filtering
The system SHALL use `AppInputSearch` for the search field inside `AppTableQueryWrapper` without changing table query behavior.

#### Scenario: Update table search query
- **WHEN** the user types in the `AppTableQueryWrapper` search field
- **THEN** the wrapper MUST call `onQueryChange` with the updated `search` value

#### Scenario: Preserve optional search visibility
- **WHEN** `AppTableQueryWrapper` is rendered with search disabled
- **THEN** the wrapper MUST NOT render `AppInputSearch`
