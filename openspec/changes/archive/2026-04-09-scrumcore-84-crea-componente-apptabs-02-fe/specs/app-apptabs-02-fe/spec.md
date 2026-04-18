## ADDED Requirements

### Requirement: Componente reusable AppTabs fase 02
El sistema SHALL consolidar el componente reusable `AppTabs` como contrato oficial para pestanas, manteniendo una API tipada y estable para integraciones futuras.

#### Scenario: Uso basico con items declarativos
- **WHEN** un modulo renderiza `AppTabs` con una lista de `items`
- **THEN** el componente SHALL mostrar las pestanas definidas con sus labels y contenidos correspondientes

### Requirement: UI/UX con iconos y badges
El componente SHALL renderizar iconos antes del label y badges a la derecha usando `Badge` de AntD cuando el item lo defina.

#### Scenario: Icono y badge visibles
- **WHEN** un item incluye icono y badge
- **THEN** el tab SHALL mostrar el icono antes del label y el badge alineado a la derecha del label

### Requirement: Modo controlado y no controlado
El componente SHALL soportar modo controlado (`activeKey`) y no controlado (`defaultActiveKey`) sin mezclar ambos.

#### Scenario: Modo controlado
- **WHEN** `activeKey` esta definido
- **THEN** el componente SHALL reflejar ese valor como pestana activa y SHALL ignorar `defaultActiveKey`

#### Scenario: Modo no controlado
- **WHEN** `activeKey` no esta definido y se provee `defaultActiveKey`
- **THEN** el componente SHALL inicializar la pestana activa con `defaultActiveKey` y permitir cambios internos

### Requirement: Bloqueo por disabled y beforeChange
El componente SHALL bloquear cambios de pestana cuando `disabled` este activo o cuando `beforeChange` retorne `false`.

#### Scenario: Bloqueo por disabled
- **WHEN** `disabled` es `true`
- **THEN** el componente SHALL impedir click/teclado y SHALL evitar disparar `onChange`

#### Scenario: Bloqueo por beforeChange
- **WHEN** `beforeChange` retorna `false`
- **THEN** el componente SHALL mantener la pestana activa actual sin ejecutar `onChange`

### Requirement: Accesibilidad de tabs
El componente SHALL mantener atributos de accesibilidad consistentes con el control de tabs.

#### Scenario: Roles y estados de accesibilidad
- **WHEN** el componente renderiza tabs
- **THEN** el contenedor principal SHALL conservar `role=\"tablist\"` y las tabs deshabilitadas SHALL usar `aria-disabled=\"true\"`

### Requirement: Variantes y tamanos con design tokens
El componente SHALL soportar `variant` (default, card, underline, pills) y `size` (sm, md, lg) ajustando padding, font-size y altura mediante tokens CSS.

#### Scenario: Aplicacion de variantes y tamanos
- **WHEN** se proveen `variant` y `size`
- **THEN** el componente SHALL aplicar estilos basados en `--tabs-padding-sm`, `--tabs-padding-md` y `--tabs-padding-lg`

### Requirement: Responsive y overflow controlado
El componente SHALL comportarse de forma responsive con overflow controlado y dropdown de tabs adicionales.

#### Scenario: Comportamiento responsive
- **WHEN** el viewport es mobile
- **THEN** el contenedor de tabs SHALL habilitar `overflow-x` y SHALL evitar wrap descontrolado

#### Scenario: Overflow con mas
- **WHEN** hay mas tabs que el espacio disponible
- **THEN** el componente SHALL agrupar tabs en `more` con label "Mas" y contador `+N`, alineando el dropdown a la derecha

### Requirement: Feedback visual de disabled
El componente SHALL reflejar estado disabled con menor opacidad, cursor `not-allowed` y sin hover.

#### Scenario: Estilos disabled
- **WHEN** el componente esta `disabled`
- **THEN** el tab SHALL renderizar opacidad reducida, cursor `not-allowed` y sin estilos de hover
