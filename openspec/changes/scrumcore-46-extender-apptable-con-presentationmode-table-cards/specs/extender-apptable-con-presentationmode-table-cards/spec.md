## ADDED Requirements

### Requirement: `AppTable` soporta `presentationMode`

`AppTable` MUST soportar dos modos de presentación reutilizables.

#### Scenario: `table` preserva el comportamiento actual

- **WHEN** una pantalla renderiza `AppTable` sin `presentationMode` o con `presentationMode="table"`
- **THEN** el componente conserva la experiencia actual basada en AG Grid
- **AND** no rompe implementaciones existentes

#### Scenario: `cards` renderiza filas como cards

- **WHEN** una pantalla renderiza `AppTable` con `presentationMode="cards"`
- **THEN** cada fila se representa como una card
- **AND** la capa de consulta y paginación sigue siendo la misma

### Requirement: ambos modos comparten el mismo pipeline de datos

La presentación de cards MUST reutilizar el mismo pipeline que la vista tabular.

#### Scenario: mismo query state y paginación

- **WHEN** una pantalla alterna entre `table` y `cards`
- **THEN** conserva `queryState`, `page`, `pageSize`, `total` y `sort`
- **AND** no duplica hooks ni request mappers

### Requirement: la unidad visual de cards es la fila

La vista card MUST renderizar una card por fila, no una card por celda.

#### Scenario: campos visibles en card

- **WHEN** una pantalla usa `presentationMode="cards"`
- **THEN** la card muestra un subconjunto controlado de campos
- **AND** el orden visual de esos campos debe ser explícito

### Requirement: las acciones dinámicas siguen funcionando en cards

Las acciones de fila MUST seguir disponibles en la vista card.

#### Scenario: acciones reutilizadas

- **WHEN** una fila tiene acciones dinámicas disponibles
- **THEN** esas acciones se renderizan también en la card
- **AND** conservan su comportamiento funcional
