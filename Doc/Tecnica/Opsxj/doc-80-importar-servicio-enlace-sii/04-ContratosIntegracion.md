# IMPORTAR-SERVICIO-ENLACE-SII

- Ticket: DOC-80
- Cambio OpenSpec: doc-80-importar-servicio-enlace-sii
- Clasificacion: cross_cutting (Transversal)
## Contratos e integraciones

Endpoint moderno: `ResolveCapabilities`, `QueryItems` y `GetPreview`. El payload agrega `Capability` de forma opcional; vacío conserva constancias. SII usa `solicitarToken` y `consultarRadicado` con credenciales existentes. Preview usa descriptor/handler vigente. Sin cambios de esquema.
