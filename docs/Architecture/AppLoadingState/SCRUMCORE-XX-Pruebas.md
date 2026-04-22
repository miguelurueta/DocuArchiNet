# SCRUMCORE-XX — Pruebas: `AppLoadingState`

## Unit tests (obligatorios)
- No renderiza antes de `delayMs`.
- Renderiza después de `delayMs` si `loading` sigue `true`.
- Se oculta al pasar `loading=false`.
- Limpia timers correctamente (no `setState` tras unmount).

## Integration tests (obligatorios)
- Puede renderizar dentro de contenedores/paneles sin romper layout.
- Si se usa modo wrapper (`children`):
  - `children` se muestran cuando `loading=false`.

## Browser checks (manual)
- Aparece solo tras el delay esperado.
- Desaparece correctamente.
- No genera flicker perceptible al alternar cargas rápidas.

## E2E
- Solo aplica si el ticket incluye integración en pantalla real.

