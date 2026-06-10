# SCRUMCORE-211 — Arquitectura Técnica

## Responsabilidades

- `AppVisorEmbedPdf` mantiene encapsulación de plugins y estado del visor.
- `AppPdfSignatureModal` contiene UI del modal y tabs.
- `useWorkflowPersonalSignature` encapsula consumo de API temporal y lifecycle de ObjectURL.
- `DocumentosWorkbench` no conoce la lógica (permanece limpio).

## Flujo técnico (Firma personal)

```mermaid
sequenceDiagram
  participant UI as AppPdfSignatureModal
  participant Hook as useWorkflowPersonalSignature
  participant API as Workflow API
  participant Embed as EmbedPDF Signature Plugin

  UI->>Hook: load() (al entrar a tab)
  Hook->>API: GET /api/workflow/usuarios/firma-temporal (Bearer JWT)
  API-->>Hook: { success, data.UrlTemporal, ExpiresAt }
  Hook->>API: GET {UrlTemporal} (Bearer JWT) (blob)
  alt 404 token expirado
    Hook->>API: GET metadata (refresh)
    Hook->>API: GET {UrlTemporal} (retry 1 vez)
  end
  Hook-->>UI: blobUrl + imageData (ArrayBuffer)
  UI->>Embed: preparar SignatureStampFieldDefinition (previewDataUrl + imageData)
  UI->>Embed: placement standard en el PDF
```

## Puntos de performance / memory

- Se utiliza `URL.createObjectURL(blob)` para preview.
- Se revoca con `URL.revokeObjectURL` al cerrar modal, al reintentar o al “usar firma”.

