# DOC-102 — contrato e integración

- Ticket: DOC-102
- Cambio OpenSpec: doc-102-streamer-http-rangos
- Clasificacion: cross_cutting

## Contratos e integraciones

### Entrada autorizada

`GET|HEAD workflow/Handler_image_wf.ashx?doc_ref=<referencia-opaca>`

La referencia requiere la misma sesión que la emitió y expira a los cinco minutos. `GET` admite una cabecera Range simple; `HEAD` describe la representación completa y no envía cuerpo.

| Caso | Estado | Cuerpo y encabezados relevantes |
|---|---:|---|
| Completo | 200 | `Content-Length`, MIME, disposición y cuerpo completo |
| Rango válido | 206 | `Content-Range`, longitud exacta y solo el intervalo |
| Revalidación coincidente | 304 | sin cuerpo |
| Rango inválido | 416 | `Content-Range: bytes */N`, sin cuerpo |

Todas las representaciones autorizadas anuncian `Accept-Ranges: bytes`, ETag fuerte opaco, `Last-Modified`, `Cache-Control` privado/no-cache/con revalidación, `Vary: Cookie` y `X-Content-Type-Options: nosniff`.

## Compatibilidad legacy

`rut_image` continúa para descarga, otros formatos, versiones/migración, visores externos y ramas `LIBRE_PDF`. Devuelve cuerpo completo por bloques, caché `no-store`, sin `206`, `Accept-Ranges`, ETag ni el nuevo modelo de autorización. Los tres emisores enriquecidos no pueden volver a usarlo, protegidos por una prueba de política.

No hay migración de datos, cambio de esquema ni nueva conexión externa.

## Contrato experimental local

`npm.cmd --prefix tools/e2e run test:doc102:linearization-poc` opera fuera del flujo productivo. Solo usa un fixture público, `127.0.0.1`, un directorio temporal y el PDF.js existente. Ghostscript genera la variante `FastWebView`; el runner valida `/Linearized`, páginas, huella del primer canvas, bytes únicos, bytes físicos, solicitudes y limpieza. No recibe `doc_ref`, sesión, credenciales ni rutas de documentos del sistema y nunca reemplaza el fixture.

El contrato experimental no forma parte del handler ni crea una nueva modalidad de almacenamiento. Su salida es evidencia para una decisión posterior, no una dependencia desplegable.
