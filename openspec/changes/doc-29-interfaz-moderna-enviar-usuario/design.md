<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05 -->
## Contexto

DOC-29 moderniza exclusivamente **Enviar a usuario** de `workflow/Webworkflow.aspx`. DOC-28 entrega el backend de destino directo usuario–actividad: el navegador expresa intención y el servidor conserva autorización, token, concurrencia, requisitos y auditoría.

La entrada actual usa `ImageButtonEnviarUsuario`, campos ocultos, un modal Web Forms y `After_envio_usuario_workflow`. Esa cadena admite reasignación de respuesta y no cumple el contrato moderno. Este cambio no modifica endpoints, esquema, motor legacy, Grupo ni Continuar flujo.

## Decisiones

### D-01 — Entrada oficial sin gate ni postback

`Panel_EnviarUsuario` expondrá `workflow-user-send-trigger`, un `button` sin atributo de gate. `Webworkflow.aspx.vb` registrará estilo, adaptador, confirmación y bootstrap propios antes de la rama que usa `WorkflowModernPresentationBootstrap` para Grupo y Continuar flujo.

Se retirarán de esta página el enlace a `ImageButtonEnviarUsuario`, su handler y los controles/handlers exclusivamente de envío legacy de usuario. Los controles que sirven otro recorrido no se cambian sin evidencia de uso exclusivo. No quedará enlace, postback ni modal Web Forms habilitado para este comando.

### D-02 — Cliente paginado sobre contratos DOC-28

`workflow-user-send-ui.js` hará POST `same-origin` a `PreviewEnviarUsuario` con `{ idTarea, consulta, cursor, tamanoPagina }`. Mantendrá historial de cursores por búsqueda, aplicará debounce y contador monotónico, y descartará respuestas tardías. El cliente renderizará únicamente la página devuelta.

La selección contiene `{ idTarea, idUsuarioWorkflowDestino, idActividadDestino, tokenVersion, contexto, destino }`. La confirmación enviará los cuatro campos de ejecución a `EjecutarEnvioUsuario`. No habrá `IdConector`, SQL, motores, controles ocultos, `Cambia_Estado` ni autorización JavaScript.

### D-03 — Aislamiento y accesibilidad

Modal, IDs, evento `workflow:user-destination-selected`, evento de invalidación, contador de solicitudes y objeto global serán propios. No se usan `workflow:destination-selected`, `WorkflowTransitionUi`, sus selectores ni payload de conector.

Se reutilizan `ConfirmationDialog`, estilos y el patrón de foco: apertura al cierre, trampa Tab/Shift+Tab, Escape, cancelación, clic de fondo, recuperación de foco y prevención de doble ejecución. La confirmación muestra solo JSON autorizado.

Durante la ejecución pendiente, `ConfirmationDialog` deshabilita confirmar, cancelar y X. Sus rutas de cierre por fondo, Escape, API o intento de reemplazo conservan el diálogo y anuncian que debe esperarse la respuesta; `beforeunload` solicita la confirmación nativa del navegador. Una respuesta controlada libera el bloqueo conforme a su resultado. El navegador no permite impedir de forma absoluta el cierre de una pestaña, por lo que la salvaguarda de esa ruta es la advertencia nativa.

### D-04 — Presentación parcial correlacionada

La confirmación de usuario invocará la presentación moderna con un mensaje de éxito exclusivo. La presentación quitará solo la fila de la tarea resultante, actualizará una vez el contador, limpiará visor/contexto y asociará el temporizador a cada mensaje para no compartir estado con Grupo ni Continuar flujo.

### D-05 — Evidencia sin operación no autorizada

Las pruebas CJS simularán ASMX y cubrirán contratos, ausencia de `IdConector` y rutas legacy, cursores, obsolescencia, errores, accesibilidad, confirmación y presentación. La validación final incluye MSBuild. E2E autenticado, carga, gate y transición real quedan fuera hasta recibir autorización explícita.

## Secuencia de implementación

1. Sustituir el disparador legacy y registrar bootstrap independiente.
2. Añadir modal, adaptador paginado y confirmación exclusiva.
3. Integrar presentación parcial y mensaje correlacionado.
4. Añadir pruebas, compilar y actualizar el paquete técnico único de `TerminarUsuario`.

## Compatibilidad y reversión

Grupo y Continuar flujo conservan sus archivos, endpoints, selectores, eventos, payload y gate. No hay cambios de datos ni configuración. La reversión es revertir el cambio versionado completo; el contrato DOC-28 y los datos Workflow no se alteran.
