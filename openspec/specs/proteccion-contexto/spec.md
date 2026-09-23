# proteccion-contexto Specification

## Purpose

Proteger la identidad de tarea de una importación y recuperar su estado autoritativo después de conflictos, recargas o pérdida de conexión.

## Requirements

### Requirement: Contexto inmutable de importación
El sistema SHALL conservar la tarea y la identidad originales de la intención durante todo el flujo. **Origen: D-01, RQ-01.**

#### Scenario: Cambio de tarea visible
- **WHEN** la tarea visible difiere de la tarea capturada
- **THEN** el frontend detiene la continuación y no sustituye el contexto original

### Requirement: Preflight anterior al primer efecto
El sistema SHALL validar nuevamente el contexto inmediatamente antes de enviar la ejecución. **Origen: D-02, RQ-02.**

#### Scenario: Contexto modificado después de preparar
- **WHEN** el contexto cambia entre preparación y confirmación
- **THEN** no se llama `ExecuteImportIntent` y la preparación termina sin efectos

### Requirement: Bloqueo de acciones incompatibles
El sistema SHALL bloquear de forma reversible selección/búsqueda de tareas y acciones Workflow incompatibles mientras exista una escritura en curso. **Origen: D-03, RQ-03.**

#### Scenario: Escritura iniciada
- **WHEN** se envía `ExecuteImportIntent`
- **THEN** los controles declarados quedan bloqueados y se explica que cerrar no cancela la operación

#### Scenario: Escritura finalizada
- **WHEN** la ejecución alcanza un resultado terminal verificable
- **THEN** cada control recupera su estado anterior

### Requirement: Conflictos normativos de contexto
El sistema SHALL tratar `TASK_CONTEXT_MISMATCH` y `PERSISTED_CONTEXT_MISMATCH` como conflictos que detienen pendientes y fuerzan consulta autoritativa. **Origen: D-04, RQ-04.**

#### Scenario: Conflicto backend
- **WHEN** el backend devuelve uno de los códigos normativos
- **THEN** se conservan resultados previos, se detienen pendientes y no se inventa un estado alternativo

### Requirement: Recuperación autoritativa
El sistema SHALL recuperar una intención solo con un `IntentId` autoritativo y mediante la API moderna, sin reejecutar ciegamente. **Origen: D-05, RQ-05.**

#### Scenario: Recarga o pérdida de conexión
- **WHEN** la página recibe un `IntentId` autoritativo después de una interrupción
- **THEN** consulta o reconcilia el snapshot persistido sin `localStorage`, polling ni mutación por elemento

#### Scenario: IntentId ausente
- **WHEN** no existe un identificador autoritativo
- **THEN** la recuperación permanece deshabilitada y no busca intenciones desde el navegador

### Requirement: Aislamiento entre tarea y pestañas
El sistema SHALL proyectar documentos únicamente cuando la vista corresponda a la tarea original. **Origen: D-06, RQ-06.**

#### Scenario: Otra pestaña cambia el contexto
- **WHEN** se recibe una señal de posible cambio de tarea en otra pestaña
- **THEN** se solicita verificación autoritativa y no se actualiza la lista actual

#### Scenario: Resultado de tarea diferente
- **WHEN** un resultado contiene un `TaskId` distinto de la vista
- **THEN** no se inserta ni abre ese documento en la tarea actual
