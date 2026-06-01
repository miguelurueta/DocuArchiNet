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
