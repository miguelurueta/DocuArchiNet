# SCRUMCORE-213 — Testing Enterprise

## Unit/Integration (Vitest + RTL)

- Archivo: `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`

Validaciones:

- En `ready` existe `<img alt="Firma personal">`.
- No existe botón “Usar firma personal”.
- Click en “Usar firma” inicia placement (mocks) sin pasos extra.
- No se renderiza texto `blob:`.

