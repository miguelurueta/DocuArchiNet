## 1) Refinamiento (pre‑implementación)

- [ ] 1.1 Confirmar alcance: auto‑fit determinístico (NO heurísticas OCR/imagen/ML).
- [ ] 1.2 Definir política exacta: `fitMode` default (`width` vs `page`) + cuándo reaplicar en resize.
- [ ] 1.3 Acordar “no pelear con el usuario”: auto‑fit apply-once post‑ready + criterio de `userZoomDirty` (sin toggle/manual UI).
- [ ] 1.4 Identificar APIs reales disponibles en EmbedPDF (zoom/viewport/page size) para evitar supuestos.

## 2) Diseño (artefactos)

- [ ] 2.1 Actualizar `design.md` con:
  - arquitectura propuesta (módulo `autoFit/`),
  - concurrencia/stale‑ignore (por `documentId`/`loadSeq`),
  - puntos de integración (post‑ready, resize, eventos de zoom).
- [ ] 2.2 Actualizar `spec.md` con requisitos verificables + criterios de aceptación.

## 3) Implementación (código)

- [ ] 3.1 Crear módulo `src/app/Components/UI/AppVisorEmbedPdf/autoFit/` (types/math/controller/apply).
- [ ] 3.2 Integrar auto‑fit post‑ready en `AppVisorEmbedPdf` (solo una vez por load si aplica).
- [ ] 3.3 Integrar handler de resize (debounce 50–100ms) solo para métricas/observabilidad; NO re‑auto‑fit por defecto.
- [ ] 3.4 Instrumentar tracking de “zoom manual” para setear `userZoomDirty` (wheel/pinch/buttons) sin loops.
- [ ] 3.5 Stale‑safe: ignorar auto‑fit si cambió `documentId/loadSeq` o si se canceló el load.

## 4) Pruebas

- [ ] 4.1 Unit: `computeFitScale()` (`width`/`page`) + guards (`userZoomDirty`, stale).
- [ ] 4.2 Integración React: load→post‑ready aplica una vez; zoom manual desactiva auto‑fit en resize; toggle reactiva.
- [ ] 4.3 Manual QA checklist (mínimo): portrait/landscape/rotación metadata + rotate manual + thumbnails/scroll.
- [ ] 4.4 (Si aplica) Playwright: smoke de no‑regresión (zoom/rotate/thumbnail) en harness.

## 5) Documentación y cierre

- [ ] 5.1 Crear docs enterprise en `docs/Components/AppTable/Auto-Fit/` (estructura SCRUMCORE‑234).
- [ ] 5.2 Registrar evidencia de pruebas ejecutadas (comandos + resultados).
- [ ] 5.3 `opsxj:archive` + PR + cierre Jira cuando esté mergeado.
