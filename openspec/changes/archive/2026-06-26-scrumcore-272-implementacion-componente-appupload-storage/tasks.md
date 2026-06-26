## 1. Refinement

- [x] 1.1 Consolidar alcance final desde Jira, docs de AppUploadDocumental y patrones locales de upload temporal.
- [x] 1.2 Confirmar que los DTOs backend externos `D:\imagenesda\...` no estan disponibles en este workspace.
- [x] 1.3 Corregir proposal con capability `implementacion-componente-appupload-storage` y alcance real.
- [x] 1.4 Refinar design con decisiones, arquitectura, riesgos, flujo, errores y plan.
- [x] 1.5 Reescribir spec como requisitos verificables en lugar de prompt Jira pegado.
- [x] 1.6 Validar OpenSpec estricto antes de publish/implementacion.
- [x] 1.7 Antes de implementar, si los paths backend `D:\imagenesda\...` estan disponibles, leer controller/DTOs reales y ajustar tipos/documentacion a esa evidencia.

## 2. Module Scaffold

- [x] 2.1 Crear `src/modules/almacenamientoDocumental/services/`.
- [x] 2.2 Crear `src/modules/almacenamientoDocumental/types/`.
- [x] 2.3 Crear `src/modules/almacenamientoDocumental/utils/`.
- [x] 2.4 Crear archivos de tests para servicio y utils.
- [x] 2.5 Verificar que el modulo no cree componentes React, modales, layout, manejo visual de tipologias ni dependencias de UI.
- [x] 2.6 Verificar que no se use jQuery, WebForms, `.ashx`, `XMLHttpRequest`, `fetch` directo ni `FormData` legacy.

## 3. Types and Error Model

- [x] 3.1 Definir `AppResponse<T>` compatible con envelopes locales `success/data/message/errors/meta`.
- [x] 3.2 Definir DTOs frontend: init, status, complete, cancel, documento entrada, request final y response final.
- [x] 3.3 Definir `UploadStorageProgress`, `UploadOneDocumentInput` y `UploadOneDocumentResult`.
- [x] 3.4 Definir codigos de error por fase.
- [x] 3.5 Implementar `AlmacenamientoDocumentalUploadError` con code, phase, message, requestId y cause.
- [x] 3.6 Garantizar `unknown` para datos no modelados y cero `any` nuevos.
- [x] 3.7 Modelar explicitamente `StorageUploadInitRequest`, `StorageUploadInitResponse`, `DocumentoEntrada`, `AlmacenarDocumentoRequest` y `AlmacenarDocumentoResponse` segun el prompt o DTO real validado.
- [x] 3.8 Incluir `storage_contract_error`, `storage_init_error`, `storage_chunk_error`, `storage_complete_error`, `storage_cancel_error`, `storage_store_error` y `storage_aborted`; agregar codigos extra solo si se documentan.
- [x] 3.9 No ocultar errores de contrato: toda respuesta invalida debe fallar con error tipado y trazable.

## 4. Storage Utils

- [x] 4.1 Implementar normalizacion de extension.
- [x] 4.2 Implementar content type fallback.
- [x] 4.3 Implementar calculo de total chunks con validacion de numeros positivos.
- [x] 4.4 Implementar slicing de chunks con `Blob.slice`.
- [x] 4.5 Implementar generacion de `requestId`.
- [x] 4.6 Implementar helpers/guards reutilizables si no se colocan en el servicio.

## 5. Service Implementation

- [x] 5.1 Definir `ALMACENAMIENTO_DOCUMENTAL_ENDPOINTS`.
- [x] 5.2 Codificar ids temporales con `encodeURIComponent`.
- [x] 5.3 Implementar `unwrapStorageResponse` y guards de respuesta.
- [x] 5.4 Implementar `initTemporaryUpload`.
- [x] 5.5 Implementar `uploadTemporaryChunk` con body binario, `Content-Type: application/octet-stream` y `X-Total-Chunks`.
- [x] 5.6 Implementar `getTemporaryUploadStatus`.
- [x] 5.7 Implementar `completeTemporaryUpload`.
- [x] 5.8 Implementar `cancelTemporaryUpload`.
- [x] 5.9 Implementar `almacenarDocumento`.
- [x] 5.10 Implementar `uploadAndStoreOneDocument` con progreso, recalc de chunks backend, abort y cleanup.
- [x] 5.11 Usar exclusivamente `clienteApi` para llamadas HTTP y pasar `signal` de Axios cuando exista.
- [x] 5.12 Exponer la API publica del prompt: `initTemporaryUpload`, `uploadTemporaryChunk`, `completeTemporaryUpload`, `cancelTemporaryUpload`, `almacenarDocumento` y `uploadAndStoreOneDocument`.
- [x] 5.13 Construir el payload final por archivo con un solo `DocumentoEntrada`, `rutaTemporalId`, `requestId` y metadata documental disponible.
- [x] 5.14 Preservar campos backend adicionales relevantes como `rawBackendResult?: unknown` sin interpretarlos para interfaz.
- [x] 5.15 No concatenar campos con `|`, no transformar por `funcion_name`, no llamar callbacks de interfaz y no importar tipos de `AppUploadDocumental`.

## 6. Failure and Cancellation Behavior

- [x] 6.1 No llamar chunks si falla init.
- [x] 6.2 No llamar complete ni store si falla un chunk.
- [x] 6.3 No llamar store si falla complete.
- [x] 6.4 Intentar cancel temporal si abort ocurre despues de init.
- [x] 6.5 Propagar errores tipados por fase.
- [x] 6.6 Preservar `requestId` cuando venga en envelope.
- [x] 6.7 Preservar `rawBackendResult` para final storage.
- [x] 6.8 Reportar progreso con fases `initializing`, `uploading`, `completing` y `storing`, percent 0-100 y datos de chunk cuando aplique.
- [x] 6.9 Si backend retorna `chunkSizeBytes` diferente, recalcular `totalChunks` real antes de subir chunks.
- [x] 6.10 No leer el archivo completo en memoria cuando `Blob.slice` permita partirlo.

## 7. Tests

- [x] 7.1 Tests de extension normalizada y archivos sin extension.
- [x] 7.2 Tests de chunk count, invalid sizes y slice bounds.
- [x] 7.3 Tests de init con payload y guard invalido.
- [x] 7.4 Tests de chunk con bytes crudos y headers esperados.
- [x] 7.5 Tests de status, complete, cancel y store.
- [x] 7.6 Tests de `uploadAndStoreOneDocument` happy path `init -> chunks -> complete -> store`.
- [x] 7.7 Tests de recalc con `chunkSizeBytes` backend.
- [x] 7.8 Tests de no-store cuando falla chunk o complete.
- [x] 7.9 Tests de abort antes/despues de init y cancel best-effort.
- [x] 7.10 Tests de preservacion de `rawBackendResult`.
- [x] 7.11 Tests de progreso por fases `initializing`, `uploading`, `completing` y `storing`.
- [x] 7.12 Tests de `requestId` preservado desde data/meta/errors del envelope.
- [x] 7.13 Tests de ausencia de llamadas legacy: no `FormData`, no `XMLHttpRequest`, no `.ashx` en endpoints.

## 8. Documentation

- [x] 8.1 Crear documentacion enterprise `docs/Architecture/AppUploadDocumental/SCRUMCORE-272-*` o ubicacion equivalente.
- [x] 8.2 Documentar matriz FE/BE basada en prompt y evidencia disponible.
- [x] 8.3 Documentar que DTOs externos no estaban accesibles en este workspace si no se validan durante implementacion.
- [x] 8.4 Confirmar explicitamente: backend no modificado, endpoints no modificados, cliente sin UI, sin `.ashx`, sin `FormData` legacy, sin `any`.
- [x] 8.5 Confirmar explicitamente: no se loguean tokens, bytes de archivo ni payload sensible; no se persisten URLs temporales ni `File` en storage global.
- [x] 8.6 Documentar si `chunkIndex` queda base cero o base uno segun contrato backend real; si no hay evidencia, usar y testear el precedente local base cero.

## 9. Verification and Publish Readiness

- [x] 9.1 Ejecutar `npx.cmd openspec validate scrumcore-272-implementacion-componente-appupload-storage --strict`.
- [x] 9.2 Ejecutar suite enfocada de tests nuevos.
- [x] 9.3 Ejecutar lint/TypeScript enfocado o documentar deuda no relacionada.
- [x] 9.4 Revisar `git diff --stat` para confirmar alcance.
- [x] 9.5 Commit de refinamiento OpenSpec antes de publish.
