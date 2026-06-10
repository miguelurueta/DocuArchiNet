# SCRUMCORE-242 Implementacion Detallada

## Servicios

Se agregaron servicios bajo `src/modules/digitalizacion/services`:

- `digitalizacionConfiguracion.api.ts`
- `digitalizacionListaChequeo.api.ts`
- `digitalizacionMetadata.api.ts`
- `digitalizacionUploadTemporal.api.ts`
- `digitalizacionDocumentos.api.ts`
- `adjuntarDigitalizacion.api.ts`

## Validacion Runtime

`digitalizacionApiClient.ts` centraliza:

- unwrap de `AppResponses<T>`.
- soporte de casing `success/data` y `Success/Data`.
- validacion de data obligatoria.
- validacion de IDs y strings requeridos.
- validacion de PDF.
- normalizacion de errores.

## Hooks

Se agregaron hooks reutilizables:

- `useDigitalizacionConfiguracion`
- `useDigitalizacionListaChequeo`
- `useDigitalizacionMetadataResolve`
- `useUploadTemporalPdf`
- `useCrearDocumentoDigitalizado`
- `useAdjuntarDigitalizacion`

`useDigitalizacionApiOperation` provee el patron comun de loading/data/error, cancelacion, stale protection y anti doble submit.

## Retry Strategy

Los hooks exponen `run`/acciones especificas y `cancel`. El retry se hace repitiendo la misma accion luego de que el estado deje de estar loading. La idempotencia final debe usar `RequestId` estable cuando backend lo confirme.

## Ownership

La capa API no reconstruye contexto ni scanner state. Recibe DTOs explicitos y retorna datos normalizados. El contenedor que integre submit debe construir request desde `DigitalizacionContext`, metadata resuelta y referencia temporal.
