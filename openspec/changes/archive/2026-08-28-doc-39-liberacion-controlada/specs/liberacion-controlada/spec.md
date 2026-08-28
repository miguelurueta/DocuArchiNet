<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
## Purpose

Definir una liberación documental, controlada y reversible para Devolver → Usuario anterior, sin convertir la evidencia técnica en una autorización o despliegue implícito.

## ADDED Requirements

### Requirement: Línea base de liberación verificable

Trazabilidad: D-01.

La liberación controlada SHALL identificar la evidencia DOC-38 y la versión aprobada como precondiciones separadas de cualquier autorización por ambiente.

#### Scenario: Evidencia técnica sin autorización operativa

- **WHEN** DOC-39 revisa una recomendación técnica aprobada
- **THEN** la registra como precondición y no la interpreta como autorización para desplegar

### Requirement: Matriz de ambiente sin secretos

Trazabilidad: D-02.

DOC-39 SHALL mantener por cada ambiente una matriz con autorización, versión, alcance, ventana, responsables, evidencia y continuación, sin credenciales ni cadenas de conexión.

#### Scenario: Ambiente pendiente de autorización

- **WHEN** un ambiente no tiene autorización, ventana o responsables explícitos
- **THEN** la matriz lo marca pendiente y no permite reutilizar aprobaciones de otro ambiente

### Requirement: Runbook de operación futura autorizada

Trazabilidad: D-03.

El runbook SHALL describir prechequeos, operación, comprobación y continuación para una ventana autorizada, sin ejecutar despliegues, gates ni cambios de configuración durante DOC-39.

#### Scenario: Sin autorización operativa vigente

- **WHEN** no existe una autorización explícita para el ambiente y la ventana
- **THEN** el runbook se limita a preparación documental y no inicia ninguna operación

### Requirement: Verificaciones de solo lectura

Trazabilidad: D-04.

Los controles de liberación SHALL usar únicamente evidencia documental y consultas `SELECT` autorizadas, con resultados saneados y sin registrar secretos.

#### Scenario: Comprobación previa o posterior

- **WHEN** se valida versión, auditoría, historial, token o lock
- **THEN** la comprobación no modifica configuración, tarea, estado, auditoría ni datos de negocio

### Requirement: Reversión que preserva el historial

Trazabilidad: D-05.

La reversión SHALL realizarse exclusivamente mediante la gestión de despliegue aprobada y afectar solo nuevos intentos, sin revertir transiciones confirmadas.

#### Scenario: Reversión requerida

- **WHEN** se autoriza una reversión del artefacto desplegado
- **THEN** no altera tareas terminadas, estados históricos, auditoría ni la ruta UI de operaciones existentes

### Requirement: Preservación de rutas y contratos

Trazabilidad: D-06.

La preparación de liberación SHALL confirmar que Usuario anterior conserva su ruta moderna oficial y que las operaciones Workflow vecinas no cambian sus contratos.

#### Scenario: Revisión de compatibilidad

- **WHEN** se revisa el alcance del artefacto candidato
- **THEN** no contiene postback, fallback a Actividad anterior ni destinos de Usuario anterior en operaciones vecinas

### Requirement: Decisión de liberación única

Trazabilidad: D-07.

DOC-39 SHALL emitir exactamente una decisión: **bloquear**, **solicitar aprobación** o **lista para despliegue autorizado**, sustentada en la matriz y el runbook.

#### Scenario: Precondiciones incompletas

- **WHEN** falta autorización de ambiente, ventana, responsable o evidencia crítica
- **THEN** la decisión es solicitar aprobación o bloquear, y no se inicia un despliegue
