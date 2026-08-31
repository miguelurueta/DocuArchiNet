# Notas Workflow: lectura moderna

DOC-41 habilita únicamente la lectura moderna de notas asociadas a una tarea Workflow autorizada. El límite público es `webservice/WebServiceWorkflowNotesModern.asmx`; las páginas Web Forms, los endpoints legacy y las operaciones de escritura no forman parte de este cambio.

## Operaciones

- `ListarNotas(idTarea, cursor, tamanoPagina)`: lista metadatos de notas operativas con orden estable y paginación por cursor.
- `ConsultarNota(idTarea, idNota)`: obtiene el contenido solo cuando la nota pertenece a la tarea autorizada.
- `ContarNotas(idTarea)`: devuelve el número de notas con la misma visibilidad del listado.

El ASMX resuelve la sesión con `WorkflowPreviewSessionContextGate.AsegurarContextoNotas`. La tarea se valida con el repositorio moderno de tareas y nunca con `ID_TAREA_SELECCIONDA`.

## Límites y compatibilidad

- El listado usa 25 elementos de forma predeterminada y admite como máximo 50.
- El orden fijo es `FECHA_ANOTACION DESC, ID_ANOTACION DESC`; esta fase no ofrece un orden alternativo.
- El histórico moderno permanece deshabilitado: todas las lecturas filtran `ANOTACION_TAREA.ESTADO_TAREA = 1`.
- Ninguna operación escribe tareas, notas, auditoría ni cambia `WorkflowCentroTrabajoModernActive`.
- El consumidor no debe sondear el contador a un intervalo inferior a 30 segundos. La actualización por eventos pertenece a una fase posterior.

Consulte [contrato-lectura.md](contrato-lectura.md) para el detalle del transporte y [matriz-pruebas.md](matriz-pruebas.md) para la evidencia reproducible.
