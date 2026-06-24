## ADDED Requirements

### Requirement: Boton contextual de captura
El sistema SHALL reemplazar los botones independientes `Escanear` y `Nuevo` por una unica accion contextual en la toolbar principal del digitalizador documental.

#### Scenario: Documento vacio inicia captura directa
- **GIVEN** el documento no contiene paginas capturadas
- **WHEN** el usuario ve la toolbar principal
- **THEN** se muestra un boton accesible como `Escanear`
- **AND** no se muestra un boton separado `Nuevo`
- **WHEN** el usuario pulsa `Escanear`
- **THEN** se inicia captura sin pedir confirmacion

#### Scenario: Documento con paginas inicia nuevo documento con confirmacion
- **GIVEN** el documento contiene una o mas paginas capturadas
- **WHEN** el usuario ve la toolbar principal
- **THEN** el mismo boton contextual se muestra como `Nuevo documento`
- **AND** no coexisten los botones `Escanear` y `Nuevo`
- **WHEN** el usuario pulsa `Nuevo documento`
- **THEN** se reutiliza la confirmacion existente de la operacion `NEW`
- **AND** al continuar se ejecuta `captureOperation.type = "NEW"`

#### Scenario: Compatibilidad de acciones restantes
- **GIVEN** el documento contiene paginas capturadas
- **WHEN** el usuario usa `Reemplazar`, `Insertar` o `Agregar`
- **THEN** esas operaciones mantienen su comportamiento existente y su contrato `captureOperation`
