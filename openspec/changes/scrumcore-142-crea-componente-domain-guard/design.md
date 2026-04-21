## Context

`SCRUMCORE-142` crea un patrón reusable de **Domain Guard / Screen Guard** para
bloquear pantallas o secciones UI cuando no se cumple una condición de dominio.

En el repositorio ya existen guards, pero están acoplados a otros contextos:

- Guard de autorización (routing/auth): `src/app/auth/ProteccionRuta/RutaProtegida.tsx`,
  `src/app/auth/ProteccionRuta/Autorizado.tsx`.
- Guard de acciones (tabla dinámica): `src/app/Components/UI/AppTable/utils/dynamicUiActionGuard.ts`.

La necesidad actual es un guard genérico de dominio (opt-in), aplicable a
cualquier módulo, que asegure:

- **no renderizar children** cuando esté bloqueado (para evitar montaje/efectos);
- fallback configurable;
- tipado estricto (sin `any`);
- adopción progresiva (sin refactor global).

Este patrón será el fundamento para bloquear pantallas como Gestión Respuesta
cuando falte estructura para `idTareaWf` (ticket separado).

Referencias funcionales:

- `docs/Architecture/DomainGuard/TICKET-SCRUMCORE-DOMAIN-GUARD.md`
- `docs/Architecture/GestionCorrrespondecia/TICKET-SCRUMCORE-GESTION-RESPUESTA-BLOQUEO-ESTRUCTURA.md`

## Goals / Non-Goals

**Goals**

- Crear componente reusable `DomainGuard` con contrato mínimo:
  - `isBlocked: boolean`
  - `fallback: ReactNode`
  - `children: ReactNode`
- Garantizar que `children` **no se montan** cuando `isBlocked=true`.
- (Opcional) Crear hook helper `useDomainGuard` para normalizar razones de bloqueo.
- Agregar tests unitarios e integración UI (RTL) que validen el montaje/no montaje.
- Añadir documentación técnica mínima (API + a11y + ejemplos + checklist de adopción).

**Non-Goals**

- No migrar pantallas existentes de forma masiva.
- No implementar el bloqueo de `GestionRespuesta` en este cambio.
- No introducir un sistema global de skeleton loaders.
- No acoplar `DomainGuard` a routing, claims, react-query o un dominio específico.

## Decisions

1. **`DomainGuard` como “mount gate” (no solo disabled)**
   - **Decision:** `DomainGuard` debe retornar `fallback` y **no renderizar**
     `children` cuando `isBlocked=true`.
   - **Rationale:** bloquear interacción con `disabled` no evita efectos
     secundarios (queries, subscriptions, `useEffect`).
   - **Trade-off:** el layout cambia por completo; se acepta porque el prompt
     exige “no UI parcial” y CTA(s) en fallback.

2. **Componente “dumb” (sin dominio)**
   - **Decision:** `DomainGuard` no evalúa reglas de negocio; recibe `isBlocked`
     y `fallback` desde afuera.
   - **Rationale:** reutilizable, testeable y consistente con Clean Architecture.

3. **Hook helper opcional**
   - **Decision:** `useDomainGuard` puede existir para normalizar (`isEmpty`,
     `error`, `condition` → `isBlocked` + `reason`), pero no es obligatorio.
   - **Rationale:** el valor crítico es el componente; el hook es ergonomía.

4. **Naming y ubicación respetan convenciones del repo**
   - **Decision:** ubicar el componente bajo `src/app/Components/...` (convención
     actual del repo) y evitar introducir una carpeta paralela con casing distinto.
   - **Rationale:** reduce fricción, evita paths duplicados y facilita adopción.

## Risks / Trade-offs

- [Riesgo] Crear un nuevo patrón sin adopción real.
  - Mitigación: integrar como piloto en una pantalla posterior (ticket separado).
- [Riesgo] “Guard proliferation” (muchos guards).
  - Mitigación: documentar cuándo usar `RutaProtegida` (auth) vs `DomainGuard` (dominio).
- [Riesgo] A11y inconsistente en fallbacks.
  - Mitigación: checklist de a11y y ejemplos con `role="alert"` y CTA accesible.

## Migration Plan

- Introducir `DomainGuard` como componente opt‑in.
- Agregar tests base que validen contrato (mount/no mount).
- Documentar API y uso recomendado.
- (Siguiente ticket) Adoptar en pantalla piloto (Gestión Respuesta) y validar no regresión.

