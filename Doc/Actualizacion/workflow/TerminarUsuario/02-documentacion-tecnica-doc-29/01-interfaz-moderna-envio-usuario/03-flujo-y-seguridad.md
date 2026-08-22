# Flujo, seguridad y compatibilidad

- Ticket: DOC-29
- Cambio OpenSpec: doc-29-interfaz-moderna-enviar-usuario
- Clasificacion: cross_cutting

## Flujo de interfaz

La capa de interfaz se compone de `workflow-user-send-ui.js`, `workflow-user-send-confirmation.js` y `workflow-transition-page-presentation.js`. El primero consume la página actual de destinos y protege su estado con contador monotónico, `AbortController` cuando existe, historial de cursores e invalidación de una selección previa. El segundo coordina la confirmación y bloquea doble clic; el tercero elimina una única fila, limpia visor/contexto y actualiza una sola vez el contador cuando el resultado corresponde al token seleccionado.

`WebServiceWorkflowModern` y `ServicioEnvioUsuarioTarea`, entregados por DOC-28, mantienen las reglas de servidor: permiso efectivo, tarea activa, token de versión, usuario–actividad disponible, requisitos y lock. JavaScript no calcula autorización, no accede a SQL y no llama el motor legacy. Los errores de red, bloqueo, token vencido y destino inválido se presentan como resultados funcionales sin exponer detalles técnicos.

El aislamiento es obligatorio: `WorkflowUserSendUi` y los eventos `workflow:user-destination-selected` e `workflow:user-destination-invalidated` no comparten listeners, selectores, estado ni payload con `WorkflowTransitionUi`. El único componente común es el diálogo genérico de confirmación, invocado con una configuración propia de usuario.

Mientras `EjecutarEnvioUsuario` está pendiente, el diálogo de confirmación deshabilita confirmar, Cancelar y X. Los intentos de cierre por fondo, Escape, API o apertura de otra confirmación se conservan en el mismo diálogo y anuncian que debe esperarse la respuesta. Al recibir éxito, bloqueo o error controlado se libera el estado correspondiente. Si se intenta cerrar o recargar la pestaña, el navegador muestra su confirmación nativa; JavaScript no puede impedir ese cierre de manera absoluta.

La capa visual asigna `ctw-action-slot--terminal`, `ctw-action-slot--handoff` y `ctw-action-slot--handoff-user` al nuevo disparador. Esa adaptación conserva el orden de acciones existente sin reintroducir el `onclick` legacy.

## Seguridad y aislamiento

La interfaz transmite únicamente intención al backend DOC-28. El servidor vuelve a verificar autorización, tarea, token, usuario–actividad, requisitos y concurrencia antes de la transición. Preview es una lectura; una cancelación, cierre o búsqueda nueva no modifica tarea, estado ni auditoría.

No se activa ni se consulta el gate para este comando. La reversión es el revert del cambio versionado, sin migraciones ni modificaciones de ambiente; las transiciones confirmadas por el servidor no se revierten desde el navegador.
