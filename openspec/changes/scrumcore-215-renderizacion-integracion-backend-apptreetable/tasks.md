## Tasks

- [ ] Refinar contrato objetivo (confirmar endpoints `query/action` + `visualizacion/resolve`, `TableId` y `ViewMode` soportados) y documentarlo en el spec del cambio.
- [ ] Crear DTOs tipados para wrapper `success/message/data/errors` y para `Rows[]` (`Values`, `Meta`) en el módulo consumidor (sin `any`).
- [ ] Implementar servicio `query` (POST) con payload mínimo recomendado y soporte de flags (`EnablePagination`, `EnableColumnFilters`, `IncludeConfig`).
- [ ] Implementar servicio `query` jerárquico para hijos (POST) usando `ParentRowId`, `ParentNodeType`, `Level`.
- [ ] Implementar servicio `action` (POST) soportando al menos `ver_documento` y el mapping a `DocumentResolveRequest`.
- [ ] Implementar servicio `visualizacion/resolve` (POST) invocado directamente desde frontend (sin API->API).
- [ ] Implementar adaptador `Rows[] -> AppTreeTableRow[]` (incluye `hasChildren` y meta de dominio para acciones).
- [ ] Extender `AppTreeTable` para soportar lazy-load de hijos al expandir (API de callbacks) sin romper el modo actual `rows` / `load()`.
- [ ] Integrar `AppTreeTable` en `DocumentosWorkbench` “Listado” usando `load()` real y handlers de expand/acción (sin tocar `AppVisorEmbedPdf`).
- [ ] Manejo de errores:
  - [ ] HTTP no-2xx -> error técnico + reintento opcional
  - [ ] `success=false` -> error funcional desde `errors[0].errorMessage`/`message`
- [ ] Tests (Vitest/RTL):
  - [ ] `AppTreeTable`: carga inicial + estado loading/empty/error
  - [ ] `AppTreeTable`: expand dispara lazy-load y renderiza hijos
  - [ ] Workbench: monta listado con hook mock sin afectar visor
  - [ ] Acción `ver_documento`: action -> resolve (mocks) y validación de payload
- [ ] Documentación enterprise del ticket:
  - [ ] Metadatos (branch/commit/tests ejecutados)
  - [ ] Contrato consumido (request/response + reglas)
  - [ ] Decisiones y trade-offs (por qué UI vs services)

