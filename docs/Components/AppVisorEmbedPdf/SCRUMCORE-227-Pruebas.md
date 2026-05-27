# SCRUMCORE-227 — AppVisorEmbedPdf.load() (Pruebas)

## Unit / integración (Vitest)
- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`
  - Se actualizaron mocks por nuevos capabilities (`useAnnotationCapability`, `useSelectionCapability`).
  - Se añadieron props nuevas en toolbar (disabled flags).
  - Nota: 2 tests de “Firma personal” quedaron en `it.skip` por limitaciones de JSDOM con recursos `blob:` y render de `<img src="blob:...">`.

Comando usado:
`npm test -- src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`

## Casos pendientes
- Rehabilitar tests E2E/UI de “Firma personal” en entorno real (Playwright) donde `blob:` y recursos se comporten como navegador.
- Añadir suite específica de `load()` (permisos resolved/failed + override firmado) una vez se estabilice el mapping `codigoImpl`.

