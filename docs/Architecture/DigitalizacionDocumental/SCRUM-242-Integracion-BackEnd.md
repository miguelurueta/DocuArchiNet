# SCRUM-242 Integracion BackEnd

## Matriz FE-BE

| FE service | Endpoint |
| --- | --- |
| `getDigitalizacionConfiguracion` | `GET /api/gestor-documental/digitalizacion/configuracion` |
| `getDigitalizacionListaChequeo` | `GET /api/gestor-documental/digitalizacion/lista-chequeo` |
| `resolveDigitalizacionMetadata` | `POST /api/gestor-documental/digitalizacion/metadata/resolve` |
| `uploadPdfTemporal` | `POST/PUT/POST /api/gestor-documental/almacenamiento/upload-temporal` |
| `crearDocumentoDigitalizado` | `POST /api/gestor-documental/digitalizacion/documentos` |
| `validarAdjuntarDigitalizacion` | `GET /api/gestor-documental/documentos/{id}/adjuntar-digitalizacion/validacion` |
| `adjuntarDigitalizacion` | `POST /api/gestor-documental/documentos/{id}/adjuntar-digitalizacion` |

## Request

Los requests mantienen nombres PascalCase alineados con los contratos C# documentados.

## Response

Toda respuesta final se espera en envelope `AppResponses<T>` con:

- `success === true`
- `data != null`
- IDs y strings minimos validos

## Upload

El upload usa:

1. Init para obtener `RutaTemporalId` y `ArchivoTemporalId`.
2. Chunks binarios con `Content-Type: application/octet-stream`.
3. Complete con confirmacion `Completado === true`.

## Validaciones

Frontend bloquea:

- contexto/documento destino invalido;
- PDF ausente, vacio o no PDF;
- responses sin data;
- IDs temporales faltantes;
- create/attach sin IDs validos.
