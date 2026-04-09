# app-input-search Specification

## Purpose
Define el componente de busqueda reusable `AppInputSearch` para la capa UI compartida, preservando el contrato base de `AppInput` y evitando que los consumidores repitan semantica, iconografia y accesibilidad de campos de busqueda.
## Requirements
### Requirement: Reusable search input component
The system SHALL provide an `AppInputSearch` component in the shared UI layer that renders a text search control with `AutoComplete` + `Input`, exposes a value-based contract, and does not own API or domain state.

#### Scenario: Render controlled search value
- **WHEN** a consumer renders `AppInputSearch` with `value`, `placeholder`, and `aria-label`
- **THEN** the component MUST render an autocomplete combobox with the provided accessible name, placeholder, and controlled value

#### Scenario: Notify text changes
- **WHEN** the user types in `AppInputSearch`
- **THEN** the component MUST notify the consumer through `onChange(value)` without exposing DOM events as the public search contract

#### Scenario: Preserve input states
- **WHEN** a consumer renders `AppInputSearch` as disabled, loading, or with an error state
- **THEN** the component MUST preserve disabled priority, keep loading editable, and preserve error behavior aligned with `AppInput`

#### Scenario: Support uncontrolled initial value
- **WHEN** a consumer renders `AppInputSearch` without `value` and with `defaultValue`
- **THEN** the component MUST initialize the visible text from `defaultValue` without mixing controlled and uncontrolled sources of truth

### Requirement: Search input accessibility and visual semantics
The system SHALL expose `AppInputSearch` as an accessible search text control with an interactive search icon, conditional clear control, and keyboard-compatible autocomplete.

#### Scenario: Accessible name is provided by consumer
- **WHEN** `AppInputSearch` is rendered with `aria-label` or `aria-labelledby`
- **THEN** assistive technologies MUST identify the textbox by that accessible name rather than by decorative or icon content

#### Scenario: Search icon triggers immediate search
- **WHEN** the user activates the search icon
- **THEN** the component MUST execute `onSearch(value)` immediately when the current value satisfies `minLength`

#### Scenario: Clear button is accessible
- **WHEN** `AppInputSearch` has a visible value and is not disabled
- **THEN** the component MUST expose a clear button with `aria-label="Limpiar"` that clears the value through `onChange("")` and `onClear()` without automatically calling `onSearch("")`

#### Scenario: Autocomplete keyboard navigation remains available
- **WHEN** options are provided to `AppInputSearch`
- **THEN** the component MUST allow keyboard navigation and selection through the underlying autocomplete behavior

### Requirement: AppTableQueryWrapper uses AppInputSearch for table filtering
The system SHALL use `AppInputSearch` for the search field inside `AppTableQueryWrapper` without changing table query behavior.

#### Scenario: Update table search query
- **WHEN** the user types in the `AppTableQueryWrapper` search field
- **THEN** the wrapper MUST call `onQueryChange` with the updated `search` value

#### Scenario: Preserve optional search visibility
- **WHEN** `AppTableQueryWrapper` is rendered with search disabled
- **THEN** the wrapper MUST NOT render `AppInputSearch`

### Requirement: Deterministic search events
The system SHALL make `AppInputSearch` execute `onSearch` deterministically from Enter, search icon activation, option selection, and optional typing debounce.

#### Scenario: Enter bypasses debounce
- **WHEN** the user types text that schedules a debounced search and then presses Enter
- **THEN** the component MUST cancel or neutralize the pending debounce and execute one immediate `onSearch(value)`

#### Scenario: Search icon bypasses debounce
- **WHEN** the user types text that schedules a debounced search and then activates the search icon
- **THEN** the component MUST cancel or neutralize the pending debounce and execute one immediate `onSearch(value)`

#### Scenario: Debounced typing search
- **WHEN** `debounceMs` is greater than zero and the user types a value that satisfies `minLength`
- **THEN** the component MUST execute `onSearch(value)` after the debounce interval if no manual search supersedes it

#### Scenario: Debounce disabled
- **WHEN** `debounceMs` is zero or undefined
- **THEN** typing MUST NOT schedule a debounced `onSearch`

#### Scenario: minLength blocks short searches
- **WHEN** the current value length is lower than `minLength`
- **THEN** the component MUST NOT execute `onSearch` from typing debounce, Enter, search icon activation, or option selection

### Requirement: Autocomplete options remain presentational
The system SHALL accept autocomplete `options` with `value` and optional `label` while keeping API and domain mapping outside `AppInputSearch`.

#### Scenario: Render empty options without breaking input
- **WHEN** `options` is empty or undefined
- **THEN** `AppInputSearch` MUST keep accepting free text input and manual search events

#### Scenario: Selecting option updates and searches
- **WHEN** the user selects an autocomplete option
- **THEN** `AppInputSearch` MUST call `onChange(selectedValue)` and immediately call `onSearch(selectedValue)` without debounce

#### Scenario: Options are not mutated
- **WHEN** `AppInputSearch` renders provided options
- **THEN** the component MUST NOT mutate the `options` array or option objects supplied by the consumer

### Requirement: Size variants and local visual consistency
The system SHALL provide `sm`, `md`, and `lg` size variants for `AppInputSearch` while preserving visual consistency with `AppInput`.

#### Scenario: Size classes apply
- **WHEN** a consumer renders `AppInputSearch` with `size="sm"`, `size="md"`, or `size="lg"`
- **THEN** the component MUST apply the corresponding size class affecting height, padding, and icon sizing

#### Scenario: Default size is md
- **WHEN** a consumer renders `AppInputSearch` without `size`
- **THEN** the component MUST use `md` as the default size

