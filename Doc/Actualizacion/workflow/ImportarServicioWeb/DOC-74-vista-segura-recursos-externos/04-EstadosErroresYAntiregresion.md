# Estados, errores y antirregresión

| Estado | Origen | Acción disponible |
| --- | --- | --- |
| `preparando` | Solicitud activa | Esperar |
| `disponible` | Descriptor y MIME visualizable | Ver contenido |
| `formato-no-visualizable` | MIME permitido sin visor embebido | Descargar temporalmente |
| `recurso-vencido` | Error de expiración | Solicitar recurso nuevo |
| `proveedor-no-disponible` | Error externo seguro | Reintentar explícitamente |
| `no-autorizado` | Contexto o permiso rechazado | Ninguna |
| `bloqueado` | Gate/B10 no disponible | Ninguna |

## Antirregresión

- Foco, resize y rerender no invocan `GetPreview`.
- Cerrar invalida la generación activa y remueve el `src` del frame.
- Respuestas tardías de una apertura cerrada se ignoran.
- No se reutiliza descriptor vencido.
- `ClassAlmacenamiento`, `AlmacenaDocumentoTareaWorkflow(...)` y visores no cambian.
- El gate continúa apagado; no se realizaron E2E ni llamadas autenticadas reales.

## Rollback

Retirar los dos scripts, su registro, el markup/CSS aditivo y cambios del adaptador. Backend, handler, almacenamiento y flujo legacy permanecen intactos.
