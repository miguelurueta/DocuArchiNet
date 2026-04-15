## ADDED Requirements

### Requirement: Refinamiento visual de AppInputSelect existente
El sistema SHALL refinar la capa visual del componente shared `AppInputSelect`
ya existente, sin crear un componente paralelo ni alterar su propósito reusable.

#### Scenario: Evolucion visual del componente
- **WHEN** se implementa la FE visual de `AppInputSelect`
- **THEN** los cambios recaen sobre el componente shared ya creado
- **AND** no se introduce otro wrapper distinto derivado del naming del ticket

### Requirement: Tamanos shared alineados al sistema UI
El sistema SHALL consolidar los tamaños `sm`, `md` y `lg` de `AppInputSelect`
para alinearlos visualmente con el lenguaje del sistema compartido.

#### Scenario: Sizing consistente
- **WHEN** el consumidor configura `size="sm"`, `size="md"` o `size="lg"`
- **THEN** `AppInputSelect` adapta altura, área interactiva y densidad visual al
  tamaño configurado

### Requirement: Estados visuales consistentes con Ant Design
El sistema SHALL representar los estados `idle`, `focused`, `disabled`,
`loading`, `empty`, `error`, `warning` y `selected` de forma consistente con la
apariencia nativa de Ant Design.

#### Scenario: Estado de foco y soporte
- **WHEN** el usuario interactúa con `AppInputSelect`
- **THEN** foco, hover, warning y error se mantienen legibles y coherentes con
  Ant Design

#### Scenario: Estado vacio y loading
- **WHEN** no hay opciones o el componente está cargando datos
- **THEN** `AppInputSelect` renderiza estados vacio y loading claros y estables

### Requirement: Responsive usable en desktop, tablet y mobile
El sistema SHALL mantener `AppInputSelect` usable y legible en desktop, tablet y
mobile, incluyendo modo simple, multiple y tags.

#### Scenario: Render responsive del control
- **WHEN** `AppInputSelect` se renderiza en diferentes breakpoints
- **THEN** el control conserva legibilidad, touch target suficiente y dropdown usable
- **AND** etiquetas largas o tags múltiples no rompen el layout inmediato

### Requirement: Border radius leve y moderno
El sistema SHALL usar un border radius discreto y moderno en `AppInputSelect`,
alineado al lenguaje visual actual del sistema.

#### Scenario: Radius del selector
- **WHEN** `AppInputSelect` se renderiza
- **THEN** el selector mantiene esquinas suaves pero sobrias
- **AND** el radio no rompe la apariencia nativa de Ant Design

### Requirement: Cobertura de validacion visual del contrato
El sistema SHALL cubrir esta FE con pruebas del contrato visual relevante del
componente existente.

#### Scenario: Validacion de sizing y estados
- **WHEN** se ejecutan las pruebas de `AppInputSelect`
- **THEN** la suite valida al menos sizing, estados de soporte y comportamiento
  basico del wrapper visual
