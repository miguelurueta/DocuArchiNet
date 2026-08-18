# CONFIRMACION-ESPECIALIZADA

- Ticket: DOC-13
- Cambio OpenSpec: doc-13-confirmacion-especializada
- Clasificacion: cross_cutting (Transversal)
## Servicios y reglas

DOC-13 consume los servicios ya disponibles; no añade una regla de negocio ni amplía el backend para campos de presentación. `PreviewEnviarTarea` entrega la selección y `EjecutarEnvioTarea` vuelve a validar el contexto de sesión, el gate, el destino y la versión antes de efectuar la transición.

`WebServiceWorkflowModern.asmx.vb` conserva la autoridad de Application. `ClassListandoTareas.vb` y el flujo Web Forms existente siguen siendo responsables de la asignación y de las adaptaciones legacy. El cliente no decide autorización ni elimina una tarea por su cuenta: solo refleja el resultado correlacionado del servidor.

El adaptador normaliza la respuesta JSON. Mensajes funcionales se muestran como resultado controlado; fallas de red, respuesta inválida o errores tardíos se sustituyen por un mensaje seguro sin exponer texto técnico, HTML ni datos internos. Solo un éxito confirmado cierra el diálogo y limpia el contexto visual.
