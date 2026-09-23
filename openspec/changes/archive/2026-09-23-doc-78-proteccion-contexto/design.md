<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## Context

DOC-78 agrega protección de contexto y recuperación al flujo moderno. La UI toma la tarea visible al construir requests, pero aún no conserva una identidad inmutable durante toda la operación ni coordina el bloqueo de acciones Workflow durante una escritura.

## Goals / Non-Goals

**Goals**
- Impedir ejecución o proyección sobre una tarea distinta de la original.
- Revalidar el contexto inmediatamente antes del primer efecto.
- Representar conflictos y recuperar el snapshot persistido sin reintentos ciegos.
- Bloquear reversiblemente las acciones incompatibles durante escritura.

**Non-Goals**
- Cancelar `ExecuteImportIntent` después de enviarlo.
- Modificar endpoints, ASMX, persistencia o handlers legacy.
- Usar `localStorage`, polling o sesión cliente como autoridad.

## Decisions

### D-01 — Contexto inmutable
El guard capturará tarea, operación, ruta, proveedor, identidades externas e intención. La tarea visible solo permite detectar divergencia y nunca reemplaza la capturada.

### D-02 — Preflight en la frontera del efecto
La confirmación realizará una comprobación fresca antes de `ExecuteImportIntent`. Si el contexto difiere, la ejecución no se envía.

### D-03 — Bloqueo declarativo y reversible
Un módulo aislado recibirá controles marcados en `Webworkflow.aspx`, preservará su estado y los deshabilitará durante escritura mediante eventos públicos del feature.

### D-04 — Conflictos normativos
Solo `TASK_CONTEXT_MISMATCH` y `PERSISTED_CONTEXT_MISMATCH` activan conflicto. Se conservan resultados confirmados, se detienen pendientes y se consulta el backend.

### D-05 — Recuperación autoritativa
`importar-servicio-web-recovery.js` aceptará únicamente un `IntentId` entregado por página/backend y consultará la API moderna existente, sin reconstruir autoridad desde el navegador.

### D-06 — Aislamiento de vista y pestañas
El guard comprobará la tarea antes de proyectar resultados. Una señal de otra pestaña solo dispara verificación; la autoridad permanece en el snapshot backend.

## Risks / Trade-offs

- Algunos controles legacy requieren una estrategia de bloqueo reversible distinta de `disabled`.
- Cerrar el modal no cancela la operación; la UI debe explicarlo.
- `beforeunload` y señales entre pestañas son auxiliares y pueden no llegar.

## Migration Plan

1. Agregar módulos y pruebas dentro del gate moderno.
2. Declarar controles incompatibles y el `IntentId` recuperable en la página.
3. Integrar guard/recovery y validar la regresión completa.
4. Documentar en `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-78-proteccion-contexto-recuperacion/`.

## Open Questions

- Ninguna bloqueante; los contratos backend existentes conservan la autoridad.
