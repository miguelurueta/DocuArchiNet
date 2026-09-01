# Contrato cliente

| Operación | Entrada explícita | Resultado usado por UI |
|---|---|---|
| `ListarNotas` | `idTarea`, `cursor`, `tamanoPagina` | `Exito`, `Notas[]` |
| `ContarNotas` | `idTarea` | `Exito`, `Contador` |
| `CrearNota` | `idTarea`, `contenido`, `clientRequestId` | éxito o código funcional |
| `ActualizarNota` | `idTarea`, `idNota`, `contenido`, `version` | éxito o `VersionConflict` |
| `EliminarNota` | `idTarea`, `idNota`, `version` | éxito o `VersionConflict` |

La UI no concatena JSON, no obtiene la tarea desde sesión y no interpreta una respuesta HTTP como autorización funcional. El texto de notas, autor y fecha se construye mediante nodos DOM y `textContent`.
