# app-appvisorpdf-05-fe Specification

## Purpose
Definir mejoras responsive + accesibilidad (A11y) para `AppVisorPdf` (Ticket: `SCRUMCORE-194`),
sin acoplar el componente a pantallas consumidoras.

## Requirements

### Requirement: AppVisorPdf SHALL be responsive across mobile/tablet/desktop
El sistema SHALL adaptar layout y controles para mobile/tablet/desktop sin introducir alturas fijas.

#### Scenario: No fixed height layout
- **WHEN** the visor is embedded in a constrained container
- **THEN** it SHALL use flexible layout (`min-height: 0`) and internal scrolling without fixed heights

### Requirement: AppVisorPdf toolbar SHALL support compact mode on small screens
El sistema SHALL exponer un modo compacto de toolbar para <= 768px.

#### Scenario: Compact toolbar is applied on mobile
- **WHEN** viewport width is <= 768px
- **THEN** the toolbar SHALL render in compact mode (via class/data-attr) and secondary actions SHALL move to a dropdown

### Requirement: AppVisorPdf SHALL provide collapsible thumbnails
El sistema SHALL permitir abrir/cerrar thumbnails con un control accesible.

#### Scenario: Mobile thumbnails use overlay drawer
- **WHEN** the user is on mobile and opens thumbnails
- **THEN** thumbnails SHALL open in an overlay/drawer and be closable

#### Scenario: Tablet/desktop thumbnails use collapsible rail
- **WHEN** the user is on tablet/desktop
- **THEN** thumbnails SHALL be presented as a collapsible side rail

### Requirement: AppVisorPdf controls SHALL meet touch targets and a11y
El sistema SHALL asegurar hit-targets >= 40px y accesibilidad b\u00e1sica.

#### Scenario: Toggle exposes aria state
- **WHEN** thumbnails toggle is rendered
- **THEN** it SHALL expose `role="button"` (or native button), `aria-expanded`, and `aria-controls`

#### Scenario: Focus is managed when opening/closing thumbnails
- **WHEN** thumbnails open
- **THEN** focus SHALL move into the drawer/rail
- **WHEN** thumbnails close
- **THEN** focus SHALL return to the toggle button

### Requirement: AppVisorPdf SHALL be tested for responsive/a11y behavior
El sistema SHALL incluir tests unitarios que validen:
- toolbar compacta en mobile (clase o data-attr)
- toggle thumbnails accesible
- navegaci\u00f3n por teclado en controles principales

