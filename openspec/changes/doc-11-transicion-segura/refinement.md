<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - DOC-11: transición segura

## Fuente y alcance

- Ticket: DOC-11 — TRANSICION-SEGURA.
- Endpoint único: WebServiceWorkflowModern.EjecutarEnvioTarea.
- Plataforma: ASP.NET Web Forms .NET Framework 4.6.1, VB.NET y ASMX.
- Fuera de alcance: sustituir ClassWorkflow, cambiar Webworkflow.aspx/Webworkflow.aspx.vb, crear un segundo ASMX, modificar la interfaz legacy o habilitar el piloto por defecto.

## Contexto inspeccionado

- webservice/WebServiceWorkflowModern.asmx.vb expone solo PreviewEnviarTarea y compone ServicioTransicionTarea sin dependencias de escritura.
- webservice/WorkflowPreviewSessionContextGate.vb reconstruye usuario, grupo y conexión desde una sesión Gestión autenticada; no carga los permisos que el motor legacy necesita para ejecutar.
- Services/Workflow/Terminar/ServicioTransicionTarea.vb ya revalida gate, tarea activa, token y conector, pero el ejecutor recibe aún la solicitud no confiable.
- Infrastructure/Repositories/Workflow/MySqlWorkflowPreviewRepositories.vb consulta la tarea asignada al usuario actual. Para FLUJO publica el identificador de actividad de flujo, por lo que no basta para llamar al motor legacy; RUTA sí obtiene el conector de ruta.
- Infrastructure/Workflow/Terminar/WorkflowLegacyExecutorAdapter.vb es el límite reservado, pero hoy devuelve WORKFLOW_MODERN_EXECUTION_PENDING.
- workflow/ClassWorkflow.vb contiene Terminar_Tarea_Workflow, Cambia_Estado, PRETERMINARACTIVIAD y TERMINARACTIVIDAD. La llamada admite desactivar solo la actualización de controles Web Forms mediante activa_actualizacion_paramtros_interface=0, conservando eventos y motor legacy.
- workflow/Webworkflow.aspx.vb y workflow/Class_flujo_trabajo_workflow.vb muestran las verificaciones legacy de respuesta, aprobaciones y resolución de conector que se deben conservar sin editar esos archivos.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | El único endpoint de ejecución será EjecutarEnvioTarea en el ASMX moderno existente; valida sesión antes de componer dependencias y no ejecuta fallback. | webservice/WebServiceWorkflowModern.asmx.vb | D-01 | RQ-01 | 2.3, 2.6; Origen: D-01, RQ-01 |
| D-02 | La ejecución inicia el contexto Workflow completo desde la sesión Gestión autenticada y falla cerrada cuando usuario, grupo, permisos o conexión no son válidos. | webservice/WorkflowPreviewSessionContextGate.vb; workflow/InicioWorkflow.vb | D-02 | RQ-01, RQ-02 | 2.3, 3.1; Origen: D-02, RQ-01, RQ-02 |
| D-03 | idTarea, idConector y tokenVersion son solo identificadores de solicitud: la tarea, su versión y el destino ejecutable se vuelven a resolver en servidor. | Services/Workflow/Terminar/ServicioTransicionTarea.vb; Infrastructure/Repositories/Workflow/MySqlWorkflowPreviewRepositories.vb | D-03 | RQ-02 | 2.1, 2.2, 2.4, 3.1; Origen: D-03, RQ-02 |
| D-04 | Un repositorio específico resuelve el destino autorizado de RUTA o FLUJO, incluidos los argumentos que el motor legacy requiere; no reutiliza el DTO de preview como autoridad. | workflow/Class_flujo_trabajo_workflow.vb; workflow/Webworkflow.aspx.vb | D-04 | RQ-02, RQ-03 | 2.1, 2.2, 3.2; Origen: D-04, RQ-02, RQ-03 |
| D-05 | Solo WorkflowLegacyExecutorAdapter invoca Terminar_Tarea_Workflow. Llama al motor con actualización de interfaz desactivada, conserva PRETERMINARACTIVIAD, TERMINARACTIVIDAD, correo y Cambia_Estado internos. | Infrastructure/Workflow/Terminar/WorkflowLegacyExecutorAdapter.vb; workflow/ClassWorkflow.vb | D-05 | RQ-03 | 2.5, 2.6, 3.2; Origen: D-05, RQ-03 |
| D-06 | Antes del adaptador se ejecutan gate, tarea asignada activa, token, conector/origen, respuesta, aprobaciones y los requisitos que ya aplica el motor legacy; cada bloqueo devuelve código y texto funcional. | workflow/Webworkflow.aspx.vb; workflow/ClassWorkflow.vb | D-06 | RQ-04 | 2.4, 2.5, 3.1, 3.2; Origen: D-06, RQ-04 |
| D-07 | Se serializa por tarea y token con un bloqueo transitorio de MySQL, se relee la tarea dentro del bloqueo y se retorna conflicto controlado sin crear una transacción de estado paralela. | Infrastructure/Shared/Data/AdoNetDataInfrastructure.vb; estados_tarea_workflow | D-07 | RQ-05 | 2.1, 2.4, 3.3; Origen: D-07, RQ-05 |
| D-08 | La respuesta pública normaliza errores del motor legacy, conserva solo campos del contrato y registra referencia de auditoría con usuario, tarea, origen, destino, mecanismo y resultado. | DTOs/Workflow/Terminar/TransicionWorkflowDtos.vb; Modelo/Workflow/Terminar/WorkflowModernModels.vb | D-08 | RQ-06 | 2.1, 2.5, 3.1, 3.2; Origen: D-08, RQ-06 |
| D-09 | La entrega incluye pruebas focales, compilación, QA manual, E2E reutilizable y concurrencia. Una E2E que cambie estado solo se ejecuta con tarea, cuentas y ambiente autorizados para descarte. | tools/e2e; AGENTS.md | D-09 | RQ-07 | 3.1, 3.2, 3.3, 3.4, 4.1; Origen: D-09, RQ-07 |
| D-10 | El piloto permanece cerrado por WorkflowCentroTrabajoModernActive=false; la reversa es desactivar el gate, sin migración ni alteración del camino Web Forms. | Infrastructure/Workflow/Terminar/ConfiguracionWorkflowModernFeatureGate.vb | D-10 | RQ-01, RQ-07 | 2.6, 4.1, 4.2; Origen: D-10, RQ-01, RQ-07 |

## Requisitos verificables

| ID | Resultado observable | Escenario de aceptación | Riesgo y compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | El ASMX expone la operación en el mismo servicio y la sesión/gate inválidos no alcanzan el motor. | Con gate inactivo, sesión inválida o sin permisos, devuelve un bloqueo funcional y no hay transición. | El gate empieza cerrado; no se modifica Webworkflow.aspx ni su code-behind. |
| RQ-02 | El servidor identifica la tarea asignada, token y destino actual antes de ejecutar. | Token vencido, conector alterado, ruta cerrada, flujo/origen inconsistentes o tarea ajena devuelven bloqueo. | Evita confiar en valores de navegador y evita el mapeo incompleto de preview para FLUJO. |
| RQ-03 | La transición efectiva ocurre una sola vez por medio del adaptador legacy. | Ruta y flujo exitosos llegan a Terminar_Tarea_Workflow con argumentos resueltos en servidor; PRETERMINARACTIVIAD bloquea si falla. | No se crea Cambia_Estado ni transacción alternativa. |
| RQ-04 | Los requisitos previos se validan sin filtrar detalle interno. | Respuesta, aprobación, firma, expediente, autorización, copia o balanceo faltantes devuelven bloqueo funcional. | Se conservan las reglas existentes en el motor y sus validadores legacy. |
| RQ-05 | Doble clic o dos solicitudes simultáneas no producen dos transiciones. | Una solicitud completa o la segunda recibe resultado conocido o WORKFLOW_VERSION_CONFLICT/WORKFLOW_TRANSITION_IN_PROGRESS. | El bloqueo es transitorio y no deja estados persistentes nuevos. |
| RQ-06 | JSON y auditoría son seguros y útiles para el cliente. | Éxito/bloqueo incluye solo el contrato; una excepción no expone SQL, credenciales, Session, HTML o traza. | Los errores legacy se normalizan y la advertencia posterior no revierte una transición ya efectiva. |
| RQ-07 | La entrega se puede verificar y revertir. | Pruebas focales, build, QA, concurrencia, E2E autorizada y documentación quedan registradas; desactivar el gate detiene el camino moderno. | No se ejecutan pruebas mutantes sobre tareas productivas sin autorización expresa. |

## Resultado del refinamiento

Estado: aprobado para implementación. Las tareas de código deben conservar el origen indicado en la matriz y no pueden marcarse completas sin evidencia.
