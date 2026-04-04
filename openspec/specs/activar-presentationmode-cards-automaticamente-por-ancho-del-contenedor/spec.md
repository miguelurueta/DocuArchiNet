# activar-presentationmode-cards-automaticamente-por-ancho-del-contenedor Specification

## Purpose
TBD - created by archiving change scrumcore-47-activar-presentationmode-cards-automaticamente-por-ancho-del-contenedor. Update Purpose after archive.
## Requirements
### Requirement: `AppTable` soporta activación responsive de `presentationMode`

`AppTable` MUST poder alternar automáticamente entre `table` y `cards` según el ancho del contenedor.

#### Scenario: override manual tiene prioridad

- **WHEN** una pantalla informa `presentationMode="table"` o `presentationMode="cards"`
- **THEN** ese valor se usa sin aplicar cálculo responsive

#### Scenario: activación automática por ancho

- **WHEN** `responsivePresentation.enabled === true`
- **AND** no existe `presentationMode` explícito
- **THEN** `AppTable` calcula el modo según el ancho del contenedor

### Requirement: el umbral de cards es configurable

La activación de cards MUST depender de un umbral configurable.

#### Scenario: cards por debajo del ancho configurado

- **WHEN** el ancho del contenedor es menor a `cardsBelow`
- **THEN** el renderer activo debe ser `cards`

#### Scenario: tabla por encima del ancho configurado

- **WHEN** el ancho del contenedor es mayor o igual a `cardsBelow`
- **THEN** el renderer activo debe ser `table`

### Requirement: el comportamiento sigue siendo reusable

La activación responsive MUST ser reutilizable para cualquier pantalla.

#### Scenario: no depende del nombre de la pantalla

- **WHEN** distintas pantallas usan `AppTable`
- **THEN** la lógica responsive depende solo del ancho del contenedor y la configuración recibida
- **AND** no depende de rutas ni módulos específicos

