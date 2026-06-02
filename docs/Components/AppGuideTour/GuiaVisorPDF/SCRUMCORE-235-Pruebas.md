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
