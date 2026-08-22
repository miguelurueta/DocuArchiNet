<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## Purpose

Define la decisión y el procedimiento de liberación controlada para la capacidad moderna de Enviar a usuario, sin convertir la evidencia técnica en una autorización de ambiente.

## ADDED Requirements

### Requirement: Decisión operativa explícita

La liberación SHALL expresar una única decisión entre bloquear, solicitar aprobación o lista para despliegue autorizado.

#### Scenario: Evidencia técnica aprobada sin autorización de ambiente

- **WHEN** DOC-30 está aprobado y no existe ambiente, ventana ni responsables autorizados
- **THEN** la decisión es `solicitar aprobación operativa` y no se inicia despliegue.

Trazabilidad: D-01, RQ-01.

### Requirement: Matriz independiente por ambiente

La liberación SHALL registrar por cada ambiente autorizado la versión, alcance, ventana, responsables por rol, aprobador, evidencia y continuación, sin secretos.

#### Scenario: Ambiente no incluido en una solicitud aprobada

- **WHEN** un ambiente no está nombrado y aprobado explícitamente
- **THEN** queda fuera de operación y no hereda autorización de otro ambiente o de pruebas anteriores.

Trazabilidad: D-02, D-03, RQ-02.

### Requirement: Runbook de control y reversión

La liberación SHALL disponer de un runbook que limite las verificaciones a evidencia documental y consultas `SELECT` autorizadas, con criterios de continuar, abortar y revertir.

#### Scenario: Verificación posterior autorizada

- **WHEN** un ambiente recibe autorización explícita
- **THEN** el operador valida versión, evidencia sanitizada, gate inactivo y estado esperado mediante controles de solo lectura antes de continuar.

#### Scenario: Reversión requerida

- **WHEN** la gestión de despliegue ordena reversión
- **THEN** se restaura el paquete previamente aprobado para intentos nuevos sin cambiar tareas, auditoría ni respuestas ya confirmadas.

Trazabilidad: D-04, D-05, RQ-03.

### Requirement: Compatibilidad funcional durante la operación

La liberación SHALL conservar Enviar a usuario como ruta moderna oficial y mantener Continuar flujo con su contrato `IdConector`.

#### Scenario: Operación de la versión autorizada

- **WHEN** se verifica el paquete liberado
- **THEN** no se habilita una ruta Web Forms alternativa de usuario ni se modifica el contrato de transición por conector.

Trazabilidad: D-06, RQ-04.
