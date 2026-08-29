<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento — doc-40-backend-contratos-notas

## Fuente y alcance

- Ticket: `DOC-40` — BACKEND-CONTRATOS-NOTAS.
- Cambio OpenSpec: `doc-40-backend-contratos-notas`.
- Fuente funcional: `specs/backend-contratos-notas/jira-context.md` y `Doc/Actualizacion/workflow/Notas/Exploracion/`.
- Perfil técnico confirmado por el ticket y la inspección: ASP.NET Web Forms/VB.NET, ASMX y MySQL mediante ADO.NET.

El cambio queda limitado a la fundación de backend para Notas dentro de Workflow. Esta aprobación cubre la arquitectura, los contratos, la trazabilidad y la documentación; no aprueba cambios de código, configuración, datos, pruebas ejecutables ni ambiente.

## Contexto inspeccionado

- `webservice/WorkflowPreviewSessionContextGate.vb` resuelve sesión autenticada y permisos en servidor; sus métodos específicos muestran el patrón de composición fail-closed reutilizable.
- `Modelo/Workflow/Terminar/WorkflowModernInterfaces.vb` define `ITareaWorkflowRepository.ObtenerTarea(contexto, idTarea)` sin lectura de sesión en el repositorio.
- `Infrastructure/Repositories/Workflow/MySqlWorkflowPreviewRepositories.vb` usa `IModuleConnectionFactory`, `IDataExecutor` y parámetros MySQL para consultas del patrón moderno.
- `ContextoModuloWorkflow.IdRutaWorkflow` se valida al construir el contexto; `MySqlTareaWorkflowRepository.ObtenerTarea` obtiene `TareaWorkflow.IdRuta` desde el estado de la tarea y los metadatos se resuelven con `rutas_workflow` y `@idRuta`.
- `workflow/Class_anotacion_tarea.vb` y las operaciones de Notas de `webservice/WebServiceWorkflow.asmx.vb` representan la ruta legacy; crear, editar y eliminar toman la tarea desde una selección mutable de sesión y no son una base reutilizable para el dominio moderno.
- Los documentos de exploración registran los riesgos de autorización, pertenencia nota-tarea, SQL concatenado, concurrencia, auditoría y aislamiento entre pestañas.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | DOC-40 define solo la fundación interna de Notas de Workflow; no cambia UI, consumidor, endpoint publicado, gate, esquema ni datos. | `workflow/Webworkflow.aspx`, `webservice/WebServiceWorkflow.asmx.vb` y `WorkflowCentroTrabajoModernActive` permanecen fuera de este cambio. | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Cada contrato moderno recibe `idTarea`; las operaciones sobre nota reciben también `idNota`, sin usar la tarea seleccionada de sesión. | `ITareaWorkflowRepository.ObtenerTarea(contexto, idTarea)` en `Modelo/Workflow/Terminar/WorkflowModernInterfaces.vb`. | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | El gate de Notas resuelve identidad, grupo y permiso desde sesión autenticada y falla cerrado. | `WorkflowPreviewSessionContextGate.vb` resuelve contexto y permisos en servidor para operaciones modernas existentes. | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Los contratos separan solicitudes, respuestas y resultados funcionales estables sin detalles técnicos. | Los resultados de previsualización y las interfaces modernas del módulo sirven de patrón de tipado y separación. | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Persistencia futura mediante repositorio Workflow parametrizado, sin copiar, envolver ni extender `Class_anotacion_tarea`. | `MySqlWorkflowPreviewRepositories.vb` concentra fábricas de conexión, ejecutor ADO.NET y parámetros. | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | La futura escritura usa borrado físico auditado, solo propietario, histórico de lectura autorizado, texto plano de máximo 16.000 unidades UTF-16, auditoría por huella y longitud, e idempotencia de 30 días. Su implementación queda condicionada a un preflight por esquema. | La consulta MySQL de solo lectura confirmó siete `ANOTACION_TAREA` MyISAM con `TEXT utf8`, índice solo por tarea y tres `wf_log_workflow` InnoDB con datos de auditoría en `latin1`; sin migración no hay atomicidad nota-auditoría. | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | La verificación de esta fundación será local y no productiva; E2E inicia junto al primer recorrido de usuario expuesto. | El ticket establece que no hay endpoint ni recorrido verificable en esta fase y el runbook limita las pruebas autenticadas. | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | La ruta de negocio de Workflow es parte del contexto autorizado y snapshot de tarea; no se acepta ruta ni metadatos desde cliente. | `ContextoModuloWorkflow.IdRutaWorkflow`, `TareaWorkflow.IdRuta`, `rutas_workflow` y la validación de identificadores seguros en `MySqlWorkflowPreviewRepositories.vb`. | D-08 | RQ-08 | Origen: D-08, RQ-08 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | DOC-40 no modifica superficies, datos ni configuración productiva. | La revisión de diff solo contiene contratos, documentación y, tras autorización, capas internas de Workflow. | Conserva la ruta legacy sin doble operación. |
| RQ-02 | Cada solicitud moderna identifica la tarea de forma explícita y validable. | Dos pestañas sobre tareas distintas conservan su recurso de solicitud, sin heredar selección mutable. | Evita tarea y auditoría cruzadas. |
| RQ-03 | La autorización procede del servidor y falla cerrada. | Sesión, identidad, grupo, permiso o tarea no resolubles impiden procesar la operación. | Evita invocación directa sin autorización. |
| RQ-04 | El dominio devuelve códigos funcionales tipados y seguros. | Un rechazo de autorización, estado, pertenencia, versión o contenido no revela SQL ni excepción. | Evita filtración técnica y contratos ambiguos. |
| RQ-05 | La capa moderna queda aislada de WebForms y de la clase legacy. | Servicio y repositorio no conocen `Page`, `GridView`, `UpdatePanel` ni `HttpContext`; los parámetros llegan al repositorio. | Reduce SQL concatenado y acoplamiento de interfaz. |
| RQ-06 | La política de futura mutación está documentada y no se ejecuta sin el preflight de datos. | DOC-40 no publica escritura; en MySQL 5.1 la siguiente fase rechaza con `Unavailable` un esquema sin InnoDB, `TEXT utf8`, auditoría, índices o idempotencia, y rechaza caracteres fuera de Unicode BMP. | Evita una operación no atómica o una semántica implícita. |
| RQ-07 | El primer código autorizado cuenta con evidencia local reproducible. | Pruebas unitarias cubren gate, contratos y resultados; E2E se incorpora cuando exista recorrido expuesto y autorización. | Evita E2E sin superficie verificable y acceso no autorizado a ambiente. |
| RQ-08 | La ruta se resuelve y valida en servidor como parte de la tarea autorizada. | Ruta ausente, inválida o incoherente bloquea la operación; la solicitud no aporta nombre de ruta, tabla ni metadatos. | Evita acceso cruzado y construcción dinámica controlada por cliente. |

## Trazabilidad

Cada decisión D-01 a D-08 se desarrolla en `design.md`, se refleja en `specs/backend-contratos-notas/spec.md` y se vincula en `tasks.md` mediante el formato `Origen: D-XX, RQ-XX`. Las tareas de código, prueba, integración y cierre permanecen sin iniciar hasta recibir autorización explícita.

## Resultado del refinamiento

- Estado: aprobado; fundación interna implementada bajo autorización para las tareas 2.1–3.4.
- Implementado sin exposición: contratos, DTOs, servicio, gate y repositorio de Notas fail-closed; no se modificaron endpoint, consumidor, configuración de gate, base de datos ni ambiente.
- Evidencia local: `node --test tests/workflow-notes-contracts.test.cjs tests/workflow-user-send.test.cjs` con 16 pruebas aprobadas y `msbuild.exe GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m` con 0 errores.
- Comando de trazabilidad: `npm.cmd --prefix Tools/opsxj run opsxj:refine -- DOC-40 --sync`.

## Continuidad restringida

La fase de lectura se encuentra definida en `Prompt/02-lectura-listado-y-contador.md` y reutilizará esta fundación con E2E integrada en su propio cambio. DOC-40 no introduce persistencia; la fase de escritura requiere una migración autorizada y el preflight por esquema definido en el diseño. La validación y el cierre final solo requieren consolidar evidencia local y validar OpenSpec.
