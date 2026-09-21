# Seguridad y amenazas

| Amenaza | Control implementado |
| --- | --- |
| Enumeración | Descriptor CSPRNG de 256 bits y respuesta 404 uniforme. |
| Robo o cruce de descriptor | Hash SHA-256 y vínculo a usuario, tarea y proveedor. |
| SSRF | La URL solo proviene de SII y conserva allowlist de host/esquema. |
| Doble exposición | UPDATE condicional `Disponible -> Reclamado`. |
| Fuga por logs | No se registra descriptor, BLOB, URL, token ni identidad del sujeto. |
| Caché del navegador/proxy | Política privada `no-store`, `no-cache` y `nosniff`. |
| Contenido activo | Allowlist PDF/PNG/JPEG/TIFF y disposición calculada por servidor. |
| Agotamiento | Límite predeterminado 10 MiB, TTL cinco minutos y limpieza oportunista. |

Riesgo aceptado: una desconexión posterior a la reclamación consume el descriptor; se prioriza impedir la doble entrega.

Fuentes: `ImportPreviewDescriptorService`, `ImportPreviewContentService`, `ImportPreviewDescriptorRepository`, `ImportarServicioWebPreview`.
