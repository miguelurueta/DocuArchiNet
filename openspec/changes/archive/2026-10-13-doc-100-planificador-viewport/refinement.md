<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-100-planificador-viewport

## Fuente y alcance

- Ticket: `DOC-100` — PLANIFICADOR-VIEWPORT.
- Cambio OpenSpec: `doc-100-planificador-viewport`.
- Fuente Jira: `specs/planificador-viewport/jira-context.md`.
- Perfil tecnológico: JavaScript legacy, PDF.js, jQuery, Node.js y Playwright dentro de ASP.NET WebForms.
- Alcance: planificación/render cliente y runner E2E de lectura; se excluyen backend, persistencia, streaming, dependencias y guardado de grafos.

## Contexto inspeccionado

- `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js`: `setZoom`, `_setScrollListener`, `_visiblePages`, `loadPages`, `_loadingTask`, `_cleanPage`, `_renderPage`, `rotate`.
- `pdfjs/pdf_anotate_view/script.js`: `load_pdf_viwer_anotation` y handlers de zoom/rotación.
- `tools/e2e/scripts/run-workflow-pdf-viewer-performance-interactive.cjs`: probe, métricas, errores, evidencia y estado final.
- `tools/e2e/tests/workflow-pdf-viewer-performance-policy.test.cjs`: contratos actuales de perfil, sanitización y recorrido.
- Exploración del visor: línea base, condición de carrera, escaneo global y error por iteración.
- Compatibilidad: contratos públicos, IDs DOM, `let pdfViewer`, página activa, scroll, anotaciones, rotación, firma e impresión.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Clasificar errores en registros permitidos sin mensaje, URL, ruta ni datos documentales. | Runner: `installPerformanceProbe`, eventos de `runMeasurement` | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Derivar `PASSED`, `PASSED_WITH_OBSERVATIONS` y `FAILED`; eliminar el `PASSED` constante. | Runner: `main` imprime siempre `PASSED` | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Identificar, deduplicar y ejecutar secuencialmente trabajos por página/generación hasta que termine el render. | `loadPages`, `_loadingTask`, `_renderPage` | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Conservar/cancelar `RenderTask` y bloquear commits de generaciones obsoletas. | `_cleanPage`, `_renderPage`, `setZoom`, `rotate` | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Mantener viewport acotado con observer y fallback coalescido; agrupar scroll/zoom por frame. | `_setScrollListener`, `_visiblePages` | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Preservar API, DOM, estado visual y aislamiento de fallos por página. | `script.js`, callbacks y navegación del visor | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Validar con dobles, política, sintaxis y E2E autorizada; documentar bloqueos reales. | `tools/e2e`, AGENTS.md y runbook | D-07 | RQ-07 | Origen: D-07, RQ-07 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Evidencia con errores clasificados y saneados. | WHEN ocurre cada clase de error THEN solo se persisten campos permitidos y origen estable. | Una categoría desconocida falla de forma segura; no conserva texto crudo. |
| RQ-02 | Estado final coherente con errores e hitos. | WHEN termina una medición THEN nunca se informa `PASSED` si existen errores u observaciones. | Mantener códigos de salida consumibles por automatización. |
| RQ-03 | Máximo un render efectivo por página/generación. | WHEN hay duplicados o errores THEN la cola deduplica, espera y continúa sin quedar bloqueada. | Preservar render diferido y orden útil. |
| RQ-04 | Trabajo obsoleto cancelado o ignorado. | WHEN cambia zoom/rotación o sale del viewport THEN no se instala un canvas de generación anterior. | `RenderingCancelledException` no cuenta como fallo funcional esperado. |
| RQ-05 | Scroll/zoom con trabajo acotado. | WHEN llegan eventos rápidos THEN se ejecuta como máximo una reconciliación por frame y solo páginas visibles/adyacentes. | Fallback compatible si no existe `IntersectionObserver`. |
| RQ-06 | Experiencia y contratos conservados. | WHEN se navega, amplía, rota o falla una página THEN se conservan página activa, posición, overlays y continuidad de otras páginas. | No cambiar DOM, firma, impresión ni persistencia. |
| RQ-07 | Evidencia reproducible y honesta. | WHEN se valida DOC-100 THEN pasan pruebas focales/política/sintaxis y la E2E se ejecuta solo con autorización o queda bloqueada explícitamente. | No inventar métricas ni afirmar corrección real sin E2E. |

## Reglas de trazabilidad obligatorias

1. Cada decisión `D-XX` está desarrollada en `design.md`, reflejada en `spec.md` y vinculada a tareas mediante `Origen: D-XX, RQ-XX`.
2. Cada tarea conserva un entregable, área afectada y verificación primaria.
3. No se introducen reglas de frameworks ajenos ni capas backend.
4. La implementación no puede comenzar si `opsxj:refine --sync` o la validación estricta fallan.

## Resultado del refinamiento

- Estado: aprobado para sincronización y auditoría OPSXJ.
- Comando: `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-100 --sync`.
