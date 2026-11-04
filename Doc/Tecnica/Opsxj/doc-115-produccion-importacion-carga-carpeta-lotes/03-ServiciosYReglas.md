# DOC-115 — ServiciosYReglas

- Ticket: DOC-115
- Cambio OpenSpec: doc-115-produccion-importacion-carga-carpeta-lotes
- Clasificacion: cross_cutting

Estado: implementación y validación completas, entrega pendiente de revisión/publicación.

## Servicios y reglas

ServicioRecepcionImportacion y MySqlImportacionCarpetasRepository existentes conservan autoridad, idempotencia, revisión y estados. 07-A verificó concurrencia de confirmación/reserva/recepción y cuatro rollbacks en conexiones físicas independientes. Staging conserva un original y partes rechazadas; el reenvío no duplica contenido. Cuotas efectivas de archivo se comprobaron en HTTP y cuota acumulada finita se caracteriza localmente con inyección, sin inventar configuración global. El contexto directo de repositorio es fixture explícita; autoridad HTTP se acredita por RECIB.
