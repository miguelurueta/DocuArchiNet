# SCRUMCORE-211 — Metadata

## Alcance

Agregar pestaña **Firma personal** en el modal de firmas del visor `AppVisorEmbedPdf`, consumiendo API temporal (contrato SCRUM-201) y manteniendo el Workbench libre de lógica EmbedPDF.

## Componentes impactados

- `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfSignatureModal.tsx`
- `src/app/Components/UI/AppVisorEmbedPdf/hooks/useWorkflowPersonalSignature.ts`
- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`

## API / Seguridad

- Endpoints:
  - `GET /api/workflow/usuarios/firma-temporal`
  - `GET /api/workflow/usuarios/firma-temporal/download/{token}`
- Header requerido:
  - `Authorization: Bearer <JWT>`
- Claims requeridos por backend (contrato):
  - `defaulaliaswf`
  - `IdUsuarioWorkflow`

## Estado actual observado (entorno local)

- El backend responde `400` con mensaje:
  - `"No se encontró el claim 'IdUsuarioWorkflow'."`
- Esto indica que el JWT de sesión no contiene el claim esperado por la API (o el backend espera un nombre distinto).

## Evidencias

- Unit tests: `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx` incluye escenarios `[SPEC:SCRUMCORE-211]` (mocks).

