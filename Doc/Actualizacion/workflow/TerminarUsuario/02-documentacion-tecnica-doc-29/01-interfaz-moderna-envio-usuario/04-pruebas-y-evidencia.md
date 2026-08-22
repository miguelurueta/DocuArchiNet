# Pruebas, evidencia y riesgos

- Ticket: DOC-29
- Cambio OpenSpec: doc-29-interfaz-moderna-enviar-usuario
- Clasificacion: cross_cutting

## Pruebas y evidencia

El 2026-08-21 se ejecutó la batería local CJS con `node --test tests\workflow-user-send.test.cjs tests\workflow-user-send-ui.test.cjs tests\workflow-user-send-confirmation.test.cjs tests\confirmation-dialog.test.cjs tests\workflow-group-send.test.cjs tests\workflow-transition-ui.test.cjs tests\workflow-transition-confirmation-integration.test.cjs tests\workflow-transition-page-presentation.test.cjs tests\workflow-modern-feature-gate.test.cjs`. Resultado: 66 pruebas correctas, sin red, sesión ni escritura de Workflow.

La compilación local se ejecutó con `msbuild .\GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m /verbosity:minimal /clp:ErrorsOnly` y terminó con código 0. También pasó `openspec.cmd validate doc-29-interfaz-moderna-enviar-usuario --strict` y el refinement sincronizado de OPSXJ.

La cobertura verifica disparador sin gate, ausencia de fallback Web Forms de usuario, preview paginado, debounce, cursores, respuesta obsoleta, selección, confirmación, bloqueo, cancelación, doble clic, foco, teclado, aislamiento respecto a Continuar flujo y actualización parcial. `confirmation-dialog.test.cjs` mantiene pendiente la promesa de ejecución y prueba que no se puede cerrar por X, Cancelar, fondo, Escape, API ni reemplazo de diálogo; también comprueba la confirmación nativa al cerrar o recargar la pestaña y que el cierre vuelve a estar disponible tras la respuesta. Incluye la geometría estable del modal y la clasificación visual del nuevo botón.

Se agregó `test:doc29:user-send-lock` para la validación E2E autenticada. El caso selecciona la tarea activa de la sesión, abre preview y confirmación por la UI, deja pasar el único POST al ASMX y retiene solo su respuesta. Mientras el estado es `enviando`, comprueba controles deshabilitados, fondo, Escape, cierre por API y `beforeunload`; al liberar la respuesta exige que el diálogo pueda cerrarse y que cambien las huellas de estado y auditoría. Es mutante, por lo que no se ejecuta sin tarea descartable ni las autorizaciones `DOC28_E2E_EXECUTION_AUTHORIZED=true` y `DOC29_E2E_UI_LOCK_AUTHORIZED=true`.

## QA/E2E WebForms

El operador entregó la grabación `Grabación 2026-08-21 174246-comportamiento-todos.mp4` como evidencia de QA visual. Se revisaron fotogramas reales distribuidos entre los segundos 0 y 36, incluidos apertura del modal, búsqueda, estado transitorio del listado, selección, recarga y cierre. El disparador se mantiene entre Devolver y Enviar a grupo y el diálogo conserva posición y altura estable aun cuando el listado se actualiza; se acepta la corrección visual.

El preview E2E autenticado de `PreviewEnviarUsuario` se ejecutó en el ambiente local autorizado con `test:doc28:preview`. Las huellas de estado y de auditoría específica de usuario fueron iguales antes y después, por lo que se confirmó su carácter de solo lectura. Con autorización explícita se ejecutó además `test:doc28:execute`: el destino y token se obtuvieron del preview vigente y las huellas posteriores confirmaron los cambios esperados de estado y auditoría. No se ejecutaron carga ni activación de gate.

En dos corridas autorizadas del caso UI DOC-29, cada único POST real respondió exitosamente con estado funcional `completada`; los controles posteriores confirmaron cambio de estado y auditoría propia de envío a usuario, con gate apagado y sin cambios legacy. Ambas tareas quedaron consumidas. La primera corrida reveló una carrera de limpieza al liberar la respuesta interceptada; la segunda confirmó la ruta real hasta el guardia de recarga y reveló que Chromium no permite construir manualmente `BeforeUnloadEvent`. Se corrigieron la sincronización de liberación y la comprobación para despachar el evento estándar compatible.

Como control adicional sin mutación, se abrió la confirmación real en Chromium con una promesa simulada. Durante la promesa, `beforeunload` quedó prevenido y el cierre por API devolvió bloqueo; tras resolverla, el diálogo se cerró normalmente.

La corrida integral final del arnés corregido pasó con una nueva tarea descartable: emitió exactamente un POST, bloqueó confirmar, cancelar, X, fondo, Escape, cierre por API y recarga mientras el ASMX estaba pendiente, y permitió el cierre al liberar la respuesta. El ASMX respondió con éxito y estado `completada`; las huellas de estado y auditoría cambiaron. La evidencia saneada se conserva en `tools/e2e/artifacts/doc29-user-send-ui-lock-e2e.json` y no incluye secretos, destinos, tokens ni cuerpos de respuesta.

Antes de una prueba autenticada se debe leer `tools/e2e/AGENT-RUNBOOK.md`. La corrida debe usar una tarea descartable, conservar el gate en `false` y limitar las consultas de control a `SELECT`. No se almacenan credenciales, cookies ni cadenas de conexión en esta evidencia.

## Riesgos residuales

La evidencia local, la grabación y las transiciones E2E autorizadas demuestran contratos, aislamiento, presentación visual, guardia de cierre en navegador y transición real controlada. El E2E UI integral está aprobado. Carga y concurrencia mutante siguen fuera de esta entrega y requerirán una autorización específica si se solicitan en el futuro.
