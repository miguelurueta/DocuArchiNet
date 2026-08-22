# Contratos, endpoints y códigos

- Ticket: DOC-29
- Cambio OpenSpec: doc-29-interfaz-moderna-enviar-usuario
- Clasificacion: cross_cutting

## Endpoints y contratos

La búsqueda usa `POST` con `credentials: "same-origin"` hacia `PreviewEnviarUsuario` y el JSON `{ idTarea, consulta, cursor, tamanoPagina }`. La respuesta publica solo la página actual de destinos, contexto mínimo, `TokenVersion`, cursor siguiente y la indicación de más resultados. El cliente no materializa la lista completa ni interpreta el preview como autorización.

La confirmación usa `EjecutarEnvioUsuario` con `{ idTarea, idUsuarioWorkflowDestino, idActividadDestino, tokenVersion }`. El par usuario–actividad identifica el destino de forma directa y el token protege contra cambios de tarea. El contrato no incluye `IdConector`, identidad de usuario, permisos, `Page`, `Session`, SQL ni un control oculto Web Forms.

La integración de éxito comunica únicamente el resultado correlacionado a la presentación de usuario. Cancelar, cerrar, cambiar la búsqueda, recibir una respuesta tardía o un bloqueo invalida la selección y evita una ejecución. Grupo y Continuar flujo conservan sus contratos por conector y no reciben eventos ni payload de esta interfaz.

No hay cambios de esquema, nuevas rutas ASMX ni feature flags. La compatibilidad depende de los endpoints DOC-28 ya validados; si el servidor rechaza permiso, token, destino o requisitos, la interfaz conserva el modal y muestra el mensaje funcional seguro.

## Compatibilidad de integración

La entrada de usuario no usa `ImageButtonEnviarUsuario`, `After_envio_usuario_workflow`, campos ocultos ni handlers de reasignación. Los recursos CSS y JavaScript se registran desde `Webworkflow.aspx.vb` antes de la rama del gate de Grupo/Continuar flujo. Grupo y Continuar flujo continúan con sus contratos, listeners y `IdConector` existentes.
