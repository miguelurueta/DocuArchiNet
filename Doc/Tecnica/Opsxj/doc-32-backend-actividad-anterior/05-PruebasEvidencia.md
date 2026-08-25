# BACKEND-ACTIVIDAD-ANTERIOR

- Ticket: DOC-32
- Cambio OpenSpec: doc-32-backend-actividad-anterior
- Clasificacion: cross_cutting (Transversal)
## Evidencia requerida

- [x] unit: 2026-08-24; `node --test tools/e2e/tests/e2e-test-resource-lifecycle.test.cjs tools/e2e/tests/workflow-e2e-orchestrator.test.cjs tools/e2e/tests/doc32-return-activity-policy.test.cjs tests/workflow-return-activity.test.cjs`; 44 aprobadas.
- [x] manual_qa: 2026-08-24; validación funcional autorizada mediante E2E autenticada de preview, ejecución real y carrera acotada. Las evidencias saneadas están en `tools/e2e/artifacts/doc32-return-activity-{preview,execution,concurrency}.json`.

## QA/E2E WebForms

La E2E real se ejecutó con una cuenta Workflow autorizada, tareas descartables preparadas y controles ODBC exclusivamente `SELECT`. Preview conservó las huellas de estado y auditoría; ejecución confirmó una única transición hacia la actividad final esperada; la carrera de dos solicitudes produjo una única transición efectiva. La evidencia conserva solo resultados, latencias, banderas y huellas, sin credenciales, cookies, conexiones, tokens ni destinos. No se activó ni modificó el gate y no se alteraron páginas legacy.
