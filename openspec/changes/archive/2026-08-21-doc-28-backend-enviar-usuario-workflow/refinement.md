<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-28-backend-enviar-usuario-workflow

## Fuente y alcance

- Ticket: `DOC-28` — BACKEND-ENVIAR-USUARIO-WORKFLOW.
- Etapa autorizada: `Doc/Actualizacion/workflow/TerminarUsuario/prompts/01-backend-envio-usuario.md`.
- Perfil técnico observado: ASP.NET Web Forms y VB.NET legacy; el cambio se limita a ASMX, Application, modelos y adaptadores de Infrastructure.
- Exclusiones: UI, gate o configuración, `PreviewEnviarTarea`, `EjecutarEnvioTarea`, `ServicioTransicionTarea`, contratos por conector, Pendientes y reasignación de respuesta.

## Contexto inspeccionado

- `workflow/Webworkflow.aspx.vb` usa el recorrido legacy `After_envio_usuario_workflow`; el nuevo límite no lo invoca.
- `workflow/Class_usuario_workflow.vb` muestra que el destino válido es el par usuario–actividad de la ruta, con usuario activo y `UTIL_ASIGNA_TAREA=1`.
- `webservice/WorkflowPreviewSessionContextGate.vb` ya reconstruye el contexto Gestión→Workflow y consulta permisos desde servidor; `CAMBIO_USUARIO` ocupa el índice 18 de ese resultado.
- `webservice/WebServiceWorkflowModern.asmx.vb`, `MySqlTransicionConcurrencyGuard` y `WorkflowLegacyAuditoriaAdapter` ofrecen puntos de extensión reutilizables sin tocar Continuar flujo.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Crear modelos, DTOs, puertos y códigos exclusivos de Enviar a usuario; los contratos públicos no contienen `IdConector`. | `SolicitudTransicionWorkflow` y `IWorkflowLegacyExecutor` exigen conector en `Modelo/Workflow/Terminar`; el flujo nuevo requiere tipos paralelos. | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Calcular `CAMBIO_USUARIO` en `WorkflowPreviewSessionContextGate`, fail-closed, y no evaluar `IWorkflowModernFeatureGate`. | `SolicitaPermisosUsuarioWorkflow` expone `CAMBIO_USUARIO` en el índice 18; Envío a grupo calcula otro permiso en el índice 8. | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | El preview reduce el universo a destinos autorizados y usa solo `SELECT` parametrizado, cursor firmado, orden estable y tamaño máximo en servidor. | La consulta legacy de `Class_usuario_workflow` filtra ruta, estado y `UTIL_ASIGNA_TAREA`; `MySqlEnvioGrupoRepository` aporta el patrón de búsqueda paginada. | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | La ejecución adquiere `GET_LOCK` por tarea y token, relee tarea y reautoriza permiso, ruta, respuesta, usuario, actividad y notificación dentro del lease. | `MySqlTransicionConcurrencyGuard` conserva el lock por conexión y `ServicioEnvioGrupoTarea` demuestra la relectura bajo lock. | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Un adaptador nuevo y exclusivo es el único punto que llama una vez a `ClassWorkflow.Terminar_Tarea_Workflow`, con `Page = Nothing`, actualizaciones Web Forms desactivadas y sin conector. | `WorkflowLegacyExecutorAdapter` rechaza `IdConector <= 0`; no sirve para este envío directo. | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Los resultados públicos normalizan éxito, bloqueo, reintento y advertencias; la auditoría registra `ASMX_ENVIO_USUARIO` y una falla de auditoría no revierte el envío confirmado. | `WorkflowLegacyAuditoriaAdapter` registra datos saneados y `ServicioEnvioGrupoTarea` ya asigna mecanismo específico. | D-06 | RQ-05 | Origen: D-06, RQ-05 |
| D-07 | Preservar sin cambios los endpoints y el recorrido de Continuar flujo, no ejecutar E2E autenticado o cambios de ambiente, y documentar el relevo para la etapa UI. | `WebServiceWorkflowModern.asmx.vb` mantiene operaciones por conector y `TerminarUsuario/00-exploracion-arquitectura-envio-usuario.md` define los límites. | D-07 | RQ-06 | Origen: D-07, RQ-06 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Existen `PreviewEnviarUsuario` y `EjecutarEnvioUsuario` con DTOs exclusivos de usuario. | Cuando el cliente envía un campo de conector, el contrato no lo recibe ni lo devuelve; el resultado público solo expone datos mínimos del destino. | Continuar flujo conserva sus tres campos actuales y sus tipos existentes. |
| RQ-02 | El servidor autoriza con `CAMBIO_USUARIO`, contexto válido, tarea activa y ruta abierta. | Cuando falta sesión, permiso o pertenencia de tarea, preview y ejecución devuelven un código público seguro y no consultan ni mutan un destino. | El permiso no procede de `Session` ni de un valor enviado por navegador. |
| RQ-03 | Preview devuelve una página estable, limitada y filtrada de pares usuario–actividad autorizados. | Cuando el cursor, filtro o tamaño es inválido, se bloquea sin SQL expuesto; cuando el conjunto es extenso, no se entrega una lista completa. | El camino usa exclusivamente lecturas y no crea auditoría, eventos ni cambios de estado. |
| RQ-04 | Ejecución revalida todos los datos bajo lock antes del motor legacy. | Cuando token, respuesta, ruta, usuario, actividad o `UTIL_ASIGNA_TAREA` cambian, se bloquea antes de `Terminar_Tarea_Workflow`; dos llamadas concurrentes no terminan dos veces la tarea. | El lock se libera con la conexión y no abre una transacción paralela al motor legacy. |
| RQ-05 | El único adaptador directo termina la tarea y devuelve éxito, bloqueo, reintento o advertencias sanitizadas. | Cuando el motor confirma la transición con advertencia de correo o evento, el resultado conserva éxito y advertencia; si la auditoría falla, no revierte ese éxito. | No se invocan reasignación de respuesta, `After_envio_usuario_workflow`, handlers Web Forms ni ejecutores por conector. |
| RQ-06 | Pruebas focales y documentación prueban el aislamiento y dejan evidencia reproducible sin E2E autenticado. | Las pruebas cubren permiso, lectura, paginación, validaciones, lock, token, advertencias, auditoría y no regresión de Continuar flujo. | No se cambia configuración de ambiente ni se activan gates. |

## Resultado del refinamiento

- Estado: aprobado para implementar la etapa backend de DOC-28.
- Trazabilidad: cada decisión aparece en `design.md`, `spec.md` y `tasks.md`; las tareas tienen un origen único verificable.
- Validación siguiente: `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-28 --sync`.
