# Contratos e integraciones

- Ticket: DOC-55
- Cambio OpenSpec: doc-55-adaptador-sii-asmx
- Clasificacion: cross_cutting

## Contratos e integraciones

La identidad canónica es `INTEGRACIONSII`. La clave estable usa `SII:<libro>:<registro>:<matricula>` y nunca incluye token, URL, ruta física o autenticación.

Los seis fixtures `sii-v1` modelan consulta del proveedor, respuesta normalizada, descriptor de preview y traducciones legacy. Todos contienen valores `DEMO` inventados y se ejecutan sin red.

| Operación | Entrada | Salida |
| --- | --- | --- |
| `ResolveCapabilities` | contexto y proveedor | capacidades y timeout |
| `QueryItems` | contexto y paginación | items con clave estable |
| `GetPreview` | contexto y clave externa | descriptor temporal seguro |

El cliente utilizará `ExternalImportHttpTransport`; sus errores se sanean antes de cruzar el adaptador.
