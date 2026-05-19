# SCRUMCORE-214 — Arquitectura Técnica

## Diagrama (alto nivel)

```mermaid
flowchart LR
  DW[DocumentosWorkbench] --> ACR[AppCollapseRail]
  ACR --> ATT[AppTreeTable]
  DW --> AV[AppVisorEmbedPdf]

  ATT -->|rows| UI1[Render Tree]
  ATT -->|load()| UI2[Loading/Empty/Error]
```

## Componentes

- `AppTreeTable`: componente UI reusable (aislado), ubicado en `src/app/Components/UI/AppTreeTable/`.
- `DocumentosWorkbench`: consumidor en `src/modules/gestionCorrespondencia/components/documentosWorkbench/`.

