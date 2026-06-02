# SCRUMCORE-235 - AppGuideTour - Pruebas

## Unitarias e integracion

Comando ejecutado:

```bash
npm test -- --run src/app/Components/UI/AppGuideTour/tests src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfToolbar.test.tsx src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx
```

Resultado:

```text
6 files passed
22 tests passed
2 skipped heredados
```

Cobertura agregada:

- `AppGuideTour` renderiza sin auto-start por defecto.
- `useAppGuideTour` ejecuta `start`, `stop`, `refresh` y cleanup.
- `DriverJsAdapter` mapea steps a Driver.js y destruye instancia.
- `AppGuideTour.service` filtra steps sin target DOM.
- `AppPdfToolbar` muestra ayuda solo cuando recibe props de guia.
- Click/keyboard sobre ayuda inicia el flujo.
- `AppVisorEmbedPdf` renderiza `AppGuideTour`, expone targets y conecta el boton de ayuda.

## Workbench de documentos

Comando ejecutado durante los refinamientos visuales:

```bash
npm test -- --run src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx
```

Resultado:

```text
1 file passed
6 tests passed
```

Cobertura agregada:

- El visor embebido recibe `onEmptyDocumentHintRequest`.
- El click sobre el icono de ayuda del estado vacio activa `data-document-hint-active="true"` en el listado.
- El hint es visual y no reemplaza el flujo real de seleccion de fila.

Validacion enfocada posterior:

```bash
npm test -- --run src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx
```

Resultado:

```text
2 files passed
22 tests passed
2 skipped heredados
```

## Playwright

Smoke del tour:

```bash
npm run test:e2e -- playwright/appvisorEmbedPdfGuideTour.spec.ts
```

Resultado:

```text
1 passed
```

Valida:

- boton ayuda visible.
- `title` accesible.
- apertura del tour.
- navegacion siguiente/anterior.
- cierre con Escape.
- finalizacion.
- responsive desktop, tablet y mobile.

Regresion visor PDF:

```bash
npm run test:e2e -- playwright/appvisorEmbedPdfZoom.spec.ts playwright/appvisorEmbedPdfThumbnails.spec.ts playwright/appvisorEmbedPdfRotate.spec.ts playwright/appvisorEmbedPdfPrintExport.spec.ts
```

Resultado:

```text
4 passed
```

Valida que zoom, thumbnails, rotate, print y export siguen funcionando.

## Build

Comando ejecutado:

```bash
npm run build
```

Resultado:

```text
failed
```

El build queda bloqueado por deuda TypeScript preexistente en archivos fuera del alcance principal del cambio, incluyendo `AppEditor`, `gestionCorrespondencia`, `pluginRegistration` y errores previos de `AppVisorEmbedPdf`. Tambien se corrigieron los errores introducidos por SCRUMCORE-235 detectados en el primer intento: sintaxis del adapter no compatible con `erasableSyntaxOnly` y scope de `guideTourRef`.

La evidencia funcional del ticket queda cubierta por Vitest focused y Playwright focused/regresion.

## Validacion posterior de TypeScript y regresion focalizada

Despues de corregir la deuda TypeScript detectada en `AppEditor`, `AppVisorEmbedPdf`, plugins del visor, modal de firma PDF y hooks de Gestion Correspondencia, se ejecuto nuevamente el typecheck completo:

```bash
npx tsc -b
```

Resultado:

```text
passed
sin errores TypeScript
```

Errores corregidos por categoria:

- `AppEditor`: variables/imports sin uso, narrowing incorrecto a `never` y compatibilidad defensiva con `transaction.mapping.setMirror`.
- `AppVisorEmbedPdf`: compatibilidad de `Task<boolean>` con espera tipo void, conversion segura de buffers PDF a `BlobPart`, firma actual de `saveAsCopy()` y callback de rotacion no usado.
- `pluginRegistration`: propiedad obligatoria `exclusive` en interacciones de firma.
- `AppPdfSignatureModal`: uso de `SignatureCreationType.Upload` en lugar de string literal.
- `gestionCorrespondencia`: fallback mutable de `files`, lectura segura de `errorMessage` y helper no usado removido.

Pruebas focalizadas ejecutadas tras los cambios de visor y documentos:

```bash
npm test -- --run src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentosTable.test.tsx
```

Resultado:

```text
2 files passed
26 tests passed
2 skipped heredados
```

Observaciones de stderr no bloqueantes:

- `NaN is an invalid value for the width css style property.`
- `An update to EmbedPdfLoadedDocumentView inside a test was not wrapped in act(...).`

Ambas advertencias aparecen durante el caso heredado que usa demo PDF cuando `fileUrl` no existe. No bloquearon la suite ni fueron introducidas por la correccion de TypeScript.

Validacion posterior a los ajustes de UI de la tabla:

```bash
npx tsc -b
```

Resultado:

```text
passed
sin errores TypeScript
```

Cambios UI validados por typecheck:

- Ocultar checkboxes de seleccion en `GestionCorrespondencia` manteniendo `rowSelection="single"`.
- Mantener `onSelectionChanged={setSelectedRows}`.
- Cambiar color de fila seleccionada por CSS.
- Cambiar color/peso del texto de fila seleccionada por CSS.
- Quitar outline/radius visual de `ag-cell-focus` para evitar espacios blancos en celdas enfocadas.

## Correccion de skips heredados de firma personal

Se reemplazaron los 2 `it.skip` heredados de `AppVisorEmbedPdf.test.tsx` por pruebas activas y se agrego cobertura especifica para el hook de firma personal del workflow. Esta correccion no cambia logica productiva del visor; solo cambia pruebas y documentacion.

Pruebas agregadas/activadas:

- `AppVisorEmbedPdf.test.tsx`: la pestaña `Firma personal` renderiza la firma descargada, pide `load()` al entrar, permite usar la firma y activa el flujo de placement sin exponer el payload binario en pantalla.
- `AppVisorEmbedPdf.test.tsx`: la pestaña `Firma personal` muestra estado de carga y solicita `load()` al entrar.
- `useWorkflowPersonalSignature.test.tsx`: carga metadata, descarga blob, crea `blobUrl`, expone `imageData` y metadata normalizada.
- `useWorkflowPersonalSignature.test.tsx`: si la descarga temporal retorna 404, reintenta metadata y descarga una vez mas.
- `useWorkflowPersonalSignature.test.tsx`: `clear()` revoca el object URL y vuelve el hook a estado `idle`.

Validacion enfocada del visor y hook:

```bash
npm test -- --run src/app/Components/UI/AppVisorEmbedPdf/hooks/useWorkflowPersonalSignature.test.tsx src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx
```

Resultado:

```text
2 files passed
21 tests passed
0 skipped
```

Validacion enfocada del visor, hook y tabla de Gestion Correspondencia:

```bash
npm test -- --run src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx src/app/Components/UI/AppVisorEmbedPdf/hooks/useWorkflowPersonalSignature.test.tsx src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentosTable.test.tsx
```

Resultado:

```text
3 files passed
31 tests passed
0 skipped en los archivos ejecutados
```

Typecheck posterior:

```bash
npx tsc -b
```

Resultado:

```text
passed
sin errores TypeScript
```

Limpieza posterior de advertencias del caso heredado de demo PDF:

- Se completo el mock de `Scroller` con `width`, `height`, `rotatedWidth` y `rotatedHeight`, reflejando el contrato que consume el render real de paginas.
- Se espero el render del caso demo PDF con `waitFor` para que la actualizacion async quede cubierta por Testing Library.

Resultado posterior: la suite enfocada pasa sin las advertencias `NaN width` ni `act(...)`.

Detalle tecnico de aplicacion:

- Archivo: `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`.
- Alcance: solo test harness.
- Cambio 1: el mock de `Scroller` dejo de pasar unicamente `pageIndex` y ahora entrega dimensiones finitas de pagina (`612 x 792`) junto con dimensiones rotadas.
- Cambio 2: el test `usa el demo pdf cuando fileUrl no existe` paso de asercion sincronica a espera async con `waitFor`.
- Motivo: JSDOM no calcula layout real y los mocks deben entregar el contrato minimo que consume el componente para evitar estados imposibles en pruebas.
- Impacto funcional: ninguno en runtime; no se tocaron componentes, hooks, plugins ni permisos.

Validacion puntual:

```bash
npm test -- --run src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx
```

Resultado:

```text
1 file passed
18 tests passed
sin warnings NaN width ni act(...)
```
