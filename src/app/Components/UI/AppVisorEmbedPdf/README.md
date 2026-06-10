# AppVisorEmbedPdf (01-FE)

Componente reusable enterprise para **visualización** de PDFs basado en **EmbedPDF + Pdfium Engine**.

## API pública

```tsx
<AppVisorEmbedPdf fileUrl="/demo/Radicado_2026_0413.pdf" />
```

Props:
- `fileUrl?: string`: URL/ruta del PDF (local o API). Si no se provee, se usa el demo configurado.
- `className?: string`
- `style?: React.CSSProperties`

## Estructura (archivos clave)

- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx`: orquestación (engine + plugins + UI).
- `src/app/Components/UI/AppVisorEmbedPdf/plugins/pluginRegistration.ts`: registro oficial de plugins EmbedPDF.
- `src/app/Components/UI/AppVisorEmbedPdf/engine/*`: adapter + engine hook.
- `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfToolbar.tsx`: toolbar presentacional (memoizada).
- `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfSignatureModal.tsx`: modal de firmas (Dibujar/Subir).
- `src/app/Components/UI/AppVisorEmbedPdf/presentation/States.tsx`: estados `loading/error/empty`.
- `src/app/Components/UI/AppVisorEmbedPdf/hooks/useDemoPdfUrl.ts`: demo PDF (sin hardcodear URLs en lógica reusable).

## Principios de arquitectura

- Encapsulación: `DocumentosWorkbench` (consumidor) no debe conocer `@embedpdf/*`.
- Plugins oficiales: se registran vía `createPluginRegistration(Package)` (sin lógica PDF manual).
- Estados de layout viven dentro de `AppVisorEmbedPdf` (no se elevan al Workbench).
- Estilos: CSS Modules (sin Tailwind / styled-components).

## Toolbar (presentación)

Archivo: `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfToolbar.tsx`

- 100% presentacional: no conoce engine/documentId/plugins internos.
- Memoización: `React.memo` para evitar rerenders por scroll/virtualización.
- Acciones:
  - Thumbnails: toggle.
  - Zoom: `+ / - / reset` (solo cuando `rotationSteps === 0` por estabilidad).
  - Rotate: izquierda/derecha.
  - Firma: abre/cierra modal.
  - Bloquear/Desbloquear firmas: toggle con icono/tooltip según estado.
  - Eliminar firma seleccionada: solo cuando hay una firma seleccionada.
  - Print/Export: botones a la derecha.

## Zoom (SCRUMCORE-204)

- Plugin: `@embedpdf/plugin-zoom`
- Implementación: `AppVisorEmbedPdf.tsx` usa `zoom.provides.requestZoom*` con el centro del viewport para evitar
  “saltos” de scroll al cambiar el zoom.

## Thumbnails (SCRUMCORE-205)

- Plugin: `@embedpdf/plugin-thumbnail`
- Render: `ThumbnailsPane` + `ThumbImg` (sin wrappers extra).
- Auto-scroll: comportamiento nativo del plugin (sin sincronización custom).

## Rotate (SCRUMCORE-206)

- Plugin: `@embedpdf/plugin-rotate`
- Render: cuando la rotación no es 0, se usa `<Rotate>` para mantener el pipeline oficial.
- Guardrails: ajustes mínimos de layout en 90/270 para evitar clipping por rounding.

## Render / Scroll / Viewport (base)

- Plugins: `@embedpdf/plugin-viewport`, `@embedpdf/plugin-scroll`, `@embedpdf/plugin-render`
- Render principal: `Viewport` + `Scroller` + `RenderLayer`
- Virtualización: provista por `Scroller` (no se implementa virtualización manual).

## Annotation + Selection (dependencias del plugin)

- Plugin: `@embedpdf/plugin-annotation` requiere `@embedpdf/plugin-selection`.
- Selección deshabilitada en modo `default` para minimizar overlays de texto; el plugin se mantiene por dependencia.

## Export / Print (SCRUMCORE-207)

Plugins:
- `@embedpdf/plugin-export`
- `@embedpdf/plugin-print`

Regla operativa:
- Antes de exportar/imprimir, se fuerza `annotationCap.provides.commit()` para alinear **estado UI** vs **PDF real**.

Export:
- Se usa `exportApi.provides.saveAsCopy(documentId)` para obtener el PDF **materializado** por el engine y descargarlo.

## Firma (SCRUMCORE-210)

### Modal (UI)

Archivo: `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfSignatureModal.tsx`

Tabs:
- `Dibujar firma`
- `Subir firma`

Comportamientos:
- `Dibujar firma`:
  - Botón `Limpiar` aparece solo cuando ya existe trazo (`current` no es `null`).
  - `Limpiar` reinicia el `SignatureDrawPad` mediante `key` (reset controlado).
  - `Usar firma` coloca la firma y luego resetea el modal (no conserva el trazo anterior).
- `Subir firma`:
  - El botón muestra `Subir firma` o `Reemplazar firma` según si ya hay un archivo seleccionado.
  - Muestra el nombre del archivo adjunto.
  - Permite quitar el archivo adjunto con una `X` (limpia input + estado interno).

### Placement (PDF)

- Placement oficial del plugin `@embedpdf/plugin-signature`:
  - `signatureCap.provides.addEntry(...)`
  - `signatureCap.provides.forDocument(documentId).activateSignaturePlacement(entryId)`

### Eliminar firma seleccionada

Archivo: `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx`

- Detección: `annotation.provides.getSelectedAnnotationIds()` + `getAnnotationById(uid)`.
- Eliminación: `deleteAnnotation(...)` + `commit()` para persistir en el PDF.
- No se usa `purgeAnnotation` como “borrado” (solo afecta UI/state y desincroniza export/print).

### Bloquear / desbloquear firmas (UX)

- Botón de candado:
  - Si no hay firmas: deshabilitado (tooltip “requiere al menos 1 firma”).
  - Si hay firmas: permite “bloquear”.
  - Si está bloqueado: permite “desbloquear”.
- Nota: esto NO es firma digital criptográfica PKI/PAdES; es un guardrail UX sobre anotaciones.

