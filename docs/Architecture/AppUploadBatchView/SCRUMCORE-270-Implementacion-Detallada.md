# SCRUMCORE-270 - Implementacion Detallada AppUploadBatchView

## Estructura Creada

```txt
src/app/Components/UI/AppUploadBatchView/
|-- AppUploadBatchView.tsx
|-- AppUploadBatchView.types.ts
|-- AppUploadBatchView.module.css
|-- AppUploadBatchView.test.tsx
|-- README.md
`-- index.ts
```

Tambien se actualizo:

```txt
src/app/Components/UI/index.ts
```

## Archivos y Responsabilidad

| Archivo | Responsabilidad |
| --- | --- |
| `AppUploadBatchView.tsx` | Render principal, composicion con `AppUpload`, eventos, preview y summary fallback. |
| `AppUploadBatchView.types.ts` | Contratos publicos genericos y tipados. |
| `AppUploadBatchView.module.css` | Layout enterprise, responsive, estados, acciones, preview y accesibilidad visual. |
| `AppUploadBatchView.test.tsx` | Tests unitarios e integracion del componente. |
| `README.md` | Guia de uso, contrato, ejemplos y limites. |
| `index.ts` | Export publico del componente y tipos. |
| `src/app/Components/UI/index.ts` | Barrel shared para importacion centralizada. |

## API Publica

El componente queda disponible desde:

```ts
import { AppUploadBatchView } from "src/app/Components/UI";
import type { AppUploadBatchFileItem } from "src/app/Components/UI";
```

Export local:

```ts
export { AppUploadBatchView } from "./AppUploadBatchView";
export type {
  AppUploadBatchFileItem,
  AppUploadBatchFileState,
  AppUploadBatchSummary,
  AppUploadBatchViewProps,
} from "./AppUploadBatchView.types";
```

## Contratos Implementados

### Estados de archivo

```ts
export type AppUploadBatchFileState =
  | "queued"
  | "validating"
  | "ready"
  | "uploading"
  | "completing"
  | "storing"
  | "done"
  | "warning"
  | "error"
  | "cancelled"
  | "removed";
```

### Item de archivo

```ts
export type AppUploadBatchFileItem<TMetadata = unknown> = {
  uid: string;
  file: File;
  name: string;
  size: number;
  extension: string;
  state: AppUploadBatchFileState;
  progress?: number;
  phaseLabel?: string;
  error?: string;
  warning?: string;
  metadata?: TMetadata;
  previewUrl?: string;
  selected?: boolean;
  disabled?: boolean;
};
```

### Summary operacional

```ts
export type AppUploadBatchSummary = {
  total: number;
  queued: number;
  ready: number;
  uploading: number;
  done: number;
  warning: number;
  error: number;
  cancelled: number;
};
```

### Props principales

La vista recibe `files`, `selectedUid`, flags de comportamiento, callbacks y render props. Todas las extensiones de negocio se modelan como slots o callbacks:

- `onFilesSelected`
- `onSelectFile`
- `onPreviewFile`
- `onRemoveFile`
- `onSaveFile`
- `onSaveAll`
- `onClearAll`
- `onClosePreview`
- `renderMetadata`
- `renderPreview`
- `renderFileName`
- `renderFooterExtra`

## Flujo de Render

1. La vista recibe props desde el consumidor.
2. Determina si esta deshabilitada con `disabled || loading`.
3. Calcula `effectiveSummary`:
   - usa `summary` si llega por props;
   - si no llega, ejecuta `buildSummary(files)`.
4. Resuelve `selectedItem`:
   - primero por `selectedUid`;
   - despues por `item.selected`;
   - finalmente por el primer item.
5. Calcula `previewUrl`:
   - si existe `selectedItem.previewUrl`, lo usa;
   - si no existe, crea object URL temporal desde `selectedItem.file`;
   - si no hay item activo, no crea URL.
6. Renderiza header, toolbar, uploader, lista, preview y footer.
7. Las acciones llaman callbacks; no mutan la lista internamente.
8. El cleanup revoca object URL local si fue generada por el componente.

## Integracion con AppUpload

`AppUploadBatchView` compone `AppUpload` como selector controlado. La vista no reemplaza `AppUpload` ni cambia su implementacion.

Configuracion aplicada:

```tsx
<AppUpload
  value={[]}
  layout="list"
  size="sm"
  strategy="manual"
  drag={drag}
  accept={accept}
  maxSize={maxSize}
  maxCount={multiple ? undefined : 1}
  disabled={!canAddFiles || isDisabled}
  beforeUpload={handleBeforeUpload}
  onChange={handleFilesChange}
/>
```

### Por que `value={[]}`

La cola visible no pertenece a `AppUpload`; pertenece al consumidor. `AppUploadBatchView` muestra `files` y usa `AppUpload` solo para seleccionar nuevos archivos. Esto evita dos listas paralelas.

### Por que `beforeUpload`

`beforeUpload` permite interceptar la seleccion nativa y emitir `onFilesSelected`. Se usa una condicion `fileList[0] === file` para emitir una sola vez por seleccion multiple y evitar duplicados.

### Fallback `onChange`

`onChange` transforma `originFile` a `File[]` como ruta secundaria compatible con el contrato existente de `AppUpload`.

## Calculo de Summary

Si el consumidor no entrega `summary`, se calcula con la lista actual:

- `total`: cantidad total de items.
- `queued`: estados `queued` y `validating`.
- `ready`: estado `ready`.
- `uploading`: estados `uploading`, `completing` y `storing`.
- `done`: estado `done`.
- `warning`: estado `warning`.
- `error`: estado `error`.
- `cancelled`: estado `cancelled`.

`removed` no incrementa contadores operacionales porque representa un item fuera del flujo activo.

## Preview

### PDF

Los archivos con extension o MIME compatible con PDF se renderizan con `iframe`.

### Imagen

Imagenes con MIME `image/*` o extension comun se renderizan con `img`.

### Otros formatos

Se muestra fallback con nombre, extension y tamano formateado.

### Preview Custom

`renderPreview` reemplaza el preview default:

```tsx
renderPreview={({ item, previewUrl, onClose }) => (
  <CustomPreview item={item} url={previewUrl} onClose={onClose} />
)}
```

### Limpieza de recursos

Cuando la vista crea object URL local, registra cleanup para ejecutar:

```ts
URL.revokeObjectURL(url);
```

Esto evita fugas de memoria al cambiar de archivo activo o desmontar.

## Slots de Extension

### `renderMetadata`

Permite insertar campos especializados por fila, por ejemplo tipologia, fecha documental, categoria o etiquetas. El componente base no interpreta esos datos.

### `renderFileName`

Permite controlar como se presenta el nombre del archivo sin perder estructura de fila ni acciones.

### `renderPreview`

Permite reemplazar PDF/imagen/fallback por un visor especializado.

### `renderFooterExtra`

Permite agregar informacion de negocio o acciones secundarias junto al resumen.

## Acciones Implementadas

### Globales

- Agregar archivos.
- Guardar todos.
- Limpiar todos.

### Por archivo

- Seleccionar/ver.
- Guardar individual cuando `canSaveOne=true`.
- Eliminar cuando no este bloqueado.

### Politica de habilitacion

Las acciones se deshabilitan cuando:

- `disabled=true`;
- `loading=true`;
- el flag `can*` correspondiente es `false`;
- no hay archivos para acciones globales;
- el item esta marcado como `disabled`;
- el item esta `done` y la accion destructiva no debe mostrarse como disponible.

## Estilos y Layout

El CSS module implementa:

- layout general tipo workbench;
- header compacto;
- toolbar de acciones;
- grilla lista/preview en desktop;
- apilamiento responsive en mobile;
- filas compactas;
- fila activa;
- badges de estado;
- warning/error inline;
- progreso por archivo;
- preview estable;
- footer con resumen;
- botones con dimensiones estables;
- truncado de nombres largos;
- foco visible.

No se usan estilos inline, Bootstrap manual ni cards anidadas.

## Accesibilidad Aplicada

- Raiz con `section` y `aria-label`.
- Lista con `role="list"`.
- Filas con `role="listitem"`.
- Boton principal de fila con `aria-pressed`.
- Botones iconograficos con `aria-label`.
- Footer con `aria-live="polite"`.
- Preview con `aside` y `aria-label`.
- Estados visibles con texto, no solo color.
- Foco visible por CSS.

## Ejemplo Basico

```tsx
const [files, setFiles] = useState<AppUploadBatchFileItem[]>([]);

<AppUploadBatchView
  title="Adjuntar archivos"
  files={files}
  selectedUid={files[0]?.uid}
  accept=".pdf,.png,.jpg"
  onFilesSelected={(selectedFiles) => {
    setFiles((current) => [
      ...current,
      ...selectedFiles.map((file) => ({
        uid: crypto.randomUUID(),
        file,
        name: file.name,
        size: file.size,
        extension: file.name.split(".").pop() ?? "",
        state: "queued",
      })),
    ]);
  }}
  onSelectFile={(uid) => {
    setFiles((current) =>
      current.map((item) => ({ ...item, selected: item.uid === uid })),
    );
  }}
/>
```

## Ejemplo con Metadata

```tsx
type DocumentMetadata = {
  documentTypeId?: string;
  documentDate?: string;
};

<AppUploadBatchView<DocumentMetadata>
  files={files}
  renderMetadata={({ item, disabled }) => (
    <DocumentMetadataFields
      value={item.metadata}
      disabled={disabled}
      onChange={(metadata) => updateMetadata(item.uid, metadata)}
    />
  )}
/>
```

## Limites Tecnicos

- No ejecuta upload.
- No valida reglas de negocio.
- No administra progreso real.
- No reemplaza `AppUpload`.
- No abre modales por si mismo.
- No conoce backend.
- No conoce dominio documental.

## Resultado de Implementacion

El componente queda listo como base shared enterprise. La especializacion documental debe ocurrir en un componente consumidor futuro, usando `files`, callbacks y slots sin modificar `AppUploadBatchView`.
