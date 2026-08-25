# INTERFAZ-MODERNA-DEVOLVER-TAREA

- Ticket: DOC-33
- Cambio OpenSpec: doc-33-interfaz-moderna-devolver-tarea
- Clasificacion: cross_cutting (Transversal)
## Evidencia requerida

- `unit`: las pruebas focales y la regresión de módulos modernos se ejecutan localmente con `node --test`; cubren preview, confirmación, accesibilidad, aislamiento y retiro legacy. La referencia verificable se registra al cierre con el commit de DOC-33.
- `manual_qa`: se deja constancia de que no se realizó recorrido autenticado en esta corrida. Una E2E requiere autorización explícita independiente y debe usar tarea/cuentas descartables autorizadas.

## QA/E2E WebForms

Las pruebas E2E automatizadas no se suponen disponibles ni fueron ejecutadas. Si se autorizan, se debe seguir `tools/e2e/AGENT-RUNBOOK.md`, no registrar secretos y verificar que preview no cambie tarea, estado ni auditoría. Al finalizar, el gate `WorkflowCentroTrabajoModernActive` debe permanecer `false` con usuarios y grupos vacíos.
