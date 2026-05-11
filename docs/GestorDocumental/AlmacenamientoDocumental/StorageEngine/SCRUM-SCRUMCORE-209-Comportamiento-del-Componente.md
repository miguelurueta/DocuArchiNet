# SCRUMCORE-209 — Comportamiento del Componente

## Estados principales
- `engine loading`: se mantiene `EngineLoadingState`.
- `document loading`: se mantiene `DocumentLoadingState`.
- `password required / invalid password`:
  - Overlay `AppPdfPasswordPrompt` visible.
  - `isLoading=true` solo mientras el `task` de carga está ejecutándose.
  - `isInvalidPassword=true` cuando el último intento incluyó password y el DocumentManager reporta `PdfErrorCode.Password`.
- `success`:
  - `response.task` resuelve → se cierra el prompt y se habilita el visor.
- `error`:
  - se mantiene `ErrorState` existente (sin romper UX del visor).

## Lifecycle y cleanup
- Al cambiar `fileUrl`, se resetea el estado del prompt.
- Los callbacks de tasks se cancelan en cleanup para evitar `setState` en unmount.

## Performance
- No se agregan listeners manuales de scroll ni cálculos de páginas.
- No se afectan plugins existentes (zoom/rotate/thumbnails/pagination/print/export).

