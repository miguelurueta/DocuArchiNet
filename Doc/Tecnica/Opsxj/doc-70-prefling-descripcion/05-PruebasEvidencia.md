# PREFLING-DESCRIPCION

- Ticket: DOC-70
- Cambio OpenSpec: doc-70-prefling-descripcion
- Clasificacion: cross_cutting (Transversal)
## Evidencia requerida

- `unit`: `node --test tests/importar-servicio-web-*.test.cjs`; resultado aprobado, 361 pruebas, ejecutado durante la verificación de DOC-70 el 2026-09-20.
- `build`: compilación MSBuild aprobada; solo se conservaron advertencias heredadas del ensamblado.
- `policy`: `npm.cmd --prefix tools/e2e run test:doc56:policy`; resultado aprobado, 18 pruebas.
- `manual_qa`: inspección del contrato público y de las consultas `SELECT` de control sobre el recurso descartable autorizado; el plan fue coherente con los efectos persistidos.

## QA/E2E WebForms

Se reutilizó el runner real de `tools/e2e` con gate temporal y restauración obligatoria. La ejecución final sobre la tarea descartable 220582 produjo una intención completa con un elemento y dos documentos relacionados; almacenamiento, relación, índice y caché quedaron confirmados, sin errores y con veredicto `PASSED`.

El gate terminó desactivado y con usuarios y grupos vacíos. La evidencia saneada no contiene credenciales, cadenas de conexión ni datos sensibles. La limitación pendiente es que la ausencia de llamadas SII durante el preflight se demuestra estructuralmente y por pruebas automatizadas, no mediante una medición de telemetría real aislada por `OperationId`.
