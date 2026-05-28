## Contexto

SCRUMCORE-233: **APPVISOREMBEDPDF-CLICKCANCELABLE-LYFECYCLEBLOD**

En el flujo de visualización, el usuario puede hacer clicks rápidos (row click / menu action) para abrir documentos. Bajo ráfagas (y documentos grandes) pueden ocurrir:

- Cancelaciones y respuestas out-of-order (stale).
- Swaps rápidos de `fileUrl` / `blob:` mientras el motor aún consume la fuente anterior.
- Revocación prematura de `blob:` URLs.

Síntoma: aparición intermitente del prompt “Documento protegido” (p.ej. error tipo `Password`) aunque el PDF no esté protegido; el motor interpreta contenido inválido/incompleto como protegido/dañado.

Este cambio define el diseño para estabilizar la carga sin aplicar un bloqueo agresivo del click.

## Objetivo

Diseñar una estrategia enterprise, reusable y escalable para carga estable del visor bajo clicks rápidos **sin bloquear la UI**, garantizando:

- Cada click genera un intento nuevo (“attempt”).
- Antes de iniciar un intento nuevo se cancela el intento anterior en cadena:
  - `visor.cancelCurrentLoad()` (engine/visor)
  - `orchestrator.cancelCurrentRequest()` (red/orquestación)
- **Latest-wins end-to-end**: solo el intento vigente puede “commit” el documento visible.
- **Handshake “document ready”**: el visor confirma carga usable antes de swap/commit.
- **Swap seguro** (active/pending) + lifecycle correcto de fuentes (url/blob/blobUrl), sin revocar prematuramente.
- **Micro-gate UX opcional** (150–250ms) para cortar ráfagas involuntarias sin impedir cambios intencionales.

## Restricciones (inmutables)

- NO modificar backend.
- NO cambiar endpoints.
- NO persistir URLs temporales/tokens (`localStorage`/`sessionStorage`/`IndexedDB`/caches persistentes).
- NO romper consumidores legacy existentes.
- NO usar `any`.

## Alcance

Incluye:

- Concurrencia/cancelación end-to-end (UI → orquestador → visor).
- Stale ignore por `attemptId`/`documentKey`.
- Handshake “ready” y swap seguro.
- Lifecycle de blobs (sin leaks / sin revocación prematura).
- Pruebas unitarias e integración enfocadas al bug de estabilidad.

## Fuera de alcance

- Permisos/policy engine del visor (si no está explícito en SCRUMCORE-233).
- Cambios de contratos backend.
- Persistencia de fuentes.

## Componentes involucrados (3 protagonistas)

1) **Consumidor** (ej. `DocumentosWorkbench`)
- Captura click.
- Genera `attemptId`/`documentKey`.
- Cancela previo y dispara intento nuevo.
- (Opcional) aplica micro-gate.

2) **Orquestador** (ej. `useDocumentViewerOrchestrator`)
- Resuelve la fuente runtime.
- Expone `cancelCurrentRequest()`.
- Cancela requests previos y evita commits stale.

3) **Visor** (ej. `AppVisorEmbedPdf`)
- Expone `cancelCurrentLoad()`.
- Implementa handshake “ready”.
- Implementa swap seguro y cleanup seguro de blobs.
- No genera errores falsos por cancelaciones/stale.

## Contratos mínimos

### Identidad del intento

- `attemptId`: incremental por sesión UI, monotónico.
- `documentKey`: string derivado y comparable, recomendado: `${nombreGabinete}:${documentId}:${attemptId}`.

Regla: cualquier callback/response que no coincida con el intento vigente se ignora.

### Semántica de cancelación

- Cancelación ≠ error.
- `cancelled`:
  - no dispara notificaciones de error,
  - no borra el documento visible,
  - se considera estado esperado bajo clicks rápidos.

### Handshake “document ready”

El swap/commit solo se permite cuando el visor confirme que el documento es usable. La señal “ready” debe ser explícita (promise/callback) y estar sujeta a stale ignore/cancelación.

## Decisiones de diseño

1) **Click menos agresivo (recomendado)**
- No se bloquea la UI hasta “load complete”.
- Se permite click inmediato con cancelación + latest-wins.

2) **End-to-end latest-wins**
- Workbench mantiene `latestAttemptIdRef`.
- Orquestador ignora stale.
- Visor ignora stale.

3) **Swap seguro**
- Mantener `activeSource` hasta que `pendingSource` se marque loaded.
- Si `pending` falla/cancela: mantener `activeSource`.

4) **Lifecycle de blobs**
- No revocar blob activo.
- Revocar blob previo solo después de swap confirmado.
- En cancelación: limpiar solo recursos “pending” creados internamente y no visibles.

## Riesgos y mitigaciones

- **Leaks de `blob:`**: `revokeObjectURL` post-swap + cleanup en unmount.
- **Engine sin evento “ready” estable**: wrapper con timeout y stale ignore.
- **Carga excesiva (docs grandes)**: cancelación temprana + latest-wins + micro-gate opcional.

## Plan de migración

- Mantener comportamiento legacy para consumidores actuales.
- Introducir el comportamiento de handshake/swap seguro de forma incremental (feature flag si aplica).

## Preguntas abiertas (para cerrar antes de publish)

- ¿Dónde vive el contador `attemptId` (Workbench vs hook) para que sea único por sesión?
- ¿Qué evento concreto del engine PDF se usará como “ready”?
- ¿Se requiere timeout de handshake (p.ej. 10–15s) para evitar waits infinitos?
