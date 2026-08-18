# PILOTO-DESPLIGUE-CONTROLADO

- Ticket: DOC-14
- Cambio OpenSpec: doc-14-piloto-despligue-controlado
- Clasificacion: cross_cutting (Transversal)

## Evidencia requerida

- [x] unit: `node --test tests/workflow-modern-feature-gate.test.cjs`; 12 pruebas aprobadas el 2026-08-18. También aprobaron `Verify-Doc14PilotGate.ps1` y `Verify-Doc14Telemetry.ps1` en modo aislado.
- [x] build: `msbuild GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m`; compilación compatible aprobada sin errores, con advertencias heredadas registradas en `Doc/Actualizacion/workflow/Terminar/06-piloto-pruebas-rollout/04-pruebas-y-evidencia.md`.
- [x] manual_qa: matriz documentada; la QA visual completa permanece pendiente. La condición de ejecución y la evidencia requerida constan en `Doc/Actualizacion/workflow/Terminar/06-piloto-pruebas-rollout/04-pruebas-y-evidencia.md`.

## QA/E2E WebForms

Se aprobaron los E2E no mutantes de `EjecutarEnvioTarea`: anónimo, sesión autenticada con parámetros inválidos y preview autenticado de la tarea 557. También aprobó el E2E controlado de esa tarea con bloqueo esperado `WORKFLOW_REQUIREMENT_NOT_MET`: no cambió su estado y quedó auditoría del intento. No se ejecutó carga, QA visual completa ni un envío exitoso. Antes de cualquier E2E autenticado se debe leer `tools/e2e/AGENT-RUNBOOK.md`, contar con autorización explícita del ambiente y de las cuentas de prueba, y finalizar con las banderas del gate desactivadas y las listas de alcance vacías.
