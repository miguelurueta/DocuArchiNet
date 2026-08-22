# Arquitectura y componentes

- Ticket: DOC-29
- Cambio OpenSpec: doc-29-interfaz-moderna-enviar-usuario
- Clasificacion: cross_cutting

## Arquitectura de la solución

DOC-29 sustituye únicamente la entrada Web Forms de **Enviar a usuario** por una interfaz moderna oficial, accesible y aislada. La pantalla consulta y ejecuta los endpoints directos que entregó DOC-28; el navegador expresa intención y el servidor conserva autorización, token de concurrencia, validación del usuario–actividad y la única transición mutante.

El nuevo disparador es `workflow-user-send-trigger`. Abre una búsqueda paginada, solicita confirmación con el contrato directo y actualiza de manera correlacionada solo la tarea resuelta, su visor y el contador. No habilita una ruta alternativa ni traslada reglas de negocio al cliente.

## Alcance y compatibilidad

Los cambios se concentran en `workflow/Webworkflow.aspx`, su code-behind, el adaptador `js/workflow/workflow-user-send-ui.js`, la confirmación exclusiva de usuario y los estilos del modal. La ruta heredada de usuario —`ImageButtonEnviarUsuario`, su modal, campos ocultos y `After_envio_usuario_workflow`— fue retirada de esta página.

Grupo y Continuar flujo mantienen sus endpoints, `IdConector`, selectores, estado y gate. Enviar a usuario no consulta ni modifica `WorkflowCentroTrabajoModernActive`; se registra antes de la rama de Grupo/Continuar flujo para todo contexto Workflow válido. La reversión consiste en revertir el paquete versionado; no requiere migraciones ni cambios de configuración.

## Impacto de interfaz

`workflow/Webworkflow.aspx` presenta `workflow-user-send-trigger` y el modal `workflow-user-send-modern-modal`. El adaptador `workflow-user-send-ui.js` mantiene búsqueda, cursor, cancelación y obsolescencia; `workflow-user-send-confirmation.js` compone la confirmación exclusiva, y `workflow-transition-page-presentation.js` actualiza la fila, visor y contador correlacionados.

El diálogo conserva una altura de 42 rem —limitada por el alto disponible— y reserva el desplazamiento interno. Durante la búsqueda cambia el contenido, no la geometría del modal. La capa visual clasifica el nuevo trigger como transferencia para conservar su posición junto a Devolver y Enviar a grupo.

El detalle operativo y los diagramas canónicos están en `Doc/Actualizacion/workflow/TerminarUsuario/01-implementacion-envio-usuario/`.

## Estado de cierre

La implementación, la validación OpenSpec estricta, las pruebas CJS y la compilación local se ejecutaron el 2026-08-21. La aceptación visual posterior al ajuste de geometría del modal fue revisada mediante la grabación de QA. El preview E2E autenticado preservó estado y auditoría, y la corrida UI integral autorizada confirmó un único envío, bloqueo de cierre/recarga mientras el ASMX respondía y cambios esperados de estado y auditoría. Gate, carga y concurrencia mutante permanecieron fuera de la corrida.
