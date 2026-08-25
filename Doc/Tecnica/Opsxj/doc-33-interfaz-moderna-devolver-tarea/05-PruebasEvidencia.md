# INTERFAZ-MODERNA-DEVOLVER-TAREA

- Ticket: DOC-33
- Cambio OpenSpec: doc-33-interfaz-moderna-devolver-tarea
- Clasificacion: cross_cutting (Transversal)
## Evidencia requerida

- `unit`: las pruebas focales y la regresión de módulos modernos se ejecutan localmente con `node --test`; cubren preview, confirmación, accesibilidad, aislamiento y retiro legacy. La referencia verificable se registra al cierre con el commit de DOC-33.
- `manual_qa`: la corrida autenticada autorizada completó preview UI, devolución UI y bloqueo con respuesta retenida. La evidencia almacenada permanece saneada; una nueva E2E requiere autorización explícita y recursos descartables diferentes.

## QA/E2E WebForms

La E2E de UI se ejecutó con autorización expresa. `doc33-return-activity-ui.spec.cjs` confirmó preview no mutante, devolución a través del modal moderno y respuesta de backend retenida. Esta última verificó doble envío, cancelar, X, fondo, Escape, cierre programático, modal de devolución y `beforeunload` mientras el resultado permaneció pendiente.

La corrida requiere autorización explícita, una cuenta Workflow y dos tareas descartables distintas. Se crea un perfil no sensible a partir de DOC-32 con `create-doc33-workflow-ui-profile.cjs`; no contiene cuentas, contraseñas, tokens, cookies, URLs de conexión ni autorizaciones. Consulte `tools/e2e/AGENT-RUNBOOK.md` para los comandos por etapa. Al finalizar, el gate `WorkflowCentroTrabajoModernActive` debe permanecer `false` con usuarios y grupos vacíos.
