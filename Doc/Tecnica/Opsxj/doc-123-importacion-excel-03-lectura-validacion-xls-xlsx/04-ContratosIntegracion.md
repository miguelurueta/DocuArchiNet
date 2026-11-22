# DOC-123 — 04-ContratosIntegracion

- Ticket: DOC-123
- Cambio OpenSpec: doc-123-importacion-excel-03-lectura-validacion-xls-xlsx
- Clasificacion: backend_vb (Backend VB.NET)

## Contratos e integraciones

SolicitudLecturaExcel recibe stream del servidor, formato/nombre descriptivo, hoja exacta, plantilla/metadatos y límites. ResultadoLecturaExcel devuelve tipo/hojas/encabezados, filas numeradas, tipos semánticos, estado completo y conteos/incidencias; números canónicos admiten DECIMAL65. El stream se conserva abierto/posición original. Consumidor fase04 resuelve autoridad y revalida hashes/esquema; el lector no acredita unicidad ni PREPARADA. Sin nuevo transporte público, SQL o cambio del snapshot v1.
