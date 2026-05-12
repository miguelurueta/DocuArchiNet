# SCRUMCORE-209 — Testing Enterprise

## Unit / Integration (Vitest + React Testing Library)
Archivo:
- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`

Ejecución local (2026-05-12):
- Comando: `npm.cmd test -- --run src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`
- Resultado: `12 passed`
- Observación: warning no bloqueante en stderr sobre `NaN` como valor inválido de `width` CSS.

Escenarios mínimos:
- Renderiza overlay de password cuando `DocumentManager` reporta `PdfErrorCode.Password`.
- Reintento por `retryDocument` permite segundo submit sin quedar bloqueado en “Validando…”.
- Contraseña válida cierra prompt al activarse el documento.

## E2E (Playwright)
- Recomendado para validar PDFs protegidos reales (happy path + invalid password + retry).
- Evidencias (traces/videos) deben adjuntarse en `test-results/` según pipeline.
