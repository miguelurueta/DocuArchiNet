# DOC-124 — 04-ContratosIntegracion

- Ticket: DOC-124
- Cambio OpenSpec: doc-124-importacion-excel-04-recepcion-preparacion
- Clasificacion: cross_cutting (Transversal)

## Contratos e integraciones

IniciarRecepcionExcel entrega ID/referencia antes del multipart; ASHX recibe un archivo; Preparar verifica hoja/modo/revisión y ConsultarCargaExcel lee resultado propio. Se registran los nuevos Compile/Content y se reutilizan las22 columnas de imp01_carga_ejecucion, sin hash/bytes ficticios. Referencia de30min vinculada a empresa/módulo/usuario, sin rutas del cliente. Vista previa25 filas/incidencias con totales y etapas no ejecutadas. No retorna configuracion_doc/ruta privada. Los contratos compilan y se prueban localmente; endpoints HTTP/MySQL reales aún no ejecutados ni desplegados.
