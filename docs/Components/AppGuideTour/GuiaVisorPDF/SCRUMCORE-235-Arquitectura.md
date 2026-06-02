# SCRUMCORE-235 - AppGuideTour - Arquitectura

## Objetivo

`AppGuideTour` introduce una capa reusable de guia interactiva basada en Driver.js. La primera integracion real es `AppVisorEmbedPdf`, donde se agrega un boton de ayuda en `AppPdfToolbar` para iniciar un recorrido sobre controles existentes del visor.

El cambio mantiene Driver.js aislado del visor PDF. `AppVisorEmbedPdf` solo conoce el componente reusable, el ref publico y la lista de steps configurada.

## Componentes

| Elemento | Responsabilidad |
| --- | --- |
| `AppGuideTour` | Componente headless que expone `start`, `stop` y `refresh` por ref. |
| `useAppGuideTour` | State machine minima, filtrado de targets y emision de eventos. |
| `DriverJsAdapter` | Unico archivo que importa `driver.js` y su CSS. |
| `AppGuideTour.service` | Filtra steps con targets DOM faltantes y normaliza eventos. |
| `AppPdfToolbar` | Renderiza controles reales, atributos `data-guide-tour-id` y boton de ayuda opcional. |
| `AppVisorEmbedPdf.guideTour` | Configuracion de steps del visor PDF. |

## Flujo

1. El usuario activa el boton `Guia interactiva` en el toolbar.
2. `AppPdfToolbar` ejecuta `onStartGuideTour`.
3. `AppVisorEmbedPdf` llama `guideTourRef.current.start()`.
4. `AppGuideTour` delega en `useAppGuideTour`.
5. El hook filtra steps cuyos selectores no existan en el DOM.
6. `DriverJsAdapter` mapea los steps a Driver.js.
7. Driver.js renderiza el overlay y controla siguiente/anterior/finalizar/cierre.
8. El hook emite eventos no sensibles.

## classDiagram

```mermaid
classDiagram
  class AppVisorEmbedPdf {
    +guideTourRef
    +onStartGuideTour()
    +onGuideTourEvent()
  }
  class AppPdfToolbar {
    +onStartGuideTour?()
    +isGuideTourAvailable?
  }
  class AppGuideTour {
    +tourId
    +steps
    +start()
    +stop()
    +refresh()
  }
  class useAppGuideTour {
    +state
    +currentStepId
    +start()
    +stop()
    +refresh()
  }
  class DriverJsAdapter {
    +start(steps)
    +stop()
    +refresh()
    +destroy()
  }
  class DriverJS

  AppVisorEmbedPdf --> AppPdfToolbar
  AppVisorEmbedPdf --> AppGuideTour
  AppGuideTour --> useAppGuideTour
  useAppGuideTour --> DriverJsAdapter
  DriverJsAdapter --> DriverJS
```

## sequenceDiagram

```mermaid
sequenceDiagram
  participant U as Usuario
  participant TB as AppPdfToolbar
  participant V as AppVisorEmbedPdf
  participant GT as AppGuideTour
  participant H as useAppGuideTour
  participant A as DriverJsAdapter
  participant D as Driver.js

  U->>TB: Click Guia interactiva
  TB->>V: onStartGuideTour()
  V->>GT: ref.start()
  GT->>H: start()
  H->>H: filterVisibleGuideTourSteps()
  H->>A: start(validSteps)
  A->>D: driver(config).drive()
  D-->>U: Overlay del tour
  D-->>A: onHighlighted/onDestroyed
  A-->>H: onStepChange/onCompleted/onCancelled
  H-->>V: onEvent(event)
```

## sequenceDiagram - Hint de seleccion de documento

```mermaid
sequenceDiagram
  participant U as Usuario
  participant E as EmptyState
  participant V as AppVisorEmbedPdf
  participant W as DocumentosWorkbench
  participant L as Listado AG Grid

  U->>E: Click icono documento/flecha
  E->>V: onDocumentHintRequest()
  V->>W: onEmptyDocumentHintRequest()
  W->>W: setCollapsed(false)
  W->>W: setDocumentHintActive(true)
  W->>L: data-document-hint-active=true
  L-->>U: Titileo primera fila completa
  W->>W: timeout 1600ms
  W->>L: data-document-hint-active=false
```

## stateDiagram-v2

```mermaid
stateDiagram-v2
  [*] --> idle
  idle --> loading: start()
  loading --> running: valid targets
  loading --> error: no valid targets
  running --> completed: final step done
  running --> cancelled: Escape / close / stop()
  completed --> idle: cleanup
  cancelled --> idle: cleanup
  error --> idle: future retry
```

## Restricciones cumplidas

- Driver.js solo se importa en `src/app/Components/UI/AppGuideTour/drivers/DriverJsAdapter.ts`.
- El visor no modifica logica de zoom, rotate, print, export, firmas, thumbnails ni scroll.
- Los consumidores del toolbar siguen siendo compatibles porque las props de guia son opcionales.
- Search, Fit Width y Fit Page no se agregan porque no existen como botones actuales en `AppPdfToolbar`.
