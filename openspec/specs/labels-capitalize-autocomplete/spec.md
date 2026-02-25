## ADDED Requirements

### Requirement: Capitalizacion de labels en campos autocomplete
El sistema SHALL mostrar los labels de campos `AUTOCOMPLETE` con efecto de letra capital cuando `campo_tip = 1`.

#### Scenario: Label con letra capital en autocomplete
- **WHEN** se renderiza un campo `AUTOCOMPLETE` con `campo_tip = 1`
- **THEN** el label se muestra con la primera letra en mayuscula (capitalizado) mediante CSS o transformacion en render
