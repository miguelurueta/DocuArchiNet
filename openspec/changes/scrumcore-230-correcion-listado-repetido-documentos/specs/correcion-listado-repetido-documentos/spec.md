## ADDED Requirements

### Requirement: APPTREETABLE-230-001 Filtrado estricto por Radicado (source of truth)
El sistema SHALL consultar `ListaDocumentosRadicados/query` **solo** cuando el `Radicado` vÃ¡lido provenga de `getSolicitaGabinetePorTareaWorkflow(idTareaWf)`.

#### Scenario: Incluye Radicado y CampoRadicado en query
- **GIVEN** `idTareaWf` es un nÃºmero vÃ¡lido
- **AND** la respuesta de gabinete contiene `Radicado` no vacÃ­o (tras `trim()`)
- **WHEN** `useGestionRespuestaDocumentosTable.load()` construye el request root
- **THEN** el request incluye:
  - `CampoRadicado = "ENLASE"`
  - `Radicado = <radicado_trim>`
- **AND** `Search` NO se usa como sustituto silencioso de `Radicado`.

#### Scenario: Radicado vacÃ­o no ejecuta query
- **GIVEN** la respuesta de gabinete tiene `Radicado` vacÃ­o/null/whitespace
- **WHEN** se ejecuta `load()`
- **THEN** NO se ejecuta `queryListaDocumentosRadicados(...)`
- **AND** retorna `{ ok: false, message: "No fue posible cargar documentos: el radicado de la tarea es obligatorio." }`

### Requirement: APPTREETABLE-230-002 EstadoExistenciaRadicado bloquea query
El sistema SHALL NO consultar documentos cuando el gabinete indique que el radicado no existe.

#### Scenario: EstadoExistenciaRadicado=NO
- **GIVEN** la respuesta de gabinete indica `EstadoExistenciaRadicado = "NO"`
- **WHEN** se ejecuta `load()`
- **THEN** NO se ejecuta `queryListaDocumentosRadicados(...)`
- **AND** retorna `{ ok: false, message: "No fue posible cargar documentos: el radicado no existe para la tarea." }`

### Requirement: APPTREETABLE-230-003 Anti-stale al cambiar tarea
El sistema SHALL evitar sobrescribir estado con respuestas de una tarea anterior.

#### Scenario: Cambio rÃ¡pido idTareaWf ignora respuesta antigua
- **GIVEN** `load()` se ejecuta para `idTareaWf = A`
- **AND** antes de resolver, cambia a `idTareaWf = B` y se ejecuta `load()` nuevamente
- **WHEN** la respuesta de A llega despuÃ©s
- **THEN** el estado final (rows/columns/count) corresponde a B
- **AND** la respuesta de A no se aplica.

### Requirement: APPTREETABLE-230-004 Limpieza de estado al cambiar tarea
El sistema SHALL limpiar datos previos para evitar render stale.

#### Scenario: Cambio de tarea limpia listado previo
- **GIVEN** el Workbench estaba mostrando documentos de `idTareaWf = A`
- **WHEN** cambia a `idTareaWf = B`
- **THEN** el listado no muestra documentos de A mientras se carga B
- **AND** selecciÃ³n mÃºltiple se recalcula sin incluir ids inexistentes en B.

### Requirement: APPTREETABLE-230-005 No regresiÃ³n (AppTreeTable/AppTable)
El sistema SHALL mantener compatibilidad con:
- selecciÃ³n mÃºltiple,
- `loadChildren` (estructura jerÃ¡rquica),
- flujo `ver_documento`.

#### Scenario: Ver documento funciona igual
- **GIVEN** un rowId vÃ¡lido en el listado filtrado
- **WHEN** se dispara `ver_documento`
- **THEN** el comportamiento de `actionListaDocumentosRadicados` y `resolveDocumentoVisualizacion` se mantiene sin cambios de contrato.

