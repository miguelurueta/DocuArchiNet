# implementacion-screen-skeleton-apptable Specification

## ADDED Requirements

### Requirement: Skeleton reusable para carga inicial de AppTable

`AppTable` MUST mostrar un skeleton reusable cuando el componente esté cargando por primera vez y todavía no existan filas renderizables.

#### Scenario: first load tabular sin filas

- **WHEN** `presentationMode` sea `table`
- **AND** `loading` sea `true`
- **AND** no existan filas previas renderizables
- **THEN** `AppTable` MUST renderizar un skeleton tabular
- **AND** MUST NOT renderizar una tabla vacía como estado principal

#### Scenario: first load cards sin filas

- **WHEN** `presentationMode` sea `cards`
- **AND** `loading` sea `true`
- **AND** no existan filas previas renderizables
- **THEN** `AppTable` MUST renderizar un skeleton de cards

### Requirement: El skeleton no reemplaza contenido útil en refetch

`AppTable` MUST preservar el contenido visible cuando ya existan filas renderizadas y ocurra un refetch posterior.

#### Scenario: refetch con filas previas

- **WHEN** `loading` sea `true`
- **AND** ya existan filas renderizables visibles
- **THEN** `AppTable` MUST mantener el contenido actual
- **AND** MUST NOT reemplazarlo por skeleton

### Requirement: Distinción entre skeleton, empty state y error

`AppTable` MUST mantener separados los estados de skeleton, empty state real y error.

#### Scenario: empty state real

- **WHEN** `loading` sea `false`
- **AND** no existan filas renderizables
- **AND** no exista error
- **THEN** `AppTable` MUST mostrar el empty state configurado
- **AND** MUST NOT mostrar skeleton

#### Scenario: error state

- **WHEN** exista un error en el flujo de datos
- **THEN** `AppTable` MUST conservar el tratamiento actual de error
- **AND** MUST NOT sustituirlo por skeleton

### Requirement: Contrato configurable de modo de carga

`AppTable` MUST exponer una configuración shared para controlar la estrategia de carga visual.

#### Scenario: loading mode explicito

- **WHEN** una pantalla configure `loadingMode`
- **THEN** `AppTable` MUST respetar esa configuración
- **AND** MUST mantener skeleton como comportamiento default recomendado para carga inicial
