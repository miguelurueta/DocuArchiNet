# E2E autorizada — almacenamiento RUP y telemetría SII

- Fecha: 2026-09-11, zona America/Bogota.
- Ambiente: certificación local autorizada, con TLS autofirmado temporal.
- Escenario: `import-sii-execution`, una muestra sobre recurso descartable expresamente reutilizado.
- Resultado: aprobado; consulta, preflight, creación, ejecución, consulta de estado y reconciliación terminaron sin código de error.
- Persistencia: una intención `Completada`, un item con persistencia conocida y un documento nuevo confirmado mediante consultas `SELECT` en el gabinete RUP.
- Índices verificados: tipología 175, código de barras y enlace/radicado concordantes; no se conserva información personal ni el contenido del documento.
- Recurso: listo, reservado y consumido.
- Restauración: gate `false`, usuarios y grupos vacíos; páginas legacy protegidas sin cambios.

## Corrección de trazabilidad y consumo redundante

La consulta previa registra `OperationId`, pero no `IntentId` ni `ClientItemId` porque la intención todavía no existe. Durante la ejecución, token, sello y descarga comparten los cuatro identificadores saneados: intención, item, operación y correlación.

Antes de la corrección, la ejecución resolvía el anexo dos veces y producía token–sello–descarga–token–sello. Después de reutilizar en memoria los metadatos ya validados, la ejecución produjo exactamente token–sello–descarga. La caché es efímera por instancia del cliente y no persiste respuestas, tokens ni URLs.

## Tiempos totales del recorrido

| Operación del endpoint | Muestra | Éxitos | Errores | Duración (ms) |
| --- | ---: | ---: | ---: | ---: |
| Consulta SII | 1 | 1 | 0 | 6988 |
| Preflight | 1 | 1 | 0 | 40 |
| Crear intención | 1 | 1 | 0 | 33 |
| Ejecutar intención | 1 | 1 | 0 | 3066 |
| Obtener intención | 1 | 1 | 0 | 21 |
| Reconciliar | 1 | 1 | 0 | 9 |

## Tiempos atribuibles a SII

Los percentiles usan rango más próximo sobre la muestra observada; no se extrapolan.

| Operación SII | Muestra | Éxitos | Errores | Mínimo | Promedio | p50 | p95 | p99 | Máximo |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `SOLICITAR_TOKEN` | 2 | 2 | 0 | 305 | 442.5 | 305 | 580 | 580 | 580 |
| `CONSULTAR_SELLO` | 2 | 2 | 0 | 826 | 969.5 | 826 | 1113 | 1113 | 1113 |
| `DESCARGAR_ANEXO` | 1 | 1 | 0 | 696 | 696 | 696 | 696 | 696 | 696 |

Una corrida anterior autorizada registró además un timeout real de `CONSULTAR_SELLO`: categoría `TIMEOUT`, reintentable y duración 30008 ms. La telemetría fallida no alteró el recurso, que fue liberado por el runner.

## Validación de contexto de negocio

Después de ampliar la tabla con snapshots opcionales se repitió la E2E sobre el recurso expresamente autorizado:

- Las llamadas de consulta conservaron tarea, código de barras y referencia genérica; radicado, intención e item permanecieron nulos porque aún no existía la intención.
- Las llamadas de ejecución conservaron tarea, radicado, código de barras, referencia genérica, intención, item, operación y correlación.
- `ReferenciaProveedor` fue una clave opaca SII y no una URL externa.
- La ejecución volvió a producir exactamente `SOLICITAR_TOKEN`, `CONSULTAR_SELLO` y `DESCARGAR_ANEXO`, sin segunda consulta del sello.
- El recorrido completo terminó sin códigos de error; el gate fue restaurado a `false` con alcance vacío.

Los valores concretos de negocio y correlación no se copian a esta evidencia saneada.

## Contrato directo de ejecución

Después de incorporar `ProjectExecutionResult`, se repitió la ejecución individual autorizada sobre el mismo tipo de recurso. El adaptador validó la respuesta de `ExecuteImportIntent` antes de consultar `GetImportIntent`: exactamente un item quedó `Disponible`, con persistencia conocida e identificador documental positivo. La consulta y reconciliación posteriores confirmaron el mismo resultado.

Una tentativa previa se detuvo en `QueryItems` con `SESSION_TASK_UNAVAILABLE`, antes de crear intención o almacenar, porque la sesión no tenía la tarea seleccionada. La reserva local fue liberada y el gate restaurado. La repetición autorizada con la cuenta asociada al recurso finalizó correctamente. No se conservan cuerpos ASMX, identificadores concretos ni datos de cuenta.
