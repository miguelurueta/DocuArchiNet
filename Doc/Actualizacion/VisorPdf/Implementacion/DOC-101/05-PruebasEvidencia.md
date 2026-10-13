# Pruebas y evidencia

Fecha local: 2026-10-13.

| Verificación | Resultado |
| --- | --- |
| `node --check` de los tres JavaScript modificados | Aprobado |
| `npm.cmd --prefix tools/e2e run test:workflow:pdf-viewer-performance:policy` | Aprobado, 28/28 |
| Cargador: concurrencia, orden, deduplicación, fallo y reintento | Aprobado para cinco capacidades |
| Política: núcleo, worker, propietarios, orden y matriz DOM | Aprobado |
| Conteos saneados y error worker-entry | Aprobado por contrato |
| Recursos locales en `.vbproj` | Existentes; no requiere cambio del proyecto |

## E2E de rendimiento autorizada

La corrida final autorizada completó tres iteraciones sobre el mismo documento de 34 páginas. El artefacto saneado quedó en `tools/e2e/artifacts/workflow-pdf-viewer-performance.json`.

| Métrica | Línea base | DOC-101 | Variación |
| --- | ---: | ---: | ---: |
| `domInteractive` inicial | 1.679 ms | 389 ms | -76,8 % |
| `domInteractive` recargas | 92–98 ms | 40 / 28 ms | al menos -56,5 % |
| Primer canvas inicial | 2.902 ms | 1.696 ms | -41,6 % |
| Mediana primer canvas | 878 ms | 519 ms | -40,9 % |
| Tareas largas en tres cargas | 5 | 1 | -80,0 % |

Todos los hitos fueron verdaderos, no hubo fallos ni evento `pdf.worker.entry.min.js`. Los conteos del iframe fueron `core=27`, `other=45` y cero para `graph`, `wacom`, `print`, `fabric` y `prettify`.

## Matriz de capacidades autorizada

El artefacto separado `workflow-pdf-viewer-capability-matrix.json` produjo `PASSED`. Grafo y configuración quedaron listos; la firma personal se creó y retiró sin guardar; Print.js, Fabric y Prettify expusieron sus símbolos; el contrato de descarga permaneció disponible y la lectura continuó estable.

Como no había tableta física, se abortó deliberadamente `q.js`: el cargador rechazó la capacidad, conservó PDF, página, scroll y zoom, y luego reintentó con éxito hasta exponer `captureFromSTU`. Por ello el resultado global fue `PASSED_WITH_OBSERVATIONS` con una única observación saneada `WORKFLOW_PDF_OPTIONAL_RESOURCE_FAILED`; la matriz propia fue `PASSED`. No se modificó el documento y no se afirma una captura física Wacom.
