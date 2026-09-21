# Estados, transiciones y concurrencia

## Tabla de transiciones

| Estado origen | Destinos permitidos | Entrada implementada |
| --- | --- | --- |
| `cerrado` | `resolviendo-proveedor` | `open(providerId)` |
| `resolviendo-proveedor` | `consultando`, `error`, `cerrado` | resolución del registro |
| `consultando` | `vacio`, `resultados`, `error`, `cerrado` | `adapter.queryItems` |
| `vacio` | `consultando`, `cerrado` | reconsulta programática o cierre |
| `resultados` | `preparando`, `consultando`, `cerrado` | `execute`, reconsulta o cierre |
| `preparando` | `ejecutando`, `error`, `resultados`, `cerrado` | transición síncrona previa a llamada |
| `ejecutando` | `reconciliando`, `completado`, `error` | promesa compartida |
| `reconciliando` | `completado`, `error` | estado modelado, no alcanzado por UI actual |
| `completado` | `consultando`, `cerrado` | reconsulta o cierre |
| `error` | `resolviendo-proveedor`, `consultando`, `cerrado` | reintento programático o cierre |

Una transición no incluida lanza `IMPORT_STATE_TRANSITION_INVALID:{origen}:{destino}`. Los listeners reciben un snapshot inmediato al suscribirse y luego uno por transición.

## Ejecución única

`execute(request)` guarda la primera promesa en `execution`. Llamadas posteriores, incluso antes de resolver, reciben el mismo objeto y no vuelven a invocar el adaptador. `close()` limpia `execution` y contexto. Esto evita duplicación dentro de una instancia core; no sustituye idempotencia/concurrencia del backend entre pestañas o procesos.

## Diferencia entre modelo y recorrido visible

Los estados `preparando`, `ejecutando`, `reconciliando` y `completado` existen y se renderizan, pero el modal actual no contiene botón de selección/importación. Solo pruebas o consumidores programáticos pueden llamar `core.execute`. No se declara recorrido E2E de importación completo para DOC-72.
