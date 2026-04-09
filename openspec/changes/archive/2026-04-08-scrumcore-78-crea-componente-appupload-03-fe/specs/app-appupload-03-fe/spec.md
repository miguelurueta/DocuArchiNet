## ADDED Requirements

### Requirement: Telemetry desacoplada
AppUpload SHALL exponer `onTelemetry(event)` para reportar eventos de carga sin acoplarse a SDKs externos.

#### Scenario: Emision de evento upload_success
- **WHEN** un archivo termina en estado `done`
- **THEN** se emite un evento `upload_success` con metadatos basicos

### Requirement: Documentacion del componente
AppUpload SHALL documentar su API y ejemplos de uso en un README asociado.

#### Scenario: Ejemplo de estrategia manual
- **WHEN** un consumidor consulta el README
- **THEN** encuentra un ejemplo funcional de uso manual

### Requirement: Accesibilidad avanzada
AppUpload SHALL garantizar `aria-label` en acciones, focus visible y soporte teclado completo.

#### Scenario: Acciones accesibles
- **WHEN** el usuario navega con teclado
- **THEN** puede activar preview y eliminar archivos sin mouse
