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

## E2E real autorizado

Se reutilizó quirúrgicamente `test:workflow:platform` con el escenario `import-sii-read`, tarea `219877`, radicado `S002188422` y código de barras `18221398`. La plataforma finalizó correctamente con 7 controles y `sinCambios=SI`.

La corrida confirmó consulta/preview/preparación individual sobre el estado persistido sin repetir la mutación ya consumida. El perfil usó `sampleSize: 1` porque la tarea conserva un único elemento seleccionable; el aislamiento por tarea, la deduplicación y la autorización de apertura quedan cubiertos adicionalmente por las suites focales DOC-77.

Al finalizar se verificó `WorkflowCentroTrabajoModernActive=false`, proveedor vacío y usuarios/grupos vacíos. No hubo diferencias residuales en las páginas legacy controladas.
