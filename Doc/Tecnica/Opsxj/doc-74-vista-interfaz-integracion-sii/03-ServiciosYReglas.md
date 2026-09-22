# VISTA-INTERFAZ-INTEGRACION-SII

- Ticket: DOC-74
- Cambio OpenSpec: doc-74-vista-interfaz-integracion-sii
- Clasificacion: cross_cutting (Transversal)

## Servicios y reglas

- Se reutiliza `ImportarServicioWebApi.getPreview`; no existe un segundo transporte HTTP.
- `ExternalKey` y `ProviderId` solicitan el recurso; `DescriptorId` es la única autoridad pública para el handler.
- El descriptor debe cumplir formato base64url y genera una ruta relativa same-origin.
- Una promesa activa se comparte durante la apertura; foco, resize y rerender no repiten la descarga.
- La renovación es explícita y crea un descriptor nuevo sin mutar tarea, estado o auditoría.
- Gate, autorización, expiración y proveedor se traducen a estados cerrados y mensajes controlados.
- El identificador interno solo habilita el visor si coincide con una fila server-rendered y con la tarea confiable actual.

No se modifican `ClassAlmacenamiento`, `AlmacenaDocumentoTareaWorkflow(...)`, el handler ni el visor documental.
