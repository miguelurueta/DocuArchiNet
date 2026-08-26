# Inventario de componentes DOC-36

- Ticket: DOC-36
- Cambio OpenSpec: `doc-36-backend-devolucion-usuario-anterior`
- Clasificación: `cross_cutting`

| Área | Archivo o componente | Responsabilidad |
| --- | --- | --- |
| Dominio | `Modelo/Workflow/DevolverUsuarioAnterior/` | Modelos y puertos exclusivos. |
| Transporte | `DTOs/Workflow/DevolverUsuarioAnterior/` | DTOs, token y códigos públicos. |
| Aplicación | `Services/Workflow/DevolverUsuarioAnterior/` | Preview, ejecución, lock y auditoría. |
| Persistencia | `Infrastructure/Repositories/Workflow/MySqlDevolverUsuarioAnteriorRepository.vb` | SELECT parametrizados de tarea, historial y usuario elegible. |
| ASMX | `webservice/WebServiceWorkflowModern.asmx.vb` | Endpoints DOC-36. |
| E2E | `tools/e2e/tests/doc36-return-user-previous.spec.cjs` | Preview, ejecución y concurrencia dinámica. |

La actividad y el usuario anterior se obtienen del historial; no son entradas configurables.
