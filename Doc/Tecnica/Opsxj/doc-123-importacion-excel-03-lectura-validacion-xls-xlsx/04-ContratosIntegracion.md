# DOC-123 — Contratos de integración

- Ticket: DOC-123
- Cambio OpenSpec: doc-123-importacion-excel-03-lectura-validacion-xls-xlsx
- Clasificacion: cross_cutting (Transversal)

## Contratos e integraciones

Entrada interna: stream legible/seekable del servidor, formato esperado, hoja, snapshot PlantillaExcel, columnas físicas/proyectadas y límites efectivos. Salida: formato/hojas/encabezados, filas tipadas con coordenadas, validez/lectura completa y conteos/incidencias acotadas. El lector conserva propiedad/posición del stream y no abre conexiones. Fase04 resolverá autoridad/DA y revalidará plantilla/esquema; el resultado no acredita unicidad MySQL ni PREPARADA. Sin cambios de transporte público o hash v1. Contrato detallado en design.md; implementación abierta.
