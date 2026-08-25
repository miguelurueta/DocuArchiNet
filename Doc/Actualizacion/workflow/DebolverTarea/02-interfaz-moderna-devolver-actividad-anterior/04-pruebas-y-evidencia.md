# Pruebas, evidencia y límites

## Cobertura local

Las pruebas CJS cubren el contrato de preview, búsqueda, debounce, cursor, cancelación, descarte de respuesta obsoleta, accesibilidad declarativa, aislamiento, confirmación, payload mínimo, bloqueo, doble envío, éxito correlacionado y retiro del postback legado.

La regresión vecina incluye los módulos de grupo, usuario, transición, presentación y feature gate. La batería local ejecutada para DOC-33 completó 83 pruebas sin fallos. La compilación MSBuild del proyecto Web Forms es una verificación separada de la integración de markup, code-behind y diseñador.

| Evidencia | Alcance | Resultado esperado |
| --- | --- | --- |
| `tests/workflow-return-activity-ui.test.cjs` | Preview y modal exclusivo | Contrato mínimo, estado aislado y accesibilidad. |
| `tests/workflow-return-activity-confirmation.test.cjs` | Confirmación y ejecución | Terna mínima, bloqueo compartido y presentación puntual. |
| `tests/workflow-return-activity.test.cjs` | Contrato DOC-32 y relevo DOC-33 | Sin regreso a postback de actividad anterior. |
| Suite moderna vecina | No regresión | Usuario, grupo, transición y gate permanecen compatibles. |
| MSBuild | Compilación Web Forms | Markup, diseñador y code-behind coherentes. |

## E2E UI autorizada y límite de reutilización

`tools/e2e/tests/doc33-return-activity-ui.spec.cjs` completó una corrida autenticada autorizada con tres etapas: preview UI no mutante, devolución UI y bloqueo UI con una tarea independiente. La evidencia saneada confirmó que el preview no alteró estado ni auditoría; las dos etapas mutantes verificaron una sola transición, cambios de estado/auditoría y la actividad final configurada por ODBC de solo lectura.

La etapa de bloqueo envía una sola devolución real y retiene la respuesta en el borde Playwright después de que el servidor atendió el POST. Mientras el navegador conserva el resultado pendiente, verifica que no se pueda confirmar por segunda vez, cancelar, cerrar por X o fondo, usar Escape, cerrar el modal de devolución ni abandonar con `beforeunload`. Al liberar la respuesta exige exactamente una transición, huellas ODBC modificadas y la actividad final configurada.

Cada corrida posterior debe seguir `tools/e2e/AGENT-RUNBOOK.md`, usar tareas descartables seleccionadas por la cuenta aprobada, no mostrar ni guardar secretos, y dejar `WorkflowCentroTrabajoModernActive` en `false` con usuarios y grupos vacíos al cierre. La evidencia solo guarda conteos, códigos, latencias, banderas y huellas.
