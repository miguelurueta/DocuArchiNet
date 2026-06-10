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

## Correccion tecnica de skips en firma personal

Se eliminaron los skips heredados de los casos de firma personal convirtiendolos en pruebas activas. La correccion se hizo en capa de tests, sin modificar el runtime del visor.

Archivos:

```text
src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx
src/app/Components/UI/AppVisorEmbedPdf/hooks/useWorkflowPersonalSignature.test.tsx
```

En `AppVisorEmbedPdf.test.tsx` se mockea `useWorkflowPersonalSignature` para controlar explicitamente los estados `idle`, `loading` y `ready`. Con eso se valida:

- al entrar a la pestaña `Firma personal`, el modal llama `load()`;
- el estado `loading` muestra feedback visual;
- el estado `ready` muestra preview y nombre de archivo;
- `Usar firma` envia `previewDataUrl`, `imageMimeType` e `imageData` al flujo de firma;
- se activa el placement de firma;
- se limpia el estado de firma personal con `clear()`;
- no se muestra el payload binario/data URL como texto visible.

En `useWorkflowPersonalSignature.test.tsx` se prueba el hook con mocks de `clienteApi.get`, `URL.createObjectURL` y `URL.revokeObjectURL`. La cobertura valida:

- endpoint de metadata `/api/workflow/usuarios/firma-temporal`;
- resolucion de URL absoluta contra `clienteApi.defaults.baseURL`;
- descarga con `responseType: "blob"`;
- conversion a `imageData` mediante `blob.arrayBuffer()`;
- reintento controlado cuando la descarga temporal retorna 404;
- cleanup de object URL al ejecutar `clear()`.

Se usa un `Blob` mockeado con `arrayBuffer()` explicito porque el entorno JSDOM/Vitest no garantiza esa API en todos los runtimes de prueba. Esto evita acoplar la prueba a una implementacion concreta de DOM y mantiene la verificacion sobre el contrato que usa el hook.

## Limpieza tecnica de warnings en pruebas del visor

Despues de activar las pruebas de firma personal, se limpio la salida de `AppVisorEmbedPdf.test.tsx` para eliminar dos advertencias que no fallaban la suite pero reducian la confiabilidad del reporte:

- `NaN is an invalid value for the width css style property.`
- `An update to EmbedPdfLoadedDocumentView inside a test was not wrapped in act(...).`

La correccion se aplico solo en el test harness del visor. No se modifico codigo productivo, componentes, hooks, permisos, plugins ni flujos del visor.

Archivo ajustado:

```text
src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx
```

### Causa del `NaN width`

El mock local de `Scroller` exponia un contrato incompleto:

```tsx
renderPage({ pageIndex: 0 })
```

El componente real renderiza paginas usando mas propiedades entregadas por el plugin de scroll:

- `pageIndex`
- `width`
- `height`
- `rotatedWidth`
- `rotatedHeight`

Dentro de `EmbedPdfLoadedDocumentView`, esos valores se usan para calcular dimensiones de slots, capas de render y contenedores rotados. Al no existir `width`/`height` en el mock, los calculos terminaban en `Math.ceil(undefined)`, generando `NaN` y React reportaba el warning de CSS.

### Como se corrigio

Se completo el mock de `Scroller` para que refleje el contrato minimo que consume el visor:

```tsx
renderPage({
  pageIndex: 0,
  width: 612,
  height: 792,
  rotatedWidth: 612,
  rotatedHeight: 792,
})
```

Los valores `612 x 792` representan una pagina carta/PDF comun en puntos y son suficientes para que el test tenga dimensiones finitas y deterministicas. No buscan validar layout visual pixel-perfect; solo evitan que el mock entregue datos imposibles para el contrato real.

### Causa del warning `act(...)`

El caso heredado `usa el demo pdf cuando fileUrl no existe` hacia una asercion sincronica inmediatamente despues del render:

```tsx
expect(screen.getByTestId("render-layer")).toBeInTheDocument();
```

Ese escenario dispara efectos internos del visor para resolver el demo PDF y montar el documento activo. React detectaba que una actualizacion async ocurria fuera de la espera del test.

### Como se corrigio

Se cambio el caso a async y se espero el render con `waitFor`:

```tsx
await waitFor(() => {
  expect(screen.getByTestId("render-layer")).toBeInTheDocument();
});
```

Esto deja que Testing Library envuelva la espera de la actualizacion async y alinea el test con el comportamiento real del usuario: el documento aparece despues de que el visor termina de resolver su estado.

### Impacto

- No cambia `AppVisorEmbedPdf.tsx`.
- No cambia la carga de demo PDF.
- No cambia `DocumentContent`.
- No cambia `Scroller` real.
- No cambia render de paginas, zoom, rotacion, thumbnails, firmas, print ni export.
- Solo hace que el mock de prueba sea fiel al contrato que ya consume el componente.

### Resultado

Comando:

```bash
npm test -- --run src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx
```

Resultado:

```text
1 file passed
18 tests passed
sin warnings NaN width
sin warnings act(...)
```

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

## Actualizaciones de estabilidad, TypeScript y Gestion Correspondencia

Durante el refinamiento posterior del SCRUMCORE-235 se realizaron cambios adicionales sobre el visor PDF, la tabla de Gestion Correspondencia, los semaforos de estado, el boton de ayuda interactiva y deuda TypeScript que bloqueaba `tsc`. Estos cambios se aplicaron de forma acotada para preservar la logica existente del visor, la navegacion de la bandeja y los contratos de tabla.

### Punto de control previo

Antes de corregir TypeScript se creo un commit de respaldo con los cambios visuales y funcionales ya aplicados:

```text
7343d118216d5d9ae0a592782e24c0f5b690d604
SCRUMCORE-235: enable visor permissions and status cues
```

Ese commit conserva el estado previo a la correccion de errores TypeScript, permitiendo volver a una base estable si una correccion posterior afectara el visor.

### DocumentContext, permisos del visor y control de re-render

Archivo:

```text
src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx
```

Problema observado:

- Existia riesgo de renderizaciones repetidas porque `documentContext` se derivaba desde `documentViewer.documentoActivo` mediante `useState` + `useEffect`.
- Ese patron podia crear un nuevo objeto de contexto tras cambios del documento activo y disparar efectos encadenados.
- El flujo de permisos del visor PDF estaba comentado o no quedaba plenamente integrado con la carga managed del visor.

Solucion aplicada:

- `documentContext` se convirtio en un valor derivado con `useMemo`.
- Las dependencias del memo se limitaron a campos primitivos del documento activo:
  - `attemptId`
  - `documentId`
  - `documentKey`
  - `fileUrl`
  - `firmaCheckStatus`
  - `isElectronicallySigned`
  - `isPdf`
  - `nombreGabinete`
  - `viewerKind`
- Se agrego `lastVisorLoadKeyRef` para evitar ejecutar `visorRef.current.load()` repetidamente sobre el mismo documento.
- La clave de carga combina `documentId`, `fileUrl`, `documentKey` e `isElectronicallySigned`.
- El load managed solo corre cuando:
  - existe `documentContext`
  - el documento es PDF
  - existe `fileUrl`
  - la clave de carga no coincide con la ultima cargada

Contexto enviado al visor:

```ts
{
  url,
  attemptId,
  documentKey,
  isElectronicallySigned,
  idImagen,
  nombreGabinete,
  idTareaWorkflow,
  radicado,
  nombre_modulo: "gestioncorrespondencia"
}
```

Impacto tecnico:

- Los permisos del visor pueden resolverse con `idImagen`, `nombreGabinete`, `idTareaWorkflow`, `radicado` y `nombre_modulo`.
- El visor mantiene su render actual porque `fileUrl={activeFileUrl}` sigue existiendo.
- El load managed se usa para consolidar permisos/policy sin cambiar la experiencia visual del PDF.
- Se reduce el riesgo de `Maximum update depth exceeded` al eliminar estado derivado innecesario y evitar cargas repetidas del mismo documento.
- El `stopViewerLoading(attemptKey)` se ejecuta cuando el load termina en `loaded`, `failed` o `cancelled`, o cuando ocurre error.

### Boton Ayuda de guia interactiva con indicador visual

Archivos:

```text
src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfToolbar.tsx
src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfToolbar.module.css
```

Actualizaciones:

- El boton conserva su estilo base anterior.
- Se agrego una clase `guideButton` solo para posicionar un indicador visual.
- Se agrego `guideButtonDot` como badge azul de notificacion.
- El badge muestra el numero `1`.
- El badge queda en una esquina del boton para sugerir que existe ayuda disponible.
- No se cambio:
  - `onClick={onStartGuideTour}`
  - `data-guide-tour-id="pdf-help"`
  - `aria-label="Guia interactiva"`
  - `title="Ayuda - Guia interactiva"`

Valores visuales:

```css
background: #2563eb;
color: #ffffff;
box-shadow: 0 0 0 2px #ffffff;
width: 14px;
height: 14px;
font-size: 9px;
```

Impacto tecnico:

- No cambia la logica del tour.
- No cambia disponibilidad del boton.
- No cambia eventos ni telemetry.
- Solo mejora el affordance visual del acceso a la guia.

### Semaforo de estados basado en columna ESTADO

Archivos:

```text
src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx
src/modules/gestionCorrespondencia/style/GestionCorrespondencia.module.css
```

Objetivo:

- Hacer que la columna visual de semaforo refleje el valor real de la columna `ESTADO`.
- Evitar colores genericos que no correspondan con el estado textual.
- Mantener la tabla y su data source sin modificar.

Implementacion:

- Se agrego `STATUS_FIELD_CANDIDATES` para leer el estado desde multiples nombres posibles:
  - `ESTADO`
  - `Estado`
  - `estado`
  - `ESTADO_TRAMITE`
  - `EstadoTramite`
  - `estadoTramite`
  - `NOMBRE_ESTADO`
  - `NombreEstado`
  - `nombreEstado`
  - `STATUS`
  - `Status`
  - `status`
- `resolveStatusValue` extrae el valor textual desde la fila.
- `normalizeStatusKey` normaliza acentos con `normalize("NFD")` y elimina diacriticos.
- `STATUS_TONE_BY_VALUE` mapea estados normalizados a tonos visuales.

Mapeo vigente:

| Estado | Tono | Color |
| --- | --- | --- |
| `Por tramitar` | `warning` | `#f59e0b` |
| `En tramite` | `info` | `#0ea5e9` |
| `Tramitado` | `success` | `#16a34a` |
| `Solicitud aprobada` | `success` | `#16a34a` |
| `Solicitud por aprobación` | `review` | `#3b82f6` |
| `Tramitado archivado` | `archived` | `#64748b` |
| sin estado o no mapeado | `neutral` | `#3b82f6` |

Cambios visuales:

- El punto del semaforo quedo sin `box-shadow`.
- Se mantienen puntos compactos de `9px`.
- El color se define por `data-tone`.
- El `aria-label` y `title` exponen `Estado: {valor}` para accesibilidad.

Impacto tecnico:

- La columna no altera datos del backend.
- No introduce sorting ni filtering nuevo.
- No modifica acciones de tabla.
- No modifica seleccion de fila.
- Solo agrega una columna visual derivada de la fila renderizada.

### Tabla Gestion Correspondencia: apariencia enterprise sin cambiar logica

Archivos:

```text
src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx
src/modules/gestionCorrespondencia/style/GestionCorrespondencia.module.css
```

Cambios de estructura visual:

- La tabla mantiene `AppTable`.
- Se mantiene `rowSelection="single"` para no romper seleccion interna.
- Se mantiene `onSelectionChanged={setSelectedRows}`.
- Se desactiva solo la UI de checks:

```tsx
rowSelectionCheckboxes={false}
rowSelectionHeaderCheckbox={false}
```

Impacto:

- La columna de checkboxes desaparece visualmente.
- La seleccion interna de AG Grid sigue funcionando.
- `selectedRows` sigue actualizandose si la fila queda seleccionada por click.
- El modo `selectedRows` del export no se elimina.
- No se toca `AppTable` global ni su renderer compartido.

Cambios CSS aplicados a la tabla:

- Header con gris enterprise mediante variables de AG Grid.
- Lineas suaves solo en filas del cuerpo.
- La linea vertical de la columna `acciones` se mantiene visible con `box-shadow: inset 1px 0 0`.
- No se muestran separadores verticales en el header.
- El hover usa gris suave.
- La fila seleccionada usa gris mas notable:

```css
--gestion-table-row-selected: rgba(100, 116, 139, 0.14);
--gestion-table-row-selected-text: #111827;
```

- El texto de la fila seleccionada queda negro enterprise y `font-weight: 600`.
- Se quito el outline de foco de celda:

```css
.ag-cell-focus,
.ag-cell:focus-within {
  outline: 0;
  border-radius: 0;
}
```

Motivo:

- AG Grid agrega `ag-cell-focus` cuando se hace click en una celda.
- El outline con `border-radius` dejaba pequenas esquinas/espacios blancos sobre el fondo de seleccion.
- Se elimino el borde visual de foco para que la fila seleccionada quede continua.
- No se cambio navegacion por teclado ni seleccion interna; solo se retiro el indicador visual de foco en esta tabla.

### Toolbar de Gestion Correspondencia

Archivo:

```text
src/modules/gestionCorrespondencia/style/GestionCorrespondencia.module.css
```

Ajustes visuales realizados durante el SCRUM:

- `AppInputSearch` del toolbar se hizo mas compacto y con radio tipo Apple/enterprise.
- Se redujo altura visual del input.
- Se suavizo el borde del input.
- Se eliminaron efectos visuales extra de foco sobre el contenedor Ant Select/AutoComplete que generaban un "ruido" alrededor del input al hacer click.
- El boton `Actualizar` quedo alineado a la derecha del input dentro del grupo de acciones.
- El boton `Actualizar` se visualizo con variante primaria azul de `AppButton` sin cambiar su accion.
- El radio del input se ajusto para convivir visualmente con el boton.
- El contenedor toolbar redujo exceso de borde para acercarse al radio del navbar.

Impacto:

- No cambia autocomplete.
- No cambia `applySearch`.
- No cambia `handleSearchChange`.
- No cambia `handleSearchClear`.
- No cambia `table.onQueryChange`.
- No cambia la accion de actualizar.

### Layout principal de Gestion Correspondencia

Archivo:

```text
src/modules/gestionCorrespondencia/style/GestionCorrespondencia.module.css
```

Ajustes aplicados:

- Se ajusto el padding superior del contenido para separar mejor del navbar.
- Se evaluaron valores intermedios y se dejo el espaciado final solicitado.
- Se redujo el padding inferior para compactar la vista.

Impacto:

- No cambia rutas.
- No cambia carga de datos.
- No cambia tabla.
- Solo afecta separacion visual del contenido en pantalla.

### Acciones de tabla

Archivo:

```text
src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx
```

Cambios aplicados:

- La columna de acciones conserva `acciones` como columna pinned/right del modelo original.
- El boton de tres puntos se visualiza en orientacion vertical (`more`) cuando el action renderer entrega ese icono.
- El handler `handleTableAction` sigue usando `actionId`:
  - `reasignar_tramite`
  - `reasignar_tramite_menu`
  - `gestionar_tramite`
  - `gestionar_tramite_menu`
- No se cambio la navegacion a detalle.
- No se cambio la apertura del modal de reasignacion.

### Correcciones TypeScript sin alterar funcionalidad

Se corrigieron errores de `npx tsc -b` en componentes relacionados y deuda cercana, manteniendo el comportamiento existente.

#### AppEditor

Archivos:

```text
src/app/Components/UI/AppEditor/application/autoPageBreak.ts
src/app/Components/UI/AppEditor/application/autoPagination.ts
src/app/Components/UI/AppEditor/application/useAppEditor.ts
src/app/Components/UI/AppEditor/presentation/AppEditor.tsx
src/app/Components/UI/AppEditor/presentation/AppEditorToolbar.tsx
```

Correcciones:

- `resolveFirstTextRangeInNode` recibio tipo explicito `{ start: number; end: number } | null` para evitar narrowing incorrecto a `never`.
- `resolveSplitPositionFromDomText` cambio parametros destructurados sin uso por `_params`, manteniendo el retorno conservador `null`.
- Se removio import no usado `wrapHtmlInVisualPages`.
- `transaction.mapping.setMirror` se volvio defensivo:

```ts
const mappingWithMirror = transaction.mapping as typeof transaction.mapping & {
  setMirror?: (from: number, to: number) => void;
};
mappingWithMirror.setMirror?.(deleteStepIndex, insertStepIndex);
```

- Se elimino `pageIndices` no usado.
- Se eliminaron variables locales duplicadas `hasSelectedImage` no usadas dentro de callbacks.

Impacto:

- No cambia paginacion visual.
- No cambia edicion.
- No cambia toolbar.
- Solo limpia errores de tipo y compatibilidad con APIs instaladas.

#### AppVisorEmbedPdf

Archivo:

```text
src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx
```

Correcciones:

- `waitPdfTaskVoid` acepta tasks cuyo `wait` resuelve cualquier valor, porque `commit()` devuelve `boolean` en la version instalada.
- Se mantiene el objetivo funcional: esperar a que `commit()` termine antes de imprimir, exportar o guardar firmado.
- Se agrego `toPdfBlobPart` para convertir de forma segura:
  - `ArrayBuffer`
  - `Uint8Array<ArrayBufferLike>`
- Esto evita pasar directamente un `Uint8Array` con `SharedArrayBuffer` potencial a `Blob`.
- `downloadBuffer` usa `toPdfBlobPart`.
- `saveAsCopy()` se llama sin `documentId`, respetando la firma actual del plugin instalado.
- Se elimino `onResetRotation` no usado.

Impacto:

- Export sigue materializando anotaciones antes de descargar.
- Guardar PDF firmado sigue haciendo commit antes de crear el blob.
- Print sigue intentando commit antes de imprimir.
- No se cambia UI del visor.
- No se cambia flujo de firmas.
- No se cambia lock de firmas.

#### Plugins EmbedPDF

Archivo:

```text
src/app/Components/UI/AppVisorEmbedPdf/plugins/pluginRegistration.ts
```

Correccion:

- Se agrego `exclusive: false` en la configuracion de interaccion de:
  - `signatureStamp`
  - `signatureInk`

Motivo:

- El tipo actual del plugin exige `exclusive`.
- `false` conserva el comportamiento previo porque antes no se estaba activando exclusividad.
- Se mantienen `isDraggable: false` e `isResizable: false`.

#### Modal de firma PDF

Archivo:

```text
src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfSignatureModal.tsx
```

Correccion:

- Se reemplazo string literal:

```ts
creationType: "upload"
```

por:

```ts
creationType: SignatureCreationType.Upload
```

Impacto:

- No cambia el flujo de firma personal.
- No cambia `previewDataUrl`.
- No cambia `imageMimeType`.
- No cambia `imageData`.
- Solo usa el enum esperado por el paquete.

#### Gestion Respuesta Documentos

Archivos:

```text
src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentos.ts
src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentosTable.ts
src/modules/gestionCorrespondencia/hooks/useListaDocumentosRadicadosTreeTable.ts
```

Correcciones:

- `useGestionRespuestaDocumentos` retorna un tipo explicito mutable compatible con `AppUpload`.
- El fallback sin provider usa `files: []` tipado como `AppUploadFile[]`, evitando `readonly []`.
- `setFiles` conserva firma `(files: AppUploadFile[]) => void`.
- Se agrego `readApiErrorMessage(error: unknown)` para leer `errorMessage` de forma segura porque `ApiResponse.errors` esta tipado como `unknown[]`.
- Se removio `inferColumnsFromRows` no usado en `useListaDocumentosRadicadosTreeTable`.

Impacto:

- No cambia provider.
- No cambia estado de archivos.
- No cambia carga de documentos.
- No cambia mensajes de error; solo se tipa la lectura.

### Resultado de TypeScript

Despues de estas correcciones:

```bash
npx tsc -b
```

Resultado:

```text
sin errores
```

Esto actualiza la situacion previa documentada, donde `npm run build` quedaba bloqueado por deuda TypeScript. La deuda listada fue corregida para los archivos detectados en `tsc -b`.

## Ajuste visual final del shell dashboard

Archivo:

```text
src/modules/dashboard/style/side.module.css
```

Cambio aplicado:

```css
.sider {
  background: #ffffff;
}
```

Motivo:

- El navbar actual usa fondo blanco.
- Se solicito que el sidebar quedara con el mismo color del navbar.
- El ajuste homologa ambas superficies del shell principal sin introducir una nueva paleta ni tocar el markup.

Alcance tecnico:

- Solo se modifica CSS del sidebar.
- No se modifica `DashboardLayout.tsx`.
- No se modifica `Sidebar.tsx`.
- No se modifica `Navbar.tsx`.
- No se cambia `collapsed`, `drawerOpen`, `openKeys`, `selectedKeys` ni `onMenuClick`.
- No se cambia la carga del menu, metricas, badges ni rutas.

Impacto visual:

- El sidebar pasa de `#f8fafc` a `#ffffff`.
- Navbar y sidebar quedan alineados por color base.
- Se conservan radio, sombra, logo, items, hover, seleccion y scroll existentes del sidebar.
