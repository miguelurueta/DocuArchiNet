## Why

`SCRUMCORE-272` implementa el cliente tecnico de almacenamiento documental que sera usado por `AppUploadDocumental` y por futuros flujos de carga. Hoy existen patrones parciales de upload temporal en digitalizacion y reemplazo de paginas PDF, pero no hay un servicio reusable de almacenamiento documental que cubra el flujo completo:

```txt
init -> chunks -> complete/status/cancel -> almacenar
```

El cliente debe quedar sin UI, sin dependencia legacy y con contratos estrictos para que la capa documental pueda procesar archivos de forma secuencial, reportar progreso, cancelar operaciones y preservar resultados backend para normalizacion posterior de eventos de interfaz.

## What Changes

- Crear `src/modules/almacenamientoDocumental/` con `services`, `types` y `utils`.
- Implementar `almacenamientoDocumentalUpload.service.ts` usando `clienteApi`.
- Implementar tipos frontend para init, chunk, status, complete, cancel, request final, response final, progreso, errores y resultado de upload.
- Implementar utilidades de archivo en `storageFile.utils.ts`: extension normalizada, content type fallback, chunk planning, slicing, request id y guards basicos.
- Implementar endpoint builders con `encodeURIComponent` para identificadores temporales.
- Implementar guards runtime para respuestas backend y envelope `AppResponses<T>` compatible con patrones locales.
- Implementar error tipado `AlmacenamientoDocumentalUploadError` con codigo, fase, detalle y request id cuando exista.
- Implementar `uploadAndStoreOneDocument` como orquestador reusable por archivo.
- Agregar pruebas unitarias de utils y servicio para flujo feliz, errores, cancelacion y preservacion de `rawBackendResult`.
- Documentar evidencia de contrato FE/BE y limites cuando los DTOs backend no esten disponibles localmente.

## Scope Boundaries

Este cambio NO crea componentes React, modales ni UI.

Este cambio NO implementa tipologias visuales, `AppUploadDocumental`, `AppUploadBatchView`, `AppProgressBatch` ni integraciones consumidoras.

Este cambio NO usa jQuery, `.ashx`, `XMLHttpRequest`, `FormData` legacy, callbacks globales, HTML por strings ni `any` nuevo.

Este cambio NO modifica backend ni inventa endpoints fuera de los definidos por el ticket.

## Capabilities

### New Capabilities

- `implementacion-componente-appupload-storage`: Cliente tecnico reusable para almacenamiento documental con upload temporal por chunks, complete/status/cancel, registro final por archivo, progreso, guards runtime, errores tipados y tests.

### Modified Capabilities

- `almacenamiento-documental`: Se introduce la base de servicios, tipos y utils frontend para almacenamiento documental.

## Impact

- Nuevo modulo frontend `src/modules/almacenamientoDocumental/`.
- Nuevo servicio reusable sobre `clienteApi`.
- Nuevos tipos y utilidades sin UI.
- Nuevas pruebas enfocadas.
- Base tecnica necesaria para `SCRUMCORE-271` (`AppUploadDocumental`) sin acoplar la UI al cliente de storage.
