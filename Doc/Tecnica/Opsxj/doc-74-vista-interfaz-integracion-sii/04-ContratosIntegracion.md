# VISTA-INTERFAZ-INTEGRACION-SII

- Ticket: DOC-74
- Cambio OpenSpec: doc-74-vista-interfaz-integracion-sii
- Clasificacion: cross_cutting (Transversal)

## Contratos e integraciones

### GetPreview

El request reutiliza el envelope ASMX con `SchemaVersion`, `OperationId`, `CorrelationId`, `TaskId`, `ProviderId` y `ExternalKey`. El servidor contrasta tarea y sesión. La respuesta consumida contiene `DescriptorId`, `ContentType`, `ExpiresAtUtc` o un `Error.Codigo` seguro.

### Handler

La UI deriva únicamente `../workflow/ImportarServicioWebPreview.ashx?d=<descriptor codificado>`. No recibe URL externa, bytes JSON/base64, ruta física ni token del proveedor. Formatos no visualizables usan el mismo handler como descarga temporal.

### Visor documental

`internalDocumentId` se contrasta contra `GridView_list_documento_relacion_wf`, renderizado por servidor. Se exige coincidencia exacta de `id_wf`, identidad en `idd_wf` y tarea actual; entonces se reutiliza el postback `Button_selecion_treview_documento`. El cliente no fabrica la selección ni modifica el visor.

### Esquema y compatibilidad

DOC-74 no agrega tablas, migraciones ni cambios de contrato backend. Depende de B10 para rollout y conserva el gate apagado.
