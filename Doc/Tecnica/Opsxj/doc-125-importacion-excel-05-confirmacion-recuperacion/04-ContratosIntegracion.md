# DOC-125 — Confirmación y recuperación

- Ticket: DOC-125
- Cambio OpenSpec: doc-125-importacion-excel-05-confirmacion-recuperacion
- Clasificacion: cross_cutting (Transversal)
- Estado: refinamiento técnico, sin implementación ni E2E escritora ejecutada.

## Contratos e integraciones

SolicitudConfirmarExcel: ConexionId, IdCarga, ArchivoHash, ConfiguracionHash, EsquemaHash, ConfirmacionReemplazo. La consulta usa ConexionId/IdCarga existentes. EstadoCargaExcel incorpora FilasConfirmadas y ConfirmadoUtc; jamás expone archivo_ref, documento interno, secretos ni identificadores de sesión MySQL.

Mantener resolución de DA y política CONEXION/COLUMNA actuales, con UNIQUE completo según ámbito. No editar SII, perfiles de carpetas ni proveedores para escribir. Las tablas nuevas se incorporarán a fuentes mediante los servicios administrativos correspondientes; confirmar no las publica automáticamente. El SQL de instalación se entrega para ejecución manual y las consultas de control son SELECT.
