# DOC-127 — Verificación integral, capacidad y cierre

- Ticket: DOC-127
- Cambio OpenSpec: doc-127-importacion-excel-07-verificacion-integral-cierre
- Clasificación: cross_cutting; perfil legacy-webforms-vb.

## Servicios y reglas

ServicioLecturaExcel valida formato/hoja/tipos/límites; Preparacion obtiene esquema, claves y snapshots; Confirmacion revalida autoridad, archivo, revisión, política y límites. MySqlConfirmacionExcelWriter mantiene locks e integridad DML y registra resultado atómico; MySqlEstructuraExcel distingue estructura propia de tabla ajena. ConsultarResultado/replay por ID recupera el resultado; no habilita pausa ni commit parcial. Los límites de archivos se expresan en bytes decimales.
