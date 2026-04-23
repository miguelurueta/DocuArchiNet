# SCRUMCORE-XX — Implementación detallada: `AppLoadingState`

## Ubicación de código
- `src/app/Components/UI/AppLoadingState/AppLoadingState.tsx`
- `src/app/Components/UI/AppLoadingState/index.ts`
- `src/app/Components/UI/AppLoadingState/AppLoadingState.module.css`
- `src/app/Components/UI/AppLoadingState/tests/*`

## Reglas de render (delay)
1. Si `loading=false`: no renderiza el card (y, si hay `children`, los muestra).
2. Si `loading=true`:
   - inicia un timer de `delayMs`.
   - antes de que el timer se cumpla: no renderiza nada (evita flicker).
   - al cumplirse el timer y si `loading` sigue `true`: renderiza el card.
3. Si `loading` cambia a `false`:
   - limpia el timer
   - oculta el card inmediatamente.

## Limpieza y seguridad
- Limpiar timers en `useEffect` cleanup.
- Evitar `setState` después de unmount.

## Accesibilidad
- Contenedor o mensaje con:
  - `role="status"`
  - `aria-live="polite"`

## Estilos (criterios)
- Card pequeño, centrado dentro del contenedor padre.
- Responsive (ancho máximo razonable, alto mínimo).
- Sin ocupar el viewport completo.
- Sin estilos globales: usar CSS Module o estilos encapsulados.

