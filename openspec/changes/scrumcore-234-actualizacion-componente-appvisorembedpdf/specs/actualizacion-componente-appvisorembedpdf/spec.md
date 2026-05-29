# SCRUMCORE-234 — Spec
Actualización del componente `AppVisorEmbedPdf`: Auto‑Fit determinístico (sin heurísticas) sin romper zoom/scroll/viewport.

## 0) Alcance

- IN: auto‑fit (fit-to-width / fit-to-page), centrado y reglas de convivencia con zoom manual.
- OUT: auto‑rotate por “contenido” (OCR/imagen/ML), cambios de backend/endpoints, persistencia de URLs temporales.

## 1) Requisitos funcionales (verificables)

### R1 — Auto‑fit post‑ready (1 vez por carga, si aplica)
**GIVEN** que el visor cargó un documento y el engine confirmó “ready”  
**WHEN** el documento queda usable en el viewport  
**THEN** el visor aplica auto‑fit determinístico (scale + centering) siguiendo `fitMode` por defecto.

Reglas:
- Auto‑fit solo puede “commit” cuando el documento actual sigue siendo el mismo (`documentId`/`loadSeq` vigente).
- Auto‑fit no debe ejecutarse antes del handshake ready.

### R2 — No pelear con el usuario (zoom manual)
**GIVEN** que el usuario realizó zoom manual (wheel/pinch/botones)  
**WHEN** ocurre un resize del viewport o un re-render  
**THEN** el visor NO reaplica auto‑fit automáticamente hasta que:
- el usuario reactive “Smart Fit”, o
- se cargue un documento distinto (nueva sesión de auto‑fit).

### R3 — Resize handling (estable, sin loops)
**GIVEN** que `smartFitEnabled=true` y `userZoomDirty=false`  
**WHEN** cambia el tamaño del viewport  
**THEN** el visor recalcula y reaplica auto‑fit con debounce (50–100ms) sin provocar loops (no re‑encadenar fit indefinidamente).

### R4 — Rotación
**GIVEN** un documento con rotación metadata por página  
**WHEN** se aplica auto‑fit  
**THEN** los cálculos de tamaño/escala respetan la rotación real reportada por el engine.

**AND** el sistema SHALL NOT auto‑rotar por heurística de “contenido”.

### R5 — Compatibilidad / No regresión
El sistema SHALL mantener:
- zoom/scroll/thumbnail/rotate/print/export y herramientas existentes,
- el flujo actual de carga/cancelación/latest‑wins,
- overlay/skeleton existentes.

## 2) Requisitos no funcionales

- Performance: resize debounced; sin jitter visible.
- Observabilidad: logs/debug opcionales sin URLs/tokens.
- Accesibilidad: toggle “Smart Fit” con `aria-pressed`, labels y foco estable.

## 3) Criterios de aceptación

- AC1: Al cargar un PDF, se aplica auto‑fit post‑ready (cuando `smartFitEnabled=true`).
- AC2: Si el usuario hace zoom manual, resize no re‑auto‑fit (a menos que reactive Smart Fit).
- AC3: No hay loops ni flicker en resize.
- AC4: No se introducen heurísticas OCR/imagen/ML.
- AC5: Pruebas unitarias para `computeFitScale` y guards; pruebas de integración básicas del flujo.

