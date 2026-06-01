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
**THEN** el visor NO reaplica auto‑fit automáticamente durante esa misma sesión de documento.

Regla:
- El auto‑fit es **automático solo una vez** post‑ready por documento (y opcionalmente post‑ready tras rotación manual, si se decide), pero no se re‑auto‑aplica por resize después de interacción manual.

### R3 — Resize handling (estable, sin loops)
**GIVEN** que el documento ya fue auto‑ajustado post‑ready  
**WHEN** cambia el tamaño del viewport  
**THEN** el visor NO debe forzar re‑auto‑fit si el usuario ya interactuó (zoom/scroll) y debe evitar loops.

Nota: si producto requiere re‑fit en resize, debe ser un ticket separado con una regla de UX explícita (para no romper intención del usuario).

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
- AC2: Si el usuario hace zoom manual, resize no re‑auto‑fit (sin controles adicionales en UI).
- AC3: No hay loops ni flicker.
- AC4: No se introducen heurísticas OCR/imagen/ML.
- AC5: Pruebas unitarias para `computeFitScale` y guards; pruebas de integración básicas del flujo.
