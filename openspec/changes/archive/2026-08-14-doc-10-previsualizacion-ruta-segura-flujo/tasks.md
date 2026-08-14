<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10 -->
## 1. Refinamiento técnico y límites de la entrega

- [x] 1.1 Inspeccionar y registrar en `refinement.md` las fuentes legacy de lectura, contexto y límites de terminación. No modificar `workflow/Webworkflow.aspx.vb`, `ClassWorkflow.vb`, `Class_flujo_trabajo_workflow.vb` ni `Class_worflow_rutas.vb`. Origen: D-01, RQ-01
- [x] 1.2 Completar la matriz de decisiones y requisitos para gate, contexto de servidor, repositorios, DTO, ASMX y E2E real. Origen: D-02, RQ-02
- [x] 1.3 Confirmar reutilización de la fundación DOC-9 y prohibir `GenericRepository`, un segundo feature gate o un ejecutor legacy alterno. Origen: D-03, RQ-03
- [x] 1.4 Sincronizar y validar el refinement aprobado antes de iniciar código: `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-10 --sync`. Origen: D-01, RQ-01

## 2. Contrato de previsualización y modelos serializables

- [x] 2.1 Extender `DTOs/Workflow/Terminar/TransicionWorkflowDtos.vb` para devolver contexto seguro, destinos, destinatario/grupo cuando aplique, requisitos, notificación, token de versión y bloqueo funcional serializable, sin datos internos. Origen: D-04, RQ-04
- [x] 2.2 Centralizar códigos y mensajes seguros para gate inactivo, contexto/tarea inválidos, tarea no disponible, ruta no disponible, conector inconsistente, tipo inválido y ausencia de destinos. Origen: D-04, RQ-04
- [x] 2.3 Ajustar modelos y puertos de `Modelo/Workflow/Terminar/` solo con datos necesarios para la respuesta y conservando `ContextoModuloWorkflow` como entrada de repositorios. Origen: D-03, RQ-03

## 3. Infraestructura de lectura por dominio Workflow

- [x] 3.1 Implementar el repositorio de tarea en `Infrastructure/Repositories/Workflow/` con consultas parametrizadas para recuperar tarea activa/autorizada, radicado, actividad origen, decisión y token. Origen: D-03, RQ-03
- [x] 3.2 Implementar el repositorio de flujo que devuelva solo conectores salientes y autorizados desde el origen real. Origen: D-03, RQ-03
- [x] 3.3 Implementar el repositorio de ruta que devuelva solo actividades permitidas para ruta, grupo y actividad actuales y bloquee rutas cerradas/inconsistentes. Origen: D-03, RQ-03
- [x] 3.4 Reutilizar exclusivamente `Infrastructure/Shared/Data/` para conexión, comandos y parámetros; los repositorios no leen sesión, no devuelven `DataSet` ni construyen HTML. Origen: D-03, RQ-03
- [x] 3.5 Corregir la semántica de flujo: no usar `TIPO_RUTA_ABIERTA_CERRADA` ni `TIPO_ABIERTA_CERRADA_ACTIVIDAD` como bloqueo; listar conectores autorizados y cubrirlo en verificación/E2E. Origen: D-08, RQ-08
- [x] 3.6 Resolver el estado `tipo_doc_entrante` de rutas mediante el snapshot Docuarchi de la sesión y conservar tarea/destinos en Workflow; verificar una ruta real sin mezclar catálogos. Origen: D-09, RQ-09

## 4. Caso de uso de Application estrictamente de lectura

- [x] 4.1 Completar `ServicioTransicionTarea.Previsualizar` para evaluar primero el feature gate y devolver bloqueo sin consultar destinos cuando no esté activo. Origen: D-02, RQ-02
- [x] 4.2 Validar contexto e `idTarea`, recuperar la tarea autorizada y devolver el bloqueo funcional antes de resolver destinos. Origen: D-02, RQ-02
- [x] 4.3 Resolver `FLUJO` y `RUTA` mediante el repositorio adecuado y mapear la respuesta segura; bloquear tipos desconocidos, destinos vacíos e inconsistencias. Origen: D-04, RQ-04
- [x] 4.4 Verificar que el caso de uso no compone ni invoca ejecutor legacy, terminación, cambio de estado, eventos, firma, correo o escritura. Origen: D-01, RQ-01

## 5. Endpoint ASMX paralelo y composición segura

- [x] 5.1 Crear `webservice/WebServiceWorkflowModern.asmx` y `webservice/WebServiceWorkflowModern.asmx.vb` con `ScriptService` y `PreviewEnviarTarea(idTarea As Long) As PrevisualizacionTransicionDto`. Origen: D-05, RQ-05
- [x] 5.2 Resolver `ContextoModuloWorkflow` solo desde sesión autenticada en el code-behind y componer gate, validador y repositorios de lectura. Origen: D-05, RQ-05
- [x] 5.3 Traducir fallos no controlados de Presentation a un bloqueo funcional seguro sin serializar excepción, configuración o sesión. Origen: D-05, RQ-05
- [x] 5.4 Incluir archivos nuevos en `GestionDocumental-Docuarchi.net.vbproj` y confirmar que `Webworkflow.aspx` y su code-behind no cambian. Origen: D-05, RQ-05
- [x] 5.5 Implementar `WorkflowPreviewSessionContextGate` para completar contexto Workflow desde la sesión Gestión y `remit_dest_interno`, sin credenciales de cliente, auditoría, scripts ni escritura; componer la conexión del módulo Workflow sin que los repositorios lean sesión. Origen: D-07, RQ-07

## 6. Pruebas, evidencia y no regresión

- [x] 6.1 Crear pruebas focales o una verificación estática reproducible para gate fail-closed, validación, DTOs, separación flujo/ruta y ausencia de terminación. Origen: D-06, RQ-06
- [x] 6.2 Cubrir flujo con uno o varios conectores, ruta, tarea inexistente/cerrada/no autorizada, nodo inválido, conector ajeno y gate inactivo. Origen: D-06, RQ-06
- [x] 6.3 Añadir comprobaciones que fallen si preview escribe estado, envía correo, dispara terminación o recibe identificadores de autorización desde el cliente. Origen: D-01, RQ-01
- [x] 6.4 Compilar con MSBuild de .NET Framework, ejecutar verificaciones focales y registrar comando, resultado y limitaciones reales. Origen: D-06, RQ-06
- [x] 6.5 Implementar una E2E automatizada real contra el ASMX desplegado con usuario autorizado y no autorizado; validar JSON/destinos/bloqueos y ausencia de mutación de tarea, estado y auditoría. Origen: D-06, RQ-06
- [x] 6.6 Ejecutar y conservar evidencia de E2E y QA manual: ambiente, piloto, usuarios, tarea, solicitud, respuesta, comprobación antes/después y resultado. Origen: D-06, RQ-06
- [x] 6.7 Implementar y ejecutar carga autenticada de 20 y 30 sesiones sobre el ASMX; conservar p50/p95/p99, fallos y huellas de estado/auditoría para sustentar la decisión de asincronía. Origen: D-10, RQ-10

## 7. Paquete documental y cierre del cambio

- [x] 7.1 Crear `Doc/Actualizacion/workflow/Terminar/02-preview-ruta-flujo/00-indice.md` con ticket, alcance de solo lectura, archivos afectados, estado y compatibilidad preservada. Origen: D-06, RQ-06
- [x] 7.2 Documentar arquitectura, responsabilidades, dependencias, alternativas descartadas y frontera legacy en `01-arquitectura.md`. Origen: D-01, RQ-01
- [x] 7.3 Documentar entrada, sesión, DTOs, JSON, token, bloqueos, errores y compatibilidad en `02-contrato.md`. Origen: D-04, RQ-04
- [x] 7.4 Documentar secuencia, autorización, concurrencia, riesgos, rollback y diagramas Mermaid en `03-flujo-y-seguridad.md` y `Diagramas/`. Origen: D-01, RQ-01
- [x] 7.5 Documentar comandos, compilación, verificaciones, E2E ejecutada, QA manual y evidencia en `04-pruebas-y-evidencia.md`; ejecutar validaciones OpenSpec y OpsXJ al cierre. Origen: D-06, RQ-06
- [x] 7.6 Documentar en `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md` la reutilización segura del login E2E y la carga, con secretos externos, gate reversible y cierre verificable. Origen: D-06, RQ-06
