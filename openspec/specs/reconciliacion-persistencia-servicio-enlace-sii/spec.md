# reconciliacion-persistencia-servicio-enlace-sii Specification

## Purpose

Definir la preparación, persistencia idempotente y reconciliación verificable de uno o varios anexos SII para tareas ENLASE, sin asignar ni cerrar la tarea.

## Requirements

### Requirement: RQ-01 Preparación de selección ENLASE

Decisión de origen: D-01.

El sistema SHALL preparar uno o varios anexos SII mediante el mismo contrato de colección, validando para cada elemento una tipología perteneciente al catálogo autorizado de la tarea y el checklist. Solo SHALL aplicar una tipología predeterminada cuando la resolución sea única e inequívoca.

#### Scenario: Selección múltiple válida

- **WHEN** el usuario autorizado prepara varios anexos con identidades externas distintas y tipologías válidas
- **THEN** el preflight devuelve un comando normalizado por elemento y un único contexto ejecutable sin realizar escrituras documentales

#### Scenario: Tipología ausente o ambigua

- **WHEN** falta una tipología obligatoria o existen varias candidatas para el valor predeterminado
- **THEN** el preflight bloquea la selección completa, solicita selección manual y no crea efectos documentales

### Requirement: RQ-02 Intención única, idempotencia y concurrencia

Decisión de origen: D-02.

El sistema SHALL crear o reutilizar una única intención por selección normalizada. La identidad idempotente SHALL incluir el contexto inmutable, proveedor, capacidad e identidades externas y SHALL excluir URLs temporales. La ejecución SHALL estar protegida contra concurrencia.

#### Scenario: Repetición de intención

- **WHEN** se repite una solicitud con la misma clave y huella canónica
- **THEN** el sistema devuelve la intención existente y no crea otro documento

#### Scenario: Solicitudes concurrentes

- **WHEN** dos solicitudes intentan ejecutar la misma selección al mismo tiempo
- **THEN** como máximo una alcanza el almacenamiento y la otra obtiene la intención/resultados idempotentes o un bloqueo seguro

### Requirement: RQ-03 Revalidación autoritativa del contexto

Decisión de origen: D-03.

El sistema SHALL reconstruir y revalidar en servidor usuario, tarea, actividad/ruta, trámite, proveedor y capacidad `ANEXOS_RADICADO_ENLASE` inmediatamente antes del primer efecto.

#### Scenario: Contexto cambió después del preflight

- **WHEN** la tarea dejó de ser operable o cambió cualquiera de las dimensiones autorizadas
- **THEN** la ejecución termina con un código seguro, sin descargar ni persistir documentos

#### Scenario: Datos de autoridad manipulados

- **WHEN** el cliente envía tarea, ruta, permiso o proveedor que no coincide con el contexto de sesión y repositorio
- **THEN** el sistema ignora la autoridad del cliente y rechaza la operación

### Requirement: RQ-04 Almacenamiento legacy encapsulado

Decisión de origen: D-04.

El sistema SHALL descargar y validar cada recurso mediante el proveedor moderno y SHALL invocar la ruta de almacenamiento existente únicamente a través de un adaptador de `IImportDocumentStoragePort`. El adaptador SHALL traducir el resultado legacy a un resultado estructurado y saneado; no SHALL considerar `YES` como confirmación final.

#### Scenario: Descarga inválida

- **WHEN** el proveedor no entrega contenido permitido por tamaño, formato o contenido
- **THEN** el elemento queda fallido antes de persistencia y los siguientes estados reflejan que no hubo documento confirmado

#### Scenario: Función legacy retorna YES

- **WHEN** `PreAlmacenaDocumentoAnexosEnlaceIntegracionSII` retorna `YES` e informa un ID interno
- **THEN** el sistema avanza a verificación autoritativa sin anunciar todavía el elemento como importado

#### Scenario: Error legacy

- **WHEN** la función legacy retorna un texto de error o lanza una excepción
- **THEN** el adaptador devuelve código y mensaje público saneados sin exponer ruta física, credenciales ni excepción cruda

### Requirement: RQ-05 Confirmación lógica y física

Decisión de origen: D-05.

El sistema SHALL considerar importado un anexo solo cuando la identidad externa, el registro lógico, la relación con la tarea y el recurso físico coincidan. El resultado confirmado SHALL conservar el identificador interno autorizado para refrescar la lista documental.

#### Scenario: Evidencia completa

- **WHEN** el registro, la relación con la tarea y el archivo físico existen una sola vez para la identidad externa
- **THEN** el elemento queda `Disponible` y una repetición se omite idempotentemente

#### Scenario: Registro sin archivo físico

- **WHEN** existe la asociación lógica pero el recurso físico ya no existe
- **THEN** el elemento queda recuperable y puede ser importado de nuevo mediante una ejecución controlada, sin tratar el registro huérfano como documento válido

#### Scenario: Evidencia contradictoria

- **WHEN** existen relaciones duplicadas o no puede determinarse si el efecto físico ocurrió
- **THEN** el elemento queda `Inconsistente` o `ResultadoIncierto`, sin exponer un documento como confirmado

### Requirement: RQ-06 Resultado por elemento y reconciliación

Decisión de origen: D-06.

El sistema SHALL persistir y devolver el resultado de cada elemento y SHALL calcular el estado agregado sin ocultar elementos fallidos, inciertos o no procesados. Un resultado incierto SHALL requerir reconciliación explícita y no SHALL reintentarse automáticamente.

#### Scenario: Resultado parcial

- **WHEN** algunos anexos quedan confirmados y otros fallan o quedan inciertos
- **THEN** la respuesta es `Parcial` y presenta de forma diferenciada el resultado seguro de cada elemento

#### Scenario: Respuesta perdida después del efecto

- **WHEN** la persistencia pudo ocurrir pero no existe una respuesta confirmada
- **THEN** la intención queda `ResultadoIncierto` y `ReconcileImportIntent` consulta evidencia por intención, contexto e identidad externa antes de resolverla

#### Scenario: Todos los elementos confirmados

- **WHEN** todos los anexos tienen evidencia lógica y física válida
- **THEN** el estado agregado es `Completado` y cada documento aparece una sola vez

### Requirement: RQ-07 Compatibilidad, seguridad y pruebas

Decisión de origen: D-07.

El sistema SHALL mantener fuera de la importación la asignación y cierre de tarea, SHALL preservar los contratos de constancias y de la ruta legacy, y SHALL producir evidencia de prueba saneada. Las pruebas E2E mutadoras SHALL requerir autorización explícita y datos descartables.

#### Scenario: Finalización de importación

- **WHEN** una intención ENLASE termina completa, parcial o fallida
- **THEN** la tarea conserva su asignación/estado previo y no se ejecutan efectos de expediente propios de constancias

#### Scenario: Validación automatizada

- **WHEN** se ejecutan las suites focales
- **THEN** cubren preflight, tipología, idempotencia, concurrencia, adaptación legacy, existencia física, reconciliación y regresión de constancias

#### Scenario: E2E sin autorización

- **WHEN** se intenta una corrida real que puede mutar documentos sin autorización explícita
- **THEN** la plataforma se detiene antes de modificar configuración de rollout o realizar escrituras
