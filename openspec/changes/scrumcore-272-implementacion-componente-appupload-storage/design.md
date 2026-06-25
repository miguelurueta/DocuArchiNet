## Context

`SCRUMCORE-272` implementa un cliente tecnico de almacenamiento documental. El modulo `src/modules/almacenamientoDocumental/` no existe aun en `main`, por lo que este cambio debe crear una base minima de servicios, tipos y utilidades.

Patrones locales relevantes:

- `src/api/Clienteaxios.ts`: cliente Axios central con token y soporte `signal`.
- `src/modules/digitalizacion/services/digitalizacionUploadTemporal.api.ts`: upload temporal por chunks contra `/api/gestor-documental/almacenamiento/upload-temporal/*`.
- `src/app/Components/UI/AppVisorEmbedPdf/services/reemplazoPaginasPdfAnotadas.service.ts`: builders con `encodeURIComponent`, envelope `AppResponses<T>`, headers `application/octet-stream` y `X-Total-Chunks`.

Los paths backend del ticket (`D:\imagenesda\...`) no estan disponibles localmente en este workspace. La implementacion debe mantener los contratos del prompt como base y documentar cualquier ajuste si luego se contrastan DTOs reales.

## Goals / Non-Goals

**Goals**

- Crear un cliente sin UI para `init -> chunks -> status -> complete -> cancel -> almacenar`.
- Usar `clienteApi` y `AbortSignal`.
- Enviar chunks como bytes crudos con `Content-Type: application/octet-stream`.
- Enviar `X-Total-Chunks` en chunks.
- Recalcular chunks si backend retorna `chunkSizeBytes`.
- Validar runtime shapes de respuestas y envelope.
- Emitir progreso por fase.
- Preservar `rawBackendResult` para capas superiores.
- Cubrir flujo feliz, errores y cancelacion con tests.

**Non-Goals**

- Crear componentes React o UI.
- Implementar tipologias visuales o metadata de `AppUploadDocumental`.
- Cambiar endpoints backend.
- Migrar legacy `.ashx`, `FormData`, `XMLHttpRequest`, jQuery o WebForms.
- Resolver refresco de interfaz; eso queda para el mapper/capa documental.

## Decisions

1. **Modulo nuevo sin UI**
   - Ubicacion: `src/modules/almacenamientoDocumental/`.
   - El servicio no importa React ni componentes shared.

2. **Endpoint builders tipados**
   - Definir `ALMACENAMIENTO_DOCUMENTAL_ENDPOINTS`.
   - Usar `encodeURIComponent` para `rutaTemporalId` y `archivoTemporalId`.

3. **Envelope compatible con backend**
   - Modelar `AppResponse<T>` con `success`, `data`, `message`, `errors` y `meta`.
   - `unwrapStorageResponse` debe soportar envelopes similares a los usados en reemplazo PDF y digitalizacion.
   - Si un endpoint retorna una respuesta directa sin envelope, la validacion debe poder procesarla solo cuando sea intencional y testeado.

4. **Errores tipados por fase**
   - Crear `AlmacenamientoDocumentalUploadError`.
   - Codigos minimos: `storage_contract_error`, `storage_init_error`, `storage_chunk_error`, `storage_status_error`, `storage_complete_error`, `storage_cancel_error`, `storage_store_error`, `storage_aborted`.

5. **Contrato frontend conservador**
   - Modelar campos del prompt con nombres frontend.
   - Para campos no confirmados o extensiones backend usar `unknown`, no `any`.
   - Conservar `rawBackendResult?: unknown` en resultados.

6. **Chunk size backend manda**
   - `uploadAndStoreOneDocument` calcula chunks iniciales con `initialChunkSizeBytes`.
   - Si `init` devuelve `chunkSizeBytes` valido y distinto, recalcula `totalChunks` y usa ese valor para `PUT chunk`.

7. **Cancelacion best-effort**
   - Si `AbortSignal` aborta antes de tener ids temporales, no llama `DELETE`.
   - Si aborta despues de `init`, intenta `cancelTemporaryUpload`.
   - Si `cancel` falla durante una cancelacion ya solicitada, propaga error tipado o warning segun API interna documentada y testeada; no marca el archivo como almacenado.

## Architecture

```txt
src/modules/almacenamientoDocumental/
  types/almacenamientoDocumental.types.ts
    - DTOs frontend
    - AppResponse<T>
    - UploadStorageProgress
    - UploadOneDocumentInput/Result
    - Error codes

  utils/storageFile.utils.ts
    - normalizeFileExtension
    - getFileContentType
    - calculateTotalChunks
    - createStorageRequestId
    - sliceFileChunk
    - assertPositiveNumber/assertNonEmptyString helpers if not colocated

  services/almacenamientoDocumentalUpload.service.ts
    - endpoint builders
    - unwrap/guards
    - initTemporaryUpload
    - uploadTemporaryChunk
    - getTemporaryUploadStatus
    - completeTemporaryUpload
    - cancelTemporaryUpload
    - almacenarDocumento
    - uploadAndStoreOneDocument
```

## API Shape

Public service functions:

```ts
initTemporaryUpload(request, options?)
uploadTemporaryChunk(input, options?)
getTemporaryUploadStatus(input, options?)
completeTemporaryUpload(input, options?)
cancelTemporaryUpload(input, options?)
almacenarDocumento(request, options?)
uploadAndStoreOneDocument(input)
```

Options should accept `signal?: AbortSignal`.

`uploadAndStoreOneDocument` returns:

- `temporal`
- `response`
- `rawBackendResult?`

## Flow

1. Validate file and request input.
2. Calculate initial chunk plan.
3. POST `upload-temporal/init`.
4. Validate init response.
5. Recalculate chunks with backend `chunkSizeBytes`.
6. Emit `initializing` and `uploading` progress.
7. PUT each chunk as `Blob` with `application/octet-stream` and `X-Total-Chunks`.
8. POST `complete`.
9. Optionally GET `status` if needed by contract or tests.
10. POST `/api/gestor-documental/almacenamiento` with one document.
11. Validate final response.
12. Return normalized result and raw backend data.

## Error Handling

- If `init` fails, do not upload chunks.
- If a chunk fails, do not call `complete` or final storage.
- If `complete` fails, do not call final storage.
- If final storage fails, include request id and enough context for caller.
- If `signal.aborted`, throw/return `storage_aborted`.
- If response contract is invalid, throw `storage_contract_error`.

## Security and Privacy

- Do not log file bytes.
- Do not persist `File`, object URLs or temporal URLs outside runtime.
- Do not expose token data.
- Do not include full payloads in user-facing errors.
- Keep binary upload in memory slices via `Blob.slice`.

## Risks / Trade-offs

- **Backend DTOs unavailable locally**: Implementation must follow prompt contract and document any later DTO adjustment.
- **Envelope variance**: Existing endpoints use `success/data`; guards must avoid silently accepting corrupt data.
- **Chunk indexing ambiguity**: Local patterns use zero-based indexes. This change should use zero-based unless backend DTO evidence says otherwise, and document/test the choice.
- **Cancel best-effort**: Backend cancel may fail after abort; caller still must not treat the document as stored.
- **Duplicate local implementations**: Some upload helpers already exist. This ticket creates a reusable storage client rather than coupling to digitalizacion or AppVisorEmbedPdf-specific services.

## Migration Plan

1. Create module directories.
2. Add shared storage types and error class.
3. Add file/chunk utilities and tests.
4. Add storage service endpoints, unwrap helpers and guards.
5. Implement per-endpoint service functions.
6. Implement `uploadAndStoreOneDocument`.
7. Add service tests for success, errors, abort and cancel.
8. Add architecture/testing docs for SCRUMCORE-272.

## Open Questions

- Confirm exact backend DTO casing and extra fields when `D:\imagenesda\...` is available.
- Confirm whether `complete` requires a body or accepts `{}` for this controller.
- Confirm whether `status` is required in the main happy path or only exposed for callers.
- Confirm if chunk indexes are zero-based in the almacenamiento endpoint; local frontend precedent is zero-based.
