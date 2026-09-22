# Pruebas y evidencia

- `importar-servicio-web-progress-adapter.test.cjs`: ejecución individual/múltiple, concurrencia y recuperación.
- `importar-servicio-web-progress-state-mapping.test.cjs`: fases, conteos y falso éxito.
- `importar-servicio-web-progress-legacy-regression.test.cjs`: aislamiento, ausencia de polling/cancelación/reintento y registro.

La regresión focal incluye intención, preparación, UI y legacy. La validación manual/E2E debe comprobar una ejecución individual, una múltiple, espera sin porcentajes, resultado mixto y cierre sin cancelación.

Antes de una prueba autenticada de `PreviewEnviarTarea` se sigue `tools/e2e/AGENT-RUNBOOK.md`, se obtiene autorización explícita y se restaura el gate apagado con usuarios y grupos vacíos. La evidencia se sanea de credenciales, cookies y conexiones.

## Resultado local 2026-09-22

- `node --test Tests/importar-servicio-web-*.test.cjs`: PASS, 415/415.
- `msbuild GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m`: PASS, 0 errores; 310 advertencias preexistentes del proyecto legacy.
- `openspec validate doc-76-progreso-integracion-sii --strict --json`: PASS.
- E2E autenticado: pendiente de autorización específica para DOC-76; no ejecutado durante esta validación local.
