# SCRUMCORE-142 — Pruebas: DomainGuard / useDomainGuard

## Pruebas unitarias (obligatorias)

Archivo:

- `src/app/Components/UI/DomainGuard/DomainGuard.test.tsx`

Casos cubiertos:

- Renderiza children cuando `isBlocked=false`.
- Renderiza fallback cuando `isBlocked=true`.
- No monta children cuando está bloqueado (verificación vía `useEffect` + spy).

## Pruebas de integración UI

- El test incluye “no mount” como garantía de que no hay efectos secundarios.
- Integración en pantalla real queda para ticket de adopción (ej. Gestión Respuesta).

## Pruebas E2E

- No aplica en este ticket (no hay ruta/pantalla nueva). Se recomienda E2E en el
  ticket de adopción (piloto) donde exista navegación/CTA.

## Comandos ejecutados (evidencia)

- `npm.cmd test -- src/app/Components/UI/DomainGuard/DomainGuard.test.tsx`

Resultado:

- 1 archivo de pruebas OK, 3 tests OK.

