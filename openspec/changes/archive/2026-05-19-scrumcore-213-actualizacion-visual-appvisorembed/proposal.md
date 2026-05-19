## Why

El tab **"Firma personal"** del modal de firmas en `AppVisorEmbedPdf` actualmente consume la firma temporal y permite usarla, pero la UX no es óptima:

- Se debe **previsualizar la imagen (PNG) descargada** dentro del tab (no mostrar URLs/strings `blob:`).
- Se debe simplificar la acción a **un único botón**: "Usar firma" (eliminar "Usar firma personal").

El cambio debe ser estrictamente **visual/UI** y mantenerse enterprise: sin mover lógica al Workbench y sin afectar el pipeline de firmas oficial de EmbedPDF.

## What Changes

- Ajustar el tab **"Firma personal"** del modal de firmas para:
  - Renderizar un preview enterprise de la firma descargada (PNG) usando el `ObjectURL` existente.
  - No renderizar la URL (ni `blob:` ni `UrlTemporal`) en la UI.
  - Reemplazar el botón "Usar firma personal" por un único botón "Usar firma".
- Mantener:
  - Encapsulación en `AppVisorEmbedPdf` (Workbench permanece limpio).
  - Contrato SCRUM-201 (UrlTemporal sin manipulación de token).
  - Cleanup de `ObjectURL` y performance (sin wrappers extra).

## Capabilities

### New Capabilities
- `actualizacion-visual-appvisorembed`: UI enterprise para preview de "Firma personal" + botón único "Usar firma".

### Modified Capabilities
-

## Impact

- Impacto acotado al visor:
  - `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfSignatureModal.tsx`
  - `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfSignatureModal.module.css`
  - Tests del visor y documentación enterprise.
