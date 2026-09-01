# DOC-42 — Transacciones de Notas Workflow

Paquete técnico de implementación para las escrituras modernas de Notas Workflow.

- Ticket: `DOC-42`
- Cambio OpenSpec: `doc-42-transacciones-notas`
- Alcance: ASMX moderno, contratos, servicio, repositorio transaccional, idempotencia, ETag, auditoría y E2E autorizada.
- Fuera de alcance: UI nueva, activación del gate, consumidores legacy y carga.

| Documento | Contenido |
| --- | --- |
| [01-arquitectura.md](01-arquitectura.md) | Capas, límites y decisiones de persistencia. |
| [02-contrato.md](02-contrato.md) | Operaciones, entradas, respuestas y códigos funcionales. |
| [03-flujo-y-seguridad.md](03-flujo-y-seguridad.md) | Preflight, autorización, transacciones, versiones y rollback. |
| [04-pruebas-y-evidencia.md](04-pruebas-y-evidencia.md) | Suites locales, E2E, controles y resultados saneados. |
| [05-inventario-funciones.md](05-inventario-funciones.md) | Inventario de componentes y archivos afectados. |
| [06-liberacion-operacion-controlada.md](06-liberacion-operacion-controlada.md) | Precondiciones, decisión operativa y reversión. |

Documentos complementarios: [matriz de pruebas](../matriz-pruebas.md), [migración SQL](../2026-08-31-migracion-transacciones.sql), [exploración](../Exploracion/) y [prompts](../Prompt/).
