# SCRUMCORE-233 — Arquitectura

## 0) Resumen ejecutivo (mental model)

- 1 click = 1 intento (attempt).
- Estrategia “menos agresiva”: no se bloquea el click; se cancela el intento anterior y se aplica latest‑wins.
- El Orquestador consolida “fuente runtime” (Blob → `blob:` URL) y metadatos; el Visor solo “abre”.
- El Visor mantiene **1 documento activo** en el engine (no acumula) para evitar límites internos del proveedor.

## 1) Problema que resuelve (con evidencia)

### Síntoma observado
Bajo clicks rápidos en el listado, el visor:
- dejaba de abrir documentos,
- “parecía bloqueado” (sin error visible), y/o
- mostraba prompts falsos (“Documento protegido”) cuando el contenido se invalidaba durante swaps.

### Evidencia y causa raíz confirmada
El error real que rompía el flujo no era backend ni permisos: era el engine (EmbedPDF DocumentManager) rechazando aperturas por llegar al máximo de documentos abiertos:

- Rechazo en `openDocumentUrl` (“outer task reject”):
  - `reason.message`: `"Maximum number of documents (10) reached"`

Modelo causal:
- clicks rápidos → múltiples `load()` + re-renders → aperturas repetidas sin cierre explícito → DocumentManager acumula docs → llega a 10 → rechaza nuevos `openDocumentUrl` → el usuario percibe “bloqueo”.

## 2) Alcance / Fuera de alcance

### Alcance (este ticket)
- Cancelación encadenada best‑effort: Workbench → Orquestador → Visor.
- latest‑wins: el intento vigente es el único que debe “commit”.
- Hardening del visor ante fallos genéricos (evitar prompt de contraseña por fallos no‑password).
- Enforzar “single-active document” en el engine para no alcanzar el límite 10.
- Observabilidad temporal con `window.__DV_DEBUG__`.

### Fuera de alcance
- Cambios de backend o endpoints.
- Rediseño del visor para tipos no‑PDF.
- Implementar un policy engine completo de permisos (solo estabilidad/concurrencia).

## 3) Componentes y responsabilidades (3 protagonistas)

### 3.1 Consumidor: `DocumentosWorkbench`
Archivo: `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`

Responsabilidades:
- Captura click en fila.
- Ejecuta action `ver_documento` y extrae `IdDocumento/NombreGabinete`.
- Inicia el intento (attempt), cancela el intento previo (best‑effort) y dispara orquestación.
- Evita llamadas redundantes a `visorRef.load()` (gate por clave de carga).
- Superficie de error de `ver_documento` (toast) cuando falla o se queda colgado (timeout).

### 3.2 Orquestador: `useDocumentViewerOrchestrator`
Archivo: `src/app/Components/UI/AppDocumentViewerOrchestrator/useDocumentViewerOrchestrator.ts`

Responsabilidades:
- Resolve de visualización y descarga autenticada como `Blob`.
- Construcción de `blob:` URL runtime (`URL.createObjectURL`).
- Cancelación de request in-flight (AbortController / cancel semantics existentes).
- latest‑wins interno por `requestId` (y trazabilidad para correlación).

### 3.3 Visor: `AppVisorEmbedPdf`
Archivo: `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx`

Responsabilidades:
- Abrir fuente (URL/blob URL) en el engine (DocumentManager).
- Handshake “ready” usando `task.wait` (motor indica doc usable).
- Hardening: no mostrar prompt de contraseña cuando el fallo es genérico (solo ante `PdfErrorCode.Password`).
- “Single-active document”: cerrar (best‑effort) el documento previo antes de abrir uno nuevo.

## 4) Contratos de concurrencia (latest‑wins)

Identidad del intento:
- En el estado actual se instrumentó correlación por `attempt` (Workbench), `req` (orquestador) y `seq` (visor).
- Se extendieron tipos para permitir `attemptId` / `documentKey` (compatibles hacia atrás), pero el criterio principal aplicado aquí es “latest‑wins + gate + single-active engine”.

Regla:
- Cancelación es “normal” (no se muestra como error).
- Respuestas stale deben ignorarse (o no deben “commit” en UI).

## 5) Diagramas (Mermaid)

### 5.1 sequenceDiagram — Click → resolve → blob → visor.open

```mermaid
sequenceDiagram
  participant UI as DocumentosWorkbench
  participant ACT as Action: ver_documento
  participant ORC as useDocumentViewerOrchestrator
  participant RES as POST visualizacion/resolve
  participant DL as GET visualizacion/download/{token}
  participant VIS as AppVisorEmbedPdf
  participant DM as EmbedPDF DocumentManager

  UI->>VIS: cancelCurrentLoad() (best-effort)
  UI->>ORC: cancelCurrentRequest() (best-effort)
  UI->>ACT: performAction("ver_documento", rowId)
  ACT-->>UI: {IdDocumento, NombreGabinete}
  UI->>ORC: visualizarDocumento({IdDocumento, NombreGabinete})
  ORC->>RES: resolve()
  RES-->>ORC: {urlTemporal, contentType, fileName, ...}
  ORC->>DL: download(blob)
  DL-->>ORC: Blob(application/pdf)
  ORC-->>UI: documentoActivo.fileUrl = blob:
  UI->>VIS: load({url: blob:})
  VIS->>DM: closeDocument(prev) (best-effort)
  VIS->>DM: openDocumentUrl(url)
  DM-->>VIS: {documentId, task}
  VIS->>DM: task.wait()
  DM-->>VIS: ready
  VIS-->>UI: loaded
```

### 5.2 stateDiagram — Estados del intento en visor (conceptual)

```mermaid
stateDiagram-v2
  [*] --> Idle
  Idle --> Loading: load()
  Loading --> Loaded: engine ready (task ok)
  Loading --> Failed: openDocumentUrl reject / task fail
  Loading --> Cancelled: cancelCurrentLoad()
  Loaded --> Loading: load(next)
  Failed --> Loading: retry/next
  Cancelled --> Loading: next
```

## 6) Cómo ubicar el intento donde se rompe (operativo)

1) Habilitar debug:
   - `window.__DV_DEBUG__ = true`
2) Buscar el primer log del visor:
   - `[DV][visor] openDocumentUrl failed (outer task)`
3) Ese `managedSeq` (o el `seq` inmediatamente anterior) se correlaciona por proximidad temporal con:
   - el último `[DV][attempt:N][seq:S]` del Workbench, y
   - el último `[DV][attempt:N][req:R]` del Orquestador.
4) Si el error es `"Maximum number of documents (10) reached"`, la falla está en el engine y se corrige con “single-active document” (close-before-open) + gate anti-duplicados.

