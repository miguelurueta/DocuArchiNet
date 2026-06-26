# SCRUMCORE-272 - Pruebas

Fecha: 2026-06-25

## Suite enfocada

Comando ejecutado:

```txt
npx.cmd vitest run src/modules/almacenamientoDocumental/utils/storageFile.utils.test.ts src/modules/almacenamientoDocumental/services/almacenamientoDocumentalUpload.service.test.ts
```

Resultado:

```txt
Test Files 2 passed
Tests 22 passed
```

Cobertura funcional:

- extension normalizada y archivos sin extension;
- calculo de chunks, sizes invalidos y bounds de slice;
- init con payload esperado y shape invalido;
- chunk con bytes crudos y headers `Content-Type`/`X-Total-Chunks`;
- status, complete, cancel y store;
- flujo `init -> chunks -> complete -> store`;
- recalc de chunks con `chunkSizeBytes` backend;
- no-store si falla chunk;
- no-store si falla complete;
- abort antes de init;
- abort despues de init con `DELETE` temporal;
- abort conserva el error principal si falla el cleanup temporal;
- preservacion de `rawBackendResult`;
- progreso por fases;
- requestId desde envelope;
- ausencia de endpoints legacy.

## Lint enfocado

Comando ejecutado:

```txt
npx.cmd eslint src/modules/almacenamientoDocumental
```

Resultado: sin errores.

## Busqueda de prohibidos en codigo productivo

Comando ejecutado:

```txt
rg -n "\bany\b|XMLHttpRequest|FormData|\.ashx|jquery|jQuery|fetch\(|React|AppUploadDocumental|funcion_name" src/modules/almacenamientoDocumental -g "!*.test.ts"
```

Resultado: sin coincidencias.
