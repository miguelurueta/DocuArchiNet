<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-14-piloto-despligue-controlado

## Fuente y alcance

- Ticket: `DOC-14` — PILOTO-DESPLIGUE-CONTROLADO.
- Cambio OpenSpec: `doc-14-piloto-despligue-controlado`.
- Fuente Jira: `specs/piloto-despligue-controlado/jira-context.md`.
- Confirmación de alcance inicial: mantener el piloto desactivado y refinar el plan sobre la configuración existente. La aprobación posterior de promoción habilitó el modo oficial explícito.

La aprobación de este refinamiento autorizó el diseño y la planificación. La activación oficial posterior fue aprobada de forma separada; no autoriza usar cuentas piloto, ejecutar E2E/carga ni modificar datos de un ambiente.

## Contexto inspeccionado

- `Infrastructure/Workflow/Terminar/ConfiguracionWorkflowModernFeatureGate.vb` implementa el gate servidor, usuarios, grupos y exclusiones; el modo oficial requiere una bandera separada, listas vacías y metadatos válidos.
- `workflow/WorkflowModernPresentationBootstrap.vb` y `workflow/Webworkflow.aspx.vb` consumen el mismo `IWorkflowModernFeatureGate` para la capa visual.
- `webservice/WebServiceWorkflowModern.asmx.vb` y `Services/Workflow/Terminar/ServicioTransicionTarea.vb` revalidan el gate antes del preview y la ejecución; el guard de concurrencia y el adaptador legacy permanecen como frontera operativa.
- `Modelo/Workflow/Terminar/WorkflowModernModels.vb`, `Modelo/Workflow/Terminar/WorkflowModernInterfaces.vb` y `Infrastructure/Workflow/Terminar/WorkflowLegacyAuditoriaAdapter.vb` ya delimitan una auditoría adicional sobre la bitácora legacy.
- `Web.config` usa `WorkflowCentroTrabajoModernActive=true` y `WorkflowCentroTrabajoModernOfficialMode=true`, con listas de usuario/grupo vacías y metadatos completos. El rollback devuelve ambas banderas a `false`.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Exigir inclusión explícita y metadatos de piloto; el modo oficial requiere bandera separada, listas vacías y metadatos válidos. Las exclusiones tienen precedencia. | `Infrastructure/Workflow/Terminar/ConfiguracionWorkflowModernFeatureGate.vb` | D-01 | RQ-01 | 1.1, 1.2, 1.5, 4.1 |
| D-02 | La página consume `WorkflowModernPresentationBootstrap`; preview y ejecución conservan gate servidor y nunca ejecutan fallback automático. | `workflow/Webworkflow.aspx.vb`, `workflow/WorkflowModernPresentationBootstrap.vb`, `webservice/WebServiceWorkflowModern.asmx.vb` | D-02 | RQ-02 | 1.3, 1.4, 4.2 |
| D-03 | Extender la auditoría tipada y su adaptador existente con telemetría mínima sanitizada, sin una persistencia paralela. | `Modelo/Workflow/Terminar/WorkflowModernModels.vb`, `Infrastructure/Workflow/Terminar/WorkflowLegacyAuditoriaAdapter.vb` | D-03 | RQ-03 | 2.1, 2.2, 2.3, 4.3 |
| D-04 | El rollback es configuración a legacy, no reversión de negocio; mantiene el guard de concurrencia y audita la acción. | `Services/Workflow/Terminar/ServicioTransicionTarea.vb`, `Web.config` | D-04 | RQ-04 | 3.1, 3.2, 4.4 |
| D-05 | La promoción requiere reporte, umbrales, responsables y aprobación; eventos críticos bloquean el piloto. | `IAuditoriaTransicionRepository`, paquete documental DOC-14 | D-05 | RQ-05 | 2.4, 3.3, 5.2 |
| D-06 | La evidencia combina pruebas focales, compilación cuando sea disponible y QA manual autorizado; E2E/carga no se presuponen. | `AGENTS.md`, `tests/`, `tools/e2e/AGENT-RUNBOOK.md` | D-06 | RQ-06 | 4.5, 4.6, 5.1, 5.3 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | El gate devuelve activo a una inclusión de piloto válida o a modo oficial explícito con listas vacías; sin ello falla a legacy. | Pruebas de contexto, exclusión, lista vacía, inclusión y modo oficial. | Evita una exposición global implícita. |
| RQ-02 | Página, preview y ejecución responden a la misma decisión; una llamada directa fuera del piloto queda bloqueada. | Cambio de gate antes de envío y acceso ASMX fuera del alcance. | Conserva página, permisos y motor legacy. |
| RQ-03 | La bitácora contiene contrato mínimo y no filtra datos sensibles. | Éxito, bloqueo, error y fallo del adaptador. | Reutiliza el log legacy sin romper el resultado funcional. |
| RQ-04 | Desactivar ambas banderas del gate devuelve nuevas aperturas a legacy y no altera transiciones confirmadas. | Rollback en fallo y con concurrencia. | No hay migración, SQL de reversa ni estado duplicado. |
| RQ-05 | El reporte permite decidir bloqueo o promoción con umbrales y aprobación registrados. | Evento crítico marca bloqueo. | La promoción no es automática. |
| RQ-06 | La evidencia deja comandos, limitaciones, matriz y estado final seguro del gate. | E2E/carga solo con autorización y restauración final. | Cumple las restricciones operativas del repositorio. |

## Resultado del refinamiento

- Estado: `approved` para planificación e implementación local.
- Cobertura: D-01 a D-06 y RQ-01 a RQ-06 están reflejados en diseño, especificación y tareas.
- Restricción operativa: cualquier piloto autenticado, E2E o carga requiere autorización explícita de ambiente y cuentas; la activación oficial queda sujeta a la aprobación de promoción y al rollback documentado.
