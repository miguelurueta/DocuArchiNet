# app-input-tags Specification

## Purpose
TBD - created by archiving change scrumcore-63-crea-componente-appinputtags-fe-01. Update Purpose after archive.
## Requirements
### Requirement: AppInputTags reusable component
The system SHALL provide an `AppInputTags` component in `src/app/Components/UI/AppInputTags/` and SHALL export it through the shared UI barrel so consumers can import it without reaching into internal file paths.

#### Scenario: Component is available from shared UI layer
- **WHEN** a consumer imports `AppInputTags` from the shared UI exports
- **THEN** the import resolves to the reusable tags input component

#### Scenario: Component remains presentational
- **WHEN** `AppInputTags` renders with `options`, `value`, and callbacks
- **THEN** it renders the UI without invoking HTTP services or endpoint-specific logic

### Requirement: Controlled and uncontrolled tag state
The system SHALL support controlled usage through `value?: string[]` and uncontrolled usage through `defaultValue?: string[]`, with `value` taking precedence whenever it is provided.

#### Scenario: Controlled value drives visible tags
- **WHEN** `AppInputTags` receives `value={["Ana"]}`
- **THEN** it renders `Ana` as the visible tag and does not use internal tag state as the source of truth

#### Scenario: Uncontrolled default value initializes tags
- **WHEN** `AppInputTags` receives `defaultValue={["Ana"]}` without `value`
- **THEN** it initializes the visible tags with `Ana`

#### Scenario: Single mode replaces current tag
- **WHEN** `mode="single"` and the user confirms a new tag
- **THEN** the component treats the new tag as the replacement for the previous single tag

#### Scenario: Multiple mode accumulates tags
- **WHEN** `mode="multiple"` and the user confirms a new tag
- **THEN** the component adds the new tag without removing existing tags

### Requirement: Tag add and remove callbacks
The system SHALL expose deterministic callbacks for tag changes: `onAddTag(tag)`, `onRemoveTag(tag)`, and `onRemoveAll()`.

#### Scenario: Selected option adds a tag
- **WHEN** the user selects an autocomplete option
- **THEN** `onAddTag` is called with the selected option value

#### Scenario: Manual confirmation adds a tag
- **WHEN** the user confirms the current input text through Enter or an explicit add action
- **THEN** `onAddTag` is called with the current text if it is valid

#### Scenario: Remove tag action removes one tag
- **WHEN** the user activates the remove action for a visible tag
- **THEN** `onRemoveTag` is called with that tag value

#### Scenario: Remove all action clears all tags
- **WHEN** the user activates the remove-all action
- **THEN** `onRemoveAll` is called once

### Requirement: Search event semantics
The system SHALL expose `onSearch(query)` for search text changes and SHALL support `minLength` and `debounceMs` without relying on deprecated `KeyPress` behavior.

#### Scenario: Debounced typing triggers search
- **WHEN** the user types text that meets `minLength` and `debounceMs` is greater than zero
- **THEN** `onSearch` is called after the debounce delay with the current query

#### Scenario: Enter bypasses pending debounce
- **WHEN** the user presses Enter while a debounce is pending
- **THEN** the pending debounce is cancelled and `onSearch` is called immediately with the current query

#### Scenario: Search icon bypasses pending debounce
- **WHEN** the user clicks the search icon while a debounce is pending
- **THEN** the pending debounce is cancelled and `onSearch` is called immediately with the current query

#### Scenario: Short query does not search
- **WHEN** the current query length is lower than `minLength`
- **THEN** `onSearch` is not called

#### Scenario: Zero debounce disables delay
- **WHEN** `debounceMs` is `0` or undefined and the query meets `minLength`
- **THEN** search execution does not wait for a debounce timer

### Requirement: Autocomplete options and loading
The system SHALL render autocomplete suggestions from immutable `options` compatible with `{ value: string; label: string; meta?: unknown }` and SHALL show `loading` without blocking input editing or manual search events.

#### Scenario: Options render as suggestions
- **WHEN** `options` contains `{ label: "Ana Perez", value: "ana" }`
- **THEN** the autocomplete presents `Ana Perez` as a selectable suggestion for value `ana`

#### Scenario: Option metadata remains UI-neutral
- **WHEN** `options` contains metadata such as `{ label: "Ana Perez", value: "ana", meta: { id: 7 } }`
- **THEN** `AppInputTags` keeps rendering the suggestion from `label` and `value` without requiring domain-specific metadata logic

#### Scenario: Empty options keep input usable
- **WHEN** `options` is empty
- **THEN** the input remains editable and manual tag confirmation remains available

#### Scenario: Loading keeps input editable
- **WHEN** `loading` is true
- **THEN** the component shows a loading indicator and keeps the input editable unless disabled

### Requirement: Clear and disabled behavior
The system SHALL provide clear behavior that does not trigger empty searches and SHALL prioritize disabled state over loading or search interactions.

#### Scenario: Clear does not search empty text
- **WHEN** the user clears the input text
- **THEN** the input text is cleared and `onSearch("")` is not called automatically

#### Scenario: Escape clears only when enabled
- **WHEN** `clearOnEscape` is true and the user presses Escape
- **THEN** the input text is cleared without automatically calling `onSearch("")`

#### Scenario: Disabled state blocks interaction
- **WHEN** `selectDisabled` or `disabled` is true
- **THEN** the component prevents input editing, tag addition, and tag removal interactions

### Requirement: Visual consistency with AppInput
The system SHALL style `AppInputTags` with a local CSS module and SHALL align its focus, hover, error, disabled, size, spacing, and border radius behavior with `AppInput`.

#### Scenario: Default styling matches AppInput
- **WHEN** `AppInputTags` renders with default size
- **THEN** it uses the same visual language as `AppInput`, including a 12px border radius

#### Scenario: Size variant applies component classes
- **WHEN** `size` is `sm`, `md`, or `lg`
- **THEN** the component applies the matching local size class for height, padding, and icon sizing

#### Scenario: Styles remain scoped
- **WHEN** `AppInputTags` styles are loaded
- **THEN** they are scoped through the component CSS module and do not require global selectors

### Requirement: Accessibility and keyboard support
The system SHALL provide accessible names, keyboard navigation, and accessible tag actions for `AppInputTags`.

#### Scenario: Input has accessible name
- **WHEN** the component receives `label`, `aria-label`, or `aria-labelledby`
- **THEN** the input exposes an accessible name to assistive technology

#### Scenario: Remove tag action is accessible
- **WHEN** a tag is rendered with a remove action
- **THEN** the remove action exposes an accessible name that identifies the tag being removed

#### Scenario: Remove all action is accessible
- **WHEN** the remove-all action is rendered
- **THEN** it exposes an accessible name equivalent to "Eliminar todos"

#### Scenario: Keyboard can select suggestions
- **WHEN** autocomplete suggestions are visible
- **THEN** the user can navigate and select a suggestion using the keyboard

### Requirement: Generic autocomplete contract
The system SHALL allow `AppInputTags` to consume autocomplete data from any parent hook that maps external API responses into a generic option contract with `value`, `label`, and optional `meta`, without coupling the component to endpoints, DTOs, or services.

#### Scenario: Parent hook provides normalized options
- **WHEN** a parent hook maps an API response into options shaped as `{ value: "123", label: "Ana Perez", meta: { source: "usuarios" } }`
- **THEN** `AppInputTags` renders the option using `label` and preserves the component as a presentational UI consumer of normalized data

#### Scenario: Component does not know endpoint details
- **WHEN** `AppInputTags` receives `options`, `loading`, and `onSearch`
- **THEN** it does not construct HTTP payloads, choose endpoints, or import domain hooks

#### Scenario: Search delegates query handling to the parent
- **WHEN** the user types a query that satisfies `minLength`
- **THEN** `AppInputTags` calls `onSearch(query)` and leaves all API lookup behavior to the parent

### Requirement: Secondary actions composition
The system SHALL allow `AppInputTags` to compose secondary actions through component slots or `AppDropdown`-compatible actions while keeping domain behavior outside the reusable component.

#### Scenario: Parent renders a secondary action slot
- **WHEN** a consumer provides a toolbar or action slot to `AppInputTags`
- **THEN** the component renders the action area without interpreting domain-specific behavior

#### Scenario: Remove all remains accessible
- **WHEN** the component exposes a remove-all action alongside autocomplete controls
- **THEN** the action remains keyboard reachable and exposes an accessible name equivalent to "Eliminar todos"

#### Scenario: Secondary actions do not block autocomplete
- **WHEN** the parent renders secondary actions and `loading` is true
- **THEN** the input remains editable unless `disabled` or `selectDisabled` is true

