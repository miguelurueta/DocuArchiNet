## Why

SCRUMCORE-271 implementa `AppUploadDocumental`, la especializacion documental que falta sobre los componentes UI ya existentes y el cliente tecnico de almacenamiento documental ya disponible.

El legacy `FileUploadHandler.js` resolvia seleccion multiple, validacion, tipologia por archivo, fecha documental, guardado individual, guardado masivo y refresco de interfaz, pero lo hacia con DOM manual, jQuery, Bootstrap, WebForms, callbacks por string, `.ashx`, `XMLHttpRequest` y `FormData`. La nueva implementacion debe conservar la semantica util sin migrar esos mecanismos.

## What Changes

- Crear `AppUploadDocumental` bajo `src/modules/almacenamientoDocumental/components/AppUploadDocumental/`.
- Componer `AppUploadBatchView` como vista base, `AppUpload` para seleccion/drag-drop y `AppProgressBatch` para guardado secuencial de lotes.
- Usar `almacenamientoDocumentalUpload.service` para `init -> chunks -> complete -> almacenar`, sin llamadas HTTP directas desde el componente.
- Exigir `loadConfig` y `loadTiposDocumentales` como loaders obligatorios mientras no exista endpoint canonico confirmado en el repo.
- Mantener metadata independiente por archivo: tipologia, fecha documental, errores, warnings, sugerencia y numero de paginas.
- Soportar `reject` y `queue-with-error` para archivos invalidos.
- Soportar guardar individual y guardar todos, con un `POST /api/gestor-documental/almacenamiento` por archivo.
- Crear mapper aislado `uploadDocumentalInterfaceRegistration.mapper.ts` para reemplazar `funcion_name` legacy por eventos discriminados.
- Crear utilidad pura `tipoDocumentalSuggestion.utils.ts`.
- Documentar README enterprise del componente con props, loaders, flujo, matriz FE/BE, errores, retry y limites.

## Jira Details

El detalle completo del ticket queda preservado en:

```txt
openspec/changes/scrumcore-271-crea-componente-appuploaddocumental/specs/crea-componente-appuploaddocumental/jira-context.md
```

## Capabilities

### New Capabilities

- `crea-componente-appuploaddocumental`: Componente documental reusable para seleccion, metadata, validacion, upload por chunks, registro final por archivo y emision de resultados tipados.

### Modified Capabilities

- `implementacion-componente-appupload-storage`: SCRUMCORE-271 consume el cliente tecnico implementado en SCRUMCORE-272, sin modificar endpoints de almacenamiento.

## Impact

- Nuevo componente/documentacion/tests en `src/modules/almacenamientoDocumental/components/AppUploadDocumental/`.
- Nuevos servicios de configuracion/tipologia solo como adaptadores/contratos, sin inventar endpoints.
- Nuevo mapper de registro de interfaz testeable.
- Nuevos tests unitarios/integracion/navegador focales.
- No se modifica backend.
- No se reemplazan `AppUpload`, `AppUploadBatchView` ni `AppProgressBatch`.
