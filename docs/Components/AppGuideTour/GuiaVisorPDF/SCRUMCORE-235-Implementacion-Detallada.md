# SCRUMCORE-235 - AppGuideTour - Implementacion Detallada

## Dependencia

Se agrego `driver.js` con `npm install driver.js`.

## Modulo reusable

Ruta base:

```text
src/app/Components/UI/AppGuideTour/
```

Archivos principales:

- `AppGuideTour.types.ts`: contratos publicos, estados, eventos, steps, ref y factory de driver.
- `AppGuideTour.constants.ts`: nombres de eventos y razon `no_valid_targets`.
- `AppGuideTour.service.ts`: resolucion de selector DOM, filtrado de steps y normalizacion de eventos.
- `AppGuideTour.adapter.ts`: puerto interno y factory por defecto.
- `drivers/DriverJsAdapter.ts`: encapsula Driver.js.
- `hooks/useAppGuideTour.ts`: ciclo de vida del tour.
- `AppGuideTour.tsx`: componente headless con API imperativa por ref.
- `index.ts`: exports publicos.

## Hook

`useAppGuideTour` recibe:

- `tourId`
- `steps`
- `autoStart`
- `onEvent`
- `driverFactory`

El hook:

1. Mantiene `state`, `currentStepId` e `isRunning`.
2. Filtra targets faltantes al iniciar, no durante render.
3. Crea el driver de forma lazy mediante `driverFactory`.
4. Emite eventos normalizados.
5. Ejecuta `destroy()` al desmontar.

## Adapter Driver.js

`DriverJsAdapter` traduce cada `AppGuideTourStep` a `DriveStep`:

- `element` queda como selector estable.
- `title` y `description` se asignan al popover.
- `side` controla posicionamiento.
- textos de navegacion: `Anterior`, `Siguiente`, `Finalizar`.

Callbacks usados:

- `onHighlighted`: emite cambio de step.
- `onDestroyed`: diferencia completado vs cancelado usando el ultimo indice activo.

## Integracion AppVisorEmbedPdf

Archivo de configuracion:

```text
src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.guideTour.ts
```

Steps configurados:

- `pdf-toolbar`
- `pdf-thumbnails`
- `pdf-zoom-out`
- `pdf-zoom-level`
- `pdf-zoom-in`
- `pdf-reset-zoom`
- `pdf-rotate-left`
- `pdf-rotate-right`
- `pdf-signature`
- `pdf-lock-signature`
- `pdf-delete-signature`
- `pdf-print`
- `pdf-export`
- `pdf-help`
- `pdf-pagination`
- `pdf-scroll-top`

Archivo de integracion:

```text
src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx
```

Se agrego:

- `guideTourRef`.
- Render de `<AppGuideTour />`.
- `onStartGuideTour`.
- `onGuideTourEvent` que despacha `CustomEvent("app-guide-tour:event")`.
- `data-guide-tour-id="pdf-toolbar"`.
- `data-guide-tour-id="pdf-pagination"`.
- `data-guide-tour-id="pdf-scroll-top"`.

## Toolbar

Archivo:

```text
src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfToolbar.tsx
```

Se agregaron props opcionales:

```ts
onStartGuideTour?: () => void;
isGuideTourAvailable?: boolean;
```

El boton de ayuda solo aparece si ambas condiciones son verdaderas:

- existe `onStartGuideTour`
- `isGuideTourAvailable` es `true`

Esto conserva compatibilidad con consumers actuales.

## Observabilidad

Eventos permitidos:

- `guide_started`
- `guide_completed`
- `guide_cancelled`
- `guide_step_changed`
- `guide_error`

Payload permitido:

- `tourId`
- `stepId`
- `stepIndex`
- `totalSteps`
- `reason`

No se incluye URL, token, nombre de archivo, texto PDF ni identificadores documentales.

## Refinamientos visuales aplicados al visor y listado

Despues de la integracion funcional de la guia se aplicaron ajustes de UX sobre el visor PDF y el panel de documentos. Estos cambios no modifican operaciones de negocio ni contratos de backend; son ajustes de presentacion, affordance y orientacion del usuario.

### Contenedor raiz del visor PDF

Archivo:

```text
src/app/Components/UI/AppVisorEmbedPdf/styles/AppVisorEmbedPdf.module.css
```

Selector:

```css
.root
```

Actualizaciones:

- Se agrego borde gris enterprise al contenedor raiz del visor.
- Se mantuvo `border-radius: 12px`.
- Se mantuvo un shadow leve del visor para separarlo del fondo sin crear una tarjeta pesada.
- El color del borde se suavizo a `rgba(203, 213, 225, 0.78)` para alinear visualmente el visor y el listado de documentos.

### Estado inicial sin documento

Archivos:

```text
src/app/Components/UI/AppVisorEmbedPdf/presentation/States.tsx
src/app/Components/UI/AppVisorEmbedPdf/styles/AppVisorEmbedPdf.module.css
src/app/Components/UI/AppVisorEmbedPdf/types/AppVisorEmbedPdfProps.ts
src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx
```

Actualizaciones:

- El estado vacio muestra el texto `Selecciona un documento`.
- La descripcion orienta al usuario hacia el listado lateral derecho: `Elige un archivo del listado lateral derecho de documentos para visualizarlo aqui.`
- El icono principal usa `FileTextOutlined`.
- El badge dejo de usar `CheckCircleFilled` y ahora usa `ArrowUpOutlined` rotado `45deg`, dando una senal visual de direccion hacia el listado.
- El icono completo se convirtio en `button` accesible con `aria-label="Resaltar listado de documentos"`.
- La prop opcional `onEmptyDocumentHintRequest?: () => void` permite que el visor notifique al contenedor cuando el usuario pide orientacion desde el estado vacio.

El visor no conoce la tabla de documentos. Solo emite el callback opcional; el modulo consumidor decide que hacer con esa senal.

### Hint visual sobre el listado de documentos

Archivos:

```text
src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx
src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.module.css
```

Comportamiento:

1. El usuario hace click en el icono/flecha del estado vacio del visor.
2. `AppVisorEmbedPdf` ejecuta `onEmptyDocumentHintRequest`.
3. `DocumentosWorkbench` recibe la senal con `triggerDocumentListHint`.
4. El panel derecho de documentos se abre con `setCollapsed(false)`.
5. Se activa temporalmente `documentHintActive`.
6. El wrapper del listado recibe `data-document-hint-active="true"`.
7. CSS aplica `documentHintPulse` solo sobre la primera fila del grid:

```css
.listSurface[data-document-hint-active="true"] :global(.ag-theme-quartz .ag-row[row-index="0"] .ag-cell)
```

Esto cubre las tres celdas visibles de la primera fila: checkbox, documento y acciones. La animacion usa un gris mas marcado que el hover normal (`rgba(15, 23, 42, 0.09)`) para que el usuario identifique donde debe seleccionar, sin introducir un color de accion nuevo.

Detalles tecnicos:

- Se usa `requestAnimationFrame` para reiniciar la animacion aunque el usuario haga click repetido.
- Se limpia `documentHintTimeoutRef` al desmontar para evitar timers vivos.
- La duracion visible es de `1600ms`, equivalente a dos ciclos de `0.8s`.
- El hint no selecciona documentos, no dispara `onSelectRow` y no cambia `activeRowId`.

### Panel derecho de documentos

Archivo:

```text
src/app/Components/UI/AppCollapseRail/AppCollapseRail.module.css
```

Selectores:

```css
.panel[data-placement="right"][data-variant="inline"][data-collapsed="false"]
.surface
```

Actualizaciones:

- El `<aside>` derecho abierto quedo con `padding: 0`.
- El borde externo del panel derecho abierto quedo en `border: none` para que no duplique el borde visual.
- La superficie interna `.surface` mantiene el borde visible del listado.
- El borde de `.surface` usa el mismo gris suave del visor: `rgba(203, 213, 225, 0.78)`.
- No se deja `box-shadow` en el listado; se retiro el shadow solicitado temporalmente para volver a la lectura visual previa.

### Header de detalle de gestion correspondencia

Archivo:

```text
src/modules/gestionCorrespondencia/style/GestionCorrespondenciaRoute.module.css
```

Selector:

```css
.detailHeader
```

Actualizacion:

- Se redujo el padding vertical de `16px 18px 12px` a `8px 18px`.
- El objetivo es reducir el alto visual del encabezado que contiene volver a bandeja y metadata, sin cambiar la estructura ni el comportamiento.

## Guia interactiva: comportamiento tecnico del boton Ayuda

El boton `Ayuda - Guia interactiva` vive en `AppPdfToolbar` y aparece solo cuando el visor entrega `onStartGuideTour` e `isGuideTourAvailable`.

Flujo tecnico:

1. `AppPdfToolbar` renderiza el boton con `data-guide-tour-id="pdf-help"`.
2. Al hacer click, ejecuta `onStartGuideTour`.
3. `AppVisorEmbedPdf` llama `guideTourRef.current?.start()`.
4. `AppGuideTour` delega en `useAppGuideTour`.
5. El hook filtra steps cuyos targets no existan o no esten disponibles en DOM.
6. `DriverJsAdapter` convierte los steps internos a `DriveStep` de Driver.js.
7. Driver.js muestra overlay, popover y controles `Anterior`, `Siguiente`, `Finalizar`.
8. El adaptador emite cambios de step mediante `onHighlighted`.
9. Al cerrar o terminar, `onDestroyed` se traduce a evento completado o cancelado.
10. `AppVisorEmbedPdf` despacha `CustomEvent("app-guide-tour:event")` con payload no sensible.

El popover se personaliza en:

```text
src/app/Components/UI/AppGuideTour/AppGuideTour.css
```

Se aplica:

- Borde azul mas visible del popover.
- `border-radius: 14px`.
- Shadow alto del popover para separarlo del visor.
- Bordes redondeados en botones de navegacion.

Tambien se agrego un override defensivo:

```css
.driver-active [data-guide-tour-id="pdf-toolbar"]:has(> .driver-active-element) {
  overflow: visible !important;
}
```

Motivo tecnico:

Driver.js aplica internamente una regla que puede forzar `overflow: hidden !important` sobre contenedores que tienen un elemento activo directo. Cuando un step pasa del toolbar completo a un boton interno, el toolbar puede quedar recortado visualmente. El override limita la correccion al toolbar del PDF durante el tour y preserva `overflow: visible`, evitando que el toolbar se achique o recorte al navegar con `Siguiente`.
