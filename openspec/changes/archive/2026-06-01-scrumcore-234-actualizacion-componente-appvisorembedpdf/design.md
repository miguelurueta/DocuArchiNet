## Context

SCRUMCORE-234: ACTUALIZACION-COMPONENTE-APPVISOREMBEDPDF

Este change implementa **Auto‑Fit determinístico** en `AppVisorEmbedPdf` sin heurísticas y sin romper UX (zoom/scroll/viewport).

## Objetivo (resumen)

- Aplicar auto‑fit (fit-to-width / fit-to-page) + centrado al cargar el documento (post‑ready).
- Auto‑fit es automático y **se aplica una vez** post‑ready por documento (no es un flujo manual).
- Respetar rotación metadata real del PDF reportada por el engine.
- Mantener compatibilidad con el flujo actual (cancel chain/latest‑wins, loaders, plugins).
- No modificar backend/endpoints ni introducir OCR/imagen/ML.

## Decisiones de diseño

### D1 — Integración post‑ready (handshake)
El auto‑fit se ejecuta **solo** después del handshake “ready” del engine (documento usable). Evita aplicar escala/scroll sobre un documento aún no activado.

### D2 — Estado mínimo de UX
Se usa un estado mínimo:
- `autoFitApplied` (para asegurar “apply once” post‑ready).
- `userZoomDirty` (true si el usuario hace zoom manual: wheel/pinch/botones; evita re‑auto‑fit).

Regla:
- Auto‑fit post‑ready solo si `!autoFitApplied` (apply once) y `!userZoomDirty`.

### D3 — Cálculo determinístico (sin heurísticas)
`fitScale` se calcula con:
- tamaño real de página provisto por el engine (incluida rotación metadata),
- tamaño del viewport disponible,
- `fitMode` (`width` por defecto; `page` opcional).

### D4 — Concurrencia / stale‑ignore
Cada aplicación de auto‑fit debe validarse contra el documento vigente (`documentId`/`loadSeq` del visor).
Si el documento cambió, se ignora (no side effects).

### D5 — Performance y estabilidad
- Resize con debounce 50–100ms.
- Evitar loops: una aplicación de fit no debe auto‑encadenarse indefinidamente.

## Arquitectura propuesta

### Módulo dedicado (aislamiento)
Crear `src/app/Components/UI/AppVisorEmbedPdf/autoFit/`:
- `autoFit.types.ts` (contracts internos)
- `autoFit.math.ts` (`computeFitScale`)
- `useAutoFitController.ts` (estado + handlers)
- `autoFit.apply.ts` (aplicación al engine con guards)

### Puntos de integración en `AppVisorEmbedPdf`
- Post‑ready: `applyAutoFitIfAllowed()`.
- Resize: no forzar re‑auto‑fit por defecto (para no romper intención del usuario).

## Riesgos y mitigaciones

- Riesgo: APIs EmbedPDF para centering/viewport no disponibles o difieren.
  - Mitigación: validar APIs reales primero y degradar a “fit” sin centering si no hay soporte.
- Riesgo: loops por eventos internos de zoom/resize.
  - Mitigación: debounce + guards (`userZoomDirty`) + evitar re-entrancia.

## Fuera de alcance (explícito)

- Auto‑rotate por “contenido” (OCR/imagen/ML).
- Persistencia cross‑session del modo de fit o rotación.
