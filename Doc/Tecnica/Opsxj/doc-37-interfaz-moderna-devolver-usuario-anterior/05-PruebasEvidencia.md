# Pruebas y evidencia — Devolver a usuario anterior

- Ticket: DOC-37
- Cambio OpenSpec: doc-37-interfaz-moderna-devolver-usuario-anterior
- Clasificación: cross_cutting

## Evidencia requerida

La evidencia local ejecutada fue `node --test tests/workflow-return-user-previous-ui.test.cjs tests/workflow-return-user-previous.test.cjs tests/workflow-return-activity.test.cjs`, con 29 pruebas aprobadas. Cubre bootstrap sin feature gate, trigger exclusivo, ausencia de postback, payload mínimo, preview, token opaco, confirmación, bloqueo de doble clic, foco, Escape, timeout, cancelación y actualización localizada; también incluye regresiones de DOC-36 y devolución de actividad. `msbuild GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m:1 /v:minimal` completó correctamente con advertencias históricas de referencias y VB.NET, sin errores de compilación.

La ampliación E2E se comprobó de forma local con las pruebas del orquestador y las políticas DOC-36/DOC-37: 28 pruebas aprobadas. Cubren perfil no sensible, derivación DOC-37 desde DOC-36, dos tareas aisladas, autorizaciones por etapa, limpieza de secretos efímeros, endpoints exclusivos, payload de ejecución mínimo, `SELECT` de control, gate apagado y bloqueo de una única mutación. No se ejecutó Playwright autenticado, no se creó evidencia de ambiente y no se modificó configuración.

## QA/E2E WebForms

No se ejecutará E2E autenticada, carga ni una transición real durante este cambio sin autorización explícita de ambiente y cuentas de prueba. El patrón DOC-36 puede reutilizarse únicamente después de leer `tools/e2e/AGENT-RUNBOOK.md` y recibir esa autorización; toda consulta de control debe ser `SELECT` y el gate debe quedar apagado al terminar.

Con esa autorización, el perfil `doc37` se deriva desde el perfil no sensible DOC-36 con dos IDs de tareas distintos y se invoca el runner para las etapas aprobadas. El runner solicita credenciales en TTY, bloquea etapas no autorizadas, reserva la tarea mutante antes de ejecutarla y confirma de nuevo el gate al cierre. No se deben colocar secretos en archivos, variables persistentes ni evidencias.
