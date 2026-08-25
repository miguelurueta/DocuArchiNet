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

## Límite E2E

No se ejecuta una E2E autenticada ni se activa un gate como parte de esta verificación local. Si se autoriza una corrida posterior, debe seguir el runbook del repositorio, usar una tarea descartable y cuentas aprobadas, no mostrar ni guardar secretos, y dejar `WorkflowCentroTrabajoModernActive` en `false` con usuarios y grupos vacíos al cierre. La E2E debe comprobar que preview no cambia estado/auditoría y que la ejecución solo afecta la tarea elegida.
