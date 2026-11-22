# DOC-124 — 03-ServiciosYReglas

- Ticket: DOC-124
- Cambio OpenSpec: doc-124-importacion-excel-04-recepcion-preparacion
- Clasificacion: cross_cutting (Transversal)

## Servicios y reglas

Se planifican ServicioRecepcionExcel/ServicioPreparacionExcel, almacén privado y repositorio de ejecuciones/metadatos. Reutilizar lector DOC123 y guarda/autoridad DOC122. Publicar RECIBIDA con bytes/hash reales y PREPARADA solo tras validación completa. Aislamiento empresarial explícito, índices completos, temporales con tipos/collation del destino y limpieza de conexión. Snapshot efectivo sin modificar huella de plantilla v1. No se implementaron estos servicios durante el refinamiento.
