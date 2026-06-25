## 1. Refinement

- [x] 1.1 Consolidar alcance final desde Jira, docs de AppUploadDocumental y patrones locales de upload temporal.
- [x] 1.2 Confirmar que los DTOs backend externos `D:\imagenesda\...` no estan disponibles en este workspace.
- [x] 1.3 Corregir proposal con capability `implementacion-componente-appupload-storage` y alcance real.
- [x] 1.4 Refinar design con decisiones, arquitectura, riesgos, flujo, errores y plan.
- [x] 1.5 Reescribir spec como requisitos verificables en lugar de prompt Jira pegado.
- [x] 1.6 Validar OpenSpec estricto antes de publish/implementacion.

## 2. Module Scaffold

- [ ] 2.1 Crear `src/modules/almacenamientoDocumental/services/`.
- [ ] 2.2 Crear `src/modules/almacenamientoDocumental/types/`.
- [ ] 2.3 Crear `src/modules/almacenamientoDocumental/utils/`.
- [ ] 2.4 Crear archivos de tests para servicio y utils.

## 3. Types and Error Model

- [ ] 3.1 Definir `AppResponse<T>` compatible con envelopes locales `success/data/message/errors/meta`.
- [ ] 3.2 Definir DTOs frontend: init, status, complete, cancel, documento entrada, request final y response final.
- [ ] 3.3 Definir `UploadStorageProgress`, `UploadOneDocumentInput` y `UploadOneDocumentResult`.
- [ ] 3.4 Definir codigos de error por fase.
- [ ] 3.5 Implementar `AlmacenamientoDocumentalUploadError` con code, phase, message, requestId y cause.
- [ ] 3.6 Garantizar `unknown` para datos no modelados y cero `any` nuevos.

## 4. Storage Utils

- [ ] 4.1 Implementar normalizacion de extension.
- [ ] 4.2 Implementar content type fallback.
- [ ] 4.3 Implementar calculo de total chunks con validacion de numeros positivos.
- [ ] 4.4 Implementar slicing de chunks con `Blob.slice`.
- [ ] 4.5 Implementar generacion de `requestId`.
- [ ] 4.6 Implementar helpers/guards reutilizables si no se colocan en el servicio.

## 5. Service Implementation

- [ ] 5.1 Definir `ALMACENAMIENTO_DOCUMENTAL_ENDPOINTS`.
- [ ] 5.2 Codificar ids temporales con `encodeURIComponent`.
- [ ] 5.3 Implementar `unwrapStorageResponse` y guards de respuesta.
- [ ] 5.4 Implementar `initTemporaryUpload`.
- [ ] 5.5 Implementar `uploadTemporaryChunk` con body binario, `Content-Type: application/octet-stream` y `X-Total-Chunks`.
- [ ] 5.6 Implementar `getTemporaryUploadStatus`.
- [ ] 5.7 Implementar `completeTemporaryUpload`.
- [ ] 5.8 Implementar `cancelTemporaryUpload`.
- [ ] 5.9 Implementar `almacenarDocumento`.
- [ ] 5.10 Implementar `uploadAndStoreOneDocument` con progreso, recalc de chunks backend, abort y cleanup.

## 6. Failure and Cancellation Behavior

- [ ] 6.1 No llamar chunks si falla init.
- [ ] 6.2 No llamar complete ni store si falla un chunk.
- [ ] 6.3 No llamar store si falla complete.
- [ ] 6.4 Intentar cancel temporal si abort ocurre despues de init.
- [ ] 6.5 Propagar errores tipados por fase.
- [ ] 6.6 Preservar `requestId` cuando venga en envelope.
- [ ] 6.7 Preservar `rawBackendResult` para final storage.

## 7. Tests

- [ ] 7.1 Tests de extension normalizada y archivos sin extension.
- [ ] 7.2 Tests de chunk count, invalid sizes y slice bounds.
- [ ] 7.3 Tests de init con payload y guard invalido.
- [ ] 7.4 Tests de chunk con bytes crudos y headers esperados.
- [ ] 7.5 Tests de status, complete, cancel y store.
- [ ] 7.6 Tests de `uploadAndStoreOneDocument` happy path `init -> chunks -> complete -> store`.
- [ ] 7.7 Tests de recalc con `chunkSizeBytes` backend.
- [ ] 7.8 Tests de no-store cuando falla chunk o complete.
- [ ] 7.9 Tests de abort antes/despues de init y cancel best-effort.
- [ ] 7.10 Tests de preservacion de `rawBackendResult`.

## 8. Documentation

- [ ] 8.1 Crear documentacion enterprise `docs/Architecture/AppUploadDocumental/SCRUMCORE-272-*` o ubicacion equivalente.
- [ ] 8.2 Documentar matriz FE/BE basada en prompt y evidencia disponible.
- [ ] 8.3 Documentar que DTOs externos no estaban accesibles en este workspace si no se validan durante implementacion.
- [ ] 8.4 Confirmar explicitamente: backend no modificado, endpoints no modificados, cliente sin UI, sin `.ashx`, sin `FormData` legacy, sin `any`.

## 9. Verification and Publish Readiness

- [ ] 9.1 Ejecutar `npx.cmd openspec validate scrumcore-272-implementacion-componente-appupload-storage --strict`.
- [ ] 9.2 Ejecutar suite enfocada de tests nuevos.
- [ ] 9.3 Ejecutar lint/TypeScript enfocado o documentar deuda no relacionada.
- [ ] 9.4 Revisar `git diff --stat` para confirmar alcance.
- [ ] 9.5 Commit de refinamiento OpenSpec antes de publish.
