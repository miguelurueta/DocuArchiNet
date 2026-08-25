<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-34-verificacion-transversal-devolver-tarea

## Fuente y alcance

- Ticket: `DOC-34` — VERIFICACION-TRANSVERSAL-DEVOLVER-TAREA.
- Cambio OpenSpec: `doc-34-verificacion-transversal-devolver-tarea`.
- Fuente Jira: `specs/verificacion-transversal-devolver-tarea/jira-context.md`.
- Alcance: revisión reproducible de DOC-32 y DOC-33; no incorpora una nueva capacidad funcional.

DOC-34 produce evidencia local y documentación técnica para decidir la fase 04. No cambia código de producción, configuración, contratos, estado de tareas, auditoría ni datos. Tampoco ejecuta E2E autenticada, carga, despliegue, archivo, publicación o liberación automática.

## Contexto inspeccionado

- `webservice/WebServiceWorkflowModern.asmx.vb` expone `PreviewDevolverActividad` y `EjecutarDevolverActividad` como borde HTTP del contrato.
- `Services/Workflow/Devolver/ServicioDevolverActividad.vb`, `Infrastructure/Repositories/Workflow/MySqlDevolverActividadRepository.vb` y `Infrastructure/Workflow/Devolver/WorkflowLegacyDevolverActividadExecutorAdapter.vb` contienen el contrato de preview, revalidación y ejecución de DOC-32.
- `js/workflow/workflow-return-activity-ui.js`, `js/workflow/workflow-return-activity-confirmation.js`, `workflow/Webworkflow.aspx` y sus pruebas CJS corresponden a la interfaz de DOC-33.
- `tests/workflow-return-activity*.test.cjs`, `tools/e2e/tests/doc32-return-activity-policy.test.cjs` y `tools/e2e/tests/doc33-return-activity-ui-policy.test.cjs` aportan pruebas focales y de política sin requerir ambiente autenticado.
- `Doc/Actualizacion/workflow/DebolverTarea/01-implementacion-devolver-actividad-anterior/` y `02-interfaz-moderna-devolver-actividad-anterior/` son el paquete documental que DOC-34 debe actualizar con resultados saneados.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | La verificación es local y no mutante: solo compilación, CJS/VB, análisis estático y QA manual no autenticada; no se invocan endpoints que alteren estado. | `tools/e2e/AGENT-RUNBOOK.md`, suites CJS locales y límites de `jira-context.md`. | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | El preview se valida como lectura autorizada: usa conectores entrantes de Ruta o Flujo, filtro de universo, orden/cursor/límite y token sin cambiar tarea, estado ni auditoría. | `ServicioDevolverActividad.vb`, `MySqlDevolverActividadRepository.vb`, `workflow-return-activity.test.cjs`. | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | La ejecución se valida con permiso, token vigente, lock exclusivo, revalidación del conector entrante y auditoría/notificaciones saneadas; la concurrencia se cubre por contrato y evidencia previa, sin repetir E2E real. | `ServicioDevolverActividad.vb`, `WorkflowLegacyDevolverActividadExecutorAdapter.vb`, `doc32-return-activity-policy.test.cjs`. | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | La interfaz moderna consume el preview y ejecuta únicamente el destino elegido; no consulta el feature gate, no deja un postback/handler Web Forms alcanzable y bloquea interacción mientras espera la respuesta. | `workflow-return-activity-ui.js`, `workflow-return-activity-confirmation.js`, `workflow-return-activity-ui.test.cjs`, `workflow-return-activity-confirmation.test.cjs`. | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | La salida compara los contratos de transiciones vecinas, documenta cobertura y límites con información saneada, y emite una sola recomendación para fase 04: apto, bloqueado o requiere corrección. | pruebas de envío/usuario anterior, documentación `DebolverTarea` y resultados locales. | D-05 | RQ-05 | Origen: D-05, RQ-05 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | La corrida de DOC-34 conserva el estado del repositorio funcional y no solicita credenciales ni modifica el ambiente. | Cuando se ejecutan las verificaciones aprobadas, entonces solo se generan resultados locales y documentación saneada. | Evita usar una validación como vía de modificación de ambiente. |
| RQ-02 | La evidencia demuestra que el preview devuelve únicamente destinos de devolución autorizados y no persiste cambios. | Cuando se revisan código y pruebas focales, entonces conectores salientes no se aceptan como sustituto de conectores entrantes y se conservan filtro, Ruta/Flujo, cursor, orden y límite. | Previene exposición de destinos y regresiones en paginación. |
| RQ-03 | La evidencia demuestra que una ejecución solo admite el preview vigente y un conector entrante autorizado bajo exclusión por tarea. | Cuando se revisan contrato y pruebas, entonces permiso, token, lock, revalidación, concurrencia y auditoría se encuentran cubiertos; cualquier brecha queda asociada a corrección reproducible. | Previene carreras, ejecución duplicada y auditoría no saneada. |
| RQ-04 | La evidencia demuestra que la UI no depende del gate y conserva confirmación segura, bloqueo temporal y accesibilidad básica. | Cuando se revisan scripts, marcado y CJS, entonces no hay postback, handler ni fallback Web Forms para devolver; cancelar y cerrar respetan el bloqueo mientras se espera respuesta. | Previene rutas legacy alcanzables y acciones duplicadas del usuario. |
| RQ-05 | El paquete documental registra resultados, cobertura, límites, correlaciones saneadas y decisión inequívoca para fase 04. | Cuando termina la verificación, entonces cada escenario crítico está aprobado o vinculado a una corrección y la recomendación no queda ambigua. | Hace reproducible el release decision y preserva contratos de Continuar, Enviar a usuario, Enviar a grupo y Usuario anterior. |

## Reglas de trazabilidad

1. Cada decisión `D-XX` está desarrollada en `design.md`, reflejada en al menos un requirement de `spec.md` y vinculada en `tasks.md` mediante `Origen: D-XX, RQ-XX`.
2. Las pruebas y la QA de DOC-34 no pueden añadir llamadas autenticadas ni operaciones mutantes; una excepción requiere un ticket de corrección separado.
3. La documentación no incluye credenciales, cookies, URL de conexión ni datos operativos identificables.
4. La recomendación de fase 04 procede de resultados verificables; un hallazgo crítico se registra como bloqueo o corrección, sin modificar la implementación dentro de DOC-34.

## Resultado del refinamiento

- Estado: `approved` para una verificación transversal de solo lectura.
- Siguiente paso: sincronizar esta trazabilidad y ejecutar los controles locales definidos en `tasks.md`.
