# SCRUMCORE-270 - Implementacion Detallada

## Estructura

```txt
src/app/Components/UI/AppUploadBatchView/
├─ AppUploadBatchView.tsx
├─ AppUploadBatchView.types.ts
├─ AppUploadBatchView.module.css
├─ AppUploadBatchView.test.tsx
├─ README.md
└─ index.ts
```

## Contrato

El contrato publico expone:

- `AppUploadBatchFileState`
- `AppUploadBatchFileItem<TMetadata>`
- `AppUploadBatchSummary`
- `AppUploadBatchViewProps<TMetadata>`

No se introduce `any`; metadata desconocida usa `unknown` o generico `TMetadata`.

## Flujo de render

1. El consumidor entrega `files`.
2. La vista resuelve el item activo por `selectedUid`, `selected` o primer item.
3. La vista calcula resumen si no se entrega `summary`.
4. `AppUpload` emite seleccion y la vista transforma `originFile` a `File[]`.
5. La lista renderiza filas con estado, tamano, warning/error, progreso y acciones.
6. El preview renderiza PDF, imagen, fallback o `renderPreview`.
7. El footer muestra resumen y `renderFooterExtra`.

## Eventos

- `onFilesSelected(files)`
- `onSelectFile(uid)`
- `onPreviewFile(uid)`
- `onRemoveFile(uid)`
- `onSaveFile(uid)`
- `onSaveAll()`
- `onClearAll()`
- `onClosePreview()`

## Slots

- `renderMetadata`: metadata por fila.
- `renderPreview`: preview custom.
- `renderFileName`: nombre custom.
- `renderFooterExtra`: contenido adicional del footer.

## Preview

PDF usa `iframe`. Imagen usa `img`. Otros formatos muestran extension y tamano. Si no hay `previewUrl`, la vista crea object URL local y lo revoca en cleanup.

## Responsive

Desktop usa dos columnas: lista y preview. Mobile apila las secciones y mantiene botones con dimensiones estables. No hay cards anidadas ni estilos inline.

## Accesibilidad

- Botones iconograficos con `aria-label`.
- Lista con `role="list"` y filas con `role="listitem"`.
- Resumen con `aria-live="polite"`.
- Preview con `aria-label`.
- Foco visible en fila seleccionable.
