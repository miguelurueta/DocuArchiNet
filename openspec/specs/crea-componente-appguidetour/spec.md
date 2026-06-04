## Purpose

Define the reusable `AppGuideTour` component and its PDF viewer integration, including Driver.js encapsulation, stable tour targets, accessibility, lifecycle, observability, tests, and enterprise documentation.

## Requirements

### Requirement: Reusable AppGuideTour component

The system SHALL provide a reusable `AppGuideTour` component for guided UI tours, independent from PDF-specific business logic.

#### Scenario: Render tour component without starting

- **GIVEN** a consumer renders `AppGuideTour` with `tourId` and valid `steps`
- **WHEN** `autoStart` is false or omitted
- **THEN** the component initializes without starting the visual tour automatically

#### Scenario: Start tour through public API

- **GIVEN** `useAppGuideTour` is configured with valid steps
- **WHEN** `start()` is called
- **THEN** the adapter receives filtered valid steps
- **AND** the state transitions to `running`

#### Scenario: Stop tour through public API

- **GIVEN** a tour is running
- **WHEN** `stop()` is called
- **THEN** the adapter stops Driver.js
- **AND** the state transitions to `cancelled` or `idle` after cleanup

### Requirement: Driver.js encapsulation

The system SHALL isolate Driver.js behind `DriverJsAdapter` so consumers do not import or depend on Driver.js directly.

#### Scenario: Consumer uses adapter abstraction

- **GIVEN** a consumer imports `AppGuideTour` or `useAppGuideTour`
- **WHEN** the consumer starts or stops a tour
- **THEN** it interacts with the AppGuideTour API
- **AND** it does not import `driver.js`

#### Scenario: Adapter maps steps to Driver.js

- **GIVEN** AppGuideTour receives `AppGuideTourStep[]`
- **WHEN** DriverJsAdapter starts the tour
- **THEN** each step is converted to Driver.js step configuration with target element, title, description and side

### Requirement: AppVisorEmbedPdf help tour integration

The system SHALL integrate AppGuideTour into `AppVisorEmbedPdf` through an accessible help button in `AppPdfToolbar`.

#### Scenario: Toolbar exposes help button

- **GIVEN** `AppVisorEmbedPdf` is loaded and toolbar is visible
- **WHEN** the user views the toolbar
- **THEN** a keyboard-accessible help button is visible
- **AND** the button has accessible name "Guia interactiva" or "Ayuda"
- **AND** the button uses the same visual language as existing toolbar buttons

#### Scenario: Click help starts PDF guide

- **GIVEN** the help button is visible
- **WHEN** the user activates it by click or keyboard
- **THEN** the PDF guide tour starts
- **AND** the first valid visible step is shown

#### Scenario: Existing toolbar consumers remain compatible

- **GIVEN** a consumer renders `AppPdfToolbar` without guide props
- **WHEN** the toolbar renders
- **THEN** no help button is required
- **AND** existing props and behavior continue to work

### Requirement: Stable tour targets

The system SHALL use stable `data-guide-tour-id` attributes for tour targets instead of CSS module class names or translated text.

#### Scenario: Toolbar target exists

- **GIVEN** `AppPdfToolbar` renders a guided control
- **WHEN** the DOM is inspected
- **THEN** the element has a stable `data-guide-tour-id`

#### Scenario: Missing targets are skipped

- **GIVEN** a configured step targets an element not present in the current viewport or policy state
- **WHEN** the tour starts
- **THEN** the missing step is skipped
- **AND** the tour continues if at least one valid step remains

### Requirement: PDF guide content

The system SHALL provide a configured guide for visible `AppVisorEmbedPdf` controls.

#### Scenario: Guide covers visible toolbar controls

- **GIVEN** toolbar controls are visible
- **WHEN** the PDF guide runs
- **THEN** the guide includes steps for thumbnails, zoom out, zoom level, zoom in, reset zoom, rotate left, rotate right, signature, lock signature, delete signature, print, export and help
- **AND** the guide may include pagination and scroll-to-top only when those overlays are present

#### Scenario: Guide does not invent unavailable controls

- **GIVEN** the current toolbar does not expose search, explicit Fit Width or explicit Fit Page buttons
- **WHEN** the PDF guide is configured
- **THEN** those unavailable controls are not added as new functional controls by this ticket
- **AND** those controls are not required as tour targets until they exist in the toolbar

### Requirement: Accessibility

The system SHALL satisfy baseline WCAG AA expectations for the help entry point and tour lifecycle.

#### Scenario: Keyboard accessible help

- **GIVEN** the user navigates with keyboard
- **WHEN** focus reaches the help button
- **THEN** focus is visible
- **AND** pressing Enter or Space starts the guide

#### Scenario: Escape closes tour

- **GIVEN** the guide is running
- **WHEN** the user presses Escape
- **THEN** the tour closes
- **AND** focus returns to a stable control when possible

### Requirement: Observability without sensitive data

The system SHALL emit non-sensitive tour events without document data.

#### Scenario: Start event

- **WHEN** a guide starts
- **THEN** `guide_started` is emitted with `tourId` and step counts only

#### Scenario: Step change event

- **WHEN** the active step changes
- **THEN** `guide_step_changed` is emitted with `tourId`, `stepId`, `stepIndex` and `totalSteps`

#### Scenario: No sensitive data

- **WHEN** any guide event is emitted
- **THEN** it does not include URLs, tokens, file names, PDF text or document identifiers

### Requirement: Performance and lifecycle

The system SHALL avoid unnecessary Driver.js reinitialization and PDF rerenders.

#### Scenario: Stable driver lifecycle

- **GIVEN** `AppGuideTour` rerenders with the same `tourId` and memoized steps
- **WHEN** no tour action occurs
- **THEN** Driver.js is not recreated unnecessarily

#### Scenario: Cleanup on unmount

- **GIVEN** a component using AppGuideTour unmounts
- **WHEN** cleanup runs
- **THEN** the adapter destroys the Driver.js instance

### Requirement: Enterprise documentation

The system SHALL include enterprise documentation for the component and PDF integration.

#### Scenario: Documentation files exist

- **WHEN** implementation is complete
- **THEN** documentation exists under `docs/Components/AppGuideTour/GuiaVisorPDF/`
- **AND** includes architecture, detailed implementation, tests and metadata files

### Requirement: Tests and regression coverage

The system SHALL include automated tests for AppGuideTour and its PDF integration.

#### Scenario: Unit tests cover reusable module

- **WHEN** unit tests run
- **THEN** they cover component render, hook start/stop, step filtering and DriverJsAdapter mapping

#### Scenario: Integration tests cover PDF toolbar

- **WHEN** `AppVisorEmbedPdf` tests run
- **THEN** they assert the help button is visible and starts the guide without breaking existing toolbar actions

#### Scenario: Playwright validates user flow

- **WHEN** Playwright smoke runs for the guide
- **THEN** it validates help button visibility, tooltip, tour opening, next/previous navigation, finish/cancel and responsive desktop/tablet/mobile behavior
