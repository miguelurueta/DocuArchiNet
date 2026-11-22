# DOC-124 — 04-ContratosIntegracion

- Ticket: DOC-124
- Cambio OpenSpec: doc-124-importacion-excel-04-recepcion-preparacion
- Clasificacion: cross_cutting (Transversal)

## Contratos e integraciones

IniciarRecepcionExcel entrega ID/referencia antes del multipart; ASHX recibe un archivo; Preparar verifica hoja/modo/revisión y ConsultarCargaExcel lee resultado propio. Persistir en las22 columnas de imp01_carga_ejecucion, sin placeholders de hash o bytes. Referencias privadas y contexto vinculados a empresa/módulo/usuario, sin rutas o conexión del cliente. Vista previa25 filas/incidencias con totales y etapas no ejecutadas expresas. Contratos detallados en design.md; no hay endpoints nuevos ejecutados en esta revisión.
