# SCRUMCORE-210 — Testing Enterprise

## Unit testing

- Archivo: `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`
- Casos sugeridos/actualizados (mínimo):
  - Render toolbar + modal.
  - Upload: muestra nombre archivo + botón “Reemplazar firma”.
  - Draw: botón “Limpiar” aparece tras dibujar.
  - Borrado: botón habilita solo con firma seleccionada.

## Integration testing

- Validar interacción modal → placement y que el visor no crashea por hooks/state.
- Validar export/print tras borrar firma (commit previo).

## E2E (Playwright)

- Instalar/usar Playwright si la pipeline lo requiere.
- Escenarios:
  - Abrir página de prueba del visor.
  - Dibujar firma → usar → colocar en PDF.
  - Eliminar firma seleccionada.
  - Exportar y validar que el PDF exportado no incluya firmas eliminadas.

## Re-render testing

- Objetivo: toolbar no debe re-renderizar por scroll.
- Estrategia: instrumentación/contador de renders en entorno de test (sin contaminar prod).

