## ADDED Requirements

### Requirement: UI galeria con acciones y cards visuales
AppUpload SHALL renderizar previews tipo galeria con acciones overlay, cards 1:1, bordes suaves y hover elevation.

#### Scenario: Visual cards en hover
- **WHEN** el usuario pasa el cursor sobre un item
- **THEN** se muestra elevacion visual y acciones overlay

### Requirement: Layout responsive por columnas
AppUpload SHALL ajustar el grid a 46 columnas en Desktop, 23 en Tablet y 2 en Mobile.

#### Scenario: Cambio de breakpoint
- **WHEN** el viewport cambia a mobile
- **THEN** la grilla se renderiza en 2 columnas con padding reducido

### Requirement: Drag & drop con estados visuales
AppUpload SHALL mostrar estado visual valido/invalido durante drag & drop cuando `drag` es true.

#### Scenario: Archivo invalido en hover
- **WHEN** el usuario arrastra un archivo no permitido
- **THEN** el drop area muestra estado invalido

### Requirement: Accesibilidad UI
AppUpload SHALL soportar teclado y focus visible en items y acciones.

#### Scenario: Accion por teclado
- **WHEN** el usuario presiona Enter en un item
- **THEN** se dispara el preview si esta habilitado
