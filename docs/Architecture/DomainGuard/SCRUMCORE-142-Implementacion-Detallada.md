# SCRUMCORE-142 — Implementación detallada: DomainGuard / useDomainGuard

## Artefactos creados

### Componente

- `src/app/Components/UI/DomainGuard/DomainGuard.tsx`
- `src/app/Components/UI/DomainGuard/index.ts`

Contrato:

- `DomainGuard({ isBlocked, fallback, children })`
- `isBlocked=true` → retorna `fallback` y NO monta children

### Hook helper (opcional)

- `src/shared/hooks/useDomainGuard.ts`

Nota de repo:

- El prompt sugería `src/app/hooks/useDomainGuard.ts`, pero en esta versión del
  repo no existe `src/app/hooks/`, por lo que se ubicó en `src/shared/hooks/`.

## Funciones modificadas

- No aplica (solo se agregaron archivos nuevos).

## Flujo de ejecución

1. Un consumidor calcula `isBlocked` (por ejemplo: `isEmpty || Boolean(error)`).
2. Renderiza:
   - `<DomainGuard isBlocked fallback={...}>{children}</DomainGuard>`
3. Si está bloqueado:
   - `children` no se renderizan → no hay montaje → no hay `useEffect`/queries.

## Decisiones técnicas

- Se usó fragment (`<>...</>`) para no introducir wrappers innecesarios en DOM.
- `DomainGuard` se mantiene “dumb”: no conoce react-query, router, ni dominio.
- `useDomainGuard` es helper para ergonomía; no reemplaza al mount gate.

