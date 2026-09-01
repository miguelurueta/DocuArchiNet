# Inventario de componentes

| Componente | Ubicación | Responsabilidad DOC-42 |
| --- | --- | --- |
| Endpoint ASMX | `webservice/WebServiceWorkflowNotesModern.asmx.vb` | Despacho moderno de lecturas y escrituras. |
| Servicio | `Services/Workflow/Notas/ServicioNotasWorkflow.vb` | Reglas de autorización y validación. |
| Repositorio | `Infrastructure/Repositories/Workflow/MySqlNotasWorkflowRepository.vb` | Preflight, SQL parametrizado, ledger, idempotencia y auditoría. |
| Contratos/modelos | `DTOs/`, `Domain/` y `Models/` de Notas | DTOs, resultados, errores y contexto confiable. |
| E2E | `tools/e2e/` | Sesión, TTY, controles SELECT, evidencia y recursos descartables. |
| Esquema | `Doc/Actualizacion/workflow/Notas/2026-08-31-migracion-transacciones.sql` | DDL revisable y rollback documental. |

`workflow/Webworkflow.aspx` y su code-behind no tienen cambios DOC-42.
