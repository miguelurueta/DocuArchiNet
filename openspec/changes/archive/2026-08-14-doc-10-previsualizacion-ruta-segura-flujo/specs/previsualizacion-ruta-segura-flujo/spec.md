<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10 -->
## ADDED Requirements

### Requirement: RQ-01 Preview sin efectos secundarios (D-01)

El sistema SHALL exponer una previsualización paralela que no modifique el flujo Workflow existente ni ejecute lógica de terminación.

#### Scenario: Invocación de preview

- **WHEN** un consumidor invoca `PreviewEnviarTarea(idTarea)`
- **THEN** la respuesta no invoca `Terminar_Tarea_Workflow`, `Cambia_Estado`, `PRETERMINARACTIVIAD` ni `TERMINARACTIVIDAD`, y no cambia estado, auditoría, correo, firma ni transacciones de escritura

### Requirement: RQ-02 Contexto y habilitación de servidor (D-02)

El sistema SHALL resolver identidad y autorización desde la sesión autenticada y evaluar `IWorkflowModernFeatureGate` antes de consultar datos del preview.

#### Scenario: Usuario fuera del piloto

- **WHEN** el feature gate devuelve un estado distinto de activo para el contexto de sesión
- **THEN** la respuesta devuelve `WORKFLOW_MODERN_INACTIVE`, no contiene destinos y no consulta repositorios de flujo o ruta

### Requirement: RQ-03 Lectura autorizada por dominio Workflow (D-03)

El sistema SHALL recuperar tarea, flujo y ruta mediante repositorios Workflow específicos con consultas parametrizadas y contexto validado.

#### Scenario: Tarea o destino no autorizado

- **WHEN** la tarea no está activa o no pertenece al usuario/grupo real, o un destino no pertenece al origen
- **THEN** el sistema no devuelve datos ajenos y retorna el bloqueo funcional correspondiente

### Requirement: RQ-08 Semántica de libertad de asignación (D-08)

El sistema SHALL tratar `TIPO_RUTA_ABIERTA_CERRADA` y `TIPO_ABIERTA_CERRADA_ACTIVIDAD` como configuración de libertad de asignación y no como estado de disponibilidad de envío del preview.

#### Scenario: Flujo con libertad de asignación restringida

- **WHEN** una tarea `FLUJO` autorizada tiene cualquiera de esos campos con valor distinto de cero
- **THEN** el preview lista sus conectores salientes autorizados desde el origen real
- **AND THEN** no devuelve `WORKFLOW_FLOW_CLOSED` por esos campos

### Requirement: RQ-09 Aislamiento del catálogo documental de ruta (D-09)

El sistema SHALL consultar `tipo_doc_entrante.estado_ruta_open_close` mediante la conexión Docuarchi resuelta desde la sesión autenticada, mientras conserva las lecturas de tarea y destinos de ruta en el módulo Workflow.

#### Scenario: Ruta con catálogo Workflow separado

- **WHEN** una tarea `RUTA` autorizada tiene un tipo documental abierto en Docuarchi y el catálogo Workflow no contiene `tipo_doc_entrante`
- **THEN** el preview resuelve el estado con Docuarchi y devuelve únicamente los destinos de ruta autorizados desde Workflow
- **AND THEN** no expone ni serializa detalles de conexión, sesión o excepción

### Requirement: RQ-10 Prueba de concurrencia autenticada y sin mutación (D-10)

La entrega SHALL proporcionar una prueba reproducible para 20 y 30 sesiones autenticadas concurrentes que mida exclusivamente el ASMX `PreviewEnviarTarea`, sin modificar la tarea ni su auditoría.

#### Scenario: Carga controlada de preview

- **WHEN** se ejecuta cada nivel de concurrencia con una sesión Gestión independiente por usuario virtual y una solicitud de preview por sesión
- **THEN** registra total de sesiones, solicitudes exitosas/fallidas y latencias p50, p95 y p99 sin guardar credenciales, cookies, cadenas de conexión ni respuestas completas
- **AND THEN** compara estado de la tarea y auditoría antes/después mediante consultas `SELECT` parametrizadas

### Requirement: RQ-04 Contrato serializable de la previsualización (D-04)

El sistema SHALL devolver exclusivamente una `PrevisualizacionTransicionDto` serializable, con tipo de transición, contexto seguro, destinos autorizados, destinatario/grupo cuando aplique, requisitos, notificación, token de versión y bloqueo funcional.

#### Scenario: Decisión por flujo, ruta o inconsistencia

- **WHEN** la tarea corresponde a `FLUJO` o `RUTA`
- **THEN** la respuesta lista solo sus destinos autorizados
- **AND WHEN** no hay destinos o la decisión es inconsistente
- **THEN** la respuesta contiene un código y mensaje legible sin HTML, `DataSet`, SQL, sesión, credenciales ni excepciones internas

### Requirement: RQ-05 ASMX paralelo con entrada mínima (D-05)

El sistema SHALL crear `webservice/WebServiceWorkflowModern.asmx` con `ScriptService` y un `WebMethod(EnableSession:=True)` llamado `PreviewEnviarTarea(idTarea As Long)`.

#### Scenario: Solicitud directa al ASMX

- **WHEN** un cliente llama el método con `idTarea`
- **THEN** el endpoint obtiene usuario, grupo, actividad, ruta y permisos desde el servidor y no acepta esos identificadores como parámetros del navegador

### Requirement: RQ-06 E2E automatizada real (D-06)

La entrega SHALL implementar y ejecutar una E2E automatizada contra el ASMX desplegado en un ambiente de prueba; una limitación de infraestructura no la sustituye.

#### Scenario: Usuario autorizado y usuario no autorizado

- **WHEN** la E2E invoca `PreviewEnviarTarea` con ambos contextos de usuario
- **THEN** verifica el JSON, los destinos o bloqueos autorizados y confirma antes/después que la tarea, su estado y la auditoría de terminación no cambiaron

### Requirement: RQ-07 Bootstrap seguro de sesión Workflow (D-07)

El sistema SHALL completar el contexto Workflow únicamente desde una sesión Gestión autenticada y su relación de servidor en `remit_dest_interno`, antes de evaluar la previsualización.

#### Scenario: Sesión Gestión sin claves Workflow

- **WHEN** una sesión autenticada de Gestión invoca el ASMX sin usuario, grupo o ruta Workflow inicializados
- **THEN** el servidor resuelve el usuario Workflow relacionado, establece solo las claves de contexto de esa sesión y usa el snapshot Workflow asociado para las consultas de preview; para rutas, usa además el snapshot Docuarchi solo para su estado documental
- **AND THEN** no recibe credenciales ni identificadores de autorización desde el cliente, no invoca `InicializaSesionModuloWorkflow`, no registra auditoría y no ejecuta escrituras

#### Scenario: Sesión o relación inválida

- **WHEN** no existe una sesión Gestión autenticada, la relación Workflow no existe o su configuración de módulo es inconsistente
- **THEN** el endpoint falla cerrado con contexto inválido, sin destinos ni acceso a repositorios de preview
