## ADDED Requirements

### Requirement: AppDigitalizador corporativo
El sistema SHALL exponer un componente reutilizable `AppDigitalizador` en `src/app/Components/UI/AppDigitalizador`.

#### Scenario: Consumo simple desde modulos
- **WHEN** un modulo consumidor renderiza `<AppDigitalizador context={context} onCompleted={handleCompleted} />`
- **THEN** el componente monta la experiencia de digitalizacion sin requerir conocimiento de Dynamsoft, scanner clients, adapters, orchestrator ni infraestructura interna.

### Requirement: Construccion sobre Workspace existente
`AppDigitalizador` SHALL construirse encima de `DigitalizacionDocumentalWorkspace`.

#### Scenario: No duplicacion de logica
- **WHEN** `AppDigitalizador` se renderiza
- **THEN** delega scanner, miniaturas, preview, metadata, generacion PDF y operaciones API al workspace existente.

### Requirement: Modo inline sin modal
`AppDigitalizador` SHALL renderizarse inline dentro del contenedor padre.

#### Scenario: Integracion en layouts corporativos
- **WHEN** `AppDigitalizador` se monta dentro de un panel como `CapDocument.centerPanel`
- **THEN** no crea `AppModal` ni `role="dialog"` y ocupa el espacio disponible del contenedor.

### Requirement: Defaults corporativos
`AppDigitalizador` SHALL encapsular defaults corporativos para scanner, Dynamsoft, licencia, apiClient y modulo origen.

#### Scenario: Provider corporativo
- **WHEN** se define `AppDigitalizadorProvider`
- **THEN** `AppDigitalizador` usa los defaults del provider salvo que reciba overrides explicitos.

### Requirement: Sandbox visual
El sistema SHALL exponer una sandbox para pruebas funcionales de `AppDigitalizador`.

#### Scenario: Validacion con scanner fisico
- **WHEN** se abre `/__sandbox/app-digitalizador`
- **THEN** la pagina monta `<AppDigitalizador />` y permite validar licencia, scanner fisico, PDF, metadata, upload y callbacks desde la fachada final.

### Requirement: Compatibilidad
La incorporacion de `AppDigitalizador` SHALL mantener operativos `DigitalizacionDocumentalModal`, `DigitalizacionDocumentalWorkspace`, hooks, servicios, contratos y `DynamsoftTwainClient`.

#### Scenario: Suite existente
- **WHEN** se ejecuta `npx vitest run src/modules/digitalizacion`
- **THEN** las pruebas existentes pasan sin regresion.
