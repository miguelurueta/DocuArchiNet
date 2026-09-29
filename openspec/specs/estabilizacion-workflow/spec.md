# estabilizacion-workflow Specification

## Purpose

Estabiliza el consumidor de Notas del Centro de Trabajo Workflow con contrato explícito, exclusión mutua del fallback y regresión autorizada y reversible.

## Requirements

### Requirement: ESTABILIZACION-WORKFLOW

El sistema SHALL implementar el alcance definido para DOC-44.

#### Scenario: Flujo principal

- **WHEN** se ejecuta el caso de uso principal del ticket
- **THEN** el comportamiento coincide con las reglas funcionales esperadas

#### Scenario: No-regresion

- **WHEN** se valida el modulo afectado
- **THEN** no se rompen flujos existentes

#### Scenario: Alcance exclusivo de Workflow (D-01, RQ-01)

- **WHEN** se inventarían o modifican puntos de entrada de Notas
- **THEN** los cambios de producto se limitan al Centro de Trabajo `workflow/` y su configuración de entrega, mientras el ASMX compartido solo se valida/reutiliza y las pruebas y documentación registran la deuda sin intervenir otros consumidores

#### Scenario: Contrato y tarea explícitos (D-02, RQ-02)

- **WHEN** el consumidor moderno lista, consulta, cuenta, crea, actualiza o elimina una nota
- **THEN** usa un único cliente y contrato moderno con `idTarea` explícito y resultados funcionales del backend

#### Scenario: Exclusión mutua y reversa (D-03, RQ-03)

- **WHEN** el contexto Workflow no es válido
- **THEN** el cliente moderno no ejecuta operaciones
- **WHEN** existe un contexto Workflow autenticado y válido
- **THEN** se muestra la ruta moderna y se oculta el disparador legacy para evitar doble operación

#### Scenario: Regresión y evidencia autorizada (D-04, RQ-04)

- **WHEN** se valida autorización, cruces de tarea/nota, contenido, cursor, conflicto y rollback
- **THEN** existe una E2E exclusiva de Workflow que reutiliza la infraestructura existente, conserva evidencia saneada y exige autorización explícita sin modificar configuración de rollout
