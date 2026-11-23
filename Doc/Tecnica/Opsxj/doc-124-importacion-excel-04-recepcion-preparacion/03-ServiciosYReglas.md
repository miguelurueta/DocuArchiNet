# DOC-124 — 03-ServiciosYReglas

- Ticket: DOC-124
- Cambio OpenSpec: doc-124-importacion-excel-04-recepcion-preparacion
- Clasificacion: cross_cutting (Transversal)

## Servicios y reglas

ServicioRecepcionExcel emite referencia MachineKey y coordina archivo inmutable/RECIBIDA; ServicioPreparacionExcel reutiliza lector DOC123, verifica metadatos/aislamiento/índices, compara claves mediante temporales y publica PREPARADA con CAS. El repositorio mantiene un lock por ID y transacciones cortas, sin parseo transaccional. Fallos inciertos conservan archivo/ganador; consulta propia no repara. Snapshot efectivo independiente de huella de plantilla v1. Limpieza de temporales/modo/lock o descarte dirigido de pool. Sin DDL o DML de negocio; SQL11 se entrega manualmente.
