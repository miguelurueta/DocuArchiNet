# FIX-SELLOS-SII

- Ticket: DOC-84
- Cambio OpenSpec: doc-84-fix-sellos-sii
- Clasificacion: cross_cutting (Transversal)
## Evidencia requerida

Fecha: 2026-09-29.

- Suite completa Node `importar-servicio-web-*`: 534 pruebas, 534 PASS.
- `npm.cmd --prefix tools/e2e run test:doc83:regression`: 191 pruebas, 191 PASS.
- E2E local `npm.cmd --prefix tools/e2e run test:doc84:workflow-row`: 1 prueba Playwright, 1 PASS.
- MSBuild de `GestionDocumental-Docuarchi.net.vbproj`: PASS, cero errores.
- Validación OpenSpec estricta: PASS.

Las pruebas cubren los ocho campos, escape, destino `wf`, deduplicación, tarea distinta, preservación en reconciliación, separación ENLASE y ausencia de recarga/postback.

La E2E local carga en Chromium los scripts productivos del appender DOC-84 y `GredviewControl.js`. Comprueba la fila real, `id_wf`, los ocho campos de `idd_wf`, tipología, icono y clic en ver, eliminar, cambiar tipología, firma, versiones y reemplazo, sin solicitudes de red.

La ampliación de tipología cubre nombre canónico, mayúsculas/tildes, prioridad frente a otra opción obligatoria, errores ortográficos menores y ambigüedad cerrada.

## QA/E2E WebForms

No se ejecutó E2E autenticada, prueba de carga ni activación de gate. La E2E local no inicia sesión ni contacta un ambiente. Las corridas reales requieren autorización explícita de ambiente y cuentas y deben seguir `tools/e2e/AGENT-RUNBOOK.md`.
