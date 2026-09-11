# E2E autorizada — lectura SII

- Fecha: 2026-09-09, zona America/Bogota.
- Ambiente: certificación local autorizada con TLS autofirmado temporal.
- Escenario: `import-sii-read`.
- Recurso: tarea descartable y dato SII registrados en la autorización saneada; no se conservan credenciales ni respuestas externas.
- Resultado: aprobado; `ResolveCapabilities`, `QueryItems` y `GetPreview` terminaron sin código de error.
- Controles de solo lectura: 3 comprobados, sin cambios antes/después.
- Recursos mutantes: 0.
- Gate al cierre: `WorkflowCentroTrabajoModernActive=false`, usuarios y grupos vacíos.
- Integridad legacy: páginas protegidas sin cambios.

## Tiempos totales observados

| Operación | Muestra | Éxitos | Errores | Total por llamada (ms) | Trabajo SII observable |
| --- | ---: | ---: | ---: | ---: | --- |
| `ResolveCapabilities` | 1 | 1 | 0 | 5743 | No |
| `QueryItems` | 1 | 1 | 0 | 569 | Sí: token y consulta del sello |
| `GetPreview` | 1 | 1 | 0 | 870 | Sí: reconsulta y descarga del anexo |

Resumen de las tres llamadas: mínimo 569 ms, promedio 2394 ms, p50 870 ms, p95 5743 ms, p99 5743 ms y máximo 5743 ms. El arnés registra tiempo total por operación; no expone ni inventa una partición interna exclusiva de red SII.

## Alcance pendiente

Esta evidencia aprueba únicamente la lectura real. La escritura, la concurrencia, la recuperación y sus métricas permanecen pendientes y requieren perfiles/autorizaciones de recurso descartable propios.
