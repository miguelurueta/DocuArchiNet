# SCRUMCORE-209 — Arquitectura Técnica

## Archivos impactados
- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx`
- `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfPasswordPrompt.tsx`
- `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfPasswordPrompt.module.css`
- `src/app/Components/UI/AppVisorEmbedPdf/hooks/useDemoPdfUrl.ts`
- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`

## Diagrama de arquitectura (alto nivel)
```mermaid
flowchart TD
  A[Pdfium Engine] --> B[EmbedPDF Host]
  B --> C[DocumentManager Plugin]
  C --> D[openDocumentUrl / retryDocument]
  C --> E[onDocumentError]
  D --> F[OpenDocumentResponse.task]
  F --> G[Viewport + Scroller + RenderLayer]
  E --> H[Password Prompt Overlay]
  H --> D
```

## Flujo Password (secuencia)
```mermaid
sequenceDiagram
  participant U as Usuario
  participant V as AppVisorEmbedPdf
  participant DM as DocumentManager (EmbedPDF)
  participant T as Task de carga (response.task)

  V->>DM: openDocumentUrl({url})
  DM-->>V: Task resolve {documentId, task}
  V->>T: task.wait(...)
  T-->>DM: (engine carga PDF)
  DM-->>V: onDocumentError(code=Password) si requiere/incorrecta
  V-->>U: muestra prompt
  U->>V: submit password
  V->>DM: retryDocument(documentId,{password})
  DM-->>V: Task resolve {documentId, task}
  V->>T: task.wait(...)
  alt password válida
    T-->>V: resolve
    V-->>U: cierra prompt y habilita visor
  else password inválida
    T-->>V: reject/abort
    V-->>U: deja prompt abierto, muestra inválida, input habilitado
  end
```

## Notas técnicas (sostenibilidad)
- El “success/failure real” se toma de `OpenDocumentResponse.task`, no del task externo de `openDocumentUrl`/`retryDocument`.
- Se usa `onDocumentError` con `PdfErrorCode.Password` para mostrar prompt sin heurísticas de string.
- Cleanup: los handlers del task se “cancelan” al cambiar documento/unmount para evitar stale updates.

