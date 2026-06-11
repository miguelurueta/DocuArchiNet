## ADDED Requirements

### Requirement: SDK Dynamsoft alineado a servicio instalado
El sistema SHALL cargar `dwt@19.3.2` para alinear el frontend con el servicio local `1.9.3.1028` y modulo TWAIN `19.3.2`.

#### Scenario: Carga de SDK
- **WHEN** se inicializa `DynamsoftTwainClient`
- **THEN** el loader usa `https://cdn.jsdelivr.net/npm/dwt@19.3.2/dist/dynamsoft.webtwain.min.js`.

### Requirement: CSS requerido por DWT 19.3.2
El sistema SHALL inyectar los CSS requeridos por DWT 19.3.2 antes de completar la carga del runtime.

#### Scenario: Carga de estilos
- **WHEN** se carga Dynamsoft Web TWAIN
- **THEN** se inyectan `src/dynamsoft.webtwain.css` y `src/dynamsoft.webtwain.viewer.css` desde el mismo `ResourcesPath`.

### Requirement: Adapter compatible
`DynamsoftTwainClient` SHALL conservar el contrato actual y las APIs usadas del modelo `Dynamsoft.DWT`.

#### Scenario: Operaciones existentes
- **WHEN** el workspace usa scanner, rotacion, eliminacion y PDF
- **THEN** el adapter sigue usando `SourceCount`, `GetSourceNameItems`, `SelectSourceByIndex`, `AcquireImage`, `Rotate`, `RemoveImage`, `RemoveAllImages` y `ConvertToBlob("application/pdf")`.

### Requirement: Compatibilidad UI
La actualizacion de SDK SHALL mantener compatibles `AppDigitalizador`, `DigitalizacionDocumentalWorkspace` y `DigitalizacionDocumentalModal`.

#### Scenario: Regresion automatizada
- **WHEN** se ejecutan las pruebas de digitalizacion y AppDigitalizador
- **THEN** las suites pasan sin cambiar contratos publicos.
