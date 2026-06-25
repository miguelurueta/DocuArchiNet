# SCRUMCORE-270 - Arquitectura AppUploadBatchView

## Contexto

`AppUploadBatchView` cubre la capa visual reutilizable para colas de carga de archivos. El objetivo es conservar la ergonomia del legacy `FileUploadHandler.js` sin transportar dependencias legacy, IDs dinamicos, Bootstrap manual, jQuery ni reglas documentales.

## Alcance

- Crear componente shared en `src/app/Components/UI/AppUploadBatchView`.
- Componer `AppUpload` como selector controlado.
- Mostrar lista compacta de archivos, estado, progreso, errores, advertencias y acciones.
- Mostrar preview del archivo activo.
- Exponer slots para metadata, preview, nombre y footer.
- Exportar la API desde el barrel shared.

## No alcance

- No endpoints.
- No almacenamiento documental.
- No upload por chunks.
- No tipologias, TRD, radicado, workflow, expediente o gabinete.
- No cambios a `AppUpload`.
- No backend.

## Responsabilidades

`AppUploadBatchView` es responsable de renderizar UI, emitir eventos y manejar URLs temporales de preview. El consumidor mantiene el source-of-truth de archivos, metadata, validaciones, persistencia y progreso real.

## Composicion

- `AppUpload`: selector de archivos.
- `AppButton`: acciones globales y por fila.
- `Progress` de Ant Design: progreso por archivo activo.
- CSS module local: layout responsive y estados visuales.

## Decisiones

- La vista recibe `files` como `ReadonlyArray` para evitar mutaciones internas.
- El preview usa `previewUrl` si existe; si no existe, crea `URL.createObjectURL(file)` y lo revoca al cambiar o desmontar.
- El resumen se recibe por prop o se calcula desde la lista.
- Los estados de `completing` y `storing` se agregan al contador operacional `uploading`.
- Las acciones destructivas se bloquean para items `done` y para items deshabilitados por el consumidor.

## Riesgos controlados

- URLs temporales: limpieza en `useEffect`.
- Layout con nombres largos: truncado visual con `title`.
- Acoplamiento de dominio: metadata generica y slots.
- Regresion de AppUpload: composicion por props, sin modificar su fuente.
