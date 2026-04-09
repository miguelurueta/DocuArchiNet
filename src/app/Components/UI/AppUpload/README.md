# AppUpload

Componente reusable para carga de archivos basado en Ant Design Upload, con estado controlado, estrategias de carga y UI configurable.

## Instalacion

```tsx
import { AppUpload } from "src/app/Components/UI";
```

## Props principales

- `value?: AppUploadFile[]` lista controlada de archivos.
- `defaultValue?: AppUploadFile[]` valor inicial (no controlado).
- `layout?: "grid" | "list"` layout de la lista.
- `accept?: string` filtros MIME/extensiones.
- `maxSize?: number` tamanio maximo en bytes.
- `validateFile?: (file) => boolean | Promise<boolean>` validacion custom.
- `maxCount?: number` limite de archivos.
- `drag?: boolean` habilita drag & drop.
- `strategy?: "auto" | "manual" | "customRequest"` estrategia de carga.
- `onChange(files)` notifica cambios de lista/estado.
- `onRemove(file)` elimina archivo.
- `onUpload()` dispara carga en modo manual.
- `onProgress(file, percent)` progreso 0-100.
- `onSuccess(file)` exito de carga.
- `onError(file, error)` error de carga.
- `onTelemetry(event)` eventos observabilidad.

## Eventos de telemetry

`onTelemetry` recibe:

```ts
type AppUploadTelemetryEvent = {
  type:
    | "select"
    | "upload_start"
    | "upload_success"
    | "upload_error"
    | "remove"
    | "preview_open"
    | "cancel";
  file?: AppUploadFile;
  timestamp: string;
  meta?: Record<string, unknown>;
};
```

## Ejemplos

### Estrategia auto

```tsx
<AppUpload
  strategy="auto"
  onChange={setFiles}
  customRequest={async (file, helpers) => {
    helpers.onProgress(50);
    helpers.onSuccess();
  }}
/>
```

### Estrategia manual

```tsx
<AppUpload
  strategy="manual"
  onChange={setFiles}
  onUpload={() => console.log("start")}
  customRequest={async (_file, helpers) => {
    helpers.onSuccess();
  }}
/>
```

### CustomRequest (presigned)

```tsx
<AppUpload
  strategy="customRequest"
  onChange={setFiles}
  customRequest={async (file, helpers) => {
    const uploadUrl = await getPresignedUrl(file);
    await uploadToUrl(uploadUrl, file.originFile);
    helpers.onSuccess();
  }}
/>
```
