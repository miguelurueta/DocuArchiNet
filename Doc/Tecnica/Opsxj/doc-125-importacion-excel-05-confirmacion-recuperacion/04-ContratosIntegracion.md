# DOC-125 — Confirmación transaccional y recuperación

- Ticket: DOC-125
- Cambio OpenSpec: doc-125-importacion-excel-05-confirmacion-recuperacion
- Clasificacion: cross_cutting (Transversal)
- Estado: implementación y aceptación real5/5 verificadas.

## Contratos e integraciones

Confirmar: ConexionId, IdCarga, ArchivoHash, ConfiguracionHash, EsquemaHash y ConfirmacionReemplazo. Usuario/empresa, tabla, modo y política proceden del servidor. Consulta: ConexionId/IdCarga propios. EstadoCargaExcel añade FilasConfirmadas, ConfirmadoUtc y estado estructural seguro cuando corresponde; no expone archivo_ref, documento interno, secretos ni connection IDs.

Las fuentes CONEXION/COLUMNA y el UNIQUE completo conservan su semántica. Para COLUMNA, el DELETE usa empresa parametrizada y la validación de claves impide filas de otra empresa. La prueba de ese filtro es local; el scope real usa CONEXION. El CREATE vincula UUID/hash completos en comentario compacto56ASCII y mantiene separado el DDL de la transacción DML. No hay publicación automática de fuentes nuevas ni cambios de SII, perfiles de carpetas o proveedores.

Se conserva la referencia a datos actuales de los consumidores: no se añade congelación/versionado de sus filas diferidas. SQL18–21 fueron aplicados/verificados por delegación expresa del usuario fuera de los tests; no se reaplican. Las fuentes/tablas originales permanecen fuera de la autorización escritora. El actor externo usa el catálogo del módulo y ODBC BigInt/Int64; su preflight real de lectura pasa antes de intentar la carrera. Los controles SELECT reutilizan las conexiones existentes, sin nueva cadena de conexión.
