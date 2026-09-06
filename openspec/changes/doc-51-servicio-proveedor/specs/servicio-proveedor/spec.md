## ADDED Requirements

### Requirement: RQ-01 Puerto externo asíncrono y aditivo (D-01)
El sistema SHALL declarar un único `IExternalImportProviderClient` en Modelo para capacidades, consulta, preview y descarga, usando `Task` y `CancellationToken`, sin alterar el puerto legacy ni los DTO v1.

#### Scenario: Contrato disponible
- **WHEN** se compila DOC-51
- **THEN** las cuatro operaciones son tipadas y cancelables y el puerto no está duplicado

### Requirement: RQ-02 Fidelidad del contrato HTTP (D-02)
El transporte SHALL preservar método, encabezados, cuerpo y media type preparados, sin convertir JSON y formulario.

#### Scenario: Formatos por operación
- **WHEN** se envían solicitudes JSON y formulario
- **THEN** loopback recibe sus bytes y media type sin transformación

### Requirement: RQ-03 Configuración y TLS seguros (D-03)
La fábrica SHALL mantener configuración estable y usar validación TLS del sistema sin callbacks globales permisivos.

#### Scenario: Políticas simultáneas
- **WHEN** solicitudes tienen timeout, encabezados y límites propios
- **THEN** ninguna modifica defaults compartidos ni certificados globales

### Requirement: RQ-04 Cancelación y timeout deterministas (D-04)
El transporte SHALL aceptar cancelación externa y timeout por operación y mapearlos a resultados distintos.

#### Scenario: Cancelación solicitada
- **WHEN** el llamador cancela una respuesta demorada
- **THEN** retorna `EXTERNAL_CANCELLED` y no timeout

#### Scenario: Timeout vencido
- **WHEN** vence el límite sin cancelación externa
- **THEN** retorna `EXTERNAL_TIMEOUT` y detiene la lectura

### Requirement: RQ-05 Respuesta validada y acotada (D-05)
El validador SHALL rechazar status no exitoso, media type no permitido, contenido inválido y cuerpo mayor al máximo antes de entregar datos.

#### Scenario: Longitud declarada excedida
- **WHEN** `Content-Length` supera el máximo
- **THEN** rechaza sin leer el cuerpo completo

#### Scenario: Stream excedido
- **WHEN** el stream supera el máximo sin longitud confiable
- **THEN** detiene la lectura y retorna `EXTERNAL_INVALID_RESPONSE`

#### Scenario: Contenido inválido
- **WHEN** MIME o JSON no cumple el contrato
- **THEN** no entrega un DTO parcial

### Requirement: RQ-06 Errores y trazas saneados (D-06)
El mapper SHALL distinguir acceso denegado, timeout, cancelación, indisponibilidad y respuesta inválida con código seguro y `correlationId`.

#### Scenario: Error sensible
- **WHEN** excepción o respuesta contiene URL, token, ruta o cuerpo
- **THEN** mensajes públicos y registrables omiten esos valores

### Requirement: RQ-07 Preview mediado (D-07)
La frontera SHALL entregar solo contenido validado y metadatos para el descriptor v1, sin autoridad externa.

#### Scenario: Preview exitoso
- **WHEN** el proveedor devuelve un recurso permitido
- **THEN** el resultado no contiene URL externa, token, ruta ni respuesta cruda

### Requirement: RQ-08 Pruebas locales y compatibilidad (D-08)
La validación SHALL usar fixtures saneados y servidor loopback, sin SII, credenciales, E2E autenticado ni gate.

#### Scenario: Suite aislada
- **WHEN** corren pruebas de contrato, seguridad y cancelación
- **THEN** cubren éxito, error, timeout, cancelación, contenido inválido y tamaño sin red externa

#### Scenario: Legacy intacto
- **WHEN** se revisa el diff
- **THEN** transporte legacy, endpoints, almacenamiento y gate permanecen sin cambios
