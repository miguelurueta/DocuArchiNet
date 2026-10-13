# Pruebas y evidencia

Fecha local: 2026-10-13.

| Verificación | Resultado |
| --- | --- |
| `node --check` sobre visor, runner y dos pruebas | Aprobado, 4/4 |
| `npm.cmd --prefix tools/e2e run test:workflow:pdf-viewer-performance:policy` | Aprobado, 14/14 después del ajuste |
| `openspec.cmd validate doc-100-planificador-viewport --strict` | Aprobado |
| `git diff --check` | Aprobado; solo avisos de normalización LF/CRLF |
| E2E autorizada, corrida 1, tres iteraciones | Hitos completos; estado final `FAILED` coherente |
| E2E autorizada, corrida 2, tres iteraciones | Hitos completos; interacción mejorada; `FAILED` por DOC-101 |

Las pruebas cubren clasificación y sanitización, tres estados finales, cola vacía, duplicados, secuencia, fallos de carga/render, cancelación, reentrada al viewport, commit tardío, observer, fallback, coalescencia, scroll proporcional, rotación y contratos públicos.

## E2E corrida 1

La autorización explícita para ambiente, cuenta y tarea/documento descartable fue recibida el 2026-10-13. La cuenta y contraseña se ingresaron directamente en la consola TTY visible y no se conservaron. La evidencia versión 2 quedó saneada y no contiene URL, tarea, documento ni credenciales.

| Métrica | Resultado |
| --- | ---: |
| Primera carga / mediana / mínimo | 4.204 / 893 / 860 ms |
| Scroll total / bloqueo | 3.398 / 3.321 ms |
| Zoom total / bloqueo | 948 / 635 ms |
| Errores | `WORKFLOW_PDF_PAGE_ERROR`, `WORKFLOW_PDF_SCRIPT_ERROR` |
| Resultado | `FAILED` |

El error del visor se asocia a `pdf.worker.entry.min.js`, cargado como script normal junto con otros artefactos PDF.js en `index.html`; su corrección pertenece a DOC-101. Tras observar la regresión de zoom, DOC-100 fue ajustado para redibujar primero solo páginas realmente visibles.

## E2E corrida 2 — evidencia final

| Métrica | Línea base | Resultado | Variación |
| --- | ---: | ---: | ---: |
| Primera carga | 2.902 ms | 2.569 ms | -11,5 % |
| Mediana primer canvas | 878 ms | 988 ms | +12,5 % |
| Scroll total | 3.902 ms | 3.065 ms | -21,5 % |
| Bloqueo scroll | 3.548 ms | 2.773 ms | -21,8 % |
| Zoom total | 779 ms | 447 ms | -42,6 % |
| Bloqueo zoom | 435 ms | 104 ms | -76,1 % |

La interacción cumple el objetivo de mejora material. El resultado global permanece `FAILED`, de forma intencional y trazable, mientras DOC-101 no retire la carga incorrecta de `pdf.worker.entry.min.js`.

Riesgo residual: la mediana de primera página empeoró 12,5 % y el error de dependencia seguirá produciendo `FAILED` hasta DOC-101, aunque los hitos visuales terminen.
