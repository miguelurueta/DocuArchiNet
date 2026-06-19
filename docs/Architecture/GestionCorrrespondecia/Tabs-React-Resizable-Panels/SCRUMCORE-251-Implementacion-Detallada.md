# SCRUMCORE-251 - Implementacion Detallada

## 1. Alcance acumulado del ticket

SCRUMCORE-251 contiene dos capas de trabajo:

1. Implementacion base previa:
   - Vista paralela opt-in para tabs `Gestion` y `Documentos`.
   - Uso de `react-resizable-panels`.
   - Componente `GestionWorkbenchParallelTabs`.

2. Bloque actual documentado:
   - Compactacion de `GestionRespuestaMainTabContent`.
   - Ajuste de `AppToolbar` para densidad compacta.
   - Ajuste de `AppUpload` para adjuntos compactos y estado estable.
   - Ajuste responsive de `DocumentosWorkbench` y `AppVisorEmbedPdf`.
   - Ajuste de `AppTabs.panelContent` para Documentos.
   - Ajuste de metadata del header en mobile.

## 2. AppToolbar

### Archivo

- `src/app/Components/UI/AppToolbar/AppToolbar.tsx`

### Cambios

- Se agrego la prop:

```ts
density?: "default" | "compact";
```

- Valor por defecto:

```ts
density = "default"
```

- Aplicacion de clase:

```ts
density === "compact" && styles.compactDensity
```

### CSS

Archivo:

- `src/app/Components/UI/AppToolbar/AppToolbar.module.css`

Cambios:

- Se agrego `.compactDensity`.
- Se reducen:
  - `gap`
  - `padding`
  - `border-radius`
  - alto/min-height de botones Ant Design
  - font-size de botones
  - font-size de iconos
- Se elimino el media query global `max-width: 1100px` que forzaba layout en columna.
- Se mantiene el comportamiento compacto real controlado por el componente.

### Prueba

Archivo:

- `src/app/Components/UI/AppToolbar/AppToolbar.test.tsx`

Cambio:

- El test desktop ahora captura `container`.
- Se valida que el section no contenga clase `compact` cuando `matchMedia` indica desktop.

## 3. AppUpload

### Archivo

- `src/app/Components/UI/AppUpload/AppUpload.tsx`

### Cambios de API

- Se agrego:

```ts
className?: string;
```

Motivo:

- Permitir estilos scoped desde consumidores sin alterar el componente compartido globalmente.

### Cambios de semantica

- Cada card renderiza:

```tsx
role="listitem"
```

Motivo:

- Mejorar semantica dentro de contenedores `role="list"`.
- Permitir targeting CSS estable desde Gestion.

### Cambios de estado

- Se agrego:

```ts
const filesRef = useRef<AppUploadFile[]>(files);
```

- Se sincroniza con `useEffect`.
- `emitChange` actualiza el ref antes de emitir.
- `updateFile` usa `filesRef.current`.
- `updateStatus` valida transiciones contra el estado actual, no contra un closure anterior.

Motivo:

- Evitar perdida visual de archivos cargados cuando `strategy="auto"` actualiza estado de forma async.

### CSS

Archivo:

- `src/app/Components/UI/AppUpload/AppUpload.module.css`

Cambios:

- `.cardList` pasa a ser fila compacta.
- Se oculta preview en modo lista.
- Metadata ocupa espacio flexible.
- Acciones pasan a posicion estatica y siempre visibles en lista.
- `sizeSM .cardList` reduce padding, gap y altura.
- En mobile (`max-width: 720px`) las cards de lista son todavia mas bajas.

### Prueba

Archivo:

- `src/app/Components/UI/AppUpload/AppUpload.test.tsx`

Caso agregado:

- `mantiene visible el archivo cargado con estrategia auto`

Valida:

- El archivo `soporte.pdf` queda visible.
- `onChange` termina con estado `done`.

## 4. GestionRespuestaMainTabContent

### Archivo TSX

- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx`

Cambios:

- Se importa `DeleteOutlined`.
- Se importa `AppUploadFile`.
- Se agrega callback `removeAttachment`.
- `AppToolbar` recibe `density="compact"`.
- `AppUpload` se configura como:
  - `className={styles.compactAttachmentsUpload}`
  - `layout="list"`
  - `previewOnClick={false}`
  - `renderActions` custom con solo eliminar
  - `size="sm"`
  - `strategy="auto"`

Resultado UX:

- El upload mantiene dropzone a lo ancho.
- Las cards cargadas son pequenas, agrupadas lado a lado.
- Se elimina accion visual de preview/ojo.
- Solo queda accion de eliminar.

### Archivo CSS

- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.module.css`

Cambios:

- `.mainTab`: menor gap y padding inferior.
- `.workbench`: menor gap.
- `.toolbar`: alto fijo compacto, padding reducido, botones mas bajos.
- `.workbenchBody`: alturas por desktop/tablet/mobile.
- Tablet y desktop pequeno:
  - se reduce separacion entre toolbar y editor.
  - se ajusta alto del editor para pantallas 769-1366.
- Mobile:
  - `workbenchBody` usa `clamp(430px, 66dvh, 560px)`.
  - cards de adjuntos mas compactas.
- Adjuntos:
  - `.compactAttachmentsUpload` ocupa 100%.
  - lista flex-wrap.
  - item maximo `13.5rem`.
  - boton eliminar circular compacto.

## 5. AppTabs

### Archivo

- `src/app/Components/UI/AppTabs/AppTabs.module.css`

Cambios:

- `panelContent` reduce padding base de `1.15rem 1.2rem` a `0.65rem 1.2rem 0.7rem`.
- En tablet/desktop pequeno (`769px-1366px`):
  - menor gap.
  - padding `0.4rem 0.85rem 0.45rem`.
  - altura `calc(100vh - 190px)`.
- Para Documentos se usan overrides scoped:

```css
.panelContent:has([data-testid="documentos-workbench"])
```

Breakpoints:

- Mobile general: `min-height: clamp(560px, 76dvh, 680px)`.
- Pantallas bajas: `min-height: clamp(450px, 68dvh, 540px)`.
- Galaxy S8+: `min-height: clamp(470px, 70dvh, 560px)`.
- Mobile medio: `min-height: clamp(600px, 74dvh, 660px)`.
- Mobile alto: `min-height: clamp(680px, 78dvh, 740px)`.
- XR/familia 400-430x840-920: `min-height: clamp(660px, 76dvh, 710px)`.
- iPad Mini: `height` y `min-height: calc(100vh - 190px)`.

Detalle importante:

- El override de iPad Mini queda al final del archivo para no ser pisado por `max-width: 768px`.

## 6. DocumentosWorkbench

### Archivo

- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.module.css`

Cambios:

- Mobile general:
  - `height: clamp(540px, 72dvh, 680px)`
  - `min-height: 540px`
  - `max-height: none`
- Se mantiene `viewer`, `documentsRailGuard` y `collapseRail` al 100%.
- Se preserva lista con max-height propio.
- Se agregan breakpoints especificos:

| Vista | Rango CSS | Alto workbench/visor |
| --- | --- | --- |
| Mobile base | `max-width: 430px` | `clamp(460px, 68dvh, 620px)` |
| iPhone SE / baja | `max-width: 430px` + `max-height: 740px` | `clamp(410px, 65dvh, 490px)` |
| Galaxy S8+ | `350-380px` x `720-760px` | `clamp(425px, 68dvh, 515px)` |
| 12 Pro aprox. | `max-width: 430px` + `min-height: 741px` | `clamp(575px, 73dvh, 650px)` |
| 14 Pro Max / alto | `max-width: 430px` + `min-height: 880px` | `clamp(660px, 76dvh, 720px)` |
| XR override | `400-430px` x `840-920px` | `clamp(645px, 74dvh, 695px)` |
| iPad Mini | `744-834px` x `1000-1150px` | `calc(100vh - 215px)` |

Motivo:

- El usuario reporto desajustes especificos por dispositivo.
- Se sincronizo `.workbenchBody` con `.viewer` para que el visor no quede corto respecto al contenedor.

## 7. AppVisorEmbedPdf

### Archivo

- `src/app/Components/UI/AppVisorEmbedPdf/styles/AppVisorEmbedPdf.module.css`

Cambios:

- Mobile general pasa de `min-height: 500px` a `540px`.
- Se agregan los mismos rangos de altura que en `DocumentosWorkbench` para `.root`.
- El root del visor se alinea con el alto de `.viewer` por dispositivo.

Motivo:

- La UI mostraba un contenedor mas alto que el estado vacio/visor PDF.
- El usuario pidio que el visor PDF quedara acorde al div trabajado.

## 8. Header de detalle

### Archivo TSX

- `src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.tsx`

Cambio:

- Cada `detailMetaItem` recibe `title` completo:

```tsx
title={`${item.label}: ${item.value}`}
```

### Archivo CSS

- `src/modules/gestionCorrespondencia/style/GestionCorrespondenciaRoute.module.css`

Cambios mobile:

- `.detailHeaderStart` usa `flex-wrap: nowrap`.
- `.detailMeta` pasa a ancho automatico, margen izquierdo auto y alineacion derecha.
- En rangos pequenos:
  - metadata en columna.
  - `align-items: flex-end`.
  - fuente menor.
  - valores con wrapping normal.
  - `text-align: right`.

Resultado:

- La metadata puede mostrarse junto al boton de retorno.
- El usuario conserva tooltip nativo para contenido completo.

## 9. Testing y validacion ejecutada

Comandos ejecutados durante el bloque:

```powershell
npx.cmd tsc --noEmit --pretty false
git diff --check
```

Resultado:

- TypeScript OK.
- `git diff --check` sin errores de whitespace.
- Avisos LF/CRLF presentes por configuracion local de Git, no por errores de codigo.

## 10. No cambios

No se modifico:

- Backend.
- Endpoints.
- Services.
- Contrato de documentos.
- Contrato del visor PDF.
- Flujo de firma.
- Reemplazo de paginas anotadas.
- Permisos.
- Persistencia de layout.
- Dependencias en este bloque actual.
