## ADDED Requirements

### Requirement: Capitalizacion de labels en campos dinamicos
El sistema SHALL mostrar los labels de campos `SELECCION` y `AUTOCOMPLETE` con efecto de letra capital cuando `campo_tip = 1`.

#### Scenario: Label con letra capital
- **WHEN** se renderiza un campo `SELECCION` o `AUTOCOMPLETE` con `campo_tip = 1`
- **THEN** el label se muestra con la primera letra en mayuscula (capitalizado) mediante CSS o transformacion en render
