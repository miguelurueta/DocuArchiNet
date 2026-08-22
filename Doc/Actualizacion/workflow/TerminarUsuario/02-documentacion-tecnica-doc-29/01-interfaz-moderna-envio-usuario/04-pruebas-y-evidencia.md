# Pruebas, evidencia y riesgos

- Ticket: DOC-29
- Cambio OpenSpec: doc-29-interfaz-moderna-enviar-usuario
- Clasificacion: cross_cutting

## Pruebas y evidencia

El 2026-08-21 se ejecutó la batería local CJS con `node --test tests\workflow-user-send.test.cjs tests\workflow-user-send-ui.test.cjs tests\workflow-user-send-confirmation.test.cjs tests\confirmation-dialog.test.cjs tests\workflow-group-send.test.cjs tests\workflow-transition-ui.test.cjs tests\workflow-transition-confirmation-integration.test.cjs tests\workflow-transition-page-presentation.test.cjs tests\workflow-modern-feature-gate.test.cjs`. Resultado: 66 pruebas correctas, sin red, sesión ni escritura de Workflow.

La compilación local se ejecutó con `msbuild .\GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m /verbosity:minimal /clp:ErrorsOnly` y terminó con código 0. También pasó `openspec.cmd validate doc-29-interfaz-moderna-enviar-usuario --strict` y el refinement sincronizado de OPSXJ.

La cobertura verifica disparador sin gate, ausencia de fallback Web Forms de usuario, preview paginado, debounce, cursores, respuesta obsoleta, selección, confirmación, bloqueo, cancelación, doble clic, foco, teclado, aislamiento respecto a Continuar flujo y actualización parcial. `confirmation-dialog.test.cjs` mantiene pendiente la promesa de ejecución y prueba que no se puede cerrar por X, Cancelar, fondo, Escape, API ni reemplazo de diálogo; también comprueba la confirmación nativa al cerrar o recargar la pestaña y que el cierre vuelve a estar disponible tras la respuesta. Incluye la geometría estable del modal y la clasificación visual del nuevo botón.

## QA/E2E WebForms

El operador entregó la grabación `Grabación 2026-08-21 174246-comportamiento-todos.mp4` como evidencia de QA visual. Se revisaron fotogramas reales distribuidos entre los segundos 0 y 36, incluidos apertura del modal, búsqueda, estado transitorio del listado, selección, recarga y cierre. El disparador se mantiene entre Devolver y Enviar a grupo y el diálogo conserva posición y altura estable aun cuando el listado se actualiza; se acepta la corrección visual. No se ejecutaron E2E autenticados, carga, activación de gate ni transición real.

Antes de una prueba autenticada se debe leer `tools/e2e/AGENT-RUNBOOK.md`. La corrida debe usar una tarea descartable, conservar el gate en `false` y limitar las consultas de control a `SELECT`. No se almacenan credenciales, cookies ni cadenas de conexión en esta evidencia.

## Riesgos residuales

La evidencia local y la grabación demuestran contratos, aislamiento, comportamiento simulado y presentación visual. La ejecución mutante contra un ambiente real sigue fuera de esta entrega y requerirá una autorización específica si se solicita en el futuro.
