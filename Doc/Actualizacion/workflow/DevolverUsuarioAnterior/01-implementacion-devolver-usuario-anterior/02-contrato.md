# Contrato de servidor

## Endpoints autenticados

| Endpoint | Solicitud | Respuesta | Efecto |
| --- | --- | --- | --- |
| `PreviewDevolverUsuarioAnterior` | `{ idTarea }` | `PrevisualizacionDevolverUsuarioAnteriorDto` | Solo lectura. |
| `EjecutarDevolverUsuarioAnterior` | `{ idTarea, tokenVersion }` | `ResultadoDevolverUsuarioAnteriorDto` | Una transición protegida. |

Ambos métodos ASMX usan sesión. El preview recibe exclusivamente el identificador de tarea y devuelve, cuando aplica, contexto mínimo, actividad histórica, usuario histórico resumido y token opaco. No publica grupos, conectores, otras actividades, `IdEstado` ni datos de respuestas.

La ejecución recibe solamente la tarea y el token. Relee todo el destino dentro del lock; no acepta ni deriva autorización desde el payload. Los tipos concretos son `PrevisualizacionDevolverUsuarioAnteriorDto`, `ResultadoDevolverUsuarioAnteriorDto` y sus errores exclusivos.

## Códigos públicos previstos

| Código | Situación |
| --- | --- |
| `WORKFLOW_RETURN_USER_CONTEXT_INVALID` | Sesión o contexto no válidos. |
| `WORKFLOW_RETURN_USER_FORBIDDEN` | Permiso específico ausente o retirado. |
| `WORKFLOW_RETURN_USER_TASK_UNAVAILABLE` | Tarea no activa o no accesible. |
| `WORKFLOW_RETURN_USER_HISTORY_UNAVAILABLE` | No existe antecedente inmediato. |
| `WORKFLOW_RETURN_USER_HISTORY_GROUP` | El antecedente no corresponde a un usuario. |
| `WORKFLOW_RETURN_USER_DESTINATION_UNAVAILABLE` | Usuario, actividad o Ruta/Flujo histórico no es elegible. |
| `WORKFLOW_RETURN_USER_SELF` | El usuario histórico es el autenticado. |
| `WORKFLOW_RETURN_USER_VERSION_INVALID` | Token malformado o vencido. |
| `WORKFLOW_RETURN_USER_VERSION_CONFLICT` | El snapshot o antecedente cambió. |
| `WORKFLOW_RETURN_USER_IN_PROGRESS` | Ya hay una ejecución para la tarea. |
| `WORKFLOW_RETURN_USER_REJECTED` | El motor rechazó la transición. |
| `WORKFLOW_RETURN_USER_UNAVAILABLE` | Falla transitoria normalizada. |

Los mensajes asociados son funcionales y no incluyen SQL, controles Web Forms, sesión, credenciales, token completo ni detalles de excepción.
