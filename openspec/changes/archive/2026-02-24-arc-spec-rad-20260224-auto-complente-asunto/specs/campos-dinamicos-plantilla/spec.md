## ADDED Requirements

### Requirement: Integracion del campo ASUNTO en renderer dinamico
El sistema SHALL extender el renderizado dinamico de plantilla para que el campo `ASUNTO` pueda resolverse por `name_campo` y operar como autocompletado sin romper los atributos declarativos existentes.

#### Scenario: Render declarativo de ASUNTO
- **WHEN** el renderer procesa `camposPlantilla` y detecta `name_campo = "ASUNTO"`
- **THEN** el control conserva atributos declarativos (`required`, `disabled`, `maxLength`, metadata) y habilita comportamiento de autocompletado

### Requirement: Compatibilidad con flujo actual de radicacion
El sistema SHALL preservar el comportamiento de los demas campos dinamicos del formulario de radicacion al introducir autocompletado en `ASUNTO`.

#### Scenario: No regresion en campos dinamicos existentes
- **WHEN** se renderiza el formulario con campos distintos de `ASUNTO`
- **THEN** el comportamiento actual de controles y validaciones permanece sin cambios funcionales
