### Requirement: Reusable AppHorizontalScroller primitive

The system SHALL provide an `AppHorizontalScroller` UI component for rendering arbitrary children in a horizontally scrollable responsive rail without coupling to domain logic or data loading.

#### Scenario: Render children in a horizontal rail

- **GIVEN** a consumer renders `AppHorizontalScroller` with multiple children
- **WHEN** the component is displayed
- **THEN** the children are arranged in one horizontal flex row
- **AND** the viewport supports native horizontal scrolling when content exceeds available width
- **AND** the component does not create page-level horizontal overflow outside its own viewport

#### Scenario: Render null children safely

- **GIVEN** a consumer renders `AppHorizontalScroller` with `children={null}`
- **WHEN** React renders the component
- **THEN** no runtime error is thrown
- **AND** the accessible region remains present

### Requirement: Accessible scroll region

The system SHALL expose the horizontal scroller as an accessible named region.

#### Scenario: Required aria label

- **GIVEN** a consumer renders `AppHorizontalScroller` with `ariaLabel="Listado horizontal"`
- **WHEN** the component is displayed
- **THEN** the scroll viewport has `role="region"`
- **AND** the scroll viewport has `aria-label="Listado horizontal"`

#### Scenario: No forced keyboard trap

- **GIVEN** the children contain focusable controls
- **WHEN** a user navigates by keyboard
- **THEN** focus remains controlled by the child elements
- **AND** the scroller container does not add `tabIndex` by default
- **AND** the component does not intercept keyboard events

### Requirement: Configurable density and spacing

The system SHALL support visual density and gap options through typed props.

#### Scenario: Density variants

- **GIVEN** a consumer sets `density="compact"`
- **WHEN** the component renders
- **THEN** compact density styles are applied

- **GIVEN** a consumer omits `density`
- **WHEN** the component renders
- **THEN** comfortable density styles are applied by default

#### Scenario: Gap variants

- **GIVEN** a consumer sets `gap` to `xs`, `sm`, `md`, or `lg`
- **WHEN** the component renders
- **THEN** the matching spacing style is applied between direct children

- **GIVEN** a consumer omits `gap`
- **WHEN** the component renders
- **THEN** the `md` gap style is applied by default

### Requirement: Stable item dimensions

The system SHALL allow consumers to define direct-child minimum and maximum widths without mutating children.

#### Scenario: Numeric dimensions are converted to px

- **GIVEN** a consumer sets `itemMinWidth={220}` and `itemMaxWidth={320}`
- **WHEN** the component renders
- **THEN** CSS custom properties expose `220px` and `320px`
- **AND** direct children use those custom properties for stable layout

#### Scenario: String dimensions are accepted when non-empty

- **GIVEN** a consumer sets `itemMinWidth="14rem"` and `itemMaxWidth="20rem"`
- **WHEN** the component renders
- **THEN** CSS custom properties expose `14rem` and `20rem`

#### Scenario: Invalid dimensions are ignored

- **GIVEN** a consumer sets empty strings, zero, negative numbers, `NaN`, or infinite numeric dimensions
- **WHEN** the component renders
- **THEN** invalid dimension custom properties are not applied
- **AND** the component still renders safely

### Requirement: Optional scroll snap

The system SHALL support optional proximity-based horizontal scroll snap.

#### Scenario: No snap by default

- **GIVEN** a consumer omits `scrollSnap`
- **WHEN** the component renders
- **THEN** no snap style is applied

#### Scenario: Start snap

- **GIVEN** a consumer sets `scrollSnap="start"`
- **WHEN** the component renders
- **THEN** proximity horizontal snap is enabled
- **AND** direct children align to snap start

#### Scenario: Center snap

- **GIVEN** a consumer sets `scrollSnap="center"`
- **WHEN** the component renders
- **THEN** proximity horizontal snap is enabled
- **AND** direct children align to snap center

### Requirement: Non-blocking edge fade

The system SHALL support an optional edge fade visual affordance that never blocks child interaction.

#### Scenario: Edge fade enabled

- **GIVEN** a consumer sets `edgeFade`
- **WHEN** the component renders
- **THEN** edge fade styles are applied
- **AND** the fade layer uses `pointer-events: none`
- **AND** child buttons, links, inputs and text selection remain interactive

#### Scenario: Edge fade disabled by default

- **GIVEN** a consumer omits `edgeFade`
- **WHEN** the component renders
- **THEN** edge fade styles are not applied

### Requirement: No domain or HTTP coupling

The system SHALL keep `AppHorizontalScroller` independent from business modules, HTTP clients and tabular components.

#### Scenario: No data fetching dependencies

- **WHEN** the component source is reviewed
- **THEN** it does not import `axios`
- **AND** it does not call `fetch`
- **AND** it does not import services or hooks from business modules

#### Scenario: No table coupling

- **WHEN** the change is reviewed
- **THEN** `AppTable` is not modified
- **AND** `AppTreeTable` is not modified

### Requirement: Enterprise documentation

The system SHALL document `AppHorizontalScroller` as a reusable UI primitive.

#### Scenario: Documentation created

- **WHEN** the change is complete
- **THEN** documentation exists at `docs/Architecture/AppHorizontalScroller/SCRUMCORE-297-AppHorizontalScroller/00-indice.md`
- **AND** it covers objective, scope, non-goals, API, accessibility, responsive behavior, CSS rules, technical decisions, risks, tests and future SCRUM-162 usage boundaries
