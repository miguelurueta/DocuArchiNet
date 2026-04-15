## ADDED Requirements

### Requirement: Componente shared AppInputSelect reusable
El sistema SHALL proveer un componente shared `AppInputSelect` en la capa UI
reutilizable del proyecto, basado en `Select` de Ant Design.

#### Scenario: Render basico del componente
- **WHEN** un consumidor renderiza `AppInputSelect`
- **THEN** el control se presenta con el comportamiento base de Ant Design
- **AND** el componente puede recibir placeholder, valor y opciones

### Requirement: Soporte para opciones locales y remotas
El sistema SHALL permitir que `AppInputSelect` consuma opciones locales via
`options` y opciones remotas via `fetchOptions`.

#### Scenario: Uso con opciones locales
- **WHEN** el consumidor entrega `options`
- **THEN** `AppInputSelect` renderiza esas opciones en el dropdown

#### Scenario: Uso con opciones remotas
- **WHEN** el consumidor configura `fetchOptions`
- **THEN** el componente puede solicitar datos remotos y renderizar las opciones
  resultantes sin acoplarse a un endpoint de dominio especifico

### Requirement: Estados visuales de loading y no data
El sistema SHALL renderizar estados de `loading` y `no data` consistentes con
la experiencia visual de Ant Design.

#### Scenario: Estado loading remoto
- **WHEN** `AppInputSelect` espera la respuesta de `fetchOptions`
- **THEN** el componente muestra un estado visual de carga

#### Scenario: Estado vacio
- **WHEN** no existen opciones disponibles
- **THEN** el componente muestra `noDataText` o un empty state equivalente en el
  dropdown

### Requirement: Tamanos shared alineados al sistema UI
El sistema SHALL soportar `size="sm"`, `size="md"` y `size="lg"` para
`AppInputSelect`, alineados al lenguaje visual del sistema compartido.

#### Scenario: Sizing del control
- **WHEN** el consumidor configura el `size`
- **THEN** `AppInputSelect` adapta su presentacion visual y area interactiva al
  tamaño solicitado

### Requirement: Responsive y apariencia nativa
El sistema SHALL mantener una apariencia nativa de Ant Design, con responsive
correcto en desktop, tablet y mobile, y un border radius leve y moderno.

#### Scenario: Render responsive del select
- **WHEN** `AppInputSelect` se renderiza en distintos breakpoints
- **THEN** el control y su dropdown se mantienen legibles y usables
- **AND** el componente no rompe layout por labels largas o tags multiples

### Requirement: Export, documentacion y pruebas basicas
El sistema SHALL exportar `AppInputSelect` desde la capa shared, incluir
documentacion de uso y cubrir el contrato reusable con pruebas basicas.

#### Scenario: Consumo desde otro modulo
- **WHEN** un modulo importa `AppInputSelect` desde la capa shared
- **THEN** el componente se encuentra exportado correctamente
- **AND** existe documentacion base de uso y pruebas del contrato principal
