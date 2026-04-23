# Design: scrumcore-150 — `AppLoadingState` (inline loader con delay)

## Context
El proyecto contiene estados de carga inline implementados por pantalla con temporización local (delay) para mitigar flicker. Esto introduce:
- Duplicación de lógica (`setTimeout`/`clearTimeout`) en consumidores.
- Inconsistencias visuales (cada módulo define su card/estilo).
- Accesibilidad inconsistente (no siempre `role="status"` / `aria-live`).

El objetivo es encapsular la lógica de delay y presentación de un loader inline en un componente shared `AppLoadingState`, y migrar un primer consumidor real (panel master-detail de Gestión Correspondencia) como referencia.

### Constraints
- React 19 + TypeScript estricto (sin `any`).
- Sin estilos globales (solo CSS module/estilos encapsulados).
- No reemplazar bloqueadores globales (para eso existe `OperationBlockerContext`).
- Migración progresiva (no forzar reemplazo global inmediato).

## Goals / Non-Goals
### Goals
- Unificar el patrón de “loader inline con delay” en un componente shared reutilizable.
- Evitar flicker: no renderizar antes de `delayMs`.
- Accesibilidad uniforme: `role="status"`, `aria-live="polite"`.
- Integración segura: migrar `GestionCorrespondenciaRoute` eliminando temporización local.
- Tests: unitarios del componente + integración UI mínima en el consumidor.

### Non-Goals
- No crear un overlay global full-screen.
- No cambiar las reglas de negocio del panel (ready/blocked).
- No introducir nuevas dependencias pesadas.

## Decisions
### 1) Encapsular delay dentro del componente
**Decisión:** `AppLoadingState` maneja internamente el timer y expone una UI únicamente cuando `loading` ha permanecido activo al menos `delayMs`.

**Alternativas consideradas:**
- Delay en consumidor: rechazado por duplicación y riesgos (timers sin limpiar, divergencia).
- CSS-only skeleton: no resuelve “no renderizar antes del delay”.

**Rationale:** centraliza comportamiento temporal y evita inconsistencias.

### 2) API de props estable y agnóstica de dominio
**Decisión:** props simples y tipadas:
- `loading`, `delayMs`, `title`, `message`, `icon`, `className`, `children?`.

**Rationale:** facilita adopción progresiva y evita acoplar a módulos.

### 3) Render inline/card, no overlay global
**Decisión:** el componente renderiza un card pequeño centrado dentro del contenedor padre; no captura interacción fuera de su área.

**Rationale:** consistente con paneles master-detail y layouts internos.

### 4) Wrapper mode opcional (`children`)
**Decisión:** soportar `children` opcional para facilitar uso como wrapper (`loading ? <AppLoadingState/> : children`), manteniendo también uso standalone.

**Rationale:** reduce boilerplate y habilita patrones sin imponerlos.

## Risks / Trade-offs
- **[Riesgo]** Timers sin limpiar → **Mitigación:** cleanup en `useEffect` y evitar `setState` tras unmount.
- **[Riesgo]** Flicker por toggles rápidos de `loading` → **Mitigación:** no renderizar antes del delay; reset inmediato al pasar `loading=false`.
- **[Riesgo]** Uso incorrecto como loader global → **Mitigación:** documentación explícita + estilo inline (no full-screen) y naming.

## Migration Plan
1. Crear `AppLoadingState` en `src/app/Components/UI/AppLoadingState/` con CSS module.
2. Agregar tests unitarios del componente:
   - no renderiza antes del delay
   - renderiza tras el delay si continúa loading
   - se oculta al desactivar loading
   - limpia timers (unmount / cambios rápidos)
3. Migrar `GestionCorrespondenciaRoute`:
   - eliminar `showDelayedLoader` y temporización local
   - usar `AppLoadingState loading={detailState === "loading"}`.
4. Ejecutar tests existentes del módulo (`GestionCorrespondenciaRoute.spec.test.tsx`) y los nuevos.
5. (Posterior) Migración progresiva de otros consumidores.

## Open Questions
- ¿Se debe estandarizar un `data-testid` interno del componente (ej. `app-loading-state`) o dejarlo al consumidor?
- ¿Los iconos por defecto deben ser MUI (para consistencia) o permitir que el consumidor los provea siempre?

