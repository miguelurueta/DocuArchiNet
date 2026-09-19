# LISTADO-TIPOLOGIAS

- Ticket: DOC-68
- Cambio OpenSpec: doc-68-listado-tipologias
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Extender aditivamente `ResolveCapabilities` y `QueryItems` para publicar el catálogo de tipologías autorizado y presentar cada sello SII con metadatos saneados, estado local y acciones permitidas, sin llamadas externas por fila.

## Alcance y compatibilidad

- [x] Servicios afectados: `WebServiceImportarServicioWebModern`, mapper/cliente SII, servicio de presentación y repositorios modernos de catálogo y estado.
- [x] Contratos 1.0 preservados mediante extensiones 1.1 aditivas; `ExternalKey` continúa opaco para el cliente.
- [x] Páginas y controles legacy preservados sin modificaciones; el gate apagado mantiene el flujo anterior.
- [x] Reversa: desplegar con `WorkflowCentroTrabajoModernActive=false`; usuarios y grupos permanecen vacíos.

La implementación lista exclusivamente sellos `tipoanexo=505`. Los anexos de otros tipos no se publican y una clave fabricada para ellos es rechazada por el resolvedor físico.
