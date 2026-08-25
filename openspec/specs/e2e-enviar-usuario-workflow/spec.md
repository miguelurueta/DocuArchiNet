# e2e-enviar-usuario-workflow Specification

## Purpose

Definir una validación E2E reproducible y segura que pruebe el envío Workflow a usuario con sesiones reales autorizadas, sin confundir la evidencia estática con una ejecución integrada.

## Requirements

### Requirement: Preview E2E específico y seguro

La automatización SHALL invocar el contrato de preview de envío a usuario con una sesión Gestión autorizada y validar tanto el borde sin sesión como los parámetros inválidos, sin exponer destinos ante un bloqueo.

#### Scenario: Solicitud anónima

- **WHEN** se solicita el preview de envío a usuario sin una sesión Gestión válida
- **THEN** la respuesta devuelve el bloqueo de contexto seguro y no contiene destinos ni información interna.

#### Scenario: Parámetros inválidos autenticados

- **WHEN** una sesión Gestión válida solicita el preview con tarea, filtro, cursor o tamaño de página inválidos
- **THEN** la respuesta devuelve el código funcional aplicable, no contiene SQL ni detalles de excepción y no produce una transición.

### Requirement: Prueba de preview sin mutación verificable

La prueba de preview autenticada SHALL exigir un ambiente autorizado y controles de estado y auditoría exclusivamente de lectura, y SHALL comparar sus huellas antes y después de la solicitud.

#### Scenario: Destinos autorizados paginados

- **WHEN** una tarea activa tiene más destinos usuario–actividad autorizados que el tamaño de página solicitado
- **THEN** el preview entrega una página limitada, cursor siguiente y datos mínimos de destino, mientras las huellas de estado y auditoría permanecen iguales.

#### Scenario: Bloqueo funcional conocido

- **WHEN** una tarea autorizada está configurada para producir un bloqueo funcional esperado
- **THEN** la prueba registra el código esperado, no recibe destinos y confirma que las huellas permanecen iguales.

### Requirement: Ejecución E2E explícitamente autorizada

La automatización SHALL mantener la ejecución de envío a usuario deshabilitada por defecto y exigir autorización explícita, un recurso Workflow descartable y reservado, un destino obtenido del preview actual y consultas de control exclusivamente `SELECT` antes de enviar una transición. El recurso SHALL superar sus prerrequisitos registrados antes de la etapa mutante y su reserva SHALL liberarse o restaurarse durante el cierre de la corrida.

#### Scenario: Falta autorización de mutación

- **WHEN** no se declara autorización explícita para ejecutar sobre un recurso Workflow descartable
- **THEN** el comando de ejecución falla antes de abrir una sesión o enviar una solicitud que pueda cambiar Workflow.

#### Scenario: Recurso Workflow no disponible

- **WHEN** la tarea descartable no cumple sus prerrequisitos o ya está reservada por otra corrida
- **THEN** el preflight falla antes de enviar una transición y conserva únicamente un código saneado de disponibilidad.

#### Scenario: Ejecución autorizada sobre recurso reservado

- **WHEN** se autoriza explícitamente un recurso Workflow descartable reservado, con token y destino actuales obtenidos del preview
- **THEN** la prueba verifica el resultado funcional esperado y las huellas de estado y auditoría correspondientes, libera o restaura el recurso al cierre y no modifica el gate ni el flujo legacy.

### Requirement: Evidencia libre de secretos y cierre seguro

La automatización SHALL recibir credenciales y conexiones solo mediante variables efímeras. Podrá reutilizar un perfil externo únicamente para campos no sensibles validados por el contrato de su DOC; SHALL rechazar perfiles que incluyan secretos, cookies, cadenas de conexión o autorizaciones. SHALL generar evidencia resumida que no contenga secretos, cookies, cadenas de conexión ni cuerpos de respuesta completos.

#### Scenario: Perfil no sensible reutilizable

- **WHEN** una corrida de envío a usuario recibe un perfil externo con solo los campos permitidos
- **THEN** reutiliza esos campos sin persistir credenciales ni autorizaciones y solicita los secretos efímeros de forma segura.

#### Scenario: Cierre de una corrida autorizada

- **WHEN** termina una corrida de preview o ejecución autorizada
- **THEN** se conserva únicamente resultado, códigos, conteos y huellas, y se verifica que el gate permanezca apagado y sus listas vacías.
