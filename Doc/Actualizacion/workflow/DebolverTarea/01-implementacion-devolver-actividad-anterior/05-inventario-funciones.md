# Inventario de componentes DOC-32

- Ticket: DOC-32
- Cambio OpenSpec: `doc-32-backend-actividad-anterior`
- Clasificación: `cross_cutting`

| Área | Archivo o componente | Responsabilidad | Reutilización y límite |
| --- | --- | --- | --- |
| Dominio | `Modelo/Workflow/Devolver/DevolverActividadModels.vb` | Modela tarea, destino, solicitudes, resultados, auditoría y tipos Ruta/Flujo. | Contratos exclusivos; no usa UI, sesión ni contratos de envío. |
| Dominio | `Modelo/Workflow/Devolver/DevolverActividadInterfaces.vb` | Define puertos para tarea, autorización, preview, ejecución, cursor, guard, auditoría y adaptador. | Permite verificar la capacidad sin acoplarla al motor legacy. |
| Transporte | `DTOs/Workflow/Devolver/DevolverActividadDtos.vb` | Declara DTOs serializables y códigos públicos `WORKFLOW_RETURN_*`. | No serializa SQL, credenciales, excepciones ni detalles técnicos. |
| Aplicación | `Services/Workflow/Devolver/ServicioDevolverActividad.vb` | Orquesta preview, revalidación, lock, ejecución, auditoría y respuesta pública. | Preview no usa guard/auditoría/motor; ejecución relee todo dentro del lease. |
| Persistencia | `Infrastructure/Repositories/Workflow/MySqlDevolverActividadRepository.vb` | Obtiene snapshot, permiso, aristas Ruta/Flujo y destino final. | Conserva separadas las semánticas de `IdConector`; las lecturas de preview son parametrizadas. |
| Cursor | `Infrastructure/Workflow/Devolver/DevolverActividadCursorCodec.vb` | Protege y valida la continuación ligada al snapshot de preview. | Usa `MachineKey`; no es autorización de ejecución. |
| Concurrencia | `Infrastructure/Workflow/Devolver/MySqlDevolverActividadConcurrencyGuard.vb` | Adquiere y libera un lock MySQL por `IdTarea`. | No modifica el guard existente basado en token. |
| Motor legacy | `Infrastructure/Workflow/Devolver/WorkflowLegacyDevolverActividadExecutorAdapter.vb` | Ejecuta la única llamada nueva a `Terminar_Tarea_Workflow`. | Usa `Page = Nothing`, sin actualización de interfaz ni reasignaciones; no toca respuestas ni helpers excluidos. |
| Auditoría | `Infrastructure/Workflow/Terminar/WorkflowLegacyAuditoriaAdapter.vb` | Registra `ASMX_DEVOLVER_ACTIVIDAD` después de la ejecución. | Una falla posterior agrega advertencia y no revierte la transición. |
| ASMX | `webservice/WebServiceWorkflowModern.asmx.vb` | Expone preview y ejecución con sesión habilitada. | Compone únicamente dependencias DOC-32; no cambia endpoints de envío. |
| Pruebas | `tests/workflow-return-activity.test.cjs` | Verifica contratos, seguridad, rutas, ejecución y compatibilidad sin sesión real. | No accede a ambiente ni a credenciales. |
| E2E | `tools/e2e/tests/doc32-return-activity.spec.cjs` y scripts asociados | Ejecuta preview, transición y concurrencia autorizadas con evidencia saneada. | Reutiliza el helper de sesión existente y requiere runbook/autorización. |

Los artefactos de pruebas y documentación son parte de la capacidad, pero no son dependencias de producción. Los endpoints y componentes de envío continúan fuera de este inventario para preservar su compatibilidad.
