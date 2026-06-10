# AppProgressBatch - Diagrama de componentes

## Proposito

Mostrar como se compone internamente `AppProgressBatch` y como interactua con consumidores externos.

```mermaid
flowchart TB
  subgraph UI["AppProgressBatch UI"]
    Modal["Dialog modal accesible"]
    Header["Header de proceso"]
    Progress["Progress global"]
    ItemStatus["Item actual y fase"]
    Actions["Acciones: cancelar, continuar, cerrar"]
    SummaryView["Resumen final"]
  end

  subgraph State["Estado interno"]
    Lifecycle["lifecycle state"]
    Counters["contadores y resumen"]
    Abort["AbortController"]
    Current["item actual, label, phase"]
  end

  subgraph API["Contrato externo"]
    Props["Props controladas"]
    ProcessItem["processItem(item, ctx)"]
    Events["Eventos externos"]
  end

  subgraph Consumer["Consumidor"]
    DomainItems["items del dominio"]
    DomainOperation["operacion concreta"]
    Refresh["actualizar UI del modulo"]
  end

  Props --> Modal
  Modal --> Header
  Modal --> Progress
  Modal --> ItemStatus
  Modal --> Actions
  Modal --> SummaryView

  Actions --> Lifecycle
  Progress --> Counters
  ItemStatus --> Current
  Lifecycle --> Abort

  DomainItems --> Props
  DomainOperation --> ProcessItem
  ProcessItem --> Lifecycle
  ProcessItem --> Counters
  Events --> Refresh
```

## Principios

- La UI no ejecuta logica de negocio directamente.
- El estado interno solo conoce progreso y ciclo de vida.
- El consumidor decide que significa procesar un item.
- Los eventos devuelven control al modulo consumidor.
