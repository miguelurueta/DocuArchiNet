# Inventario de superficies verificadas

- Ticket: DOC-34
- Cambio OpenSpec: `doc-34-verificacion-transversal-devolver-tarea`

| Superficie | Responsabilidad comprobada | Límite |
| --- | --- | --- |
| `WebServiceWorkflowModern.asmx.vb` | Endpoints ASMX y contexto de sesión de devolución. | No maneja controles Web Forms ni expone errores técnicos. |
| `ServicioDevolverActividad.vb` | Preview, permiso, token, lock, ejecución y auditoría. | El navegador no aporta destino, Ruta, Flujo ni identidad. |
| `MySqlDevolverActividadRepository.vb` | Snapshot y conectores entrantes Ruta/Flujo. | Preview solo lee y conserva semántica aislada de conector. |
| `WorkflowLegacyDevolverActividadExecutorAdapter.vb` | Frontera única hacia el motor legacy. | Sin reasignación o tratamiento de respuestas. |
| `workflow-return-activity-ui.js` | Lista, búsqueda, páginas y selección. | No consulta feature gate ni módulos de otras transiciones. |
| `workflow-return-activity-confirmation.js` | Confirmación y resultado correlacionado. | Solo envía tarea, conector y token. |
| `Webworkflow.aspx` y code-behind | Disparador y modal modernos. | Usuario anterior permanece como operación distinta; no es fallback. |
| Suites CJS DOC-32/DOC-33 | Contrato, regresión y políticas. | No requieren credenciales ni ambiente. |

La verificación no modifica estas superficies; las documenta para la decisión de fase 04.
