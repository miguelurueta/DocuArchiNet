# SCRUMCORE-272 - Implementacion detallada

Fecha: 2026-06-25

## Archivos

```txt
src/modules/almacenamientoDocumental/services/almacenamientoDocumentalUpload.service.ts
src/modules/almacenamientoDocumental/types/almacenamientoDocumental.types.ts
src/modules/almacenamientoDocumental/utils/storageFile.utils.ts
src/modules/almacenamientoDocumental/services/almacenamientoDocumentalUpload.service.test.ts
src/modules/almacenamientoDocumental/utils/storageFile.utils.test.ts
```

## Servicio publico

```txt
initTemporaryUpload
uploadTemporaryChunk
getTemporaryUploadStatus
completeTemporaryUpload
cancelTemporaryUpload
almacenarDocumento
uploadAndStoreOneDocument
```

`uploadAndStoreOneDocument` procesa un archivo por llamada y construye un `POST /api/gestor-documental/almacenamiento` con un solo `DocumentoEntrada`, `rutaTemporalId` y `requestId`.

## Chunks

- El `init` calcula `numeroChunks` con `initialChunkSizeBytes`.
- Si backend responde otro `chunkSizeBytes`, se recalcula el total real antes de subir chunks.
- Cada chunk usa `Blob.slice`.
- Los chunks se envian como body binario con `Content-Type: application/octet-stream`.
- Se envia header `X-Total-Chunks`.
- El indice de chunk queda base cero, alineado con precedentes locales.

## Errores

Se implementa `AlmacenamientoDocumentalUploadError` con:

```txt
code
phase
message
requestId
details
cause
```

Codigos:

```txt
storage_contract_error
storage_init_error
storage_chunk_error
storage_status_error
storage_complete_error
storage_cancel_error
storage_store_error
storage_aborted
```

`storage_status_error` se agrega porque el endpoint `status` es parte del contrato obligatorio.

## Seguridad y desacople

- Se usa exclusivamente `clienteApi`.
- No se usa `.ashx`.
- No se usa `XMLHttpRequest`.
- No se usa `fetch` directo.
- No se usa jQuery.
- No se usa `FormData` para chunks.
- No se importan tipos de `AppUploadDocumental`.
- No se guardan URLs temporales.
- No se guarda `File` en storage global.
- No se loguean tokens, bytes de archivo ni payload sensible.
- No se concatena informacion con `|`.
- No se transforma por nombres legacy como `funcion_name`.

## Retorno para interfaz

`almacenarDocumento` retorna la respuesta normalizada `AlmacenarDocumentoResponse`.

`uploadAndStoreOneDocument` retorna:

```txt
temporal
response
rawBackendResult
```

`rawBackendResult` preserva campos adicionales del backend para que capas superiores construyan eventos visuales sin acoplar este cliente a UI.
