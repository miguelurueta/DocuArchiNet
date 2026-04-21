# SCRUMCORE-142 — Integración BackEnd

## Aplica integración con backend

No aplica.

`DomainGuard` es un patrón de UI que no conoce endpoints ni contratos de datos.
Se integra indirectamente cuando un consumidor calcula `isBlocked` a partir de
hooks que sí consumen backend (por ejemplo, react-query).

## Consideraciones

- La responsabilidad de interpretar `200 empty` vs `400 error` es del dominio
  consumidor (hook/pantalla), no del guard.
- El guard solo aplica el gating (montaje/no montaje) y el fallback UI.

