# Contrato HTTP

## Creación

`POST webservice/WebServiceImportarServicioWebModern.asmx/GetPreview`

Entrada: `GetPreviewRequestDto` con contexto estándar y `ExternalKey`. Salida: `GetPreviewResponseDto` con `DescriptorId` opaco, tipo, longitud, disposición y expiración. No contiene URL, token, ruta ni contenido.

## Canje

`HEAD workflow/ImportarServicioWebPreview.ashx?d={descriptor}` devuelve 200 y metadatos sin consumir; `GET` devuelve 200 una sola vez. Métodos distintos devuelven 405. Descriptor ausente, alterado, vencido, ajeno o consumido devuelve 404 vacío; infraestructura no disponible devuelve 503 vacío.

Headers exitosos: `Content-Type`, `Content-Length`, `Content-Disposition`, caché privada sin almacenamiento, `Pragma: no-cache`, `X-Content-Type-Options: nosniff` y `X-Frame-Options: SAMEORIGIN`.

Fuentes: `DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb`, `workflow/ImportarServicioWebPreview.ashx.vb`.
