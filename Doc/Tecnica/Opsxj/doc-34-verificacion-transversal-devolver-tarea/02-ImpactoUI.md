# VERIFICACION-TRANSVERSAL-DEVOLVER-TAREA

- Ticket: DOC-34
- Cambio OpenSpec: doc-34-verificacion-transversal-devolver-tarea
- Clasificacion: cross_cutting (Transversal)
## Superficies UI

Se verifican `workflow-return-activity-trigger`, el modal `workflow-return-activity-modern-*`, `workflow-return-activity-ui.js` y `workflow-return-activity-confirmation.js`. Las pruebas CJS cubren búsqueda, páginas, selección, foco, Escape, bloqueo de confirmación, cancelación, cierre, responsive y regiones accesibles.

El código UI no consulta `WorkflowCentroTrabajoModernActive`. La operación no tiene postback, handler ni fallback Web Forms; `Button_tool_devolver_a_usuario` permanece exclusivamente para Usuario anterior.

## Validacion visual

La QA no autenticada abrió la página local en escritorio y móvil. Confirmó el shell accesible del modal, viewport responsive y carga de los scripts. Sin tarea seleccionada el disparador no se publica y no se invocó preview; las interacciones dinámicas se validan con CJS y evidencia E2E previa saneada. El dictamen técnico queda apto para solicitar fase 04, sin autorizar ambiente.
