# SCRUMCORE-207 — Arquitectura Técnica

## Registro de plugins

- `PrintPluginPackage` (React) + `ExportPluginPackage` (React) se registran en `pluginRegistration.ts`.

```mermaid
flowchart LR
  UI[AppPdfToolbar] -->|onPrint/onExport| V[AppVisorEmbedPdf]
  V --> P[usePrint(documentId)]
  V --> E[useExport(documentId)]
  P --> PP[PrintPluginPackage]
  E --> EP[ExportPluginPackage]
```

