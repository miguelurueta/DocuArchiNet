## Why

DOC121/DOC122 entregaron contratos y plantillas, pero falta un lector común de XLS/XLSX que conserve valores y detecte errores antes de cargar. El envoltorio FastExcel elimina caracteres y el importador antiguo omite la primera fila; usarlos como lector nuevo reproduciría pérdidas.

## What Changes

- Añadir lectura por stream/hoja con ExcelDataReader3.6.0 net45 y formato visual con ExcelNumberFormat1.1.0 net20, conservando net461.
- Aplicar mapeos y transformaciones guardados y validar capacidad física, fechas, nulos y límites del servidor.
- Devolver filas tipadas/coordenadas e incidencias controladas para fase04.
- Ampliar harness existentes con libros sintéticos de ambos formatos, preservando SII y FastExcel.

## Capabilities

### New Capabilities

- `importacion-excel-03-lectura-validacion-xls-xlsx`: lectura por hoja y validación de valores/filas sin efectos en la base.

### Modified Capabilities

Ninguna. Se reutilizan los contratos y conexión DA del módulo de DOC121/DOC122.

## Impact

Modelos internos, nuevos ServicioLecturaExcel/LectorArchivoExcel, packages.config, referencias/Compile del vbproj y pruebas nativas. Sin ASMX/ASHX, UI, escritura MySQL, staging persistente ni cambios SQL. Se mantienen permiso RA, cuenta DA compartida, huella v1 y límites10000/10MB decimales.

## Fuente y límites

Ticket DOC-123 y Doc/Actualizacion/Importacion/Exploracion/03-lectura-y-validacion-excel.md; contexto Jira conservado en specs/importacion-excel-03-lectura-validacion-xls-xlsx/jira-context.md. La cuenta dedicada del prompt original fue sustituida por decisión posterior del usuario en DOC122. Su autorización E2E ya está cerrada.

La fidelidad de errores/fórmulas se caracteriza primero: ExcelDataReader3.6.0 convierte algunos errores en null. La integración exige distinguirlos de celdas vacías; instalar el paquete no acredita esa capacidad.
