## Context

SCRUMCORE-233: APPVISOREMBEDPDF-CLICKCANCELABLE-LYFECYCLEBLOD

## Jira Details

> PROMPT ARQUITECTÓNICO (ENTERPRISE) — Estabilidad de carga en visor  (click cancelable + latest‑wins end‑to‑end + handshake “document ready” + swap seguro + lifecycle blob)
>   Rol esperado  Arquitecto frontend senior (React 19, TypeScript estricto, control de concurrencia, estabilidad runtime, testing enterprise)
>   Objetivo  Implementar una estrategia enterprise, reusable y escalable para abrir documentos bajo clicks rápidos sin bloquear la UI, garantizando:
> Cada click genera un intento nuevo (attempt).
> 
> Antes de iniciar un intento nuevo se cancela el intento anterior en cadena:
> visorRef.current.cancelCurrentLoad() (engine/visor)
> 
> orchestrator.cancelCurrentRequest() (red/orquestación)
> 
> Latest‑wins end‑to‑end: solo el intento vigente puede “commit” el documento visible.
> 
> Handshake explícito “document ready”: el visor confirma carga usable antes de swap/commit.
> 
> Swap seguro (active/pending) + lifecycle correcto de fuentes (url/blob/blobUrl) sin revocar prematuramente.
> 
> Micro‑gate UX opcional (150–250ms) para cortar ráfagas involuntarias, sin impedir cambios intencionales.
> 
>   IMPORTANTE — Este ticket NO debe
> Modificar backend.
> 
> Cambiar endpoints.
> 
> Persistir URLs temporales/tokens (localStorage/sessionStorage/IndexedDB/caches persistentes).
> 
> Romper consumidores legacy existentes.
> 
>   IMPORTANTE — Este ticket SÍ debe
> Implementar cancelación real (red + engine) con propagación del intento.
> 
> Ignorar respuestas stale por attemptId/documentKey.
> 
>   Componentes involucrados (3 protagonistas)
> Consumidor (ej. DocumentosWorkbench): captura click, crea attemptId, cancela previo, dispara nuevo intento.
> 
> Orquestador (ej. useDocumentViewerOrchestrator): resuelve fuente runtime, respeta attemptId, cancela requests previos, no comitea stale.
> 
> Visor (ej. AppVisorEmbedPdf.load()): carga engine, handshake “ready”, swap seguro, revocación segura de blobs, no muestra errores falsos por cancelación.
> 
>   Diseño obligatorio — Identidad del intento (latest‑wins)  Definir un identificador estable por intento:
> attemptId: number monotónico (incremental), o
> 
> documentKey: string estable y comparable.
> 
>   Debe viajar por:
> Consumidor → Orquestador (visualizarDocumento({ ..., attemptId/documentKey }))
> 
> Orquestador → Visor (visor.load({ ..., attemptId/documentKey }))
> 
>   Regla: cualquier callback/response que no coincida con el intento vigente se ignora.
>   Contrato mínimo — Visor (managed)  El visor debe exponer imperative API:
> load(input): Promise<LoadResult>
> 
> cancelCurrentLoad(): void
> 
> reset(): void
> 
>   Entrada LoadInput (mínima para estabilidad)
> attemptId o documentKey
> 
> source: { kind: "url" | "blobUrl" | "blob", ... }
> 
> contentType?: string | null
> 
> fileName?: string
> 
>   Salida LoadResult (mínima para estabilidad)
> attemptId o documentKey
> 
> loadStatus: "loaded" | "failed" | "cancelled"
> 
> activeFileUrl: string | null
> 
> errors: string[]
> 
>   Reglas de concurrencia (menos agresiva) — obligatorias  Consumidor:
> Cada click:
> Incrementa attemptId / crea documentKey.
> 
> Ejecuta visor.cancelCurrentLoad() y orchestrator.cancelCurrentRequest().
> 
> Dispara orchestrator.visualizarDocumento(...) con attemptId/documentKey.
> 
> Cuando el orquestador entrega fuente, llama visor.load({ attemptId/documentKey, source, ... }).
> 
>   Orquestador:
> Debe abortar requests previos (AbortController).
> 
> Debe ignorar respuestas stale por attemptId/documentKey.
> 
> No debe limpiar el documento visible por cancelación; cancelación es estado normal.
> 
>   Visor:
> Debe ignorar loads stale por attemptId/documentKey.
> 
> cancelCurrentLoad() debe cancelar el load en curso y resolver la promesa con loadStatus="cancelled" (sin notificación de error).
> 
> Debe implementar handshake “ready” y no comitear hasta confirmación del engine.
> 
>   Handshake “document ready” — obligatorio  load() solo se considera loaded cuando:
> El engine confirma documento abierto y usable (ready/activated/loaded).
> 
> Si falla, devolver failed sin swap destructivo.
> 
>   Swap seguro (active/pending) — obligatorio  Mantener:
> activeSource: lo visible
> 
> pendingSource: lo que carga
> 
>   Reglas:
> No reemplazar active hasta handshake loaded del pending.
> 
> Si pending falla/cancela: mantener active.
> 
> Solo después de swap confirmado limpiar recursos previos.
> 
>   Lifecycle de blobs (sin leaks / sin revocación prematura) — obligatorio
> Si el visor crea blobUrl internamente: revocar solo cuando deje de ser active y el nuevo esté loaded.
> 
> Si llega un blobUrl desde afuera: no revocarlo salvo contrato explícito de ownership.
> 
> Al cancelar pending: limpiar solo recursos pending creados internamente y nunca visibles.
> 
>   Micro‑gate UX (opcional, recomendado)
> Ignorar dobles clicks 150–250ms (solo ráfagas involuntarias, idealmente mismo documento).
> 
> No bloquear cambios a otro documento.
> 
>   Manejo de errores obligatorio
> Cancelación: silenciosa (no toast error, no prompt falso).
> 
> Falla real de load: mensaje controlado, mantener active visible.
> 
> No mapear “contenido inválido” automáticamente a “documento protegido”; distinguir causas.
> 
>   Pruebas obligatorias
> Unit:
> latest‑wins (stale ignored)
> 
> cancelación ⇒ cancelled
> 
> swap seguro (active no cambia en failed/cancelled)
> 
> lifecycle blob (no revocar active antes de loaded)
> 
> Integración:
> clicks rápidos (distintos docs)
> 
> docs grandes (simulado) + cancel
> 
> no prompt falso por swap
> 
> E2E (si existe):
> stress switching + estabilidad
> 
>   Documentación obligatoria (estructura enterprise SCRUMCORE-[ID])  Guardar en docs/Components/<Componente>/:
> SCRUMCORE-[ID]-Arquitectura.md
> 
> SCRUMCORE-[ID]-Implementacion-Detallada.md
> 
> SCRUMCORE-[ID]-Integracion-BackEnd.md (aunque “no aplica”, documentar “NO aplica”)
> 
> SCRUMCORE-[ID]-Pruebas.md
> 
> SCRUMCORE-[ID]-Metadata.md
> 
>   Estructura enterprise obligatoria dentro de la documentación (plantilla)  0. Metadata
> Resumen ejecutivo (mental model)
> 
> Objetivo técnico
> 
> Problema que resuelve (con evidencia)
> 
> Alcance / Fuera de alcance
> 
> Restricciones y garantías
> 
> Arquitectura por capas (presentation/hooks/services/adapters/types/tests)
> 
> Contratos oficiales (inputs/outputs tipados)
> 
> Flujo end‑to‑end (paso a paso)
> 
> Concurrencia (latest‑wins, cancelación, stale ignore)
> 
> Swap seguro + lifecycle blob (active/pending, revocación)
> 
> Manejo de errores y fallback
> 
> Performance y UX (micro‑gate, no lock total)
> 
> Observabilidad (eventos; sin datos sensibles)
> 
> Integración con consumidores reales (ejemplo)
> 
> Archivos modificados + trazabilidad (archivo→símbolo→test)
> 
> Pruebas + evidencias ejecutadas
> 
> Pendientes / deuda técnica
> 
> ADRs / decisiones
> 
> Diagramas Mermaid (sequence/state/class)
> 
> Confirmación explícita de cumplimiento (checklist)
> 
>   Entrega esperada
> Diff de archivos tocados.
> 
> Resumen técnico del flujo.
> 
> Evidencia de tests ejecutados.
> 
> Confirmación de restricciones.
> 
>   Instrucción final  Implementar la estrategia “menos agresiva” (cancelable + latest‑wins end‑to‑end + handshake ready + swap seguro + lifecycle blob) para eliminar errores por swaps rápidos sin bloquear la  UI, manteniendo estabilidad del documento visible y evitando prompts falsos, junto con documentación enterprise SCRUMCORE-[ID] siguiendo la estructura definida.

## Goals / Non-Goals

**Goals**
- Refinar alcance tecnico usando el contexto completo de Jira.
- Definir decisiones arquitectonicas, riesgos y plan de migracion.

**Non-Goals**
- Cambios fuera del alcance descrito por el ticket.

## Decisions

1. TBD

## Risks / Trade-offs

- TBD

## Migration Plan

1. TBD

## Open Questions

- TBD
