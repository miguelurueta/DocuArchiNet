# DOC-114 — Preparación focal de planes

- Ticket: DOC-114
- Cambio OpenSpec: doc-114-produccion-importacion-carga-carpeta-plan-imple
- Clasificacion: cross_cutting (Transversal)

## Servicios y reglas

ServicioPreparacionImportacion.Abrir/ListarPerfiles/CalcularPlan/Preparar y MySqlImportacionCarpetasRepository se conservan. El cliente focal permite solo apertura/listado/preparación; la suite comprueba contexto/huella/revisión/TTL y metadatos persistidos mediante SELECT. PLAN-02 acredita EXPEDIENTE/LOTE/PARCIAL y conservación de rechazados. PLAN-01 comprueba formatos, errores correlacionados y valores diferidos; PLAN-03 prueba rechazo de datos finales/refs y selección compatible.

IMPORTAR_CARPETAS se revocó y restauró exclusivamente para la cuenta autorizada (141/2). Restored=true, permiso1 por SELECT y otros campos intactos son evidencia real; no se modificó el permiso administrativo. Los comparadores respetan solo las omisiones null del modelo y el orden canónico documentado. [Informe consolidado](../../../Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc114-final-report-20261103.md).
