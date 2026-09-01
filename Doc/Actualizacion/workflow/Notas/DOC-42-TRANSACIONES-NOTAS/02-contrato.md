# Contrato de servidor

Todos los métodos reciben `idTarea` explícito y derivan actor, ruta y autorización de la sesión autenticada.

| Operación | Entrada mínima | Resultado |
| --- | --- | --- |
| `CrearNota` | `idTarea`, `contenido`, `clientRequestId` UUID | Nota creada y ETag; reintento devuelve la respuesta original. |
| `ConsultarNota` | `idTarea`, `idNota` | Nota visible y ETag calculado en .NET. |
| `ActualizarNota` | `idTarea`, `idNota`, `contenido`, `version` | Nueva versión o conflicto seguro. |
| `EliminarNota` | `idTarea`, `idNota`, `version` | Borrado físico o conflicto seguro. |

Las respuestas bloqueadas son funcionales y no exponen SQL, contenido actual, tokens, sesión ni detalles de excepción. La identidad, propietario, actividad y estado se validan en servidor.
