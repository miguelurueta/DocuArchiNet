# NUCLEO-INTRFAZ-INTEGRACION-SII

- Ticket: DOC-72
- Cambio OpenSpec: doc-72-nucleo-intrfaz-integracion-sii
- Clasificación: cross_cutting

## Contratos e integraciones

El cliente consume `WebServiceImportarServicioWebModern.asmx` con POST JSON `{ request: dto }`, `Content-Type: application/json; charset=utf-8` y credenciales de mismo origen. La tarea y el proveedor llegan al bootstrap mediante atributos `data-*` codificados desde el servidor.

No se cambiaron endpoints, DTO, esquema de datos, almacenamiento ni consumidores legacy. `ImportarServicioWebProviderId` queda vacío por defecto y un valor ausente produce `PROVIDER_NOT_CONFIGURED` sin redirigir a proveedor alguno.
