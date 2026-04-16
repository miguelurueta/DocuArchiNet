## ADDED Requirements

### Requirement: AppCheckbox reusable basado en Ant Design

El sistema SHALL proporcionar un componente shared `AppCheckbox` construido sobre
`Checkbox` de Ant Design en `src/app/Components/UI/AppCheckbox/`.

#### Scenario: render basico del checkbox

- **WHEN** un consumidor renderiza `AppCheckbox` con `label`
- **THEN** el componente muestra la casilla de Ant Design
- **AND** muestra el label visible alineado al control
- **AND** mantiene una API shared tipada y desacoplada del dominio

### Requirement: soporte controlado y no controlado para checkbox individual

`AppCheckbox` SHALL soportar modo controlado y no controlado sin ambiguedad.

#### Scenario: checkbox controlado

- **WHEN** el consumidor pasa `checked` y `onChange`
- **THEN** el componente refleja el estado controlado externo
- **AND** emite `onChange(checked, event)` con tipado fuerte

#### Scenario: checkbox no controlado

- **WHEN** el consumidor pasa `defaultChecked`
- **THEN** el componente inicializa el estado visual correctamente
- **AND** mantiene el comportamiento esperado de Ant Design

### Requirement: estados visuales y semanticos del checkbox

`AppCheckbox` SHALL soportar los estados `disabled`, `indeterminate` y `error`
sin romper accesibilidad ni consistencia visual.

#### Scenario: checkbox indeterminate

- **WHEN** el consumidor pasa `indeterminate=true`
- **THEN** la casilla muestra el estado parcial propio de Ant Design

#### Scenario: checkbox disabled

- **WHEN** el consumidor pasa `disabled=true`
- **THEN** la casilla bloquea interaccion
- **AND** mantiene apariencia visual coherente

### Requirement: tamanos alineados con el sistema UI

`AppCheckbox` SHALL soportar `size="sm" | "md" | "lg"` alineado con el lenguaje
visual del sistema UI.

#### Scenario: tamanos del componente

- **WHEN** el consumidor cambia la prop `size`
- **THEN** el wrapper ajusta spacing, tipografia o layout auxiliar
- **AND** no rompe la apariencia base de Ant Design

### Requirement: accesibilidad y compatibilidad con formularios

`AppCheckbox` SHALL exponer una integracion accesible y compatible con formularios.

#### Scenario: label accesible

- **WHEN** el consumidor pasa `label`
- **THEN** el label queda asociado a la casilla clicable

#### Scenario: atributos aria

- **WHEN** el consumidor pasa `aria-label`, `aria-labelledby` o `aria-describedby`
- **THEN** el componente reenvia correctamente esos atributos al control base

### Requirement: contratos publicos de Group y CheckAll definidos

La capability SHALL dejar definidos los contratos publicos de `AppCheckboxGroup`
y `AppCheckboxCheckAll`, aunque su logica completa se implemente en FE posteriores.

#### Scenario: ownership de estado grupal

- **WHEN** se documentan `AppCheckboxGroup` y `AppCheckboxCheckAll`
- **THEN** ambos contratos quedan definidos como controlados
- **AND** usan `value: TValue[]`
- **AND** usan `onChange: (value: TValue[]) => void`

#### Scenario: integracion con Form.Item

- **WHEN** se definen los contratos grupales
- **THEN** contemplan `name` y `rules` con tipado fuerte alineado con Ant Design

#### Scenario: relacion entre Group y CheckAll

- **WHEN** se define la arquitectura de la familia `AppCheckbox`
- **THEN** queda explicito que `AppCheckboxCheckAll` usa `AppCheckboxGroup` o un
  helper interno comun
- **AND** no debe existir una implementacion paralela desconectada del grupo
