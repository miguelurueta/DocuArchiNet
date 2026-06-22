## ADDED Requirements

### Requirement: Auditoria Y Diseno Tecnico OCR Por Zona
El sistema SHALL documentar el alcance tecnico para OCR por zona sin implementar OCR funcional hasta confirmar licencia, runtime e idiomas soportados.

#### Scenario: Documento tecnico creado
- **WHEN** se revisa la arquitectura de DigitalizacionDocumental
- **THEN** existe `docs/Architecture/DigitalizacionDocumental/SCRUMCORE-277-ocr-zona.md`
- **AND** el documento incluye arquitectura, flujo, dependencias, riesgos y casos de uso.

#### Scenario: No se implementa OCR funcional sin confirmacion
- **WHEN** SCRUMCORE-260 se completa
- **THEN** no se agrega una accion funcional `OCR Zona` al toolbar
- **AND** no se agrega una segunda seleccion de area
- **AND** no se agregan librerias OCR ni assets de idiomas al runtime frontend.

#### Scenario: Reutilizacion de seleccion existente
- **WHEN** se disene la implementacion futura
- **THEN** el OCR por zona SHALL reutilizar `PageCropSelection` y el `pageId` activo de la seleccion visual existente
- **AND** el flujo futuro SHALL operar solo sobre la region seleccionada.

#### Scenario: Compuerta de capacidades
- **WHEN** se habilite una implementacion futura de OCR por zona
- **THEN** la UI SHALL depender de una capacidad confirmada de OCR
- **AND** la capacidad SHALL validar licencia, API runtime, idiomas y rendimiento esperado.

### Requirement: Detalle funcional Jira
El sistema SHALL considerar las reglas detalladas del ticket.

#### Scenario: Reglas del ticket
- OCR POR ZONA (EXTRACCION DE TEXTO DESDE AREA SELECCIONADA)
- Reutilizar seleccion de area existente de SCRUMCORE-269.
- No crear un segundo mecanismo de seleccion.
- Disenar boton futuro `OCR Zona` con tooltip `Extraer texto de la seleccion`.
- Si no existe seleccion, el boton futuro debe estar deshabilitado con tooltip `Seleccione un area primero`.
- Disenar resultado futuro en modal lateral o drawer con `Texto extraido`, `Copiar`, `Insertar en metadato` y `Cerrar`.
- Preparar soporte posterior para OCR multiple, OCR por varias regiones, extraccion automatica de metadatos e IA documental.
- No implementar OCR funcional antes de auditar licencia, disponibilidad de OCR, APIs, idiomas y rendimiento.
