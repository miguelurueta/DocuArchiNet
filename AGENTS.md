# Guía para agentes

## Prioridad del repositorio local

- La carpeta principal del repositorio es la fuente de verdad del trabajo local. Los worktrees son auxiliares; no dejar una entrega final únicamente en ellos.
- Antes de integrar ramas, conservar los cambios locales y resolver las diferencias sin descartar avances de la carpeta principal.
- Al finalizar una entrega autorizada, reunir sus cambios en la carpeta principal, verificarlos y registrarlos en Git; comprobar y comunicar su sincronización con el remoto.
- Conservar datos de ejecución, staging y secretos en sus ubicaciones locales excluidas de Git.

## E2E y carga DOC-10

Antes de ejecutar una prueba autenticada de `PreviewEnviarTarea`, leer [tools/e2e/AGENT-RUNBOOK.md](tools/e2e/AGENT-RUNBOOK.md).

- No guardar ni imprimir credenciales, cookies ni cadenas de conexión.
- No ejecutar E2E real ni carga sin autorización explícita para el ambiente y las cuentas de prueba.
- La experiencia moderna es oficial: no reintroducir `WorkflowCentroTrabajoModernActive` ni listas de usuarios o grupos como mecanismo de activación.
- Las consultas de control deben ser solo `SELECT`; el preview no puede cambiar tarea, estado ni auditoría.
