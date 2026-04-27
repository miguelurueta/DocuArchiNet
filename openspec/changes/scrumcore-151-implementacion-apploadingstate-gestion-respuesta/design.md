## Context

El ticket `SCRUMCORE-151` pide implementar (y/o completar) el componente shared `AppLoadingState` como loader **inline** con delay (anti-flicker) y accesible, y migrar el manejo de loading del panel master-detail en `GestionCorrespondenciaRoute`, eliminando lógica ad-hoc y estandarizando el patrón visual y funcional.

Contexto existente: actualmente existen loaders implementados de forma local con lógica de temporización en vistas (`showDelayedLoader`, `setTimeout`/`clearTimeout`), lo que genera duplicación, inconsistencias visuales y flicker en cargas rápidas.

Restricciones relevantes:
- Mantener React + Vite + TS estricto + ESM.
- No introducir dependencias nuevas para loading.
- No usar `any`.
- No acoplar el componente a módulos específicos.
- No usarlo como bloqueador global full-screen (debe ser card pequeña inline).
- No introducir estilos globales.
- No duplicar lógica de delay en consumidores.
- Preferir UX consistente y accesible (texto + `aria-*` cuando aplique).

## Goals / Non-Goals

**Goals:**
- Implementar `AppLoadingState` como componente shared, reusable y accesible, con control interno de delay/visibilidad.
- Migrar `GestionCorrespondenciaRoute` para usar `AppLoadingState` y eliminar lógica de temporización ad-hoc.
- Garantizar que consumidores solo pasen `loading` y `delayMs` (sin timers en vistas) y que el comportamiento sea consistente.
- Cubrir con pruebas unitarias (componente) y pruebas de integración UI (panel master-detail) sin regresiones.

**Non-Goals:**
- Implementar un loader global full-screen.
- Migrar masivamente otros módulos/pantallas fuera de `GestionCorrespondenciaRoute` en este ticket base.
- Cambios de arquitectura del router, layout o estructura de módulos.

## Decisions

1) **La lógica de delay/visibilidad vive exclusivamente en `AppLoadingState`**
   - Decisión: `AppLoadingState` encapsula timers, control de render y limpieza al desmontar/cambiar estado.
   - Racional: regla arquitectónica obligatoria del ticket; elimina duplicación y evita errores de setState post-unmount.
   - Alternativas consideradas:
     - Timers en consumidores (showDelayedLoader) → explícitamente prohibido.

2) **Contrato de props estable**
   - Decisión: `AppLoadingState` expone el contrato esperado:
     - `loading: boolean`
     - `delayMs?: number` (default `500`)
     - `title?: string`, `message?: string`, `icon?: ReactNode`, `className?: string`, `children?: ReactNode`
   - Racional: permite reutilización sin acoplar a módulos y sin estilos globales.

3) **Comportamiento obligatorio**
   - Decisión: implementar reglas:
     - No renderizar antes de que `loading` supere `delayMs`.
     - Render inline (card pequeña), no full-screen.
     - Ocultar correctamente cuando `loading=false`.
     - Limpiar timers al desmontar y al cambiar `loading`.
     - Accesibilidad: `role="status"` y `aria-live="polite"`.

4) **Migración en `GestionCorrespondenciaRoute`**
   - Decisión: reemplazar lógica ad-hoc (`showDelayedLoader` + timers) por:

     ```tsx
     <AppLoadingState
       loading={detailState === "loading"}
       delayMs={500}
       title="Cargando estructura de la tarea"
       message="Validando información…"
     />
     ```

   - Mantener: `data-testid="gestion-correspondencia-loading-state"`.

## Risks / Trade-offs

- **[Riesgo] Fugas de timers / setState post-unmount** → **Mitigación:** pruebas unitarias obligatorias de cleanup y asegurar limpieza en `useEffect`.
- **[Trade-off] `delayMs` ocultando feedback** → **Mitigación:** default `500ms` (según ticket) y posibilidad de override por consumidor sin duplicar timers.

## Migration Plan

1) Completar `AppLoadingState` según contrato/reglas y pruebas unitarias obligatorias.
2) Migrar `GestionCorrespondenciaRoute` eliminando `showDelayedLoader` y timers, usando `AppLoadingState`.
3) Pruebas de integración UI obligatorias en el panel master-detail:
   - Loading se renderiza correctamente y no rompe layout.
   - `children` visibles cuando `loading=false` (si aplica wrapper mode).
4) Verificar `npm run test -- --run` y `npm run spec:validate`.

Rollback:
- Revertir commits del cambio (no hay migraciones de datos ni dependencias nuevas).

## Open Questions

- ¿El componente debe soportar explícitamente “wrapper mode” (renderizar `children` cuando `loading=false`) como parte del contrato, o se maneja en consumidores?
- ¿Qué iconografía se considera estándar (si se usa), o se deja el default del componente?
