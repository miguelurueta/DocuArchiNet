# SCRUMCORE-209 — Arquitectura Técnica

## Archivos impactados
- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx`
- `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfPasswordPrompt.tsx`
- `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfPasswordPrompt.module.css`
- `src/app/Components/UI/AppVisorEmbedPdf/hooks/useDemoPdfUrl.ts`
- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`

## Arquitectura (alto nivel)
```mermaid
flowchart TD
  A[Pdfium Engine] --> B[EmbedPDF Host]
  B --> C[DocumentManager Plugin]
  C --> D[openDocumentUrl / retryDocument]
  C --> E[onDocumentError (PdfErrorCode.Password)]
  D --> F[OpenDocumentResponse.task.wait()]
  F --> G[Viewport + Scroller + RenderLayer]
  E --> H[AppPdfPasswordPrompt (overlay)]
  H --> D
```

## Flujo de password (secuencia)
```mermaid
sequenceDiagram
  participant U as Usuario
  participant V as AppVisorEmbedPdf
  participant DM as DocumentManager
  participant T as OpenDocumentResponse.task

  V->>DM: openDocumentUrl({ url })
  DM-->>V: response { documentId, task }
  V->>T: task.wait()
  alt documento requiere password o password inválida
    DM-->>V: onDocumentError(PdfErrorCode.Password)
    V-->>U: muestra prompt
    U->>V: submit password
    V->>DM: retryDocument(documentId, { password })
    DM-->>V: response { documentId, task }
    V->>T: task.wait()
  else documento abre correctamente
    T-->>V: resolve
    V-->>U: render normal
  end
```

## Notas técnicas (sostenibilidad)
- El “fin de validación” se determina por `OpenDocumentResponse.task` (no por heurísticas de mensajes).
- Se evita romper reglas de hooks: el prompt es UI presentacional; la orquestación queda en `EmbedPdfDocumentHost`.
- Cleanup: al cambiar `fileUrl` se resetea el estado y se invalidan intentos previos para evitar updates stale.
