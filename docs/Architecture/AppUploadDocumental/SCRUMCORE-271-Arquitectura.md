# SCRUMCORE-271 - Arquitectura AppUploadDocumental

## Objetivo

`AppUploadDocumental` es la especializacion documental enterprise para seleccion multiple, metadata por archivo, procesamiento secuencial y almacenamiento final usando el cliente tecnico entregado en SCRUMCORE-272.

El componente conserva capacidades funcionales del legacy `FileUploadHandler.js` sin migrar su UI ni sus dependencias legacy.

## Capas

```txt
AppUploadDocumental.tsx
  -> useAppUploadDocumentalState
  -> useAppUploadDocumentalActions
  -> AppUploadBatchView
       -> AppUpload
       -> renderMetadata
       -> preview base
  -> AppProgressBatch
  -> uploadAndStoreOneDocument
  -> buildUploadDocumentalInterfaceRegistration
```

## Componentes base

- `AppUploadBatchView`: vista base de cola, toolbar, preview, metadata y acciones.
- `AppUpload`: seleccion y drag/drop dentro de `AppUploadBatchView`.
- `AppProgressBatch`: proceso secuencial para guardar todos.
- `AppInputSelect`: tipologia por archivo.
- `AppInput`: fecha documental por archivo.
- `AppButton`: acciones globales y por fila desde la vista base.

## Source of truth

- Configuracion: `loadConfig`.
- Tipologias: `loadTiposDocumentales`.
- Cola y metadata: estado React del componente.
- Persistencia: backend storage invocado por `almacenamientoDocumentalUpload.service`.
- Registro visual consumidor: eventos tipados de `uploadDocumentalInterfaceRegistration.mapper.ts`.

## Reglas principales

- `loadConfig` y `loadTiposDocumentales` son obligatorios porque no hay endpoint canonico confirmado en el repo.
- Cada archivo se almacena con un request final independiente.
- `trd` se arma por archivo, nunca con tipologias mezcladas.
- La UI no importa `clienteApi`.
- La UI no usa `.ashx`, `XMLHttpRequest`, `FormData`, jQuery, Bootstrap manual ni WebForms.
- `rawBackendResult` se preserva para consumidores que necesiten datos no modelados.

## Flujo

1. Montaje valida `context.nombreGabinete`.
2. Se cargan config y tipologias.
3. Se habilita seleccion.
4. Se normaliza archivo, extension, uid y metadata.
5. Se valida extension/tamano con config.
6. Se sugiere tipologia por nombre si aplica.
7. Usuario ajusta tipologia y fecha.
8. Guardar individual o todos valida metadata.
9. Se ejecuta `uploadAndStoreOneDocument`.
10. El storage client ejecuta `init -> chunks -> complete -> almacenar`.
11. Se normaliza resultado en `AlmacenarDocumentoStoredResult`.
12. Se construyen eventos `UploadDocumentalInterfaceRegistration`.
13. Se emiten `onStored`, `onInterfaceRegistration`, `onBatchComplete` y `onError`.

## Anti-stale

El hook de acciones aborta controladores activos y limpia lote/resultados cuando cambia el `operationId`, derivado del ciclo de carga de contexto/config. Esto evita emitir resultados para un proceso, gabinete o modo documental obsoleto.

## Evidencia backend

Las rutas externas indicadas en el prompt bajo `D:\imagenesda\...` no estuvieron accesibles desde este workspace. La implementacion se apoya en el contrato local validado por SCRUMCORE-272:

- `src/modules/almacenamientoDocumental/types/almacenamientoDocumental.types.ts`
- `src/modules/almacenamientoDocumental/services/almacenamientoDocumentalUpload.service.ts`

No se modifico backend.
