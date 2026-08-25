# Contratos, endpoints y códigos

- Ticket: DOC-32
- Cambio OpenSpec: `doc-32-backend-actividad-anterior`
- Clasificación: `cross_cutting`

## Endpoints autenticados

| Endpoint | Payload JSON | Respuesta | Efecto |
| --- | --- | --- | --- |
| `PreviewDevolverActividad` | `{ idTarea, termino, cursor, tamanoPagina }` | `PrevisualizacionDevolverActividadDto` | Solo lectura paginada de aristas entrantes autorizadas. |
| `EjecutarDevolverActividad` | `{ idTarea, idConector, tokenVersion }` | `ResultadoDevolverActividadDto` | Devolución revalidada y exclusiva por tarea. |

Los dos métodos ASMX usan sesión habilitada y formato JSON. No son parámetros admitidos actividad destino, usuario, grupo, Ruta, Flujo, tipo de contexto, `Page`, controles WebForms ni datos de conexión. El contexto siempre se recalcula en servidor.

## Preview y datos expuestos

`PrevisualizacionDevolverActividadDto` entrega `IdTarea`, contexto resumido, `TokenVersion`, `TamanoPagina`, `HayMas`, `CursorSiguiente`, `Destinos` y, cuando aplica, un error funcional. Cada destino contiene solo `IdConector`, actividad, destinatario o grupo resumido, tipo de contexto y orden estable.

El término vacío es válido; cuando existe debe tener entre 2 y 80 caracteres. La página predeterminada es 25 y el máximo es 50. Filtro, límite y orden se aplican después de construir el universo autorizado mediante consultas parametrizadas.

## Ejecución y resultado público

`ResultadoDevolverActividadDto` publica `Exito`, `EstadoFinal`, `CodigoBloqueo`, `MensajeFuncional`, `EsReintentable`, `ReferenciaAuditoria`, advertencias y un error funcional cuando la operación no se completa. El token y el conector son referencias de revalidación, no autorización del cliente: dentro del lock se vuelve a leer tarea, permiso, tipo de contexto y destino.

| Grupo de códigos | Códigos públicos relevantes |
| --- | --- |
| Contexto y autorización | `WORKFLOW_RETURN_CONTEXT_INVALID`, `WORKFLOW_RETURN_FORBIDDEN`, `WORKFLOW_RETURN_CONTEXT_INCONSISTENT` |
| Tarea y conector | `WORKFLOW_RETURN_TASK_INVALID`, `WORKFLOW_RETURN_TASK_UNAVAILABLE`, `WORKFLOW_RETURN_CONNECTOR_INVALID`, `WORKFLOW_RETURN_CONNECTOR_UNAVAILABLE` |
| Preview | `WORKFLOW_RETURN_SEARCH_TERM_INVALID`, `WORKFLOW_RETURN_CURSOR_INVALID`, `WORKFLOW_RETURN_NO_DESTINATIONS` |
| Revalidación y concurrencia | `WORKFLOW_RETURN_VERSION_INVALID`, `WORKFLOW_RETURN_VERSION_CONFLICT`, `WORKFLOW_RETURN_IN_PROGRESS` |
| Ejecución controlada | `WORKFLOW_RETURN_REJECTED`, `WORKFLOW_RETURN_UNAVAILABLE` |

Los errores se normalizan sin SQL, excepciones, cookies, credenciales, controles WebForms, sesión ni detalles internos.

## Cursor y aislamiento de contexto

`DevolverActividadCursorCodec` protege el cursor con `MachineKey`. La continuación contiene identidad de tarea, token de versión, usuario, grupo, ruta, tipo de contexto, término normalizado, orden y último conector. Solo es válida para el mismo snapshot de preview; no autoriza ejecución, no se reutiliza en otra tarea y no representa un permiso transferible.
