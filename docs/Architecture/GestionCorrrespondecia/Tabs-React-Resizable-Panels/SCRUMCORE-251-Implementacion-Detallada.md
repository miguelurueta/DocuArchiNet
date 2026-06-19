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
   - Incorporacion de asistente IA flotante persistente en el workbench.
   - Reubicacion del indicador de palabras/caracteres del `AppEditor` para evitar solapamiento con el FAB de IA.

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

## 11. Asistente IA flotante del workbench

### Archivo TSX

- `src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx`

### Objetivo

Agregar una accion flotante persistente, azul y enterprise, visible sobre el workbench de `GestionRespuesta` en ambos tabs (`Gestion` y `Documentos`), que abra un panel tipo chat sin depender de una extension del navegador ni de un servicio backend.

### Imports agregados

Se extendio el import de `@ant-design/icons` con:

```ts
CloseOutlined
RobotOutlined
SendOutlined
```

Se agrego:

```ts
import type { KeyboardEvent } from "react";
import { useEffect, useRef, useState } from "react";
```

Motivo:

- `RobotOutlined`: identidad visual del boton `IA`.
- `CloseOutlined`: cierre del chat y boton interno para limpiar input.
- `SendOutlined`: accion explicita de envio.
- `KeyboardEvent`: tipado del handler en captura.
- `useRef`: input no controlado para evitar perdida de foco.

### Estado local agregado

```ts
const [isAssistantOpen, setIsAssistantOpen] = useState(false);
const [isAssistantClosing, setIsAssistantClosing] = useState(false);
const assistantInputRef = useRef<HTMLInputElement | null>(null);
const [assistantMessages, setAssistantMessages] = useState<
  Array<{ id: string; role: "assistant" | "user"; text: string }>
>([
  {
    id: "assistant-welcome",
    role: "assistant",
    text: "Asistente listo para apoyar la gestion del tramite.",
  },
]);
```

Notas tecnicas:

- `isAssistantOpen`: controla si el panel esta montado.
- `isAssistantClosing`: permite animar la salida antes de desmontar.
- `assistantInputRef`: evita controlar el input con React y elimina el re-render por cada caracter.
- `assistantMessages`: mantiene historial local de la sesion visual.

### Envio de mensaje

Funcion agregada:

```ts
const sendAssistantMessage = () => {
  const trimmedDraft = assistantInputRef.current?.value.trim() ?? "";
  if (!trimmedDraft) return;

  setAssistantMessages((currentMessages) => [
    ...currentMessages,
    {
      id: `user-${Date.now()}`,
      role: "user",
      text: trimmedDraft,
    },
    {
      id: `assistant-${Date.now()}`,
      role: "assistant",
      text: "Mensaje recibido. La integracion del servicio conversacional queda lista para conectarse aqui.",
    },
  ]);

  if (assistantInputRef.current) {
    assistantInputRef.current.value = "";
    assistantInputRef.current.focus();
  }
};
```

Comportamiento:

- Lee directamente el valor del input via ref.
- Ignora mensajes vacios.
- Agrega mensaje del usuario.
- Agrega respuesta placeholder local.
- Limpia el input y restaura foco.

Limitacion:

- No hay llamada a API, streaming, modelo IA ni persistencia. El texto de respuesta es un placeholder de integracion.

### Apertura y cierre

Funciones agregadas:

```ts
const openAssistant = () => {
  setIsAssistantClosing(false);
  setIsAssistantOpen(true);
};

const closeAssistant = () => {
  setIsAssistantClosing(true);
  window.setTimeout(() => {
    setIsAssistantOpen(false);
    setIsAssistantClosing(false);
  }, 190);
};

const toggleAssistant = () => {
  if (isAssistantOpen) {
    closeAssistant();
    return;
  }

  openAssistant();
};
```

Motivo:

- React desmonta inmediatamente si `isAssistantOpen` pasa a `false`.
- Para una animacion de cierre real se introduce `isAssistantClosing`.
- El timeout de 190 ms coincide con la duracion CSS de cierre.

### Teclado y foco

Handler agregado:

```ts
const handleAssistantKeyDownCapture = (event: KeyboardEvent<HTMLElement>) => {
  if (event.target === assistantInputRef.current && event.key === "Enter" && !event.shiftKey) {
    event.preventDefault();
    sendAssistantMessage();
  }

  event.stopPropagation();
};
```

Razon:

- El `AppEditor` y superficies relacionadas pueden tener listeners de teclado.
- El primer intento con input controlado perdia foco despues del primer caracter.
- La solucion final usa input no controlado y detiene eventos en captura dentro del panel.
- `Enter` envia mensaje.
- El input es de una linea por requerimiento final del usuario; no se soporta multilinea en esta version.

### JSX agregado

El asistente se monta dentro de:

```tsx
<div className={styles.layoutBody}>
  ...
  <div className={styles.workbenchAssistantLayer}>...</div>
</div>
```

Estructura:

- `workbenchAssistantLayer`
  - capa fija con `pointer-events: none`.
- `assistantPanel`
  - panel tipo chat.
  - `aria-label="Chat de asistencia del workbench"`.
  - captura keyboard/pointer para proteger foco.
  - `data-closing` para animacion de salida.
- `assistantHeader`
  - titulo `IA`.
  - boton cerrar.
- `assistantMessages`
  - `role="log"`.
  - `aria-live="polite"`.
- `assistantComposer`
  - input normal.
  - boton limpiar interno.
  - boton enviar.
- `assistantFab`
  - boton flotante persistente.
  - `aria-expanded`.
  - `aria-label` dinamico.
  - muestra `RobotOutlined` cerrado y `CloseOutlined` abierto.
  - label `IA` encapsulado en `assistantFabLabel` para ocultarlo sin ocultar el span interno del icono de Ant Design.

### Correcciones iterativas aplicadas

1. El boton inicialmente decia `Ayuda`.
   - Se cambio a `IA`.
2. El icono inicial era de mensaje.
   - Se cambio a `RobotOutlined`.
3. El panel inicialmente abria con animacion simple.
   - Se cambio a animacion de salida/entrada desde el FAB.
4. La animacion se percibia demasiado ludica.
   - Se ajusto a una animacion enterprise: menos rebote, halo sobrio, expansion limpia.
5. El textarea permitia multilinea, pero el usuario pidio input normal.
   - Se reemplazo por `input type="text"`.
6. El input controlado perdia foco despues del primer caracter.
   - Se reemplazo por input no controlado via `useRef`.
7. El evento de teclado era interceptado por el aislamiento en captura.
   - Se envio Enter desde `handleAssistantKeyDownCapture`.
8. La X del FAB no se veia.
   - La regla `.assistantFab[data-open="true"] span { display: none; }` ocultaba el span interno de Ant Design.
   - Se agrego `assistantFabLabel` y ahora solo se oculta el label `IA`.
9. El input necesitaba limpiar texto.
   - Se agrego `assistantClearButton` dentro de `assistantInputShell`.
10. El chat se veia ancho.
   - Se redujo ancho a `clamp(276px, 28vw, 342px)` y mobile a `min(330px, calc(100vw - 1.75rem))`.

### CSS agregado

Archivo:

- `src/modules/gestionCorrespondencia/style/GestionRespuesta.module.css`

Clases principales:

- `.workbenchAssistantLayer`
- `.assistantFab`
- `.assistantFabLabel`
- `.assistantPanel`
- `.assistantHeader`
- `.assistantTitle`
- `.assistantCloseButton`
- `.assistantMessages`
- `.assistantMessage`
- `.assistantComposer`
- `.assistantInputShell`
- `.assistantInput`
- `.assistantClearButton`
- `.assistantSendButton`

Keyframes:

- `assistantFabIn`
- `assistantFabRing`
- `assistantFabAttention`
- `assistantPanelOpenFromFab`
- `assistantPanelCloseToFab`
- `assistantFabSelected`
- `assistantMessageIn`
- `assistantSendPulse`

### Posicionamiento

```css
.workbenchAssistantLayer {
  position: fixed;
  right: clamp(1rem, 2.4vw, 2rem);
  bottom: clamp(1rem, 2.4vw, 1.75rem);
  z-index: 340;
}
```

Mobile:

```css
right: 0.875rem;
bottom: 0.875rem;
```

Motivo:

- Persistencia visual en ambos tabs y durante scroll interno.
- Ubicacion inferior derecha solicitada por el usuario.
- No depende de si el tab activo es `Gestion` o `Documentos`.

### Interaccion del input

El input final:

- Es un `input` normal.
- No tiene estado React por cada caracter.
- Envia con Enter.
- Permite limpiar con X interna.
- Mantiene foco despues de enviar o limpiar.
- Detiene propagacion para no activar atajos del AppEditor.

### Comportamiento accesible

- FAB:
  - `aria-label="Abrir asistente"` cuando esta cerrado.
  - `aria-label="Cerrar asistente"` cuando esta abierto.
  - `aria-expanded={isAssistantOpen}`.
- Panel:
  - `aria-label="Chat de asistencia del workbench"`.
- Log:
  - `role="log"`.
  - `aria-live="polite"`.
- Input:
  - `aria-label="Mensaje para el asistente"`.
- Boton limpiar:
  - `aria-label="Limpiar mensaje"`.
- Boton enviar:
  - `aria-label="Enviar mensaje"`.

## 12. AppEditor - indicador flotante de caracteres

### Archivo

- `src/app/Components/UI/AppEditor/AppEditor.module.css`

### Cambio

```diff
.pageStatsIndicator {
-  right: 1rem;
+  right: 8rem;
}
```

Motivo:

- El indicador flotante de palabras/caracteres quedaba en la misma zona inferior derecha que el FAB de IA.
- Se reubico hacia la izquierda para evitar interrupcion visual y solapamiento.

Alcance:

- No cambia la medicion de palabras.
- No cambia la medicion de caracteres.
- No cambia `pageIndicator`.
- No cambia la logica de paginacion visual.
- No cambia la interaccion del editor.

## 13. Estado final del asistente IA

El asistente queda como una capa visual lista para integracion futura.

Incluye:

- Boton flotante azul `IA`.
- Icono de robot.
- Estado abierto con X visible.
- Panel chat compacto.
- Animacion de entrada desde el FAB.
- Animacion de salida hacia el FAB.
- Mensajes locales.
- Input normal con Enter para enviar.
- Boton interno para limpiar texto.
- Boton enviar.
- Aislamiento de foco y teclado frente al AppEditor.

No incluye:

- IA real.
- Streaming.
- Persistencia.
- Contexto del tramite.
- Adjuntos en chat.
- Llamada a backend.
- Auditoria conversacional.
- Autorizacion o permisos especificos.

## 14. GestionWorkbenchParallelTabs - grip de resize enterprise

### Archivos

- `src/modules/gestionCorrespondencia/components/workbenchParallelTabs/GestionWorkbenchParallelTabs.tsx`
- `src/modules/gestionCorrespondencia/components/workbenchParallelTabs/GestionWorkbenchParallelTabs.module.css`

### Objetivo

Hacer que el divisor entre paneles de la vista paralela sea menos invasivo visualmente, pero que siga comunicando claramente que se puede arrastrar para redimensionar.

### Cambio TSX

Se agrego tooltip nativo al `PanelResizeHandle`:

```tsx
<PanelResizeHandle
  className={styles.resizeHandle}
  aria-label="Redimensionar paneles"
  title="Arrastra para redimensionar"
>
  <span className={styles.resizeGrip} aria-hidden="true" />
</PanelResizeHandle>
```

Motivo:

- El `aria-label` conserva accesibilidad.
- El `title` da una ayuda directa al usuario mouse/desktop.
- El `span` interno sigue marcado como decorativo (`aria-hidden`) porque el nombre accesible vive en el handle.

### Cambio CSS

El contenedor `.resizeHandle` se mantiene como area interactiva:

- `flex: 0 0 10px`
- `width: 10px`
- `min-width: 10px`
- `cursor: col-resize`

Pero su fondo base queda transparente:

```css
background: transparent;
```

Se agrego pseudo-elemento de feedback:

```css
.resizeHandle::before {
  position: absolute;
  inset: 0;
  content: "";
  border-radius: 6px;
  background: rgba(47, 111, 237, 0);
  transition: background 160ms ease, opacity 160ms ease;
}
```

En hover/focus/active:

```css
.resizeHandle:hover::before,
.resizeHandle:focus-visible::before,
.resizeHandle[data-resize-handle-active]::before {
  background: rgba(47, 111, 237, 0.08);
}
```

El focus visible queda sutil:

```css
box-shadow: inset 0 0 0 1px rgba(47, 111, 237, 0.2);
```

### Grip interno

El `span` `.resizeGrip` se reemplazo visualmente por puntos verticales:

```css
.resizeGrip {
  width: 4px;
  height: 28px;
  color: #6f8297;
  background:
    radial-gradient(circle, currentColor 1.5px, transparent 1.7px) center 2px / 4px 8px repeat-y;
}
```

En hover/focus/active:

```css
color: #2f6fed;
opacity: 1;
```

Motivo:

- Los puntos verticales son mas claros que una barra solida para indicar arrastre.
- No agregan dependencias.
- No usan SVG manual.
- Mantienen estetica enterprise y baja ornamentacion.

### Correcciones aplicadas

1. Se hizo invisible todo el handle.
2. El usuario pidio conservar visible el `span` interno.
3. Se restauro visibilidad del `span`.
4. El usuario pidio una senal mas clara de que se podia correr.
5. Se implemento grip de puntos verticales, hover/focus azul sutil y tooltip nativo.

### Alcance

No se modifico:

- `PanelGroup`.
- `Panel`.
- `minSize`.
- `defaultSize`.
- comportamiento de resize.
- tests de render accesible.
