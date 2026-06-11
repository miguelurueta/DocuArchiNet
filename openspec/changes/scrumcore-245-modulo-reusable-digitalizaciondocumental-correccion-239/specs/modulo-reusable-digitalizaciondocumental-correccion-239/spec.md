## ADDED Requirements

### Requirement: Workspace inline de digitalizacion documental

El sistema SHALL exponer un componente React reutilizable que renderice el digitalizador documental sin depender de `AppModal`.

#### Scenario: Render inline sin overlay

- **GIVEN** un modulo consumidor como `CapDocument`
- **WHEN** renderiza `DigitalizacionDocumentalWorkspace`
- **THEN** el componente se monta dentro del contenedor padre
- **AND** no crea un dialog/modal overlay
- **AND** no renderiza `AppModal`.

#### Scenario: Funcionalidad operativa conservada

- **GIVEN** `DigitalizacionDocumentalWorkspace` recibe un `DigitalizacionContext` valido y un `DigitalizacionScannerClient`
- **WHEN** el usuario opera el digitalizador
- **THEN** puede inicializar scanner, seleccionar dispositivo, capturar paginas, ver miniaturas, ver preview, rotar/eliminar paginas y generar PDF
- **AND** puede completar flujo crear o adjuntar mediante las APIs existentes.

### Requirement: Wrapper modal compatible

El sistema SHALL conservar `DigitalizacionDocumentalModal` para consumidores que requieran overlay.

#### Scenario: Modal usa workspace interno

- **GIVEN** un consumidor renderiza `DigitalizacionDocumentalModal` con `open=true`
- **WHEN** el componente se monta
- **THEN** renderiza `AppModal`
- **AND** dentro del modal monta `DigitalizacionDocumentalWorkspace`
- **AND** conserva callbacks `onClose`, `onCompleted` y `onError`.

### Requirement: Trazabilidad legacy SCRUMCORE-239

El sistema SHALL documentar equivalencias entre archivos legacy y la arquitectura React/API moderna.

#### Scenario: Matriz creada

- **WHEN** se revisa la documentacion de `DigitalizacionDocumental`
- **THEN** existe `docs/Architecture/DigitalizacionDocumental/SCRUMCORE-239-legacy-traceability.md`
- **AND** el documento incluye responsabilidades legacy, regla funcional, nueva ubicacion React/API, estado y evidencia.

#### Scenario: Bloqueos documentados

- **WHEN** un archivo legacy requerido no esta accesible
- **THEN** la trazabilidad indica ruta, motivo e impacto
- **AND** no marca como completadas las reglas que dependan exclusivamente de ese archivo.

## MODIFIED Requirements

### Requirement: Componente corporativo DigitalizacionDocumental

El sistema SHALL usar `DigitalizacionDocumentalWorkspace` como componente corporativo embebible en modulos como `CapDocument`, `Correspondencia`, `Workflow`, `Ventanilla` y `Archivo Central`.

#### Scenario: Uso en panel de layout

- **GIVEN** un layout con panel izquierdo de 70%
- **WHEN** el modulo renderiza `DigitalizacionDocumentalWorkspace`
- **THEN** el digitalizador ocupa el espacio del contenedor padre
- **AND** no bloquea el resto de la pantalla con overlay modal.
