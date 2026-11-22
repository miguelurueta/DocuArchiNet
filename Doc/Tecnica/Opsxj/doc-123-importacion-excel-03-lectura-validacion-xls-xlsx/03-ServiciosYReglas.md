# DOC-123 — Servicios y reglas

- Ticket: DOC-123
- Cambio OpenSpec: doc-123-importacion-excel-03-lectura-validacion-xls-xlsx
- Clasificacion: cross_cutting (Transversal)

## Servicios y reglas

ServicioLecturaExcel coordinará mapeos y validación pura; LectorArchivoExcel adaptará formato/hojas/celdas de un stream controlado por servidor. Nombres fuente exactos, encabezado físico1, primera/última fila incluidas y huecos conservados. Opciones coma→punto→segmento base0; preservación de otros caracteres. Validar capacidad física con ValorCompatible y complementos DATE/NULL/Unicode. Límites inclusivos con lectura completa obligatoria; errores no se convierten en vacíos válidos. Este documento describe el plan, sin acreditar código o pruebas.
