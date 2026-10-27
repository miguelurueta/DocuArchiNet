# DOC-110 — Reutilización E2E, tarea documental 8.1

Inspección estática 2026-10-20; no login ni corrida real. Alcance PER-01–03 del perfil.

| Infraestructura existente | Reutilización concreta | Límite verificado |
|---|---|---|
| `tools/e2e/playwright.config.cjs` | tests, Chromium, ejecución serial, reporteros list/JSON, carpeta de resultados | No crear otra configuración ni cambiar workers/timeouts globales para pasar casos. Trace/screenshot de fallo requieren revisión de datos sensibles antes de compartir evidencia. |
| `tools/e2e/tests/support/authenticated-workflow-session.cjs` | `createWorkflowLoginPageSession`: formulario `gestor.aspx`, selectores y opciones de módulo/cuenta parametrizadas, contexto/página preservados | El nombre Workflow no impone por sí solo ese módulo; el helper recibe la opción. El retorno tras postback no prueba autorización ni llegada a Producción. Debe verificarse página/capacidad y atender 2FA si el ambiente lo exige, reutilizando soporte existente. |
| `tools/e2e/scripts/support/interactive-e2e-console.cjs` | Entrada interactiva, confirmaciones y ejecución de hijo | Usar mecanismo existente, no .env o login paralelo. |
| `tools/e2e/scripts/run-radicacion-simple-attachment-interactive.cjs` | Referencia de composición del soporte anterior, perfil no sensible y secretos efímeros | No reutilizar su operación de carga, datos de Radicación o autorizaciones como autorización DOC-110. |
| `tools/e2e/tests/authenticated-workflow-session.test.cjs` y `authenticated-workflow-session-usage.test.cjs` | Regresión del helper si se necesita extenderlo | Son pruebas de soporte; no prueban publicación/persistencia de perfiles. |
| `tools/e2e/tests/doc88-production-document-upload-policy.test.cjs` | Regresión estática del recorrido existente de Producción | No sustituye E2E real ni comprobación de persistencia. |
| `tools/e2e/scripts/adapters/production-document-upload-e2e-adapter.cjs` | Referencia de operación legacy de carga individual | `executePreview/executeExecution` devuelven READY y count=0 sin ejecutar UI/almacenamiento. No usar ese retorno como PASS de PER-01–03. |

## Integración a implementar

La suite prevista sigue en `tools/e2e/tests/doc110-perfil-importacion.spec.cjs`, con comando focal en el package existente. Reutilizar el helper de sesión y comprobar navegación a Producción/capacidad antes del escenario. No crear una tarea Workflow ficticia para acceder a perfiles.

PER-01 verifica borrador/publicación EXPEDIENTE y recarga; PER-02 versión LOTE/historial/usuario restringido; PER-03 reglas persistidas, patrones distintos y errores. Cada escenario contrasta UI, respuesta HTTP y SELECT autorizado sobre tablas del perfil. Datos creados pertenecen a cuentas y ámbito de prueba autorizados; ninguna credencial se registra como fixture.

La autorización para consultas SELECT de esta exploración no autoriza login, publicación ni escritura de E2E. Las tareas 8.2–8.6 siguen abiertas. Si Producción requiere una extensión del soporte, concretarla en ese soporte y verificar sus consumidores; no asumir que el simple postback acredita sesión funcional.
