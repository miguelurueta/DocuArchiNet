<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento aprobado — doc-42-transacciones-notas

## Fuente y alcance

- Ticket: `DOC-42` — TRANSACCIONES-NOTAS.
- Cambio OpenSpec: `doc-42-transacciones-notas`.
- Fuente Jira: `specs/transacciones-notas/jira-context.md` y Prompt 03 de la modernización de Notas.
- Perfil tecnológico confirmado: ASP.NET Web Forms / ASMX, VB.NET y MySQL 5.1 dentro del módulo Workflow.

DOC-42 implementará exclusivamente las mutaciones modernas de Notas. La base actual contiene solicitudes, modelos e interfaces de escritura, pero `WebServiceWorkflowNotesModern.asmx.vb` sólo expone lecturas y `MySqlNotasWorkflowRepository.vb` devuelve `Unavailable` para crear, actualizar y eliminar. No se modifican WebForms, consumidores, endpoints legacy, gates, usuarios, grupos ni configuración de despliegue.

## Contexto inspeccionado

- `DTOs/Workflow/Notas/NotasWorkflowDtos.vb`, `Modelo/Workflow/Notas/NotasWorkflowModels.vb` y `NotasWorkflowInterfaces.vb` contienen contratos tipados para creación, actualización y eliminación, sin identidad ni metadatos de ruta del cliente.
- `Services/Workflow/Notas/ServicioNotasWorkflow.vb` valida contexto, tarea explícita, ruta, contenido BMP de máximo 16.000 UTF-16 y versión; la tarea no procede de selección mutable de sesión.
- `Infrastructure/Repositories/Workflow/MySqlNotasWorkflowRepository.vb` implementa lecturas parametrizadas y deja las tres mutaciones deshabilitadas mediante `Unavailable`.
- `webservice/WebServiceWorkflowNotesModern.asmx.vb` usa `WorkflowPreviewSessionContextGate` y conserva un borde de sólo lectura.
- `tools/e2e/tests/notes-workflow.spec.cjs` y `tools/e2e/scripts/run-notes-workflow-concurrency.cjs` ya contienen escenarios de escritura/concurrencia; sólo pueden ejecutarse con autorización específica de escritura y datos descartables.
- DP-01, DP-03, DP-04, DP-05 y DP-07 están resueltas en el modelo de requisitos de Notas; no queda decisión de negocio que bloquee el diseño.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código o fuente | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Mutaciones sólo en ASMX especializado y capas Workflow, con `idTarea` explícito; no cambian UI, legacy ni gates. | `webservice/WebServiceWorkflowNotesModern.asmx.vb`; DTOs/modelos. | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Persistencia condiciona nota, tarea, actor, estado y versión; contexto sólo de servidor. | `ServicioNotasWorkflow.vb`; repositorio; Prompt 03. | D-02 | RQ-01, RQ-03 | Origen: D-02, RQ-03 |
| D-03 | Crear usa UUID único por tarea y autor, conserva respuesta original 30 días y evita duplicar nota/auditoría. | DP-07; `SolicitudCrearNotaWorkflow`. | D-03 | RQ-02 | Origen: D-03, RQ-02 |
| D-04 | Editar usa ETag SHA-256; eliminar es físico, sólo por propietario y con auditoría atómica sin recuperación. | DP-01 y DP-03; solicitudes de escritura. | D-04 | RQ-03 | Origen: D-04, RQ-03 |
| D-05 | Nota, idempotencia y auditoría comparten transacción; auditoría guarda sólo metadatos, longitud y SHA-256. | DP-05; Prompt 03. | D-05 | RQ-04 | Origen: D-05, RQ-04 |
| D-06 | Preflight exige InnoDB, `TEXT utf8`, índices y almacén idempotente; migración reversible requiere autorización. | DP-04; modelo de requisitos; Prompt 03. | D-06 | RQ-02, RQ-05 | Origen: D-06, RQ-05 |
| D-07 | Pruebas locales no abren MySQL; E2E reutiliza `tools/e2e`, controles `SELECT` y autorización independiente. | `tools/e2e/AGENT-RUNBOOK.md`; escenarios existentes. | D-07 | RQ-06 | Origen: D-07, RQ-06 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Endpoints modernos reciben recurso explícito y derivan contexto sólo de servidor. | Sin contexto, ruta o tarea activa no mutan ni auditan; con contexto válido no usan tarea de sesión. | Preserva lecturas DOC-41 y legado. |
| RQ-02 | Crear valida contenido/UUID y devuelve resultado original ante reintento. | Dos envíos con igual tarea, autor y UUID dejan una nota y una auditoría efectiva. | Retención idempotente 30 días; requiere InnoDB. |
| RQ-03 | Actualizar/eliminar son condicionales por recurso, propietario, estado y ETag. | Dos operaciones con misma versión producen máximo un éxito; recurso cruzado no expone contenido. | Sin borrado lógico ni excepción administrativa inicial. |
| RQ-04 | Cambio, respuesta idempotente y auditoría se confirman o revierten juntos. | Error de auditoría no deja filas parciales ni recursos retenidos. | Auditoría no preserva texto ni cliente como valor anterior. |
| RQ-05 | Esquema sin preflight correcto responde `Unavailable` sin tocar datos. | Motor, columna, índices o almacén ausente impiden escritura. | Migración requiere inspección `SELECT` y autorización por ambiente. |
| RQ-06 | Pruebas focales y E2E están en el cambio y conservan controles de seguridad. | Sin autorización de escritura se registra bloqueo, no se ejecuta E2E ni se habilita gate. | Autorización de lectura no autoriza alteración de estado. |

## Resultado del refinamiento

- Estado: aprobado tras contrastar decisiones de negocio resueltas, Prompt 03 y rutas presentes.
- No se ejecutó E2E, migración ni consulta de ambiente durante el refinamiento.
- Siguiente paso: sincronizar trazabilidad con `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-42 --sync` y comenzar tareas atómicas de implementación.
