# SCRUMCORE-XX — Arquitectura: `AppLoadingState`

## Objetivo
Estandarizar un estado de carga *inline/card* reutilizable con soporte de `delayMs`, para evitar *flicker* en cargas rápidas y eliminar implementaciones ad-hoc en vistas consumidoras.

## Contexto
En el front existen loaders implementados por vista, con lógica de delay local. Esto produce:
- Inconsistencia visual entre módulos.
- Duplicación de lógica.
- Parpadeo cuando una carga es muy rápida.

## Decisión
Crear un componente shared `AppLoadingState` dentro de `src/app/Components/UI` que encapsule:
- Lógica de delay/visibilidad.
- Accesibilidad (`role="status"`, `aria-live="polite"`).
- Presentación *inline* (no full-screen).

## Alcance
- El componente **no** debe comportarse como overlay global.
- El componente puede usarse:
  - *standalone*: renderizar solo el card de loading.
  - *wrapper*: renderizar `children` cuando `loading=false` (opcional, según decisión de implementación).

## Restricciones
- TypeScript estricto, sin `any`.
- Sin acoplar el componente a módulos específicos.
- Sin estilos globales.
- La lógica de delay vive dentro del componente (no en consumidores).

## Contrato propuesto (props)
- `loading: boolean`
- `delayMs?: number` (default recomendado: `500`)
- `title?: string`
- `message?: string`
- `icon?: ReactNode`
- `className?: string`
- `children?: ReactNode`

## No objetivos
- No reemplaza bloqueadores globales (para eso existe `OperationBlockerContext`).
- No hace polling ni controla queries; solo presenta estado.

