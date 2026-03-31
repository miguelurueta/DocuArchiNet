## ADDED Requirements

### Requirement: Toolbar con superficie blanca
El sistema MUST definir `--toolbar-surface: white;` en el selector `.toolbar` de `GestionCorrespondencia.module.css`.

#### Scenario: Superficie blanca aplicada
- **WHEN** se renderiza el toolbar de GestionCorrespondencia
- **THEN** el fondo usa la variable `--toolbar-surface` con valor `white`
