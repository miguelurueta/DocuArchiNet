# Spec — `SCRUMCORE-142` DomainGuard / ScreenGuard

## Objetivo

Crear un patrón reusable `DomainGuard` para bloquear pantallas o secciones UI
cuando no se cumpla una condición de dominio, con fallback configurable.

## Requerimientos funcionales

1. El guard debe aceptar:
   - `isBlocked: boolean`
   - `fallback: ReactNode`
   - `children: ReactNode`
2. Cuando `isBlocked=true`:
   - renderizar `fallback`
   - **NO renderizar children** (no montaje → sin efectos secundarios)
3. Cuando `isBlocked=false`:
   - renderizar `children` sin cambios de comportamiento
4. Tipado estricto:
   - no usar `any`
5. Accesibilidad:
   - el componente no impone a11y del fallback, pero la documentación debe
     recomendar `role="alert"` y CTA accesible cuando se trate de error bloqueante.

## Requerimientos no funcionales

- Reusable y desacoplado de dominio específico
- Adopción progresiva (opt‑in)
- Tests obligatorios: render children vs render fallback y verificación de no montaje

## Exclusiones (Non-goals)

- No migración masiva de pantallas
- No integración directa con backend (el guard no conoce endpoints)
- No skeleton loaders

## Criterios de aceptación

- `DomainGuard` existe y cumple la semántica de montaje/no montaje.
- Tests pasan y cubren los casos base.
- Documentación técnica creada en `docs/Architecture/DomainGuard/` para el ticket.

