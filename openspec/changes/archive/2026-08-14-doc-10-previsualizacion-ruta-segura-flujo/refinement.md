<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-10-previsualizacion-ruta-segura-flujo

## Fuente y alcance

- Ticket: `DOC-10` — PREVISUALIZACION-RUTA-SEGURA-FLUJO.
- Cambio OpenSpec: `doc-10-previsualizacion-ruta-segura-flujo`.
- Fuente funcional: `specs/previsualizacion-ruta-segura-flujo/jira-context.md` y aclaraciones posteriores del responsable: la E2E automatizada real es obligatoria y los campos de apertura/cierre del flujo modelan libertad de asignación, no disponibilidad de envío.
- Perfil técnico observado: ASP.NET Web Forms sobre .NET Framework, VB.NET, ASMX y MySQL; no hay componentes React, TypeScript ni un contrato frontend ajeno dentro del alcance.

DOC-10 implementa únicamente la previsualización de destinos de una tarea. Convive con el envío legacy y no termina, reasigna ni cambia el estado de una tarea.

## Contexto inspeccionado

- Entrada ASMX existente: `webservice/WebServiceWorkflow.asmx` y `webservice/WebServiceWorkflow.asmx.vb`; sus métodos devuelven estructuras legacy y no son el contrato del preview moderno.
- Selección legacy de tarea y contexto: `workflow/Webworkflow.aspx.vb` obtiene `ID_TAREA_SELECCIONDA`, `Id_Usuario_Workflow`, `Id_actividad_Workflow` e `Id_Ruta_Workflow` desde sesión. El nuevo ASMX solo resuelve el contexto autenticado en servidor; nunca recibe estos valores del navegador.
- Flujo: `Class_flujo_trabajo_workflow.Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado(idTarea, idUsuarioWorkflow)` valida disponibilidad; `Solicita_listado_actividades_para_envio_tarea_a_flujo(radicado, idTarea, ...)` es la referencia de destinos de lectura.
- Ruta: `Class_worflow_rutas.Solicita_etado_abierto_cerrado_ruta_tarea(idTarea, idRuta, estadoRuta, tramite)` obtiene disponibilidad y radicado; `Class_Listado_Actividades_workflow.Solicita_listado_actividades_ruta(...)` es la referencia de actividades permitidas.
- Frontera prohibida: `ClassWorkflow.Validar_enviar_actividad_por_conector_flujo_o_ruta(Page)` depende de `Page`, controles y sesión; `ClassWorkflow.Terminar_Tarea_Workflow` y `ClassWorkflow.Cambia_Estado` escriben estado y transacciones. DOC-10 no los invoca ni modifica.
- Base reutilizada de DOC-9: `Modelo/Workflow/Terminar/`, `DTOs/Workflow/Terminar/`, `Services/Workflow/Terminar/`, `Domain/Shared/`, `Infrastructure/Shared/Data/` e `Infrastructure/Workflow/Terminar/`. `WorkflowLegacyExecutorAdapter` permanece inerte y fuera de la composición del preview.

## Decisiones aprobadas

| ID | Decision verificable | Evidencia de codigo | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | El preview es una capacidad paralela y de solo lectura; no modifica Web Forms ni llama a terminación, cambio de estado, eventos, firma, correo o transacciones de escritura. | `workflow/Webworkflow.aspx.vb`, `workflow/ClassWorkflow.vb`, `Infrastructure/Workflow/Terminar/WorkflowLegacyExecutorAdapter.vb`. | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | El contexto y la habilitación se resuelven en servidor. El feature gate se evalúa antes de cualquier consulta de tarea, flujo o ruta y bloquea de forma fail-closed. | `Modelo/Workflow/Terminar/WorkflowModernModels.vb`, `Services/Workflow/Terminar/ServicioTransicionTarea.vb`, `Infrastructure/Workflow/Terminar/ConfiguracionWorkflowModernFeatureGate.vb`. | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Las lecturas de tarea, flujo y ruta se implementan en repositorios Workflow específicos, con parámetros y contexto validado, usando solamente la infraestructura compartida de datos. | `Modelo/Workflow/Terminar/WorkflowModernInterfaces.vb`, `Infrastructure/Shared/Data/`, `Infrastructure/Repositories/Workflow/README.md`. | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Application devuelve un contrato tipado y seguro que distingue flujo/ruta, destinos, contexto, requisitos, notificación, token de versión y bloqueos funcionales sin filtrar detalles internos. | `DTOs/Workflow/Terminar/TransicionWorkflowDtos.vb`, `Services/Workflow/Terminar/ServicioTransicionTarea.vb`, `Services/Workflow/Terminar/ValidadorTransicionTarea.vb`. | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | El ASMX moderno expone exclusivamente `PreviewEnviarTarea(idTarea)` y compone Application con contexto de sesión autenticada; no acepta identificadores de autorización desde el cliente. | `webservice/WebServiceWorkflow.asmx`, `webservice/WebServiceWorkflow.asmx.vb`, `GestionDocumental-Docuarchi.net.vbproj`. | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | La entrega implementa y ejecuta una E2E automatizada real contra un ASMX desplegado, además de compilación, verificaciones focales, QA manual y documentación de resultados. | `Doc/Actualizacion/workflow/Terminar/02-preview-ruta-flujo/`, futura automatización E2E y evidencia de ambiente. | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | El ASMX completa el contexto Workflow desde la sesión Gestión autenticada y `remit_dest_interno`, sin inicializar el módulo legacy completo ni escribir auditoría. | `webservice/WorkflowPreviewSessionContextGate.vb`. | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | `TIPO_RUTA_ABIERTA_CERRADA` y `TIPO_ABIERTA_CERRADA_ACTIVIDAD` expresan libertad de asignación; el preview no los interpreta como cierre ni como veto de envío. Para `FLUJO` lista los conectores salientes autorizados igual que la lectura legacy. | `workflow/Class_flujo_trabajo_workflow.vb` y aclaración funcional del responsable. | D-08 | RQ-08 | Origen: D-08, RQ-08 |
| D-09 | El estado documental de una ruta se obtiene de `docuarchi.tipo_doc_entrante`, con un snapshot de conexión Docuarchi resuelto por Presentation; la tarea y los destinos continúan en Workflow. | `Defaul/GestorModuleSesion.vb`, `Defaul/conect.vb`, `Infrastructure/Repositories/Workflow/MySqlWorkflowPreviewRepositories.vb`. | D-09 | RQ-09 | Origen: D-09, RQ-09 |
| D-10 | La decisión de asincronía se fundamenta en una carga autenticada controlada de 20 y 30 sesiones contra el ASMX, con métricas y huellas de solo lectura; la prueba no cambia el diseño síncrono del endpoint. | `tools/e2e/scripts/run-doc10-concurrency.cjs`. | D-10 | RQ-10 | Origen: D-10, RQ-10 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptacion | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | El endpoint no tiene efectos secundarios. | Una invocación no llama los métodos de terminación ni cambia tarea, auditoría, correo, firma, evento o transacción. | Conserva el envío y la terminación legacy vigentes. |
| RQ-02 | Una llamada fuera del piloto recibe `WORKFLOW_MODERN_INACTIVE` sin destinos ni consultas de resolución. | El feature gate se evalúa antes de repositorios para un usuario/grupo no habilitado. | Evita exponer rutas a usuarios fuera del alcance. |
| RQ-03 | Solo se devuelven tarea y destinos autorizados del origen real. | Repositorios parametrizados filtran tarea activa, grupo, flujo/ruta y conectores/actividades permitidos. | Evita confiar en datos enviados por el navegador y duplicar SQL legacy. |
| RQ-04 | La respuesta es una `PrevisualizacionTransicionDto` serializable y segura. | Distingue `FLUJO` y `RUTA`, incluye token/contexto seguro y devuelve bloqueos legibles para inconsistencias o ausencia de destinos. | No expone HTML, `DataSet`, SQL, sesión, credenciales ni excepciones. |
| RQ-05 | El nuevo ASMX acepta solo `idTarea` y obtiene autorización en servidor. | `PreviewEnviarTarea` usa `ScriptService`, `WebMethod(EnableSession:=True)` y el contexto autenticado para componer Application. | Aísla Presentation y deja sin cambios `Webworkflow.aspx`. |
| RQ-06 | Una E2E real prueba usuario autorizado y no autorizado sin mutar la tarea. | La automatización invoca el ASMX desplegado, valida JSON/destinos/bloqueos y comprueba estado y auditoría antes/después. | Detecta integración real entre hospedaje, sesión, Application e Infrastructure. |
| RQ-07 | El contexto Workflow se inicia de forma limitada desde una sesión Gestión válida. | El gate resuelve la relación de servidor y el módulo Workflow sin recibir autorización del navegador ni escribir datos. | Conserva el límite de solo lectura del preview. |
| RQ-08 | Los campos de libertad de asignación no ocultan conectores de flujo. | Con valores distintos de cero, una tarea autorizada por flujo devuelve sus conectores salientes autorizados; el preview no devuelve `WORKFLOW_FLOW_CLOSED` por esos campos. | Conserva la lectura de destinos del legacy y no autoriza ni ejecuta el envío. |
| RQ-09 | El estado de ruta se consulta en el catálogo documental correcto. | Para una tarea `RUTA`, el estado de `tipo_doc_entrante` se lee con la conexión Docuarchi de la sesión; la ausencia de esa tabla en Workflow no genera una excepción ni oculta los destinos válidos. | Conserva el límite de solo lectura y evita mezclar catálogos. |
| RQ-10 | La concurrencia se mide sin cambiar el endpoint ni los datos. | Con 20 y 30 sesiones autenticadas concurrentes se miden respuestas, p50/p95/p99, fallos y huellas antes/después de estado y auditoría. | Aporta evidencia para decidir asincronía; no confunde el login con la latencia del ASMX. |

## Reglas de trazabilidad obligatorias

1. Cada decisión `D-XX` se desarrolla en `design.md`, se refleja en un requisito de `spec.md` y se vincula a tareas mediante `Origen: D-XX, RQ-XX`.
2. Toda tarea de código, validación, E2E, documentación o cierre conserva su origen.
3. Las nuevas clases respetan Web Forms/VB.NET y el límite Presentation → Application → Domain → Infrastructure; no introducen reglas de otro stack.
4. La aprobación solo habilita el trabajo descrito; cada tarea permanece abierta hasta que su cambio y evidencia existan.

## Resultado del refinamiento

La matriz D-01 a D-10 se refleja en diseño, especificación y tareas. El siguiente paso es sincronizar la trazabilidad con `opsxj:refine -- DOC-10 --sync` y continuar la implementación desde los contratos del preview.
