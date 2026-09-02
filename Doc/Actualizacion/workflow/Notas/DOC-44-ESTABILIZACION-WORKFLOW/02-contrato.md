# Contrato cliente

| Operación | Payload explícito |
| --- | --- |
| `ListarNotas` | `idTarea`, `cursor`, `tamanoPagina` |
| `ConsultarNota` | `idTarea`, `idNota` |
| `ContarNotas` | `idTarea` |
| `CrearNota` | `idTarea`, `contenido`, `clientRequestId` |
| `ActualizarNota` | `idTarea`, `idNota`, `contenido`, `version` |
| `EliminarNota` | `idTarea`, `idNota`, `version` |

El cliente envía JSON real mediante `JSON.stringify`, credenciales de mismo origen y `X-Requested-With`. No usa `Session("ID_TAREA_SELECCIONDA")`, concatena JSON ni renderiza contenido con HTML. Autorización, propiedad, cursor y conflicto de versión son resultados funcionales del backend.
