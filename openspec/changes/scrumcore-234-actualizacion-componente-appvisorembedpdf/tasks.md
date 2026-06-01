## 1) Refinamiento (pre‑implementación)

- [x] 1.1 Confirmar alcance: auto‑fit determinístico (NO heurísticas OCR/imagen/ML).
- [x] 1.2 Definir política exacta: `fitMode` default (`width` vs `page`) + cuándo reaplicar en resize.
- [x] 1.3 Acordar “no pelear con el usuario”: auto‑fit apply-once post‑ready + criterio de `userZoomDirty` (sin toggle/manual UI).
- [x] 1.4 Identificar APIs reales disponibles en EmbedPDF (zoom/viewport/page size) para evitar supuestos.

## 2) Diseño (artefactos)

- [x] 2.1 Actualizar `design.md` con:
  - arquitectura propuesta (módulo `autoFit/`),
  - concurrencia/stale‑ignore (por `documentId`/`loadSeq`),
  - puntos de integración (post‑ready, resize, eventos de zoom).
- [x] 2.2 Actualizar `spec.md` con requisitos verificables + criterios de aceptación.

## 3) Implementación (código)

- [x] 3.1 Crear módulo `src/app/Components/UI/AppVisorEmbedPdf/autoFit/` (types/math/controller/apply).
- [x] 3.2 Integrar auto‑fit post‑ready en `AppVisorEmbedPdf` (solo una vez por load si aplica).
- [x] 3.3 Decisión de cierre: NO agregar handler de resize; el visor ya funciona correctamente y no se introduce funcionalidad adicional.
- [x] 3.4 Decisión de cierre: NO instrumentar `userZoomDirty`; se conserva el comportamiento actual para no alterar UX ya validada.
- [x] 3.5 Stale‑safe: ignorar auto‑fit si cambió `documentId/loadSeq` o si se canceló el load.
- [x] 3.6 Ajustar render por página para usar slot rotado (`rotatedWidth/rotatedHeight`) y evitar clipping en rotación metadata/manual.
- [x] 3.7 Integrar `SelectionLayer` dentro de `PagePointerProvider` para selección de texto en PDF.
- [x] 3.8 Registrar `InteractionManagerPluginPackage` y `SelectionPluginPackage` desde entradas React oficiales (`/react`) para habilitar copy-to-clipboard.
- [x] 3.9 Agregar menú contextual `Copy` y soporte `Ctrl/Cmd+C` usando `selection.provides.forDocument(documentId).copyToClipboard()`.
- [x] 3.10 Ajustar CSS para evitar drag del bitmap renderizado y priorizar la interacción sobre `SelectionLayer`.

## 4) Pruebas

- [x] 4.1 Unit: `computeFitScale()` (`width`/`page`) + guards (`userZoomDirty`, stale).
- [x] 4.2 Integración React: se descarta agregar/tocar toggle o resize; comportamiento actual validado sin nueva funcionalidad.
- [x] 4.3 Manual QA checklist: validación funcional actual confirmada por usuario (visor funcionando OK).
- [x] 4.4 Playwright: evaluado como no aplicable en este cierre; no se agrega harness ni smoke nuevo para evitar cambios fuera de alcance.

## 5) Documentación y cierre

- [x] 5.1 Crear docs enterprise en `docs/Components/AppTable/Auto-Fit/` (estructura SCRUMCORE‑234).
- [x] 5.2 Registrar evidencia de pruebas ejecutadas (comandos + resultados).
- [x] 5.3 Documentar minuciosamente zoom, rotación, render por página, selección de texto, plugin selection React y flujo copy-to-clipboard.
- [x] 5.4 Crear copia canonical de documentación en `docs/Components/AppVisorEmbedPdf/Auto-Fit/` y espejo en `docs/Components/AppTable/Auto-Fit/`.
- [x] 5.5 Crear commit de documentación y ajustes relacionados: `8475bfc SCRUMCORE-234: document zoom and selection updates`.
- [ ] 5.6 `opsxj:archive` + PR + cierre Jira cuando esté mergeado.
