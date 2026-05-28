## SCRUMCORE-233 — Tasks (accionables y verificables)

Objetivo: eliminar inestabilidad por clicks rápidos (falsos “Documento protegido”) mediante cancelación real, latest-wins, handshake ready, swap seguro y lifecycle de blobs.

### 1) Refinamiento (antes de publish)

- [ ] 1.1 Confirmar alcance exacto del ticket (sin permisos/policy engine si no aplica).
- [ ] 1.2 Definir contrato único de identidad del intento (`attemptId` vs `documentKey`) y **dónde se genera** (Consumidor vs Orquestador).
- [ ] 1.3 Definir mecanismo concreto de handshake “ready” del engine PDF (evento/callback/promesa), su criterio de “usable” y si requiere wrapper.
- [ ] 1.4 Definir política de timeout del handshake “ready” (p.ej. 10–15s) y comportamiento ante timeout (`failed` sin swap destructivo) para evitar waits infinitos.
- [ ] 1.5 Definir ownership de `blob:` (internal vs external) y regla explícita de revoke por ownership:
  - `blobUrl` creado por visor (internal) vs `blobUrl` provisto por orquestador (external).
- [ ] 1.6 Documentar semántica de cancelación: `cancelled` es esperado y silencioso.
- [ ] 1.7 Acordar contrato mínimo “managed” del visor (API + tipos):
  - `load(input): Promise<LoadResult>`
  - `cancelCurrentLoad(): void`
  - `reset(): void`
  - `LoadInput` mínimo: `attemptId/documentKey`, `source`, `contentType?`, `fileName?`
  - `LoadResult` mínimo: `attemptId/documentKey`, `loadStatus`, `activeFileUrl`, `errors`

> Gate de implementación (no iniciar 2.x/3.x hasta cerrar 1.2–1.5):
> - `attemptId` owner (Consumidor vs Orquestador)
> - señal “ready” del engine
> - timeout y comportamiento ante timeout
> - ownership blob y reglas de revoke

### 2) Implementación — Concurrencia end-to-end (click menos agresivo)

- [ ] 2.1 Consumidor (Workbench):
  - Generar `attemptId` incremental por sesión (ref).
  - En cada click: invocar `visor.cancelCurrentLoad()` y `orchestrator.cancelCurrentRequest()` antes del nuevo intento.
  - Aplicar latest-wins: ignorar commits si `attemptId !== latestAttemptId`.
  - (Opcional) micro-gate 150–250ms para dobles clicks involuntarios (mismo documento).

- [ ] 2.2 Orquestador:
  - Asegurar `cancelCurrentRequest()` aborta requests HTTP (AbortController / signal).
  - Propagar `attemptId/documentKey` a lo largo de su pipeline.
  - Ignorar respuestas stale (no comitear estado/documento si no coincide intento vigente).
  - Cancelación no limpia documento visible ni agrega error.

- [ ] 2.3 Visor (`AppVisorEmbedPdf`):
  - Exponer imperative API managed:
    - `load(input): Promise<LoadResult>`
    - `cancelCurrentLoad(): void`
    - `reset(): void`
  - Implementar stale ignore para loads en curso (por `attemptId/documentKey`).
  - Implementar cancelación real del load:
    - `cancelCurrentLoad()` corta el load en curso y resuelve `LoadResult.loadStatus="cancelled"` (sin notificación de error).
  - Implementar handshake “ready”:
    - `loadStatus="loaded"` solo tras confirmación del engine (documento usable).
    - Timeout (si definido en 1.4) → `failed` sin swap destructivo.
  - Implementar swap seguro (active/pending) preservando documento visible anterior en `failed/cancelled`.

### 3) Implementación — Swap seguro + lifecycle blob

- [ ] 3.1 Implementar estado `activeSource` vs `pendingSource` y reglas de commit:
  - No reemplazar `active` hasta `pending` “ready”.
  - Si `pending` falla/cancela: mantener `active`.

- [ ] 3.2 Lifecycle de blobs:
  - Ownership:
    - Si el visor crea `blob:` internamente (internal) → el visor es responsable del revoke.
    - Si el visor recibe `blob:` externo (external) → no revocar salvo contrato explícito.
  - Reglas de revoke:
    - Nunca revocar el `blob:` actualmente visible (active).
    - Revocar el blob previo solo después del swap confirmado (nuevo `loaded`).
    - En cancelación: limpiar solo recursos `pending` creados internamente y nunca visibles.
  - Cleanup en unmount (sin leaks).

### 4) Manejo de errores (sin falsos positivos)

- [ ] 4.1 `ERR_CANCELED`/cancelación no debe mapearse a “Documento protegido”.
- [ ] 4.2 En `failed` real, mostrar error controlado sin borrar documento visible anterior.
- [ ] 4.3 Agregar trazabilidad mínima (sin loguear URLs con token): attemptId/documentKey y estados.

### 5) Pruebas (obligatorias para este bug)

- [ ] 5.1 Unit tests (visor/orquestador):
  - latest-wins (stale ignored).
  - cancelación → `cancelled` (sin error visible).
  - swap seguro (active no cambia en `failed/cancelled`).
  - lifecycle blob (no revocar active antes de loaded; cleanup en unmount).
  - contrato managed del visor:
    - `load()` resuelve `LoadResult` consistente (incluye `attemptId/documentKey`, `loadStatus`, `activeFileUrl`, `errors`).

- [ ] 5.2 Integration tests:
  - clicks rápidos sobre documentos distintos.
  - cancelación encadenada (se cancela anterior y commit solo del último).
  - simular documento grande (o demorar handshake) y validar estabilidad.

- [ ] 5.3 E2E (si Playwright está disponible en el repo):
  - stress switching (clicks rápidos) sin aparición del prompt falso.

### 6) Documentación (estructura enterprise)

- [ ] 6.1 Crear/actualizar docs con estructura enterprise (ver sección “Plantilla” abajo).
- [ ] 6.2 Incluir diagramas Mermaid (sequence/state) del flujo attempt/cancel/swap.
- [ ] 6.3 Confirmación explícita de restricciones:
  - backend no modificado
  - endpoints no modificados
  - sin persistencia de URLs/tokens
  - sin `any`

---

## Plantilla de documentación enterprise (SCRUMCORE-[ID])

Guardar en `docs/Components/<Componente>/`:

- `SCRUMCORE-[ID]-Arquitectura.md`
- `SCRUMCORE-[ID]-Implementacion-Detallada.md`
- `SCRUMCORE-[ID]-Integracion-BackEnd.md` (si “no aplica”, documentar explícitamente “NO aplica”)
- `SCRUMCORE-[ID]-Pruebas.md`
- `SCRUMCORE-[ID]-Metadata.md`

Estructura interna recomendada (mínimo):

1. Metadata (ticket, autor, fecha, versión)
2. Resumen ejecutivo (mental model)
3. Objetivo técnico
4. Problema que resuelve (con evidencia)
5. Alcance / Fuera de alcance
6. Restricciones y garantías
7. Arquitectura por capas (presentation/hooks/services/adapters/types/tests)
8. Contratos oficiales (inputs/outputs tipados)
9. Flujo end-to-end (paso a paso)
10. Concurrencia (latest-wins, cancelación, stale ignore)
11. Handshake “ready” (criterio de loaded)
12. Swap seguro + lifecycle blob (active/pending, revocación)
13. Manejo de errores y fallback
14. Performance y UX (micro-gate, no lock total)
15. Observabilidad (eventos sin datos sensibles)
16. Integración con consumidores reales (ejemplo)
17. Archivos modificados + trazabilidad (archivo → símbolo → test)
18. Pruebas + evidencias ejecutadas
19. Pendientes / deuda técnica
20. ADRs / decisiones
21. Diagramas Mermaid (sequence/state)
22. Checklist de cumplimiento (restricciones)
