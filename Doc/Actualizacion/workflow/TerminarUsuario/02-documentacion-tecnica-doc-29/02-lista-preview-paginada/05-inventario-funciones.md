# Inventario de funciones y componentes — Lista preview

- Ticket: DOC-29
- Cambio OpenSpec: doc-29-interfaz-moderna-enviar-usuario
- Clasificacion: cross_cutting

## Componentes implementados

| Componente | Ruta | Responsabilidad |
| --- | --- | --- |
| `solicitarPrevisualizacion` | `js/workflow/workflow-user-send-ui.js` | Envía el contrato paginado al ASMX. |
| `loadPage` | `js/workflow/workflow-user-send-ui.js` | Coordina cursor, secuencia, cancelación y renderizado. |
| `startSearch` | `js/workflow/workflow-user-send-ui.js` | Aplica término mínimo, debounce e invalidación. |
| `renderDestinations` y `renderEmpty` | `js/workflow/workflow-user-send-ui.js` | Representan una página o un estado vacío. |
| Tabla y tarjetas | `workflow/Webworkflow.aspx` | Ofrecen representación equivalente para escritorio y móvil. |
| Estilos del modal | `Styles/workflow-transition-modern.css` | Conservan alto y scroll internos durante la consulta. |

## Componentes reutilizados

| Componente | Uso | Límite |
| --- | --- | --- |
| `PreviewEnviarUsuario` | Fuente de destinos autorizados. | Solo lectura, paginada y sanitizada. |
| `PrevisualizacionEnvioUsuarioDto` | Contrato de página, token y contexto. | No concede autorización de ejecución. |
| `ConfirmationDialog` | Consume la selección vigente. | No participa en la búsqueda ni conserva cursores. |
| `WorkflowTransitionUi` | Ninguno. | Se mantiene aislado del preview de usuario. |

## Pruebas asociadas

Las suites `workflow-user-send-ui.test.cjs` y `workflow-user-send.test.cjs` cubren el cliente y el contrato del backend. Los escenarios de Grupo y Continuar flujo se ejecutan en la batería focal para demostrar que sus recorridos no se mezclan con la lista preview.
