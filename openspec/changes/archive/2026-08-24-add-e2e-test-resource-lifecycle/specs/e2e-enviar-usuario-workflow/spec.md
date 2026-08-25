## MODIFIED Requirements

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
