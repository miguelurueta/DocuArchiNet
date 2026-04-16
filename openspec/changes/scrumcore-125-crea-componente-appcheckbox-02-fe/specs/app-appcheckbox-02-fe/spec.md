## ADDED Requirements

### Requirement: AppCheckboxGroup reusable basado en AppCheckbox

El sistema SHALL proporcionar un componente shared `AppCheckboxGroup`
construido a partir de `AppCheckbox`, dentro de
`src/app/Components/UI/AppCheckbox/`.

#### Scenario: composicion del grupo

- **WHEN** un consumidor renderiza `AppCheckboxGroup` con `options`
- **THEN** el grupo compone multiples instancias de `AppCheckbox`
- **AND** no implementa una familia paralela de checkboxes desconectada del shared base

### Requirement: ownership de estado controlado para el grupo

`AppCheckboxGroup` SHALL operar con contrato controlado obligatorio.

#### Scenario: grupo controlado

- **WHEN** un consumidor pasa `value` y `onChange`
- **THEN** el grupo refleja exactamente los valores controlados por el contenedor
- **AND** emite `onChange(value)` con la nueva seleccion
- **AND** no expone `defaultValue` como parte del contrato base del grupo

### Requirement: layout vertical y horizontal

`AppCheckboxGroup` SHALL soportar disposiciones `vertical` y `horizontal`
manteniendo legibilidad y consistencia visual.

#### Scenario: layout vertical

- **WHEN** el consumidor pasa `direction="vertical"`
- **THEN** las casillas se renderizan en columna
- **AND** el spacing entre items es consistente

#### Scenario: layout horizontal

- **WHEN** el consumidor pasa `direction="horizontal"`
- **THEN** las casillas se renderizan en fila o wrap controlado
- **AND** el grupo mantiene lectura clara sin colapsos visuales

### Requirement: tamanos y estados visuales consistentes

`AppCheckboxGroup` SHALL respetar la semantica visual del sistema UI para
`size`, `disabled`, `helperText` y `error`.

#### Scenario: tamanos del grupo

- **WHEN** el consumidor cambia `size`
- **THEN** el grupo transmite el tamaño a las instancias de `AppCheckbox`
- **AND** mantiene consistencia visual con la capa shared

#### Scenario: estado disabled del grupo

- **WHEN** el consumidor pasa `disabled=true`
- **THEN** todas las casillas del grupo quedan no interactivas

#### Scenario: helperText y error

- **WHEN** el consumidor pasa `helperText` o `error`
- **THEN** el grupo renderiza esa informacion de manera consistente con otros wrappers UI

### Requirement: responsive del grupo

`AppCheckboxGroup` SHALL comportarse correctamente en desktop, tablet y mobile.

#### Scenario: responsive en viewport estrecho

- **WHEN** el grupo horizontal se renderiza en una pantalla estrecha
- **THEN** el layout se adapta mediante wrap o reorganizacion visual
- **AND** no rompe la lectura ni la interaccion

### Requirement: compatibilidad con formularios

`AppCheckboxGroup` SHALL contemplar integracion compatible con `Form.Item` de Ant Design.

#### Scenario: integracion con name y rules

- **WHEN** el consumidor usa `AppCheckboxGroup` dentro de `Form.Item`
- **THEN** la API contempla `name`
- **AND** contempla `rules` con tipado fuerte alineado con Ant Design
- **AND** no bloquea validaciones externas del formulario
