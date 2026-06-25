# AppUploadBatchView

`AppUploadBatchView` es una vista shared reusable para representar colas de carga de archivos sin conocer dominio de negocio. Compone `AppUpload` como selector, muestra lista, preview, resumen, acciones globales y acciones por archivo.

## Objetivo

- Mantener una UI enterprise consistente para cargas por lote.
- Delegar la lista canonica, metadata, validaciones y persistencia al consumidor.
- Permitir especializaciones como `AppUploadDocumental`, anexos, evidencias, reemplazo PDF o importaciones.

## Contrato principal

```tsx
<AppUploadBatchView
  title="Adjuntar documentos"
  files={files}
  selectedUid={selectedUid}
  accept=".pdf,.tif"
  onFilesSelected={(nextFiles) => addFiles(nextFiles)}
  onSelectFile={(uid) => setSelectedUid(uid)}
  onRemoveFile={(uid) => removeFile(uid)}
  onSaveAll={() => saveAll()}
/>
```

La vista recibe `files` como `ReadonlyArray<AppUploadBatchFileItem<TMetadata>>`. Cada item contiene `uid`, `file`, `name`, `size`, `extension`, `state`, progreso opcional, mensajes opcionales y metadata generica.

Estados soportados:

- `queued`
- `validating`
- `ready`
- `uploading`
- `completing`
- `storing`
- `done`
- `warning`
- `error`
- `cancelled`
- `removed`

## Slots

`renderMetadata` permite insertar metadata por fila sin acoplar la vista:

```tsx
renderMetadata={({ item, disabled }) => (
  <MyMetadataEditor value={item.metadata} disabled={disabled} />
)}
```

`renderPreview` permite reemplazar el preview default:

```tsx
renderPreview={({ item, previewUrl, onClose }) => (
  <MyPreview item={item} url={previewUrl} onClose={onClose} />
)}
```

`renderFileName` personaliza el nombre visible y `renderFooterExtra` agrega contenido al footer.

## Preview

La vista genera un object URL local cuando el item activo no trae `previewUrl`. El URL se revoca al cambiar el archivo activo o desmontar el componente. El preview default soporta PDF, imagen y fallback para otros formatos.

## Limites

- No llama endpoints.
- No implementa almacenamiento documental.
- No valida reglas TRD, tipologias, workflow ni radicado.
- No reemplaza `AppUpload`.
- No modifica backend.
- No usa jQuery, Bootstrap manual, HTML por strings ni variables globales.

## Relacion con AppUploadDocumental

`AppUploadDocumental` puede usar esta vista como capa visual y conservar por fuera la metadata documental, validaciones, carga por chunks y persistencia. Esta separacion evita mezclar negocio documental con layout compartido.
