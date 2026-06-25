## Why

`SCRUMCORE-270` crea `AppUploadBatchView`, una vista shared reusable para cargas por lote. Hoy `AppUpload` cubre seleccion, drag and drop y lista basica, pero no existe una vista enterprise que represente una cola operacional con preview activo, acciones por archivo, acciones globales, estados por fila y slots de metadata. Sin esta capa, `AppUploadDocumental` mezclaría reglas documentales con layout y seria dificil reutilizar la experiencia en otros flujos.

El cambio moderniza la parte visual util del legacy `FileUploadHandler.js` sin portar jQuery, Bootstrap manual, IDs dinamicos, tablas HTML manuales, callbacks globales ni dependencias WebForms.

## What Changes

- Crear `src/app/Components/UI/AppUploadBatchView/` con componente, tipos, estilos, tests, README e `index.ts`.
- Exportar `AppUploadBatchView` desde `src/app/Components/UI/index.ts`.
- Componer el `AppUpload` existente como zona de seleccion de archivos; no reemplazarlo ni modificar su contrato.
- Definir contrato generico sin dominio para `files`, `summary`, estados, slots/render props y callbacks.
- Renderizar un workbench compacto con header, toolbar, selector, lista de archivos, preview activo y footer/resumen.
- Soportar acciones globales: agregar, guardar todos, limpiar todos.
- Soportar acciones por archivo: seleccionar/ver, eliminar, guardar individual cuando este habilitado.
- Soportar estados visuales: `queued`, `validating`, `ready`, `uploading`, `completing`, `storing`, `done`, `warning`, `error`, `cancelled`, `removed`.
- Soportar preview default para PDF/imagenes/fallback y preview custom por `renderPreview`.
- Soportar metadata por fila mediante `renderMetadata` sin conocer tipologia, TRD, gabinete, workflow, radicado ni almacenamiento.
- Agregar pruebas unitarias/integracion de render, eventos, slots, preview, accesibilidad basica y object URL cleanup.

## Scope Boundaries

Este cambio NO implementa almacenamiento documental, endpoints, upload por chunks, tipologias, TRD, validacion documental, loaders de configuracion, backend ni integracion en pantallas consumidoras. Es una vista shared controlada por props.

## Capabilities

### New Capabilities

- `crea-componente-appuploadbatchview`: Vista shared reusable para cargas por lote con lista, preview, acciones, estados, slots y accesibilidad.

### Modified Capabilities

- `ui-shared-components`: Se agrega export publico desde `src/app/Components/UI/index.ts`.

## Impact

- Nuevo componente compartido en `src/app/Components/UI/AppUploadBatchView/`.
- Nuevas pruebas enfocadas del componente.
- Nueva documentacion local del componente.
- Base lista para `AppUploadDocumental`, cargas de anexos, evidencias, reemplazo PDF e importaciones.

## Out Of Scope

- Modificar `AppUpload`.
- Modificar backend o endpoints.
- Consumir `clienteApi`.
- Crear servicios documentales.
- Integrar con `AppUploadDocumental` en este ticket.
- Migrar HTML legacy literalmente.
