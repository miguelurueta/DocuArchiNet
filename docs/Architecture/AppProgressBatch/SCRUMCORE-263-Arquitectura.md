# SCRUMCORE-263 - Arquitectura AppProgressBatch

## Objetivo

Crear `AppProgressBatch` como componente shared enterprise para orquestar procesos batch secuenciales genericos en React. El componente reemplaza conceptualmente la parte reusable del legacy `JSProgresBar.js`, pero no migra sus dependencias ni su acoplamiento a dominio.

## Contexto legacy

`JSProgresBar.js` resolvia progreso secuencial, cancelacion y confirmaciones mediante jQuery, Bootstrap manual, estado global, funciones globales, `name_service`, polling y codigos string ambiguos. La nueva solucion separa UI, ejecucion y dominio:

- UI React controlada.
- Contrato tipado por props.
- Operacion concreta inyectada por el consumidor.
- Cancelacion propagada con `AbortSignal`.
- Resumen operacional tipado.

## Alcance

Incluye:

- Componente `AppProgressBatch`.
- Tipos publicos genericos.
- UI modal con progreso global e item actual.
- Lifecycle explicito.
- Cancelacion segura.
- Errores controlados, advertencias, omitidos y errores fatales.
- Tests unitarios/integracion.
- README local y documentacion enterprise.

No incluye en la entrega shared original:

- Upload documental.
- Storage documental.
- Servicios de negocio.
- Endpoints.
- Backend.
- Integracion con consumidores existentes.
- Cambios en `AppUpload` o `AppUploadDocumental`.

## Dependencias permitidas

- React 19.
- TypeScript estricto.
- `AppModal` para contenedor modal.
- `AppButton` para acciones.
- `Progress` y `Alert` de Ant Design como elementos visuales compartidos disponibles.
- CSS Modules para estilos del componente.

## Separacion de responsabilidades

### UI State

Controla visibilidad, item actual, label, fase, progreso global, progreso de item, mensajes, confirmacion de cancelacion y resumen renderizado.

### Execution State

Controla lifecycle, `runId`, `AbortController`, indice actual, resumen acumulado, decision pendiente de error controlado y proteccion contra doble corrida.

### Consumer State

Permanece fuera del componente: items de negocio, implementacion de `processItem`, efectos laterales, endpoints y reaccion a `onComplete`, `onCancel` u `onError`.

## Lifecycle

```txt
idle -> running -> completed
idle -> running -> paused -> running -> completed
idle -> running -> paused -> cancelling -> completed(cancelled)
idle -> running -> cancelling -> completed(cancelled)
idle -> running -> error
```

- `idle`: listo para iniciar.
- `running`: procesa un item a la vez.
- `paused`: espera decision por `controlled-error`.
- `cancelling`: cancelacion solicitada y `AbortController.abort()` ejecutado.
- `completed`: proceso finalizado o cancelado con resumen visible.
- `error`: error fatal o resultado invalido.

## Run isolation

Cada ejecucion genera un `runId`. Las actualizaciones asincronas solo aplican si el `runId` coincide con la corrida activa. Cierre, cancelacion, unmount o nueva apertura invalidan la corrida anterior.

## Cancelacion

La cancelacion:

- pasa por politica de confirmacion si `confirmOnCancel=true`;
- llama `abortController.abort()`;
- impide procesar pendientes;
- emite `onCancel(summary)`;
- no emite `onComplete` como exito total;
- deja resumen parcial cancelado si el modal permanece abierto.

El componente no asume que el backend soporte rollback.

## Seguridad

- No loguea payloads de items.
- No persiste datos de negocio.
- No expone informacion sensible salvo labels provistos por el consumidor.
- No usa variables globales ni IDs fijos.

## Restricciones arquitectonicas

- Sin `any` nuevo.
- Sin jQuery.
- Sin Bootstrap manual.
- Sin timers para pausar.
- Sin services ni endpoints.
- Sin dominio documental, workflow, firmas, indices o upload.
- Sin ejecucion concurrente de items.
