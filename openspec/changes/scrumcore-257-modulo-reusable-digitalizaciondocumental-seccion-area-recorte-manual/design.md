## Context

SCRUMCORE-257 agrega seleccion visual de area y recorte manual al modulo reutilizable de Digitalizacion Documental.

El preview ya soporta:

- Captura y visualizacion de paginas.
- Miniaturas laterales.
- Organizador de paginas como overlay.
- Rotacion, eliminacion, reordenamiento y zoom.

## Goals / Non-Goals

**Goals**

- Agregar boton `Seleccionar area` al toolbar del Preview PDF.
- Dibujar seleccion rectangular con eventos pointer sobre la pagina visible.
- Almacenar `x`, `y`, `width`, `height` respecto a dimensiones reales de pagina.
- Ejecutar recorte solo sobre la pagina activa usando `DWT.Crop`.
- Invalidar el PDF pendiente sin reconstruir todo el lote.
- Documentar arquitectura, coordenadas, riesgos y evidencia visual.

**Non-Goals**

- No crear modal.
- No reemplazar preview, miniaturas, scanner ni organizador.
- No implementar OCR, firmas, sellos, anotaciones ni redaccion de datos.
- No redimensionar/mover una seleccion ya cerrada en esta fase.

## Decisions

1. La seleccion vive en `DigitalizacionDocumentalWorkspace`, porque ahi ya estan el preview, zoom, pagina activa y toolbar.
2. El rectangulo visual se renderiza dentro de una superficie `position: relative` alrededor de la imagen, sin desmontar la imagen ni el preview.
3. La fuente de verdad del recorte es `PageCropSelection` en coordenadas reales de pagina, no coordenadas de pantalla ni zoom.
4. `useDigitalizacionScanner` expone `cropPage(pageId, selection)` para mantener el contrato de operaciones junto a rotar, eliminar y reordenar.
5. `DynamsoftTwainClient.cropPage` usa `DWT.Crop(index, left, top, right, bottom)` y refresca solo la pagina afectada.
6. Al aplicar crop se limpia `scanner.pdf` para obligar a regenerar el PDF pendiente con la pagina actualizada.

## Risks / Trade-offs

- Si el runtime Dynamsoft no expone `Crop`, se devuelve error controlado y no se modifica `scanner.pages`.
- En paginas sin `width`/`height`, el preview usa el rect DOM como fallback para la conversion visual.
- La seleccion se reinicia al aplicar, cancelar o cambiar el modo; la edicion avanzada de rectangulos queda para una fase futura.

## Migration Plan

1. Extender tipos `PageCropSelection` y `DigitalizacionScannerClient`.
2. Implementar `cropPage` en hook y cliente Dynamsoft.
3. Agregar UI de seleccion y acciones en el preview existente.
4. Agregar pruebas de UI y de cliente Dynamsoft.
5. Documentar en `docs/Architecture/DigitalizacionDocumental/SCRUMCORE-269-selection-crop.md`.

## Validation

- `npx tsc --noEmit`
- `npx eslint <archivos afectados>`
- `npm test -- --run src/app/Components/UI/AppDigitalizador/tests/AppDigitalizador.test.tsx src/modules/digitalizacion/tests/DynamsoftTwainClient.test.ts`
