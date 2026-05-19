# SCRUMCORE-213 — Arquitectura Técnica

## Responsabilidades

- `useWorkflowPersonalSignature`: descarga `Blob` y crea `ObjectURL` (lifecycle controlado).
- `AppPdfSignatureModal`: renderiza preview y dispara placement vía `onStartPlacement`.
- Workbench: sin cambios.

## Flujo (UI simplificada)

```mermaid
sequenceDiagram
  participant UI as AppPdfSignatureModal (tab Firma personal)
  participant Hook as useWorkflowPersonalSignature
  participant API as Workflow API
  participant Embed as EmbedPDF Signature Plugin

  UI->>Hook: load() (al entrar al tab)
  Hook->>API: metadata + download (Bearer)
  Hook-->>UI: blobUrl + imageData
  UI-->>UI: render <img alt="Firma personal">
  UI->>Embed: click "Usar firma" -> onStartPlacement(stamp)
```

