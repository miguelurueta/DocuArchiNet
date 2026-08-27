# Pruebas y evidencia — Devolver a usuario anterior

- Ticket: DOC-37
- Cambio OpenSpec: doc-37-interfaz-moderna-devolver-usuario-anterior
- Clasificacion: cross_cutting

## Evidencia requerida

La evidencia local ejecutada fue `node --test tests/workflow-return-user-previous-ui.test.cjs tests/workflow-return-user-previous.test.cjs tests/workflow-return-activity.test.cjs`, con 29 pruebas aprobadas. Cubre bootstrap sin feature gate, trigger exclusivo, ausencia de postback, payload mínimo, preview, token opaco, confirmación, bloqueo de doble clic, foco, Escape, timeout, cancelación y actualización localizada; también incluye regresiones de DOC-36 y devolución de actividad. `msbuild GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m:1 /v:minimal` completó correctamente con advertencias históricas de referencias y VB.NET, sin errores de compilación.

La ampliación E2E se comprobó de forma local con 21 pruebas del orquestador y 4 políticas DOC-37 aprobadas. Cubren perfil no sensible, derivación DOC-37 desde DOC-36, dos tareas aisladas, autorizaciones por etapa, limpieza de secretos efímeros, endpoints exclusivos, payload de ejecución mínimo, `SELECT` de control, gate apagado, selección oficial de tarea y bloqueo de una única mutación.

## QA/E2E WebForms

El 2026-08-27, con autorización expresa de ambiente, cuenta y dos tareas descartables distintas, se ejecutaron en GESTOR las tres etapas DOC-37 por separado: `preview` (22.2 s), `execution` (25.5 s) y `ui-lock` (25.7 s). El preview conservó las huellas de estado y auditoría. Las etapas mutantes confirmaron una transición real con las huellas esperadas; `ui-lock` verificó una sola solicitud mientras confirmación, cierre, Escape, backdrop y abandono permanecían bloqueados hasta recibir la respuesta.

El perfil `doc37` se derivó desde el perfil no sensible DOC-36 y cada invocación solicitó las credenciales exclusivamente en TTY. La prueba seleccionó la tarea autorizada mediante el comando oficial de la bandeja antes de las huellas de control, sin simular campos ocultos o sesión. El runner rechazó etapas no autorizadas, reservó cada recurso mutante y comprobó al cierre que el gate permanecía apagado y que las páginas legacy no tenían cambios. La evidencia saneada de cada etapa conserva únicamente banderas, latencias y huellas; no se guardaron secretos, datos de respuesta, tokens, usuarios, actividades ni destinos.
