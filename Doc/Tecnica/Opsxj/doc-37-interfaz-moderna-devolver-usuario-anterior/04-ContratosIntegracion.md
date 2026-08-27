# Contratos e integraciones — Devolver a usuario anterior

- Ticket: DOC-37
- Cambio OpenSpec: doc-37-interfaz-moderna-devolver-usuario-anterior
- Clasificación: cross_cutting

## Contratos e integraciones

El preview se invoca por `POST` a `WebServiceWorkflowModern.asmx/PreviewDevolverUsuarioAnterior` con `{ idTarea }`. La respuesta ASMX aporta `IdTarea`, `Contexto.ActividadActual`, `Contexto.ActividadAnterior`, `Contexto.UsuarioAnterior`, `TokenVersion` y, cuando corresponde, un error funcional. La ejecución se invoca por `POST` a `EjecutarDevolverUsuarioAnterior` con `{ idTarea, tokenVersion }`. Ambas requests usan sesión de mismo origen y timeout controlado de quince segundos. No se envían usuario, actividad, ruta, flujo, conector, grupo, historial ni permisos desde la interfaz.

## Contrato de prueba E2E

`doc37` agrega al orquestador las etapas `preview`, `execution` y `ui-lock`. El runner acepta una sola etapa por invocación para que cada sesión opere una única tarea seleccionada. `preview` requiere autorización de ambiente y verifica que UI muestre el único destino resuelto por servidor sin cambiar las huellas de estado ni auditoría. `execution` y `ui-lock` requieren, además, autorización individual y reservan tareas distintas mediante `DOC37_RESOURCE_CONTRACT`. La etapa de bloqueo retiene la respuesta del endpoint para demostrar que confirmación, cierre, Escape, backdrop y descarga no duplican ni abandonan la única solicitud mutante.

El perfil de ejemplo y el creador `create-doc37-workflow-user-previous-ui-profile.cjs` reciben solo los datos operativos no sensibles ya declarados por DOC-36 más ambiente y los dos IDs de tarea autorizados. Las credenciales se pasan de forma efímera por consola; no se guardan en el perfil, artefactos ni código. La actividad anterior se obtiene del preview en memoria para la validación final de la misma prueba. La evidencia conserva únicamente banderas, latencias y huellas; no registra respuestas, token, usuario, actividad ni destino.
