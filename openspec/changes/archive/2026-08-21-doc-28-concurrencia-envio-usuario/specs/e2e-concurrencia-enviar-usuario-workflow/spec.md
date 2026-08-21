## Purpose

Proporcionar evidencia E2E autorizada de que dos envíos simultáneos a usuario no duplican la transición de una tarea Workflow descartable.

## ADDED Requirements

### Requirement: Carrera DOC-28 autorizada y acotada

La automatización SHALL ejecutar exactamente dos solicitudes simultáneas de `EjecutarEnvioUsuario` únicamente cuando se declaren autorización de ejecución y autorización específica de concurrencia, una tarea descartable, una cuenta Gestión válida y controles MySQL de solo lectura.

#### Scenario: Falta autorización de concurrencia

- **WHEN** no se declara la autorización exacta de concurrencia
- **THEN** el comando falla antes de abrir navegador, autenticar o enviar HTTP.

#### Scenario: Dos solicitudes sobre el preview vigente

- **WHEN** una tarea descartable tiene un preview vigente con destino y token válidos
- **THEN** la automatización deriva esos valores del preview y emite exactamente dos solicitudes concurrentes sin aceptar destino ni token como configuración externa.

### Requirement: Exclusión mutua observable

La carrera SHALL confirmar que una sola solicitud completa la transición y que la otra devuelve un bloqueo funcional seguro permitido por la política de concurrencia.

#### Scenario: Una ganadora y una bloqueada

- **WHEN** ambas solicitudes llegan con el mismo token vigente
- **THEN** exactamente una respuesta es exitosa y la otra devuelve `WORKFLOW_TRANSITION_IN_PROGRESS`, `WORKFLOW_VERSION_CONFLICT` o `WORKFLOW_TASK_UNAVAILABLE`.

#### Scenario: Estado posterior sin duplicación

- **WHEN** finaliza la carrera autorizada
- **THEN** las huellas de estado muestran una única transición efectiva y la evidencia confirma una modificación de auditoría sin registrar cuerpos de respuesta.

### Requirement: Evidencia segura y sin carga masiva

La automatización SHALL conservar solo métricas agregadas, códigos funcionales, banderas de estado y huellas; SHALL preservar gate apagado y flujo legacy sin cambios, y SHALL no exponer un comando de carga masiva DOC-28.

#### Scenario: Cierre de carrera

- **WHEN** termina una corrida de concurrencia autorizada o bloqueada por configuración
- **THEN** no se guardan secretos, cookies, token, destinos ni cadenas de conexión, y se verifica que el gate permanezca apagado y sin alcance.
