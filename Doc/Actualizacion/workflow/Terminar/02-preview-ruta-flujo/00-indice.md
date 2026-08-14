# DOC-10 — Preview seguro de ruta y flujo

- Ticket: `DOC-10`.
- Estado: implementado, E2E automatizada reejecutada localmente para un flujo y una ruta, y carga autenticada medida a 20/30 solicitudes; QA manual permanece pendiente.
- Alcance: previsualizar destinos de una tarea por el ASMX paralelo `WebServiceWorkflowModern.asmx`.
- Compatibilidad: `workflow/Webworkflow.aspx` y `workflow/Webworkflow.aspx.vb` no se modifican; el envío legacy conserva la escritura, eventos, correo y auditoría.

| Documento | Contenido |
| --- | --- |
| [01-arquitectura.md](01-arquitectura.md) | Límites, capas y dependencias. |
| [02-contrato.md](02-contrato.md) | Entrada, sesión, DTO JSON y bloqueos. |
| [03-flujo-y-seguridad.md](03-flujo-y-seguridad.md) | Secuencia, autorización, concurrencia y rollback. |
| [04-pruebas-y-evidencia.md](04-pruebas-y-evidencia.md) | Compilación, validaciones, E2E ejecutada, evidencia y QA manual pendiente. |
| [05-consumo-frontend-asmx.md](05-consumo-frontend-asmx.md) | Integración JavaScript segura del ASMX. |
| [06-inventario-tecnico.md](06-inventario-tecnico.md) | Funciones, clases, rutas y dependencias reales. |
| [Runbook de agentes](../../../../../tools/e2e/AGENT-RUNBOOK.md) | Reutilización segura del login E2E y la carga. |
| [07-checklist-qa-manual.md](07-checklist-qa-manual.md) | Ejecución humana pendiente para cerrar DOC-10. |

## Diagramas Mermaid

Los siguientes archivos son Markdown con bloques `mermaid`, para que el visor los renderice directamente.

| Diagrama | Propósito |
| --- | --- |
| [01-casos-de-uso.md](Diagramas/01-casos-de-uso.md) | Consumidores, resultado y límites del preview. |
| [02-clases-y-componentes.md](Diagramas/02-clases-y-componentes.md) | Capas, puertos e implementaciones. |
| [01-secuencia-preview.md](Diagramas/01-secuencia-preview.md) | Recorrido autorizado y bloqueado. |
| [04-estados-preview.md](Diagramas/04-estados-preview.md) | Estados observables de la solicitud. |
| [05-decision-flujo-ruta.md](Diagramas/05-decision-flujo-ruta.md) | Decisión y filtros de flujo/ruta. |
| [06-carga-concurrencia.md](Diagramas/06-carga-concurrencia.md) | Sesiones, ráfaga ASMX y métricas de carga. |

Archivos principales: `webservice/WebServiceWorkflowModern.asmx(.vb)`, `webservice/WorkflowPreviewSessionContextGate.vb`, `Services/Workflow/Terminar/ServicioTransicionTarea.vb`, `Infrastructure/Repositories/Workflow/MySqlWorkflowPreviewRepositories.vb`, `Infrastructure/Shared/Data/WorkflowModuleConnectionFactory.vb`, `tools/validation/Verify-Doc10Preview.ps1` y `tools/e2e/Invoke-Doc10PreviewE2E.ps1`.
