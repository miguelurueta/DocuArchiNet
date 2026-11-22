# DOC-123 — 03-ServiciosYReglas

- Ticket: DOC-123
- Cambio OpenSpec: doc-123-importacion-excel-03-lectura-validacion-xls-xlsx
- Clasificacion: backend_vb (Backend VB.NET)

## Servicios y reglas

ServicioLecturaExcel valida mapeos, transformaciones y tipos físicos; LectorArchivoExcel adapta stream/hoja; InspectorFidelidadExcel conserva errores/caché que el paquete oculta. Flujo puro sin DB/HTTP. Preserva posiciones y caracteres, aplica coma→punto→segmento y usa ValorCompatible más reglas DATE/NULL/Unicode. Un error invalida la salida y vacía filas; ausencia de caché interrumpe sin ofrecer prefijos. Límites positivos10000/10MB y diagnósticos acotados1000 con total. Detalle vigente en diseño03 y evidencia local DOC123.
