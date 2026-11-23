# DOC-125 — Confirmación transaccional y recuperación

- Ticket: DOC-125
- Cambio OpenSpec: doc-125-importacion-excel-05-confirmacion-recuperacion
- Clasificacion: cross_cutting (Transversal)
- Estado: implementación local verificada, aceptación real abierta.

## Contratos e integraciones

Confirmar: ConexionId, IdCarga, ArchivoHash, ConfiguracionHash, EsquemaHash y ConfirmacionReemplazo. Usuario/empresa, tabla, modo y política proceden del servidor. Consulta: ConexionId/IdCarga propios. EstadoCargaExcel añade FilasConfirmadas, ConfirmadoUtc y estado estructural seguro cuando corresponde; no expone archivo_ref, documento interno, secretos ni connection IDs.

Las fuentes CONEXION/COLUMNA y el UNIQUE completo conservan su semántica. Para COLUMNA, el DELETE usa empresa parametrizada y la validación de claves impide filas de otra empresa. La prueba de ese filtro es local; el scope real preparado usa CONEXION. El CREATE lleva UUID/hash en comentario y mantiene separado el DDL de la transacción DML. No hay publicación automática de fuentes nuevas ni cambios de SII, perfiles de carpetas o proveedores.

Se conserva la referencia a datos actuales de los consumidores: no se añade congelación/versionado de sus filas diferidas. SQL18–21 son archivos manuales no ejecutados; las fuentes/tablas reales previas permanecen fuera de la autorización escritora de prueba.
