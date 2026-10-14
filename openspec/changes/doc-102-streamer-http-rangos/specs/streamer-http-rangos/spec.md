<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09 -->
## Purpose

Permitir que el visor PDF enriquecido obtenga documentos privados mediante una referencia autorizada y rangos HTTP, reduciendo memoria, bytes iniciales y tiempo a primera página sin romper consumidores WebForms existentes.

## ADDED Requirements

### Requirement: Referencia documental autorizada
El sistema SHALL emitir para el visor enriquecido una referencia opaca, protegida, expirable y ligada a la sesión que autorizó el documento, y SHALL NOT aceptar una ruta física proporcionada por el navegador como autoridad del contrato nuevo. <!-- RQ-01; D-01 -->

#### Scenario: Referencia válida de la misma sesión
- **WHEN** el handler recibe una referencia intacta, vigente y emitida para la sesión actual
- **THEN** resuelve el documento autorizado sin exponer su ruta física y continúa con la respuesta HTTP

#### Scenario: Referencia inválida, expirada o ajena
- **WHEN** la referencia fue alterada, expiró o pertenece a otra sesión
- **THEN** el handler rechaza la solicitud con una respuesta pública genérica, sin abrir el archivo ni revelar ruta o detalle interno

### Requirement: Contrato de métodos y rango único
El streamer SHALL admitir `GET` y `HEAD` y SHALL procesar como máximo un rango de bytes por solicitud con límites calculados sobre la longitud vigente del archivo. <!-- RQ-02; D-02 -->

#### Scenario: Documento completo y HEAD
- **WHEN** una referencia autorizada se solicita sin `Range` mediante `GET` o `HEAD`
- **THEN** responde `200` con metadatos y longitud completos, y `HEAD` omite el cuerpo

#### Scenario: Rangos soportados
- **WHEN** `Range` contiene un único intervalo cerrado, abierto o de sufijo válido
- **THEN** responde `206`, `Accept-Ranges: bytes`, `Content-Range` y `Content-Length` exactos y transmite solo el intervalo resuelto

#### Scenario: Rango inválido o múltiple
- **WHEN** el rango tiene sintaxis inválida, múltiples intervalos, sufijo cero, inicio fuera del archivo o fin anterior al inicio
- **THEN** responde `416` con `Content-Range: bytes */<length>`, sin cuerpo ni información de ruta

### Requirement: Revalidación privada autorizada
El streamer SHALL emitir validadores opacos y directivas de caché privada que requieran revalidación y SHALL NOT convertir un acierto de caché en autorización para otra sesión o documento. <!-- RQ-03; D-03 -->

#### Scenario: Revalidación de representación completa
- **WHEN** `If-None-Match` o, en su ausencia, `If-Modified-Since` valida la representación autorizada actual
- **THEN** responde `304` sin cuerpo y conserva encabezados de caché privada

#### Scenario: If-Range coincide
- **WHEN** una solicitud parcial contiene un `If-Range` que coincide con el ETag fuerte o la fecha vigente
- **THEN** el servidor conserva el rango y responde `206`

#### Scenario: If-Range no coincide
- **WHEN** `If-Range` no coincide o no es válido
- **THEN** el servidor ignora `Range` y responde `200` con la representación completa autorizada

### Requirement: Streaming con memoria acotada
El handler SHALL copiar desde `FileStream` mediante un buffer fijo y SHALL NOT usar `ClassRaEnvioCorrespondencia.ReadFile`, un arreglo del tamaño del documento, `BinaryWrite` ni cambios de atributos para servir el cuerpo. <!-- RQ-04; D-04 -->

#### Scenario: Transferencia completa o parcial
- **WHEN** se transmite una respuesta `200` o `206`
- **THEN** el número de bytes escritos coincide con `Content-Length` y la memoria de copia no crece con el tamaño total del archivo

#### Scenario: Desconexión o excepción de E/S
- **WHEN** el cliente se desconecta o la lectura falla durante la transferencia
- **THEN** el stream se cierra determinísticamente, no se filtra la excepción y no se altera el documento

### Requirement: Concurrencia sin mutación de sesión
El handler SHALL adquirir sesión en modo de solo lectura y SHALL NOT modificar sesión, tarea, documento ni auditoría al responder `GET`, `HEAD`, `206`, `304` o `416`. <!-- RQ-05; D-05 -->

#### Scenario: Rangos concurrentes
- **WHEN** PDF.js solicita dos o más rangos autorizados de la misma sesión
- **THEN** las solicitudes pueden progresar sin el bloqueo exclusivo de `IRequiresSessionState` y cada una devuelve su intervalo correcto

#### Scenario: Lectura sin efectos persistentes
- **WHEN** se completa o rechaza una solicitud del streamer
- **THEN** los valores de sesión y los controles `SELECT` de tarea, estado y auditoría permanecen sin cambios

### Requirement: Apertura directa y adaptativa desde PDF.js
El visor enriquecido SHALL entregar la URL autorizada a `pdfjsLib.getDocument`, SHALL seleccionar rangos para PDF grandes linearizados y streaming secuencial para PDF no linearizados, pequeños o indeterminados, y SHALL NOT ejecutar la cadena previa `fetch -> Blob -> FileReader -> Uint8Array` para abrir el PDF. <!-- RQ-06; D-06 -->

#### Scenario: PDF grande linearizado
- **WHEN** el servidor detecta el diccionario `/Linearized` al inicio de un PDF de al menos 1 MiB
- **THEN** PDF.js negocia rangos de 256 KiB, deshabilita streaming progresivo y precarga automática, y solicita los bytes requeridos para la vista activa

#### Scenario: PDF no linearizado, pequeño o indeterminado
- **WHEN** el servidor no confirma linearización, el PDF mide menos de 1 MiB o la inspección falla
- **THEN** PDF.js deshabilita Range y consume una única respuesta `200` transmitida secuencialmente por el handler, sin materializar el documento completo en JavaScript ni en memoria del servidor

#### Scenario: Acciones del visor preservadas
- **WHEN** el usuario autorizado usa scroll, zoom, rotación, anotación, firma, impresión o descarga
- **THEN** cada acción conserva sus permisos y contrato existente, incluido el identificador de ruta legacy que todavía requieren los servicios de escritura o salida

#### Scenario: Refresco después del pegado de grafo
- **WHEN** iText guarda el grafo y devuelve la URL del documento actualizado
- **THEN** el visor refresca solo la página afectada mediante streaming completo compatible, sin reinicializar el visor ni cambiar `url_document`, coordenadas, permisos o payload del servicio de anotación

### Requirement: Compatibilidad legacy delimitada
El sistema SHALL mantener temporalmente el parámetro `rut_image` para consumidores simples inventariados, sin otorgarle el contrato nuevo de rangos, referencias autorizadas o caché revalidada, y SHALL transmitir su respuesta sin materializar el archivo completo. <!-- RQ-07; D-07 -->

#### Scenario: Consumidor simple existente
- **WHEN** un visor de descarga, formato no PDF, versión, migración o rama `LIBRE_PDF` usa el contrato legacy
- **THEN** conserva su comportamiento observable de cuerpo, MIME y disposición sin usar `ReadFile` ni `BinaryWrite`

#### Scenario: Separación de contratos
- **WHEN** una solicitud legacy intenta usar encabezados Range o una solicitud nueva omite `doc_ref`
- **THEN** no obtiene las capacidades del streamer autorizado y los constructores enriquecidos no retroceden a una ruta física como autoridad

### Requirement: Evidencia integral y saneada
DOC-102 SHALL verificar contrato HTTP, seguridad, compilación, compatibilidad y beneficio de carga mediante pruebas reproducibles y una E2E autorizada reutilizando `tools/e2e`. <!-- RQ-08; D-08 -->

#### Scenario: Validación automatizada
- **WHEN** se valida la entrega
- **THEN** pruebas focales cubren archivo completo, rangos cerrado/abierto/sufijo/límite, `416`, rango múltiple, `HEAD`, validadores, MIME, desconexión, referencia alterada/ajena/expirada, traversal y consumidores simples, y MSBuild termina correctamente

#### Scenario: E2E autorizada
- **WHEN** existe autorización expresa para ambiente, cuenta y documento de prueba
- **THEN** la evidencia saneada registra primer canvas, bytes transferidos, solicitudes Range, estados y encabezados sin credenciales, cookies, tokens, rutas ni cadenas de conexión

#### Scenario: E2E no autorizada
- **WHEN** falta autorización o un recurso descartable válido
- **THEN** no se ejecuta la prueba real, se registra el bloqueo y no se sustituyen resultados con mocks o evidencia inventada

### Requirement: Prueba descartable de linearización
El cambio SHALL demostrar localmente el efecto de Fast Web View sobre una copia temporal sintética, sin reemplazar el archivo fuente, modificar almacenamiento, autenticar contra un ambiente ni incorporar la herramienta de conversión al runtime de la aplicación. <!-- RQ-09; D-09 -->

#### Scenario: Comparación controlada con PDF.js
- **WHEN** el runner crea desde un fixture público una línea base mayor de 1 MiB y una variante linearizada
- **THEN** sirve ambas por HTTP Range al PDF.js existente, registra bytes y solicitudes hasta el primer canvas y confirma igual cantidad de páginas y render equivalente de la primera página

#### Scenario: Aislamiento y limpieza
- **WHEN** la prueba termina con éxito o error
- **THEN** elimina su directorio temporal, no reemplaza el fixture y solo emite métricas saneadas sin rutas, contenido documental ni datos de sesión
