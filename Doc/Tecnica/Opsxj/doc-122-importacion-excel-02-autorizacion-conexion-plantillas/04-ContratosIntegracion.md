# DOC-122 — IMPORTACION-EXCEL-02-AUTORIZACION-CONEXION-PLANTILLAS

- Ticket: DOC-122
- Cambio OpenSpec: doc-122-importacion-excel-02-autorizacion-conexion-plantillas
- Clasificacion: cross_cutting

## Contratos e integraciones
ASMX: AbrirCargaExcel, ListarPlantillas, ConsultarPlantilla y GuardarPlantilla. POST JSON, CSRF propio X-Importacion-Excel-CSRF y sesión como autoridad. Respuestas tipadas con ok/codigo/mensaje/operacionId/datos. Navegador envía IDs y configuración de plantilla, nunca host, base o credenciales. Guardar exige revisión y huella esperadas; alta genera ruta relativa web y edición conserva ruta almacenada. XLS→8.0 xls, XLSX→12.0 xlsx. Crear/agregar comparten flags históricos; metadatos distinguen destino ausente. Catálogos reutilizados imp01_xmlconexion/plantillaimp/campos_plantilla; consumidores SII sin cambios. No expone interfaces Dictionary en ASMX. SQL10 es solo SELECT; concesiones del SQL05 continúan manuales y no acreditadas por esta entrega.
