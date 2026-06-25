## Context

`SCRUMCORE-270` implementa `AppUploadBatchView` como una vista base reusable para experiencias de carga de archivos. El componente debe ubicarse en la capa shared UI y ser agnostico al dominio. El consumidor conserva el estado canonico de archivos, metadata, validaciones y resultados; la vista renderiza ese estado y emite eventos.

Referencias obligatorias:

- `docs/Architecture/AppUploadDocumental/AppUploadDocumental-Requisitos.md`
- `docs/Architecture/AppUploadDocumental/Legacy-Gap-Analysis.md`
- `docs/Architecture/AppUploadDocumental/Matriz-Migracion-Legacy-AppUploadDocumental-AppProgressBatch.md`
- `docs/Architecture/AppUploadDocumental/PROMPT-IMPLEMENTACION-AppUploadBatchView.md`
- `src/app/Components/UI/AppUpload/AppUpload.tsx`
- `src/app/Components/UI/AppButton/AppButton.tsx`
- `src/app/Components/UI/index.ts`

## Goals / Non-Goals

**Goals**

- Crear una vista shared reusable y tipada para cargas por lote.
- Componer `AppUpload` para seleccion de archivos.
- Mostrar cola compacta, fila activa, estados, errores, warning, progreso y resumen.
- Mostrar preview activo con soporte default y slot custom.
- Exponer slots de metadata por archivo y footer extra.
- Exponer acciones globales y por archivo mediante callbacks.
- Cumplir accesibilidad basica y layout responsive enterprise.
- Probar eventos principales, slots y sincronizacion lista/preview.

**Non-Goals**

- No implementar almacenamiento documental.
- No llamar endpoints ni `clienteApi`.
- No cargar tipologias ni validar TRD.
- No conocer `nombreGabinete`, expediente, workflow, radicado ni campos documentales.
- No implementar upload por chunks.
- No modificar `AppUpload`.
- No integrar aun `AppUploadDocumental`.
- No migrar jQuery, Bootstrap, WebForms, HTML por strings ni callbacks globales legacy.

## Decisions

1. **Componente controlado por props.** `files`, `selectedUid`, `summary`, estados y metadata pertenecen al consumidor. `AppUploadBatchView` solo mantiene estado UI derivado, principalmente object URLs de preview cuando sean necesarios.

2. **`AppUpload` se usa solo como selector.** La lista operacional propia del batch se renderiza en `AppUploadBatchView` para permitir metadata por fila, acciones y preview activo. `AppUpload` no se reemplaza ni se cambia.

3. **Metadata por slot.** `renderMetadata` recibe `{ item, disabled }` y puede renderizar selects, fechas o cualquier UI de consumidor. La vista no interpreta esos datos.

4. **Preview desacoplado.** La vista provee preview default para PDF/imagenes/fallback. `renderPreview` puede reemplazarlo para visores especializados. Los object URLs se revocan al cambiar el archivo activo o desmontar.

5. **Acciones declarativas.** `canSaveAll`, `canClearAll`, `canAddFiles`, `canPreview`, `canSaveOne` y `disabled/loading` gobiernan la disponibilidad visual. La vista no decide reglas de negocio.

6. **Estados visuales sin semantica de negocio.** Los estados admitidos son genericos: `queued`, `validating`, `ready`, `uploading`, `completing`, `storing`, `done`, `warning`, `error`, `cancelled`, `removed`.

7. **Diseño operativo.** La UI debe ser densa y legible: header/resumen, toolbar, selector, lista + preview, footer. No usar hero, cards anidadas ni decoracion de marketing.

8. **Sin `any`.** El metadata generico usa `unknown` por defecto y genericos tipados por consumidor.

## Component Contract

Archivos esperados:

```txt
src/app/Components/UI/AppUploadBatchView/
├─ AppUploadBatchView.tsx
├─ AppUploadBatchView.types.ts
├─ AppUploadBatchView.module.css
├─ AppUploadBatchView.test.tsx
├─ README.md
└─ index.ts
```

Barrel:

```txt
src/app/Components/UI/index.ts
```

Tipos publicos minimos:

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

## Layout

Desktop:

```txt
Header: titulo, descripcion, resumen compacto
Toolbar: agregar, guardar todos, limpiar todos
Selector: AppUpload
Body: lista de archivos | preview activo
Footer: resumen, errores agregados, slot extra
```

Mobile:

```txt
Header
Toolbar
Selector
Preview activo
Lista de archivos
Footer
```

Reglas visuales:

- No usar cards anidadas.
- No usar secciones flotantes decorativas.
- Mantener dimensiones estables para botones e iconos.
- Truncar nombres largos con `title`.
- Mostrar tamaño formateado.
- Mostrar estados con texto o badge accesible.
- Mostrar errores/warnings inline por archivo.
- La fila activa debe ser visible y no depender solo de color.

## Accessibility

- Botones iconograficos con `aria-label`.
- Resumen con `aria-live="polite"`.
- Lista con roles apropiados (`list`/`listitem` o botones por fila).
- Errores visibles y asociados al archivo.
- Foco visible y navegacion por teclado razonable.
- Preview con `title`/`aria-label` descriptivo.

## Risks / Trade-offs

- **Duplicidad visual con lista interna de `AppUpload`:** se mitiga usando `AppUpload` como selector compacto y lista operacional propia.
- **Object URLs filtrados:** se mitiga con cleanup en cambio de preview y unmount, cubierto por tests.
- **Demasiada responsabilidad visual:** se evita manteniendo negocio fuera y usando slots para metadata/preview.
- **Acciones destructivas durante carga:** la vista expone `disabled/loading/item.disabled`; el consumidor define la politica.
- **Nombres largos y metadata compleja:** se requiere layout con grid/flex estable y truncamiento controlado.

## Migration Plan

1. Crear carpeta `AppUploadBatchView` con tipos publicos.
2. Implementar shell visual con header, toolbar, selector `AppUpload`, lista, preview y footer.
3. Implementar renderizado de estados, progreso, error/warning, fila activa y resumen.
4. Implementar callbacks de acciones globales y por archivo.
5. Implementar slots `renderMetadata`, `renderPreview`, `renderFileName`, `renderFooterExtra`.
6. Implementar preview default PDF/imagen/fallback con cleanup de object URLs.
7. Agregar README con objetivo, props, ejemplos y limites.
8. Exportar desde barrel local y shared.
9. Agregar tests unitarios/integracion.
10. Validar OpenSpec, tests enfocados, TypeScript y lint si aplica.

## Open Questions

- Si `AppUpload` no expone un modo selector sin lista suficiente, se debe minimizar su lista usando props existentes (`layout`, `size`, `renderItem`) sin modificar su contrato.
- La decision de ubicar preview arriba o abajo en mobile puede ajustarse durante implementacion si las capturas muestran mejor ergonomia.
