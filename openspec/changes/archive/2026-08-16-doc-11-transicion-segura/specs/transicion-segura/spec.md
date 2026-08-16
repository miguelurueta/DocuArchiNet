<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10 -->

Decisiones cubiertas: D-01, D-02, D-03, D-04, D-05, D-06, D-07, D-08, D-09 y D-10.

## ADDED Requirements

### Requirement: RQ-01 — Punto de entrada moderno cerrado por defecto

El sistema SHALL exponer EjecutarEnvioTarea(idTarea, idConector, tokenVersion) en WebServiceWorkflowModern, con sesión habilitada y respuesta JSON. La operación SHALL reconstruir el contexto desde la sesión autenticada, evaluar IWorkflowModernFeatureGate antes de ejecutar y no hacer fallback al envío legacy de la interfaz.

#### Scenario: Gate o sesión no autorizan la ejecución

- **WHEN** el gate está inactivo, la sesión Gestión no está autenticada, no existe relación Workflow, faltan permisos, usuario, grupo o conexión
- **THEN** se devuelve ResultadoTransicionDto con Exito=false y un código funcional estable
- **AND THEN** no se invoca WorkflowLegacyExecutorAdapter ni se modifica una tarea

#### Scenario: Error inesperado en la capa ASMX

- **WHEN** ocurre una excepción durante la composición del endpoint
- **THEN** se devuelve WORKFLOW_TRANSITION_UNAVAILABLE con mensaje funcional
- **AND THEN** JSON no contiene excepción, SQL, cadena de conexión, Session, HTML ni traza

### Requirement: RQ-02 — Revalidación de tarea y destino en servidor

El sistema SHALL aceptar idTarea, idConector y tokenVersion únicamente como identificadores de intención. Antes de ejecutar SHALL releer la tarea asignada al usuario Workflow actual, confirmar que siga activa, comparar la versión y resolver un destino de ejecución autorizado por RUTA o FLUJO.

#### Scenario: Solicitud inválida o versión vencida

- **WHEN** idTarea no es positivo, idConector no es positivo, tokenVersion está vacío o la tarea releída no coincide con la versión
- **THEN** se devuelve WORKFLOW_TASK_INVALID o WORKFLOW_VERSION_CONFLICT
- **AND THEN** el motor legacy no se invoca

#### Scenario: Conector alterado o desactualizado

- **WHEN** el conector no pertenece a la ruta, grupo y actividad de origen actuales, o al flujo, nodo y usuario/grupo origen actuales
- **THEN** se devuelve WORKFLOW_CONNECTOR_UNAVAILABLE o WORKFLOW_TRANSITION_INCONSISTENT
- **AND THEN** no se usa el destino publicado en un preview anterior

### Requirement: RQ-03 — Delegación única al motor legacy

El sistema SHALL usar WorkflowLegacyExecutorAdapter como único nuevo llamador de ClassWorkflow.Terminar_Tarea_Workflow. El adaptador SHALL usar un destino tipado y resuelto en servidor, desactivar solo la actualización de controles Web Forms, y preservar PRETERMINARACTIVIAD, TERMINARACTIVIDAD, correo, auditoría legacy y Cambia_Estado interno.

#### Scenario: Envío exitoso por RUTA

- **WHEN** la tarea y el conector de ruta son válidos y sus requisitos se cumplen
- **THEN** el adaptador llama Terminar_Tarea_Workflow con el conector, actividad destino y notificación resueltos en servidor
- **AND THEN** el resultado contiene Exito=true solo si el motor legacy confirmó éxito

#### Scenario: Envío exitoso por FLUJO

- **WHEN** la tarea pertenece a un flujo y el conector valida fuente, destino y usuario/grupo
- **THEN** el adaptador usa la actividad real destino y los identificadores de flujo requeridos por Terminar_Tarea_Workflow
- **AND THEN** no se usa el identificador de actividad de flujo como si fuera una actividad de listado

#### Scenario: Evento previo rechaza el envío

- **WHEN** PRETERMINARACTIVIAD falla o rechaza la operación
- **THEN** la operación devuelve un bloqueo funcional
- **AND THEN** no se confirma una transición efectiva

### Requirement: RQ-04 — Requisitos de negocio previos

El sistema SHALL revalidar respuesta o confirmación, solicitudes de aprobación y autorización aplicable antes de delegar. Firma, expediente, copia documental y balanceo SHALL conservar las reglas actuales del motor legacy y devolver un bloqueo funcional si impiden el envío.

#### Scenario: Requisito pendiente

- **WHEN** falta una respuesta, decisión de aprobación, firma, expediente, autorización, copia o balanceo requerido
- **THEN** se devuelve Exito=false con un código funcional estable y mensaje visible
- **AND THEN** no se filtra el detalle técnico de la regla legacy

### Requirement: RQ-05 — Concurrencia e idempotencia

El sistema SHALL serializar la ejecución por tarea y token mediante un guard transitorio compartido por instancias. Dentro del guard SHALL releer tarea y destino, sin crear una transacción paralela que cambie estados Workflow.

#### Scenario: Doble clic o solicitudes simultáneas

- **WHEN** dos solicitudes intentan ejecutar la misma tarea y versión
- **THEN** como máximo una alcanza el motor legacy
- **AND THEN** la otra recibe WORKFLOW_TRANSITION_IN_PROGRESS, WORKFLOW_VERSION_CONFLICT o el resultado conocido

#### Scenario: Solicitud posterior a una transición exitosa

- **WHEN** la primera solicitud terminó la tarea y se reintenta con el token anterior
- **THEN** la relectura devuelve conflicto de versión o tarea no disponible
- **AND THEN** no ocurre un segundo cambio de estado

### Requirement: RQ-06 — Contrato seguro y auditoría

El sistema SHALL devolver ResultadoTransicionDto con Exito, EstadoFinal, MensajeFuncional, CodigoBloqueo, Advertencias, ActividadDestino, Destino, TokenVersion, ReferenciaAuditoria y EsReintentable. SHALL normalizar los textos recibidos del motor legacy y registrar auditoría con usuario, tarea, origen, destino, mecanismo, fecha y resultado.

#### Scenario: Resultado exitoso con advertencia posterior

- **WHEN** el motor termina la tarea pero correo, TERMINARACTIVIDAD o trazabilidad posterior informa una advertencia
- **THEN** Exito permanece true y Advertencias contiene un mensaje seguro
- **AND THEN** la tarea no se restaura ni se oculta por un resultado simulado

#### Scenario: Resultado rechazado o error controlado

- **WHEN** el motor legacy rechaza, lanza o produce un texto técnico
- **THEN** el DTO devuelve un código y mensaje funcional normalizados
- **AND THEN** no contiene DataSet, controles Web Forms, SQL, credenciales ni el texto original de la excepción

### Requirement: RQ-07 — Compatibilidad, evidencia y reversa

El sistema SHALL conservar workflow/Webworkflow.aspx, workflow/Webworkflow.aspx.vb y el camino legado sin cambios funcionales. SHALL entregar pruebas focales, compilación, QA manual, E2E reutilizable, prueba de concurrencia y documentación en Doc/Actualizacion/workflow/Terminar/03-ejecucion-segura. El gate SHALL iniciar desactivado y permitir reversa sin migrar datos.

#### Scenario: Validación antes de piloto

- **WHEN** se prepara el piloto
- **THEN** existen resultados de build, pruebas focales, QA de RUTA/FLUJO/bloqueos/concurrencia y guía E2E
- **AND THEN** una prueba que cambie estado solo se ejecuta con ambiente, cuentas y tareas descartables autorizadas

#### Scenario: Reversa del piloto

- **WHEN** se desactiva WorkflowCentroTrabajoModernActive
- **THEN** EjecutarEnvioTarea rechaza nuevas ejecuciones modernas
- **AND THEN** Webworkflow.aspx continúa usando el camino legacy sin cambios
