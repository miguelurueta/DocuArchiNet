# SCRUMCORE-209 — Comportamiento del Componente

## Estados principales
- `engine loading`: muestra `EngineLoadingState`.
- `engine error`: muestra `ErrorState`.
- `empty`: cuando no hay `fileUrl` y no hay demo (no esperado) muestra `EmptyState`.
- `document loading`: estado normal del visor (existente).
- `password required / invalid password`:
  - Overlay `AppPdfPasswordPrompt` visible.
  - `isLoading=true` sólo mientras se espera `OpenDocumentResponse.task`.
  - `isInvalidPassword=true` cuando el intento anterior incluyó password y `DocumentManager` reporta `PdfErrorCode.Password`.
- `success`:
  - cuando el documento queda activo (`activeDocumentId`) se cierra el prompt y se limpia el estado de error.

## Reintentos (sin bloqueo)
- Se permite reintentar con la misma contraseña (mismo string) incrementando un contador de intento interno.
- Si un intento falla, se vuelve a habilitar el input y se muestra invalid password.

## Cleanup / memory safety
- Al cambiar `fileUrl` se resetea estado de password prompt y refs asociadas.
- Se evita que tasks previas actualicen estado al cambiar documento (guardrails con refs).
