# Pruebas

## Validación local

`tools/validation/Verify-ImportarServicioWebFrontend.ps1` ejecuta:

- arquitectura UI;
- gate integral;
- regresión UI legacy;
- invariancia de almacenamiento;
- progreso, reconciliación y protección de contexto existentes.

Es determinista, no autenticada y no usa red.

## Plataforma E2E reutilizada

Se conserva `tools/e2e/tests/importar-servicio-web-modern.spec.cjs`, sus escenarios registrados, perfiles, autenticación, validadores, controles SELECT y saneamiento. No se crea otro proyecto Playwright ni perfiles paralelos.

Una corrida real requiere leer `tools/e2e/AGENT-RUNBOOK.md` y recibir autorización explícita para ambiente, cuentas, gate y mutación cuando corresponda.
