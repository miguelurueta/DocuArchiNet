# NUCLEO-INTRFAZ-INTEGRACION-SII

- Ticket: DOC-72
- Cambio OpenSpec: doc-72-nucleo-intrfaz-integracion-sii
- Clasificación: cross_cutting

## Evidencia requerida

- unit: PASS el 2026-09-21, 18/18 pruebas con `node --test` sobre core, registro/UI, accesibilidad, gate y regresión legacy.
- manual_qa: revisión estática del marcado, registro condicional de assets, configuración versionada y preservación de ambos disparadores; sin sesión autenticada.
- build: PASS el 2026-09-21 con MSBuild del proyecto WebForms; código de salida 0, con advertencias legacy preexistentes y sin errores.
- syntax: PASS con `node --check` sobre los cuatro módulos JavaScript y `git diff --check`.

## QA/E2E WebForms

No se ejecutó E2E autenticado, carga ni activación del gate porque no hubo autorización explícita para ambiente y cuentas. `WorkflowCentroTrabajoModernActive` permaneció `false` y las listas de usuarios y grupos permanecieron vacías. Una corrida futura deberá seguir `tools/e2e/AGENT-RUNBOOK.md` y restaurar esos valores al finalizar.
