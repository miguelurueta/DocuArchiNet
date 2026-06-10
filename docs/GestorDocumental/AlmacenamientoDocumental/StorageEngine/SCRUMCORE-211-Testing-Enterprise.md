# SCRUMCORE-211 — Testing Enterprise

## Unit / Integration (Vitest + RTL)

- Archivo: `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`

Cobertura de escenarios (mocks):

- `[SPEC:SCRUMCORE-211]` renderiza pestaña “Firma personal” y permite “Usar firma personal”.
- `[SPEC:SCRUMCORE-211]` si download responde `404`, reintenta metadata + download una sola vez.
- Verificación de cleanup: revocación de `ObjectURL`.

## Notas

Los tests no hacen red real; mockean `clienteApi.get` y validan el comportamiento observable.

