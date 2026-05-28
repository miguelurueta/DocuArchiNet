## Requisitos — SCRUMCORE-233

Ticket: **APPVISOREMBEDPDF-CLICKCANCELABLE-LYFECYCLEBLOD**

Objetivo: estabilidad de carga del visor bajo clicks rápidos mediante cancelación real, latest-wins end-to-end, handshake “document ready”, swap seguro y lifecycle correcto de blobs.

### Glosario

- **Attempt / Intento**: ejecución iniciada por un click de usuario para abrir un documento.
- **latest-wins**: solo el último intento vigente puede “commit” el documento visible y su estado asociado.
- **stale**: respuesta/callback perteneciente a un intento no vigente.
- **handshake ready**: confirmación explícita de que el documento es usable por el engine antes del swap definitivo.
- **activeSource / pendingSource**: fuente visible vs fuente en carga.

### Restricciones (obligatorias)

1. El sistema **SHALL NOT** modificar backend.
2. El sistema **SHALL NOT** cambiar endpoints.
3. El sistema **SHALL NOT** persistir URLs temporales o tokens (`localStorage`, `sessionStorage`, `IndexedDB`, caches persistentes).
4. El sistema **SHALL NOT** introducir `any`.
5. El sistema **SHALL** mantener compatibilidad con consumidores legacy existentes.

---

## Requirement R1 — Identidad del intento (latest-wins)

El sistema **SHALL** identificar cada intento con `attemptId` (incremental) y/o `documentKey` (derivado y comparable).

### Scenario R1.1 — Propagación end-to-end
- **GIVEN** un click de usuario para abrir un documento
- **WHEN** se crea un nuevo intento
- **THEN** `attemptId/documentKey` viaja por: Consumidor → Orquestador → Visor

### Scenario R1.2 — Stale ignored
- **GIVEN** dos intentos consecutivos A y B (B es el vigente)
- **WHEN** llega una respuesta/callback perteneciente al intento A
- **THEN** esa respuesta/callback se ignora y no puede “commit” estado ni fuente visible

---

## Requirement R2 — Cancelación encadenada (click menos agresivo)

El sistema **SHALL** permitir clicks rápidos sin lock agresivo, implementando cancelación encadenada:

- Antes de iniciar un intento nuevo: `visor.cancelCurrentLoad()` y `orchestrator.cancelCurrentRequest()`.

### Scenario R2.1 — Cancelación antes de iniciar
- **GIVEN** un intento en curso
- **WHEN** el usuario hace click para abrir otro documento
- **THEN** se invoca la cancelación del intento previo antes de iniciar el nuevo intento

### Scenario R2.2 — Cancelled ≠ error
- **GIVEN** un intento cancelado por un click posterior
- **WHEN** el sistema procesa la cancelación
- **THEN** el estado resultante es `cancelled` sin notificación de error y sin limpiar el documento visible

---

## Requirement R3 — Handshake “document ready”

El visor **SHALL** implementar una confirmación explícita “ready” y **SHALL** considerar un documento `loaded` solo cuando el engine confirme “usable”.

### Scenario R3.1 — Commit sólo después de ready
- **GIVEN** una fuente nueva en carga (pending)
- **WHEN** el engine confirma “ready”
- **THEN** el visor permite el swap/commit del documento visible

### Scenario R3.2 — Ready no alcanzado
- **GIVEN** que la carga falla o el intento se cancela antes de “ready”
- **WHEN** se completa el intento
- **THEN** el visor no hace swap destructivo y preserva el documento visible anterior

---

## Requirement R4 — Swap seguro (active/pending)

El visor **SHALL** mantener `activeSource` estable hasta que `pendingSource` sea `loaded`.

### Scenario R4.1 — Pending fails
- **GIVEN** un documento visible `activeSource`
- **WHEN** `pendingSource` falla
- **THEN** `activeSource` permanece visible y el error se reporta como `failed`

### Scenario R4.2 — Pending cancelled
- **GIVEN** un documento visible `activeSource`
- **WHEN** `pendingSource` se cancela por un click posterior
- **THEN** `activeSource` permanece visible y el resultado es `cancelled`

---

## Requirement R5 — Lifecycle de blobs (sin leaks, sin revocación prematura)

El sistema **SHALL** manejar `blob:` URLs de forma segura:

- Nunca revocar el `blob:` correspondiente al documento actualmente visible.
- Revocar recursos previos solo después de swap confirmado.
- En cancelación: limpiar solo recursos `pending` creados internamente y que nunca llegaron a ser visibles.

### Scenario R5.1 — No premature revoke
- **GIVEN** un documento visible cuya fuente es un `blob:`
- **WHEN** se inicia la carga de un nuevo documento
- **THEN** el `blob:` visible no se revoca hasta que el nuevo documento esté `loaded`

### Scenario R5.2 — Cleanup on unmount
- **GIVEN** que el componente visor se desmonta
- **WHEN** ocurre `unmount`
- **THEN** se liberan recursos internos (`blob:`) para evitar memory leaks

---

## Requirement R6 — Micro-gate UX (opcional)

El sistema **MAY** implementar un micro-gate de 150–250ms para ignorar dobles clicks involuntarios (idealmente mismo documento) sin bloquear cambios intencionales a otro documento.

### Scenario R6.1 — Double click suppression
- **GIVEN** clicks duplicados sobre el mismo documento en <250ms
- **WHEN** el visor está en estado `loading`
- **THEN** el sistema puede ignorar el segundo click sin afectar la navegación normal

---

## Requirement R7 — No regresión

El sistema **SHALL** preservar flujos legacy existentes: si un consumidor actual pasa `fileUrl` directo, el visor sigue funcionando.

### Scenario R7.1 — Legacy mode
- **GIVEN** un consumidor legacy
- **WHEN** renderiza el visor con `fileUrl` directo
- **THEN** el documento se visualiza como antes y sin requerir `attemptId/documentKey`
