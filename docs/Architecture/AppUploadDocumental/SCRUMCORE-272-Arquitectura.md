# SCRUMCORE-272 - Arquitectura upload-storage-client

Fecha: 2026-06-25

## Alcance

Se implementa un cliente tecnico reusable para almacenamiento documental con flujo:

```txt
init -> chunks -> complete -> almacenar
```

El modulo vive fuera de UI en:

```txt
src/modules/almacenamientoDocumental/
```

No crea componentes React, modales, layout, manejo visual de tipologias ni callbacks de interfaz. El consumidor interpreta el resultado backend.

## Endpoints usados

```txt
POST /api/gestor-documental/almacenamiento/upload-temporal/init
PUT /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}
GET /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status
POST /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete
DELETE /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}
POST /api/gestor-documental/almacenamiento
```

Los ids temporales se codifican con `encodeURIComponent`. `chunkIndex` queda base cero por precedente local de upload temporal y por ausencia de evidencia backend accesible en este workspace.

## Matriz FE/BE

| FE type | Campo FE | Evidencia disponible | Guard runtime |
| --- | --- | --- | --- |
| `StorageUploadInitRequest` | `nombreOriginal` | Prompt SCRUMCORE-272 | string requerido |
| `StorageUploadInitRequest` | `tamanoBytes` | Prompt SCRUMCORE-272 | numero positivo |
| `StorageUploadInitRequest` | `extension` | Prompt SCRUMCORE-272 | string, puede ser vacio si archivo no tiene extension |
| `StorageUploadInitRequest` | `hashSha256Esperado` | Prompt SCRUMCORE-272 | opcional `string | null` |
| `StorageUploadInitRequest` | `numeroChunks` | Prompt SCRUMCORE-272 | numero positivo |
| `StorageUploadInitResponse` | `rutaTemporalId` | Prompt SCRUMCORE-272 | string no vacio |
| `StorageUploadInitResponse` | `archivoTemporalId` | Prompt SCRUMCORE-272 | string no vacio |
| `StorageUploadInitResponse` | `chunkSizeBytes` | Prompt SCRUMCORE-272 | numero positivo |
| `StorageUploadInitResponse` | `estado` | Prompt SCRUMCORE-272 | string no vacio |
| `DocumentoEntrada` | `archivoTemporalId`, `nombreOriginal`, `extension` | Prompt SCRUMCORE-272 | payload construido por archivo |
| `AlmacenarDocumentoRequest` | `nombreGabinete`, `rutaTemporalId`, `nombreDocumento`, `requestId`, `documentos` | Prompt SCRUMCORE-272 | strings requeridos y documentos no vacio |
| `AlmacenarDocumentoResponse` | `idAlmacen` | Prompt SCRUMCORE-272 | numero positivo |
| `AlmacenarDocumentoResponse` | `idRegistroProduccionDocumental` | Prompt SCRUMCORE-272 | numero positivo |
| `AlmacenarDocumentoResponse` | `nombreArchivoFinal` | Prompt SCRUMCORE-272 | string no vacio |
| `AlmacenarDocumentoResponse` | `requestId` | Prompt SCRUMCORE-272 | string no vacio |

## Evidencia backend

Los paths externos solicitados no estaban disponibles desde este workspace durante la implementacion:

```txt
D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\Controllers\GestorDocumental\AlmacenamientoDocumental\AlmacenamientoDocumentalController.cs
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.DTOs\DTOs\GestorDocumental\AlmacenamientoDocumental
```

La implementacion refleja el contrato del prompt y acepta nombres camelCase/PascalCase en respuestas para tolerar serializacion .NET sin ocultar errores de contrato.
