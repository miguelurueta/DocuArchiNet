# Guía para agentes

## E2E y carga DOC-10

Antes de ejecutar una prueba autenticada de `PreviewEnviarTarea`, leer [tools/e2e/AGENT-RUNBOOK.md](tools/e2e/AGENT-RUNBOOK.md).

- No guardar ni imprimir credenciales, cookies ni cadenas de conexión.
- No ejecutar E2E real ni carga sin autorización explícita para el ambiente y las cuentas de prueba.
- La experiencia moderna es oficial: no reintroducir `WorkflowCentroTrabajoModernActive` ni listas de usuarios o grupos como mecanismo de activación.
- Las consultas de control deben ser solo `SELECT`; el preview no puede cambiar tarea, estado ni auditoría.
