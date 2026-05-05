## ADDED Requirements

### Requirement: Integración visual del visor en DocumentosWorkbench
El sistema SHALL integrar `AppVisorEmbedPdf` en `DocumentosWorkbench` como panel/área de visualización de documentos.

#### Scenario: Workbench muestra un panel de visor
- **WHEN** el usuario entra a `DocumentosWorkbench`
- **THEN** existe un área visible destinada al visor de PDF (aunque no haya documento seleccionado)

### Requirement: DocumentosWorkbench no conoce EmbedPDF
`DocumentosWorkbench` MUST NOT importar `@embedpdf/*` ni implementar lógica de engine/plugins; SHALL consumir únicamente el componente reusable.

#### Scenario: Consumo desacoplado del engine
- **WHEN** se revisan imports del módulo `DocumentosWorkbench`
- **THEN** no existen imports desde `@embedpdf/*`

### Requirement: Selección de documento actualiza el visor
El sistema SHALL renderizar `AppVisorEmbedPdf` dentro de `DocumentosWorkbench` sin listado/selección de documentos en esta iteración (02-FE).

#### Scenario: Render simple con demo PDF por defecto
- **WHEN** el usuario entra a `DocumentosWorkbench`
- **THEN** `DocumentosWorkbench` renderiza `<AppVisorEmbedPdf />` sin proveer `fileUrl` y el visor utiliza su demo local predeterminado

### Requirement: Tests de integración con visor mockeado
Los tests del workbench SHALL validar la integración sin depender de engine real, mockeando `AppVisorEmbedPdf`.

#### Scenario: Tests no usan WASM/engine real
- **WHEN** se ejecutan tests de `DocumentosWorkbench`
- **THEN** el visor es un mock/stub y los tests validan únicamente el wiring/estado de UI
