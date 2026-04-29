## 1. Domain contracts

- [ ] 1.1 Crear `src/app/Components/UI/AppVisorPdf/domain/visorPdfApi.types.ts` con:
  - `VisorPdfStampConfig`
  - interface `AppVisorPdfApi` (m\u00e9todos completos del contrato Jira)
- [ ] 1.2 Reutilizar `VisorPdfAnnotationsPayloadV1` desde `src/app/Components/UI/AppVisorPdf/domain/annotations.types.ts`
- [ ] 1.3 Aclarar alcance: `grafo assets/placements` queda **pendiente/fuera** de este ticket (solo documentaci\u00f3n), sin endpoints implementados

## 2. Infrastructure adapter

- [ ] 2.1 Implementar `src/app/Components/UI/AppVisorPdf/infrastructure/visorPdfApi.ts` usando `src/api/Clienteaxios.ts` (sin endpoints hardcodeados en UI)
- [ ] 2.2 Implementar los 5 m\u00e9todos del contrato (scaffolding listo):
  - [ ] 2.2.1 `getPdfUrl(documentId)`
  - [ ] 2.2.2 `getAnnotations(documentId)`
  - [ ] 2.2.3 `saveAnnotations(documentId, payload)`
  - [ ] 2.2.4 `getStampConfig()`
  - [ ] 2.2.5 `saveStampConfig(payload)`
- [ ] 2.3 Mantener `ApiResponse<T>` como retorno de cada m\u00e9todo (sin normalizar a data plana)
- [ ] 2.4 Propagar errores 401/403 (sin capturarlos y convertirlos a success)
- [ ] 2.5 Manejo 400: mapping m\u00ednimo si aplica, sin acoplarse a shapes no documentados (mantener enfoque de adapter)
- [ ] 2.6 Tipado estricto (sin `any`) en adapter/DTOs

## 3. Tests (unit)

- [ ] 3.1 Configurar mocks con `vi.mock(Clienteaxios)` (patr\u00f3n del repo)
- [ ] 3.2 Tests happy path por m\u00e9todo (validar envelope `ApiResponse<T>`):
  - [ ] 3.2.1 `getPdfUrl` retorna `ApiResponse<{ url; expiresAtIso? }>`
  - [ ] 3.2.2 `getAnnotations` retorna `ApiResponse<VisorPdfAnnotationsPayloadV1>`
  - [ ] 3.2.3 `saveAnnotations` env\u00eda payload correcto y retorna `ApiResponse<{ savedAtIso }>`
  - [ ] 3.2.4 `getStampConfig` retorna `ApiResponse<VisorPdfStampConfig>`
  - [ ] 3.2.5 `saveStampConfig` env\u00eda payload correcto y retorna `ApiResponse<{ savedAtIso }>`
- [ ] 3.3 Tests error path: 401/403 se propagan (reject) para consumo por notificador central

## 4. Documentation

- [ ] 4.1 Actualizar `src/app/Components/UI/AppVisorPdf/README.md` con:
  - Endpoints/operaciones esperadas del backend (los 5 m\u00e9todos)
  - Contratos `ApiResponse<T>` y DTOs (`VisorPdfStampConfig`, `VisorPdfAnnotationsPayloadV1`)
- [ ] 4.2 Documentar ejemplo de mocks (vi.mock Clienteaxios) y manejo de errores 400/401/403
- [ ] 4.3 Documentar expl\u00edcitamente que `grafo assets/placements` queda pendiente/fuera de alcance del ticket 04-FE

