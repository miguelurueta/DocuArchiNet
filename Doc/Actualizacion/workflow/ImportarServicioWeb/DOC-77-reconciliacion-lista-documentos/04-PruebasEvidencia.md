# Pruebas y evidencia

- Ticket: DOC-77
- Cambio OpenSpec: doc-77-reconciliacion-lista-documentos
- Clasificacion: cross_cutting

## Automatización

Comandos ejecutados el 22 de septiembre de 2026:

```powershell
node --test Tests/importar-servicio-web-*.test.cjs
msbuild GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m
```

Resultados:

- Regresión Importar Servicio Web: 425 pruebas aprobadas, 0 fallidas.
- MSBuild Debug: 0 errores, 310 advertencias preexistentes.

Suites DOC-77:

- `Tests/importar-servicio-web-reconciliation-ui.test.cjs`
- `Tests/importar-servicio-web-document-list-adapter.test.cjs`
- `Tests/importar-servicio-web-task-isolation.test.cjs`

## Cobertura

- Get/reconcile mediante API moderna y contexto autorizado.
- Reconciliación única por identidad externa.
- Timeout, ausencia y estado desconocido con clasificación conservadora.
- Reapertura desde snapshot persistido.
- Recorrido único, deduplicación y fallback autoritativo.
- Rechazo de tarea distinta, cambio de tarea y documento no confirmado.
- Apertura solo con identificador interno positivo.

## E2E pendiente

El E2E real requiere autorización explícita del ambiente y cuentas, más identificadores de tarea/radicado/código de barras. Debe seguir `tools/e2e/AGENT-RUNBOOK.md`, dejar el gate apagado y ejecutar controles SQL únicamente de lectura.
