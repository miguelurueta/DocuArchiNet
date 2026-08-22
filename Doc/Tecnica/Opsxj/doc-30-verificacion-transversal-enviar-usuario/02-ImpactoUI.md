# Impacto UI — Verificación transversal de Enviar a usuario

- Ticket: DOC-30
- Cambio OpenSpec: doc-30-verificacion-transversal-enviar-usuario
- Clasificacion: cross_cutting

## Superficies UI

La revisión cubre el disparador moderno `workflow-user-send-trigger`, el modal `workflow-user-send-modern-modal`, la lista paginada y la confirmación. Los adaptadores `workflow-user-send-ui.js` y `workflow-user-send-confirmation.js` permanecen aislados de `WorkflowTransitionUi`; el envío a usuario no publica `IdConector` ni una ruta Web Forms alternativa.

## Validacion visual

La evidencia QA visual no autenticada revisada cubre apertura, búsqueda, estado transitorio, selección, recarga y cierre. La geometría del diálogo se mantiene estable y el recorrido de pruebas CJS cubre foco, teclado, Escape, bloqueo durante la ejecución, representación responsive y actualización correlacionada. El detalle está en el paquete canónico DOC-30.
