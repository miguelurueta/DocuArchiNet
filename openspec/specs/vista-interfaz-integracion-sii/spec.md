# vista-interfaz-integracion-sii Specification

## Purpose

TBD: definir el propósito estable de la vista segura de recursos externos después del archivo.

## Requirements

### Requirement: Preview mediado por descriptor

El sistema SHALL solicitar el recurso por proveedor e identidad externa y consumirlo únicamente mediante el descriptor emitido por backend. Origen: D-01, RQ-01.

#### Scenario: Apertura segura

- **WHEN** el usuario abre un recurso SII no importado
- **THEN** la UI invoca `GetPreview` mediante el cliente existente
- **AND** obtiene contenido del handler same-origin usando `DescriptorId`
- **AND** ignora cualquier URL externa de la fila

#### Scenario: Datos inseguros ausentes

- **WHEN** se renderiza preview o error
- **THEN** no se muestran ni registran tokens, rutas físicas, respuestas completas o URLs técnicas

### Requirement: Estado determinista y renovación explícita

El sistema SHALL modelar el preview sin repetir la descarga por foco, resize o layout. Origen: D-02, RQ-02.

#### Scenario: Una solicitud por apertura

- **WHEN** cambia el foco o tamaño con el preview abierto
- **THEN** se conserva la solicitud o descriptor actual
- **AND** no se descarga nuevamente

#### Scenario: Descriptor vencido

- **WHEN** el backend informa expiración
- **THEN** se presenta `recurso-vencido`
- **AND** se puede solicitar otro descriptor sin mutar tarea, estado o auditoría

#### Scenario: Formato no visualizable

- **WHEN** el MIME permitido no puede embeberse
- **THEN** se presenta el fallback y la descarga usa el mismo handler seguro

### Requirement: Panel accesible y responsive

El sistema SHALL presentar panel lateral en escritorio y subvista en pantallas pequeñas, preservando contexto. Origen: D-03, RQ-03.

#### Scenario: Abrir y volver

- **WHEN** el usuario abre y luego usa `Volver a la lista`
- **THEN** se restauran selección, filtros, scroll y foco

#### Scenario: Navegación por teclado

- **WHEN** el usuario opera con teclado
- **THEN** los estados se anuncian y cerrar devuelve el foco al control de origen

### Requirement: Separación del documento importado

El sistema SHALL distinguir la vista temporal y reutilizar el visor vigente solo con identidad interna autorizada. Origen: D-04, RQ-04.

#### Scenario: Documento reconciliado

- **WHEN** existe identificador interno autorizado
- **THEN** se habilita `Ver documento importado` mediante el visor existente

#### Scenario: Documento no reconciliado

- **WHEN** no existe identificador interno autorizado
- **THEN** la acción no está disponible y el preview no se presenta como almacenado

### Requirement: Fallo cerrado

El sistema SHALL bloquear la vista si el mediador, B10, gate o proveedor no están disponibles. Origen: D-05, RQ-05.

#### Scenario: Dependencia ausente

- **WHEN** la dependencia no está disponible
- **THEN** se muestra estado bloqueado o proveedor no disponible
- **AND** no se usa URL externa, contenido simulado ni bytes JSON/base64

#### Scenario: Acceso no autorizado

- **WHEN** backend rechaza el acceso
- **THEN** se muestra `no-autorizado` sin detalles internos

### Requirement: Cambios acotados y verificables

La implementación SHALL respetar rutas canónicas y proteger superficies legacy. Origen: D-06, RQ-06.

#### Scenario: Revisión del diff

- **WHEN** se revisa DOC-74
- **THEN** los módulos nuevos son `importar-servicio-web-preview.js` y `importar-servicio-web-preview-state.js`
- **AND** markup, CSS, code-behind y `.vbproj` solo reciben integración aditiva
- **AND** no cambian almacenamiento ni visores existentes

#### Scenario: Evidencia focal

- **WHEN** se valida la implementación
- **THEN** pruebas funcionales, seguridad y accesibilidad cubren foco, cierre, expiración y fallback
- **AND** la evidencia se registra sin secretos
