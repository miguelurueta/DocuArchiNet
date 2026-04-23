# SCRUMCORE-XX — Implementar `AppLoadingState` y migrar loading del panel `GestionRespuesta`

## Rol esperado
Arquitecto de software senior frontend (React 19, TypeScript estricto, componentes shared, UX states, accesibilidad, testing, Clean Architecture)

## Objetivo
Implementar el componente reusable `AppLoadingState` (loader inline con delay, accesible) y migrar el manejo de loading del panel de Gestión Correspondencia (`GestionCorrespondenciaRoute`), eliminando lógica ad-hoc y estandarizando el patrón visual y funcional.

## Dependencias
- Infraestructura de componentes shared UI
- `GestionCorrespondenciaRoute` (pantalla consumidora)
- Patrón de testing del repositorio

## Contexto existente
Actualmente existen loaders implementados de forma local con lógica de delay en vistas, generando duplicación, inconsistencias visuales y flicker en cargas rápidas.

## Estado actual (a migrar)
`GestionCorrespondenciaRoute` contiene lógica local basada en:
- `showDelayedLoader`
- `setTimeout / clearTimeout`

## Ubicación esperada (código)
- `src/app/Components/UI/AppLoadingState/AppLoadingState.tsx`
- `src/app/Components/UI/AppLoadingState/index.ts`
- `src/app/Components/UI/AppLoadingState/AppLoadingState.module.css`
- `src/app/Components/UI/AppLoadingState/tests/*`
- Migración en:
  - `src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.tsx`

## Restricciones obligatorias
- NO usar `any`
- NO acoplar el componente a módulos específicos
- NO usarlo como bloqueador global full-screen
- NO introducir estilos globales
- NO duplicar lógica de delay en consumidores

## Regla arquitectónica obligatoria
La lógica de delay y control de visibilidad del loading debe residir exclusivamente en el componente shared `AppLoadingState`.

Esto implica:
- Consumidores solo pasan `loading` y `delayMs`.
- No existe lógica de temporización en vistas.
- Se centraliza el comportamiento temporal.
- Se elimina duplicación.

## Contrato esperado (`AppLoadingState` props)
- `loading: boolean`
- `delayMs?: number` (default 500)
- `title?: string`
- `message?: string`
- `icon?: ReactNode`
- `className?: string`
- `children?: ReactNode`

## Reglas de implementación obligatorias
- No renderizar antes de que `loading` supere `delayMs`.
- Renderizar como card pequeña inline (no full-screen).
- Ocultar correctamente cuando `loading=false`.
- Limpiar timers al desmontar y al cambiar estado.
- Accesibilidad:
  - `role="status"`
  - `aria-live="polite"`

## Migración en `GestionCorrespondenciaRoute`
- Eliminar:
  - `showDelayedLoader`
  - `setTimeout / clearTimeout`
- Reemplazar por:

```tsx
<AppLoadingState
  loading={detailState === "loading"}
  delayMs={500}
  title="Cargando estructura de la tarea"
  message="Validando información…"
/>
```

- Mantener:
  - `data-testid="gestion-correspondencia-loading-state"`

## Pruebas unitarias obligatorias
- No renderiza antes de `delayMs`.
- Renderiza después de `delayMs` si `loading` sigue `true`.
- Se oculta cuando `loading=false`.
- Limpia timers correctamente (sin `setState` tras unmount).

## Pruebas de integración UI obligatorias
- Se renderiza correctamente dentro del panel master-detail.
- No rompe layout.
- `children` visibles cuando `loading=false` (si aplica wrapper mode).

## Pruebas E2E
- No aplica en este ticket base (componente + migración puntual).

## Criterios de aceptación
- `AppLoadingState` implementado y reusable.
- `delayMs` evita parpadeo.
- `GestionCorrespondenciaRoute` usa el componente.
- No existe lógica de delay duplicada.
- No hay regresiones visuales/funcionales.

