# Contratos y compatibilidad

- Ticket: DOC-34
- Cambio OpenSpec: `doc-34-verificacion-transversal-devolver-tarea`

## Contrato de devolución

`PreviewDevolverActividad` recibe `{ idTarea, termino, cursor, tamanoPagina }` y devuelve solo destinos autorizados, contexto publicado, token de versión, cursor y códigos funcionales. El preview no recibe Ruta, Flujo, actividad ni usuario del navegador.

`EjecutarDevolverActividad` recibe únicamente `{ idTarea, idConector, tokenVersion }`. El servidor identifica el tipo de contexto a partir de la tarea, relee autorización y snapshot bajo lock, y resuelve de nuevo el conector entrante antes de invocar el adaptador.

## Semántica de `IdConector`

En Ruta identifica `actividades_disponibles_envio.id_actividades_disponibles_envio`; en Flujo identifica el conector de Flujo. El contrato público no mezcla estas semánticas ni acepta un destino saliente de Continuar flujo como devolución. Las pruebas focales verifican ambas consultas con filtro de universo autorizado, orden, cursor y límite.

## Compatibilidad

La inspección y las pruebas confirman que Continuar flujo mantiene su transición por conector, mientras Enviar a usuario, Enviar a grupo y Usuario anterior conservan contratos y rutas separados. `Button_tool_devolver_a_usuario` corresponde a Usuario anterior y no es un fallback de Elegir actividad anterior; el disparador de esta última operación es el botón moderno `workflow-return-activity-trigger`.

No se detectó cambio de endpoint, DTO, esquema ni configuración atribuible a DOC-34.
