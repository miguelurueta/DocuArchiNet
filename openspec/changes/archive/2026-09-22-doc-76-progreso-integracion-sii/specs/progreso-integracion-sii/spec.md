<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## Purpose

Presentar una espera global accesible y resultados autoritativos por elemento durante la ejecución moderna de importaciones SII, sin duplicar la ejecución backend ni afectar el recorrido legacy.

## ADDED Requirements

### Requirement: RQ-01 Ejecución única de la intención
Origen: D-01, RQ-01.
El sistema SHALL ejecutar una intención moderna completa mediante exactamente una llamada síncrona a `ExecuteImportIntent`.

#### Scenario: Intención individual
- **WHEN** se confirma una intención con un elemento
- **THEN** el frontend realiza una sola llamada con `IntentId` y `VersionToken`

#### Scenario: Intención múltiple
- **WHEN** se confirma una intención con varios elementos
- **THEN** el frontend realiza una sola llamada y no fragmenta ni consulta SII por elemento

### Requirement: RQ-02 Mapeo autoritativo por elemento
Origen: D-02, RQ-02.
El sistema SHALL adaptar cada elemento de `response.Items` usando las fases y estados visibles del contrato compartido.

#### Scenario: Resultado importado confirmado
- **WHEN** un elemento está Reconciliada o Completada y posee documento confirmado
- **THEN** se presenta como Importada conservando clave externa, fase y metadatos

#### Scenario: Resultado no exitoso
- **WHEN** un elemento está en otra fase o en una fase desconocida
- **THEN** se presenta con estado seguro y nunca como Importada por optimismo

### Requirement: RQ-03 Espera indeterminada y resumen fiel
Origen: D-03, RQ-03.
El sistema SHALL presentar espera global accesible mientras la ejecución está pendiente y resultados independientes al finalizar.

#### Scenario: Solicitud pendiente
- **WHEN** `ExecuteImportIntent` aún no resolvió ni falló
- **THEN** la vista anuncia procesamiento global sin porcentaje, temporizador ni avance individual simulado

#### Scenario: Resultado parcial
- **WHEN** la respuesta contiene al menos un elemento no importado
- **THEN** muestra todos los resultados, cuenta importadas, omitidas, fallidas y no procesadas, y no anuncia éxito total

### Requirement: RQ-04 Recuperación sin polling
Origen: D-04, RQ-04.
El sistema SHALL reservar `GetImportIntent` para recuperación explícita y autorizada.

#### Scenario: Flujo síncrono normal
- **WHEN** `ExecuteImportIntent` está pendiente
- **THEN** no invoca `GetImportIntent` ni programa polling

#### Scenario: Respuesta perdida o reapertura
- **WHEN** ocurre timeout, pérdida de respuesta o reapertura autorizada
- **THEN** permite una única consulta explícita del snapshot sin mutaciones

### Requirement: RQ-05 Cierre sin cancelación ni reintento
Origen: D-05, RQ-05.
El sistema SHALL separar el cierre visual del ciclo de vida de la ejecución.

#### Scenario: Cierre durante ejecución
- **WHEN** el usuario cierra la vista con la solicitud pendiente
- **THEN** no aborta, cancela, revierte ni envía `StopRequested`

#### Scenario: Resultado fallido
- **WHEN** existen elementos fallidos
- **THEN** no ofrece “Reintentar fallidos”

### Requirement: RQ-06 Invariancia legacy
Origen: D-06, RQ-06.
El sistema SHALL mantener el feature moderno aislado de infraestructura y códigos legacy.

#### Scenario: Dependencias modernas
- **WHEN** se prueban los módulos DOC-76
- **THEN** no importan, copian ni invocan `JSProgresBar` y no interpretan `YES`, `CTRL`, `CTRLRETURN` ni `dato_lista`

#### Scenario: Gate moderno apagado
- **WHEN** `WorkflowCentroTrabajoModernActive` está apagado
- **THEN** el comportamiento del recorrido legacy permanece intacto
