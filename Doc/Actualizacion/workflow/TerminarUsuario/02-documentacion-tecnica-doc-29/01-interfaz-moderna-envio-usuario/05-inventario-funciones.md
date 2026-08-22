# Inventario de funciones y componentes

- Ticket: DOC-29
- Cambio OpenSpec: doc-29-interfaz-moderna-enviar-usuario
- Clasificacion: cross_cutting

## Componentes implementados

| Componente | Ruta | Responsabilidad |
| --- | --- | --- |
| Disparador y modal | `workflow/Webworkflow.aspx` | Presenta `workflow-user-send-trigger` y la estructura accesible del modal. |
| Registro y bootstrap | `workflow/Webworkflow.aspx.vb` | Registra recursos de usuario antes del gate de otras operaciones. |
| Adaptador paginado | `js/workflow/workflow-user-send-ui.js` | Consulta preview, aplica debounce, cursores, cancelación e invalidación. |
| Confirmación | `js/workflow/workflow-user-send-confirmation.js` | Ejecuta solo la terna usuario–actividad–token y previene doble envío. |
| Presentación parcial | `js/workflow/workflow-transition-page-presentation.js` | Elimina la fila correlacionada y actualiza visor, contador y mensaje propio. |
| Capa visual | `js/workflow/centro-trabajo-visual.js` | Mantiene el botón en el grupo de transferencias. |
| Estilos del modal | `Styles/workflow-transition-modern.css` | Mantiene geometría estable y desplazamiento interno. |

## Componentes reutilizados

| Componente | Uso en DOC-29 | Límite |
| --- | --- | --- |
| `ConfirmationDialog` | Diálogo accesible de confirmación, doble envío y cierre durante ejecución pendiente. | No recibe estado ni payload de Continuar flujo; al cerrar la pestaña solo puede solicitar la confirmación nativa del navegador. |
| `PreviewEnviarUsuario` | Consulta paginada de destino directo. | Solo lectura y datos mínimos autorizados. |
| `EjecutarEnvioUsuario` | Ejecución del destino seleccionado. | El servidor conserva autorización, token, lock y mutación. |
| `WorkflowTransitionUi` | Ninguno. | Se mantiene aislado; no se reutilizan sus eventos ni `IdConector`. |

## Pruebas asociadas

`tests/workflow-user-send-ui.test.cjs`, `tests/workflow-user-send-confirmation.test.cjs`, `tests/confirmation-dialog.test.cjs` y `tests/workflow-transition-page-presentation.test.cjs` cubren el adaptador, confirmación, foco, aislamiento, bloqueo de cierre durante el POST y actualización parcial. Las suites de grupo, transición y gate protegen la compatibilidad con las operaciones existentes.
