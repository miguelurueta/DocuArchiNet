# Contratos y mapping

No existe upload nuevo en DOC-100. El nombre se conserva por el formato documental requerido.

## Contrato del planificador

| Campo | Tipo | Regla |
| --- | --- | --- |
| `id` | string | `pageNumber:generation` |
| `pageNumber` | entero | Entre 1 y `pdf.numPages` |
| `generation` | entero | Generación vigente de zoom/rotación |
| `state` | string | `queued`, `rendering`, `rendered` o `cancelled` |

Entradas inválidas se descartan; una identidad ya en cola, renderizando o renderizada no se duplica.

## Contrato de evidencia

Cada fallo contiene exclusivamente `code`, `source`, `type`, `fileBase` y `line`. No se mapean mensajes, stacks, URL, query, ruta, tarea, usuario, documento, cookie ni contenido. Los orígenes son `main_frame`, `viewer`, `worker` y `optional_resource`.

No cambian DTO, esquema, endpoint, autenticación ni payload del producto.
