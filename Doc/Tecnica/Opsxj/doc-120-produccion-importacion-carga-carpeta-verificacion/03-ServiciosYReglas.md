# DOC-120 — ServiciosYReglas

- Ticket: DOC-120
- Cambio OpenSpec: doc-120-produccion-importacion-carga-carpeta-verificacion
- Clasificacion: cross_cutting

## Servicios y reglas

ServicioConsultaImportacion reautoriza y exporta una página sin reconciliar. Coordinador/Barreras/Valores/RecuperacionV2 conservan fases, integridad, valores diferidos y revisiones; INCIERTO y stop no autorizan replay/rollback. Los adaptadores únicos y la limpieza deshabilitada se preservan. Ninguna fuente de producto cambia en este refinamiento.
