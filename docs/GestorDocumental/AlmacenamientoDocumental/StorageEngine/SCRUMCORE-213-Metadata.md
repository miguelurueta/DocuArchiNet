# SCRUMCORE-213 — Metadata

## Objetivo

Actualización visual del tab **“Firma personal”** en el modal de firmas del visor `AppVisorEmbedPdf`:

- Mostrar preview de la imagen (PNG) descargada (sin mostrar URL/`blob:`).
- Dejar un único CTA: **“Usar firma”** (eliminar “Usar firma personal”).
- Asegurar UX consistente al reabrir el modal (no quedar “pegado” en tabs anteriores).

## Alcance técnico

- UI/CSS: `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfSignatureModal.tsx`
- Estilos: `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfSignatureModal.module.css`
- Tests: `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`

## Restricciones

- `DocumentosWorkbench` permanece limpio (sin estados/lógica).
- No se modifica el pipeline de plugins EmbedPDF (solo UI).
- No se cambia el contrato SCRUM-201 (UrlTemporal no se manipula).

## Notas de UX implementadas

- Al abrir el modal, el tab se restablece a “Dibujar firma” y se limpia el estado de “Firma personal” para evitar estados stale entre aperturas.
