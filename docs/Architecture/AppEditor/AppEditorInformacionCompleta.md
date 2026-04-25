# AppEditorInformacionCompleta

Documento de análisis generado para ejecutar la solicitud `SCRUMCORE-157` sobre el módulo `AppEditor`.

Ruta del módulo analizado:
`src/app/Components/UI/AppEditor/`

Ruta del prompt base de la solicitud:
[AppEditosfuncionamiento.md](/abs/path/C:/Users/SEBASTIAN%20FORERO/Documents/DocuarchiReact20-04/DocuArchiCore.react/docs/Architecture/AppEditor/AppEditosfuncionamiento.md)

## 1. Resumen ejecutivo
`AppEditor` es un editor enriquecido reusable basado en Tiptap/ProseMirror, extendido con capacidades propias de documento tipo Word: paginación visual, page breaks manuales y automáticos, continuidad de escritura entre hojas, cálculo de página actual, soporte de imágenes locales persistidas en IndexedDB y dirty state externo.

El punto de entrada público del módulo está en [index.ts](/abs/path/C:/Users/SEBASTIAN%20FORERO/Documents/DocuarchiReact20-04/DocuArchiCore.react/src/app/Components/UI/AppEditor/index.ts), pero el núcleo técnico real no está allí. La orquestación vive en [useAppEditor.ts](/abs/path/C:/Users/SEBASTIAN%20FORERO/Documents/DocuarchiReact20-04/DocuArchiCore.react/src/app/Components/UI/AppEditor/application/useAppEditor.ts), mientras que la lógica de paginación y cortes se reparte entre [autoPagination.ts](/abs/path/C:/Users/SEBASTIAN%20FORERO/Documents/DocuarchiReact20-04/DocuArchiCore.react/src/app/Components/UI/AppEditor/application/autoPagination.ts) y [autoPageBreak.ts](/abs/path/C:/Users/SEBASTIAN%20FORERO/Documents/DocuarchiReact20-04/DocuArchiCore.react/src/app/Components/UI/AppEditor/application/autoPageBreak.ts).

La capa visual de documento se arma en [AppEditor.tsx](/abs/path/C:/Users/SEBASTIAN%20FORERO/Documents/DocuarchiReact20-04/DocuArchiCore.react/src/app/Components/UI/AppEditor/presentation/AppEditor.tsx), que compone el canvas paginado, los controles de zoom y el contador. La toolbar vive en [AppEditorToolbar.tsx](/abs/path/C:/Users/SEBASTIAN%20FORERO/Documents/DocuarchiReact20-04/DocuArchiCore.react/src/app/Components/UI/AppEditor/presentation/AppEditorToolbar.tsx), donde se concentra el comportamiento de selección preservada para formato combinado.

Los archivos más críticos para comprender y modificar el editor con seguridad son:
- [presentation/AppEditor.tsx](/abs/path/C:/Users/SEBASTIAN%20FORERO/Documents/DocuarchiReact20-04/DocuArchiCore.react/src/app/Components/UI/AppEditor/presentation/AppEditor.tsx)
- [presentation/AppEditorToolbar.tsx](/abs/path/C:/Users/SEBASTIAN%20FORERO/Documents/DocuarchiReact20-04/DocuArchiCore.react/src/app/Components/UI/AppEditor/presentation/AppEditorToolbar.tsx)
- [application/useAppEditor.ts](/abs/path/C:/Users/SEBASTIAN%20FORERO/Documents/DocuarchiReact20-04/DocuArchiCore.react/src/app/Components/UI/AppEditor/application/useAppEditor.ts)
- [application/autoPagination.ts](/abs/path/C:/Users/SEBASTIAN%20FORERO/Documents/DocuarchiReact20-04/DocuArchiCore.react/src/app/Components/UI/AppEditor/application/autoPagination.ts)
- [application/autoPageBreak.ts](/abs/path/C:/Users/SEBASTIAN%20FORERO/Documents/DocuarchiReact20-04/DocuArchiCore.react/src/app/Components/UI/AppEditor/application/autoPageBreak.ts)
- [application/usePaginationMetrics.ts](/abs/path/C:/Users/SEBASTIAN%20FORERO/Documents/DocuarchiReact20-04/DocuArchiCore.react/src/app/Components/UI/AppEditor/application/usePaginationMetrics.ts)
- [application/usePageContext.ts](/abs/path/C:/Users/SEBASTIAN%20FORERO/Documents/DocuarchiReact20-04/DocuArchiCore.react/src/app/Components/UI/AppEditor/application/usePageContext.ts)

## 2. Arquitectura por capas
El módulo sí tiene una separación clara por capas.

| Capa | Ubicación | Responsabilidad real |
| --- | --- | --- |
| `presentation` | `presentation/*.tsx` | Shell visual, toolbar, botón guardar, canvas paginado, zoom, contador |
| `application` | `application/*.ts` | Orquestación del editor, paginación, métricas, página actual, dirty state, ids, normalización |
| `domain` | `domain/*.ts` | Tipos, contratos, normalización base del valor |
| `infrastructure` | `infrastructure/*` | Configuración Tiptap, extensiones custom, render wrapper, IndexedDB |
| `tests` | `*.test.*` | Contrato de comportamiento del módulo |

### Diagrama ASCII de alto nivel
```text
AppEditor.tsx
  |
  +-- AppEditorToolbar.tsx
  |
  +-- TiptapEditorContent.tsx
  |
  +-- useAppEditor()
  |     |
  |     +-- createAppEditorConfig()
  |     +-- buildAppEditorExtensions()
  |     +-- appEditorImageStore
  |     +-- autoPagination engine
  |     +-- autoPageBreak operations
  |
  +-- usePaginationMetrics()
  |
  +-- usePageContext()

buildAppEditorExtensions()
  +-- StarterKit
  +-- Underline
  +-- Link
  +-- TaskList/TaskItem
  +-- TextAlign
  +-- ResizableImage
  +-- PageBreak
```

## 3. Casos de uso
### 3.1 Edición básica
- Actor: usuario
- Archivo principal: [useAppEditor.ts](/abs/path/C:/Users/SEBASTIAN%20FORERO/Documents/DocuarchiReact20-04/DocuArchiCore.react/src/app/Components/UI/AppEditor/application/useAppEditor.ts)
- Flujo: Tiptap genera `transaction` -> `onUpdate` normaliza HTML -> `onChange`
- Resultado: contenido HTML enriquecido

### 3.2 Modo controlado
- Actor: formulario padre
- Archivo principal: `useAppEditor.ts`
- Flujo: cambia `value` -> `syncControlledValue()` compara contra `lastKnownValueRef` -> `setContent()` y restaura selección
- Resultado: el editor refleja el estado externo sin perder caret cuando es posible

### 3.3 Modo no controlado
- Actor: componente consumidor
- Archivo principal: `useAppEditor.ts`
- Flujo: `defaultValue` se normaliza en `initialContentRef`
- Resultado: editor autosuficiente

### 3.4 Toolbar de formato
- Actor: usuario
- Archivo principal: [AppEditorToolbar.tsx](/abs/path/C:/Users/SEBASTIAN%20FORERO/Documents/DocuarchiReact20-04/DocuArchiCore.react/src/app/Components/UI/AppEditor/presentation/AppEditorToolbar.tsx)
- Flujo: `handleToolbarMouseDownCapture()` preserva rango -> `runWithPreservedTextSelection()` ejecuta comando -> se intenta mantener la selección entre clicks
- Resultado: comandos inline y estructurales con menos pérdida de selección

### 3.5 Formato inline combinado
- Actor: usuario
- Archivo principal: `AppEditorToolbar.tsx`
- Flujo: `Negrita` -> `Cursiva` -> `Subrayado` sobre el mismo rango
- Resultado: la selección combinada se preserva entre acciones consecutivas

### 3.6 Listas y viñetas
- Actor: usuario
- Archivo principal: `AppEditorToolbar.tsx`, `autoPagination.ts`, `autoPageBreak.ts`
- Flujo: `toggleBulletList`/`toggleOrderedList`/`toggleTaskList` -> si hay overflow en hoja, `resolveDirectChildOverflowAction()` o split de lista
- Resultado: listas continuas entre hojas

### 3.7 Inserción de enlaces
- Actor: usuario
- Archivo principal: `AppEditorToolbar.tsx`
- Flujo: popover -> `formatUrl()` -> `setLink()` o `unsetLink()`
- Resultado: enlaces normalizados a `https`

### 3.8 Inserción de imagen por URL
- Actor: usuario
- Archivo principal: `AppEditorToolbar.tsx`
- Flujo: URL -> `setImage()` -> `updateAttributes("image", { width })`
- Resultado: imagen con ancho persistido

### 3.9 Inserción de imagen local
- Actor: usuario
- Archivo principal: `useAppEditor.ts`, `appEditorImageStore.ts`
- Flujo: archivo -> `generateLocalImageId()` -> `saveImage()` en IndexedDB -> `setImage()` con `localImageId` e `imageId`
- Resultado: imagen persistida localmente y rehidratable

### 3.10 Paginación visual
- Actor: usuario
- Archivo principal: `AppEditor.tsx`, `usePaginationMetrics.ts`, `usePageContext.ts`
- Flujo: medición del `.ProseMirror` -> boundaries -> canvas -> página actual
- Resultado: vista paginada tipo documento

### 3.11 Page break manual
- Actor: usuario/comando
- Archivo principal: [page-break.extension.ts](/abs/path/C:/Users/SEBASTIAN%20FORERO/Documents/DocuarchiReact20-04/DocuArchiCore.react/src/app/Components/UI/AppEditor/infrastructure/page-break.extension.ts)
- Resultado: salto de página explícito persistido en HTML

### 3.12 Page break automático
- Actor: motor interno
- Archivo principal: `useAppEditor.ts`, `autoPagination.ts`, `autoPageBreak.ts`
- Flujo: cambia documento -> se detecta overflow -> se insertan `pageBreak` con `auto=true`
- Resultado: continuidad de contenido entre hojas sin persistir basura al exterior

### 3.13 Escritura al final de hoja
- Actor: usuario
- Archivo principal: `useAppEditor.ts`, `autoPagination.ts`, `autoPageBreak.ts`
- Flujo: typing cerca del límite -> split del mismo párrafo o movimiento before/list-item
- Resultado: el cursor debe continuar en la hoja siguiente y empujar el resto

### 3.14 Paste multipágina
- Actor: usuario
- Archivo principal: `useAppEditor.ts`
- Flujo: `paste` -> repaginación inmediata -> múltiples iteraciones de acciones
- Resultado: el cursor queda en el bloque final pegado y el contenido se redistribuye

### 3.15 Zoom
- Actor: usuario o padre
- Archivo principal: `AppEditor.tsx`
- Flujo: controlado o no controlado -> `normalizeZoomLevel()` -> actualiza CSS variables y `boundaryScale`
- Resultado: zoom visual del documento

### 3.16 Dirty state / save state
- Actor: shell externo
- Archivo principal: [useAppEditorSaveState.ts](/abs/path/C:/Users/SEBASTIAN%20FORERO/Documents/DocuarchiReact20-04/DocuArchiCore.react/src/app/Components/UI/AppEditor/application/useAppEditorSaveState.ts)
- Flujo: `normalizeEditorHtml(currentValue)` vs `normalizeEditorHtml(savedValue)`
- Resultado: `saveStatus = "dirty" | "idle"`

### 3.17 Disabled / readOnly
- Actor: padre
- Archivo principal: `AppEditor.tsx`, `useAppEditor.ts`
- Resultado: UI y editabilidad bloqueadas

### 3.18 Cálculo de página actual
- Actor: scroll del canvas
- Archivo principal: `usePageContext.ts`
- Resultado: `currentPage` con histéresis para evitar jitter

## 4. Estados
### 4.1 Estados de UI
- `resolvedThemeMode` en `AppEditor.tsx`
- `isVisualPagination` en `AppEditor.tsx`
- `uncontrolledZoomLevel` en `AppEditor.tsx`
- popovers y dropdowns en `AppEditorToolbar.tsx`

### 4.2 Estados de edición
- `editor` creado por `useEditor()` en `useAppEditor.ts`
- `isEditable` derivado de `disabled || readOnly`
- `lastKnownValueRef` para evitar loops en modo controlado

### 4.3 Estados de selección
- `editor.state.selection`
- `textSelectionRef` y `alignSelectionRef` en `AppEditorToolbar.tsx`
- `PaginationScrollAnchor` en `useAppEditor.ts`

### 4.4 Estados de paginación
- `dirtyStartChildIndexRef`
- `dirtyNeedsPreviousBreakCleanupRef`
- `totalPages`, `pageBoundaries`, `visualPageBoundaries`, `visualContentHeight` en `usePaginationMetrics.ts`

### 4.5 Estados de scroll
- `scrollContainer` detectado en `findScrollableAncestor()`
- `isUserScrolling` en `useAppEditor.ts`
- `currentPageRef/currentPage` en `usePageContext.ts`

### 4.6 Estados de imágenes
- `localImageUrlsRef`
- `localImageScopeRef`
- `localImageSyncTokenRef`
- `__appEditorLastImagePos`
- `__appEditorLastImageIdentity`

### 4.7 Estados de guardado
- `normalizedCurrentValue`
- `normalizedSavedValue`
- `isDirty`
- `saveStatus`

## 5. Secuencias
### 5.1 Render inicial
```text
Parent
  -> AppEditor
     -> resolvePaginationMetrics
     -> useAppEditor
        -> createAppEditorConfig
        -> buildAppEditorExtensions
        -> useEditor(Tiptap)
     -> usePaginationMetrics
     -> usePageContext
     -> AppEditorToolbar
     -> TiptapEditorContent
```

### 5.2 Escritura simple
```text
Usuario escribe
  -> Tiptap transaction(docChanged)
  -> useAppEditor.handleEditorTransaction
  -> calcula dirtyStartChildIndex
  -> scheduleAutoPagination(deferred)
  -> performAutoPagination
  -> removeAutoPageBreaks
  -> resolveAutoPageBreakActions
  -> si hay overflow: split/before/list-item
  -> syncAutoPageBreakSpacerHeights
  -> dispatch "app-editor-pagination-updated"
```

### 5.3 Escritura al final de hoja
```text
Usuario escribe al borde
  -> transaction docChanged
  -> resolveAutoPageBreakActions encuentra "split"
  -> splitTextBlockAtPositionAndInsertPageBreak
  -> se parte el mismo párrafo
  -> se preserva selección en fragmento correcto
  -> el resto se empuja a la hoja siguiente
```

### 5.4 Paste largo
```text
Usuario pega varios párrafos
  -> transaction meta paste
  -> scheduleAutoPagination(immediate)
  -> cleanup previo
  -> while(actions.length > 0)
       -> before / list-item / split
  -> se recalculan boundaries y scroll
```

### 5.5 Formato combinado
```text
Usuario selecciona texto
  -> toolbar mousedown capture guarda rango
  -> click Negrita -> runWithPreservedTextSelection
  -> click Cursiva -> reutiliza rango guardado
  -> click Subrayado -> reutiliza rango guardado
  -> repaginación preserva anclaje visual
```

### 5.6 Imagen local
```text
Usuario elige archivo
  -> AppEditorToolbar.handleImageFileChange
  -> useAppEditor.insertLocalImage
  -> appEditorImageStore.saveImage
  -> setImage(localImageId, imageId, src=objectURL)
  -> rehydrateLocalImages en recarga/sync
```

### 5.7 Scroll y página actual
```text
Usuario hace scroll en canvas
  -> usePageContext.handleScroll
  -> resolveStablePageFromOffset
  -> commitPage
  -> badge "Pagina X de Y"
```

## 6. Unidades arquitectónicas
### AppEditor
- Archivo: `presentation/AppEditor.tsx`
- Rol: shell principal del componente
- Dependencias: `useAppEditor`, `usePaginationMetrics`, `usePageContext`, `AppEditorToolbar`, `TiptapEditorContent`
- Resuelve: layout, canvas paginado, zoom, accesibilidad, header, error/helper

### AppEditorToolbar
- Archivo: `presentation/AppEditorToolbar.tsx`
- Rol: comandos del editor y popovers
- Dependencias: `editor.chain()`, imágenes, selección preservada
- Resuelve: UX de edición

### AppEditorSaveAction
- Archivo: `presentation/AppEditorSaveAction.tsx`
- Rol: botón guardar acoplado al `saveStatus`

### useAppEditor
- Archivo: `application/useAppEditor.ts`
- Rol: orquestador principal
- Dependencias: Tiptap, autoPagination, autoPageBreak, IndexedDB, normalización HTML
- Resuelve: ciclo de vida del editor, imágenes locales, controlado/no controlado, paginación

### usePaginationMetrics
- Archivo: `application/usePaginationMetrics.ts`
- Rol: transformar layout natural en métricas de documento

### usePageContext
- Archivo: `application/usePageContext.ts`
- Rol: derivar página actual estable desde scroll

### useAppEditorSaveState
- Archivo: `application/useAppEditorSaveState.ts`
- Rol: dirty state puro

### autoPagination
- Archivo: `application/autoPagination.ts`
- Rol: decidir dónde cortar

### autoPageBreak
- Archivo: `application/autoPageBreak.ts`
- Rol: ejecutar cortes y remapear cursor/selección

### PageBreak
- Archivo: `infrastructure/page-break.extension.ts`
- Rol: extensión custom para saltos de página

### ResizableImage
- Archivo: `infrastructure/resizable-image.extension.ts`
- Rol: extensión image con attrs extra, align y persistencia visual de selección

### appEditorImageStore
- Archivo: `infrastructure/indexeddb/appEditorImageStore.ts`
- Rol: persistencia local de blobs de imagen

## 7. Inventario de funciones clave
| Función | Archivo | Tipo | Propósito |
| --- | --- | --- | --- |
| `buildAriaLabel` | `presentation/AppEditor.tsx` | interna | resolver etiqueta accesible |
| `resolvePaginationMetrics` | `presentation/AppEditor.tsx` | interna | resolver dimensiones A4 y márgenes |
| `normalizeZoomLevel` | `presentation/AppEditor.tsx` | interna | normalizar zoom |
| `useAppEditor` | `application/useAppEditor.ts` | exportada | hook principal del editor |
| `capturePaginationScrollAnchor` | `useAppEditor.ts` | exportada | guardar offset visual del caret |
| `restorePaginationScrollAnchor` | `useAppEditor.ts` | exportada | restaurar scroll relativo |
| `resolveSelectionPageIndex` | `useAppEditor.ts` | interna | estimar página de la selección |
| `scrollSelectionIntoViewWithinContainer` | `useAppEditor.ts` | interna | mover viewport si la selección avanzó |
| `syncControlledValue` | `useAppEditor.ts` | interna | reconciliar value externo |
| `insertLocalImage` | `useAppEditor.ts` | retorno del hook | insertar imagen local persistida |
| `calculatePaginationMetrics` | `usePaginationMetrics.ts` | exportada | calcular boundaries y páginas |
| `usePaginationMetrics` | `usePaginationMetrics.ts` | exportada | hook de medición DOM |
| `calculatePageFromOffset` | `usePageContext.ts` | exportada | cálculo puro de página |
| `usePageContext` | `usePageContext.ts` | exportada | hook de página actual |
| `resolveAutoPageBreakActions` | `autoPagination.ts` | exportada | detectar acciones de corte |
| `removeAutoPageBreaks` | `autoPagination.ts` | exportada | limpiar cortes automáticos |
| `syncAutoPageBreakSpacerHeights` | `autoPagination.ts` | exportada | ajustar alturas visuales |
| `insertPageBreakBeforeBlock` | `autoPageBreak.ts` | exportada | mover bloque completo a nueva hoja |
| `splitListBlockBeforeItemAndInsertPageBreak` | `autoPageBreak.ts` | exportada | partir listas |
| `splitTextBlockAtPositionAndInsertPageBreak` | `autoPageBreak.ts` | exportada | partir párrafos |
| `normalizeEditorHtml` | `normalizeEditorHtml.ts` | exportada | limpiar HTML persistido |
| `stripAutoLayoutMetadata` | `normalizeEditorHtml.ts` | exportada | quitar metadata auto |
| `generateLocalImageId` | `localImageIds.ts` | exportada | id de imagen local |
| `generateEditorImageId` | `localImageIds.ts` | exportada | id lógico de nodo image |
| `createAppEditorConfig` | `tiptap.config.ts` | exportada | crear config Tiptap |
| `buildAppEditorExtensions` | `tiptap.extensions.ts` | exportada | registrar extensiones |
| `useAppEditorSaveState` | `useAppEditorSaveState.ts` | exportada | calcular dirty state |

### Efectos secundarios relevantes
- `useAppEditor.ts` despacha transacciones ProseMirror, toca scroll, escucha resize/scroll y crea object URLs.
- `usePaginationMetrics.ts` lee DOM (`offsetTop`, `offsetHeight`, `scrollHeight`) y muta atributos `data-pagination-*`.
- `autoPageBreak.ts` cambia documento y selección.
- `appEditorImageStore.ts` persiste en IndexedDB.

## 8. Mapa de archivos
| Archivo | Capa | Propósito | Cuándo tocarlo |
| --- | --- | --- | --- |
| `index.ts` | API | exportaciones públicas | si cambia API pública |
| `README.md` | docs | guía funcional | si cambian capacidades visibles |
| `domain/editor.types.ts` | domain | contratos del componente | si cambian props o resultados |
| `domain/editor.model.ts` | domain | normalización base | si cambia documento vacío o clamp |
| `domain/save-state.types.ts` | domain | tipos save state | si cambian estados de guardado |
| `presentation/AppEditor.tsx` | presentation | shell visual completo | layout, zoom, canvas, wiring |
| `presentation/AppEditorToolbar.tsx` | presentation | toolbar y comandos | UX de formato, selección, popovers |
| `presentation/AppEditorSaveAction.tsx` | presentation | acción de guardar | UX de guardado |
| `application/useAppEditor.ts` | application | hook central | edición, repaginación, imágenes |
| `application/usePaginationMetrics.ts` | application | métricas de documento | boundaries, altura, layout visual |
| `application/usePageContext.ts` | application | página actual | scroll, histéresis |
| `application/useAppEditorSaveState.ts` | application | dirty state | comparación current/saved |
| `application/normalizeEditorHtml.ts` | application | limpieza de HTML | serialización persistida |
| `application/localImageIds.ts` | application | ids | cambios de identidad |
| `application/autoPagination.ts` | application | decisión de cortes | overflow, split detection |
| `application/autoPageBreak.ts` | application | ejecución de cortes | remapeo de selección |
| `infrastructure/tiptap.config.ts` | infrastructure | config de editor | setup general |
| `infrastructure/tiptap.extensions.ts` | infrastructure | bundle de extensiones | agregar/quitar capacidades |
| `infrastructure/TiptapEditorContent.tsx` | infrastructure | wrapper EditorContent | aria/render básico |
| `infrastructure/page-break.extension.ts` | infrastructure | nodo pageBreak | manual breaks |
| `infrastructure/resizable-image.extension.ts` | infrastructure | nodo image extendido | align/attrs/selection |
| `infrastructure/indexeddb/appEditorImageStore.ts` | infrastructure | store local de imágenes | persistencia local |

## 9. Mapa de modificación por necesidad
| Necesidad | Archivo principal | Archivo secundario | Riesgo |
| --- | --- | --- | --- |
| cambiar toolbar | `presentation/AppEditorToolbar.tsx` | `AppEditorToolbar.test.tsx` | perder selección/foco |
| cambiar formato combinado | `AppEditorToolbar.tsx` | `useAppEditor.ts` | colapso de selección |
| cambiar comportamiento de selección | `useAppEditor.ts` | `autoPageBreak.ts` | cursor en párrafo incorrecto |
| cambiar repaginación visual | `usePaginationMetrics.ts` | `AppEditor.tsx` | contador/layout inconsistente |
| cambiar split de párrafos | `autoPageBreak.ts` | `autoPageBreak.test.ts` | salto de cursor |
| cambiar listas entre hojas | `autoPagination.ts` | `autoPageBreak.ts` | jitter en viñetas |
| cambiar page breaks | `page-break.extension.ts` | `autoPageBreak.ts` | HTML/parse roto |
| cambiar página actual | `usePageContext.ts` | `usePageContext.test.tsx` | jitter de contador |
| cambiar zoom | `AppEditor.tsx` | `usePageContext.ts` | boundaries desfasados |
| cambiar HTML serializado | `normalizeEditorHtml.ts` | `useAppEditor.ts` | persistencia de metadata |
| cambiar imágenes locales | `useAppEditor.ts` | `appEditorImageStore.ts` | blobs huérfanos |
| cambiar estilos | `AppEditor.module.css` | `AppEditor.tsx` | desalineación visual |

## 10. Riesgos y zonas sensibles
### Selección y foco
La zona más frágil está entre `AppEditorToolbar.tsx`, `useAppEditor.ts` y `autoPageBreak.ts`. El riesgo real es que la selección se remapee al párrafo siguiente o se colapse al enfocarse la toolbar.

### Scroll anchor
`capturePaginationScrollAnchor()` y `restorePaginationScrollAnchor()` en `useAppEditor.ts` son sensibles. Un cambio allí puede producir sensación de “salto” aunque la selección lógica sea correcta.

### Medición DOM
`usePaginationMetrics.ts` y `autoPagination.ts` dependen de `offsetTop`, `offsetHeight`, `scrollHeight`, `getBoundingClientRect()` y `Range.getBoundingClientRect()`. Esto es inherentemente frágil y dependiente del layout real.

### Split de texto y listas
`splitTextBlockAtPositionAndInsertPageBreak()` y `splitListBlockBeforeItemAndInsertPageBreak()` son zonas de alta regresión. Ahí se decide si el cursor queda en el mismo párrafo/lista o salta a otro bloque.

### Imágenes locales
`useAppEditor.ts` + `ResizableImage` + `appEditorImageStore.ts` están acoplados por `imageId`, `localImageId` y `src`. Si uno cambia sin el otro, se rompe selección persistente o rehidratación.

## 11. Relación entre tests y funcionalidades
- [AppEditor.test.tsx](/abs/path/C:/Users/SEBASTIAN%20FORERO/Documents/DocuarchiReact20-04/DocuArchiCore.react/src/app/Components/UI/AppEditor/AppEditor.test.tsx): shell visual, zoom, contador, integración general.
- [AppEditorToolbar.test.tsx](/abs/path/C:/Users/SEBASTIAN%20FORERO/Documents/DocuarchiReact20-04/DocuArchiCore.react/src/app/Components/UI/AppEditor/AppEditorToolbar.test.tsx): comandos de toolbar, selección combinada.
- [useAppEditor.test.tsx](/abs/path/C:/Users/SEBASTIAN%20FORERO/Documents/DocuarchiReact20-04/DocuArchiCore.react/src/app/Components/UI/AppEditor/useAppEditor.test.tsx): controlado/no controlado, paste, continuidad de escritura, dirty selection cleanup.
- [autoPageBreak.test.ts](/abs/path/C:/Users/SEBASTIAN%20FORERO/Documents/DocuarchiReact20-04/DocuArchiCore.react/src/app/Components/UI/AppEditor/autoPageBreak.test.ts): split de texto/listas y preservación de cursor.
- [autoPagination.test.ts](/abs/path/C:/Users/SEBASTIAN%20FORERO/Documents/DocuarchiReact20-04/DocuArchiCore.react/src/app/Components/UI/AppEditor/autoPagination.test.ts): detección de overflow y decisiones de corte.
- [usePageContext.test.tsx](/abs/path/C:/Users/SEBASTIAN%20FORERO/Documents/DocuarchiReact20-04/DocuArchiCore.react/src/app/Components/UI/AppEditor/usePageContext.test.tsx): página actual e histéresis.
- [usePaginationMetrics.test.tsx](/abs/path/C:/Users/SEBASTIAN%20FORERO/Documents/DocuarchiReact20-04/DocuArchiCore.react/src/app/Components/UI/AppEditor/usePaginationMetrics.test.tsx): cálculo de métricas.
- `pageBreak.extension.test.ts`, `resizableImage.extension.test.ts`, `appEditorImageStore.test.ts`, `localImageIds.test.ts`: extensiones e infraestructura.

### Zonas con cobertura más débil
- No se observa E2E browser persistente dentro del módulo para todos los casos de layout real.
- La medición exacta por DOM sigue muy dependiente de tests sintéticos y harnesses de JSDOM.
- No hay una suite formal de backend porque no aplica directamente.

## 12. Conclusión
La arquitectura real de `AppEditor` es la de un editor Tiptap extendido con una capa de documento visual. No es solo un componente UI: es un sistema con motor de repaginación incremental, normalización de persistencia, identidad de imágenes y control explícito de selección/scroll.

La pieza más difícil de mantener sin regresiones es la triada:
- `useAppEditor.ts`
- `autoPagination.ts`
- `autoPageBreak.ts`

La parte más peligrosa de tocar es cualquier cambio que combine:
- selección
- scroll
- medición DOM
- split de párrafos o listas

Si un desarrollador quiere modificar el editor con seguridad, debe empezar por entender esos tres archivos y luego validar siempre contra las pruebas del módulo.

## Supuestos y pendientes
- No se detectó integración directa del módulo con backend; el editor trabaja principalmente en cliente.
- El comportamiento base de formato, selección y nodos viene parcialmente de Tiptap/ProseMirror; aquí se documentó la parte propia del proyecto y se diferenció donde fue visible.
- No se ejecutó una exploración browser amplia adicional en este análisis; la trazabilidad se basó en código y tests del módulo.
- `useAppEditor.ts` concentra mucha responsabilidad. Es funcional hoy, pero también es el archivo con mayor complejidad accidental del módulo.
